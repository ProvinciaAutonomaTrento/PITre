// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace InformaticaTrentinaPCL.iOS.Delega
{
    [Register ("UIViewControllerNewDelegation")]
    partial class UIViewControllerNewDelegation
    {
        [Outlet]
        UIKit.NSLayoutConstraint bottom { get; set; }

        [Outlet]
        UIKit.UIButton buttonDone { get; set; }

        [Outlet]
        UIKit.UILabel labelTitle { get; set; }

        [Outlet]
        UIKit.NSLayoutConstraint leadingBotton { get; set; }

        [Outlet]
        UIKit.NSLayoutConstraint left { get; set; }

        [Outlet]
        UIKit.NSLayoutConstraint right { get; set; }

        [Outlet]
        UIKit.UITableView tableView { get; set; }

        [Outlet]
        UIKit.NSLayoutConstraint top { get; set; }

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