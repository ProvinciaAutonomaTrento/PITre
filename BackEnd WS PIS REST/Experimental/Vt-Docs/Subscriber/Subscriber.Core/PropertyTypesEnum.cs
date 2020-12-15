using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Subscriber
{
    /// <summary>
    /// Tipologie di dato della proprietà
    /// </summary>
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public enum PropertyTypesEnum
    {
        /// <summary>
        /// Valore stringa
        /// </summary>
        String,

        /// <summary>
        /// Valore numerico
        /// </summary>
        Numeric,

        /// <summary>
        /// Valore data
        /// </summary>
        Date,

        /// <summary>
        /// Valore boolean
        /// </summary>
        Boolean,

        /// <summary>
        /// Contenuto binario (es. file)
        /// </summary>
        BinaryContent,
    }
}
