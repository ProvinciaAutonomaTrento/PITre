using System.Collections.Generic;
using System.Text;

using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Content;
using Android.Content;
using Android.Graphics;
using Android.Util;
using InformaticaTrentinaPCL.Droid.Core;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Droid.CustomLoader;
using InformaticaTrentinaPCL.Utils;
using System;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Home;
using Android.Content.Res;
using InformaticaTrentinaPCL.Home.MVP;

namespace InformaticaTrentinaPCL.Droid.Document
{
    public class DocumentRecyclerViewAdapter : RecyclerView.Adapter
    {
        List<AbstractDocumentListItem> documents = null;
        SectionType sectionType;
        IDocumentRecyclerViewItemClickListener documentListener;
        IInfiniteScrollListener infiniteScrollListener;
        IDocumentListPresenter presenter;

        public override int ItemCount => documents == null ? 0 : 
                                         documents.Count == 0 ? 1 : 
                                         documents.Count;

        public DocumentRecyclerViewAdapter(SectionType sectionType, 
                                           IDocumentRecyclerViewItemClickListener documentListener,
                                           IInfiniteScrollListener infiniteScrollListener,
                                           IDocumentListPresenter presenter)
        {
            this.sectionType = sectionType;
            this.documentListener = documentListener;
            this.infiniteScrollListener = infiniteScrollListener;
            this.presenter = presenter;
        }

        public override int GetItemViewType(int position)
        {
            if (documents == null)
            {
                return (int)CellType.FIRST_LOAD;
            }
            
            if (documents.Count == 0)
            {
                return (int)CellType.EMPTY;
            }

            var document = documents[position];

            if (document is DocumentListLoader)
                return (int)CellType.LOADER;
            else 
                return (int)CellType.DOCUMENT;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = GetItem(position);

            if (holder.ItemViewType == (int)CellType.DOCUMENT)
            {
                var isAttachment = presenter.IsAttachment(item);
                var hasFather = presenter.HasDocumentFather(item);
                var hasAttachments = presenter.HasAttachments(item);

                var viewHolder = (DocumentViewHolder)holder;
                viewHolder.position = position;

                StringBuilder dateAndName = new StringBuilder(this.GetDocData(item));
                if (!string.IsNullOrEmpty(item.GetMittente()))
                {
                    dateAndName.Append(", ").Append(item.GetMittente());
                }

                int drawableToShow = isAttachment ? Resource.Drawable.ic_attachment : 
                                           item.tipoDocumento == TypeDocument.DOCUMENTO ? Resource.Drawable.ico_documenti : Resource.Drawable.ico_fascicoli;

                manageSegnatureLabel(viewHolder.segnature, item.getSignatureInfo());
                viewHolder.dateAndName.Text = dateAndName.ToString();
                viewHolder.documentName.Text = item.GetOggetto();
                viewHolder.documentType.Text = string.IsNullOrEmpty(item.GetInfo()) ? "" : item.GetInfo();
                viewHolder.documentImage.Background = ContextCompat.GetDrawable(viewHolder.ItemView.Context, drawableToShow);
                viewHolder.documentExtension.Text = item.GetEstensione();
                SetTextLabelSignType(item, viewHolder);

                if(sectionType == SectionType.SIGN)
                {
                    ConfigureVisibilitySignRejectButtons(item, viewHolder);

                    UIUtility.SetTextColor(viewHolder.rejectButton, item.rejectFlag ? Resource.Color.rejectRed : Resource.Color.textBlack);
                    UIUtility.SetTextColor(viewHolder.signButton, item.signFlag ? Resource.Color.signGreen : Resource.Color.textBlack);

                    viewHolder.ClickButtons(documents);

                    viewHolder.attachmentViewUp.Visibility = (hasFather && isAttachment) ? ViewStates.Visible : ViewStates.Invisible;
                    viewHolder.attachmentViewDown.Visibility = ((hasFather && isAttachment) || hasAttachments) ? ViewStates.Visible : ViewStates.Invisible;
                }

                if (position == documents.Count - InfiniteScrollSettings.ITEMS_BEFORE_END_TO_START_INFINITE_SCROLL)
                {
                    infiniteScrollListener.OnLastDocumentInListShown();
                }
            }
            else if (holder.ItemViewType == (int)CellType.LOADER)
            {
                var viewHolder = (LoaderViewHolder)holder;
                viewHolder.loaderView.startAnimation();
            }
        }

        private string GetDocData(AbstractDocumentListItem item)
        {
            if (item is SignDocumentModel)
            {
                var m = item as SignDocumentModel;
                return m.GetDataToShow();
            }
            return item.GetData();
        }

        /// <summary>
        /// Sets the type of the text label sign.
        /// </summary>
        /// <param name="document">Document.</param>
        /// <param name="holder">Holder.</param>
        private void SetTextLabelSignType(AbstractDocumentListItem document, DocumentViewHolder viewHolder)
        {
            viewHolder.typeSign.Text = documentListener?.GetLabelSignType(document);
        }
       
        private void ConfigureVisibilitySignRejectButtons(AbstractDocumentListItem item, DocumentViewHolder viewHolder)
        {
            if (item is SignDocumentModel)
            {
                var itemSignDocumentModel = (SignDocumentModel)item;

                if (documentListener.IsSignRejectButtonsDisplayed(((SignDocumentModel)item).statoFirma))
                {
                    viewHolder.signButton.Visibility = ViewStates.Visible;
                    viewHolder.rejectButton.Visibility = ViewStates.Visible;
                }
                else
                {
                    viewHolder.signButton.Visibility = ViewStates.Gone;
                    viewHolder.rejectButton.Visibility = ViewStates.Gone;
                }
            }
        }
        private void manageSegnatureLabel(TextView tv, SignatureInfo signatureInfo)
        {
            tv.Visibility = ViewStates.Visible;
            tv.Text = signatureInfo.value;
            tv.SetTextColor(signatureInfo.isSignature ? Color.Rgb(204,0,0) : Color.Black);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            switch (viewType)
            {
                case (int) CellType.EMPTY:
                {
                    View v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.view_empty_list, parent, false);
                    return new EmptyViewViewHolder(v);
                }
                case (int) CellType.DOCUMENT:
                {
                    LinearLayout layout = new LinearLayout(parent.Context);
                        layout.Orientation = Android.Widget.Orientation.Horizontal;
                        layout.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                    return new DocumentViewHolder(parent, layout, documentListener, sectionType);
                }
                case (int) CellType.LOADER:
                {
                    int height = (int) parent.Context.Resources.GetDimension(Resource.Dimension.list_item_height);

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

        public AbstractDocumentListItem GetItem(int index){
            if(index < 0 || documents == null || index >= documents.Count){
                return null;
            }
            return documents[index];
        }

        /// <summary>
        /// Metodo utilizzato per aggiungere items alla recyclerview.
        /// </summary>
        /// <param name="newDocuments">New documents.</param>
        public void addDocuments(List<AbstractDocumentListItem> newDocuments)
        {
            documents = newDocuments;
            NotifyDataSetChanged();
        }

        protected class DocumentViewHolder : RecyclerView.ViewHolder
        {
            public int position;
            IDocumentRecyclerViewItemClickListener listener;
            Context context;
            ViewGroup parent;
            SectionType sectionType;
            public ImageView documentImage;
            public TextView documentExtension;
            public TextView dateAndName;
            public TextView segnature;
            public TextView documentName;
            public TextView documentType;
            public LinearLayout buttonsLayout;
            public TextView signButton;
            public TextView rejectButton;
            public TextView typeSign;
            public ViewGroup cellContainer;
            public View attachmentViewUp;
            public View attachmentViewDown;

            View leftView;
            View rightView;

            public DocumentViewHolder(ViewGroup parent, View view, IDocumentRecyclerViewItemClickListener listener, SectionType sectionType) : base(view)
            {
                this.context = ItemView.Context;
                this.parent = parent;
                this.ItemView = view;
                this.listener = listener;
                this.sectionType = sectionType;
                leftView = GenerateLeftView();
                rightView = GenerateRightView();

                ((LinearLayout)ItemView).AddView(leftView);
                ((LinearLayout)ItemView).AddView(rightView);

            }

            protected View GenerateLeftView()
            {

                var vi = LayoutInflater.From(context);
                var v = vi.Inflate(Resource.Layout.item_document, parent, false);

                documentImage = v.FindViewById<ImageView>(Resource.Id.imageView_documentImage);
                documentExtension = v.FindViewById<TextView>(Resource.Id.textView_documentIconDescription);
                dateAndName = v.FindViewById<TextView>(Resource.Id.textView_date_and_name);
                segnature = v.FindViewById<TextView>(Resource.Id.textview_segnature);
                documentName = v.FindViewById<TextView>(Resource.Id.textView_documentTitle);
                documentType = v.FindViewById<TextView>(Resource.Id.textView_documentType);
                typeSign = v.FindViewById<TextView>(Resource.Id.type_sign);
                cellContainer = v.FindViewById<ViewGroup>(Resource.Id.relativeLayout_firstColumn);
                attachmentViewUp = v.FindViewById<View>(Resource.Id.attachmentViewUp);
                attachmentViewDown = v.FindViewById<View>(Resource.Id.attachmentViewDown);

                if (sectionType == SectionType.SIGN)
                {
                    buttonsLayout = v.FindViewById<LinearLayout>(Resource.Id.linearLayout_buttons);
                    buttonsLayout.Visibility = ViewStates.Visible;
                    signButton = v.FindViewById<TextView>(Resource.Id.textView_sign);
                    rejectButton = v.FindViewById<TextView>(Resource.Id.textView_reject);
                    typeSign.Visibility = ViewStates.Visible;
                }

                v.Click += (sender, e) =>
                {
                    int[] outLocation = { 0, 0, 0, 0 };
                    rightView.GetLocationOnScreen(outLocation);

                    if (outLocation[0] >= context.Resources.DisplayMetrics.WidthPixels && sectionType != SectionType.SIGN)
                        listener.OnItemClick(position);
                };

                return v;
            }

            protected View GenerateRightView()
            {
                Color darkGreyColor = UIUtility.GetColor(context, Resource.Color.textDarkGrey);
                Color whiteColor = UIUtility.GetColor(context, Resource.Color.colorWhite);
                int slideButtonWidth = (int)context.Resources.GetDimension(Resource.Dimension.list_item_slide_button_width);

                FrameLayout.LayoutParams frameParams = new FrameLayout.LayoutParams(slideButtonWidth, ViewGroup.LayoutParams.MatchParent);

                FrameLayout.LayoutParams tvParams = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                tvParams.Gravity = GravityFlags.Center;

                FrameLayout backgroundFrame = new FrameLayout(context);
                backgroundFrame.LayoutParameters = frameParams;
                backgroundFrame.SetBackgroundColor(darkGreyColor);

                TextView tvApri = new TextView(context);
                tvApri.SetText(Resource.String.document_list_open);
                tvApri.SetTextColor(whiteColor);
                tvApri.SetTextSize(ComplexUnitType.Px, context.Resources.GetDimension(Resource.Dimension.item_document_textview_apri));

                backgroundFrame.Click += (sender, e) =>
                {
                    listener.OnOpenClick(position);
                };

                backgroundFrame.AddView(tvApri);

                tvApri.LayoutParameters = tvParams;

                return backgroundFrame;
            }

            public void ClickButtons(List<AbstractDocumentListItem> documents)
            {

                if (!signButton.HasOnClickListeners)
                {
                    signButton.Click += delegate
                    {
                        listener.TapSign(documents[AdapterPosition]);
                    };
                }

                if (!rejectButton.HasOnClickListeners)
                {
                    rejectButton.Click += delegate
                     {
                         listener.TapReject(documents[AdapterPosition]);
                     };
                }
            }
        }

        public void NotifyItemChanged(AbstractDocumentListItem document)
        {
            NotifyItemChanged(documents.IndexOf(document));
        }

        #region Inner classes

        public enum CellType
        {
            DOCUMENT,
            LOADER,
            EMPTY,
            FIRST_LOAD
        }

       
        public interface IDocumentRecyclerViewItemClickListener
        {
            void OnItemClick(int position);
            void OnOpenClick(int position);
            void TapSign(AbstractDocumentListItem document);
            void TapReject(AbstractDocumentListItem document);
            bool IsSignRejectButtonsDisplayed(string statoFirma);
            string GetLabelSignType(AbstractDocumentListItem document);
            string GetDocumentFormatDescr();
        }
        #endregion


    }
}
