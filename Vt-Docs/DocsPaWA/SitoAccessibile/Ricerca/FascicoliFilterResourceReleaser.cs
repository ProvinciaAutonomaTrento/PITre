using System;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Classe contenente la logica per il rilascio delle risorse 
	/// relativamente ai filtri dei fascicoli
	/// </summary>
	public class FascicoliFilterResourceReleaser : IResourceReleaser
	{
		public FascicoliFilterResourceReleaser()
		{
		}

		/// <summary>
		/// Metodo necessario per il rilascio delle risorse
		/// </summary>
		public void ReleaseResources()
		{
			// Rilascio del filtro corrente dei fascicoli
			RicercaFascicoliHandler.CurrentFilter=null;
		}
	}
}
