using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VtDocsWS.Services;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetDocumentFilters
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetDocumentFiltersRequest"
    /// </summary>
    [DataContract]
    public class GetDocumentFiltersRequest : Request
    {
    }
}