using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using OrganigrammaManagerETDOCS = DocsPaDocumentale_ETDOCS.Documentale.OrganigrammaManager;
using OrganigrammaManagerOCS = DocsPaDocumentale_OCS.Documentale.OrganigrammaManager;


namespace DocsPaDocumentale_CDC.Documentale
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
        private OrganigrammaManagerOCS _orgManagerOCS = null;

        /// <summary>
        /// Credenziali utente
        /// </summary>
        private InfoUtente _infoUtente = null;

        public OrganigrammaManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;

            this._orgManagerETDOCS = new OrganigrammaManagerETDOCS(this._infoUtente);
            this._orgManagerOCS = new OrganigrammaManagerOCS(this._infoUtente);
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
                // inserimento del ruolo nel documentale OCS
                result = this.OrganigrammaManagerOCS.InserisciRuolo(ruolo, computeAtipicita);
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
                // modifica del ruolo nel documentale OCS
                result = this.OrganigrammaManagerOCS.ModificaRuolo(ruolo);
            }

            return result;
        }

        /// <summary>
        /// Informa lamministratore se il ruolo ha documenti associati(in questo caso 
        /// il ruolo può essere solo disabilitato)
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione OnlyDisabledRole(OrgRuolo ruolo)
        {
            //documentale ETDOCS
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
            // Eliminazione del ruolo nel documentale ETDOCS
            EsitoOperazione result = this.OrganigrammaManagerETDOCS.EliminaRuolo(ruolo);

            if (result.Codice == 0)
            {
                // Se l'eliminazione è andata a buon fine, 
                // eliminazione del ruolo nel documentale OCS
                result = this.OrganigrammaManagerOCS.EliminaRuolo(ruolo);
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
            return this.OrganigrammaManagerETDOCS.SpostaRuolo(ruolo);
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
                // Inserimento utente in un ruolo nel documentale OCS
                result = this.OrganigrammaManagerOCS.InserisciUtente(utente);
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
                // Eliminazione utente dal GRUPPO degli utenti OCS 
                result = this.OrganigrammaManagerOCS.EliminaUtente(utente);
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
                // Inserimento utente in un ruolo nel documentale OCS
                result = this.OrganigrammaManagerOCS.InserisciUtenteInRuolo(idPeople, idGruppo);
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
                // Eliminazione utente in un ruolo nel documentale OCS
                result = this.OrganigrammaManagerOCS.EliminaUtenteDaRuolo(idPeople, idGruppo);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="copyVisibility"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public EsitoOperazione CopyVisibility(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Security.CopyVisibility copyVisibility)
        {
            // Copia visibilità da ruolo a ruolo nel documentale ETDOCS
            EsitoOperazione result = this.OrganigrammaManagerETDOCS.CopyVisibility(infoUtente, copyVisibility);

            if (result.Codice == 0)
            {
                // Copia visibilità da ruolo a ruolo nel documentale OCS
                result = this.OrganigrammaManagerOCS.CopyVisibility(infoUtente, copyVisibility);
            }

            return result;
        }

        public OrgRuolo HistoricizeRole(OrgRuolo role)
        {
            // Storicizzazione in ET-Docs
            OrgRuolo result = this.OrganigrammaManagerETDOCS.HistoricizeRole(role);

            if (result != null)
            {
                // Storicizzazione in OCS
                result = this.OrganigrammaManagerOCS.HistoricizeRole(role);
            }

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
            // Invocazione dell'operazione su EtDocs
            return this.OrganigrammaManagerETDOCS.ExtendVisibilityToHigherRoles(idAmm, idGroup, extendScope);
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
        private OrganigrammaManagerOCS OrganigrammaManagerOCS
        {
            get
            {
                return this._orgManagerOCS;
            }
        }

        #endregion


        public EsitoOperazione CalcolaAtipicita(OrgRuolo ruolo, string idTipoRuoloVecchio, string idVecchiaUo, bool calcolaSuiSottoposti)
        {
            return this.OrganigrammaManagerETDOCS.CalcolaAtipicita(ruolo, idTipoRuoloVecchio, idVecchiaUo, calcolaSuiSottoposti);
        }
    }
}
