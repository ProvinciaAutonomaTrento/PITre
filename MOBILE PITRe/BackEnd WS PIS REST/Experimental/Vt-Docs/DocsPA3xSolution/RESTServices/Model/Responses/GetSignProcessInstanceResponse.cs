using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetSignProcessInstanceResponse
    {
        public Domain.SignBook.SignatureProcessInstance ProcessInstance { get; set; }
        public string ErrorMessage { get; set; }
        public GetSignProcessInstanceResponseCode Code { get; set; }
    }
    public enum GetSignProcessInstanceResponseCode { OK, SYSTEM_ERROR }
}