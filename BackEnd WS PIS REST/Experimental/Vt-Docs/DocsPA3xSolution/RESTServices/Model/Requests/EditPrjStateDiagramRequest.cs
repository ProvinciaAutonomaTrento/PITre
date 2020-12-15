using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class EditPrjStateDiagramRequest
    {
        //string stateOfDiagram, string idProject = "", string classificationSchemeId = "", string codeProject = ""
        public string StateOfDiagram { get; set; }
        public string IdProject { get; set; }
        public string ClassificationSchemeId { get; set; }
        public string CodeProject { get; set; }
    }
}