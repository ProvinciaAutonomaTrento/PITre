using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class EditDocStateDiagramRequest
    {
        public string StateOfDiagram { get; set; }
        public string IdDocument { get; set; }
        public string Signature { get; set; }
    }
}