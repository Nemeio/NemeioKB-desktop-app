using System;
using System.IO;
using System.Xml.Serialization;
using Nemeio.Core.Keyboard.Map;
using Nemeio.LayoutGen.Mapping.Dto;

namespace Nemeio.LayoutGen.Mapping
{
    public abstract class KeyboardMapFactory : IKeyboardMapFactory
    {
        private const string HolitechMapFileName = "nemeio.holitech.xml";
        private const string EinkMapFileName = "nemeio.eink.xml";

        public KeyboardMap CreateHolitechMap() => CreateMap(HolitechMapFileName);

        public KeyboardMap CreateEinkMap() => CreateMap(EinkMapFileName);

        private KeyboardMap CreateMap(string withName)
        {
            var resourceName = CreateResourceFullName(withName);

            using (var fileStream = Resources.Resources.GetResourceStream(resourceName))
            using (var stream = new StreamReader(fileStream))
            {
                var xmlSerializer = new XmlSerializer(typeof(KeyboardMapDto));
                var dto = (KeyboardMapDto)xmlSerializer.Deserialize(stream);

                return ConvertDto(dto);
            }

            throw new InvalidOperationException("No valid map found");
        }

        public abstract KeyboardMap ConvertDto(KeyboardMapDto mapDto);

        private string CreateResourceFullName(string fileName) => $"Map.{fileName}";
    }
}
