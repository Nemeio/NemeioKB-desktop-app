using Nemeio.Core.Services.Layouts;

namespace Nemeio.Presentation.Menu.Layouts
{
    public sealed class LayoutSubsection
    {
        public ILayout Layout { get; private set; }
        public string Title { get; private set; }
        public bool IsSelected { get; private set; }
        public bool IsEnabled { get; private set; }
        public string ToggleTooltip { get; private set; }
        public bool AugmentedHidEnabled { get; private set; }
        public bool ToggleAssociationEnabled { get; private set; }
        public bool IsStandard { get; private set; }

        public LayoutSubsection(ILayout layout, string title, bool isSelected, bool isEnabled, string toggleTooltip, bool augmentedHidEnabled, bool toggleAssociationEnabled, string toogleTooltip, bool isStandard)
        {
            Layout = layout;
            Title = title;
            IsSelected = isSelected;
            IsEnabled = isEnabled;
            ToggleTooltip = toggleTooltip;
            AugmentedHidEnabled = augmentedHidEnabled;
            ToggleAssociationEnabled = toggleAssociationEnabled;
            ToggleTooltip = toggleTooltip;
            IsStandard = isStandard;
        }
    }
}
