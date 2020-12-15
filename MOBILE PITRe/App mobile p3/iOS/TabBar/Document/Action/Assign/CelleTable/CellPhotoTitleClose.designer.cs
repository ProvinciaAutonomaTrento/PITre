// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace InformaticaTrentinaPCL.iOS.TabBar.Document.Action.Root.Storyboard
{
	[Register ("CellPhotoTitleClose")]
	partial class CellPhotoTitleClose
	{
		[Outlet]
		UIKit.UIImageView imageview { get; set; }

		[Outlet]
		UIKit.UILabel label_desc { get; set; }

		[Outlet]
		UIKit.UILabel label_title { get; set; }

		[Outlet]
		UIKit.UILabel labelCenter { get; set; }

		[Action ("ActionButtonClose:")]
		partial void ActionButtonClose (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (imageview != null) {
				imageview.Dispose ();
				imageview = null;
			}

			if (label_desc != null) {
				label_desc.Dispose ();
				label_desc = null;
			}

			if (label_title != null) {
				label_title.Dispose ();
				label_title = null;
			}

			if (labelCenter != null) {
				labelCenter.Dispose ();
				labelCenter = null;
			}
		}
	}
}
