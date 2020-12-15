// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace InformaticaTrentinaPCL.iOS.Filter
{
    [Register ("UIVControllerFilter")]
    partial class UIVControllerFilter
    {
        [Outlet]
        UIKit.UIButton buttondone { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint leadingButton { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint marginDown { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint marginLeft { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint marginRight { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint marginUp { get; set; }


        [Outlet]
        UIKit.UITableView tableView { get; set; }


        [Outlet]
        UIKit.UILabel tilteController { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint trailingButton { get; set; }


        [Action ("ActionButtonClose:")]
        partial void ActionButtonClose (Foundation.NSObject sender);


        [Action ("ActionButtonDone:")]
        partial void ActionButtonDone (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}