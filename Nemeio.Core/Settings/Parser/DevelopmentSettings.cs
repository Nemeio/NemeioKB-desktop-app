using System.Xml.Serialization;

namespace Nemeio.Core.DataModels
{
    [XmlRoot("settings")]
    public class DevelopmentSettings
    {
        [XmlElement("apiPort")]
        public string ApiPort { get; set; }

        [XmlElement("swaggerEnable")]
        public bool? SwaggerEnable { get; set; }

        [XmlElement("environment")]
        public string Environment { get; set; }

        [XmlElement("jpegCompressionPercent")]
        public int? JpegCompressionPercent { get; set; }

        [XmlElement("autoStartWebServer")]
        public bool? AutoStartWebServer { get; set; }
    }
}
