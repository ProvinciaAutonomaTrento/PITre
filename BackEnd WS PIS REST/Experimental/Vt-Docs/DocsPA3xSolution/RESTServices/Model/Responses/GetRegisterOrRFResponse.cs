using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetRegisterOrRFResponse
    {
        public Domain.Register Register { get; set; }
        public string ErrorMessage { get; set; }
        public GetRegisterOrRFResponseCode Code { get; set; }
    }

    public enum GetRegisterOrRFResponseCode { OK, SYSTEM_ERROR }
}