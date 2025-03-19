using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Keyboard.Communication.Tools.Frames
{
    public class LayoutInfoDto
    {
        private const int DarkBackground = 0;
        private const int LightBackground = 1;

        public string Id { get; }
        public string Hash { get; }
        public string Factory { get; }
        public string Hid { get; }
        public string Mac { get; }
        public SpecialSequences SpecialSequences { get; }
        public string AssociatedId { get; }
        public int ImageBpp { get; }
        public string DisableModifiers { get; }
        public int BackgroundColor { get; }

        public LayoutInfoDto(ILayout layout)
        {
            Id = layout.LayoutId;
            Hash = layout.Hash;
            Factory = BoolToString(layout.LayoutInfo.Factory);
            Hid = BoolToString(layout.LayoutInfo.Hid);
            Mac = BoolToString(layout.LayoutInfo.Mac);
            SpecialSequences = layout.LayoutInfo.Hid ? SpecialSequences.Default : layout.SpecialSequences;
            AssociatedId = layout.AssociatedLayoutId;
            ImageBpp = layout.LayoutImageInfo.Screen.Format.ImageBpp;
            DisableModifiers = BoolToString(layout.LayoutImageInfo.ImageType == LayoutImageType.Classic);
            BackgroundColor = layout.LayoutImageInfo.Color.IsBlack() ? DarkBackground : LightBackground;
        }

        private string BoolToString(bool val) => val ? "1" : "0";
    }
}
