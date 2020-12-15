using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Proceedings.AddDocToProceeding
{
    [DataContract]
    public class AddDocToProceedingRequest : Request
    {

        /// <summary>
        /// ID del procedimento
        /// </summary>
        [DataMember]
        public String IdProceeding
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
        /// Oggetto del documento
        /// </summary>
        [DataMember]
        public String DocumentObject
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
        /// Contenuto del file
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
        public Domain.File[] Attachment
        {
            get;
            set;
        }

    }
}