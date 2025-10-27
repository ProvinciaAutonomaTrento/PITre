// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaDocumentale.Interfaces;
using DocsPaVO.CheckInOut;
using DocsPaVO.utente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaDocumentale
{
    public class CheckInOutDocumentManager : ICheckInOutDocumentManager
    {
        private ICheckInOutDocumentManager _instance = null;

        private InfoUtente _infoUtente = null;

        private static Type _type = null;

        static CheckInOutDocumentManager()
        {
            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["documentale"]))
            //{
            //    string documentale = ConfigurationManager.AppSettings["documentale"].ToLower();

            //    if (documentale.Equals(TipiDocumentaliEnum.Etnoteam.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.CheckInOutDocumentManager);
            //    else if (documentale.Equals(TipiDocumentaliEnum.Hummingbird.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_HUMMINGBIRD.Documentale.CheckInOutDocumentManager);
            //    else if (documentale.Equals(TipiDocumentaliEnum.Filenet.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_FILENET.Documentale.CheckInOutDocumentManager);
            //    else if (documentale.Equals(TipiDocumentaliEnum.Pitre.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_PITRE.Documentale.CheckInOutDocumentManager);
            //    else if (documentale.Equals(TipiDocumentaliEnum.CDC.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_CDC.Documentale.CheckInOutDocumentManager);
            //    else if (documentale.Equals(TipiDocumentaliEnum.GFD.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_GFD.Documentale.CheckInOutDocumentManager);


            //    //Giordano Iacozzilli  08/10/2012 Aggiunta strato SharePoint
            //    else if (documentale.Equals(TipiDocumentaliEnum.SharePoint.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_CDC_SP.Documentale.CheckInOutAdminDocumentManager);
            //    //Fine
            //}
            _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.CheckInOutDocumentManager);
        }

        protected ICheckInOutDocumentManager Instance
        {
            get
            {
                return this._instance;
            }
        }

        public CheckInOutDocumentManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;

            this._instance = (ICheckInOutDocumentManager)Activator.CreateInstance(_type, infoUtente);
        }
        public DocsPaVO.CheckInOut.CheckOutStatus GetCheckOutStatus(string idDocument, string documentNumber)
        {
            return this.Instance.GetCheckOutStatus(idDocument, documentNumber);
        }

        public bool IsCheckedOut(string idDocument, string documentNumber)
        {
            return this.Instance.IsCheckedOut(idDocument, documentNumber);
        }

        public bool IsCheckedOut(string idDocument, string documentNumber, out string ownerUser)
        {
            return this.Instance.IsCheckedOut(idDocument, documentNumber, out ownerUser);
        }

        public bool CheckOut(string idDocument, string documentNumber, string documentLocation, string machineName, out CheckOutStatus checkOutStatus)
        {
            return this.Instance.CheckOut(idDocument, documentNumber, documentLocation, machineName, out checkOutStatus);
        }

        public bool CheckIn(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus, byte[] content, string checkInComments)
        {
            return this.Instance.CheckIn(checkOutStatus, content, checkInComments);
        }

        public bool UndoCheckOut(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus)
        {
            return this.Instance.UndoCheckOut(checkOutStatus);
        }

    }
}
