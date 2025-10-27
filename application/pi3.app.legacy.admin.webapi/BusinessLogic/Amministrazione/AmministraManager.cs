// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaDB;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using DocsPaVO.Validations;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Serilog;
using System.Collections;
using System.Xml.Serialization;

namespace BusinessLogic.Amministrazione;

public class AmministraManager
{
    private static ILogger logger = Log.ForContext(typeof(AmministraManager));

    public bool CheckSessionID(string sessionID)
    {
        bool retValue = false;

        string valore = null;
        DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECK_ADMIN_USER_LOGIN_ON_SESSION_ID");
        queryDef.setParam("param1", sessionID);
        string commandText = queryDef.getSQL();

        using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
        {
            dbProvider.ExecuteScalar(out valore, commandText);
            if (valore != null && !valore.Equals("0"))
                retValue = true;

        }
        return retValue;
    }

    public ArrayList getListaTemi()
    {
        DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
        ArrayList result = amm.GetListaTemi();
        return result;
    }

    public bool setTemaAmministrazione(string idAmm, int valore)
    {
        bool result = false;
        try
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            result = amm.setTemaAmministrazione(idAmm, valore);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in Amministra Manager - metodo setTemaAmministrazione " + e);
        }
        return result;
    }

    public bool setColoreSegnatura(string idAmm, int valore)
    {
        bool result = false;
        try
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            result = amm.setColoreSegnatura(idAmm, valore);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in Amministra Manager - metodo setColoreSegnatura " + e);
        }
        return result;
    }

    public static DocsPaVO.amministrazione.VisualizzaStatoDoc GetInfoDocument(string docnumber, string numProto, string anno, string idAmm, string codiceRegistro)
    {
        DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
        return amm.GetInfoDocument(docnumber, numProto, anno, idAmm, codiceRegistro);
    }

    public static Disservizio getDisservizio()
    {
        Disservizio disservizio = new Disservizio();
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                disservizio = amm.getDisservizio();
                transactionContext.Complete();
                return disservizio;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in AmministraManager.cs  - metodo: getDisservizio", e);
                return disservizio;
            }

        }
    }

    public static bool updateDisservizio(Disservizio disservizio)
    {
        bool result = true;
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                amm.updateDisservizio(disservizio);
                transactionContext.Complete();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in AmministraManager.cs  - metodo: updateDisservizio", e);
                result = false;
                return result;
            }
        }
    }

    public static bool creaDisservizio(Disservizio disservizio)
    {
        bool result = true;
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                amm.creaDisservizio(disservizio);
                transactionContext.Complete();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in AmministraManager.cs  - metodo: creaDisservizio", e);
                result = false;
                return result;
            }
        }
    }

    public static bool setStatoNotificaDisservizio(string systemId, string stato)
    {
        bool result = true;
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                amm.setNotificaDisservizio(systemId, stato);
                transactionContext.Complete();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in AmministraManager.cs  - metodo: setStatoNotificaDisservizio", e);
                result = false;
                return result;
            }
        }

    }

    public static bool cambiaStatoDisservizio(string stato, string system_id)
    {
        bool result = true;
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                amm.cambiaStatoDisservizio(stato, system_id);
                transactionContext.Complete();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in AmministraManager.cs  - metodo: cambiaStatoDisservizio", e);
                result = false;
                return result;
            }
        }
    }

    public static bool deleteDisservizio(string system_id)
    {
        bool result = true;
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                amm.deleteDisservizio(system_id);
                transactionContext.Complete();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in AmministraManager.cs  - metodo: deleteDisservizio", e);
                result = false;
                return result;
            }
        }

    }

    /// <summary>
    /// verifica se l'utente è:
    ///							-	System Admin
    ///							-	Super Admin
    ///							-	User Admin
    ///							-	NON è collegato al sistema
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="password"></param>
    /// <param name="sessionID"></param>
    /// <param name="datiAmministratore"></param>
    /// <returns>	0	=	tutto ok;
    ///				1	=	errore generico;
    ///				99	=	non riconosciuto;
    ///				100	=	utente già connesso
    ///				200 =	utente presente su più amministrazioni (tranne per il SYSTEM ADMIN [tipo = 1])
    ///	</returns>
    public static EsitoOperazione LoginAmministratoreProfilato(DocsPaVO.utente.UserLogin userLogin, bool forceLogin, out DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente)
    {
        EsitoOperazione result = new EsitoOperazione();
        infoUtente = null;

        // Creazione contesto transazionale
        using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
        {
            DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();

            DocsPaVO.utente.UserLogin.LoginResult loginResult;

            // Connessione dell'utente nel documentale corrente
            if (!userManager.LoginAdminUser(userLogin, forceLogin, out infoUtente, out loginResult))
            {
                if (loginResult == DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER)
                {
                    // Utente amministratore non riconosciuto
                    result.Codice = 99;
                    result.Descrizione = string.Format("Utente {0} non riconosciuto", userLogin.UserName);
                }
                else if (loginResult == DocsPaVO.utente.UserLogin.LoginResult.USER_ALREADY_LOGGED_IN)
                {
                    // Utente amministratore già collegato
                    result.Codice = 100;
                    result.Descrizione = string.Format("Utente {0} già collegato", userLogin.UserName);
                }
                else if (loginResult == DocsPaVO.utente.UserLogin.LoginResult.DTCM_SERVICE_NO_CONTACT)
                {
                    // Utente amministratore già collegato
                    result.Codice = 100;
                    result.Descrizione = string.Format("Errore, Servizi del Documentale DTCM non sono raggiungibili.", userLogin.UserName);
                }
                else
                {
                    result.Codice = 101;
                    result.Descrizione = string.Format("Errore nella connessione dell'utente {0}", userLogin.UserName);
                }
            }
            else
            {
                // Completamento della transazione
                transactionContext.Complete();
            }
        }

        return result;
    }

    public static DocsPaVO.amministrazione.InfoAmministrazione AmmGetInfoAmmCorrente(string idAmm)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            DocsPaVO.amministrazione.InfoAmministrazione infoAmm = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                infoAmm = amm.AmmGetInfoAmmCorrente(idAmm);

                // Completamento transazione
                transactionContext.Complete();
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore AmmGetInfoAmmCorrente : ", ex.Message));
            }
            return infoAmm;
        }
    }

    public static DocsPaVO.amministrazione.InfoAmministrazione GetInfoAmmAppartenenzaUtente(string userid, string pwd)
    {
        InfoAmministrazione amm = null;

        DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_APPARTENENZA_UTENTE");
        queryDef.setParam("param1", userid);

        string commandText = queryDef.getSQL();

        using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
        {
            using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
            {
                while (reader.Read())
                {
                    amm = new InfoAmministrazione();

                    amm.IDAmm = reader.GetValue(reader.GetOrdinal("ID")).ToString();
                    amm.Codice = reader.GetValue(reader.GetOrdinal("CODICE")).ToString();
                    amm.Descrizione = reader.GetValue(reader.GetOrdinal("DESCR")).ToString();
                }
            }
        }
        return amm;
    }

    public static string findRecords(string idAmm)
    {
        string result = "";

        try
        {
            logger.Debug("Metodo \"findRecords\" classe \"AmministraManager\" : inizio ricerca del limite dei record per l'archiviazione nello storico dell'amministrazione " + idAmm);
            //return File.Exists(serverPath);
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_NUM_RECORD");
            queryDef.setParam("param1", idAmm);
            string commandText = queryDef.getSQL();

            //string commandText = "SELECT NUM_RECORD, CHA_ARCHIVIAZIONE_LOG FROM DPA_AMMINISTRA WHERE SYSTEM_ID = " + idAmm;
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        result += reader.GetValue(reader.GetOrdinal("CHA_ARCHIVIAZIONE_LOG")).ToString() + "^" + reader.GetValue(reader.GetOrdinal("NUM_RECORD")).ToString();
                    }
                }
            }

        }
        catch (Exception ex)
        {
            logger.Debug("Metodo \"findRecord\" classe \"AmministraManager\" ERRORE : " + ex.Message);
            result = null;
        }
        return result;
    }

    public static string getSeRegistroObbl(string idAmm, string nome)
    {
        string retValue = string.Empty;
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                retValue = amm.getSeRegistroObbl(idAmm, nome);
                transactionContext.Complete();
            }
            catch (Exception e)
            {
                logger.Debug("Errore nel metodo getApplicationName");
            }
        }
        return retValue;
    }

    public static string getNews(string idAmm)
    {
        DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
        string result = amm.getNews(idAmm);
        return result;
    }

    public static bool setNews(string idAmm, string news, bool enable_news)
    {
        DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
        bool result = amm.setNews(idAmm, news, enable_news);
        return result;
    }

    public static string getIdAmmCorrGlobali(string idAmm)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                string result = amm.getIdAmmCorrGlobali(idAmm);
                transactionContext.Complete();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in AmministraManager.cs  - metodo: getIdAmmCorrGlobali", e);
                return null;
            }
        }
    }

    public static string GetCodAmmById(string idAmm)
    {
        string codiceAmm = string.Empty;
        try
        {
            using (DocsPaDB.Query_Utils.Utils dbUtils = new DocsPaDB.Query_Utils.Utils())
                codiceAmm = dbUtils.getCodAmm(idAmm);
        }
        catch (Exception ex)
        {
            logger.Error("Errore in GetCodAmm " + ex.Message);
            codiceAmm = string.Empty;
        }
        return codiceAmm;
    }

    public static bool SbloccoCasella(string email, string idRegistro)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.SbloccoCasella(email, idRegistro);
        }
        catch (Exception e)
        {
            logger.Error("Errore in SbloccoCasella " + e.Message);
            return false;
        }
    }

    public static List<DocsPaVO.amministrazione.DispositivoStampaEtichetta> GetDispositiviStampaEtichetta()
    {
        using (DocsPaDB.Query_DocsPAWS.Amministrazione amministrazioneDb = new DocsPaDB.Query_DocsPAWS.Amministrazione())
        {
            return amministrazioneDb.GetDispositiviStampaEtichetta();
        }
    }

    public static ArrayList GetAmministrazioniByUser(string userId, bool controllo)
    {
        DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
        ArrayList result = amm.GetDatiAmministrazioneByUser(userId, controllo);
        return result;
    }

    public static string GetPasswordUtenteMultiAmm(string userId)
    {
        DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
        string result = amm.GetPasswordUtenteMultiAmm(userId);
        return result;
    }

    public static bool SetPasswordUtenteMultiAmm(string userId, string password)
    {
        DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
        bool result = amm.SetPasswordUtenteMultiAmm(userId, password);
        return result;
    }

    public static bool SetUtenteVerticale(string userId, string idAmm, bool isVerticale)
    {
        DocsPaDB.Query_DocsPAWS.Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();
        bool result = utente.SetUtenteVerticale(userId, idAmm, isVerticale);
        return result;
    }

    public static bool ModificaPasswordUtenteMultiAmm(string userId, string idAmm)
    {
        DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
        bool result = amm.ModificaPasswordUtenteMultiAmm(userId, idAmm);
        return result;
    }

    public static bool IsUtenteVerticale(string idPeople)
    {
        DocsPaDB.Query_DocsPAWS.Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();
        bool result = utente.IsUtenteVerticale(idPeople);
        return result;
    }

    public static ValidationResultInfo ChangeAdminPassword(DocsPaVO.utente.UserLogin user, string oldPassword)
    {
        DocsPaVO.Validations.ValidationResultInfo result = null;

        if (AdminPasswordConfig.IsSupportedPasswordConfig())
        {
            // Se è abilitata la gestione configurazioni password,
            // la password viene modificata seguendo i criteri di validazione impostati
            result = BusinessLogic.Utenti.Login.ChangePassword(user, oldPassword);
        }
        else
        {
            result = new ValidationResultInfo();

            DocsPaDB.Query_DocsPAWS.Amministrazione dmAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            if (!dmAmm.ChangeAdminPwd(user.UserName, user.Password))
                result.BrokenRules.Add(new BrokenRule("CHANGE_PASSWORD_ERROR", string.Format("Errore nella modifica della password per l'amministratore: utente '{0}' non riconosciuto", user.UserName), BrokenRule.BrokenRuleLevelEnum.Error));

            result.Value = (result.BrokenRules.Count == 0);
        }

        return result;
    }

    public static bool setStatoAccettazioneDisservizio(string stato)
    {
        bool result = true;
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                amm.setStatoAccettazioneDisservizio(stato);
                transactionContext.Complete();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in AmministraManager.cs  - metodo: setStatoAccettazioneDisservizio", e);
                result = false;
                return result;
            }
        }
    }

    /// <summary>
    /// Restituisce il nome dell'applicazione dalla tabella del DB
    /// </summary>
    /// <returns></returns>
    public static string getApplicationName()
    {
        string retValue = string.Empty;
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                retValue = amm.getApplicationName();
                transactionContext.Complete();
            }
            catch (Exception e)
            {
                logger.Debug("Errore nel metodo getApplicationName");
            }
        }
        return retValue;
    }

    public static ArrayList GetAmministrazioni()
    {
        DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
        ArrayList result = amm.GetDatiAmministrazione();
        return result;
    }

    public static InfoAmministrazione[] AmmGetListAmministrazioni()
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            DocsPaVO.amministrazione.InfoAmministrazione[] infoAmm = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                infoAmm = amm.AmmGetListAmministrazioni();

                // Completamento transazione
                transactionContext.Complete();
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore AmmGetListAmministrazioni : ", ex.Message));
            }
            return infoAmm;
        }
    }

    public static List<DocsPaVO.documento.FascicolazioneTipiDocumento> GetFascicolazioneTipiDocumento(string idAmm, DocsPaVO.utente.InfoUtente infoutente)
    {
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        return doc.GetFascicolazioneTipiDocumento(idAmm, infoutente);
    }

    public static string findFontColor(string idAmm)
    {
        string result = "";

        try
        {
            logger.Debug("Metodo \"findFontColor\" classe \"AmministraManager\" : inizio verifica esistenza colore del testo per l'amministrazione " + idAmm);
            //return File.Exists(serverPath);
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_FONT_COLOR");
            queryDef.setParam("param1", idAmm);
            string commandText = queryDef.getSQL();

            //string commandText = "SELECT FONTCOLOR FROM DPA_AMMINISTRA WHERE SYSTEM_ID = " + idAmm;
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        result = reader.GetValue(reader.GetOrdinal("FONTCOLOR")).ToString();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.Debug("Metodo \"findFontColor\" classe \"AmministraManager\" ERRORE : " + ex.Message);
            result = "";
        }
        return result;
    }

    /// <summary>
    /// Logout dell'utente amministratore
    /// </summary>
    /// <param name="adminUser"></param>
    /// <returns></returns>
    public bool LogoutAmministratore(DocsPaVO.amministrazione.InfoUtenteAmministratore adminUser)
    {
        bool retValue = false;

        // Creazione contesto transazionale
        using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
        {
            DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();

            retValue = userManager.LogoutUser(adminUser.dst);

            if (retValue)
                transactionContext.Complete();
        }

        return retValue;
    }

    public static EsitoOperazione UpdateAmministrazione(InfoAmministrazione infoAmministrazione, InfoUtenteAmministratore infoUtente)
    {
        EsitoOperazione retValue = null;

        // Creazione contesto transazionale
        using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
        {
            DocsPaDocumentale.Interfaces.IAmministrazioneManager ammMng = new DocsPaDocumentale.Documentale.AmministrazioneManager(infoUtente);

            retValue = ammMng.Update(infoAmministrazione);

            if (retValue.Codice == 0)
                transactionContext.Complete();
        }

        return retValue;
    }

    public static ArrayList GetDatiAmministrazioneByUserAdministrator(string userId, bool controllo)
    {
        DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
        ArrayList result = amm.GetDatiAmministrazioneByUserAdministrator(userId, controllo);
        return result;
    }


    public static InfoUtenteAmministratore GetDatiAmministratore(string userId, string idAmm)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.GetDatiAmministratore(userId, idAmm);
        }
        catch (Exception e)
        {
            logger.Error("Errore nel reperimento dei dati amministrazione");
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="infoAmministrazione"></param>
    /// <param name="infoUtente"></param>
    /// <returns></returns>
    public static EsitoOperazione InsertAmministrazione(InfoAmministrazione infoAmministrazione, InfoUtenteAmministratore infoUtente)
    {
        EsitoOperazione retValue = null;

        // Creazione contesto transazionale
        using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
        {
            DocsPaDocumentale.Interfaces.IAmministrazioneManager ammMng = new DocsPaDocumentale.Documentale.AmministrazioneManager(infoUtente);

            retValue = ammMng.Insert(infoAmministrazione);

            if (retValue.Codice == 0)
            {
                if (FormatiDocumento.Configurations.SupportedFileTypesEnabled)
                {
                    // Inserimento dei formati documento predefiniti
                    ValidationResultInfo validationResult = FormatiDocumento.SupportedFormatsManager.InitializeDefaultFileTypes(Convert.ToInt32(infoAmministrazione.IDAmm));

                    if (!validationResult.Value)
                    {
                        retValue.Codice = -1;
                        retValue.Descrizione = ((BrokenRule)validationResult.BrokenRules[0]).Description;
                    }
                }

                transactionContext.Complete();
            }
        }

        return retValue;
    }

    public static bool SetFascicolazioneTipiDocumento(List<DocsPaVO.documento.FascicolazioneTipiDocumento> listTipiDoc, string idAmm, DocsPaVO.utente.InfoUtente infoutente)
    {
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        return doc.SetFascicolazioneTipiDocumento(listTipiDoc, idAmm, infoutente);
    }

    public static EsitoOperazione DeleteAmministrazione(InfoAmministrazione infoAmministrazione, InfoUtenteAmministratore infoUtente)
    {
        EsitoOperazione retValue = null;

        // Creazione contesto transazionale
        using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
        {
            DocsPaDocumentale.Interfaces.IAmministrazioneManager ammMng = new DocsPaDocumentale.Documentale.AmministrazioneManager(infoUtente);

            retValue = ammMng.Delete(infoAmministrazione);

            if (retValue.Codice == 0)
            {
                if (FormatiDocumento.Configurations.SupportedFileTypesEnabled)
                {
                    // Rimozione dei formati documento predefiniti
                    ValidationResultInfo validationResult = FormatiDocumento.SupportedFormatsManager.ClearFileTypes(Convert.ToInt32(infoAmministrazione.IDAmm));

                    if (!validationResult.Value)
                    {
                        retValue.Codice = -1;
                        retValue.Descrizione = ((BrokenRule)validationResult.BrokenRules[0]).Description;
                    }
                }

                transactionContext.Complete();
            }
        }

        return retValue;
    }
    public static bool ExistsTrasmissioniPendentiConWorkflowUtente(string idRuoloInUO, string idPeople)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.ExistsTrasmissioniPendentiConWorkflowUtente(idRuoloInUO, idPeople);
        }
        catch (Exception e)
        {
            logger.Error("Errore in ExistsTrasmissioniPendentiConWorkflowUtente " + e.Message);
            return false;
        }
    }

    public static DocsPaVO.documento.FileDocumento GetReportTrasmissioniPendentiUtente(string idPeople, string idCorrGlobalRuolo, string formato, ISpreadsheetService spreadsheetService, IFileConverterFactory fileConverterPDF, IReportGeneratorService reportGeneratorService)
    {
        DocsPaVO.documento.FileDocumento report = new DocsPaVO.documento.FileDocumento();
        try
        {

            // filtri report
            List<DocsPaVO.filtri.FiltroRicerca> filters = new List<DocsPaVO.filtri.FiltroRicerca>();
            filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idPeople", valore = idPeople });
            filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idCorrGlobali", valore = idCorrGlobalRuolo });

            // request generazione report
            DocsPaVO.Report.PrintReportRequest request = new DocsPaVO.Report.PrintReportRequest();
            request.SearchFilters = filters;

            // selezione formato report
            switch (formato)
            {
                case "XLS":
                    request.ReportType = DocsPaVO.Report.ReportTypeEnum.Excel;
                    break;

                case "ODS":
                    request.ReportType = DocsPaVO.Report.ReportTypeEnum.ODS;
                    break;
            }

            string descrizione = string.Empty;

            string descrizioneRuolo = BusinessLogic.Utenti.UserManager.GetRoleDescriptionByIdCorrGlobali(idCorrGlobalRuolo);
            string descrizioneUtente = BusinessLogic.Utenti.UserManager.getUtente(idPeople).descrizione;

            request.ContextName = "ExportTrasmissioniPendenti";
            request.ReportKey = "ExportTrasmissioniPendenti";
            request.Title = string.Empty;

            if (!string.IsNullOrEmpty(descrizioneRuolo) && !string.IsNullOrEmpty(descrizioneUtente))
                request.SubTitle = string.Format("Utente: {0} - Ruolo: {1}", descrizioneUtente, descrizioneRuolo);
            else if (!string.IsNullOrEmpty(descrizioneRuolo))
                request.SubTitle = string.Format("Ruolo: {0}", descrizioneRuolo);
            else if (!string.IsNullOrEmpty(descrizioneUtente))
                request.SubTitle = string.Format("Utente: {0}", descrizioneUtente);

            request.AdditionalInformation = string.Empty;

            report = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request, spreadsheetService, fileConverterPDF, reportGeneratorService).Document;
        }
        catch (Exception ex)
        {
            logger.Debug(ex.Message);
        }

        return report;
    }

}
