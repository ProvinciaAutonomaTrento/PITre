using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.Integrita
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/CreateFascicoloRequest")]
    public class GetHashDocumentoResponse
    {
        public enum AlgoritomoHash
        {
            none=0,
            SHA1,
            SHA256,
        }
       
        /// <summary>
        /// 
        /// </summary>
        public bool Success
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string HashRepository
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string HashDatabase
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public AlgoritomoHash HashAlgo
        {
            get;
            set;
        }
    }
}