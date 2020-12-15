using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Support.Constraints;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Droid.Utils;

namespace InformaticaTrentinaPCL.Droid.Search
{
    public class CustomEditText : ConstraintLayout
    {
        Context mContext;

        EditText searchEditText;
        TextView deleteTextView;

        private string customHint;

        public CustomEditTextListener listener { set; get; }

        public CustomEditText(Context context) : base(context)
        {
            init(context);
        }

        public CustomEditText(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            manageAttrs(context, attrs);
            init(context);
        }

        public CustomEditText(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            manageAttrs(context, attrs);
            init(context);
        }

        private void manageAttrs(Context context, IAttributeSet attrs)
        {
            TypedArray a = context.Theme.ObtainStyledAttributes(
                attrs,
                Resource.Styleable.CustomEditText,
                0, 0);

            try
            {
                customHint = a.GetString(Resource.Styleable.CustomEditText_customHint);
            }
            finally
            {
                a.Recycle();
            }
        }

        void init(Context context)
        {
            mContext = context;

            Inflate(mContext, Resource.Layout.custom_edit_text, this);

            searchEditText = FindViewById<EditText>(Resource.Id.editText_search);
            
            if (!string.IsNullOrEmpty(customHint))
            {
                searchEditText.Hint = customHint;
            }
            
            searchEditText.KeyPress += (object sender, View.KeyEventArgs e) => {
                e.Handled = false;
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    if (listener == null)
                        throw new Exception("MUST set listener before use CustomEditText");

                    listener.OnSearch(searchEditText.Text);
                    e.Handled = true;
                }
            };
            
            searchEditText.TextChanged += delegate
            {
                if (string.IsNullOrEmpty(searchEditText.Text))
                {
                    deleteTextView.SetTextColor(UIUtility.GetColor(mContext, Resource.Color.colorGrey));                    
                }
                else
                {
                    deleteTextView.SetTextColor(UIUtility.GetColor(mContext, Resource.Color.colorBlue));
                }
                
            };

            deleteTextView = FindViewById<TextView>(Resource.Id.button_delete);
            deleteTextView.Click += delegate
            {
                if (listener == null)
                    throw new Exception("MUST set listener before use CustomEditText");

                if (!"".Equals(searchEditText.Text.Trim()))
                {
                    searchEditText.Text = "";
                    listener.OnDelete();
                }
            };
        }

        public void SetEditTextHint(string hint)
        {
            searchEditText.Hint = hint;
        }

        public string GetCurrentSearch()
        {
            return searchEditText.Text;
        }

        public interface CustomEditTextListener 
        {
            void OnSearch(string textToSearch);
            void OnDelete();
        }
    }
}
