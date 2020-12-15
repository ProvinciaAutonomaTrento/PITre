using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class AddDocInProjectRequest
    {
        public string IdDocument { get; set; }
        public string IdProject { get; set; }
        public string CodeProject { get; set; }
    }
}