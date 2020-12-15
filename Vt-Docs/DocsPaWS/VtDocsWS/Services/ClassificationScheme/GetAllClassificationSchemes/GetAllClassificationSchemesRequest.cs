using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.ClassificationScheme.GetAllClassificationSchemes
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetAllClassificationSchemesRequest"
    /// </summary>
   [DataContract]
    public class GetAllClassificationSchemesRequest: Request
    {
    }
}