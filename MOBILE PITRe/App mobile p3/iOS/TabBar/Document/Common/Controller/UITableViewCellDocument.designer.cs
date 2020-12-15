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
    [Register ("UITableViewCellDocument")]
    partial class UITableViewCellDocument
    {
        [Outlet]
        UIKit.NSLayoutConstraint hightImg { get; set; }


        [Outlet]
        UIKit.UIImageView image { get; set; }


        [Outlet]
        UIKit.UILabel labelDocumentDescp { get; set; }


        [Outlet]
        UIKit.UILabel labelDocumentSignType { get; set; }


        [Outlet]
        UIKit.UILabel labelDucumentSudDes { get; set; }


        [Outlet]
        UIKit.UILabel labelName { get; set; }


        [Outlet]
        UIKit.UILabel labelSegnatura { get; set; }


        [Outlet]
        UIKit.UILabel labelTypeDocument { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint leadingImg { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint leftMargin { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint RightMargin { get; set; }


        [Outlet]
        UIKit.UIView viewEtichetta { get; set; }


        [Outlet]
        UIKit.UIView viewRoot { get; set; }


        [Outlet]
        UIKit.UIView viewSeparatorFooter { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton firma { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView lineAttachmentDown { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView lineAttachmentUp { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView pulsanti { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint pulsantiWidth { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton rifiuta { get; set; }


        [Action ("firmaAction:")]
        partial void firmaAction (UIKit.UIButton sender);


        [Action ("rifiutaAction:")]
        partial void rifiutaAction (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (firma != null) {
                firma.Dispose ();
                firma = null;
            }

            if (lineAttachmentDown != null) {
                lineAttachmentDown.Dispose ();
                lineAttachmentDown = null;
            }

            if (lineAttachmentUp != null) {
                lineAttachmentUp.Dispose ();
                lineAttachmentUp = null;
            }

            if (pulsanti != null) {
                pulsanti.Dispose ();
                pulsanti = null;
            }

            if (pulsantiWidth != null) {
                pulsantiWidth.Dispose ();
                pulsantiWidth = null;
            }

            if (rifiuta != null) {
                rifiuta.Dispose ();
                rifiuta = null;
            }
        }
    }
}