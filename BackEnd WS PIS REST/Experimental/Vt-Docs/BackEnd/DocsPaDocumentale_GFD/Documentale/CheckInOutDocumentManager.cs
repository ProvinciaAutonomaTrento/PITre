using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.utente;
using DocsPaVO.CheckInOut;
using DocsPaVO.documento;
using DocsPaUtils.LogsManagement;
using CheckInOutManagerETDOCS = DocsPaDocumentale_ETDOCS.Documentale.CheckInOutDocumentManager;


namespace DocsPaDocumentale_GFD.Documentale
{
    /// <summary>
    /// Classe per la gestione del checkIn/checkOut dell'ultima versione di un documento
    /// </summary>
    public class CheckInOutDocumentManager : ICheckInOutDocumentManager
    {
        #region Ctros, variables, constants

        /// <summary>
        /// 
        /// </summary>
        private ICheckInOutDocumentManager _checkInOutETDOCS = null;

        /// <summary>
        /// 
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public CheckInOutDocumentManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Reperimento delle informazioni di stato di checkout del documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <returns></returns>
        /// <remarks>
        /// Le informazioni vengono reperite solamente in ETDOCS
        /// </remarks>
        public CheckOutStatus GetCheckOutStatus(string idDocument, string documentNumber)
        {
            return this.CheckInOutManagerETDOCS.GetCheckOutStatus(idDocument, documentNumber);
        }

        /// <summary>
        /// Verifica se il documento è in stato checkout
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <returns></returns>
        /// <remarks>
        /// Le informazioni vengono reperite solamente in ETDOCS
        /// </remarks>
        public bool IsCheckedOut(string idDocument, string documentNumber)
        {
            return this.CheckInOutManagerETDOCS.IsCheckedOut(idDocument, documentNumber);
        }

        /// <summary>
        /// Verifica se il documento è in stato checkout
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <param name="ownerUser"></param>
        /// <returns></returns>
        /// <remarks>
        /// Le informazioni vengono reperite solamente in ETDOCS
        /// </remarks>
        public bool IsCheckedOut(string idDocument, string documentNumber, out string ownerUser)
        {
            return this.CheckInOutManagerETDOCS.IsCheckedOut(idDocument, documentNumber, out ownerUser);
        }

        /// <summary>
        /// CheckOut del documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <param name="documentLocation"></param>
        /// <param name="machineName"></param>
        /// <param name="checkOutStatus"></param>
        /// <returns></returns>
        /// <remarks>
        /// Il blocco del documento viene impostato solamente in ETDOCS
        /// </remarks>
        public bool CheckOut(string idDocument, string documentNumber, string documentLocation, string machineName, out CheckOutStatus checkOutStatus)
        {
            return this.CheckInOutManagerETDOCS.CheckOut(idDocument, documentNumber, documentLocation, machineName, out checkOutStatus);
        }

        /// <summary>
        /// CheckIn del documento
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <param name="content"></param>
        /// <param name="checkInComments"></param>
        /// <returns></returns>
        /// <remarks>
        /// Il documento è in checkout solo nel documentale ETDOCS,
        /// pertanto l'operazione di checkin, oltre ad effettuare il checkin in ETDOCS,
        /// dovrà aggiungere la nuova versione anche in DCTM. Ciò comporta
        /// l'utilizzo dei servizi di checkout e checkin del documentale DCTM.
        /// </remarks>
        public bool CheckIn(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus, byte[] content, string checkInComments)
        {
            return this.CheckInOutManagerETDOCS.CheckIn(checkOutStatus, content, checkInComments);
        }

        /// <summary>
        /// UndoCheckOut del documento
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <returns></returns>
        /// <remarks>
        /// L'operazione viene effettuata solamente nel documentale ETDOCS
        /// </remarks>
        public bool UndoCheckOut(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus)
        {
            return this.CheckInOutManagerETDOCS.UndoCheckOut(checkOutStatus);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        protected ICheckInOutDocumentManager CheckInOutManagerETDOCS
        {
            get
            {
                if (this._checkInOutETDOCS == null)
                    this._checkInOutETDOCS = new CheckInOutManagerETDOCS(this._infoUtente);
                return this._checkInOutETDOCS;
            }
        }

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