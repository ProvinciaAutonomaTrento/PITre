using System;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Home
{
    public class ToDoDocumentModel : AbstractDocumentListItem
    {
        public ToDoDocumentModel(string id, string idTrasm, string oggetto,
                            string tipoProto, string segnatura, string dataDoc,
                            int tipo, string note, string mittente,
                            string ragione, bool hasWorkflow, string extension,
                            string idEvento
                                )
        {
            this.id = id;
            this.idTrasm = idTrasm;
            this.oggetto = oggetto;
            this.tipoProto = tipoProto;
            this.segnatura = segnatura;
            this.dataDoc = dataDoc;
            this.tipo = tipo;
            this.note = note;
            this.mittente = mittente;
            this.ragione = ragione;
            this.hasWorkflow = hasWorkflow;
            this.extension = extension;
            this.idEvento = idEvento;
        }

        [JsonProperty(PropertyName = "Id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "IdTrasm")]
        public string idTrasm { get; set; }

        [JsonProperty(PropertyName = "Oggetto")]
        public string oggetto { get; set; }

        [JsonProperty(PropertyName = "TipoProto")]
        public string tipoProto { get; set; }

        [JsonProperty(PropertyName = "Segantura")]
        public string segnatura { get; set; }

        [JsonProperty(PropertyName = "DataDoc")]
        public string dataDoc { get; set; }

        [JsonProperty(PropertyName = "Note")]
        public string note { get; set; }

        [JsonProperty(PropertyName = "Mittente")]
        public string mittente { get; set; }

        [JsonProperty(PropertyName = "Ragione")]
        public string ragione { get; set; }

        [JsonProperty(PropertyName = "HasWorkflow")]
        public bool hasWorkflow { get; set; }

        [JsonProperty(PropertyName = "Extension")]
        public string extension { get; set; }

        [JsonProperty(PropertyName = "IdEvento")]
        public string idEvento { get; set; }
        
        [JsonProperty("AccessRights")]
        public string accessRights { get; set; }

        //Se presente da visualizzare come da grafiche sotto l'icona dell'elemento
        public string estensioneDocumento
        {
            get
            {
                if (string.IsNullOrEmpty(extension) || extension.StartsWith("-", StringComparison.CurrentCulture))
                {
                    return null;
                }
                else
                {
                    return extension.ToUpper();
                }
            }
        }

        //Da visulizzare sotto al titolo del documento
        public string infoDocumento
        {
            get
            {
                return string.IsNullOrEmpty(ragione) ? "" : ragione.ToUpper();
            }
        }

        public override string GetData()
        {
            return dataDoc;
        }

        public override string GetEstensione()
        {
            return estensioneDocumento;
        }

        public override string GetInfo()
        {
            return infoDocumento;
        }

        public override string GetMittente()
        {
            return mittente;
        }

        public override string GetOggetto()
        {
            return oggetto;
        }

        public override string GetIdTrasmissione()
        {
            return idTrasm;
        }

        public override string getIdEvento()
        {
            return idEvento;
        }

        public override string GetIdDocumento()
        {
            return id;
        }

        public override string GetRagione(){
            return ragione;
        }
        
        public override SignatureInfo getSignatureInfo()
        {
            return SignatureInfo.Create(segnatura, id);
        }

        public override void SetFlags()
        {
            return;
        }
    }
}
