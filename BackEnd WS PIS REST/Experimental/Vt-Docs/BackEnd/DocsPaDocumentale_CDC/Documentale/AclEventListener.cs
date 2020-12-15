using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;

namespace DocsPaDocumentale_CDC.Documentale
{
    public class AclEventListener : IAclEventListener
    {
        #region Ctros, variables, constants

        /// <summary>
        /// 
        /// </summary>
        private IAclEventListener _instanceETDOCS = null;

        /// <summary>
        /// 
        /// </summary>
        private IAclEventListener _instanceOCS = null;

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

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="ruoliSuperiori"></param>
        public void DocumentoCreatoEventHandler(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            this.InstanceETDOCS.DocumentoCreatoEventHandler(schedaDocumento, ruolo, ruoliSuperiori);

            this.InstanceOCS.DocumentoCreatoEventHandler(schedaDocumento, ruolo, ruoliSuperiori);
        }

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
        {
            this.InstanceETDOCS.FascicoloCreatoEventHandler(classificazione, fascicolo, ruolo, ruoliSuperiori);

            this.InstanceOCS.FascicoloCreatoEventHandler(classificazione, fascicolo, ruolo, ruoliSuperiori);
        }

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
        {
            this.InstanceETDOCS.SottofascicoloCreatoEventHandler(folder, ruolo, ruoliSuperiori);

            this.InstanceOCS.SottofascicoloCreatoEventHandler(folder, ruolo, ruoliSuperiori);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trasmissione"></param>
        /// <param name="infoSecurity"></param>
        public void TrasmissioneCompletataEventHandler(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.trasmissione.infoSecurity[] infoSecurityList)
        {
            this.InstanceETDOCS.TrasmissioneCompletataEventHandler(trasmissione, infoSecurityList);

            this.InstanceOCS.TrasmissioneCompletataEventHandler(trasmissione, infoSecurityList);
        }

        /// <summary>
        /// Handler dell'evento di avvenuta accettazione / rifiuto di una trasmissione di un documento / fascicolo
        /// </summary>
        /// <param name="trasmissione"></param>
        /// <param name="ruolo"></param>
        /// <param name="tipoRisposta"></param>
        public void TrasmissioneAccettataRifiutataEventHandler(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.trasmissione.TipoRisposta tipoRisposta)
        {
            this.InstanceETDOCS.TrasmissioneAccettataRifiutataEventHandler(trasmissione, ruolo, tipoRisposta);

            this.InstanceOCS.TrasmissioneAccettataRifiutataEventHandler(trasmissione, ruolo, tipoRisposta);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mittente"></param>
        /// <param name="documento"></param>
        /// <param name="ruolo"></param>
        /// <param name="accessRights"></param>
        public void SmistamentoDocumentoCompletatoEventHandler(DocsPaVO.Smistamento.MittenteSmistamento mittente, DocsPaVO.Smistamento.DocumentoSmistamento documento, DocsPaVO.Smistamento.RuoloSmistamento ruolo, string accessRights)
        {
            this.InstanceETDOCS.SmistamentoDocumentoCompletatoEventHandler(mittente, documento, ruolo, accessRights);

            this.InstanceOCS.SmistamentoDocumentoCompletatoEventHandler(mittente, documento, ruolo, accessRights);
        }

        #endregion

        #region Protected method

        /// <summary>
        /// 
        /// </summary>
        protected DocsPaVO.utente.InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected IAclEventListener InstanceETDOCS
        {
            get
            {
                if (this._instanceETDOCS == null)
                    this._instanceETDOCS = new DocsPaDocumentale_ETDOCS.Documentale.AclEventListener(this.InfoUtente);
                return this._instanceETDOCS;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected IAclEventListener InstanceOCS
        {
            get
            {
                if (this._instanceOCS == null)
                    this._instanceOCS = new DocsPaDocumentale_OCS.Documentale.AclEventListener(this.InfoUtente);
                return this._instanceOCS;
            }
        }

        #endregion
    }
}
