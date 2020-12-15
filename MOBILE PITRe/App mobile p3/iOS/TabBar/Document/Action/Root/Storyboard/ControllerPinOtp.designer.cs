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
    [Register ("ControllerPinOtp")]
    partial class ControllerPinOtp
    {
        [Outlet]
        UIKit.UIButton buttonOtp { get; set; }

        [Outlet]
        UIKit.UIButton buttonRemberPin { get; set; }

        [Outlet]
        UIKit.UIView footer1 { get; set; }

        [Outlet]
        UIKit.UIView footer2 { get; set; }

        [Outlet]
        UIKit.UILabel labelButtonOtp { get; set; }

        [Outlet]
        UIKit.UILabel labelRememberPin { get; set; }

        [Outlet]
        UIKit.NSLayoutConstraint otpRight { get; set; }

        [Outlet]
        UIKit.NSLayoutConstraint pinRight { get; set; }

        [Outlet]
        UIKit.UITextField texOtp { get; set; }

        [Outlet]
        UIKit.UITextField textPin { get; set; }

        [Action ("ActionButtonOtp:")]
        partial void ActionButtonOtp (Foundation.NSObject sender);

        [Action ("ActionbuttonRememberPin:")]
        partial void ActionbuttonRememberPin (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}