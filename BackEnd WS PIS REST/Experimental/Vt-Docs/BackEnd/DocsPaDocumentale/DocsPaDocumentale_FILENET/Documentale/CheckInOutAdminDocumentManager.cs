using System;
using System.Data;
using System.Collections;
using DocsPaVO.CheckInOut;
using DocsPaVO.utente;
using DocsPaDocumentale.Interfaces;

namespace DocsPaDocumentale_FILENET.Documentale
{
	/// <summary>
	/// Gestione del checkin/checkout di documenti per l'amministrazione
	/// per il documentale Filenet
	/// </summary>
	public class CheckInOutAdminDocumentManager : ICheckInOutAdminDocumentManager
	{
        private ICheckInOutAdminDocumentManager _instance = null;

        /// <summary>
        /// 
        /// </summary>
        protected ICheckInOutAdminDocumentManager Instance
        {
            get
            {
                if (this._instance == null)
                    this._instance = new DocsPaDocumentale_ETDOCS.Documentale.CheckInOutAdminDocumentManager(this.InfoUtente);
                return this._instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private DocsPaVO.utente.InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public CheckInOutAdminDocumentManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }
	
		#region ICheckInOutAdminDocumentManager Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkOutAdminStatus"></param>
        /// <returns></returns>
		public bool ForceUndoCheckOut(CheckOutStatus checkOutAdminStatus)
		{
			return this.Instance.ForceUndoCheckOut(checkOutAdminStatus);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministration"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
		public CheckOutStatus[] GetCheckOutStatusDocuments(string idAmministration, string idUser)
		{
            return this.Instance.GetCheckOutStatusDocuments(idAmministration, idUser);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministration"></param>
        /// <returns></returns>
        public CheckOutStatus[] GetCheckOutStatusDocuments(string idAmministration)
		{
            return this.Instance.GetCheckOutStatusDocuments(idAmministration);
		}

		/// <summary>
		/// Verifica se, per il documentale, è possibile annullare il blocco
		/// </summary>
		/// <returns></returns>
		public bool CanForceUndoCheckOut()
		{
            return this.Instance.CanForceUndoCheckOut();
		}

		#endregion

        #region Protected methods

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

        #endregion
    }
}
