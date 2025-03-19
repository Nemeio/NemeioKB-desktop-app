using System;
using System.IO;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Nemeio.UpdateInquiry.Models;

namespace Nemeio.UpdateInquiry.Parser
{
    public class BinaryParser
    {
        private const string MetadataChecksumName       = "checksum";
        private const string MetadataPlatformName       = "platform";
        private const string MetadataVersionName        = "version";
        private const string LegacyMetadataOsName       = "os";
        private const string MetadataComponentName      = "component";
        private const string MetadataCommitName         = "commit";

        private const string ManufacturingFolderName    = "manufacturing";

        public Binary Parse(BlobItem blob, BlobContainerClient blobContainerClient)
        {
            if (blob.Metadata != null)
            {
                string component = null;
                if (blob.Metadata.ContainsKey(MetadataComponentName))
                {
                    component = blob.Metadata[MetadataComponentName];
                }
                else
                {
                    //  Search for legacy key
                    if (blob.Metadata.ContainsKey(LegacyMetadataOsName))
                    {
                        component = blob.Metadata[LegacyMetadataOsName];
                    }
                }

                string checksum = null;
                if (blob.Metadata.ContainsKey(MetadataChecksumName))
                {
                    checksum = blob.Metadata[MetadataChecksumName];
                }

                string platform = null;
                if (blob.Metadata.ContainsKey(MetadataPlatformName))
                {
                    platform = blob.Metadata[MetadataPlatformName];
                }

                string version = null;
                if (blob.Metadata.ContainsKey(MetadataVersionName))
                {
                    version = blob.Metadata[MetadataVersionName];
                }

                string commit = null;
                if (blob.Metadata.ContainsKey(MetadataCommitName))
                {
                    commit = blob.Metadata[MetadataCommitName];
                }

                //  No date associated to this blob
                //  By pass it
                var createdAt = blob.Properties.CreatedOn;
                if (createdAt == null)
                {
                    return null;
                }

                //  Version is required
                if (string.IsNullOrEmpty(version))
                {
                    return null;
                }

                Version blobVersion;
                var versionParseSuccess = Version.TryParse(version, out blobVersion);

                if (!string.IsNullOrEmpty(component) &&
                    !string.IsNullOrEmpty(checksum) &&
                    versionParseSuccess)
                {
                    var newBinary = new Binary()
                    {
                        Name = GetBlobFilename(blob.Name),
                        BlobName = blob.Name,
                        Version = blobVersion,
                        Platform = GetPlatformByValue(platform),
                        Checksum = checksum,
                        Component = component,
                        Commit = commit,
                        CreatedAt = createdAt.Value,
                        Url = blob.Properties.CopySource?.AbsoluteUri ?? new Uri(Path.Combine(blobContainerClient.Uri.OriginalString, blob.Name)).AbsoluteUri
                    };

                    return newBinary;
                }
            }

            return null;
        }

        /// <summary>
        /// WARNING ! This methods is a copy / paste from "Binary.Parse(BlobItem blob)"
        /// because object implementation are different and not implement same parent (interface or class)
        /// </summary>
        public Binary Parse(CloudBlockBlob blob)
        {
            if (blob.Metadata != null)
            {
                string component = null;
                if (blob.Metadata.ContainsKey(MetadataComponentName))
                {
                    component = blob.Metadata[MetadataComponentName];
                }
                else
                {
                    //  Search for legacy key
                    if (blob.Metadata.ContainsKey(LegacyMetadataOsName))
                    {
                        component = blob.Metadata[LegacyMetadataOsName];
                    }
                }

                string checksum = null;
                if (blob.Metadata.ContainsKey(MetadataChecksumName))
                {
                    checksum = blob.Metadata[MetadataChecksumName];
                }

                string platform = null;
                if (blob.Metadata.ContainsKey(MetadataPlatformName))
                {
                    platform = blob.Metadata[MetadataPlatformName];
                }

                string version = null;
                if (blob.Metadata.ContainsKey(MetadataVersionName))
                {
                    version = blob.Metadata[MetadataVersionName];
                }

                string commit = null;
                if (blob.Metadata.ContainsKey(MetadataCommitName))
                {
                    commit = blob.Metadata[MetadataCommitName];
                }

                if (!string.IsNullOrEmpty(component) &&
                    !string.IsNullOrEmpty(checksum) &&
                    !string.IsNullOrEmpty(version) &&
                    !string.IsNullOrEmpty(commit))
                {
                    var newBinary = new Binary()
                    {
                        Name = GetBlobFilename(blob.Name),
                        BlobName = blob.Name,
                        Version = new Version(version),
                        Platform = GetPlatformByValue(platform),
                        Checksum = checksum,
                        Component = component,
                        Commit = commit
                    };

                    return newBinary;
                }
            }

            return null;
        }

        private Platform GetPlatformByValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Platform.AnyCPU;
            }

            var lowerName = value.ToLower();
            switch (lowerName)
            {
                case "x86": return Platform.x86;
                case "x64": return Platform.x64;
                default:
                    return Platform.AnyCPU;
            }
        }

        private string GetBlobFilename(string blobName)
        {
            const char virtualFolderSeparator = '/';
            var subParts = blobName.Split(virtualFolderSeparator);

            return subParts[subParts.Length - 1];
        }
    }
}
