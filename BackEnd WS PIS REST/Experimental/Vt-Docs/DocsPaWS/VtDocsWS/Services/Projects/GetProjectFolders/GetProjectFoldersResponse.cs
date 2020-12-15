using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.GetProjectFolders
{
    [DataContract]
    public class GetProjectFoldersResponse : Response
    {
        [DataMember]
        public string IdProject { get; set; }

        [DataMember]
        public string ProjectDescription { get; set; }

        [DataMember]
        public Domain.Folder[] Folders
        {
            get;
            set;
        }
    }
}