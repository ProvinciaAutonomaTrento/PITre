using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetUsersResponse
    {
        public Domain.User[] Users { get; set; }
        public string ErrorMessage { get; set; }
        public GetUsersResponseCode Code { get; set; }
    }
    public enum GetUsersResponseCode { OK,SYSTEM_ERROR}
}