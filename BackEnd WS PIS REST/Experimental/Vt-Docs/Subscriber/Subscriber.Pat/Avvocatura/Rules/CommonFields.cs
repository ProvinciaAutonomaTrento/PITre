using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat.Avvocatura.Rules
{
    /// <summary>
    /// Campi comuni ai tipi fascicolo
    /// </summary>
    public sealed class CommonFields
    {
        /// <summary>
        /// Nome del campo contenente il legale incaricato
        /// </summary>
        /// <remarks>
        /// E' un campo calcolato in base al dato inserito nel profilo
        /// </remarks>
        public const string MAIL_LEGALE_INCARICATO = "MAIL:Legale incaricato";

        /// <summary>
        /// 
        /// </summary>
        public const string DESCRIZIONE_FASCICOLO = "Descrizione del fascicolo";

        /// <summary>
        /// 
        /// </summary>
        public const string AUTORITA_GIUDIZIARIA = "Autorità giudiziaria";

        /// <summary>
        /// 
        /// </summary>
        public const string COMPETENZA_TERRITORIALE = "Competenza territoriale";
    }
}
