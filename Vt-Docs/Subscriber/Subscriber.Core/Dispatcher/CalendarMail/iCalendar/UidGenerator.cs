using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Dispatcher.CalendarMail.iCalendar
{
    /// <summary>
    /// Classe per la generazione degli identificativi UID per gli appuntamenti iCalendar
    /// </summary>
    public sealed class UidGenerator
    {
        /// <summary>
        /// Creazione nuovo identificativo univoco
        /// </summary>
        /// <returns></returns>
        public static string Create()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
    }
}
