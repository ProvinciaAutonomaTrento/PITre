using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetRegistersOrRFResponse
    {
        public Domain.Register[] Registers { get; set; }
        public string ErrorMessage { get; set; }
        public GetRegistersOrRFResponseCode Code { get; set; }
    }

    public enum GetRegistersOrRFResponseCode { OK, SYSTEM_ERROR }
}