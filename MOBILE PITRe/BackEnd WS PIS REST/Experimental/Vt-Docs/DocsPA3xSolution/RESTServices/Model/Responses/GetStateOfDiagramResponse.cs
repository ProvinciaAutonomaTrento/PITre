using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetStateOfDiagramResponse
    {
        public Domain.StateOfDiagram StateOfDiagram { get; set; }
        public string ErrorMessage { get; set; }
        public GetStateOfDiagramResponseCode Code { get; set; }
    }
    public enum GetStateOfDiagramResponseCode { OK, SYSTEM_ERROR}
}