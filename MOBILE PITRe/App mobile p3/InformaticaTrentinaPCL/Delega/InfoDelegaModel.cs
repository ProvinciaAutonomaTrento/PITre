using System;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Delega
{
    public class InfoDelegaModel
    {
        [JsonProperty(PropertyName = "id_delega")]
        public string idDelega { get; set; }

        [JsonProperty(PropertyName = "id_utente_delegato")]
        public string idUtenteDelegato { get; set; }

        [JsonProperty(PropertyName = "cod_utente_delegato")]
        public string codUtenteDelegato { get; set; }

        [JsonProperty(PropertyName = "id_ruolo_delegato")]
        public string idRuoloDelegato { get; set; }

        [JsonProperty(PropertyName = "cod_ruolo_delegato")]
        public string codRuoloDelegato { get; set; }

        [JsonProperty(PropertyName = "id_utente_delegante")]
        public string idUtenteDelegante { get; set; }

        [JsonProperty(PropertyName = "cod_utente_delegante")]
        public string codUtenteDelegante { get; set; }

        [JsonProperty(PropertyName = "id_ruolo_delegante")]
        public string idRuoloDelegante { get; set; }

        [JsonProperty(PropertyName = "cod_ruolo_delegante")]
        public string codRuoloDelegante { get; set; }

        [JsonProperty(PropertyName = "id_people_corr_globali")]
        public string idPeopleCorrGlobali { get; set; }

        [JsonProperty(PropertyName = "id_uo_delegato")]
        public string idUoDelegato { get; set; }

        [JsonProperty(PropertyName = "dataDecorrenza")]
        public string dataDecorrenza { get; set; }

        [JsonProperty(PropertyName = "dataScadenza")]
        public string dataScadenza { get; set; }

        [JsonProperty(PropertyName = "inEsercizio")]
        public string inEsercizio { get; set; }

        [JsonProperty(PropertyName = "utDelegatoDismesso")]
        public string utDelegatoDismesso { get; set; }

        [JsonProperty(PropertyName = "utDeleganteDismesso")]
        public string utDeleganteDismesso { get; set; }

        [JsonProperty(PropertyName = "stato")]
        public string stato { get; set; }

        [JsonProperty(PropertyName = "codiceDelegante")]
        public string codiceDelegante { get; set; }

		public InfoDelegaModel(string idDelega, string idUtenteDelegato, string codUtenteDelegato, string idRuoloDelegato, string codRuoloDelegato, string idUtenteDelegante, string codUtenteDelegante, string idRuoloDelegante, string codRuoloDelegante, string idPeopleCorrGlobali, string idUoDelegato, string dataDecorrenza, string dataScadenza, string inEsercizio, string utDelegatoDismesso, string utDeleganteDismesso, string stato, string codiceDelegante)
		{
			this.idDelega = idDelega;
			this.idUtenteDelegato = idUtenteDelegato;
			this.codUtenteDelegato = codUtenteDelegato;
			this.idRuoloDelegato = idRuoloDelegato;
			this.codRuoloDelegato = codRuoloDelegato;
			this.idUtenteDelegante = idUtenteDelegante;
			this.codUtenteDelegante = codUtenteDelegante;
			this.idRuoloDelegante = idRuoloDelegante;
			this.codRuoloDelegante = codRuoloDelegante;
			this.idPeopleCorrGlobali = idPeopleCorrGlobali;
			this.idUoDelegato = idUoDelegato;
			this.dataDecorrenza = dataDecorrenza;
			this.dataScadenza = dataScadenza;
			this.inEsercizio = inEsercizio;
			this.utDelegatoDismesso = utDelegatoDismesso;
			this.utDeleganteDismesso = utDeleganteDismesso;
			this.stato = stato;
			this.codiceDelegante = codiceDelegante;
		}


    }
}
