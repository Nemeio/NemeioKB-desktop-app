using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Enums;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Models.LayoutWarning
{
    public class LayoutAnalyser
    {
        private ILayout _layout;

        public LayoutAnalyser(ILayout layout)
        {
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public IEnumerable<LayoutWarning> Analyse()
        {
            return new List<LayoutWarning>()
                .Concat(AnalyseLinkedApplications())
                .Concat(AnalyseKeysApplicationAction());
        }

        private IEnumerable<LayoutWarning> AnalyseLinkedApplications()
        {
            var warnings = new List<LayoutWarning>();
            var applicationLinks = _layout.LayoutInfo.LinkApplicationPaths;

            if (applicationLinks.Count() == 0)
            {
                return warnings;
            }

            foreach (var applicationLink in applicationLinks)
            {
                try
                {
                    // beware we don't care about result path or name here, we just expect it to throw InvalidOperationException
                    // if path does not conform to executable or does not exist

                    FileHelpers.IsValidPathString(applicationLink);
                }
                catch (InvalidOperationException)
                {
                    warnings.Add(
                        new ApplicationPathWarning(applicationLink)
                    );
                }
            }

            return warnings;
        }

        private IEnumerable<LayoutWarning> AnalyseKeysApplicationAction()
        {
            var warnings = new List<LayoutWarning>();

            if (_layout.Keys == null)
            {
                return warnings;
            }

            if (_layout.Keys.Count == 0)
            {
                return warnings;
            }

            foreach (var key in _layout.Keys)
            {
                foreach (var action in key.Actions)
                {
                    foreach (var subAction in action.Subactions)
                    {
                        if (subAction.Type == KeyActionType.Application)
                        {
                            try
                            {
                                // beware we don't care about result path or name here, we just expect it to throw InvalidOperationException
                                // if path does not conform to executable or does not exist

                                FileHelpers.IsValidPathString(subAction.Data);
                            }
                            catch (InvalidOperationException)
                            {
                                warnings.Add(
                                    new KeyApplicationPathWarning(key, action, subAction)
                                );
                            }
                        }
                    }
                }
            }

            return warnings;
        }
    }
}
