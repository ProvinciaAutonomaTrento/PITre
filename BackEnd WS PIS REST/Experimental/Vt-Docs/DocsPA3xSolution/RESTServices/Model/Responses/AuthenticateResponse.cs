using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class AuthenticateResponse
    {
        public AuthenticateResponse() { }
        public AuthenticateResponse(AuthenticateResponseCode code)
        {
            this.Code = code;
           
        }
        public AuthenticateResponseCode Code { get; set; }
        public string Token { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum AuthenticateResponseCode
    {
        OK, SYSTEM_ERROR
    }
}