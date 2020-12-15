using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.utente;
using DocsPaVO.CheckInOut;
using DocsPaUtils.LogsManagement;

namespace DocsPaDocumentale_OCS.Documentale
{
    /// <summary>
    /// Classe per la gestione, a livello amministrativo, del checkIn/checkOut dell'ultima versione di un documento.
    /// Consente di enumerare i documenti bloccati e forzare lo sblocco di un documento.
    /// </summary>
    public class CheckInOutAdminDocumentManager : ICheckInOutAdminDocumentManager
    {
        #region Ctros, variables, constants

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

        #endregion

        #region Public methods

        /// <summary>
        /// Reperimento dei metadati relativi a tutti i documenti bloccati nell'ambito dell'amministrazione
        /// </summary>
        /// <param name="idAmministration"></param>
        /// <returns></returns>
        public CheckOutStatus[] GetCheckOutStatusDocuments(string idAmministration)
        {
            return null;
            
            //return this.GetCheckOutStatusDocuments(query);
        }

        /// <summary>
        /// Reperimento dei metadati relativi a tutti i documenti bloccati nell'ambito dell'amministrazione
        /// con la possibilità di filtrare per utente
        /// </summary>
        /// <param name="idAmministration"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public CheckOutStatus[] GetCheckOutStatusDocuments(string idAmministration, string idUser)
        {
            return null;

           // return this.GetCheckOutStatusDocuments(query);
        }

        /// <summary>
        /// Rimozione forzata del blocco sul documento
        /// </summary>
        /// <param name="checkOutAdminStatus"></param>
        /// <returns></returns>
        public bool ForceUndoCheckOut(CheckOutStatus checkOutAdminStatus)
        {
            CheckInOutDocumentManager checkInOutMng = new CheckInOutDocumentManager(this.InfoUtente);

            return checkInOutMng.UndoCheckOut(checkOutAdminStatus);
        }

        /// <summary>
        /// Se tru, il blocco del documento può essere forzato
        /// </summary>
        /// <returns></returns>
        public bool CanForceUndoCheckOut()
        {
            return true;
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
