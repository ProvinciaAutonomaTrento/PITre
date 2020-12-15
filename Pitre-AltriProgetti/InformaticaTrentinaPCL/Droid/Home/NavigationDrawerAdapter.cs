using System;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Content.Res;

namespace InformaticaTrentinaPCL.Droid.Home
{
    public class NavigationDrawerAdapter : RecyclerView.Adapter
    {
        public override int ItemCount => menuEntries.Length; 

        String[] menuEntries;
        TypedArray menuIcons;
        INavigationDrawerItemClickListener onItemClickListener;

        public enum NavigationDrawerActions
        {
            Logout
        }
		
        public interface INavigationDrawerItemClickListener 
        {
            void OnClick(NavigationDrawerActions action);
		}


        public NavigationDrawerAdapter(string[] menuEntries, TypedArray menuIcons, INavigationDrawerItemClickListener listener)
		{
            this.menuEntries = menuEntries;
            this.menuIcons = menuIcons;
            this.onItemClickListener = listener;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var vi = LayoutInflater.From(parent.Context);
			var v = vi.Inflate(Resource.Layout.item_navigation_drawer, parent, false);

            return new NavigationDrawerItemViewHolder(v, onItemClickListener);
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var viewHolder = (NavigationDrawerItemViewHolder)holder;

            viewHolder.menuDrawerItemTextView.Text = menuEntries[position];
            viewHolder.menuDrawerItemImageView.SetImageResource(menuIcons.GetResourceId(position, -1));
			viewHolder.action = NavigationDrawerActions.Logout;
		}

		public class NavigationDrawerItemViewHolder : RecyclerView.ViewHolder
		{
            public NavigationDrawerActions action;
			public readonly ImageView menuDrawerItemImageView;
			public readonly TextView menuDrawerItemTextView;

			public NavigationDrawerItemViewHolder(View view, INavigationDrawerItemClickListener listener) : base(view)
			{
				this.ItemView = view;
				menuDrawerItemImageView = ItemView.FindViewById<ImageView>(Resource.Id.imageView_menuDrawerItem);
				menuDrawerItemTextView = ItemView.FindViewById<TextView>(Resource.Id.textView_menuDrawerItem);

				this.ItemView.Click += (object sender, EventArgs args) => {
                    listener.OnClick(action);
				};
			}
		}
    }
}
