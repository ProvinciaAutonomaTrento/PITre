// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace InformaticaTrentinaPCL.iOS.Menu.Storyboard
{
    [Register ("UIViewControllerMaster")]
    partial class UIViewControllerMaster
    {
        [Outlet]
        UIKit.NSLayoutConstraint heightTable { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint heightTableDown { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint heightViewTop { get; set; }


        [Outlet]
        UIKit.UILabel labelName { get; set; }


        [Outlet]
        UIKit.UILabel labelOrganization { get; set; }


        [Outlet]
        UIKit.UIImageView profileImg { get; set; }


        [Outlet]
        UIKit.UITableView tabBar { get; set; }


        [Outlet]
        UIKit.UITableView tableView { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint topDescRole { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint topImage { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint topName { get; set; }


        [Outlet]
        UIKit.UIView viewDownTabBar { get; set; }


        [Outlet]
        UIKit.UIView viewProfile { get; set; }


        [Outlet]
        UIKit.UIView viewTable { get; set; }


        [Outlet]
        UIKit.UIView viewTopTabBar { get; set; }


        [Action ("ActionButtonChangeRole:")]
        partial void ActionButtonChangeRole (Foundation.NSObject sender);


        [Action ("actionProfileImage:")]
        partial void actionProfileImage (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}