using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Color;

namespace Nemeio.Core.Layouts.Images
{
    public class LayoutImageInfo
    {
        public HexColor Color { get; set; }
        public Font Font { get; set; }
        public LayoutImageType ImageType { get; set; }
        public IScreen Screen { get; set; }
        public float XPositionAdjustment { get; set; }
        public float YPositionAdjustement { get; set; }

        public LayoutImageInfo(HexColor color, Font font, IScreen screen, LayoutImageType imageType = LayoutImageType.Classic, float xAdjustment = 0, float yAdjustment = 0)
        {
            Color = color;
            Font = font;
            ImageType = imageType;
            Screen = screen;
            XPositionAdjustment = xAdjustment;
            YPositionAdjustement = yAdjustment;
        }
    }
}
