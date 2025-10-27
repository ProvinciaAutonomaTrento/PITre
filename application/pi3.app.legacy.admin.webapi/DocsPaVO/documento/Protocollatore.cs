// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
	/// <summary>
	/// Summary description for Protocollatore.
	/// </summary>
    [Serializable()]
	[DataContract]
	public class Protocollatore
	{
		[DataMember]
		public string utente_idPeople { get; set; }
        [DataMember]
        public string ruolo_idCorrGlobali { get; set; }
        [DataMember]
        public string uo_idCorrGlobali { get; set; }
        [DataMember]
        public string uo_codiceCorrGlobali { get; set; }

        public Protocollatore()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public Protocollatore(string idPeople, string idRuolo, string idUo, string codiceUo)
		{
			this.utente_idPeople = idPeople;
			this.ruolo_idCorrGlobali = idRuolo;
			this.uo_idCorrGlobali = idUo;
			this.uo_codiceCorrGlobali = codiceUo;
		}

		public Protocollatore(DocsPaVO.utente.InfoUtente utente, DocsPaVO.utente.Ruolo ruolo)
		{
			this.utente_idPeople = utente.idPeople;
			this.ruolo_idCorrGlobali = ruolo.systemId;
			this.uo_idCorrGlobali = ruolo.uo.systemId;
			this.uo_codiceCorrGlobali = ruolo.uo.codice; //VAR_CODICE della CORR_GLOBALI 
		}
	}
}
