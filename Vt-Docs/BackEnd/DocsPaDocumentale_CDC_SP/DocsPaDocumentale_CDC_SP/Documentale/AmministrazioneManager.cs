using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;

namespace DocsPaDocumentale_CDC_SP.Documentale
{
    /// <summary>
    /// Gestione dell'amministrazione nel documentale
    /// </summary>
    public class AmministrazioneManager : IAmministrazioneManager
    {
        #region Ctors, constants, variables

        /// <summary>
        /// 
        /// </summary>
        private IAmministrazioneManager _instanceETDOCS = null;

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
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public EsitoOperazione Insert(InfoAmministrazione info)
        {
            EsitoOperazione retValue = this.InstanceETDOCS.Insert(info);

            return retValue;
        }

        /// <summary>
        /// Aggiornamento di un'amministrazione esistente nel documentale
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public EsitoOperazione Update(InfoAmministrazione info)
        {
            return this.InstanceETDOCS.Update(info);
        }

        /// <summary>
        /// Cancellazione di un'amministrazione nel documentale
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public EsitoOperazione Delete(InfoAmministrazione info)
        {
            // Prima della cancellazione in etdocs, è necessario
            // reperire i gruppi docspa e passarli successivamente a documentum
            string[] docspaGroups = this.GetGroupNames(info.IDAmm);

            EsitoOperazione retValue = this.InstanceETDOCS.Delete(info);

            return retValue;
        }

        #endregion

        #region Protected methods

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
        /// Reperimento istanza gestore amministratore in etdocs
        /// </summary>
        protected IAmministrazioneManager InstanceETDOCS
        {
            get
            {
                if (this._instanceETDOCS == null)
                    this._instanceETDOCS = new DocsPaDocumentale_ETDOCS.Documentale.AmministrazioneManager(this.InfoUtente);
                return this._instanceETDOCS;
            }
        }

       
        /// <summary>
        /// Reperimento nomi gruppi docspa per l'amministrazione
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        protected string[] GetGroupNames(string idAmministrazione)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione ammDb = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return ammDb.GetRuoli(idAmministrazione);
        }
        #endregion
    }
}
