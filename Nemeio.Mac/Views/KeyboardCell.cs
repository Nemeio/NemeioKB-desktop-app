using System;
using System.IO;
using System.Linq;
using AppKit;
using Foundation;
using MvvmCross.Platform;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Layouts.Images.AugmentedLayout;
using Nemeio.Core.Services.Layouts;
using Nemeio.Mac.Models;

namespace Nemeio.Mac.Views
{
    public interface IAssociatedApplicationDelegate
    {
        void KeyboardAssociatedApplicationClicked(ILayout layout);
    }

    public partial class KeyboardCell : BaseTableViewCell
    {
        private const string HidKeyboardImageName       = "Menu_Hid_Keyboard_Icon";
        private const string CustomKeyboardImageName    = "Menu_Custom_Keyboard_Icon";
        private const string AugmentedKeyboardImageName = "Menu_Augmented_Keyboard_Icon";
        private const string MenuLinkedAppImageName     = "Menu_Linked_App_Icon";
        private const string MenuNotLinkedAppImageName  = "Menu_NotLinked_App_Icon";

        #region Constructors

        // Called when created from unmanaged code
        public KeyboardCell(IntPtr handle)
            : base(handle) { }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public KeyboardCell(NSCoder coder)
            : base(coder) { }

        #endregion

        public override void Setup(TableViewRow row)
        {
            TitleLabel.SupportClick = false;
            TitleLabel.TextColor = NSColor.White;

            var currentRow = row as KeyboadTableViewRow;
            if (currentRow == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            var layout = currentRow.AssociatedObject as ILayout;

            if (SelectionView != null)
            {
                SelectionView.WantsLayer = true;
                SelectionView.Layer.BackgroundColor = currentRow.Selected ? MacColorHelper.FromHex(NemeioColor.Purple).CGColor : NSColor.Clear.CGColor;
            }

            if (AssociationImageView != null)
            {
                if (layout.LayoutInfo.LinkApplicationPaths.Count() > 0)
                {
                    if (layout.LayoutInfo.LinkApplicationEnable)
                    {
                        AssociationImageView.Hidden = false;
                        AssociationImageView.Image = NSImage.ImageNamed(MenuLinkedAppImageName);
                    }
                    else
                    {
                        AssociationImageView.Hidden = false;
                        AssociationImageView.Image = NSImage.ImageNamed(MenuNotLinkedAppImageName);
                    }

                    var apps = layout.LayoutInfo.LinkApplicationPaths.Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
                    var associatedAppNames = String.Join(", ", apps);
                    var localizedAssociatedAppsTitle = LanguageManager.GetLocalizedValue(StringId.AssociatedApplications);

                    AssociationImageView.ToolTip = $"{localizedAssociatedAppsTitle} {associatedAppNames}";
                    AssociationImageView.AddGestureRecognizer(new NSClickGestureRecognizer(() =>
                    {
                        if (FromController != null && FromController is IAssociatedApplicationDelegate associatedApplicationController)
                        {
                            associatedApplicationController.KeyboardAssociatedApplicationClicked(layout);
                        }
                    }));
                }
                else
                {
                    AssociationImageView.Hidden = true;
                }
            }
           
            if (TitleLabel != null)
            {
                TitleLabel.StringValue = layout.Title;
                TitleLabel.Font = MacFontHelper.GetOpenSans(12);
            }

            if (IconImageView != null)
            {
                var imageName = string.Empty;

                if (layout.LayoutInfo.Hid)
                {
                    var augmentedLayoutImageProvider = Mvx.Resolve<IAugmentedLayoutImageProvider>();

                    if (currentRow.ApplicationAugmentedHidEnabled && layout.LayoutInfo.AugmentedHidEnable && augmentedLayoutImageProvider.AugmentedLayoutImageExists(layout))
                    {
                        imageName = AugmentedKeyboardImageName;
                    }
                    else
                    {
                        imageName = HidKeyboardImageName;
                    }
                }
                else
                {
                    imageName = CustomKeyboardImageName;
                }

                IconImageView.Image = NSImage.ImageNamed(imageName);
            }
        }
    }
}
