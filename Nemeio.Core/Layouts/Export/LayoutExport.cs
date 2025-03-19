using System.Collections.Generic;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Images;

namespace Nemeio.Core.Layouts.Export
{
    public class LayoutExport
    {
        public string Filename { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public string Version { get; set; }
        public string AssociatedLayoutId { get; set; }
        public IReadOnlyCollection<Key> Keys { get; set; }
        public Font Font { get; set; }
        public IEnumerable<string> LinkApplicationPaths { get; set; }
        public bool LinkApplicationEnable { get; set; }
        public bool IsDarkMode { get; set; }
        public LayoutImageType ImageType { get; set; }
        public ScreenType Screen { get; set; }
    }
}