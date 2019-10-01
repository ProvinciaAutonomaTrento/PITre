using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.SearchDocuments
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "SearchDocumentsRequest"
    /// </summary>
    [DataContract]
    public class SearchDocumentsRequest : Request
    {
        /// <summary>
        /// Filtri di ricerca
        /// </summary>
        [DataMember]
        public Domain.Filter[] Filters
        {
            get;
            set;
        }

        /// <summary>
        /// Pagina che si desidera visualizzare
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public int? PageNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Quanti elementi sono presenti nella pagina se la ricerca è paginata
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public int? ElementsInPage
        {
            get;
            set;
        }

        /// <summary>
        /// Se true la relativa response conterrà il numero totale di documenti ottenuti nella ricerca
        /// </summary>
        [DataMember]
        public bool GetTotalDocumentsNumber
        {
            get;
            set;
        }
    }
}