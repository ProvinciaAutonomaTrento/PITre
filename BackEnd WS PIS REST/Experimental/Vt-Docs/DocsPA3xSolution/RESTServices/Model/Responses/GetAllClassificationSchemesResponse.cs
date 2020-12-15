using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetAllClassificationSchemesResponse
    {
        public Domain.ClassificationScheme[] ClassificationSchemes { get; set; }
        public string ErrorMessage { get; set; }
        public GetAllClassificationSchemesResponseCode Code { get; set; }
    }
    public enum GetAllClassificationSchemesResponseCode { OK,SYSTEM_ERROR}
}