using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RESTServices.Model.Domain;

namespace RESTServices.Model.Responses
{
    public class GetDocumentResponse
    {
        public Document Document { get; set; }

        public GetDocumentResponseCode Code { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum GetDocumentResponseCode { OK, SYSTEM_ERROR }
}