using System;
using System.Data;
using DocsPaDocumentale.Documentale;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.utente;
using DocsPaVO.documento;
using DocsPaVO.CheckInOut;
using DocsPaVO.Validations;
using DocsPaDB;
using log4net;

namespace BusinessLogic.CheckInOut
{
    /// <summary>
    /// Gestione dei servizi relativi al CheckIn / CheckOut dei documenti;
    /// accessibili solamente da utenti amministratori.
    /// NB: Per sapere se un utente è amministratore,
    /// verificare che la proprietà "amministratore"
    /// dell'oggetto "Utente"
    /// </summary>
    public sealed class CheckInOutAdminServices
    {
        private static ILog logger = LogManager.GetLogger(typeof(CheckInOutAdminServices));
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

        private CheckInOutAdminServices()
        {
        }

        #region Public methods

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
        /// Reperimento di tutti i documenti in stato CheckedOut relativamente ad un utente
        /// </summary>
        /// <param name="adminInfoUtente"></param>
        /// <param name="idAdministration"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public static CheckOutStatus[] GetCheckOutDocuments(InfoUtente adminInfoUtente, string idAdministration, string idUser)
        {
            CheckOutStatus[] retValue = null;

            // Verifica se l'utente è amministratore
            //if (IsAdminUser(adminInfoUtente))
            //{
            CheckInOutAdminDocumentManager adminDocumentMng = new CheckInOutAdminDocumentManager(adminInfoUtente);

            retValue = adminDocumentMng.GetCheckOutStatusDocuments(idUser);
            //}
            //else
            //retValue = new CheckOutStatus[0];

            return retValue;
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

        #endregion

        #region Private methods

        /// <summary>
        /// Verifica se l'utente richiesto è amministratore
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private static bool IsAdminUser(InfoUtente user)
        {
            bool retValue = false;

            if (user != null)
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_IS_ADMIN_USER");
                queryDef.setParam("idUser", user.idPeople);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                try
                {
                    using (DBProvider dbProvider = new DBProvider())
                    {
                        string outParam;
                        if (dbProvider.ExecuteScalar(out outParam, commandText))
                            retValue = (outParam == "1");
                    }
                }
                catch (Exception ex)
                {
                    logger.Debug(ex.Message);
                }
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <returns></returns>
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

        #endregion
    }
}
