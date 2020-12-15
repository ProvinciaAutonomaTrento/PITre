using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using VTDocsMobile.VTDocsWSMobile;
using VTDocs.mobile.fe;
using VTDocs.mobile.fe.controllers;
using VTDocs.mobile.fe.commands.model;
using System.Web.Script.Serialization;
using System.Globalization;

namespace VTDocs.mobile.fe.controllers
{
    public class DelegaController : GeneralController
    {
        private ILog logger = LogManager.GetLogger(typeof(DelegaController));

        [HttpPost]
        [NoCache]
        public ActionResult CreaDelegaDaModello(string idModello,string dataInizio,string dataFine)
        {
            logger.Info("begin");
            DateTime dataInizio1 = DateTime.ParseExact(dataInizio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dataFine1 = DateTime.ParseExact(dataFine, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            logger.Info("idModello: " + idModello + " dataInizio: " + dataInizio1 + " dataFine: " + dataFine1);
            ActionResult res = JsonWithConverters(new CreaDelegaDaModelloCommand(idModello,dataInizio1,dataFine1).Execute());
            logger.Info("end");
            return res;
        }

        [HttpPost]
        [NoCache]
        public ActionResult CreaDelega(string idUtente,string idRuolo,string dataInizio,string dataFine)
        {
            logger.Info("begin");
            DateTime dataInizio1 = DateTime.ParseExact(dataInizio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dataFine1 = !string.IsNullOrEmpty(dataFine) && dataFine != "aN/aN/NaN" ? DateTime.ParseExact(dataFine, "dd/MM/yyyy", CultureInfo.InvariantCulture) : dataInizio1.AddDays(-1);
            logger.Info("idUtente: " + idUtente + " idRuolo: " + idRuolo + " dataInizio: " + dataInizio1 + " dataFine: " + dataFine1);
            ActionResult res = JsonWithConverters(new CreaDelegaCommand(idUtente, idRuolo, dataInizio1, dataFine1).Execute());
            logger.Info("end");
            return res;
        }

        [HttpPost]
        [NoCache]
        public ActionResult RevocaDeleghe(string deleghe)
        {
            logger.Info("begin");
            JavaScriptSerializer jss = new JavaScriptSerializer();
            List<Delega> delegheList= jss.Deserialize<List<Delega>>(deleghe);
            ActionResult res = CommandExecute(new RevocaDelegaCommand(delegheList));
            logger.Info("end");
            return res;
        }

        [HttpPost]
        [NoCache]
        public ActionResult DismettiDelega(string idDelegante){
            logger.Info("begin");
            ActionResult res = CommandExecute(new DismettiDelegaCommand(idDelegante));
            logger.Info("end");
            return res;
        }

        [HttpPost]
        [NoCache]
        public ActionResult AccettaDelega(DelegaInfo delega)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new AccettaDelegaCommand(delega));
            logger.Info("end");
            return res;
        }

        [HttpPost]
        [NoCache]
        public ActionResult ListaDeleghe()
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new ListaDelegheCommand());
            logger.Info("end");
            return res;
        }

        //[HttpPost]
        [NoCache]
        public ActionResult CreaDelegaForm()
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new CreaDelegaFormCommand());
            logger.Info("end");
            return res;
        }

        [HttpPost]
        [NoCache]
        public ActionResult RicercaUtenti(string descrizione)
        {
            logger.Info("begin");
            try
            {
                logger.Debug("descrizione: " + descrizione);
                RicercaUtentiRequest request = new RicercaUtentiRequest();
                ConfigurationHandler confHandler = new ConfigurationHandler();
                request.Descrizione = descrizione;
                request.UserInfo = NavigationHandler.CurrentUser;
                request.Ruolo = NavigationHandler.RuoloInfo;
                logger.Debug("idAmministrazione: " + request.UserInfo.IdAmministrazione);
                request.NumMaxResults = confHandler.MaxNumRisultatiAutocomplete;
                logger.Debug("Chiamata ws...");
                RicercaUtentiResponse response = WSStub.ricercaUtenti(request);
                logger.Info("end");
                return JsonWithConverters(response.Risultati);
            }
            catch (Exception e)
            {
                logger.Error("Exception: " + e);
                return null;
            }
        }

    }
}
