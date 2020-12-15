using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class ObjectAccessRight
    {
        public string IdObject { get; set; }
        public string AccessRights { get; set; }
        public string AccessRightsType { get; set; }
        public string SubjectDescription { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectType { get; set; }
        public string SubjectId { get; set; }
        public string AccessDate { get; set; }
        public string Note { get; set; }
    }
}