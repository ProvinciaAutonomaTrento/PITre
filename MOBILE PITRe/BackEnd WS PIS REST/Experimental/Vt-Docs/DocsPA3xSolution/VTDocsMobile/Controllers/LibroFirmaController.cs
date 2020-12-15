using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VTDocsMobile.VTDocsWSMobile;
using VTDocs.mobile.fe.commands;

using System.Web.Mvc;
using VTDocs.mobile.fe.commands.model;
using log4net;

namespace VTDocs.mobile.fe.controllers
{
    public class LibroFirmaController : GeneralController
    {
        private ILog logger = LogManager.GetLogger(typeof(LibroFirmaController));

        [NoCache]
        public ActionResult ChangePage(int numPage)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new LfChangePageCommand(numPage));
            logger.Info("end");
            return res;

        }

        [NoCache]
        public ActionResult ChangeState(string idElemento, string statoCorrente, string dataAccettazione, string motivoRespingimento, string idIstanzaProcesso)
        {
            logger.Info("begin");
            string nuovoStato = "PROPOSTO";
            switch (statoCorrente)
            { 
                case "PROPOSTO":
                    nuovoStato = "DA_FIRMARE";
                    break;
                case "DA_FIRMARE":
                    nuovoStato = "DA_RESPINGERE";
                    break;
                case "DA_RESPINGERE":
                    nuovoStato = "PROPOSTO";
                    break;
            }
            ActionResult res = JsonWithConverters((new CambiaStatoElementoLFCommand(idElemento, dataAccettazione, statoCorrente, nuovoStato, motivoRespingimento, idIstanzaProcesso).Execute()));
            res = CommandExecute(new LibroFirmaCommand());
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult RespingiSelezionati()
        {
            logger.Info("begin");
            ActionResult res = JsonWithConverters((new RespingiSelezionatiElementiLf().Execute()));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult FirmaSelezionati()
        {
            logger.Info("begin");
            ActionResult res = JsonWithConverters((new FirmaSelezionatiElementiLf().Execute()));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult ExistsElementWithSignCades()
        {

            logger.Info("begin");
            ActionResult res = CommandExecute(new ExistsElementWithSignCadesCommand());
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult ExistsElementWithSignPades()
        {

            logger.Info("begin");
            ActionResult res = CommandExecute(new ExistsElementWithSignPadesCommand());
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult LibroFirmaIsAutorized()
        {

            logger.Info("begin");
            ActionResult res = CommandExecute(new LibroFirmaIsAutorizedCommand());
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult RicercaTesto(string testo, RicercaType tipoRic)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new LibroFirmaTestoExecuteCommand(testo, tipoRic));
            logger.Info("end");
            return res;
        }
    }
}
