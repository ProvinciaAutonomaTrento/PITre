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
        public ArrayList positions = new ArrayList();
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.amministrazione.carattere))]
        public ArrayList carattere = new ArrayList();
        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.amministrazione.color))]
        public ArrayList color = new ArrayList();
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
