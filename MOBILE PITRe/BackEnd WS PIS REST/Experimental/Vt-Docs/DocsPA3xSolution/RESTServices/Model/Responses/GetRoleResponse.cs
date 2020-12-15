using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetRoleResponse
    {
        public Domain.Role Role { get; set; }
        public string ErrorMessage { get; set; }
        public GetRoleResponseCode Code { get; set; }
    }
    public enum GetRoleResponseCode { OK,SYSTEM_ERROR}
}