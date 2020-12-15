using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using DocsPaDocumentale.Interfaces;
using DocsPaUtils.LogsManagement;
using DocsPaDocumentale_OCS.OCSServices;
using DocsPaDocumentale_OCS.CorteContentServices;
using log4net;

namespace DocsPaDocumentale_OCS.Documentale
{
    public class AclEventListener : IAclEventListener
    {
        private ILog logger=LogManager.GetLogger(typeof(AclEventListener));
        #region Ctros, variables, constants

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
        { }

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
        { }

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
            // In caso di trasmissione con cessione diritti attivata, cambia il proprietario del documento
            // in OCS proprietario e non, hanno il solo diritto di lettura, quindi basta aggiornare l'acl
            
            //TODO:
            if (trasmissione.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
            {
                // Reperimento ACL per il documento smistato

                bool retValue = false;

                try
                {
                    //bisogna reperire l'id OCS
                    UserCredentialsType credentials = OCSUtils.getApplicationUserCredentials();

                    long idOCS = OCSDocumentHelper.getDocumentIdOCS(trasmissione.infoDocumento.docNumber, null, credentials);
                    
                    //reperisco la acl del documento docspa e aggiorno quelle di OCS
                    retValue = OCSDocumentHelper.setAclDocument(trasmissione.infoDocumento.idProfile, idOCS, this.InfoUtente, credentials);
                }
                catch (Exception ex)
                {
                    retValue = false;

                    logger.Error("Errore in OCS.TrasmissioneCompletataEventHandler", ex);
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
            // Reperimento ACL per il documento smistato
            bool retValue = false;

            try
            {
                //bisogna reperire l'id OCS
                UserCredentialsType credentials = OCSUtils.getApplicationUserCredentials();

                long idOCS = OCSDocumentHelper.getDocumentIdOCS(documento.DocNumber, null, credentials);
                
                //reperisco la acl del documento docspa e aggiorno quelle di OCS
                retValue = OCSDocumentHelper.setAclDocument(documento.IDDocumento, idOCS, this.InfoUtente, credentials);
            }
            catch (Exception ex)
            {
                retValue = false;

                logger.Debug("Errore in OCS.SmistamentoDocumentoCompletatoEventHandler", ex);
            }

            return;
        }

        #endregion

        #region Protected methods


        #endregion
    }
}
