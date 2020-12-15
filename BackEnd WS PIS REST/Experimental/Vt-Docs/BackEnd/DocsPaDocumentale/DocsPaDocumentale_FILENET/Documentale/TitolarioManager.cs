using System;
using System.Collections.Generic;
using System.Text;
using DocsPaVO.utente;
using DocsPaDocumentale.Interfaces;

namespace DocsPaDocumentale_FILENET.Documentale
{
    public class TitolarioManager : ITitolarioManager
    {
        #region Ctros, variables, constants

        private InfoUtente _infoUtente = null;

        private ITitolarioManager _instance = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public TitolarioManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Attivazione di un titolario
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public bool AttivaTitolario(DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            return this.InstanceETDOCS.AttivaTitolario(titolario);
        }

        public bool SaveTitolario(DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            return this.InstanceETDOCS.SaveTitolario(titolario);
        }

        /// <summary>
        /// Cancellazione struttura di classificazione
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public bool DeleteTitolario(DocsPaVO.amministrazione.OrgTitolario titolario)
        {
            return this.InstanceETDOCS.DeleteTitolario(titolario);
        }

        public bool SaveNodoTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
        {
            return this.InstanceETDOCS.SaveNodoTitolario(nodoTitolario);
        }

        public bool DeleteNodoTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario)
        {
            return this.InstanceETDOCS.DeleteNodoTitolario(nodoTitolario);
        }

        public bool SetAclRuoloNodoTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario, DocsPaVO.amministrazione.OrgRuoloTitolario ruoloTitolario)
        {
            return this.InstanceETDOCS.SetAclRuoloNodoTitolario(nodoTitolario, ruoloTitolario);
        }

        /// <summary>
        /// Aggiornamento visibilità di più ruoli su un titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="ruoliTitolario"></param>
        /// <returns></returns>
        public DocsPaVO.amministrazione.EsitoOperazione[] SetAclNodoTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario, DocsPaVO.amministrazione.OrgRuoloTitolario[] ruoliTitolario)
        {
            return this.InstanceETDOCS.SetAclNodoTitolario(nodoTitolario, ruoliTitolario);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Credenziali dell'utente connesso a documentum
        /// </summary>
        protected InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected ITitolarioManager InstanceETDOCS
        {
            get
            {
                if (this._instance == null)
                    this._instance = new DocsPaDocumentale_ETDOCS.Documentale.TitolarioManager(this.InfoUtente);
                return this._instance;
            }
        }

        #endregion
    }
}
