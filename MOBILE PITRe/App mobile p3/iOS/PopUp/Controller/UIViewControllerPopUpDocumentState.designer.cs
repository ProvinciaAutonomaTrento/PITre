// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace InformaticaTrentinaPCL.iOS.PopUp.Storyboard
{
    [Register ("UIViewControllerPopUpDocumentState")]
    partial class UIViewControllerPopUpDocumentState
    {
        [Outlet]
        UIKit.NSLayoutConstraint bottom { get; set; }


        [Outlet]
        UIKit.UIButton buttonClose { get; set; }


        [Outlet]
        UIKit.UIButton buttonNextDocument { get; set; }


        [Outlet]
        UIKit.UIImageView image { get; set; }


        [Outlet]
        UIKit.UIImageView imageClose { get; set; }


        [Outlet]
        UIKit.UILabel labelDescDocument { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint left { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint right { get; set; }


        [Outlet]
        UIKit.UITableView table { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint top { get; set; }


        [Outlet]
        UIKit.UIView viewRoots { get; set; }


        [Action ("ActionButtonClose:")]
        partial void ActionButtonClose (Foundation.NSObject sender);


        [Action ("ActionButtonNexDocument:")]
        partial void ActionButtonNexDocument (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}