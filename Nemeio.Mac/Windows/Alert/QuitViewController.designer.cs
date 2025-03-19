using AppKit;
using Foundation;

namespace Nemeio.Mac.Windows.Alert
{

    // Should subclass AppKit.NSViewController
    [Foundation.Register("QuitViewController")]
    public partial class QuitViewController
    {
        [Outlet]
        NSView TitleDivider { get; set; }

        [Outlet]
        NSView TitleIconContainer { get; set; }

        [Outlet]
        NSImageView TitleIconImageView { get; set; }

        [Outlet]
        NSTextField TitleLabel { get; set; }

        [Outlet]
        NSTextField MessageLabel { get; set; }

        [Outlet]
        NSTextField InformationLabel { get; set; }

        [Outlet]
        NSButton PositiveButton { get; set; }

        [Outlet]
        NSButton NegativeButton { get; set; }

        void ReleaseDesignerOutlets()
        {
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

            if (TitleLabel != null)
            {
                TitleLabel.Dispose();
                TitleLabel = null;
            }

            if (MessageLabel != null)
            {
                MessageLabel.Dispose();
                MessageLabel = null;
            }

            if (InformationLabel != null)
            {
                InformationLabel.Dispose();
                InformationLabel = null;
            }

            if (PositiveButton != null)
            {
                PositiveButton.Dispose();
                PositiveButton = null;
            }

            if (NegativeButton != null)
            {
                NegativeButton.Dispose();
                NegativeButton = null;
            }
        }
    }
}
