using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{

    [Serializable()]
    public class MessageEngineWrapper
    {
        public String MessageType
        {
            get;
            set;
        }

        public String Interface
        {
            get;
            set;
        }

        public String Action
        {
            get;
            set;
        }

        public String RecipientType
        {
            get;
            set;
        }

        public String From
        {
            get;
            set;
        }

        public String Subject
        {
            get;
            set;
        }

        public String Body
        {
            get;
            set;
        }

        public String ServicesUrl
        {
            get;
            set;
        }

        public String MessageEngine
        {
            get;
            set;
        }
    }
    
    
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class TrasmettiDocumentoOrganigrammaRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        public string IdDocumento
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> IdUtentiDestinatari
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> IdRuoliDestinatari
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> IdUODestinatarie
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string CodiceQualificaDiscriminanteUO
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool NotificaInToDoList
        {
            get;
            set;
        }

        /// <summary>
        /// Contiene le informazioni utili all'invio della mail
        /// </summary>
        public MessageEngineWrapper MessageEngineObj
        {
            get;
            set;
        }
    }
}
