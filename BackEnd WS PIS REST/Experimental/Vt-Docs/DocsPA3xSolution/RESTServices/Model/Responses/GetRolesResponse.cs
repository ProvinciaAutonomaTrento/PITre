using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetRolesResponse
    {
     
        public Domain.Role[] Roles { get; set; }
        public string ErrorMessage { get; set; }
        public GetRolesResponseCode Code { get; set; }
    }
    public enum GetRolesResponseCode { OK,SYSTEM_ERROR}
}