using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using DocsPaDocumentale.Interfaces;
using DocsPaUtils.LogsManagement;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.DataModel.Core;
using DocsPaDocumentale_DOCUMENTUM.DctmServices;
using DocsPaDocumentale_DOCUMENTUM.DocsPaServices;
using CustomServices = DocsPaDocumentale_DOCUMENTUM.DctmServices.Custom;
using log4net;

namespace DocsPaDocumentale_DOCUMENTUM.Documentale
{
    /// <summary>
    /// 
    /// </summary>
    public class AclEventListener : IAclEventListener
    {
        
        #region Ctros, variables, constants

        /// <summary>
        /// Instanziazione logger
        /// </summary>
        private ILog logger = LogManager.GetLogger(typeof(AclEventListener));

        /// <summary>
        /// 
        /// </summary>
        private DocsPaVO.utente.InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public AclEventListener(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Protected methods

        protected DocsPaVO.utente.InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Evento di creazione documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruoliSuperiori"></param>
        public void DocumentoCreatoEventHandler(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {}

        /// <summary>
        /// Handler dell'evento di avvenuta creazione di un fascicolo
        /// </summary>
        /// <param name="classificazione">Posizione nella gerarchia di titolario del fascicolo</param>
        /// <param name="fascicolo"></param>
        /// <param name="ruolo">Ruolo creatore del fascicolo</param>
        /// <param name="ruoliSuperiori">
        /// Riporta una lista di ruoli superiori al ruolo creatore
        /// che devono avere la visibilità sul fascicolo
        /// </param>
        public void FascicoloCreatoEventHandler(DocsPaVO.fascicolazione.Classificazione classificazione, DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        { }

        /// <summary>
        /// Handler dell'evento di avvenuta creazione di un sottofascicolo
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="ruolo">Ruolo creatore del sottofascicolo</param>
        /// <param name="ruoliSuperiori">
        /// Riporta una lista di ruoli superiori al ruolo creatore
        /// che devono avere la visibilità sul sottofascicolo
        /// </param>
        public void SottofascicoloCreatoEventHandler(DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {}

        /// <summary>
        /// Handler dell'evento di avvenuta trasmissione di un documento / fascicolo
        /// </summary>
        /// <param name="trasmissione"></param>
        /// <param name="infoSecurity">
        /// Oggetto contenente i metadati relativi alla tipologia di ACL
        /// impostata nella trasmissione di un documento / fascicolo
        /// </param>
        public void TrasmissioneCompletataEventHandler(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.trasmissione.infoSecurity[] infoSecurityList)
        {
            // In caso di trasmissione con cessione diritti, 
            // viene modificata l'ownership del documento / fascicolo trasmesso
            // se il documento era di proprietà del cedente
            if (trasmissione.cessione != null && trasmissione.cessione.docCeduto)
            {
                // imposta l'Ownership solo se è stato impostato il nuovo utente che diventerà proprietario del doc/fas.
                // Questo avviene quando si stanno cedendo i diritti di proprietà (P).
                // Se si stanno cedendo i diritti acquisiti (A o T o F) questo blocco viene saltato.
                if (trasmissione.cessione.idPeopleNewPropr != null && trasmissione.cessione.idPeopleNewPropr != "")
                {
                    IQueryService queryService = DctmServiceFactory.GetServiceInstance<IQueryService>(UserManager.ImpersonateSuperUser());

                    // Reperimento nome utente e ruolo cui viene impostata l'ownership del documento
                    string ownerUser = DocsPaObjectTypes.TypeUtente.NormalizeUserName(DocsPaQueryHelper.getCodiceUtente(trasmissione.cessione.idPeopleNewPropr));

                    if (trasmissione.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
                    {
                        string ownerRole = DocsPaObjectTypes.TypeGruppo.NormalizeGroupName(DocsPaQueryHelper.getCodiceRuoloFromIdGroups(trasmissione.cessione.idRuoloNewPropr));

                        Dfs4DocsPa.setOwnershipDocumento(trasmissione.infoDocumento.idProfile, ownerUser, ownerRole, queryService);
                    }
                    else if (trasmissione.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.FASCICOLO)
                    {
                        Dfs4DocsPa.setOwnershipFascicolo(trasmissione.infoFascicolo.idFascicolo, ownerUser, queryService);
                    }
                }
            }

            CustomServices.AclDefinition aclData = null;
            
            if (trasmissione.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
            {
                // Reperimento della lista delle ACL aggiornata nella tabella security di docspa
                aclData = Dfs4DocsPa.getAclDefinition(trasmissione.infoDocumento.docNumber, DocsPaObjectTypes.ObjectTypes.DOCUMENTO, this.InfoUtente);

                // Aggiornamento ACL in DCTM, con le credenziali di superuser
                CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());
                aclService.ClearAndGrant(aclData, trasmissione.infoDocumento.idProfile);
            }
            else if (trasmissione.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.FASCICOLO)
            {
                // Reperimento della lista delle ACL aggiornata nella tabella security di docspa
                aclData = Dfs4DocsPa.getAclDefinition(trasmissione.infoFascicolo.idFascicolo, DocsPaObjectTypes.ObjectTypes.FASCICOLO, this.InfoUtente);

                // Aggiornamento ACL in DCTM, con le credenziali di superuser
                CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());
                aclService.ClearAndGrant(aclData, trasmissione.infoFascicolo.idFascicolo);

                string[] idProfiles = DocsPaQueryHelper.getDocumentiInFascicolo(trasmissione.infoFascicolo.idFascicolo);
                //PORKING: nonè necessario farlo ora, perchè le aCL di Documenti vengono rimappate se necesario quando gli utenti o i ruoli faranno 
                //          GetDettaglio con il metedo RefreshAcl
                if (idProfiles != null && idProfiles.Length < 20)
                {
                    foreach (string idProfile in idProfiles)
                    {
                        // Aggiornamento delle ACL per tutti i documenti nel fascicolo (compresi eventuali sottofascicoli)
                        //DCTM è usato solo con PITRE e in PITRE storicamente il idprofile è sempre uguale a docnumber quindi posso togliere questa query
                        //DocsPaQueryHelper.getDocNumber(idProfile) è passare in input direttamente idProfile al metodo getAclDefinition

                        aclData = Dfs4DocsPa.getAclDefinition(idProfile, DocsPaObjectTypes.ObjectTypes.DOCUMENTO, this.InfoUtente);

                        aclService.ClearAndGrant(aclData, idProfile);
                    }
                }
            }
        }

        /// <summary>
        /// Handler dell'evento di avvenuta accettazione / rifiuto di una trasmissione di un documento / fascicolo
        /// </summary>
        /// <param name="trasmissione"></param>
        /// <param name="ruolo"></param>
        /// <param name="tipoRisposta"></param>
        public void TrasmissioneAccettataRifiutataEventHandler(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.trasmissione.TipoRisposta tipoRisposta)
        {
            CustomServices.AclDefinition aclData = null;

            DateTime startDate = DateTime.Now;

            // Aggiornamento ACL in DCTM, con le credenziali di superuser
            CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());

            if (trasmissione.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
            {
                // Reperimento della lista delle ACL aggiornata nella tabella security di docspa
                aclData = Dfs4DocsPa.getAclDefinition(trasmissione.infoDocumento.docNumber, DocsPaObjectTypes.ObjectTypes.DOCUMENTO, this.InfoUtente);
                aclService.ClearAndGrant(aclData, trasmissione.infoDocumento.idProfile);
            }
            else if (trasmissione.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.FASCICOLO)
            {
                DateTime startDateFasc = DateTime.Now;

                // Reperimento della lista delle ACL aggiornata nella tabella security di docspa
                aclData = Dfs4DocsPa.getAclDefinition(trasmissione.infoFascicolo.idFascicolo, DocsPaObjectTypes.ObjectTypes.FASCICOLO, this.InfoUtente);
                aclService.ClearAndGrant(aclData, trasmissione.infoFascicolo.idFascicolo);

                logger.Debug("Tot sec. FASC: " + DateTime.Now.Subtract(startDateFasc).TotalSeconds.ToString());
                string[] idProfiles = DocsPaQueryHelper.getDocumentiInFascicolo(trasmissione.infoFascicolo.idFascicolo);


                //PORKING: nonè necessario farlo ora, perchè le aCL di Documenti vengono rimappate se necesario quando gli utenti o i ruoli faranno 
                //          GetDettaglio con il metedo RefreshAcl
                if (idProfiles != null && idProfiles.Length < 20)
                {

                    foreach (string idProfile in idProfiles)
                    {
                        DateTime startDateDoc = DateTime.Now;

                        // Aggiornamento delle ACL per tutti i documenti nel fascicolo (compresi eventuali sottofascicoli)
                        // Aggiornamento delle ACL per tutti i documenti nel fascicolo (compresi eventuali sottofascicoli)
                        //DCTM è usato solo con PITRE e in PITRE storicamente il idprofile è sempre uguale a docnumber quindi posso togliere questa query
                        //DocsPaQueryHelper.getDocNumber(idProfile) è passare in input direttamente idProfile al metodo getAclDefinition
                        aclData = Dfs4DocsPa.getAclDefinition(idProfile, DocsPaObjectTypes.ObjectTypes.DOCUMENTO, this.InfoUtente);
                        aclService.ClearAndGrant(aclData, idProfile);

                        logger.DebugFormat("Tot sec. DOC_{0}: {1}", idProfile, DateTime.Now.Subtract(startDateDoc).TotalSeconds.ToString());
                    }
                }
            }

            logger.Debug("Tot sec.: " + DateTime.Now.Subtract(startDate).TotalSeconds.ToString());
        }

        /// <summary>
        /// Handler dell'evento di avvenuto smistamento di un documento ad un ruolo
        /// </summary>
        /// <param name="mittente"></param>
        /// <param name="documento"></param>
        /// <param name="ruolo"></param>
        /// <param name="accessRights"></param>
        public void SmistamentoDocumentoCompletatoEventHandler(DocsPaVO.Smistamento.MittenteSmistamento mittente, DocsPaVO.Smistamento.DocumentoSmistamento documento, DocsPaVO.Smistamento.RuoloSmistamento ruolo, string accessRights)
        {
            //// Reperimento ACL per il documento smistato
            //CustomServices.AclDefinition aclDefinition = AclHelper.getAclDefinition(DocsPaQueryHelper.getCodiceAmministrazione(this.InfoUtente.idAmministrazione), documento.DocNumber, DocsPaObjectTypes.ObjectTypes.DOCUMENTO);
            
            //// Inserimento dell'entry relativa al ruolo destinatario
            //List<CustomServices.AclEntry> entries = new List<CustomServices.AclEntry>(aclDefinition.entries);
            //AclHelper.addBasicPermit(entries, DocsPaObjectTypes.TypeGruppo.NormalizeGroupName(ruolo.Codice), Dfs4DocsPa.getPermitLevel(accessRights));
            //aclDefinition.entries = entries.ToArray();

            //// Aggiornamento ACL in DCTM, con le credenziali di superuser
            //CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());
            //aclService.Clear(aclDefinition);
            //aclService.UpdateWithGrant(aclDefinition);

            CustomServices.AclDefinition aclData = null;

            // Reperimento della lista delle ACL aggiornata nella tabella security di docspa
            aclData = Dfs4DocsPa.getAclDefinition(documento.DocNumber, DocsPaObjectTypes.ObjectTypes.DOCUMENTO, this.InfoUtente);

            // Aggiornamento ACL in DCTM, con le credenziali di superuser
            CustomServices.IAclService aclService = DctmServiceFactory.GetCustomServiceInstance<CustomServices.IAclService>(UserManager.ImpersonateSuperUser());
            aclService.ClearAndGrant(aclData, documento.IDDocumento);
        }


        #endregion

        #region Protected methods


        #endregion
    }
}
