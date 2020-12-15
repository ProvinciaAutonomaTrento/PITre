using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class SendDocAdvRequest
    {
        public string IdDocument { get; set; }

        public string Signature { get; set; }

        public string IdRegister { get; set; }

        public string CodeRegister { get; set; }

        public string SenderMail { get; set; }

        public Domain.Correspondent[] Recipients { get; set; }
    }
}