using System;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using DocsPaDbManagement.Functions;
using log4net;
using DocsPaVO.Notification;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Summary description for SmistamentoDocumenti.
    /// </summary>
    public class SmistamentoDocumenti : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(SmistamentoDocumenti));
        public SmistamentoDocumenti()
        {
        }

        #region PUBLIC

        /// <summary>
        /// verifica l'esistenza delle ragioni di trasmissione: COMPETENZA e CONOSCENZA
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public DataSet VerificaRagTrasmSmista(string idAmm)
        {
            #region OLD
            //DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_VER_RAG_TRASM");
            //q.setParam("param1", idAmm);

            //string queryString = q.getSQL();
            //logger.Debug(queryString);

            //DataSet ds = new DataSet();

            //this.ExecuteQuery(out ds, "RAG_TRASM", queryString);
            //return ds;
            #endregion

            DataSet ds = new DataSet();
            DataSet dsRagioni = new DataSet();

            // COMPETENZA           
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministra2");
            q.setParam("param1", "ID_RAGIONE_COMPETENZA");
            q.setParam("param2", idAmm);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            string idOut;
            this.ExecuteScalar(out idOut, queryString);

            if (idOut != null && idOut != "")
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPARagTrasm_ALL");
                q.setParam("param1", "*");
                q.setParam("param2", "WHERE SYSTEM_ID = " + idOut); // competenza

                queryString = q.getSQL();
                logger.Debug(queryString);

                this.ExecuteQuery(out dsRagioni, "RAG_TRASM", queryString);
            }

            if (dsRagioni != null && dsRagioni.Tables["RAG_TRASM"].Rows.Count > 0)
            {
                // CONOSCENZA           
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministra2");
                q.setParam("param1", "ID_RAGIONE_CONOSCENZA");
                q.setParam("param2", idAmm);

                queryString = q.getSQL();
                logger.Debug(queryString);

                this.ExecuteScalar(out idOut, queryString);

                if (idOut != null && idOut != "")
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPARagTrasm_ALL");
                    q.setParam("param1", "*");
                    q.setParam("param2", "WHERE SYSTEM_ID = " + idOut); // conoscenza

                    queryString = q.getSQL();
                    logger.Debug(queryString);

                    this.ExecuteQuery(out ds, "RAG_COMP", queryString);

                    foreach (DataRow r in ds.Tables["RAG_COMP"].Rows)
                    {
                        DataRow newRow = dsRagioni.Tables["RAG_TRASM"].NewRow();
                        newRow["SYSTEM_ID"] = r["SYSTEM_ID"];
                        newRow["VAR_DESC_RAGIONE"] = r["VAR_DESC_RAGIONE"];
                        newRow["CHA_TIPO_RAGIONE"] = r["CHA_TIPO_RAGIONE"];
                        newRow["CHA_TIPO_DIRITTI"] = r["CHA_TIPO_DIRITTI"];
                        newRow["CHA_TIPO_DEST"] = r["CHA_TIPO_DEST"];
                        newRow["CHA_RISPOSTA"] = r["CHA_RISPOSTA"];
                        newRow["VAR_NOTE"] = r["VAR_NOTE"];
                        newRow["CHA_EREDITA"] = r["CHA_EREDITA"];
                        newRow["CHA_TIPO_RISPOSTA"] = r["CHA_TIPO_RISPOSTA"];
                        newRow["CHA_CEDE_DIRITTI"] = r["CHA_CEDE_DIRITTI"];

                        dsRagioni.Tables["RAG_TRASM"].Rows.Add(newRow);
                    }
                }
            }

            return dsRagioni;
        }


        //public DataSet GetDettaglioRagioneInSmistamento(string idAmm, string inString)
        //{
        //    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_DETTAGLIO_RAG_TRASM");
        //    q.setParam("param1", idAmm);
        //    q.setParam("param2", inString);
        //    string queryString = q.getSQL();
        //    logger.Debug(queryString);

        //    DataSet ds = new DataSet();

        //    this.ExecuteQuery(out ds, "DETT_TRASM", queryString);
        //    return ds;
        //}

        /// <summary>
        /// Reperimento di tutti i documenti trasmessi ad un utente
        /// </summary>
        /// <param name="mittente"></param>
        /// <returns></returns>
        public DataSet GetListDocumentiTrasmessi(DocsPaVO.Smistamento.MittenteSmistamento mittente)
        {
            //nuova implementazione 1/10/2007
            string userDB = String.Empty;
            if (dbType.ToUpper() == "SQL")
            {
                userDB = getUserDB();
            }
            // aggiungo registro "0" per i doc grigi.
            mittente.RegistriAppartenenza.Add("0");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_LIST_DOCUMENTI_IN_TODO");
            q.setParam("param1", mittente.IDPeople);
            q.setParam("param2", mittente.IDGroup);

            //FILTRO AOO PER TUTTI I REGISTRI
            if (System.Configuration.ConfigurationManager.AppSettings["NO_FILTRO_AOO"] != null && System.Configuration.ConfigurationManager.AppSettings["NO_FILTRO_AOO"] == "1")
            {
                ArrayList registri = new ArrayList();
                mittente.RegistriAppartenenza.Clear();
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                registri = utenti.getRegistriNoFiltroAOO(mittente.IDAmministrazione);
                mittente.RegistriAppartenenza.Add("0");
                foreach (DocsPaVO.utente.Registro reg in registri)
                {
                    mittente.RegistriAppartenenza.Add(reg.systemId);
                }

            }
            q.setParam("param3", this.ConcatArrayRegRuolo(mittente.RegistriAppartenenza));
            ///

            //necessita di utente db SQL per la gestione della funzione vardescribe
            if (userDB != null) q.setParam("dbuser", userDB);


            //ORDINAMENTO
            Trasmissione trasm = new Trasmissione();
            string filterOrder = trasm.getorderFilter(null, "S");
            q.setParam("order", filterOrder);
            // OLD_CODE
            // DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_LIST_DOCUMENTI_TRASMESSI");
            //q.setParam("param1", mittente.);
            //q.setParam("param2", mittente.IDPeople);
            //q.setParam("param3", this.ConcatArrayRegRuolo(mittente.RegistriAppartenenza));

            string queryString = q.getSQL();
            logger.Debug("S_SMISTADOC_GET_LIST_DOCUMENTI_IN_TODO:" + queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "DOCUMENTI_TRASMESSI", queryString);
            return ds;
        }

        /// <summary>
        /// Reperimento di tutti i documenti trasmessi ad un utente
        /// </summary>
        /// <param name="mittente"></param>
        /// <param name="filtriRicerca"></param>
        /// <returns></returns>
        public DataSet GetListDocumentiTrasmessi(DocsPaVO.Smistamento.MittenteSmistamento mittente, DocsPaVO.filtri.FiltroRicerca[] filtriRicerca)
        {
          

            //nuova implementazione 2/10/2007
            string userDB = String.Empty;
            if (dbType.ToUpper() == "SQL")
            {
                userDB = getUserDB();
            }
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_LIST_DOCUMENTI_IN_TODO_FILTERED");
            // aggiungo registro "0" per i doc grigi.
            mittente.RegistriAppartenenza.Add("0");
            q.setParam("param1", mittente.IDPeople);
            q.setParam("param2", mittente.IDGroup);
            q.setParam("param3", this.ConcatArrayRegRuolo(mittente.RegistriAppartenenza));
            //necessita di utente db SQL per la gestione della funzione vardescribe
            if (userDB != null) q.setParam("dbuser", userDB);
            Trasmissione trasm = new Trasmissione();

            if (filtriRicerca != null)
            {
                
                string filterWhere = trasm.getwhereFilter(filtriRicerca);
                q.setParam("filtri", filterWhere);
                  

            }

            //ORDINAMENTO, NB: funziona anche con filtriRicerca =null
            string filterOrder = trasm.getorderFilter(filtriRicerca, "S");
            q.setParam("order", filterOrder);

            trasm.Dispose();


            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "DOCUMENTI_TRASMESSI", queryString);
            return ds;
        }

        /// <summary>
        /// Restituzione di un oggetto di tipo "DocumentoTrasmesso"
        /// </summary>
        /// <param name="IDDocumento">SystemID documento</param>
        /// <returns></returns>
        public DataSet GetDocumentoSmistamento(string idDocumento)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_DOCUMENTO");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("E.CREATION_DATE", false));
            q.setParam("param2", idDocumento);
            q.setParam("param3", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(ds, "DOCUMENTI", queryString);

            if (ds.Tables.Count > 0 && ds.Tables["DOCUMENTI"].Rows.Count > 0)
            {
                string tipoDocumento = ds.Tables["DOCUMENTI"].Rows[0]["TipoDocumento"].ToString();

                // Se il documento è protocollato e di tipo "A", "P", "I" (arrivo, partenza, interno),
                // vengono reperiti i soggetti "Mittenti" e "Destinatari"
                if (tipoDocumento.Equals("A") ||
                    tipoDocumento.Equals("P") ||
                    tipoDocumento.Equals("I"))
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_MITT_DEST_DOCUMENTO");
                    q.setParam("param1", idDocumento);

                    queryString = q.getSQL();
                    logger.Debug(queryString);

                    this.ExecuteQuery(ds, "MITT_DEST", queryString);
                }

                // Reperimento dati file
                this.AppendDataFileDocument(ds);
            }

            return ds;
        }

        /// <summary>
        /// Restituzione di un oggetto DataSet contenente
        /// i dati riguardanti l'UO, i Ruoli e gli Utenti
        /// dell'UO di appartenenza (possibili destinatari del documento) o della UO corrente (se siamo in smista_naviga_UO)
        /// </summary>
        /// <param name="idCorrGlobale">system_id della UO (dpa_corr_globali)</param>
        /// <param name="registriAppartenenza">lista dei registri</param>
        /// <param name="idAmministrazione">system_id dell'amministrazione</param>
        /// <param name="livelloRuolo">livello del ruolo</param>
        /// <param name="isCurrentUO">True se deve reperire la UO corrente (navigazione UO in smistamento), altimenti False (default)</param>
        /// <returns></returns>
        public DataSet GetUOAppartenenza(string idCorrGlobale,
                                         ArrayList registriAppartenenza,
                                         string idAmministrazione,
                                         string livelloRuolo,
                                         bool isCurrentUO)
        {
            DataSet ds = new DataSet();

            DocsPaUtils.Query q;

            if (isCurrentUO)
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_UO_CORRENTE");
            else
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_UO_APPARTENENZA");

            q.setParam("param1", idCorrGlobale);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            this.ExecuteQuery(ds, "UO", queryString);

            if(ds.Tables["UO"].Rows.Count > 0)
                this.FillRuoliInUO(ds, livelloRuolo, registriAppartenenza, isCurrentUO);

            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IDUnitaOrganizzativa"></param>
        /// <param name="registriAppartenenza"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="idUnitaOrganizzativa"></param>
        /// <param name="livelloRuolo"></param>
        /// <returns></returns>
        public DataSet GetUOInferiori(string idUnitaOrganizzativa,
                                        ArrayList registriAppartenenza,
                                        string idAmministrazione,
                                        string livelloRuolo)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_UO_SOTTOSTANTI");
            q.setParam("param1", idUnitaOrganizzativa);
            q.setParam("param2", this.GetConditionRegistriAppartenenza(registriAppartenenza));
            q.setParam("param3", idAmministrazione);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();
            this.ExecuteQuery(ds, "UO", queryString);

            if (ds.Tables["UO"].Rows.Count > 0)
                this.FillRuoliInUOInferiori(ds, livelloRuolo, this.ConcatArrayRegRuolo(registriAppartenenza),false);

            return ds;
        }

        //public bool RifiutaDocumento(string notaRifiuto, string IDTrasmUtente, string idTrasmissione, string idPeople)
        //{
        //    bool retValue = false;

        //    System.DateTime now = System.DateTime.Now;
        //    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US"); 
        //    string dateString = DocsPaDbManagement.Functions.Functions.ToDate(now.ToString("dd/MM/yyyy hh:mm:ss tt", ci ));
        //    string addInQuery = " IN (SELECT TU.SYSTEM_ID FROM DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE=" + idPeople + " AND TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=" + idTrasmissione + " AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA)";
        //    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASM_UTENTE_DTA_RIFIUTATA");
        //    q.setParam("param1", "DTA_RIFIUTATA= " + dateString + " ,");
        //    q.setParam("param2", ", CHA_IN_TODOLIST = '0', VAR_NOTE_RIF='" + notaRifiuto.Replace("'", "''") + "'");
        //    //q.setParam("param3"," = " + IDTrasmUtente);
        //    q.setParam("param3", addInQuery);

        //    string queryString = q.getSQL();
        //    logger.Debug(queryString);

        //    retValue=this.ExecuteNonQuery(queryString);

        //    return retValue;
        //}


        public bool RifiutaDocumentoInSmistamento(string notaRifiuto, string IDTrasmUtente, string idTrasmissione, string idPeople)
        {
            bool retValue = false;

            string myParam2 = "";
            System.DateTime now = System.DateTime.Now;
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
            string dateString = DocsPaDbManagement.Functions.Functions.ToDate(now.ToString("dd/MM/yyyy hh:mm:ss tt", ci));

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASM_UTENTE_RIFIUTATA_NEW");
            myParam2 = ", DTA_RIFIUTATA = " + dateString;
            if (notaRifiuto != null)
                myParam2 = myParam2 + ", VAR_NOTE_RIF='" + notaRifiuto.Replace("'", "''") + "'";
            q.setParam("param100", myParam2);
            q.setParam("param1", IDTrasmUtente);
            q.setParam("param2", idPeople);
            q.setParam("param3", idTrasmissione);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            retValue = this.ExecuteNonQuery(queryString);

            return retValue;
        }

        //public bool SetDocumentoVisto(string IDTrasmUtente, bool IsTrasmConWorkFlow, string idPeople, string idTrasmissione)
        //{
        //    bool retValue = false;
        //    int retProc;

        //    // Creazione parametri SP
        //    ArrayList parameters = new ArrayList();
        //    parameters.Add(this.CreateParameter("IDTrasmissioneUtenteMittente", IDTrasmUtente));

        //    //new
        //    parameters.Add(this.CreateParameter("IDPeopleMittente", idPeople));
        //    parameters.Add(this.CreateParameter("IDTrasmissione", idTrasmissione));
        //    // 
        //    if (IsTrasmConWorkFlow)
        //        parameters.Add(this.CreateParameter("TrasmissioneConWorkflow", 1));
        //    else
        //        parameters.Add(this.CreateParameter("TrasmissioneConWorkflow", 0));



        //    // Inizio transazione
        //    this.BeginTransaction();

        //    try
        //    {
        //        retProc = this.ExecuteStoreProcedure("I_Smistamento_DocVisto", parameters);
        //        logger.Debug("Chiamata SP 'I_Smistamento_DocVisto'. Esito: " + retProc);
        //    }
        //    catch
        //    {
        //        retProc = 1;



        //    }

        //    // Commit / rollback transazione
        //    if (retProc.Equals(0))
        //    {
        //        this.CommitTransaction();
        //        logger.Debug("Eseguita Commit alla Stored Procedure: I_Smistamento_DocVisto");
        //        retValue = true;
        //    }
        //    else
        //    {
        //        this.RollbackTransaction();
        //        logger.Debug("Eseguita Rollback alla Stored Procedure: I_Smistamento_DocVisto");
        //    }

        //    this.CloseConnection();

        //    return retValue;
        //}


        public bool ScartaDocumento(DocsPaVO.utente.InfoUtente infoUtente, string idOggetto, string tipoOggetto, string idTrasmissione)
        {
            bool retValue = false;

            ArrayList parameters = new ArrayList();
            DocsPaUtils.Data.ParameterSP outParam;

            parameters.Add(this.CreateParameter("idPeople", infoUtente.idPeople));
            parameters.Add(this.CreateParameter("idOggetto", idOggetto));
            parameters.Add(this.CreateParameter("idGruppo", infoUtente.idGruppo));
            parameters.Add(this.CreateParameter("idTrasmissione", idTrasmissione));
            parameters.Add(this.CreateParameter("tipoOggetto", tipoOggetto));

            int idDelegato = 0;
            if (infoUtente.delegato != null)
                idDelegato = Convert.ToInt32(infoUtente.delegato.idPeople);
            parameters.Add(this.CreateParameter("idDelegato", idDelegato));

            outParam = new DocsPaUtils.Data.ParameterSP("resultValue", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
            parameters.Add(outParam);

            // Inizio transazione
            this.BeginTransaction();

            try
            {
                //retProc=this.ExecuteStoreProcedure("I_Smistamento_DocVisto",parameters);				
                //logger.Debug("Chiamata SP 'I_Smistamento_DocVisto'. Esito: " + retProc);
                this.ExecuteStoredProcedure("SPsetDataVistaSmistamento", parameters, null);

                if (outParam.Valore != null && outParam.Valore.ToString() != "" && outParam.Valore.ToString() != "1")
                {
                    retValue = true;
                    logger.Debug("STORE PROCEDURE SPsetDataVistaSmistamento: esito positivo");
                }
                if (retValue)
                    this.CommitTransaction();
                else
                    this.RollbackTransaction();
            }
            catch (Exception e)
            {

                logger.Debug("STORE PROCEDURE SPsetDataVistaSmistamento: esito negativo" + e.Message);
                this.RollbackTransaction();
                retValue = false;
            }
            finally
            {
                this.CloseConnection();
                logger.Debug("FINE SPsetDataVistaSmistamento");
            }

            return retValue;
        }

        /// <summary>
        /// Smistamento a ruolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="mittente"></param>
        /// <param name="documentoSmistamento"></param>
        /// <param name="datiTrasmissioneDocumento"></param>
        /// <param name="ruoloDestinatario"></param>
        /// <param name="ragioneSmistamento"></param>
        /// <returns></returns>
        public DocsPaVO.Smistamento.EsitoSmistamentoDocumento SmistaDocumentoRuolo(
                                    DocsPaVO.utente.InfoUtente infoUtente,
                                    DocsPaVO.Smistamento.MittenteSmistamento mittente,
                                    DocsPaVO.Smistamento.DocumentoSmistamento documentoSmistamento,
                                    DocsPaVO.Smistamento.DatiTrasmissioneDocumento datiTrasmissioneDocumento,
                                    DocsPaVO.Smistamento.RuoloSmistamento ruoloDestinatario,
                                    DocsPaVO.trasmissione.RagioneTrasmissione ragioneSmistamento,
                                    bool modelloNoNOtify)
        {
            DocsPaVO.Smistamento.EsitoSmistamentoDocumento retValue = new DocsPaVO.Smistamento.EsitoSmistamentoDocumento();
            retValue.IDPeopleDestinatario = null;
            retValue.IDCorrGlobaleDestinatario = ruoloDestinatario.ID;
            retValue.DenominazioneDestinatario = ruoloDestinatario.Descrizione;

            this.BeginTransaction();

            // Creazione parametri SP
            ArrayList parameters = new ArrayList();

            parameters.Add(this.CreateParameter("IDPeopleMittente", mittente.IDPeople));
            parameters.Add(this.CreateParameter("IDCorrGlobaleRuoloMittente", mittente.IDCorrGlobaleRuolo));
            parameters.Add(this.CreateParameter("IDGruppoMittente", mittente.IDGroup));
            parameters.Add(this.CreateParameter("IDAmministrazioneMittente", mittente.IDAmministrazione));
            parameters.Add(this.CreateParameter("IDCorrGlobaleDestinatario", ruoloDestinatario.ID));
            parameters.Add(this.CreateParameter("IDDocumento", documentoSmistamento.IDDocumento));
            //new
            parameters.Add(this.CreateParameter("IDTrasmissione", datiTrasmissioneDocumento.IDTrasmissione));

            //if (ruoloDestinatario.FlagCompetenza)
            //    parameters.Add(this.CreateParameter("FlagCompetenza", 1));
            //else
            //    parameters.Add(this.CreateParameter("FlagCompetenza", 0));

            parameters.Add(this.CreateParameter("IDTrasmissioneUtenteMittente", datiTrasmissioneDocumento.IDTrasmissioneUtente));

            //if (datiTrasmissioneDocumento.TrasmissioneConWorkflow)
            //    parameters.Add(this.CreateParameter("TrasmissioneConWorkflow", 1));
            //else
            //    parameters.Add(this.CreateParameter("TrasmissioneConWorkflow", 0));

            if (ragioneSmistamento.tipo.Equals("W"))
                parameters.Add(this.CreateParameter("TrasmissioneConWorkflow", 1));
            else
                parameters.Add(this.CreateParameter("TrasmissioneConWorkflow", 0));

            //INSERIMENTO NOTE GENERALI
            if (datiTrasmissioneDocumento.NoteGenerali != "" && datiTrasmissioneDocumento.NoteGenerali != null)
                parameters.Add(this.CreateParameter("NoteGeneraliDocumento", datiTrasmissioneDocumento.NoteGenerali));
            else
                parameters.Add(this.CreateParameter("NoteGeneraliDocumento", DBNull.Value));

            //INSERIMENTO NOTE INDIVIDUALI
            if (ruoloDestinatario.datiAggiuntiviSmistamento.NoteIndividuali != "" && ruoloDestinatario.datiAggiuntiviSmistamento.NoteIndividuali != null)
                parameters.Add(this.CreateParameter("NoteIndividuali", ruoloDestinatario.datiAggiuntiviSmistamento.NoteIndividuali));
            else
                parameters.Add(this.CreateParameter("NoteIndividuali", DBNull.Value));

            //INSERIMENTO DATA DI SCADENZA
            if (ruoloDestinatario.datiAggiuntiviSmistamento.dtaScadenza != "" && ruoloDestinatario.datiAggiuntiviSmistamento.dtaScadenza != null)
                parameters.Add(this.CreateParameter("DataScadenza", DocsPaUtils.Functions.Functions.ToDate(ruoloDestinatario.datiAggiuntiviSmistamento.dtaScadenza)));
            else
                parameters.Add(this.CreateParameter("DataScadenza", DBNull.Value));

            //INSERIMENTO TIPO DI TRASMISSIONE
            if (!string.IsNullOrEmpty(ruoloDestinatario.chaTipoTrasm))
                parameters.Add(this.CreateParameter("TipoTrasmissione", ruoloDestinatario.chaTipoTrasm));
            else
            {
                if (ruoloDestinatario.datiAggiuntiviSmistamento.tipoTrasm != "" && ruoloDestinatario.datiAggiuntiviSmistamento.tipoTrasm != null)
                    parameters.Add(this.CreateParameter("TipoTrasmissione", ruoloDestinatario.datiAggiuntiviSmistamento.tipoTrasm));
                else
                    parameters.Add(this.CreateParameter("TipoTrasmissione", "S")); // viene impostato a "Uno" come era il default prima
            }

            //TIPO DIRITTO
            parameters.Add(this.CreateParameter("TipoDiritto", this.GetTipoDirittoSmistamento(ragioneSmistamento)));

            //RIGHTS
            parameters.Add(this.CreateParameter("Rights", this.GetAccessRightSmistamento(ragioneSmistamento)));


            //RIGHTS ORIGINALI
            parameters.Add(this.CreateParameter("OriginalRights", this.GetAccessRightSmistamentoOriginali(ragioneSmistamento)));
            //ID RAGIONE TRASMISSIONE
            parameters.Add(this.CreateParameter("IDRagioneTrasm", ragioneSmistamento.systemId));

            // GESTIONE DELEGATO
            int idDelegato = 0;
            if (infoUtente.delegato != null)
                idDelegato = Convert.ToInt32(infoUtente.delegato.idPeople);
            parameters.Add(this.CreateParameter("idPeopleDelegato", idDelegato));

            //NOTIFICA
            string notify = "1";
            if (modelloNoNOtify)
                notify = "0";
            parameters.Add(this.CreateParameter("nonotify", notify));
            try
            {
                retValue.CodiceEsitoSmistamento = this.ExecuteStoreProcedure("I_SMISTAMENTO_SMISTADOC_U", parameters);
                logger.Debug("Chiamata SP 'I_SMISTAMENTO_SMISTADOC_U. Esito: " + retValue.CodiceEsitoSmistamento.ToString());
                if (retValue.CodiceEsitoSmistamento > 0)
                {

                    foreach (DocsPaVO.Smistamento.UtenteSmistamento utente in ruoloDestinatario.Utenti)
                    {
                        if (utente.FlagCompetenza || utente.FlagConoscenza || !string.IsNullOrEmpty(ruoloDestinatario.ragioneTrasmRapida))
                        {
                            //notifico solo agli utenti selezionati
                           if (!insertTrasmUtenteSmistamento(utente.ID, retValue.CodiceEsitoSmistamento.ToString()))
                           {
                               throw new Exception("Errore durante l'inserimento nella DPA_TRASM_UTENTE");
                           }
                       }							
                   }

                    if (!modelloNoNOtify)
                    {
                        //aggiornamento della dta_invio della trasmissione per correzione bug TODOLIST (27/12/2007)
                        if (!updateDtaInvioInSmistamento(retValue.CodiceEsitoSmistamento.ToString()))
                        {
                            throw new Exception("Errore durante l'aggiornamento della Data Invio della trasmissione");
                        }
                    }
                   retValue.CodiceEsitoSmistamento = 0;                    
                }
            }
			catch (Exception e)
			{
				retValue.CodiceEsitoSmistamento=-5;
				retValue.DescrizioneEsitoSmistamento=e.Message;
			}
			finally
			{
				parameters=null;
				
				switch (retValue.CodiceEsitoSmistamento)
				{
					case 0:
						retValue.DescrizioneEsitoSmistamento="Documento smistato correttamente.";
						logger.Debug("Documento smistato correttamente.");
						break;

                    case -2:
                        retValue.DescrizioneEsitoSmistamento = "Errore inserimento in tabella DPA_TRASMISSIONI.";
                        logger.Debug("Errore inserimento in tabella DPA_TRASMISSIONI.");
                        break;

                    case -3:
                        retValue.DescrizioneEsitoSmistamento = "Errore inserimento in tabella DPA_TRASM_SINGOLE.";
                        logger.Debug("Errore inserimento in tabella DPA_TRASM_SINGOLE.");
                        break;

                    case -4:
                        retValue.DescrizioneEsitoSmistamento = "Errore durante l'esecuzione della store procedure SPsetDataVista.";
                        logger.Debug("Errore durante l'esecuzione della store procedure SPsetDataVista.");
                        break;

                    case -5:
                        retValue.DescrizioneEsitoSmistamento = "Si è verificato un errore durante lo smistamento agli utenti";
                        logger.Debug("Errore durante lo smistamento agli utenti");
                        break;
				}
			
				// Commit / rollback transazione
				if (retValue.CodiceEsitoSmistamento.Equals(0))
				{
					this.CommitTransaction();
					logger.Debug("Eseguita Commit alla Stored Procedure: I_SMISTAMENTO_SMISTADOC_U");
				}
				else
				{
					this.RollbackTransaction();
					logger.Debug("Eseguita Rollback alla Stored Procedure: I_SMISTAMENTO_SMISTADOC_U");
				}

                this.CloseConnection();
            }

            return retValue;
        }

        /// <summary>
        /// Usato nello smistamento per inserire le trasmissioni utente relative agli utenti del ruolo che
        /// sono stati selezionati
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idTrasmSingola"></param>
        /// <returns></returns>
        public bool insertTrasmUtenteSmistamento(string idPeople, string idTrasmSingola)
        {

            bool retValue = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_TRASM_UTENTE_insertTrasmUtente");

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());

            q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_TRASM_UTENTE"));

            q.setParam("param3", idPeople);
            q.setParam("param4", "NULL");
            q.setParam("param5", idTrasmSingola);

            string res;
            logger.Debug(q.getSQL().ToString());
            this.InsertLocked(out res, q.getSQL(), "DPA_TRASM_UTENTE");

            if (res != null && !res.Equals(""))
            {
                logger.Debug("Inserimento trasmissione utente per idPeople: " + idPeople);
                retValue = true;
            }
            return retValue;

        }

        public bool updateDtaInvioInSmistamento(string idTrasmSingola)
        {
            logger.Debug("inizio aggiornamento Data Invio della trasmissione relativa alla trasmissione singola: " + idTrasmSingola);
            bool retValue = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASMISSIONE_DTA_INVIO");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetDate());
            q.setParam("param2", " (SELECT ID_TRASMISSIONE FROM DPA_TRASM_SINGOLA WHERE SYSTEM_ID = " + idTrasmSingola + ")");
            string update = q.getSQL();
            logger.Debug(update);
            if (this.ExecuteNonQuery(update))
            {

                logger.Debug("aggiornamento della data invio eseguito con successo.");

                retValue = true;

            }

            return retValue;

        }


        /// <summary>
        /// Smistamento a utenti
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="mittente"></param>
        /// <param name="documentoSmistamento"></param>
        /// <param name="datiTrasmissioneDocumento"></param>
        /// <param name="utenteSmistamento"></param>
        /// <param name="ragioneSmistamento"></param>
        /// <returns></returns>
        public DocsPaVO.Smistamento.EsitoSmistamentoDocumento SmistaDocumento(
                                    DocsPaVO.utente.InfoUtente infoUtente,
                                    DocsPaVO.Smistamento.MittenteSmistamento mittente,
                                    DocsPaVO.Smistamento.DocumentoSmistamento documentoSmistamento,
                                    DocsPaVO.Smistamento.DatiTrasmissioneDocumento datiTrasmissioneDocumento,
                                    DocsPaVO.Smistamento.UtenteSmistamento utenteSmistamento,
                                    DocsPaVO.trasmissione.RagioneTrasmissione ragioneSmistamento,
                                    bool modelloNoNotify)
        {
            DocsPaVO.Smistamento.EsitoSmistamentoDocumento retValue = new DocsPaVO.Smistamento.EsitoSmistamentoDocumento();
            retValue.IDPeopleDestinatario = utenteSmistamento.ID;
            retValue.IDCorrGlobaleDestinatario = utenteSmistamento.IDCorrGlobali;
            retValue.DenominazioneDestinatario = utenteSmistamento.Denominazione;

            // Creazione parametri SP
            ArrayList parameters = new ArrayList();

            parameters.Add(this.CreateParameter("IDPeopleMittente", mittente.IDPeople));
            parameters.Add(this.CreateParameter("IDCorrGlobaleRuoloMittente", mittente.IDCorrGlobaleRuolo));
            parameters.Add(this.CreateParameter("IDGruppoMittente", mittente.IDGroup));
            parameters.Add(this.CreateParameter("IDAmministrazioneMittente", mittente.IDAmministrazione));
            parameters.Add(this.CreateParameter("IDPeopleDestinatario", utenteSmistamento.ID));
            parameters.Add(this.CreateParameter("IDCorrGlobaleDestinatario", utenteSmistamento.IDCorrGlobali));
            parameters.Add(this.CreateParameter("IDDocumento", documentoSmistamento.IDDocumento));
            //new
            parameters.Add(this.CreateParameter("IDTrasmissione", datiTrasmissioneDocumento.IDTrasmissione));

            //if (utenteSmistamento.FlagCompetenza)
            //    parameters.Add(this.CreateParameter("FlagCompetenza", 1));
            //else
            //    parameters.Add(this.CreateParameter("FlagCompetenza", 0));

            parameters.Add(this.CreateParameter("IDTrasmissioneUtenteMittente", datiTrasmissioneDocumento.IDTrasmissioneUtente));

            if (datiTrasmissioneDocumento.TrasmissioneConWorkflow)
                parameters.Add(this.CreateParameter("TrasmissioneConWorkflow", 1));
            else
                parameters.Add(this.CreateParameter("TrasmissioneConWorkflow", 0));

            //INSERIMENTO NOTE GENERALI
            if (datiTrasmissioneDocumento.NoteGenerali != null && datiTrasmissioneDocumento.NoteGenerali != "")
                parameters.Add(this.CreateParameter("NoteGeneraliDocumento", datiTrasmissioneDocumento.NoteGenerali));
            else
                parameters.Add(this.CreateParameter("NoteGeneraliDocumento", DBNull.Value));

            //INSERIMENTO NOTE INDIVIDUALI
            if (utenteSmistamento.datiAggiuntiviSmistamento.NoteIndividuali != null && utenteSmistamento.datiAggiuntiviSmistamento.NoteIndividuali != "")
                parameters.Add(this.CreateParameter("NoteIndividuali", utenteSmistamento.datiAggiuntiviSmistamento.NoteIndividuali));
            else
                parameters.Add(this.CreateParameter("NoteIndividuali", DBNull.Value));

            //INSERIMENTO DATA DI SCADENZA
            if (utenteSmistamento.datiAggiuntiviSmistamento.dtaScadenza != null && utenteSmistamento.datiAggiuntiviSmistamento.dtaScadenza != "")
                parameters.Add(this.CreateParameter("DataScadenza", DocsPaUtils.Functions.Functions.ToDate(utenteSmistamento.datiAggiuntiviSmistamento.dtaScadenza)));
            else
                parameters.Add(this.CreateParameter("DataScadenza", DBNull.Value));

            //TIPO DIRITTO
            parameters.Add(this.CreateParameter("TipoDiritto", this.GetTipoDirittoSmistamento(ragioneSmistamento)));

            //RIGHTS
            parameters.Add(this.CreateParameter("Rights", this.GetAccessRightSmistamento(ragioneSmistamento)));

            //RIGHTS ORIGINALI
            parameters.Add(this.CreateParameter("OriginalRights", this.GetAccessRightSmistamentoOriginali(ragioneSmistamento)));
            
            //ID RAGIONE TRASMISSIONE
            parameters.Add(this.CreateParameter("IDRagioneTrasm", ragioneSmistamento.systemId));

            // GESTIONE DELEGATO
            int idDelegato = 0;
            if (infoUtente.delegato != null)
                idDelegato = Convert.ToInt32(infoUtente.delegato.idPeople);
            parameters.Add(this.CreateParameter("idPeopleDelegato", idDelegato));

            if (modelloNoNotify)
                parameters.Add(this.CreateParameter("nonotify", 1));
            else
                parameters.Add(this.CreateParameter("nonotify", 0));

            // Inizio transazione
            this.BeginTransaction();

            try
            {
                retValue.CodiceEsitoSmistamento = this.ExecuteStoreProcedure("I_SMISTAMENTO_SMISTADOC", parameters);
                logger.Debug("Chiamata SP 'I_SMISTAMENTO_SMISTADOC. Esito: " + retValue.CodiceEsitoSmistamento.ToString());
            }
            catch (Exception e)
            {
                retValue.CodiceEsitoSmistamento = -1;
                retValue.DescrizioneEsitoSmistamento = e.Message;
            }
            finally
            {
                parameters = null;

                switch (retValue.CodiceEsitoSmistamento)
                {
                    case 0:
                        retValue.DescrizioneEsitoSmistamento = "Documento smistato correttamente.";
                        break;

                    case -2:
                        retValue.DescrizioneEsitoSmistamento = "Errore inserimento in tabella DPA_TRASMISSIONI.";
                        break;

                    case -3:
                        retValue.DescrizioneEsitoSmistamento = "Errore inserimento in tabella DPA_TRASM_SINGOLE.";
                        break;

                    case -4:
                        retValue.DescrizioneEsitoSmistamento = "Errore inserimento in tabella DPA_TRASM_UTENTE.";
                        break;
                    case -5:
                        retValue.DescrizioneEsitoSmistamento = "Errore durante l'esecuzione della store procedure I_SMISTAMENTO_SMISTADOC.";
                        break;
                }

                // Commit / rollback transazione
                if (retValue.CodiceEsitoSmistamento.Equals(0))
                {
                    this.CommitTransaction();
                    logger.Debug("Eseguita Commit alla Stored Procedure: I_SMISTAMENTO_SMISTADOC");
                }
                else
                {
                    this.RollbackTransaction();
                    logger.Debug("Eseguita Rollback alla Stored Procedure: I_SMISTAMENTO_SMISTADOC");
                }

                this.CloseConnection();
            }

            return retValue;
        }

        public bool SetVisibilitaRuoliSup
                            (DocsPaVO.Smistamento.MittenteSmistamento mittente,
                             DocsPaVO.Smistamento.DocumentoSmistamento documentoSmistamento,
                             DocsPaVO.Smistamento.RuoloSmistamento ruolo,
                             string accessRights)
        {
            return this.SetVisibilitaRuoliSup(documentoSmistamento.IDDocumento, mittente, ruolo, accessRights);
        }

        public bool SetVisibilitaRuoliSup
                        (DocsPaVO.Smistamento.MittenteSmistamento mittente,
                         DocsPaVO.Smistamento.DatiTrasmissioneDocumento datiTrasmissioneDocumento,
                         DocsPaVO.Smistamento.RuoloSmistamento ruolo,
                         string accessRights)
        {
            return this.SetVisibilitaRuoliSup(datiTrasmissioneDocumento.IDDocumento, mittente, ruolo, accessRights);
        }

        /// <summary>
        /// Reperimento delle UO per lo smistamento (da tabella "DPA_UO_SMISTAMENTO") e dei ruoli inferiori,
        /// usato nel calcolo dei destinatari per le trasmissioni del protocollo ingresso semplificato e dello smistamento
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <param name="livelloRuolo"></param>
        /// <returns></returns>
        public DataSet GetUOSmistamento(string idRegistro, string livelloRuolo)
        {
            DataSet ds = new DataSet();

            ds = GetListaUOSmistamento(idRegistro);

            if (ds.Tables["UO"].Rows.Count > 0)
            {
                // fromProtoSempl: true se provengo dal proto semplificato
                this.FillRuoliInUOInferiori(ds, livelloRuolo, idRegistro, true);
            }
            return ds;
        }
        /// <summary>
        /// Reperimento delle UO per lo smistamento (da tabella "DPA_UO_SMISTAMENTO")
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public DataSet GetListaUOSmistamento(string idRegistro)
        {
            DataSet ds = new DataSet();

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTAMENTO_GET_UO");
            queryDef.setParam("IDRegistro", idRegistro);

            string queryString = queryDef.getSQL();
            logger.Debug(queryString);

            this.ExecuteQuery(ds, "UO", queryString);
            return ds;
        }


        /// <summary>
        /// Smistamento del documento non trasmesso a ruolo di riferimento
        /// </summary>
        /// <param name="mittente"></param>
        /// <param name="documentoSmistamento"></param>
        /// <param name="ruoloDestinatario"></param>
        /// <returns></returns>
        public DocsPaVO.Smistamento.EsitoSmistamentoDocumento SmistaDocumentoRuolo(
                            DocsPaVO.Smistamento.MittenteSmistamento mittente,
                            DocsPaVO.Smistamento.DocumentoSmistamento documentoSmistamento,
                            DocsPaVO.Smistamento.RuoloSmistamento ruoloDestinatario,
                            DocsPaVO.trasmissione.RagioneTrasmissione ragioneSmistamento)
        {

            DocsPaVO.Smistamento.EsitoSmistamentoDocumento retValue = new DocsPaVO.Smistamento.EsitoSmistamentoDocumento();
            retValue.IDPeopleDestinatario = null;
            retValue.IDCorrGlobaleDestinatario = ruoloDestinatario.ID;
            retValue.DenominazioneDestinatario = ruoloDestinatario.Descrizione;

            // Creazione parametri SP
            ArrayList parameters = new ArrayList();

            parameters.Add(this.CreateParameter("IDPeopleMittente", mittente.IDPeople));
            parameters.Add(this.CreateParameter("IDCorrGlobaleRuoloMittente", mittente.IDCorrGlobaleRuolo));
            parameters.Add(this.CreateParameter("IDGruppoMittente", mittente.IDGroup));
            parameters.Add(this.CreateParameter("IDAmministrazioneMittente", mittente.IDAmministrazione));
            parameters.Add(this.CreateParameter("IDCorrGlobaleDestinatario", ruoloDestinatario.ID));
            parameters.Add(this.CreateParameter("IDDocumento", documentoSmistamento.IDDocumento));

            //if (ruoloDestinatario.FlagCompetenza)
            //    parameters.Add(this.CreateParameter("FlagCompetenza", 1));
            //else
            //    parameters.Add(this.CreateParameter("FlagCompetenza", 0));

            //TIPO DIRITTO
            parameters.Add(this.CreateParameter("TipoDiritto", this.GetTipoDirittoSmistamento(ragioneSmistamento)));

            //RIGHTS
            parameters.Add(this.CreateParameter("Rights", this.GetAccessRightSmistamento(ragioneSmistamento)));

            //ID RAGIONE TRASMISSIONE
            parameters.Add(this.CreateParameter("IDRagioneTrasm", ragioneSmistamento.systemId));

            // Inizio transazione
            this.BeginTransaction();

            try
            {
                retValue.CodiceEsitoSmistamento = this.ExecuteStoreProcedure("I_SMISTAMENTO_SMISTADOC_R_2", parameters);
                logger.Debug("Chiamata SP 'I_SMISTAMENTO_SMISTADOC_R_2. Esito: " + retValue.CodiceEsitoSmistamento.ToString());
            }
            catch (Exception e)
            {
                retValue.CodiceEsitoSmistamento = -1;
                retValue.DescrizioneEsitoSmistamento = e.Message;
            }
            finally
            {
                parameters = null;

                switch (retValue.CodiceEsitoSmistamento)
                {
                    case 0:
                        retValue.DescrizioneEsitoSmistamento = "Documento smistato correttamente.";
                        break;

                    case 2:
                        retValue.DescrizioneEsitoSmistamento = "Errore inserimento in tabella DPA_TRASMISSIONI.";
                        break;

                    case 3:
                        retValue.DescrizioneEsitoSmistamento = "Errore inserimento in tabella DPA_TRASM_SINGOLE.";
                        break;

                    case 4:
                        retValue.DescrizioneEsitoSmistamento = "Errore inserimento in tabella DPA_TRASM_UTENTE.";
                        break;
                }

                // Commit / rollback transazione
                if (retValue.CodiceEsitoSmistamento.Equals(0))
                {
                    this.CommitTransaction();
                    logger.Debug("Eseguita Commit alla Stored Procedure: I_SMISTAMENTO_SMISTADOC_R_2");
                }
                else
                {
                    this.RollbackTransaction();
                    logger.Debug("Eseguita Rollback alla Stored Procedure: I_SMISTAMENTO_SMISTADOC_R_2");
                }

                this.CloseConnection();
            }

            return retValue;
        }

        public ArrayList GetRegistriRuolo(string idRuolo)
        {
            ArrayList lista = new ArrayList();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REGISTRO_RUOLO");
                q.setParam("param1", idRuolo);

                string queryString = q.getSQL();
                logger.Debug(queryString);

                DataSet ds = new DataSet();

                this.ExecuteQuery(out ds, "LISTA", queryString);
                if (ds != null && ds.Tables["LISTA"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["LISTA"].Rows)
                        lista.Add(row["id_registro"].ToString());
                }
            }
            catch
            {
                lista = null;
            }
            return lista;
        }

        public bool ExistUOInf(string idUO, ArrayList registriAppartenenza)
        {
            bool retValue = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_COUNT_UO_SOTTOSTANTI");
            q.setParam("param1", idUO);
            q.setParam("param2", this.GetConditionRegistriAppartenenza(registriAppartenenza));            

            string queryString = q.getSQL();
            logger.Debug(queryString);

            string outValue = string.Empty;
            this.ExecuteScalar(out outValue, queryString);

            retValue = (outValue != null && outValue != string.Empty && (Convert.ToInt32(outValue)>0));            

            return retValue;
        }

        #endregion

        #region PROTECTED


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtriRicerca"></param>
        /// <param name="queryFrom"></param>
        /// <returns></returns>
        protected virtual string GetFilterString(DocsPaVO.filtri.FiltroRicerca[] filtriRicerca, ref string queryFrom)
        {
            queryFrom = string.Empty;

            StringCollection items = new StringCollection();

            string peopleId = string.Empty;

            foreach (DocsPaVO.filtri.FiltroRicerca item in filtriRicerca)
            {
                if (item.argomento.Equals("RAGIONE"))
                {
                    items.Add("UPPER(D.VAR_DESC_RAGIONE) LIKE '%" + item.valore.ToUpper().Replace("'", "''") + "%'");
                }
                else if (item.argomento.Equals("TRASMISSIONE_IL"))
                {
                    items.Add(DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO", false) + "= '" + item.valore + "'");
                }
                else if (item.argomento.Equals("TRASMISSIONE_SUCCESSIVA_AL"))
                {
                    items.Add("A.DTA_INVIO>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(item.valore, true));
                }
                else if (item.argomento.Equals("TRASMISSIONE_PRECEDENTE_IL"))
                {
                    items.Add("A.DTA_INVIO<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(item.valore, false));
                }
                else if (item.argomento.Equals("SCADENZA_IL"))
                {
                    items.Add(DocsPaDbManagement.Functions.Functions.ToChar("B.DTA_SCADENZA", false) + "= '" + item.valore + "'");
                }
                else if (item.argomento.Equals("SCADENZA_SUCCESSIVA_AL"))
                {
                    items.Add("B.DTA_SCADENZA>=" + DocsPaDbManagement.Functions.Functions.ToDate(item.valore));
                }
                else if (item.argomento.Equals("SCADENZA_PRECEDENTE_IL"))
                {
                    items.Add("B.DTA_SCADENZA<=" + DocsPaDbManagement.Functions.Functions.ToDate(item.valore));
                }
                else if (item.argomento.Equals("COD_RUBR_MITT_UTENTE"))
                {
                    if (peopleId.Equals(string.Empty))
                        peopleId = GetPeopleID(item.valore);
                    items.Add("A.ID_PEOPLE IN (" + peopleId + ")");
                }
                if (item.argomento.Equals("COD_RUBR_MITT_RUOLO"))
                {
                    items.Add("A.ID_RUOLO_IN_UO IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '" + item.valore.ToUpper().Replace("'", "''") + "')");
                }
                else if (item.argomento.Equals("MITTENTE_UTENTE"))
                {
                    items.Add("A.ID_PEOPLE IN (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + item.valore.ToUpper().Replace("'", "''") + "%')");
                }
                else if (item.argomento.Equals("MITTENTE_RUOLO"))
                {
                    items.Add("A.ID_RUOLO_IN_UO IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + item.valore.ToUpper().Replace("'", "''") + "%')");
                }
                else if (item.argomento.Equals("ID_UO_MITT"))
                {
                    items.Add("A.ID_RUOLO_IN_UO IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND ID_UO =" + item.valore + ")");
                }
                else if (item.argomento.Equals("OGGETTO_DOCUMENTO_TRASMESSO"))
                {
                    items.Add("UPPER(E.VAR_PROF_OGGETTO) LIKE '%" + item.valore.ToUpper().Replace("'", "''") + "%'");
                }
                else if (item.argomento.Equals("OGGETTO_FASCICOLO_TRASMESSO"))
                {
                    queryFrom += " ,PROJECT PJ";
                    items.Add("PJ.SYSTEM_ID = A.ID_PROJECT" +
                                  " AND UPPER(PJ.DESCRIPTION) LIKE '%" + item.valore.ToUpper().Replace("'", "''") + "%'");
                }
            }

            string filterString = string.Empty;

            foreach (string item in items)
            {
                if (filterString != string.Empty)
                    filterString += " AND ";
                filterString += item;
            }

            return filterString;
        }

        /// <summary>
        /// Reperimento valore system_id dalla tabella DPA_CORR_GLOBALI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected string GetPeopleID(string codice)
        {
            string commandText = "SELECT ID_PEOPLE " +
                "FROM DPA_CORR_GLOBALI " +
                "WHERE CHA_TIPO_URP='P' AND " +
                "CHA_TIPO_IE='I' AND " +
                "UPPER(VAR_COD_RUBRICA) = '" + codice.ToUpper().Replace("'", "''") + "'";

            logger.Debug(commandText);

            string retValue;
            this.ExecuteScalar(out retValue, commandText);
            return retValue;
        }

        #endregion

        #region PRIVATE

        private string ConcatArrayRegRuolo(ArrayList listaRegRuolo)
        {
            string retValue = string.Empty;
            for (int n = 0; n <= listaRegRuolo.Count - 1; n++)
            {
                retValue += "," + listaRegRuolo[n].ToString();
            }
            retValue = retValue.Substring(1, retValue.Length - 1);
            return retValue;
        }

        private DocsPaUtils.Data.ParameterSP CreateParameter(string name, object value)
        {
            return new DocsPaUtils.Data.ParameterSP(name, value);
        }

        private void AppendDataFileDocument(DataSet dsTrasmissione)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_COMPONENT_DATA");
            q.setParam("param1", dsTrasmissione.Tables["DOCUMENTI"].Rows[0]["IDDocumento"].ToString());

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(dsTrasmissione, "COMPONENTS", queryString);
        }

        private void FillRuoliInUO(DataSet ds, string livelloRuolo, ArrayList registriAppartenenza, bool isCurrentUO)
        {
            DocsPaUtils.Query q = null;

            foreach (DataRow rowUO in ds.Tables["UO"].Rows)
            {
                // Reperimento gruppi nella UO
                if(isCurrentUO)
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_RUOLI_UO_APP_NAV");
                else
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_RUOLI_UO_APPARTENENZA");
                q.setParam("param1", rowUO["ID"].ToString());
                q.setParam("param2", livelloRuolo);
                q.setParam("param3", this.ConcatArrayRegRuolo(registriAppartenenza));

                string queryString = q.getSQL();
                logger.Debug(queryString);

                this.ExecuteQuery(ds, "RUOLI", queryString);

                string paramIDRuoli = string.Empty;

                DataRow[] rowsGruppiUO = ds.Tables["RUOLI"].Select("ID_UO = " + rowUO["ID"].ToString());

                if (rowsGruppiUO.Length > 0)
                {
                    foreach (DataRow rowRuolo in rowsGruppiUO)
                    {
                        if (paramIDRuoli != string.Empty)
                            paramIDRuoli += ", ";

                        paramIDRuoli += rowRuolo["ID_GRUPPO"].ToString();
                    }

                    if (paramIDRuoli != string.Empty)
                    {
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_UTENTI_RUOLI_UO_APPARTENENZA");

                        q.setParam("param1", paramIDRuoli);

                        queryString = q.getSQL();
                        logger.Debug(queryString);

                        this.ExecuteQuery(ds, "UTENTI", queryString);
                    }
                }
            }

            // creazione relazioni tra datatable nel dataset
            if (ds.Tables["UO"].Rows.Count > 0 && ds.Tables["RUOLI"].Rows.Count > 0)
            {
                ds.Relations.Add(new DataRelation("UO_RUOLI", ds.Tables["UO"].Columns["ID"], ds.Tables["RUOLI"].Columns["ID_UO"], false));
            }
            if (ds.Tables["RUOLI"].Rows.Count > 0 && ds.Tables["UTENTI"].Rows.Count > 0)
            {
                ds.Relations.Add(new DataRelation("RUOLI_UTENTI", ds.Tables["RUOLI"].Columns["ID_GRUPPO"], ds.Tables["UTENTI"].Columns["ID_GRUPPO"], false));
            }
        }

        private void FillRuoliInUOInferiori(DataSet ds, string livelloRuolo, string listaRegistri, bool fromProtoSempl)
        {
            DocsPaUtils.Query q = null;

            foreach (DataRow rowUO in ds.Tables["UO"].Rows)
            {
                // Reperimento gruppi nella UO
                if (fromProtoSempl)
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_RUOLI_UO_SOTTOSTANTI_PROTO_SEMPL");
                }
                else
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_RUOLI_UO_SOTTOSTANTI");
                }
                q.setParam("param1", rowUO["ID"].ToString());
                q.setParam("param2", listaRegistri);
                q.setParam("livello", livelloRuolo);

                string queryString = q.getSQL();
                logger.Debug(queryString);

                this.ExecuteQuery(ds, "RUOLI", queryString);

                string paramIDRuoli = string.Empty;

                DataRow[] rowsGruppiUO = ds.Tables["RUOLI"].Select("ID_UO = " + rowUO["ID"].ToString());

                if (rowsGruppiUO.Length > 0)
                {
                    foreach (DataRow rowRuolo in rowsGruppiUO)
                    {
                        if (paramIDRuoli != string.Empty)
                            paramIDRuoli += ", ";

                        paramIDRuoli += rowRuolo["ID_GRUPPO"].ToString();
                    }

                    if (paramIDRuoli != string.Empty)
                    {
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_UTENTI_RUOLI_UO_APPARTENENZA");

                        q.setParam("param1", paramIDRuoli);

                        queryString = q.getSQL();
                        logger.Debug(queryString);

                        this.ExecuteQuery(ds, "UTENTI", queryString);
                    }
                }
            }
            // creazione relazioni tra datatable nel dataset
            if (ds.Tables["UO"].Rows.Count > 0 && ds.Tables["RUOLI"].Rows.Count > 0)
            {
                ds.Relations.Add(new DataRelation("UO_RUOLI", ds.Tables["UO"].Columns["ID"], ds.Tables["RUOLI"].Columns["ID_UO"], false));
            }
            if (ds.Tables["RUOLI"].Rows.Count > 0 && ds.Tables["UTENTI"].Rows.Count > 0)
            {
                ds.Relations.Add(new DataRelation("RUOLI_UTENTI", ds.Tables["RUOLI"].Columns["ID_GRUPPO"], ds.Tables["UTENTI"].Columns["ID_GRUPPO"], false));
            }
        }

        private string GetConditionRegistriAppartenenza(ArrayList registriAppartenenza)
        {
            string retValue = string.Empty;

            for (int i = 0; i < registriAppartenenza.Count; i++)
            {
                if (!retValue.Equals(string.Empty))
                    retValue += ", ";

                retValue += (string)registriAppartenenza[i];
            }

            return retValue;
        }


        private bool SetVisibilitaRuoliSup(string idDocumento,
                                            DocsPaVO.Smistamento.MittenteSmistamento mittente,
                                            DocsPaVO.Smistamento.RuoloSmistamento ruolo,
                                            string accessRights)
        {
            bool result = false;
            int retValue;
            string anchePariLivello = string.Empty;

            // Creazione parametri SP
            ArrayList parameters = new ArrayList();

            parameters.Add(this.CreateParameter("IDCorrGlobaleRuolo", ruolo.ID));
            parameters.Add(this.CreateParameter("IDGruppoMittente", mittente.IDGroup));
            parameters.Add(this.CreateParameter("LivelloRuoloMittente", mittente.LivelloRuolo));
            parameters.Add(this.CreateParameter("IDDocumento", idDocumento));

            // gestione della visibilità anche ai pari livello (solo per le UO superiori)
            anchePariLivello = System.Configuration.ConfigurationManager.AppSettings["EST_VIS_SUP_PARI_LIV"];
            if (anchePariLivello != null && anchePariLivello.Equals("1"))
            {
                anchePariLivello = "1";
            }
            else
            {
                anchePariLivello = "0";
            }
            parameters.Add(this.CreateParameter("PariLivello", anchePariLivello));
            // FINE ... (gestione della visibilità anche ai pari livello)

            parameters.Add(this.CreateParameter("DirittoDaEred", accessRights));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                retValue = dbProvider.ExecuteStoreProcedure("I_Smistamento_SetVisibSup", parameters);

                logger.Debug("Chiamata SP 'I_Smistamento_SetVisibSup'. Esito: " + retValue.ToString());

                if (retValue.Equals(0))
                {
                    logger.Debug("Eseguita Commit alla Stored Procedure: I_Smistamento_SetVisibSup");
                    result = true;
                }
                else
                {
                    logger.Debug("Eseguito RollBack alla Stored Procedure: I_Smistamento_SetVisibSup");
                }
            }

            return result;
        }

        /// <summary>
        /// Estrae le note individuali associate alla trasmissione singola corrente
        /// </summary>
        /// <param name="idTrasmSingola"></param>
        /// <returns></returns>
        public string GetNoteIndividuali(string idTrasmSingola)
        {
            string noteIndividuali = "";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TRASM_SINGOLA");
            q.setParam("param1", "VAR_NOTE_SING");
            q.setParam("param2", "SYSTEM_ID = " + idTrasmSingola);
            string sql = q.getSQL();
            logger.Debug("GetNoteIndividuali - idTrasmSingola= " + idTrasmSingola);
            logger.Debug(sql);

            this.ExecuteScalar(out noteIndividuali, sql);
            return noteIndividuali;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string getUserDB()
        {
           return Functions.GetDbUserSession();
        }

        public int GetAccessRightSmistamento(DocsPaVO.trasmissione.RagioneTrasmissione ragione)
        {
            int retValue = 0;

            DocsPaVO.HMDiritti.HMdiritti HMD = new DocsPaVO.HMDiritti.HMdiritti(); 

            if (ragione.tipo.Equals("W") && IsEnabledVisibPosticipataInTrasmConWF())
                retValue = HMD.HDdiritti_Waiting;
            else
            {
                switch (ragione.tipoDiritti)
                {
                    case DocsPaVO.trasmissione.TipoDiritto.READ:
                        retValue = HMD.HMdiritti_Read;
                        break;
                    case DocsPaVO.trasmissione.TipoDiritto.WRITE:
                        retValue = HMD.HMdiritti_Write;
                        break;
                    case DocsPaVO.trasmissione.TipoDiritto.WAITING:
                        retValue = HMD.HDdiritti_Waiting;
                        break;
                }
            }

            return retValue;
        }
        /// <summary>
        /// ritorna i diritti non considerando la IsEnabledVisibPosticipataInTrasmConWF
        /// servono all SP per fare poi update al valore originale diritti sulla security dato dalla TRASMissione.
        /// </summary>
        /// <param name="ragione"></param>
        /// <returns></returns>
        public int GetAccessRightSmistamentoOriginali(DocsPaVO.trasmissione.RagioneTrasmissione ragione)
        {
            int retValue = 0;

            DocsPaVO.HMDiritti.HMdiritti HMD = new DocsPaVO.HMDiritti.HMdiritti();

            switch (ragione.tipoDiritti)
                {
                    case DocsPaVO.trasmissione.TipoDiritto.READ:
                        retValue = HMD.HMdiritti_Read;
                        break;
                    case DocsPaVO.trasmissione.TipoDiritto.WRITE:
                        retValue = HMD.HMdiritti_Write;
                        break;
                    case DocsPaVO.trasmissione.TipoDiritto.WAITING:
                        retValue = HMD.HDdiritti_Waiting;
                        break;
                }
            

            return retValue;
        }

        /// <summary>
        /// Verifica se il sistema è impostato ad avere la gestione della visibilità posticipata ai superiori gerarchici
        /// nelle trasmisissioni con workflow
        /// </summary>
        /// <returns></returns>
        private bool IsEnabledVisibPosticipataInTrasmConWF()
        {
            return (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["VISIB_POST_TRASM_WF"]) && System.Configuration.ConfigurationManager.AppSettings["VISIB_POST_TRASM_WF"].Equals("1"));
        }

        private string GetTipoDirittoSmistamento(DocsPaVO.trasmissione.RagioneTrasmissione ragione)
        {
            string tipoDiritto = string.Empty;

            if (ragione.tipo.Equals("W") && IsEnabledVisibPosticipataInTrasmConWF())
                tipoDiritto = "A";
            else
                tipoDiritto = "T";

            return tipoDiritto;
        }

        public DataSet getDatiAggUtente(ref DocsPaVO.Smistamento.UtenteSmistamento utente, string idUtente)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_DATI_UTENTI");
            q.setParam("idPeople", idUtente);         

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();
            this.ExecuteQuery(ds, "UTENTI", queryString);

            return ds;
        }

        public DataSet getUtentiRuolo(string id, string param)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_UTENTI_RUOLO");

            string queryParam = "";
            if (param == "R")
                queryParam = " b.groups_system_id =(select cs.ID_GRUPPO from dpa_corr_globali cs where system_id=" + id + ")";
            else
                queryParam = " a.SYSTEM_ID=" + id;
            q.setParam("param1", queryParam);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();
            this.ExecuteQuery(ds, "UTENTI", queryString);

            return ds;
        }

        public DataSet getRuoliUo(string idUo)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_RUOLI_UO");

            q.setParam("param1", idUo);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();
            this.ExecuteQuery(ds, "RUOLI", queryString);

            return ds;
        }

        #endregion

        public DataSet getAllRagioniTrasmSmista(string idUo)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_ALL_RAGIONI_TRASM");

            q.setParam("param1", idUo);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();
            this.ExecuteQuery(ds, "RAG_TRASM", queryString);

            return ds;
        }

        public bool IsRagioneEreditaByTrasmSingola(string id_trasm_singola)
        {
            bool result = false;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_EREDITA_RAGIONE_TRASM_SINGOLA");
                q.setParam("id_trasm", id_trasm_singola);
                string queryString = q.getSQL();
                DataSet ds = new DataSet();
                this.ExecuteQuery(ds, "TableEredita", queryString);
                if (ds != null && ds.Tables["TableEredita"].Rows.Count != 0)
                {
                    foreach (DataRow row in ds.Tables["TableEredita"].Rows)
                    {
                        if ((row["CHA_EREDITA"].ToString()).Equals("1"))
                        {
                            result = true;
                        }

                    }
                }
            }

            return result;
        }

        public DocsPaVO.Smistamento.MittenteSmistamento getMittenteSmistamentoByIdTrasm(string id_trasmissione)
        {
            DocsPaVO.Smistamento.MittenteSmistamento info = new DocsPaVO.Smistamento.MittenteSmistamento();

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_MITTENTE_BY_TRASMISSIONE");
                q.setParam("id_trasm", id_trasmissione);
                string queryString = q.getSQL();
                DataSet ds = new DataSet();
                this.ExecuteQuery(ds, "TableMittente", queryString);
                if (ds != null && ds.Tables["TableMittente"].Rows.Count != 0)
                {
                    foreach (DataRow row in ds.Tables["TableMittente"].Rows)
                    {
                        info.IDAmministrazione = row["ID_AMM"].ToString();
                        info.IDGroup = row["ID_GRUPPO"].ToString();
                        info.IDCorrGlobaleRuolo = row["ID_CORR_GLOBALE_RUOLO"].ToString();
                        info.IDPeople = row["ID_PEOPLE"].ToString();
                        info.LivelloRuolo = row["num_livello"].ToString();
                    }
                }
            }

            return info;
        }

        /// <summary>
        /// Reperimento di tutti i documenti trasmessi ad un utente ( tab Notify)
        /// </summary>
        /// <param name="mittente"></param>
        /// <returns></returns>
        public DataSet GetListDocumentiTrasmessiNotify(System.Collections.Generic.List<Notification> notifications, DocsPaVO.Smistamento.MittenteSmistamento mittente)
        {
            //Lnr 22/05/2013

            string userDB = String.Empty;
            if (dbType.ToUpper() == "SQL")
            {
                userDB = getUserDB();
            }
            // aggiungo registro "0" per i doc grigi.
            mittente.RegistriAppartenenza.Add("0");
            string getDescCorr = !string.IsNullOrEmpty(userDB) ? userDB + ".getDescCorr(a.ID_RUOLO_IN_UO)" : "getDescCorr(a.ID_RUOLO_IN_UO)";
            string getPeopleName = !string.IsNullOrEmpty(userDB) ? userDB + ".getPeopleName(a.ID_PEOPLE)" : "getPeopleName(a.ID_PEOPLE)";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_LIST_DOCUMENTI_IN_NOTIFY");
            q.setParam("param1", mittente.IDPeople);
            q.setParam("getDescCorr", getDescCorr);
            q.setParam("getPeopleName", getPeopleName);
            
            string queryWhere = string.Empty;

            // Se la lista dei system id è valorizzata, viene aggiunta un filtro dui system id
            if (notifications != null &&
                notifications.Count > 0)
            {
                int i = 0;
                queryWhere += " AND ( S.SYSTEM_ID IN(";
                foreach (Notification idTrasm in notifications)
                {
                    queryWhere += idTrasm.ID_SPECIALIZED_OBJECT;
                    if (i < notifications.Count - 1)
                    {
                        if (i % 998 == 0 && i > 0)
                        {
                            queryWhere += ") OR S.SYSTEM_ID IN (";
                        }
                        else
                        {
                            queryWhere += ", ";
                        }
                    }
                    else
                    {
                        queryWhere += ")";
                    }
                    i++;
                }
                queryWhere += ")";
            }

            q.setParam("param2", queryWhere);

            string queryString = q.getSQL();

            logger.Debug("S_SMISTADOC_GET_LIST_DOCUMENTI_IN_NOTIFY:" + queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "DOCUMENTI_TRASMESSI", queryString);
            return ds;
        }

        /// <summary>
        /// Reperimento di tutti i documenti trasmessi ad un utente con filtri
        /// </summary>
        /// <param name="mittente"></param>
        /// <param name="filtriRicerca"></param>
        /// <returns></returns>
        public DataSet GetListDocumentiTrasmessiNotifyFilters(DocsPaVO.Smistamento.MittenteSmistamento mittente, DocsPaVO.filtri.FiltroRicerca[] filtriRicerca)
        {
            //Lnr 09/06/2013

            string userDB = String.Empty;
            if (dbType.ToUpper() == "SQL")
            {
                userDB = getUserDB();
            }
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SMISTADOC_GET_LIST_DOCUMENTI_IN_NOTIFY_FILTERED");
            // aggiungo registro "0" per i doc grigi.
            mittente.RegistriAppartenenza.Add("0");
            q.setParam("param1", mittente.IDPeople);
            q.setParam("param2", mittente.IDGroup);
            //q.setParam("param3", this.ConcatArrayRegRuolo(mittente.RegistriAppartenenza));

            //necessita di utente db SQL per la gestione della funzione vardescribe
            if (userDB != null) q.setParam("dbuser", userDB);
            Trasmissione trasm = new Trasmissione();

            if (filtriRicerca != null)
            {
                string filterWhere = trasm.getwhereNotifyFilter(filtriRicerca);
                q.setParam("filters", filterWhere);
            }

            //ORDINAMENTO, NB: funziona anche con filtriRicerca =null
            string filterOrder = trasm.getorderNotifyFilter(filtriRicerca, "S");
            q.setParam("order", filterOrder);

            trasm.Dispose();


            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet ds = new DataSet();

            this.ExecuteQuery(out ds, "DOCUMENTI_TRASMESSI", queryString);
            return ds;
        }

    }
}
