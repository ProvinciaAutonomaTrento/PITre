// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace InformaticaTrentinaPCL.iOS.Login.Storyboard
{
    [Register ("UIViewControllerLogin")]
    partial class UIViewControllerLogin
    {
        [Outlet]
        UIKit.NSLayoutConstraint bottomButton { get; set; }


        [Outlet]
        UIKit.UIButton buttonCheck { get; set; }


        [Outlet]
        UIKit.UIButton buttonClicAdmin { get; set; }


        [Outlet]
        UIKit.UIButton changeEnviromentButton { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint downImg { get; set; }


        [Outlet]
        UIKit.UILabel enviromentLabel { get; set; }


        [Outlet]
        UIKit.UILabel eviromentTitleLabel { get; set; }


        [Outlet]
        UIKit.UIImageView imageView { get; set; }


        [Outlet]
        UIKit.UILabel labelRemember { get; set; }


        [Outlet]
        UIKit.UILabel labelVersion { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint marginDown { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint marginLeft { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint marginRight { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint marginUp { get; set; }


        [Outlet]
        UIKit.UILabel nameAdmini { get; set; }


        [Outlet]
        UIKit.UILabel titleAdmin { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint topUSername { get; set; }


        [Outlet]
        UIKit.UIButton UIButton_Login { get; set; }


        [Outlet]
        UIKit.UILabel uilabel_rememberPassword { get; set; }


        [Outlet]
        UIKit.UITextField uitextfield_username { get; set; }


        [Outlet]
        UIKit.UITextField uitextfiled_password { get; set; }


        [Outlet]
        UIKit.UIView viewAdministrator { get; set; }


        [Outlet]
        UIKit.UIView viewcheck { get; set; }


        [Outlet]
        UIKit.UIView viewContainer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PasswordDimenticataButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PasswordDimenticataButton2 { get; set; }


        [Action ("ActionButton_Login:")]
        partial void ActionButton_Login (Foundation.NSObject sender);


        [Action ("ActionButtonChangeEnviroment:")]
        partial void ActionButtonChangeEnviroment (UIKit.UIButton sender);


        [Action ("ActionbuttonCheck:")]
        partial void ActionbuttonCheck (Foundation.NSObject sender);


        [Action ("ActionButtonDeleteAdmin:")]
        partial void ActionButtonDeleteAdmin (Foundation.NSObject sender);


        [Action ("TextFieldEditingChanged:")]
        partial void TextFieldEditingChanged (Foundation.NSObject sender);


        [Action ("TextFieldEditingDidEnd:")]
        partial void TextFieldEditingDidEnd (Foundation.NSObject sender);


        [Action ("TextFieldValueChanged:")]
        partial void TextFieldValueChanged (Foundation.NSObject sender);

        [Action ("ActionButtonPasswordDimenticata:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionButtonPasswordDimenticata (UIKit.UIButton sender);

        [Action ("OpenViewRecuperaPassword:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OpenViewRecuperaPassword (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (PasswordDimenticataButton != null) {
                PasswordDimenticataButton.Dispose ();
                PasswordDimenticataButton = null;
            }

            if (PasswordDimenticataButton2 != null) {
                PasswordDimenticataButton2.Dispose ();
                PasswordDimenticataButton2 = null;
            }
        }
    }
}