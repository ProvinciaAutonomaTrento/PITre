//using System;
//using System.Collections.Generic;
//using Android.Support.V7.Widget;
//using Android.Views;
//using Android.Widget;
//using InformaticaTrentinaPCL.Assegna;
//using InformaticaTrentinaPCL.Assign;
//using InformaticaTrentinaPCL.Droid.Utils;
//using InformaticaTrentinaPCL.Interfaces;
//using Square.Picasso;
//
//namespace InformaticaTrentinaPCL.Droid.Assign.Assignable
//{
//    public class AssignableRecyclerViewAdapter : RecyclerView.Adapter
//    {
//        List<AbstractRecipient> assignables;
//        IAssignableRecyclerViewItemClickListener listener;
//
//        public override int ItemCount => assignables.Count;
//
//		public interface IAssignableRecyclerViewItemClickListener
//		{
//			void OnClick(View view, AbstractRecipient selectedAssignee);
//		}
//
//        public AssignableRecyclerViewAdapter(List<AbstractRecipient> assignables, IAssignableRecyclerViewItemClickListener listener)
//        {
//            this.assignables = assignables;
//            this.listener = listener;
//        }
//
//        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
//        {
//            var viewHolder = (AssignableViewHolder)holder;
//
//            var assignable = assignables[position];
//
//            if (assignable.getType() ==  AbstractRecipient.RecipientType.User )
//            {
//                viewHolder.favoriteCheckBox.Visibility = ViewStates.Visible;
//
////				if (assignables[position].ImageUrl != null)
////                {
////                    Picasso.With(viewHolder.ItemView.Context)
////                           .Load(assignable.ImageUrl)
////                           .Resize(50, 50)
////                           .Transform(new Home.PicassoCircleTransformation())
////                           .Into(viewHolder.assignableImage);
////                }
////                else
////                {
//                    viewHolder.assignableImage.Visibility = ViewStates.Gone;
////                }
//            } else {
//				viewHolder.assignableImage.Visibility = ViewStates.Gone;
//				viewHolder.favoriteCheckBox.Visibility = ViewStates.Gone;
//            }
//
//	        if (!string.IsNullOrEmpty(assignable.getTitle()))
//	        {
//		        viewHolder.assignableName.Text = assignable.getTitle();
//	        }
//            else if (!string.IsNullOrEmpty(assignables[position].getSubtitle()))
//	        {
//		        viewHolder.assignableName.Text = assignable.getSubtitle();
//	        }
//            
//            viewHolder.assignee = assignables[position];
//        }
//
//        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
//        {
//			var vi = LayoutInflater.From(parent.Context);
//			var v = vi.Inflate(Resource.Layout.item_assignable, parent, false);
//
//			return new AssignableViewHolder(v, listener);
//        }
//
//		protected class AssignableViewHolder : RecyclerView.ViewHolder
//		{
//
//			public AbstractRecipient assignee;
//
//            public readonly ImageView assignableImage;
//            public readonly TextView assignableName;
//			public readonly CheckBox favoriteCheckBox;
//
//			public AssignableViewHolder(View view, IAssignableRecyclerViewItemClickListener listener) : base(view)
//			{
//				this.ItemView = view;
//				assignableImage = ItemView.FindViewById<ImageView>(Resource.Id.imageView_assignableImage);
//				assignableName = ItemView.FindViewById<TextView>(Resource.Id.textView_assignableName);
//				favoriteCheckBox = ItemView.FindViewById<CheckBox>(Resource.Id.checkbox_favorite);
//
//                UIUtility.setTextAppearance(view.Context, assignableName, Resource.Style.link_to_page);
//
//				ItemView.Click += (object sender, EventArgs args) => {
//					listener.OnClick((View)sender, assignee);
//				};
//			}
//		}
//    }
//}
