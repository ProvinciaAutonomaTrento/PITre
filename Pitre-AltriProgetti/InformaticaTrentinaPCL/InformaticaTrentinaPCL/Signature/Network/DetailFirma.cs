using System;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Signature.Network
{
    public class DetailFirma
    {
        [JsonProperty("NomeFile")]
        public string NomeFile { get; set; }

        [JsonProperty("Livello")]
        public string Livello { get; set; }

        [JsonProperty("VerificaStatoFirma")]
        public string VerificaStatoFirma { get; set; }

        [JsonProperty("VerificaStatoCertificato")]
        public string VerificaStatoCertificato { get; set; }

        [JsonProperty("VerificaCRL")]
        public string VerificaCrl { get; set; }

        [JsonProperty("VerificaDataValiditaDocumento")]
        public string VerificaDataValiditaDocumento { get; set; }

        [JsonProperty("VerificaStatusCode")]
        public long VerificaStatusCode { get; set; }

        [JsonProperty("CertificatoEnte")]
        public string CertificatoEnte { get; set; }

        [JsonProperty("CertificatoSN")]
        public string CertificatoSn { get; set; }

        [JsonProperty("CertificatoValidoDal")]
        public string CertificatoValidoDal { get; set; }

        [JsonProperty("CertificatoValidoAl")]
        public string CertificatoValidoAl { get; set; }

        [JsonProperty("CertificatoAlgoritmo")]
        public string CertificatoAlgoritmo { get; set; }

        [JsonProperty("CertificatoFirmatario")]
        public string CertificatoFirmatario { get; set; }

        [JsonProperty("CertificatoThumbprint")]
        public string CertificatoThumbprint { get; set; }

        [JsonProperty("SoggettoNome")]
        public string SoggettoNome { get; set; }

        [JsonProperty("SoggettoCognome")]
        public string SoggettoCognome { get; set; }

        [JsonProperty("SoggettoCodiceFiscale")]
        public string SoggettoCodiceFiscale { get; set; }

        [JsonProperty("SoggettoDataNascita")]
        public string SoggettoDataNascita { get; set; }

        [JsonProperty("SoggettoOrganizzazione")]
        public string SoggettoOrganizzazione { get; set; }

        [JsonProperty("SoggettoRuolo")]
        public string SoggettoRuolo { get; set; }

        [JsonProperty("SoggettoPaese")]
        public string SoggettoPaese { get; set; }

        [JsonProperty("SoggettoIdTitolare")]
        public string SoggettoIdTitolare { get; set; }

        [JsonProperty("InfoFirmaAlgoritmo")]
        public string InfoFirmaAlgoritmo { get; set; }

        [JsonProperty("InfoFirmaImpronta")]
        public string InfoFirmaImpronta { get; set; }

        [JsonProperty("InfoFirmaControfirmatario")]
        public string InfoFirmaControfirmatario { get; set; }

        [JsonProperty("InfoFirmaData")]
        public string InfoFirmaData { get; set; }

        [JsonProperty("TimeStampInfo")]
        public TimeStampInfo TimeStampInfo { get; set; }

        public DetailFirma()
        {
        }
    }
}
