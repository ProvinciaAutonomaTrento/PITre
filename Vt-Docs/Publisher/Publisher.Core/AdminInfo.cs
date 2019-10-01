using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Publisher
{
    /// <summary>
    /// Dati dell'amministrazione
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publisher")]
    public class AdminInfo
    {
        /// <summary>
        /// Identificativo dell'amministrazione cui si riferisce l'istanza di pubblicazione
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get;
            set;
        }
    }
}
