using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.fascicolazione
{
    /// <summary>
    /// </summary>
    [Serializable()]
    public class Folder
    {
        [XmlArray()]
        [XmlArrayItem(typeof(Folder))]
        public System.Collections.ArrayList childs;

        public string systemID;
        public string idFascicolo;
        public string idParent;
        public string descrizione;
        public string dtaApertura;
        public string livello;
        public string codicelivello;

        /// <summary>
        /// </summary>
        public Folder()
        {
            childs= new System.Collections.ArrayList();
        }
    }
}
