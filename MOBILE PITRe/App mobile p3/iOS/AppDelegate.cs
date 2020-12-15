using System;
using Firebase.Core;
using Foundation;
using HockeyApp.iOS;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.iOS.Helper;
using InformaticaTrentinaPCL.iOS.Login.Session;
using InformaticaTrentinaPCL.iOS.Login.Storyboard;
using InformaticaTrentinaPCL.iOS.Menu.Storyboard;
using InformaticaTrentinaPCL.Network;
using PinningSSL;
using UIKit;

namespace InformaticaTrentinaPCL.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        static public SharedUrl sharedUrl;
        static public string sharedUrlString;

        public override UIWindow Window
        {
            get;
            set;
        }

        private void StarControllerLogin()
		{
            UIViewControllerLogin controllerRootSideMenu = (UIViewControllerLogin) UIStoryboard.FromName(UIViewControllerLogin.NAME_STORYBOARD, null)
           .InstantiateViewController(UIViewControllerLogin.NAME_CONTROLLER);
            Window.RootViewController = controllerRootSideMenu;
		}
    
        static public void StarMenuController()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            UIViewControllerRootSideMenu controllerRootSideMenu = (UIViewControllerRootSideMenu)UIStoryboard.FromName(UIViewControllerRootSideMenu.NAME_STORYBOARD, null)
            .InstantiateViewController(UIViewControllerRootSideMenu.NAME_CONTROLLER);
            window.RootViewController = controllerRootSideMenu;
        }

        private UIViewController TopViewController(UIViewController rootViewController) 
        {
            Console.WriteLine("TOP controller :" + rootViewController.ToString());

            if (rootViewController.PresentedViewController == null)
                return rootViewController;
  
  
            if ( rootViewController.PresentedViewController is UINavigationController) 
            {
                UINavigationController navigationController = (UINavigationController) rootViewController.PresentedViewController;
                UIViewController lastViewController = navigationController.ViewControllers[navigationController.ViewControllers.Length];
                return TopViewController(lastViewController);
             }

            if (rootViewController.PresentedViewController is UITabBarController)
            {
                UITabBarController tabbar = (UITabBarController)rootViewController.PresentedViewController;
                UIViewController lastViewController = tabbar.ViewControllers[tabbar.ViewControllers.Length];
                return TopViewController(lastViewController);
            }


            UIViewController presentedViewController = (UIViewController)rootViewController.PresentedViewController;
            return TopViewController(presentedViewController);
        }
      
        /// <summary>
        /// Starts the hockey app.
        /// </summary>
        private void StartHockeyApp()
        {
			/**INIT HOCKEYAPP**/
			var manager = BITHockeyManager.SharedHockeyManager;
			manager.Configure("e8515112b4834bd2b0c1e67d75f238c4");
			//manager.Authenticator.AuthenticateInstallation();
			manager.CrashManager.CrashManagerStatus = BITCrashManagerStatus.AutoSend;
			manager.StartManager();
		}

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White });

            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.Default, true);
            else            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);

            UIView statusBar = UIDevice.CurrentDevice.CheckSystemVersion(13, 0) ? new UIView(UIApplication.SharedApplication.StatusBarFrame) : UIApplication.SharedApplication.ValueForKey(new NSString("statusBar")) as UIView;
          //  UIView statusBar = UIApplication.SharedApplication.ValueForKey(new NSString("statusBar")) as UIView;
            if (statusBar.RespondsToSelector(new ObjCRuntime.Selector("setBackgroundColor:")))
            {
                statusBar.BackgroundColor = Colors.COLOR_NAVIGATION;
            }
            if(UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                statusBar.BackgroundColor = UIColor.Red;
            }

            StartHockeyApp();
			StarControllerLogin();
            StartPinningSSL();
            EnableCertificateValidation();

            return true;
        }


        private void StartPinningSSL()
        {
            if(NetworkConstants.IsPinningSSLEnabled())
            {
                ServicePointConfig.SetUp();
            }
        }

        public void EnableCertificateValidation()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
            (sender, cert, chain, sslPolicyErrors) =>
            {
                if (cert != null) System.Diagnostics.Debug.WriteLine(cert);
                return true;
            };
        }

        /// <summary>
        /// Opens the URL.  tyeo url p3i://www.pi3.it/share/xfkmdfkdmfd 
        /// </summary>
        /// <returns><c>true</c>, if URL was opened, <c>false</c> otherwise.</returns>
        /// <param name="application">Application.</param>
        /// <param name="url">URL.</param>
        /// <param name="sourceApplication">Source application.</param>
        /// <param name="annotation">Annotation.</param>
        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {

            sharedUrl = new SharedUrl(url.Description);

            if (sharedUrl.isValid)
            {
                sharedUrlString = url.Description;

                var session = IosNativeFactory.Instance().GetSessionData();
                // user not logged 
                if (session.userInfo == null)
                {
                    StarControllerLogin();
                }
                // user logged
                else
                {
                    StarMenuController();
                }

                return true;
            }
            return false;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }
    }
}

