using System;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Classe contenente la logica per il rilascio delle risorse 
	/// relativamente ai filtri delle trasmissioni
	/// </summary>
	public class TrasmissioniFilterResourceReleaser : IResourceReleaser
	{
		public TrasmissioniFilterResourceReleaser()
		{
		}

		/// <summary>
		/// Metodo necessario per il rilascio delle risorse
		/// </summary>
		public void ReleaseResources()
		{
			// Rilascio del filtro corrente delle trasmissioni
			RicercaTrasmissioniHandler.CurrentFilter=null;
		}
	}
}