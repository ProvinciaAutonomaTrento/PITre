using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.trasmissione
{
    /// <summary>
    /// Classe per il mantenimento delle informazioni di stato di una trasmissionte utente
    /// </summary>
    [Serializable()]
    public class StatoTrasmissioneUtente
    {
        /// <summary>
        /// Se true, il documento / fascicolo trasmesso è stato visto dall'utente
        /// </summary>
        public bool Vista { get; set; }

        /// <summary>
        /// Se true, la trasmissione è stata accettata dall'utente        
        /// </summary>
        public bool Accettata { get; set; }

        /// <summary>
        /// Se true, la trasmissione è stata rifiutata dall'utente
        /// </summary>
        public bool Rifiutata { get; set; }

        /// <summary>
        /// Se true, la trasmissione è visibile in todolist per l'utente
        /// </summary>
        public bool InTodoList { get; set; }
    }
}
