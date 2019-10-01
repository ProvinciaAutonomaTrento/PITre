using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using System.Configuration;

namespace DocsPaDocumentale.Documentale
{
    /// <summary>
    /// 
    /// </summary>
    public class AclEventListener : IAclEventListener
    {
        #region Ctros, variables, constants

        /// <summary>
        /// Tipo documentale corrente
        /// </summary>
        private static Type _type = null;

        /// <summary>
        /// Oggetto documentale corrente
        /// </summary>
        private IAclEventListener _instance = null;

        /// <summary>
        /// Reperimento del tipo relativo al documentale corrente
        /// </summary>
        static AclEventListener()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["documentale"]))
            {
                string documentale = ConfigurationManager.AppSettings["documentale"].ToLower();

                if (documentale.Equals(TipiDocumentaliEnum.Etnoteam.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.AclEventListener);

                else if (documentale.Equals(TipiDocumentaliEnum.Hummingbird.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_HUMMINGBIRD.Documentale.AclEventListener);

                else if (documentale.Equals(TipiDocumentaliEnum.Filenet.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_FILENET.Documentale.AclEventListener);

                else if (documentale.Equals(TipiDocumentaliEnum.Pitre.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_PITRE.Documentale.AclEventListener);

                else if (documentale.Equals(TipiDocumentaliEnum.CDC.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC.Documentale.AclEventListener);

                else if (documentale.Equals(TipiDocumentaliEnum.GFD.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_GFD.Documentale.AclEventListener);

              

                 //Giordano Iacozzilli  08/10/2012 Aggiunta strato SharePoint
                else if (documentale.Equals(TipiDocumentaliEnum.SharePoint.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC_SP.Documentale.AclEventListener);
                //Fine

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public AclEventListener(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._instance = (IAclEventListener)Activator.CreateInstance(_type, infoUtente);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Reperimento istanza oggetto "IDocumentEventsNotification"
        /// relativamente al documentale correntemente configurato
        /// </summary>
        protected IAclEventListener Instance
        {
            get
            {
                return this._instance;
            }
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
            this.Instance.DocumentoCreatoEventHandler(schedaDocumento, ruolo, ruoliSuperiori);
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
            this.Instance.FascicoloCreatoEventHandler(classificazione, fascicolo, ruolo, ruoliSuperiori);
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
            this.Instance.SottofascicoloCreatoEventHandler(folder, ruolo, ruoliSuperiori);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trasmissione"></param>
        /// <param name="infoSecurity"></param>
        public void TrasmissioneCompletataEventHandler(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.trasmissione.infoSecurity[] infoSecurityList)
        {
            this.Instance.TrasmissioneCompletataEventHandler(trasmissione, infoSecurityList);
        }

        /// <summary>
        /// Handler dell'evento di avvenuta accettazione / rifiuto di una trasmissione di un documento / fascicolo
        /// </summary>
        /// <param name="trasmissione"></param>
        /// <param name="ruolo"></param>
        /// <param name="tipoRisposta"></param>
        public void TrasmissioneAccettataRifiutataEventHandler(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.trasmissione.TipoRisposta tipoRisposta)
        {
            this.Instance.TrasmissioneAccettataRifiutataEventHandler(trasmissione, ruolo, tipoRisposta);
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
            this.Instance.SmistamentoDocumentoCompletatoEventHandler(mittente, documento, ruolo, accessRights);
        }

        #endregion
    }
}
