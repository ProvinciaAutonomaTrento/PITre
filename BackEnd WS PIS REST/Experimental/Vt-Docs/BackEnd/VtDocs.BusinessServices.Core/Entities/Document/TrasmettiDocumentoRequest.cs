using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class TrasmettiDocumentoRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        public TrasmettiDocumentoRequest()
        {
            this.TrasmissioniUtente = new List<TrasmissioneUtente>();
            this.TrasmissioniRuolo = new List<TrasmissioneRuolo>();
        }

        /// <summary>
        /// Identificativo univoco del documento da trasmettere
        /// </summary>
        public string IdDocumento
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se eseguire la trasmissione in modalità asincrona
        /// </summary>
        public bool Asincrono
        {
            get;
            set;
        }

        /// <summary>
        /// Lista delle trasmissioni ad utente da effettuare 
        /// </summary>
        public List<TrasmissioneUtente> TrasmissioniUtente
        {
            get;
            set;
        }

        /// <summary>
        /// Lista delle trasmissioni a ruolo da effettuare 
        /// </summary>
        public List<TrasmissioneRuolo> TrasmissioniRuolo
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public class TrasmissioneRuolo
        {
            /// <summary>
            /// Indica se salvare i dati della trasmissione senza effettuare l'invio
            /// </summary>
            public bool SalvaSenzaInviare
            {
                get;
                set;
            }

            /// <summary>
            /// Indica se il destinatario del documento vedrà o meno la trasmissione in todolist
            /// </summary>
            public bool NotificaInToDoList
            {
                get;
                set;
            }

            /// <summary>
            /// Indica la ragione trasmissione
            /// </summary>
            /// <remarks>
            /// Se non indicata, viene impostata automaticamente la ragione trasmissione predefinita in competenza definita in amministrazione
            /// </remarks>
            public string Ragione
            {
                get;
                set;
            }

            /// <summary>
            /// Note generali della trasmissione
            /// </summary>
            public string NoteGenerali
            {
                get;
                set;
            }

            /// <summary>
            /// Identificativi univoci dei ruoli cui trasmettere il documento
            /// </summary>
            public string[] IdRuoliDestinatari
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public class TrasmissioneUtente
        {
            /// <summary>
            /// Indica se salvare i dati della trasmissione senza effettuare l'invio
            /// </summary>
            public bool SalvaSenzaInviare
            {
                get;
                set;
            }

            /// <summary>
            /// Indica se il destinatario del documento vedrà o meno la trasmissione in todolist
            /// </summary>
            public bool NotificaInToDoList
            {
                get;
                set;
            }

            /// <summary>
            /// Indica la ragione trasmissione
            /// </summary>
            /// <remarks>
            /// Se non indicata, viene impostata automaticamente la ragione trasmissione predefinita in competenza definita in amministrazione
            /// </remarks>
            public string Ragione
            {
                get;
                set;
            }

            /// <summary>
            /// Note generali della trasmissione
            /// </summary>
            public string NoteGenerali
            {
                get;
                set;
            }

            /// <summary>
            /// Identificativi univoci degli utenti cui trasmettere il documento
            /// </summary>
            public string[] IdUtentiDestinatari
            {
                get;
                set;
            }
        }
    }
}