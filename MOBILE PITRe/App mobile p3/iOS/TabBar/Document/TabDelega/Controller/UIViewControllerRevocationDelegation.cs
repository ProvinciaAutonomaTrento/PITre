// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using InformaticaTrentinaPCL.iOS.Helper;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.Delega.Storyboard
{
	public partial class UIViewControllerRevocationDelegation : UIViewController
	{
		public UIViewControllerRevocationDelegation (IntPtr handle) : base (handle)
		{
		}

        public Action<bool> Callback;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            title_.Text = Utility.LanguageConvert("revocation_title");
            subTitle.Text = Utility.LanguageConvert("revocation_title1");
            desc.Text = Utility.LanguageConvert("revocation_desc");
            buttonDone.SetTitle(Utility.LanguageConvert("button_done").ToUpper(),UIControlState.Normal);		
            Font.SetCustomStyleFont(title_, Font.MODALE_TITLE,UITextAlignment.Center);
            Font.SetCustomStyleFont(subTitle, Font.LABEL, UITextAlignment.Center);
            Font.SetCustomStyleFont(desc, Font.DETAILS, UITextAlignment.Center);
            Font.SetCustomStyleFont(buttonDone, Font.ENABLED_BUTTONS);
		}

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            if (!Utility.IsTablet()) return;
            width.Constant = -(this.View.Frame.Width / 2);
            buttonBottom.Constant = UIApplication.SharedApplication.StatusBarOrientation.IsLandscape() ? 5 : 20;
        }

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			Console.WriteLine("ViewDidAppear");

            Animation.Start(Animation.DEFAULT_ANIMATION,() => {

                viewRoot.Alpha = 0.45f;

            },()=>{
                

            });
		}

        partial void ActionButtonClose(Foundation.NSObject sender)
        {
            viewRoot.Alpha = 0;
            Callback(false);
			this.DismissViewController(true,null);
        }

		partial void ActionbuttonDone(Foundation.NSObject sender)
        {
			viewRoot.Alpha = 0;
			Callback(true);
			this.DismissViewController(true, null);
		}

	}
}
