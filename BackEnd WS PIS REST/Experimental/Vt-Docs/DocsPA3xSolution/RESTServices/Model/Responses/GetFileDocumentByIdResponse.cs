using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetFileDocumentByIdResponse
    {
        public Domain.File File { get; set; }
        public GetFileDocumentByIdResponseCode Code { get; set; }
        public string ErrorMessage { get; set; }

    }
    public enum GetFileDocumentByIdResponseCode { OK, SYSTEM_ERROR}
}