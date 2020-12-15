using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat.Geologico.Rules
{
    /// <summary>
    /// Regola per la pubblicazione dei fascicoli per il Servizio Geologico
    /// </summary>
    public class SondeGeotermiche2013Rule : ServizioGeologicoBaseRule
    {
        #region Public Members

        /// <summary>
        /// Reperimento del nome della regola
        /// </summary>
        public override string RuleName
        {
            get
            {
                return RULE_NAME;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string[] GetSubRules()
        {
            return new string[0];
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(SondeGeotermicheRule));

        /// <summary>
        /// Nome della regola
        /// </summary>
        private const string RULE_NAME = "SONDE_GEOTERMICHE_RULE";

        /// <summary>
        /// Nome del tipo fascicolo in PITRE
        /// </summary>
        private const string TEMPLATE_NAME = "Sonde geotermiche - dal 2013";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetTemplateName()
        {
            return TEMPLATE_NAME;
        }

        #endregion
    }
}