using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Spotlight
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    //[System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/VtDocs/Business/Entities/Spotlight/SpotlightRequest")]
    public class SpotlightRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        public string Filter
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Start
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Limit
        {
            get;
            set;
        }
    }
}
