using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Subscriber
{
    /// <summary>
    /// Dati di una proprietà di un oggetto
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public class Property
    {
        /// <summary>
        /// 
        /// </summary>
        [NonSerialized()]
        private byte[] _binaryValue = null;

        /// <summary>
        /// 
        /// </summary>
        public Property()
        { }

        /// <summary>
        /// Nome della proprietà
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo di dato della proprietà
        /// </summary>
        public PropertyTypesEnum Type
        {
            get;
            set;
        }

        /// <summary>
        /// Valore della proprietà
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// Valore binario della proprietà
        /// </summary>
        /// <remarks>
        /// Utilizzata solo se il tipo dato è Binary
        /// </remarks>
        public byte[] BinaryValue
        {
            get
            {
                return this._binaryValue;
            }
            set
            {
                this._binaryValue = value;
            }
        }

        /// <summary>
        /// Determina se la proprietà è visibile o meno
        /// </summary>
        /// <remarks>
        /// Utile quando il Publisher deve fornire al Subscriber proprietà o attributi nascosti
        /// (es. Id dell'oggetto)
        /// </remarks>
        public bool Hidden
        {
            get;
            set;
        }

        /// <summary>
        /// Verifica se è stato impostato il valore per la proprietà
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                if (this.Value == null)
                    return true;

                return string.IsNullOrEmpty(this.Value.ToString());
            }
        }

        /// <summary>
        /// Indica se la proprietà si riferisce ad un template oggetto 
        /// </summary>
        public bool IsTemplateProperty
        {
            get;
            set;
        }
    }
}
