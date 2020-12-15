using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using DocsPaLdapServices.Core;
using DocsPaVO.utente;
using DocsPaVO.amministrazione;
using DocsPaVO.Ldap;
using log4net;

namespace BusinessLogic.Ldap
{
    /// <summary>
    /// Classe per la gestione della sincronizzazione delle utenze in LDAP con le utenze docspa
    /// </summary>
    public sealed class LdapUserSyncronizationServices
    {
        private static ILog logger = LogManager.GetLogger(typeof(LdapUserSyncronizationServices));
        /// <summary>
        /// 
        /// </summary>
        private LdapUserSyncronizationServices()
        { }

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static LdapSyncronizationHistoryItem[] GetLdapSyncHistory(InfoUtente infoUtente, string idAmministrazione)
        {
            try
            {
                // Verifica che il servizio di integrazione con ldap è attivo o meno
                DocsPaLdapServices.LdapConfigurations.CheckForLdapIntegrationActivated();

                DocsPaDB.Query_DocsPAWS.Ldap ldapDbServices = new DocsPaDB.Query_DocsPAWS.Ldap();

                return ldapDbServices.GetLdapSyncHistory(idAmministrazione);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);

                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idHistoryItem"></param>
        public static void DeleteLdapSyncHistoryItem(InfoUtente infoUtente, int idHistoryItem)
        {
            try
            {
                // Verifica che il servizio di integrazione con ldap è attivo o meno
                DocsPaLdapServices.LdapConfigurations.CheckForLdapIntegrationActivated();

                DocsPaDB.Query_DocsPAWS.Ldap ldapDbServices = new DocsPaDB.Query_DocsPAWS.Ldap();

                ldapDbServices.DeleteLdapSyncHistoryItem(idHistoryItem);

                // Rimozione file metadati
                string filePath = GetSyncResponseFilePath(idHistoryItem);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);

                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idHistoryItem"></param>
        /// <returns></returns>
        public static LdapSyncronizationResponse GetLdapSyncResponse(InfoUtente infoUtente, int idHistoryItem)
        {
            try
            {
                // Verifica che il servizio di integrazione con ldap è attivo o meno
                DocsPaLdapServices.LdapConfigurations.CheckForLdapIntegrationActivated();

                return RestoreResponseHistory(idHistoryItem);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);

                throw ex;
            }
        }

        /// <summary>
        /// Task di sincronizzazione degli utenti in docspa con gli utenti in ldap per l'amministrazione corrente
        /// </summary>
        /// <param name="request"></param>
        public static LdapSyncronizationResponse SyncronizeLdapUsers(LdapSyncronizationRequest request)
        {
            LdapSyncronizationResponse response = new LdapSyncronizationResponse();
            response.User = request.InfoUtente.userId;

            DocsPaDB.Query_DocsPAWS.Ldap ldapDbServices = new DocsPaDB.Query_DocsPAWS.Ldap();

            try
            {
                // Reperimento amministrazione corrente
                InfoAmministrazione admin = GetAdmin(request.IdAmministrazione);

                if (admin == null)
                    throw new ApplicationException(string.Format("Amministrazione con id '{0}' non trovata", request.IdAmministrazione));
                else
                    response.AdminCode = admin.Codice;

                //if (string.IsNullOrEmpty(admin.Dominio))
                //    throw new ApplicationException("Dominio predefinito non specificato per l'amministrazione");

                // Verifica che il servizio di integrazione con ldap è attivo o meno
                DocsPaLdapServices.LdapConfigurations.CheckForLdapIntegrationActivated();

                LdapConfig ldapInfo = ldapDbServices.GetLdapConfig(request.IdAmministrazione);

                if (ldapInfo != null)
                {
                    // Reperimento password per l'utente di dominio
                    ldapInfo.DomainUserPassword = ldapDbServices.GetLdapConfigDomainUserPassword(request.IdAmministrazione);

                    // Reperimento utenti in amministrazione
                    Dictionary<string, OrgUtente> docspaUsers = GetUsers();

                    // Reperimento utenti in ldap
                    Dictionary<string, LdapUser> ldapUsers = GetLdapUsers(ldapInfo);

                    List<LdapSyncronizationResponseItem> responseItems = new List<LdapSyncronizationResponseItem>();

                    // Rimozione utenti in docspa non più presenti in ldap
                    RemoveUsers(request.InfoUtente, request.IdAmministrazione, docspaUsers, ldapUsers, responseItems);

                    // Inserimento o aggiornamento degli utenti in docspa da ldap
                    AddOrUpdateUsers(request.InfoUtente, request.IdAmministrazione, admin.Dominio, docspaUsers, ldapUsers, responseItems);

                    // Calcolo del numero di elementi sincronizzati
                    foreach (LdapSyncronizationResponseItem item in responseItems)
                        if (item.Result != LdapSyncronizationResultEnum.Error)
                            response.ItemsSyncronized++;

                    response.Items = responseItems.ToArray();
                }
            }
            catch (Exception ex)
            {
                response.ErrorDetails = ex.Message;

                logger.Error(response.ErrorDetails);
            }
            finally
            {
                LdapSyncronizationHistoryItem historyItem = ldapDbServices.SaveLdapSyncResult(request.IdAmministrazione, response);

                // Persistenza istanza LdapSyncronizationHistoryItem    
                PersistResponseHistory(response, historyItem.Id);
            }

            return response;
        }

        /// <summary>
        /// Task di sincronizzazione degli utenti in docspa con gli utenti in ldap per l'amministrazione richiesta
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="adminCode"></param>
        /// <returns></returns>
        public static LdapSyncronizationResponse SyncronizeLdapUsers(string userId, string password, string adminCode)
        {
            LdapSyncronizationResponse response = null;

            DocsPaVO.utente.UserLogin userLogin = new DocsPaVO.utente.UserLogin();
            userLogin.UserName = userId;
            userLogin.Password = password;

            DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente;
            DocsPaVO.amministrazione.EsitoOperazione result = BusinessLogic.Amministrazione.AmministraManager.LoginAmministratoreProfilato(userLogin, true, out infoUtente);

            if (result.Codice == 0)
            {
                string idAdmin = string.Empty;
                using (DocsPaDB.Query_DocsPAWS.Amministrazione adminDbServices = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                    idAdmin = adminDbServices.GetIDAmm(adminCode);

                if (string.IsNullOrEmpty(idAdmin))
                {
                    response = new LdapSyncronizationResponse();
                    response.User = userId;
                    response.ErrorDetails = string.Format("Nessuna amministrazione identificata con il codice '{0}'", adminCode);
                }
                else
                {
                    LdapSyncronizationRequest request = new LdapSyncronizationRequest
                    {
                        InfoUtente = infoUtente,
                        IdAmministrazione = idAdmin
                    };

                    response = SyncronizeLdapUsers(request);
                }
            }
            else
            {
                response = new LdapSyncronizationResponse(userId, adminCode);
                response.ErrorDetails = result.Descrizione;
            }

            return response;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Verifica che la userid dell'utente in ldap non sia già presente in altre amministrazioni docspa
        /// </summary>
        /// <param name="docspaUsers"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static bool ExistUserDocsPa(Dictionary<string, OrgUtente> docspaUsers, string idAmministrazione, string userId)
        {
            foreach (KeyValuePair<string, OrgUtente> pair in docspaUsers)
                if (pair.Value.IDAmministrazione != idAmministrazione &&
                    pair.Value.UserId.ToUpper() == userId.ToUpper())
                    return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="dominio"></param>
        /// <param name="docspaUsers"></param>
        /// <param name="ldapUsers"></param>
        /// <param name="responseItems"></param>
        private static void AddOrUpdateUsers(InfoUtente infoUtente, string idAmministrazione, string dominio, Dictionary<string, OrgUtente> docspaUsers, Dictionary<string, LdapUser> ldapUsers, List<LdapSyncronizationResponseItem> responseItems)
        {

            logger.Debug("inserimento nuovi utenti o update già presenti se ricevuti da LDAP metodo AddOrUpdateUsers  ");
            foreach (KeyValuePair<string, LdapUser> pair in ldapUsers)
            {
                string idSync = pair.Key;

                string validationMessage;

                // Validazione utente ldap per l'aggiornamento
                if (ValidateLdapUser(pair.Value, out validationMessage))
                {
                    // Verifica presenza utente in altre amministrazioni docspa 
                    if (ExistUserDocsPa(docspaUsers, idAmministrazione, pair.Value.UserID))
                    {
                        // L'utente in ldap è già presente in docspa ma in un'amministrazione 
                        // diversa da quella corrente, pertanto non può essere né inserito né modificato
                        // qualora abbia già effettuato delle attività
                        responseItems.Add(CreateResponseItem(pair.Value.UserID, LdapSyncronizationResultEnum.Error, "Utente già presente in un'altra amministrazione"));
                    }
                    else
                    {
                        if (docspaUsers.ContainsKey(idSync))
                        {
                            // Se l'utente è già presente in docspa ma le informazioni relative 
                            // all’utente sono cambiate rispetto all'ultima sincronizzazione i campi 
                            // in docspa saranno aggiornati con quelli estratti da ldap
                            OrgUtente orgUser = docspaUsers[idSync];

                            if (orgUser.Amministratore == "0" && orgUser.SincronizzaLdap)
                            {
                                // Sono esclusi dalla lista tutti gli utenti superamministratori e
                                // gli utenti per cui è abilitata la sincronizzazione ldap
                                if (UpdateUserIfDirty(orgUser, pair.Value))
                                {

                                    logger.Debug("update già presenti se ricevuti da LDAP metodo AddOrUpdateUsers userid:  " + orgUser.UserId);
                                    EsitoOperazione result = BusinessLogic.Amministrazione.OrganigrammaManager.AmmModUtente(infoUtente, orgUser);

                                    if (result.Codice == 0)
                                        responseItems.Add(CreateResponseItem(pair.Value.UserID, LdapSyncronizationResultEnum.Updated, string.Empty));
                                    else
                                        responseItems.Add(CreateResponseItem(pair.Value.UserID, LdapSyncronizationResultEnum.Error, result.Descrizione));
                                }
                            }
                        }
                        else
                        {
                            OrgUtente newUser = CreateUser(pair.Value, idAmministrazione, dominio);

                            // Se l'utente non è presente in docspa, sarà inserito come nuovo utente attivo.
                            logger.Debug("inserimento già presenti se ricevuti da LDAP metodo AddOrUpdateUsers userid:  " + newUser.UserId);
                            EsitoOperazione result = BusinessLogic.Amministrazione.OrganigrammaManager.AmmInsNuovoUtente(infoUtente, newUser);

                            if (result.Codice == 0)
                            {
                                if (string.IsNullOrEmpty(newUser.Dominio))
                                {
                                    // Autenticazione standard tramite password, che viene definita come amministratoro scaduta
                                    DocsPaVO.Validations.ValidationResultInfo passResult = DocsPaPwdServices.UserPasswordServices.SetPassword(newUser.UserId, newUser.Password, true, true);

                                    if (!passResult.Value)
                                    {
                                        if (passResult.BrokenRules.Count > 0)
                                        {
                                            DocsPaVO.Validations.BrokenRule br = (DocsPaVO.Validations.BrokenRule)passResult.BrokenRules[0];
                                            responseItems.Add(CreateResponseItem(pair.Value.UserID, LdapSyncronizationResultEnum.Error, br.Description));
                                        }
                                    }
                                }

                                responseItems.Add(CreateResponseItem(pair.Value.UserID, LdapSyncronizationResultEnum.Inserted, string.Empty));
                            }
                            else if (result.Codice == 1)
                                responseItems.Add(CreateResponseItem(pair.Value.UserID, LdapSyncronizationResultEnum.Error, "Utente già presente in un'altra amministrazione"));
                            else if (result.Codice == 2)
                                responseItems.Add(CreateResponseItem(pair.Value.UserID, LdapSyncronizationResultEnum.Error, "Codice rubrica già presente"));
                            else
                                responseItems.Add(CreateResponseItem(pair.Value.UserID, LdapSyncronizationResultEnum.Error, result.Descrizione));
                        }
                    }
                }
                else
                    responseItems.Add(CreateResponseItem(pair.Value.UserID, LdapSyncronizationResultEnum.Error, validationMessage));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="docspaUsers"></param>
        /// <param name="ldapUsers"></param>
        /// <param name="responseItems"></param>
        private static void RemoveUsers(InfoUtente infoUtente, string idAmministrazione, Dictionary<string, OrgUtente> docspaUsers, Dictionary<string, LdapUser> ldapUsers, List<LdapSyncronizationResponseItem> responseItems)
        {
            logger.Debug("REMOVEUSER DA PITRE SE NON + ATTIVI IN LDAP");
            foreach (KeyValuePair<string, OrgUtente> pair in docspaUsers)
            {
                string idSync = pair.Key;

                // Verifica se l'utente appartiene all'amministrazione corrente,
                // solo in tal modo può essere rimosso / disabilitato
                if (pair.Value.IDAmministrazione == idAmministrazione &&
                    pair.Value.Amministratore == "0" && pair.Value.SincronizzaLdap && !ldapUsers.ContainsKey(idSync))
                {
                    EsitoOperazione result = BusinessLogic.Amministrazione.OrganigrammaManager.AmmVerificaEliminazioneUtente(pair.Value);

                    if (result.Codice == 0)
                    {
                        // Rimozione fisica dell'utente

                        logger.Debug("REMOVEUSER DA PITRE SE NON + ATTIVI IN LDAP AmmEliminaUtente userid:  " + pair.Value.UserId);
                        result = BusinessLogic.Amministrazione.OrganigrammaManager.AmmEliminaUtente(infoUtente, pair.Value);

                        if (result.Codice == 0)
                            responseItems.Add(CreateResponseItem(pair.Value.UserId, LdapSyncronizationResultEnum.Deleted, string.Empty));
                        else
                            responseItems.Add(CreateResponseItem(pair.Value.UserId, LdapSyncronizationResultEnum.Error, result.Descrizione));
                    }
                    else
                    {
                        if (pair.Value.Abilitato == "1" || pair.Value.Abilitato == "N")
                        {
                            // L'utente non può essere rimosso in quanto ha effettuato almeno un'attività; pertanto verrà impostato come disabilitato
                            pair.Value.Abilitato = "0";

                            logger.Debug("disabilita DA PITRE SE NON + ATTIVI IN LDAP AmmEliminaUtente userid:  " + pair.Value.UserId);
                            result = BusinessLogic.Amministrazione.OrganigrammaManager.AmmModUtente(infoUtente, pair.Value);

                            if (result.Codice == 0)
                                responseItems.Add(CreateResponseItem(pair.Value.UserId, LdapSyncronizationResultEnum.Updated, "Utente disabilitato in quanto ha già effettuato delle attività"));
                            else
                                responseItems.Add(CreateResponseItem(pair.Value.UserId, LdapSyncronizationResultEnum.Error, result.Descrizione));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reperimento utenti in amministrazione
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, OrgUtente> GetUsers()
        {
            Dictionary<string, OrgUtente> result = new Dictionary<string, OrgUtente>();
            OrgUtente userError = null;
            try
            {


                foreach (OrgUtente user in BusinessLogic.Amministrazione.OrganigrammaManager.GetUtenti())
                {
                    userError = user;
                    logger.Debug("GET UTENTE PITRE utente userid " + user.UserId + " matricola " + user.Matricola + " DN " + " email  " + user.Email);
                    try
                    {
                        result.Add(user.IdSincronizzazioneLdap.ToUpper(), user);
                    }
                    catch (Exception e)
                    { }
                }
            }
            catch (Exception ex)
            {

                logger.Debug("errore per utente userid " + userError.UserId + " matricola " + userError.Matricola + " " + ex.StackTrace + " " + ex.Message);

            }

            return result;

        }

        /// <summary>
        /// Reperimento degli utenti in LDAP appartenenti al gruppo configurato in amministrazione
        /// </summary>
        /// <param name="ldapInfo"></param>
        /// <returns></returns>
        private static Dictionary<string, LdapUser> GetLdapUsers(LdapConfig ldapInfo)
        {
            Dictionary<string, LdapUser> result = new Dictionary<string, LdapUser>();
            LdapUser userError = null;
            try
            {
                // Creazione dei servizi per la gestione LDAP
                BaseLdapUserServices ldapUserServices = LdapUserServicesFactory.CreateInstance(ldapInfo);

                // Reperimento da LDAP di tutti gli utenti appartenti al gruppo configurato
                foreach (LdapUser user in ldapUserServices.GetUsers())
                {
                    userError = user;
                    logger.Debug("GET UTENTE LDAP utente userid " + user.UserID + " matricola " + user.Matricola + " DN " + user.DN + " email  " + user.Email);
                    try
                    {
                        result.Add(user.Key.ToUpper(), user);
                    }
                    catch (Exception ex)
                    { }
                }

            }
            catch (Exception ex)
            {

                logger.Debug("errore per utente userid " + userError.UserID + " matricola " + userError.Matricola + " " + ex.StackTrace + " " + ex.Message);

            }

            return result;
        }

        /// <summary>
        /// Creazione nuovo oggetto utente in amministrazione
        /// </summary>
        /// <param name="ldapUser"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="dominio"></param>
        /// <returns></returns>
        private static OrgUtente CreateUser(LdapUser ldapUser, string idAmministrazione, string dominio)
        {
            OrgUtente user = new OrgUtente();

            user.UserId = ldapUser.UserID;
            user.IDAmministrazione = idAmministrazione;
            user.Dominio = dominio;
            if (string.IsNullOrEmpty(user.Dominio))
                user.Password = "password";
            user.Email = ldapUser.Email;
            user.Codice = user.UserId;
            user.CodiceRubrica = user.Codice;
            user.Nome = ldapUser.Nome;
            user.Cognome = ldapUser.Cognome;
            user.Sede = ldapUser.Sede;
            user.Abilitato = "1";
            user.Amministratore = "0";

            // Impostazione dell'id sincronizzazione in ldap
            user.IdSincronizzazioneLdap = ldapUser.Matricola;

            // Per default, tutti i nuovi utenti importati da un archivio LDAP 
            // potranno utilizzare le credenziali di dominio per connettersi al sitema
            user.AutenticatoInLdap = true;

            return user;
        }

        /// <summary>
        /// Aggiornamento dati utente già presente in amministrazione
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ldapUser"></param>
        private static bool UpdateUserIfDirty(OrgUtente user, LdapUser ldapUser)
        {
            bool dirty = (user.UserId != ldapUser.UserID ||
                    user.Email != ldapUser.Email ||
                    user.Nome != ldapUser.Nome ||
                    user.Cognome != ldapUser.Cognome ||
                    user.Sede != ldapUser.Sede);

            if (dirty)
            {
                user.UserId = ldapUser.UserID;


                user.Email = ldapUser.Email;
                user.Nome = ldapUser.Nome;
                user.Cognome = ldapUser.Cognome;
                user.Sede = ldapUser.Sede;
                user.Abilitato = "1";
                user.Amministratore = "0";
            }

            return dirty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="result"></param>
        /// <param name="details"></param>        
        /// <returns></returns>
        private static LdapSyncronizationResponseItem CreateResponseItem(string userId, LdapSyncronizationResultEnum result, string details)
        {
            LdapSyncronizationResponseItem responseItem = new LdapSyncronizationResponseItem();
            responseItem.UserId = userId;
            responseItem.Date = DateTime.Now;
            responseItem.Result = result;
            responseItem.Details = details;
            return responseItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="validationMessage"></param>
        private static bool ValidateLdapUser(LdapUser user, out string validationMessage)
        {
            validationMessage = string.Empty;

            if (string.IsNullOrEmpty(user.ErrorMessage))
                return true;
            else
            {
                validationMessage = user.ErrorMessage;

                return false;
            }

            //// Validazione campi obbligatori
            //if (!ValidateRequiredProperty(user.UserID, "UserID", out validationMessage))
            //    return false;

            //if (!ValidateRequiredProperty(user.Matricola, "Matricola", out validationMessage))
            //    return false;

            //if (!ValidateRequiredProperty(user.Email, "Email", out validationMessage))
            //    return false;

            //if (!ValidateRequiredProperty(user.Nome, "Nome", out validationMessage))
            //    return false;

            //if (!ValidateRequiredProperty(user.Nome, "Cognome", out validationMessage))
            //    return false;

            //return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="attributeName"></param>
        /// <param name="validationMessage"></param>
        /// <returns></returns>
        private static bool ValidateRequiredProperty(string value, string attributeName, out string validationMessage)
        {
            validationMessage = string.Empty;
            bool retValue = (!string.IsNullOrEmpty(value));

            if (!retValue)
                validationMessage = string.Format("Attributo '{0}' mancante in LDAP", attributeName);

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        private static InfoAmministrazione GetAdmin(string idAmministrazione)
        {
            return BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(idAmministrazione);
        }

        /// <summary>
        /// Persistenza su file dell'istanza LdapSyncronizationResponse
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="idSyncHistory"></param>
        private static void PersistResponseHistory(LdapSyncronizationResponse instance, int idHistoryItem)
        {
            Serialize(GetSyncResponseFilePath(idHistoryItem), instance);
        }

        /// <summary>
        /// Ripristino da file dell'istanza LdapSyncronizationResponse
        /// </summary>
        /// <param name="idHistoryItem"></param>
        /// <returns></returns>
        private static LdapSyncronizationResponse RestoreResponseHistory(int idHistoryItem)
        {
            string filePath = GetSyncResponseFilePath(idHistoryItem);

            if (File.Exists(filePath))
                return (LdapSyncronizationResponse)Deserialize(filePath);
            else
                return null;
        }

        /// <summary>
        /// Reperimento path file dell'istanza LdapSyncronizationResponse
        /// </summary>
        /// <param name="idHistoryItem"></param>
        /// <returns></returns>
        private static string GetSyncResponseFilePath(int idHistoryItem)
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "LdapSyncHistory");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return Path.Combine(folder, (idHistoryItem.ToString() + ".xml"));
        }

        /// <summary>
        /// Serializzazione oggetto
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="graph"></param>
        /// <returns></returns>
        private static void Serialize(string filePath, object graph)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, graph);
            }
        }

        /// <summary>
        /// Deserializzazione oggetto
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static object Deserialize(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream);
            }
        }


        #endregion
    }
}
