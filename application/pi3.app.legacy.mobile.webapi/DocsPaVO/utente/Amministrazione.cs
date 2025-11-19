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
	public class Amministrazione 
	{
		[DataMember]
        public string systemId { get; set; }
        [DataMember]
        public string descrizione { get; set; }
        [DataMember]
        public string codice { get; set; }
        [DataMember]
        public string libreria { get; set; }
        [DataMember]
        public string email { get; set; } = string.Empty;
	}
}