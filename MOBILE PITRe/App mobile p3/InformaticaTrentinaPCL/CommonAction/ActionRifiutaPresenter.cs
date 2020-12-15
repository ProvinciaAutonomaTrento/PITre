using System;
using InformaticaTrentinaPCL.CommonAction.MVP;
using InformaticaTrentinaPCL.CommonAction.Network;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.CommonAction
{
    public class ActionRifiutaPresenter : IActionPresenter
    {
        IActionModel model;
        IActionView view;
        SessionData sessionData;
        AbstractDocumentListItem currentDocument;
        private string note = "";
        IReachability reachability;

        public ActionRifiutaPresenter(IActionView view, INativeFactory nativeFactory)
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
            AccettaRifiutaRequestModel request = new AccettaRifiutaRequestModel(new AccettaRifiutaRequestModel.Body(NetworkConstants.CONSTANT_RIFIUTA_TRANSMISSION, note, abstractDocument.GetIdTrasmissione()), sessionData.userInfo.token);
            AccettaRifiutaResponseModel response = await model.DoActionAccetta(request);
            view.OnUpdateLoader(false);
            this.ManageResponse(response);

        }

        public void UpdateNote(string note)
        {
            //TODO eventuale controllo su consistenza stringa?
            this.note = note;
            view.EnableConfirmButton(note.Trim().Length > NetworkConstants.CONSTANT_SEARCH_CHARS);
        }

        public void OnViewReady()
        {
            view.EnableConfirmButton(false);
        }

        public void Dispose()
        {
            model.Dispose();
        }

        #region ManageResponse
        private void ManageResponse(AccettaRifiutaResponseModel response)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response,reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        view.CompletedActionOK(Ext.CreateStringForThankYouPage(currentDocument));
                        break;
                    case 2:
                        view.ShowError(LocalizedString.ACTIONTYPE_TRANSMISSION_ERROR.Get());
                        break;
                    default: 
                        view.ShowError(LocalizedString.SERVICE_ERROR.Get()); 
                        break;
                }
            }
        }
        #endregion
    }
}
