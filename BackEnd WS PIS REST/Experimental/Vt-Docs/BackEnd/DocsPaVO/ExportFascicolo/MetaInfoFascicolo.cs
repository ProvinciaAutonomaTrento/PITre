using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.ExportFascicolo
{
    /// <summary>
    /// Contiene i metadati di un fascicolo 
    /// necessari per la funzione di export su fileSystem
    /// </summary>
    [Serializable()]
    public class MetaInfoFascicolo
    {
        /// <summary>
        /// Id univoco del fascicolo
        /// </summary>
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "nome")]
        public string Nome { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MetaInfoFascicolo[] Fascicoli { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MetaInfoDocumento[] Documenti { get; set; }
    }
}
