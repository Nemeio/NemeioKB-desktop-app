using System.Xml.Serialization;

namespace Nemeio.LayoutGen.Mapping.Dto
{
    public class PlatformDto
    {
        [XmlAttribute("isFirstLine")]
        public bool IsFirstLine { get; set; }

        [XmlAttribute("isModifier")]
        public bool IsModifier { get; set; }

        [XmlElement("keyCode")]
        public string KeyCode { get; set; }

        [XmlElement("displayValue", IsNullable = true)]
        public string DisplayValue { get; set; }

        [XmlElement("dataValue", IsNullable = true)]
        public string DataValue { get; set; }
        
        [XmlElement("function", IsNullable = true)]
        public FunctionDto Function { get; set; }
    }
}
