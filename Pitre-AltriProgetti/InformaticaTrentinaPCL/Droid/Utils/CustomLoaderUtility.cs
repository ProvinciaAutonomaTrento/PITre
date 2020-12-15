using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Com.Wang.Avi;
using InformaticaTrentinaPCL.Droid.CustomLoader;

namespace InformaticaTrentinaPCL.Droid.Utils
{
    /// <summary>
    /// Classe utilizzata per centralizzare la gestione della ui di caricamento
    /// </summary>
    public class CustomLoaderUtility
    {
        CustomLoaderView loaderView;
        Dialog loaderDialog;

        public CustomLoaderUtility()
        {
        }

        /// <summary>
        /// Mostra il loader
        /// </summary>
        public void showLoader(Activity activity, Action action = null)
        {
            if (loaderView == null)
            {
                loaderView = new CustomLoaderView(activity);
            }
            if (loaderDialog == null)
            {
                int width = (int) activity.Resources.GetDimension(Resource.Dimension.progress_indicator_width);
                int height = (int) activity.Resources.GetDimension(Resource.Dimension.progress_indicator_height);
                FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(width, height);
                layoutParams.Gravity = GravityFlags.Center;
                loaderDialog = new Dialog(activity, Resource.Style.LoaderBackground);
                loaderDialog.SetCancelable(action != null);
                loaderDialog.AddContentView(loaderView, layoutParams);
            }

            if (action != null)
            {
                loaderDialog.SetCanceledOnTouchOutside(true);
                loaderDialog.CancelEvent += delegate { action(); };
            }
            loaderDialog.Show();
            loaderView.startAnimation();
        }

        

        /// <summary>
        /// Nasconde il loader
        /// </summary>
        public void hideLoader()
        {
            loaderView?.stopAnimation();
            loaderDialog?.Dismiss();
            loaderView = null;
            loaderDialog = null;
        }
    }
}
