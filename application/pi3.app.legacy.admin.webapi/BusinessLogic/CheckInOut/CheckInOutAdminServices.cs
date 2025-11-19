// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using BusinessLogic.Amministrazione;
using DocsPaDB;
using DocsPaDocumentale.Documentale;
using DocsPaVO.CheckInOut;
using DocsPaVO.utente;
using DocsPaVO.Validations;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.CheckInOut
{
    public class CheckInOutAdminServices
    {
        private static ILogger logger = Serilog.Log.ForContext(typeof(CheckInOutAdminServices));
        /// <summary>
        /// Costanti che identificano le tipologie di errore
        /// </summary>
        private const string USER_NOT_ADMIN = "USER_NOT_ADMIN";
        private const string FORCE_UNDO_CHECK_OUT_ERROR = "FORCE_UNDO_CHECK_OUT_ERROR";

        /// <summary>
        /// Costanti che identificano i messaggi di validazione
        /// </summary>
        private const string MSG_USER_NOT_ADMIN = "Per effettuare l'operazione richiesta è necessario accedere al sistema come utente amministratore";
        private const string MSG_FORCE_UNDO_CHECK_OUT_ERROR = "Si è verificato un errore nell'operazione di annullamento del blocco";

        /// <summary>
        /// Reperimento di tutti i documenti in stato CheckedOut
        /// </summary>
        /// <param name="idAdministration"></param>
        /// <param name="adminInfoUtente">Utente amministratore</param>
        /// <returns></returns>
        public static CheckOutStatus[] GetCheckOutDocuments(InfoUtente adminInfoUtente, string idAdministration)
        {
            CheckOutStatus[] retValue = null;

            // Verifica se l'utente è amministratore
            //if (IsAdminUser(adminInfoUtente))
            //{
            CheckInOutAdminDocumentManager adminDocumentMng = new CheckInOutAdminDocumentManager(adminInfoUtente);

            retValue = adminDocumentMng.GetCheckOutStatusDocuments(idAdministration);
            //}
            //else
            //   retValue = new CheckOutStatus[0];

            return retValue;
        }

        /// <summary>
        /// Verifica se, per il documentale, è possibile annullare il blocco
        /// </summary>
        /// <param name="adminInfoUtente"></param>
        /// <returns></returns>
        public static bool CanForceUndoCheckOut(InfoUtente adminInfoUtente)
        {
            CheckInOutAdminDocumentManager adminDocumentMng = new CheckInOutAdminDocumentManager(adminInfoUtente);

            return adminDocumentMng.CanForceUndoCheckOut();
        }

        /// <summary>
        /// Annullamento dello stato CheckedOut per un documento
        /// </summary>
        /// <param name="adminInfoUtente">Utente amministratore</param>
        /// <param name="checkOutAdminStatus"></param>
        /// <returns></returns>
        public static ValidationResultInfo ForceUndoCheckOut(InfoUtente adminInfoUtente, CheckOutStatus checkOutAdminStatus)
        {
            ValidationResultInfo retValue = new ValidationResultInfo();

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
            {
                //if (IsAdminUser(adminInfoUtente))
                //{
                CheckInOutAdminDocumentManager adminDocumentMng = new CheckInOutAdminDocumentManager(adminInfoUtente);

                try
                {
                    if (!adminDocumentMng.ForceUndoCheckOut(checkOutAdminStatus))
                    {
                        retValue.BrokenRules.Add(CreateBrokenRule(FORCE_UNDO_CHECK_OUT_ERROR, MSG_FORCE_UNDO_CHECK_OUT_ERROR));
                    }
                    else
                    {
                        if (BusinessLogic.Documenti.DocManager.isDocInConversionePdf(checkOutAdminStatus.IDDocument))
                        {
                            // Rimozione del documento dalla coda di conversione PDF qualora il blocco sia stato iniziato da una conversione PDF
                            BusinessLogic.Documenti.DocManager.delDocRichiestaConversionePdf(checkOutAdminStatus.IDDocument);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Debug(ex.Message);

                    retValue.BrokenRules.Add(CreateBrokenRule(FORCE_UNDO_CHECK_OUT_ERROR, ConcatErrorMessage(MSG_FORCE_UNDO_CHECK_OUT_ERROR, ex)));
                }
                //}
                //else
                //{
                //    retValue.BrokenRules.Add(CreateBrokenRule(USER_NOT_ADMIN,MSG_USER_NOT_ADMIN));
                //}

                retValue.Value = (retValue.BrokenRules.Count == 0);

                if (retValue.Value)
                    transactionContext.Complete();
            }

            return retValue;
        }

        private static BrokenRule CreateBrokenRule(string id, string description)
        {
            BrokenRule brokenRule = new BrokenRule();
            brokenRule.ID = id;
            brokenRule.Description = description;
            return brokenRule;
        }

        /// <summary>
        /// Creazione messaggio di errore
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static string ConcatErrorMessage(string errorMessage, Exception ex)
        {
            string retValue = errorMessage;

            if (retValue != string.Empty)
                retValue += ":\\n";

            retValue += ex.Message;

            return retValue;
        }


    }
}
