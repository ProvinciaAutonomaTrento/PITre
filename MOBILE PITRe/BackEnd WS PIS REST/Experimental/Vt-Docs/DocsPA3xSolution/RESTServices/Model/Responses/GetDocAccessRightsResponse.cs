using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetDocAccessRightsResponse
    {
        public List<Domain.ObjectAccessRight> AccessRights
        {
            get;
            set;
        }
        public string ErrorMessage { get; set; }
        public GetDocAccessRightsResponseCode Code { get; set; }
    }

    public enum GetDocAccessRightsResponseCode{OK,SYSTEM_ERROR}
}