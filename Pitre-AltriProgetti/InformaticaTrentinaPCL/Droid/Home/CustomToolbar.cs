using System;
using Android.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Droid.Utils;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace InformaticaTrentinaPCL.Droid.Home
{
    public class CustomToolbar : Toolbar
    {
        private TextView titleTextView;
        private View closeBtn;
        
        public CustomToolbar(Context context) : base(context)
        {
        }

        public CustomToolbar(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public CustomToolbar(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        private void Init()
        {
            if (titleTextView == null)
            {
                titleTextView = FindViewById<TextView>(Resource.Id.textView_title);
            }

            if (titleTextView == null)
            {
                return;
            }
            
            UIUtility.setTextAppearance(Context, titleTextView, Resource.Style.toolbar_page_title);    
            LayoutParams lp = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            lp.Gravity = (int) GravityFlags.Center;
            titleTextView.LayoutParameters = lp;
        }
        
        public void UpdateToolbarForSectionType(ActionBar SupportActionBar, string title, Action action)
        {
            Init();
            titleTextView.Text = title;
            
            bool isSmartphone = Resources.GetBoolean(Resource.Boolean.isSmartphone);

            if (!isSmartphone)
            {
                SupportActionBar.SetDisplayHomeAsUpEnabled(false);
                SupportActionBar.SetHomeButtonEnabled(false);
                SetBackgroundColor(Resources.GetColor(Resource.Color.colorWhite));
                UIUtility.setTextAppearance(Context, titleTextView, Resource.Style.toolbar_section_title_tablet);    
                View closeBtn = FindViewById(Resource.Id.closeBtn);
                closeBtn.Visibility = ViewStates.Visible;    
                closeBtn.Click += delegate { action(); };
            }
            else
            {
                UIUtility.setTextAppearance(Context, titleTextView, Resource.Style.toolbar_section_title);    
                LayoutParams lp = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
                lp.Gravity = (int) GravityFlags.Left;
                titleTextView.LayoutParameters = lp;
            }
        }

        public void UpdateTitle(string title)
        {
            Init();
            titleTextView.Text = title;
        }
    }
}