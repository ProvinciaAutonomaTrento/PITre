using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class UploadFileToDocumentResponse
    {
        public string ErrorMessage { get; set; }
        public UploadFileToDocumentResponseCode Code { get; set; }
        public string ResultMessage { get; set; }
    }

    public enum UploadFileToDocumentResponseCode { OK, SYSTEM_ERROR}
}