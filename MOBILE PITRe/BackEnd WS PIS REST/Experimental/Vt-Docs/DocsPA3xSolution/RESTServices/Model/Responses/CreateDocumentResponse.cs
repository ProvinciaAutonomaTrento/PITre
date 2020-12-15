using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class CreateDocumentResponse
    {
        public Domain.Document Document { get; set; }

        public CreateDocumentResponseCode Code { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum CreateDocumentResponseCode { OK,SYSTEM_ERROR}
}