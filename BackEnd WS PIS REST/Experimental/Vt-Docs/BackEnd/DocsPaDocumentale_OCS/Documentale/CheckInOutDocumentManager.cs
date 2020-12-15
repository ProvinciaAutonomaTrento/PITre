using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.utente;
using DocsPaVO.CheckInOut;
using DocsPaDocumentale_OCS.OCSServices;
using DocsPaDocumentale_OCS.CorteContentServices;
using log4net;

namespace DocsPaDocumentale_OCS.Documentale
{
    /// <summary>
    /// Classe per la gestione del checkIn/checkOut dell'ultima versione di un documento
    /// </summary>
    public class CheckInOutDocumentManager : ICheckInOutDocumentManager
    {
        private ILog logger = LogManager.GetLogger(typeof(CheckInOutDocumentManager));
        #region Ctros, variables, constants

        /// <summary>
        /// 
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// Istanza webservices documenti
        /// </summary>
        private CorteContentServices.DocumentManagementSOAPHTTPBinding _wsDocument = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public CheckInOutDocumentManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Reperimento dei metadati sullo stato del checkout del documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <returns></returns>
        public CheckOutStatus GetCheckOutStatus(string idDocument, string documentNumber)
        {
            CheckOutStatus retValue = null;

            //TODO:


            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <returns></returns>
        public bool IsCheckedOut(string idDocument, string documentNumber)
        {
            string ownerUser;
            return this.IsCheckedOut(idDocument, documentNumber, out ownerUser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <param name="checkOutStatus"></param>
        /// <returns></returns>
        public bool IsCheckedOut(string idDocument, string documentNumber, out string ownerUser)
        {
            ownerUser = string.Empty;
            //TODO:
            return false;
        }

        /// <summary>
        /// Checkout di un documento
        /// </summary>
        /// <param name="checkOutStatus">Metadati relativi allo stato del documento in checkout</param>
        /// <param name="user"></param>
        /// <param name="library"></param>
        /// <returns></returns>
        public bool CheckOut(string idDocument, string documentNumber, string documentLocation, string machineName, out CheckOutStatus checkOutStatus)
        {
            checkOutStatus = null;

            bool retValue = false;

            try
            {
                //Il documento viene messo nello stato LOCK
                long idOCS = OCSDocumentHelper.getDocumentIdOCS(documentNumber, null, OCSUtils.getApplicationUserCredentials());
                CorteContentServices.ItemIdRequestType requestId = new DocsPaDocumentale_OCS.CorteContentServices.ItemIdRequestType();
                CorteContentServices.ResultType result;
                requestId.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                requestId.itemId = idOCS; // id del documento
                result = this.WsDocumentInstance.LockDocument(requestId);

                OCSUtils.throwExceptionIfInvalidResult(result);

                retValue = true;
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.CheckOut", ex);
            }

            return retValue;
        }

        /// <summary>
        /// Checkin di un documento in stato checkout
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <param name="library"></param>
        /// <returns></returns>
        public bool CheckIn(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus, byte[] content, string checkInComments)
        {
            bool retValue = false;

            try
            {
                //// Il checkin richiede prima lo sblocco del documento,
                long idOCS = OCSDocumentHelper.getDocumentIdOCS(checkOutStatus.DocumentNumber, null, OCSUtils.getApplicationUserCredentials());

                CorteContentServices.ItemIdRequestType requestId = new DocsPaDocumentale_OCS.CorteContentServices.ItemIdRequestType();
                requestId.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                requestId.itemId = idOCS; // id del documento
                
                CorteContentServices.ResultType result = this.WsDocumentInstance.UnlockDocument(requestId);

                OCSUtils.throwExceptionIfInvalidResult(result);

                retValue = true;
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.CheckIn", ex);
            }

            return retValue;
        }

        /// <summary>
        /// UndoCheckout di un documento in stato checkout
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <param name="library"></param>
        /// <returns></returns>
        public bool UndoCheckOut(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus)
        {
            bool retValue = false;

            try
            {
                long idOCS = OCSDocumentHelper.getDocumentIdOCS(checkOutStatus.DocumentNumber, null, OCSUtils.getApplicationUserCredentials());

                CorteContentServices.ItemIdRequestType requestId = new DocsPaDocumentale_OCS.CorteContentServices.ItemIdRequestType();
                requestId.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                requestId.itemId = idOCS; // id del documento

                CorteContentServices.ResultType result = this.WsDocumentInstance.UnlockDocument(requestId);

                OCSUtils.throwExceptionIfInvalidResult(result);

                retValue = true;
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.UndoCheckOut", ex);
            }

            return retValue;
        }

        #endregion

        #region protected metod
        /// <summary>
        /// Reperimento istanza webservices per la gestione dei documenti
        /// </summary>
        protected DocumentManagementSOAPHTTPBinding WsDocumentInstance
        {
            get
            {
                if (this._wsDocument == null)
                    this._wsDocument = OCSServiceFactory.GetDocumentServiceInstance<DocumentManagementSOAPHTTPBinding>();
                return this._wsDocument;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Credenziali dell'utente connesso a documentum
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
