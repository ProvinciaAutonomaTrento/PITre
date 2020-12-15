using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using OrganigrammaManagerETDOCS = DocsPaDocumentale_ETDOCS.Documentale.OrganigrammaManager;
using OrganigrammaManagerDCTM = DocsPaDocumentale_DOCUMENTUM.Documentale.OrganigrammaManager;

namespace DocsPaDocumentale_PITRE.Documentale
{
    /// <summary>
    /// Gestione dell'organigramma dell'amministrazione
    /// </summary>
    public class OrganigrammaManager : IOrganigrammaManager
    {
        #region Ctors, constants, variables

        /// <summary>
        /// 
        /// </summary>
        private OrganigrammaManagerETDOCS _orgManagerETDOCS = null;

        /// <summary>
        /// 
        /// </summary>
        private OrganigrammaManagerDCTM _orgManagerDCTM = null;

        /// <summary>
        /// Credenziali utente
        /// </summary>
        private InfoUtente _infoUtente = null;

        public OrganigrammaManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;

            this._orgManagerETDOCS = new OrganigrammaManagerETDOCS(this._infoUtente);
            this._orgManagerDCTM = new OrganigrammaManagerDCTM(this._infoUtente);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Inserimento nuovo ruolo in amministrazione
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione InserisciRuolo(OrgRuolo ruolo, bool computeAtipicita)
        {
            // Inserimento del ruolo nel documentale ETDOCS
            EsitoOperazione result = this.OrganigrammaManagerETDOCS.InserisciRuolo(ruolo, computeAtipicita);

            if (result.Codice == 0)
            {
                // Se l'inserimento è andato a buon fine, 
                // inserimento del ruolo nel documentale DOCUMENTUM
                result = this.OrganigrammaManagerDCTM.InserisciRuolo(ruolo, computeAtipicita);
            }

            return result;
        }

        /// <summary>
        /// Modifica dei metadati di un ruolo
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione ModificaRuolo(OrgRuolo ruolo)
        {
            // Modifica del ruolo nel documentale ETDOCS
            EsitoOperazione result = this.OrganigrammaManagerETDOCS.ModificaRuolo(ruolo);

            if (result.Codice == 0)
            {
                // Se la modifica è andata a buon fine, 
                // modifica del ruolo nel documentale DOCUMENTUM
                result = this.OrganigrammaManagerDCTM.ModificaRuolo(ruolo);
            }

            return result;
        }

        /// <summary>
        /// informa l'amministratore se il ruolo può essere disabilitato ma non eliminato
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione OnlyDisabledRole(OrgRuolo ruolo)
        {
            // possibili valori di ritorno:
            // 1 - il ruolo presenta record nella dpa_doc_arrivo_par
            // 2 - il ruolo presenta record nella dpa_trasm_singola
            // 21 - il ruolo presenta record nella profile
            // 0 - il ruolo può essere eliminato	
            //non siamo interessati agli altri valori, serviranno in elimina ruolo
            // Eliminazione del ruolo nel documentale ETDOCS
            EsitoOperazione result = this.OrganigrammaManagerETDOCS.OnlyDisabledRole(ruolo);
            return result;
        }
        
        /// <summary>
        /// Cancellazione di un ruolo in amministrazione
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaRuolo(OrgRuolo ruolo)
        {
            // Eliminazione del ruolo nel documentale DOCUMENTUM
            EsitoOperazione result = this.OrganigrammaManagerDCTM.EliminaRuolo(ruolo);

            if (result.Codice == 0)
            {
                // Eliminazione del ruolo nel documentale ETDOCS se è stato cancellato con successo da et docs
                result = this.OrganigrammaManagerETDOCS.EliminaRuolo(ruolo);

                
            }

            return result;
        }

        /// <summary>
        /// Spostamento di un ruolo in amministrazione
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione SpostaRuolo(OrgRuolo ruolo)
        {
            // Spostamento del ruolo nel documentale ETDOCS
            EsitoOperazione result = this.OrganigrammaManagerETDOCS.SpostaRuolo(ruolo);

            if (result.Codice == 0)
            {
                // Spostamento del ruolo nel documentale DCTM
                result = this.OrganigrammaManagerDCTM.SpostaRuolo(ruolo);
            }

            return result;
        }

        /// <summary>
        /// Impostazione di un ruolo come predefinito
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione ImpostaRuoloPreferito(string idPeople, string idGruppo)
        {
            // Operazione nel documentale ETDOCS
            return this.OrganigrammaManagerETDOCS.ImpostaRuoloPreferito(idPeople, idGruppo);
        }

        /// <summary>
        /// Inserimento di un nuovo utente in amministrazione
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione InserisciUtente(OrgUtente utente)
        {
            // Inserimento utente nel documentale ETDOCS
            EsitoOperazione result = this.OrganigrammaManagerETDOCS.InserisciUtente(utente);

            if (result.Codice == 0)
            {
        #region Inserimento utente nel documentale DOCUMENTUM
                //MEV Inserimento utenti multi-amministrazione su Documentum
                
                // verifica se nuovo utente esiste su Documentum
                if (this.OrganigrammaManagerDCTM.ContainsUser(utente.UserId))
                {
                    // recupero info gruppo Amministratore
                    string codiceAmm = DocsPaDocumentale_DOCUMENTUM.DocsPaServices.DocsPaQueryHelper.getCodiceAmministrazione(utente.IDAmministrazione);
                    string gruppoAmm = DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes.TypeGruppo.GetGroupNameForAmministrazione(codiceAmm);

                    // aggiunge l'utente nei ruoli di amministratore
                    OrganigrammaManagerDCTM.InserisciUtenteInAmm(utente);
                    result.Codice = 0;
                    result.Descrizione = string.Empty;
                }
                else
                // se l'utente non esiste in nessuna amministrazione, ne crea uno nuovo
                result = this.OrganigrammaManagerDCTM.InserisciUtente(utente);
        #endregion 
            }

            return result;
        }

        /// <summary>
        /// Modifica dei dati di un utente in amministrazione
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione ModificaUtente(OrgUtente utente)
        {
            // Modifica utente nel documentale ETDOCS
            EsitoOperazione result = this.OrganigrammaManagerETDOCS.ModificaUtente(utente);

            if (result.Codice == 0 || result.Codice == 5 || result.Codice == 4 || result.Codice == 6)
            {
                // Modifica utente nel documentale DOCUMENTUM
                result = this.OrganigrammaManagerDCTM.ModificaUtente(utente);
            }

            return result;
        }

        /// <summary>
        /// Elimina un utente in amministrazione
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaUtente(OrgUtente utente)
        {
            // Eliminazione utente nel documentale ETDOCS
            EsitoOperazione result = this.OrganigrammaManagerETDOCS.EliminaUtente(utente);

            if (result.Codice == 0)
            {
                // Eliminazione utente nel documentale DOCUMENTUM
                result = this.OrganigrammaManagerDCTM.EliminaUtenteAmm(utente);
            }

            return result;
        }

        /// <summary>
        /// Inserimento di un utente in un ruolo
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione InserisciUtenteInRuolo(string idPeople, string idGruppo)
        {
            // Inserimento utente in un ruolo nel documentale ETDOCS
            EsitoOperazione result = this.OrganigrammaManagerETDOCS.InserisciUtenteInRuolo(idPeople, idGruppo);

            if (result.Codice == 0)
            {
                // Inserimento utente in un ruolo nel documentale DOCUMENTUM
                result = this.OrganigrammaManagerDCTM.InserisciUtenteInRuolo(idPeople, idGruppo);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaUtenteDaRuolo(string idPeople, string idGruppo)
        {
            // Eliminazione utente in un ruolo nel documentale ETDOCS
            EsitoOperazione result = this.OrganigrammaManagerETDOCS.EliminaUtenteDaRuolo(idPeople, idGruppo);

            if (result.Codice == 0)
            {
                // Eliminazione utente in un ruolo nel documentale DOCUMENTUM
                result = this.OrganigrammaManagerDCTM.EliminaUtenteDaRuolo(idPeople, idGruppo);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="copyVisibility"></param>
        /// <returns></returns>
        public EsitoOperazione CopyVisibility(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Security.CopyVisibility copyVisibility)
        {
            // Copia visibilità da ruolo a ruolo nel documentale ETDOCS
            EsitoOperazione resultEtdocs = this.OrganigrammaManagerETDOCS.CopyVisibility(infoUtente, copyVisibility);

            if (resultEtdocs.Codice == 0)
            {
                // Copia visibilità da ruolo a ruolo nel documentale DCTM
                EsitoOperazione resultDctm = this.OrganigrammaManagerDCTM.CopyVisibility(infoUtente, copyVisibility);

                // Bug INC000000442394 APSS il risultato DCTM non conta i doc / fasc copiati, ritorno il result ETDOCS se è tutto ok,
                // altrimenti il log amm sarà sempre KO
                if (resultDctm.Codice == 0)
                    return resultEtdocs;
                else
                    return resultDctm;
            }           

          

            return resultEtdocs;
        }

        public OrgRuolo HistoricizeRole(OrgRuolo role)
        {
            OrgRuolo result = this.OrganigrammaManagerETDOCS.HistoricizeRole(role);
        
            return result;

        }

        /// <summary>
        /// Metodo per l'estensione di visibilità ai ruoli superiori di un ruolo
        /// </summary>
        /// <param name="idAmm">Id dell'amministrazione</param>
        /// <param name="idGroup">Id del gruppo di cui estendere la visibilità</param>
        /// <param name="extendScope">Scope di estensione</param>
        /// <param name="copyIdToTempTable">True se bisogna copiare gli id id dei documenti e fascicoli in una tabella tamporanea per l'allineamento asincrono della visibilità</param>
        /// <returns>Esito dell'operazione</returns>
        public EsitoOperazione ExtendVisibilityToHigherRoles(
            String idAmm,
            String idGroup,
            DocsPaVO.amministrazione.SaveChangesToRoleRequest.ExtendVisibilityOption extendScope)
        {
            // Invocazione dell'operazione su Documentum
            return this.OrganigrammaManagerDCTM.ExtendVisibilityToHigherRoles(idAmm, idGroup, extendScope);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// 
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
        protected OrganigrammaManagerETDOCS OrganigrammaManagerETDOCS
        {
            get
            {
                return this._orgManagerETDOCS;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private OrganigrammaManagerDCTM OrganigrammaManagerDCTM
        {
            get
            {
                return this._orgManagerDCTM;
            }
        }

        #endregion


        public EsitoOperazione CalcolaAtipicita(OrgRuolo ruolo, string idTipoRuoloVecchio, string idVecchiaUo, bool calcolaSuiSottoposti)
        {
            return this.OrganigrammaManagerETDOCS.CalcolaAtipicita(ruolo, idTipoRuoloVecchio, idVecchiaUo, calcolaSuiSottoposti);
        }
    }
}
