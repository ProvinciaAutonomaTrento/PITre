using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetClassificationSchemeResponse
    {
        public Domain.ClassificationScheme ClassificationScheme { get; set; }
        public string ErrorMessage { get; set; }
        public GetClassificationSchemeResponseCode Code { get; set; }
    }

    public enum GetClassificationSchemeResponseCode { OK, SYSTEM_ERROR}
}