using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VTDocs.mobile.fe.controllers;
using log4net;
using System.Web.Mvc;
using VTDocsMobile.VTDocs.mobile.fe.commands.model;
using VTDocsMobile.VTDocsWSMobile;
using System.Web.Script.Serialization;
using VTDocs.mobile.fe;

namespace VTDocsMobile.Controllers
{
    public class SmistamentoController : GeneralController
    {
        private ILog logger = LogManager.GetLogger(typeof(SmistamentoController));

        [NoCache]
        public ActionResult SmistamentoForm(int numPage)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new SmistamentoFormCommand(numPage));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult EseguiSmistamento(string idDoc, string idTrasm, string idTrasmUtente, string idTrasmSingola, string note, string elements)
        {
            logger.Info("begin");
            logger.Debug("Elements: " + elements);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            EseguiSmistamentoElement[] elements1 = jss.Deserialize<List<EseguiSmistamentoElement>>(elements).ToArray();
            ActionResult res = JsonWithConverters(new EseguiSmistamentoCommand(elements1,idDoc,idTrasm,idTrasmUtente,idTrasmSingola,note).Execute());
            logger.Info("end");
            return res;
        }

        [HttpPost]
        [NoCache]
        public ActionResult RicercaElementi(string descrizione, string ragione)
        {
            logger.Info("begin");
            try
            {
                //NavigationHandler.CurrentUser.Ruoli 
                logger.Debug("descrizione: " + descrizione);
                logger.Debug("ragione:" + ragione);
                RicercaSmistamentoRequest request = new RicercaSmistamentoRequest();
                ConfigurationHandler confHandler = new ConfigurationHandler();
                request.Descrizione = descrizione;
                request.UserInfo = NavigationHandler.CurrentUser;
                request.Ruolo = NavigationHandler.RuoloInfo;
                logger.Debug("idAmministrazione: " + request.UserInfo.IdAmministrazione);
                request.NumMaxResults = confHandler.MaxNumRisultatiAutocomplete;
                request.numMaxResultsForCategory = confHandler.MaxNumRisultatiAutocompleteURP;
                // MEV SMISTAMENTO
                // metto nella request il tipo di ragione (competenza/conoscenza)
                request.Ragione = ragione;
                logger.Debug("Chiamata ws...");
                RicercaSmistamentoResponse response = WSStub.ricercaSmistamento(request);
                

                logger.Info("end");
                return JsonWithConverters(response.Elements);
            }
            catch (Exception e)
            {
                logger.Error("Exception: " + e);
                return null;
            }
        }

        /*
        [HttpPost]
        [NoCache]
        public ActionResult AggiungiElemento(string idCorrGlobali)
        {
            logger.Info("begin");
            try
            {
                //NavigationHandler.CurrentUser.Ruoli 
                RicercaSmistamentoRequest request = new RicercaSmistamentoRequest();
                ConfigurationHandler confHandler = new ConfigurationHandler();
                request.Descrizione = idCorrGlobali;
                request.UserInfo = NavigationHandler.CurrentUser;
                request.Ruolo = NavigationHandler.RuoloInfo;
                logger.Debug("idAmministrazione: " + request.UserInfo.IdAmministrazione);
                request.NumMaxResults = confHandler.MaxNumRisultatiAutocomplete;
                request.numMaxResultsForCategory = confHandler.MaxNumRisultatiAutocompleteURP;
                
                logger.Debug("Chiamata ws...");
                //RicercaSmistamentoResponse response = WSStub.ricercaSmistamento(request);
                RicercaSmistamentoResponse response = WSStub.aggiungiSmistamentoElement(request);

                logger.Info("end");
                return JsonWithConverters(response.Elements);
            }
            catch (Exception e)
            {
                logger.Error("Exception: " + e);
                return null;
            }
        }
        */

        [HttpPost]
        [NoCache]
        public ActionResult RicercaUtentiInRuolo(string descrizione)
        {
            logger.Debug("RicercaUtentiInRuolo");
            logger.Info("begin");
            try
            {
                //NavigationHandler.CurrentUser.Ruoli 
                logger.Debug("descrizione: " + descrizione);
                RicercaSmistamentoRequest request = new RicercaSmistamentoRequest();
                ConfigurationHandler confHandler = new ConfigurationHandler();
                request.Descrizione = descrizione;
                request.UserInfo = NavigationHandler.CurrentUser;
                request.Ruolo = NavigationHandler.RuoloInfo;
                logger.Debug("idAmministrazione: " + request.UserInfo.IdAmministrazione);
                request.NumMaxResults = -99;
                request.numMaxResultsForCategory = confHandler.MaxNumRisultatiAutocompleteURP;
                logger.Debug("Chiamata ws...");
                //RicercaSmistamentoResponse response = WSStub.ricercaSmistamento(request);
                RicercaSmistamentoResponse response = WSStub.aggiungiSmistamentoElement(request);

                logger.Info("end");
                return JsonWithConverters(response.Elements);
            }
            catch (Exception e)
            {
                logger.Error("Exception: " + e);
                return null;
            }
        }

        [HttpPost]
        [NoCache]
        public ActionResult NavigaUO(string idUO)
        {
            logger.Debug("RicercaUtentiInRuolo");
            logger.Info("begin");
            try
            {
                GetSmistamentoTreeRequest request = new GetSmistamentoTreeRequest();
                ConfigurationHandler confHandler = new ConfigurationHandler();
                request.UserInfo = NavigationHandler.CurrentUser;
                request.Ruolo = NavigationHandler.RuoloInfo;
                request.IdUO = idUO;

                GetSmistamentoTreeResponse response = WSStub.getSmistamentoTree(request);

                logger.Info("end");
                return JsonWithConverters(response.Element);
                          
            }
            catch (Exception e)
            {
                logger.Error("Exception: " + e);
                return null;
            }

        }

    }
}