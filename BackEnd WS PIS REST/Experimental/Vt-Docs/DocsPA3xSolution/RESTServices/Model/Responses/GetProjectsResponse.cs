using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetProjectsResponse
    {
        public Domain.Project[] Projects { get; set; }
        public string ErrorMessage { get; set; }
        public GetProjectsResponseCode Code { get; set; }
    }
    public enum GetProjectsResponseCode { OK, SYSTEM_ERROR }
}