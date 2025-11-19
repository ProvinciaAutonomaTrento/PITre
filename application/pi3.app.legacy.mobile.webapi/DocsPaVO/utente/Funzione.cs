// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	[DataContract]
	public class Funzione
	{
		[DataMember]
        public string systemId { get; set; }
        [DataMember]
        public string codice { get; set; }
        [DataMember]
        public string descrizione { get; set; }
        [DataMember]
        public string idTipoFunzione { get; set; }
        [DataMember]
        public string codTipoFunzione { get; set; }
        [DataMember]
        public string descTipoFunzione { get; set; }
        [DataMember]
        public string idParent { get; set; }

        /// <summary>
        /// </summary>
        public Funzione()
		{
		}

		/// <summary>
		/// </summary>
		/// <param name="systemId"></param>
		/// <param name="descrizione"></param>
		/// <param name="codice"></param>
		/// <param name="idTipoFunzione"></param>
		/// <param name="codTipoFunzione"></param>
		/// <param name="descTipoFunzione"></param>
		public Funzione(string systemId,
						string descrizione,
						string codice,
						string idTipoFunzione,
						string codTipoFunzione,
						string descTipoFunzione) 
		{
			this.systemId         = systemId;
			this.descrizione	  = descrizione;
			this.codice			  = codice;
			this.idTipoFunzione   = idTipoFunzione;
			this.codTipoFunzione  = codTipoFunzione;
            this.descTipoFunzione = descTipoFunzione;
		}
	}
}