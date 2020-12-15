using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetTransmModelResponse
    {
        public Domain.TransmissionModel TransmissionModel { get; set; }
        public string ErrorMessage { get; set; }
        public GetTransmModelResponseCode Code { get; set; }
    }
    public enum GetTransmModelResponseCode { OK, SYSTEM_ERROR }
}