using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ErrorCodes
    {
        /// <summary>
        /// 
        /// </summary>
        public const string SUBRULE_NAME_NOT_DECLARED = "SUBRULE_NAME_NOT_DECLARED";

        /// <summary>
        /// Mail del legale incaricato non presente nel profilo dell'oggetto
        /// </summary>
        public const string LEGALE_INCARICATO_MISSING_EMAIL = "LEGALE_INCARICATO_MISSING_EMAIL";

        /// <summary>
        /// Valore di un campo non presente nel profilo dell'oggetto
        /// </summary>
        public const string MISSING_FIELD_VALUE = "MISSING_FIELD_VALUE";

        /// <summary>
        /// Valore di un campo presente nel profilo non valido
        /// </summary>
        public const string INVALID_FIELD_VALUE = "INVALID_FIELD_VALUE";

        /// <summary>
        /// Valore del campo data del profilo riferito ad un evento passato
        /// </summary>
        public const string APPOINTMENT_DATE_VALUE_IS_PAST_DATE = "APPOINTMENT_DATE_VALUE_IS_PAST_DATE";

        /// <summary>
        /// Indica che un appuntamento non è stato modificato
        /// </summary>
        public const string APPOINTMENT_DATE_VALUE_IS_NOT_CHANGED = "APPOINTMENT_DATE_VALUE_IS_NOT_CHANGED";
    }
}
