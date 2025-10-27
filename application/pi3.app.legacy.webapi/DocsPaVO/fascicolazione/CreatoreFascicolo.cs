// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;


namespace DocsPaVO.fascicolazione
{
    [Serializable()]
    public class CreatoreFascicolo
    {
        public string idPeople;
		public string idCorrGlob_Ruolo;
		public string idCorrGlob_UO;
        public string uo_codiceCorrGlobali;
        public string idPeopleDelegato;

		public CreatoreFascicolo()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public CreatoreFascicolo(string idPeople, string idRuolo, string idUo)
		{
			this.idPeople = idPeople;
			this.idCorrGlob_Ruolo = idRuolo;
			this.idCorrGlob_UO = idUo;			
		}
    }
}
