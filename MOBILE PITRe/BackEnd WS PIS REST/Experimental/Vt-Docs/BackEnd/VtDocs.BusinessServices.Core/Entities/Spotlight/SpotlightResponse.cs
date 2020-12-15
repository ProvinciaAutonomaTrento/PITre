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
    //[System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/VtDocs/Business/Entities/Spotlight/SpotlightResponse")]
    public class SpotlightResponse : Response
    {
        /// <summary>
        /// 
        /// </summary>
        public SpotlightResponse()
        {
            this.Records = new SpotlightResultRecord[0];
        }

        /// <summary>
        /// 
        /// </summary>
        public int TotalRecordCount
        {
            get;
            set;
        }

        /// <summary>
        /// Elementi trovati
        /// </summary>
        public SpotlightResultRecord[] Records
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    //[System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/VtDocs/Business/Entities/Spotlight/SpotlightResultRecord")]
    public class SpotlightResultRecord
    {
        /// <summary>
        /// 
        /// </summary>
        public SpotlightResultRecord()
        {
            this.Cells = new SpotlightResultCell[0];
        }

        /// <summary>
        /// 
        /// </summary>
        public SpotlightResultCell[] Cells
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    //[System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/VtDocs/Business/Entities/Spotlight/SpotlightResultCell")]
    public class SpotlightResultCell
    {
        /// <summary>
        /// 
        /// </summary>
        public string FieldName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string FieldValue
        {
            get;
            set;
        }
    }
}
