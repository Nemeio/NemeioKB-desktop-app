using System.Collections.Generic;
using Nemeio.Tools.LayoutConverter.Models;
using Nemeio.Tools.LayoutConverter.Models.Requirements;

namespace Nemeio.Tools.LayoutConverter.Exceptions
{
    internal class MissingRequirementException : ToolException
    {
        internal IList<RequirementError> Errors { get; private set; }

        internal MissingRequirementException(List<RequirementError> errors)
            : base(ToolErrorCode.MissingRequirement, string.Empty) => Errors = errors;
    }
}
