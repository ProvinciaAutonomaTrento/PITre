using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class SearchProjectsResponse
    {
        public Domain.Project[] Projects
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale dei fascicoli trovati
        /// </summary>
        public int TotalProjectsNumber
        {
            get;
            set;
        }

        public string ErrorMessage { get; set; }
        public SearchProjectsResponseCode Code { get; set; }
    }

    public enum SearchProjectsResponseCode { OK, SYSTEM_ERROR }
}