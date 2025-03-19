using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Nemeio.Tools.LayoutConverter.Exceptions;
using Nemeio.Tools.LayoutConverter.Models.Requirements;

//  Needed for unit tests
[assembly: InternalsVisibleTo("Nemeio.Tools.LayoutConverter.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Nemeio.Tools.LayoutConverter.Validators
{
    internal class ImageValidator
    {
        private ISet<Requirement> _requirements;

        internal ImageValidator()
        {
            _requirements = new HashSet<Requirement>();
        }

        internal bool AddRequirement(Requirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            return _requirements.Add(requirement);
        }

        internal void Validate(string imageFilePath)
        {
            var errors = new List<RequirementError>();

            if (string.IsNullOrWhiteSpace(imageFilePath))
            {
                throw new ArgumentNullException(nameof(imageFilePath));
            }

            if (!_requirements.Any())
            {
                throw new InvalidOperationException("No requirements found");
            }

            foreach (var requirement in _requirements)
            {
                var error = requirement.Check(imageFilePath);
                if (error != null)
                {
                    errors.AddRange(error);
                }
            }

            if (errors.Any())
            {
                throw new MissingRequirementException(errors);
            }
        }
    }
}
