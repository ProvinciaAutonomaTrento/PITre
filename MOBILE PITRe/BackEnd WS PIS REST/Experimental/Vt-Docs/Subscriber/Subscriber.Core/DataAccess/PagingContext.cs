using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.DataAccess
{
    /// <summary>
    /// Criteri necessari per la paginazione dei dati 
    /// </summary>
    [Serializable()]
    public class PagingContext
    {
        /// <summary>
        /// Indica il numero di pagina richiesto 
        /// </summary>
        public int Page
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
        /// Indica il numero totale delle pagine restituite dalla ricerca 
        /// </summary>
        /// <remarks>
        /// Il dato è disponibile solamente come risultato della ricerca effettuata 
        /// </remarks>
        public int PagesCount
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
        public int ObjectsCount
        {
            get;
            set;
        }
    }
}
