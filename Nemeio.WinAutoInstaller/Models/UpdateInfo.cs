using System.IO;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Nemeio.WinAutoInstaller.Models
{
    [XmlRoot(ElementName = "root")]
    public class UpdateInfo
    {
        [XmlElement("win")]
        public PlatformInfo Windows { get; set; }

        public static UpdateInfo FromJson(string json)
        {
            using (var jsonStream = GenerateStreamFromString(json))
            {
                var selializer = new XmlSerializer(typeof(UpdateInfo));
                var jsonReader = JsonReaderWriterFactory.CreateJsonReader(jsonStream, new System.Xml.XmlDictionaryReaderQuotas());
                var root = XElement.Load(jsonReader);

                return (UpdateInfo)selializer.Deserialize(root.CreateReader(ReaderOptions.OmitDuplicateNamespaces));
            }
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
