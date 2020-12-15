using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.Network;

namespace InformaticaTrentinaPCL.Droid.ChooseInstance
{
    public class SelectInstanceRecyclerViewAdapter : RecyclerView.Adapter
    {
        List<InstanceModel> instanceList;

        IInstanceRecyclerViewItemClickListener listener;

        public SelectInstanceRecyclerViewAdapter(List<InstanceModel> instanceList, IInstanceRecyclerViewItemClickListener listener)
        {
            this.instanceList = instanceList;
            this.listener = listener;
        }

        public override int ItemCount => instanceList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = (InstanceViewHolder)holder;

            viewHolder.description.Text = instanceList[position].descrizione;
            viewHolder.item = instanceList[position];
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var vi = LayoutInflater.From(parent.Context);
            var v = vi.Inflate(Resource.Layout.item_administration, parent, false);

            return new InstanceViewHolder(v, listener);
        }

        protected class InstanceViewHolder : RecyclerView.ViewHolder
        {
            public InstanceModel item;

            public readonly TextView description;

            public InstanceViewHolder(View view, IInstanceRecyclerViewItemClickListener listener) : base(view)
            {
                ItemView = view;
                description = ItemView.FindViewById<TextView>(Resource.Id.textView_administration);

                ItemView.Click += (object sender, EventArgs args) => {
                    listener.OnClick(item);
                };

                ItemView.FindViewById(Resource.Id.imageView_removeAdministration).Visibility = ViewStates.Gone;
            }
        }

        public interface IInstanceRecyclerViewItemClickListener
        {
            void OnClick(InstanceModel selectedInstance);
        }
    }
}
