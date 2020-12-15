using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetStampResponse
    {
        public Domain.Stamp Stamp { get; set; }
        public string ErrorMessage { get; set; }
        public GetStampResponseCode Code { get; set; }
    }
    public enum GetStampResponseCode { OK, SYSTEM_ERROR }
}