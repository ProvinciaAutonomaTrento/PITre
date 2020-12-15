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
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/VtDocs/Business/DataObject")]
    public class DataObject 
    {
        /// <summary>
        /// 
        /// </summary>
        public DataObject()
        {
            this.Identity = new ObjectIdentity();
            this.Properties = new ObjectProperty[0];
            this.Childs = new DataObject[0];
        }

        /// <summary>
        /// 
        /// </summary>
        public ObjectIdentity Identity
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ObjectName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string TypeName
        {
            get;
            set;
        }

        /// <summary>
        /// Tipologia oggetto
        /// </summary>
        public string TemplateName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ObjectProperty[] Properties
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DataObject[] Childs
        {
            get;
            set;
        }
    }
}
