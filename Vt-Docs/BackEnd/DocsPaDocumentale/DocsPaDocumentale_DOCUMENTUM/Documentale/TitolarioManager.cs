using System;
using System.Collections.Generic;
using System.Text;
using DocsPaVO.amministrazione;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.utente;
using DocsPaDocumentale_DOCUMENTUM.DocsPaServices;
using DocsPaDocumentale_DOCUMENTUM.Documentale;
using DocsPaDocumentale_DOCUMENTUM.DctmServices;
using DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.DataModel.Core.Profiles;
using Emc.Documentum.FS.DataModel.Core.Content;
using CustomServices = DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom;
using log4net;

namespace DocsPaDocumentale_DOCUMENTUM.Documentale
{
    /// <summary>
    /// Servizi per la gestione della gerarchia dei nodi classificazione 
    /// (titolario) per Documentum
    /// </summary>
    public class TitolarioManager : ITitolarioManager
    {
        private ILog logger = LogManager.GetLogger(typeof(TitolarioManager));
        #region Ctros, variables, constants

        /// <summary>
        /// 
        /// </summary>
        private IObjectService _objectServiceInstance = null;

        /// <summary>
        /// 
        /// </summary>
        private IQueryService _queryServiceInstance = null;

        /// <summary>
        /// 
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public TitolarioManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Attivazione di un titolario
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public bool AttivaTitolario(OrgTitolario titolario)
        {   
            bool retValue = true;
            
            try
            {
                // Verifica della presenza di un titolario attivo
                string repositoryName = DctmConfigurations.GetRepositoryName();

                IObjectService objSrvc = this.GetObjectServiceInstance();
                
                // Verifica se in documentum è presente un titolario attivo
                bool existTitolarioAttivo = (Dfs4DocsPa.containsTitolarioAttivo(titolario.CodiceAmministrazione, this.GetQueryServiceInstance()));
                
                DataObject dataObjectTitolarioAttivo = null;
                
                if (existTitolarioAttivo)
                {
                    // Reperimento oggetto identity per il titolario attivo
                    ObjectIdentity identity = Dfs4DocsPa.getTitolarioAttivoIdentity(titolario.CodiceAmministrazione);

                    // Reperimento oggetto DataObject relativo al titolario attivo (se presente)
                    List<string> filters = new List<string>();
                    filters.Add(TypeTitolario.ID_DOCSPA);
                    dataObjectTitolarioAttivo = DfsHelper.getAllPropsAndFolders(objSrvc, identity, filters, false);
                    string idDocsPa = dataObjectTitolarioAttivo.Properties.Get(TypeTitolario.ID_DOCSPA).GetValueAsString();

                    // Reperimento del titolario appena chiuso in docspa, cui corrisponde il titolario da chiudere in documentum
                    OrgTitolario titolarioChiuso = DocsPaQueryHelper.getTitolario(idDocsPa);
                    
                    dataObjectTitolarioAttivo = new DataObject();
                    dataObjectTitolarioAttivo.Identity = Dfs4DocsPa.getTitolarioIdentity(titolarioChiuso.ID);
                    dataObjectTitolarioAttivo.Type = ObjectTypes.TITOLARIO;
                    dataObjectTitolarioAttivo.Properties = new PropertySet();
                    dataObjectTitolarioAttivo.Properties.Properties.AddRange(Dfs4DocsPa.getTitolarioProperties(titolarioChiuso));
                }

                // Reperimento identity titolario in definizione
                ObjectIdentity identityTitolarioInDef = Dfs4DocsPa.getTitolarioIdentity(titolario.ID);
                DataObject dataObjectTitolarioInDef = new DataObject();
                dataObjectTitolarioInDef.Identity = identityTitolarioInDef;
                dataObjectTitolarioInDef.Type = ObjectTypes.TITOLARIO;
                dataObjectTitolarioInDef.Properties = new PropertySet();
                dataObjectTitolarioInDef.Properties.Properties.AddRange(Dfs4DocsPa.getTitolarioProperties(titolario));

                List<DataObject> dataObjectList = new List<DataObject>();

                if (existTitolarioAttivo)
                    dataObjectList.Add(dataObjectTitolarioAttivo);

                dataObjectList.Add(dataObjectTitolarioInDef);

                // Creazione oggetto DataPackage
                DataPackage dataPackage = new DataPackage();
                dataPackage.DataObjects.AddRange(dataObjectList);

                dataPackage = objSrvc.Update(dataPackage, null);

                string objectId = ((ObjectId) dataPackage.DataObjects[0].Identity.Value).Id.ToString();

                retValue = (!string.IsNullOrEmpty(objectId));

                if (retValue)
                    logger.Debug(string.Format("Documentum.AttivaTitolario: attivato titolario con id {0}", titolario.ID));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in Documentum.AttivaTitolario: " + ex.ToString());
            }

            return retValue;
        }

        /// <summary>
        /// Inserimento o aggiornamento dei metadati generali relativi 
        /// all’intera struttura di classificazione dei documenti
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public bool SaveTitolario(OrgTitolario titolario)
        {
            bool retValue = false;
            
            try
            {
                //titolario.ID viene SEMPRE valorizzato da ETDOCS; se così non è --> ERRORE
                if (string.IsNullOrEmpty(titolario.ID))
                {
                    logger.Debug("Errore passaggio dati da ETDOCS: ID Titolario mancante");
                }
                else
                {
                    // Reperimento istanza objectservice
                    IObjectService objSrvc = GetObjectServiceInstance();

                    DataObject dataObject = new DataObject();
                    dataObject.Type = ObjectTypes.TITOLARIO;

                    // Reperimento oggetto identity per il titolario corrente
                    ObjectIdentity titolarioIdentity = Dfs4DocsPa.getTitolarioIdentity(titolario.ID);

                    IQueryService querySrvc = this.GetQueryServiceInstance();

                    // Verifica se il titolario è già esistente
                    bool insertMode = (!Dfs4DocsPa.containsTitolario(titolario.ID, querySrvc));
                    
                    if (insertMode)
                    {
                        // Creazione oggetto identity per nuovo inserimento
                        dataObject.Identity = new ObjectIdentity(DctmConfigurations.GetRepositoryName());

                        // Modalità di in inserimento, creazione oggetto Identity parent e oggetto relationship
                        ObjectIdentity parentIdentity = DfsHelper.createObjectIdentityByPath(DocsPaAdminCabinet.getRootTitolario(titolario.CodiceAmministrazione));

                        dataObject.Relationships = new List<Relationship>();
                        dataObject.Relationships.Add(DfsHelper.createParentFolderRelationship(parentIdentity));
                    }
                    else
                    {
                        // Reperimento oggetto identity per titolario da modificare
                        dataObject.Identity = titolarioIdentity;
                    }

                    // Impostazione proprietà titolario
                    dataObject.Properties = new PropertySet();
                    dataObject.Properties.Properties.AddRange(Dfs4DocsPa.getTitolarioProperties(titolario));

                    DataPackage dataPackage = new DataPackage(dataObject);
                    dataPackage.RepositoryName = DctmConfigurations.GetRepositoryName();

                    // Save oggetto titolario in documentum
                    if (insertMode)
                    {
                        // Reperimento ACL dell'amministrazione da associare al titolario
                        CustomServices.AclDefinition aclDefinition = Dfs4DocsPa.getAclDefinitionAmministrazione(titolario.CodiceAmministrazione);

                        // Impostazione delle properties relative all'acl per il nodo titolario
                        AclHelper.setAclObjectProperties(dataObject.Properties, aclDefinition);

                        dataPackage = objSrvc.Create(dataPackage, null);
                    }
                    else
                        dataPackage = objSrvc.Update(dataPackage, null);

                    retValue = (dataPackage.DataObjects.Count > 0);

                    if (retValue)
                        logger.Debug(string.Format("Documentum.SaveTitolario: salvato titolario con id {0}", titolario.ID));
                }
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in Documentum.SaveTitolario: " + ex.ToString());
            }

            return retValue;
        }

        /// <summary>
        /// Cancellazione dei metadati generali relativi 
        /// all’intera struttura di classificazione dei documenti
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public bool DeleteTitolario(OrgTitolario titolario)
        {
            bool retValue = false;

            try
            {
                IObjectService objSrvc = this.GetObjectServiceInstance();

                DeleteProfile deleteProfile = new DeleteProfile();
                deleteProfile.IsDeepDeleteFolders = true;
                deleteProfile.IsDeepDeleteChildrenInFolders = true;

                OperationOptions opts = new OperationOptions();
                opts.Profiles = new List<Profile>();
                opts.Profiles.Add(deleteProfile);

                ObjectIdentitySet identitySet = new ObjectIdentitySet();
                identitySet.Identities = new List<ObjectIdentity>();
                identitySet.Identities.Add(Dfs4DocsPa.getTitolarioIdentity(titolario.ID));

                objSrvc.Delete(identitySet, opts);

                // Rimozione delle ACL di tutti i fascicoli procedimentali contenuti nel titolario
                this.ClearAclFascicoliTitolario(titolario);

                retValue = true;

                logger.Debug(string.Format("Documentum.DeleteTitolario: rimosso titolario con id {0}", titolario.ID));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in Documentum.DeleteTitolario: " + ex.ToString());
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="refreshAclIfUpdate">
        /// Se true, indica di aggiornare le entries dell'ACL associata al nodo titolario
        /// </param>
        /// <returns></returns>
        public bool SaveNodoTitolario(OrgNodoTitolario nodoTitolario, bool refreshAclIfUpdate)
        {
            bool retValue = false;

            bool aclCreated = false;

            try
            {
                //titolario.ID  viene SEMPRE valorizzato da ETDOCS; se così non è --> ERRORE
                if (string.IsNullOrEmpty(nodoTitolario.ID))
                {
                    retValue = false;

                    logger.Debug("Errore passaggio dati da ETDOCS: ID nodo titolario mancante");
                }
                else
                {
                    IQueryService querySrvc = this.GetQueryServiceInstance();

                    DataObject dataObject = new DataObject();
                    dataObject.Type = ObjectTypes.NODO_TITOLARIO;

                    DataObject dataObjectFascicoloGenerale = null;

                    // Reperimento oggetto identity per il titolario corrente
                    ObjectIdentity nodoTitolarioIdentity = Dfs4DocsPa.getNodoTitolarioIdentity(nodoTitolario.ID);

                    // Verifica esistenza nodo titolario
                    bool insertMode = (!Dfs4DocsPa.containsNodoTitolario(nodoTitolario.ID, querySrvc));

                    IObjectService objSrvc = this.GetObjectServiceInstance();

                    if (insertMode)
                    {
                        // Modalità di inserimento
                        // Creazione oggetto identity per nuovo inserimento
                        dataObject.Identity = new ObjectIdentity(DctmConfigurations.GetRepositoryName());

                        // Determinazione dell'oggetto parent del nodo di titolario da inserire
                        ObjectIdentity parentIdentity = null;

                        if (nodoTitolario.Livello == "1")
                            // Se nodo di primo livello, deve essere legato alla struttura di titolario
                            parentIdentity = Dfs4DocsPa.getTitolarioIdentity(nodoTitolario.ID_Titolario);
                        else
                            // Se nodo figlio di un altro nodo, lo lega al padre
                            parentIdentity = Dfs4DocsPa.getNodoTitolarioIdentity(nodoTitolario.IDParentNodoTitolario);

                        dataObject.Relationships = new List<Relationship>();
                        dataObject.Relationships.Add(DfsHelper.createParentFolderRelationship(parentIdentity));

                        // Creazione fascicolo generale, figlio nodo del titolario da creare
                        dataObjectFascicoloGenerale = new DataObject();
                        dataObjectFascicoloGenerale.Type = ObjectTypes.FASCICOLO_GENERALE;
                        dataObjectFascicoloGenerale.Identity = new ObjectIdentity(DctmConfigurations.GetRepositoryName());

                        // Creazione oggetto relationship
                        dataObjectFascicoloGenerale.Relationships = new List<Relationship>();
                        dataObjectFascicoloGenerale.Relationships.Add(DfsHelper.createParentFolderRelationship(nodoTitolarioIdentity));

                        dataObjectFascicoloGenerale.Properties = new PropertySet();
                        dataObjectFascicoloGenerale.Properties.Properties.AddRange(Dfs4DocsPa.getFascicoloGeneraleProperties(nodoTitolario));
                    }
                    else
                    {
                        // Modalità di aggiornamento

                        // Reperimento oggetto identity per nodo titolario da modificare
                        dataObject.Identity = nodoTitolarioIdentity;
                    }

                    // Impostazione proprietà nodo titolario
                    dataObject.Properties = new PropertySet();
                    dataObject.Properties.Properties.AddRange(Dfs4DocsPa.getNodoTitolarioProperties(nodoTitolario));

                    List<DataObject> dataObjectList = new List<DataObject>();
                    dataObjectList.Add(dataObject);
                    if (dataObjectFascicoloGenerale != null)
                    {
                        // Inserimento DataObject relativo al fascicolo generale
                        dataObjectList.Add(dataObjectFascicoloGenerale);
                    }

                    // Creazione oggetto DataPackage
                    DataPackage dataPackage = new DataPackage();
                    dataPackage.DataObjects.AddRange(dataObjectList);
                    dataPackage.RepositoryName = DctmConfigurations.GetRepositoryName();

                    // Save oggetto nodo titolario in documentum
                    if (insertMode)
                    {
                        // Creazione ACL, comune sia per il nodo titolario che per il fascicolo generale in esso contenuto
                        CustomServices.AclDefinition aclDefinition = this.CreateAclNodoTitolario(nodoTitolario);

                        // ACL del titolario creata
                        aclCreated = true;

                        // Impostazione delle properties relative all'acl per il nodo titolario
                        AclHelper.setAclObjectProperties(dataObject.Properties, aclDefinition);

                        // Impostazione delle properties relative all'acl per il fascicolo generale
                        AclHelper.setAclObjectProperties(dataObjectFascicoloGenerale.Properties, aclDefinition);

                        dataPackage = objSrvc.Create(dataPackage, null);
                    }
                    else
                    {
                        dataPackage = objSrvc.Update(dataPackage, null);

                        if (refreshAclIfUpdate)
                        {
                            // Aggiornamento delle entries dell'acl associata al nodo
                            this.RefreshAclNodoTitolario(nodoTitolario);
                        }
                    }

                    retValue = (dataPackage.DataObjects.Count > 0);

                    if (retValue)
                        logger.Debug(string.Format("Documentum.SaveNodoTitolario: salvato nodo di titolario con id {0}", nodoTitolario.ID));
                }
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in Documentum.SaveNodoTitolario: " + ex.ToString());

                if (aclCreated)
                    // Se l'ACL è stata creata, viene rimossa
                    this.DeleteAclNodoTitolario(nodoTitolario);
            }

            return retValue;
        }

        /// <summary>
        /// Aggiornamento metadati del nodo di titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public bool SaveNodoTitolario(OrgNodoTitolario nodoTitolario)
        {
            return this.SaveNodoTitolario(nodoTitolario, false);
        }
        
        /// <summary>
        /// Eliminazione di un nodo di titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public bool DeleteNodoTitolario(OrgNodoTitolario nodoTitolario)
        {
            bool retValue = false;

            try
            {
                IObjectService objSrvc = this.GetObjectServiceInstance();

                DeleteProfile deleteProfile = new DeleteProfile();
                deleteProfile.IsDeepDeleteFolders = true;
                deleteProfile.IsDeepDeleteChildrenInFolders = true;

                OperationOptions opts = new OperationOptions();
                opts.Profiles = new List<Profile>();
                opts.Profiles.Add(deleteProfile);

                ObjectIdentitySet identitySet = new ObjectIdentitySet();
                identitySet.Identities = new List<ObjectIdentity>();
                identitySet.Identities.Add(Dfs4DocsPa.getNodoTitolarioIdentity(nodoTitolario.ID));

                objSrvc.Delete(identitySet, opts);

                // Rimozione ACL nodo titolario
                this.DeleteAclNodoTitolario(nodoTitolario);

                // Rimozione di tutte le ACL degli eventuali 
                // fascicoli procedimentali contenuti nel titolario
                this.ClearAclFascicoliNodoTitolario(nodoTitolario);
                
                retValue = true;

                logger.Debug(string.Format("Documentum.DeleteNodoTitolario: rimosso nodo di titolario con id {0}", nodoTitolario.ID));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in Documentum.DeleteNodoTitolario: " + ex.ToString());
            }

            return retValue;
        }

        /// <summary>
        /// Impostazione / rimozione della visibilità di un singolo nodo di titolario per un ruolo
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="ruoloTitolario">
        /// Ruolo cui deve essere associata / rimossa la visibilità verso il nodo titolario
        /// </param>
        /// <returns></returns>
        public bool SetAclRuoloNodoTitolario(OrgNodoTitolario nodoTitolario, OrgRuoloTitolario ruoloTitolario)
        {
            bool retValue = false;

            try
            {
                // Reperimento ACL associate al nodo di titolario SOLO per il ruolo richiesto
                CustomServices.AclDefinition aclData = AclHelper.getAclDefinition(nodoTitolario.CodiceAmministrazione, nodoTitolario.ID, ObjectTypes.NODO_TITOLARIO);

                List<CustomServices.AclEntry> entries = new List<CustomServices.AclEntry>(aclData.entries);
                //AclHelper.addBasicPermit(entries, TypeGruppo.NormalizeGroupName(ruoloTitolario.Codice), Permission.DELETE);
                AclHelper.addBasicPermit(entries, TypeGruppo.NormalizeGroupName(DocsPaQueryHelper.getCodiceRuoloFromIdGroups(ruoloTitolario.ID)), Permission.DELETE);
                aclData.entries = entries.ToArray();

                CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());

                if (ruoloTitolario.Associato)
                {
                    // Impostazione visibilità del ruolo 
                    aclService.UpdateWithGrant(aclData);
                }
                else
                {
                    // Rimozione visibilità  del ruolo 
                    aclService.Revoke(aclData);
                }

                retValue = true;
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in Documentum.SetAclRuoloNodoTitolario: " + ex.ToString());
            }

            return retValue;
        }

        /// <summary>
        /// Aggiornamento visibilità di più ruoli su un titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="ruoliTitolario"></param>
        /// <returns></returns>
        public DocsPaVO.amministrazione.EsitoOperazione[] SetAclNodoTitolario(OrgNodoTitolario nodoTitolario, OrgRuoloTitolario[] ruoliTitolario)
        {
            // Per documentum, viene riportato un'unico EsitoOperazione
            // contenente i dettagli relativi all'esito dell'intera operazione effettuata
            // (a differenza di etdocs, l'aggiornamento è effetuato in un'unica operazione)
            EsitoOperazione retValue = new EsitoOperazione();

            try
            {
                // Reperimento ACL del nodo titolario
                CustomServices.AclDefinition aclData = Dfs4DocsPa.getAclDefinition(nodoTitolario.ID, ObjectTypes.NODO_TITOLARIO, nodoTitolario.CodiceAmministrazione);

                CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());

                // Aggiornamento della visibilità dei ruoli sul nodo titolario
                aclService.ClearAndGrant(aclData);

                retValue.Codice = 0;
                retValue.Descrizione = string.Format("Aggiornamento dei ruoli nel nodo titolario '{0}' terminato correttamente", nodoTitolario.Codice);
            }
            catch (Exception ex)
            {
                retValue.Codice = -1;
                retValue.Descrizione = string.Format("Errore in Documentum nell'aggiornamento dei ruoli nel nodo titolario '{0}'", nodoTitolario.Codice);

                logger.Debug("Errore in Documentum.SetAclNodoTitolario: " + ex.ToString());
            }

            return new EsitoOperazione[1] { retValue };
        }

        /// <summary>
        /// Verifica dell'esistenza in dctm di un titolario
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <returns></returns>
        public bool ContainsTitolario(string idTitolario)
        {
            IQueryService queryService = DctmServiceFactory.GetServiceInstance<IQueryService>(UserManager.ImpersonateSuperUser());

            return Dfs4DocsPa.containsTitolario(idTitolario, queryService);
        }

        /// <summary>
        /// Verifica dell'esistenza di un titolario attivo in amministrazione
        /// </summary>
        /// <param name="codiceAmministrazione"></param>
        /// <returns></returns>
        public bool ContainsTitolarioAttivo(string codiceAmministrazione)
        {
            IQueryService queryService = DctmServiceFactory.GetServiceInstance<IQueryService>(UserManager.ImpersonateSuperUser());

            return Dfs4DocsPa.containsTitolarioAttivo(codiceAmministrazione, queryService);
        }

        /// <summary>
        /// Verifica dell'esistenza in dctm di un nodo di titolario
        /// </summary>
        /// <param name="idNodoTitolario"></param>
        /// <returns></returns>
        public bool ContainsNodoTitolario(string idNodoTitolario)
        {
            IQueryService queryService = DctmServiceFactory.GetServiceInstance<IQueryService>(UserManager.ImpersonateSuperUser());

            return Dfs4DocsPa.containsNodoTitolario(idNodoTitolario, queryService);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Creazione delle ACL per il nodo titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns>ACL creata, ma ancora non associata al nodo titolario</returns>
        protected virtual CustomServices.AclDefinition CreateAclNodoTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
        {
            CustomServices.AclDefinition aclData = null;

            try
            {
                // Reperimento ACL nodo titolario
                aclData = Dfs4DocsPa.getAclDefinition(nodoTitolario.ID, ObjectTypes.NODO_TITOLARIO, nodoTitolario.CodiceAmministrazione);
                
                CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());
                CustomServices.ObjectIdentity aclIdentity = aclService.Create(aclData);

                if (aclIdentity == null)
                    throw new ApplicationException(string.Concat("Errore nella creazione dell'ACL per il nodo titolario con ID '{0}'", nodoTitolario.ID));
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore in Documentum.CreateAclNodoTitolario:\n{0}", ex.ToString());
                logger.Debug(errorMessage);

                throw new ApplicationException(errorMessage);
            }

            return aclData;
        }

        /// <summary>
        /// Aggiornamento delle entries dell'acl associata al nodo di titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        protected virtual void RefreshAclNodoTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
        {
            try
            {
                // Reperimento ACL nodo titolario
                CustomServices.AclDefinition aclData = Dfs4DocsPa.getAclDefinition(nodoTitolario.ID, ObjectTypes.NODO_TITOLARIO, nodoTitolario.CodiceAmministrazione);

                CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());
                aclService.ClearAndGrant(aclData);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore in Documentum.CreateAclNodoTitolario:\n{0}", ex.ToString());
                logger.Debug(errorMessage);

                throw new ApplicationException(errorMessage);
            }
        }

        /// <summary>
        /// Rimozione ACL per il nodo di titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        protected virtual void DeleteAclNodoTitolario(OrgNodoTitolario nodoTitolario)
        {
            CustomServices.AclDefinition aclToRemove = Dfs4DocsPa.getAclDefinition(nodoTitolario.ID, ObjectTypes.NODO_TITOLARIO, nodoTitolario.CodiceAmministrazione);

            this.DeleteAcl(aclToRemove);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aclToRemove"></param>
        protected virtual void DeleteAcl(CustomServices.AclDefinition aclToRemove)
        {
            // Creazione servizio ACL
            CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());
            
            aclService.Delete(aclToRemove);
        }

        /// <summary>
        /// Rimozione di tutte le ACL dei fascicoli procedimentali contenuti nel titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        protected void ClearAclFascicoliNodoTitolario(OrgNodoTitolario nodoTitolario)
        {
            foreach (string idFascicolo in DocsPaQueryHelper.getFascicoliProcedimentaliNodoTitolario(nodoTitolario.ID))
            {
                // Reperimento e cancellazione ACL per il fascicolo procedimentale
                CustomServices.AclDefinition aclDefinition = Dfs4DocsPa.getAclDefinition(idFascicolo, ObjectTypes.FASCICOLO, this.InfoUtente);

                this.DeleteAcl(aclDefinition);
            }   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="titolario"></param>
        protected void ClearAclFascicoliTitolario(OrgTitolario titolario)
        {
            foreach (string idFascicolo in DocsPaQueryHelper.getFascicoliProcedimentaliTitolario(titolario.ID))
            {
                // Reperimento e cancellazione ACL per il fascicolo procedimentale
                CustomServices.AclDefinition aclDefinition = Dfs4DocsPa.getAclDefinition(idFascicolo, ObjectTypes.FASCICOLO, this.InfoUtente);

                this.DeleteAcl(aclDefinition);
            }   
        }

        /// <summary>
        /// 
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
        /// <returns></returns>
        protected IQueryService GetQueryServiceInstance()
        {
            if (this._queryServiceInstance == null)
                this._queryServiceInstance = DctmServiceFactory.GetServiceInstance<IQueryService>(this.InfoUtente.dst);
            return this._queryServiceInstance;
        }

        /// <summary>
        /// 
        /// </summary>
        protected InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
            set
            {
                this._infoUtente = value;
            }
        }

        #endregion
    }
}