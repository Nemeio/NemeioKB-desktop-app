using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Nemeio.Core
{
    public static class FileHelpers
    {
        public const string TmpExtension = ".tmp";
        public const string OsxExtension = ".app";
        public const string WinExtension = ".exe";

        private static IList<string> _validExtensions = new List<string>() { OsxExtension, WinExtension };

        public static FileWatcher WatchMe(string path)
        {
            return new FileWatcher(path);
        }

        /// <summary>
        /// Check if executable extension is known and valid
        /// </summary>
        /// <param name="applicationPathOrName">Application path or name</param>
        /// <returns>Is valid executable</returns>
        /// <exception cref="InvalidOperationException">When argument miss application name or miss valid application extension</exception>
        public static bool IsValidExecutableExtension(string applicationPathOrName)
        {
            if (string.IsNullOrWhiteSpace(applicationPathOrName))
            {
                throw new InvalidOperationException($"Path <{applicationPathOrName}> looks empty");
            }

            var workingPath = applicationPathOrName.ToLower();
            var extension = Path.GetExtension(workingPath);

            return _validExtensions.Contains(extension);
        }

        /// <summary>
        /// Method to returns whether the currently provided string is consuidered as a path name 
        /// (with executable) against a single executable name (without path)
        /// </summary>
        /// <param name="applicationPathOrName">Application path or name</param>
        /// <returns>True if the provided name is considered as a path name</returns>
        /// <exception cref="InvalidOperationException">When argument miss application name or miss valid application extension</exception>
        public static bool IsValidPathString(string applicationPathOrName)
        {
            // sanity
            if (string.IsNullOrWhiteSpace(applicationPathOrName))
            {
                throw new InvalidOperationException($"Path <{applicationPathOrName}> looks empty");
            }

            // split over elements in lowercase
            var workingPath = applicationPathOrName.ToLower();
            var folder = Path.GetDirectoryName(workingPath);
            var name = Path.GetFileNameWithoutExtension(workingPath);
            var extension = Path.GetExtension(workingPath);

            // check validities
            if (!IsValidExecutableExtension(applicationPathOrName))
            {
                throw new InvalidOperationException($"Path <{applicationPathOrName}> has invalid extension <{extension}>");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException($"Path <{applicationPathOrName}> has invalid application name <{name}>");
            }

            if (string.IsNullOrWhiteSpace(folder))
            {
                return false;
            }

            // check path validity in case of path provided
            if ((extension.Equals(WinExtension) && !File.Exists(applicationPathOrName)) ||
                (extension.Equals(OsxExtension) && !Directory.Exists(applicationPathOrName)))
            {
                throw new InvalidOperationException($"Path to <{applicationPathOrName}> does not exist");
            }

            return true;
        }

        public static string CalculateMD5(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return null;
            }

            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = md5.ComputeHash(stream);

                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
