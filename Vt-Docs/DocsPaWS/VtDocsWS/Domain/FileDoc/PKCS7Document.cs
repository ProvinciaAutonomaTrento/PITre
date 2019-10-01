using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain.FileDoc
{
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class PKCS7Document
    {
        /// <summary>
        /// Level - int
        /// </summary>
        [DataMember]
        public string Level
        {
            get;
            set;
        }

        /// <summary>
        /// DocumentFileName
        /// </summary>
        [DataMember]
        public string DocumentFileName
        {
            get;
            set;
        }

        /// <summary>
        /// SignAlgorithm, Algoritmo usato per firmare il documento
        /// </summary>
        [DataMember]
        public string SignAlgorithm
        {
            get;
            set;
        }

        /// <summary>
        /// SignersInfo
        /// </summary>
        [DataMember]
        public SignerInfo[] SignersInfo
        {
            get;
            set;
        }
        
    }
}