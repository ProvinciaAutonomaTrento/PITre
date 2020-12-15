using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.RemoveProjectFolders
{
    public class RemoveProjectFoldersRequest :Request
    {
        [DataMember]
        public Domain.Folder Folder
        {
            get;
            set;
        }
    }
}