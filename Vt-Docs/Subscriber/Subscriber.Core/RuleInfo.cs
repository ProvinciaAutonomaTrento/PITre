using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Serialization;

namespace Subscriber
{
    /// <summary>
    /// Medatadi di una regola di pubblicazione
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public class RuleInfo : BaseRuleInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public RuleInfo()
        {
            this.SubRulesList = new List<SubRuleInfo>();
        }

        /// <summary>
        /// FullName della classe Rule che implementa l'interfaccia IRule
        /// </summary>
        public string RuleClassFullName
        {
            get;
            set;
        }

        /// <summary>
        /// Lista delle eventuali sottoregole di pubblicazione
        /// </summary>
        [XmlIgnore()]
        public List<SubRuleInfo> SubRulesList
        {
            get;
            set;
        }

        /// <summary>
        /// Lista delle eventuali sottoregole di pubblicazione
        /// </summary>
        public SubRuleInfo[] SubRules
        {
            get
            {
                return this.SubRulesList.ToArray();
            }
            set
            {
            }
        }
    }
}
