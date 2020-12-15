using System;
using CoreGraphics;
using UIKit;
using Xamarin.iOS.DGActivityIndicatorViewBinding;

namespace InformaticaTrentinaPCL.iOS.Helper
{
    public class LoadingOverlay : UIViewController
	{

		// control declarations
		private UIActivityIndicatorView activitySpinner;
		private UIView bgSpinner;
		private UILabel loadingLabel;
		private nfloat bgSize = 150f;
		private DGActivityIndicatorView indicatorCustom;

		public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var f = this.View.Frame;
            //Console.WriteLine(": diload: "+f.Height);

			AnimationCustom();

		}

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }


        public override void ViewDidLayoutSubviews()
        {
			var f = this.View.Frame;
			//Console.WriteLine(": ViewDidLayoutSubviews: "+f.Height);
            nfloat centerX = this.View.Frame.Width / 2;
            nfloat centerY = this.View.Frame.Height / 2;
			indicatorCustom.Frame = new CoreGraphics.CGRect(centerX - 50, centerY - 50, 100, 100);
			base.ViewDidLayoutSubviews();
        }

		private void AnimationCustom()
		{
			nfloat centerX = this.View.Frame.Width / 2;
			nfloat centerY = this.View.Frame.Height / 2;
            indicatorCustom = new DGActivityIndicatorView(DGActivityIndicatorAnimationType.DoubleBounce, Colors.COLOR_NAVIGATION);
            indicatorCustom.Frame = new CoreGraphics.CGRect(centerX-50, centerY-50, 100, 100);
            this.View.AddSubview(indicatorCustom);
			indicatorCustom.StartAnimating();
		}


        private void AnimationNormal()
        {
            this.View.BackgroundColor = UIColor.Black;
            this.View.Alpha = 0.3f;
            //AutoresizingMask = UIViewAutoresizing.All;
            nfloat labelHeight = 22;
            nfloat labelWidth = this.View.Frame.Width - 20;
            // derive the center x and y
            nfloat centerX = this.View.Frame.Width / 2;
            nfloat centerY = this.View.Frame.Height / 2;

            bgSpinner = new UIView(new CGRect(centerX - bgSize / 2, centerY - bgSize / 2, bgSize, bgSize))
            {
                BackgroundColor = UIColor.Clear //UIColor.Black
            };
            bgSpinner.Layer.CornerRadius = 10f;
            bgSpinner.Layer.MasksToBounds = true;
            this.View.AddSubview(bgSpinner);

            // create the activity spinner, center it horizontall and put it 5 points above center x
            activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            activitySpinner.Frame = new CGRect(
                centerX - (activitySpinner.Frame.Width / 2),
                centerY - activitySpinner.Frame.Height,
                activitySpinner.Frame.Width,
                activitySpinner.Frame.Height
            );
            activitySpinner.AutoresizingMask = UIViewAutoresizing.All;
            this.View.AddSubview(activitySpinner);
            activitySpinner.StartAnimating();
            activitySpinner.Color = Colors.COLOR_NAVIGATION;

            // create and configure the "Loading Data" label
            loadingLabel = new UILabel(new CGRect(
                centerX - (labelWidth / 2),
                centerY + 20,
                labelWidth,
                labelHeight
            ));
            loadingLabel.BackgroundColor = UIColor.Clear;
            loadingLabel.TextColor = UIColor.Black;  //White;
            loadingLabel.Text = Utility.LanguageConvert("Loading");
            loadingLabel.TextAlignment = UITextAlignment.Center;
            loadingLabel.AutoresizingMask = UIViewAutoresizing.All;



			this.View.AddSubview(loadingLabel);

		}

		/// <summary>
		/// Fades out the control and then removes it from the super view
		/// </summary>
		public void Hide()
		{
            

            if (indicatorCustom != null)
            {
                indicatorCustom.StopAnimating();
                indicatorCustom.RemoveFromSuperview();
                indicatorCustom = null;
            }
            else 
            {
				UIView.Animate(
				  0.5, // duration
                    () => { this.View.Alpha = 0; },
				  () => { this.View.RemoveFromSuperview(); }
				);
			}
		}

	}
}
