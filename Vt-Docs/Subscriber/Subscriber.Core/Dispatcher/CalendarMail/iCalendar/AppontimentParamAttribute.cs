using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Dispatcher.CalendarMail.iCalendar
{
    /// <summary>
    /// Identifica il nome dell'attributo iCalendar
    /// </summary>
    [Serializable()]
    [AttributeUsage(AttributeTargets.Property)]
    public class AppointmentParamAttribute : Attribute
    {
        /// <summary>
        /// Nome dell'attributo iCalendar
        /// </summary>
        public string CalName
        {
            get;
            set;
        }

        /// <summary>
        /// Nome dell'attributo dell'oggetto AppointmentInfo
        /// </summary>
        public string Name
        {
            get;
            set;
        }
    }
}
