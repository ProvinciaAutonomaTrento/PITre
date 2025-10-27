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
	public class ProtocolloDestinatario 
	{
		[DataMember]
		public string systemId { get; set; }
        [DataMember]
        public string codiceAOO { get; set; }
        [DataMember]
        public string protocolloDestinatario { get; set; }
        [DataMember]
        public string dataProtocolloDestinatario { get; set; }
        [DataMember]
        public string codiceAmm { get; set; }
        [DataMember]
        public string descrizioneCorr { get; set; }
        [DataMember]
        public string documentType { get; set; }
        [DataMember]
        public string dta_spedizione { get; set; }
        [DataMember]
        public string annullato { get; set; }
        [DataMember]
        public string motivo { get; set; }
        [DataMember]
        public string provvedimento { get; set; }
    }
}