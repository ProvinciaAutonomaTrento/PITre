using System;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;

namespace NttDataWA.CheckInOut
{
    /// <summary>
    /// Classe per la gestione dei servizi relativi al checkin/checkout dei documenti
    /// </summary>
    public sealed class CheckInOutServices
    {

        /// <summary>
        /// 
        /// </summary>
        private static DocsPaWebService _webServices = null;

        /// <summary>
        /// Verifica se il documento principale o un allegato è in checkout, relativamente al parametro checkAllegati
        /// </summary>
        /// <param name="idDocument">SystemID del documento</param>
        /// <param name="checkedOutUser"></param>
        /// <param name="documentNumber"></param>
        /// <param name="utente">Utente che ha fatto il checkout del documento</param>
        /// <returns></returns>
        public static bool IsCheckedOutDocument(string idDocument, string documentNumber, InfoUtente utente, bool checkAllegati)
        {
            //bool retValue = false;
            //if (!string.IsNullOrEmpty(idDocument) && !string.IsNullOrEmpty(documentNumber)) retValue = _webServices.IsCheckedOutDocument(idDocument, documentNumber, utente, checkAllegati);
            //return retValue;

            return false;
        }

        /// <summary>
        /// Verifica se il documento principale è in checkout
        /// </summary>
        /// <returns></returns>
        public static bool IsCheckedOutDocument()
        {
            SchedaDocumento schedaDocumento = DocumentManager.getSelectedRecord();

            if (schedaDocumento != null && schedaDocumento.checkOutStatus != null)
                return (!string.IsNullOrEmpty(schedaDocumento.checkOutStatus.ID));
            else
                return false;
        }

        /// <summary>
        /// Verifica se un documento è in checkout
        /// </summary>
        /// <param name="ownerUser">Utente che ha fatto il checkout del documento</param>
        /// <returns></returns>
        public static bool IsCheckedOutDocumentWithUser(out string ownerUser)
        {
            bool isCheckedOut = false;
            ownerUser = string.Empty;

            SchedaDocumento schedaDocumento = DocumentManager.getSelectedRecord();

            if (schedaDocumento != null && schedaDocumento.checkOutStatus != null)
            {
                isCheckedOut = true;
                ownerUser = schedaDocumento.checkOutStatus.UserName;
            }

            return isCheckedOut;
        }

        /// <summary>
        /// Reperimento scheda documento corrente
        /// </summary>
        public static SchedaDocumento CurrentSchedaDocumento
        {
            get
            {
                DocsPaWR.SchedaDocumento schedaDocumento = null;
                DocsPaWR.FileRequest fileRequest = FileManager.getSelectedFile();
                if (fileRequest != null)
                {
                    if (fileRequest.GetType() == typeof(DocsPaWR.Allegato))
                    {
                        // reperimento dello stato checkout dell'allegato selezionato.
                        // Solo se attiva la profilazione allegati.
                        schedaDocumento = DocumentManager.getDocumentDetails(null, fileRequest.docNumber, fileRequest.docNumber);
                    }
                    else
                    {
                        // reperimento dello stato checkout del documento principale
                        schedaDocumento = DocumentManager.getSelectedRecord();
                    }
                }
                
                return schedaDocumento;
            }
        }

        /// </summary>
        /// <remarks>
        /// Qualora sia attivata la gestione degli allegati profilati, la scheda documento sarà relativa
        /// all'allegato correntemente selezionato da tab allegati
        /// </remarks>
        public static void RefreshCheckOutStatus()
        {
            DocsPaWR.SchedaDocumento schedaDocumento = null;
            DocsPaWR.FileRequest fileRequest = FileManager.getSelectedFile();
            if (fileRequest != null)
            {
                if (fileRequest.GetType() == typeof(DocsPaWR.Allegato))
                {
                    // Tab "allegati" correntemente selezionato,
                    // reperimento dello stato checkout dell'allegato selezionato.
                    // Solo se attiva la profilazione allegati.
                    if (fileRequest != null && fileRequest.GetType() == typeof(DocsPaWR.Allegato))
                    {
                        schedaDocumento = DocumentManager.getDocumentDetails(null, fileRequest.docNumber, fileRequest.docNumber);
                    }
                }
                else
                {
                    schedaDocumento = DocumentManager.getSelectedRecord();
                }

                if (schedaDocumento != null)
                    //Inizializzazione del contesto di checkout del documento
                    CheckInOut.CheckOutContext.Current = new CheckInOut.CheckOutContext(schedaDocumento);
                else
                    CheckInOut.CheckOutContext.Current = null;
            }
        }

    }
}
