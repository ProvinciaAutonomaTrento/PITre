using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace InformaticaTrentinaPCL.Droid.Core
{
    public class EmptyViewViewHolder : RecyclerView.ViewHolder
    {
        public readonly TextView message;
        
        public EmptyViewViewHolder(View view) : base(view)
        {
            message = view.FindViewById<TextView>(Resource.Id.textView_no_results);
        }
    }
}