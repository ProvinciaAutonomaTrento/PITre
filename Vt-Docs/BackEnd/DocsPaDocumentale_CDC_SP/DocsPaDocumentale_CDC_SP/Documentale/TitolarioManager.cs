using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using TitolarioManagerETDOCS = DocsPaDocumentale_ETDOCS.Documentale.TitolarioManager;


namespace DocsPaDocumentale_CDC_SP.Documentale
{
    /// <summary>
    /// 
    /// </summary>
    public class TitolarioManager : ITitolarioManager
    {
        #region Ctros, variables, constants

        /// <summary>
        /// 
        /// </summary>
        private TitolarioManagerETDOCS _titolarioEtdocs = null;

       

        /// <summary>
        /// 
        /// </summary>
        private InfoUtente _infoUtente = null;

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
        public bool AttivaTitolario(OrgTitolario titolario)
        {
            // Attivazione struttura del titolario nel documentale ETDOCS
            bool saved = this.TitolarioManagerETDOCS.AttivaTitolario(titolario);
            
            return saved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public bool SaveTitolario(OrgTitolario titolario)
        {
            // Aggiornamento dati del titolario nel documentale ETDOCS
            bool saved = this.TitolarioManagerETDOCS.SaveTitolario(titolario);

            return saved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="titolario"></param>
        /// <returns></returns>
        public bool DeleteTitolario(OrgTitolario titolario)
        {
            // Cancellazione dati del titolario nel documentale ETDOCS
            bool canceled = this.TitolarioManagerETDOCS.DeleteTitolario(titolario);

            return canceled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public bool SaveNodoTitolario(OrgNodoTitolario nodoTitolario)
        {
            // Aggiornamento dati del titolario nel documentale ETDOCS
            bool saved = this.TitolarioManagerETDOCS.SaveNodoTitolario(nodoTitolario);

            return saved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <returns></returns>
        public bool DeleteNodoTitolario(OrgNodoTitolario nodoTitolario)
        {
            // Cancellazione dati del titolario nel documentale ETDOCS
            bool deleted = this.TitolarioManagerETDOCS.DeleteNodoTitolario(nodoTitolario);

            return deleted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="ruoloTitolario"></param>
        /// <returns></returns>
        public bool SetAclRuoloNodoTitolario(OrgNodoTitolario nodoTitolario, OrgRuoloTitolario ruoloTitolario)
        {
            // Cancellazione dati del titolario nel documentale ETDOCS
            bool retValue = this.TitolarioManagerETDOCS.SetAclRuoloNodoTitolario(nodoTitolario, ruoloTitolario);

            return retValue;
        }

        /// <summary>
        /// Aggiornamento visibilità di più ruoli su un titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="ruoliTitolario"></param>
        /// <returns></returns>
        public DocsPaVO.amministrazione.EsitoOperazione[] SetAclNodoTitolario(OrgNodoTitolario nodoTitolario, OrgRuoloTitolario[] ruoliTitolario)
        {
            // Aggiornamento delle acl di un nodo titolario per il documentale ETDOCS
            EsitoOperazione[] esitoOperazione = this.TitolarioManagerETDOCS.SetAclNodoTitolario(nodoTitolario, ruoliTitolario);

            return esitoOperazione;
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
        protected TitolarioManagerETDOCS TitolarioManagerETDOCS
        {
            get
            {
                if (this._titolarioEtdocs == null)
                    this._titolarioEtdocs = new TitolarioManagerETDOCS(this.InfoUtente);
                return this._titolarioEtdocs;
            }
        }

        

        #endregion
    }
}
