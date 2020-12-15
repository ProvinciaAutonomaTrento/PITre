using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class LinkedDocument
    {
        public string Id { get; set; }
        public string Signature { get; set; }
        public string Object { get; set; }
        public string DocumentType { get; set; }
        public string LinkType { get; set; }
    }
}