using AppKit;
using Foundation;
using Nemeio.Mac.UserControls;

namespace Nemeio.Mac.Views
{

    // Should subclass AppKit.NSView
    [Foundation.Register("KeyboardCell")]
    public partial class KeyboardCell
    {
        [Outlet]
        NSImageView IconImageView { get; set; }

        [Outlet]
        NemeioLabel TitleLabel { get; set; }

        [Outlet]
        NSView SelectionView { get; set; }

        [Outlet]
        NSImageView AssociationImageView { get; set; }

        [Outlet]
        NSView DisabledView { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (IconImageView != null)
            {
                IconImageView.Dispose();
                IconImageView = null;
            }

            if (TitleLabel != null)
            {
                TitleLabel.Dispose();
                TitleLabel = null;
            }

            if (SelectionView != null)
            {
                SelectionView.Dispose();
                SelectionView = null;
            }

            if (AssociationImageView != null)
            {
                AssociationImageView.Dispose();
                AssociationImageView = null;
            }

            if (DisabledView != null)
            {
                DisabledView.Dispose();
                DisabledView = null;
            }
        }
    }
}
