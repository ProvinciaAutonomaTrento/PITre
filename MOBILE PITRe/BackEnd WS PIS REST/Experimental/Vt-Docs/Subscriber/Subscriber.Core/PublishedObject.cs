using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Subscriber
{
    /// <summary>
    /// Dati dell'oggetto messo in pubblicazione dal sistema documentale
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public class PublishedObject
    {
        /// <summary>
        /// 
        /// </summary>
        public PublishedObject()
        {
            this.Properties = new Property[0];
        }

        /// <summary>
        /// Rappresenta l'id univoco dell'oggetto
        /// </summary>
        public string IdObject
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione testuale dell'oggetto
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo oggetto
        /// </summary>
        public string ObjectType
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del template dell'oggetto
        /// </summary>
        public string TemplateName
        {
            get;
            set;
        }

        /// <summary>
        /// Proprietà estese di un oggetto
        /// </summary>
        public Property[] Properties
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual Property FindProperty(string propertyName)
        {
            return this.Properties.Where(e => e.Name.ToLowerInvariant() == propertyName.ToLowerInvariant()).FirstOrDefault();
        }
    }
}
