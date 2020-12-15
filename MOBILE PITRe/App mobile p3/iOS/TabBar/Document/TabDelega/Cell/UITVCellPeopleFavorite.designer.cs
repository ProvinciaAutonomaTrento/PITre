// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace InformaticaTrentinaPCL.iOS.Delega.Storyboard
{
	[Register ("UITVCellPeopleFavorite")]
	partial class UITVCellPeopleFavorite
	{
		[Outlet]
		UIKit.UILabel desc { get; set; }

		[Outlet]
		UIKit.UIImageView image { get; set; }

		[Outlet]
		UIKit.UIImageView imageStart { get; set; }

		[Outlet]
		UIKit.UILabel label_title { get; set; }

		[Action ("ActionButtonStart:")]
		partial void ActionButtonStart (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (imageStart != null) {
				imageStart.Dispose ();
				imageStart = null;
			}

			if (desc != null) {
				desc.Dispose ();
				desc = null;
			}

			if (image != null) {
				image.Dispose ();
				image = null;
			}

			if (label_title != null) {
				label_title.Dispose ();
				label_title = null;
			}
		}
	}
}
