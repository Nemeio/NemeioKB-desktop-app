using AppKit;
using Foundation;

namespace Nemeio.Mac.Views
{
    // Should subclass AppKit.NSView
    [Foundation.Register("DividerCell")]
    public partial class DividerCell
    {
        [Outlet]
        NSView DividerView { get; set; }

        [Outlet]
        NSTextField TitleLabel { get; set; }

        [Outlet]
        NSView DisabledView { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (DividerView != null)
            {
                DividerView.Dispose();
                DividerView = null;
            }

            if (TitleLabel != null)
            {
                TitleLabel.Dispose();
                TitleLabel = null;
            }

            if (DisabledView != null)
            {
                DisabledView.Dispose();
                DisabledView = null;
            }
        }
    }
}
