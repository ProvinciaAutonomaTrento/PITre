using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Publisher
{
    /// <summary>
    /// Parametri di configurazione per l'avvio del job di pubblicazione
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publisher")]
    public class JobExecutionConfigurations
    {
        /// <summary>
        /// 
        /// </summary>
        public JobExecutionConfigurations()
        {
            this.IntervalType = IntervalTypesEnum.ByMinute;
        }

        /// <summary>
        /// 
        /// </summary>
        public IntervalTypesEnum IntervalType
        {
            get;
            set;
        }

        /// <summary>
        /// Istante di esecuzione temporale
        /// </summary>
        public string ExecutionTicks
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum IntervalTypesEnum
        {
            BySecond = 1,
            ByMinute = 2,
            Hourly = 3,
            Daily = 4,
            Weekly = 5,
            Block = 6,
        }
    }
}
