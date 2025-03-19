using System;
using System.Security.Cryptography;
using Nemeio.Core.DataModels;

namespace Nemeio.Core.Services.Layouts
{
    public class LayoutId : GuidType<LayoutId>
    {
        public static LayoutId NewLayoutId => new LayoutId(Guid.NewGuid().ToString());

        public LayoutId(string value) : base(value) { }

        public LayoutId(byte[] layoutId) : base(layoutId) { }

        public static LayoutId Compute(LayoutInfo layoutInfo)
        {
            var bytes = layoutInfo.GetBytes();
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(bytes);
                return new LayoutId(hash);
            }
        }
    }
}
