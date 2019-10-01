using System;
using NttDataWA.DocsPaWR;

namespace NttDataWA.CheckInOut
{
	/// <summary>
	/// Classe per la gestione dei servizi di amministrazione
	/// relativamente al checkin/checkout dei documenti
	/// </summary>
	public sealed class CheckInOutAdminServices
	{
		private static DocsPaWebService _webServices=null;

		static CheckInOutAdminServices()
		{
			_webServices=new DocsPaWebService();
		}

		#region Public methods

		/// <summary>
		/// Reperimento di tutti i documenti in stato CheckedOut
		/// </summary>
		/// <param name="adminInfoUtente"></param>
        /// <param name="idAdministration"></param>
		/// <returns></returns>
		public static CheckOutStatus[] GetCheckOutAdminDocuments(InfoUtente adminInfoUtente, string idAdministration)
		{
            return _webServices.GetCheckOutAdminDocuments(adminInfoUtente, idAdministration);
		}

		/// <summary>
		/// Reperimento di tutti i documenti in stato CheckedOut relativamente ad un utente
		/// </summary>
		/// <param name="adminInfoUtente"></param>
        /// <param name="idAdministration"></param>
        /// <param name="idUser"></param>
		/// <returns></returns>
		public static CheckOutStatus[] GetCheckOutAdminDocumentsUser(InfoUtente adminInfoUtente, string idAdministration, string idUser)
		{
            return _webServices.GetCheckOutAdminDocumentsUser(adminInfoUtente, idAdministration, idUser);
		}

		/// <summary>
		/// Annullamento dello stato CheckedOut per un documento
		/// </summary>
		/// <param name="adminInfoUtente">Utente amministratore</param>
		/// <param name="checkOutStatus"></param>
		/// <returns></returns>
		public static ValidationResultInfo ForceUndoCheckOutAdminDocument(InfoUtente adminInfoUtente,CheckOutStatus checkOutAdminStatus)
		{
			return _webServices.ForceUndoCheckOutAdminDocument(adminInfoUtente,checkOutAdminStatus);
		}

		/// <summary>
		///  Verifica se lo stato CheckedOut per un documento può 
		/// essere annullato da un utente amministratore
		/// </summary>
		/// <param name="adminInfoUtente"></param>
		/// <returns></returns>
		public static bool CanForceUndoCheckOutAdminDocument(InfoUtente adminInfoUtente)
		{
			return _webServices.CanForceUndoCheckOutAdminDocument(adminInfoUtente);
		}

		#endregion

		#region Private methods

		#endregion
	}
}
