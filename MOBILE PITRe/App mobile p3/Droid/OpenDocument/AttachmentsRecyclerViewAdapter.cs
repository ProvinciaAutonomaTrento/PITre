using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.App;
using Android.Widget;
using InformaticaTrentinaPCL.Droid.Core;
using InformaticaTrentinaPCL.Droid.CustomLoader;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Utils;
using InformaticaTrentinaPCL.Droid.Utils;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V4.Content;
using InformaticaTrentinaPCL.OpenFile.MVP;

namespace InformaticaTrentinaPCL.Droid.OpenDocument
{
    public class AttachmentsRecyclerViewAdapter : RecyclerView.Adapter
    {
        private IAttachmentsRecyclerViewItemClickListener attachmentListener;
        private IInfiniteScrollListener infiniteScrollListener;
        private List<AbstractDocumentListItem> attachmentList;
        private string documentId;
        private IOpenDocumentPresenter presenter;

        public override int ItemCount => attachmentList == null ? 0 :
                                         attachmentList.Count == 0 ? 1 :
                                         attachmentList.Count;

        public AttachmentsRecyclerViewAdapter(IAttachmentsRecyclerViewItemClickListener attachmentListener,
                                              IInfiniteScrollListener infiniteScrollListener,
                                              IOpenDocumentPresenter presenter,
                                             string documentId)
        {
            this.attachmentListener = attachmentListener;
            this.infiniteScrollListener = infiniteScrollListener;
            this.documentId = documentId;
            this.presenter = presenter;
        }

        public override int GetItemViewType(int position)
        {
            if (attachmentList == null)
            {
                return (int)CellType.FIRST_LOAD;
            }

            if (attachmentList.Count == 0)
            {
                return (int)CellType.EMPTY;
            }

            var document = attachmentList[position];
            var isCurrentDocument = (documentId != null) && document.GetIdDocumento() == documentId;
            return (int)(document is DocumentListLoader ? CellType.LOADER : isCurrentDocument ? CellType.CURRENT_DOCUMENT : CellType.DOCUMENT);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = GetItem(position);

            switch (holder.ItemViewType)
            {
                case (int)CellType.DOCUMENT:
                case (int)CellType.CURRENT_DOCUMENT:
                    var viewHolder = (AttachmentViewHolder)holder;
                    viewHolder.attachmentName.Text = (presenter != null) ? presenter.GetDocumentTitle(item) : item.GetOggetto();
                    viewHolder.attachment = item;

                    if (viewHolder.ItemViewType == (int)CellType.CURRENT_DOCUMENT)
                    {
                        viewHolder.ItemView.SetBackgroundColor(UIUtility.GetColor(viewHolder.ItemView.Context, Resource.Color.selectedDocumentColor));
                        viewHolder.attachmentName.SetTextColor(Android.Graphics.Color.White);

                        var drawable = ContextCompat.GetDrawable(viewHolder.ItemView.Context, Resource.Drawable.ic_ico_arrow_blu).Mutate();
                        DrawableCompat.SetTint(drawable, Android.Graphics.Color.White.ToArgb());
                        viewHolder.attachmentName.SetCompoundDrawablesWithIntrinsicBounds(null, null, drawable, null);
                    }

                    if (position == attachmentList.Count - InfiniteScrollSettings.ITEMS_BEFORE_END_TO_START_INFINITE_SCROLL)
                    {
                        infiniteScrollListener?.OnLastDocumentInListShown();
                    }
                    break;
                case (int)CellType.LOADER:
                    ((LoaderViewHolder)holder).loaderView.startAnimation();
                    break;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            switch (viewType)
            {
                case (int)CellType.EMPTY:
                    {
                        View v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.view_empty_list, parent, false);
                        return new EmptyViewViewHolder(v);
                    }
                case (int)CellType.DOCUMENT:
                case (int)CellType.CURRENT_DOCUMENT:
                    {
                        var vi = LayoutInflater.From(parent.Context);
                        var v = vi.Inflate(Resource.Layout.item_attachment, parent, false);
                        var viewHolder = new AttachmentViewHolder(v, attachmentListener);
                        return viewHolder;
                    }
                case (int)CellType.LOADER:
                    {
                        int height = (int)parent.Context.Resources.GetDimension(Resource.Dimension.attachment_item_height);

                        FrameLayout.LayoutParams frameParams = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                            ViewGroup.LayoutParams.WrapContent);
                        FrameLayout backgroundFrame = new FrameLayout(parent.Context);
                        backgroundFrame.LayoutParameters = frameParams;
                        backgroundFrame.SetMinimumHeight(height);

                        return new LoaderViewHolder(backgroundFrame);
                    }
            }

            return null;
        }

        public AbstractDocumentListItem GetItem(int index)
        {
            if (index < 0 || attachmentList == null || index >= attachmentList.Count)
            {
                return null;
            }
            return attachmentList[index];
        }

        protected class AttachmentViewHolder : RecyclerView.ViewHolder
        {
            public AbstractDocumentListItem attachment;

            public readonly TextView attachmentName;

            public AttachmentViewHolder(View view, IAttachmentsRecyclerViewItemClickListener listener) : base(view)
            {
                ItemView = view;

                attachmentName = ItemView.FindViewById<TextView>(Resource.Id.textView_attachmentName);
                ItemView.Click += delegate
                {
                    listener.OnOpenClick(attachment);
                };
            }
        }

        public void UpdateDataset(List<AbstractDocumentListItem> list)
        {
            attachmentList = list;
            NotifyDataSetChanged();
        }

        public interface IAttachmentsRecyclerViewItemClickListener
        {
            void OnOpenClick(AbstractDocumentListItem document);
        }

        public enum CellType
        {
            FIRST_LOAD,
            EMPTY,
            DOCUMENT,
            LOADER,
            CURRENT_DOCUMENT
        }
    }
}