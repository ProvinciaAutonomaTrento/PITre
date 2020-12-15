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
    [Register ("UIViewControllerPeopleChooseFavorite")]
    partial class UIViewControllerPeopleChooseFavorite
    {
        [Outlet]
        UIKit.NSLayoutConstraint bottom { get; set; }


        [Outlet]
        UIKit.UIButton buttonClose { get; set; }


        [Outlet]
        UIKit.UIView conteinerEmpty { get; set; }


        [Outlet]
        UIKit.UIView conteinerList { get; set; }


        [Outlet]
        UIKit.UILabel labelTitle { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint leading { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint top { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint trailing { get; set; }


        [Action ("ActionButtonBack:")]
        partial void ActionButtonBack (Foundation.NSObject sender);


        [Action ("ActionButtonClose:")]
        partial void ActionButtonClose (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}