using System;
using System.Collections.Generic;
using System.Linq;

namespace Nemeio.Tools.LayoutConverter.Models
{
    internal class ImageType
    {
        internal string TypeName { get; set; }
        internal IEnumerable<string> SupportedModifiers { get; set; }

        internal ImageType(string typeName, IEnumerable<string> supportedModifiers)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            TypeName = typeName;

            if (supportedModifiers == null)
            {
                throw new ArgumentNullException(nameof(supportedModifiers));
            }

            if (supportedModifiers.Count() < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(supportedModifiers), "SupportedModifier must be equal or greater than 1");
            }

            SupportedModifiers = supportedModifiers;
        }

        internal string ComposeFileName(string layoutId, string modifier)
        {
            if (string.IsNullOrWhiteSpace(layoutId))
            {
                throw new ArgumentNullException(nameof(layoutId));
            }

            if (string.IsNullOrWhiteSpace(modifier))
            {
                throw new ArgumentNullException(nameof(modifier));
            }

            return $"{layoutId}_{modifier}_{TypeName}.png";
        }
    }
}
