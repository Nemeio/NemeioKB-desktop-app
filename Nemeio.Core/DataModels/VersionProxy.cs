using System;

namespace Nemeio.Core.DataModels
{
    public enum VersionStatus
    {
        Higher,
        Equal,
        Lower
    }

    [Serializable]
    public class VersionProxy
    {
        private const int IsEqual = 0;
        private const int IsLater = 1;
        private const int IsEarlier = -1;

        private readonly Version _version;

        public VersionProxy(Version version) => _version = version;

        public VersionProxy(string version) => _version = new Version(version);

        public bool IsEqualTo(VersionProxy version) => Compare(version) == VersionStatus.Equal;

        public bool IsHigherThan(VersionProxy version) => Compare(version) == VersionStatus.Higher;

        public VersionStatus Compare(VersionProxy version)
        {
            if (version == null)
            {
                return VersionStatus.Equal;
            }

            switch (CompareTo(version))
            {
                case IsLater:
                    return VersionStatus.Higher;
                case IsEarlier:
                    return VersionStatus.Lower;
                case IsEqual:
                default:
                    return VersionStatus.Equal;
            }
        }

        private int CompareTo(VersionProxy version) => _version.CompareTo(version._version);

        public override string ToString() => _version.ToString();

        public static implicit operator string(VersionProxy value) => value?.ToString();
    }
}
