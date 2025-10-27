// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Runtime.Serialization;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Summary description for OrgTipoRuolo.
	/// </summary>
	[DataContract]
    public class OrgTipoRuolo
	{
		[DataMember]
        public string IDTipoRuolo { get; set; } = string.Empty;

        [DataMember]
        public string Codice { get; set; } = string.Empty;

        [DataMember]
        public string Descrizione { get; set; } = string.Empty;

        [DataMember]
        public string Livello { get; set; } = string.Empty;

        [DataMember]
        public string IDAmministrazione { get; set; } = string.Empty;
	}
}
