using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using DocsPaUtils.LogsManagement;
using log4net;

namespace DocsPaDocumentale_OCS.Documentale
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
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public EsitoOperazione Insert(InfoAmministrazione info)
        {
            // NB: Per ora non implementata, in quanto l'amministrazione non è stata mappata su OCS 
            throw new NotSupportedException("Operazione 'Insert' non supportata dal documentale");
        }

        /// <summary>
        /// Aggiornamento di un'amministrazione esistente nel documentale
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public EsitoOperazione Update(InfoAmministrazione info)
        {
            // NB: Per ora non implementata, in quanto l'amministrazione non è stata mappata su OCS
            throw new NotSupportedException("Operazione 'Update' non supportata dal documentale");
        }

        /// <summary>
        /// Cancellazione di un'amministrazione nel documentale ocs
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public EsitoOperazione Delete(InfoAmministrazione info)
        {
            return this.Delete(info, DocsPaServices.DocsPaQueryHelper.getAdminGroups(info.IDAmm));
        }

        /// <summary>
        /// Cancellazione di un'amministrazione nel documentale ocs
        /// </summary>
        /// <param name="info"></param>
        /// <param name="adminRoles">
        /// Ruoli docspa creati e da rimuovere in ocs
        /// </param>
        /// <returns></returns>
        public EsitoOperazione Delete(InfoAmministrazione info, DocsPaVO.amministrazione.OrgRuolo[] adminRoles)
        {
            EsitoOperazione retValue = new EsitoOperazione();

            try
            {
                //// Prima della cancellazione in etdocs, è necessario
                //// reperire i gruppi docspa e passarli successivamente a OCS
                //OrgRuolo[] docspaGroups = this.GetGroups(info.IDAmm);

                IOrganigrammaManager orgMng = new OrganigrammaManager(this.InfoUtente);

                foreach (OrgRuolo role in adminRoles)
                {
                    retValue = orgMng.EliminaRuolo(role);

                    if (retValue.Codice != 0)
                        break;
                }

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Errore in OCS.DeleteAmministrazione:\n{0}", ex.ToString()));

                retValue.Codice = -1;
                retValue.Descrizione = string.Format("Errore nella rimozione in OCS", info.Codice);
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

        #endregion
    }
}
