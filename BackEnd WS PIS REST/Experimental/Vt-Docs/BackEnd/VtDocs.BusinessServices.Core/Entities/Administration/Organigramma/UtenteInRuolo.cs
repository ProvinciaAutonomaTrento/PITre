using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    /// <summary>
    /// Dati dell'utente posizionato nell'organigramma
    /// </summary>
    [Serializable()]
    public class UtenteInRuolo
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Matricola
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Cognome
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Nome
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string NomeCompleto
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string EMail
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int IdAmministrazione
        {
            get;
            set;
        }

        /// <summary>
        /// Ruolo di appartenenza dell'utente
        /// </summary>
        public RuoloInUO Ruolo
        {
            get;
            set;
        }
    }
}
