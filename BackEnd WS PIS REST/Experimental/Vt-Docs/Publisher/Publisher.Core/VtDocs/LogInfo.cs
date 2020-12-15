using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Publisher.VtDocs
{
    /// <summary>
    /// Dati di un singolo log del sistema documentale
    /// </summary>
    [Serializable()]
    public class LogInfo
    {
        /// <summary>
        /// Id univoco del log nel sistema
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Id dell'amministrazione
        /// </summary>
        public int IdAdmin
        {
            get;
            set;
        }

        /// <summary>
        /// Id utente che ha generato l'evento nel sistema
        /// </summary>
        public int IdUser
        {
            get;
            set;
        }

        /// <summary>
        /// Utente che ha generato l'evento nel sistema
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Id ruolo cui l'utente appartiene
        /// </summary>
        public int IdRole
        {
            get;
            set;
        }

        /// <summary>
        /// Codice ruolo cui l'utente appartiene
        /// </summary>
        public string RoleCode
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione ruolo cui l'utente appartiene
        /// </summary>
        public string RoleDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Data in cui è stato generato l'evento nel sistema
        /// </summary>
        public DateTime Data
        {
            get;
            set;
        }

        /// <summary>
        /// Id dell'oggetto che ha generato l'evento
        /// </summary>
        public int ObjectId
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo oggetto che ha generato l'evento
        /// </summary>
        public string ObjectType
        {
            get;
            set;
        }

        /// <summary>
        /// Codice dell'evento
        /// </summary>
        public string EventCode
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione dell'evento
        /// </summary>
        public string EventDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione dell'azione
        /// </summary>
        public string ObjectDescription
        {
            get;
            set;
        }

    }

}
