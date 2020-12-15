using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Dispatcher.CalendarMail.iCalendar
{
    /// <summary>
    /// Lista degli status di un appuntamento iCalendar
    /// </summary>
    public sealed class AppointmentStatusTypes
    {
        /// <summary>
        /// Appuntamento confermato
        /// </summary>
        public const string CONFIRMED = "CONFIRMED";

        /// <summary>
        /// Appuntamento revocato
        /// </summary>
        public const string CANCELLED = "CANCELLED";
    }
}
