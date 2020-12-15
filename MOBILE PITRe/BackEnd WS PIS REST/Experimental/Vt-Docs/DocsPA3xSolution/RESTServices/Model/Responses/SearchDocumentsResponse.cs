using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class SearchDocumentsResponse
    {
        /// <summary>
        /// Documenti cercati
        /// </summary>
        public Domain.Document[] Documents
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale dei documenti trovati
        /// </summary>
        public int TotalDocumentsNumber
        {
            get;
            set;
        }

        public string ErrorMessage { get; set; }
        public SearchDocumentsResponseCode Code { get; set; }
    }

    public enum SearchDocumentsResponseCode { OK, SYSTEM_ERROR}
}