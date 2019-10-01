using System;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.SitoAccessibile.Registri
{
	/// <summary>
	/// Classe per la gestione della logica relativa ai registri
	/// </summary>
	public class RegistroHandler
	{
		public RegistroHandler()
		{
		}

		/// <summary>
		/// Reperimento dei registri disponibili per l'utente corrente
		/// </summary>
		/// <returns></returns>
		public Registro[] GetRegistri()
		{
			DocsPaWebService ws=new DocsPaWebService();
			return ws.UtenteGetRegistri(UserManager.getInfoUtente().idCorrGlobali);
		}
	}
}
