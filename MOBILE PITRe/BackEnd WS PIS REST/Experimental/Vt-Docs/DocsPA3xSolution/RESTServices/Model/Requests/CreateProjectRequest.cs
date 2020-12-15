using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class CreateProjectRequest
    {
        public Domain.Project Project { get; set; }
    }
}