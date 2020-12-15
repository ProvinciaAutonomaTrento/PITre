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
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/VtDocs/Business/ObjectProperty")]
    public class ObjectProperty 
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Value
        {
            get;
            set;
        }
    }
}
