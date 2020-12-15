using System;
using System.Web;
using System.Web.UI;

namespace DocsPAWA.SitoAccessibile
{
	/// <summary>
	/// Summary description for EnvironmentContext.
	/// </summary>
	public sealed class EnvironmentContext
	{
		private EnvironmentContext()
		{
		}

		/// TODO: 
		/// LA CLASSE DOVREBBE CONTENERE ANCHE LA LOGICA PER GESTIRE L'UTENTE
		/// 
		/// <summary>
		/// Verifica se l'utente è abilitato alla protocollazione interna
		/// </summary>
		/// <returns></returns>
		public static bool IsEnabledProtocolloInterno()
		{	
			DocsPaWR.DocsPaWebService ws=new DocsPAWA.DocsPaWR.DocsPaWebService();
			return ws.IsInternalProtocolEnabled(UserManager.getUtente().idAmministrazione);
		}

		/// <summary>
		/// Reperimento url radice dell'applicazione "SitoAccessibile"
		/// </summary>
		public static string RootPath
		{
			get
			{
				HttpRequest request=HttpContext.Current.Request;

				string rootPath="http://" + request.Url.Host + request.ApplicationPath;

				rootPath=request.Url.Scheme + "://" + request.Url.Host;

				if(!request.Url.Port.Equals(80))
					rootPath+= ":" + request.Url.Port;

				rootPath+=request.ApplicationPath + "/SitoAccessibile/";

				return rootPath;
			}
		}
	}
}