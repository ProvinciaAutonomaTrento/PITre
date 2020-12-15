using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using DocsPaDocumentale.Interfaces;

namespace DocsPaDocumentale.Documentale
{
    /// <summary>
    /// Classe per l'interazione con il motore documentale corrente.
    /// Internamente istanzia e utilizza l'oggetto del motore documentale
    /// che implementa l'interfaccia "ICheckInOutAdminDocumentManager"
    /// </summary>
    public class CheckInOutAdminDocumentManager : ICheckInOutAdminDocumentManager
    {
        #region Ctros, variables, constants

        /// <summary>
        /// Tipo documentale corrente
        /// </summary>
        private static Type _type = null;

        /// <summary>
        /// Oggetto documentale corrente
        /// </summary>
        private ICheckInOutAdminDocumentManager _instance = null;

        /// <summary>
        /// Reperimento del tipo relativo al documentale corrente
        /// </summary>
        static CheckInOutAdminDocumentManager()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["documentale"]))
            {
                string documentale = ConfigurationManager.AppSettings["documentale"].ToLower();

                if (documentale.Equals(TipiDocumentaliEnum.Etnoteam.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.CheckInOutAdminDocumentManager);
                else if (documentale.Equals(TipiDocumentaliEnum.Pitre.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_PITRE.Documentale.CheckInOutAdminDocumentManager);
                else if (documentale.Equals(TipiDocumentaliEnum.CDC.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC.Documentale.CheckInOutAdminDocumentManager);
                //Giordano Iacozzilli  08/10/2012 Aggiunta strato SharePoint
                else if (documentale.Equals(TipiDocumentaliEnum.SharePoint.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC_SP.Documentale.CheckInOutAdminDocumentManager);
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
        public CheckInOutAdminDocumentManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._instance = (ICheckInOutAdminDocumentManager)Activator.CreateInstance(_type, infoUtente);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Reperimento istanza oggetto "ICheckInOutAdminDocumentManager"
        /// relativamente al documentale correntemente configurato
        /// </summary>
        protected ICheckInOutAdminDocumentManager Instance
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
        /// <param name="idAmministration"></param>
        /// <returns></returns>
        public DocsPaVO.CheckInOut.CheckOutStatus[] GetCheckOutStatusDocuments(string idAmministration)
        {
            return this.Instance.GetCheckOutStatusDocuments(idAmministration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministration"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public DocsPaVO.CheckInOut.CheckOutStatus[] GetCheckOutStatusDocuments(string idAmministration, string idUser)
        {
            return this.Instance.GetCheckOutStatusDocuments(idAmministration, idUser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkOutAdminStatus"></param>
        /// <returns></returns>
        public bool ForceUndoCheckOut(DocsPaVO.CheckInOut.CheckOutStatus checkOutAdminStatus)
        {
            return this.Instance.ForceUndoCheckOut(checkOutAdminStatus);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adminInfoUtente"></param>
        /// <returns></returns>
        public bool CanForceUndoCheckOut()
        {
            return this.Instance.CanForceUndoCheckOut();
        }

        #endregion
    }
}
