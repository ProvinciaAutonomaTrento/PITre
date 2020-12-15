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
    public class AddAssociazioneUtenteRuoloRequest : Request
    {
        /// <summary>
        /// Se valorizzato, il consumer ha richiesto la contestuale associazione di una qualifica alla relazione utente / ruolo
        /// </summary>
        public AddAssociazioneQualificaRequest AssociazioneQualifica
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco dell'utente da inserire nel ruolo
        /// </summary>
        public string IdUtente
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco del ruolo in UO in cui inserire l'utente
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