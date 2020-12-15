using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Login;

namespace InformaticaTrentinaPCL.Droid.UpdatePassword
{
    public class SelectAdministrationRecyclerViewAdapter : RecyclerView.Adapter
    {
        List<AmministrazioneModel> administrationList;

        IAdministrationRecyclerViewItemClickListener listener;

        public SelectAdministrationRecyclerViewAdapter(List<AmministrazioneModel> administrationList, IAdministrationRecyclerViewItemClickListener listener)
        {
            this.administrationList = administrationList;
            this.listener = listener;
        }

        public override int ItemCount => administrationList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = (AdministrationViewHolder)holder;

            viewHolder.administration.Text = administrationList[position].descrizione;
            viewHolder.item = administrationList[position];
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var vi = LayoutInflater.From(parent.Context);
            var v = vi.Inflate(Resource.Layout.item_administration, parent, false);

            return new AdministrationViewHolder(v, listener);
        }

        protected class AdministrationViewHolder : RecyclerView.ViewHolder
        {
            public AmministrazioneModel item;

            public readonly TextView administration;

            public AdministrationViewHolder(View view, IAdministrationRecyclerViewItemClickListener listener) : base(view)
            {
                ItemView = view;
                administration = ItemView.FindViewById<TextView>(Resource.Id.textView_administration);

                ItemView.Click += (object sender, EventArgs args) => {
                    listener.OnClick(item);
                };

                ItemView.FindViewById(Resource.Id.imageView_removeAdministration).Visibility = ViewStates.Gone;
            }
        }

        public interface IAdministrationRecyclerViewItemClickListener
        {
            void OnClick(AmministrazioneModel selectedAdministration);
        }


    }
}
