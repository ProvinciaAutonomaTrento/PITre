using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.AnalyticsCore;
using InformaticaTrentinaPCL.Assegna.MVPD;
using InformaticaTrentinaPCL.Assegna.Network;
using InformaticaTrentinaPCL.Assign;
using InformaticaTrentinaPCL.Delega.Network;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Assegna
{
    public class AssegnaPresenter : IAssegnaPresenter
    {
        protected IAssegnaView view;
        protected IAssegnaModel model;
        protected SessionData sessionData;
        protected IAnalyticsManager analyticsManager;
        protected IReachability rechability;
        protected AssegnaObject assegnaObject;
        protected List<Ragione> cacheRagioni;

        public AssegnaPresenter(IAssegnaView view, INativeFactory nativeFactory, AbstractDocumentListItem currentDocument)
        {
            this.view = view;
            this.sessionData = nativeFactory.GetSessionData();
            this.analyticsManager = nativeFactory.GetAnalyticsManager();
            this.rechability = nativeFactory.GetReachability();
            this.assegnaObject = new AssegnaObject();
            assegnaObject.currentDocument = currentDocument;

#if CUSTOM
            this.model = new DummyAssegnaModel();
#else
            this.model = new AssegnaModel();
#endif
        }

        #region IImplementation
        public void OnViewReady()
        {
            view.EnableButton(null != assegnaObject.assegnatario);
            view.EnableButton(CheckCredentials());
        }

        public async void Trasmetti()
        {
            view.OnUpdateLoader(true);

            bool isModello = assegnaObject.assegnatario.getRecipientType().Equals(AbstractRecipient.RecipientType.MODEL);
            bool isFasc = TypeDocument.FASCICOLO.Equals(assegnaObject.currentDocument.tipoDocumento);
            bool hasWorkFlow = (assegnaObject.currentDocument is ToDoDocumentModel) && ((ToDoDocumentModel)assegnaObject.currentDocument).hasWorkflow;

            SmistaRequestModel.Body body = new SmistaRequestModel.Body(assegnaObject.currentDocument.GetIdTrasmissione(), 
                                                                       isFasc,
                                                                       hasWorkFlow,
                                                                       assegnaObject.currentDocument.getIdEvento(),
                                                                       assegnaObject.currentDocument.GetIdDocumento(),
                                                                       isModello ? null : assegnaObject.assegnatario.getIdCorrespondant(),
                                                                       true,
                                                                       assegnaObject.ragione == null ? string.Empty : assegnaObject.ragione.descrizione,
                                                                       assegnaObject.note,
                                                                       NetworkConstants.TIPO_TRASMISSIONE_S,
                                                                       isModello ? assegnaObject.assegnatario.getId() : null);

            SmistaRequestModel request = new SmistaRequestModel(body, sessionData.userInfo.token);
            SmistaResponseModel response = await model.Smista(request);

            view.OnUpdateLoader(false);
            this.ManageTrasmissioneResponse(response);
        }

        public async void GetListaRagioni()
        {
            if (null != cacheRagioni)
            {
                view.ShowListaRagioni(cacheRagioni);
            }
            else
            {
                view.OnUpdateLoader(true);
                string docFas = TypeDocument.FASCICOLO.Equals(assegnaObject.currentDocument.tipoDocumento) ? NetworkConstants.TYPE_DOCUMENT_F : NetworkConstants.TYPE_DOCUMENT_D;
                ListaRagioniRequestModel.Body body = new ListaRagioniRequestModel.Body(assegnaObject.currentDocument.GetIdDocumento(), docFas);
                ListaRagioniRequestModel request = new ListaRagioniRequestModel(body, sessionData.userInfo.token);
                ListaRagioniResponseModel response = await model.GetListaRagioni(request);
                view.OnUpdateLoader(false);
                this.ManageResponse(response);
            }
        }

        public void UpdateAssegnatario(AbstractRecipient assegnatario)
        {
            this.assegnaObject.assegnatario = assegnatario;
            view.EnableButton(CheckCredentials());

        }

        public void UpdateRagione(Ragione ragione)
        {
            this.assegnaObject.ragione = ragione;
            view.EnableButton(CheckRagione());
        }

        private bool CheckCredentials()
        {
            if (null != assegnaObject.assegnatario && assegnaObject.assegnatario is ModelliTrasmissioneModel)
            {
                view.ShowSelectRagione(false);
                return true;
            }
            else if(null != assegnaObject.assegnatario)
            {
                view.ShowSelectRagione(true);
                return false;
            }
            return false;
        }

        public bool CheckRagione()
        {
            return null != assegnaObject.ragione;
        }

        public void UpdateNote(string note)
        {
            this.assegnaObject.note = note;
        }

        public void Dispose()
        {
            model.Dispose();
        }
        #endregion
        #region ManageResponse

        private void ManageTrasmissioneResponse(BaseResponseModel response)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, rechability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        view.OnAssegnaOk(Ext.CreateStringForThankYouPage(assegnaObject.currentDocument));
                        break;
                    case 2:
                        view.ShowError(LocalizedString.EFFETTUA_TRASMISSIONE_ERROR_CODE_1.Get());
                        break;
                    default:
                        view.ShowError(LocalizedString.GENERIC_ERROR.Get());
                        break;
                }
            }
        }

        private void ManageResponse(ListaRagioniResponseModel response)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, rechability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        cacheRagioni = response.ragioni;
                        view.ShowListaRagioni(cacheRagioni);
                        break;
                    default:
                        view.ShowError(LocalizedString.GENERIC_ERROR.Get());
                        break;
                }
            }
        }
        #endregion
    }

    public class AssegnaObject
    {
        public string note { get; set; }
        public Ragione ragione { get; set; }
        public AbstractRecipient assegnatario { get; set; }
        public AbstractDocumentListItem currentDocument { get; set; }
    }
}
