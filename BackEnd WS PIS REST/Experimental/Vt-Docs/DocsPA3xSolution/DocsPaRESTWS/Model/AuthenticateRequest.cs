using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class AuthenticateRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public string idAmministrazione { get; set; }
        public string Otp { get; set; }
        public string Version { get; set; }

    }
}