using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using TitolarioManagerETDOCS = DocsPaDocumentale_ETDOCS.Documentale.TitolarioManager;
using TitolarioManagerDCTM = DocsPaDocumentale_DOCUMENTUM.Documentale.TitolarioManager;

namespace DocsPaDocumentale_PITRE.Documentale
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
        private TitolarioManagerDCTM _titolarioDctm = null;

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
                // Attivazione struttura del titolario nel documentale DOCUMENTUM
                saved = this.TitolarioManagerDCTM.AttivaTitolario(titolario);
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
                // Aggiornamento dati del titolario nel documentale DOCUMENTUM
                saved = this.TitolarioManagerDCTM.SaveTitolario(titolario);
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
                // Aggiornamento dati del titolario nel documentale DOCUMENTUM
                canceled = this.TitolarioManagerDCTM.DeleteTitolario(titolario);
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
                // Aggiornamento dati del titolario nel documentale DOCUMENTUM
                saved = this.TitolarioManagerDCTM.SaveNodoTitolario(nodoTitolario);
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
                // Cancellazione dati del titolario nel documentale DOCUMENTUM
                deleted = this.TitolarioManagerDCTM.DeleteNodoTitolario(nodoTitolario);
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
                // Cancellazione dati del titolario nel documentale DOCUMENTUM
                retValue = this.TitolarioManagerDCTM.SetAclRuoloNodoTitolario(nodoTitolario, ruoloTitolario);
            }

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

            if (esitoOperazione != null && esitoOperazione.Length > 0)
            {
                //// Dall'esito dell'operazione in ETDOCS, reperimento dei ruoli aggiornati correttamente (attributo "Codice" == 0)
                //var ruoliNonAggiornati = esitoOperazione.Where(e => e.Codice != 0);

                //OrgRuoloTitolario[] ruoliTitolarioDctm = null;
                //if (ruoliNonAggiornati.Count() > 0)
                //    // Reperimento dei soli ruoli che sono stati aggiornati correttamente in etdocs
                //    ruoliTitolarioDctm = ruoliTitolario.Any(e => e.ID != (from c in ruoliNonAggiornati select c.Codice.ToString())).ToArray();
                //else
                //    ruoliTitolarioDctm = ruoliTitolario;

                //var d = (from c in ruoliTitolario join 
                //        x in ruoliNonAggiornati on c.ID equals x.Codice.ToString()
                //        select c);
                        
                EsitoOperazione[] esitoOperazioneDctm = this.TitolarioManagerDCTM.SetAclNodoTitolario(nodoTitolario, ruoliTitolario);

                if (esitoOperazioneDctm[0].Codice == -1)
                    // Errore nell'aggiornamento in Documentum
                    esitoOperazione = esitoOperazioneDctm;
            }

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

        /// <summary>
        /// 
        /// </summary>
        private TitolarioManagerDCTM TitolarioManagerDCTM
        {
            get
            {
                if (this._titolarioDctm == null)
                    this._titolarioDctm = new TitolarioManagerDCTM(this.InfoUtente);
                return this._titolarioDctm;
            }
        }

        #endregion
    }
}
