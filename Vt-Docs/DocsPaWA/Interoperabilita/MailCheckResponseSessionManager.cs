using System;
using System.Web;

namespace DocsPAWA.Interoperabilita
{
	/// <summary>
	/// Classe per la gestione dell'inserimento e reperimento
	/// dalla sessione dell'oggetto "MailCheckResponse" relativo
	/// al controllo della casella istituzionale corrente
	/// </summary>
	public sealed class MailCheckResponseSessionManager
	{
		private const string SESSION_KEY="MailCheckResponseSessionManager.MailCheckResponse";

		public MailCheckResponseSessionManager()
		{	
		}

		public static DocsPAWA.DocsPaWR.MailAccountCheckResponse CurrentMailCheckResponse
		{
			get
			{
				return HttpContext.Current.Session[SESSION_KEY] as DocsPAWA.DocsPaWR.MailAccountCheckResponse;
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