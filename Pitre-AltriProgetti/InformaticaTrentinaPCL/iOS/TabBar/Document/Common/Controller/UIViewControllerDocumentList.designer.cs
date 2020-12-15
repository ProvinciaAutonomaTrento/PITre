// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace InformaticaTrentinaPCL.iOS.TabBar.Document.Common.Storyboard
{
    [Register ("UIViewControllerDocumentList")]
    partial class UIViewControllerDocumentList
    {
        [Outlet]
        UIKit.UIView ContainerEmptyView { get; set; }


        [Outlet]
        UIKit.UILabel errorLabel { get; set; }


        [Outlet]
        UIKit.UITableView errorView { get; set; }

        // UIKit.UIView errorView { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint heightFilterView { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint heightSearchView { get; set; }


        [Outlet]
        UIKit.UILabel labelDateFilter { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint leading { get; set; }


        [Outlet]
        UIKit.UITableView tableView { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint trailing { get; set; }


        [Outlet]
        UIKit.UIView viewFilter { get; set; }


        [Outlet]
        UIKit.UIView viewSearch { get; set; }


        [Action ("ActionButtonDeleteFilter:")]
        partial void ActionButtonDeleteFilter (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}