using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.documento;
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
    /// Classe per la gestione del documentale DOCUMENTUM.
    /// Ha la responsabilità di interagire direttamente 
    /// con le API esposte da DOCUMENTUM per soddisfare le richieste.
    /// </summary>
    public class DocumentManager : IDocumentManager
    {
        private ILog logger = LogManager.GetLogger(typeof(DocumentManager));
        #region Ctors, constants, variables

        /// <summary>
        /// Credenziali utente
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// Inizializza l'istanza della classe acquisendo i dati relativi all'utente 
        /// ed alla libreria per la connessione al documentale.
        /// </summary>
        /// <param name="infoUtente">Dati relativi all'utente</param>
        public DocumentManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.utente.Ruolo[] ruoliSuperiori;
            return this.CreateDocumentoGrigio(schedaDocumento, ruolo, out ruoliSuperiori);
        }

        /// <summary>
        /// Metodo per l'assegnazione di un diritto di tipo A ad un ruolo
        /// </summary>
        /// <param name="rights">Informazioni sul diritto da assegnare</param>
        /// <returns>True se è andato bene</returns>
        public bool AddPermissionToRole(DirittoOggetto rights)
        {
            return this.AddPermission(rights);
        }

        /// <summary>
        /// Creazione di un nuovo documento per la stampa registro.
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.utente.Ruolo[] ruoliSuperiori;
            return this.CreateDocumentoStampaRegistro(schedaDocumento, ruolo, out ruoliSuperiori);
        }

        /// <summary>
        /// Inserimento dei soli metadati di un nuovo allegato PEC senza controllo security documento principale
        ///
        /// 
        /// </summary>
        /// <param name="allegato"></param>
        /// <param name="putfile"></param>
        /// <returns></returns>
        public bool AddAttachmentPECNoSecurity(DocsPaVO.documento.Allegato allegato, string putfile)
        {
            return AddAttachment(allegato, putfile);

        }

        /// <summary>
        /// Operazione per la creazione di un nuovo documento
        /// 
        /// PreCondizioni:
        ///     Il documento grigio deve essere già stato come entità in DocsPa
        /// 
        /// PostCondizioni:
        ///     I metadati del documento devono essere stati inseriti in Documentum
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            bool retValue = false;
            ruoliSuperiori = null;

            try
            {
                // Creazione oggetto identity per nuovo inserimento
                ObjectIdentity identity = new ObjectIdentity(DctmConfigurations.GetRepositoryName());

                // Creazione DataObject
                DataObject dataObject = new DataObject(identity, ObjectTypes.DOCUMENTO_STAMPA_REGISTRO);

                // Reperimento proprietà documentum (solo la descrizione dell'oggetto)
                dataObject.Properties.Properties.AddRange(Dfs4DocsPa.getDocumentoStampaRegistroProperties(schedaDocumento, true));

                // Reperimento ACL dell'amministrazione da associare al documento stampa registro
                CustomServices.AclDefinition aclDefinition = this.CreateAclDocumento(schedaDocumento);

                // Associazione ACL al documento
                AclHelper.setAclObjectProperties(dataObject.Properties, aclDefinition);

                // Impersonation con il superuser documentum per la creazione del folder per i documenti
                IObjectService objectServiceSuperUser = this.GetServiceInstance<IObjectService>(true);

                // Reperimento parent folder per il documento
                ObjectIdentity parentIdentity = Dfs4DocsPa.getStampaRegistroParentFolderIdentity(schedaDocumento, this.InfoUtente, objectServiceSuperUser);

                dataObject.Relationships.Add(DctmServices.DfsHelper.createParentFolderRelationship(parentIdentity));

                DataPackage dataPackage = new DataPackage(dataObject);

                IObjectService service = this.GetServiceInstance<IObjectService>(false);

                dataPackage = service.Create(dataPackage, null);

                retValue = (dataPackage.DataObjects.Count > 0);

                if (retValue)
                    logger.Debug(string.Format("Documentum.CreateDocumentoStampaRegistro: creato nuovo documento stampa registro {0}", schedaDocumento.oggetto.descrizione));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.CreateDocumentoStampaRegistro:\n{0}", ex.ToString()));
            }

            return retValue;
        }

        /// <summary>
        /// Creazione di un nuovo documento grigio
        /// 
        /// PreCondizioni:
        ///     Il documento grigio deve essere già stato come entità in DocsPa
        /// 
        /// PostCondizioni:
        ///     I metadati del documento devono essere stati inseriti in Documentum
        ///     Nessun dato deve essere modificato negli oggetti forniti in ingresso
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null)
        {
            logger.Info("BEGIN");
            ruoliSuperiori = null;
            bool retValue = false;

            CustomServices.AclDefinition aclDocumento = null;

            try
            {
                // Creazione oggetto identity per nuovo inserimento
                ObjectIdentity identity = new ObjectIdentity(DctmConfigurations.GetRepositoryName());

                // Creazione DataObject
                DataObject dataObject = new DataObject(identity, ObjectTypes.DOCUMENTO);

                // Reperimento proprietà documentum
                dataObject.Properties.Properties.AddRange(DctmServices.Dfs4DocsPa.getDocumentoProperties(schedaDocumento, this.InfoUtente, true));

                // Creazione ACL per il documento
                aclDocumento = this.CreateAclDocumento(schedaDocumento);

                // Associazione ACL al documento
                AclHelper.setAclObjectProperties(dataObject.Properties, aclDocumento);

                // Impersonation con il superuser documentum per la creazione del folder per i documenti
                IObjectService objectServiceSuperUser = this.GetServiceInstance<IObjectService>(true);

                // Reperimento parent folder per il documento
                ObjectIdentity parentIdentity = Dfs4DocsPa.getDocumentoParentFolderIdentity(schedaDocumento, this.InfoUtente, objectServiceSuperUser);

                dataObject.Relationships.Add(DfsHelper.createParentFolderRelationship(parentIdentity));

                DataPackage dataPackage = new DataPackage(dataObject);

                IObjectService service = this.GetServiceInstance<IObjectService>(false);

                dataPackage = service.Create(dataPackage, null);

                retValue = (dataPackage.DataObjects.Count > 0);

                if (retValue)
                    logger.Debug(string.Format("Documentum.CreateDocumentoGrigio: creato nuovo documento, docnumber {0}", schedaDocumento.docNumber));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.CreateDocumentoGrigio:\n{0}", ex.ToString()));

                if (aclDocumento != null)
                {
                    // Rimozione ACL creata per il documento
                    this.DeleteAclDocumento(aclDocumento);
                }
            }
            logger.Info("END");
            return retValue;
        }

        public virtual void RefreshAclDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            try
            {
                CustomServices.AclDefinition aclData = null;
                // Reperimento ACL per il documento
                aclData = Dfs4DocsPa.getAclDefinition(schedaDocumento.docNumber, DocsPaObjectTypes.ObjectTypes.DOCUMENTO, this.InfoUtente);

                // istanziamo ACLservice in modalità super-user per poter comunque creare una acl di sistema
                // nota: questo non sarebbe strettamente necessario (si potrebbe usare una user-acl, di tipo public)
                // ma si ritiene che non sia bello avere le acl di proprietà degli utenti creatori del documento
                CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());

                aclService.ClearAndGrant(aclData, schedaDocumento.systemId);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore in Documentum.RefreshAclDocumento:\n{0}", ex.ToString());
                logger.Debug(errorMessage);

                throw new ApplicationException(errorMessage);
            }
        }

        /// <summary>
        /// Creazione delle ACL per il documento
        /// NOTA: RW di DocsPA equivale a DELETE di Documentum
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns>ACL creata, ma ancora non associata al documento</returns>
        protected virtual CustomServices.AclDefinition CreateAclDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            CustomServices.AclDefinition aclData = null;

            try
            {
                // Reperimento ACL documento
                // aclData = Dfs4DocsPa.getAclDefinitionDocumento(schedaDocumento, ruoloCreatore, this.InfoUtente);

                string objectType = string.Empty;
                if (schedaDocumento.tipoProto == "R")
                    objectType = ObjectTypes.DOCUMENTO_STAMPA_REGISTRO;
                else
                    objectType = ObjectTypes.DOCUMENTO;

                // Reperimento della lista delle ACL aggiornata nella tabella security di docspa
                aclData = Dfs4DocsPa.getAclDefinition(schedaDocumento.docNumber, objectType, this.InfoUtente);

                // istanziamo ACLservice in modalità super-user per poter comunque creare una acl di sistema
                // nota: questo non sarebbe strettamente necessario (si potrebbe usare una user-acl, di tipo public)
                // ma si ritiene che non sia bello avere le acl di proprietà degli utenti creatori del documento
                CustomServices.IAclService aclSvcSuper = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());

                CustomServices.ObjectIdentity aclIdentity = aclSvcSuper.Create(aclData);

                if (aclIdentity == null)
                    throw new ApplicationException(string.Concat("Errore nella creazione dell'ACL per il documento con DocNumber '{0}'", schedaDocumento.docNumber));
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore in Documentum.CreateAclDocumento:\n{0}", ex.ToString());
                logger.Debug(errorMessage);

                throw new ApplicationException(errorMessage);
            }

            return aclData;
        }

        /// <summary>
        /// Reperimento ACL per l'allegato
        /// NB: Deve essere lo stesso oggetto AclDefinition del documento
        /// in quanto un allegato ne segue necessariamente le stesse regole di visibilità
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        protected virtual CustomServices.AclDefinition GetAclDefinitionAllegato(string docNumber)
        {
            return Dfs4DocsPa.getAclDefinition(docNumber, ObjectTypes.DOCUMENTO, this.InfoUtente);
        }

        /// <summary>
        /// Rimozione dell'ACL per il documento
        /// </summary>
        /// <param name="docNumber"></param>
        protected void DeleteAclDocumento(string docNumber)
        {
            // Reperimento ACL da rimuovere
            CustomServices.AclDefinition aclToRemove = AclHelper.getAclDefinition(DocsPaQueryHelper.getCodiceAmministrazione(this.InfoUtente.idAmministrazione), docNumber, ObjectTypes.DOCUMENTO);

            this.DeleteAclDocumento(aclToRemove);
        }

        /// <summary>
        /// Rimozione dell'ACL per il documento
        /// </summary>
        /// <param name="aclToRemove"></param>
        protected virtual void DeleteAclDocumento(CustomServices.AclDefinition aclToRemove)
        {
            // Creazione servizio ACL (con credenziali da superamministratore)
            CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());
            aclService.Delete(aclToRemove);
        }

        /// <summary>
        /// Aggiornamento delle ACL per il documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        protected virtual CustomServices.AclDefinition UpdateAclDocumento(SchedaDocumento schedaDocumento)
        {
            CustomServices.AclDefinition aclData = null;

            try
            {
                string systemId = string.Empty;

                if (schedaDocumento.documentoPrincipale != null)
                    // Se è un allegato, viene reperito l'id del documento principale
                    systemId = schedaDocumento.documentoPrincipale.idProfile;
                else
                    systemId = schedaDocumento.systemId;

                aclData = Dfs4DocsPa.getAclDefinition(systemId, ObjectTypes.DOCUMENTO, this.InfoUtente);
                
                // istanziamo ACLservice in modalità super-user per poter comunque creare una acl di sistema
                // nota: questo non sarebbe strettamente necessario (si potrebbe usare una user-acl, di tipo public)
                // ma si ritiene che non sia bello avere le acl di proprietà degli utenti creatori del documento
                CustomServices.IAclService aclSvcSuper = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());

                // Aggiornamento ACL
                aclSvcSuper.ClearAndGrant(aclData, schedaDocumento.systemId);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore in Documentum.UpdateAclDocumento:\n{0}", ex.ToString());
                logger.Debug(errorMessage);

                throw new ApplicationException(errorMessage);
            }

            return aclData;
        }

        /// <summary>
        /// Rimozione di un documento grigio
        /// 
        /// BUG documentum:
        /// sembra che le dfs abbiano dei problemi nella cancellazione degli
        /// allegati, o più in generale nella cancellazione di tutti i documenti
        /// facenti parte di un vdoc.
        /// Per questo motivo, si è costretti a cancellare il documento grigio in
        /// due fasi, la prima per la canc di tutte le versioni, la seconda
        /// per la cancellazione degli allegati dove presenti.
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public string RemoveDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            string retValue = string.Empty;

            try
            {
                if (this.Remove(new DocsPaVO.documento.InfoDocumento(schedaDocumento)))
                {
                    retValue = "Del";

                    logger.Debug(string.Format("Documentum.RemoveDocumentoGrigio: rimosso documento con docnumber {0}", schedaDocumento.docNumber));
                }
                else
                {
                    retValue = string.Format("Errore nella rimozione del documento {0} in Documentum", schedaDocumento.docNumber);

                    logger.Debug(retValue);
                }
            }
            catch (Exception ex)
            {
                retValue = string.Format("Errore in Documentum.RemoveDocumentoGrigio:\n{0}", ex.ToString());

                logger.Debug(retValue);
            }

            return retValue;
        }

        /// <summary>
        /// Inserimento di un documento nel cestino,
        /// in DCTM, il documento viene impostato come invisibile
        /// </summary>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        public bool AddDocumentoInCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            if (infoDocumento.allegato)
            {
                // Nel caso il documento da rimuovere sia un allegato, viene rimosso definitivamente
                //return this.RemoveAttatchment(infoDocumento.docNumber);
                return true;
            }
            else
                return this.AddDocumentoInCestino(infoDocumento.docNumber);
        }

        /// <summary>
        /// Ripristino del documento dal cestino
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <returns></returns>
        public bool RestoreDocumentoDaCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            bool retValue = false;

            try
            {
                retValue = this.SetDocumentVisibility(infoDocumento.idProfile, infoDocumento.docNumber, true);
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.RestoreDocumentoDaCestino:\n{0}", ex.ToString()));
            }

            return retValue;
        }

        /// <summary>
        /// Rimozione documenti
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool Remove(params DocsPaVO.documento.InfoDocumento[] items)
        {
            bool retValue = false;

            try
            {
                ObjectIdentitySet identitySet = new ObjectIdentitySet();

                string deletedList = string.Empty;

                // La rimozione viene effettuata con le credenziali da superamministratore
                IQueryService queryService = this.GetServiceInstance<IQueryService>(true);

                // Per ogni documento da rimuovere, reperimento 
                // degli objectidentity relativi al documento principale 
                // e agli eventuali allegati
                foreach (DocsPaVO.documento.InfoDocumento item in items)
                {
                    identitySet.AddIdentity(DctmServices.Dfs4DocsPa.getDocumentoIdentityByDocNumber(item.docNumber));

                    identitySet.Identities.AddRange(Dfs4DocsPa.getAllegatiDocumentoIdentities(queryService, item.docNumber));

                    deletedList = string.Format("{0}, {1}", deletedList, item.docNumber);
                }

                IVersionControlService service = this.GetServiceInstance<IVersionControlService>(true);
                service.DeleteAllVersions(identitySet);

                // Nella fase successiva, rimozione di tutte le ACL per tutti i documenti rimossi
                foreach (DocsPaVO.documento.InfoDocumento item in items)
                {
                    // Rimozione ACL legata al documento
                    this.DeleteAclDocumento(item.docNumber);
                }

                retValue = true;

                logger.Debug(string.Format("Documentum.Remove: rimossi documenti {0}", deletedList));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.Remove:\n{0}", ex.ToString()));
            }

            return retValue;
        }

        /// <summary>
        /// Save delle modifiche apportate al documento
        ///
        /// PreCondizioni:
        ///     Le modifiche sul documento devono essere già state apportate con successo in DocsPa 
        /// 
        /// PostCondizioni:
        ///     I relativi oggetti in Documentum che rappresentano la scheda documento devono essere aggiornati
        ///     in maniera coerente
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ufficioReferenteEnabled">
        /// Gestione ufficio referente, non di pertinenza per documentum
        /// </param>
        /// <param name="ufficioReferenteSaved">
        /// Gestione ufficio referente, non di pertinenza per documentum
        /// </param>
        /// <returns></returns>
        public bool SalvaDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento, bool ufficioReferenteEnabled, out bool ufficioReferenteSaved)
        {
            bool retValue = false;
            ufficioReferenteSaved = false;

            try
            {
                // Aggiornamento ACL per il documento
                this.UpdateAclDocumento(schedaDocumento);

                // Creazione oggetto identity per il documento da modificare
                ObjectIdentity identity = DctmServices.Dfs4DocsPa.getDocumentoIdentityByDocNumber(schedaDocumento.docNumber);

                // Creazione DataObject
                DataObject dataObject = new DataObject(identity, ObjectTypes.DOCUMENTO);

                // Reperimento proprietà documentum
                Property[] properties = DctmServices.Dfs4DocsPa.getDocumentoProperties(schedaDocumento, this.InfoUtente, false);
                dataObject.Properties.Properties.AddRange(properties);

                DataPackage dataPackage = new DataPackage(dataObject);

                IObjectService service = this.GetServiceInstance<IObjectService>(true);

                dataPackage = service.Update(dataPackage, null);

                retValue = (dataPackage.DataObjects.Count > 0);

                if (retValue)
                    logger.Debug(string.Format("Documentum.SalvaDocumento: salvato documento con docnumber {0}", schedaDocumento.docNumber));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.SalvaDocumento:\n{0}", ex.ToString()));
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="sede"></param>
        /// <param name="risultatoProtocollazione"></param>
        /// <returns></returns>
        public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione)
        {
            DocsPaVO.utente.Ruolo[] ruoliSuperiori;
            return this.CreateProtocollo(schedaDocumento, ruolo, out risultatoProtocollazione, out ruoliSuperiori);
        }

        /// <summary>
        /// Operazione per la creazione di un nuovo protocollo
        /// 
        /// PreCondizioni:
        ///     - Oggetto SchedaDocumento valido, completo
        ///       degli attributi e degli oggetti significativi per il protocollo 
        ///       (es. numero, dataProtocollo, segnatura, oggetto "Protocollo")
        ///     - Il documento deve essere già protocollato
        /// PostCondizioni:
        ///     - Operazioni di aggiornamento dati documento in "Documentum" 
        ///       effettuate con successo
        /// 
        /// </summary>
        /// <param name="schedaDocumento">Oggetto contenente i metadati del documento da protocollare</param>
        /// <param name="ruolo">Ruolo dell'utente che protocolla il documento</param>
        /// <param name="risultatoProtocollazione">Parametro output: Esito più dettagliato dell'operazione</param>
        /// <returns>Boolean, l'operazione di protocollazione è andata a buon fine o meno</returns>
        public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null)
        {
            // Scheda documento in input (attributi e oggetti valorizzati):
            //      schedaDocumento.oggetto     --> obbligatorio
            //      schedaDocumento.tipoProto   --> obbligatorio
            //          tipologia del documento, può essere:
            //                  - A: Protocollo in arrivo
            //                  - P: Protocollo in uscita
            //                  - I: Protocollo interno
            //              Negli ultimi 3 casi, l'istanza dell'attributo "Protocollo"
            //              di SchedaDocumento può essere:
            //                  - ProtocolloEntrata
            //                  - ProtocolloUscita
            //                  - ProtocolloInterno
            //              In caso di documento grigio, l'oggetto "Protocollo" non è sicuramente istanziato
            //      schedaDocumento.protocollatore  --> obbligatorio
            //          utente che protocolla il documento (non necessariamente chi lo ha creato)
            //          in breve contiene la user id e il ruolo
            //      schedaDocumento.protocollo --> obbligatorio
            //          fornisce i dati del protocollo creato
            //      schedaDocumento.registro --> obbligatoroi
            //          registro di protocollo
            //
            // Scheda documento in output (attributi e oggetti valorizzati):
            //      nessuna modifica

            ruoliSuperiori = null;
            bool retValue = false;

            //// Controllo sulla tipologia di documento (non può essere grigio)
            //if (schedaDocumento.tipoProto == "G")
            //{
            //    risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;
            //}
            //else if (schedaDocumento.registro == null)
            //{
            //    risultatoProtocollazione = ResultProtocollazione.REGISTRO_MANCANTE;
            //}
            //else if (schedaDocumento.registro.stato == "C")
            //{
            //    risultatoProtocollazione = ResultProtocollazione.REGISTRO_CHIUSO;
            //}
            //else
            //{
                retValue = this.CreateDocumentoGrigio(schedaDocumento, ruolo);

                if (retValue)
                    risultatoProtocollazione = ResultProtocollazione.OK;
                else
                    risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;
            //}

            return retValue;
        }

        /// <summary>
        /// Annullamento di un protocollo
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="protocolloAnnullato"></param>
        /// <returns></returns>
        public bool AnnullaProtocollo(ref DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.ProtocolloAnnullato protocolloAnnullato)
        {
            bool retValue = false;

            try
            {
                bool uffRefSaved;
                retValue = this.SalvaDocumento(schedaDocumento, false, out uffRefSaved);

                if (retValue)
                    logger.Debug(string.Format("Documentum.AnnullaProtocollo: annullato protocollo con segnatura {0}", schedaDocumento.protocollo.segnatura));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.AnnullaProtocollo:\n{0}", ex.ToString()));
            }

            return retValue;
        }

        /// <summary>
        /// Predisposizione di un documento alla protocollazione
        /// 
        /// PreCondizioni:
        ///     Il documento deve essere già stato predisposto alla protocollazione
        ///     nelle entità di DocsPa
        /// 
        /// PostCondizioni:
        ///     In Documentum, il documento deve risultare predisposto alla protocollazione
        ///     Nessun attributo deve essere modificato nella SchedaDocumento
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public bool PredisponiProtocollazione(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            bool retValue = false;

            try
            {
                bool uffRefSaved;
                retValue = this.SalvaDocumento(schedaDocumento, false, out uffRefSaved);
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.ProtocollaDocumentoPredisposto:\n{0}", ex.ToString()));
            }

            return retValue;
        }

        /// <summary>
        /// Protocollazione di un documento predisposto alla protocollazione
        /// 
        /// PreCondizioni:
        ///     Il documento deve essere già stato protocollato in DocsPa
        /// 
        /// PostCondizioni:
        ///     In Documentum, il documento deve risultare come protocollato
        ///     Nessun oggetto fornito in ingresso deve essere modificato
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruolo"></param>
        /// <param name="risultatoProtocollazione"></param>
        /// <returns></returns>
        public bool ProtocollaDocumentoPredisposto(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione)
        {
            bool retValue = false;
            risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;

            try
            {
                bool uffRefSaved;
                retValue = this.SalvaDocumento(schedaDocumento, false, out uffRefSaved);

                if (retValue)
                    risultatoProtocollazione = ResultProtocollazione.OK;
                else
                    risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;
            }
            catch (Exception ex)
            {
                retValue = false;
                risultatoProtocollazione = ResultProtocollazione.APPLICATION_ERROR;

                logger.Debug(string.Format("Errore in Documentum.ProtocollaDocumentoPredisposto:\n{0}", ex.ToString()));
            }

            return retValue;
        }


        /// <summary>
        /// Operazione per l'inserimento dei metadati di una nuova versione di un documento
        /// 
        /// PreCondizioni:
        ///     la versione / allegato deve essere già stata inserita come entità in docspa
        ///  
        /// PostCondizioni:
        ///     i metadati della versione / allegato devono essere stati inseriti in documentum
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="daInviare"></param>
        /// <returns></returns>
        public bool AddVersion(DocsPaVO.documento.FileRequest fileRequest, bool daInviare)
        {
            logger.Info("BEGIN");
            bool retValue = false;

            bool checkedOut = false;
            IVersionControlService service = null;
            ObjectIdentitySet identitySet = null;

            try
            {
                if (daInviare)
                {
                    retValue = true;
                }
                else
                {
                    // Verifica se la versione esista già per il documento
                    if (Dfs4DocsPa.containsVersion(fileRequest.docNumber, fileRequest.version, null))
                    {
                        logger.Debug(string.Format("Versione '{0}' già esistente per documento con id '{1}'", fileRequest.version, fileRequest.docNumber));

                        // Versione già esistente
                        retValue = false;
                    }
                    else
                    {
                        ObjectIdentity identity = null;

                        // Verifica se il documento è di tipo stampa registro
                        bool isStampaRegistro = (DocsPaQueryHelper.isStampaRegistro(fileRequest.docNumber));

                        // Reperimento identity del documento da bloccare
                        if (isStampaRegistro)
                            identity = DctmServices.Dfs4DocsPa.getDocumentoStampaRegistroIdentityByDocNumber(fileRequest.docNumber);
                        else
                            identity = DctmServices.Dfs4DocsPa.getDocumentoIdentityByDocNumber(fileRequest.docNumber);

                        identitySet = new ObjectIdentitySet();
                        identitySet.Identities.Add(identity);
                        service = this.GetServiceInstance<IVersionControlService>(false);

                        DataPackage dataPackage = service.Checkout(identitySet, null);

                        if (dataPackage.DataObjects.Count == 1)
                        {
                            checkedOut = true;

                            // Reperimento ObjectId della versione in checkout
                            DataObject dataObject = dataPackage.DataObjects[0];

                            // Impostazione properties
                            if (isStampaRegistro)
                                dataObject.Properties.Properties.Add(new StringProperty(TypeDocumentoStampaRegistro.NUMERO_VERSIONE, fileRequest.version));
                            else
                                dataObject.Properties.Properties.Add(new StringProperty(TypeDocumento.NUMERO_VERSIONE, fileRequest.version));

                            // CheckIn del documento
                            dataPackage = service.Checkin(dataPackage, VersionStrategy.IMPLIED, true, null, null);

                            // Workaround:
                            // in presenza di allegati, il checkin non rilascia il blocco
                            // su questi ultimi. Per ovviare a quest'inconveniente, 
                            // si è costretti ad inviare un messaggio "CancelCheckout" dopo aver
                            // fatto checkin (con parametro "isRetainLock" = true)
                            service.CancelCheckout(new ObjectIdentitySet(identity));

                            retValue = (dataPackage.DataObjects.Count == 1);

                            if (retValue)
                                logger.Debug(string.Format("Documentum.AddVersion: creata versionid {0} per docnumento con docnumber {0}", fileRequest.versionId, fileRequest.docNumber));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.AddVersion:\n{0}", ex.ToString()));

                if (checkedOut)
                    // Annullamento del checkout
                    service.CancelCheckout(identitySet);
            }
            logger.Info("END");
            return retValue;
        }

        /// <summary>
        /// Operazione per l'inserimento di un file in una versione / allegato di un documento
        /// 
        /// PreCondizione:
        ///     la versione / allegato deve essere già stato creato come entità persistente
        /// 
        ///     Attributi dell'oggetto FileRequest già valorizzati in ingresso
        ///         VersionId:      ID della versione / allegato in docspa
        ///         Version:        0 se allegato, > 0 se versione
        ///         SubVersion:     ! se allegato, stringa vuota se versione
        ///         VersionLabel:   A# se allegato (dove # è il num. dell'allegato, fino a 99)
        ///                         (es. A01, A02, A03, ecc.)
        ///                         1, 2, 3, 4, ecc. se versione
        /// PostCondizioni:
        ///     il file deve essere stato associato alla versione / allegato
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="fileDocumento"></param>
        /// <param name="estensione"></param>
        /// <param name="objSicurezza"></param>
        /// <returns></returns>
        public bool PutFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocumento, string estensione)
        {
            logger.Info("BEGIN");
            bool retValue = false;

            try
            {
                IQueryService queryService = this.GetServiceInstance<IQueryService>(false);

                int indexOf = estensione.LastIndexOf(".");
                if (indexOf > -1)
                    estensione = estensione.Substring(indexOf);

                // Reperimento FileFormat documentum
                string ext = System.IO.Path.GetExtension(fileRequest.fileName);

                string fileFormat = DctmServices.DfsHelper.getDctmFileFormat(queryService, ext);
                BinaryContent content = new BinaryContent(fileDocumento.content, fileFormat);

                ObjectIdentity identity = null;
                string objectType = string.Empty;

                if (DocsPaQueryHelper.isStampaRegistro(fileRequest.docNumber))
                {
                    // Reperimento oggetto identity per il documento stampa registro
                    identity = DctmServices.Dfs4DocsPa.getDocumentoStampaRegistroIdentityByDocNumber(fileRequest.docNumber);
                    objectType = ObjectTypes.DOCUMENTO_STAMPA_REGISTRO;
                }
                else
                {
                    // Reperimento oggetto identity per il documento
                    identity = DctmServices.Dfs4DocsPa.getDocumentoIdentityByDocNumber(fileRequest.docNumber);
                    objectType = ObjectTypes.DOCUMENTO;
                }

                DataObject dataObject = new DataObject(identity, objectType);
                dataObject.Contents.Add(content);
                
                OperationOptions opts = new OperationOptions();
                opts.ContentTransferProfile = new ContentTransferProfile();

                DataPackage dataPackage = new DataPackage(dataObject);

                //Se pec, allora superadmin
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                string PEC = doc.GeIsAllegatoPEC(fileRequest.versionId);
                bool superadmin = false;
                if (!string.IsNullOrEmpty(PEC))
                    superadmin = true;

                IObjectService objectService = this.GetServiceInstance<IObjectService>(superadmin);
                dataPackage = objectService.Update(dataPackage, opts);


                //IObjectService objectService = this.GetServiceInstance<IObjectService>(false);
                //dataPackage = objectService.Update(dataPackage, opts);


                retValue = (dataPackage.DataObjects.Count == 1);

                if (retValue)
                {
                    // Al termine del metodo

                    // if inserito per permettere la memorizzazione corretta del path nella components in caso di PitreDualFileWritingMode=false
                    if (!fileRequest.fileName.ToUpper().EndsWith("P7M".ToUpper()) && string.IsNullOrEmpty(fileDocumento.fullName))
                    {
                        fileRequest.fileName = fileDocumento.fullName;
                        fileRequest.fileSize = fileDocumento.content.Length.ToString();
                    }

                    logger.Debug(string.Format("Documentum.PutFile: inviato file di dimensione {0} per documento con docnumber {0} e versionid {0}", fileDocumento.content.Length.ToString(), fileRequest.versionId, fileRequest.docNumber, fileRequest.versionId));
                }
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.PutFile:\n{0}", ex.ToString()));
            }
            logger.Info("END");
            return retValue;
        }

        /// <summary>
        /// Reperimento (semplificato) del contenuto di un file associato ad una versione / allegato.
        /// 
        /// PreCondizioni:
        ///     La versione / allegato deve essere già stato creata come entità persistente
        ///     Il file deve essere già acquisito nel repository documentale
        /// 
        /// PostCondizioni:
        ///     Restituzione del solo contenuto del file dal documentale Documentum
        /// 
        /// </summary>
        /// <remarks>Se almeno uno dei parametri forniti in ingresso non è valido, viene restituito null</remarks>
        /// <param name="docNumber"></param>
        /// <param name="version"></param>
        /// <param name="versionId"></param>
        /// <param name="versionLabel"></param>
        /// <returns></returns>
        public byte[] GetFile(string docNumber, string version, string versionId, string versionLabel)
        {
            // Verifica se il file è di tipo stampa registro
            bool isStampaRegistro = DocsPaQueryHelper.isStampaRegistro(docNumber);

            return this.GetFile(docNumber, version, versionId, versionLabel, isStampaRegistro);
        }

        /// <summary>
        /// Operazione per il reperimento di un file di una versione / allegato di un documento
        /// 
        /// PreCondizioni:
        ///     la versione / allegato deve essere già stato creato come entità persistente
        ///     il file deve essere già acquisito nel repository documentale
        /// 
        /// PostCondizioni:
        ///     i seguenti attributi dell'oggetto "FileDocumento" devono essere inizializzati:
        ///     - name              (nome "fittizio" del file con estensione)
        ///     - estensioneFile    (estensione del file)
        ///     - content           (array di byte, contenuto del file)
        ///     - contentType       (mimetype del file)
        ///     - length            (dimensione dell'array di byte)
        /// </summary>
        /// <param name="fileDocumento"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public bool GetFile(ref DocsPaVO.documento.FileDocumento fileDocumento, ref DocsPaVO.documento.FileRequest fileRequest)
        {
            logger.Info("BEGIN");
            bool retValue = false;

            try
            {
                // Verifica se il file è di tipo stampa registro
                bool isStampaRegistro = DocsPaQueryHelper.isStampaRegistro(fileRequest.docNumber);

                // Reperimento contenuto del file
                fileDocumento.content = this.GetFile(fileRequest.docNumber, fileRequest.version, fileRequest.versionId, fileRequest.versionLabel, isStampaRegistro);

                retValue = (fileDocumento.content != null);

                if (retValue)
                {
                    fileDocumento.name = System.IO.Path.GetFileName(fileRequest.fileName);
                    fileDocumento.length = fileDocumento.content.Length;

                    logger.Debug(string.Format("Documentum.GetFile: reperimento file di dimensione {0} per documento con docnumber {0} e versionid {0}", fileDocumento.content.Length.ToString(), fileRequest.versionId, fileRequest.docNumber, fileRequest.versionId));
                }
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.GetFile:\n{0}", ex.ToString()));
            }
            logger.Info("END");
            return retValue;
        }

        /// <summary>
        /// Inserimento dei soli metadati di un nuovo allegato per un documento
        /// 
        /// PreCondizioni:
        ///     L'entità allegato deve essere già stata aggiunta in DocsPa
        /// 
        /// PostCondizioni:
        ///     I dati utilizzati da Documentum devono essere stati salvati
        /// 
        /// </summary>
        /// <param name="allegato"></param>
        /// <param name="putfile"></param>
        /// <returns></returns>
        public bool AddAttachment(DocsPaVO.documento.Allegato allegato, string putfile)
        {
            bool retValue = false;

            bool hasCreated = false;

            try
            {
                if (!string.IsNullOrEmpty(putfile) && putfile == "Y")
                {
                    // Se putfile == "Y", l'operazione si presume
                    // come andata a buon fine in quanto nessun
                    // attributo è da modificare in documentum
                    retValue = true;
                }
                else
                {
                    // Reperimento del docnumber relativo al documento principale che contiene l'allegato
                    string docNumber = DocsPaQueryHelper.getDocNumberDocumentoPrincipale(allegato.docNumber);
                    
                    // Creazione di un nuovo dataobject per l'allegato
                    ObjectIdentity identityAllegato = new ObjectIdentity(DctmConfigurations.GetRepositoryName());
                    DataObject dataObjectAllegato = new DataObject(identityAllegato, ObjectTypes.DOCUMENTO);

                    // Impostazione properties per l'allegato
                    dataObjectAllegato.Properties.Properties.AddRange(Dfs4DocsPa.getAllegatoProperties(allegato, this.InfoUtente, true));
                     
                    // Associazione ACL del documento di appartenenza all'allegato e al VDoc
                    CustomServices.AclDefinition aclDefinition = this.GetAclDefinitionAllegato(docNumber);
                    AclHelper.setAclObjectProperties(dataObjectAllegato.Properties, aclDefinition);

                    DataPackage dataPackage = new DataPackage(dataObjectAllegato);

                    // NB: l'oggetto allegato viene creato con le credenziali di superuser in quanto
                    // è possibile che chi lo crei non sia lo stesso utente che ha creato il documento principale.
                    IObjectService objectService = this.GetServiceInstance<IObjectService>(true);
                  //dataPackage = objectService.Update(dataPackage, null);  //Vecchio 6.0
                    dataPackage = objectService.Create(dataPackage, null);

                    hasCreated = (dataPackage.DataObjects.Count > 0);

                    // NB: Stranamente, non si riesce a fare l'aggiornamento della relazione
                    // al virtual document per l'allegato insieme agli altri aggiornamenti,
                    // probabilmante per un bug di dctm.
                    // Come workaround, la relation viene creata solo dopo che 
                    // il doc è stato modificato in virtual document e l'allegato è stato inserito
                    if (hasCreated)
                    {
                        dataObjectAllegato = dataPackage.DataObjects[0];

                        // Reperimento identity del documento principale
                        ObjectIdentity identityVDoc = Dfs4DocsPa.getDocumentoIdentityByDocNumber(this.GetServiceInstance<IQueryService>(true), docNumber);

                        try
                        {
                            // Impostazione del documento come virtuale
                            //this.SetVirtualDocumentAttribute(((ObjectId)identityVDoc.Value), true);


                            // Aggiunta dell'allegato al documento virtuale
                            //this.AddChildToVirtualDocument(((ObjectId)dataObjectAllegato.Identity.Value),
                            //                              ((ObjectId)identityVDoc.Value));


                            // Impostazione del documento come virtuale
                            //this.SetVirtualDocumentAttribute(((ObjectId)dataObjectVDoc.Identity.Value), true);

                            //// Impostazione relazioni dell'allegato con il virtual document
                            //dataObjectAllegato.Relationships.Add(new ReferenceRelationship(dataObjectVDoc.Identity, Relationship.VIRTUAL_DOCUMENT_RELATIONSHIP, Relationship.ROLE_PARENT));

                            //dataPackage = new DataPackage();
                            //dataPackage.AddDataObject(dataObjectAllegato);
                            //dataPackage = objectService.Update(dataPackage, null);

                            //retValue = (dataPackage.DataObjects.Count > 0);

                            //if (retValue)
                            //    logger.Debug(string.Format("Documentum.AddAttachment: inserito allegato con versionid {0} per documento con docnumber {0}", allegato.versionId, allegato.docNumber));
                            //else
                            //    // undo delle modifiche, cancellazione dell'allegato appena inserito
                            //    objectService.Delete(new ObjectIdentitySet(dataObjectVDoc.Identity), null);

                            retValue = true;
                               
                            logger.Debug(string.Format("Documentum.AddAttachment: inserito allegato con versionid {0} per documento con docnumber {0}", allegato.versionId, allegato.docNumber));
                        }
                        catch (Exception ex)
                        {
                            // undo delle modifiche, cancellazione dell'allegato appena inserito
                            objectService.Delete(new ObjectIdentitySet(dataObjectAllegato.Identity), null);

                            throw ex;
                        }
                    }
                    else
                    {
                        throw new ApplicationException(string.Format("Allegato con id {0} non inserito", allegato.versionId));
                    }
                }
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.AddAttachment:\n{0}", ex.ToString()));
            }

            return retValue;
        }

        /// <summary>
        /// Modifica dei dati di un allegato
        /// 
        /// PreCondizioni:
        ///     L'entità allegato deve essere già stata modificata in DocsPa 
        /// 
        /// PostCondizioni:
        ///     I dati utilizzati da Documentum devono essere stati salvati
        /// </summary>
        /// <param name="allegato"></param>
        public void ModifyAttatchment(DocsPaVO.documento.Allegato allegato)
        {
            try
            {
                // Reperimento oggetto identity per l'allegato da modificare
                ObjectIdentity identity = DctmServices.Dfs4DocsPa.getDocumentoIdentityByDocNumber(allegato.docNumber);

                DataObject dataObject = new DataObject(identity, ObjectTypes.DOCUMENTO);

                // Impostazione proprietà aggiornate per l'allegato
                dataObject.Properties.Properties.AddRange(DctmServices.Dfs4DocsPa.getAllegatoProperties(allegato, this.InfoUtente, false));

                DataPackage dataPackage = new DataPackage(dataObject);

                IObjectService objectService = this.GetServiceInstance<IObjectService>(true);
                dataPackage = objectService.Update(dataPackage, null);

                if (dataPackage.DataObjects.Count == 1)
                    logger.Debug(string.Format("Documentum.ModifyAttatchment: modificato allegato con versionid {0} per documento con docnumber {0}", allegato.versionId, allegato.docNumber));
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore in Documentum.ModifyAttatchment:\n{0}", ex.ToString()));
            }
        }



        /// <summary>
        /// Rimozione di un allegato
        /// </summary>
        /// <param name="allegato"></param>
        /// <returns></returns>
        public bool RemoveAttatchment(DocsPaVO.documento.Allegato allegato)
        {
            return this.RemoveAttatchment(allegato.docNumber);
        }

        /// <summary>
        /// Operazione per la rimozione dei metadati di una versione / allegato di un documento
        /// 
        /// PreCondizioni:
        ///     la versione / allegato deve essere già stata rimossa come entità di un documento
        ///     
        /// PostCondizioni:
        ///     i metadati della versione / allegato devono essere rimossi da documentum
        /// 
        /// <remarks>
        ///     In caso di rimozione di una versione, può essere rimossa solamente la versione corrente
        /// </remarks>
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public bool RemoveVersion(DocsPaVO.documento.FileRequest fileRequest)
        {
            bool retValue = false;

            try
            {
                // Reperimento identity del documento
                IQueryService queryService = this.GetServiceInstance<IQueryService>(false);
                ObjectIdentity identity = Dfs4DocsPa.getDocumentoIdentityByDocNumber(queryService, fileRequest.docNumber, fileRequest.version);

                if (DocsPaQueryHelper.isAllegatoProfilato(fileRequest.docNumber))
                {
                    // Se si richiede la rimozione di una versione di un allegato, 
                    // viene dapprima richiamato il servizio custom per la gestione
                    // dei Virtual Documents, in particolare per rimuovere solo la relazione
                    // tra documento parent e documento child.
                    // Come step successivo, viene utilizzato sempre il VersionControlService 
                    // per rimuovere fisicamente la versione.

                    string docNumberDocumentoPrincipale = string.Empty;

                    using (DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti())
                        docNumberDocumentoPrincipale = DocsPaQueryHelper.getDocNumber(documentiDb.GetIdDocumentoPrincipale(fileRequest.docNumber));

                    ObjectIdentity parentDocumentIdentity = Dfs4DocsPa.getDocumentoIdentityByDocNumber(queryService, docNumberDocumentoPrincipale);

                    // Rimozione della versione dell'allegato
                    this.RemoveChildToVirtualDocument((ObjectId)identity.Value, (ObjectId)parentDocumentIdentity.Value, false);
                }

                IVersionControlService versionControl = this.GetServiceInstance<IVersionControlService>(false);
                versionControl.DeleteVersion(new ObjectIdentitySet(identity));

                retValue = true;

                logger.Debug(string.Format("Documentum.RemoveVersion: rimossa versione {0} per documento con docnumber {0}", fileRequest.versionId, fileRequest.docNumber));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.RemoveVersion:\n{0}", ex.ToString()));
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="docNumber"></param>
        /// <param name="version_id"></param>
        /// <param name="version"></param>
        /// <param name="subVersion"></param>
        /// <param name="versionLabel"></param>
        /// <returns></returns>
        public bool ModifyExtension(ref DocsPaVO.documento.FileRequest fileRequest, string docNumber, string version_id, string version, string subVersion, string versionLabel)
        {
            throw new NotImplementedException("Operazione 'ModifyExtension' non implementata in documentum");
        }

        /// <summary>
        /// Modifica dei metadati di una versione
        /// </summary>
        /// <param name="fileReq"></param>
        public void ModifyVersion(DocsPaVO.documento.FileRequest fileReq)
        {
            throw new NotImplementedException("Operazione 'ModifyVersion' non implementata in documentum");
        }

        /// <summary>
        /// Setta a 1 cha_segnatura se la versione contiene un documento in formato pdf, con segnatura impressa
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns>
        /// bool che indica l'esito dell'operazione di update
        /// </returns>
        public bool ModifyVersionSegnatura(string versionId)
        {
            throw new NotImplementedException("Operazione 'ModifyVersionSegnatura' non implementata in documentum");
        }

        /// <summary>
        /// Informa se la versione ha associato un file con impressa la segnatura
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns>
        /// bool che indica se la versione ha associato un file con impressa segnatura o meno
        /// </returns>
        public bool IsVersionWithSegnature(string versionId)
        {
            throw new NotImplementedException("Operazione 'IsVersionWithSegnature' non implementata in documentum");
        }

        /// <summary>
        /// Reperimento dell'ultimo id di versione di un documento
        /// 
        /// IMPORTANTE: 
        /// L'operazione può non essere implementata in quanto non è 
        /// documentum che assegna il docNumber ma DocsPa.
        /// In tal caso non verrà richiamata dallo strato PITRE.
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public string GetLatestVersionId(string docNumber)
        {
            throw new NotImplementedException("Operazione 'GetLatestVersionId' non implementata in documentum");
        }

        /// <summary>
        /// Reperimento dell'estensione del file associato al documento e alla versione richiesta
        /// 
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="versionid"></param>
        /// <returns></returns>
        public string GetFileExtension(string docnumber, string versionid)
        {
            throw new NotImplementedException("Operazione 'GetFileExtension' non implementata in documentum");
        }

        /// <summary>
        /// Reperimento del nome del file presente nel repository del documentale
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="versionid"></param>
        /// <returns></returns>
        public string GetOriginalFileName(string docnumber, string versionid)
        {
            throw new NotImplementedException("Operazione 'GetOriginalFileName' non implementata in documentum");
        }

        /// <summary>
        /// Impostazione della visibilità su un documento 
        /// (e dell'ownership, nel caso l'utente / ruolo rimosso fosse il proprietario)
        /// </summary>
        /// <param name="infoDiritto"></param>
        /// <returns></returns>
        public bool AddPermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            bool added = false;

            // se si revocano i diritti all'utente proprietario, 
            // si revocano anche al ruolo proprietario e viceversa. 
            // Il proprietario del documento diventa l'utente e il ruolo del revocante

            try
            {
                // Reperimento utente e ruolo proprietario
                string ownerUser = TypeUtente.NormalizeUserName(DocsPaQueryHelper.getCodiceRubricaProprietario(infoDiritto.idObj, true));
                string ownerRole = TypeGruppo.NormalizeGroupName(DocsPaQueryHelper.getCodiceRubricaProprietario(infoDiritto.idObj, false));

                if (this.SetOwnershipDocumento(infoDiritto.idObj, ownerUser, ownerRole))
                {
                    CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());

                    CustomServices.AclDefinition aclDefinition = Dfs4DocsPa.getAclDefinition(infoDiritto.idObj, ObjectTypes.DOCUMENTO, this.InfoUtente);

                    aclService.ClearAndGrant(aclDefinition, infoDiritto.idObj);
                }

                added = true;
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
        public bool RemovePermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            bool removed = false;

            try
            {
                // Reperimento utente e ruolo proprietario
                string ownerUser = TypeUtente.NormalizeUserName(DocsPaQueryHelper.getCodiceRubricaProprietario(infoDiritto.idObj, true));
                string ownerRole = TypeGruppo.NormalizeGroupName(DocsPaQueryHelper.getCodiceRubricaProprietario(infoDiritto.idObj, false));

                if (this.SetOwnershipDocumento(infoDiritto.idObj, ownerUser, ownerRole))
                {
                    CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());

                    CustomServices.AclDefinition aclDefinition = Dfs4DocsPa.getAclDefinition(infoDiritto.idObj, ObjectTypes.DOCUMENTO, this.InfoUtente);

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
        /// Scambia il file associato ad un allegato con il file associato ad un documento
        /// </summary>
        /// <param name="allegato"></param>
        /// <param name="documento"></param>
        /// <returns></returns>
        public bool ScambiaAllegatoDocumento(DocsPaVO.documento.Allegato allegato, DocsPaVO.documento.Documento documento)
        {
            bool retValue = false;
            
            try
            {
                IObjectService objectService = this.GetServiceInstance<IObjectService>(false);
                
                List<ObjectContentSet> contentList = null;

                Content contentAllegato = null;

                if (this.IsFileAcquired(allegato))
                {
                    // Reperimento contentlist associati all'allegato
                    contentList = objectService.GetObjectContentUrls(new ObjectIdentitySet(Dfs4DocsPa.getDocumentoIdentityByDocNumber(allegato.docNumber)));
                    
                    if (contentList.Count == 1)
                        contentAllegato = contentList[0].Contents[0];
                }

                Content contentDocumento = null;

                if (this.IsFileAcquired(documento))
                {
                    // Reperimento contentlist associati al documento
                    contentList = objectService.GetObjectContentUrls(new ObjectIdentitySet(Dfs4DocsPa.getDocumentoIdentityByDocNumber(documento.docNumber)));
                    
                    if (contentList.Count == 1)
                        contentDocumento = contentList[0].Contents[0];
                }

                if (contentAllegato != null || contentDocumento != null)
                {
                    // Creazione di un oggetto DataObject relativo all'allegato
                    DataObject dataObjectAllegato = new DataObject(Dfs4DocsPa.getDocumentoIdentityByDocNumber(allegato.docNumber));
                    dataObjectAllegato.Contents.Add(contentDocumento);

                    // Creazione di un oggetto DataObject relativo all'allegato
                    DataObject dataObjectDocumento = new DataObject(Dfs4DocsPa.getDocumentoIdentityByDocNumber(documento.docNumber));
                    dataObjectDocumento.Contents.Add(contentAllegato);

                    DataPackage dataPackage = new DataPackage();
                    dataPackage.AddDataObject(dataObjectAllegato);
                    dataPackage.AddDataObject(dataObjectDocumento);
                    dataPackage = objectService.Update(dataPackage, null);

                    retValue = (dataPackage.DataObjects.Count > 0);
                }
                else
                    retValue = false;
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.ScambiaAllegatoDocumento:\n{0}", ex.ToString()));
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
        /// Reperimento del ruolo o utente creatore di un documento in DCTM
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="ownerRole"></param>
        /// <param name="ownerUser"></param>
        /// <returns></returns>
        protected virtual void GetOwnershipDocumento(string idProfile, out string ownerUser, out string ownerRole)
        {
            ownerUser = DocsPaQueryHelper.getCodiceRubricaProprietario(idProfile, true);
            ownerRole = DocsPaQueryHelper.getCodiceRubricaProprietario(idProfile, false);
        }

        /// <summary>
        /// Impostazione dell'ownership sul documento e di tutti gli eventuali allegati
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="ownerUser"></param>
        /// <param name="ownerRole"></param>
        /// <returns></returns>
        public virtual bool SetOwnershipDocumento(string idProfile, string ownerUser, string ownerRole)
        {
            bool retValue = false;

            try
            {
                // Impostazione dell'ownership del documento, con le credenziali da superamministratore
                Dfs4DocsPa.setOwnershipDocumento(idProfile, ownerUser, ownerRole, this.GetServiceInstance<IQueryService>(true));

                retValue = true;
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.SetOwnershipDocumento:\n{0}", ex.ToString()));
            }

            return retValue;
        }

        /// <summary>
        /// Verifica l'esistenza di un documento
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public virtual bool ContainsDocumento(string docNumber)
        {
            return Dfs4DocsPa.containsDocumento(docNumber, this.GetServiceInstance<IQueryService>(true));
        }

        /// <summary>
        /// Verifica l'esistenza di un documento di tipo StampaRegistro
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public virtual bool ContainsDocumentoStampaRegistro(string docNumber)
        {
            return Dfs4DocsPa.containsDocumentoStampaRegistro(docNumber, this.GetServiceInstance<IQueryService>(true));
        }

        /// <summary>
        /// Reperimento (semplificato) del contenuto di un file associato ad una versione / allegato.
        /// 
        /// PreCondizioni:
        ///     La versione / allegato deve essere già stato creata come entità persistente
        ///     Il file deve essere già acquisito nel repository documentale
        /// 
        /// PostCondizioni:
        ///     Restituzione del solo contenuto del file dal documentale Documentum
        /// 
        /// </summary>
        /// <remarks>Se almeno uno dei parametri forniti in ingresso non è valido, viene restituito null</remarks>
        /// <param name="docNumber"></param>
        /// <param name="version"></param>
        /// <param name="versionId"></param>
        /// <param name="versionLabel"></param>
        /// <param name="isStampaRegistro"></param>
        /// <returns></returns>
        protected virtual byte[] GetFile(string docNumber, string version, string versionId, string versionLabel, bool isStampaRegistro)
        {
            logger.Info("BEGIN");
            byte[] contentByteArray = null;

            try
            {
                ObjectIdentity identity = null;
                string objectType = string.Empty;

                if (isStampaRegistro)
                {
                    // Reperimento oggetto identity per il documento di tipo stampa registro
                    identity = DctmServices.Dfs4DocsPa.getDocumentoStampaRegistroIdentityByDocNumber(docNumber);
                }
                else
                {
                    // Reperimento oggetto identity per il documento
                    identity = DctmServices.Dfs4DocsPa.getDocumentoIdentityByDocNumber(docNumber, version);
                }

                ObjectIdentitySet identitySet = new ObjectIdentitySet(identity);

                IObjectService objectService = this.GetServiceInstance<IObjectService>(true);
                List<ObjectContentSet> contentList = objectService.GetObjectContentUrls(identitySet);

                if (contentList.Count == 1)
                {
                    Content content = contentList[0].Contents[0];

                    content.ContentTransferMode = ContentTransferMode.MTOM;
                    content.contentTransferModeSpecified = true;

                    contentByteArray = content.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                contentByteArray = null;

                logger.Debug(string.Format("Errore in Documentum.GetFile:\n{0}", ex.ToString()));
            }
            logger.Info("END");
            return contentByteArray;
        }

        /// <summary>
        /// Aggiunta di un documento figlio ad un virtual document
        /// </summary>
        /// <param name="objectIdChild"></param>
        /// <param name="objectIdParent"></param>        
        protected virtual void AddChildToVirtualDocument(ObjectId objectIdChild, ObjectId objectIdParent)
        {
            try
            {
                DctmServices.Custom.IDocumentService service = DctmServiceFactory.GetCustomServiceInstance<DctmServices.Custom.IDocumentService>(UserManager.ImpersonateSuperUser());
                
                service.AddChild(DctmConfigurations.GetRepositoryName(), objectIdChild.Id, objectIdParent.Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Errore in Documentum.AddChildToVirtualDocument:\n{0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Rimozione di un documento figlio da un virtual document
        /// </summary>
        /// <param name="objectIdChild"></param>
        /// <param name="objectIdParent"></param>      
        /// <param name="destroyChild">
        /// Se true, indica di rimuovere fisicamente il documento child,
        /// altrimenti viene rimossa solamente la relazione che lo lega con il parent
        /// </param>
        protected virtual void RemoveChildToVirtualDocument(ObjectId objectIdChild, ObjectId objectIdParent, bool destroyChild)
        {
            try
            {
                DctmServices.Custom.IDocumentService service = DctmServiceFactory.GetCustomServiceInstance<DctmServices.Custom.IDocumentService>(UserManager.ImpersonateSuperUser());

                service.RemoveChild(DctmConfigurations.GetRepositoryName(), objectIdChild.Id, objectIdParent.Id, destroyChild);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Errore in Documentum.RemoveChildToVirtualDocument:\n{0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Impostazione dell'attributo "r_is_virtual" per il documento
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="isVirtual"></param>
        protected virtual void SetVirtualDocumentAttribute(ObjectId objectId, bool isVirtual)
        {
            try
            {
                DctmServices.Custom.IDocumentService service = DctmServiceFactory.GetCustomServiceInstance<DctmServices.Custom.IDocumentService>(UserManager.ImpersonateSuperUser());

                service.SetIsVirtual(DctmConfigurations.GetRepositoryName(), objectId.Id, isVirtual);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Errore in Documentum.SetVirtualDocumentAttribute:\n{0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Impostazione visibilità documento,
        /// modifica dell'attributo "a_is_hidden" del documento
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        protected virtual bool SetDocumentVisibility(string idProfile, string docNumber, bool isVisible)
        {
            bool retValue = false;

            try
            {
                string value = string.Empty;
                if (isVisible)
                    value = "0";
                else
                    value = "1";

                string dqlTemplate = "UPDATE {0} OBJECT SET a_is_hidden = {1} WHERE {2} = '{3}'";

                string dql = string.Format(dqlTemplate,
                                            ObjectTypes.DOCUMENTO,
                                            value,
                                            TypeDocumento.DOC_NUMBER,
                                            docNumber);

                IQueryService queryService = this.GetServiceInstance<IQueryService>(true);
                DfsHelper.executePassThrough(queryService, dql);

                if (DocsPaQueryHelper.getCountAllegati(idProfile) > 0)
                {
                    dql = string.Format(dqlTemplate,
                                        ObjectTypes.DOCUMENTO,
                                        value,
                                        TypeDocumento.ID_DOCUMENTO_PRINCIPALE,
                                        docNumber);

                    DfsHelper.executePassThrough(queryService, dql);
                }

                retValue = true;
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.SetDocumentVisibility:\n{0}", ex.ToString()));
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se per un documento è stato acquisito un file
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        protected bool IsFileAcquired(FileRequest fileRequest)
        {
            return (!string.IsNullOrEmpty(fileRequest.fileName) &&
                    !string.IsNullOrEmpty(fileRequest.fileSize));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        protected bool AddDocumentoInCestino(string docNumber)
        {
            bool retValue = false;

            try
            {
                retValue = this.SetDocumentVisibility(DocsPaQueryHelper.getIdProfile(docNumber), docNumber, false);
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.AddDocumentoInCestino:\n{0}", ex.ToString()));
            }

            return retValue;
        }

        /// <summary>
        /// Rimozione di un allegato
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        protected virtual bool RemoveAttatchment(string docNumber)
        {
            bool retValue = false;

            try
            {
                // E' necessario prima rimuovere tutte le relation con il virtual document
                retValue = true;

                ////IQueryService queryService = this.GetServiceInstance<IQueryService>(false);
                
                ////// Reperimento identity del documento principale
                ////ObjectIdentity identityVdoc = Dfs4DocsPa.getDocumentoPrincipaleIdentity(docNumber, queryService);

                ////ObjectIdentity identity = DctmServices.Dfs4DocsPa.getDocumentoIdentityByDocNumber(queryService, docNumber);

                ////this.RemoveChildToVirtualDocument((ObjectId) identity.Value, (ObjectId) identityVdoc.Value);







                //DataObject dataObjectRemoveRel = new DataObject(identity);
                //dataObjectRemoveRel.Relationships.Add(new ReferenceRelationship(identityVdoc, Relationship.VIRTUAL_DOCUMENT_RELATIONSHIP, Relationship.ROLE_PARENT, RelationshipIntentModifier.REMOVE, null));

                //IObjectService objectService = DctmServices.DctmServiceFactory.GetServiceInstance<IObjectService>(this.InfoUtente.dst);

                //// L'unico modo per rimuovere le relazioni è tramite il metodo update
                //DataPackage dataPackageRemoveRel = new DataPackage(dataObjectRemoveRel);
                //dataPackageRemoveRel = objectService.Update(dataPackageRemoveRel, null);

                //// Rimozione di tutte le versioni del documento
                //OperationOptions opts = new OperationOptions();
                //opts.DeleteProfile = new DeleteProfile();
                //opts.DeleteProfile.versionStrategySpecified = true;
                //opts.DeleteProfile.VersionStrategy = DeleteVersionStrategy.ALL_VERSIONS;

                //objectService.Delete(new ObjectIdentitySet(identity), opts);

                retValue = true;

                logger.Debug(string.Format("Documentum.RemoveAttatchment: rimosso allegato con docnumber {0}", docNumber));
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug(string.Format("Errore in Documentum.RemoveAttatchment:\n{0}", ex.ToString()));
            }

            return retValue;
        }

        #endregion

        #region Private methods

        #endregion
    }
}