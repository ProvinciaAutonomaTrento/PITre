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
    public class Protocollo 
	{
		[DataMember]
        public string numero { get; set; }
        [DataMember]
        public string dataProtocollazione { get; set; }
        [DataMember]
        public string anno { get; set; }
        [DataMember]
        public string segnatura { get; set; }
        [DataMember]
        public string daProtocollare { get; set; }
        [DataMember]
        public string invioConferma { get; set; }
        [DataMember]
        public string modMittDest { get; set; }
        [DataMember]
        public string modMittInt { get; set; }
        [DataMember]
        public bool ModUffRef { get; set; } = false;
        //public bool modificaRispostaProtocollo = false;		
        [DataMember]
        public ProtocolloAnnullato protocolloAnnullato { get; set; }
        //public InfoDocumento rispostaProtocollo;
        [DataMember]
        public string descMezzoSpedizione { get; set; }
        [DataMember]
        public int mezzoSpedizione { get; set; }
        [DataMember]
        public string stampeEffettuate { get; set; }
        [DataMember]
        public bool isDestModificato { get; set; } = false;
        [DataMember]
        public bool isMittModificato { get; set; } = false;
        [DataMember]
        public bool isMittMultiModificato { get; set; } = false;
    }
}
