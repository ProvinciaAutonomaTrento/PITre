using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class StartSignatureProcessRequest
    {
        public Domain.SignBook.SignatureProcess SignatureProcess { get; set; }

        public string IdDocument { get; set; }

        public string Note { get; set; }

        public bool InterruptionGeneratesNote { get; set; }

        public bool EndGeneratesNote { get; set; }
    }
}