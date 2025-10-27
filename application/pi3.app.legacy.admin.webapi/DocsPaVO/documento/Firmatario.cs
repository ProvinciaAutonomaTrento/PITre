// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	[DataContract]
	public class Firmatario 
	{
		[DataMember]
        public string systemId { get; set; }
        [DataMember]
        public string nome { get; set; }
        [DataMember]
        public string cognome { get; set; }
        [DataMember]
        public string codiceFiscale { get; set; }
        [DataMember]
        public string dataNascita { get; set; }
        [DataMember]
        public string identificativoCA { get; set; }
        [DataMember]
        public int livello { get; set; }
    }
}