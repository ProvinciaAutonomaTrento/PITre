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
    public class GetResponsabiliRequest : Request
    {
        /// <summary>
        /// Identificativo univoco dell'UO per cui reperire i responsabili
        /// </summary>
        public string IdUO
        {
            get;
            set;
        }

        /// <summary>
        /// Indica l'eventuale qualifica richiesta per il responsabile
        /// </summary>
        public string Qualifica
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se ricercare nelle uo figlie
        /// </summary>
        /// <remarks>
        /// La condizione discriminante è la qualifica discriminante UO
        /// </remarks>
        public bool RecursiveUO
        {
            get;
            set;
        }

        /// <summary>
        /// Indica l'eventuale qualifica utente da utilizzare come discriminante per l'estrazione delle UO
        /// </summary>
        public string QualificaDiscriminanteUO
        {
            get;
            set;
        }
    }
}
