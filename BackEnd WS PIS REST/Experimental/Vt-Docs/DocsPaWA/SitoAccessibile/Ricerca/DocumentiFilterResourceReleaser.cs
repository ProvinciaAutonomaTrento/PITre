using System;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Classe contenente la logica per il rilascio delle risorse 
	/// relativamente ai filtri dei documenti
	/// </summary>
	public class DocumentiFilterResourceReleaser : IResourceReleaser
	{
		public DocumentiFilterResourceReleaser()
		{
		}

		/// <summary>
		/// Metodo necessario per il rilascio delle risorse
		/// </summary>
		public void ReleaseResources()
		{
			// Rilascio del filtro corrente dei documenti
			RicercaDocumentiHandler.CurrentFilter=null;
		}
	}
}
