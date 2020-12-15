using System;
using System.Collections.Generic;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.ChangeRole;
using InformaticaTrentinaPCL.Droid.Utils;
using Com.Bumptech.Glide;

namespace InformaticaTrentinaPCL.Droid.ChangeRole
{
    public class ChangeRoleRecyclerViewAdapter : RecyclerView.Adapter
    {
        List<RuoloInfo> roleList;
        IChangeRoleRecyclerViewItemClickListener listener;

        public override int ItemCount => roleList.Count;

		public interface IChangeRoleRecyclerViewItemClickListener
		{
			void OnClick(View view, int position);
		}

        public ChangeRoleRecyclerViewAdapter(List<RuoloInfo> roleList, IChangeRoleRecyclerViewItemClickListener listener)
        {
            this.roleList = roleList;
            this.listener = listener;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = (RoleViewHolder)holder;

            var assignable = roleList[position];
                
            viewHolder.assignableName.Text = assignable.descrizione;
            
            viewHolder.position = position;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
			var vi = LayoutInflater.From(parent.Context);
			var v = vi.Inflate(Resource.Layout.item_assignable, parent, false);

			return new RoleViewHolder(v, listener);
        }

		protected class RoleViewHolder : RecyclerView.ViewHolder
		{

			public int position;

            public readonly ImageView assignableImage;
            public readonly TextView assignableName;
			public readonly CheckBox favoriteCheckBox;


			public RoleViewHolder(View view, IChangeRoleRecyclerViewItemClickListener listener) : base(view)
			{
				this.ItemView = view;
				assignableImage = ItemView.FindViewById<ImageView>(Resource.Id.imageView_assignableImage);
				assignableName = ItemView.FindViewById<TextView>(Resource.Id.textView_assignableName);
                favoriteCheckBox = ItemView.FindViewById<CheckBox>(Resource.Id.checkbox_favorite);

				assignableImage.Visibility = ViewStates.Gone;
				favoriteCheckBox.Visibility = ViewStates.Gone;
				
				int leftRightPadding = (int)view.Resources.GetDimension(Resource.Dimension
					.cambia_ruolo_left_padding);
			    
				int topBottomPadding = (int)view.Resources.GetDimension(Resource.Dimension
					.item_assignable_top_bottom_padding);
			    

				view.SetPadding(leftRightPadding,topBottomPadding,leftRightPadding,topBottomPadding);
				
                UIUtility.setTextAppearance(view.Context, assignableName, Resource.Style.link_to_page);

				ItemView.Click += (object sender, EventArgs args) => {
					listener.OnClick((View)sender, position);
				};
			}
		}
    }
}
