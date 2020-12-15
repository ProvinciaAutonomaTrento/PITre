using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using DocsPaDocumentale_OCS.CorteContentServices;
using DocsPaDocumentale_OCS.OCSServices;
using log4net;

namespace DocsPaDocumentale_OCS.Documentale
{
    /// <summary>
    /// Gestione dell'organigramma dell'amministrazione di OCS
    /// </summary>
    public class OrganigrammaManager : IOrganigrammaManager
    {
        private ILog logger = LogManager.GetLogger(typeof(OrganigrammaManager));

        #region Ctors, constants, variables

        /// <summary>
        /// String usata come preambolo per i messaggi di errore (user-level) di questa classe
        /// </summary>
        private const string ERR_HEADER = "Errore in OCS: ";

        /// <summary>
        /// String usata come preambolo per i messaggi di debug di questa classe
        /// </summary>
        private const string DEBUG_HEADER = "DocsPaDocumentale_OCS: ";

        /// <summary>
        /// Credenziali utente
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// Istanza webservice per la gestione dei gruppi in OCS
        /// </summary>
        private GroupManagementSOAPHTTPBinding _wsGroupInstance = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public OrganigrammaManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
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
            EsitoOperazione ret = new EsitoOperazione();
            string logMsg;

            // test sui campi obbligatori
            if (string.IsNullOrEmpty(ruolo.Codice) ||
                string.IsNullOrEmpty(ruolo.Descrizione))
            {
                ret.Codice = -1;
                logMsg = ERR_HEADER + "InserisciRuolo: dati insufficienti";
                ret.Descrizione = logMsg;
                logger.Debug(logMsg);
                return ret;
            }

            // il campo Codice corrisponde a:
            //  (ETDOCS) DPA_CORR_GLOBALI.VAR_COD_RUBRICA varchar(128)
            //  (OCS)   

            try
            {
                CorteContentServices.GroupRequestType groupReq = new CorteContentServices.GroupRequestType();
                groupReq.userCredentials = OCSUtils.getApplicationUserCredentials();
                groupReq.group = new CorteContentServices.GroupType();
                groupReq.group.name = ruolo.Codice;
                groupReq.group.users = new CorteContentServices.UserType[0];
                groupReq.group.description = ruolo.Descrizione;

                CorteContentServices.ResultType result = this.WsGroupInstance.CreateGroup(groupReq);

                if (OCSUtils.isValidServiceResult(result))
                    ret.Codice = 0;
                else
                {
                    ret.Codice = -1;
                    ret.Descrizione = result.message;
                }

                return ret;

            }
            catch (Exception ex)
            {
                String st = ex.ToString();
                logger.Debug(DEBUG_HEADER + "InserisciRuolo FALLITA, Exception=" + st);
                ret.Codice = -1;
                ret.Descrizione = ERR_HEADER + "InserisciRuolo";
                return ret;
            }
        }

        /// <summary>
        /// Modifica dei metadati di un ruolo
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione ModificaRuolo(OrgRuolo ruolo)
        {
            EsitoOperazione ret = new EsitoOperazione();
            string logMsg;

            // test sui campi obbligatori
            if (string.IsNullOrEmpty(ruolo.Codice) ||
                string.IsNullOrEmpty(ruolo.Descrizione))
            {
                ret.Codice = -1;
                logMsg = ERR_HEADER + "ModificaRuolo: dati insufficienti";
                ret.Descrizione = logMsg;
                logger.Debug(logMsg);
                return ret;
            }

            // il campo Codice corrisponde a:
            //  (ETDOCS) DPA_CORR_GLOBALI.VAR_COD_RUBRICA varchar(128)
            //  (OCS)   

            try
            {
                CorteContentServices.GroupRequestType groupReq = new CorteContentServices.GroupRequestType();
                groupReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                groupReq.group = new CorteContentServices.GroupType();
                groupReq.group.name = ruolo.Codice;
                groupReq.group.description = ruolo.Descrizione;
                groupReq.group.users = new DocsPaDocumentale_OCS.CorteContentServices.UserType[0];

                CorteContentServices.ResultType result = this.WsGroupInstance.ModifyGroup(groupReq);

                if (OCSUtils.isValidServiceResult(result))
                    ret.Codice = 0;
                else
                {
                    logger.Debug(DEBUG_HEADER + "ModificaRuolo operazione con ERRORE su OCS: " + ret.Descrizione);
                    ret.Codice = -1;
                    ret.Descrizione = result.message.Replace("'"," ");
                }

                return ret;
            }
            catch (Exception ex)
            {
                string st = ex.ToString();
                logger.Debug(DEBUG_HEADER + "ModificaRuolo FALLITA, Exception=" + st);
                ret.Codice = -1;
                ret.Descrizione = ERR_HEADER + "ModificaRuolo";
                return ret;
            }
        }

        public EsitoOperazione OnlyDisabledRole(OrgRuolo ruolo)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            esito.Codice = 0;
            return esito;
        }
        /// <summary>
        /// Cancellazione di un ruolo in amministrazione
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaRuolo(OrgRuolo ruolo)
        {
            EsitoOperazione ret = new EsitoOperazione();
            string logMsg;

            // test sui campi obbligatori
            if (string.IsNullOrEmpty(ruolo.Codice))
            {
                logMsg = ERR_HEADER + "EliminaRuolo: dati insufficienti";
                ret.Codice = -1;
                ret.Descrizione = logMsg;
                logger.Debug(logMsg);
                return ret;
            }

            try
            {
                CorteContentServices.GroupDefinitionRequestType groupReq = new CorteContentServices.GroupDefinitionRequestType();
                CorteContentServices.ResultType result;
                groupReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                groupReq.groupDefinition = new CorteContentServices.GroupDefinitionType();
                groupReq.groupDefinition.name = ruolo.Codice;
                groupReq.groupDefinition.description = ruolo.Descrizione;

                result = this.WsGroupInstance.DeleteGroup(groupReq);
                
                if (OCSUtils.isValidServiceResult(result))
                {
                    logger.Debug(DEBUG_HEADER + "EliminaRuolo completata con SUCCESSO");
                    ret.Codice = 0;
                }
                else
                {
                    logger.Debug(DEBUG_HEADER + "EliminaRuolo operazione con ERRORE su OCS: " + result.message);
                    ret.Codice = -1;
                    ret.Descrizione = result.message.Replace("'"," ");
                }

                return ret;
            }
            catch (Exception ex)
            {
                String st = ex.ToString();
                logger.Debug(DEBUG_HEADER + "EliminaRuolo FALLITA, Exception=" + st);
                ret.Codice = -1;
                ret.Descrizione = ERR_HEADER + "EliminaRuolo";
                return ret;
            }
        }
 
        /// <summary>
        /// Spostamento di un ruolo in amministrazione
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione SpostaRuolo(OrgRuolo ruolo)
        {
            throw new NotSupportedException("SpostaRuolo: operazione non supportata in OCS");
        }

        /// <summary>
        /// Impostazione di un ruolo come predefinito
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione ImpostaRuoloPreferito(string idPeople, string idGruppo)
        {
            throw new NotSupportedException("ImpostaRuoloPreferito: operazione non supportata in OCS");
        }

        /// <summary>
        /// Inserimento di un nuovo utente in amministrazione. L'utente verrà inserito in un gruppo che contiene tutti gli utenti DocsPA.
        /// il gruppo deve già esistere su OCS!!!
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione InserisciUtente(OrgUtente utente)
        {
            //inserisce l'utente nel gruppo dei lettori DocsPA
            EsitoOperazione ret = new EsitoOperazione();
            try
            {
                
                string codiceRuolo = OCSConfigurations.GetGroupUsers();
                string codiceUtente = utente.CodiceRubrica;
                CorteContentServices.GroupRequestType groupReq = new CorteContentServices.GroupRequestType();
                CorteContentServices.ResultType result;
                groupReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                groupReq.group = new CorteContentServices.GroupType();
                groupReq.group.name = codiceRuolo;
                //bisogna specificare la descrizione perchè l'elemento non è facoltativo al momento
                groupReq.group.description = "";
                groupReq.group.users = new CorteContentServices.UserType[1];
                groupReq.group.users[0] = new CorteContentServices.UserType();
                groupReq.group.users[0].userId = codiceUtente;
                
                result = this.WsGroupInstance.AddGroupUsers(groupReq);
                
                if (OCSUtils.isValidServiceResult(result))
                {
                    logger.Debug(DEBUG_HEADER + "Inserimento utente nel gruppo DocsPa: " + codiceRuolo + " completata con SUCCESSO");
                    ret.Codice = 0;
                }
                else
                {
                    logger.Debug(DEBUG_HEADER + "Inserimento utente nel gruppo DocsPa: " + codiceRuolo + " operazione con ERRORE");
                    ret.Codice = -1;
                    ret.Descrizione = "problema su OCS: " + result.message.Replace("'"," ");
                }

                return ret;
            }
            catch (Exception ex)
            {
                String st = ex.ToString();
                logger.Debug(DEBUG_HEADER + "InserisciUtenteInRuolo FALLITA, Exception=" + st);
                ret.Codice = -1;
                ret.Descrizione = ERR_HEADER + "InserisciUtenteInRuolo";
                return ret;
            }
            //throw new NotSupportedException("InserisciUtente: operazione non supportata in OCS");
        }


        /// <summary>
        /// Modifica dei dati di un utente in amministrazione
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione ModificaUtente(OrgUtente utente)
        {
            throw new NotSupportedException("ModificaUtente: operazione non supportata in OCS");
        }

        /// <summary>
        /// Elimina un utente in amministrazione
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaUtente(OrgUtente utente)
        {
            EsitoOperazione ret = new EsitoOperazione();

            try
            {
                string codiceRuolo = OCSConfigurations.GetGroupUsers();
                string codiceUtente = utente.UserId;
                CorteContentServices.GroupRequestType groupReq = new CorteContentServices.GroupRequestType();
                CorteContentServices.ResultType result;
                groupReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                groupReq.group = new CorteContentServices.GroupType();
                groupReq.group.name = codiceRuolo;
                //bisogna specificare la descrizione perchè l'elemento non è facoltativo al momento
                groupReq.group.description = "";
                groupReq.group.users = new CorteContentServices.UserType[1];
                groupReq.group.users[0] = new CorteContentServices.UserType();
                groupReq.group.users[0].userId = codiceUtente;

                result = this.WsGroupInstance.RemoveGroupUsers(groupReq);

                if (OCSUtils.isValidServiceResult(result))
                {
                    logger.Debug(DEBUG_HEADER + "EliminaUtente completata con SUCCESSO");
                    ret.Codice = 0;
                }
                else
                {
                    logger.Debug(DEBUG_HEADER + "EliminaUtente operazione con ERRORE su OCS: " + result.message);
                    ret.Codice = -1;
                    ret.Descrizione = "Errore in OCS: " + result.message.Replace("'", " ");
                }

                return ret;
            }
            catch (Exception ex)
            {
                String st = ex.ToString();
                logger.Debug(DEBUG_HEADER + "EliminaUtente FALLITA, Exception=" + st);
                ret.Codice = -1;
                ret.Descrizione = ERR_HEADER + "EliminaUtente";
                return ret;
            }
        }

        /// <summary>
        /// Inserimento di un utente in un ruolo
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione InserisciUtenteInRuolo(string idPeople, string idGruppo)
        {
            EsitoOperazione ret = new EsitoOperazione();

            try
            {

                string codiceRuolo = DocsPaServices.DocsPaQueryHelper.getCodiceRuoloFromIdGroup(idGruppo);
                string codiceUtente = DocsPaServices.DocsPaQueryHelper.getCodiceUtente(idPeople);
                CorteContentServices.GroupRequestType groupReq = new CorteContentServices.GroupRequestType();
                CorteContentServices.ResultType result;
                groupReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                groupReq.group = new CorteContentServices.GroupType();
                groupReq.group.name = codiceRuolo;
                //bisogna specificare la descrizione perchè l'elemento non è facoltativo al momento
                groupReq.group.description = "";
                groupReq.group.users = new CorteContentServices.UserType[1];
                groupReq.group.users[0] = new CorteContentServices.UserType();
                groupReq.group.users[0].userId = codiceUtente;

                result = this.WsGroupInstance.AddGroupUsers(groupReq);
                
                if (OCSUtils.isValidServiceResult(result))
                {
                    logger.Debug(DEBUG_HEADER + "InserisciUtenteInRuolo completata con SUCCESSO");
                    ret.Codice = 0;
                }
                else
                {
                    logger.Debug(DEBUG_HEADER + "InserisciUtenteInRuolo operazione con ERRORE");
                    ret.Codice = -1;
                    ret.Descrizione = "problema su OCS: " + result.message.Replace("'", " ");
                }

                return ret;
            }
            catch (Exception ex)
            {
                String st = ex.ToString();
                logger.Debug(DEBUG_HEADER + "InserisciUtenteInRuolo FALLITA, Exception=" + st);
                ret.Codice = -1;
                ret.Descrizione = ERR_HEADER + "InserisciUtenteInRuolo";
                return ret;
            }
        }

        /// <summary>
        /// Eliminazione di un utente da un ruolo
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaUtenteDaRuolo(string idPeople, string idGruppo)
        {
            EsitoOperazione ret = new EsitoOperazione();

            try
            {

                string codiceRuolo = DocsPaServices.DocsPaQueryHelper.getCodiceRuoloFromIdGroup(idGruppo);
                string codiceUtente = DocsPaServices.DocsPaQueryHelper.getCodiceUtente(idPeople);
                CorteContentServices.GroupRequestType groupReq = new CorteContentServices.GroupRequestType();
                CorteContentServices.ResultType result;
                groupReq.userCredentials = OCSServices.OCSUtils.getApplicationUserCredentials();
                groupReq.group = new CorteContentServices.GroupType();
                groupReq.group.name = codiceRuolo;
                //bisogna specificare la descrizione perchè l'elemento non è facoltativo al momento
                groupReq.group.description = "";
                groupReq.group.users = new CorteContentServices.UserType[1];
                groupReq.group.users[0] = new CorteContentServices.UserType();
                groupReq.group.users[0].userId = codiceUtente;

                result = this.WsGroupInstance.RemoveGroupUsers(groupReq);
                
                if (OCSUtils.isValidServiceResult(result))
                {
                    logger.Debug(DEBUG_HEADER + "EliminaUtenteDaRuolo completata con SUCCESSO");
                    ret.Codice = 0;
                }
                else
                {
                    logger.Debug(DEBUG_HEADER + "EliminaUtenteDaRuolo operazione con ERRORE su OCS: " + result.message);
                    ret.Codice = -1;
                    ret.Descrizione = "Errore in OCS: " + result.message.Replace("'", " ");
                }

                return ret;
            }
            catch (Exception ex)
            {
                String st = ex.ToString();
                logger.Debug(DEBUG_HEADER + "EliminaUtenteDaRuolo FALLITA, Exception=" + st);
                ret.Codice = -1;
                ret.Descrizione = ERR_HEADER + "EliminaUtenteDaRuolo";
                return ret;
            }
        }

        /// <summary>
        /// Copia visibilità da ruolo a ruolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="copyVisibility"></param>
        /// <returns></returns>
        public EsitoOperazione CopyVisibility(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Security.CopyVisibility copyVisibility)
        {
            throw new NotSupportedException("CopyCopyVisibility: operazione non supportata in OCS");
        }

        /// <summary>
        /// Operazione non consentita in OCS
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public OrgRuolo HistoricizeRole(OrgRuolo role)
        {
            throw new NotSupportedException("HistoricizeRole: operazione non supportata in OCS");

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
            throw new NotSupportedException("HistoricizeRole: operazione non supportata in OCS");
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
        /// Reperimento istanza webservice per la gestione dei gruppi in OCS
        /// </summary>
        protected GroupManagementSOAPHTTPBinding WsGroupInstance
        {
            get
            {
                if (this._wsGroupInstance == null)
                    this._wsGroupInstance = OCSServiceFactory.GetDocumentServiceInstance<GroupManagementSOAPHTTPBinding>();
                return this._wsGroupInstance;
            }
        }

        #endregion


        public EsitoOperazione CalcolaAtipicita(OrgRuolo ruolo, string idTipoRuoloVecchio, string idVecchiaUo, bool calcolaSuiSottoposti)
        {
            throw new NotSupportedException("CalcolaAtipicita: operazione non supportata in OCS");
        }
    }
}
