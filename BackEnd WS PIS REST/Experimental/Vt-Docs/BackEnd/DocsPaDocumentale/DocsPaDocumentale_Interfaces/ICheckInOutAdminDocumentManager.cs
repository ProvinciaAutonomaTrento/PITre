using System;
using DocsPaVO.CheckInOut;
using DocsPaVO.utente;

namespace DocsPaDocumentale.Interfaces
{
	/// <summary>
	/// Interfaccia che fornisce servizi per la gestione del checkin/checkout di documenti
	/// per gli utenti amministratori
	/// </summary>
	public interface ICheckInOutAdminDocumentManager
	{
        /// <summary>
        /// Reperimento delle informazioni di stato sui documenti in stato checkedout
        /// </summary>
        /// <param name="idAmministration">
        /// ID dell'amministrazione per cui sono richieste le informazioni
        /// </param>
        /// <returns></returns>
        CheckOutStatus[] GetCheckOutStatusDocuments(string idAmministration);

		/// <summary>
		/// Reperimento delle informazioni di stato sui documenti in stato checkedout
		/// relativamente ad un utente
		/// </summary>
        /// <param name="idAmministration">
        /// ID dell'amministrazione per cui sono richieste le informazioni
        /// </param>
		/// <param name="idUser"></param>
		/// <returns></returns>
		CheckOutStatus[] GetCheckOutStatusDocuments(string idAmministration, string idUser);

		/// <summary>
		/// Annullamento del blocco del documento nel documentale
		/// </summary>
		/// <param name="checkOutAdminStatus"></param>
		/// <returns></returns>
		bool ForceUndoCheckOut(CheckOutStatus checkOutAdminStatus);

		/// <summary>
		/// Verifica se, per il documentale, è possibile annullare il blocco
		/// </summary>
		/// <returns></returns>
		bool CanForceUndoCheckOut();
	}
}