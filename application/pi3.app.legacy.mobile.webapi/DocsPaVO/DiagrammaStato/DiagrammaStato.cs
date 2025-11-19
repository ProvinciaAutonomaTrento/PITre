// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.DiagrammaStato
{
    [DataContract]
    public class DiagrammaStato
	{
        [DataMember]
        public int SYSTEM_ID { get; set; } = 0;
        [DataMember]
        public string DESCRIZIONE { get; set; } = "";
        [DataMember]
        public int ID_AMM { get; set; } = 0;

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.DiagrammaStato.Stato))]
        [DataMember]
        public ArrayList STATI { get; set; } = new ArrayList();

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.DiagrammaStato.Passo))]
        [DataMember]
        public ArrayList PASSI { get; set; } = new ArrayList();		
	}
}
