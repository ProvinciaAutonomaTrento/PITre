using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;

namespace DocsPaDocumentale_CDC.Documentale
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
        private IAmministrazioneManager _instanceOCS = null;

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

            // ?? al momento non sembra necessario creare qualcosa relativo ad un'amministrazione su OCS
            //if (retValue.Codice == 0)
            //{
            //    retValue = this.InstanceOCS.Insert(info);
            //}

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
            // reperire i gruppi docspa e passarli successivamente a OCS
            DocsPaVO.amministrazione.OrgRuolo[] adminRoles = DocsPaDocumentale_OCS.DocsPaServices.DocsPaQueryHelper.getAdminGroups(info.IDAmm);

            EsitoOperazione retValue = this.InstanceETDOCS.Delete(info);

            if (retValue.Codice == 0)
            {
                retValue = ((DocsPaDocumentale_OCS.Documentale.AmministrazioneManager)this.InstanceOCS).Delete(info, adminRoles);
            }

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
        /// Reperimento istanza gestore amministratore in OCS
        /// </summary>
        protected IAmministrazioneManager InstanceOCS
        {
            get
            {
                if (this._instanceOCS == null)
                    this._instanceOCS = new DocsPaDocumentale_OCS.Documentale.AmministrazioneManager(this.InfoUtente);
                return this._instanceOCS;
            }
        }

        #endregion
    }
}


