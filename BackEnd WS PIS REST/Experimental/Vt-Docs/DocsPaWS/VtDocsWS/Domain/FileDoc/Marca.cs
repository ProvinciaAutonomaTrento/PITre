using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain.FileDoc
{
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Marca
    {
        /// <summary>
        /// contenuto binario della marca, tipicamente sarà rappresentato come un HTML
        /// e conterrà come informazioni il timestamp e l'hash del file al quale è associata
        /// la marca
        /// </summary>
        [DataMember]
        public byte[] content
        {
            get;
            set;
        }
        
        /// <summary>
        /// dimensione dell'array di byte contenente la marca - int
        /// </summary>
        [DataMember]
        public string length
        {
            get;
            set;
        }
        
        /// <summary>
        /// MIME type del contenuto della marca che sarà tipicamente un HTML
        /// </summary>
        [DataMember]
        public string contentType
        {
            get;
            set;
        }
        
    }
}