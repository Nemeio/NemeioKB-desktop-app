using AppKit;
using Foundation;

namespace Nemeio.Mac.Windows.Alert.FactoryResetModal
{

    // Should subclass AppKit.NSViewController
    [Foundation.Register("FactoryResetModalController")]
    public partial class FactoryResetModalController
    {
        [Outlet]
        NSTextField TitleLabel { get; set; }

        [Outlet]
        NSTextField ExplanationLabel { get; set; }

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

            if (ExplanationLabel != null)
            {
                ExplanationLabel.Dispose();
                ExplanationLabel = null;
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
