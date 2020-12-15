using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.documento;
using DocsPaVO.Note;
using System.Globalization;
using DocsPaVO.utente;
using DocsPaVO.fascicolazione;

namespace DocsPaVO.Mobile
{
    public class InfoDocFirmato 
    {
        public List<NodoFirma> Firme { get; set; }
    }

    public class NodoFirma
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public List<NodoFirma> ChildNodiFirma { get; set; }
        public InfoDetailFirma DetailFirma { get; set; }
    }

    public class InfoDetailFirma
        {
            public enum StatusCode 
            { 
                NOT_DEFINED,
                ERROR_SERVICE_CRL,
                ERROR,
                VERIFY_OK,
                REVOKED_CERT
            }

            public string NomeFile { get; set; }
            public string Livello { get; set; }
            public string VerificaStatoFirma { get; set; }
            public string VerificaStatoCertificato { get; set; }
            public string VerificaCRL { get; set; }
            public string VerificaDataValiditaDocumento { get; set; }
            public StatusCode VerificaStatusCode { get; set; }
            public string CertificatoEnte { get; set; }
            public string CertificatoSN { get; set; }
            public string CertificatoValidoDal { get; set; }
            public string CertificatoValidoAl { get; set; }
            public string CertificatoAlgoritmo { get; set; }
            public string CertificatoFirmatario { get; set; }
            public string CertificatoThumbprint { get; set; }
            public string SoggettoNome { get; set; }
            public string SoggettoCognome { get; set; }
            public string SoggettoCodiceFiscale { get; set; }
            public string SoggettoDataNascita { get; set; }
            public string SoggettoOrganizzazione { get; set; }
            public string SoggettoRuolo { get; set; }
            public string SoggettoPaese { get; set; }
            public string SoggettoIdTitolare { get; set; }
            public string InfoFirmaAlgoritmo { get; set; }
            public string InfoFirmaImpronta { get; set; }
            public string InfoFirmaControfirmatario { get; set; }
            public string InfoFirmaData { get; set; }
            public TSInfo TimeStampInfo { get; set; }
        }


}
