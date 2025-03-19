using System.Xml.Serialization;
using Nemeio.Models.Fonts;

namespace Nemeio.Core.Models.Fonts
{
    public class FontSizeAppearance
    {
        [XmlAttribute("type")]
        public FontSize Size { get; set; }

        [XmlAttribute("size")]
        public int RealSize { get; set; }

        /// <summary>
        /// Only for XML parsing
        /// </summary>
        private FontSizeAppearance() { }

        public FontSizeAppearance(FontSize size, int realSize)
        {
            Size = size;
            RealSize = realSize;
        }
    }
}
