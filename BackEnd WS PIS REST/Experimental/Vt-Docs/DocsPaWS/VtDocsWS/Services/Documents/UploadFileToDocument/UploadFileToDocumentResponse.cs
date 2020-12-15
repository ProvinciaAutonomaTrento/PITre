using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.UploadFileToDocument
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "UploadFileToDocumentResponse"
    /// </summary>
   [DataContract]
    public class UploadFileToDocumentResponse : Response
    {
       public string IdObject;
    }
}