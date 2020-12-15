using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Subscriber
{
    /// <summary>
    /// Criteri necessari per la paginazione dei dati nelle ricerche degli oggetti
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public partial class PagingContextInfo
    {
        /// <summary>
        /// Indica il numero di pagina richiesto 
        /// </summary>
        public int PageNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Indica il numero di oggetti da includere nella ricerca per ciascuna pagina 
        /// </summary>
        public int ObjectsPerPage
        {
            get;
            set;
        }

        /// <summary>
        /// Indica il numero totale di oggetti (non paginati) restituiti dalla ricerca 
        /// </summary>
        /// <remarks>
        /// Il dato è disponibile solamente come risultato della ricerca effettuata 
        /// </remarks>
        public int TotalObjects
        {
            get;
            set;
        }
    }
}
