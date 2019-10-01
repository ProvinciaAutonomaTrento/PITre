using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.FilesUploader.UploaderVersion
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "UploaderVersion"
    /// </summary>
    [DataContract]
    public class UploaderVersionResponse : Response
    {
        /// <summary>
        /// Versione accettata di Uploader
        /// </summary>
        [DataMember]
        public string uploaderVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Link per il download di Uploader
        /// </summary>
        [DataMember]
        public string downloadLink
        {
            get;
            set;
        }
    }
}