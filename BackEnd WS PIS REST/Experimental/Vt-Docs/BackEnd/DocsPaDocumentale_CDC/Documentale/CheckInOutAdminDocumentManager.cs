using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.CheckInOut;
using DocsPaUtils.LogsManagement;
using CheckInOutAdminManagerOCS = DocsPaDocumentale_OCS.Documentale.CheckInOutAdminDocumentManager;
using CheckInOutAdminManagerETDOCS = DocsPaDocumentale_ETDOCS.Documentale.CheckInOutAdminDocumentManager;


namespace DocsPaDocumentale_CDC.Documentale
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

        private CheckInOutAdminManagerOCS _checkInOutAdminOCS = null;
        private CheckInOutAdminManagerETDOCS _checkInOutAdminETDOCS = null;

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
            return this.CheckInOutAdminManagerETDOCS.GetCheckOutStatusDocuments(idAmministration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministration"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public CheckOutStatus[] GetCheckOutStatusDocuments(string idAmministration, string idUser)
        {
            return this.CheckInOutAdminManagerETDOCS.GetCheckOutStatusDocuments(idAmministration, idUser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkOutAdminStatus"></param>
        /// <returns></returns>
        public bool ForceUndoCheckOut(CheckOutStatus checkOutAdminStatus)
        {
            bool retValue = false;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                retValue = this.CheckInOutAdminManagerETDOCS.ForceUndoCheckOut(checkOutAdminStatus);
                if (retValue)
                    this.CheckInOutAdminManagerOCS.ForceUndoCheckOut(checkOutAdminStatus);
                if (retValue)
                    transactionContext.Complete();
            }
            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CanForceUndoCheckOut()
        {
            return this.CheckInOutAdminManagerETDOCS.CanForceUndoCheckOut();
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
        

        /// </summary>
        protected CheckInOutAdminManagerOCS CheckInOutAdminManagerOCS
        {
            get
            {
                if (this._checkInOutAdminOCS == null)
                    this._checkInOutAdminOCS = new CheckInOutAdminManagerOCS(this.InfoUtente);
                return this._checkInOutAdminOCS;
            }
        }

        /// </summary>
        protected CheckInOutAdminManagerETDOCS CheckInOutAdminManagerETDOCS
        {
            get
            {
                if (this._checkInOutAdminETDOCS == null)
                    this._checkInOutAdminETDOCS = new CheckInOutAdminManagerETDOCS(this.InfoUtente);
                return this._checkInOutAdminETDOCS;
            }
        }

        #endregion
    }
}
