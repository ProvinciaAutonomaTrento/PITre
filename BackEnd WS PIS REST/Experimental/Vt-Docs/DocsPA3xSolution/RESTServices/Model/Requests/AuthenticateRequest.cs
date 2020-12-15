using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class AuthenticateRequest
    {
        public string Username { get; set; }
        public string CodeRole { get; set; }
        public string CodeApplication { get; set; }
        public string CodeAdm { get; set; }
    }
}