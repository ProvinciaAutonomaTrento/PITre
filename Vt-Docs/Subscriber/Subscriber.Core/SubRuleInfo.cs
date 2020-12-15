using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Subscriber
{
    /// <summary>
    /// Sottoregola di pubblicazione applicata da una regola.
    /// <remarks>
    /// Una sottoregola rappresenta la scomposizione di più calcoli complessi in carico ad una 
    /// regola in una parte più piccola e facilmente manutenibile.
    /// </remarks>
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public class SubRuleInfo : BaseRuleInfo
    {
        /// <summary>
        /// Nome della sottoregola
        /// </summary>
        public string SubRuleName
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo della regola padre
        /// </summary>
        public int IdParentRule
        {
            get;
            set;
        }
    }
}
