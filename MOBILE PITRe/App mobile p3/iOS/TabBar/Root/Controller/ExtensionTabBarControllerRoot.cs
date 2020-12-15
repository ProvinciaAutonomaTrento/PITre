using System;
using InformaticaTrentinaPCL.iOS.Menu.Storyboard;
using InformaticaTrentinaPCL.iOS.TabBar.Root.Storyboard;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.TabBar.Root.Controller
{
    public class ExtensionTabBarControllerRoot:UIViewController
    {
		public ExtensionTabBarControllerRoot(IntPtr handle) : base (handle)
        {
		}

        public ExtensionTabBarControllerRoot()
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

        }

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			Console.WriteLine("ViewDidAppear ExtensionTabBarControllerRoot");
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			Console.WriteLine("ViewDidDisappear ExtensionTabBarControllerRoot");
		}

    }
}
