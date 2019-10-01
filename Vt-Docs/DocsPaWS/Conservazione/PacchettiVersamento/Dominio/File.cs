using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Dominio
{
    /// <summary>
    /// Classe per la gestione del contenuto di un file
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/File")]
    public class File
    {
        /// <summary>
        /// 
        /// </summary>
        public string Nome
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Contenuto
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string TipoMime
        {
            get;
            set;
        }
    }
}