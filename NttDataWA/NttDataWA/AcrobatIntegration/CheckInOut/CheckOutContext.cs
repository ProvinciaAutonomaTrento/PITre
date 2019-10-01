using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;

namespace NttDataWA.CheckInOut
{
    public sealed class CheckOutContext
    {

        /// <summary>
        /// 
        /// </summary>
        public CheckOutContext(SchedaDocumento schedaDocumento)
        {
            this._schedaDocumento = schedaDocumento;
        }

        private const string SESSION_KEY = "CheckOutContext";

        /// <summary>
        /// Reperimento contesto di checkout corrente
        /// </summary>
        public static CheckOutContext Current
        {
            get
            {
                if (HttpContext.Current.Session[SESSION_KEY] != null)
                    return (CheckOutContext)HttpContext.Current.Session[SESSION_KEY];
                else
                    return null;
            }
            set
            {
                CheckOutContext current = HttpContext.Current.Session[SESSION_KEY] as CheckOutContext;

                if (value == null && current != null && current._schedaDocumento != null)
                {
                    // Rimozione informazioni di stato sul checkout dal documento corrente
                    current._schedaDocumento.checkOutStatus = null;
                }

                if (value == null)
                    HttpContext.Current.Session.Remove(SESSION_KEY);
                else
                    HttpContext.Current.Session[SESSION_KEY] = value;
            }
        }

        /// <summary>
        /// Scheda documento corrente
        /// </summary>
        private SchedaDocumento _schedaDocumento = null;

        /// <summary>
        /// Eventuali commenti impostati in fase di checkin
        /// </summary>
        private string _checkInComments = string.Empty;

        /// <summary>
        /// Informazioni di stato sul checkout corrente
        /// </summary>
        public CheckOutStatus Status
        {
            get
            {
                return this._schedaDocumento.checkOutStatus;
            }
        }

        /// <summary>
        /// Eventuali commenti inseriti nel documento al momento del CheckIn
        /// </summary>
        public string CheckInComments
        {
            get
            {
                return this._checkInComments;
            }
            set
            {
                this._checkInComments = value;
            }
        }

    }
}