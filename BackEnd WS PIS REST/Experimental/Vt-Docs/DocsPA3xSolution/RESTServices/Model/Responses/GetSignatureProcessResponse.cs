using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetSignatureProcessResponse
    {
        public Domain.SignBook.SignatureProcess SignatureProcess { get; set; }
        public string ErrorMessage { get; set; }
        public GetSignatureProcessResponseCode Code { get; set; }
    }
    public enum GetSignatureProcessResponseCode { OK, SYSTEM_ERROR }
}