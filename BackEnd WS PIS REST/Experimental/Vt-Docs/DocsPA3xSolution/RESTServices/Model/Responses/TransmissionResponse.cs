using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class TransmissionResponse
    {
        public string TransmMessage { get; set; }
        public string ErrorMessage { get; set; }
        public TransmissionResponseCode Code { get; set; }
    }

    public enum TransmissionResponseCode { OK, SYSTEM_ERROR}
}