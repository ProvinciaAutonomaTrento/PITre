using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class MessageResponse
    {
        public string ResultMessage { get; set; }
        public string ErrorMessage { get; set; }
        public MessageResponseCode Code { get; set; }
    }
    public enum MessageResponseCode { OK, SYSTEM_ERROR }
}