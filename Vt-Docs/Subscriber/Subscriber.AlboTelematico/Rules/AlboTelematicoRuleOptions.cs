using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.AlboTelematico.Rules
{
    /// <summary>
    /// Opzioni configurabili per tutte le regole dell'albo telematico
    /// </summary>
    [Serializable()]
    public class AlboTelematicoRuleOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public AlboTelematicoRuleOptions()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule"></param>
        public AlboTelematicoRuleOptions(BaseRuleInfo rule)
        {
            this.AlboTelematicoServicesUrl = rule.GetOptionByName("WS_ALBO", true).Replace('$', ':');
            this.AlboTelematicoServicesUid = rule.GetOptionByName("UID_ALBO", true);
            this.AlboTelematicoServicesPwd = rule.GetOptionByName("PWD_ALBO", true);
            this.PitreServicesUrl = rule.GetOptionByName("WS_PITRE", true).Replace('$', ':');
            this.PitreServicesUid = rule.GetOptionByName("UID_PITRE", true);
            this.PitreServicesPwd = rule.GetOptionByName("PWD_PITRE", true);
        }

        /// <summary>
        /// 
        /// </summary>
        public string AlboTelematicoServicesUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AlboTelematicoServicesUid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AlboTelematicoServicesPwd { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string PitreServicesUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PitreServicesUid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PitreServicesPwd { get; set; }
    }
}
