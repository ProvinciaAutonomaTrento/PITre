using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace VtDocsWS.Services.CAPServices.DownloadDoc
{
    [DataContract]
    public class DownloadDocResponse : Response
    {
        [DataMember]
        public Domain.File FileDoc { get; set; }
    }
}