using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Procedimento
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Proceeding
    {
        /// <summary>
        /// Id del procedimento
        /// </summary>
        [DataMember]
        public String Id
        {
            get;
            set;
        }

        /// <summary>
        /// Utente proprietario del procedimento
        /// </summary>
        [DataMember]
        public Domain.Correspondent User
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione istanza
        /// </summary>
        [DataMember]
        public String Description
        {
            get;
            set;
        }

        /// <summary>
        /// Codice di classifica del fascicolo da creare
        /// </summary>
        [DataMember]
        public String FolderCode
        {
            get;
            set;
        }

        /// <summary>
        /// Tipologia fascicolo
        /// </summary>
        [DataMember]
        public int IdFolderTypology
        {
            get;
            set;
        }

        /// <summary>
        /// Tipologia documento
        /// </summary>
        [DataMember]
        public int IdDocumentTypology
        {
            get;
            set;
        }

        /// <summary>
        /// Oggetto del documento
        /// </summary>
        [DataMember]
        public String DocumentObject
        {
            get;
            set;
        }

        /// <summary>
        /// Contenuto del documento
        /// </summary>
        [DataMember]
        public byte[] Content 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Allegati al documento
        /// </summary>
        [DataMember]
        public File[] Attachment
        {
            get;
            set;
        }

        /// <summary>
        /// Documenti non visualizzati
        /// </summary>
        [DataMember]
        public int UnreadDocuments
        {
            get;
            set;
        }
    }
}