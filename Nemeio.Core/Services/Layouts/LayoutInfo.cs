using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nemeio.Core.Extensions;

namespace Nemeio.Core.Services.Layouts
{
    public class LayoutInfo
    {
        private IEnumerable<string> _linkApplicationPaths;
        public OsLayoutId OsLayoutId { get; set; }
        public bool Factory { get; set; }
        public bool Hid { get; set; }
        public bool Mac { get; set; }
        public IEnumerable<string> LinkApplicationPaths
        {
            get
            {
                return _linkApplicationPaths;
            }
            set
            {
                _linkApplicationPaths = CoreHelpers.SanitizePathList(value);
            }
        }
        public bool LinkApplicationEnable { get; set; }
        public bool IsTemplate { get; set; }
        public bool AugmentedHidEnable { get; set; }

        public LayoutInfo(OsLayoutId osLayoutId, bool isFactory, bool isHid, IEnumerable<string> linkPath = null, bool linkEnable = false, bool isTemplate = false, bool augmentedHidEnable = false)
        {
            if (linkPath == null)
            {
                linkPath = new List<string>();
            }

            OsLayoutId = osLayoutId;
            Factory = isFactory;
            Hid = isHid;
            Mac = this.IsOSXPlatform();
            LinkApplicationPaths = linkPath.Select(x => x.ToLower());
            LinkApplicationEnable = linkEnable;
            IsTemplate = isTemplate;
            AugmentedHidEnable = augmentedHidEnable;
        }

        public byte[] GetBytes()
        {
            var id = Encoding.UTF8.GetBytes(OsLayoutId.ToString().ToCharArray());
            var factory = Encoding.UTF8.GetBytes(BoolToString(Factory));
            var hid = Encoding.UTF8.GetBytes(BoolToString(Hid));
            var mac = Encoding.UTF8.GetBytes(BoolToString(Mac));

            var res = id.Append(factory).Append(hid).Append(mac);

            return res;
        }

        private string BoolToString(bool val) => val ? "1" : "0";
    }
}
