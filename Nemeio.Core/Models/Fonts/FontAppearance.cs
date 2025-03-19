using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Nemeio.Core.Models.Fonts
{
    public class FontAppearance
    {
        [XmlAttribute("size")]
        public int KeySize { get; set; }

        [XmlElement("fontSize")]
        public FontSizeAppearance[] SizeAppearances { get; set; }

        /// <summary>
        /// Only for XML parsing
        /// </summary>
        private FontAppearance() { }

        public FontAppearance(int keySize, IEnumerable<FontSizeAppearance> sizeAppearances)
        {
            KeySize = keySize;
            SizeAppearances = sizeAppearances.ToArray();
        }
    }
}
