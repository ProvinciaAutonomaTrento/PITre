using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocsAdapter
{
    public class Tracer : IDisposable
    {
        // Evento da scatenare al completamento del task
        private EventHandler<TimeInfo> summaryEvent;

        // Argomenti da passare all'evento scatenato al termine del task
        private TimeInfo arguments = new TimeInfo();

        public Tracer(EventHandler<TimeInfo> summaryEvent)
        {
            // Registrazione dell'evento da richiamare al termine del task corrente
            if (summaryEvent != null)
                this.summaryEvent = summaryEvent;
            
        }


        public void Dispose()
        {
            // Registrazione dell'istante in cui è terminato il calcolo e lancio dell'evento
            this.arguments.EndTime = DateTime.Now;

            if (this.summaryEvent != null)
                this.summaryEvent(this, this.arguments);
        }
    }

    /// <summary>
    /// Informazioni sul tempo di esecuzione di un determinato task
    /// </summary>
    public class TimeInfo : EventArgs
    {
        public TimeInfo()
        {
            this.StartTime = DateTime.Now;
        }

        /// <summary>
        /// Orario di inizio del task
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Orario di fine del task
        /// </summary>
        public DateTime EndTime { get; set; }

        public TimeSpan ElapsedTime { get { return this.EndTime - this.StartTime; } }
    }
}
