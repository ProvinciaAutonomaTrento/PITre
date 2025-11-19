// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.Modelli_Trasmissioni
{
	[DataContract]
	public class RagioneDest
	{
		[DataMember]
        public string RAGIONE { get; set; }
        [DataMember]
        public string CHA_TIPO_RAGIONE { get; set; }


        [XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.Modelli_Trasmissioni.MittDest))]
		public System.Collections.ArrayList DESTINATARI { get; set; } = new System.Collections.ArrayList();	
	}
}
