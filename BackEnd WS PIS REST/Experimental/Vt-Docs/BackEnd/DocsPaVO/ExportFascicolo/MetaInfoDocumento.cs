using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.ExportFascicolo
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class MetaInfoDocumento
    {
        /// <summary>
        /// 
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
        [XmlAttribute(AttributeName = "fullName")]
        public string FullName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "isProtocollo")]
        public bool IsProtocollo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "isAllegato")]
        public bool IsAllegato { get; set; }
    }
}
