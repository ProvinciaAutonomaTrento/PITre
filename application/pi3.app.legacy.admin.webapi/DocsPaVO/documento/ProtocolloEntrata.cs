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
	public class ProtocolloEntrata : Protocollo 
	{
		[DataMember]
        public DocsPaVO.utente.Corrispondente mittente { get; set; }
        [DataMember]
        public DocsPaVO.utente.Corrispondente mittenteIntermedio { get; set; }
        [DataMember]
        public DocsPaVO.utente.Corrispondente ufficioReferente { get; set; }
        [DataMember]
        public string descrizioneProtocolloMittente { get; set; }
        [DataMember]
        public string dataProtocolloMittente { get; set; }
        [DataMember]
        public bool daAggiornareMittente { get; set; } = false;
        [DataMember]
        public bool daAggiornareMittenteIntermedio { get; set; } = false;
        [DataMember]
        public string emailMittente { get; set; } = string.Empty;

        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.utente.Corrispondente))]
        [DataMember]
        public System.Collections.ArrayList mittenti { get; set; }
        [DataMember]
        public bool daAggiornareMittentiMultipli { get; set; } = false;
	}
}