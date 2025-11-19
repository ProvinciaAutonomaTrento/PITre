// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
	public class ProtocolloUscita : Protocollo 
	{
		[DataMember]
        public bool daAggiornareDestinatari { get; set; } = false;
        [DataMember]
        public bool daAggiornareDestinatariConoscenza { get; set; } = false;
        [DataMember]
        public bool daAggiornareMittente { get; set; } = false;
        [DataMember]
        public DocsPaVO.utente.Corrispondente mittente { get; set; }
        [DataMember]
        public DocsPaVO.utente.Corrispondente ufficioReferente { get; set; }

        [XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.utente.Corrispondente))]
        [DataMember]
        public System.Collections.ArrayList destinatari { get; set; }

        [XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.utente.Corrispondente))]
        [DataMember]
        public System.Collections.ArrayList destinatariConoscenza { get; set; }
    }
}