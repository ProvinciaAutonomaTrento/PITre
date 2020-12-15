using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetCorrespondentResponse
    {
        public Domain.Correspondent Correspondent { get; set; }
        public string ErrorMessage { get; set; }
        public GetCorrespondentResponseCode Code { get; set; }
    }
    public enum GetCorrespondentResponseCode { OK , SYSTEM_ERROR}
}