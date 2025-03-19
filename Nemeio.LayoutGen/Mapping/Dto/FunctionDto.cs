using System.Xml.Serialization;

namespace Nemeio.LayoutGen.Mapping.Dto
{
    public class FunctionDto
    {
        [XmlElement("displayValue", IsNullable = false)]
        public string DisplayValue { get; set; }

        [XmlElement("dataValue", IsNullable = false)]
        public string DataValue { get; set; }
    }
}
