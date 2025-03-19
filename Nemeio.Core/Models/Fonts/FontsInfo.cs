using System.Xml.Serialization;

namespace Nemeio.Core.Models.Fonts
{
    [XmlRoot("fonts")]
    public class FontsInfo
    {
        [XmlElement("font")]
        public FontInfo[] Fonts { get; set; }

        /// <summary>
        /// Only for XML parsing
        /// </summary>
        private FontsInfo() { }
    }
}
