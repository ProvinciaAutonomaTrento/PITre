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
    [Register ("CellImgStarTwoTitle")]
    partial class CellImgStarTwoTitle
    {
        [Outlet]
        UIKit.UIButton buttonStar { get; set; }


        [Outlet]
        UIKit.UIImageView imageProfile { get; set; }


        [Outlet]
        UIKit.UILabel labelBottom { get; set; }


        [Outlet]
        UIKit.UILabel labelCenter { get; set; }


        [Outlet]
        UIKit.UILabel labelTop { get; set; }


        [Action ("ActionButtonStar:")]
        partial void ActionButtonStar (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}