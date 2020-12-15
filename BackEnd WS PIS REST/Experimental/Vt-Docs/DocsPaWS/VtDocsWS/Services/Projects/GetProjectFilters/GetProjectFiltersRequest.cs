using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.GetProjectFilters
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetProjectFiltersRequest"
    /// </summary>
    [DataContract]
    public class GetProjectFiltersRequest : Request
    {
    }
}