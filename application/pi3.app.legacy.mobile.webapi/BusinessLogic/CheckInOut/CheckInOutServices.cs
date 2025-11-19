// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaDocumentale;
using DocsPaVO.CheckInOut;
using DocsPaVO.utente;

namespace BusinessLogic.CheckInOut
{
    public class CheckInOutServices
    {
        private const string MSG_MISSING_ID_DOCUMENT = "IDDocumento non valido";
        private const string MSG_MISSING_DOCUMENT_NUMBER = "DocumentNumber non valido";
        private const string MSG_MISSING_USER = "Oggetto utente non valido";


        public static CheckOutStatus GetCheckOutStatus(string idDocument, string documentNumber, InfoUtente utente)
        {
            /*
                Precondizioni:
                IDDocumento valido.
                Oggetto "utente" valido.
                Il documento deve essere in stato checked out.
                L'utente fornito deve essere lo stesso che ha impostato lo stato checked out per il documento.
				
                PostCondizioni:
                Restituzione dello stato del documento.
            */
            if (idDocument == null || idDocument == string.Empty)
                throw new ApplicationException(MSG_MISSING_ID_DOCUMENT);

            if (documentNumber == null || documentNumber == string.Empty)
                throw new ApplicationException(MSG_MISSING_DOCUMENT_NUMBER);

            CheckOutStatus retValue = null;

            CheckInOutDocumentManager documentManagement = new CheckInOutDocumentManager(utente);

            string ownerUser;
            if (documentManagement.IsCheckedOut(idDocument, documentNumber, out ownerUser))
            {
                retValue = documentManagement.GetCheckOutStatus(idDocument, documentNumber);

                if (retValue != null)
                {
                    // Se il documento è in checkout, reperimento dello stato di conversione PDF
                    retValue.InConversionePdf = IsInConversionePdf(idDocument);
                }
            }

            return retValue;
        }

        private static bool IsInConversionePdf(string idDocument)
        {
            if (BusinessLogic.Documenti.DocManager.isEnabledConversionePdfServer())
            {
                // Se il documento è in checkout, reperimento dello stato di conversione PDF
                return BusinessLogic.Documenti.DocManager.isDocInConversionePdf(idDocument);
            }
            else
                return false;
        }

        /// <summary>
        /// Verifica se un documento è in checkout
        /// </summary>
        /// <param name="idDocument">SystemID del documento</param>
        /// <param name="documentNumber"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static bool IsCheckedOut(string idDocument, string documentNumber, InfoUtente utente)
        {
            /*
                Precondizioni:
                IDDocumento valido.
                Il documento deve essere in stato checked out.
				
                Postcondizioni:
                Esito dell'operazione	
            */
            if (idDocument == null || idDocument == string.Empty)
                throw new ApplicationException(MSG_MISSING_ID_DOCUMENT);

            if (documentNumber == null || documentNumber == string.Empty)
                throw new ApplicationException(MSG_MISSING_DOCUMENT_NUMBER);

            if (utente == null)
                throw new ApplicationException(MSG_MISSING_USER);

            CheckInOutDocumentManager documentManagement = new CheckInOutDocumentManager(utente);

            return documentManagement.IsCheckedOut(idDocument, documentNumber);
        }

    }
}
