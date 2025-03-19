using System;
using System.IO;

namespace Nemeio.Core.Extensions
{
    public static class StringExtensions
    {
        public const string Prefix = "file:///";

        public static Stream GetStream(this string str)
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            streamWriter.Write(str);
            streamWriter.Flush();
            stream.Position = 0;
            return stream;
        }

        public static bool IsFileRef(this string val)
        {
            try
            {
                return val.Length >1 && Path.HasExtension(val);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public static bool IsFileUrl(this string val)
        {
            return File.Exists(val);
        }

        public static string RemoveFilePrefix(this string val)
        {
            if (val.ToLower().StartsWith(Prefix))
            {
                return val.Remove(0, Prefix.Length);
            }

            return val;
        }
    }
}
