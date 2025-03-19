using System.Collections.Generic;
using System.Linq;

namespace Nemeio.Tools.LayoutConverter.Models.Requirements
{
    internal class ImageSizeRequirement : Requirement
    {
        internal const int LayoutImageWidth          = 1496;
        internal const int LayoutImageHeight         = 624;

        internal override IEnumerable<RequirementError> Check(string filePath)
        {
            using (var bitmap = new LayoutImageLoader().LoadImage(filePath))
            {
                var errors = new List<RequirementError>();

                if (bitmap.Width != LayoutImageWidth)
                {
                    errors.Add(new RequirementError(filePath, $"Image width is invalid <{bitmap.Width}>. It must be equal to <{LayoutImageWidth}>"));
                }

                if (bitmap.Height != LayoutImageHeight)
                {
                    errors.Add(new RequirementError(filePath, $"Image height is invalid <{bitmap.Height}>. It must be equal to <{LayoutImageHeight}>"));
                }

                return errors.Any() ? errors : null;
            }
        }
    }
}
