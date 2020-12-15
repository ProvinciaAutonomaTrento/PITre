using Android.Content;
using Android.Support.Constraints;
using Android.Util;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Filter;

namespace InformaticaTrentinaPCL.Droid.Home
{
    public class CustomFilterView : ConstraintLayout
    {
        Context mContext;

        public ICustomFilterViewListener filterViewListener { get; set; }

        TextView filter;
        ImageView remove;

        public CustomFilterView(Context context) : base(context)
        {
            init(context);
        }

        public CustomFilterView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            init(context);
        }

        public CustomFilterView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            init(context);
        }

        void init(Context context)
        {
            mContext = context;

            Inflate(mContext, Resource.Layout.custom_filter_view, this);

            filter = FindViewById<TextView>(Resource.Id.textView_filter);
            remove = FindViewById<ImageView>(Resource.Id.imageView_remove);

            remove.Click += delegate {
                Visibility = ViewStates.Gone;

                if(filterViewListener!=null)
                    filterViewListener.OnRemove();
            };

            Visibility = ViewStates.Gone;
        }

        public void ShowFilter(FilterModel filterModel) 
        {
            if (filterModel != null)
            {
                Visibility = ViewStates.Visible;
                filter.Text = filterModel.GetLabel();    
            }
            else
            {
                Visibility = ViewStates.Gone;
            }
            
        }

        public interface ICustomFilterViewListener
        {
            void OnRemove();
        }

    }
}
