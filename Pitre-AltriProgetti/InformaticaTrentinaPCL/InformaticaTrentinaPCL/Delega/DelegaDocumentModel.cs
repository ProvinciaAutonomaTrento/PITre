using System;
using Newtonsoft.Json;
using InformaticaTrentinaPCL.Utils;
using InformaticaTrentinaPCL.Delega;

namespace InformaticaTrentinaPCL.Home
{
    public class DelegaDocumentModel
    {

        [JsonProperty(PropertyName = "DataDecorrenza")]
        private DateTime dataDecorrenza { get; set; }

        [JsonProperty(PropertyName = "DataScadenza")]
        private DateTime dataScadenza { get; set; }

        [JsonProperty(PropertyName = "InEsercizio")]
        public string inEsercizio { get; set; }

        [JsonProperty(PropertyName = "Stato")]
        public int stato { get; set; }

        [JsonProperty(PropertyName = "Delegato")]
        public string delegato { get; set; }

        [JsonProperty(PropertyName = "IdDelegato")]
        public string idDelegato { get; set; }

        [JsonProperty(PropertyName = "RuoloDelegato")]
        public string ruoloDelegato { get; set; }

        [JsonProperty(PropertyName = "InfoDelega")]
        public InfoDelegaModel infoDelega { get; set; }

        [JsonProperty(PropertyName = "Id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "IdRuoloDelegante")]
        public string idRuoloDelegante { get; set; }

        [JsonProperty(PropertyName = "Delegante")]
        public string delegante { get; set; }

        [JsonProperty(PropertyName = "CodiceDelegante")]
        public string codiceDelegante { get; set; }

        [JsonProperty(PropertyName = "idRuoloDelegato")]
        public string idRuoloDelegato { get; set; }

		public DelegaDocumentModel() { }

        public DelegaDocumentModel(DateTime dataDecorrenza, DateTime dataScadenza, string inEsercizio, int stato, string delegato, string idDelegato, string ruoloDelegato, InfoDelegaModel infoDelega, string id, string idRuoloDelegante, string delegante, string codiceDelegante, string idRuoloDelegato=null)
        {
            this.dataDecorrenza = dataDecorrenza;
            this.dataScadenza = dataScadenza;
            this.inEsercizio = inEsercizio;
            this.stato = stato;
            this.delegato = delegato;
            this.idDelegato = idDelegato;
            this.ruoloDelegato = ruoloDelegato;
            this.infoDelega = infoDelega;
            this.id = id;
            this.idRuoloDelegante = idRuoloDelegante;
            this.delegante = delegante;
            this.codiceDelegante = codiceDelegante;
            this.idRuoloDelegato = idRuoloDelegato;
        }

        /// <summary>
        /// COstruttore per l'oggetto che viene utilizzato per la creazione di una Delega.
        /// idRuoloDelegante può contenere 'TUTTI' per generare una delega per tutti i ruoli oppure se 'null' 
        /// la delega verrà creata per il ruolo con il quale si è fatto l'accesso
        /// </summary>
        /// <param name="dataDecorrenza">Data decorrenza.</param>
        /// <param name="dataScadenza">Data scadenza.</param>
        /// <param name="idDelegato">Identifier delegato.</param>
        /// <param name="idRuoloDelegante">Identifier ruolo delegante.</param>
        public DelegaDocumentModel(DateTime dataDecorrenza, DateTime dataScadenza, string idDelegato, string idRuoloDelegante = null, string idRuoloDelegato=null)
        {
            this.dataDecorrenza = dataDecorrenza;
            this.dataScadenza = dataScadenza;
            this.idDelegato = idDelegato;
            this.idRuoloDelegante = idRuoloDelegante;
            this.idRuoloDelegato = idRuoloDelegato;

        }

        public string dataDecorrenzaDelega
        {
            get
            {
                return dataDecorrenza.ToReadableTimeString();
            }
            set
            {
                dataDecorrenza = DateTime.Parse(value);
            }
        }

        public string dataScadenzaDelega
        {
            get
            {
                return dataScadenza.ToReadableTimeString();
            }
            set
            {
                dataScadenza = DateTime.Parse(value);
            }
        }

        public DateTime GetDateTimeDecorrenza { get { return dataDecorrenza; } }
        public DateTime GetDateTimeScadenza { get { return dataScadenza; } }
    }
}
