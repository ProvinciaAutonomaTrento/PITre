// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace InformaticaTrentinaPCL.iOS.TabBar.Document.Open.Storyboard
{
    [Register ("UIViewControllerSeeDocument")]
    partial class UIViewControllerSeeDocument
    {
        [Outlet]
        UIKit.UILabel labelTitle { get; set; }


        [Outlet]
        WebKit.WKWebView webview { get; set; }


        [Action ("ActionbuttonClose:")]
        partial void ActionbuttonClose (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}