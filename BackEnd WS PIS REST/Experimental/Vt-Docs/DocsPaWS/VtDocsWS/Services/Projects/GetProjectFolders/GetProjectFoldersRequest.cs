using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.GetProjectFolders
{
    [DataContract]
    public class GetProjectFoldersRequest : Request
    {
        /// <summary>
        /// Id del fascicolo
        /// </summary>
        [DataMember]
        public string IdProject
        {
            get;
            set;
        }

        /// <summary>
        /// Id del titolario
        /// </summary>
        [DataMember]
        public string ClassificationSchemeId
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del fascicolo
        /// </summary>
        [DataMember]
        public string CodeProject
        {
            get;
            set;
        }
    }
}