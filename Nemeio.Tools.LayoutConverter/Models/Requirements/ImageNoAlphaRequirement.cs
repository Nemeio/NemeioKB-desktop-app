using System.Collections.Generic;

namespace Nemeio.Tools.LayoutConverter.Models.Requirements
{
    internal class ImageNoAlphaRequirement : Requirement
    {
        internal override IEnumerable<RequirementError> Check(string filePath)
        {
            var foundAlphaPixel = AnyAlphaPixel(filePath);
            if (foundAlphaPixel)
            {
                return new List<RequirementError>()
                {
                    new RequirementError(
                        filePath,
                        $"Contains alpha value(s)"
                    )
                };
            }

            return null;
        }

        private bool AnyAlphaPixel(string filePath)
        {
            using (var bitmap = new LayoutImageLoader().LoadImage(filePath))
            {
                var width = bitmap.Width;
                var height = bitmap.Height;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var pixel = bitmap.GetPixel(x, y);
                        if (pixel.Alpha < 255)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }
    }
}
