using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Dispatcher.CalendarMail.iCalendar
{
    /// <summary>
    /// Lista dei methods consentiti per un appuntamento iCalendar
    /// </summary>
    public sealed class AppointmentMethodTypes
    {
        /// <summary>
        /// Appuntamento di richiesta
        /// </summary>
        public const string REQUEST = "REQUEST";

        /// <summary>
        /// Appuntamento di cancellazione
        /// </summary>
        public const string CANCEL = "CANCEL";
    }
}
