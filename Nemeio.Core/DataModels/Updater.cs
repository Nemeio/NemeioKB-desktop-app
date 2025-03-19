using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Nemeio.Core.DataModels
{
    public enum UpdateType
    {
        App,
        Stm32,
        Nrf
    }

    [Serializable]
    public class Updater : IComparable<Updater>
    {
        public const string DescriptorExtension = ".dat";

        private const int BeforeObject = -1;
        private const int EquivalentObject = 0;
        private const int AfterObject = 1;

        public Uri Url { get; }
        public VersionProxy VersionProxy { get; }
        public UpdateType Type { get; }
        public string Checksum { get; }
        public string InstallerPath { get; private set; }

        /// <summary>
        /// Base updater record constructor
        /// </summary>
        /// <param name="url">Updater download URI (file name included)</param>
        /// <param name="versionProxy">Version of pending URI content</param>
        /// <param name="type">Updater type</param>
        /// <param name="checksum">File checksum</param>
        /// <exception cref="ArgumentException">Invalid arguments provided</exception>
        /// <exception cref="UriFormatException">Provided url name is not a valid Uri</exception>
        public Updater(string url, VersionProxy versionProxy, UpdateType type, string checksum)
        {
            if (string.IsNullOrWhiteSpace(url) || versionProxy == null || string.IsNullOrWhiteSpace(checksum))
            {
                throw new ArgumentException($"Invalid updater record <Url:{url}> <version:{versionProxy}> <type:{type}> <checksum:{checksum}");
            }

            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                throw new UriFormatException($"Invalid URI name <{url}>");
            }

            Url = new Uri(url);
            VersionProxy = versionProxy;
            Type = type;
            Checksum = checksum;
        }

        /// <summary>
        /// Comparer to possibly sort updates having Nrf ahead, then Stm32, then App.
        /// Beware when keyboard is not connnected only App might be present and then deployed!
        /// </summary>
        /// <param name="other">Other updater being compared</param>
        /// <returns></returns>
        public int CompareTo(Updater other)
        {
            // Apps should come last in the list
            if (Type == UpdateType.App)
            {
                if (other.Type == UpdateType.App)
                {
                    return EquivalentObject;
                }
                return AfterObject;
            }
            else if (Type == UpdateType.Nrf)
            {
                if (other.Type == UpdateType.Nrf)
                {
                    return EquivalentObject;
                }
                return BeforeObject;
            }

            if (other.Type == UpdateType.App)
            {
                return BeforeObject;
            }
            else if (other.Type == UpdateType.Nrf)
            {
                return AfterObject;
            }

            return EquivalentObject;
        }

        public bool IsKeyboardUpdate()
        {
            return Type == UpdateType.Stm32 || Type == UpdateType.Nrf;
        }

        public string GetDescriptor()
        {
            return Type.ToString() + DescriptorExtension;
        }

        public string GetDescriptorPath()
        {
            var folder = Path.GetDirectoryName(InstallerPath);
            if (string.IsNullOrWhiteSpace(folder))
            {
                throw new ArgumentException($"Updater.GetDescriptorPath(): invalid folder path {InstallerPath}");
            }
            return Path.Combine(folder, GetDescriptor());
        }

        public void ComputeInstallerPath(string folderPath)
        {
            var packageName = Path.GetFileName(Url.ToString());
            if (string.IsNullOrWhiteSpace(packageName))
            {
                throw new InvalidOperationException($"Target package name from URL is empty. This shoud lnot be allowed {Url}");
            }
            InstallerPath = Path.Combine(folderPath.Trim(), packageName).ToLower();
        }

        public void DumpUpdaterInDescriptor()
        {
            var folder = Path.GetDirectoryName(InstallerPath);
            if (string.IsNullOrWhiteSpace(folder))
            {
                throw new ArgumentException("Updater.DumpUpdaterInDescriptor(): invalid folder path");
            }

            var formatter = new BinaryFormatter();
            var stream = new FileStream(GetDescriptorPath(), FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, this);
            stream.Close();
        }

        public static Updater RecoverUpaterInstance(string filePath)
        {
            var formatter = new BinaryFormatter();
            Updater newUpdater = null;
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                newUpdater = (Updater)formatter.Deserialize(stream);
            }
            return newUpdater;
        }

        public bool IsSameVersion(Updater other)
        {
            return !VersionProxy.IsHigherThan(other.VersionProxy) && !other.VersionProxy.IsHigherThan(VersionProxy);
        }

        public void CleanDiskInstance()
        {
            if (string.IsNullOrWhiteSpace(InstallerPath))
            {
                return;
            }

            var path = GetDescriptorPath();
            if (File.Exists(path))
            {
                var instance = RecoverUpaterInstance(GetDescriptorPath());
                if (IsSameVersion(instance))
                {
                    File.Delete(GetDescriptorPath());
                }
            }

            if (File.Exists(InstallerPath))
            {
                File.Delete(InstallerPath);
            }
        }
    }
}
