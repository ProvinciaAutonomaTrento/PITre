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
    public class TrasmissioneController : GeneralController
    {
        private ILog logger = LogManager.GetLogger(typeof(TrasmissioneController));

        [NoCache]
        public ActionResult RifiutaTrasm(string idTrasm, string idTrasmUtente,string note)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new AccettaRifiutaTrasmCommand(idTrasm, idTrasmUtente, note, AccettaRifiutaAction.RIFIUTA));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult AccettaTrasm(string idTrasm, string idTrasmUtente,string note)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new AccettaRifiutaTrasmCommand(idTrasm, idTrasmUtente, note, AccettaRifiutaAction.ACCETTA));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult VistoTrasm(string idTrasm)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new VistoTrasmCommand(idTrasm));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult AccettaTrasmInAdl(string idTrasm, string idTrasmUtente, string note, string idDoc, string tipoProto)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new AccettaRifiutaTrasmCommand(idTrasm, idTrasmUtente, note, AccettaRifiutaAction.ACCETTA,idDoc,tipoProto));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult TrasmissioneFormFasc(string idFasc, string idTrasmPerAcc)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new TrasmissioneFormCommand(idFasc, true, idTrasmPerAcc));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult TrasmissioneFormDoc(string idDoc, string idTrasmPerAcc)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new TrasmissioneFormCommand(idDoc, false, idTrasmPerAcc));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult EseguiTrasmDoc(string idTrasmModel, string idDoc, string note, string idTrasmPerAcc)
        {
            logger.Info("begin");
            //Accettazione
            if (!String.IsNullOrEmpty(idTrasmPerAcc))
            {
                VTDocs.mobile.fe.model.MainModel mm = new AccettaRifiutaTrasmCommand(idTrasmPerAcc, null, "Accettazione automatica da mobile per trasmissione effettuata", AccettaRifiutaAction.ACCETTA).Execute();
               if (mm.Errori != null)
               {
                   ActionResult resErr = JsonWithConverters(mm);
                   logger.Info("end with errors accepting");
                   return resErr;
               }
            }

            ActionResult res = JsonWithConverters(new EseguiTrasmCommand(idTrasmModel, note, idDoc, true).Execute());
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult EseguiTrasmFasc(string idTrasmModel, string idFasc, string note, string idTrasmPerAcc)
        {
            logger.Info("begin");

            //Accettazione
            if (!String.IsNullOrEmpty(idTrasmPerAcc))
            {
                VTDocs.mobile.fe.model.MainModel mm = new AccettaRifiutaTrasmCommand(idTrasmPerAcc, null, "Accettazione automatica da mobile per trasmissione effettuata", AccettaRifiutaAction.ACCETTA).Execute();
                if (mm.Errori != null)
                {
                    ActionResult resErr = JsonWithConverters(mm);
                    logger.Info("end with errors accepting");
                    return resErr;
                }
            }

            ActionResult res = JsonWithConverters(new EseguiTrasmCommand(idTrasmModel, note, idFasc, false).Execute());
            logger.Info("end");
            return res;
        }

    }
}
