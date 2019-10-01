using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Dispatcher.CalendarMail.iCalendar
{

//4.3.6   Duration

//   Value Name: DURATION

//   Purpose: This value type is used to identify properties that contain
//   a duration of time.

//   Formal Definition: The value type is defined by the following
//   notation:

//     dur-value  = (["+"] / "-") "P" (dur-date / dur-time / dur-week)

//     dur-date   = dur-day [dur-time]
//     dur-time   = "T" (dur-hour / dur-minute / dur-second)
//     dur-week   = 1*DIGIT "W"
//     dur-hour   = 1*DIGIT "H" [dur-minute]
//     dur-minute = 1*DIGIT "M" [dur-second]
//     dur-second = 1*DIGIT "S"
//     dur-day    = 1*DIGIT "D"

//   Description: If the property permits, multiple "duration" values are
//   specified by a COMMA character (US-ASCII decimal 44) separated list
//   of values. The format is expressed as the [ISO 8601] basic format for
//   the duration of time. The format can represent durations in terms of
//   weeks, days, hours, minutes, and seconds.



    /// <summary>
    /// Classe per la gestione delle informazioni relative all'eventuale alert associato ad un appuntamento
    /// </summary>
    [Serializable()]
    public class AlertInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int TriggerDays
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [AppointmentParamAttribute(Name = "{trigger}", CalName = "TRIGGER")]
        public string Trigger
        {
            get
            {
                // Calcolo dei minuti
                return string.Format("-PT{0}M", (this.TriggerDays * (60 * 24)) );
            }
            set
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [AppointmentParamAttribute(Name = "{alertDescription}", CalName = "DESCRIPTION")]
        public string Description
        {
            get;
            set;
        }
    }
}
