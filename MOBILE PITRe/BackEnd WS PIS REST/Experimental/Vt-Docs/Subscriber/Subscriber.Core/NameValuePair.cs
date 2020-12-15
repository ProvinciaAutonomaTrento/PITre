using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Subscriber
{
    /// <summary>
    /// Coppia chiave - valore 
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public class NameValuePair
    {
        /// <summary>
        /// Nome della chiave
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Valore della chiave
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Rappresentazione stringa della coppia chiave / valore
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}|", this.NormalizeString(this.Name), this.NormalizeString(this.Value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string NormalizeString(string value)
        {
            if (value == null)
                return string.Empty;

            return value.Replace("|", string.Empty);
        }
    }
}
