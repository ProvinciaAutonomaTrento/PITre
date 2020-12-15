using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Home;

namespace InformaticaTrentinaPCL.Droid.Search
{
    public class SearchDocumentRecyclerViewAdapter : RecyclerView.Adapter
    {
        List<RicercaDocumentModel> documentList;
        ISearchDocumentRecyclerViewItemClickListener listener;

       
        public interface ISearchDocumentRecyclerViewItemClickListener
        {
            void OnSearchDocumentItemClick(View view, int position);
        }

        public override int ItemCount => documentList.Count;

        public SearchDocumentRecyclerViewAdapter(List<RicercaDocumentModel> documentList, ISearchDocumentRecyclerViewItemClickListener listener)
        {
            this.documentList = documentList;
            this.listener = listener;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = (DocumentViewHolder)holder;

            var document = documentList[position];
            viewHolder.documentName.Text = document.segnatura;
            viewHolder.position = position;       
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var vi = LayoutInflater.From(parent.Context);
            var v = vi.Inflate(Resource.Layout.item_search_list_document, parent, false);

            return new DocumentViewHolder(v, listener);
        }

        protected class DocumentViewHolder : RecyclerView.ViewHolder
        {

            public int position;

            public readonly TextView documentName;

            public DocumentViewHolder(View view, ISearchDocumentRecyclerViewItemClickListener listener) : base(view)
            {
                this.ItemView = view;
                documentName = ItemView.FindViewById<TextView>(Resource.Id.textView_documentName);

                ItemView.Click += (object sender, EventArgs args) => {
                    listener.OnSearchDocumentItemClick((View)sender, position);
                };
            }
        }
    }
}
