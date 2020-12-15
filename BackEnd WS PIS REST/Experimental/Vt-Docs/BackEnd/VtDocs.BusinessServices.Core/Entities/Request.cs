using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities
{
    /// <summary>
    /// Classe per l'incapsulamento dei pacchetti di richiesta di tutti i BusinessServices
    /// </summary>
    [Serializable()]
    public class Request
    {
        /// <summary>
        /// Indica il comportamento dei servizi BusinessServices in caso di errori.
        /// Se true, il servizio solleva un'eccezione di tipo "SoapException" riportando l'eccezione originale 
        /// 
        /// </summary>
        public bool TrowOnError
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DocsPaVO.utente.InfoUtente InfoUtente
        {
            get;
            set;
        }
    }
}
