// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace InformaticaTrentinaPCL.iOS.TabBar.Root.Storyboard
{
	[Register ("UIViewControllerDocumentToDo")]
	partial class UIViewControllerDocumentToDo
	{
		[Outlet]
		UIKit.UILabel errorLabel { get; set; }

		[Outlet]
		UIKit.UIView errorView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (errorLabel != null) {
				errorLabel.Dispose ();
				errorLabel = null;
			}

			if (errorView != null) {
				errorView.Dispose ();
				errorView = null;
			}
		}
	}
}
