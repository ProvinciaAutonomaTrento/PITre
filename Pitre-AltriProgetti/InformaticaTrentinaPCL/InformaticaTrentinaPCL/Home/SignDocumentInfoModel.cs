using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Home
{
    public class SignDocumentInfoModel
    {
        [JsonProperty(PropertyName = "IdDoc")]
        public string idDoc { get; set; }

        [JsonProperty(PropertyName = "OriginalFileName")]
        public string originalFileName { get; set; }

        [JsonProperty(PropertyName = "Oggetto")]
        public string oggetto { get; set; }

        [JsonProperty(PropertyName = "Note")]
        public string note { get; set; }

        [JsonProperty(PropertyName = "IsAcquisito")]
        public bool isAcquisito { get; set; }

        [JsonProperty(PropertyName = "HasPreview")]
        public bool hasPreview { get; set; }

        [JsonProperty(PropertyName = "DataDoc")]
        public string dataDoc { get; set; }

        [JsonProperty(PropertyName = "TipoProto")]
        public string tipoProto { get; set; }

        [JsonProperty(PropertyName = "Segnatura")]
        public string segnatura { get; set; }

        [JsonProperty(PropertyName = "DataProto")]
        public string dataProto { get; set; }

        [JsonProperty(PropertyName = "Mittente")]
        public string mittente { get; set; }

        [JsonProperty(PropertyName = "Destinatari")]
        public List<string> destinatari { get; set; }

        [JsonProperty(PropertyName = "Fascicoli")]
        public List<string> fascicoli { get; set; }

        [JsonProperty(PropertyName = "DescrFasc")]
        public List<string> descrFasc { get; set; }

        [JsonProperty(PropertyName = "IsProtocollato")]
        public bool isProtocollato { get; set; }

        [JsonProperty(PropertyName = "IdElemento")]
        public bool canTransmit { get; set; }

        [JsonProperty(PropertyName = "AccessRights")]
        public string accessRights { get; set; }

        [JsonProperty(PropertyName = "IdDocPrincipale")]
        public string idDocPrincipale { get; set; }

        public SignDocumentInfoModel(string idDoc, string originalFileName, string oggetto, string note, bool isAcquisito,
                            bool hasPreview, string dataDoc, string tipoProto, string segnatura, string dataProto,
                            string mittente, List<string> destinatari, List<string> fascicoli, List<string> descrFasc, bool isProtocollato,
                            bool canTransmit, string accessRights, string idDocPrincipale)
        {

            this.idDoc = idDoc;
            this.originalFileName = originalFileName;
            this.oggetto = oggetto;
            this.note = note;
            this.isAcquisito = isAcquisito;
            this.hasPreview = hasPreview;
            this.dataDoc = dataDoc;
            this.tipoProto = tipoProto;
            this.segnatura = segnatura;
            this.dataProto = dataProto;
            this.mittente = mittente;
            this.destinatari = destinatari;
            this.fascicoli = fascicoli;
            this.descrFasc = descrFasc;
            this.isProtocollato = isProtocollato;
            this.canTransmit = canTransmit;
            this.accessRights = accessRights;
            this.idDocPrincipale = idDocPrincipale;

        }
    }
}
