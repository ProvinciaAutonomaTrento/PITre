using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VTDocsMobile.VTDocsWSMobile;
using VTDocs.mobile.fe.commands.model;
using log4net;

namespace VTDocs.mobile.fe.controllers
{
    public class AdlController : GeneralController
    {
        //
        private ILog logger = LogManager.GetLogger(typeof(AdlController));

        [NoCache]
        public ActionResult Ricerca(string idRicSalvata, RicercaSalvataType tipoRicSalvata)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new AdlExecuteCommand(idRicSalvata, tipoRicSalvata));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult RicercaTesto(string testo, RicercaType tipoRic)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new AdlTestoExecuteCommand(testo, tipoRic));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult ChangePage(int numPage)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new AdlChangePageCommand(numPage));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult EnterInFolder(string idFolder, string nameFolder)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new AdlEnterInFolderCommand(idFolder, nameFolder));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult UpFolder()
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new AdlUpFolderCommand());
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult AddDocInAdl(string Id, string TipoProto)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new AddToAdlCommand(Id,TipoProto,false));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult AddFascInAdl(string Id, string TipoProto)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new AddToAdlCommand(Id, TipoProto, true));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult RemoveDocFromAdl(string Id, string TipoProto)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new RemoveFromAdlCommand(Id, TipoProto, false));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult RemoFascFromAdl(string Id, string TipoProto)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new RemoveFromAdlCommand(Id, TipoProto, true));
            logger.Info("end");
            return res;
        }

    }
}
