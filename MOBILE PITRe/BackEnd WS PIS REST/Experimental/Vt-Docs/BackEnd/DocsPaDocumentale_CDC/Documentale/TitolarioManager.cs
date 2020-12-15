using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using TitolarioManagerETDOCS = DocsPaDocumentale_ETDOCS.Documentale.TitolarioManager;
using TitolarioManagerOCS = DocsPaDocumentale_OCS.Documentale.TitolarioManager;

namespace DocsPaDocumentale_CDC.Documentale
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
        private TitolarioManagerOCS _titolarioOCS = null;

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

            if (saved)
            {
                // Attivazione struttura del titolario nel documentale OCS
                saved = this.TitolarioManagerOCS.AttivaTitolario(titolario);
            }

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

            if (saved)
            {
                // Se l'inserimento è andato a buon fine, 
                // Aggiornamento dati del titolario nel documentale OCS
                saved = this.TitolarioManagerOCS.SaveTitolario(titolario);
            }

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

            if (canceled)
            {
                // Se l'inserimento è andato a buon fine, 
                // Aggiornamento dati del titolario nel documentale OCS
                canceled = this.TitolarioManagerOCS.DeleteTitolario(titolario);
            }

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

            if (saved)
            {
                // Se l'inserimento è andato a buon fine, 
                // Aggiornamento dati del titolario nel documentale OCS
                saved = this.TitolarioManagerOCS.SaveNodoTitolario(nodoTitolario);
            }

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

            if (deleted)
            {
                // Se l'inserimento è andato a buon fine, 
                // Cancellazione dati del titolario nel documentale OCS
                deleted = this.TitolarioManagerOCS.DeleteNodoTitolario(nodoTitolario);
            }

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

            if (retValue)
            {
                // Se l'inserimento è andato a buon fine, 
                // Cancellazione dati del titolario nel documentale OCS
                retValue = this.TitolarioManagerOCS.SetAclRuoloNodoTitolario(nodoTitolario, ruoloTitolario);
            }

            return retValue;
        }

        /// <summary>
        /// Aggiornamento visibilità di più ruoli su un titolario
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="ruoliTitolario"></param>
        /// <returns></returns>
        public DocsPaVO.amministrazione.EsitoOperazione[] SetAclNodoTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario, DocsPaVO.amministrazione.OrgRuoloTitolario[] ruoliTitolario)
        {
            // Cancellazione dati del titolario nel documentale ETDOCS
            return this.TitolarioManagerETDOCS.SetAclNodoTitolario(nodoTitolario, ruoliTitolario);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Credenziali dell'utente connesso a OCS
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

        /// <summary>
        /// 
        /// </summary>
        private TitolarioManagerOCS TitolarioManagerOCS
        {
            get
            {
                if (this._titolarioOCS == null)
                    this._titolarioOCS = new TitolarioManagerOCS(this.InfoUtente);
                return this._titolarioOCS;
            }
        }

        #endregion
    }
}

