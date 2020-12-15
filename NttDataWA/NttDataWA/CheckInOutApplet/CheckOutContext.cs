using System;
using System.Web;
using NttDataWA.DocsPaWR;

namespace NttDataWA.CheckInOutApplet
{
	/// <summary>
	/// Classe per la gestione del contesto di CheckOut di un documento
	/// </summary>
	/// <remarks>
	/// L'oggetto viene mantenuto in sessione finché non viene 
	/// effettuato il CkeckIn del documento
	/// </remarks>
	public sealed class CheckOutAppletContext
	{
        /// <summary>
        /// 
        /// </summary>
        public CheckOutAppletContext(SchedaDocumento schedaDocumento)
        {
            this._schedaDocumento = schedaDocumento;
        }

        public CheckOutAppletContext(Allegato allegato)
        {
            this._allegato = allegato;
        }

        private const string SESSION_KEY = "CheckOutAppletContext";

		/// <summary>
		/// Reperimento contesto di checkout corrente
		/// </summary>
		public static CheckOutAppletContext Current
		{
			get
            {
                if (HttpContext.Current.Session[SESSION_KEY] != null)
                    return (CheckOutAppletContext)HttpContext.Current.Session[SESSION_KEY];
                else
                    return null;
			}
			set
			{
                CheckOutAppletContext current = HttpContext.Current.Session[SESSION_KEY] as CheckOutAppletContext;

                if (value == null && current != null && current._schedaDocumento != null)
                {
                    // Rimozione informazioni di stato sul checkout dal documento corrente
                    current._schedaDocumento.checkOutStatus = null;
                    current._allegato = null;
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

        private Allegato _allegato = null;

        /// <summary>
        /// Eventuali commenti impostati in fase di checkin
        /// </summary>
		private string _checkInComments=string.Empty;

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
        /// Informazioni di stato sul checkout corrente
        /// </summary>
        public CheckOutStatus AllStatus
        {
            get
            {
                return UIManager.DocumentManager.GetCheckOutDocumentStatus(this._allegato.docNumber);
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
				this._checkInComments=value;
			}
		}
	}
}