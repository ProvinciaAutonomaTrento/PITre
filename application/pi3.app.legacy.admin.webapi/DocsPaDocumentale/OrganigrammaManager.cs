// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaDocumentale.Documentale
{
    public class OrganigrammaManager : IOrganigrammaManager
    {
        private static Type _type = null;

        private IOrganigrammaManager _instance = null;

        static OrganigrammaManager()
        {
            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["documentale"]))
            //{
            //    string documentale = ConfigurationManager.AppSettings["documentale"].ToLower();

            //    if (documentale.Equals(TipiDocumentaliEnum.Etnoteam.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.OrganigrammaManager);
            //    else if (documentale.Equals(TipiDocumentaliEnum.Hummingbird.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_HUMMINGBIRD.Documentale.OrganigrammaManager);
            //    else if (documentale.Equals(TipiDocumentaliEnum.Filenet.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_FILENET.Documentale.OrganigrammaManager);
            //    else if (documentale.Equals(TipiDocumentaliEnum.Pitre.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_PITRE.Documentale.OrganigrammaManager);
            //    else if (documentale.Equals(TipiDocumentaliEnum.CDC.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_CDC.Documentale.OrganigrammaManager);
            //    else if (documentale.Equals(TipiDocumentaliEnum.GFD.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_GFD.Documentale.OrganigrammaManager);

            //    //Giordano Iacozzilli  08/10/2012 Aggiunta strato SharePoint
            //    else if (documentale.Equals(TipiDocumentaliEnum.SharePoint.ToString().ToLower()))
            //        _type = typeof(DocsPaDocumentale_CDC_SP.Documentale.OrganigrammaManager);
            //    //Fine
            //}
            _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.OrganigrammaManager);
        }

        protected IOrganigrammaManager Instance
        {
            get
            {
                return this._instance;
            }
        }


        public OrganigrammaManager(InfoUtente infoUtente)
        {
            this._instance = (IOrganigrammaManager)Activator.CreateInstance(_type, infoUtente);
        }

        public EsitoOperazione InserisciRuolo(OrgRuolo ruolo, bool computeAtipicita)
        {
            return this.Instance.InserisciRuolo(ruolo, computeAtipicita);
        }

        public EsitoOperazione ModificaRuolo(OrgRuolo ruolo)
        {
            return this.Instance.ModificaRuolo(ruolo);
        }

        public EsitoOperazione OnlyDisabledRole(OrgRuolo ruolo)
        {
            return this.Instance.OnlyDisabledRole(ruolo);
        }

        public EsitoOperazione EliminaRuolo(OrgRuolo ruolo)
        {
            return this.Instance.EliminaRuolo(ruolo);
        }

        public EsitoOperazione SpostaRuolo(OrgRuolo ruolo)
        {
            return this.Instance.SpostaRuolo(ruolo);
        }

        public EsitoOperazione ImpostaRuoloPreferito(string idPeople, string idGruppo)
        {
            return this.Instance.ImpostaRuoloPreferito(idPeople, idGruppo);
        }

        public EsitoOperazione InserisciUtente(OrgUtente utente)
        {
            return this.Instance.InserisciUtente(utente);
        }

        public EsitoOperazione ModificaUtente(OrgUtente utente)
        {
            return this.Instance.ModificaUtente(utente);
        }

        public EsitoOperazione EliminaUtente(OrgUtente utente)
        {
            return this.Instance.EliminaUtente(utente);
        }

        public EsitoOperazione InserisciUtenteInRuolo(string idPeople, string idGruppo)
        {
            return this.Instance.InserisciUtenteInRuolo(idPeople, idGruppo);
        }

        public EsitoOperazione EliminaUtenteDaRuolo(string idPeople, string idGruppo)
        {
            return this.Instance.EliminaUtenteDaRuolo(idPeople, idGruppo);
        }

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

        public EsitoOperazione CalcolaAtipicita(OrgRuolo ruolo, string idTipoRuoloVecchio, string idVecchiaUo, bool calcolaSuiSottoposti)
        {
            return this.Instance.CalcolaAtipicita(ruolo, idTipoRuoloVecchio, idVecchiaUo, calcolaSuiSottoposti);
        }


    }
}
