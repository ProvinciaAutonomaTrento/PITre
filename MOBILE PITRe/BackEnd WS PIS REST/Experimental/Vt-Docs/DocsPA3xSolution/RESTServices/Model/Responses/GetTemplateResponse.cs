using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetTemplateResponse
    {
        public Domain.Template Template { get; set; }
        public string ErrorMessage { get; set; }
        public GetTemplateResponseCode Code { get; set; }
    }
    public enum GetTemplateResponseCode { OK, SYSTEM_ERROR}
}