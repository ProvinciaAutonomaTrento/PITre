using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// FileInUpload
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class FileInUpload
    {
        /// <summary>
        /// Identificativo univoco (descrizione e nome file)
        /// </summary>
        [DataMember]
        public string strIdentity
        {
            get;
            set;
        }

        /// <summary>
        /// Nome macchina su cui è iniziato l'upload
        /// </summary>
        [DataMember]
        public string machineName
        {
            get;
            set;
        }

        /// <summary>
        /// Hash del file (completo) in dwnload
        /// </summary>
        [DataMember]
        public string fileHash
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del file in download
        /// </summary>
        [DataMember]
        public string fileName
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del file in download
        /// </summary>
        [DataMember]
        public string fileDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Percorso del file sulla macchina di partenza
        /// </summary>
        [DataMember]
        public string fileSenderPath
        {
            get;
            set;
        }

        /// <summary>
        /// Dimensioni del file completo
        /// </summary>
        [DataMember]
        public long fileSize
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di parti
        /// </summary>
        [DataMember]
        public int totalChunkNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Numero di parte file in download
        /// </summary>
        [DataMember]
        public int chunkNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Ordine nella lista dei download
        /// </summary>
        [DataMember]
        public int order
        {
            get;
            set;
        }

        /// <summary>
        /// Id ruolo con visibilità sul file
        /// </summary>
        [DataMember]
        public string idRuolo
        {
            get;
            set;
        }

        /// <summary>
        /// Content della parte in download
        /// </summary>
        [DataMember]
        public byte[] fileContent
        {
            get;
            set;
        }
    }
}