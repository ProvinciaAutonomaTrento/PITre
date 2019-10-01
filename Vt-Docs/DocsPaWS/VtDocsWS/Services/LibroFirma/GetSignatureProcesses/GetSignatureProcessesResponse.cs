using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.LibroFirma.GetSignatureProcesses
{
    [DataContract]
    public class GetSignatureProcessesResponse : Response
    {
        [DataMember]
        public Domain.SignBook.SignatureProcess[] Processes
        {
            get;
            set;
        }

        [DataMember(EmitDefaultValue = false)]
        public int? TotalProcessesNumber
        {
            get;
            set;
        }
    }
}