using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VTDocs.mobile.fe.commands.model;
using log4net;

namespace VTDocs.mobile.fe.controllers
{
    public class FascicoloController : GeneralController
    {
        private ILog logger = LogManager.GetLogger(typeof(FascicoloController));

        [NoCache]
        public ActionResult DettaglioFascTrasm(string idFasc, string idTrasm)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new DettaglioFascTrasmCommand(idFasc, idTrasm));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult DettaglioFasc(string idFasc)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new DettaglioFascTrasmCommand(idFasc));
            logger.Info("end");
            return res;
        }

    }
}
