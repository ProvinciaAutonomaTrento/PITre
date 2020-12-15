using System;
using System.Collections;

namespace Amministrazione.Manager
{
	/// <summary>
	/// Classe per l'interazione con i servizi 
	/// per la gestione del titolario
	/// </summary>
	public class TitolarioManager
	{
		#region Public members

		public TitolarioManager()
		{
		}

		/// <summary>
		/// Reperimento di tutti i ruoli che hanno, tramite il registro,
		/// la visibilità su un nodo di titolario.
		/// Se non viene fornito l'idRegistro, verranno ricercati i 
		/// ruoli in base a tutti i registri presenti nell'amministrazione richiesta.
		/// </summary>
		/// <param name="idAmministrazione"></param>
		/// <param name="idNodoTitolario"></param>
		/// <param name="idRegistro"></param>
		/// <returns></returns>
        public DocsPAWA.DocsPaWR.OrgRuoloTitolario[] GetRuoliInTitolario(string idNodoTitolario, string idRegistro, string codiceRicerca, string tipoRicerca)
		{
            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            return ws.AmmGetRuoliInTitolario(idNodoTitolario, idRegistro, codiceRicerca, tipoRicerca);
		}

//		/// <summary>
//		/// Caricamento di tutti i ruoli che hanno la visibilità
//		/// sul nodo di titolario (mediante il registro associato)
//		/// fornito in ingresso.
//		/// </summary>
//		/// <param name="nodoTitolario"></param>
//		public void FillListRuoliInTitolario(DocsPAWA.DocsPaWR.OrgNodoTitolario nodoTitolario)
//		{
//			DocsPaWR.DocsPaWebService ws=new DocsPAWA.DocsPaWR.DocsPaWebService();
//			ws.AmmFillListRuoliInTitolario(nodoTitolario);
//		}

		#endregion

		#region Private members

		#endregion
	}
}
