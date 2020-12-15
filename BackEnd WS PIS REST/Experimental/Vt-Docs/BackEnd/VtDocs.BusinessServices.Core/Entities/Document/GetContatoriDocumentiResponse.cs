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
    public class GetContatoriDocumentiResponse : Response
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ContatoreDocumento> Contatori
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable()]
        public class ContatoreDocumento
        {
            /// <summary>
            /// 
            /// </summary>
            public string Descrizione
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public int Totale
            {
                get;
                set;
            }
        }
    }
}
