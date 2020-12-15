using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetTransmModelsResponse
    {
        public Domain.TransmissionModel[] TransmissionModels { get; set; }
        public string ErrorMessage { get; set; }
        public GetTransmModelsResponseCode Code { get; set; }
    }
    public enum GetTransmModelsResponseCode { OK, SYSTEM_ERROR }
}