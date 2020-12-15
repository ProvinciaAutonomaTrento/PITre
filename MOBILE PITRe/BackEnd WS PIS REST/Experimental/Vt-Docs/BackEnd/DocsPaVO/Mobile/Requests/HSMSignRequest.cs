using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class HSMSignRequest
    {
        public UserInfo UserInfo
        {
            get;
            set;
        }

        public string IdGruppo
        {
            get;
            set;
        }

        public string IdCorrGlobali
        {
            get;
            set;
        }

        public string IdDoc
        {
            get;
            set;
        }

        //DocsPaVO.utente.InfoUtente infoUtente, ????????????????????????????????????

        public bool cofirma
        {
            get;
            set;
        }

        public bool timestamp
        {
            get;
            set;
        }

        public string TipoFirma
        {
            get;
            set;
        }

        public string AliasCertificato
        {
            get;
            set;
        }

        public string DominioCertificato
        {
            get;
            set;
        }

        public string OtpFirma
        {
            get;
            set;
        }

        public string PinCertificato
        {
            get;
            set;
        }

        public bool ConvertPdf
        {
            get;
            set;
        }
    }
}
