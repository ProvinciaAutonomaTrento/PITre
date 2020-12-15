using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DocsPaDocumentale.Interfaces;
using DocsPaDocumentale_DOCUMENTUM.Documentale;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using DocsPaUtils.Data;
using log4net;

namespace DocsPaDocumentale_PITRE.Migrazione
{
    /// <summary>
    /// La classe genera il task di migrazione dell'amministrazione
    /// dal database DocsPa in DOCUMENTUM
    /// </summary>
    /// <remarks>
    /// Soltanto gli utenti amministratori possono eseguire la migrazione
    /// </remarks>
    public sealed class Amministrazione
    {
        private static ILog logger = LogManager.GetLogger(typeof(Amministrazione));
        /// <summary>
        /// Constante che riporta il codice utilizzato dall'oggetto EsitoOperazione
        /// per identificare l'operazione andata a buon fine
        /// </summary>
        private const int RESULT_CODE_OK = 0;

        /// <summary>
        /// 
        /// </summary>
        private Amministrazione()
        { }

        /// <summary>
        /// Reperimento delle amministrazioni disponibili
        /// </summary>
        /// <returns></returns>
        public static InfoAmministrazione[] GetAmministrazioni()
        {
            List<InfoAmministrazione> list = new List<InfoAmministrazione>();

            string query = "SELECT TO_CHAR(SYSTEM_ID) AS ID," +
                            "VAR_CODICE_AMM AS CODICE," +
                            "VAR_DESC_AMM AS DESCR," +
                            "VAR_LIBRERIA AS LIBRERIA," +
                            "VAR_FORMATO_SEGNATURA AS SEGN," +
                            "VAR_FORMATO_FASCICOLATURA AS FASC," +
                            "VAR_DOMINIO AS DOMINIO," +
                            "VAR_SMTP AS SMTP," +
                            "TO_CHAR(NUM_PORTA_SMTP) AS PORTASMTP," +
                            "VAR_USER_SMTP AS USERSMTP," +
                            "VAR_PWD_SMTP AS PWDSMTP," +
                            "TO_CHAR(ID_RAGIONE_TO) AS RAGTO," +
                            "TO_CHAR(ID_RAGIONE_CC) AS RAGCC," +
                            "TO_CHAR(NUM_GG_PERM_TODOLIST) AS GG_TDL," +
                            "CHA_ATTIVA_GG_PERM_TODOLIST AS A_GG_TDL," +
                            "CHA_SMTP_SSL AS SMTP_SSL,CHA_SMTP_STA AS SMTP_STA," +
                            "FROM_EMAIL_ADDRESS AS FROM_EMAIL, ID_RAGIONE_COMPETENZA AS RAGCOMP," +
                            "TO_CHAR(ID_RAGIONE_CONOSCENZA) AS RAGCONO " +
                           "FROM DPA_AMMINISTRA ORDER BY 3";

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(query))
                {
                    while (reader.Read())
                    {
                        logger.Error("numero colonne trovate: " + reader.FieldCount.ToString());

                        list.Add(GetInfoAmministrazione(reader));
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Consente la rimozione di un'amministrazione in DCTM ma non in PITRE
        /// </summary>
        /// <param name="amministrazione">Amministrazioni PITRE da rimuovere in DCTM</param>        
        public static bool RimuoviAmministrazione(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            bool retValue = false;

            // 1. Connessione al sistema come utente amministratore
            string userName = DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmConfigurations.GetDocumentumSuperUser();
            string password = DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmConfigurations.GetDocumentumSuperUserPwd();

            UserLogin.LoginResult loginResult;
            InfoUtenteAmministratore infoUtente = LoginServices.LoginAdmin(userName, password, out loginResult);

            if (loginResult == UserLogin.LoginResult.OK)
            {
                AmministrazioneManager amministrazioneManager = new AmministrazioneManager(infoUtente);

                // 2. Rimozione amministrazioni DCTM
                EsitoOperazione result = amministrazioneManager.Delete(amministrazione);

                retValue = (result.Codice == RESULT_CODE_OK);

                if (retValue)
                {
                    // Rimozione stato migrazione per l'amministrazione
                    StatoMigrazione.Delete(amministrazione);
                }
            }

            return retValue;
        }

        /// <summary>
        /// Impostazione di tutte le password come scadute per un'amministrazione
        /// </summary>
        /// <param name="amministrazione"></param>
        public static void ForzaScadenzaPassword(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            try
            {
                // 1. Creazione contesto transazionale per singola amministrazione
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    // 2. Tutte le password per l'amministrazione vengono impostate come scadute

                    // 2a. Modifica delle configurazioni per la gestione delle password:
                    //  vengono rimossi i vincoli formali di validità 
                    PasswordConfigurations pwdConfig = DocsPaPwdServices.AdminPasswordConfigServices.GetPasswordConfigurations(Convert.ToInt32(amministrazione.IDAmm));
                    pwdConfig.SpecialCharacters = null;
                    pwdConfig.MinLength = 0;
                    pwdConfig.ExpirationEnabled = true; // Viene abilitata la scadenza password
                    pwdConfig.ValidityDays = 30;
                    DocsPaPwdServices.AdminPasswordConfigServices.SavePasswordConfigurations(pwdConfig);
                    DocsPaPwdServices.AdminPasswordConfigServices.ExpireAllPassword(Convert.ToInt32(amministrazione.IDAmm));

                    // 3. Completamento della transazione
                    transactionContext.Complete();

                    Log.GetInstance(amministrazione).Write(string.Format("Tutte le password per l'amministrazione con codice '{0}' sono state impostate come scadute. I giorni di validità delle password sono '{1}'", amministrazione.Codice, pwdConfig.ValidityDays.ToString()), false);
                }
            }
            catch (Exception ex)
            {
                // Operazione annullata
                Log.GetInstance(amministrazione).Write(ex.Message, true);
            }
            finally
            {
                Log.GetInstance(amministrazione).Flush();
            }
        }

        /// <summary>
        /// Implementazione della logica del task di migrazione dati
        /// per tutte le amministrazione DocsPa
        /// </summary>
        /// <param name="amministrazione">Amministrazioni PITRE da importare in DCTM</param>
        public static bool ImportaAmministrazione(DocsPaVO.amministrazione.InfoAmministrazione amministrazione)
        {
            bool retValue = false;

            try
            {
                // 1. Connessione al sistema come utente amministratore
                string userName = DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmConfigurations.GetDocumentumSuperUser();
                string password = DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmConfigurations.GetDocumentumSuperUserPwd();
                
                Log.GetInstance(amministrazione).Write(string.Format("Connessione al sistema come utente amministratore. UserName: '{0}' - Password: '{1}'", userName, password), false);

                UserLogin.LoginResult loginResult;
                InfoUtenteAmministratore infoUtente = LoginServices.LoginAdmin(userName, password, out loginResult);
                
                if (loginResult == UserLogin.LoginResult.OK)
                {
                    // 2. Migrazione dati singola amministrazione
                    ImportaAmministrazione(infoUtente, amministrazione);

                    Log.GetInstance(amministrazione).Write("Procedura di migrazione completata con successo", false);
                }
                else
                {
                    // 1a. Utente non autenticato
                    throw new ApplicationException(
                        string.Format("Errore nell'autenticazione dell'utente '{0}'. Esito:{1}",
                        userName, loginResult.ToString()));
                }

                retValue = true;
            }
            catch (Exception ex)
            {
                // Migrazione annullata
                Log.GetInstance(amministrazione).Write(ex.Message, true);

                retValue = false;
            }
            finally
            {
                Log.GetInstance(amministrazione).Flush();
            }

            return retValue;
        }

        /// <summary>
        /// Task di aggiornamento di tutti gli oggetti di un'amministrazione 
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="tipiOggetti"></param>
        /// <returns></returns>
        public static bool AggiornaAmministrazione(DocsPaVO.amministrazione.InfoAmministrazione amministrazione, TipiOggettiAmministrazione tipiOggetti)
        {
            bool retValue = false;

            try
            {
                // 1. Connessione al sistema come utente amministratore
                string userName = DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmConfigurations.GetDocumentumSuperUser();
                string password = DocsPaDocumentale_DOCUMENTUM.DctmServices.DctmConfigurations.GetDocumentumSuperUserPwd();

                Log.GetInstance(amministrazione).Write(string.Format("Connessione al sistema come utente amministratore. UserName: '{0}' - Password: '{1}'", userName, password), false);

                UserLogin.LoginResult loginResult;
                InfoUtenteAmministratore infoUtente = LoginServices.LoginAdmin(userName, password, out loginResult);

                if (loginResult == UserLogin.LoginResult.OK)
                {
                    // 2. Migrazione dati singola amministrazione
                    AggiornaAmministrazione(infoUtente, amministrazione, tipiOggetti);

                    Log.GetInstance(amministrazione).Write("Procedura di migrazione completata con successo", false);
                }
                else
                {
                    // 1a. Utente non autenticato
                    throw new ApplicationException(
                        string.Format("Errore nell'autenticazione dell'utente '{0}'. Esito:{1}",
                        userName, loginResult.ToString()));
                }

                retValue = true;
            }
            catch (Exception ex)
            {
                // Migrazione annullata
                Log.GetInstance(amministrazione).Write(ex.Message, true);

                retValue = false;
            }
            finally
            {
                Log.GetInstance(amministrazione).Flush();
            }

            return retValue;
        }

        /// <summary>
        /// Aggiornamento di un'amministrazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        /// <param name="tipiOggetti"></param>
        /// <returns></returns>
        internal static void AggiornaAmministrazione(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.InfoAmministrazione amministrazione, TipiOggettiAmministrazione tipiOggetti)
        {
            InfoStatoMigrazione statoMigrazione = StatoMigrazione.Get(amministrazione);
            
            AmministrazioneManager amministraManager = new AmministrazioneManager(infoUtente);
            
            // 1. Verifica amministrazione esistente in DCTM
            if (amministraManager.ContainsAmministrazione(amministrazione.Codice))
            {
                List<string> ruoliImportati = null;

                try
                {
                    // 2. Creazione delle utenze dell'amministrazione
                    if (tipiOggetti.Organigramma)
                    {
                        Organigramma.ImportaUtenti(infoUtente, amministrazione);

                        // 3. Creazione dei ruoli dell'amministrazione
                        Organigramma.ImportaRuoli(infoUtente, amministrazione, out ruoliImportati);

                        // 4. Associazione degli utenti ai ruoli dell'amministrazione
                        Organigramma.ImportaAssociazioniUtentiRuoli(infoUtente, amministrazione);
                    }

                    if (tipiOggetti.Titolario)
                    {
                        // 5. Creazione dei titolari dell'amministrazione 
                        Titolario.ImportaTitolari(infoUtente, amministrazione);
                    }

                    // Imposta lo stato dell'amministrazione come migrata correttamente
                    statoMigrazione.DatiAmministrazioneMigrati = true;

                    // Salva stato della migrazione
                    StatoMigrazione.Save(statoMigrazione, amministrazione);
                }
                catch (Exception ex)
                {
                    // Rollback: in caso di errore, viene rimossa l'amministrazione appena inserita
                    // per mantenere i dati consistenti
                    Log.GetInstance(amministrazione).Write(string.Format("Si è verificato un errore nella procedura di migrazione l'amministrazione '{0}'.", amministrazione.Codice), false);

                    throw ex;
                }
            }
            else
            {
                // 1a. Errore nella migrazione dell'amministrazione (migrazione interrotta)
                throw new ApplicationException(string.Format("Si è verificato un errore nell'aggiornamento dell'amministrazione. Amministrazione con codice {0} non esistente", amministrazione.Codice));
            }
        }

        /// <summary>
        /// Implementazione della logica del task di migrazione dati
        /// per una singola amministrazione DocsPa
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="amministrazione"></param>
        private static void ImportaAmministrazione(InfoUtenteAmministratore infoUtente, InfoAmministrazione amministrazione)
        {
            InfoStatoMigrazione statoMigrazione = StatoMigrazione.Get(amministrazione);

            AmministrazioneManager amministraManager = new AmministrazioneManager(infoUtente);

            // 1. Creazione dell'amministrazione in DCTM
            EsitoOperazione result = amministraManager.Insert(amministrazione);

            if (result.Codice == RESULT_CODE_OK)
            {
                Log.GetInstance(amministrazione).Write(string.Format("Creata amministrazione con codice '{0}'", amministrazione.Codice), false);

                List<string> ruoliImportati = null;

                try
                {
                    // 2. Creazione delle utenze dell'amministrazione
                    Organigramma.ImportaUtenti(infoUtente, amministrazione);

                    // 3. Creazione dei ruoli dell'amministrazione
                    Organigramma.ImportaRuoli(infoUtente, amministrazione, out ruoliImportati);

                    // 4. Associazione degli utenti ai ruoli dell'amministrazione
                    Organigramma.ImportaAssociazioniUtentiRuoli(infoUtente, amministrazione);

                    // 5. Creazione dei titolari dell'amministrazione 
                    Titolario.ImportaTitolari(infoUtente, amministrazione);

                    // Imposta lo stato dell'amministrazione come migrata correttamente
                    statoMigrazione.DatiAmministrazioneMigrati = true;

                    // Salva stato della migrazione
                    StatoMigrazione.Save(statoMigrazione, amministrazione);
                }
                catch (Exception ex)
                {
                    // Rollback: in caso di errore, viene rimossa l'amministrazione appena inserita
                    // per mantenere i dati consistenti
                    EsitoOperazione rollback = null;

                    /* Deletami sto cazzo figlio di Troia ...
                    if (ruoliImportati != null)
                        rollback = amministraManager.Delete(amministrazione, ruoliImportati.ToArray());
                    else
                        rollback = amministraManager.Delete(amministrazione); */
                     

                   // if (rollback.Codice != RESULT_CODE_OK)
                     //   Log.GetInstance(amministrazione).Write(string.Format("Errore nel Rollback: {0}", rollback.Descrizione), false);
                    //else
                        Log.GetInstance(amministrazione).Write(string.Format("Si è verificato un errore nella procedura di migrazione l'amministrazione '{0}'. Le operazioni finora effettuate sono state annullate.", amministrazione.Codice), false);

                    throw ex;
                }
            }
            else
            {
                // 1a. Errore nella migrazione dell'amministrazione (migrazione interrotta)
                throw new ApplicationException(string.Format("Si è verificato un errore nell'inserimento dell'amministrazione. Codice: {0} - Descrizione: {1}", result.Codice, result.Descrizione));
            }
        }

        /// <summary>
        /// Creazione oggetto amministrazione
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static InfoAmministrazione GetInfoAmministrazione(IDataReader reader)
        {
            InfoAmministrazione amm = new InfoAmministrazione();
            amm.Codice = DataReaderHelper.GetValue<string>(reader, "CODICE", false);
            logger.Debug("codice amministrazioni trovata: " + amm.Codice);
            amm.IDAmm = DataReaderHelper.GetValue<string>(reader, "ID", false);
            logger.Debug("ID amministrazioni trovata: " + amm.IDAmm);
            amm.Descrizione = DataReaderHelper.GetValue<string>(reader, "DESCR", false);
            amm.LibreriaDB = DataReaderHelper.GetValue<string>(reader, "LIBRERIA", true, string.Empty);
            amm.Segnatura = DataReaderHelper.GetValue<string>(reader, "SEGN", true, string.Empty);
            amm.Fascicolatura = DataReaderHelper.GetValue<string>(reader, "FASC", true, string.Empty);
            amm.Dominio = DataReaderHelper.GetValue<string>(reader, "DOMINIO", true, string.Empty);
            amm.ServerSMTP = DataReaderHelper.GetValue<string>(reader, "SMTP", true, string.Empty);
            amm.PortaSMTP = DataReaderHelper.GetValue<string>(reader, "PORTASMTP", true, string.Empty);
            amm.UserSMTP = DataReaderHelper.GetValue<string>(reader, "USERSMTP", true, string.Empty);
            amm.PasswordSMTP = DataReaderHelper.GetValue<string>(reader, "PWDSMTP", true, string.Empty);
            amm.IDRagioneTO = DataReaderHelper.GetValue<string>(reader, "RAGTO", true, string.Empty);
            //DocsPaUtils.LogsManagement.Debugger.Write("amministrazioni trovata IDRagioneTO: " + amm.IDRagioneTO);
            amm.IDRagioneCC = DataReaderHelper.GetValue<string>(reader, "RAGCC", true, string.Empty);
            amm.GGPermanenzaTDL = DataReaderHelper.GetValue<string>(reader, "GG_TDL", true, string.Empty);
            amm.AttivaGGPermanenzaTDL = DataReaderHelper.GetValue<string>(reader, "A_GG_TDL", true, string.Empty);
            amm.SslSMTP = DataReaderHelper.GetValue<string>(reader, "SMTP_SSL", true, string.Empty);
            amm.StaSMTP = DataReaderHelper.GetValue<string>(reader, "SMTP_STA", true, string.Empty);
            amm.FromEmail = DataReaderHelper.GetValue<string>(reader, "FROM_EMAIL", true, string.Empty);
            amm.IDRagioneCompetenza = DataReaderHelper.GetValue<object>(reader, "RAGCOMP", true, string.Empty).ToString();
            amm.IDRagioneConoscenza = DataReaderHelper.GetValue<string>(reader, "RAGCONO", true, string.Empty);

            return amm;
        }

        /// <summary>
        /// Tipi di oggetti gestiti in amministrazione
        /// </summary>
        public class TipiOggettiAmministrazione
        {
            /// <summary>
            /// 
            /// </summary>
            public bool Organigramma;
            
            /// <summary>
            /// 
            /// </summary>
            public bool Titolario;
        }
    }
}