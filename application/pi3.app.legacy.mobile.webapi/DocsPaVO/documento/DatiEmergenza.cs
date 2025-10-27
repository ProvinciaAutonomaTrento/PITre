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
	[XmlType("DocumentoDatiEmergenza")]
    [Serializable()]
	[DataContract]
	public class DatiEmergenza 
	{
		[DataMember]
        public string protocolloEmergenza { get; set; }
        [DataMember]
        public string dataProtocollazioneEmergenza { get; set; }
        [DataMember]
        public string nomeProtocollatoreEmergenza { get; set; }
        [DataMember]
        public string cognomeProtocollatoreEmergenza { get; set; }
    }
}