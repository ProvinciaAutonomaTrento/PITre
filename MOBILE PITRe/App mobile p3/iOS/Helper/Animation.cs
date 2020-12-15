using System;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.Helper
{
    public class Animation
    {

        public static nfloat DEFAULT_ANIMATION = 1.0f;
		public static nfloat SHORT_ANIMATION = 0.5f;
        public static nfloat RAPID_ANIMATION = 0.2f;

		// secondi 
		public static void Start(nfloat duration, Action start, Action end)
        {
            UIView.Animate(duration, 0, UIViewAnimationOptions.CurveEaseInOut,
                       () =>
                    {
                        start();
                    },
                   () =>
                    {
                        end();
                    });

        }
        #region test
        //     public static LOTAnimationView AnimationLOTApp(string nameImage, UIView view, bool loop = true)
        //     {

        //LOTAnimationView animation = LOTAnimationView.AnimationNamed("jsonImg/"+nameImage);
        //         animation.Frame = new CoreGraphics.CGRect(0, 0, view.Frame.Width, view.Frame.Height);
        //view.AddSubview(animation);
        //animation.LoopAnimation = loop;

        //animation.PlayWithCompletion((animationFinished) =>
        //{
        //	Console.WriteLine("PlayWithCompletion");
        //});


        //foreach (string family in UIFont.FamilyNames)
        //{
        //	Console.Write( "- "+family+" :");
        //               foreach (string name in UIFont.FontNamesForFamilyName(family))
        //	{
        //		Console.WriteLine(name + ", ");
        //	}
        //}


        //    return animation;

        //}
        #endregion

    }
}
