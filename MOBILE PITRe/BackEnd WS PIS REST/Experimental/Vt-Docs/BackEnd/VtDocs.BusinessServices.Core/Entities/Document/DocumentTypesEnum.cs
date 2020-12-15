using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Enumeration che indica le tipologie documento
    /// </summary>
    //[System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/VtDocs/Business/Entities/Document/DocumentTypesEnum")]
    public enum DocumentTypesEnum
    {
        NP,
        Arrivo,
        Partenza,
        Interno,
    }

    /// <summary>
    /// Identifica i tipi documento come costanti
    /// </summary>
    //[System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/VtDocs/Business/Entities/Document/DocumentTypesConstants")]
    public class DocumentTypesConstants
    {
        public const string NP = "G";
        public const string Arrivo = "A";
        public const string Partenza = "P";
        public const string Interno = "I";
    }
}
