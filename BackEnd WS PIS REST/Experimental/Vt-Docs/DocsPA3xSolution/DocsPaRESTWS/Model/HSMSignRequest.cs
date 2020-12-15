using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class HSMSignRequest
    {
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