using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DocsPaVO;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;
using DocsPaVO.utente;
using DocsPaDocumentale.Interfaces;
using DocsPaDocumentale_DOCUMENTUM.DctmServices;
using DocsPaDocumentale_DOCUMENTUM.DocsPaServices;
using DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.Runtime.Context;
using Emc.Documentum.FS.DataModel.Core.Context;
using Emc.Documentum.FS.DataModel.Core.Query;
using Emc.Documentum.FS.DataModel.Core.Profiles;
using CustomServices = DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom;
using log4net;

namespace DocsPaDocumentale_DOCUMENTUM.Documentale
{
   /// <summary>
   /// 
   /// </summary>
   public class ProjectManager : DocsPaDocumentale.Interfaces.IProjectManager
   {
       private ILog logger = LogManager.GetLogger(typeof(ProjectManager));
      #region Ctors, constants, variables

      /// <summary>
      /// Credenziali utente
      /// </summary>
      private InfoUtente _infoUtente = null;

      /// <summary>
      /// 
      /// </summary>
      protected ProjectManager()
      {
      }

      /// <summary>
      /// Inizializza l'istanza della classe acquisendo i dati relativi all'utente 
      /// ed alla libreria per la connessione al documentale.
      /// </summary>
      /// <param name="infoUtente">Dati relativi all'utente</param>
      /// <param name="currentLibrary">Libreria per la connessione al documentale</param>
      public ProjectManager(DocsPaVO.utente.InfoUtente infoUtente)
      {
         this._infoUtente = infoUtente;
      }

      #endregion

      #region Public methods

      /// <summary>
      /// Inserimento di un documento in un folder
      /// </summary>
      /// <param name="idProfile"></param>
      /// <param name="idFolder"></param>
      /// <returns></returns>
      public bool AddDocumentInFolder(string idProfile, string idFolder)
      {
          logger.Info("BEGIN");
          bool retValue = false;

          // Reperimento idfolder per il record di tipo "C" in docspa (root folder)
          idFolder = DocsPaQueryHelper.getIdFolderCType(idFolder);

          // Reperimento docNumber da idProfile
          string docNumber = DocsPaQueryHelper.getDocNumber(idProfile);

          //bool stampaRegistro = DocsPaQueryHelper.isStampaRegistro(docNumber);
          bool stampaRegistro = DocsPaQueryHelper.isStampaRegistroRepertorio(docNumber);

          try
          {
              IObjectService objectService = DctmServiceFactory.GetServiceInstance<IObjectService>(UserManager.ImpersonateSuperUser());

              // 1. recuperare il documento con tutti i link a folder attuali
              
              ObjectIdentity documentIdentity = Dfs4DocsPa.getDocumentoIdentityByDocNumber(docNumber);

             if (stampaRegistro)
                  documentIdentity = Dfs4DocsPa.getDocumentoStampaRegistroIdentityByDocNumber(docNumber);

              // 2. Creare il nuovo link al folder di classificazione
              ObjectIdentity folderIdentity = null;
              
              // Verifica se l'idFolder si riferisce ad un fascicolo o a un sottofascicolo
              if (!DocsPaQueryHelper.isSottofascicolo(idFolder))
              {
                  // Reperimento id del fascicolo dal folder (record tipo "C")
                  string tipoFascicolo;
                  string idFascicolo = DocsPaQueryHelper.getIdFascicoloFromFolder(idFolder, out tipoFascicolo);
                  
                  folderIdentity = Dfs4DocsPa.getFascicoloIdentityBySystemId(idFascicolo);

                  //Effettuo un refresh dei permessi sul fascicolo per allineare le ACL con DOCUMENTUM
                  //Fascicolo fascicolo = DocsPaQueryHelper.getFascicolo(idFascicolo, this.InfoUtente);
                  //if (fascicolo != null)
                    //  RefreshAclFascicolo(fascicolo);
              }
              else
                  folderIdentity = Dfs4DocsPa.getSottofascicoloIdentityById(idFolder);

              ReferenceRelationship addRelationship = DfsHelper.createParentFolderRelationship(folderIdentity);
              //la aggiungeremo alla lista degli update nell'ultimo passo

              // 3. salvare
              DataObject updatedDoc = new DataObject(documentIdentity);
              updatedDoc.Relationships.Add(addRelationship);

              // 4. Allineamento ACL documento: 
              //  per coerenza è necessario che, una volta che il documento 
              //  viene inserito in un fascicolo, questo diventi visibile anche 
              //  a tutti gli utenti che hanno la visibilità solo sul fascicolo e 
              //  non sul documento 
              this.UpdateAclDocumento(idProfile, docNumber);

              DataPackage dataPackage = new DataPackage(updatedDoc);
              dataPackage = objectService.Update(dataPackage, null);

              retValue = (dataPackage.DataObjects.Count > 0);

              if (retValue)
                  logger.Debug(string.Format("Documentum.AddDocumentInFolder: inserito il documento con docnumber {0} nel folder con id {1}", idProfile, idFolder));
          }
          catch (Exception ex)
          {
              retValue = false;

              logger.Debug(string.Format("Errore in Documentum.AddDocumentInFolder:\n{0}", ex.ToString()));
          }
          logger.Info("END");
          return retValue;
      }

      /// <summary>
      /// Aggiornamento Acl per il documento
      /// </summary>
      /// <param name="idProfile"></param>
      protected virtual CustomServices.AclDefinition UpdateAclDocumento(string idProfile, string docNumber)
      {
          bool stamparegistro = DocsPaQueryHelper.isStampaRegistro(docNumber) && !DocsPaQueryHelper.isStampaRegistroRepertorio(docNumber);
          CustomServices.AclDefinition aclData = null;
          if (stamparegistro)
               aclData = Dfs4DocsPa.getAclDefinition(idProfile, ObjectTypes.DOCUMENTO_STAMPA_REGISTRO, this.InfoUtente);
          else
              aclData = Dfs4DocsPa.getAclDefinition(idProfile, ObjectTypes.DOCUMENTO, this.InfoUtente);
         // istanziamo ACLservice in modalità super-user per poter comunque creare una acl di sistema
         // nota: questo non sarebbe strettamente necessario (si potrebbe usare una user-acl, di tipo public)
         // ma si ritiene che non sia bello avere le acl di proprietà degli utenti creatori del documento
         CustomServices.IAclService aclSvcSuper = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());

         // Aggiornamento ACL
         aclSvcSuper.ClearAndGrant(aclData, idProfile);

         return aclData;
      }

      /// <summary>
      /// Creazione nuovo fascicolo
      /// </summary>
      /// <param name="classifica"></param>
      /// <param name="fascicolo"></param>
      /// <param name="ruolo"></param>
      /// <param name="enableUfficioReferente"></param>
      /// <param name="result"></param>
      /// <param name="ruoliSuperiori">
      /// Ruoli superiori cui è stata impostata la visibilità del fascicolo
      /// </param>
      /// <returns></returns>
      public bool CreateProject(Classificazione classifica, Fascicolo fascicolo, Ruolo ruolo, bool enableUfficioReferente, out ResultCreazioneFascicolo result, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
      {
         ruoliSuperiori = null;
         return this.CreateProject(classifica, fascicolo, ruolo, enableUfficioReferente, out result);
      }

      /// <summary>
      /// Creazione di un nuovo fascicolo in DCTM
      /// </summary>
      /// <remarks>
      /// 
      /// PreCondizioni:
      ///     Il fascicolo è stato inserito correttamente in DocsPa
      ///     ed è stato generato un'identificativo univoco 
      /// 
      /// PostCondizioni:
      ///     Creato un oggetto in Documentum corrispondente all'oggetto
      ///     fascicolo di DocsPa. L'oggetto avrà i metadati del fascicolo
      ///     per la sola consultazione in documentum.
      /// 
      /// </remarks>
      /// <param name="classifica"></param>
      /// <param name="fascicolo"></param>
      /// <param name="ruolo"></param>
      /// <param name="enableUfficioReferente"></param>
      /// <returns></returns>
      public bool CreateProject(Classificazione classifica, Fascicolo fascicolo, Ruolo ruolo, bool enableUfficioReferente, out ResultCreazioneFascicolo result)
      {
          logger.Info("BEGIN");
          bool retValue = false;
          result = ResultCreazioneFascicolo.GENERIC_ERROR;

          CustomServices.AclDefinition aclFascicolo = null;

          try
          {
              //fascicolo.systemID  viene SEMPRE valorizzato da ETDOCS; se così non è --> ERRORE
              if (string.IsNullOrEmpty(fascicolo.systemID))
              {
                  logger.Debug("Errore passaggio dati da ETDOCS.");
              }
              else
              {
                  // Reperimento dell'objectidentity relativo al nodo titolario in cui andrà inserito il fascicolo
                  ObjectIdentity nodoTitolarioIdentity = Dfs4DocsPa.getNodoTitolarioIdentity(fascicolo.idClassificazione);

                  // Reperimento properties del fascicolo procedimentale
                  PropertySet props = new PropertySet();
                  props.Properties.AddRange(Dfs4DocsPa.getFascicoloProcedimentaleProperties(fascicolo));

                  // Creazione delle ACL per il fascicolo
                  aclFascicolo = this.CreateAclFascicolo(fascicolo, ruolo);

                  // Associazione delle ACL al fascicolo da creare
                  AclHelper.setAclObjectProperties(props, aclFascicolo);

                  ObjectIdentity identity = new ObjectIdentity(DctmConfigurations.GetRepositoryName());
                  DataObject dataObject = new DataObject(identity, ObjectTypes.FASCICOLO_PROCEDIMENTALE);
                  dataObject.Properties = props;
                  dataObject.Relationships.Add(DfsHelper.createParentFolderRelationship(nodoTitolarioIdentity));

                  DataPackage dataPackage = new DataPackage(dataObject);

                  IObjectService objectService = this.GetServiceInstance<IObjectService>(false);
                  dataPackage = objectService.Create(dataPackage, null);

                  retValue = (dataPackage.DataObjects.Count == 1);

                  if (retValue)
                  {
                      result = ResultCreazioneFascicolo.OK;

                      logger.Debug(string.Format("Documentum.CreateProject: creato fascicolo con id {0}", fascicolo.systemID));
                  }
              }
          }
          catch (Exception ex)
          {
              retValue = false;
              result = ResultCreazioneFascicolo.GENERIC_ERROR;

              logger.Debug(string.Format("Errore in Documentum.CreateProject:\n{0}", ex.ToString()));

              if (aclFascicolo != null)
              {
                  // Rimozione ACL fascicolo in caso di errore
                  this.DeleteAcl(aclFascicolo);
              }
          }
          logger.Info("END");
          return retValue;
      }

      /// <summary>
      /// Creazione nuovo sottofascicolo
      /// </summary>
      /// <param name="folder"></param>
      /// <param name="ruolo"></param>
      /// <param name="result"></param>
      /// <param name="ruoliSuperiori">
      /// Ruoli superiori cui è impostata la visibilità del sottofascicolo
      /// </param>
      /// <returns></returns>
      public bool CreateFolder(DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.fascicolazione.ResultCreazioneFolder result, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
      {
          bool retValue = false;
          result = ResultCreazioneFolder.GENERIC_ERROR;
          ruoliSuperiori = null;

          try
          {
              // Recuperare l'ID del padre del folder: dato che la modalità è diversa se il padre
              // è Fascicolo o Sottofascicolo, è necessario verificare la casistica

              // Caso in cui il parent del sottofascicolo è un sottofascicolo stesso
              Qualification qualSottoFascicoloParent = Dfs4DocsPa.getSottofascicoloQualificationById(folder.idParent);
              string parentId = DfsHelper.getDctmObjectId(this.GetServiceInstance<IQueryService>(false), qualSottoFascicoloParent);

              if (string.IsNullOrEmpty(parentId))
              {
                  // Caso in cui il parent del sottofascicolo è un fascicolo stesso                    
                  qualSottoFascicoloParent = Dfs4DocsPa.getFascicoloQualificationById(folder.idFascicolo);
                  parentId = DfsHelper.getDctmObjectId(this.GetServiceInstance<IQueryService>(false), qualSottoFascicoloParent);
              }

              if (!string.IsNullOrEmpty(parentId))
              {
                  ObjectIdentity parentIdentity = new ObjectIdentity(new ObjectId(parentId), DctmConfigurations.GetRepositoryName());

                  ObjectIdentity identity = new ObjectIdentity(DctmConfigurations.GetRepositoryName());
                  DataObject dataObject = new DataObject(identity, ObjectTypes.SOTTOFASCICOLO);
                  dataObject.Properties.Properties.AddRange(Dfs4DocsPa.getSottoFascicoloProperties(folder));
                  dataObject.Relationships.Add(DfsHelper.createParentFolderRelationship(parentIdentity));

                  // Reperimento ACL del fascicolo di appartenenza e associazione al sottofascicolo
                  CustomServices.AclDefinition aclDefinition = this.GetAclDefinitionSottoFascicolo(folder);
                  AclHelper.setAclObjectProperties(dataObject.Properties, aclDefinition);

                  DataPackage dataPackage = new DataPackage(dataObject);
                  dataPackage = this.GetServiceInstance<IObjectService>(false).Create(dataPackage, null);

                  retValue = (dataPackage.DataObjects.Count == 1);

                  if (retValue)
                  {
                      result = ResultCreazioneFolder.OK;

                      logger.Debug(string.Format("Documentum.CreateFolder: creato sottofascicolo con id {0}", folder.systemID));
                  }
              }
              else
              {
                  throw new ApplicationException(string.Format("Non è stato possibile reperire l'oggetto parent per il folder con id '{0}'", folder.systemID));
              }
          }
          catch (Exception ex)
          {
              retValue = false;

              logger.Debug(string.Format("Errore in Documentum.CreateFolder:\n{0}", ex.ToString()));
          }

          return retValue;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="folder"></param>
      /// <param name="ruolo"></param>
      /// <returns></returns>
      public bool CreateFolder(DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.fascicolazione.ResultCreazioneFolder result)
      {
          DocsPaVO.utente.Ruolo[] ruoliSuperiori;
          return this.CreateFolder(folder, ruolo, out result, out ruoliSuperiori);
      }

      /// <summary>
      /// Modifica dei metadati di un fascicolo
      /// </summary>
      /// <param name="fascicolo"></param>
      /// <param name="refreshAclIfUpdate">
      /// Se true, indica di aggiornare le entries dell'ACL associata al nodo titolario
      /// </param>
      /// <returns></returns>
      public bool ModifyProject(DocsPaVO.fascicolazione.Fascicolo fascicolo, bool refreshAclIfUpdate)
      {
          bool retValue = false;

          try
          {
              // Solo gli attributi del fascicolo procedimentale può essere modificato in documentum
              if (fascicolo.tipo == "P")
              {
                  // Reperimento identity del fascicolo procedimentale
                  ObjectIdentity identity = Dfs4DocsPa.getFascicoloProcedimentaleIdentityById(fascicolo.systemID);

                  DataObject dataObject = new DataObject(identity);

                  // Impostazione proprietà fascicolo
                  dataObject.Properties.Properties.AddRange(Dfs4DocsPa.getFascicoloProcedimentaleProperties(fascicolo));

                  DataPackage dataPackage = new DataPackage(dataObject);

                  IObjectService objectService = this.GetServiceInstance<IObjectService>(true);
                  dataPackage = objectService.Update(dataPackage, null);

                  retValue = (dataPackage.DataObjects.Count == 1);

                  if (retValue)
                  {
                      if (refreshAclIfUpdate)
                      {
                          // Aggiornamento delle entries dell'acl associata al fascicolo
                          this.RefreshAclFascicolo(fascicolo);
                      }

                      logger.Debug(string.Format("Documentum.ModifyProject: modificato fascicolo con id {0}", fascicolo.systemID));
                  }
              }
              else
                  retValue = true;
          }
          catch (Exception ex)
          {
              retValue = false;

              logger.Debug(string.Format("Errore in Documentum.ModifyProject:\n{0}", ex.ToString()));
          }

          return retValue;
      }

      /// <summary>
      /// Modifica dei metadati di un fascicolo
      /// </summary>
      /// <param name="fascicolo"></param>
      /// <returns></returns>
      public bool ModifyProject(DocsPaVO.fascicolazione.Fascicolo fascicolo)
      {
          return this.ModifyProject(fascicolo, false);
      }

      /// <summary>
      /// Cancellazione di un folder in DocsPa
      /// </summary>
      /// <param name="idProject">Identificativo univoco del folder</param>
      /// <remarks>
      /// 
      /// PreCondizioni:
      ///     Il folder è stato rimosso correttamente in DocsPa
      /// 
      /// PostCondizioni:
      ///     Il corrispondente oggetto in documentum per il folder 
      ///     è stato rimosso correttamente
      /// 
      /// </remarks>
      /// <returns></returns>
      public bool DeleteFolder(DocsPaVO.fascicolazione.Folder folder)
      {
         bool retValue = false;

         try
         {
            DeleteProfile deleteProfile = new DeleteProfile();
            deleteProfile.IsDeepDeleteFolders = true;
            deleteProfile.IsDeepDeleteChildrenInFolders = true;

            IObjectService objectService = this.GetServiceInstance<IObjectService>(false);

            OperationOptions opts = new OperationOptions();
            opts.DeleteProfile = deleteProfile;

            // Reperimento identity per il sottofascicolo
            ObjectIdentity identity = Dfs4DocsPa.getSottofascicoloIdentityById(folder.systemID);

            objectService.Delete(new ObjectIdentitySet(identity), opts);

            retValue = true;

            logger.Debug(string.Format("Documentum.DeleteFolder: rimosso sottofascicolo con id {0}", folder.systemID));
         }
         catch (Exception ex)
         {
            retValue = false;

            logger.Debug(string.Format("Errore in Documentum.DeleteFolder:\n{0}", ex.ToString()));
         }

         return retValue;
      }

      /// <summary>
      /// Modifica dei dati di un folder
      /// </summary>
      /// <param name="folder"></param>
      /// <returns></returns>
      public bool ModifyFolder(DocsPaVO.fascicolazione.Folder folder)
      {
          bool retValue = false;

          try
          {
              // Reperimento identity per il sottofascicolo
              ObjectIdentity identity = Dfs4DocsPa.getSottofascicoloIdentityById(folder.systemID);

              DataObject dataObject = new DataObject(identity, ObjectTypes.SOTTOFASCICOLO);

              // Reperimento properties del sottofascicolo
              dataObject.Properties.Properties.AddRange(Dfs4DocsPa.getSottoFascicoloProperties(folder));

              DataPackage dataPackage = new DataPackage(dataObject);

              IObjectService objectService = this.GetServiceInstance<IObjectService>(true);
              dataPackage = objectService.Update(dataPackage, null);

              retValue = (dataPackage.DataObjects.Count == 1);

              if (retValue)
                  logger.Debug(string.Format("Documentum.ModifyFolder: modificato sottofascicolo con id {0}", folder.systemID));
          }
          catch (Exception ex)
          {
              retValue = false;

              logger.Debug(string.Format("Errore in Documentum.ModifyFolder:\n{0}", ex.ToString()));
          }

          return retValue;
      }

      /// <summary>
      /// Rimozione di un documento da un folder
      /// </summary>
      /// <param name="idProfile"></param>
      /// <param name="folder"></param>
      /// <returns></returns>
      public bool RemoveDocumentFromFolder(string idProfile, DocsPaVO.fascicolazione.Folder folder)
      {
         bool retValue = false;

         // Reperimento docNumber da idProfile
         logger.Debug("Reperimento docNumber da idProfile");
         string docNumber = DocsPaQueryHelper.getDocNumber(idProfile);

         bool isStampaRegistro = DocsPaQueryHelper.isStampaRegistroRepertorio(docNumber);

         try
         {
             logger.Debug("getDocumentoIdentityByDocNumber");
            ObjectIdentity documentIdentity = Dfs4DocsPa.getDocumentoIdentityByDocNumber(docNumber);
            
             if (isStampaRegistro)
                documentIdentity = Dfs4DocsPa.getDocumentoStampaRegistroIdentityByDocNumber(docNumber);

            ObjectIdentity folderIdentity = null;

            // Verifica se l'id è relativo ad un fascicolo o a un sottofascicolo
            logger.Debug("Verifica se l'id è relativo ad un fascicolo o a un sottofascicolo");
            if (!DocsPaQueryHelper.isSottofascicolo(folder.systemID))
                folderIdentity = Dfs4DocsPa.getFascicoloIdentityBySystemId(folder.idFascicolo);
            else
                folderIdentity = DctmServices.Dfs4DocsPa.getSottofascicoloIdentityById(folder.systemID);

            logger.Debug("createRemoveParentFolder");
            ReferenceRelationship removeRelationship = DctmServices.DfsHelper.createRemoveParentFolder(folderIdentity);

            logger.Debug("updatedDoc.Relationships");
            DataObject updatedDoc = new DataObject(documentIdentity);
            updatedDoc.Relationships.Add(removeRelationship);

            DataPackage dp = new DataPackage(updatedDoc);

            logger.Debug("update");
            IObjectService objSrvc = DctmServiceFactory.GetServiceInstance<IObjectService>(UserManager.ImpersonateSuperUser());
            DataPackage retDp = objSrvc.Update(dp, null);

            retValue = (retDp.DataObjects.Count > 0);

            if (retValue)
               logger.Debug(string.Format("Documentum.RemoveDocumentFromFolder: rimosso documento con docnumber {0} dal sottofascicolo con id {0}", idProfile, folder.systemID));
         }
         catch (Exception ex)
         {
            retValue = false;

            logger.Debug(string.Format("Errore in Documentum.RemoveDocumentFromFolder:\n{0}", ex.ToString()));
         }

         return retValue;
      }

      /// <summary>
      /// Rimozione di un documento dal fascicolo (in generale, da tutti i folder presenti nel fascicolo)
      /// </summary>
      /// <param name="idProfile"></param>
      /// <param name="folder"></param>
      /// <returns></returns>
      public bool RemoveDocumentFromProject(string idProfile, DocsPaVO.fascicolazione.Folder folder)
      {
          logger.Debug(string.Format("RemoveDocumentFromProject - IdProfile: {0} - IdFolder: {1} - IdFascicolo: {2}", idProfile, folder.systemID, folder.idFascicolo));

          // 1. recuperare tutti link del documento
          // 2. identificare quelli da rimuovere (tutti quelli sotto il folder in input)
          // 3. aggiungere eventualmente il link a documenti non classificati
          //    (se non c'è neanche più un link residuo)

          bool retValue = false;
          IObjectService objSrvc = null;

          // Reperimento docNumber da idProfile
          string docNumber = DocsPaQueryHelper.getDocNumber(idProfile);

          bool isStampaRegistro = DocsPaQueryHelper.isStampaRegistroRepertorio(docNumber);
          try
          {
              string repositoryName = DctmConfigurations.GetRepositoryName();
              //objSrvc = this.GetObjectServiceInstance();
              objSrvc = DctmServiceFactory.GetServiceInstance<IObjectService>(UserManager.ImpersonateSuperUser());
              List<Relationship> removeRelationships = new List<Relationship>();

              // 1. recuperare tutti link del documento:
              ObjectIdentity documentIdentity = Dfs4DocsPa.getDocumentoIdentityByDocNumber(docNumber);
              if (isStampaRegistro)
                  documentIdentity = Dfs4DocsPa.getDocumentoStampaRegistroIdentityByDocNumber(docNumber);

              logger.Debug("RemoveDocumentFromProject: 1.recuperare tutti link del documento");

              List<string> filters = new List<string>();
              filters.Add("r_object_id");
              DataObject documentData = DfsHelper.getAllPropsAndFolders(objSrvc, documentIdentity, filters, true);

              // 2. identificare quelli da rimuovere (tutti quelli sotto il folder in input):
              if (documentData.Relationships != null && documentData.Relationships.Count > 0)
              {
                  logger.Debug("RemoveDocumentFromProject:  2. identificare quelli da rimuovere (tutti quelli sotto il folder in input)");

                  // 2a. recuperiamo la root del nodo associato a Folder:
                  ObjectIdentity folderIdentity = Dfs4DocsPa.getFascicoloIdentityBySystemId(folder.idFascicolo);

                  if (folderIdentity.ValueType == ObjectIdentityType.QUALIFICATION)
                      logger.Debug(((Qualification)folderIdentity.Value).GetValueAsString());

                  filters = new List<string>();
                  filters.Add("r_folder_path");
                  DataObject folderData = DfsHelper.getAllPropsAndFolders(objSrvc, folderIdentity, filters, false);
                  Property p = folderData.Properties.Get("r_folder_path");
                  if (p == null)
                      throw new Exception("Impossibile leggere r_folder_path");
                  string rootPath = p.GetValueAsString();

                  logger.Debug(string.Format("RemoveDocumentFromProject: RootPath: {0}", rootPath));

                  foreach (ReferenceRelationship r in documentData.Relationships)
                  {
                      // 2b. recuperiamo il path del folder in esame
                      // qui contiamo sul fatto che gli objectIdentity dei folder a cui è linkato vengano istanziati 
                      // come OBJECT_ID
                      string targetDctmId = ((ObjectId)r.Target.Value).Id;

                      logger.Error(string.Format("LOG-DEBUG: RemoveDocumentFromProject: targetDctmId: {0}", targetDctmId));

                      ObjectIdentity targetIdentity = new ObjectIdentity(new ObjectId(targetDctmId), repositoryName);
                      DataObject targetData = DfsHelper.getAllPropsAndFolders(objSrvc, targetIdentity, filters, false);

                      Property prop = targetData.Properties.Properties.Find(e => e.Name == "r_folder_path");

                      if (prop != null)
                      {
                          string targetPath = targetData.Properties.Get("r_folder_path").GetValueAsString();

                          logger.Debug(string.Format("RemoveDocumentFromProject: targetPath: {0}", targetPath));

                          //se il target è sotto la root allora è da rimuovere
                          if (targetPath.StartsWith(rootPath))
                              removeRelationships.Add(DfsHelper.createRemoveParentFolder(targetIdentity));
                      }
                  }
              }

              // 3. committare il tutto
              DataObject updatedDoc = new DataObject(documentIdentity);
              updatedDoc.Relationships = removeRelationships;

              DataPackage dp = new DataPackage(updatedDoc);
              DataPackage retDp = objSrvc.Update(dp, null);

              logger.Debug("RemoveDocumentFromProject: 3. committare il tutto");

              if (retDp.DataObjects.Count > 0)
              {
                  retValue = true;

                  logger.Debug(string.Format("Documentum.RemoveDocumentFromProject: rimosso documento con docnumber {0} dal fascicolo con id {0}", idProfile, folder.systemID));
              }
          }
          catch (Exception e)
          {
              retValue = false;

              logger.Debug(string.Format("Errore in Documentum.RemoveDocumentFromProject:\n{0}", e.ToString()));
          }

          return retValue;
      }

      /// <summary>
      /// Impostazione della visibilità su un fascicolo 
      /// (e dell'ownership, nel caso l'utente / ruolo rimosso fosse il proprietario)
      /// </summary>
      /// <param name="infoDiritto"></param>
      /// <returns></returns>
      public bool AddPermission(DocsPaVO.fascicolazione.DirittoOggetto infoDiritto)
      {
         bool added = false;

         // se si revocano i diritti all'utente proprietario, 
         // si revocano anche al ruolo proprietario e viceversa. 
         // Il proprietario del documento diventa l'utente e il ruolo del revocante

         try
         {
            // Reperimento utente proprietario
            string ownerUser = TypeUtente.NormalizeUserName(DocsPaQueryHelper.getCodiceRubricaProprietario(infoDiritto.idObj, true));

            // Modifica ownership del fascicolo
            if (this.SetOwnershipFascicolo(infoDiritto.idObj, ownerUser))
            {
               CustomServices.AclDefinition aclDefinition = Dfs4DocsPa.getAclDefinition(infoDiritto.idObj, ObjectTypes.FASCICOLO, this.InfoUtente);

               CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());

               aclService.ClearAndGrant(aclDefinition, infoDiritto.idObj);

               added = true;
            }
         }
         catch (Exception ex)
         {
            added = false;

            logger.Debug(string.Format("Errore in Documentum.RemovePermission:\n{0}", ex.ToString()));
         }

         return added;
      }

      /// <summary>
      /// Revoca della visibilità su un documento (e dell'ownership, nel caso l'utente / ruolo rimosso è proprietario)
      /// </summary>
      /// <param name="documentInfo"></param>
      /// <returns></returns>
      public bool RemovePermission(DocsPaVO.fascicolazione.DirittoOggetto infoDiritto)
      {
         bool removed = false;

         try
         {
            // Reperimento utente proprietario
            string ownerUser = TypeUtente.NormalizeUserName(DocsPaQueryHelper.getCodiceRubricaProprietario(infoDiritto.idObj, true));

            if (this.SetOwnershipFascicolo(infoDiritto.idObj, ownerUser))
            {
               CustomServices.AclDefinition aclDefinition = Dfs4DocsPa.getAclDefinition(infoDiritto.idObj, ObjectTypes.FASCICOLO, this.InfoUtente);

               CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());

               aclService.ClearAndGrant(aclDefinition, infoDiritto.idObj);
            }

            removed = true;
         }
         catch (Exception ex)
         {
            removed = false;

            logger.Debug(string.Format("Errore in Documentum.RemovePermission:\n{0}", ex.ToString()));
         }

         return removed;
      }

      /// <summary>
      /// Rimozione di un fascicolo in un DCTM
      /// </summary>
      /// <param name="fascicolo"></param>
      /// <returns></returns>
      public bool DeleteProject(Fascicolo fascicolo)
      {
          bool retValue = false;
          
          try
          {
              // Rimozione oggetto fascicolo in DCTM
              DeleteProfile deleteProfile = new DeleteProfile();
              deleteProfile.IsDeepDeleteFolders = true;
              deleteProfile.IsDeepDeleteChildrenInFolders = true;
              deleteProfile.IsDeepDeleteVdmInFolders = true;

              OperationOptions opts = new OperationOptions();
              opts.DeleteProfile = deleteProfile;
              
              // Reperimento identity per il fascicolo
              ObjectIdentity identity = Dfs4DocsPa.getFascicoloProcedimentaleIdentityById(fascicolo.systemID);

              // Rimozione del fascicolo
              IObjectService objectService = DctmServiceFactory.GetServiceInstance<IObjectService>(UserManager.ImpersonateSuperUser());

              objectService.Delete(new ObjectIdentitySet(identity), opts);

              // Rimozione dell'ACL associata al fascicolo
              this.DeleteAcl(Dfs4DocsPa.getAclDefinition(fascicolo.systemID, ObjectTypes.FASCICOLO, this.InfoUtente));

              retValue = true;

              logger.Debug(string.Format("Documentum.DeleteFolder: rimosso fascicolo con id {0}", fascicolo.systemID));
          }
          catch (Exception ex)
          {
              retValue = false;

              logger.Debug(string.Format("Errore in Documentum.DeleteProject:\n{0}", ex.ToString()));
          }

          return retValue;
      }

      /// <summary>
      /// Aggiornamento delle fascicolazioni
      /// </summary>
      /// <param name="idFolder"></param>
      public virtual bool UpdateFascicolazioni(string idFolder)
      {
          bool retValue = false;

          // Reperimento record di tipo "C"
          string idFolderC = DocsPaQueryHelper.getIdFolderCType(idFolder);

          try
          {
              IQueryService queryService = DctmServiceFactory.GetServiceInstance<IQueryService>(UserManager.ImpersonateSuperUser());

              // 1. Reperimento di tutti i documenti contenuti in fascicolo in DCTM
              List<string> documentiInDctm = new List<string>(Dfs4DocsPa.getIdDocumentiInFascicolo(queryService, idFolder));

              // 2. Reperimento di tutti i documenti contenuti in fascicolo in PITRE
              foreach (string idDocumento in DocsPaQueryHelper.getDocumentiInFolder(idFolderC))
              {
                  // 3. Per ogni documento contenuto nel fascicolo, verifica se esiste nel corrispondente fascicolo in DCTM
                  if (!documentiInDctm.Contains(idDocumento))
                  {
                      // 4. Verifica se il documento esiste in DCTM
                      if (!Dfs4DocsPa.containsDocumento(idDocumento, queryService))
                          throw new ApplicationException(string.Format("Documento con id '{0}' non esistente in Documentum", idDocumento));

                      // 4. Inserimento del documento nel fascicolo in DCTM se non esiste
                      if (!this.AddDocumentInFolder(idDocumento, idFolderC))
                          throw new ApplicationException(string.Format("Errore in inserimento del documento con id '{0}' nel fascicolo con id '{1}'", idDocumento, idFolderC));
                  }
              }

              retValue = true;
          }
          catch (Exception ex)
          {
              retValue = false;

              logger.Debug(string.Format("Errore in Documentum.UpdateFascicolazioni:\n{0}", ex.ToString()));
          }

          return retValue;
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
      /// Reperimento istanza servizio
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="asSuperUser">
      /// Se true, si collega con le credenziali da superamministratore
      /// </param>
      /// <returns></returns>
      protected virtual T GetServiceInstance<T>(bool asSuperUser)
      {
          string dst = string.Empty;

          if (asSuperUser)
              dst = UserManager.ImpersonateSuperUser();
          else
              dst = this.InfoUtente.dst;

          return DctmServiceFactory.GetServiceInstance<T>(dst);
      }

      /// <summary>
      /// Impostazione dell'ownership sul documento
      /// </summary>
      /// <param name="idProject"></param>        
      /// <param name="ownerName"></param>
      /// <returns></returns>
      public virtual bool SetOwnershipFascicolo(string idFascicolo, string ownerName)
      {
         // Modifica del campo "owner_name" per il fascicolo e tutti gli eventuali sottofascicoli
         bool retValue = false;

         try
         {
            // Creazione queryservice con credenziali di superutente
            Dfs4DocsPa.setOwnershipFascicolo(idFascicolo, ownerName, DctmServiceFactory.GetServiceInstance<IQueryService>(UserManager.ImpersonateSuperUser()));

            retValue = true;
         }
         catch (Exception ex)
         {
            retValue = false;

            logger.Debug(string.Format("Errore in Documentum.SetOwnershipFascicolo:\n{0}", ex.ToString()));
         }

         return retValue;
      }

      /// <summary>
      /// Reperimento dell'utente creatore di un fascicolo in DCTM
      /// </summary>
      /// <param name="idFascicolo"></param>
      /// <param name="ownerRole"></param>
      /// <returns></returns>
      protected virtual void GetOwnershipFascicolo(string idFascicolo, out string ownerUser)
      {
         IQueryService queryService = DctmServiceFactory.GetServiceInstance<IQueryService>(this.InfoUtente.dst);

         Dfs4DocsPa.getOwnershipFascicolo(idFascicolo, out ownerUser, queryService);
      }

      /// <summary>
      /// Rimozione dell'ACL
      /// </summary>
      /// <param name="aclToRemove"></param>
      protected virtual void DeleteAcl(CustomServices.AclDefinition aclToRemove)
      {
         // Creazione servizio ACL (con credenziali da superamministratore)
         CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());

         aclService.Delete(aclToRemove);
      }

      /// <summary>
      /// Reperimento ACL associate al sottofascicolo
      /// </summary>
      /// <param name="folder"></param>
      /// <returns></returns>
      protected virtual CustomServices.AclDefinition GetAclDefinitionSottoFascicolo(DocsPaVO.fascicolazione.Folder folder)
      {
          // Reperimento ACL del fascicolo di appartenenza e associazione al sottofascicolo
          CustomServices.AclDefinition aclDefinition = null;

          if (DocsPaQueryHelper.isFascicoloGenerale(folder.idFascicolo))
          {
              // Determina se il fascicolo in cui si sta inserendo il folder è un fascicolo generale;
              // in tal caso l'acl da assegnare è la stessa del nodo titolario di appartenenza
              string idNodoTitolario = DocsPaQueryHelper.getIdNodoTitolario(folder.idFascicolo);

              aclDefinition = Dfs4DocsPa.getAclDefinition(idNodoTitolario, ObjectTypes.NODO_TITOLARIO, this.InfoUtente);
          }
          else
          {
              // Nel caso di inserimento di un folder in un fascicolo procedimentale,
              // l'acl del folder sarà quella del fascicolo di appartenenza
              aclDefinition = Dfs4DocsPa.getAclDefinition(folder.idFascicolo, ObjectTypes.FASCICOLO, this.InfoUtente);
          }

          return aclDefinition;
      }

      /// <summary>
      /// Aggiornamento delle entries dell'acl del fascicolo
      /// </summary>
      /// <param name="fascicolo"></param>
      protected virtual void RefreshAclFascicolo(DocsPaVO.fascicolazione.Fascicolo fascicolo)
      {
          try
          {
              // Reperimento ACL per il fascicolo
              CustomServices.AclDefinition aclData = Dfs4DocsPa.getAclDefinition(fascicolo.systemID, ObjectTypes.FASCICOLO, this.InfoUtente);

              // istanziamo ACLservice in modalità super-user per poter comunque creare una acl di sistema
              // nota: questo non sarebbe strettamente necessario (si potrebbe usare una user-acl, di tipo public)
              // ma si ritiene che non sia bello avere le acl di proprietà degli utenti creatori del documento
              CustomServices.IAclService aclSvcSuper = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());

              aclSvcSuper.ClearAndGrant(aclData, fascicolo.systemID);
          }
          catch (Exception ex)
          {
              string errorMessage = string.Format("Errore in Documentum.RefreshAclFascicolo:\n{0}", ex.ToString());
              logger.Debug(errorMessage);

              throw new ApplicationException(errorMessage);
          }
      }

      /// <summary>
      /// Creazione delle ACL per il fascicolo
      /// NOTA: RW di DocsPA equivale a DELETE di Documentum
      /// </summary>
      /// <param name="fascicolo"></param>
      /// <param name="ruoloCreatore"></param>
      /// <returns>ACL creata, ma ancora non associata al documento</returns>
      protected virtual CustomServices.AclDefinition CreateAclFascicolo(DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.utente.Ruolo ruoloCreatore)
      {
         CustomServices.AclDefinition aclData = null;

         try
         {
            // Reperimento ACL per il fascicolo
            aclData = Dfs4DocsPa.getAclDefinition(fascicolo.systemID, ObjectTypes.FASCICOLO, this.InfoUtente);

            // istanziamo ACLservice in modalità super-user per poter comunque creare una acl di sistema
            // nota: questo non sarebbe strettamente necessario (si potrebbe usare una user-acl, di tipo public)
            // ma si ritiene che non sia bello avere le acl di proprietà degli utenti creatori del documento
            CustomServices.IAclService aclSvcSuper = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());

            CustomServices.ObjectIdentity aclIdentity = aclSvcSuper.Create(aclData);

            if (aclIdentity == null)
               throw new ApplicationException(string.Concat("Errore nella creazione dell'ACL per il fascicolo con id '{0}'", fascicolo.systemID));
         }
         catch (Exception ex)
         {
            string errorMessage = string.Format("Errore in Documentum.CreateAclFascicolo:\n{0}", ex.ToString());
            logger.Debug(errorMessage);

            throw new ApplicationException(errorMessage);
         }

         return aclData;
      }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="idFascicolo"></param>
       /// <returns></returns>
      public bool ContainsFascicoloProcedimentale(string idFascicolo)
      {
          IQueryService queryService = DctmServiceFactory.GetServiceInstance<IQueryService>(UserManager.ImpersonateSuperUser());

          return Dfs4DocsPa.containsFascicoloProcedimentale(idFascicolo, queryService);
      }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="idSottofascicolo"></param>
       /// <returns></returns>
      public bool ContainsSottofascicolo(string idSottofascicolo)
      {
          IQueryService queryService = DctmServiceFactory.GetServiceInstance<IQueryService>(UserManager.ImpersonateSuperUser());

          return Dfs4DocsPa.containsSottofascicolo(idSottofascicolo, queryService);
      }

      #endregion

      #region Private methods

      #endregion
   }
}
