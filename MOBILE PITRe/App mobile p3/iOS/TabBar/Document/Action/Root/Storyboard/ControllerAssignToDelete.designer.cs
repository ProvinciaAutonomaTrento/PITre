// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace InformaticaTrentinaPCL.iOS.TabBar.Document.Action.Root.Storyboard
{
    [Register ("ControllerAssignToDelete")]
    partial class ControllerAssignToDelete
    {
        [Outlet]
        UIKit.UIImageView image { get; set; }


        [Outlet]
        UIKit.UILabel labelCenter { get; set; }


        [Outlet]
        UIKit.UILabel labelDesc { get; set; }


        [Outlet]
        UIKit.UILabel labelSubTitle { get; set; }


        [Outlet]
        UIKit.UILabel labelTitle { get; set; }


        [Action ("ActionButtonDelete:")]
        partial void ActionButtonDelete (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}