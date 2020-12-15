using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.OpenFile
{
    public class DocInfo : AbstractDocumentListItem
    {
        [JsonProperty("AccessRights")]
        public string accessRights { get; set; }

        [JsonProperty("CanTransmit")]
        public bool canTransmit { get; set; }

        [JsonProperty("DataDoc")]
        public string dataDoc { get; set; }

        [JsonProperty("DataProto")]
        public string dataProto { get; set; }

        [JsonProperty("DescrFasc")]
        public List<string> descrFasc { get; set; }

        [JsonProperty("Destinatari")]
        public List<string> destinatari { get; set; }

        //[JsonProperty("Fascicoli")]
        //public object[] Fascicoli { get; set; }

        [JsonProperty("HasPreview")]
        public bool hasPreview { get; set; }

        [JsonProperty("IdDoc")]
        public string idDoc { get; set; }

        [JsonProperty("IdDocPrincipale")]
        public string idDocPrincipale { get; set; }

        [JsonProperty("IsAcquisito")]
        public bool isAcquisito { get; set; }

        [JsonProperty("IsProtocollato")]
        public bool isProtocollato { get; set; }

        [JsonProperty("Mittente")]
        public string mittente { get; set; }

        [JsonProperty("Note")]
        public string note { get; set; }

        [JsonProperty("Oggetto")]
        public string oggetto { get; set; }

        [JsonProperty("OriginalFileName")]
        public string originalFileName { get; set; }

        [JsonProperty("Segnatura")]
        public string segnatura { get; set; }

        [JsonProperty("TipoProto")]
        public string tipoProto { get; set; }

        public DocInfo()
        {
        }

        public DocInfo(string accessRights, bool canTransmit, string dataDoc, string dataProto, List<string> descrFasc, 
                       List<string> destinatari,  bool hasPreview, string idDoc, string idDocPrincipale, bool isAcquisito, 
                       bool isProtocollato, string mittente, string note, string oggetto, string originalFileName, string segnatura, string tipoProto)
        {
            this.accessRights = accessRights;
            this.canTransmit = canTransmit;
            this.dataDoc = dataDoc;
            this.dataProto = dataProto;
            this.descrFasc = descrFasc;
            this.destinatari = destinatari;
            this.hasPreview = hasPreview;
            this.idDoc = idDoc;
            this.idDocPrincipale = idDocPrincipale;
            this.isAcquisito = isAcquisito;
            this.isProtocollato = isProtocollato;
            this.mittente = mittente;
            this.note = note;
            this.oggetto = oggetto;
            this.originalFileName = originalFileName;
            this.segnatura = segnatura;
            this.tipoProto = tipoProto;
        }

        public override string GetData()
        {
            return dataDoc;
        }

        public override string GetMittente()
        {
            return mittente;
        }

        public override string GetOggetto()
        {
            return oggetto;
        }

        public override string GetInfo()
        {
            return note; //Check if is correct
        }

        public override string GetEstensione()
        {
            return ""; //TODO Check 
        }

        public override string GetIdTrasmissione()
        {
            return ""; //TODO Check 
        }

        public override string getIdEvento()
        {
            return ""; //TODO Check  
        }

        public override string GetIdDocumento()
        {
            return idDoc;
        }

        public override string GetRagione()
        {
            return "";//TODO Check 
        }
        
        public override SignatureInfo getSignatureInfo()
        {
            return SignatureInfo.Create(segnatura, idDoc);
        }

        public override void SetFlags()
        {
            return;
        }
    }
}
