// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace InformaticaTrentinaPCL.iOS.EmbedComponent
{
    [Register ("EmbedComponentSearch")]
    partial class EmbedComponentSearch
    {
        [Outlet]
        UIKit.UIButton buttonCancel { get; set; }


        [Outlet]
        UIKit.UILabel labelDescriptionSearch { get; set; }


        [Outlet]
        UIKit.UITextField textfield { get; set; }


        [Outlet]
        UIKit.UIView viewFooter { get; set; }


        [Action ("ActionButtonCalcel:")]
        partial void ActionButtonCalcel (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}