using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Droid.Document;
using InformaticaTrentinaPCL.Droid.Assign;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.MVP;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Droid.Utils.SwipeRecyclerView;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Home.MVPD;
using Android.Support.Constraints;
using InformaticaTrentinaPCL.Droid.Core;
using Android.Support.V4.Widget;
using InformaticaTrentinaPCL.Droid.Delegation;
using InformaticaTrentinaPCL.Filter;
using InformaticaTrentinaPCL.Home.ActionDialog;
using InformaticaTrentinaPCL.Droid.Filter;
using InformaticaTrentinaPCL.Droid.OpenDocument;
using InformaticaTrentinaPCL.Droid.OpenDossier;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.OpenFile;
using Newtonsoft.Json;
using InformaticaTrentinaPCL.OpenFile;
using InformaticaTrentinaPCL.Utils;
using InformaticaTrentinaPCL.Droid.ActionCompleted;
using InformaticaTrentinaPCL.Droid.Sign;
using InformaticaTrentinaPCL.Network;
using Android.Support.Design.Widget;

namespace InformaticaTrentinaPCL.Droid.Home
{
    public class DocumentListFragment : AbstractListFragment, IDocumentListView, IInfiniteScrollListener,
                                        DocumentRecyclerViewAdapter.IDocumentRecyclerViewItemClickListener
    {
        private static string KEY_SECTION_TYPE_INDEX = "KEY_SECTION_TYPE_INDEX";
        public static DocumentListFragment CreateInstance(SectionType sectionType)
        {
            Bundle bundle = new Bundle();
            bundle.PutInt(KEY_SECTION_TYPE_INDEX, (int)sectionType);
            DocumentListFragment fragment = new DocumentListFragment();
            fragment.Arguments = bundle;
            return fragment;
        }

        RecyclerSwipeListView documentsRecyclerView;

        DocumentRecyclerViewAdapter documentRecyclerViewAdapter;
        SwipeRefreshLayout swipeRefreshLayout;

        CustomLoaderUtility loaderUtility;

        private SectionType sectionType;
        private DocumentListNativePresenter presenter;

        public DocumentListFragment() { }

        public override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (this.Arguments != null && !this.Arguments.IsEmpty)
            {
                int sectionTypeIndex = this.Arguments.GetInt(KEY_SECTION_TYPE_INDEX, 0);
                sectionType = (SectionType)Enum.ToObject(typeof(SectionType), sectionTypeIndex);
            }
            presenter = new DocumentListNativePresenter(this, AndroidNativeFactory.Instance(), this.sectionType);
            RetainInstance = true;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutAll(presenter.GetStateToSave());
            base.OnSaveInstanceState(outState);
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_documentList, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            loaderUtility = new CustomLoaderUtility();

            documentsRecyclerView = View.FindViewById<RecyclerSwipeListView>(Resource.Id.recyclerView_documentList);
            documentsRecyclerView.HasFixedSize = true;
            LinearLayoutManager layoutManager = new LinearLayoutManager(Activity);
            layoutManager.Orientation = (int)Orientation.Vertical;
            DividerItemDecoration dividerItemDecoration = new DividerItemDecoration(documentsRecyclerView.Context, layoutManager.Orientation);
            documentsRecyclerView.AddItemDecoration(dividerItemDecoration);
            dividerItemDecoration.SetDrawable(Resources.GetDrawable(Resource.Drawable.line_divider));
            documentsRecyclerView.SetLayoutManager(layoutManager);
            documentRecyclerViewAdapter = new DocumentRecyclerViewAdapter(sectionType, this, this, presenter);
            documentsRecyclerView.SetAdapter(documentRecyclerViewAdapter);

            swipeRefreshLayout = View.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetOnRefreshListener(new PullToRefreshListener(this));

            string userID = MainApplication.GetSessionData().userInfo.idPeople;

            presenter.OnRestoreInstanceState(savedInstanceState);
            presenter.LoadDocumentList();

            if (!string.IsNullOrEmpty(MainApplication.GetSessionData().urlToOpen))
            {
                presenter.OnShareDocumentReceived(MainApplication.GetSessionData().urlToOpen);
                MainApplication.GetSessionData().urlToOpen = null;
            }
        }

        public void OpenURL(string urlToOpen)
        {
            presenter.OnShareDocumentReceived(urlToOpen);
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            switch (requestCode)
            {
                case (int)ActivityRequestCodes.SelectDelegate:
                    if (resultCode == Result.Ok)
                    {
                        AbstractRecipient assignee = AbstractRecipientHelper.DeserializeAbstractRecipient(
                            data.GetStringExtra(SelectDelegateActivity.SELECTED_DELEGATE_KEY));

                        presenter.ShareDocument(presenter.AbstractDocumentToShare, assignee);
                    }
                    break;

                case (int)ActivityRequestCodes.Sign:
                    if (data != null && data.HasExtra(SignActivity.KEY_SIGN_TYPE) && resultCode == Result.Ok)
                    {
                        var signType = data.GetStringExtra(SignActivity.KEY_SIGN_TYPE);
                        var success = resultCode == Result.Ok;
                        switch (signType)
                        {
                            case Constants.ACTION_SIGN_CADES:
                                presenter.OnDocumentsSignCadesActionFinished(success);
                                break;
                            case Constants.ACTION_SIGN_CADES_NOT_SIGNED:
                                presenter.OnDocumentsSignCadesNotSignedActionFinished(success);
                                break;
                            case Constants.ACTION_SIGN_PADES:
                                presenter.OnDocumentsSignPadesActionFinished(success);
                                break;
                        }
                    }
                    break;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            presenter.Dispose();
        }

        #region IGeneralView
        public void ShowError(string e, bool isLight)
        {
            ShowErrorHelper.Show(this.Activity, e, isLight);
        }

        public void OnUpdateLoader(bool isShow)
        {
            if (isShow)
            {
                loaderUtility.showLoader(Activity);
            }
            else
            {
                loaderUtility.hideLoader();
            }
        }
        #endregion


        #region DocumentRecyclerViewAdapter.IDocumentRecyclerViewItemClickListener
        public void OnItemClick(int position)
        {
            presenter.OnTapDocument(documentRecyclerViewAdapter.GetItem(position));
        }

        public void OnOpenClick(int position)
        {
            documentsRecyclerView.resetSwipe();
            if ((sectionType == SectionType.TODO) || (sectionType == SectionType.ADL) || (sectionType == SectionType.SIGN))
            {
                presenter.OnOpenDocument(documentRecyclerViewAdapter.GetItem(position));
            }
        }

        public void OnLastDocumentInListShown()
        {
            presenter.LoadDocumentList();
        }
        #endregion

        #region IDocumentListView
        public void UpdateList(List<AbstractDocumentListItem> documents)
        {
            documentsRecyclerView.resetSwipe();
            documentRecyclerViewAdapter.addDocuments(documents);

            swipeRefreshLayout.Refreshing = false;
        }

        public void DoOpenFirma(AbstractDocumentListItem abstractDocument)
        {
            StartActivityForResult(((HomeActivity)Activity).StartSignActivity(abstractDocument, sectionType), (int)ActivityRequestCodes.Sign);
        }

        public void DoOpenDialog(List<DialogItem> itemList)
        {
            ((HomeActivity)Activity).ShowBottomSheetDialog(itemList);
        }

        public void DoAccetta(AbstractDocumentListItem abstractDocument)
        {
            ((HomeActivity)Activity).startCommonActionActivity(ActionType.ACCEPT.id, JsonConvert.SerializeObject(abstractDocument), sectionType);
        }

        public void DoAccettaEADL(AbstractDocumentListItem abstractDocument)
        {
            ((HomeActivity)Activity).startCommonActionActivity(ActionType.ACCEPT_AND_ADL.id, JsonConvert.SerializeObject(abstractDocument), sectionType);
        }

        public void DoRifiuta(AbstractDocumentListItem abstractDocument)
        {
            ((HomeActivity)Activity).startCommonActionActivity(ActionType.REFUSE.id, JsonConvert.SerializeObject(abstractDocument), sectionType);
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
            ((HomeActivity)Activity).startAssignActivity(ActionType.ASSIGN.id, JsonConvert.SerializeObject(abstractDocument));
        }

        public void DoInserisciInADL(AbstractDocumentListItem abstractDocument)
        {
            presenter.AddADLAction(abstractDocument);
        }

        public void OnShareLinkReady(string link)
        {
            ((HomeActivity)Activity).OpenShareView(link);
        }

        public void OnRemoveFromADLOk(string message)
        {
            if (sectionType == SectionType.ADL)
            {
                presenter.OnPullToRefresh();
            }
            else
            {
                Toast.MakeText(Activity, message, ToastLength.Short).Show();
            }
        }

        #endregion


        #region AbstractListFragment
        public override void reloadData()
        {
            presenter.OnPullToRefresh();
        }

        public override void RequestOpenFilterView()
        {
            presenter.OpenFilterView();
        }

        public override void SetFilter(FilterModel filterModel)
        {
            presenter.SetFilterModel(filterModel);
        }

        public override void RequestShowActionsDocuments()
        {
            presenter.ShowDialogActionsDocuments();
        }

        #endregion


        #region UPDATE FILTER implementation

        public void UpdateFilterUI(FilterModel filterModel)
        {
            ((HomeActivity)Activity).UpdateFilterView(filterModel);
        }

        public void OpenFilterView(FilterModel filterModel)
        {
            Intent intent = new Intent(Activity, typeof(FilterActivity));

            Bundle bundle = new Bundle();

            if (filterModel != null)
            {
                bundle.PutString(FilterActivity.KEY_FILTER, JsonConvert.SerializeObject(filterModel));
            }
            bundle.PutInt(FilterActivity.KEY_SECTION_TYPE_INDEX, (int)sectionType);
            intent.PutExtras(bundle);
            Activity.StartActivityForResult(intent, (int)ActivityRequestCodes.Filter);
        }

        #endregion

        public void DoApriDocumentoCondiviso(SharedDocumentBundle sharedDocument)
        {
            Intent intent = new Intent(Activity, typeof(OpenDocumentActivity));
            intent.PutExtra(OpenDocumentActivity.KEY_DOCUMENT_BUNDLE, JsonConvert.SerializeObject(sharedDocument));
            Activity.StartActivity(intent);
        }

        public void OnAddInADLOk(string message)
        {
            if (sectionType == SectionType.ADL)
            {
                presenter.LoadDocumentList();
            }
            else
            {
                Toast.MakeText(Activity, message, ToastLength.Short).Show();
            }
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
            presenter.TapSign(document);
            
        }

        public void TapReject(AbstractDocumentListItem document)
        {
            presenter.TapReject(document);
        }

        public void CompletedActionOK(Dictionary<string, string> extra)
        {
            Bundle bundle = Tools.ConvertDictionaryInBundle(extra);
            Intent intent = new Intent(Activity.ApplicationContext, typeof(ActionCompletedActivity));
            intent.PutExtra(ActionCompletedActivity.KEY_ACTION_TYPE, 8);
            intent.PutExtras(bundle);
            StartActivity(intent);
        }

        public void DoOpenViewOtpCades(List<AbstractDocumentListItem> listStateDaFirmareOTP, int TotalRecordCountSigned)
        {
            StartActivityForResult(((HomeActivity)Activity).StartSignActivityWithOtp(listStateDaFirmareOTP, TotalRecordCountSigned, Constants.ACTION_SIGN_CADES, presenter.signFlowType), (int)ActivityRequestCodes.Sign);
        }

        public void DoOpenViewOtpCadesNotSigned(List<AbstractDocumentListItem> listStateDaFirmareOTP, int TotalRecordCountSigned)
        {
            StartActivityForResult(((HomeActivity)Activity).StartSignActivityWithOtp(listStateDaFirmareOTP, TotalRecordCountSigned, Constants.ACTION_SIGN_CADES_NOT_SIGNED, presenter.signFlowType), (int)ActivityRequestCodes.Sign);
        }

        public void DoOpenViewOtpPades(List<AbstractDocumentListItem> listStateDaFirmareOTP, int TotalRecordCountSigned)
        {
            StartActivityForResult(((HomeActivity)Activity).StartSignActivityWithOtp(listStateDaFirmareOTP, TotalRecordCountSigned, Constants.ACTION_SIGN_PADES, presenter.signFlowType), (int)ActivityRequestCodes.Sign);
        }

        public bool IsSignRejectButtonsDisplayed(string statoFirma)
        {
            return presenter.SignStatusUndefined(statoFirma);
        }

        public string GetLabelSignType(AbstractDocumentListItem document)
        {
            return presenter.GetLabelSignTypeDescr(document);
        }

        public void DoSignAll()
        {
            presenter.TapSignAll();
        }

        public void DoRejectAll()
        {
            presenter.TapRejectAll();
        }

        public string GetDocumentFormatDescr()
        {
            return presenter.GetDocumentFormatDescr();
        }

        public void ShowAlertWithNumberDocuments(string description, string actionType)
        {
            ShowErrorHelper.CreateChoiceAlert(Activity, description, LocalizedString.TITLE_ALERT.Get(), () => { presenter.ActionContinue(actionType); }, () => { });
        }

        public void OnSignCompleted(string message)
        {
            Toast.MakeText(Activity, message, ToastLength.Long).Show();
        }

        public void OnDocumentUpdated(AbstractDocumentListItem document)
        {
            documentRecyclerViewAdapter.NotifyItemChanged(document);
        }

        public void OnViewedADLOk(string message)
        {
            presenter.OnPullToRefresh();
            Toast.MakeText(Activity, message, ToastLength.Short).Show();
        }
    }
}