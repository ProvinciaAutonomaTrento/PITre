using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VTDocs.mobile.fe.commands.model;
using VTDocs.mobile.fe.model;
using VTDocsMobile.VTDocsWSMobile;
using log4net;

namespace VTDocs.mobile.fe.commands.model
{
    public class DettaglioFascTrasmCommand : MainModelCommand
    {
        private string _idTrasm;
        private string _idFasc;
        private Tab _tab;
        private ILog logger = LogManager.GetLogger(typeof(DettaglioFascTrasmCommand));

        public DettaglioFascTrasmCommand(string idFasc,string idTrasm)
        {
            this._idFasc = idFasc;
            this._idTrasm = idTrasm;
            this._tab = Tab.DETTAGLIO_DF_TRASM;
        }

        public DettaglioFascTrasmCommand(string idFasc)
        {
            this._idFasc = idFasc;
            this._tab = Tab.DETTAGLIO_FASC;
        }
 
        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            model.TabShow = _tab;
            GetFascInfoRequest request = new GetFascInfoRequest();
            request.UserInfo = NavigationHandler.CurrentUser;

            if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                request.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

            if (!string.IsNullOrEmpty(_idTrasm))
            {
                request.IdTrasm = _idTrasm;
            }
            request.IdFasc = _idFasc;
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
            GetFascInfoResponse response = WSStub.getFascInfo(request);
            logger.Info("responseCode: " + response.Code);
            if (response.Code == GetFascInfoResponseCode.OK)
            {
                DettaglioDFTrasmModel tabModel = new DettaglioDFTrasmModel();
                tabModel.FascInfo = response.FascInfo;
                tabModel.TrasmInfo = response.TrasmInfo;
                model.TabModel = tabModel;
                if (tabModel.TrasmInfo != null && !tabModel.TrasmInfo.HasWorkflow && ConfigurationHandler.RemoveTrasmInTDL)
                {
                    //NavigationHandler.ToDoListMemento = null;
                }
            }
            else
            {
                logger.Info("add system error");
                addSystemError(model);
            }
            logger.Info("end");
        }
    }
}
