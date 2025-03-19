using System.Security.Cryptography;
using Nemeio.Core.DataModels;

namespace Nemeio.Core.Services.Layouts
{
    public class LayoutHash : GuidType<LayoutHash>
    {
        public LayoutHash(string value) 
            : base(value) { }

        public LayoutHash(byte[] layoutId) 
            : base(layoutId) { }

        public static LayoutHash Compute(LayoutId layoutId, byte[] imageBytes)
        {
            var hashes = imageBytes.Append(layoutId.GetBytes());
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(hashes);

                return new LayoutHash(hash);
            }
        }
    }
}
