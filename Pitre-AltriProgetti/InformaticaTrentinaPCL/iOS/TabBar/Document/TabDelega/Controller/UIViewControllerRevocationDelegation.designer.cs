// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace InformaticaTrentinaPCL.iOS.Delega.Storyboard
{
    [Register ("UIViewControllerRevocationDelegation")]
    partial class UIViewControllerRevocationDelegation
    {
        [Outlet]
        UIKit.NSLayoutConstraint buttonBottom { get; set; }


        [Outlet]
        UIKit.UIButton buttonDone { get; set; }


        [Outlet]
        UIKit.UILabel desc { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint leading { get; set; }


        [Outlet]
        UIKit.UILabel subTitle { get; set; }


        [Outlet]
        UIKit.UILabel title_ { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint trailing { get; set; }


        [Outlet]
        UIKit.UIView viewRoot { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint width { get; set; }


        [Action ("ActionButtonClose:")]
        partial void ActionButtonClose (Foundation.NSObject sender);


        [Action ("ActionbuttonDone:")]
        partial void ActionbuttonDone (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}