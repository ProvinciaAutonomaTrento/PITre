using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class SearchCorrespondentsResponse
    {
        public Domain.Correspondent[] Correspondents
        {
            get;
            set;
        }
        public string ErrorMessage { get; set; }
        public SearchCorrespondentsResponseCode Code { get; set; }
    }

    public enum SearchCorrespondentsResponseCode { OK, SYSTEM_ERROR}
}