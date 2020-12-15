using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace VtDocsWS.Services.CAPServices.GetDocsInOpportunity
{
    public class GetDocsInOpportunityResponse : Response
    {
        /// <summary>
        /// Documents in the Opportunity
        /// </summary>
        [DataMember]
        public Domain.Document[] Documents
        {
            get;
            set;
        }

        /// <summary>
        /// Number of documents
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public int? TotalDocumentsNumber
        {
            get;
            set;
        }
    }
}