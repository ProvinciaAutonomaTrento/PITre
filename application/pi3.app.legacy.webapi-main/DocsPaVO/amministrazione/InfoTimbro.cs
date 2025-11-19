// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// 
    /// </summary>
    public class InfoTimbro
    {
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.amministrazione.posizione))]
        public DocsPaVO.amministrazione.posizione[] positions;
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.amministrazione.carattere))]
        public DocsPaVO.amministrazione.carattere[] carattere;
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.amministrazione.color))]
        public DocsPaVO.amministrazione.color[] color;
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
