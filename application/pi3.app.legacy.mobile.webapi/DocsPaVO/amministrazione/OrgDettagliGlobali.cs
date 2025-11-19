// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Runtime.Serialization;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Summary description for OrgDettagliGlobali.
	/// </summary>
	[DataContract]
	public class OrgDettagliGlobali
	{
		[DataMember]
        public string Indirizzo { get; set; } = string.Empty;

        [DataMember]
        public string Citta { get; set; } = string.Empty;

        [DataMember]
        public string Provincia { get; set; } = string.Empty;

        [DataMember]
        public string Cap { get; set; } = string.Empty;

        [DataMember]
        public string Nazione { get; set; } = string.Empty;

        [DataMember]
        public string CodiceFiscale { get; set; } = string.Empty;

        [DataMember]
        public string PartitaIva { get; set; } = string.Empty;

        [DataMember]
        public string Telefono1 { get; set; } = string.Empty;

        [DataMember]
        public string Telefono2 { get; set; } = string.Empty;

        [DataMember]
        public string Fax { get; set; } = string.Empty;

        [DataMember]
        public string Note { get; set; } = string.Empty;

	}
}
