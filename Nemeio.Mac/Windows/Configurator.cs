using System;
using AppKit;
using Foundation;

namespace Nemeio.Mac.Windows
{
    public partial class Configurator : NSWindow
    {
        public Configurator(IntPtr handle)
            : base(handle) { }

        [Export("initWithCoder:")]
        public Configurator(NSCoder coder)
            : base(coder) { }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            IsZoomed = true;
        }
    }
}
