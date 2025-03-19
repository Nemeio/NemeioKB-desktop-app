using System;
using AppKit;
using Foundation;
using Nemeio.Core.Managers;
using Nemeio.Mac.Models;

namespace Nemeio.Mac.Views
{
    public abstract class BaseTableViewCell : NSView
    {
        public NSViewController FromController { get; set; }
        public ILanguageManager LanguageManager { get; set; }

        // Called when created from unmanaged code
        public BaseTableViewCell(IntPtr handle)
            : base(handle) { }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public BaseTableViewCell(NSCoder coder)
            : base(coder) { }

        public abstract void Setup(TableViewRow row);
    }
}
