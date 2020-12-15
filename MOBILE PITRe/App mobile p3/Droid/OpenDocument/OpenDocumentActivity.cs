using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Droid.Core;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.OpenFile;
using InformaticaTrentinaPCL.OpenFile.MVP;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Droid.OpenDocument
{
    [Activity(Label = "@string/app_name")]
    public class OpenDocumentActivity : BaseActivity, IOpenDocumentView, AttachmentsRecyclerViewAdapter.IAttachmentsRecyclerViewItemClickListener
    {
        public const string KEY_DOCUMENT_ID = "KEY_DOCUMENT_ID";
        public const string KEY_DOCUMENT_BUNDLE = "KEY_DOCUMENT_BUNDLE";

        TextView title;
        TextView mainDocument;
        TextView attachmentsLabel;
        RecyclerView recyclerView;
        string documentId;
        
        CustomLoaderUtility loaderUtility;

        IOpenDocumentPresenter presenter;
        AttachmentsRecyclerViewAdapter adapter;
        
        private AlertDialog stopDownloadDialog;
        
        protected override int LayoutResource => Resource.Layout.activity_open_document;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            documentId = Intent.GetStringExtra(KEY_DOCUMENT_ID);
            string stringSharedDocument = Intent.GetStringExtra(KEY_DOCUMENT_BUNDLE);

            if (string.IsNullOrEmpty(documentId) && string.IsNullOrEmpty(stringSharedDocument))
                throw new Exception("DocumentId or SharedDocumentBundle missing in ActivityOpenDocument invocation");

            title = FindViewById<TextView>(Resource.Id.textView_title);
            mainDocument = FindViewById<TextView>(Resource.Id.textView_mainDocument);
            attachmentsLabel = FindViewById<TextView>(Resource.Id.textView_attachmentsLabel);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            FindViewById(Resource.Id.imageView_close).Click += delegate
            {
                Finish();
            };
            
            loaderUtility = new CustomLoaderUtility();
            
            recyclerView.HasFixedSize = true;
            LinearLayoutManager layoutManager = new LinearLayoutManager(this);
            layoutManager.Orientation = (int)Orientation.Vertical;
            DividerItemDecoration dividerItemDecoration = new DividerItemDecoration(this, layoutManager.Orientation);
            dividerItemDecoration.SetDrawable(Resources.GetDrawable(Resource.Drawable.line_divider_apri_documento));
            recyclerView.AddItemDecoration(dividerItemDecoration);
            recyclerView.SetLayoutManager(layoutManager);

            if (null != documentId)
            {
                presenter = new OpenDocumentPresenter(this,AndroidNativeFactory.Instance(), documentId);
            }
            else
            {
                JsonConverter[] converters = { new AbstractDocumentListItemJsonConverter()};
                string sharedDocumentRaw = Intent.GetStringExtra(KEY_DOCUMENT_BUNDLE);
                SharedDocumentBundle sharedDocument = JsonConvert.DeserializeObject<SharedDocumentBundle>(sharedDocumentRaw, 
                    new JsonSerializerSettings() { Converters = converters });
                presenter =  new OpenDocumentPresenter(this,AndroidNativeFactory.Instance(),sharedDocument);
            }

            adapter = new AttachmentsRecyclerViewAdapter(this, null, presenter, documentId);
            recyclerView.SetAdapter(adapter);
        }
        
        protected override void OnStart()
        {
            base.OnStart();
            presenter.OnViewReady();
        }

        #region IOpenDocumentView
        public void ShowError(string e, bool isLight)
        {
            ShowErrorHelper.Show(this, e, isLight, OnBackPressed);
        }

        public void OnUpdateLoader(bool isShow)
        {
            if (isShow)
            {
                loaderUtility.showLoader(this);
            }
            else
            {
                loaderUtility.hideLoader();
            }
        }
        
        public void OnUpdateLoaderWithAction(bool isShow)
        {
            if (isShow)
            {
                loaderUtility.showLoader(this, showConfirmDialog);
            }
            else
            {
                if (stopDownloadDialog != null && stopDownloadDialog.IsShowing)
                {
                    stopDownloadDialog.Dismiss();
                    stopDownloadDialog = null;
                }
                
                loaderUtility.hideLoader();
            }
        }
 
        private void showConfirmDialog()
        {
            stopDownloadDialog = new AlertDialog.Builder(this)
                .SetTitle(LocalizedString.TITLE_ALERT.Get())
                .SetMessage(LocalizedString.CANCEL_DOWNLOAD.Get())
                .SetPositiveButton(LocalizedString.DONE_ACTION.Get(), delegate { presenter.CancelDownload(); })
                .SetNegativeButton(LocalizedString.CANCEL_ACTION.Get(), delegate { loaderUtility.showLoader(this, showConfirmDialog); })
                .Create();

            stopDownloadDialog.Show();
        }

        public void ShowList(List<AbstractDocumentListItem> list)
        {
            if (list != null && list.Count > 0)
            {
                if (documentId == list[0].GetIdDocumento())
                {
                    ((View)mainDocument.Parent).SetBackgroundColor(UIUtility.GetColor(this, Resource.Color.selectedDocumentColor));
                    mainDocument.SetTextColor(Android.Graphics.Color.White);
                    var drawable = ContextCompat.GetDrawable(this, Resource.Drawable.ic_ico_arrow_blu).Mutate();
                    DrawableCompat.SetTint(drawable, Android.Graphics.Color.White.ToArgb());
                    mainDocument.SetCompoundDrawablesWithIntrinsicBounds(null, null, drawable, null);
                }
                mainDocument.Text = presenter.GetDocumentTitle(list[0]);
                mainDocument.Click += delegate{ OnOpenClick(list[0]); };
                adapter.UpdateDataset(list.GetRange(1, list.Count - 1));

                if (HasAttachments(list))
                {
                    attachmentsLabel.Visibility = ViewStates.Visible;
                    recyclerView.Visibility = ViewStates.Visible;
                }
                else
                {
                    attachmentsLabel.Visibility = ViewStates.Gone;
                    recyclerView.Visibility = ViewStates.Gone;
                }
            }
            
        }

        private bool HasAttachments(List<AbstractDocumentListItem> list)
        {
            return list.Count > 1;
        }

        #endregion

        #region AttachmentsRecyclerViewAdapter.IAttachmentsRecyclerViewItemClickListener

        public void OnOpenClick(AbstractDocumentListItem document)
        {
            presenter.OnSelect(document);
        }

        #endregion
        
    }
}