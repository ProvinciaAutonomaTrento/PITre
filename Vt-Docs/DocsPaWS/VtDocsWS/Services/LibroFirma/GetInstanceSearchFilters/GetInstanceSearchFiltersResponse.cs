using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.LibroFirma.GetInstanceSearchFilters
{
    [DataContract]
    public class GetInstanceSearchFiltersResponse : Response
    {
        [DataMember]
        public Domain.Filter[] Filters { get; set; }
    }
}