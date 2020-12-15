using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class GetDocInfoRequest
    {
        public string IdTrasm { get; set; }
        public string IdEvento { get; set; }
        public string idDoc { get; set; }
    }
}