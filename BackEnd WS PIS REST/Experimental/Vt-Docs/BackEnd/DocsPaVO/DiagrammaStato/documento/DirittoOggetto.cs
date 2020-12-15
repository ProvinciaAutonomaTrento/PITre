using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
    /// <summary>
    /// </summary>
    [XmlType("DocumentoDiritto")]
    [Serializable()]
    public class DirittoOggetto
    {
        public string idObj;
        public TipoDiritto tipoDiritto;
        public DocsPaVO.utente.Corrispondente soggetto;
        public int accessRights;
        public bool deleted;
        public string note;
        public string personorgroup;
        public string description;
        public bool hideDocVersions;
        public string dtaInsSecurity;
        public string noteSecurity;
		public string removed;
		public string daReinserire;
    }
}