using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Publisher
{
    /// <summary>
    /// Mantiene i dati necessari per inviare i dati di un oggetto 
    /// da pubblicare ad un servizio listener
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publisher")]
    public class ChannelRefInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public ChannelRefInfo()
        {
            this.Events = new EventInfo[0];
            this.ExecutionConfiguration = new JobExecutionConfigurations();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Nome canale di pubblicazione referenziato
        /// </summary>
        public string ChannelName
        {
            get;
            set;
        }

        /// <summary>
        /// Metadati dell'amministrazione cui si riferisce l'istanza di pubblicazione
        /// </summary>
        public AdminInfo Admin
        {
            get;
            set;
        }

        /// <summary>
        /// Url del servizio web del subscriber
        /// </summary>
        public string SubscriberServiceUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Configurazioni per l'avvio delle pubblicazioni
        /// </summary>
        public JobExecutionConfigurations ExecutionConfiguration
        {
            get;
            set;
        }

        /// <summary>
        /// Data / ora del log a partire dal quale deve essere effettuata la pubblicazione
        /// </summary>
        public DateTime StartLogDate
        {
            get;
            set;
        }

        /// <summary>
        /// Id dell'ultimo log raccolto dalla pubblicazione
        /// </summary>
        public int LastLogId
        {
            get;
            set;
        }

        /// <summary>
        /// Data / ora ultima raccolta oggetti per la pubblicazione
        /// </summary>
        public DateTime LastExecutionDate
        {
            get;
            set;
        }

        /// <summary>
        /// Numero di pubblicazioni effettuate dal canale 
        /// </summary>
        public int ExecutionCount
        {
            get;
            set;
        }

        /// <summary>
        /// Numero di oggetti pubblicati dal canale
        /// </summary>
        public int PublishedObjects
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di pubblicazioni effettuate dal canale
        /// </summary>
        public int TotalExecutionCount
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale di oggetti pubblicati dal canale
        /// </summary>
        public int TotalPublishedObjects
        {
            get;
            set;
        }

        /// <summary>
        /// Indica, se true, di eseguire l'azione di modifica degli eventi
        /// </summary>
        public bool UpdateEventsAction
        {
            get;
            set;
        }

        /// <summary>
        /// Eventi da cui dovranno scaturire le pubblicazioni di oggetti
        /// </summary>
        public EventInfo[] Events
        {
            get;
            set;
        }

        /// <summary>
        /// Stato del servizio
        /// </summary>
        public ChannelStateEnum State
        {
            get;
            set;
        }

        /// <summary>
        /// Data in cui è stato avviato il servizio di pubblicazione
        /// </summary>
        public DateTime StartExecutionDate
        {
            get;
            set;
        }

        /// <summary>
        /// Data in cui è stato fermato il servizio di pubblicazione
        /// </summary>
        public DateTime EndExecutionDate
        {
            get;
            set;
        }

        /// <summary>
        /// Computer in cui risulta avviato il servizio
        /// </summary>
        public string MachineName
        {
            get;
            set;
        }

        /// <summary>
        /// Riferimento all'url del servizio publisher in cui risulta avviato il canale di pubblicazione
        /// </summary>
        /// <remarks>
        /// Valorizzato solamente se il canale di pubblicazione risulta avviato o meno
        /// </remarks>
        public string PublisherServiceUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetKey()
        {
            return string.Format("{0}_{1}", this.Id, this.ChannelName);
        }

    }
}