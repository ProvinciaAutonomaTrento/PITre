using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace InformaticaTrentinaPCL.Droid.CustomLoader
{
    public class CustomLoaderView : FrameLayout
    {
        bool animationEnabled;

        Context mContext;
        ImageView imageView1;
        ImageView imageView2;
        Animation anim1;
        Animation anim2;

        public CustomLoaderView(Context context) : base(context)
        {
            init(context);
        }

        public CustomLoaderView(Context context, IAttributeSet attrs) : base(context)
        {
            init(context);
        }

        public CustomLoaderView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context)
        {
            init(context);
        }

        public CustomLoaderView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context)
        {
            init(context);
        }

        private void init(Context context)
        {
            mContext = context;
            
            Inflate(mContext, Resource.Layout.custom_loader, this);

            //View v = inflate(mContext, R.layout.custom_loader, null);

            imageView1 = FindViewById<ImageView>(Resource.Id.image1);
            anim1 = AnimationUtils.LoadAnimation(mContext, Resource.Animation.anim1);

            imageView2 = (ImageView)FindViewById(Resource.Id.image2);
            anim2 = AnimationUtils.LoadAnimation(mContext, Resource.Animation.anim2);

            //this.addView(v);
            this.Visibility = ViewStates.Gone;
        }

        public void startAnimation()
        {
            this.Visibility = ViewStates.Visible;
            animationEnabled = true;
            imageView1.StartAnimation(anim1);
            imageView2.StartAnimation(anim2);
        }

        public void stopAnimation()
        {
            this.Visibility = ViewStates.Gone;
            animationEnabled = false;
            imageView1.ClearAnimation();
            imageView2.ClearAnimation();

        }

        public bool isAnimationEnabled()
        {
            return animationEnabled;
        }
    }
}
