using System.Xml.Serialization;

namespace Nemeio.Core.DataModels.Locale
{
    public class Entry
    {
        [XmlAttribute("key")]
        public StringId Key { get; set; }

        [XmlText]
        public string TranslateValue { get; set; }
    }
}
