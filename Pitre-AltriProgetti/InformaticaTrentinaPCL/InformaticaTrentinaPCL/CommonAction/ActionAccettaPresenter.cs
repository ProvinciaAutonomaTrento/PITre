using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.CommonAction.MVP;
using InformaticaTrentinaPCL.CommonAction.Network;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Resources;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.CommonAction
{
    public class ActionAccettaPresenter : IActionPresenter
    {
        IActionModel model;
        IActionView view;
        SessionData sessionData;
        private string note = "";
        AbstractDocumentListItem currentDocument;
        IReachability reachability;

        public ActionAccettaPresenter(IActionView view, INativeFactory nativeFactory)
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
            AccettaRifiutaRequestModel request = new AccettaRifiutaRequestModel(
                new AccettaRifiutaRequestModel.Body(NetworkConstants.CONSTANT_ACCETTA_TRANSMISSION, note, abstractDocument.GetIdTrasmissione()),
                sessionData.userInfo.token);
            AccettaRifiutaResponseModel response = await model.DoActionAccetta(request);
            view.OnUpdateLoader(false);
            this.ManageResponse(response);

        }

        private void ManageResponse(AccettaRifiutaResponseModel response)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response,this.reachability);
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

        public void Dispose()
        {
            model.Dispose();
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
    }
}
