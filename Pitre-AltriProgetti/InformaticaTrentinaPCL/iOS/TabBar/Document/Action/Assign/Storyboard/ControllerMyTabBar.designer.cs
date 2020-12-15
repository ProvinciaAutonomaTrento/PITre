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
    [Register ("ControllerMyTabBar")]
    partial class ControllerMyTabBar
    {
        [Outlet]
        UIKit.UIButton buttonModel { get; set; }


        [Outlet]
        UIKit.UIButton buttonStar { get; set; }


        [Outlet]
        UIKit.UIView container { get; set; }


        [Outlet]
        UIKit.UIView containerSearch { get; set; }


        [Outlet]
        UIKit.UIView containerTab { get; set; }


        [Outlet]
        UIKit.UILabel labelEmptySearch { get; set; }


        [Outlet]
        UIKit.UILabel labelEmptyTab { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint leading { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint trailing { get; set; }


        [Outlet]
        UIKit.UIView viewFooter { get; set; }


        [Outlet]
        UIKit.UIView viewFooterSeleModel { get; set; }


        [Outlet]
        UIKit.UIView viewFooterSeleStar { get; set; }


        [Action ("ActionButtonTab:")]
        partial void ActionButtonTab (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}