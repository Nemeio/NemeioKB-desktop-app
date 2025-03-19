using System;
using System.Collections.Generic;
using Nemeio.Core.Managers;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Presentation.Menu.Layouts
{
    public sealed class LayoutsSectionBuilder
    {
        private ILanguageManager _languageManager;

        public LayoutsSectionBuilder(ILanguageManager languageManager)
        {
            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));
        }

        public LayoutsSection Build(IList<ILayout> layouts, ILayout selectedLayout, bool keyboardIsPlugged, bool keyboardIsReady, bool synchronizing, bool augmentedHidEnabled)
        {
            var layoutSubsections = new List<LayoutSubsection>();

            if (layouts == null)
            {
                return new LayoutsSection(layoutSubsections, false);
            }

            var layoutSubsectionBuilder = new LayoutSubsectionBuilder(_languageManager);

            foreach (var layout in layouts)
            {
                var isSelected = false;
                if (selectedLayout != null)
                {
                    isSelected = layout.LayoutId.ToString() == selectedLayout.LayoutId.ToString();
                }

                var subsection = layoutSubsectionBuilder.Build(
                    layout,
                    isSelected,
                    keyboardIsReady,
                    synchronizing,
                    augmentedHidEnabled
                );

                layoutSubsections.Add(subsection);
            }

            return new LayoutsSection(layoutSubsections, keyboardIsPlugged);
        }
    }
}
