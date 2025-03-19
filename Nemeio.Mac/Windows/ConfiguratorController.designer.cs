// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using AppKit;
using Foundation;
using System.CodeDom.Compiler;

namespace Nemeio.Mac.Windows
{
	[Register ("ConfiguratorController")]
	partial class ConfiguratorController
	{
		[Outlet]
		WebKit.WKWebView browser { get; set; }

        [Outlet]
        NSProgressIndicator progressIndicator { get; set; }

        [Outlet]
        NSTextField progressTextField { get; set; }

        void ReleaseDesignerOutlets ()
		{
			if (browser != null)
            {
				browser.Dispose ();
				browser = null;
			}

            if (progressIndicator != null)
            {
                progressIndicator.Dispose();
                progressIndicator = null;
            }

            if (progressTextField != null)
            {
                progressTextField.Dispose();
                progressTextField = null;
            }
        }
	}
}
