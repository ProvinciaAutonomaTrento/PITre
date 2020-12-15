using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace InformaticaTrentinaPCL.Droid.CustomLoader
{
    public class LoaderViewHolder : RecyclerView.ViewHolder
    {
        public CustomLoaderView loaderView;

        public LoaderViewHolder(View view) : base(view)
        {
            Context context = view.Context;
            ItemView = view;

            if (loaderView == null)
            {
                loaderView = new CustomLoaderView(context);
                FrameLayout.LayoutParams loaderParams = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                loaderParams.Gravity = GravityFlags.Center;
                loaderView.LayoutParameters = loaderParams;

                ((FrameLayout)ItemView).AddView(loaderView);
            }

        }
    }
}
