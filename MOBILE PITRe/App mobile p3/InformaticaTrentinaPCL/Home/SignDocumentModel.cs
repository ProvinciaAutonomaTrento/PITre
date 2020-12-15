using System;
using System.Globalization;
using System.Text.RegularExpressions;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Home
{
    public class SignDocumentModel : AbstractDocumentListItem
    {
        [JsonProperty(PropertyName = "IdElemento")]
        public string idElemento { get; set; }

        [JsonProperty(PropertyName = "StatoFirma")]
        public string statoFirma { get; set; }

        [JsonProperty(PropertyName = "TipoFirma")]
        public string tipoFirma { get; set; }

        [JsonProperty(PropertyName = "Modalita")]
        public string modalita { get; set; }

        [JsonProperty(PropertyName = "DataInserimento")]
        public string dataInserimento { get; set; }

        [JsonProperty(PropertyName = "RuoloProponente")]
        public string ruoloProponente { get; set; }

        [JsonProperty(PropertyName = "UtenteProponente")]
        public string utenteProponente { get; set; }

        [JsonProperty(PropertyName = "InfoDocumento")]
        public SignDocumentInfoModel infoDocumento { get; set; }

        [JsonProperty(PropertyName = "Note")]
        public string note { get; set; }

        [JsonProperty(PropertyName = "IdIstanzaProcesso")]
        public string idIstanzaProcesso { get; set; }

        [JsonProperty(PropertyName = "MotivoRespingimento")]
        public string motivoRespingimento { get; set; }

        [JsonProperty(PropertyName = "IdIstanzaPasso")]
        public string idIstanzaPasso { get; set; }

        [JsonProperty(PropertyName = "IdTrasmSingola")]
        public string idTrasmSingola { get; set; }

        [JsonProperty(PropertyName = "IdUtenteTitolare")]
        public string idUtenteTitolare { get; set; }

        [JsonProperty(PropertyName = "IdRuoloTitolare")]
        public string idRuoloTitolare { get; set; }

        [JsonProperty(PropertyName = "DataAccettazione")]
        public string dataAccettazione { get; set; }

        [JsonProperty(PropertyName = "FileSize")]
        public int fileSize { get; set; }

        [JsonProperty(PropertyName = "ErroreFirma")]
        public string erroreFirma { get; set; }

        [JsonProperty(PropertyName = "IsAllegato")]
        public bool isAllegato { get; set; }

        [JsonProperty(PropertyName = "Id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "TipoFirmaFileOriginale")]
        int tipoFirmaFileOriginale { get; set; }

        public SignDocumentModel(string idElemento, string statoFirma, string tipoFirma, string modalita, string dataInserimento, string ruoloProponente,
                                 string utenteProponente, SignDocumentInfoModel infoDocumento, string note, string idIstanzaProcesso, string motivoRespingimento,
                                 string idIstanzaPasso, string idTrasmSingola, string idUtenteTitolare, string idRuoloTitolare, string dataAccettazione,
                                 int fileSize, string erroreFirma, bool isAllegato, int tipo, string id, int tipoFirmaFileOriginale)
        {
            this.idElemento = idElemento;
            this.statoFirma = statoFirma;
            this.tipoFirma = tipoFirma;
            this.modalita = modalita;
            this.dataInserimento = dataInserimento;
            this.ruoloProponente = ruoloProponente;
            this.utenteProponente = utenteProponente;
            this.infoDocumento = infoDocumento;
            this.note = note;
            this.idIstanzaProcesso = idIstanzaProcesso;
            this.motivoRespingimento = motivoRespingimento;
            this.idIstanzaPasso = idIstanzaPasso;
            this.idTrasmSingola = idTrasmSingola;
            this.idUtenteTitolare = idUtenteTitolare;
            this.idRuoloTitolare = idRuoloTitolare;
            this.dataAccettazione = dataAccettazione;
            this.fileSize = fileSize;
            this.erroreFirma = erroreFirma;
            this.isAllegato = isAllegato;
            this.tipo = tipo;
            this.id = id;
            this.tipoFirmaFileOriginale = tipoFirmaFileOriginale;
            this.SetFlags();
        }

        public override string GetData()
        {
            return infoDocumento.dataDoc;
        }
        
        public string GetDataToShow()
        {
            return infoDocumento.isProtocollato ? ConvertDateFormatIfNeeded(infoDocumento.dataProto) : ConvertDateFormatIfNeeded(infoDocumento.dataDoc);
        }

        public override string GetMittente()
        {
            return infoDocumento.mittente;
        }

        public override string GetOggetto()
        {
            return infoDocumento.oggetto;
        }

        public override void SetFlags()
        {
            if (NetworkConstants.CONSTANT_DA_FIRMARE.Equals(this.statoFirma.ToLower()))
            {
                rejectFlag = false;
                signFlag = true;
            }
            else if (NetworkConstants.CONSTANT_DA_RESPINGERE.Equals(this.statoFirma.ToLower()))
            {
                rejectFlag = true;
                signFlag = false;
            }
            else if (NetworkConstants.CONSTANT_PROPOSTO.Equals(this.statoFirma.ToLower()))
            {
               rejectFlag = false;
               signFlag = false;
            }
        }

        public override string GetInfo()
        {
            //TODO check property
            return "";
        }

        public override string GetEstensione()
        {
            //TODO check property
            return "";
        }

        public override string GetIdTrasmissione()
        {
            return idTrasmSingola;
        }

        public override string getIdEvento()
        {
            //TODO check property
            return "";
        }

        public override string GetIdDocumento()
        {
            return id;
        }

        public override string GetRagione(){
			//TODO check property
			return "";
        }
        
        public override SignatureInfo getSignatureInfo()
        {
            return SignatureInfo.Create(null, id);
        }

        public SignTypeOriginalFile GetTipoFirmaFileOriginale()
        {
            return (SignTypeOriginalFile)Enum.GetValues(typeof(SignTypeOriginalFile)).GetValue(tipoFirmaFileOriginale);
        }

        public enum SignTypeOriginalFile
        {
            NOT_SIGNED,
            SIGNED_PADES,
            SIGNED_CADES
        }
    }
}
