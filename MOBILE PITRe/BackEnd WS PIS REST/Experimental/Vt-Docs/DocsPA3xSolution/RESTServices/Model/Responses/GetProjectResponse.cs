using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetProjectResponse
    {
        public Domain.Project Project { get; set; }
        public string ErrorMessage { get; set; }
        public GetProjectResponseCode Code { get; set; }
    }
    public enum GetProjectResponseCode { OK, SYSTEM_ERROR}
}