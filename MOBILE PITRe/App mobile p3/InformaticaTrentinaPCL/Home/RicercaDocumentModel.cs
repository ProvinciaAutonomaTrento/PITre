using System;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Home
{
    public class RicercaDocumentModel : AbstractDocumentListItem
    {
        [JsonProperty(PropertyName = "Id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "Note")]
        public string note { get; set; }

        [JsonProperty(PropertyName = "TipoProto")]
        public string tipoProto { get; set; }

        [JsonProperty(PropertyName = "Segnatura")]
        public string segnatura { get; set; }

        [JsonProperty(PropertyName = "Extension")]
        public string extension { get; set; }

        [JsonProperty(PropertyName = "Oggetto")]
        public string oggetto { get; set; }

        [JsonProperty(PropertyName = "Data")]
        public string data { get; set; }
            
        public RicercaDocumentModel(string id, int tipo, string note, string tipoProto, string segnatura, string extension, string oggetto)
        {
            this.id = id;
            this.tipo = tipo;
            this.note = note;
            this.tipoProto = tipoProto;
            this.segnatura = segnatura;
            this.extension = extension;
            this.oggetto = oggetto;
        }

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

        public override string GetData()
        {
            //TODO check property
            return data;
        }

        public override string GetEstensione()
        {
            return estensioneDocumento;
        }

        public override string GetInfo()
        {
            //TODO check property
            return "";
        }

        public override string GetMittente()
        {
            //TODO check property
            return "";
        }

        public override string GetOggetto()
        {
            return oggetto;
        }

        public override string GetIdTrasmissione()
        {
            //TODO check property
            return "";
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
            return SignatureInfo.Create(segnatura, id);
        }

        public override void SetFlags()
        {
            return;
        }
    }
}
