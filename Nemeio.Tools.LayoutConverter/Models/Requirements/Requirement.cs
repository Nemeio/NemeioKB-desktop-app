using System.Collections.Generic;

namespace Nemeio.Tools.LayoutConverter.Models.Requirements
{
    internal abstract class Requirement
    {
        internal abstract IEnumerable<RequirementError> Check(string filePath);
    }
}
