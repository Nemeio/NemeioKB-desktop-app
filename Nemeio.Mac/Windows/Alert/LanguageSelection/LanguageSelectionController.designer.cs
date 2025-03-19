using AppKit;
using Foundation;

namespace Nemeio.Mac.Windows.Alert.LanguageSelection
{

    // Should subclass AppKit.NSViewController
    [Foundation.Register("LanguageSelectionController")]
    public partial class LanguageSelectionController
    {
        [Outlet]
        NSTextField TitleLabel { get; set; }

        [Outlet]
        NSTextField InformationLabel { get; set; }

        [Outlet]
        NSComboBox LanguagesComboBox { get; set; }

        [Outlet]
        NSButton ValidButton { get; set; }

        [Outlet]
        NSView TitleDivider { get; set; }

        [Outlet]
        NSView TitleIconContainer { get; set; }

        [Outlet]
        NSImageView TitleIconImageView { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (TitleLabel != null)
            {
                TitleLabel.Dispose();
                TitleLabel = null;
            }

            if (InformationLabel != null)
            {
                InformationLabel.Dispose();
                InformationLabel = null;
            }

            if (LanguagesComboBox != null)
            {
                LanguagesComboBox.Dispose();
                LanguagesComboBox = null;
            }

            if (ValidButton != null)
            {
                ValidButton.Dispose();
                ValidButton = null;
            }

            if (TitleDivider != null)
            {
                TitleDivider.Dispose();
                TitleDivider = null;
            }

            if (TitleIconContainer != null)
            {
                TitleIconContainer.Dispose();
                TitleIconContainer = null;
            }

            if (TitleIconImageView != null)
            {
                TitleIconImageView.Dispose();
                TitleIconImageView = null;
            }
        }
    }
}
