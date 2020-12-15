using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class GiveUpRightsRequest
    {
        // string rightToKeep, string idObject
        public string RightToKeep { get; set; }
        public string IdObject { get; set; }
    }
}