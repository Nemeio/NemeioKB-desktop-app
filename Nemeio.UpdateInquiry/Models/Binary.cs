using System;

namespace Nemeio.UpdateInquiry.Models
{
    public enum Platform
    {
        AnyCPU,
        x86,
        x64
    }

    public class Binary
    {
        public string Name { get; set; }
        public string BlobName { get; set; }
        public Version Version { get; set; }
        public Platform Platform { get; set; }
        public string Checksum { get; set; }
        public string Component { get; set; }
        public string Commit { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Url { get; set; }
    }
}
