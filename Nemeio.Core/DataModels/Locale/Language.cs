using System.Xml.Serialization;

namespace Nemeio.Core.DataModels.Locale
{
    [XmlRoot("properties")]
    public class Language
    {
        [XmlElement("entry")]
        public Entry[] Entries { get; set; }
    }
}
