using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using DocsPaVO.utente;
using DocsPaVO.CheckInOut;
using DocsPaDocumentale.Interfaces;

namespace DocsPaDocumentale.Documentale
{
    /// <summary>
    /// Classe per l'interazione con il motore documentale corrente.
    /// Internamente istanzia e utilizza l'oggetto del motore documentale
    /// che implementa l'interfaccia "ICheckInOutDocumentManager"
    /// </summary>
    public class CheckInOutDocumentManager : ICheckInOutDocumentManager
    {
        #region Ctros, variables, constants

        /// <summary>
        /// Tipo documentale corrente
        /// </summary>
        private static Type _type = null;

        /// <summary>
        /// Oggetto documentale corrente
        /// </summary>
        private ICheckInOutDocumentManager _instance = null;

        /// <summary>
        /// 
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// Reperimento de tipo relativo al documentale corrente
        /// </summary>
        static CheckInOutDocumentManager()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["documentale"]))
            {
                string documentale = ConfigurationManager.AppSettings["documentale"].ToLower();

                if (documentale.Equals(TipiDocumentaliEnum.Etnoteam.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.CheckInOutDocumentManager);
                else if (documentale.Equals(TipiDocumentaliEnum.Pitre.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_PITRE.Documentale.CheckInOutDocumentManager);
                else if (documentale.Equals(TipiDocumentaliEnum.CDC.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC.Documentale.CheckInOutDocumentManager);
                //Giordano Iacozzilli  08/10/2012 Aggiunta strato SharePoint
                else if (documentale.Equals(TipiDocumentaliEnum.SharePoint.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC_SP.Documentale.CheckInOutDocumentManager);
                //Fine
                //else if (documentale.Equals(TipiDocumentaliEnum.GFD.ToString().ToLower()))
                //    _type = typeof(DocsPaDocumentale_GFD.Documentale.TitolarioManager);
                //else if (documentale.Equals(TipiDocumentaliEnum.Hummingbird.ToString().ToLower()))
                //    _type = typeof(DocsPaDocumentale_HUMMINGBIRD.Documentale.TitolarioManager);
                //else if (documentale.Equals(TipiDocumentaliEnum.Filenet.ToString().ToLower()))
                //    _type = typeof(DocsPaDocumentale_FILENET.Documentale.TitolarioManager);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public CheckInOutDocumentManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;

            this._instance = (ICheckInOutDocumentManager)Activator.CreateInstance(_type, infoUtente);
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

        /// <summary>
        /// Reperimento istanza oggetto "ICheckInOutDocumentManager"
        /// relativamente al documentale correntemente configurato
        /// </summary>
        protected ICheckInOutDocumentManager Instance
        {
            get
            {
                return this._instance;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <returns></returns>
        public DocsPaVO.CheckInOut.CheckOutStatus GetCheckOutStatus(string idDocument, string documentNumber)
        {
            return this.Instance.GetCheckOutStatus(idDocument, documentNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <returns></returns>
        public bool IsCheckedOut(string idDocument, string documentNumber)
        {
            return this.Instance.IsCheckedOut(idDocument, documentNumber);
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
            return this.Instance.IsCheckedOut(idDocument, documentNumber, out ownerUser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        //public bool IsOwnerCheckedOut(string idDocument, string documentNumber, string idUser)
        //{
        //    return this.Instance.IsOwnerCheckedOut(idDocument, documentNumber, idUser);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <param name="user"></param>
        /// <param name="library"></param>
        /// <returns></returns>
        public bool CheckOut(string idDocument, string documentNumber, string documentLocation, string machineName, out CheckOutStatus checkOutStatus)
        {
            return this.Instance.CheckOut(idDocument, documentNumber, documentLocation, machineName, out checkOutStatus);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <param name="user"></param>
        /// <param name="library"></param>
        /// <returns></returns>
        public bool CheckIn(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus, byte[] content, string checkInComments)
        {
            return this.Instance.CheckIn(checkOutStatus, content, checkInComments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <param name="user"></param>
        /// <param name="library"></param>
        /// <returns></returns>
        public bool UndoCheckOut(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus)
        {
            return this.Instance.UndoCheckOut(checkOutStatus);
        }

        #endregion
    }
}
