using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Oggetto Request del servizio di download di un file associato ad una versione del documento
    /// </summary>
    [Serializable()]
    public class DownloadFileRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        public string IdProfile
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string DocNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Version
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string VersionLabel
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string VersionId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string RepositoryContextToken
        {
            get;
            set;
        }
    }
}
