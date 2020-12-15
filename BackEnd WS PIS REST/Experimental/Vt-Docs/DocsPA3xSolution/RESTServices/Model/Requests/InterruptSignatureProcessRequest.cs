using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class InterruptSignatureProcessRequest
    {
        public string IdSignProcessInstance { get; set; }

        public string InterruptionNote { get; set; }
    }
}