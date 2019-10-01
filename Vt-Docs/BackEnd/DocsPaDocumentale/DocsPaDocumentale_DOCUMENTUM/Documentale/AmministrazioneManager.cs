using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using DocsPaDocumentale_DOCUMENTUM.DctmServices;
using DocsPaDocumentale_DOCUMENTUM.DocsPaServices;
using DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.DataModel.Core.Profiles;
using Emc.Documentum.FS.DataModel.Core.Content;
using Emc.Documentum.FS.DataModel.Core.Query;
using CustomServices = DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom;
using log4net;

namespace DocsPaDocumentale_DOCUMENTUM.Documentale
{
    /// <summary>
    /// Gestione dell'amministrazione nel documentale
    /// </summary>
    public class AmministrazioneManager : IAmministrazioneManager
    {
        private ILog logger = LogManager.GetLogger(typeof(AmministrazioneManager));
        #region Ctors, constants, variables

        /// <summary>
        /// 
        /// </summary>
        private IObjectService _objectServiceInstance = null;

        /// <summary>
        /// 
        /// </summary>
        private InfoUtenteAmministratore _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public AmministrazioneManager(InfoUtenteAmministratore infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Inserimento di una nuova amministrazione nel documentale
        /// 
        /// Problema delle DFS:
        /// come da documentazione, inizialmente gli inserimenti venivano effettuati 
        /// tutti in un'unica operazione. Il problema è che le cartelle DNC e StampaRegistro
        /// venivano create nel folder HomeCabinet (oltre che normalmente nel cabinet dell'amministrazione).
        /// Ciò sicuramente è dovuto al fatto di inserire tutto in un'unica richiesta.
        /// Per ovviare a questo inconveniente, si è scelto di inserire le cartelle in 2 fasi:
        /// 1. inserimento cabinet 2. inserimento cartelle sottostanti
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public EsitoOperazione Insert(InfoAmministrazione info)
        {
            EsitoOperazione retValue = new EsitoOperazione();

            // ACL per l'amministrazione
            CustomServices.AclDefinition aclDefinition = null;

            // Identity del cabinet dell'amministrazione
            ObjectIdentity cabinetIdentity = null;

            // Identity del gruppo di sistema associato all'amministrazione
            ObjectIdentity groupIdentity = null;

            // Identity del gruppo di sistema che contiene gli amministratori dell'amministrazione
            ObjectIdentity adminGroupIdentity = null;

            try
            {
                // Creazione del cabinet per l'amministrazione
                ObjectPath cabinetPath = this.GetCabinetPath(info);
                logger.Debug("cabinetPath " + cabinetPath);
                cabinetIdentity = this.GetObjectServiceInstance().CreatePath(cabinetPath, DctmConfigurations.GetRepositoryName());
                
                // Creazione documenti
                ObjectIdentity documentiIdentity = new ObjectIdentity(DctmConfigurations.GetRepositoryName());
                DataObject dataObjectDocumenti = new DataObject(documentiIdentity, "dm_folder");
                dataObjectDocumenti.Properties.Set<string>("object_name", DocsPaAdminCabinet.FOLDER_DOCUMENTI);
                dataObjectDocumenti.Relationships.Add(new ReferenceRelationship(cabinetIdentity, Relationship.RELATIONSHIP_FOLDER, Relationship.ROLE_PARENT));

                // Creazione folder titolario
                ObjectIdentity titolarioIdentity = new ObjectIdentity(DctmConfigurations.GetRepositoryName());
                DataObject dataObjectTitolario = new DataObject(titolarioIdentity, "dm_folder");
                dataObjectTitolario.Properties.Set<string>("object_name", DocsPaAdminCabinet.FOLDER_TITOLARIO);
                dataObjectTitolario.Relationships.Add(new ReferenceRelationship(cabinetIdentity, Relationship.RELATIONSHIP_FOLDER, Relationship.ROLE_PARENT));

                // Creazione del gruppo di sistema associato all'amministrazione
                groupIdentity = new ObjectIdentity(DctmConfigurations.GetRepositoryName());
                DataObject groupData = new DataObject(groupIdentity, ObjectTypes.GRUPPO);
                groupData.Properties.Set<string>("group_name", TypeGruppo.GetGroupNameForAmministrazione(info.Codice));
                groupData.Properties.Set<string>("description", "Gruppo di sistema: tutti gli utenti dell'amministrazione " + info.Codice);
                groupData.Properties.Set<string>("group_class", "group");

                // Creazione del gruppo di sistema che contiene gli amministratori dell'amministrazione
              //adminGroupIdentity = new ObjectIdentity(DctmConfigurations.GetRepositoryName().ToLower());  //<- PERCHE' TO LOWER!? (Vecchio 6.0)
                adminGroupIdentity = new ObjectIdentity(DctmConfigurations.GetRepositoryName());
                DataObject adminGroupData = new DataObject(adminGroupIdentity, ObjectTypes.GRUPPO);
                adminGroupData.Properties.Set<string>("group_name", TypeGruppo.GetGroupNameForSysAdminAmministrazione(info.Codice));
                adminGroupData.Properties.Set<string>("description", "Gruppo di sistema: sysadmin dell'amministrazione " + info.Codice);
                adminGroupData.Properties.Set<string>("group_class", "group");

                DataPackage dataPackage = new DataPackage();
                dataPackage.AddDataObject(dataObjectDocumenti);
                dataPackage.AddDataObject(dataObjectTitolario);
                dataPackage.AddDataObject(groupData);
                dataPackage.AddDataObject(adminGroupData);

                dataPackage = this.GetObjectServiceInstance().Create(dataPackage, null);
                
                if (dataPackage.DataObjects.Count == 0)
                    throw new ApplicationException("Nessun oggetto creato");
                else
                {
                    dataObjectDocumenti = dataPackage.DataObjects[0];
                    dataObjectTitolario = dataPackage.DataObjects[1];
                    groupIdentity = dataPackage.DataObjects[2].Identity;
                    adminGroupIdentity = dataPackage.DataObjects[3].Identity;

                    // Creazione AclDefinition per tutti gli oggetti dell'amministrazione 
                    aclDefinition = this.CreateAclAdmin(info);

                    // Associazione dell'ACL appena creata con gli oggetti dell'amministrazione
                    AclHelper.setAclObjectProperties(dataObjectDocumenti.Properties, aclDefinition);
                    AclHelper.setAclObjectProperties(dataObjectTitolario.Properties, aclDefinition);

                    dataPackage = new DataPackage();
                    dataPackage.AddDataObject(dataObjectDocumenti);
                    dataPackage.AddDataObject(dataObjectTitolario);
                    dataPackage = this.GetObjectServiceInstance().Update(dataPackage, null);

                    logger.Debug(string.Format("Documentum.InsertAmministrazione: {0} oggetti creati per nuova amministrazione {1}, CabinetPath {2}", dataPackage.DataObjects.Count.ToString(), info.Codice, cabinetPath.Path));
                }
            }
            catch (Exception ex)
            {
                // In caso di errore, viene annullata la procedura
                // rimuovendo gli oggetti finora inseriti
                if (aclDefinition != null)
                {
                    try
                    {
                        // Rimozione ACL appena inserita
                        this.DeleteAclAdmin(aclDefinition);
                    }
                    catch (Exception exInner) 
                    {
                        logger.Debug(string.Format("Documentum.InsertAmministrazione: errore in rimozione acl '{0}'", aclDefinition.name));
                    }
                }

                // Inserimento nell'identityset degli oggetti da rimuovere
                ObjectIdentitySet createdObjects = new ObjectIdentitySet();

                if (cabinetIdentity != null && cabinetIdentity.ValueType == ObjectIdentityType.OBJECT_ID)
                    createdObjects.AddIdentity(cabinetIdentity);

                if (groupIdentity != null && groupIdentity.ValueType == ObjectIdentityType.OBJECT_ID)
                    createdObjects.AddIdentity(groupIdentity);

                if (adminGroupIdentity != null && adminGroupIdentity.ValueType == ObjectIdentityType.OBJECT_ID)
                    createdObjects.AddIdentity(adminGroupIdentity);

                if (createdObjects.Identities.Count > 0)
                {
                    OperationOptions opts = new OperationOptions();
                    opts.DeleteProfile = this.CreateDeleteProfile();

                    try
                    {
                        // Rimozione di tutti gli oggetti creati per l'amministrazione
                        this.GetObjectServiceInstance().Delete(createdObjects, opts);
                    }
                    catch (Exception exInner)
                    {
                        logger.Debug(string.Format("Documentum.InsertAmministrazione: errore in rimozione cabinet amministrazione '{0}'", info.Codice));
                    }
                }

                logger.Debug(string.Format("Errore in Documentum.InsertAmministrazione:\n{0}", ex.ToString()));

                retValue.Codice = -1;
                retValue.Descrizione = string.Format("Errore nella creazione del cabinet {0} in Documentum", info.Codice);             
            }

            return retValue;
        }

        /// <summary>
        /// Aggiornamento di un'amministrazione esistente nel documentale
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public EsitoOperazione Update(InfoAmministrazione info)
        {
            // NB: Per ora non implementata, in quanto non c'è l'esigenza di aggiornare i dati del cabinet
            throw new NotSupportedException("Operazione 'Update' non supportata dal documentale");
        }

        /// <summary>
        /// Cancellazione di un'amministrazione nel documentale
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public EsitoOperazione Delete(InfoAmministrazione info)
        {
            return this.Delete(info, DocsPaQueryHelper.getGroupNames(info.IDAmm));
        }

        /////// <summary>
        /////// Reperimento di tutti gli utenti dell'amministrazione
        /////// </summary>
        /////// <param name="infoUtente"></param>
        /////// <param name="amministrazione"></param>
        /////// <returns></returns>
        ////private static string[] GetUtenti(InfoAmministrazione amm)
        ////{
        ////    List<string> list = new List<string>();

        ////    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
        ////    {
        ////        string commandText = string.Format("SELECT user_id FROM PEOPLE WHERE id_amm = {0}", amm.IDAmm);

        ////        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
        ////        {
        ////            while (reader.Read())
        ////            {
        ////                list.Add(DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "user_id", false));
        ////            }
        ////        }
        ////    }

        ////    return list.ToArray();
        ////}

        /// <summary>
        /// Cancellazione di un'amministrazione nel documentale.
        /// NB: L'operazione di rimozione deve essere effettuata con le credenziali
        /// dell'utente superuser
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ruoliAmministrazione">
        /// Ruoli docspa da rimuovere in documentum
        /// </param>
        /// <returns></returns>
        public EsitoOperazione Delete(InfoAmministrazione info, string[] ruoliAmministrazione)
        {
            logger.Debug(string.Format("Documentum.DeleteAmministrazione: inizio rimozione amministrazione. Codice: '{0}'", info.Codice));

            EsitoOperazione retValue = new EsitoOperazione();

            try
            {
                ObjectIdentitySet identitySet = new ObjectIdentitySet();

                // Reperimento degli oggetti Identity relativi a tutti i gruppi e gli utenti dell'amministrazione
                IQueryService queryService = DctmServiceFactory.GetServiceInstance<IQueryService>(this.InfoUtente.dst);
                string[] users = Dfs4DocsPa.getUsersSystemGroup(info.Codice, queryService);

                // Cancellazione di tutti gli utenti legati al cabinet / amministrazione
                foreach (string userId in users)
                {
                    string normalizedUserId = TypeUtente.NormalizeUserName(userId);

                    // Reperimento oggetto identity per ogni utente
                    identitySet.AddIdentity(Dfs4DocsPa.getUserIdentityByName(normalizedUserId));

                    // Cancellazione dell'home cabinet per ogni 'utente
                    identitySet.AddIdentity(Dfs4DocsPa.getUserHomeFolderIdentity(normalizedUserId));

                    logger.Debug(string.Format("Documentum.DeleteAmministrazione: rimozione utente '{0}'", normalizedUserId));
                }

                //foreach (string userId in GetUtenti(info))
                //{
                //    string normalizedUserId = TypeUtente.NormalizeUserName(userId);

                //    ObjectIdentity id = Dfs4DocsPa.getUserIdentityByName(normalizedUserId, queryService);

                //    if (id != null)
                //    {
                //        identitySet.AddIdentity(id);

                //        // Cancellazione dell'home cabinet per ogni 'utente
                //        identitySet.AddIdentity(Dfs4DocsPa.getUserHomeFolderIdentity(normalizedUserId));
                //    }
                //}
                
                // Cancellazione del ruolo di sistema dell'amministrazione
                ObjectIdentity identity = Dfs4DocsPa.getSystemGroupIdentity(info.Codice, queryService);

                if (identity != null)
                    identitySet.AddIdentity(identity);

                // Cancellazione del ruolo di sistema degli amministratori dell'amministrazione
                identity = Dfs4DocsPa.getAmmSystemGroupIdentity(info.Codice, queryService);

                if (identity != null)
                    identitySet.AddIdentity(identity);
                
                // Cancellazione di tutti gli gruppi legati al cabinet / amministrazione
                foreach (string docspaGroupName in ruoliAmministrazione)
                {
                    string normalizedGroupName = TypeGruppo.NormalizeGroupName(docspaGroupName);

                    ObjectIdentity groupIdentity = Dfs4DocsPa.getGroupIdentityByName(normalizedGroupName, queryService);

                    if (groupIdentity != null)
                    {
                        identitySet.AddIdentity(groupIdentity);

                        logger.Debug(string.Format("Documentum.DeleteAmministrazione: rimozione ruolo '{0}'", normalizedGroupName));
                    }
                }
                
                // Reperimento oggetto Identity relativamente al cabinet da rimuovere
                identitySet.AddIdentity(Dfs4DocsPa.getCabinetIdentity(info.Codice));

                OperationOptions opts = new OperationOptions();
                opts.DeleteProfile = this.CreateDeleteProfile();

                IObjectService objectService = this.GetObjectServiceInstance();
                objectService.Delete(identitySet, opts);

                logger.Debug(string.Format("Documentum.DeleteAmministrazione: rimozione amministrazione. Codice: '{0}'", info.Codice));

                // Cancellazione di tutte le ACL create per gli oggetti dell'amministrazione
                this.DeleteAcl(info);
                logger.Debug(string.Format("Documentum.DeleteAmministrazione: rimozione ACL amministrazione. Codice: '{0}'", info.Codice));
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore in Documentum.DeleteAmministrazione:\n{0}", ex.ToString()));

                retValue.Codice = -1;
                retValue.Descrizione = string.Format("Errore nella rimozione del cabinet {0} in Documentum", info.Codice);
            }

            return retValue;
        }

        /// <summary>
        /// Verifica dell'esistenza dell'amministrazione
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public bool ContainsAmministrazione(string codiceAmministrazione)
        {
            return Dfs4DocsPa.containsAmministrazione(codiceAmministrazione);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Creazione oggetto "DeleteProfile" necessario per rimuovere il cabinet dell'amministrazione
        /// </summary>
        /// <returns></returns>
        protected DeleteProfile CreateDeleteProfile()
        {
            DeleteProfile profile = new DeleteProfile();
            profile.IsPopulateWithReferences = true;
            profile.IsDeepDeleteFolders = true;
            profile.IsDeepDeleteChildrenInFolders = true;
            profile.IsDeepDeleteVdmInFolders = true;
            profile.VersionStrategy = DeleteVersionStrategy.ALL_VERSIONS;
            profile.versionStrategySpecified = true;
            return profile;
        }

        /// <summary>
        /// Reperimento istanza IObjectService
        /// </summary>
        /// <returns></returns>
        protected IObjectService GetObjectServiceInstance()
        {
            if (this._objectServiceInstance == null)
                this._objectServiceInstance = DctmServiceFactory.GetServiceInstance<IObjectService>(this.InfoUtente.dst);
            return this._objectServiceInstance;
        }

        /// <summary>
        /// 
        /// </summary>
        protected InfoUtenteAmministratore InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

        /// <summary>
        /// Creazione ObjectPath relativamente al cabinet che rappresenta l'amministrazione di docspa in documentum
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected ObjectPath GetCabinetPath(InfoAmministrazione info)
        {
            return new ObjectPath(string.Format("{0}/", DocsPaAdminCabinet.getRootAmministrazione(info.Codice)));
        }

        /// <summary>
        /// Rimozione dell'ACL per gli oggetti visibili da tutta l'amministrazione
        /// </summary>
        /// <param name="aclAdmin"></param>
        protected virtual void DeleteAclAdmin(CustomServices.AclDefinition aclAdmin)
        {
            // Creazione servizio ACL (con credenziali da superamministratore)
            CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());
            aclService.Delete(aclAdmin);
        }

        /// <summary>
        /// Rimozione delle ACL per tutti gli oggetti dell'amministrazione
        /// </summary>
        /// <param name="info"></param>
        protected virtual void DeleteAcl(InfoAmministrazione info)
        {
            // Creazione servizio ACL (con credenziali da superamministratore)
            CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());

            // Rimozione di tutte le ACL di tutti gli oggetti dell'amministrazione
            // NB: Il parametro force + per ogni evenienza, ma sarebbe da evitarne l'uso: può creare inconsistenze nel DB di Documentum
            int affectedRows = aclService.DeleteAll(DctmConfigurations.GetRepositoryName(), AclHelper.getAclName(info.Codice), true);

            logger.Debug(string.Format("Rimozione ACL per tutti gli oggetti dell'amministrazione. Rimossi {0} oggetti", affectedRows.ToString()));
        }

        /// <summary>
        /// Creazione ACL per l'amministrazione
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected virtual CustomServices.AclDefinition CreateAclAdmin(InfoAmministrazione info)
        {
            CustomServices.AclDefinition aclData = null;
            
            try
            {
                // Reperimento ACL per gli oggetti visibili da tutta l'amministrazione
                aclData = Dfs4DocsPa.getAclDefinitionAmministrazione(info.Codice);
                
                CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());
                CustomServices.ObjectIdentity aclIdentity = aclService.Create(aclData);

                if (aclIdentity == null)
                    throw new ApplicationException(string.Concat("Errore nella creazione dell'ACL per l'amministrazione con ID '{0}'", info.IDAmm));
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore in Documentum.CreateAclAdmin:\n{0}", ex.ToString());
                logger.Debug(errorMessage);

                throw new ApplicationException(errorMessage);
            }

            return aclData;
        }

        #endregion
    }
}