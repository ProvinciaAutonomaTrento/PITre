using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Subscriber
{
    /// <summary>
    /// Metadati di una regola di pubblicazione del Subscriber
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public abstract class BaseRuleInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public BaseRuleInfo()
        { }

        /// <summary>
        /// Identificativo univoco della regola
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco del canale di pubblicazione per cui è definita la regola
        /// </summary>
        public int IdInstance
        {
            get;
            set;
        }

        /// <summary>
        /// Nome della regola di pubblicazione
        /// </summary>
        public string RuleName
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione della regola di pubblicazione
        /// </summary>
        public string RuleDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se la regola risulta attivata o meno
        /// </summary>
        public bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se la regola è stata calcolata o meno
        /// </summary>
        public bool Computed
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la data in cui è stata calcolata la regola
        /// </summary>
        /// <remarks>
        /// Riporta DateTime.MinValue se la regola non è stata calcolata
        /// </remarks>
        public DateTime ComputeDate
        {
            get;
            set;
        }

        /// <summary>
        /// Ordinale della regola nell'ambito del canale di pubblicazione che stabilisce
        /// l'ordine di calcolo
        /// </summary>
        public int Ordinal
        {
            get;
            set;
        }

        /// <summary>
        /// Opzioni aggiuntive definite per la regola di pubblicazione
        /// </summary>
        public NameValuePair[] Options
        {
            get;
            set;
        }

        /// <summary>
        /// Dettagli di un eventuale errore nel calcolo della regola
        /// </summary>
        public ErrorInfo Error
        {
            get;
            set;
        }

        /// <summary>
        /// Opzioni della regola in formato testuale
        /// </summary>
        /// <returns></returns>
        public string GetOptions()
        {
            StringBuilder sb = new StringBuilder();

            if (this.Options != null)
            {
                foreach (NameValuePair pair in this.Options)
                    sb.Append(pair.ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Individua un'opzione della sottoregola a partire dal nome
        /// </summary>
        /// <param name="name"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        public string GetOptionByName(string name, bool required)
        {
            NameValuePair pair = this.Options.Where(e => e.Name.ToLowerInvariant() == name.ToLowerInvariant()).FirstOrDefault();

            if (pair == null)
            {
                if (required)
                {
                    string subRuleName = string.Empty;
                    if (this.GetType() == typeof(SubRuleInfo))
                        subRuleName = ((SubRuleInfo)this).SubRuleName;

                    throw new SubscriberException(ErrorCodes.RULE_OPTION_NOT_DEFINIED,
                            string.Format(ErrorDescriptions.RULE_OPTION_NOT_DEFINIED, name, subRuleName));
                }
                else
                    return string.Empty;
            }
            else
                return pair.Value.Trim();
        }
    }
}
