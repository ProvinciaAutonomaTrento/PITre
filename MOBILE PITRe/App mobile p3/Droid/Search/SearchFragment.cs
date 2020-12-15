
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Droid.Core;
using InformaticaTrentinaPCL.Droid.Delegation;
using InformaticaTrentinaPCL.Droid.Document;
using InformaticaTrentinaPCL.Droid.Filter;
using InformaticaTrentinaPCL.Droid.Home;
using InformaticaTrentinaPCL.Droid.OpenDocument;
using InformaticaTrentinaPCL.Droid.OpenDossier;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Droid.Utils.SwipeRecyclerView;
using InformaticaTrentinaPCL.Filter;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.ActionDialog;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.OpenFile;
using InformaticaTrentinaPCL.Search;
using InformaticaTrentinaPCL.Search.MVP;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Droid.Search
{
    public class SearchFragment : AbstractListFragment, ISearchView, IInfiniteScrollListener,
                                  DocumentRecyclerViewAdapter.IDocumentRecyclerViewItemClickListener, CustomEditText.CustomEditTextListener
    {

        SearchNativePresenter presenter;

        CustomLoaderUtility loader;

        CustomEditText customEditText;

        RecyclerSwipeListView documentList;
        
        DocumentRecyclerViewAdapter documentRecyclerViewAdapter;

        string textToSearch;

        public static SearchFragment CreateInstance()
        {
            Bundle bundle = new Bundle();
            SearchFragment fragment = new SearchFragment();
            fragment.Arguments = bundle;
            return fragment;
        }


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (this.Arguments != null && !this.Arguments.IsEmpty)
            {
            }
            presenter = new SearchNativePresenter(this, AndroidNativeFactory.Instance());
            RetainInstance = true;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutAll(presenter.GetStateToSave());
            base.OnSaveInstanceState(outState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_search, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            loader = new CustomLoaderUtility();

            documentList = View.FindViewById<RecyclerSwipeListView>(Resource.Id.recyclerView_documentList);
            documentList.HasFixedSize = true;
            LinearLayoutManager layoutManager = new LinearLayoutManager(Activity);
            layoutManager.Orientation = (int)Orientation.Vertical;
            DividerItemDecoration dividerItemDecoration = new DividerItemDecoration(documentList.Context, layoutManager.Orientation);
            documentList.AddItemDecoration(dividerItemDecoration);
            dividerItemDecoration.SetDrawable(Resources.GetDrawable(Resource.Drawable.line_divider));
            documentList.SetLayoutManager(layoutManager);
            documentRecyclerViewAdapter = new DocumentRecyclerViewAdapter(SectionType.SEARCH, this, this, presenter);
            documentList.SetAdapter(documentRecyclerViewAdapter);
            
            customEditText = View.FindViewById<CustomEditText>(Resource.Id.customEditText);
            customEditText.listener = this;

            string userID = MainApplication.GetSessionData().userInfo.idPeople;

            Activity.Window.SetSoftInputMode(SoftInput.AdjustPan);
            
            presenter.OnRestoreInstanceState(savedInstanceState);

        }
        
        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            
            if ((resultCode == Result.Ok) && (requestCode == (int)ActivityRequestCodes.SelectDelegate))
            {

                AbstractRecipient assignee = AbstractRecipientHelper.DeserializeAbstractRecipient(
                    data.GetStringExtra(SelectDelegateActivity.SELECTED_DELEGATE_KEY));

                presenter.ShareDocument(presenter.AbstractDocumentToShare, assignee);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            presenter.Dispose();
        }

        #region CustomEditText.CustomAdministrationViewListener
        public void OnSearch(string textToSearch)
        {
            this.textToSearch = textToSearch;
            presenter.UpdateSearchString(this.textToSearch);
        }

        public void OnDelete()
        {
            presenter.ClearSearch();
        }
        #endregion

        #region AbstractListFragment
        public override void reloadData()
        {

        }
        
        public override void RequestOpenFilterView()
        {
            presenter.OpenFilterView();
        }
        
        public override void SetFilter(FilterModel filterModel)
        {
            presenter.SetFilterModel(filterModel);
        }
        #endregion

        #region DocumentRecyclerViewAdapter.IDocumentRecyclerViewItemClickListener
        public void OnItemClick(int position)
        {
            presenter.OnTapDocument(documentRecyclerViewAdapter.GetItem(position));
        }

        public void OnOpenClick(int position)
        {
            documentList.resetSwipe();
            presenter.OnOpenDocument(documentRecyclerViewAdapter.GetItem(position));
        }

        public void OnLastDocumentInListShown()
        {
            presenter.DoNextResultPage();
        }
        #endregion

        #region ISearchView
        public void UpdateList(List<AbstractDocumentListItem> documents)
        {
            documentList.resetSwipe();
            documentRecyclerViewAdapter.addDocuments(documents);
        }

        public void DoOpenFirma(AbstractDocumentListItem abstractDocument)
        {
            StartActivityForResult(((HomeActivity)Activity).StartSignActivity(abstractDocument, SectionType.SEARCH), (int)ActivityRequestCodes.Sign);
        }

        public void DoOpenDialog(List<DialogItem> itemList)
        {
            ((HomeActivity) Activity).ShowBottomSheetDialog(itemList);
        }

        public void DoAccetta(AbstractDocumentListItem abstractDocument)
        {
            ((HomeActivity)Activity).startCommonActionActivity(ActionType.ACCEPT.id, JsonConvert.SerializeObject(abstractDocument), SectionType.SEARCH);
        }

        public void DoAccettaEADL(AbstractDocumentListItem abstractDocument)
        {
            ((HomeActivity)Activity).startCommonActionActivity(ActionType.ACCEPT_AND_ADL.id, JsonConvert.SerializeObject(abstractDocument), SectionType.SEARCH);
        }

        public void DoRifiuta(AbstractDocumentListItem abstractDocument)
        {
            ((HomeActivity)Activity).startCommonActionActivity(ActionType.REFUSE.id, JsonConvert.SerializeObject(abstractDocument), SectionType.SEARCH);
        }

        public void DoApriDocumento(AbstractDocumentListItem abstractDocument)
        {
            Intent intent = new Intent(Activity, typeof(OpenDocumentActivity));
            intent.PutExtra(OpenDocumentActivity.KEY_DOCUMENT_ID, abstractDocument.GetIdDocumento());
            Activity.StartActivity(intent);   
        }
        
        public void DoApriFascicolo(AbstractDocumentListItem abstractDocument)
        {
            Intent intent = new Intent(Activity, typeof(OpenDossierActivity));
            intent.PutExtra(OpenDossierActivity.KEY_DOSSIER, JsonConvert.SerializeObject(abstractDocument));
            Activity.StartActivity(intent); 
        }

        public void DoRimuoviDaADL(AbstractDocumentListItem abstractDocument)
        {
            presenter.RemoveADLAction(abstractDocument);
        }
        
        public void DoCondividi(AbstractDocumentListItem abstractDocument)
        {
            presenter.AbstractDocumentToShare = abstractDocument;
            
            Intent intent = new Intent(Activity, typeof(SelectDelegateActivity));
            intent.PutExtra(SelectDelegateActivity.TITLE_KEY,
                Activity.GetString(Resource.String.activity_select_delegate_share_with));
            StartActivityForResult(intent, (int)ActivityRequestCodes.SelectDelegate);
        }
        
        public void DoAssegna(AbstractDocumentListItem abstractDocument)
        {
            ((HomeActivity) Activity).startAssignActivity(ActionType.ASSIGN.id, JsonConvert.SerializeObject(abstractDocument));
        }

        public void DoInserisciInADL(AbstractDocumentListItem abstractDocument)
        {
            presenter.AddADLAction(abstractDocument);
        }

        public void OnRemoveFromADLOk(string message)
        {
            Toast.MakeText(Activity, message, ToastLength.Short).Show();
        }

        public void ShowError(string e, bool isLight)
        {
            ShowErrorHelper.Show(this.Activity, e, isLight);
        }

        public void OnUpdateLoader(bool isShow)
        {
            if (isShow)
            {
                loader.showLoader(Activity);
            }
            else
            {
                loader.hideLoader();
            }
        }

        public void OnShareLinkReady(string link)
        {
            ((HomeActivity) Activity).OpenShareView(link);
        }
        
        public void DoApriDocumentoCondiviso(SharedDocumentBundle sharedDocument)
        {
            Toast.MakeText(Activity, "DoApriDocumentoCondiviso", ToastLength.Short).Show();
        }
        
        #endregion

        #region UPDATE FILTER implementation
        public void UpdateFilterUI(FilterModel filterModel)
        {
            ((HomeActivity) Activity).UpdateFilterView(filterModel);
        }

        public void OpenFilterView(FilterModel filterModel)
        {
            Intent intent = new Intent(Activity, typeof(FilterActivity));

            Bundle bundle = new Bundle();

            if (filterModel != null)
            {
                bundle.PutString(FilterActivity.KEY_FILTER, JsonConvert.SerializeObject(filterModel));
            }
            bundle.PutInt(FilterActivity.KEY_SECTION_TYPE_INDEX, (int)SectionType.SEARCH);
            intent.PutExtras(bundle);
            Activity.StartActivityForResult(intent, (int)ActivityRequestCodes.Filter);
        }

        public void OnAddInADLOk(string message)
        {
            Toast.MakeText(Activity, message, ToastLength.Short).Show();
        }

        public void DoViewed(AbstractDocumentListItem abstractDocument)
        {
            presenter.DoViewed(abstractDocument);
        }

        public void DoViewedADL(AbstractDocumentListItem abstractDocument)
        {
            presenter.DoViewedADL(abstractDocument);
        }

        public void OnViewedOk(string message)
        {
            presenter.OnPullToRefresh();
            Toast.MakeText(Activity, message, ToastLength.Short).Show();
        }

        public void TapSign(AbstractDocumentListItem document)
        {
            return;
        }

        public void TapReject(AbstractDocumentListItem document)
        {
            return;
        }

        public void DoSignAll()
        {
            throw new NotImplementedException();
        }

        public void DoRejectAll()
        {
            throw new NotImplementedException();
        }

        public void CompletedActionOK(Dictionary<string, string> extra)
        {
            throw new NotImplementedException();
        }

        public void DoOpenViewOtp(List<AbstractDocumentListItem> listStateDaFirmareOTP, int TotalRecordCountSigned)
        {
            throw new NotImplementedException();
        }

        public bool IsSignRejectButtonsDisplayed(string statoFirma)
        {
            throw new NotImplementedException();
        }

        public string GetLabelSignType(AbstractDocumentListItem document)
        {
            return presenter.GetLabelSignTypeDescr(document);
        }

        public void DoSignAllCades()
        {
            throw new NotImplementedException();
        }

        public void DoSignAllPades()
        {
            throw new NotImplementedException();
        }

        public string GetDocumentFormatDescr()
        {
            return presenter.GetDocumentFormatDescr();
        }

        public void DoSignAllSequentially()
        {
            throw new NotImplementedException();
        }

        public void DoOpenViewOtpCades(List<AbstractDocumentListItem> listStateDaFirmareOTP, int TotalRecordCountSigned)
        {
            throw new NotImplementedException();
        }

        public void DoOpenViewOtpPades(List<AbstractDocumentListItem> listStateDaFirmareOTP, int TotalRecordCountSigned)
        {
            throw new NotImplementedException();
        }

        public void ShowAlertWithNumberDocuments(string description, string actionType)
        {
            throw new NotImplementedException();
        }

        public void OnSignCompleted(string message)
        {
            throw new NotImplementedException();
        }

        public void OnDocumentUpdated(AbstractDocumentListItem document)
        {
            throw new NotImplementedException();
        }

        public void OnViewedADLOk(string message)
        {
            presenter.OnPullToRefresh();
            Toast.MakeText(Activity, message, ToastLength.Short).Show();
        }

        public void DoOpenViewOtpCadesNotSigned(List<AbstractDocumentListItem> listStateDaFirmareOTP, int TotalRecordCountSigned)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
