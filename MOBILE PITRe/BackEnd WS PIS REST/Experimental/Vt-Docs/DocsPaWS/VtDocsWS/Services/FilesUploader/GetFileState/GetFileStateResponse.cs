using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.FilesUploader.GetFilesState
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetFilesState"
    /// </summary>
    [DataContract]
    public class GetFilesStateResponse : Response
    {
        /// <summary>
        /// File in upload
        /// </summary>
        [DataMember]
        public List<Domain.FileInUpload> FileInUp
        {
            get;
            set;
        }

        /// <summary>
        /// True se ha eseguto correttamente l'upload del file
        /// </summary>
        [DataMember]
        public bool IsSuccess
        {
            get;
            set;
        }
    }
}