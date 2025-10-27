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
	[XmlType("DocumentoParolaChiave")]
    [Serializable()]
	[DataContract]
	public class ParolaChiave 
	{
		[DataMember]
		public string systemId { get; set; }
        [DataMember]
        public string descrizione { get; set; }
        [DataMember]
        public string idAmministrazione { get; set; }
        [DataMember]
        public string idRegistro { get; set; }
    }
}