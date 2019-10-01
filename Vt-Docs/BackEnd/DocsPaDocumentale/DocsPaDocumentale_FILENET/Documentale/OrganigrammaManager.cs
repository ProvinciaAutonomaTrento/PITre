using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;

namespace DocsPaDocumentale_FILENET.Documentale
{
    /// <summary>
    /// Gestione dell'organigramma dell'amministrazione.
    /// Delega l'implementazione ai servizi esposti 
    /// dall'assembly DocsPaAdmin_ETDOCS.
    /// </summary>
    public class OrganigrammaManager : IOrganigrammaManager
    {
        #region Ctros, variables, constants

        private IOrganigrammaManager _instance = null;

        /// <summary>
        /// Credenziali utente
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public OrganigrammaManager(InfoUtente infoUtente)
        {
            this._infoUtente = InfoUtente;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione InserisciRuolo(OrgRuolo ruolo, bool computeAtipicita)
        {
            return this.InstanceETDOCS.InserisciRuolo(ruolo, computeAtipicita);
        }

        /// <summary>
        /// Modifica dei metadati di un ruolo
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione ModificaRuolo(OrgRuolo ruolo)
        {
            return this.InstanceETDOCS.ModificaRuolo(ruolo);
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
            EsitoOperazione result = this.InstanceETDOCS.OnlyDisabledRole(ruolo);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaRuolo(OrgRuolo ruolo)
        {
            return this.InstanceETDOCS.EliminaRuolo(ruolo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione SpostaRuolo(OrgRuolo ruolo)
        {
            return this.InstanceETDOCS.SpostaRuolo(ruolo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione ImpostaRuoloPreferito(string idPeople, string idGruppo)
        {
            return this.InstanceETDOCS.ImpostaRuoloPreferito(idPeople, idGruppo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione InserisciUtente(OrgUtente utente)
        {
            EsitoOperazione result = this.InstanceETDOCS.InserisciUtente(utente);

            if (result.Codice == 0)
            {
                UserManager userManager = new UserManager();
                
                string defaultFNETGroup = System.Configuration.ConfigurationManager.AppSettings["FNET_userGroup"];
                bool retValue = userManager.AddUserFilenet(utente.UserId, utente.Password, utente.IDAmministrazione, string.Format("{0} {1}", utente.Cognome, utente.Nome), defaultFNETGroup);

                if (!retValue)
                {
                    result.Codice = 9;
                    result.Descrizione = "si è verificato un errore: inserimento nuovo utente in FILENET";
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione ModificaUtente(OrgUtente utente)
        {
            // memorizzo la vecchia PWD per Filenet
            DocsPaDB.Query_DocsPAWS.Utenti user = new DocsPaDB.Query_DocsPAWS.Utenti();
            string oldPwdFileNet = user.GetPasswordUserFilenet(utente.UserId);
            
            EsitoOperazione result = this.InstanceETDOCS.ModificaUtente(utente);

            if (result.Codice == 0)
            {
                UserManager userManager = new UserManager();
                bool retValue = userManager.UpdateUserFilenet(utente.UserId, oldPwdFileNet, utente.Password, string.Format("{0} {1}", utente.Cognome, utente.Nome), utente.IDAmministrazione);
                if (!retValue)
                {
                    result.Codice = 10;
                    result.Descrizione = "si è verificato un errore: modifica dati utente in FILENET";
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaUtente(OrgUtente utente)
        {
            EsitoOperazione result = this.InstanceETDOCS.EliminaUtente(utente);

            if (result.Codice == 0)
            {
                UserManager userManager = new UserManager();
                if (!userManager.DisableUserFilenet(utente.UserId))
                {
                    result.Codice = 1;
                    result.Descrizione = "si è verificato un errore: eliminazione utente in FILENET";
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione InserisciUtenteInRuolo(string idPeople, string idGruppo)
        {
            return this.InstanceETDOCS.InserisciUtenteInRuolo(idPeople, idGruppo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaUtenteDaRuolo(string idPeople, string idGruppo)
        {
            return this.InstanceETDOCS.EliminaUtenteDaRuolo(idPeople, idGruppo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="copyVisibility"></param>
        /// <returns></returns>
        public EsitoOperazione CopyVisibility(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Security.CopyVisibility copyVisibility)
        {
            return this.InstanceETDOCS.CopyVisibility(infoUtente, copyVisibility);
        }

        public OrgRuolo HistoricizeRole(OrgRuolo role)
        {
            return this.InstanceETDOCS.HistoricizeRole(role);

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
            return this.InstanceETDOCS.ExtendVisibilityToHigherRoles(idAmm, idGroup, extendScope);
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
        protected IOrganigrammaManager InstanceETDOCS
        {
            get
            {
                if (this._instance == null)
                    this._instance = new DocsPaDocumentale_ETDOCS.Documentale.OrganigrammaManager(this.InfoUtente);
                return this._instance;
            }
        }

        #endregion


        public EsitoOperazione CalcolaAtipicita(OrgRuolo ruolo, string idTipoRuoloVecchio, string idVecchiaUo, bool calcolaSuiSottoposti)
        {
            return this.InstanceETDOCS.CalcolaAtipicita(ruolo, idTipoRuoloVecchio, idVecchiaUo, calcolaSuiSottoposti);
        }
    }
}
