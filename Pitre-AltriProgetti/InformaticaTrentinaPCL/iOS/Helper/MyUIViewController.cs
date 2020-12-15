using System;
using Foundation;
using InformaticaTrentinaPCL.iOS.Menu.Storyboard;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.Helper
{
    public class MyUIViewController : UIViewController
    {
        public MyUIViewController()
        {
        }

		public MyUIViewController(IntPtr handle) : base (handle)
        {
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
		}

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            UIViewControllerRootSideMenu.SetDetailVisible(true);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            UIViewControllerRootSideMenu.SetDetailVisible(false);
		}

        public override void PerformSegue(string identifier, Foundation.NSObject sender)
        {
            base.PerformSegue(identifier, sender);
        }

        public string NameClass()
        {
            return this.GetType().Name;
        }

    }
}
