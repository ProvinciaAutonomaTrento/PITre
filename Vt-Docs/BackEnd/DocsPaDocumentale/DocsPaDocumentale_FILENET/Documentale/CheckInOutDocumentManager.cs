using System;
using DocsPaVO.CheckInOut;
using DocsPaVO.utente;
using DocsPaDocumentale.Interfaces;

namespace DocsPaDocumentale_FILENET.Documentale
{
	/// <summary>
	/// Gestione del checkin/checkout di documenti per il documentale FILENET
	/// </summary>
	public class CheckInOutDocumentManager : ICheckInOutDocumentManager
	{
        private ICheckInOutDocumentManager _instance = null;

        /// <summary>
        /// 
        /// </summary>
        protected ICheckInOutDocumentManager Instance
        {
            get
            {
                if (this._instance == null)
                    this._instance = new DocsPaDocumentale_ETDOCS.Documentale.CheckInOutDocumentManager(this.InfoUtente);
                return this._instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        protected InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public CheckInOutDocumentManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

		#region ICheckInOutDocumentManager Members

		public bool UndoCheckOut(CheckOutStatus checkOutStatus)
		{	
            return this.Instance.UndoCheckOut(checkOutStatus);
		}

        public bool CheckIn(CheckOutStatus checkOutStatus, byte[] content, string checkInComments)
		{
            return this.Instance.CheckIn(checkOutStatus, content, checkInComments);
		}

		public CheckOutStatus GetCheckOutStatus(string idDocument, string documentNumber)
		{
            return this.Instance.GetCheckOutStatus(idDocument, documentNumber);
		}

        public bool IsCheckedOut(string idDocument, string documentNumber, out string ownerUser)
		{
            return this.Instance.IsCheckedOut(idDocument, documentNumber, out ownerUser);
		}

		public bool IsCheckedOut(string idDocument, string documentNumber)
		{
            return this.Instance.IsCheckedOut(idDocument, documentNumber);
		}

        public bool CheckOut(string idDocument, string documentNumber, string documentLocation, string machineName, out CheckOutStatus checkOutStatus)
		{
            return this.Instance.CheckOut(idDocument, documentNumber, documentLocation, machineName, out checkOutStatus);
		}

		#endregion
	}
}
