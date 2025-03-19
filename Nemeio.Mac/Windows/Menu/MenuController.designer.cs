using AppKit;
using Foundation;
using Nemeio.Mac.UserControls;

namespace Nemeio.Mac.Windows.Menu
{

    // Should subclass AppKit.NSViewController
    [Foundation.Register("MenuController")]
    public partial class MenuController
    {
        [Outlet]
        NSStackView BodyStackView { get; set; }

        //  QUIT SECTION

        [Outlet]
        NSView QuitRow { get; set; }

        [Outlet]
        NSView QuitRowIconContainer { get; set; }

        [Outlet]
        NSView QuitRowDivider { get; set; }

        [Outlet]
        NSTextField QuitRowLabel { get; set; }

        //  KEYBOARD MANAGER SECTION

        [Outlet]
        NSView KeyboardManagerRow { get; set; }

        [Outlet]
        NSView KeyboardManagerRowIconContainer { get; set; }

        [Outlet]
        NSView KeyboardManagerRowDivider { get; set; }

        [Outlet]
        NSTextField KeyboardManagerRowLabel { get; set; }

        [Outlet]
        NSImageView KeyboardManagerRowImageView { get; set; }

        //  VERSION SECTION

        [Outlet]
        NSView VersionRow { get; set; }

        [Outlet]
        NSTextField VersionRowLabel { get; set; }

        [Outlet]
        NSTextField VersionRowStateLabel { get; set; }

        [Outlet]
        NSView VersionRowDivider { get; set; }

        [Outlet]
        NSImageView VersionImageImageView { get; set; }

        //  BATTERY SECTION

        [Outlet]
        NSTextField BatteryRowTitleLabel { get; set; }

        [Outlet]
        NSImageView BatteryRowIconImageView { get; set; }

        [Outlet]
        NSView BatteryRow { get; set; }

        //  KEYBOARD SECTION

        [Outlet]
        NSTableView KeyboardTableView { get; set; }

        [Outlet]
        NSView KeyboardOverlay { get; set; }

        [Outlet]
        NSView LayoutsContainerView { get; set; }

        //  SYNC IN PROGRESS SECTION

        [Outlet]
        NSView SyncInProgressRow { get; set; }

        [Outlet]
        NSTextField SyncInProgressTitleLabel { get; set; }

        [Outlet]
        NSTextField SyncInProgressValueLabel { get; set; }

        [Outlet]
        NSImageView SyncIconImageView { get; set; }

        //  DEBUG SECTION

        [Outlet]
        NSImageView DebugIconImageView { get; set; }

        [Outlet]
        NSTextField DebugTitleLabel { get; set; }

        [Outlet]
        NSView DebugRowView { get; set; }

        //  CONNECTED SECTION

        [Outlet]
        NSImageView ConnectedIconImageView { get; set; }

        [Outlet]
        NSTextField ConnectedTitleLabel { get; set; }

        [Outlet]
        NSView ConnectedRowView { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (ConnectedIconImageView != null)
            {
                ConnectedIconImageView.Dispose();
                ConnectedIconImageView = null;
            }

            if (ConnectedTitleLabel != null)
            {
                ConnectedTitleLabel.Dispose();
                ConnectedTitleLabel = null;
            }

            if (ConnectedRowView != null)
            {
                ConnectedRowView.Dispose();
                ConnectedRowView = null;
            }

            if (DebugRowView != null)
            {
                DebugRowView.Dispose();
                DebugRowView = null;
            }

            if (DebugIconImageView != null)
            {
                DebugIconImageView.Dispose();
                DebugIconImageView = null;
            }

            if (DebugTitleLabel != null)
            {
                DebugTitleLabel.Dispose();
                DebugTitleLabel = null;
            }

            if (BodyStackView != null)
            {
                BodyStackView.Dispose();
                BodyStackView = null;
            }

            if (QuitRowIconContainer != null)
            {
                QuitRowIconContainer.Dispose();
                QuitRowIconContainer = null;
            }

            if (QuitRowLabel != null)
            {
                QuitRowLabel.Dispose();
                QuitRowLabel = null;
            }

            if (QuitRowDivider != null)
            {
                QuitRowDivider.Dispose();
                QuitRowDivider = null;
            }

            if (KeyboardManagerRowIconContainer != null)
            {
                KeyboardManagerRowIconContainer.Dispose();
                KeyboardManagerRowIconContainer = null;
            }

            if (KeyboardManagerRowDivider != null)
            {
                KeyboardManagerRowDivider.Dispose();
                KeyboardManagerRowDivider = null;
            }

            if (KeyboardManagerRowLabel != null)
            {
                KeyboardManagerRowLabel.Dispose();
                KeyboardManagerRowLabel = null;
            }

            if (VersionRowLabel != null)
            {
                VersionRowLabel.Dispose();
                VersionRowLabel = null;
            }

            if (VersionRowDivider != null)
            {
                VersionRowDivider.Dispose();
                VersionRowDivider = null;
            }

            if (KeyboardTableView != null)
            {
                KeyboardTableView.Dispose();
                KeyboardTableView = null;
            }

            if (QuitRow != null)
            {
                QuitRow.Dispose();
                QuitRow = null;
            }

            if (KeyboardManagerRow != null)
            {
                KeyboardManagerRow.Dispose();
                KeyboardManagerRow = null;
            }

            if (VersionRow != null)
            {
                VersionRow.Dispose();
                VersionRow = null;
            }

            if (BatteryRowTitleLabel != null)
            {
                BatteryRowTitleLabel.Dispose();
                BatteryRowTitleLabel = null;
            }

            if (BatteryRow != null)
            {
                BatteryRow.Dispose();
                BatteryRow = null;
            }

            if (KeyboardManagerRowImageView != null)
            {
                KeyboardManagerRowImageView.Dispose();
                KeyboardManagerRowImageView = null;
            }

            if (SyncInProgressValueLabel != null)
            {
                SyncInProgressValueLabel.Dispose();
                SyncInProgressValueLabel = null;
            }

            if (SyncInProgressTitleLabel != null)
            {
                SyncInProgressTitleLabel.Dispose();
                SyncInProgressTitleLabel = null;
            }

            if (SyncInProgressRow != null)
            {
                SyncInProgressRow.Dispose();
                SyncInProgressRow = null;
            }

            if (VersionRowStateLabel != null)
            {
                VersionRowStateLabel.Dispose();
                VersionRowStateLabel = null;
            }

            if (KeyboardOverlay != null)
            {
                KeyboardOverlay.Dispose();
                KeyboardOverlay = null;
            }

            if (SyncIconImageView != null)
            {
                SyncIconImageView.Dispose();
                SyncIconImageView = null;
            }

            if (LayoutsContainerView != null)
            {
                LayoutsContainerView.Dispose();
                LayoutsContainerView = null;
            }
        }
    }
}
