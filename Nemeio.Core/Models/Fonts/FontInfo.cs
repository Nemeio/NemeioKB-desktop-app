using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Nemeio.Core.Models.Fonts
{
    public class FontInfo : IDisposable
    {
        private const string FontResourcesPath = "Nemeio.Core.Resources.Fonts";

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("priority")]
        public int Priority { get; set; }

        private Stream _fontStream;
        public Stream FontStream 
        { 
            get
            {
                if (_fontStream != null)
                {
                    return _fontStream;
                }

                return GetEmbeddedFont(Name);
            }
            set => _fontStream = value;
        }

        [XmlElement("appearance")]
        public FontAppearance[] Appearances { get; set; }

        /// <summary>
        /// Only for XML parsing
        /// </summary>
        private FontInfo() { }

        public FontInfo(string name, int priority, Stream fontStream)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Priority = priority;
            FontStream = fontStream ?? throw new ArgumentNullException(nameof(fontStream));
        }

        public void Dispose()
        {
            FontStream?.Dispose();
        }

        private Stream GetEmbeddedFont(string name) => GetCoreAssembly().GetManifestResourceStream($"{FontResourcesPath}.{name}");

        private Assembly GetCoreAssembly() => Assembly.GetAssembly(typeof(IFontProvider));
    }
}
