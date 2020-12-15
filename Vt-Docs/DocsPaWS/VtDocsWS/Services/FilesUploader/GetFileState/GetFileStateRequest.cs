using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.FilesUploader.GetFilesState
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetFilesState"
    /// </summary>
    [DataContract]
    public class GetFilesStateRequest : Request
    {
        /// <summary>
        /// Nome macchina
        /// </summary>
        [DataMember]
        public string MachineName
        {
            get;
            set;
        }
    }
}