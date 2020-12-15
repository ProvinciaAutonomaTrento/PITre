using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VTDocs.mobile.fe.model;
using VTDocsMobile.VTDocsWSMobile;
using log4net;

namespace VTDocs.mobile.fe.commands.model
{
    public class HSMSignatureCommand : MainModelCommand
    {
        private string _idDoc;
        private bool _cofirma;
        private bool _timestamp;
        private string _tipoFirma;
        private string _aliasCertificato;
        private string _dominioCertificato;
        private string _otpFirma;
        private string _pinCertificato;
        private bool _convertPdf;

        private ILog logger = LogManager.GetLogger(typeof(HSMSignatureCommand));

        public HSMSignatureCommand(string IdDoc, bool cofirma, bool timestamp, string tipoFirma, string alias, string dominio, string otpFirma, string pinCertificato, bool convertPdf)
        {
            this._idDoc = IdDoc;
            this._cofirma = cofirma;
            this._timestamp = timestamp;
            this._tipoFirma = tipoFirma;
            this._aliasCertificato = alias;
            this._dominioCertificato = dominio;
            this._otpFirma = otpFirma;
            this._pinCertificato = pinCertificato;
            this._convertPdf = convertPdf;
        }


        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");

            HSMSignRequest request = new HSMSignRequest();

            request.UserInfo = NavigationHandler.CurrentUser;
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
            request.IdDoc = _idDoc;

            request.AliasCertificato = this._aliasCertificato.ToUpper();
            request.DominioCertificato = this._dominioCertificato.ToUpper();
            request.cofirma = this._cofirma;
            request.ConvertPdf = this._convertPdf;
            request.OtpFirma = this._otpFirma.ToUpper();
            request.PinCertificato = this._pinCertificato.ToUpper();
            request.TipoFirma = this._tipoFirma;
            request.timestamp = this._timestamp;

            HSMSignResponse response = WSStub.hsmSign(request);
            logger.Info("responseCode: " + response.Code);
            if (response.Code == HSMSignResponseCode.OK)
            {
                model.HSMSignResult = true;
            }
            else
            {
                logger.Info("add system error");
                model.HSMSignResult = false;
                addSystemError(model);
            }

            logger.Info("end");
        }
    }

    public class HSMMultiSignatureCommand : MainModelCommand
    {
        private string _idDoc;
        private bool _cofirma;
        private bool _timestamp;
        private string _tipoFirma;
        private string _aliasCertificato;
        private string _dominioCertificato;
        private string _otpFirma;
        private string _pinCertificato;
        private bool _convertPdf;

        private ILog logger = LogManager.GetLogger(typeof(HSMSignatureCommand));

        public HSMMultiSignatureCommand(bool cofirma, bool timestamp, string tipoFirma, string alias, string dominio, string otpFirma, string pinCertificato, bool convertPdf)
        {
            this._cofirma = cofirma;
            this._timestamp = timestamp;
            this._tipoFirma = tipoFirma;
            this._aliasCertificato = alias;
            this._dominioCertificato = dominio;
            this._otpFirma = otpFirma;
            this._pinCertificato = pinCertificato;
            this._convertPdf = convertPdf;
        }


        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");

            HSMSignRequest request = new HSMSignRequest();

            request.UserInfo = NavigationHandler.CurrentUser;
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;

            request.AliasCertificato = this._aliasCertificato.ToUpper();
            request.DominioCertificato = this._dominioCertificato.ToUpper();
            request.cofirma = this._cofirma;
            request.ConvertPdf = this._convertPdf;
            request.OtpFirma = this._otpFirma.ToUpper();
            request.PinCertificato = this._pinCertificato.ToUpper();
            request.TipoFirma = this._tipoFirma;
            request.timestamp = this._timestamp;

            HSMSignResponse response = WSStub.HsmMultiSign(request);
            logger.Info("responseCode: " + response.Code);
            if (response.Code == HSMSignResponseCode.OK)
            {
                model.HSMSignResult = true;
            }
            else
            {
                logger.Info("add system error");
                model.HSMSignResult = false;
                addSystemError(model);
            }

            logger.Info("end");
        }
    }

    public class HSMRequestOTPCommand : MainModelCommand
    {

        private string _aliasCertificato;
        private string _dominioCertificato;

        private ILog logger = LogManager.GetLogger(typeof(HSMRequestOTPCommand));

        public HSMRequestOTPCommand(string aliasCertificato, string dominioCertificato)
        {
            this._aliasCertificato = aliasCertificato;
            this._dominioCertificato = dominioCertificato;
        }

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            HSMSignRequest request = new HSMSignRequest();
            request.AliasCertificato = this._aliasCertificato.ToUpper();
            request.DominioCertificato = this._dominioCertificato.ToUpper();
            request.UserInfo = NavigationHandler.CurrentUser;
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;

            HSMSignResponse response = WSStub.hsmRequestOTP(request);
            logger.Info("responseCode: " + response.Code);
            if (response.Code == HSMSignResponseCode.OK)
            {
                model.HSMRequestOTPResult = true;
            }
            else
            {
                logger.Info("add system error");
                model.HSMRequestOTPResult = false;
                addSystemError(model);
            }

            logger.Info("end");
        }
    }

    public class HSMSignDetailCommand : MainModelCommand
    {
        private string _idDoc;

        private ILog logger = LogManager.GetLogger(typeof(HSMSignDetailCommand));

        public HSMSignDetailCommand(string IdDoc)
        {
            this._idDoc = IdDoc;
        }

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            HSMSignRequest request = new HSMSignRequest();
            request.UserInfo = NavigationHandler.CurrentUser;
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
            request.IdDoc = _idDoc;

            HSMSignResponse response = WSStub.hsmInfoSign(request);
            logger.Info("responseCode: " + response.Code);
            if (response.Code == HSMSignResponseCode.OK)
            {
                model.InfoDocFirme= response.infoFirma ;
            }
            else
            {
                logger.Info("add system error");
                addSystemError(model);
            }

            logger.Info("end");
        }
    }

    public class HSMVerifySignCommand : MainModelCommand
    {
        private string _idDoc;

        private ILog logger = LogManager.GetLogger(typeof(HSMVerifySignCommand));

        public HSMVerifySignCommand(string IdDoc)
        {
            this._idDoc = IdDoc;
        }

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            HSMSignRequest request = new HSMSignRequest();
            request.UserInfo = NavigationHandler.CurrentUser;
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
            request.IdDoc = _idDoc;

            HSMSignResponse response = WSStub.hsmVerifySign(request);
            logger.Info("responseCode: " + response.Code);
            if (response.Code== HSMSignResponseCode.OK) 
                model.HSMVerifySignResult =  true;
            else if (response.Code== HSMSignResponseCode.KO) 
                      model.HSMVerifySignResult = false;
                  else
                    {
                        logger.Info("add system error");
                        addSystemError(model);
                    }

            logger.Info("end");
        }
    }


    public class HSMGetMementoForUserCommand : MainModelCommand
    {
        private string _idDoc;

        private ILog logger = LogManager.GetLogger(typeof(HSMVerifySignCommand));

        public HSMGetMementoForUserCommand(string IdDoc)
        {
            this._idDoc = IdDoc;
        }

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            HSMSignRequest request = new HSMSignRequest();
            request.UserInfo = NavigationHandler.CurrentUser;
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
            request.IdDoc = _idDoc;

            HSMSignResponse response = WSStub.hsmInfoMemento(request);
            if (response.Code == HSMSignResponseCode.OK)
                model.HSMMemento = response.memento;
            else
            {
                logger.Info("add system error");
                addSystemError(model);
            }

            logger.Info("end");
        }
    }

    public class HSMIsAllowedOTPCommand : MainModelCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(HSMIsAllowedOTPCommand));

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            RuoloInfo ruolo = NavigationHandler.RuoloInfo;
            bool result = WSStub.isAllowedOTP(ruolo);
            if (result)
                model.HSMIsAllowedOTP = true;
            else
                model.HSMIsAllowedOTP = false;

            logger.Info("end");
        }
    }


}