// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace InformaticaTrentinaPCL.iOS.Filter
{
    [Register ("UITVCellSelectedData")]
    partial class UITVCellSelectedData
    {
        [Outlet]
        UIKit.UILabel text { get; set; }

        [Outlet]
        UIKit.UITextField textDay { get; set; }

        [Outlet]
        UIKit.UITextField textMonth { get; set; }

        [Outlet]
        UIKit.UITextField textyear { get; set; }

        [Outlet]
        UIKit.UIView viewFooter1 { get; set; }

        [Outlet]
        UIKit.UIView viewFooter2 { get; set; }

        [Outlet]
        UIKit.UIView viewFooter3 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel desc_Ricerca { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (desc_Ricerca != null) {
                desc_Ricerca.Dispose ();
                desc_Ricerca = null;
            }
        }
    }
}