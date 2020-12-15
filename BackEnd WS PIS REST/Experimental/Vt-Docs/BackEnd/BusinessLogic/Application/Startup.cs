using System;

namespace BusinessLogic.Application
{
	/// <summary>
	/// Questa classe gestisce le attività correlate allo startup dell'applicazione
	/// </summary>
	public class Startup
	{
		public static void ResetSemaforo()
		{
			DocsPaDB.Query_DocsPAWS.Documenti d = new DocsPaDB.Query_DocsPAWS.Documenti();
			d.ResetSemaforo();
		}
	}
}
