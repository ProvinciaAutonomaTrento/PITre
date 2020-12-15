using System;
using Foundation;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.Helper
{
    public class StyleTablet
    {
        public StyleTablet()
        {
        }

        public static nfloat TOP_FROM_NAVIGATION = Utility.IsTablet()?80f:0;

        public static nfloat LEFT_MARGIN_BUTTON_DEFAULT = Utility.IsTablet() ? 100 : 0;
        public static nfloat RIGHT_MARGIN_BUTTON_DEFAULT = Utility.IsTablet() ? 100 : 0;


		private static UIView ViewNavigationForTablet;
        private static UIButton ButtonBack;
        private static UIButton ButtonClose;
        private static UILabel LabelTitle;

        public static nfloat TopKeyboard()
        {
            nfloat top = Utility.IsTablet()?350:0;
            if (UIApplication.SharedApplication.StatusBarOrientation.IsLandscape())
            {
               top = 100;
            }

            return top;
        }



		/// <summary>
		/// Margins the bottom and top.
		/// </summary>
		/// <returns>The bottom and top.</returns>
		public static nfloat MarginBottomAndTopForViewController()
		{
			var idiom = UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom;
			if (UIUserInterfaceIdiom.Pad == idiom)
			{
				var w = UIScreen.MainScreen.Bounds.Width / 2;
				var h = UIScreen.MainScreen.Bounds.Height / 2;
                var ow = Utility.getOriginalScreenWidth() / 2;
				var oh = Utility.getOriginalScreenHeight() / 2;
				return h - oh;
			}

			return 0;
		}

        public static nfloat WidthCommonPresentation(UIView View, int space = 100)
        {
            nfloat spacelate = Utility.IsTablet()?space:0;
            if (UIApplication.SharedApplication.StatusBarOrientation.IsLandscape())
            {
                nfloat diff = View.Frame.Height - View.Frame.Width;
                diff = diff * (-1) * (-1) * (-1);  // per rednerlo positivo
                diff = diff / 2;
                spacelate = diff+80;
            }

            return spacelate;
        }


        public static nfloat MarginTopDefault()
        {
            var idiom = UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom;

            if (UIUserInterfaceIdiom.Pad == idiom)
            {
                return 50;
            }

            return 0;
        }


        public static nfloat MarginBottonDefault()
        {
            var idiom = UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom;

            if (UIUserInterfaceIdiom.Pad == idiom)
            {
                return 50;
            }

            return 0;
        }


        public static nfloat MarginLeftDefault()
        {
            var idiom = UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom;

            if (UIUserInterfaceIdiom.Pad == idiom)
            {
                return 80;
            }

            return 0; 
        }

        public static nfloat MarginRightDefault()
        {
            var idiom = UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom;

            if (UIUserInterfaceIdiom.Pad == idiom)
            {
                return 80;
            }

            return 0;
        }


		/// <summary>
		/// Margins the right and left.
		/// </summary>
		/// <returns>The right and left.</returns>
		public static nfloat MarginRightAndLeftForViewController()
		{
			var idiom = UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom;

			if (UIUserInterfaceIdiom.Pad == idiom)
			{
				var w = UIScreen.MainScreen.Bounds.Width / 2;
				var h = UIScreen.MainScreen.Bounds.Height / 2;
				var ow = Utility.getOriginalScreenWidth() / 2;
				var oh = Utility.getOriginalScreenHeight() / 2;

				return w - ow;
			}

			return 0;
		}

		/// <summary>
		/// Margins the right and left for table view.
		/// </summary>
		/// <returns>The right and left for table view.</returns>
		public static nfloat MarginRightAndLeftForTableView(int size = 75)
		{
			var idiom = UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom;

			if (UIUserInterfaceIdiom.Pad == idiom)
				return size;

			return 0;
		}

        public static UIView NavigationForTablet(string title, UIViewController controller, Action<ActionNavigationType> callback, bool other, bool upper = true)
        {
            int top = 50;
            int space = 30;
			NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidChangeStatusBarOrientationNotification, (obj) =>
			{
                    if (ViewNavigationForTablet != null)
                    ViewNavigationForTablet.Frame = new CoreGraphics.CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, top * 2);
					
                    if (ButtonClose != null)
                    ButtonClose.Frame = new CoreGraphics.CGRect(UIScreen.MainScreen.Bounds.Width - top, top, space, space);
					
                    //if (ButtonBack != null)
                    //ButtonBack.Frame = new CoreGraphics.CGRect(space, top, space, space);
					
                    if (LabelTitle != null)
                    LabelTitle.Frame = new CoreGraphics.CGRect(UIScreen.MainScreen.Bounds.Width/2 - top*2, top, top*4, space);
			});
            ViewNavigationForTablet = null;
            //ButtonBack = null;
            ButtonClose = null;
            LabelTitle = null;
            ViewNavigationForTablet = new UIView();
            ViewNavigationForTablet.BackgroundColor = UIColor.White;
            ViewNavigationForTablet.Frame = new CoreGraphics.CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, top * 2);
            if (other)
            {
                ButtonClose = new UIButton();
                ButtonClose.SetImage(UIImage.FromBundle("close"), UIControlState.Normal);
                ButtonClose.Frame = new CoreGraphics.CGRect(UIScreen.MainScreen.Bounds.Width - top, top, space, space);
                ButtonClose.TintColor = UIColor.Black;
                ViewNavigationForTablet.Add(ButtonClose);

                ButtonClose.TouchUpInside += (object sender, System.EventArgs e) =>
                {
                    if (callback != null)
                        callback(ActionNavigationType.ActionTabletClose);
                };
            }
            //ButtonBack = new UIButton();
            //ButtonBack.SetImage(UIImage.FromBundle("leftback"), UIControlState.Normal);
            //ButtonBack.Frame = new CoreGraphics.CGRect(space, top, space, space);
            //ViewNavigationForTablet.Add(ButtonBack);
            //ButtonBack.TintColor = UIColor.Black;
            //ButtonBack.TouchUpInside += (object sender, System.EventArgs e) =>
            //{
            //    if (callback != null)
            //        callback(ActionNavigationType.ActionTabletBack);
            //};
            //ButtonBack.Hidden = true;
            LabelTitle = new UILabel();
            if (upper) LabelTitle.Text = title.ToUpper();
            else
                LabelTitle.Text = title;
            Font.SetCustomStyleFont(LabelTitle, Font.TITLE_BLACK, UITextAlignment.Center, false);
            LabelTitle.TextColor = Colors.COLOR_BLUE_TEXT_ROW_COLOR;
            LabelTitle.BackgroundColor = UIColor.Clear;
            LabelTitle.Frame = new CoreGraphics.CGRect(ViewNavigationForTablet.Center.X - top*2, top, top*4, space);
            ViewNavigationForTablet.AddSubview(LabelTitle);
            controller.Add(ViewNavigationForTablet);
			return ViewNavigationForTablet;
        }



    }
}
