using System;
using InformaticaTrentinaPCL.CommonAction.MVP;
using InformaticaTrentinaPCL.CommonAction.Network;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.CommonAction
{
    public class ActionADLPresenter : IActionPresenter
    {
        IActionModel model;
        IActionView view;
        SessionData sessionData;
        private string note = "";
        AbstractDocumentListItem currentDocument;
        private AbstractDocumentListItem _currentAbstractDocument; //Used only for flow accetta e ADL
        protected IReachability reachability;


        public ActionADLPresenter(IActionView view, INativeFactory nativeFactory)
        {
            this.view = view;
            this.sessionData = nativeFactory.GetSessionData();
            this.reachability = nativeFactory.GetReachability();
#if CUSTOM
            model = new DummyActionModel();
#else
            model = new ActionModel();
#endif
        }

        public async void ButtonConfirm(AbstractDocumentListItem abstractDocument)
        {
            currentDocument = abstractDocument;
            view.OnUpdateLoader(true);
            this._currentAbstractDocument = abstractDocument;
            AccettaRifiutaRequestModel request = new AccettaRifiutaRequestModel(
                new AccettaRifiutaRequestModel.Body(NetworkConstants.CONSTANT_ACCETTA_TRANSMISSION, note, abstractDocument.GetIdTrasmissione()),
                sessionData.userInfo.token);
            AccettaRifiutaResponseModel response = await model.DoActionAccetta(request);

            this.ManageResponse(response);
        }

        private async void AddADLAction()
        {
            string tipo = TypeDocument.FASCICOLO.Equals(_currentAbstractDocument.tipoDocumento)
                                      ? NetworkConstants.TYPE_DOCUMENT_F
                                      : NetworkConstants.TYPE_DOCUMENT_D;

            ActionADLRequestModel request = new ActionADLRequestModel(
                new ActionADLRequestModel.Body(NetworkConstants.CONSTANT_ADD_ADL_ACTION, _currentAbstractDocument.GetIdDocumento(), tipo),
                sessionData.userInfo.token);
            ActionADLResponseModel response = await model.DoActionAddRemoveADL(request);
            this.ManageResponse(response);

        }

        private void ShowError(string s)
        {
            view.OnUpdateLoader(false);
            view.ShowError(s);
        }

        private void ManageResponse(AccettaRifiutaResponseModel response)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        AddADLAction();
                        break;
                    default:
                        ShowError(LocalizedString.GENERIC_ERROR.Get());
                        break;
                }
            }
        }

        private void ManageResponse(ActionADLResponseModel response)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        view.OnUpdateLoader(false);
                        view.CompletedActionOK(Ext.CreateStringForThankYouPage(currentDocument));
                        break;
                    default:
                        ShowError(LocalizedString.GENERIC_ERROR.Get());
                        break;
                }
            }
        }

        public void UpdateNote(string note)
        {
            //TODO eventuale controllo su consistenza stringa?
            this.note = note;
        }

        public void OnViewReady()
        {
            view.EnableConfirmButton(true);
        }

        public void Dispose()
        {
            model.Dispose();
        }
    }
}
