// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace InformaticaTrentinaPCL.iOS.TabBar.Document.Action.Assign.Storyboard
{
    [Register ("ControllerAssignChooseRole")]
    partial class ControllerAssignChooseRole
    {
        [Outlet]
        UIKit.UILabel assegnatario { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint bottom { get; set; }


        [Outlet]
        UIKit.UIButton buttonLeft { get; set; }


        [Outlet]
        UIKit.UIButton buttonright { get; set; }


        [Outlet]
        UIKit.UIView containerTable { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint left { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint right { get; set; }


        [Outlet]
        UIKit.UILabel titleLabel { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint top { get; set; }


        [Outlet]
        UIKit.UIView viewContainer { get; set; }


        [Action ("ActionButtonBack:")]
        partial void ActionButtonBack (Foundation.NSObject sender);


        [Action ("ActionButtonExit:")]
        partial void ActionButtonExit (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}