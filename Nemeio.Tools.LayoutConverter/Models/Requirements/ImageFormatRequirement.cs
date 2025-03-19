using System;
using System.Collections.Generic;
using System.IO;

namespace Nemeio.Tools.LayoutConverter.Models.Requirements
{
    internal class ImageFormatRequirement : Requirement
    {
        private const string PngExtension = ".png";

        internal override IEnumerable<RequirementError> Check(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var currentFileExtension = Path.GetExtension(filePath);

            if (!currentFileExtension.Equals(PngExtension, StringComparison.OrdinalIgnoreCase))
            {
                return new List<RequirementError>()
                {
                    new RequirementError(
                        filePath,
                        $"Your file's extension is <{currentFileExtension}>, it must be '{PngExtension}'"
                    )
                };
            }

            return null;
        }
    }
}
