using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.fascicolazione
{
    [Serializable()]
    public class ChiudeFascicolo
    {
                public string idPeople;
		public string idCorrGlob_Ruolo;
		public string idCorrGlob_UO;
        public string uo_codiceCorrGlobali;
        public string idPeopleDelegato;

		public ChiudeFascicolo()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public ChiudeFascicolo(string idPeople, string idRuolo, string idUo)
		{
			this.idPeople = idPeople;
			this.idCorrGlob_Ruolo = idRuolo;
			this.idCorrGlob_UO = idUo;			
		}

        public ChiudeFascicolo(string idPeople, string idRuolo, string idUo, string codiceUo) : this(idPeople, idRuolo, idUo)
        {
            this.uo_codiceCorrGlobali = codiceUo;
        }

        public ChiudeFascicolo(DocsPaVO.utente.InfoUtente utente, DocsPaVO.utente.Ruolo ruolo)
		{
			this.idPeople = utente.idPeople;
			this.idCorrGlob_Ruolo = ruolo.systemId;
			this.idCorrGlob_UO = ruolo.uo.systemId;
            this.uo_codiceCorrGlobali = ruolo.uo.codice;
            if (utente.delegato != null)
                this.idPeopleDelegato = utente.delegato.idPeople;
            else
                this.idPeopleDelegato = "0";
		}
    }
}
