using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using log4net;

namespace DocsPaDocumentale_ETDOCS.Documentale
{
    /// <summary>
    /// Gestione dell'organigramma dell'amministrazione
    /// per il documentale ETDOCS
    /// </summary>
    public class OrganigrammaManager : IOrganigrammaManager
    {
        private ILog logger = LogManager.GetLogger(typeof(OrganigrammaManager));
        #region Ctros, variables, constants

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

        #region Private methods

        /// <summary>
        /// Metodo per la storicizzazione del ruolo
        /// </summary>
        /// <param name="ruolo">Ruolo da storicizzare ig gruppo e id corr globali saranno aggiornate con i nuovi valori</param>
        /// <returns>Nuovo ruolo ottenuto a seguito della storicizzazione del ruolo passato per parametro. Il ruolo passato per parametro avrà un nuovo id corr globali derivante dalla storicizzazione</returns>
        public OrgRuolo HistoricizeRole(OrgRuolo ruolo)
        {
            OrgRuolo newRole = new DocsPaDB.Query_DocsPAWS.Amministrazione().HistoricizeRole(ruolo);
            return newRole;
        }

        #endregion

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione InserisciRuolo(OrgRuolo ruolo, bool computeAtipicita)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            //verifica se il codice è univoco sulla corr_globali
            DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
            if (!obj.CheckCountCondition("DPA_CORR_GLOBALI", "UPPER(VAR_CODICE)=UPPER('" + ruolo.Codice + "') AND ID_AMM=" + ruolo.IDAmministrazione))
            {
                esito.Codice = 1;
                esito.Descrizione = "codice già utilizzato da altro ruolo";
            }
            else
            {
                //verifica se il codice è univoco sulla groups
                if (!obj.CheckCountCondition("GROUPS", "UPPER(GROUP_ID)=UPPER('" + ruolo.Codice + "')"))
                {
                    esito.Codice = 2;
                    esito.Descrizione = "codice già utilizzato da altro gruppo";
                }
                else
                {
                    if (!dbAmm.AmmInsNuovoRuolo(ruolo))
                    {
                        esito.Codice = 3;
                        esito.Descrizione = "si è verificato un errore: inserimento del ruolo";
                    }

                    //verifica se si vuole disabilitare alla ricezione delle trasmissioni un ruolo di riferimento
                    if (ruolo.DiRiferimento.ToString().Equals("1") && ruolo.DisabledTrasm.ToString().Equals("1"))
                    {
                        esito.Codice = 4;
                        esito.Descrizione = "Impossibile disabilitare alla ricezione delle trasmissioni un ruolo di riferimento";
                    }
                }
            }

            // Se è richiesto il calcolo dell'atipicità, viene richiamata la procedura
            if (esito.Codice == 0 && computeAtipicita)
            {
                try
                {
                    using (DocsPaDB.Query_DocsPAWS.Documentale doc = new DocsPaDB.Query_DocsPAWS.Documentale())
                        doc.CalcolaAtipicitaRuoliSottoposti(ruolo);
                }
                catch (Exception e)
                {
                    esito.Codice = 5;
                    esito.Descrizione = "Errore durante il calcolo di atipicità su documenti e fascicoli dei ruoli sottoposti";
                }
            }

            obj = null;
            dbAmm = null;

            return esito;
        }

        /// <summary>
        /// Modifica dei metadati di un ruolo
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns>Ruolo creato a seguito della modifica</returns>
        public EsitoOperazione ModificaRuolo(OrgRuolo ruolo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            if (!dbAmm.AmmModRuolo(ruolo))
            {
                esito.Codice = 1;
                esito.Descrizione = "Si è verificato un errore durante l'aggiornamento del ruolo";
            }

            //verifica se si vuole disabilitare alla ricezione delle trasmissioni un ruolo di riferimento
            if (ruolo.DiRiferimento.ToString().Equals("1") && ruolo.DisabledTrasm.ToString().Equals("1"))
            {
                esito.Codice = 1;
                esito.Descrizione = "Impossibile disabilitare alla ricezione delle trasmissioni un ruolo di riferimento";
            }

            return esito;
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione OnlyDisabledRole(OrgRuolo ruolo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            string result = dbAmm.AmmOnlyDisabledRole(ruolo);

            // possibili valori di ritorno:
            // 1 - il ruolo presenta record nella dpa_doc_arrivo_par
            // 2 - il ruolo presenta record nella dpa_trasm_singola
            // 9 - il ruolo presenta record nella profile
            // gli altri valori non ci interessano in questo caso, verranno ripresi in EliminaRuolo	

            switch (result)
            {
                case "1":
                    esito.Codice = 1;
                    esito.Descrizione = "il ruolo non può essere eliminato ma solo disabilitato\\n\\poiché risulta essere MITTENTE o DESTINATARIO di alcuni documenti protocollati.";
                    break;

                case "2":
                    esito.Codice = 2;
                    esito.Descrizione = "il ruolo non può essere eliminato ma solo disabilitato\\n\\poiché risulta essere DESTINATARIO di trasmissioni.";
                    break;

                case "21":
                    esito.Codice = 9;
                    esito.Descrizione = "il ruolo non può essere eliminato ma solo disabilitato\\n\\poiché risulta essere proprietario di documenti.";
                    break;
            }

            dbAmm = null;
            return esito;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaRuolo(OrgRuolo ruolo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            string result = dbAmm.AmmEliminaRuolo(ruolo);

            // possibili valori di ritorno:
            // 1 - il ruolo presenta record nella dpa_doc_arrivo_par
            // 2 - il ruolo presenta record nella dpa_trasm_singola
            // 21 - il ruolo presenta record nella profile
            // 3 - errore nella cancellazione nella groups
            // 4 - errore nella cancellazione nella dpa_corr_globali
            // 5 - errore nella cancellazione dei registri associati
            // 6 - errore nella cancellazione delle funzioni associati
            // 7 - errore nell'aggiornamento della dta_fine nella dpa_corr_globali
            // 8 - errore nell'aggiornamento della disabilitazione del gruppo
            // 0 - tutto ok!	

            switch (result)
            {
                case "1":
                    esito.Codice = 1;
                    esito.Descrizione = "il ruolo non è stato eliminato ma disabilitato\\n\\poiché risulta essere MITTENTE o DESTINATARIO di alcuni documenti protocollati.";
                    break;

                case "2":
                    esito.Codice = 2;
                    esito.Descrizione = "il ruolo non è stato eliminato ma disabilitato\\n\\poiché risulta essere DESTINATARIO di trasmissioni.";
                    break;

                case "3":
                    esito.Codice = 3;
                    esito.Descrizione = "si è verificato un errore: eliminazione del ruolo (come GRUPPO)";
                    break;

                case "4":
                    esito.Codice = 4;
                    esito.Descrizione = "si è verificato un errore: eliminazione del ruolo (come CORRISPONDENTE)";
                    break;

                case "5":
                    esito.Codice = 5;
                    esito.Descrizione = "si è verificato un errore: cancellazione dei registri associati al ruolo";
                    break;

                case "6":
                    esito.Codice = 6;
                    esito.Descrizione = "si è verificato un errore: cancellazione delle funzioni associate al ruolo";
                    break;

                case "7":
                    esito.Codice = 7;
                    esito.Descrizione = "si è verificato un errore: disabilitazione del ruolo";
                    break;

                case "8":
                    esito.Codice = 8;
                    esito.Descrizione = "si è verificato un errore: disabilitazione del gruppo";
                    break;

                case "21":
                    esito.Codice = 9;
                    esito.Descrizione = "il ruolo non è stato eliminato ma disabilitato\\n\\poiché risulta essere proprietario di documenti.";
                    break;
            }

            // Se il codice di ritorno è minore di 3, si procede con il calcolo di atipicità
            //if (esito.Codice < 3)
                this.CalcolaAtipicitaEliminaRuolo(ruolo);

            dbAmm = null;

            return esito;
        }

        /// <summary>
        /// Metodo per il calcolo dell'atipicità all'eleminazione del ruolo.
        /// Quando si elimina un ruolo, l'atipicità viene calcolata sugli oggetti
        /// visti dai ruoli parilivello e sottoposti del ruolo eliminato
        /// </summary>
        /// <param name="ruolo">Ruolo eliminato</param>
        private void CalcolaAtipicitaEliminaRuolo(OrgRuolo ruolo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione ammManager = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            ammManager.CalcolaAtipicitaEliminaRuolo(ruolo);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        public EsitoOperazione SpostaRuolo(OrgRuolo ruolo)
        {
            logger.Debug("INIZIO Spostamento ruolo - ID Gruppo: " + ruolo.IDGruppo);

            EsitoOperazione retVal = new EsitoOperazione();

            // Viene eseguito il salvataggio delle modifiche
            retVal = this.ModificaRuolo(ruolo);


            //int rowsAffected;
            //string commandText = string.Empty;
            //DocsPaUtils.Query queryDef = null;
            //DocsPaDB.DBProvider dbProvider = null;

            //DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            //DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            ////verifica se il codice è univoco sulla corr_globali
            //DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
            //if (!obj.CheckCountCondition("DPA_CORR_GLOBALI", "UPPER(VAR_CODICE)=UPPER('" + ruolo.Codice + "') AND ID_AMM=" + ruolo.IDAmministrazione + " AND SYSTEM_ID NOT IN (" + ruolo.IDCorrGlobale + ")"))
            //{
            //    esito.Codice = 1;
            //    esito.Descrizione = "codice già utilizzato da altro ruolo";
            //}
            //else
            //{
            //    //verifica se il codice è univoco sulla groups
            //    if (!obj.CheckCountCondition("GROUPS", "UPPER(GROUP_ID)=UPPER('" + ruolo.Codice + "') AND SYSTEM_ID NOT IN (" + ruolo.IDGruppo + ")"))
            //    {
            //        esito.Codice = 2;
            //        esito.Descrizione = "codice già utilizzato da altro gruppo";
            //    }
            //    else
            //    {
            //        //update Groups
            //        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_GROUPS_SPOSTA_RUOLO");
            //        queryDef.setParam("param1", ruolo.Codice.Replace("'", "''"));
            //        queryDef.setParam("param2", ruolo.Descrizione.Replace("'", "''"));
            //        queryDef.setParam("param3", ruolo.IDGruppo);

            //        commandText = queryDef.getSQL();
            //        logger.Debug(commandText);

            //        dbProvider = new DocsPaDB.DBProvider();
            //        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
            //        if (rowsAffected == 0)
            //        {
            //            esito.Codice = 3;
            //            esito.Descrizione = "fallito aggiornamento della tabella dei gruppi";
            //        }
            //        else
            //        {
            //            //update DPA_CORR_GLOBALI
            //            queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_CORR_GLOB_SPOSTA_RUOLO");
            //            queryDef.setParam("param1", ruolo.Codice.Replace("'", "''"));
            //            queryDef.setParam("param2", ruolo.Descrizione.Replace("'", "''"));
            //            queryDef.setParam("param3", ruolo.IDUo);
            //            queryDef.setParam("param4", ruolo.IDCorrGlobale);

            //            commandText = queryDef.getSQL();
            //            logger.Debug(commandText);

            //            dbProvider = new DocsPaDB.DBProvider();
            //            dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
            //            if (rowsAffected == 0)
            //            {
            //                esito.Codice = 4;
            //                esito.Descrizione = "fallito aggiornamento della tabella dei corrispondenti";
            //            }
            //            else
            //            {
            //                //inserimento UO / registri
            //                if (!dbAmm.AmmInsUOReg(ruolo.IDUo))
            //                {
            //                    esito.Codice = 5;
            //                    esito.Descrizione = "errore durante l'aggiornamento dell'associazione tra l'unità organizzativa ed i registri";
            //                }
            //            }
            //        }
            //    }
            //}

            //obj = null;
            //dbAmm = null;

            logger.Debug("FINE Spostamento ruolo - Esito: " + retVal.Codice);
            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione ImpostaRuoloPreferito(string idPeople, string idGruppo)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            using (DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
            {
                if (!dbAmm.AmmImpostaRuoloPreferito(idPeople, idGruppo))
                {
                    esito.Codice = 1;
                    esito.Descrizione = "si è verificato un errore: inserimento ruolo preferito";
                }
            }

            return esito;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione InserisciUtente(OrgUtente utente)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            string result = string.Empty;

            using (DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                result = dbAmm.AmmInsNuovoUtente(utente);

            // possibili valori di ritorno:
            // 1 - userid già presente
            // 2 - codice rubrica già presente		
            // 9 - errore generico
            // 0 - tutto ok!	

            switch (result)
            {   
                case "0":
                    // Modifica dei dati della password
                    this.UpdatePasswordData(esito, utente);
                    break;
                case "1":
                    esito.Codice = 1;
                    esito.Descrizione = "la USERID è già utilizzata da altro utente";
                    break;
                case "2":
                    esito.Codice = 2;
                    esito.Descrizione = "il CODICE RUBRICA è già utilizzato da altro utente";
                    break;
                case "9":
                    esito.Codice = 9;
                    esito.Descrizione = "si è verificato un errore: inserimento nuovo utente";
                    break;
            }

            return esito;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione ModificaUtente(OrgUtente utente)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            string result = string.Empty;
            using (DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                result = dbAmm.AmmModUtente(utente);

            // possibili valori di ritorno:
            // 1 - utente al momento connesso a DocsPA
            // 2 - userid già presente
            // 3 - codice rubrica già presente		
            // 9 - errore generico
            // 0 - tutto ok!	
            switch (result)
            {
                case "0":
                    // Modifica dei dati della password
                    this.UpdatePasswordData(esito, utente);
                    if (!string.IsNullOrEmpty(utente.Password) && esito.Codice == 0)
                        esito.Codice = 4;
                    break;
                case "1":
                    esito.Codice = 1;
                    esito.Descrizione = "utente connesso a DocsPA. Impossibile modificare i dati!";
                    break;
                case "2":
                    esito.Codice = 1;
                    esito.Descrizione = "la USERID è già utilizzata da altro utente";
                    break;
                case "3":
                    esito.Codice = 2;
                    esito.Descrizione = "il CODICE RUBRICA è già utilizzato da altro utente";
                    break;
                case "4":
                    this.UpdatePasswordData(esito, utente);
                    if (!string.IsNullOrEmpty(utente.Password))
                    {
                        if (esito.Codice == 0)
                            esito.Codice = 6;
                    }
                    else
                        esito.Codice = 5;
                    break;
                case "9":
                    esito.Codice = 9;
                    esito.Descrizione = "si è verificato un errore: modifica dati utente";
                    break;
            }

            return esito;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaUtente(OrgUtente utente)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            using (DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
            {
                if (!dbAmm.AmmEliminaUtente(utente))
                {
                    esito.Codice = 1;
                    esito.Descrizione = "si è verificato un errore: eliminazione utente";
                }
            }

            return esito;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione InserisciUtenteInRuolo(string idPeople, string idGruppo)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            using (DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
            {
                if (!dbAmm.AmmInsUtenteInRuolo(idPeople, idGruppo))
                {
                    esito.Codice = 1;
                    esito.Descrizione = "si è verificato un errore: inserimento utente nel ruolo";
                }
            }

            return esito;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public EsitoOperazione EliminaUtenteDaRuolo(string idPeople, string idGruppo)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            bool result = false;

            using (DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
            {

                result = dbAmm.AmmEliminaUtenteInRuolo(idPeople, idGruppo);

                if (!result)
                {
                    esito.Codice = 1;
                    esito.Descrizione = "si è verificato un errore: disabilitazione associazione utente in ruolo";
                }
                //Se la cancellazione dell'utente dal ruolo è andata a buon fine e è abilitata da chiave di amministrazione
                //la gestione delle qualifiche, procedo ad eliminare tutte le qualifiche per il dato utente nel dato ruolo
                else
                {

                    string chiaveQualifiche = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "GESTIONE_QUALIFICHE");

                    if (!string.IsNullOrEmpty(chiaveQualifiche) && chiaveQualifiche.Equals("1"))
                    {
                        result = dbAmm.AmmEliminaQualificheUtenteInRuolo(idPeople, idGruppo);

                        if (!result)
                        {
                            esito.Codice = 1;
                            esito.Descrizione = "si è verificato un errore: disabilitazione qualifiche per utente in ruolo";
                        }
                    }

                }
            }

            return esito;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="copyVisibility"></param>
        /// <returns></returns>
        public EsitoOperazione CopyVisibility(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Security.CopyVisibility copyVisibility)
        {
            EsitoOperazione esitoOperazione = new EsitoOperazione();

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
                    esitoOperazione = documentale.CopyVisibility(infoUtente, copyVisibility);
                    transactionContext.Complete();
                    return esitoOperazione;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in OrganigrammaManager  - metodo: CopyVisibility", e);
                    esitoOperazione.Codice = -1;
                    esitoOperazione.Descrizione = "Problemi durante la copia della visibilità ETDOCS. Cosultare il file di log.";
                    return esitoOperazione;
                }
            }
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
            EsitoOperazione retVal = new EsitoOperazione();
            
            using(DocsPaDB.Query_DocsPAWS.Documentale doc = new DocsPaDB.Query_DocsPAWS.Documentale())
	        {
                bool result = doc.ExtendVisibilityToHigherRoles(idAmm, idGroup, extendScope, false);

                retVal.Codice = result ? 0 : -1;
                retVal.Descrizione = result ? String.Empty : "Si è verificato un errore durante l'estensione della visibilità di documenti e fascicoli ai superiori gerarchici";
	        }

    	    
            return retVal;
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

        #region Gestione scadenza password

        /// <summary>
        /// Aggiornamento dei dati della password di tipo amministratore per l'utente
        /// </summary>
        /// <param name="esito"></param>
        /// <param name="utente"></param>
        protected void UpdatePasswordData(EsitoOperazione esito, OrgUtente utente)
        {
            AdminPasswordConfig pwdConfig = new AdminPasswordConfig();

            if (pwdConfig.IsSupportedPasswordConfig())
            {
                int idAmministrazione = Convert.ToInt32(utente.IDAmministrazione);
                
                if (!string.IsNullOrEmpty(utente.Password))
                {
                    // Se l'inserimento dell'utente è andato a buon fine, 
                    // viene inserita la password di tipo amministratore
                    // (solo se supportata la configurazione password)
                    DocsPaVO.Validations.ValidationResultInfo ret =
                        DocsPaPwdServices.UserPasswordServices.SetPassword(idAmministrazione, utente.UserId, utente.Password, true);
                    
                    if (!ret.Value)
                    {
                        esito.Codice = 9;
                        esito.Descrizione = ret.BrokenRules[0].ToString();
                    }
                }

                if (esito.Codice == 0)
                {
                    try
                    {
                        // Aggiornamento valore per flag "Nessuna scadenza password"
                        DocsPaPwdServices.UserPasswordServices.SetPasswordNeverExpireOption(utente.NessunaScadenzaPassword, utente.UserId);
                    }
                    catch
                    {
                        esito.Codice = 9;
                        esito.Descrizione = "Errore nella modifica dei dati della password";
                    }
                }

            }
        }

        #endregion

        #endregion

        public EsitoOperazione CalcolaAtipicita(OrgRuolo ruolo, string idTipoRuoloVecchio, string idVecchiaUo, bool calcolaSuiSottoposti)
        {
            EsitoOperazione result = new EsitoOperazione();

            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            if (!amm.CalcolaAtipicita(ruolo, idTipoRuoloVecchio, idVecchiaUo, calcolaSuiSottoposti))
                result = new EsitoOperazione() { Codice = -1, Descrizione = "Errore durante il calcolo di atipicità" };

            return result;
        }
    }
}
