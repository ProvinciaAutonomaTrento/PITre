using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.AddAttachment
{
    public class AddAttachmentResponse: Response
    {
        /// <summary>
        /// IdDocumento se Allegato / VersionId se add Version
        /// </summary>
        [DataMember]
        public string IdObject
        {
            get;
            set;
        }
    }
}