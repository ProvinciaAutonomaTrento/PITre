using System;
using System.Web;

namespace DocsPAWA.Interoperabilita
{
	/// <summary>
	/// Classe per la gestione dell'inserimento e reperimento
	/// dalla sessione dell'oggetto "SendDocumentResponse" relativo
	/// alla spedizione del documento
	/// </summary>
	public sealed class SendDocumentSessionManager
	{
		private const string SESSION_KEY="SendDocumentSessionManager.SendDocumentResponse";

		private SendDocumentSessionManager()
		{
		}

		public static DocsPAWA.DocsPaWR.SendDocumentResponse CurrentSendDocumentResponse
		{
			get
			{
				return HttpContext.Current.Session[SESSION_KEY] as DocsPAWA.DocsPaWR.SendDocumentResponse;
			}
			set
			{
				if (value==null)
					HttpContext.Current.Session.Remove(SESSION_KEY);
				else 
					HttpContext.Current.Session[SESSION_KEY]=value;
			}
		}
	}
}
