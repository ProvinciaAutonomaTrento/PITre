using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class SendDocAdvResponse
    {
        public string ResultMessage { get; set; }

        public Domain.SendingResult[] SendingResults { get; set; }

        public string ErrorMessage { get; set; }
        public SendDocAdvResponseCode Code { get; set; }
    }
    public enum SendDocAdvResponseCode { OK, SYSTEM_ERROR}
}