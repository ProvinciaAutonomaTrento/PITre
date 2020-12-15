using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Requests
{
    public class AddCorrespondentRequest
    {
        public Domain.Correspondent Correspondent { get; set; }
    }

}