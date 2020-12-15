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
    public class ContentDocumento
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "fileExtension")]
        public string FileExtension { get; set; }

        /// <summary>
        /// Contenuto del documento
        /// </summary>
        [XmlAttribute(AttributeName = "fileContent")]
        public byte[] FileContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "mimeType")]
        public string MimeType { get; set; }
    }
}
