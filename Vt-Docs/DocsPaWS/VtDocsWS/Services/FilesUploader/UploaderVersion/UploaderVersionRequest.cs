using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.FilesUploader.UploaderVersion
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "UploaderVersion"
    /// </summary>
    [DataContract]
    public class UploaderVersionRequest : Request
    {
        /// <summary>
        /// Uploader version
        /// </summary>
        [DataMember]
        public string uploaderVersion
        {
            get;
            set;
        }
    }
}