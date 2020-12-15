using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetTemplatesResponse
    {
        public Domain.Template[] Templates { get; set; }
        public string ErrorMessage { get; set; }
        public GetTemplatesResponseCode Code { get; set; }
    }
    public enum GetTemplatesResponseCode { OK, SYSTEM_ERROR}
}