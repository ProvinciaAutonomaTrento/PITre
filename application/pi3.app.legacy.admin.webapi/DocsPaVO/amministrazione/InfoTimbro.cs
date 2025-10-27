// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class InfoTimbro
    {
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.amministrazione.posizione))]
        [DataMember]
        public ArrayList positions { get; set; } = new ArrayList();
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.amministrazione.carattere))]
        [DataMember]
        public ArrayList carattere { get; set; } = new ArrayList();
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.amministrazione.color))]
        [DataMember]
        public ArrayList color { get; set; } = new ArrayList();
        //[XmlArray()]
        //[XmlArrayItem(typeof(DocsPaVO.amministrazione.position))]
        //public ArrayList positions = new ArrayList();
    }

    public class posizione
    {
        public string id;
        public string posName;
        public string PosX;
        public string PosY;
    }

    public class color
    {
        public string id;
        public string colName;
        public string descrizione;
    }

    public class carattere
    {
        public string id;
        public string caratName;
        public string dimensione;
    }
}
