using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetDocumentEventsResponse
    {
        public List<Domain.LogEvent> Events { get; set; }
        public GetDocumentEventsResponseCode Code { get; set; }
        public string ErrorMessage { get; set; }
    }
    public enum GetDocumentEventsResponseCode { OK,SYSTEM_ERROR}
}