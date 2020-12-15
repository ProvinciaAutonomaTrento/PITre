using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.LibroFirma.SearchSignProcessInstances
{
    [DataContract]
    public class SearchSignProcessInstancesResponse :Response
    {
        /// <summary>
        /// Documenti cercati
        /// </summary>
        [DataMember]
        public Domain.SignBook.SignatureProcessInstance[] SignatureProcessInstances
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale dei documenti trovati
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public int? TotalNumber
        {
            get;
            set;
        }
    }
}