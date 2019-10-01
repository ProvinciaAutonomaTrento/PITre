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
    public class GetUtentiDestinatariTrasmissioniResponse : Response
    {
        /// <summary>
        /// 
        /// </summary>
        public GetUtentiDestinatariTrasmissioniResponse()
        {
            this.Utenti = new List<DestinatarioTrasmissione>();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<DestinatarioTrasmissione> Utenti
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public class DestinatarioTrasmissione
        {
            /// <summary>
            /// 
            /// </summary>
            public string Id
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string IdUtente
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string Utente
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string DataInvio
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string DataVista
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string DataAccettazione
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string DataRifiuto
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string Ragione
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string IdTrasmissione
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string Email
            {
                get;
                set;
            }
        }
    }
}
