using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rubrica.Library.Data
{
 
    /// <summary>
    /// Rappresenta un utente cui è consentito l'accesso alla rubrica
    /// </summary>
    [Serializable()]
    public class Utente
    {
        /// <summary>
        /// 
        /// </summary>
        public Utente()
        { }

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Amministratore { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DataCreazione { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DataUltimaModifica { get; set; }
    }
}
