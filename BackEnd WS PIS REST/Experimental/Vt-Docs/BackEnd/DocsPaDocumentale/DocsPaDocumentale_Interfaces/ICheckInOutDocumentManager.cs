using System;
using DocsPaVO.CheckInOut;

namespace DocsPaDocumentale.Interfaces
{
	/// <summary>
	/// Interfaccia per la gestione del checkin/checkout di documenti
	/// </summary>
	public interface ICheckInOutDocumentManager
	{
		/// <summary>
		/// Reperimento delle informazioni di stato su un documento in stato checkedout
		/// </summary>
		/// <param name="idDocument"></param>
		/// <returns></returns>
		CheckOutStatus GetCheckOutStatus(string idDocument, string documentNumber);

		/// <summary>
		/// Verifica se un documento è in stato checkedout
		/// </summary>
		/// <param name="idDocument"></param>
		/// <returns></returns>
		bool IsCheckedOut(string idDocument, string documentNumber);

		/// <summary>
		/// Verifica se un documento è in stato checkedout
		/// </summary>
		/// <param name="idDocument"></param>
        /// <param name="ownerUser">Utente proprietario del blocco sul documento</param>
		/// <returns></returns>
		bool IsCheckedOut(string idDocument, string documentNumber, out string ownerUser);

		/// <summary>
		/// CheckOut di un documento
		/// </summary>
		/// <param name="checkOutInfo">Informazioni di stato sul documento in checkOut</param>
		/// <returns></returns>
        bool CheckOut(string idDocument, string documentNumber, string documentLocation, string machineName, out CheckOutStatus checkOutStatus);

		/// <summary>
		/// CheckIn di un documento
		/// </summary>
		/// <param name="checkOutInfo">Informazioni di stato sul documento in checkOut</param>
		/// <returns></returns>
        bool CheckIn(CheckOutStatus checkOutStatus, byte[] content, string checkInComments);
        
		/// <summary>
		/// Annullamento del CheckOut di un documento
		/// </summary>
		/// <param name="checkOutInfo">Informazioni di stato sul documento in checkOut</param>
		/// <returns></returns>
		bool UndoCheckOut(CheckOutStatus checkOutStatus);
	}
}