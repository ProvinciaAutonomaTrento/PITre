using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/VtDocs/Business/ObjectIdentity")]
    public class ObjectIdentity 
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get;
            set;
        }
    }
}
