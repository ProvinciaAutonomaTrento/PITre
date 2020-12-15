using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.CommonAction;
using InformaticaTrentinaPCL.CommonAction.MVP;
using InformaticaTrentinaPCL.CommonAction.Network;
using InformaticaTrentinaPCL.Filter;
using InformaticaTrentinaPCL.Home.MVP;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.OpenFile;
using InformaticaTrentinaPCL.OpenFile.Network;
using InformaticaTrentinaPCL.Search;
using InformaticaTrentinaPCL.Utils;
using TestRefit.Network;
using static InformaticaTrentinaPCL.Home.Network.LibroFirmaResponseModel;

namespace InformaticaTrentinaPCL.Home
{
    public partial class DocumentListPresenter : IDocumentListPresenter
    {
        protected IDocumentListView view;
        protected IDocumentListModel model;
        protected IActionModel actionModel;
        protected SectionType sectionType;
        protected SessionData sessionData;
        protected ActionDialogBuilder actionDialogBuilder;
        protected int countPage;
        protected int totalCount;
        protected List<AbstractDocumentListItem> results = new List<AbstractDocumentListItem>();
        protected bool isPullToRefresh = false;
        protected FilterModel currentFilterModel;
        protected IReachability reachability;

        #region Liste di supporto FIRMA / REJECT 

        public SignFlowType signFlowType;
        private List<AbstractDocumentListItem> listStateProposto
        {
            get { return results.Where(d => d is SignDocumentModel && isProposto((SignDocumentModel)d)).ToList(); }
        }
        private List<AbstractDocumentListItem> listStateDaRespingere
        {
            get { return results.Where(d => d is SignDocumentModel && isDaRespingere((SignDocumentModel)d)).ToList(); }
        }
        private List<AbstractDocumentListItem> listStateDaFirmare
        {
            get { return results.Where(d => d is SignDocumentModel && isDaFirmare((SignDocumentModel)d)).ToList(); }
        }
        private List<AbstractDocumentListItem> listStateDaFirmareOTPPades
        {
            get { return results.Where(d => d is SignDocumentModel && isPades((SignDocumentModel)d)).ToList(); }
        }
        private List<AbstractDocumentListItem> listStateDaFirmareOTPCades
        {
            get { return results.Where(d => d is SignDocumentModel && isCades((SignDocumentModel)d) && !isCadesNotOriginalSigned((SignDocumentModel)d)).ToList(); }
        }
        //cades that aren't signed yet or that were signed as pades
        private List<AbstractDocumentListItem> listStateDaFirmareOTPCadesNotOriginalSigned
        {
            get { return results.Where(d => d is SignDocumentModel && isCadesNotOriginalSigned((SignDocumentModel)d)).ToList(); }
        }

        /// <summary>
        /// The total record count signed.
        /// </summary>
        private int TotalRecordCountSigned = 0;

        #endregion

        public DocumentListPresenter(IDocumentListView view, INativeFactory nativeFactory, SectionType sectionType)
        {
            this.sectionType = sectionType;
            this.sessionData = nativeFactory.GetSessionData();
            this.reachability = nativeFactory.GetReachability();
            this.view = view;
            actionDialogBuilder = new ActionDialogBuilder(view, sectionType);
#if CUSTOM
            this.model = new DummyDocumentListModel();
            this.actionModel = new ActionModel();
#else
            this.model = new DocumentListModel();
            this.actionModel = new ActionModel();
#endif
            Initialize();
        }

        public void OnPullToRefresh()
        {
            Initialize();
            isPullToRefresh = true;
            LoadDocumentList();
            isPullToRefresh = false;
        }

        protected void Initialize()
        {
            ((WS)model).Dispose();
            this.totalCount = int.MaxValue;
            this.countPage = 0;
            results.Clear();
        }

        public virtual void LoadDocumentList()
        {
            if (ListHasMoreElement())
            {
                switch (sectionType)
                {
                    case SectionType.TODO:
                        LoadToDoDocumentList();
                        break;
                    case SectionType.SIGN:
                        LoadSignDocumentsList();
                        break;
                    case SectionType.ADL:
                        LoadADLDocumentList();
                        break;
                }
            }
        }

        #region LoadDocument Tasks

        private async void LoadToDoDocumentList()
        {
            PreRequest();
            LoadToDoDocumentsRequestModel request = new LoadToDoDocumentsRequestModel(new LoadToDoDocumentsRequestModel.Body(countPage, NetworkConstants.DEFAULT_LIST_PAGE_SIZE), sessionData.userInfo.token);
            LoadToDoDocumentsResponseModel response = await model.LoadToDoDocuments(request);
            PostRequest(response);
        }

        private async void LoadSignDocumentsList()
        {
            PreRequest();
            LibroFirmaRequestModel request = this.GenereteLibroFirmaRequest();
            LibroFirmaResponseModel response = await model.LoadSignDocuments(request);
            PostRequest(response);
        }

        private async void LoadADLDocumentList()
        {
            PreRequest();
            ADLListRequestModel request = this.GenereteADLRequest();
            ADLListResponseModel response = await model.LoadADLDocuments(request);
            PostRequest(response);
        }

        public async void ShareDocument(AbstractDocumentListItem abstractDocument, AbstractRecipient recipient)
        {
            view.OnUpdateLoader(true);
            CondividiRequestModel.Body body = new CondividiRequestModel.Body(recipient.getId(), abstractDocument.GetIdDocumento());
            CondividiRequestModel request = new CondividiRequestModel(body, sessionData.userInfo.token);
            CondividiResponseModel response = await model.Condividi(request);
            ManageResponse(response);
            view.OnUpdateLoader(false);
        }

        protected ADLListRequestModel GenereteADLRequest()
        {
            ADLListRequestModel request;
            if (null != currentFilterModel)
            {
                request = new ADLListRequestModel(new RicercaRequestModel.Body(NetworkConstants.DEFAULT_LIST_PAGE_SIZE, countPage,
                    currentFilterModel?.GetADLRequestType(), currentFilterModel.fromDate, currentFilterModel.endDate, currentFilterModel.fromDateProtocol,
                    currentFilterModel.endDateProtocol, currentFilterModel.idDocument, currentFilterModel.NumProto, currentFilterModel.yearProto),
                  sessionData.userInfo.token);
            }
            else
            {
                request = new ADLListRequestModel(new RicercaRequestModel.Body(NetworkConstants.DEFAULT_LIST_PAGE_SIZE, countPage, NetworkConstants.TIPO_ADL_ALL), sessionData.userInfo.token);
            }

            return request;
        }

        protected LibroFirmaRequestModel GenereteLibroFirmaRequest()
        {
            LibroFirmaRequestModel request;
            if (null != currentFilterModel)
            {
                var body = new LibroFirmaRequestModel.Body(NetworkConstants.DEFAULT_LIST_PAGE_SIZE, countPage,
                  currentFilterModel?.GetADLRequestType(), currentFilterModel.fromDate, currentFilterModel.endDate, currentFilterModel.fromDateProtocol,
                  currentFilterModel.endDateProtocol, currentFilterModel.idDocument, currentFilterModel.NumProto, currentFilterModel.yearProto);

                if (!string.IsNullOrEmpty(currentFilterModel.oggetto))
                {
                    body.oggetto = currentFilterModel.oggetto;
                }

                request = new LibroFirmaRequestModel(body, sessionData.userInfo.token);
            }
            else
            {
                request = new LibroFirmaRequestModel(new LibroFirmaRequestModel.Body(NetworkConstants.DEFAULT_LIST_PAGE_SIZE, countPage, NetworkConstants.TIPO_ADL_ALL), sessionData.userInfo.token);
            }
            return request;
        }

        protected void PreRequest()
        {
            // WARNING: Non muovere l'incremento del countPage da qua. 
            // Deve essere la prima operazione dell'if.
            countPage++;
            shouldUpdateLoader(true);
        }

        protected void PostRequest(DocumentResponseModel response)
        {
            shouldUpdateLoader(false);
            ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:

                        if (response is LibroFirmaResponseModel)
                            signFlowType = ((LibroFirmaResponseModel)response).GetSignFlowType();

                        totalCount = response.GetTotalRecordCount();
                        shouldRemoveCellLoader();
                        results.AddRange(response.GetResults());
                        shouldAddCellLoader();

                        view.UpdateList(results);
                        break;
                    default:
                        view.ShowError(LocalizedString.SERVICE_ERROR.Get());
                        break;
                }
            }
        }

        private void ManageResponse(CondividiResponseModel response)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        view.OnShareLinkReady(string.Format(LocalizedString.SHARE_MESSAGE.Get(),
                          NetworkConstants.GetShareLink(response.key), NetworkConstants.GetDownloadUrl()));
                        break;
                    default:
                        view.ShowError(LocalizedString.SERVICE_ERROR.Get());
                        break;
                }
            }
        }

        private void ManageResponse(GetDocumentoCondivisoResponseModel response)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        SharedDocumentBundle sharedDocument = new SharedDocumentBundle(response.GetDocumentsList(), response.docInfo.idDoc);
                        view.DoApriDocumentoCondiviso(sharedDocument);
                        break;
                    case 1:
                        view.ShowError(LocalizedString.SHARED_DOCUMENT_WRONG_USER.Get());
                        break;
                    case 2:
                        view.ShowError(LocalizedString.SHARED_DOCUMENT_EXPIRED_TOKEN.Get());
                        break;
                    default:
                        view.ShowError(LocalizedString.SERVICE_ERROR.Get());
                        break;
                }
            }
        }

        private void ManageResponse(ActionADLResponseModel response, Action<String> action, bool isFascicolo)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        string tipo = isFascicolo ? LocalizedString.FASCICOLO.Get() : LocalizedString.DOCUMENTO.Get();
                        action(tipo);
                        break;
                    case 2:
                        view.ShowError(LocalizedString.ACTIONTYPE_TRANSMISSION_ERROR.Get());
                        break;
                    default:
                        view.ShowError(LocalizedString.GENERIC_ERROR.Get());
                        break;
                }
            }
        }

        private void ManageResponse(ViewedResponseModel response, Action action)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        action();
                        break;
                    case 2:
                        view.ShowError(LocalizedString.ACTIONTYPE_TRANSMISSION_ERROR.Get());
                        break;
                    default:
                        view.ShowError(LocalizedString.GENERIC_ERROR.Get());
                        break;
                }
            }
        }

        /// <summary>
        /// Next two methods are inserted to manage response for ADL Add/Remove action 
        /// </summary>
        private void OnRemoveFromADLOk(string tipo)
        {
            view.OnRemoveFromADLOk(string.Format(LocalizedString.RIMOSSO_DA_ADL.Get(), tipo));
        }

        private void OnAddInADLOk(string tipo)
        {
            view.OnAddInADLOk(string.Format(LocalizedString.AGGIUNTO_IN_ADL.Get(), tipo));
        }

        private void OnViewedOk()
        {
            view.OnViewedOk(LocalizedString.VIEWED.Get());
        }

        #endregion

        #region IImplementation

        /// <summary>
        /// L'update del loader è fatto solo per la prima pagina, mentre per le
        /// altre pagine il loading è trasparente.
        /// Valutare questo comportamento con UX.
        /// </summary>
        /// <param name="isShow">If set to <c>true</c> the loader is shown.</param>
        protected void shouldUpdateLoader(bool isShow)
        {
            if (countPage == 1 && !isPullToRefresh)
            {
                view.OnUpdateLoader(isShow);
            }
        }

        public void OnTapDocument(AbstractDocumentListItem abstractDocument)
        {
            if (sectionType == SectionType.SIGN)
            {
                view.DoOpenFirma(abstractDocument);
            }
            else
            {
                view.DoOpenDialog(actionDialogBuilder.GetDialogActions(abstractDocument, sessionData.isTodoListRemovalManual, sessionData.shareAllowed));
            }
        }

        public void ShowDialogActionsDocuments()
        {
            view.DoOpenDialog(actionDialogBuilder.GetDialogActionDocuments());
        }

        public void OnOpenDocument(AbstractDocumentListItem abstractDocument)
        {
            if (TypeDocument.DOCUMENTO.Equals(abstractDocument.tipoDocumento))
            {
                view.DoApriDocumento(abstractDocument);
            }
            else
            {
                view.DoApriFascicolo(abstractDocument);
            }
        }

        public void SetFilterModel(FilterModel filterModel)
        {
            currentFilterModel = filterModel;
            Initialize();
            if (this is SearchPresenter && filterModel == null)
            {
                ((SearchPresenter)this).ClearSearch();
            }
            else
            {
                LoadDocumentList();
            }

            view.UpdateFilterUI(filterModel);
        }

        public void OpenFilterView()
        {
            view.OpenFilterView(currentFilterModel);
        }

        public async void OnShareDocumentReceived(string url)
        {
            SharedUrl sharedUrl = new SharedUrl(url);
            if (sharedUrl.isValid)
            {
                view.OnUpdateLoader(true);
                GetDocumentoCondivisoRequestModel.Body body = new GetDocumentoCondivisoRequestModel.Body(sharedUrl.sharedKey);
                GetDocumentoCondivisoRequestModel request = new GetDocumentoCondivisoRequestModel(body, sessionData.userInfo.token);
                GetDocumentoCondivisoResponseModel response = await model.GetDocumentoCondiviso(request);
                ManageResponse(response);
                view.OnUpdateLoader(false);
            }
            else
            {
                view.ShowError(LocalizedString.MALFORMED_SHARE_URL_ERROR.Get());
            }
        }

        public async void RemoveADLAction(AbstractDocumentListItem abstractDocument)
        {
            view.OnUpdateLoader(true);
            var response = await DoAdlCall(abstractDocument, false);
            view.OnUpdateLoader(false);
            ManageResponse(response, OnRemoveFromADLOk, TypeDocument.FASCICOLO.Equals(abstractDocument.tipoDocumento));
        }

        public async void AddADLAction(AbstractDocumentListItem abstractDocument)
        {
            view.OnUpdateLoader(true);
            var response = await DoAdlCall(abstractDocument, true);
            view.OnUpdateLoader(false);
            ManageResponse(response, OnAddInADLOk, TypeDocument.FASCICOLO.Equals(abstractDocument.tipoDocumento));
        }

        private async Task<ActionADLResponseModel> DoAdlCall(AbstractDocumentListItem abstractDocument, bool add)
        {
            string tipo = TypeDocument.FASCICOLO.Equals(abstractDocument.tipoDocumento)
              ? NetworkConstants.TYPE_DOCUMENT_F
              : NetworkConstants.TYPE_DOCUMENT_D;
            ActionADLRequestModel request = new ActionADLRequestModel(
                new ActionADLRequestModel.Body(add ? NetworkConstants.CONSTANT_ADD_ADL_ACTION : NetworkConstants.CONSTANT_REMOVE_ADL_ACTION, abstractDocument.GetIdDocumento(), tipo),
              sessionData.userInfo.token);
            return await actionModel.DoActionAddRemoveADL(request);
        }

        public async void DoViewed(AbstractDocumentListItem abstractDocument)
        {
            view.OnUpdateLoader(true);
            var response = await DoViewedCall(abstractDocument);
            view.OnUpdateLoader(false);
            ManageResponse(response, OnViewedOk);
        }

        private async Task<ViewedResponseModel> DoViewedCall(AbstractDocumentListItem abstractDocument)
        {
            ViewedRequestModel.Body body = new ViewedRequestModel.Body(abstractDocument.GetIdTrasmissione(), abstractDocument.getIdEvento());
            ViewedRequestModel request = new ViewedRequestModel(body, sessionData.userInfo.token);
            return await actionModel.DoViewed(request);
        }

        public async void DoViewedADL(AbstractDocumentListItem abstractDocument)
        {
            view.OnUpdateLoader(true);
            var response = await DoViewedCall(abstractDocument);
            view.OnUpdateLoader(false);

            ManageResponse(response, async () => {
                view.OnUpdateLoader(true);
                var adlResponse = await DoAdlCall(abstractDocument, true);
                view.OnUpdateLoader(false);

                ManageResponse(adlResponse, (tipo) => {
                    view.OnViewedADLOk(LocalizedString.VIEWED_AND_ADL.Get());
                }, TypeDocument.FASCICOLO.Equals(abstractDocument.tipoDocumento));
            });
        }

        #endregion

        #region Utility

        /// <summary>
        /// Check if there are more elements to show
        /// </summary>
        /// <returns><c>true</c>, if has more element was listed, <c>false</c> otherwise.</returns>
        public bool ListHasMoreElement()
        {
            return totalCount > NetworkConstants.DEFAULT_LIST_PAGE_SIZE * countPage;
        }

        protected void shouldRemoveCellLoader()
        {
            if (results.Count() == 0)
            {
                return;
            }

            int lastItemIndex = results.Count() - 1;
            if (results.ElementAt(lastItemIndex) is DocumentListLoader)
            {
                results.RemoveAt(lastItemIndex);
            }
        }

        protected void shouldAddCellLoader()
        {
            if (ListHasMoreElement())
            {
                results.Add(new DocumentListLoader());
            }
        }

        #endregion

        public void Dispose()
        {
            actionDialogBuilder.Dispose();
            model.Dispose();
            view = null;
        }

        #region Button FIRMA / REJECT 

        bool isDaRespingere(SignDocumentModel item)
        {
            return item.statoFirma.ToLower() == NetworkConstants.CONSTANT_DA_RESPINGERE;
        }

        bool isProposto(SignDocumentModel item)
        {
            return item.statoFirma.ToLower() == NetworkConstants.CONSTANT_PROPOSTO;
        }

        bool isDaFirmare(SignDocumentModel item)
        {
            return item.statoFirma.ToLower() == NetworkConstants.CONSTANT_DA_FIRMARE && !IsSignOtpFromTypeSign(item.tipoFirma);
        }

        bool isPades(SignDocumentModel item)
        {
            return item.statoFirma.ToLower() == NetworkConstants.CONSTANT_DA_FIRMARE && IsSignOtpFromTypeSign(item.tipoFirma) && GetLabelSignType(item).Equals(LocalizedString.PADES_LABEL.Get());
        }

        bool isCades(SignDocumentModel item)
        {
            return item.statoFirma.ToLower() == NetworkConstants.CONSTANT_DA_FIRMARE && IsSignOtpFromTypeSign(item.tipoFirma) && GetLabelSignType(item).Equals(LocalizedString.CADES_LABEL.Get());
        }

        bool isCadesNotOriginalSigned(SignDocumentModel item)
        {
            return isCades(item) && signFlowType == SignFlowType.PARALLELA && (item.GetTipoFirmaFileOriginale() == SignDocumentModel.SignTypeOriginalFile.SIGNED_PADES ||
                                                                                item.GetTipoFirmaFileOriginale() == SignDocumentModel.SignTypeOriginalFile.NOT_SIGNED);
        }

        /// <summary>
        /// Ises the sign otp from type sign.
        /// </summary>
        /// <returns><c>true</c>, if sign otp from type sign was ised, <c>false</c> otherwise.</returns>
        /// <param name="tipoFirma">Tipo firma.</param>
        /// 
        public bool IsSignOtpFromTypeSign(string tipoFirma)
        {
            return (NetworkConstants.CONSTANT_TIPO_FIRMA_DOC_SIGNATURE_P.Equals(tipoFirma.ToLower()) ||
                    NetworkConstants.CONSTANT_TIPO_FIRMA_DOC_SIGNATURE_cades.Equals(tipoFirma.ToLower()));
        }

        /// <summary>
        /// Signs the status undefined.
        /// </summary>
        /// <returns><c>true</c>, if status undefined was signed, <c>false</c> otherwise.</returns>
        /// <param name="statoFirma">Stato firma.</param>
        public bool SignStatusUndefined(string statoFirma)
        {
            return (NetworkConstants.CONSTANT_PROPOSTO.Equals(statoFirma.ToLower()) ||
                    NetworkConstants.CONSTANT_DA_RESPINGERE.Equals(statoFirma.ToLower()) || NetworkConstants.CONSTANT_DA_FIRMARE.Equals(statoFirma.ToLower()));
        }

        /// <summary>
        /// Taps the sign.
        /// </summary>
        /// <param name="abstractDocument">Abstract document.</param>
        public void TapSign(AbstractDocumentListItem abstractDocument)
        {
            if (abstractDocument != null && abstractDocument is SignDocumentModel)
            {
                //only if press sign (and not remove it) mark also attachments
                var hasToMarkAttachmentsToo = !abstractDocument.signFlag;
                OnSignStateChanged((SignDocumentModel)abstractDocument, !abstractDocument.signFlag, false, hasToMarkAttachmentsToo);
            }
        }

        /// <summary>
        /// Taps the reject.
        /// </summary>
        /// <param name="abstractDocument">Abstract document.</param>
        public void TapReject(AbstractDocumentListItem abstractDocument)
        {
            if (abstractDocument != null && abstractDocument is SignDocumentModel)
            {
                OnSignStateChanged((SignDocumentModel)abstractDocument, false, !abstractDocument.rejectFlag, false);
            }
        }

        private async void OnSignStateChanged(SignDocumentModel abstractDocument, bool signFlag, bool rejectFlag, bool recursive)
        {
            if (!IsAttachment(abstractDocument) && recursive)
            {
                //only if doc is not an attachment, search if there are attachments for this doc, and set same flags as the parent
                GetAttachments(abstractDocument).ForEach(d =>
                {
                    //only if the document hasn't marked nothing
                    if (!d.signFlag && !d.rejectFlag)
                        OnSignStateChanged((SignDocumentModel)d, signFlag, rejectFlag, true);
                });
            }

            var newState = "";
            if (!rejectFlag && !signFlag)
            {
                newState = NetworkConstants.CONSTANT_PROPOSTO;
            }
            else if (signFlag)
            {
                newState = NetworkConstants.CONSTANT_DA_FIRMARE;
            }
            else if (rejectFlag)
            {
                newState = NetworkConstants.CONSTANT_DA_RESPINGERE;
            }

            var newDocument = await ChangeState(abstractDocument, newState);
            if (newDocument != null)
            {
                //TODO new document is not updated from ws, so for now update only the 3 flags manually
                //abstractDocument = newDocument;
                abstractDocument.signFlag = signFlag;
                abstractDocument.rejectFlag = rejectFlag;
                abstractDocument.statoFirma = newState;
                view.OnDocumentUpdated(abstractDocument);
            }
            else
            {
                view.ShowError(LocalizedString.GENERIC_ERROR.Get());
            }
        }

        /// <summary>
        /// Taps the reject all.
        /// </summary>
        public async void TapRejectAll()
        {
            ShowAlertWithNumberDocumentsReject();
        }

        private async void ContinueRejectAll()
        {
            await RejectElements();
        }

        /// <summary>
        /// Taps the sign all.
        /// </summary>
        /// <returns>The sign all.</returns>
        public async void TapSignAll()
        {
            if (listStateDaFirmare.Count == 0 && listStateDaFirmareOTPCades.Count == 0 && listStateDaFirmareOTPPades.Count == 0 && listStateDaFirmareOTPCadesNotOriginalSigned.Count == 0)
            {
                view.ShowError(LocalizedString.MESSAGE_NO_DOCUMENT_FOR_SIGN.Get());
            }
            else
            {
                //begin the sign flow
                var description = Ext.CreateStringOfNumberDocuments(listStateDaFirmare.Count, LocalizedString.MESSAGE_NUMBER_DOCUMENTS_SIGN_HEADER.Get()) + Environment.NewLine;
                if (listStateDaFirmareOTPPades?.Count > 0)
                    description += Ext.CreateStringOfNumberDocuments(listStateDaFirmareOTPPades.Count, LocalizedString.MESSAGE_NUMBER_DOCUMENTS_SIGN_PADES.Get()) + Environment.NewLine;
                if (listStateDaFirmareOTPCades?.Count > 0 || listStateDaFirmareOTPCadesNotOriginalSigned?.Count > 0)
                    description += Ext.CreateStringOfNumberDocuments(listStateDaFirmareOTPCades.Count + listStateDaFirmareOTPCadesNotOriginalSigned.Count, LocalizedString.MESSAGE_NUMBER_DOCUMENTS_SIGN_CADES.Get()) + Environment.NewLine;
                if (listStateDaFirmare?.Count > 0)
                    description += Ext.CreateStringOfNumberDocuments(listStateDaFirmare.Count, LocalizedString.MESSAGE_NUMBER_DOCUMENTS_SIGN.Get()) + Environment.NewLine;

                description += Ext.CreateStringOfNumberDocuments(listStateDaFirmare.Count, LocalizedString.MESSAGE_NUMBER_DOCUMENTS_SIGN_FOOTER.Get());

                view.ShowAlertWithNumberDocuments(description, Constants.ACTION_SIGN);
            }
        }

        private async void ContinueSignAll()
        {
            if (listStateDaFirmare.Count > 0)
            {
                await SignElements();
            }
            else
            {
                OnDocumentsSignActionFinished(true);
            }
        }

        /// <summary>
        /// Taps the sign all cades.
        /// </summary>

        private async void ContinueSignAllCades()
        {
            if (listStateDaFirmareOTPCades.Count > 0)
            {
                view.DoOpenViewOtpCades(listStateDaFirmareOTPCades, TotalRecordCountSigned);
            }
            else
            {
                OnDocumentsSignCadesActionFinished(true);
            }
        }

        private async void ContinueSignAllCadesNotOriginalSignedOrPades()
        {
            if (listStateDaFirmareOTPCadesNotOriginalSigned.Count > 0)
            {
                view.DoOpenViewOtpCadesNotSigned(listStateDaFirmareOTPCadesNotOriginalSigned, TotalRecordCountSigned);
            }
            else
            {
                OnDocumentsSignCadesNotSignedActionFinished(true);
            }
        }

        /// <summary>
        /// Taps the sign all cades.
        /// </summary>

        private async void ContinueSignAllPades()
        {
            if (listStateDaFirmareOTPPades.Count > 0)
            {
                view.DoOpenViewOtpPades(listStateDaFirmareOTPPades, TotalRecordCountSigned);
            }
            else
            {
                OnDocumentsSignPadesActionFinished(true);
            }
        }

        /// <summary>
        /// Actions the continue.
        /// </summary>
        /// <param name="typeAction">Type action.</param>
        public async void ActionContinue(string typeAction)
        {
            if (typeAction.Equals(Constants.ACTION_SIGN))
            {
                ContinueSignAllPades();
            }
            else if (typeAction.Equals(Constants.ACTION_REJECT))
            {
                ContinueRejectAll();
            }
        }

        /// <summary>
        /// Changes the state.
        /// </summary>
        /// <returns>The state.</returns>
        /// <param name="list">List.</param>
        /// <param name="nuovoStato">Nuovo stato.</param>
        private async Task<SignDocumentModel> ChangeState(SignDocumentModel document, String nuovoStato)
        {
            view.OnUpdateLoader(true);

            var listSignDocumentNewStateModel = new List<SignDocumentNewStateModel>();
            CambiaStatoElementoRequestModel request = new CambiaStatoElementoRequestModel();

            document.statoFirma = document.statoFirma.ToUpper();

            SignDocumentNewStateModel itemTmp = new SignDocumentNewStateModel();
            itemTmp.NuovoStato = nuovoStato.ToUpper();
            itemTmp.Elemento = document;
            listSignDocumentNewStateModel.Add(itemTmp);

            request.Elementi = listSignDocumentNewStateModel;
            request.token = sessionData.userInfo.token;

            var response = await model.ChangeDocumentsState(request);

            view.OnUpdateLoader(false);

            if (ManageResponse(response))
            {
                return response.Elementi.Find(d => d.Elemento.GetIdDocumento() == document.GetIdDocumento()).Elemento;
            }
            return null;
        }

        /// <summary>
        /// Manages the response.
        /// </summary>
        /// <param name="response">Response.</param>
        private bool ManageResponse(Object response)
        {
            var responseTmp = response as BaseResponseModel;
            ResponseHelper responseHelper = new ResponseHelper(view, responseTmp, reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (responseTmp.Code)
                {
                    case 0:
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Rejects the element.
        /// </summary>
        private async Task RejectElements()
        {
#if DEBUG
            //CompletedActionOK(listStateDaRespingere);
            //return;
#endif
            view.OnUpdateLoader(true);
            PreRequest();
            LibroFirmaRequestModel.Body body = new LibroFirmaRequestModel.Body(NetworkConstants.DEFAULT_LIST_PAGE_SIZE, countPage);
            LibroFirmaRequestModel request = new LibroFirmaRequestModel(body, sessionData.userInfo.token);
            RespingiElementiResponseModel response = await this.model.RejectDocuments(request);
            view.OnUpdateLoader(false);

            if (ManageResponse(response as Object))
            {
                CompletedActionOK(listStateDaRespingere);
            }
        }

        /// <summary>
        /// Signs the element.
        /// </summary>
        private async Task SignElements()
        {
#if DEBUG
            //CompletedActionOK(listStateDaFirmare);
            //return;
#endif
            view.OnUpdateLoader(true);
            PreRequest();
            LibroFirmaRequestModel.Body body = new LibroFirmaRequestModel.Body(NetworkConstants.DEFAULT_LIST_PAGE_SIZE, countPage);
            LibroFirmaRequestModel request = new LibroFirmaRequestModel(body, sessionData.userInfo.token);
            FirmaElementiResponseModel response = await this.model.SignDocuments(request);
            view.OnUpdateLoader(false);
            TotalRecordCountSigned = response.totalRecordCount;

            var success = ManageResponse(response as Object);
            OnDocumentsSignActionFinished(success);
        }

        public string GetDocumentFormatDescr()
        {
            return LocalizedString.PDF_LABEL.Get();
        }

        public string GetLabelSignTypeDescr(AbstractDocumentListItem document)
        {
            if (document is SignDocumentModel)
            {
                SignDocumentModel signDocumentModel = (SignDocumentModel)document;

                if (signDocumentModel.tipoFirma.ToLower().Equals(NetworkConstants.CONSTANT_TIPO_FIRMA_DOC_SIGNATURE_P))
                {
                    return LocalizedString.PADES_SIGN.Get();
                }
                if (signDocumentModel.tipoFirma.ToLower().Equals(NetworkConstants.CONSTANT_TIPO_FIRMA_DOC_SIGNATURE_cades))
                {
                    return LocalizedString.CADES_SIGN.Get();
                }

                return LocalizedString.ELECTRONIC_SIGN.Get();
            }
            return "";
        }

        /// <summary>
        /// Gets the type of the label sign.
        /// </summary>
        /// <returns>The label sign type.</returns>
        /// <param name="document">Document.</param>
        public string GetLabelSignType(AbstractDocumentListItem document)
        {
            if (document is SignDocumentModel)
            {
                SignDocumentModel signDocumentModel = (SignDocumentModel)document;

                if (signDocumentModel.tipoFirma.ToLower().Equals(NetworkConstants.CONSTANT_TIPO_FIRMA_DOC_SIGNATURE_P))
                {
                    return LocalizedString.PADES_LABEL.Get();
                }
                else if (signDocumentModel.tipoFirma.ToLower().Equals(NetworkConstants.CONSTANT_TIPO_FIRMA_DOC_SIGNATURE_cades))
                {
                    return LocalizedString.CADES_LABEL.Get();
                }
            }
            return LocalizedString.PDF_LABEL.Get();
        }

        /// <summary>
        /// Shows the alert with number documents reject.
        /// </summary>
        /// <returns><c>true</c>, if alert with number documents reject was shown, <c>false</c> otherwise.</returns>
        public void ShowAlertWithNumberDocumentsReject()
        {
            if (listStateDaRespingere?.Count > 0)
            {
                var description = Ext.CreateStringOfNumberDocuments(listStateDaRespingere.Count, LocalizedString.MESSAGE_NUMBER_DOCUMENTS_REJECT.Get());
                view.ShowAlertWithNumberDocuments(description, Constants.ACTION_REJECT);
            }
            else
            {
                view.ShowError(LocalizedString.MESSAGE_NO_DOCUMENT_FOR_REJECT.Get());
            }
        }

        /// <summary>
        /// Completeds the action ok.
        /// </summary>
        /// <param name="list">List.</param>
        public void CompletedActionOK(List<AbstractDocumentListItem> list)
        {
            var text = Ext.CreateStringForThankYouPage(list);
            OnPullToRefresh();
            view.CompletedActionOK(text);
        }

        public void OnDocumentsSignPadesActionFinished(bool success)
        {
            if (success)
                ContinueSignAllCades();
            else
            {
                OnPullToRefresh();
                view.ShowError(LocalizedString.MESSAGE_ERROR_DOCUMENTS_SIGN_PADES.Get());
            }
        }

        public void OnDocumentsSignCadesActionFinished(bool success)
        {
            if (success)
                ContinueSignAllCadesNotOriginalSignedOrPades();
            else
            {
                OnPullToRefresh();
                view.ShowError(LocalizedString.MESSAGE_ERROR_DOCUMENTS_SIGN_CADES.Get());
            }
        }

        public void OnDocumentsSignCadesNotSignedActionFinished(bool success)
        {
            if (success)
                ContinueSignAll();
            else
            {
                OnPullToRefresh();
                view.ShowError(LocalizedString.MESSAGE_ERROR_DOCUMENTS_SIGN_CADES.Get());
            }
        }

        public void OnDocumentsSignActionFinished(bool success)
        {
            OnPullToRefresh();

            if (success)
                view.OnSignCompleted(LocalizedString.MESSAGE_SIGN_SUCCESS.Get());
            else
                view.ShowError(LocalizedString.MESSAGE_ERROR_DOCUMENTS_SIGN.Get());
        }

        public bool HasAttachments(AbstractDocumentListItem abstractDocument)
        {
            return GetAttachments(abstractDocument).Count > 0;
        }

        public bool IsAttachment(AbstractDocumentListItem abstractDocument)
        {
            return (abstractDocument is SignDocumentModel && !string.IsNullOrEmpty(((SignDocumentModel)abstractDocument).infoDocumento.idDocPrincipale));
        }

        public bool HasDocumentFather(AbstractDocumentListItem abstractDocument)
        {
            return IsAttachment(abstractDocument) && results.Where(d => d.GetIdDocumento() == ((SignDocumentModel)abstractDocument).infoDocumento.idDocPrincipale).ToList().Count > 0;
        }

        private List<AbstractDocumentListItem> GetAttachments(AbstractDocumentListItem document)
        {
            Func<AbstractDocumentListItem, bool> searchPredicate = d => d is SignDocumentModel &&
                ((SignDocumentModel)d).infoDocumento.idDocPrincipale == document.GetIdDocumento() &&
                                      d.GetIdDocumento() != document.GetIdDocumento();
            return results.Where(searchPredicate).ToList();
        }

        #endregion
    }
}