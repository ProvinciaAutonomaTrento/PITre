// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.DiagrammaStato
{
    [DataContract]
    public class Passo
	{
        [DataMember]
        public int ID_DIAGRAMMA { get; set; }
        [DataMember]
        public Stato STATO_PADRE { get; set; }
        [DataMember]
        public string ID_STATO_AUTOMATICO { get; set; }
        [DataMember]
        public string ID_STATO_AUTOMATICO_LF { get; set; }
        [DataMember]
        public string DESCRIZIONE_STATO_AUTOMATICO { get; set; } = "";

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.DiagrammaStato.Stato))]
        [DataMember]
        public ArrayList SUCCESSIVI { get; set; } = new ArrayList();
	}
}
