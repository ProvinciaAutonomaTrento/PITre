// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Runtime.Serialization;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Summary description for EsitoOperazione.
	/// </summary>
	[DataContract]
	public class EsitoOperazione
	{
		[DataMember]
		public int Codice { get; set; } = 0;
        [DataMember]
        public string Descrizione { get; set; } = String.Empty;
        [DataMember]
        public int numDocCopiati { get; set; } = 0;
        [DataMember]
        public int numeroFascCopiati { get; set; } = 0;
        [DataMember]
        public int numeroDocinFascCopiati { get; set; } = 0;
    }
}
