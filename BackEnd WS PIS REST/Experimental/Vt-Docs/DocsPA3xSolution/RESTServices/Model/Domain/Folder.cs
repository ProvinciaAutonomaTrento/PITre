using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class Folder
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string IdParent { get; set; }
        public string IdProject { get; set; }
        public string CreationDate { get; set; }
    }
}