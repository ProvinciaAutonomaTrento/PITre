// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using System.Collections;
using System.Data;

namespace BusinessLogic.Trasmissioni
{
    public class QueryTrasmManager
    {
        private static ILogger logger = Serilog.Log.ForContext(typeof(QueryTrasmManager));

        private static Hashtable utenti = new Hashtable();
        private static Hashtable ruoli = new Hashtable();
        private static Hashtable corrispondenti = new Hashtable();
        private static Hashtable ragioni = new Hashtable();

        public static DocsPaVO.trasmissione.RagioneTrasmissione getRagioneById(string idRagione)
        {
            DocsPaVO.trasmissione.RagioneTrasmissione ragione = null;
            if (ragioni.ContainsKey(idRagione))
                ragione = (DocsPaVO.trasmissione.RagioneTrasmissione)ragioni[idRagione];
            else
            {
                ragione = RagioniManager.getRagione(idRagione);
                if (ragione != null)
                    ragioni.Add(ragione.systemId, ragione);
                else
                    logger.Debug("ATTENZIONE! ragione di trasmissione inesistente. ID: " + idRagione);
            }
            return ragione;
        }

        public static System.Collections.ArrayList getQueryDettaglioTrasmMethod(
               DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente,
               DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, string systemIDTrasm)
        {

            logger.Debug("getQueryDettaglioTrasmMethod");
            System.Collections.ArrayList lista = new System.Collections.ArrayList();

            try
            {
                DataSet dataSet;
                bool repeatQuery = false;
                DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();

                // prima query ritorna un bool !?!?
                strDB.getDettaglioTrasmissione(ref repeatQuery, out dataSet, objUtente.idPeople, objRuolo, objListaFiltri, objOggettoTrasmesso, systemIDTrasm);

                bool inf = false;
                // seconda query 
                lista = getListaTrasmissioni(dataSet, repeatQuery, "R", inf, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione delle trasmissioni (getQueryDettaglioTrasmMethod)", e);
                throw new Exception(e.Message);
            }

            return lista;
        }

        private static System.Collections.ArrayList getListaTrasmissioni(DataSet dataSet, bool repeatQuery, string tipoQuery, bool inf, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            //DocsPa_V15_Utils.database.SqlServerAgent db = new DocsPa_V15_Utils.database.SqlServerAgent();
            logger.Debug("getListaTrasmissioni");

            if (!utenti.ContainsKey(objUtente.idPeople))
            {
                utenti.Add(objUtente.idPeople, objUtente);
            }
            if (!ruoli.ContainsKey(objRuolo.systemId))
            {
                ruoli.Add(objRuolo.systemId, objRuolo);
            }

            #region Codice Commentato
            //dataSet = getDocumentiTrasmissioni(db, dataSet, objUtente, objRuolo);

            // leggo il valore del corrispondente associato all'utente
            /*
            string sqlString = 
                "SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE = " + objUtente.idPeople;
            logger.Debug(sqlString);
            objUtente.systemId = db.executeScalar(sqlString).ToString();
            */
            #endregion

            DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            objUtente.systemId = strDB.getQueryEffMet1(objUtente.idPeople);

            logger.Debug("repeatQuery = " + repeatQuery);
            if (repeatQuery) repeatQueryTrasmissioni(ref dataSet, objOggettoTrasmesso, objListaFiltri);

            Hashtable htTrasmissioni = new Hashtable();
            ArrayList result = new ArrayList();

            int numRighe = Int32.Parse(DocsPaVO.Settings.AppSettings.Instance.numeroMaxRisultatiQuery);
            for (int i = 0; i < dataSet.Tables["TRASMISSIONI"].Rows.Count && i < numRighe; i++)
            {
                DataRow trasmissioneRow = dataSet.Tables["TRASMISSIONI"].Rows[i];
                string idTrasmissione = trasmissioneRow["ID_TRASMISSIONE"].ToString();

                if (!htTrasmissioni.ContainsKey(idTrasmissione))
                {
                    DocsPaVO.trasmissione.Trasmissione trasm = getTrasmissione(dataSet, trasmissioneRow, tipoQuery, inf, objOggettoTrasmesso, objUtente, objRuolo);
                    if (trasm != null)
                    {
                        try
                        {
                            htTrasmissioni.Add(idTrasmissione, null);
                        }
                        catch { };

                        result.Add(trasm);
                    }
                }
                else
                {
                    numRighe++;
                }
                if (htTrasmissioni.Count > Int32.Parse(DocsPaVO.Settings.AppSettings.Instance.numeroMaxRisultatiQuery))
                    break;
            }
            utenti.Clear();
            ruoli.Clear();
            corrispondenti.Clear();
            ragioni.Clear();

            return result;
        }

        private static void repeatQueryTrasmissioni(ref DataSet dataSet, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            logger.Debug("repeatQueryTrasmissioni");
            ArrayList idTrasmissioneList = new ArrayList();

            foreach (DataRow dataRow in dataSet.Tables["TRASMISSIONI"].Rows)
            {
                if (!idTrasmissioneList.Contains(dataRow["ID_TRASMISSIONE"].ToString()))
                    idTrasmissioneList.Add(dataRow["ID_TRASMISSIONE"].ToString());
            }
            if (idTrasmissioneList.Count > 0)
            {
                #region Codice Commentato
                /*
				string inStr = "(" + (string)idTrasmissioneList[0];
				for (int i = 1; i < idTrasmissioneList.Count; i++)
					inStr += ", " + (string)idTrasmissioneList[i];
				inStr += ")";
				dataSet.Tables.Remove("TRASMISSIONI");
				DocsPa_V15_Utils.Query query = getQueryTrasmissioni(objOggettoTrasmesso);
				query.Where += " AND B.ID_TRASMISSIONE IN " + inStr;
				string queryString = query.getQuery();
				logger.Debug(queryString);
				db.fillTable(queryString, dataSet, "TRASMISSIONI");
				*/
                #endregion

                DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                strDB.repeatQueryTrasm(ref dataSet, objOggettoTrasmesso, idTrasmissioneList, objListaFiltri);

            }
            //			DocsPa_V15_Utils.Logger.log("Fine repeatQueryTrasmissioni", logLevelTime);
        }

        // tipoQuery: E=effettuata, R=ricevuta
        /// <summary>
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="dataRow"></param>
        /// <param name="tipoQuery"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <returns></returns>
        private static DocsPaVO.trasmissione.Trasmissione getTrasmissione(DataSet dataSet, DataRow dataRow, string tipoQuery, bool inf, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo)
        {
            logger.Debug("getTrasmissione");

            DocsPaVO.trasmissione.Trasmissione objTrasm = new DocsPaVO.trasmissione.Trasmissione();

            objTrasm.systemId = dataRow["ID_TRASMISSIONE"].ToString();
            DocsPaVO.utente.InfoUtente objSicurezza = null;
            if (objOggettoTrasmesso != null && objOggettoTrasmesso.infoDocumento != null)
            {
                objTrasm.infoDocumento = objOggettoTrasmesso.infoDocumento;
            }
            else
            {
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                if (objUtente != null && objRuolo != null)
                {
                    objSicurezza = new DocsPaVO.utente.InfoUtente(objUtente, objRuolo);

                    //Tolta la sicurezza per permettere di vedre trasmissioni sottoposti
                    objTrasm.infoDocumento = doc.GetInfoDocumento(objSicurezza.idGruppo, objSicurezza.idPeople, dataRow["ID_PROFILE"].ToString(), true);
                    //objTrasm.infoDocumento = doc.GetInfoDocumento(null, null, dataRow["ID_PROFILE"].ToString(), false);
                }
                else
                {
                    objTrasm.infoDocumento = doc.GetInfoDocumento(null, null, dataRow["ID_PROFILE"].ToString(), false);
                }
            }
            if (objOggettoTrasmesso != null && objOggettoTrasmesso.infoFascicolo != null)
                objTrasm.infoFascicolo = objOggettoTrasmesso.infoFascicolo;
            else
            {
                if (objSicurezza == null && objUtente != null && objRuolo != null)
                    objSicurezza = new DocsPaVO.utente.InfoUtente(objUtente, objRuolo);
                // objTrasm.infoFascicolo = BusinessLogic.Fascicoli.FascicoloManager.getInfoFascicolo(dataRow["ID_PROJECT"].ToString(),objSicurezza);
                //objTrasm.infoFascicolo = null;

                //25102004 aggiunta creazione oggetto infofascicolo 
                //per risolvere anomalia(137) sul link fascicolo
                //per il futuro verificare se l'oggetto necessita di ulteriori informazioni
                objTrasm.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo();
                objTrasm.infoFascicolo.idFascicolo = dataRow["ID_PROJECT"].ToString();

                //29102004
                //aggiunto Veronica, in risposta al commento sopra... è arrivato il momento in cui servono
                //ulteriori informazioni. Per poter visualizzare il codice e la descrizione del fascicolo 
                //nel datagrid della ricerca delle trasmissioni vado a leggere la tabella project (anomalia 641)
                if (objTrasm.infoFascicolo.idFascicolo != "")
                {
                    DataSet dataSetFasc = new DataSet();
                    DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                    strDB.GetInfoFasc(out dataSetFasc, Convert.ToInt32(objTrasm.infoFascicolo.idFascicolo));
                    if (dataSetFasc.Tables["FASCICOLI"].Rows.Count > 0)
                    {
                        DataRow fascicoloRow = dataSetFasc.Tables["FASCICOLI"].Rows[0];
                        objTrasm.infoFascicolo.codice = fascicoloRow["VAR_CODICE"].ToString();
                        objTrasm.infoFascicolo.descrizione = fascicoloRow["DESCRIPTION"].ToString();
                        objTrasm.infoFascicolo.apertura = fascicoloRow["DTA_APERTURA"].ToString();
                        if (fascicoloRow["ID_REGISTRO"] != DBNull.Value)
                            objTrasm.infoFascicolo.idRegistro = fascicoloRow["ID_REGISTRO"].ToString();
                    }
                    dataSetFasc.Dispose();
                }
                //fine aggiunta 29102004
                //fine aggiunta 25102004
            }
            if (objTrasm.infoDocumento == null && objTrasm.infoFascicolo == null)
                return null;
            if (objUtente.idPeople.Equals(dataRow["ID_PEOPLE"].ToString()))
                objTrasm.utente = objUtente;
            {
                DocsPaVO.utente.Utente userNew = new DocsPaVO.utente.Utente();
                userNew.idPeople = dataRow["ID_PEOPLE"].ToString();
                userNew.descrizione = dataRow["FULL_NAME"].ToString();
                userNew.userId = dataRow["USER_ID"].ToString();
                objTrasm.utente = userNew;
                // objTrasm.utente = getUtente(dataRow["ID_PEOPLE"].ToString());
            }

            if (objRuolo.systemId.Equals(dataRow["ID_RUOLO_IN_UO"].ToString()))
                objTrasm.ruolo = objRuolo;
            else
            {
                //objTrasm.ruolo = getRuoloEnabledAndDisabled(dataRow["ID_RUOLO_IN_UO"].ToString());
                DocsPaVO.utente.Ruolo ruoloNew = new DocsPaVO.utente.Ruolo();
                ruoloNew.systemId = dataRow["ID_RUOLO_IN_UO"].ToString();
                ruoloNew.descrizione = dataRow["desc_ruolo_mitt"].ToString();
                ruoloNew.codice = dataRow["cod_ruolo_mitt"].ToString();

                //Emanuela 15-06-2015: aggiunto idGruppo
                if (dataRow.Table.Columns.Contains("id_gruppo") && !string.IsNullOrEmpty(dataRow["id_gruppo"].ToString()))
                {
                    ruoloNew.idGruppo = dataRow["id_gruppo"].ToString();
                }
                objTrasm.ruolo = ruoloNew;
                //objTrasm.ruolo = getRuolo(dataRow["ID_RUOLO_IN_UO"].ToString())
            }

            if (dataRow["DTA_INVIO_F"] != null)
                objTrasm.dataInvio = dataRow["DTA_INVIO_F"].ToString().Trim();
            objTrasm.daAggiornare = false;
            objTrasm.tipoOggetto = (DocsPaVO.trasmissione.TipoOggetto)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.Trasmissione.oggettoStringa, dataRow["CHA_TIPO_OGGETTO"].ToString().Trim());

            objTrasm.noteGenerali = dataRow["VAR_NOTE_GENERALI"].ToString();

            objTrasm.delegato = "";
            if (dataRow["ID_PEOPLE_DELEGATO"] != null)
                if (!string.IsNullOrEmpty(dataRow["ID_PEOPLE_DELEGATO"].ToString()))
                {
                    DocsPaDB.Query_DocsPAWS.Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();
                    DocsPaVO.utente.Utente delegato = utente.GetUtenteNoFiltroDisabled(dataRow["ID_PEOPLE_DELEGATO"].ToString());
                    //Uso il metodo GetUtenteNoFiltroDisabled e non GetUtente perchè nel caso in cui il delegato è disabilitato non ritorna nulla
                    //info.UtenteDelegato = utente.GetUtente(dataRow["ID_PEOPLE_DELEGATO"].ToString()).descrizione;

                    objTrasm.delegato = (delegato != null) ? delegato.descrizione : string.Empty;
                }

            // trasmissione salvata con cessione dei diritti
            if (dataRow["CHA_SALVATA_CON_CESSIONE"].ToString() != null && dataRow["CHA_SALVATA_CON_CESSIONE"].ToString().Equals("1"))
                objTrasm.salvataConCessione = true;

            objTrasm = getTrasmSingole(dataSet, objTrasm, tipoQuery, inf, objUtente, objRuolo);

            return objTrasm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="objTrasm"></param>
        /// <param name="tipoQuery"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <returns></returns>
        private static DocsPaVO.trasmissione.Trasmissione getTrasmSingole(DataSet dataSet, DocsPaVO.trasmissione.Trasmissione objTrasm, string tipoQuery, bool inf, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo)
        {
            logger.Debug("getTrasmSingole");
            string queryString = "";
            queryString = "ID_TRASMISSIONE=" + objTrasm.systemId;

            //EM tolto perchè non permetteva di trovare da ricerca trasm ricevute le trasmissioni singole.
            // leggo solo le righe riferite all'utente, ho settato objUtente.systemId in precedenza, se necessario
            //if (tipoQuery.Equals("R") && !inf) 
            //{	
            //   queryString += " AND (ID_CORR_GLOBALE = " + objRuolo.systemId + " OR ID_CORR_GLOBALE=" + objUtente.systemId + ")";
            //}		

            logger.Debug(queryString);

            DataView dv = dataSet.Tables["TRASMISSIONI"].DefaultView;
            dv.RowFilter = queryString;
            dv.Sort = "CHA_TIPO_DEST ASC";

            string id_TrasmSingola = string.Empty;
            Hashtable trasmsingole = new Hashtable();

            foreach (DataRowView rowView in dv)
            {
                DataRow dataRow = rowView.Row;

                DocsPaVO.trasmissione.RagioneTrasmissione newRag = new DocsPaVO.trasmissione.RagioneTrasmissione();
                if (!trasmsingole.ContainsKey(Int32.Parse(dataRow["ID_TRASM_SINGOLA"].ToString().Trim())))
                {
                    trasmsingole.Add(Int32.Parse(dataRow["ID_TRASM_SINGOLA"].ToString().Trim()), dataRow["ID_TRASM_SINGOLA"].ToString());
                    id_TrasmSingola = dataRow["ID_TRASM_SINGOLA"].ToString();
                    DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
                    objTrasmSingola.systemId = id_TrasmSingola;
                    newRag.systemId = dataRow["ID_RAGIONE"].ToString();
                    newRag.descrizione = dataRow["VAR_DESC_RAGIONE"].ToString();
                    newRag.tipo = dataRow["cha_tipo_ragione"].ToString();
                    newRag.tipoDiritti = (DocsPaVO.trasmissione.TipoDiritto)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoDirittoStringa, dataRow["cha_tipo_diritti"].ToString());
                    newRag.risposta = dataRow["cha_risposta"].ToString();
                    newRag.tipoDestinatario = (DocsPaVO.trasmissione.TipoGerarchia)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa, dataRow["tipo_dest_rag"].ToString());
                    newRag.note = dataRow["var_note"].ToString();
                    newRag.eredita = dataRow["cha_eredita"].ToString();
                    if (dataRow["cha_tipo_risposta"] != null)
                    {
                        newRag.tipoRisposta = dataRow["cha_tipo_risposta"].ToString();
                    }
                    newRag.notifica = dataRow["var_notifica_trasm"].ToString();

                    if (dataRow["var_testo_msg_notifica_doc"] != null)
                        newRag.testoMsgNotificaDoc = dataRow["var_testo_msg_notifica_doc"].ToString();

                    if (dataRow["var_testo_msg_notifica_fasc"] != null)
                        newRag.testoMsgNotificaFasc = dataRow["var_testo_msg_notifica_fasc"].ToString();

                    if (dataRow["cha_cede_diritti"] != null)
                        newRag.prevedeCessione = dataRow["cha_cede_diritti"].ToString();
                    else
                        newRag.prevedeCessione = "N";
                    if (dataRow["cha_mantieni_lett"] != null)
                        newRag.mantieniLettura = dataRow["cha_mantieni_lett"].ToString();
                    else
                        newRag.mantieniLettura = "false";

                    //MEV LIBRO FIRMA 04-06-2015
                    if (dataRow.Table.Columns.Contains("CHA_PROC_RES") && !string.IsNullOrEmpty(dataRow["CHA_PROC_RES"].ToString()))
                    {
                        newRag.azioneRichiesta = dataRow["CHA_PROC_RES"].ToString();
                    }
                    //MEV FASCICOLAZIONE OBBLIGATORIA
                    if (dataRow.Table.Columns.Contains("CHA_FASC_OBBLIGATORIA") && !string.IsNullOrEmpty(dataRow["CHA_FASC_OBBLIGATORIA"].ToString()))
                    {
                        newRag.fascicolazioneObbligatoria = dataRow["CHA_FASC_OBBLIGATORIA"].ToString().Equals("1");
                    }

                    //VEDO SE è UNA RAGIONE DI TIPO TASK
                    if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ENABLE_TASK")) &&
                            DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ENABLE_TASK").ToString().Equals("1"))
                    {
                        DocsPaDB.Query_DocsPAWS.Task t = new DocsPaDB.Query_DocsPAWS.Task();
                        newRag.isTipoTask = t.IsRagioneDiTipoTask(newRag.systemId);
                    }
                    objTrasmSingola.ragione = newRag;
                    //  objTrasmSingola.ragione = getRagione(dataRow["ID_RAGIONE"].ToString());


                    objTrasmSingola.tipoDest = (DocsPaVO.trasmissione.TipoDestinatario)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.TrasmissioneSingola.tipoDestStringa, dataRow["CHA_TIPO_DEST"].ToString());
                    objTrasmSingola.corrispondenteInterno = getCorrispondente(dataRow["ID_CORR_GLOBALE"].ToString(), true);
                    //Emanuela 17/03/2014: aggiungo un controllo sul corrispondente per evitare l'errore in caso di ruolo storicizzato
                    if (objTrasmSingola.corrispondenteInterno != null)
                    {
                        objTrasmSingola.noteSingole = dataRow["VAR_NOTE_SING"].ToString();
                        objTrasmSingola.tipoTrasm = dataRow["CHA_TIPO_TRASM"].ToString();
                        objTrasmSingola.idTrasmUtente = dataRow["ID_TRASM_UTENTE"].ToString();
                        objTrasmSingola.dataScadenza = dataRow["DTA_SCADENZA"].ToString().Trim();

                        if (dataRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
                            objTrasmSingola.hideDocumentPreviousVersions = (dataRow["HIDE_DOC_VERSIONS"].ToString() == "1" ? true : false);
                        else
                            objTrasmSingola.hideDocumentPreviousVersions = false;

                        objTrasmSingola = getTrasmUtente(dataSet, objTrasmSingola, tipoQuery, inf, objUtente, objRuolo);
                        objTrasm.trasmissioniSingole.Add(objTrasmSingola);
                    }
                }
            }

            // DataRow[] trasmissioniSingoleRows = dataSet.Tables["TRASMISSIONI"].Select(queryString);

            //string id_TrasmSingola = "";
            //Hashtable trasmsingole=new Hashtable();

            //foreach(DataRow dataRow in trasmissioniSingoleRows) 
            //{
            //    if (!trasmsingole.ContainsKey(Int32.Parse(dataRow["ID_TRASM_SINGOLA"].ToString().Trim()))) 
            //    {
            //        trasmsingole.Add(Int32.Parse(dataRow["ID_TRASM_SINGOLA"].ToString().Trim()),dataRow["ID_TRASM_SINGOLA"].ToString());
            //        id_TrasmSingola = dataRow["ID_TRASM_SINGOLA"].ToString();
            //        DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
            //        objTrasmSingola.systemId = id_TrasmSingola;				
            //        objTrasmSingola.ragione = getRagione(dataRow["ID_RAGIONE"].ToString());		
            //        objTrasmSingola.tipoDest = (DocsPaVO.trasmissione.TipoDestinatario) DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.TrasmissioneSingola.tipoDestStringa,dataRow["CHA_TIPO_DEST"].ToString());			
            //        objTrasmSingola.corrispondenteInterno = getCorrispondente(dataRow["ID_CORR_GLOBALE"].ToString(), true);
            //        objTrasmSingola.noteSingole = dataRow["VAR_NOTE_SING"].ToString();			
            //        objTrasmSingola.tipoTrasm = dataRow["CHA_TIPO_TRASM"].ToString();	
            //        objTrasmSingola.idTrasmUtente = dataRow["ID_TRASM_UTENTE"].ToString();
            //        objTrasmSingola.dataScadenza = dataRow["DTA_SCADENZA"].ToString().Trim();
            //        objTrasmSingola = getTrasmUtente(dataSet, objTrasmSingola, tipoQuery, inf, objUtente, objRuolo);
            //        objTrasm.trasmissioniSingole.Add(objTrasmSingola);
            //    }
            //}

            //			DocsPa_V15_Utils.Logger.log("Fine getTrasmSingole", logLevelTime);	

            return objTrasm;
        }

        private static DocsPaVO.utente.Corrispondente getCorrispondente(string idCorrispondente, bool isEnable)
        {
            DocsPaVO.utente.Corrispondente corr = null;
            if (corrispondenti.ContainsKey(idCorrispondente))
                corr = (DocsPaVO.utente.Corrispondente)corrispondenti[idCorrispondente];
            else
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondente(idCorrispondente, isEnable);
                if (corr != null)
                    corrispondenti.Add(corr.systemId, corr);
            }
            return corr;
        }

        private static DocsPaVO.trasmissione.TrasmissioneSingola getTrasmUtente(DataSet dataSet, DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola, string tipoQuery, bool inf, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo)
        {
            IFormatProvider format = new System.Globalization.CultureInfo("it-IT", true);
            DateTime xdatePost;

            logger.Debug("getTrasmUtente");

            string queryString = "ID_TRASM_SINGOLA = " + objTrasmSingola.systemId;

            // legge solo le righe riferite all'utente
            if (tipoQuery.Equals("R") && !inf)
                queryString += " AND ID_DESTINATARIO=" + objUtente.idPeople;
            logger.Debug(queryString);

            DataRow[] trasmissioniUtenteRows = dataSet.Tables["TRASMISSIONI"].Select(queryString);

            foreach (DataRow dataRow in trasmissioniUtenteRows)
            {
                DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                objTrasmUtente.systemId = dataRow["ID_TRASMISSIONE_UTENTE"].ToString();
                if (objUtente.idPeople.Equals(dataRow["ID_DESTINATARIO"].ToString()))
                {
                    logger.Debug("senza getUtente");
                    objTrasmUtente.utente = objUtente;
                }
                else
                {
                    logger.Debug("getUtente");
                    objTrasmUtente.utente = getUtente(dataRow["ID_DESTINATARIO"].ToString());
                }
                objTrasmUtente.dataVista = dataRow["DTA_VISTA"].ToString().Trim();
                objTrasmUtente.dataAccettata = dataRow["DTA_ACCETTATA"].ToString().Trim();
                objTrasmUtente.dataRifiutata = dataRow["DTA_RIFIUTATA"].ToString().Trim();
                objTrasmUtente.noteRifiuto = dataRow["VAR_NOTE_RIF"].ToString();
                objTrasmUtente.noteAccettazione = dataRow["VAR_NOTE_ACC"].ToString();
                objTrasmUtente.valida = dataRow["CHA_VALIDA"].ToString();
                if (dataRow["DELEGATO_UTENTE"] != null)
                {
                    if (!string.IsNullOrEmpty(dataRow["DELEGATO_UTENTE"].ToString()) && !dataRow["DELEGATO_UTENTE"].ToString().Equals("0"))
                    {
                        DocsPaDB.Query_DocsPAWS.Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();
                        //objTrasmUtente.idPeopleDelegato = utente.GetUtente(dataRow["DELEGATO_UTENTE"].ToString()).descrizione;

                        objTrasmUtente.idPeopleDelegato = utente.GetUtenteNoFiltroDisabled(dataRow["DELEGATO_UTENTE"].ToString()).descrizione;
                    }
                }

                if (dataRow["ACCETTATA_DELEGATO"] != null)
                {
                    if (!string.IsNullOrEmpty(dataRow["ACCETTATA_DELEGATO"].ToString()))
                    {
                        objTrasmUtente.cha_accettata_delegato = dataRow["ACCETTATA_DELEGATO"].ToString();
                    }
                }

                if (dataRow["VISTA_DELEGATO"] != null)
                {
                    if (!string.IsNullOrEmpty(dataRow["VISTA_DELEGATO"].ToString()))
                    {
                        objTrasmUtente.cha_vista_delegato = dataRow["VISTA_DELEGATO"].ToString();
                    }
                }

                if (dataRow["RIFIUTATA_DELEGATO"] != null)
                {
                    if (!string.IsNullOrEmpty(dataRow["RIFIUTATA_DELEGATO"].ToString()))
                    {
                        objTrasmUtente.cha_rifiutata_delegato = dataRow["RIFIUTATA_DELEGATO"].ToString();
                    }
                }

                if (dataRow.Table.Columns.Contains("RIMOSSA_DELEGATO") && dataRow["RIMOSSA_DELEGATO"] != null)
                {
                    if (!string.IsNullOrEmpty(dataRow["RIMOSSA_DELEGATO"].ToString()))
                    {
                        objTrasmUtente.cha_rimossa_delegato = dataRow["RIMOSSA_DELEGATO"].ToString();
                    }
                }

                // nuovo campo rimozione dalla todolist
                if (!dataRow["DTA_RIMOSSA_TDL"].ToString().Equals(string.Empty))
                {
                    xdatePost = Convert.ToDateTime(dataRow["DTA_RIMOSSA_TDL"].ToString(), format);
                    objTrasmUtente.dataRimossaTDL = xdatePost.ToShortDateString();
                }
                objTrasmSingola.trasmissioneUtente.Add(objTrasmUtente);
                logger.Debug("Aggiunta tr.ut.");
            }

            //			DocsPa_V15_Utils.Logger.log("Fine getTrasmUtente", logLevelTime);

            return objTrasmSingola;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        private static DocsPaVO.utente.Utente getUtente(string idPeople)
        {
            logger.Debug("getUtente");
            DocsPaVO.utente.Utente utente = null;
            if (utenti != null && utenti.Count > 0 && utenti.ContainsKey(idPeople))
                utente = (DocsPaVO.utente.Utente)utenti[idPeople];
            else
            {
                utente = BusinessLogic.Utenti.UserManager.getUtenteNoFiltroDisabled(idPeople);
                if (utente.idPeople != null)
                {
                    try
                    {
                        if (!utenti.ContainsKey(idPeople))
                        {
                            utenti.Add(utente.idPeople, utente);
                        }
                    }
                    catch { };

                }
            }
            return utente;
        }

    }
}
