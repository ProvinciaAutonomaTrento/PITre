using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.Helper
{
    public class Keyboard
    {
        public static int CUSTOM_Y = 0;

        private CGRect frameInit;
        private UIView view;
        private int heightCustom = 0;
        private NSNotificationCenter center1 = NSNotificationCenter.DefaultCenter;
        private NSNotificationCenter center2 = NSNotificationCenter.DefaultCenter;

        public Keyboard()
        {
        }

        /// <summary>
        /// Keyboards the listener did show.
        /// </summary>
        /// <param name="Callback">Callback.</param>
        public void KeyboardListenerDidShow(Action<CGRect> Callback)
        {
            center1 = NSNotificationCenter.DefaultCenter;
            center1.AddObserver(UIKeyboard.DidShowNotification, (notification) =>
            {
                    CoreGraphics.CGRect r = UIKeyboard.BoundsFromNotification(notification);
                    Callback(r);
            });
        }

        public void KeyboardListenerWillDidShow(Action<CGRect> Callback)
        {
            center1 = NSNotificationCenter.DefaultCenter;
            center1.AddObserver(UIKeyboard.WillShowNotification, (notification) =>
            {
                CoreGraphics.CGRect r = UIKeyboard.BoundsFromNotification(notification);
                Callback(r);
            });
        }

        /// <summary>
        /// Keyboards the listener did hide.
        /// </summary>
        /// <param name="Callback">Callback.</param>
        public void KeyboardListenerDidHide(Action<CGRect> Callback)
        {
            center2 = NSNotificationCenter.DefaultCenter;
            center2.AddObserver(UIKeyboard.DidHideNotification, (notification) =>
            {
                CoreGraphics.CGRect r = UIKeyboard.BoundsFromNotification(notification);
                Callback(r);
            });
        }

      /// <summary>
      /// Keyboards the listener will did hide.
      /// </summary>
      /// <param name="Callback">Callback.</param>
        public void KeyboardListenerWillDidHide(Action<CGRect> Callback)
        {
            center2 = NSNotificationCenter.DefaultCenter;
           
            center2.AddObserver(UIKeyboard.WillHideNotification, (notification) =>
            {
                CoreGraphics.CGRect r = UIKeyboard.BoundsFromNotification(notification);
                Callback(r);
            });
        }

        // UIApplication.SharedApplication.StatusBarOrientation.IsLandscape() 
        public void RegisterForNotificationsOrientations(Action ChangeStatus)
        {
            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidChangeStatusBarOrientationNotification, (obj) =>
            {
                if (ChangeStatus != null)
                {
                    ChangeStatus();
                }
            });
        }

        /// <summary>
        /// Keyboards the listener.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="heightCustom">Height custom. La view viene splittata dalla altezza desiderata. Se settato a zero 
        /// la view si alzera precisamente della stessa altezza della keyboard. Normalmente conviene impostare un valore preciso
        ///  </param>
        public void KeyboardListener(UIView view,int heightCustom = 0)
        {
            center1 = NSNotificationCenter.DefaultCenter;
            center2 = NSNotificationCenter.DefaultCenter;
            frameInit = view.Frame;
            this.view = view;
            this.heightCustom = heightCustom;

            center1.AddObserver(UIKeyboard.DidShowNotification, (notification) => 
            {
                CoreGraphics.CGRect r = UIKeyboard.BoundsFromNotification(notification);
                Animation.Start(Animation.RAPID_ANIMATION, () => 
                {
                    this.view.Frame = new CoreGraphics.CGRect(0,heightCustom==0?(-r.Height):(-heightCustom), this.view.Frame.Width, this.view.Frame.Height);
                }, () => { });
            });

            center2.AddObserver(UIKeyboard.DidHideNotification, (notification) => 
            {
              CoreGraphics.CGRect r = UIKeyboard.BoundsFromNotification(notification);
                Animation.Start(Animation.RAPID_ANIMATION, () =>
                {
                    this.view.Frame = frameInit;
                }, () => { });
            });
        }


        public void RemoveListener()
        {
            center2.RemoveObserver(UIKeyboard.DidHideNotification);
            center1.RemoveObserver(UIKeyboard.DidShowNotification);
            center2.RemoveObserver(UIKeyboard.WillHideNotification);
            center1.RemoveObserver(UIKeyboard.WillShowNotification);
            center1 = null;
            center2 = null;
        }

    }
}
