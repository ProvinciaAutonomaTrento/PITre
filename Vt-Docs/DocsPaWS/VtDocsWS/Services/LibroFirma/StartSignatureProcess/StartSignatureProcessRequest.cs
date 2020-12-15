using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace VtDocsWS.Services.LibroFirma.StartSignatureProcess
{
    [DataContract]
    public class StartSignatureProcessRequest : Request
    {
        [DataMember]
        public Domain.SignBook.SignatureProcess SignatureProcess { get; set; }

        [DataMember]
        public string IdDocument { get; set; }

        //[DataMember]
        //public string IdMainDocument { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public bool InterruptionGeneratesNote { get; set; }

        [DataMember]
        public bool EndGeneratesNote { get; set; }
    }
}