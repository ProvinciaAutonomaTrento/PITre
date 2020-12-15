// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using InformaticaTrentinaPCL.Filter;
using InformaticaTrentinaPCL.iOS.Filter;
using InformaticaTrentinaPCL.iOS.TabBar.Document.Common.Storyboard;
using InformaticaTrentinaPCL.iOS.TabBar.Root.Controller;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.TabBar.Root.Storyboard
{
	public partial class TabBarNavigationManager : UINavigationController
	{
        private static TabBarNavigationManager manager;
        private InterfaceNavigation interfaceNavigation;

        public static TabBarNavigationManager Instance()
        {
            return manager;
        }

		public TabBarNavigationManager (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewDidLoad()
        {
            manager = null;
            manager = this;
            base.ViewDidLoad();
            UITabBarControllerRoot controller = (UITabBarControllerRoot)ViewControllers[0];
            controller.myNavigationManager = this;
        }

        public override void PushViewController(UIViewController viewController, bool animated)
        {
            base.PushViewController(viewController, animated);
        }

        public override UIViewController[] PopToRootViewController(bool animated)
        {
            return base.PopToRootViewController(animated);
        }

        /// <summary>
        /// set the callback button.
        /// </summary>
        /// <returns><c>true</c>, if callback button was registered, <c>false</c> otherwise.</returns>
        /// <param name="interfaceNavigation">Interface navigation.</param>
        public bool SetCallbackButton(InterfaceNavigation interfaceNavigation)
        {
            this.interfaceNavigation = interfaceNavigation;

            return TabBarNavigationManager.manager != null;
        }

        /// <summary>
        /// Notifications the callback filter.
        /// </summary>
        /// <returns><c>true</c>, if callback filter was notificationed, <c>false</c> otherwise.</returns>
        public bool NotificationCallbackFilter()
        {
            if (this.interfaceNavigation != null)
            this.interfaceNavigation.CallbackButtonFilter();

            return this.interfaceNavigation != null;
        }

        /// <summary>
        /// Notifications the call sign document.
        /// </summary>
        /// <returns><c>true</c>, if call sign document was notificationed, <c>false</c> otherwise.</returns>
        public bool NotificationCallSignDocument()
        {
            if (this.interfaceNavigation != null)
                this.interfaceNavigation.CallSignDocument();

            return this.interfaceNavigation != null;
        }
	}
}