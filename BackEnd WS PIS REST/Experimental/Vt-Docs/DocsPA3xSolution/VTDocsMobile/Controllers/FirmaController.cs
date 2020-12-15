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
    public class FirmaController : GeneralController
    {
        private ILog logger = LogManager.GetLogger(typeof(FirmaController));

        /// <summary>
        /// Firma HSM
        /// </summary>
        [NoCache]
        public ActionResult HSMSign(string iddoc,
                                   bool cofirma, 
                                   bool timestamp, 
                                   string tipoFirma, 
                                   string alias, 
                                   string dominio, 
                                   string otp, 
                                   string pin, 
                                   bool convert)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new HSMSignatureCommand(iddoc, cofirma, timestamp, tipoFirma, alias, dominio, otp, pin, convert));
            logger.Info("end");
            return res;
        }

        /// <summary>
        /// Firma multipla HSM
        /// </summary>
        [NoCache]
        public ActionResult HSMMultiSign(
                                   bool cofirma,
                                   bool timestamp,
                                   string tipoFirma,
                                   string alias,
                                   string dominio,
                                   string otp,
                                   string pin,
                                   bool convert)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new HSMMultiSignatureCommand(cofirma, timestamp, tipoFirma, alias, dominio, otp, pin, convert));
            logger.Info("end");
            return res;
        }

        /// <summary>
        /// Richiesta OTP
        /// </summary>
        [NoCache]
        public ActionResult HSMRequestOTP(string aliasCertificato, string dominioCertificato)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new HSMRequestOTPCommand(aliasCertificato, dominioCertificato));
            logger.Info("end");
            return res;
        }

        /// <summary>
        /// Detail Firma Hsm
        /// </summary>
        [NoCache]
        public ActionResult HSMSignDetail(string iddoc)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new HSMSignDetailCommand(iddoc));
            logger.Info("end");
            return res;
        }



        /// <summary>
        /// Verify Firma Hsm
        /// </summary>
        [NoCache]
        public ActionResult HSMVerifySign(string iddoc)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new HSMVerifySignCommand(iddoc));
            logger.Info("end");
            return res;
        }


        /// <summary>
        /// Info Memento firma (Alias/Dominio)
        /// </summary>
        [NoCache]
        public ActionResult HSMGetMemento(string iddoc)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new HSMGetMementoForUserCommand(iddoc));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult HSMIsAllowedOTP()
        {
            
            logger.Info("begin");
            ActionResult res = CommandExecute(new HSMIsAllowedOTPCommand());
            logger.Info("end");
            return res;
        }

    }
}
