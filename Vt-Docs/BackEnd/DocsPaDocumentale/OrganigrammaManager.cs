using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;

namespace DocsPaDocumentale.Documentale
{
    /// <summary>
    /// Gestione dell'organigramma dell'amministrazione
    /// </summary>
    public class OrganigrammaManager : IOrganigrammaManager
    {
        #region Ctros, variables, constants

        /// <summary>
        /// Tipo documentale corrente
        /// </summary>
        private static Type _type = null;

        /// <summary>
        /// Oggetto documentale corrente
        /// </summary>
        private IOrganigrammaManager _instance = null;

        /// <summary>
        /// Reperimento del tipo relativo al documentale corrente
        /// </summary>
        static OrganigrammaManager()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["documentale"]))
            {
                string documentale = ConfigurationManager.AppSettings["documentale"].ToLower();

                if (documentale.Equals(TipiDocumentaliEnum.Etnoteam.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.OrganigrammaManager);
                else if (documentale.Equals(TipiDocumentaliEnum.Hummingbird.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_HUMMINGBIRD.Documentale.OrganigrammaManager);
                else if (documentale.Equals(TipiDocumentaliEnum.Filenet.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_FILENET.Documentale.OrganigrammaManager);
                else if (documentale.Equals(TipiDocumentaliEnum.Pitre.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_PITRE.Documentale.OrganigrammaManager);
                else if (documentale.Equals(TipiDocumentaliEnum.CDC.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC.Documentale.OrganigrammaManager);
                else if (documentale.Equals(TipiDocumentaliEnum.GFD.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_GFD.Documentale.OrganigrammaManager);
            
                //Giordano Iacozzilli  08/10/2012 Aggiunta strato SharePoint
                else if (documentale.Equals(TipiDocumentaliEnum.SharePoint.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC_SP.Documentale.OrganigrammaManager);
                //Fine
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public OrganigrammaManager(InfoUtente infoUtente)
        {
            this._instance = (IOrganigrammaManager)Activator.CreateInstance(_type, infoUtente);
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
            return this.Instance.InserisciRuolo(ruolo, computeAtipicita);
        }

        /// <summary>
        /// Modifica dei metadati di un ruolo
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione ModificaRuolo(OrgRuolo ruolo)
        {
            return this.Instance.ModificaRuolo(ruolo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione OnlyDisabledRole(OrgRuolo ruolo)
        {
            return this.Instance.OnlyDisabledRole(ruolo);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaRuolo(OrgRuolo ruolo)
        {
            return this.Instance.EliminaRuolo(ruolo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione SpostaRuolo(OrgRuolo ruolo)
        {
            return this.Instance.SpostaRuolo(ruolo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione InserisciUtente(OrgUtente utente)
        {
            return this.Instance.InserisciUtente(utente);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione ModificaUtente(OrgUtente utente)
        {
            return this.Instance.ModificaUtente(utente);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaUtente(OrgUtente utente)
        {
            return this.Instance.EliminaUtente(utente);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione InserisciUtenteInRuolo(string idPeople, string idGruppo)
        {
            return this.Instance.InserisciUtenteInRuolo(idPeople, idGruppo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaUtenteDaRuolo(string idPeople, string idGruppo)
        {
            return this.Instance.EliminaUtenteDaRuolo(idPeople, idGruppo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="copyVisibility"></param>
        /// <returns></returns>
        public EsitoOperazione CopyVisibility(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Security.CopyVisibility copyVisibility)
        {
            return this.Instance.CopyVisibility(infoUtente, copyVisibility);
        }

        public OrgRuolo HistoricizeRole(OrgRuolo role)
        {
            return this.Instance.HistoricizeRole(role);

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
            return this.Instance.ExtendVisibilityToHigherRoles(idAmm, idGroup, extendScope);
        }


        #endregion

        #region Protected methods

        /// <summary>
        /// Oggetto documentale corrente
        /// </summary>
        protected IOrganigrammaManager Instance
        {
            get
            {
                return this._instance;
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione ImpostaRuoloPreferito(string idPeople, string idGruppo)
        {
            return this.Instance.ImpostaRuoloPreferito(idPeople, idGruppo);
        }



        public EsitoOperazione CalcolaAtipicita(OrgRuolo ruolo, string idTipoRuoloVecchio, string idVecchiaUo, bool calcolaSuiSottoposti)
        {
            return this.Instance.CalcolaAtipicita(ruolo, idTipoRuoloVecchio, idVecchiaUo, calcolaSuiSottoposti);
        }
    }
}