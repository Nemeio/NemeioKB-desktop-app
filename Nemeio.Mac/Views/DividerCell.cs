using System;
using AppKit;
using Foundation;
using Nemeio.Mac.Models;

namespace Nemeio.Mac.Views
{
    public partial class DividerCell : BaseTableViewCell
    {
        #region Constructors

        // Called when created from unmanaged code
        public DividerCell(IntPtr handle)
            : base(handle) { }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public DividerCell(NSCoder coder)
            : base(coder) { }

        #endregion

        public override void Setup(TableViewRow row)
        {
            DividerView.WantsLayer = true;
            DividerView.Layer.BackgroundColor = NSColor.White.CGColor;

            if (row.AssociatedObject is string dividerTitle && TitleLabel != null)
            {
                TitleLabel.StringValue = dividerTitle;
                TitleLabel.Editable = false;
                TitleLabel.TextColor = NSColor.Gray;
                TitleLabel.Font = MacFontHelper.GetOpenSans(11);
            }
        }
    }
}

