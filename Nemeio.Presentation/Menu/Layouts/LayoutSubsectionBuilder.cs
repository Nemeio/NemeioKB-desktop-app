using System;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Managers;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Presentation.Menu.Layouts
{
    public sealed class LayoutSubsectionBuilder
    {
        private ILanguageManager _languageManager;

        public LayoutSubsectionBuilder(ILanguageManager languageManager)
        {
            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));
        }

        public LayoutSubsection Build(ILayout layout, bool isSelected, bool keyboardIsReady, bool synchronizing, bool augmentedHidEnabled)
        {
            if (layout == null)
            {
                return new LayoutSubsection(null, string.Empty, false, false, string.Empty, false, false, string.Empty, false);
            }

            return new LayoutSubsection(
                layout,
                layout.Title,
                isSelected: isSelected,
                isEnabled: !synchronizing && keyboardIsReady,
                layout.Title,
                augmentedHidEnabled: augmentedHidEnabled,
                toggleAssociationEnabled: layout.LayoutInfo.LinkApplicationEnable,
                BuildToogleTooltip(layout),
                layout.LayoutInfo.Hid
            );
        }

        private string BuildToogleTooltip(ILayout layout)
        {
            var prefix = _languageManager.GetLocalizedValue(StringId.AssociatedApplications);
            var applicationPaths = string.Join("\n", layout.LayoutInfo.LinkApplicationPaths);

            var text = $"{prefix}\n{applicationPaths}";

            return text;
        }
    }
}
