using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VTDocs.mobile.fe.commands.model;
using log4net;
using VTDocs.mobile.fe.model;
using VTDocsMobile.VTDocsWSMobile;
using VTDocs.mobile.fe.commands;

namespace VTDocsMobile.VTDocs.mobile.fe.commands.model
{
    public class SmistamentoFormCommand : MainModelCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(SmistamentoFormCommand));

        private int _currentPage;

        public SmistamentoFormCommand()
        {
            this._currentPage = 1;
        }

        public SmistamentoFormCommand(int currentPage)
        {
            this._currentPage = currentPage;
        }

        
        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("Start");
            model.TabShow = Tab.SMISTAMENTO;
            GetSmistamentoElementsRequest request = new GetSmistamentoElementsRequest();
            request.UserInfo = NavigationHandler.CurrentUser;
            if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                request.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

            request.PageSize = ConfigurationHandler.NumResultsForPage;
            request.RequestedPage = _currentPage;
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
            request.Registri = NavigationHandler.Registri;
            logger.Info("Invoco il metodo getSmistamentoElements");
            GetSmistamentoElementsResponse response = WSStub.getSmistamentoElements(request);
            if (response.Code == GetSmistamentoElementsResponseCode.OK)
            {
                logger.Debug("Invocato");
                GetSmistamentoTreeRequest request1 = new GetSmistamentoTreeRequest();
                request1.UserInfo = NavigationHandler.CurrentUser;
                request1.IdUO = string.Empty;
                if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                    request1.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

                request1.Ruolo = NavigationHandler.RuoloInfo;
                logger.Info("Invoco il metodo getSmistamentoTree");
                GetSmistamentoTreeResponse response1 = WSStub.getSmistamentoTree(request1);
                if (response1.Code == GetSmistamentoTreeResponseCode.OK)
                {
                    logger.Info("Invocato");
                
                    SmistamentoModel tab = new SmistamentoModel(response.TotalRecordCount, ConfigurationHandler.NumResultsForPage);
                    tab.CurrentPage=(response.TotalRecordCount == 0)? 0 :_currentPage;
                    tab.Tree = response1.Element;
                    tab.SmistamentoElements=response.Elements;
                    tab.NumElements = response.TotalRecordCount;
                    tab.CollapseRuoli = ConfigurationHandler.SmistamentoCollapseRuoli;
                    model.TabModel = tab;
                    ListaModelliTrasmRequest req = new ListaModelliTrasmRequest();
                    req.UserInfo = NavigationHandler.CurrentUser;
                    if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                        req.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

                    req.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
                    req.TrasmFasc = false;
                    logger.Info("Invoco il metodo getListaModelliTrasm");
                
                    ListaModelliTrasmResponse resp = WSStub.getListaModelliTrasm(req);
                    logger.Info("Invocato.");
                    tab.ModelliTrasm = resp.Modelli;

                    //Verifico se il pulsante visto è visibile
                    bool setDataVista = ConfigurationHandler.IsVisibleButtonVisto;
                    tab.SetDataVista = setDataVista;
                }
                else
                {
                    logger.Info("Errore getSmistamentoTree");
                    addSystemError(model);
                }
            }
            else
            {
                logger.Info("Errore getSmistamentoElements");
                addSystemError(model);
            }
            logger.Info("End");
        }

    }

    public class EseguiSmistamentoCommand : Command<OperationResponse>
    {
        private ILog logger = LogManager.GetLogger(typeof(EseguiSmistamentoCommand));
        private EseguiSmistamentoElement[] _elements;
        private string _idDocument;
        private string _idTrasm;
        private string _idTrasmUtente;
        private string _idTrasmSingola;
        private string _note;

        public EseguiSmistamentoCommand(EseguiSmistamentoElement[] elements, string idDocument, string idTrasm, string idTrasmUtente, string idTrasmSingola, string note)
        {
            this._elements = elements;
            this._idDocument = idDocument;
            this._idTrasm = idTrasm;
            this._idTrasmUtente = idTrasmUtente;
            this._idTrasmSingola = idTrasmSingola;
            this._note = note;
        }

        public override OperationResponse Execute()
        {
            logger.Info("begin");
            OperationResponse res = new OperationResponse();
            try
            {
                EseguiSmistamentoRequest request = new EseguiSmistamentoRequest();
                request.Elements = _elements;
                request.IdDocumento = _idDocument;
                request.IdTrasmissione = _idTrasm;
                request.IdTrasmissioneUtente = _idTrasmUtente;
                request.IdTrasmissioneSingola = _idTrasmSingola;
                request.NoteGenerali = _note;
                request.Ruolo = NavigationHandler.RuoloInfo;
                request.UserInfo = NavigationHandler.CurrentUser;

                if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                    request.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

//                request.UserInfo.    = NavigationHandler.DeleganteInfo ;

                EseguiSmistamentoResponse response = WSStub.eseguiSmistamento(request);
                if (response.Code == EseguiSmistamentoResponseCode.OK)
                {
                    res.Success = true;
                }
                else
                {
                    logger.Debug("errore di sistema");
                    res.Success = false;
                    res.Error = Resources.Errors.Common_SystemError;
                }
            }
            catch (Exception e)
            {
                logger.Debug("eccezione: "+e);
                res.Success = false;
                res.Error = Resources.Errors.Common_SystemError;
            }
            return res;
        }
    }
}