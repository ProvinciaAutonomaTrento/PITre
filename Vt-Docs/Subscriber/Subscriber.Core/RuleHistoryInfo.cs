using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Subscriber
{
    /// <summary>
    /// Oggetto di input per il servizio di ricerca delle pubblicazioni storicizzate effettuate da una regola
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public class GetRuleHistoryListRequest
    {
        /// <summary>
        /// Id della regola di pubblicazione
        /// <remarks>
        /// Dato obbligatorio
        /// </remarks>
        /// </summary>
        public int IdRule
        {
            get;
            set;
        }
        
        /// <summary>
        /// Filtri personalizzati ammessi dal servizio di ricerca delle storicizzazioni delle pubblicazioni
        /// </summary>
        public RuleHistoryCustomFiltersInfo CustomFilters
        {
            get;
            set;
        }

        /// <summary>
        /// Criteri di paginazione
        /// <remarks>
        /// Se non definiti, saranno riportate tutte le storicizzazioni senza applicare paginazioni server-side
        /// </remarks>
        /// </summary>
        public PagingContextInfo PagingContext
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Oggetto di output al servizio di reperimento dello storico delle pubblicazioni
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public class GetRuleHistoryListResponse
    {
        /// <summary>
        /// Dati storicizzati delle pubblicazioni effettuate da una regola
        /// </summary>
        public RuleHistoryInfo[] Rules
        {
            get;
            set;
        }
        
        /// <summary>
        /// Contesto di paginazione
        /// </summary>
        public PagingContextInfo PagingContext
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Rappresenta i criteri di filtro delle pubblicazioni storicizzate effettuate da una regola
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public class RuleHistoryCustomFiltersInfo
    {
        /// <summary>
        /// Descrizione dell'oggetto
        /// </summary>
        public string ObjectDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Autore della pubblicazione
        /// </summary>
        public string AuthorName
        {
            get;
            set;
        }

        /// <summary>
        /// Ruolo autore della pubblicazione
        /// </summary>
        public string RoleName
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Rappresenta la storicizzazione dei dati di una pubblicazione di un oggetto effettuate da una regola nel corso del tempo
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public class RuleHistoryInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static RuleHistoryInfo CreateInstance(BaseRuleInfo rule)
        {
            RuleHistoryInfo instance = new RuleHistoryInfo();
            
            instance.IdRule = rule.Id;
            instance.ErrorInfo = rule.Error;
            instance.Published = rule.Computed;
            instance.PublishDate = rule.ComputeDate;

            return instance;
        }

        /// <summary>
        /// Identificativo univoco della pubblicazione storicizzata
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco della regola di pubblicazione
        /// </summary>
        public int IdRule
        {
            get;
            set;
        }

        /// <summary>
        /// Metadati dell'autore della pubblicazione
        /// </summary>
        public EventAuthorInfo Author
        {
            get;
            set;
        }

        /// <summary>
        /// Immagine storicizzata dell'oggetto pubblicato
        /// </summary>
        public PublishedObject ObjectSnapshot
        {
            get;
            set;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public string AppointmentAsText
        //{
        //    get
        //    {
        //        if (this.MailMessageSnapshot != null)
        //            return this.MailMessageSnapshot.AppointmentAsText;
        //        else
        //            return string.Empty;
        //    }
        //    set
        //    {
        //    }
        //}

        /// <summary>
        /// Immagine storicizzata dell'eventuale mail di notifica alla pubblicazione dell'oggetto
        /// </summary>
        //[XmlIgnore()]
        public Dispatcher.CalendarMail.MailRequest MailMessageSnapshot
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se l'oggetto è stato pubblicato o meno
        /// </summary>
        public bool Published
        {
            get;
            set;
        }

        /// <summary>
        /// Data in cui l'oggetto è stato messo in pubblicazione
        /// <remarks>
        /// La data è valorizzata indipendentemente dall'esito della pubblicazione
        /// </remarks>
        /// </summary>
        public DateTime PublishDate
        {
            get;
            set;
        }

        /// <summary>
        /// Eventuale errore che si è verificato nel processo di pubblicazione
        /// </summary>
        /// <remarks>
        /// Risulterà valorizzato con i dati dell'errore solo se la pubblicazione non è andata a buon fine,
        /// ovvero se l'attributo Published è impostato a false
        /// </remarks>
        public ErrorInfo ErrorInfo
        {
            get;
            set;
        }
    }
}
