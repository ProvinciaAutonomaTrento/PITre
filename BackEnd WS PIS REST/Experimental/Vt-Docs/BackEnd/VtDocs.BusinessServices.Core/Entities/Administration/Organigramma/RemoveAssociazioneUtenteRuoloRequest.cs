using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class RemoveAssociazioneUtenteRuoloRequest : Request
    {
        /// <summary>
        /// Identificativo univoco dell'utente da rimouovere dal ruolo
        /// </summary>
        public string IdUtente
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco del ruolo in UO in cui rimuovere l'utente
        /// </summary>
        /// <remarks>
        /// L'attributo può gestire sia l'ID che il Codice
        /// </remarks>
        public string RuoloInUO
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco del tipo ruolo
        /// </summary>
        /// <remarks>
        /// Valido soltanto se non è stato specificato il RuoloInUO
        /// </remarks>
        public string TipoRuoloInUO
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco dell'UO
        /// </summary>
        public string IdUO
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco dell'amministrazione
        /// </summary>
        public string IdAmministrazione
        {
            get;
            set;
        }

    }
}
