using AppKit;
using Foundation;

namespace Nemeio.Mac.Windows.Alert.UpdateModal
{
    // Should subclass AppKit.NSViewController
    [Foundation.Register("UpdateModalController")]
    public partial class UpdateModalController
    {
        [Outlet]
        NSTextField TitleLabel { get; set; }

        [Outlet]
        NSView TitleDivider { get; set; }

        [Outlet]
        NSView TitleIconContainer { get; set; }

        [Outlet]
        NSImageView TitleIconImageView { get; set; }

        [Outlet]
        NSView DownloadingPageContainer { get; set; }

        [Outlet]
        NSTextField DownloadingTitle { get; set; }

        [Outlet]
        NSProgressIndicator DownloadingProgressBar { get; set; }

        [Outlet]
        NSButton DownloadingCloseButton { get; set; }

        [Outlet]
        NSView ActionPageContainer { get; set; }

        [Outlet]
        NSTextField PageTitleLabel { get; set; }

        [Outlet]
        NSTextField PageSubtitleLabel { get; set; }

        [Outlet]
        NSButton MainActionButton { get; set; }

        [Outlet]
        NSButton SecondaryActionButton { get; set; }

        [Outlet]
        NSTextField InstallingTitleLabel { get; set; }

        [Outlet]
        NSTextField InstallingSubtitleLabel { get; set; }

        [Outlet]
        NSView InstallingPageContainer { get; set; }

        [Outlet]
        NSView CrossImageContainer { get; set; }

        [Outlet]
        NSImageView CrossIconImageView { get; set; }

        [Outlet]
        NSButton InstallingCloseButton { get; set; }

        [Outlet]
        NSProgressIndicator InstallingSpinner { get; set; }

        [Outlet]
        NSTextField InstallingPercentLabel { get; set; }

        [Outlet]
        NSProgressIndicator InstallingProgressBar { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (InstallingPercentLabel != null)
            {
                InstallingPercentLabel.Dispose();
                InstallingPercentLabel = null;
            }

            if (InstallingProgressBar != null)
            {
                InstallingProgressBar.Dispose();
                InstallingProgressBar = null;
            }

            if (InstallingSpinner != null)
            {
                InstallingSpinner.Dispose();
                InstallingSpinner = null;
            }

            if (InstallingCloseButton != null)
            {
                InstallingCloseButton.Dispose();
                InstallingCloseButton = null;
            }

            if (InstallingTitleLabel != null)
            {
                InstallingTitleLabel.Dispose();
                InstallingTitleLabel = null;
            }

            if (InstallingSubtitleLabel != null)
            {
                InstallingSubtitleLabel.Dispose();
                InstallingSubtitleLabel = null;
            }

            if (InstallingPageContainer != null)
            {
                InstallingPageContainer.Dispose();
                InstallingPageContainer = null;
            }

            if (CrossImageContainer != null)
            {
                CrossImageContainer.Dispose();
                CrossImageContainer = null;
            }

            if (CrossIconImageView != null)
            {
                CrossIconImageView.Dispose();
                CrossIconImageView = null;
            }

            if (ActionPageContainer != null)
            {
                ActionPageContainer.Dispose();
                ActionPageContainer = null;
            }

            if (PageTitleLabel != null)
            {
                PageTitleLabel.Dispose();
                PageTitleLabel = null;
            }

            if (PageSubtitleLabel != null)
            {
                PageSubtitleLabel.Dispose();
                PageSubtitleLabel = null;
            }

            if (MainActionButton != null)
            {
                MainActionButton.Dispose();
                MainActionButton = null;
            }

            if (SecondaryActionButton != null)
            {
                SecondaryActionButton.Dispose();
                SecondaryActionButton = null;
            }

            if (TitleLabel != null)
            {
                TitleLabel.Dispose();
                TitleLabel = null;
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

            if (DownloadingPageContainer != null)
            {
                DownloadingPageContainer.Dispose();
                DownloadingPageContainer = null;
            }

            if (DownloadingTitle != null)
            {
                DownloadingTitle.Dispose();
                DownloadingTitle = null;
            }

            if (DownloadingProgressBar != null)
            {
                DownloadingProgressBar.Dispose();
                DownloadingProgressBar = null;
            }

            if (DownloadingCloseButton != null)
            {
                DownloadingCloseButton.Dispose();
                DownloadingCloseButton = null;
            }
        }
    }
}
