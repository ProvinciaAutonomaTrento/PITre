using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.CheckInOut;
using DocsPaUtils.LogsManagement;

namespace DocsPaDocumentale_GFD.Documentale
{
    /// <summary>
    /// Classe per la gestione, a livello amministrativo, del checkIn/checkOut dell'ultima versione di un documento.
    /// Consente di enumerare i documenti bloccati e forzare lo sblocco di un documento.
    /// </summary>
    public class CheckInOutAdminDocumentManager : ICheckInOutAdminDocumentManager
    {
        #region Ctors, constants, variables

        /// <summary>
        /// 
        /// </summary>
        private DocsPaVO.utente.InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        private ICheckInOutAdminDocumentManager _instance = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public CheckInOutAdminDocumentManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Public methods

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
        /// <param name="checkOutAdminStatus"></param>
        /// <returns></returns>
        public bool ForceUndoCheckOut(CheckOutStatus checkOutAdminStatus)
        {
            return this.Instance.ForceUndoCheckOut(checkOutAdminStatus);
        }

        /// <summary>
        /// 
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
        protected DocsPaVO.utente.InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

        /// <summary>
        /// Reperimento istanza manager del documentale utilizzato dall'orchestratore
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

        #endregion
    }
}