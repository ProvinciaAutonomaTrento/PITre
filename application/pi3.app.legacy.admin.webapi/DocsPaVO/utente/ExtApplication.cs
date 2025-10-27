// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
    public class ExtApplication
	{
		/// <summary>
		/// id univoco Applicazione
		/// </summary>
		[DataMember]
        public string systemId { get; set; }
        /// <summary>
        /// codice Applicazione
        /// </summary>
		[DataMember]
        public string codice { get; set; }
        /// <summary>
        ///  descrizione Applicazione
        /// </summary>
		[DataMember]
        public string descrizione { get; set; }
		

		/// <summary>
		/// </summary>
		public ExtApplication()
		{
		}

		/// <summary>
		/// </summary>
		/// <param name="systemId"></param>
		/// <param name="codice"></param>
        /// <param name="descrizione"></param>
		public ExtApplication(string systemId,
					          string codice, 
                              string descrizione)
		{
			this.systemId=systemId;
            this.codice = codice;
            this.descrizione = descrizione;
		}
	}
}