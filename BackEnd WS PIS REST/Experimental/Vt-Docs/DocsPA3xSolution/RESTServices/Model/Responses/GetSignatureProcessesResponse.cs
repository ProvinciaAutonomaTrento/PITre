using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetSignatureProcessesResponse
    {
        public Domain.SignBook.SignatureProcess[] Processes
        {
            get;
            set;
        }

        public int TotalProcessesNumber
        {
            get;
            set;
        }

        public string ErrorMessage { get; set; }
        public GetSignatureProcessesResponseCode Code { get; set; }
    }
    public enum GetSignatureProcessesResponseCode { OK, SYSTEM_ERROR }
}