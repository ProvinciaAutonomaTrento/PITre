using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.Runtime.Context;
using Emc.Documentum.FS.DataModel.Core.Context;
using Emc.Documentum.FS.DataModel.Core.Profiles;
using Emc.Documentum.FS.DataModel.Core.Query;

using DocsPaDocumentale_DOCUMENTUM.DctmServices;
using DocsPaDocumentale_DOCUMENTUM.DocsPaServices;
using DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes;
using log4net;

namespace DocsPaDocumentale_DOCUMENTUM.Documentale
{
    /// <summary>
    /// Gestione dell'organigramma dell'amministrazione di DOCUMENTUM
    /// </summary>
    public class OrganigrammaManager : IOrganigrammaManager
    {
        private ILog logger = LogManager.GetLogger(typeof(OrganigrammaManager));
        /// <summary>
        /// String usata come preambolo per i messaggi di errore (user-level) di questa classe
        /// </summary>
        public const string ERR_HEADER = "Errore in Documentum: ";

        /// <summary>
        /// String usata come preambolo per i messaggi di debug di questa classe
        /// </summary>
        public const string DEBUG_HEADER = "DocsPaDocumentale_DOCUMENTUM: ";

        #region Ctors, constants, variables

        /// <summary>
        /// Istanza corrente objectservice
        /// </summary>
        private IObjectService _objectServiceInstance = null;

        /// <summary>
        /// Istanza corrente queryservice
        /// </summary>
        private IQueryService _queryServiceInstance = null;

        /// <summary>
        /// Credenziali utente
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public OrganigrammaManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Inserimento nuovo ruolo in amministrazione
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione InserisciRuolo(OrgRuolo ruolo, bool computeAtipicita)
        {
            EsitoOperazione ret = new EsitoOperazione();
            string logMsg;
            IObjectService objSrvc = null;
            String repositoryName = DctmConfigurations.GetRepositoryName();

            // test sui campi obbligatori
            if (string.IsNullOrEmpty(ruolo.Codice) ||
                string.IsNullOrEmpty(ruolo.Descrizione))
            {
                ret.Codice = -1;
                logMsg = ERR_HEADER + "InserisciRuolo: dati insufficienti";
                ret.Descrizione = logMsg;
                logger.Debug(logMsg);
                return ret;
            }

            // il campo Codice corrisponde a:
            //  (ETDOCS) DPA_CORR_GLOBALI.VAR_COD_RUBRICA varchar(128)
            //  (DCTM)   dm_group.group_name string(32)
            // se mi viene passato un codice di lunghezza > 32 lancio un'eccezione
            if (ruolo.Codice.Length > 32)
            {
                ret.Codice = -1;
                logMsg = ERR_HEADER + "InserisciRuolo: campo CODICE supera la lunghezza massima (32)";
                ret.Descrizione = logMsg;
                logger.Debug(logMsg);
                return ret;
            }

            try
            {
                objSrvc = this.GetObjectServiceInstance();

                ObjectIdentity userIdentity = new ObjectIdentity(repositoryName);
                DataObject groupData = new DataObject(userIdentity, ObjectTypes.GRUPPO);

                groupData.Properties.Properties.AddRange(Dfs4DocsPa.getGroupProperties(ruolo, false));

                DataPackage pkg = new DataPackage(groupData);
                objSrvc.Create(pkg, null);
                logger.Debug(DEBUG_HEADER + "InserisciRuolo completata con SUCCESSO");
                return ret;

            }
            catch (Exception ex)
            {
                String st = ex.ToString();
                logger.Debug(DEBUG_HEADER + "InserisciRuolo FALLITA, Exception=" + st);
                ret.Codice = -1;
                ret.Descrizione = ERR_HEADER + "InserisciRuolo";
                return ret;
            }
        }

        /// <summary>
        /// Modifica dei metadati di un ruolo
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione ModificaRuolo(OrgRuolo ruolo)
        {
            EsitoOperazione ret = new EsitoOperazione();
            string logMsg;
            IObjectService objSrvc = null;
            string repositoryName = DctmConfigurations.GetRepositoryName();

            // test sui campi obbligatori
            if (string.IsNullOrEmpty(ruolo.Codice) ||
                string.IsNullOrEmpty(ruolo.Descrizione))
            {
                ret.Codice = -1;
                logMsg = ERR_HEADER + "ModificaRuolo: dati insufficienti";
                ret.Descrizione = logMsg;
                logger.Debug(logMsg);
                return ret;
            }

            // il campo Codice corrisponde a:
            //  (ETDOCS) DPA_CORR_GLOBALI.VAR_COD_RUBRICA varchar(128)
            //  (DCTM)   dm_group.group_name string(32)
            // se mi viene passato un codice di lunghezza > 32 lancio un'eccezione
            if (ruolo.Codice.Length > 32)
            {
                ret.Codice = -1;
                logMsg = ERR_HEADER + "ModificaRuolo: campo CODICE supera la lunghezza massima (32)";
                ret.Descrizione = logMsg;
                logger.Debug(logMsg);
                return ret;
            }

            try
            {
                objSrvc = this.GetObjectServiceInstance();

                ObjectIdentity groupIdentity = Dfs4DocsPa.getGroupIdentityByName(TypeGruppo.GetGroupName(ruolo));

                // non è possibile cambiare il nome di un gruppo
                //List<string> filters = new List<string>();
                //filters.Add("group_name");
                //DataObject oldData = DfsHelper.getAllPropsAndFolders(objSrvc, groupIdentity, filters, false);
                //Property oldName = oldData.Properties.Get("group_name");
                //if (oldName == null)
                //{
                //    ret.Codice = -1;
                //    logMsg = ERR_HEADER + "ModificaRuolo: impossibile leggere il vecchio nome del gruppo";
                //    ret.Descrizione = logMsg;
                //    logger.Debug(logMsg);
                //    return ret;
                //}
                //if (!oldName.GetValueAsString().Equals(ruolo.Codice, StringComparison.OrdinalIgnoreCase))
                //{
                //    ret.Codice = -1;
                //    logMsg = ERR_HEADER + "ModificaRuolo: non è possibile modificare il nome del gruppo";
                //    ret.Descrizione = logMsg;
                //    logger.Debug(logMsg);
                //    return ret;
                //}                    

                DataObject dataObject = new DataObject(groupIdentity, ObjectTypes.GRUPPO);
                dataObject.Properties.Properties.AddRange(Dfs4DocsPa.getGroupProperties(ruolo, true));

                DataPackage dataPackage = new DataPackage(dataObject);
                dataPackage = objSrvc.Update(dataPackage, null);

                if (dataPackage.DataObjects.Count > 0)
                {
                    ret.Codice = 0;
                    ret.Descrizione = string.Empty;

                    logger.Debug(DEBUG_HEADER + "ModificaRuolo completata con SUCCESSO");

                    return ret;
                }
                else
                    throw new ApplicationException();
            }
            catch (Exception ex)
            {
                string st = ex.ToString();
                logger.Debug(DEBUG_HEADER + "ModificaRuolo FALLITA, Exception=" + st);
                ret.Codice = -1;
                ret.Descrizione = ERR_HEADER + "ModificaRuolo";
                return ret;
            }
        }

        public EsitoOperazione OnlyDisabledRole(OrgRuolo ruolo)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            esito.Codice = 0;
            return esito;
        }

        /// <summary>
        /// Cancellazione di un ruolo in amministrazione
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaRuolo(OrgRuolo ruolo)
        {
            EsitoOperazione ret = new EsitoOperazione();
            string logMsg;
            IObjectService objSrvc = null;
            String repositoryName = DctmConfigurations.GetRepositoryName();

            // test sui campi obbligatori
            if (string.IsNullOrEmpty(ruolo.Codice))
            {
                logMsg = ERR_HEADER + "EliminaRuolo: dati insufficienti";
                ret.Codice = -1;
                ret.Descrizione = logMsg;
                logger.Debug(logMsg);
                return ret;
            }

            try
            {
                objSrvc = this.GetObjectServiceInstance();
                
                ObjectIdentity groupIdentity = Dfs4DocsPa.getGroupIdentityByName(TypeGruppo.GetGroupName(ruolo));

                checkReference(TypeGruppo.GetGroupName(ruolo), ObjectTypes.UTENTE, "user_group_name", false);

                ObjectIdentitySet s = new ObjectIdentitySet(groupIdentity);
                objSrvc.Delete(s, null);

                logger.Debug(DEBUG_HEADER + "EliminaRuolo completata con SUCCESSO");
                return ret;
            }
            catch (Exception ex)
            {
                String st = ex.ToString();
                logger.Debug(DEBUG_HEADER + "EliminaRuolo FALLITA, Exception=" + st);
                ret.Codice = -1;
                ret.Descrizione = ERR_HEADER + "EliminaRuolo";
                return ret;
            }
        }

        ///// <summary>
        ///// Reperimento dipendenze per l'oggetto richiesto
        ///// </summary>
        ///// <param name="fieldValue"></param>
        ///// <param name="tableName"></param>
        ///// <param name="fieldName"></param>
        ///// <param name="repeating"></param>
        ///// <returns></returns>
        //protected ObjectIdentity[] GetObjectDependencies(string fieldValue, string tableName, string fieldName, bool repeating)
        //{
        //    List<ObjectIdentity> list = new List<ObjectIdentity>();

        //    PassthroughQuery query = new PassthroughQuery();
        //    string dql = null;
        //    if (repeating)
        //        dql = "SELECT r_object_id FROM {0} WHERE ANY upper({1}) = upper('{2}')";
        //    else
        //        dql = "SELECT r_object_id FROM {0} WHERE upper({1}) = upper('{2}')";

        //    query.QueryString = string.Format(dql, tableName, fieldName,
        //                                        fieldValue.Replace("'", "''"));
            
        //    query.AddRepository(DctmConfigurations.GetRepositoryName());

        //    QueryExecution queryEx = new QueryExecution();
        //    queryEx.CacheStrategyType = CacheStrategyType.NO_CACHE_STRATEGY;
        //    OperationOptions operationOptions = null;

        //    QueryResult queryResult = this.GetQueryServiceInstance().Execute(query, queryEx, operationOptions);

        //    if (queryResult.DataObjects.Count > 0)
        //    {
        //        ObjectId objId = (ObjectId)queryResult.DataObjects[0].Identity.Value;

        //        list.Add(DfsHelper.createObjectIdentityObjId(objId));
        //    }

        //    return list.ToArray();
        //}

        protected void checkReference(string fieldValue, string tableName, string fieldName, bool repeating)
        {
            IQueryService qrySrvc = null;
            string repositoryName = DctmConfigurations.GetRepositoryName();

            qrySrvc = this.GetQueryServiceInstance();

            PassthroughQuery query = new PassthroughQuery();
            string dql = null;
            if (repeating)
                dql = "SELECT r_object_id FROM {0} WHERE ANY lower({1}) = lower('{2}')";
            else
                dql = "SELECT r_object_id FROM {0} WHERE lower({1}) = lower('{2}')";

            query.QueryString = string.Format(dql, tableName, fieldName,
                                                fieldValue.Replace("'", "''"));
            query.AddRepository(repositoryName);
            QueryExecution queryEx = new QueryExecution();
            queryEx.CacheStrategyType = CacheStrategyType.NO_CACHE_STRATEGY;
            OperationOptions operationOptions = null;
            QueryResult queryResult = qrySrvc.Execute(query, queryEx, operationOptions);

            // il risultato dovrebbe essere vuoto
            if (queryResult.DataObjects.Count > 0)
            {
                ObjectId objId = (ObjectId) queryResult.DataObjects[0].Identity.Value;

                throw new Exception(string.Format("Non è possibile cancellare {0}, in quanto referenziato l'oggetto con id={1}", fieldValue, objId.Id));
            }
        }

        /// <summary>
        /// Spostamento di un ruolo in amministrazione
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        /// <remarks>
        /// In dctm, non esiste il concetto di ruolo contenuto in UO;
        /// pertanto verrà richiamato il metodo ModificaRuolo in quanto
        /// la funzione consente di modificare alcuni attributi, tra cui il codice.
        /// </remarks>
        public EsitoOperazione SpostaRuolo(OrgRuolo ruolo)
        {
            // Lo spostamento corrisponde al modifica
            return this.ModificaRuolo(ruolo);
            // Lo spostamento in documentum non è supportato
            //return new EsitoOperazione() { Codice = 0 };
        }

        /// <summary>
        /// Impostazione di un ruolo come predefinito
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione ImpostaRuoloPreferito(string idPeople, string idGruppo)
        {
            throw new NotSupportedException("ImpostaRuoloPreferito: operazione non supportata in Documentum");
        }

        /// <summary>
        /// Inserimento di un nuovo utente in amministrazione
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione InserisciUtente(OrgUtente utente)
        {   
            EsitoOperazione retValue = new EsitoOperazione();

            string logMsg = string.Empty;
            bool userCreated = false;
            bool pathCreated = false;

            // test sui campi obbligatori
            if (string.IsNullOrEmpty(utente.Codice) ||
                string.IsNullOrEmpty(utente.Email) ||
                string.IsNullOrEmpty(utente.IDPeople) ||
                string.IsNullOrEmpty(utente.UserId))
            {
                retValue.Codice = -1;
                retValue.Descrizione = ERR_HEADER + "InserisciUtente: dati insufficienti";
                logger.Debug(retValue.Descrizione);
            }
            else
            {
                try
                {
                    IObjectService objSrvc = this.GetObjectServiceInstance();
                    string repo = DctmConfigurations.GetRepositoryName();

                    string homePath = string.Concat("/", TypeUtente.getHomeFolderName(utente.UserId));
                    ObjectIdentity identityHomeCabinet = objSrvc.CreatePath(new ObjectPath(homePath), repo);
                    pathCreated = true;

                    ObjectIdentity userIdentity = new ObjectIdentity(repo);
                    DataObject userData = new DataObject(userIdentity, ObjectTypes.UTENTE);

                    // Reperimento properties utente
                    userData.Properties.Properties.AddRange(Dfs4DocsPa.getUserProperties(utente));
                    userData.Properties.Set<string>("default_folder", homePath);

                    DataPackage dataPackage = new DataPackage(userData);

                    dataPackage = objSrvc.Create(dataPackage, null);

                    if (dataPackage.DataObjects.Count > 0)
                    {
                        userCreated = true;

                        // Impotazione proprietà "is_private = 1", per fare in modo
                        // che il cabinet sia visibile solamente all'utente che l'ha creato
                        // e 'owner_name' con il nome dell'utente
                        DataObject homeCabinetData = new DataObject(identityHomeCabinet, "dm_cabinet");
                      //homeCabinetData.Properties.Set<int>("is_private", 1);  //Vecchio 6.0
                        homeCabinetData.Properties.Set<string>("is_private", "1");
                        homeCabinetData.Properties.Set<string>("owner_name", TypeUtente.getUserName(utente));
                        
                        dataPackage = new DataPackage(homeCabinetData);
                        dataPackage = objSrvc.Update(dataPackage, null);

                        if (dataPackage.DataObjects.Count > 0)
                        {
                            this.inserisciUtenteInAmministrazione(utente, userData);

                            retValue.Codice = 0;
                            retValue.Descrizione = string.Empty;
                            logger.Debug(DEBUG_HEADER + "InserisciUtente completata con SUCCESSO");
                        }
                        else
                            throw new ApplicationException();
                    }
                    else
                    {
                        undoCreateHomeFolder(utente);
                        pathCreated = false;
                        throw new ApplicationException();
                    }
                }
                catch (Exception ex)
                {
                    string errorMessage = string.Format("Errore in {0}: {1}", DEBUG_HEADER, ex.ToString());

                    logger.Debug(errorMessage);
                    retValue.Codice = -1;
                    retValue.Descrizione = ERR_HEADER + "InserisciUtente";
                    
                    if (pathCreated)
                        this.undoCreateHomeFolder(utente);

                    if (userCreated)
                        this.undoCreateUser(utente);
                }
            }

            return retValue;
        }

        private void undoCreateHomeFolder(OrgUtente utente)
        {
            try
            {
                IObjectService objSrvc = this.GetObjectServiceInstance();
                string homePath = "/" + TypeUtente.getHomeFolderName(utente.UserId);
                ObjectIdentity homeIdentity =
                    new ObjectIdentity(new ObjectPath(homePath),
                                        DctmConfigurations.GetRepositoryName());
                ObjectIdentitySet idSet = new ObjectIdentitySet(homeIdentity);
                objSrvc.Delete(idSet, null);
            }
            catch (Exception ex)
            {
                //se siamo arrivati qui, significa che siamo già in una condizione di errore
                //non è così grave: rimane un folder, che non pregiudica un eventuale nuovo tentativo dei definire l'utente
                //in ogni caso è inutile rilanciare l'eccezione: già il chiamante è in gestione di una ecc.
                logger.Debug(DEBUG_HEADER + "Impossibile annullare l'inserimento dell'home folder: " + ex.ToString());
            }
        }

        private  void inserisciUtenteInAmministrazione(OrgUtente utente, DataObject userDataObject)
        {
            string codiceAmm = DocsPaQueryHelper.getCodiceAmministrazione(utente.IDAmministrazione);
            string gruppoAmm = TypeGruppo.GetGroupNameForAmministrazione(codiceAmm);

            try
            {
                //inserimento del nuovo utente nel gruppo associato alla amministrazione
                IQueryService qrySrvc = this.GetQueryServiceInstance();
                DfsHelper.insertUserInGroup(TypeUtente.getUserName(utente),gruppoAmm, qrySrvc);

                // eventuale inserimento nel gruppo dei syadmin
                if (utente.Amministratore != null && utente.Amministratore != "0")
                {
                    gruppoAmm = TypeGruppo.GetGroupNameForSysAdminAmministrazione(codiceAmm);
                    DfsHelper.insertUserInGroup(TypeUtente.getUserName(utente), gruppoAmm, qrySrvc);
                }
            }
            catch (Exception e)
            {
                string msg = string.Format(
                                DEBUG_HEADER + ": errore in inserimento utente {0} nel gruppo {1}: {2}",
                                TypeUtente.getUserName(utente), gruppoAmm, e.ToString());
                logger.Debug(msg);
                undoCreateUser(utente);
                throw new ApplicationException(msg, e);
            }
}

        public void InserisciUtenteInAmm(OrgUtente utente)
        {
            string codiceAmm = DocsPaQueryHelper.getCodiceAmministrazione(utente.IDAmministrazione);
            string gruppoAmm = TypeGruppo.GetGroupNameForAmministrazione(codiceAmm);

            try
            {
                //inserimento del nuovo utente nel gruppo associato alla amministrazione
                IQueryService qrySrvc = this.GetQueryServiceInstance();
                DfsHelper.insertUserInGroup(TypeUtente.getUserName(utente), gruppoAmm, qrySrvc);
            }
            catch (Exception e)
            {
                string msg = string.Format(
                                DEBUG_HEADER + ": errore in inserimento utente {0} nel gruppo {1}: {2}",
                                TypeUtente.getUserName(utente), gruppoAmm, e.ToString());
                logger.Debug(msg);
                throw new ApplicationException(msg, e);
            }
        }


        private bool undoCreateUser(OrgUtente utente)
        {
            try
            {
                ObjectIdentity userIdentity = Dfs4DocsPa.getUserIdentityByName(TypeUtente.getUserName(utente));

                ObjectIdentitySet idSet = new ObjectIdentitySet(userIdentity);
                
                IObjectService objSrvc = this.GetObjectServiceInstance();
                objSrvc.Delete(idSet, null);

                return true;
            }
            catch (Exception ex)
            {
                //se siamo arrivati qui, significa che siamo già in una condizione di errore
                //se si verifica un'eccezione qui, siamo nei guai: non riusciamo a fare un clean
                //in ogni caso è inutile rilanciare l'eccezione: già il chiamante è in gestione di una ecc.
                logger.Debug(DEBUG_HEADER + "GRAVE: impossibile annullare l'inserimento utente");
                return false;
            }
        }

        /// <summary>
        /// Modifica dei dati di un utente in amministrazione
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione ModificaUtente(OrgUtente utente)
        {
            EsitoOperazione ret = new EsitoOperazione();
            string logMsg = string.Empty;
            IObjectService objSrvc = null;
            IQueryService qrySvc = null;
            string repositoryName = DctmConfigurations.GetRepositoryName();
            bool wasSysAdmin = false;
            bool isSysAdmin = false;
            string gruppoSysAdm;
            string codiceAmm;

            // test sui campi obbligatori
            if (string.IsNullOrEmpty(utente.Codice) ||
                string.IsNullOrEmpty(utente.Email) ||
                string.IsNullOrEmpty(utente.IDPeople) ||
                string.IsNullOrEmpty(utente.UserId))
            {
                logMsg = ERR_HEADER + "ModificaUtente: dati insufficienti";
                ret.Codice = -1;
                ret.Descrizione = logMsg;
                logger.Debug(logMsg);
                return ret;
            }

            try
            {
                objSrvc = this.GetObjectServiceInstance();
                qrySvc = this.GetQueryServiceInstance();

                ObjectIdentity userIdentity = Dfs4DocsPa.getUserIdentityByName(TypeUtente.getUserName(utente));
                
                // non è possibile cambiare il nome di un utente
                List<string> filters = new List<string>();
                filters.Add("user_name");
                DataObject oldData = DfsHelper.getAllPropsAndFolders(objSrvc, userIdentity, filters, false);
                Property oldName = oldData.Properties.Get("user_name");
                if (oldName == null)
                {
                    ret.Codice = -1;
                    logMsg = ERR_HEADER + "ModificaUtente: impossibile leggere il vecchio nome dell'utente";
                    ret.Descrizione = logMsg;
                    logger.Debug(logMsg);
                    return ret;
                }
                if (!oldName.GetValueAsString().Equals(utente.UserId, StringComparison.OrdinalIgnoreCase))
                {
                    ret.Codice = -1;
                    logMsg = ERR_HEADER + "ModificaUtente: non è possibile modificare il nome dell'utente";
                    ret.Descrizione = logMsg;
                    logger.Debug(logMsg);
                    return ret;
                }           
                
                // dobbiamo capire se prima dell'update era sysAdmin oppure no
                codiceAmm = DocsPaQueryHelper.getCodiceAmministrazione(utente.IDAmministrazione);
                gruppoSysAdm = TypeGruppo.GetGroupNameForSysAdminAmministrazione(codiceAmm);
                wasSysAdmin = DfsHelper.isUserMemberOf(TypeUtente.getUserName(utente), gruppoSysAdm, qrySvc);
                isSysAdmin = (utente.Amministratore != null && utente.Amministratore != "0");
                
                // verifico se sia stata modificata la password o meno
           //     if (!string.IsNullOrEmpty(utente.Password))
           //     {    

                // eliminato controllo su presenza password, gli altri dati dell'utente devono essere modificati
                // anche senza un cambio password (speriamo almeno..) Necessario per replicare stato attivazione rispetto a docspa.
                // il campo password non viene passato se non è valorizzato

                DataObject userData = new DataObject(userIdentity, ObjectTypes.UTENTE);

                // Reperimento properties utente
                userData.Properties.Properties.AddRange(Dfs4DocsPa.getUserProperties(utente));

                DataPackage pkg = new DataPackage(userData);
                pkg = objSrvc.Update(pkg, null);

                if (pkg.DataObjects.Count > 0)
                {

                    // eventuale inserimento nel gruppo dei syadmin
                    if (isSysAdmin && !wasSysAdmin)
                        DfsHelper.insertUserInGroup(TypeUtente.getUserName(utente), gruppoSysAdm, qrySvc);
                    else if (wasSysAdmin && !isSysAdmin)
                        DfsHelper.removeUserFromGroup(TypeUtente.getUserName(utente), gruppoSysAdm, qrySvc);

                    ret.Codice = 0;
                    ret.Descrizione = string.Empty;

                    logger.Debug(DEBUG_HEADER + "ModificaUtente completata con SUCCESSO");
                }
                else
                    throw new ApplicationException();
              /**  }
                else
                {                  
                    ret.Codice = 0;
                    ret.Descrizione = string.Empty;

                    logger.Debug(DEBUG_HEADER + "ModificaUtente completata con SUCCESSO");
                } */
                return ret;
            }
            catch (Exception ex)
            {
                String st = ex.ToString();
                logger.Error(DEBUG_HEADER + "ModificaUtente FALLITA, Exception=" + st);
                ret.Codice = -1;
                ret.Descrizione = ERR_HEADER + "ModificaUtente";
                return ret;
            }
        }

        /// <summary>
        /// Elimina un utente in amministrazione
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaUtente(OrgUtente utente)
        {
            EsitoOperazione ret = new EsitoOperazione();
            string logMsg;
            IObjectService objSrvc = null;
            String repositoryName = DctmConfigurations.GetRepositoryName();

            // test sui campi obbligatori
            if (string.IsNullOrEmpty(utente.UserId))
            {
                logMsg = ERR_HEADER + "EliminaUtente: dati insufficienti";
                ret.Codice = -1;
                ret.Descrizione = logMsg;
                logger.Debug(logMsg);
                return ret;
            }

            try
            {

                ObjectIdentitySet identitySet = new ObjectIdentitySet();

                objSrvc = this.GetObjectServiceInstance();

                ObjectIdentity userIdentity = Dfs4DocsPa.getUserIdentityByName(TypeUtente.getUserName(utente));
                
                // Cancellazione dell'home cabinet per l'utente
                identitySet.AddIdentity(Dfs4DocsPa.getUserHomeFolderIdentity(utente.UserId));
                
                identitySet.AddIdentity(userIdentity);

                OperationOptions opts = new OperationOptions();
                opts.DeleteProfile = new DeleteProfile();
                opts.DeleteProfile.IsPopulateWithReferences = true;
                
                objSrvc.Delete(identitySet, opts);

                logger.Debug(DEBUG_HEADER + "EliminaUtente completata con SUCCESSO");
                return ret;
            }
            catch (Exception ex)
            {
                String st = ex.ToString();
                logger.Debug(DEBUG_HEADER + "EliminaUtente FALLITA, Exception=" + st);
                ret.Codice = -1;
                ret.Descrizione = ERR_HEADER + "EliminaUtente";
                return ret;
            }
        }



        /// <summary>
        /// Elimina un utente in amministrazione 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaUtenteAmm(OrgUtente utente)
        {
            EsitoOperazione ret = new EsitoOperazione();
            string logMsg;
            IObjectService objSrvc = null;
            String repositoryName = DctmConfigurations.GetRepositoryName();

            // test sui campi obbligatori
            if (string.IsNullOrEmpty(utente.UserId))
            {
                logMsg = ERR_HEADER + "EliminaUtente: dati insufficienti";
                ret.Codice = -1;
                ret.Descrizione = logMsg;
                logger.Debug(logMsg);
                return ret;
            }

            try
            {
                // verifica se esistono più di un'occorrenza per utente
                if (CountGroupsByUser(utente.UserId) > 1)
                {
                    // rimuove l'utente dal gruppo di root 
                    EliminaUtenteDaRuoloAmm(utente.UserId, utente.IDAmministrazione);
                }
                else
                {
                    ObjectIdentitySet identitySet = new ObjectIdentitySet();
                    objSrvc = this.GetObjectServiceInstance();
                    ObjectIdentity userIdentity = Dfs4DocsPa.getUserIdentityByName(TypeUtente.getUserName(utente));
                    // Cancellazione dell'home cabinet per l'utente
                    identitySet.AddIdentity(Dfs4DocsPa.getUserHomeFolderIdentity(utente.UserId));
                    identitySet.AddIdentity(userIdentity);
                    OperationOptions opts = new OperationOptions();
                    opts.DeleteProfile = new DeleteProfile();
                    opts.DeleteProfile.IsPopulateWithReferences = true;
                    objSrvc.Delete(identitySet, opts);
                    logger.Debug(DEBUG_HEADER + "EliminaUtente completata con SUCCESSO");
                }
                return ret;
            }
            catch (Exception ex)
            {
                String st = ex.ToString();
                logger.Debug(DEBUG_HEADER + "EliminaUtente FALLITA, Exception=" + st);
                ret.Codice = -1;
                ret.Descrizione = ERR_HEADER + "EliminaUtente";
                return ret;
            }
        }




        /// <summary>
        /// Inserimento di un utente in un ruolo
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione InserisciUtenteInRuolo(string idPeople, string idGruppo)
        {
            EsitoOperazione ret = new EsitoOperazione();
            
            IQueryService qrySrvc = null;

            try
            {                
                qrySrvc = this.GetQueryServiceInstance();

                string codiceRuolo = DocsPaServices.DocsPaQueryHelper.getCodiceRuoloFromIdGroups(idGruppo);
                string codiceUtente = DocsPaServices.DocsPaQueryHelper.getCodiceUtente(idPeople);
                
                string query = string.Format("ALTER GROUP '{0}' ADD '{1}'",
                                                TypeGruppo.NormalizeGroupName(codiceRuolo),
                                                TypeUtente.NormalizeUserName(codiceUtente));

                QueryResult queryResult = DfsHelper.executePassThrough(qrySrvc, query);                

                logger.Debug(DEBUG_HEADER + "InserisciUtenteInRuolo completata con SUCCESSO");
                return ret;
            }
            catch (Exception ex)
            {
                String st = ex.ToString();
                logger.Debug(DEBUG_HEADER + "InserisciUtenteInRuolo FALLITA, Exception=" + st);
                ret.Codice = -1;
                ret.Descrizione = ERR_HEADER + "InserisciUtenteInRuolo";
                return ret;
            }
        }

        /// <summary>
        /// Elminiazione di un utente da un ruolo
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaUtenteDaRuolo(string idPeople, string idGruppo)
        {
            EsitoOperazione ret = new EsitoOperazione();
            IQueryService qrySrvc = null;
            try
            {
                qrySrvc = this.GetQueryServiceInstance();

                string codiceRuolo = DocsPaServices.DocsPaQueryHelper.getCodiceRuoloFromIdGroups(idGruppo);
                string codiceUtente = DocsPaServices.DocsPaQueryHelper.getCodiceUtente(idPeople);
                
                string query = string.Format("ALTER GROUP '{0}' DROP '{1}'",
                                                TypeGruppo.NormalizeGroupName(codiceRuolo),
                                                TypeUtente.NormalizeUserName(codiceUtente));

                QueryResult queryResult = DfsHelper.executePassThrough(qrySrvc, query);

                logger.Debug(DEBUG_HEADER + "EliminaUtenteDaRuolo completata con SUCCESSO");
                return ret;
            }
            catch (Exception ex)
            {
                String st = ex.ToString();
                logger.Debug(DEBUG_HEADER + "EliminaUtenteDaRuolo FALLITA, Exception=" + st);
                ret.Codice = -1;
                ret.Descrizione = ERR_HEADER + "EliminaUtenteDaRuolo";
                return ret;
            }
        }


        /// <summary>
        /// Eliminazione di un utente da gruppo di amministrazione
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaUtenteDaRuoloAmm(string userId, string idAmministrazione)
        {
            EsitoOperazione ret = new EsitoOperazione();
            IQueryService qrySrvc = null;
            try
            {
                qrySrvc = this.GetQueryServiceInstance();

                string codiceAmministrazione = string.Format("sys_{0}",DocsPaServices.DocsPaQueryHelper.getCodiceAmministrazione(idAmministrazione).ToLower());

                string query = string.Format("ALTER GROUP '{0}' DROP '{1}'",
                                                TypeGruppo.NormalizeGroupName(codiceAmministrazione).ToLower(),
                                                TypeUtente.NormalizeUserName(userId).ToLower());

                QueryResult queryResult = DfsHelper.executePassThrough(qrySrvc, query);

                logger.Debug(DEBUG_HEADER + "EliminaUtenteDaRuoloAmm completata con SUCCESSO");
                return ret;
            }
            catch (Exception ex)
            {
                String st = ex.ToString();
                logger.Debug(DEBUG_HEADER + "EliminaUtenteDaRuoloAmm FALLITA, Exception=" + st);
                ret.Codice = -1;
                ret.Descrizione = ERR_HEADER + "EliminaUtenteDaRuoloAmm";
                return ret;
            }
        }



        /// <summary>
        /// Verifica dell'esistenza dell'utente in DCTM
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool ContainsUser(string userId)
        {
            IQueryService queryServices = DctmServiceFactory.GetServiceInstance<IQueryService>(UserManager.ImpersonateSuperUser());

            return Dfs4DocsPa.containsUser(userId, queryServices);
        }


        /// <summary>
        /// Conta il numero di gruppi di appartenenza per un utente DCTM
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int CountGroupsByUser(string userId)
        {
            IQueryService queryServices = DctmServiceFactory.GetServiceInstance<IQueryService>(UserManager.ImpersonateSuperUser());

            return Dfs4DocsPa.countGroupsByUserId(userId, queryServices);
        }



        /// <summary>
        /// Verifica dell'esistenza del gruppo in DCTM
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public bool ContainsGroup(string groupName)
        {
            IQueryService queryServices = DctmServiceFactory.GetServiceInstance<IQueryService>(UserManager.ImpersonateSuperUser());

            return Dfs4DocsPa.containsGroup(groupName, queryServices);
        }

        /// <summary>
        /// Verifica se un utente è contenuto in un gruppo DCTM
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool ContainsGroupUser(string groupName, string userId)
        {
            IQueryService queryServices = DctmServiceFactory.GetServiceInstance<IQueryService>(UserManager.ImpersonateSuperUser());

            userId = DocsPaObjectTypes.TypeUtente.NormalizeUserName(userId);

            foreach (string user in Dfs4DocsPa.getUsersGroup(DocsPaObjectTypes.TypeGruppo.NormalizeGroupName(groupName), queryServices))
                if (user == userId)
                    return true;
            
            return false;
        }

        /// <summary>
        /// Verifica se un utente è contenuto in un gruppo DCTM
        /// </summary>
        /// <param name="idGroup"></param>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public bool ContainsGroupUserById(string idGroup, string idPeople)
        {
            return this.ContainsGroupUser(DocsPaQueryHelper.getCodiceRuoloFromIdGroups(idGroup), 
                                          DocsPaQueryHelper.getCodiceUtente(idPeople));
        }

        /// <summary>
        /// Copia visibilità da ruolo a ruolo
        /// </summary>
        /// <param name="idGroup"></param>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public EsitoOperazione CopyVisibility(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Security.CopyVisibility copyVisibility)
        {
            DocsPaDB.Query_DocsPAWS.Documentale documentaleDB = new DocsPaDB.Query_DocsPAWS.Documentale();
            EsitoOperazione esitoOperazione = new EsitoOperazione();

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    //L'allineamento della visibilità in DOCUMENTUM non viene fatto chiamando la "ClearAndGrant",
                    //ma i documenti / fascicoli per cui allineare la visibilità vengono inseriti in una tabella di appoggio "DPA_OBJECTS_SYNC_PENDING"
                    //Documenti
                    documentaleDB.insertInObjectsSyncPending(copyVisibility, DocsPaDB.Query_DocsPAWS.Documentale.objTypeCopyVisiblity.DOCUMENTI);

                    //Fascicoli
                    documentaleDB.insertInObjectsSyncPending(copyVisibility, DocsPaDB.Query_DocsPAWS.Documentale.objTypeCopyVisiblity.FASCICOLI);

                    /* -- CODICE COMMENTATO PERCHE' L'ALLINEAMENTO CON DOCUMENTUM NON VIENE FATTO COME DI SEGUITO CHIAMANDO LA "ClearAndGrant"
                     * -- MA I DOCUMENTI / FASCICOLI PER CUI SINCRONIZZARE LA VISIBILITA' VENGONO INSERITI IN UN TABELLA DI APPOGGIO "DPA_OBJECTS_SYNC_PENDING"
                    //Documenti
                    System.Data.DataSet dsDocumenti = documentaleDB.getIdDocOrFascCopyVis(copyVisibility, DocsPaDB.Query_DocsPAWS.Documentale.objTypeCopyVisiblity.DOCUMENTI);

                    logger.Debug("Inizio impostazione ACL documenti per copia visibilità - Documentum");
                    for (int i = 0; i < dsDocumenti.Tables[0].Rows.Count; i++)
                    {
                        string docNumber = dsDocumenti.Tables[0].Rows[i]["THING"].ToString();

                        DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom.AclDefinition aclData = null;
                        aclData = Dfs4DocsPa.getAclDefinition(docNumber, DocsPaObjectTypes.ObjectTypes.DOCUMENTO, this.InfoUtente);
                        DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom.IAclService>(UserManager.ImpersonateSuperUser());

                        //Usato per evitare il blocco dell'intero processo eventualmente un solo documento ha problemi nell'allineamento delle ACL
                        try
                        {
                            aclService.ClearAndGrant(aclData);
                        }
                        catch (Exception ex)
                        {
                            logger.Debug("Errore nell'allineamento delle ACL Documentum per il documento DOCNUMBER : " + docNumber + " - metodo: CopyVisibility - ", ex);
                        }
                    }
                    logger.Debug("Fine impostazione ACL documenti per copia visibilità - Documentum");

                    //Fascicoli
                    System.Data.DataSet dsFascicoli = documentaleDB.getIdDocOrFascCopyVis(copyVisibility, DocsPaDB.Query_DocsPAWS.Documentale.objTypeCopyVisiblity.FASCICOLI);

                    logger.Debug("Inizio impostazione ACL fascicoli per copia visibilità - Documentum");
                    for (int i = 0; i < dsFascicoli.Tables[0].Rows.Count; i++)
                    {
                        string idProject = dsFascicoli.Tables[0].Rows[i]["THING"].ToString();

                        DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom.AclDefinition aclData = null;
                        aclData = Dfs4DocsPa.getAclDefinition(idProject, DocsPaObjectTypes.ObjectTypes.FASCICOLO, this.InfoUtente);
                        DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom.IAclService>(UserManager.ImpersonateSuperUser());
                         
                        //Usato per evitare il blocco dell'intero processo eventualmente un solo fascicolo ha problemi nell'allineamento delle ACL
                        try
                        {
                            aclService.ClearAndGrant(aclData);
                        }
                        catch (Exception ex)
                        {
                            logger.Debug("Errore nell'allineamento delle ACL Documentum per il fascicolo IDPROJECT : " + idProject + "- metodo: CopyVisibility - ", ex);
                        }
                    }
                    logger.Debug("Fine impostazione ACL fascicoli per copia visibilità - Documentum");
                    */

                    transactionContext.Complete();
                    esitoOperazione.Codice = 0;
                    esitoOperazione.Descrizione = "Operazione avvenuta con successo.";                  
                    return esitoOperazione;
                }
                catch (Exception e)
                {
                    logger.Error("Errore in OrganigrammaManager  - metodo: CopyVisibility", e);
                    esitoOperazione.Codice = -1;
                    esitoOperazione.Descrizione = "Problemi durante la copia della visibilità DOCUMENTUM. Consultare il file di log.";
                    return esitoOperazione;
                }
            }            
        }

        /// <summary>
        /// Operazione non supportata da Documentum. Viene restituito lo stesso ruolo passato per parametro
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public OrgRuolo HistoricizeRole(OrgRuolo role)
        {
            return role;
        }

        /// <summary>
        /// Metodo per l'estensione di visibilità ai ruoli superiori di un ruolo
        /// </summary>
        /// <param name="idAmm">Id dell'amministrazione</param>
        /// <param name="idGroup">Id del gruppo di cui estendere la visibilità</param>
        /// <param name="extendScope">Scope di estensione</param>
        /// <param name="copyIdToTempTable">True se bisogna copiare gli id id dei documenti e fascicoli in una tabella tamporanea per l'allineamento asincrono della visibilità</param>
        /// <returns>Esito dell'operazione</returns>
        public EsitoOperazione ExtendVisibilityToHigherRoles(
            String idAmm,
            String idGroup,
            DocsPaVO.amministrazione.SaveChangesToRoleRequest.ExtendVisibilityOption extendScope)
        {
            EsitoOperazione retVal = new EsitoOperazione();

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                using (DocsPaDB.Query_DocsPAWS.Documentale doc = new DocsPaDB.Query_DocsPAWS.Documentale())
                {
                    bool result = doc.ExtendVisibilityToHigherRoles(idAmm, idGroup, extendScope, true);

                    retVal.Codice = result ? 0 : -1;
                    retVal.Descrizione = result ? String.Empty : "Si è verificato un errore durante l'estensione della visibilità di documenti e fascicoli ai superiori gerarchici";
                }

            }

            return retVal;
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
        /// 
        /// </summary>
        /// <returns></returns>
        protected IObjectService GetObjectServiceInstance()
        {
            if (this._objectServiceInstance == null)
                this._objectServiceInstance = DctmServiceFactory.GetServiceInstance<IObjectService>(UserManager.ImpersonateSuperUser());
            return this._objectServiceInstance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected IQueryService GetQueryServiceInstance()
        {
            if (this._queryServiceInstance == null)
                this._queryServiceInstance = DctmServiceFactory.GetServiceInstance<IQueryService>(UserManager.ImpersonateSuperUser());
            return this._queryServiceInstance;
        }
        
        #endregion



        public EsitoOperazione CalcolaAtipicita(OrgRuolo ruolo, string idTipoRuoloVecchio, string idVecchiaUo, bool calcolaSuiSottoposti)
        {
            throw new NotImplementedException();
        }
    }

}
