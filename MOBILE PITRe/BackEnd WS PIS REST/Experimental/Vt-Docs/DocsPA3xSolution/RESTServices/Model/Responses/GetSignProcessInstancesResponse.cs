using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetSignProcessInstancesResponse
    {
        public Domain.SignBook.SignatureProcessInstance[] SignatureProcessInstances
        {
            get;
            set;
        }
        public int TotalNumber
        {
            get;
            set;
        }
        public string ErrorMessage { get; set; }
        public GetSignProcessInstancesResponseCode Code { get; set; }
    }
    public enum GetSignProcessInstancesResponseCode { OK, SYSTEM_ERROR }
}