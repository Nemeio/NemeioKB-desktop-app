using System.Collections.Generic;
using System.Xml.Serialization;

namespace Nemeio.LayoutGen.Mapping.Dto
{
    [XmlRoot("keyboard")]
    public class KeyboardMapDto
    {
        [XmlElement("size")]
        public ScreenSizeDto Size { get; set; }

        [XmlArray("buttons")]
        [XmlArrayItem("button")]
        public List<ButtonDto> Buttons { get; set; }

        [XmlAttribute("flipHorizontal")]
        public bool FlipHorizontal { get; set; }
    }
}
