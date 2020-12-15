using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VTDocs.mobile.fe.model;
using VTDocsMobile.VTDocsWSMobile;
using log4net;

namespace VTDocs.mobile.fe.commands.model
{
    public class DettaglioDocTrasmCommand : MainModelCommand
    {
        private string _idTrasm;
        private string _idEvento;
        private bool _loadTrasm;
        private string _idDoc;
        private Tab _tab;
        private ILog logger = LogManager.GetLogger(typeof(DettaglioDocTrasmCommand));

        public DettaglioDocTrasmCommand(string idDoc,string idTrasm, string idEvento,bool loadTrasm)
        {
            this._idDoc = idDoc;
            this._idTrasm = idTrasm;
            this._idEvento = idEvento;
            this._loadTrasm = loadTrasm;
            if (loadTrasm)
            {
                this._tab = Tab.DETTAGLIO_DF_TRASM;
            }
            else
            {
                this._tab = Tab.DETTAGLIO_DOC;
            }
        }

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            model.TabShow = _tab;
            GetDocInfoRequest request = new GetDocInfoRequest();
            request.UserInfo = NavigationHandler.CurrentUser;
            request.IdTrasm = _idTrasm;
            request.IdEvento = _idEvento;
            request.IdDoc = _idDoc;
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
            if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                request.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

            GetDocInfoResponse response = WSStub.getDocInfo(request);
            logger.Info("responseCode: " + response.Code);
            if (response.Code == GetDocInfoResponseCode.OK)
            {
                DettaglioDFTrasmModel tabModel = new DettaglioDFTrasmModel();
                tabModel.IdTrasm = _idTrasm;
                tabModel.IdEvento = _idEvento; ;
                tabModel.DocInfo = response.DocInfo;
                tabModel.Allegati = response.Allegati;
                tabModel.TrasmInfo = response.TrasmInfo;
                if (tabModel.TrasmInfo != null && !tabModel.TrasmInfo.HasWorkflow && ConfigurationHandler.RemoveTrasmInTDL)
                {
                    //NavigationHandler.ToDoListMemento = null;
                }
                model.TabModel = tabModel;
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