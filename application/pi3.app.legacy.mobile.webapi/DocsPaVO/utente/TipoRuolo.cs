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
    public class TipoRuolo 
	{
		[DataMember]
        public string systemId { get; set; }
        [DataMember]
        public string id_Amm { get; set; }
        [DataMember]
        public string codice { get; set; }
        [DataMember]
        public string descrizione { get; set; }
        [DataMember]
        public string livello { get; set; }
        [DataMember]
        public bool abilitato { get; set; }
        [DataMember]
        public TipoRuolo Parent { get; set; }
    }
}