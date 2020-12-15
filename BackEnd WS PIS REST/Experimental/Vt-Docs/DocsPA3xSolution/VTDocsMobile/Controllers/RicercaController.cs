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
    public class RicercaController : GeneralController
    {
        private ILog logger = LogManager.GetLogger(typeof(RicercaController));

        [NoCache]
        public ActionResult Ricerca(string idRicSalvata, RicercaSalvataType tipoRicSalvata)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new RicExecuteCommand(idRicSalvata, tipoRicSalvata));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult RicercaTesto(string testo, RicercaType tipoRic)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new RicTestoExecuteCommand(testo, tipoRic));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult ChangePage(int numPage)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new RicChangePageCommand(numPage));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult EnterInFolder(string idFolder, string nameFolder)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new RicEnterInFolderCommand(idFolder, nameFolder));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult UpFolder()
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new RicUpFolderCommand());
            logger.Info("end");
            return res;
        }

    }
}
