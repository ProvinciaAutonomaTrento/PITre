using RESTServices.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetFiltersResponse
    {
        public GetFiltersResponseCode Code { get; set; }

        public List<Filter> Filters
        {
            get;
            set;
        }

        public string ErrorMessage { get; set; }
    }

    public enum GetFiltersResponseCode
    {
        OK, SYSTEM_ERROR
    }
}