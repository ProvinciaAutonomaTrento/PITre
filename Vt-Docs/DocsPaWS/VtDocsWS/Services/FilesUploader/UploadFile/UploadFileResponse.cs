using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.FilesUploader.UploadFile
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "UploadFile"
    /// </summary>
    [DataContract]
    public class UploadFileResponse : Response
    {
        /// <summary>
        /// True se ha eseguto correttamente l'upload del file
        /// </summary>
        [DataMember]
        public bool IsSuccess
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco (descrizione e nome file)
        /// </summary>
        [DataMember]
        public string StrIdentity
        {
            get;
            set;
        }
    }
}