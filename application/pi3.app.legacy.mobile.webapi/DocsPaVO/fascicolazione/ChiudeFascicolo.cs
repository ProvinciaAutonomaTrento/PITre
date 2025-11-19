// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.fascicolazione
{
    [Serializable()]
	[DataContract]
    public class ChiudeFascicolo
    {
		[DataMember]
        public string idPeople { get; set; }
        [DataMember]
        public string idCorrGlob_Ruolo { get; set; }
        [DataMember]
        public string idCorrGlob_UO { get; set; }
        [DataMember]
        public string uo_codiceCorrGlobali { get; set; }
        [DataMember]
        public string idPeopleDelegato { get; set; }

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
