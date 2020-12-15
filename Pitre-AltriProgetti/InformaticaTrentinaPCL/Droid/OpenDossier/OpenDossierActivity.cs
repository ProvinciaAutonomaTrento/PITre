using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Widget;
using InformaticaTrentinaPCL.Droid.Core;
using InformaticaTrentinaPCL.Droid.OpenDocument;
using InformaticaTrentinaPCL.Droid.Search;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.OpenFile;
using InformaticaTrentinaPCL.OpenFile.MVP;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Droid.OpenDossier
{
    [Activity(Label = "@string/app_name")]
    public class OpenDossierActivity : BaseActivity, IOpenFascView, CustomEditText.CustomEditTextListener, 
                                       IInfiniteScrollListener, AttachmentsRecyclerViewAdapter.IAttachmentsRecyclerViewItemClickListener
    {
        public const string KEY_DOSSIER = "KEY_DOSSIER";

        CustomLoaderUtility loaderUtility;
        RecyclerView recyclerView;

        private AttachmentsRecyclerViewAdapter adapter;
        IOpenFascPresenter presenter;

        private AlertDialog stopDownloadDialog;

        protected override int LayoutResource => Resource.Layout.activity_open_dossier;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            string dossier = Intent.GetStringExtra(KEY_DOSSIER);
            
            if (string.IsNullOrEmpty(dossier))
                throw new Exception("Dossier missing in OpenDossierActivity invocation");

            AbstractDocumentListItem item =  AbstractDocumentListItemHelper.DeserializeAbstractDocumentListItem(dossier);
            
            loaderUtility = new CustomLoaderUtility();
            
            FindViewById(Resource.Id.imageView_close).Click += delegate
            {
                Finish();
            };
            
            CustomEditText customEditText = FindViewById<CustomEditText>(Resource.Id.customEditText);
            customEditText.SetEditTextHint(GetString(Resource.String.activity_open_dossier_search_in) + " " + item.GetOggetto());
            customEditText.listener = this;

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.HasFixedSize = true;
            LinearLayoutManager layoutManager = new LinearLayoutManager(this);
            layoutManager.Orientation = (int)Orientation.Vertical;
            DividerItemDecoration dividerItemDecoration = new DividerItemDecoration(this, layoutManager.Orientation);
            dividerItemDecoration.SetDrawable(Resources.GetDrawable(Resource.Drawable.line_divider_apri_documento));
            recyclerView.AddItemDecoration(dividerItemDecoration);
            recyclerView.SetLayoutManager(layoutManager);

            adapter = new AttachmentsRecyclerViewAdapter(this, this, null, null);
            recyclerView.SetAdapter(adapter);
            presenter = new OpenFascPresenter(this,AndroidNativeFactory.Instance(), item.GetIdDocumento());
            presenter.LoadFileList();
        }

        #region IOpenFascView

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

            //Toast.MakeText(this, "PROVAVASVAVSAV", ToastLength.Long).Show();
        }

        public void ShowList(List<AbstractDocumentListItem> list)
        {
            adapter.UpdateDataset(list);
        }

        #endregion

        #region CustomEditText.CustomEditTextListener

        public void OnSearch(string textToSearch)
        {
            presenter.OnSearch(textToSearch);
        }

        public void OnDelete()
        {
            presenter.ClearSearch();
        }

        #endregion

        #region AttachmentsRecyclerViewAdapter.IAttachmentsRecyclerViewItemClickListener

        public void OnOpenClick(AbstractDocumentListItem document)
        {
            presenter.OnSelect(document);
        }

        #endregion

        public void OpenDocumentBundle(SharedDocumentBundle sharedDocument)
        {
            Intent intent = new Intent(this, typeof(OpenDocumentActivity));
            intent.PutExtra(OpenDocumentActivity.KEY_DOCUMENT_BUNDLE, JsonConvert.SerializeObject(sharedDocument));
            StartActivity(intent);
        }
        
        public void OnLastDocumentInListShown()
        {
            presenter.DoNextPage();
        }
    }
}