using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DocsPaUtils.Interfaces.DbManagement;
using DocsPaDbManagement.Functions;
using DocsPaVO.filtri;
using System.Linq;
using DocsPaVO.ProfilazioneDinamica;
using log4net;
using DocsPaVO.filtri.trasmissione;
using System.Text;
using DocsPaUtils;
using DocsPaUtils.Data;
using DocsPaUtils.Configuration;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Classe contenente lo strato DB di DocsPAWS > Trasmissioni
    /// </summary>
    public class Trasmissione : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(Trasmissione));
        protected Boolean eliminaTramissioniMultiple;

        private string chaiTableDef = string.Empty;

        private DocsPaUtils.Data.ParameterSP CreateParameter(string name, object value)
        {
            return new DocsPaUtils.Data.ParameterSP(name, value);
        }

        #region Query di ExecTrasmManager.cs


        /// <summary>
        /// ricerca dell'idpeople in base al tipo di corrisponente R o P
        /// </summary>
        /// <param name="idCorr"></param>
        /// <returns></returns>
        public string estrazioneTipoIdPeople(string idCorr)
        {
            string risultato = string.Empty;
            try
            {


                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("ESTRAZIONE_IDPEOPLEGROUP");
                q.setParam("idCorr", idCorr);
                string queryString = q.getSQL();
                logger.Debug("estrazioneTipoIdPeople - " + queryString);

                using (DocsPaDB.DBProvider db = new DocsPaDB.DBProvider())
                {
                    using (IDataReader dr = db.ExecuteReader(queryString))
                    {
                        if (dr.Read())
                        {
                            //if (dr.GetString(dr.GetOrdinal("CHA_TIPO_URP")).ToString().Equals("U")) 
                            //    return string.Empty;
                            //else
                            risultato = dr.GetValue(dr.GetOrdinal("idPeopleGroup")).ToString();
                        }

                    }
                }

            }
            catch (Exception e)
            {
                logger.Debug("estrazioneTipoIdPeople", e);
            }

            return risultato;
        }



        /// <summary>
        ///  gestione elimina trasmissioni ricevute da todolist
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <param name="idPeople"></param>
        /// <param name="idCorr"></param>
        public void trasmSetTxUtAsViste(string idCorrPeople, string idPeople, string idCorr)
        {
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("UPD_SET_TODOLIST_GIA_VISTA");
                q.setParam("param1", idCorr);
                q.setParam("param2", idPeople);
                q.setParam("param3", idCorrPeople);
                q.setParam("param4", DocsPaDbManagement.Functions.Functions.GetDate());
                string queryString = q.getSQL();
                logger.Debug(queryString);

                this.ExecuteNonQuery(queryString);
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella trasmSetTxUtAsViste", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 		/// <param name="idTrasmUtente"></param>
        /// <returns></returns>
        public string GetInRispostaA(string idTrasmUtente)
        {
            string queryString = "";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASM_UTENTE__TRASM_SINGOLA");
            q.setParam("param1", "B.SYSTEM_ID=A.ID_TRASM_SINGOLA");
            q.setParam("param2", idTrasmUtente);
            queryString = q.getSQL();

            //this.OpenConnection() ;
            string idTrasm = "";
            this.ExecuteScalar(out idTrasm, queryString);
            //this.CloseConnection() ;

            return idTrasm;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="systemId"></param>
        public string GetRispostaMethod(string systemId)
        {


            //db.openConnection();
            //this.OpenConnection();

            string queryString = "";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASM_UTENTE__TRASM_SINGOLA");
            q.setParam("param1", "A.ID_TRASM_RISP_SING=B.SYSTEM_ID");
            q.setParam("param1", systemId);
            queryString = q.getSQL();
            System.Data.DataSet ds;
            this.ExecuteQuery(out ds, "TRASM", queryString);

            string idTrasm = ds.Tables["TRASM"].Rows[0]["ID_TRASMISSIONE"].ToString();

            //si ottiene la trasmissione
            //			result=QueryTrasmManager.getTrasmissione(db,idTrasm,"E",null,objTrasmUtente.utente,((DocsPaVO.utente.Ruolo) objTrasmUtente.utente.ruoli[0]));

            //db.closeConnection();
            //this.CloseConnection();

            return idTrasm;
        }

        //public string GetUpdateTrasmUtente(DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente, string modo, string idTrasmissione)
        //{			
        //    //System.DateTime now=System.DateTime.Now;
        //    //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US"); 
        //    //string dateString = DocsPaDbManagement.Functions.Functions.ToDate(now.ToString("dd/MM/yyyy hh:mm:ss tt", ci ));

        //    string retValue = string.Empty;
        //    DocsPaUtils.Query q = null;

        //    string myParam2 = "";
        //    string addInQuery = " IN (SELECT TU.SYSTEM_ID FROM DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE=" + objTrasmUtente.utente.idPeople + " AND TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=" + idTrasmissione + " AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA AND TS.CHA_TIPO_DEST = 'U')";

        //    if(modo == "ACCETTATA")
        //    {
        //        myParam2 = " , CHA_IN_TODOLIST = '0'";

        //        q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASM_UTENTE_DTA_ACCETTATA");

        //        if(objTrasmUtente.noteAccettazione!=null)
        //            myParam2 = myParam2 + ", VAR_NOTE_ACC='" + objTrasmUtente.noteAccettazione.Replace("'", "''") + "'";

        //        q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetDate(true));

        //    }

        //    if(modo == "RIFIUTATA")
        //    {
        //        q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASM_UTENTE_DTA_RIFIUTATA");
        //        myParam2 = ", DTA_RIFIUTATA = " + DocsPaDbManagement.Functions.Functions.GetDate(true);
        //        if(objTrasmUtente.noteRifiuto!=null)
        //            myParam2 = myParam2 + ", VAR_NOTE_RIF='" + objTrasmUtente.noteRifiuto.Replace("'", "''") + "'";				   		
        //    }

        //    q.setParam("param2", myParam2);	
        //    //q.setParam("param3", " = " + objTrasmUtente.systemId);
        //    q.setParam("param3", addInQuery);

        //    retValue = q.getSQL();

        //    return retValue;		
        //}

        /// <summary>
        /// Questo metodo, a seconda della modalità (accettazione o rifiuto) ritorna la query che verrà eseguita 
        /// per aggiornare la tabella dpa_trasm_utente
        /// </summary>
        /// <param name="objTrasmUtente"></param>
        /// <param name="modo"></param>
        /// <param name="idTrasmissione"></param>
        /// <returns></returns>
        public string GetUpdateTrasmissioneUtente(DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente, string modo, string idTrasmissione, DocsPaVO.utente.InfoUtente infoUtente)
        {

            string retValue = string.Empty;
            DocsPaUtils.Query q = null;

            string myParam2 = "";

            if (modo == "ACCETTATA")
            {
                //Vecchio sistema: se si accettava direttamente dalla todolist una trasm.ne, 
                //questa scompariva dalla todolist anche se non si era mai visto il doc o il fasc.
                //Nuovo sistema: se si accetta una trasmissione dalla todolist, ma non si è 
                //ancora visto il documento o fascicolo, questo non scompare dalla todolist, ma vi rimane finchè non
                //viene visto

                if (!string.IsNullOrEmpty(objTrasmUtente.dataVista))
                {
                    DateTime dateVista = Convert.ToDateTime(objTrasmUtente.dataVista);
                    if (dateVista.CompareTo(DateTime.Parse("01/01/1753")) == 1)
                        myParam2 = " , CHA_IN_TODOLIST = '0', CHA_VALIDA = '0' ";
                    else
                        myParam2 = " , CHA_VALIDA = '0' ";
                }
                else
                    myParam2 = " , CHA_VALIDA = '0' ";

                if (infoUtente.delegato != null)
                    myParam2 += ", ID_PEOPLE_DELEGATO = " + infoUtente.delegato.idPeople + ", CHA_ACCETTATA = '1', CHA_ACCETTATA_DELEGATO = '1' ";
                else
                    myParam2 += ", CHA_ACCETTATA = '1' ";

                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASM_UTENTE_DTA_ACCETTATA_NEW");

                if (objTrasmUtente.noteAccettazione != null)
                    myParam2 = myParam2 + ", VAR_NOTE_ACC='" + objTrasmUtente.noteAccettazione.Replace("'", "''") + "'";

                q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetDate(true));
                q.setParam("param3", objTrasmUtente.systemId);
                q.setParam("param4", objTrasmUtente.utente.idPeople);
                q.setParam("param5", idTrasmissione);
                q.setParam("param2", myParam2);
            }

            if (modo == "RIFIUTATA")
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASM_UTENTE_RIFIUTATA_NEW2");
                myParam2 = " , CHA_IN_TODOLIST = '0', CHA_VALIDA = '0' ";
                // myParam2 = ", DTA_RIFIUTATA = " + DocsPaDbManagement.Functions.Functions.GetDate(true);
                if (objTrasmUtente.noteRifiuto != null)
                    myParam2 = myParam2 + ", VAR_NOTE_RIF='" + objTrasmUtente.noteRifiuto.Replace("'", "''") + "'";
                //if (!string.IsNullOrEmpty(delegato))
                //    myParam2 += ", ID_PEOPLE_DELEGATO = " + delegato + " ";
                if (infoUtente.delegato != null)
                    myParam2 += ", ID_PEOPLE_DELEGATO = " + infoUtente.delegato.idPeople + ", CHA_RIFIUTATA = '1', CHA_RIFIUTATA_DELEGATO = '1' ";
                else
                    myParam2 += ", CHA_RIFIUTATA = '1' ";

                //q.setParam("param100", myParam2);
                //q.setParam("param1", objTrasmUtente.systemId);
                //q.setParam("param2", objTrasmUtente.utente.idPeople);
                //q.setParam("param3", idTrasmissione);
                q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetDate(true));
                q.setParam("param3", objTrasmUtente.systemId);
                q.setParam("param4", objTrasmUtente.utente.idPeople);
                q.setParam("param5", idTrasmissione);
                q.setParam("param2", myParam2);

            }
            retValue = q.getSQL();

            return retValue;
        }

        //public string GetUpdateTrasmissioneUtenteDtaVista(DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente, string idTrasmissione)
        //{

        //    string retValue = string.Empty;
        //    DocsPaUtils.Query q = null;

        //    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASM_UTENTE_DTA_VISTA");

        //    q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetDate(true));
        //    q.setParam("param3", objTrasmUtente.systemId);
        //    q.setParam("param4", objTrasmUtente.utente.idPeople);
        //    q.setParam("param5", idTrasmissione);


        //    retValue = q.getSQL();

        //    return retValue;
        //}

        /// <summary>
        /// Questo metodo, aggiorna la dpa_trasm_utente solamente se la trasmissione è di tipo UNO 
        /// (questo poichè se la trasmissione è di tipo UNO se un componente del ruolo l'ha accettata
        /// tale trasmissione deve sparire dalla toDoList di ogni elemento del ruolo)
        /// </summary>
        /// <param name="objTrasmUtente"></param>
        /// <returns></returns>
        public bool UpdateTrasmUtenteAccettata(DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente, string idTrasmissione)
        {
            logger.Info("BEGIN");
            int rowsAff;
            bool retValue = true;

            System.Data.DataSet ds;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASM_UTENTE__TRASM_SINGOLA2");
            //old q.setParam("param1",objTrasmUtente.systemId);
            q.setParam("param2", idTrasmissione);
            q.setParam("param1", objTrasmUtente.utente.idPeople);
            q.setParam("param3", objTrasmUtente.systemId);
            string queryString = q.getSQL();
            logger.Debug(queryString);

            if (this.ExecuteQuery(out ds, "TRASM_SINGOLA", queryString))
            {
                System.Data.DataRow dr = ds.Tables["TRASM_SINGOLA"].Rows[0];

                if ((dr["CHA_TIPO_DEST"].ToString().Equals("R") || dr["CHA_TIPO_DEST"].ToString().Equals("U")) && dr["CHA_TIPO_TRASM"].ToString().Equals("S"))
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASM_UTENTE_CHA_VALIDA_0");
                    q.setParam("param1", idTrasmissione);
                    q.setParam("param2", objTrasmUtente.systemId);
                    q.setParam("param3", objTrasmUtente.utente.idPeople);

                    queryString = q.getSQL();
                    logger.Debug(queryString);

                    retValue = this.ExecuteNonQuery(queryString, out rowsAff);
                    logger.Debug("RowsAffected=" + Convert.ToString(rowsAff));
                }
                UpdateNotifyTrasm("Accept", objTrasmUtente.dataVista, idTrasmissione, objTrasmUtente.utente.idPeople, objTrasmUtente.systemId, dr);
            }

            ds.Dispose();
            logger.Info("END");
            return retValue;
        }

        public bool UpdateTrasmissioneUtenteRifiutata(DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente, string idTrasmissione)
        {
            logger.Info("BEGIN");
            int rowsAff;
            bool retValue = true;

            System.Data.DataSet ds;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASM_UTENTE__TRASM_SINGOLA2");
            q.setParam("param1", objTrasmUtente.utente.idPeople);
            q.setParam("param2", idTrasmissione);
            q.setParam("param3", objTrasmUtente.systemId);
            string queryString = q.getSQL();
            logger.Debug(queryString);

            if (this.ExecuteQuery(out ds, "TRASM_SINGOLA", queryString))
            {
                System.Data.DataRow dr = ds.Tables["TRASM_SINGOLA"].Rows[0];

                if ((dr["CHA_TIPO_DEST"].ToString().Equals("R") || dr["CHA_TIPO_DEST"].ToString().Equals("U")) && dr["CHA_TIPO_TRASM"].ToString().Equals("S"))
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASM_UTENTE_CHA_VALIDA_0");
                    q.setParam("param1", idTrasmissione);
                    q.setParam("param2", objTrasmUtente.systemId);
                    q.setParam("param3", objTrasmUtente.utente.idPeople);

                    queryString = q.getSQL();
                    logger.Debug(queryString);

                    retValue = this.ExecuteNonQuery(queryString, out rowsAff);
                    logger.Debug("RowsAffected=" + Convert.ToString(rowsAff));
                }
                //Update Notification Trasm
                UpdateNotifyTrasm("Reject", objTrasmUtente.dataVista, idTrasmissione, objTrasmUtente.utente.idPeople, objTrasmUtente.systemId, dr);
            }
            ds.Dispose();
            logger.Info("END");
            return retValue;
        }

        /// <summary>
        /// Update notifications related to the transmission when the user accepts or rejects it.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="dtaView"></param>
        /// <param name="idTrasm"></param>
        /// <param name="idPeople"></param>
        /// <param name="idTrasmUser"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        private bool UpdateNotifyTrasm(string mode, string dtaView, string idTrasm, string idPeople, string idTrasmUser, DataRow dr)
        {
                bool retValue = false;
                string queryString = string.Empty;
                string condition = string.Empty;
                int rowsAff;
                if ((dr["CHA_TIPO_DEST"].ToString().Equals("R") || 
                    dr["CHA_TIPO_DEST"].ToString().Equals("U")) && dr["CHA_TIPO_TRASM"].ToString().Equals("S"))
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_NOTIFY_HISTORY_TRASM_OTHER_PEOPLE_GROUP");
                    q.setParam("idPeople", idPeople);
                    q.setParam("idTrasm", idTrasm);
                    q.setParam("idTrasmUser", idTrasmUser);
                    if (string.IsNullOrEmpty(dtaView))
                    {
                        if (mode.Equals("Reject"))
                        {
                            condition = string.Empty;
                        }
                        else
                        {
                            condition = "AND ID_PEOPLE_RECEIVER != " + idPeople;
                        }
                    }
                    else
                    {
                        condition = string.Empty;
                    }
                    q.setParam("condition",condition);
                    queryString = q.getSQL();
                    retValue = this.ExecuteNonQuery(queryString, out rowsAff);
                    logger.Debug("RowsAffected=" + Convert.ToString(rowsAff));
                    //delete from dpa_notify
                    if (retValue)
                    {
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_NOTIFY_OTHER_TRASM_GROUP");
                        q.setParam("idPeople", idPeople);
                        q.setParam("idTrasm", idTrasm);
                        q.setParam("idTrasmUser", idTrasmUser);
                        q.setParam("condition", condition);
                        queryString = q.getSQL();
                        retValue = this.ExecuteNonQuery(queryString, out rowsAff);
                        logger.Debug("RowsAffected=" + Convert.ToString(rowsAff));
                    }
                }
                if ((dr["CHA_TIPO_DEST"].ToString().Equals("R") ||
                        dr["CHA_TIPO_DEST"].ToString().Equals("U")) && dr["CHA_TIPO_TRASM"].ToString().Equals("T"))
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_NOTIFY_HISTORY_TRASM_OTHER_PEOPLE_GROUP");
                    q.setParam("idPeople", idPeople);
                    q.setParam("idTrasm", idTrasm);
                    q.setParam("idTrasmUser", idTrasmUser);
                    if (string.IsNullOrEmpty(dtaView))
                    {
                        if (mode.Equals("Reject"))
                        {
                            condition = "AND ID_PEOPLE_RECEIVER = " + idPeople;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        condition = "AND ID_PEOPLE_RECEIVER = " + idPeople;
                    }
                    q.setParam("condition", condition);
                    queryString = q.getSQL();
                    retValue = this.ExecuteNonQuery(queryString, out rowsAff);
                    logger.Debug("RowsAffected=" + Convert.ToString(rowsAff));
                    //delete from dpa_notify
                    if (retValue)
                    {
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_NOTIFY_OTHER_TRASM_GROUP");
                        q.setParam("idPeople", idPeople);
                        q.setParam("idTrasm", idTrasm);
                        q.setParam("idTrasmUser", idTrasmUser);
                        q.setParam("condition", condition);
                        queryString = q.getSQL();
                        retValue = this.ExecuteNonQuery(queryString, out rowsAff);
                        logger.Debug("RowsAffected=" + Convert.ToString(rowsAff));
                    }
                }
                return retValue;
        }

        /// <summary>
        /// RIFIUTO TRASMISSIONE UTENTE. Ritorna al mittente
        /// </summary>
        /// <param name="trasmUtente"></param>
        /// <returns></returns>
        public bool RitornaAlMittTrasmUt(DocsPaVO.trasmissione.TrasmissioneUtente trasmUtente, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retValue = false;

            int retProc;

            // Creazione parametri SP
            ArrayList parameters = new ArrayList();
            string idPeople = trasmUtente.utente.idPeople;
            string idTrasmUtente = trasmUtente.systemId;
            string idAmministrazione = trasmUtente.utente.idAmministrazione;
            string idRuoloGlobMitt = string.Empty;

            if (trasmUtente.utente.ruoli.Count > 0)
            {
                DocsPaVO.utente.Ruolo ruolo = trasmUtente.utente.ruoli[0] as DocsPaVO.utente.Ruolo;
                if (ruolo != null)
                {
                    // idRuoloGlobMitt = ruolo.systemId;

                    DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                    DocsPaVO.utente.Ruolo newRuolo = new DocsPaVO.utente.Ruolo();
                    newRuolo = utenti.GetRuoloByIdGruppo(infoUtente.idGruppo);
                    idRuoloGlobMitt = newRuolo.systemId;

                    if (
                        idPeople != null &&
                        idTrasmUtente != null &&
                        idAmministrazione != null &&
                        idRuoloGlobMitt != null
                        )
                    {

                        parameters.Add(this.CreateParameter("IDPeopleMitt", idPeople));
                        parameters.Add(this.CreateParameter("IDRuoloGlobMitt", idRuoloGlobMitt));
                        parameters.Add(this.CreateParameter("IDAmministrazioneMittente", idAmministrazione));
                        parameters.Add(this.CreateParameter("IDTrasmUtente", idTrasmUtente));

                        this.BeginTransaction();

                        retProc = this.ExecuteStoreProcedure("SP_TRASM_RIF_TORNA_MITT", parameters);
                        /*
                            Valori di ritorno della SP:					
                            -- 0: Operazione andata a buon fine
                            -- da 1 a 9: Errore generico								
                        */
                        logger.Debug("Chiamata SP 'SP_TRASM_RIF_TORNA_MITT'. Esito: " + Convert.ToString(retProc));


                        // Commit / rollback transazione
                        switch (retProc)
                        {
                            case 0:
                                this.CommitTransaction();
                                logger.Debug("Eseguita Commit alla Stored Procedure: SP_TRASM_RIF_TORNA_MITT");
                                retValue = true;
                                break;
                            case 1:
                                this.RollbackTransaction();
                                logger.Debug("ERRORE - Eseguita Rollback alla Stored Procedure: SP_TRASM_RIF_TORNA_MITT - CAUSA : Non e' presente la ragione di trasmissione (RIFIUTO) per l'amministrazione corrente!");
                                break;
                            case 2:
                                this.RollbackTransaction();
                                logger.Debug("ERRORE - Eseguita Rollback alla Stored Procedure: SP_TRASM_RIF_TORNA_MITT - CAUSA : Query del reperimento del tipo di oggetto rifiutato ('documento' o 'fascicolo')");
                                break;
                            case 3:
                                this.RollbackTransaction();
                                logger.Debug("ERRORE - Eseguita Rollback alla Stored Procedure: SP_TRASM_RIF_TORNA_MITT - CAUSA : Query del reperimento della system_id della profile se il tipo oggetto è 'documento'");
                                break;
                            case 4:
                                this.RollbackTransaction();
                                logger.Debug("ERRORE - Eseguita Rollback alla Stored Procedure: SP_TRASM_RIF_TORNA_MITT - CAUSA : Query del reperimento della system_id della project se il tipo oggetto è 'fascicolo'");
                                break;
                            case 5:
                                this.RollbackTransaction();
                                logger.Debug("ERRORE - Eseguita Rollback alla Stored Procedure: SP_TRASM_RIF_TORNA_MITT - CAUSA : Query del reperimento della system_id della people del destinatario");
                                break;
                            case 6:
                                this.RollbackTransaction();
                                logger.Debug("ERRORE - Eseguita Rollback alla Stored Procedure: SP_TRASM_RIF_TORNA_MITT - CAUSA : Query del reperimento della system_id della dpa_corr_globali del destinatario o nome e cognome di chi ha rifiutato");
                                break;
                            case 7:
                                this.RollbackTransaction();
                                logger.Debug("ERRORE - Eseguita Rollback alla Stored Procedure: SP_TRASM_RIF_TORNA_MITT - CAUSA : Inserimento dati in tabella DPA_TRASMISSIONE");
                                break;
                            case 8:
                                this.RollbackTransaction();
                                logger.Debug("ERRORE - Eseguita Rollback alla Stored Procedure: SP_TRASM_RIF_TORNA_MITT - CAUSA : Inserimento dati in tabella DPA_TRASM_SINGOLA");
                                break;
                            case 9:
                                this.RollbackTransaction();
                                logger.Debug("ERRORE - Eseguita Rollback alla Stored Procedure: SP_TRASM_RIF_TORNA_MITT - CAUSA : Inserimento dati in tabella DPA_TRASM_UTENTE");
                                break;
                        }
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>
        public void SetSenderRights(string thing, string idGruppo, string idPeople)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_SECURITY_ACCESSRIGHTS_0");

            q.setParam("param1", thing);
            q.setParam("param1", idGruppo);
            q.setParam("param1", idPeople);

            this.ExecuteNonQuery(q.getSQL());

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public string GetRegistryCode(string systemId)
        {

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAELRegistri5");
            q.setParam("param1", systemId);
            string res = "";
            this.ExecuteScalar(out res, q.getSQL());
            return res;

        }

        /// <summary>
        /// Imposta la data di invio alla trasmissione
        /// </summary>
        /// <param name="systemId">ID della trasmissione</param>
        /// <returns>True o False</returns>
        public bool UpdateDataInvioTrasmissione(string systemId, string idDelegato)
        {
            bool retValue = false;

            DocsPaUtils.Query q =
                DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASMISSIONE_DTA_INVIO");
            string filterDelegato  = string.Empty;
            if (!string.IsNullOrEmpty(idDelegato))
                filterDelegato = ", ID_PEOPLE_DELEGATO = " + idDelegato;
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetDate() + filterDelegato);
            q.setParam("param2", systemId);
            string cmdText = q.getSQL();
            logger.Debug(cmdText);

            retValue = this.ExecuteNonQuery(cmdText);

            return retValue;
        }


        #region codice vecchio per gestione cessione diritti
        /// <summary>
        /// 
        /// </summary>
        /// <param name="executeQueries"></param>
        /// <param name="objTrasm"></param>
        public void execSenderRigths(ArrayList executeQueries, DocsPaVO.trasmissione.Trasmissione objTrasm)
        {
            try
            {
                //this.BeginTransaction();
                //setSenderRigths(objTrasm);
                for (int k = 0; k < executeQueries.Count; k++)
                {
                    this.ExecuteNonQuery(executeQueries[k].ToString());
                    logger.Debug("Eseguita query " + executeQueries[k]);
                }
                // this.CommitTransaction();
            }
            catch (Exception ex)
            {
                // this.RollbackTransaction(); 
                throw new Exception(ex.Message);
            }
            finally
            {
                //this.CloseConnection();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trasmissione"></param>
        /// <returns></returns>
        //private void setSenderRigths(DocsPaVO.trasmissione.Trasmissione trasmissione)
        //{
        //    logger.Debug("setSenderRigths");
        //    // verifico la consistenza dei dati
        //    bool cessione = false;
        //    if (((DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[0]).ragione.tipoDiritti == DocsPaVO.trasmissione.TipoDiritto.CESSION)
        //        cessione = true;
        //    string msg = "Le condizioni sui tipi diritti non sono rispettate";
        //    for (int i = 1; i < trasmissione.trasmissioniSingole.Count; i++)
        //    {
        //        if (cessione)
        //        {
        //            if (((DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).ragione.tipoDiritti != DocsPaVO.trasmissione.TipoDiritto.CESSION)
        //            {
        //                logger.Debug("Errore nella gestione delle trasmissioni (Query - setSenderRights). " + msg);
        //                throw new Exception(msg);
        //            }
        //        }
        //        else
        //        {
        //            if (((DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).ragione.tipoDiritti == DocsPaVO.trasmissione.TipoDiritto.CESSION)
        //            {
        //                logger.Debug("Errore nella gestione delle trasmissioni (Query - setSenderRights). " + msg);
        //                throw new Exception(msg);
        //            }
        //        }

        //    }

        //    if (cessione)
        //    {
        //        string thing = null;
        //        if (trasmissione.infoDocumento != null)
        //            thing = trasmissione.infoDocumento.idProfile;
        //        else if (trasmissione.infoFascicolo != null)
        //            thing = trasmissione.infoFascicolo.idFascicolo;
        //        if (thing != null)
        //        {
        //            /*string updateString = 
        //                "UPDATE SECURITY SET ACCESSRIGHTS = 0 WHERE THING = " + thing +
        //                " AND (PERSONORGROUP = " + trasmissione.ruolo.idGruppo +
        //                " OR PERSONORGROUP = " + trasmissione.utente.idPeople + ")";
        //            logger.Debug(updateString);
        //            db.executeNonQuery(updateString);*/
        //            SetSenderRights(thing, trasmissione.ruolo.idGruppo, trasmissione.utente.idPeople);
        //        }
        //    }
        //}
        #endregion

        /*public void execSenderRigths(DocsPaVO.trasmissione.Trasmissione objTrasm)
        {
            if (objTrasm.cessione != null)
            {
                if (objTrasm.cessione.docCeduto)
                {
                    DocsPaUtils.Query q;
                    string queryString = "";
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                    DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente();
                    infoUtente.idPeople = objTrasm.cessione.idPeople;
                    infoUtente.idGruppo = objTrasm.cessione.idRuolo;
                        
                    DocsPaVO.documento.DirittoOggetto dirittoObj = new DocsPaVO.documento.DirittoOggetto();
                    dirittoObj.idObj = objTrasm.infoDocumento.idProfile;
                    //l'utente cede il suo diritto di proprietà
                    if (objTrasm.cessione.idPeopleNewPropr != null && objTrasm.cessione.idRuoloNewPropr != null)
                    {
                        dirittoObj.tipoDiritto = DocsPaVO.documento.TipoDiritto.TIPO_PROPRIETARIO;
                        dirittoObj.personorgroup = objTrasm.cessione.idPeopleNewPropr;
                        dirittoObj.accessRights = 0;
                        DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
                        utente.idPeople = objTrasm.cessione.idPeopleNewPropr;
                        dirittoObj.soggetto = utente;
                        
                        string cod_rubrica = "";
                        using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                        {
                            //ricerca di id_gruppo_trasm in security
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_CORR_GLOB_GENERIC");
                            q.setParam("param1", "VAR_COD_RUBRICA");
                            q.setParam("param2", "ID_PEOPLE=" + objTrasm.cessione.idPeopleNewPropr);
                            queryString = q.getSQL();
                            logger.Debug(queryString);
                            dbProvider.ExecuteScalar(out cod_rubrica, queryString);

                        }

                        dirittoObj.soggetto.codiceRubrica = cod_rubrica;
                        doc.EditingACL(dirittoObj, "U", infoUtente);
                    }
                    else
                    { 
                        //l'utente cede il suo diritto non di proprietà
                        string tipoDiritto = "";
                        string accessRight = "";
                        string cod_rubrica = "";
                            
                        using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                        {
                            IDataReader dr = null;

                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SECURITY__DPA_CORR_GLOBALI__CESSIONE");
                            q.setParam("param1", objTrasm.infoDocumento.idProfile);
                            queryString = q.getSQL();
                            logger.Debug(queryString);
                            using (dr = dbProvider.ExecuteReader(queryString))
                            {
                                if (dr == null)
                                {
                                    throw new Exception("Errore in EditingACL");
                                }
                                if (dr != null && dr.FieldCount > 0)
                                {
                                    while (dr.Read())
                                    {
                                        accessRight = dr.GetValue(0).ToString();
                                        tipoDiritto = dr.GetValue(1).ToString();
                                        cod_rubrica = dr.GetValue(2).ToString();
                                    }
                                }
                            }
                            if (dr != null && (!dr.IsClosed))
                                dr.Close();

                        }

                        dirittoObj.tipoDiritto = getTipoDiritto(tipoDiritto);
                        dirittoObj.personorgroup = infoUtente.idPeople;
                        dirittoObj.accessRights = Convert.ToInt32(accessRight);
                        DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
                        utente.idPeople = infoUtente.idPeople;
                        dirittoObj.soggetto = utente;
                        dirittoObj.soggetto.codiceRubrica = cod_rubrica;

                        
                        doc.EditingACL(dirittoObj, "U", infoUtente);
                    }

                }


                

            }
           
           
        }*/

        /// <summary>
        /// CESSIONE DEI DIRITTI
        /// </summary>
        /// <param name="objTrasm"></param>
        public bool execSenderRigths(DocsPaVO.trasmissione.Trasmissione objTrasm)
        {
            bool cessioneEffettuata = false;
            logger.Debug("Inizio procedura di cessione dei diritti");
            if (objTrasm.cessione != null)
            {
                if (objTrasm.cessione.docCeduto)
                {
                    try
                    {
                        string accessRights = string.Empty;
                        string idGruppoTrasm = string.Empty;
                        string tipoDiritto = string.Empty;
                        string idPersonOrGroup = string.Empty;
                        string IDObject = string.Empty;
                        string tipoOggettoTrasm = string.Empty;
                        string tipoOggettoTrasmEsteso = string.Empty;
                        string tipoDirittoLog = string.Empty;
                        string IDObjectFolder = string.Empty;

                        bool docPersonale = false;
                        bool docPrivate = false;

                        // reprimento dell'ID dell'oggetto trasmesso
                        if (objTrasm.tipoOggetto.Equals(DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO))
                        {
                            IDObject = objTrasm.infoDocumento.idProfile;
                            tipoOggettoTrasm = "D";
                            tipoOggettoTrasmEsteso = "DOC";
                            docPersonale = (objTrasm.infoDocumento.personale != null && objTrasm.infoDocumento.personale.Equals("1"));
                            docPrivate = (!string.IsNullOrEmpty(objTrasm.infoDocumento.privato) && objTrasm.infoDocumento.privato.Equals("1"));
                        }
                        else
                        {
                            IDObject = objTrasm.infoFascicolo.idFascicolo;
                            tipoOggettoTrasm = "F";
                            tipoOggettoTrasmEsteso = "FASC";
                        }

                        using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                        {
                            // verifica se CEDE i diritti di PROPRIETA' (0,255,P) 
                            //-----------------------------------------------------------------------------------------------------
                            bool isPersonOwner = false;
                            bool isGroupOwner = false;

                            // verifica se è proprietario come UTENTE...
                            this.selectSecurity(out accessRights, out idGruppoTrasm, out tipoDiritto, IDObject, objTrasm.cessione.idPeople, "= 0");
                            isPersonOwner = (!accessRights.Equals("") && accessRights.Equals("0"));
                            logger.DebugFormat("Verifica la proprietà dell'oggeto come UTENTE. isPersonOwner: {0}", isPersonOwner.ToString());

                            // gestione del doc. personale
                            if (tipoOggettoTrasm.Equals("D") && isPersonOwner && docPersonale)
                            {
                                isGroupOwner = true; // imposta a true perchè per i doc personali non esiste il ruolo impostato                                
                            }
                            else
                            {
                                // verifica se è proprietario come RUOLO...
                                this.selectSecurity(out accessRights, out idGruppoTrasm, out tipoDiritto, IDObject, objTrasm.cessione.idRuolo, "= 255");
                                isGroupOwner = (!accessRights.Equals("") && accessRights.Equals("255"));
                            }
                            logger.DebugFormat("Verifica la proprietà dell'oggeto come RUOLO. isGroupOwner: {0}; tipoOggettoTrasm: {1}", isGroupOwner.ToString(), tipoOggettoTrasm);

                            //***************************************************************************************
                            //Modifica Iacozzilli Giordano 13/07/2012
                            //Ora posso cedere i diritti sul doc anche se non ne sono il proprietario.
                            try
                            {
                                string valoreChiaveCediDiritti = InitConfigurationKeys.GetValue(objTrasm.utente.idAmministrazione, "FE_CEDI_DIRITTI_IN_RUOLO");
                                logger.DebugFormat("Valore chiave FE_CEDI_DIRITTI_IN_RUOLO: {0}", valoreChiaveCediDiritti);
                                if (!string.IsNullOrEmpty(valoreChiaveCediDiritti) && valoreChiaveCediDiritti.Equals("1"))
                                {
                                    //Questa modifica deve scattare per ingannare il sistema facendogli credere
                                    //che l'utente che sta facendo la cessione dei diritti è il proprietario, anche se non lo è.
                                    //Deve però almeno far parte del Ruolo del proprietario (255).
                                    if (isGroupOwner)
                                        isPersonOwner = true;
                                    //FINE
                                    //**************************************************************************************************************
                                    //Modifica Iacozzilli Giordano 30/07/2012
                                    //Modifica per il LOG della cessione dei diritti da parte 
                                    //di un utente che non è l'Owner del doc.
                                    //LOG APPLICATIVO
                                    DocsPaVO.Logger.CodAzione.infoOggetto InfoOggetto;
                                    InfoOggetto = Log.getInfoOggetto("EXECSENDERRIGTHSNOOWNER", objTrasm.utente.idAmministrazione);
                                    if (InfoOggetto.Attivo == 1)
                                    {
                                        Log.InsertLog(objTrasm.cessione.userId,
                                                                    objTrasm.cessione.idPeople,
                                                                    objTrasm.cessione.idRuolo,
                                                                    objTrasm.utente.idAmministrazione,
                                                                    InfoOggetto.Oggetto.Trim(),
                                                                    IDObject,
                                                                    "Cessione diritti :" + "\\n"
                                                                                        + "Documento Num Prot -- : " + objTrasm.infoDocumento.numProt
                                                                                        + "\\n" + "Proprietario del documento -- IDPEOPLE : " + objTrasm.cessione.idPeople
                                                                                        + "\\n" + "Utente Cedente -- IDPEOPLE : " + objTrasm.utente.idPeople
                                                                                        + "\\n" + "Nuovo Proprietario -- IDPEOPLE : " + objTrasm.cessione.idPeopleNewPropr,
                                                                    InfoOggetto.Codice.Trim(),
                                                                    InfoOggetto.Descrizione.Trim(),
                                                                    DocsPaVO.Logger.CodAzione.Esito.OK, null, string.Empty);
                                    }
                                    //FINE
                                }
                            }
                            catch
                            {
                            }

                            //***************************************************************************************
                            //FINE
                            //***************************************************************************************

                            // se non è il proprietario come utente e ruolo, cederà i diritti acquisiti...
                            if (isPersonOwner && isGroupOwner)
                            {
                                tipoDirittoLog = "di proprietà";

                                logger.DebugFormat("Cessione Diritti. Tipo Diritto: {0}, isPersonOwner: {1}, isGroupOwner: {2}", tipoDirittoLog, isPersonOwner, isGroupOwner);
                                logger.DebugFormat("Valore idPeopleNewPropr: {0}, idRuoloNewPropr: {1}", objTrasm.cessione.idPeopleNewPropr, objTrasm.cessione.idRuoloNewPropr);
                                //l'utente cede il suo diritto di proprietà, prima come utente poi come ruolo
                                if ((objTrasm.cessione.idPeopleNewPropr != null && objTrasm.cessione.idRuoloNewPropr != null) ||
                                    (docPrivate && string.IsNullOrEmpty(objTrasm.cessione.idRuoloNewPropr)))
                                {
                                    logger.Debug("Cessione Diritti Di Prorietà. L'utente cede il suo diritto di proprietà, prima come utente poi come ruolo");
                                    logger.Debug("Inizio cessione diritti di proprietà documento/fascicolo come Utente");

                                    //rimozione dalla security
                                    if (!this.deleteSecurity(IDObject, objTrasm.cessione.idPeople, "0", false, false))
                                    {
                                        dbProvider.RollbackTransaction();
                                        throw new Exception();
                                    }
                                    
                                    //
                                    // Mev Cessione Diritti con Diritto Mantiene Scrittura

                                    logger.DebugFormat("Cessione Diritti di Proprietà Utente con Mantieni Diritti per Documento/fascicolo: Scrittura: {0}, Lettura: {1}", objTrasm.mantieniScrittura.ToString(), objTrasm.mantieniLettura.ToString());

                                    //Se l'opzione di mantenere i diritti di scrittura è attiva
                                    //non inserisco nella deleteSecurity ma lascio i diritti in scrittura
                                    if (objTrasm.mantieniScrittura == true)
                                    {
                                        //Lascio il permesso di scrittura al vecchio proprietario

                                        //
                                        // Modifica su riciesta di Zanotti
                                        // è stato commentato il seguente pezzo di codice:
                                        /*
                                        if (!this.insertSecurity(IDObject, objTrasm.cessione.idPeople, "63", "NULL", "A"))
                                        {
                                            dbProvider.RollbackTransaction();
                                            throw new Exception();
                                        }
                                        */
                                        //
                                        // End Modifica su riciesta di Zanotti
                                        
                                        objTrasm.dirittiCeduti = true;
                                    }
                                    else 
                                    {
                                        //Se l'opzione di mantenere i diritti di lettura è attiva
                                        //non inserisco nella deleteSecurity ma lascio i diritti in lettura
                                        if (objTrasm.mantieniLettura == true)
                                        {
                                            //Lascio il permesso di lettura al vecchio proprietario
                                            
                                            //
                                            // Modifica su riciesta di Zanotti
                                            // è stato commentato il seguente pezzo di codice:
                                            /*
                                            if (!this.insertSecurity(IDObject, objTrasm.cessione.idPeople, "45", "NULL", "A"))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }
                                            */
                                            //
                                            // End Modifica su riciesta di Zanotti

                                            objTrasm.dirittiCeduti = true;
                                        }
                                        //altrimenti inserisco nella deleteSecurity
                                        else
                                        {
                                            //inserimento nella deleted_security - il proprietario perde il diritto.
                                            if (!this.insertDeletedSecurity(objTrasm.cessione.userId, IDObject, objTrasm.cessione.idPeople, "0", "NULL", "P", objTrasm.cessione.idPeopleNewPropr, objTrasm.cessione.idRuoloNewPropr))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }
                                            else
                                            {
                                                //
                                                // MEV ACL - siamo nel caso di cessione diritti e Non mantiene alcun diritto.
                                                objTrasm.dirittiCeduti = true;
                                            }
                                        }
                                    }
                                    // End
                                    //

                                    //
                                    // Commentato per Mev Cessione Diritti Mantieni Diritti di Scrittura
                                    /*

                                    //Se l'opzione di mantenere i diritti di lettura è attiva
                                    //non inserisco nella deleteSecurity ma lascio i diritti in lettura
                                    if (objTrasm.mantieniLettura == true)
                                    {
                                        //Lascio il permesso di lettura al vecchio proprietario
                                        if (!this.insertSecurity(IDObject, objTrasm.cessione.idPeople, "45", "NULL", "A"))
                                        {
                                            dbProvider.RollbackTransaction();
                                            throw new Exception();
                                        }
                                        objTrasm.dirittiCeduti = true;
                                    }
                                    //altrimenti inserisco nella deleteSecurity
                                    else
                                    {
                                        //inserimento nella deleted_security
                                        if (!this.insertDeletedSecurity(objTrasm.cessione.userId, IDObject, objTrasm.cessione.idPeople, "0", "NULL", "P", objTrasm.cessione.idPeopleNewPropr, objTrasm.cessione.idRuoloNewPropr))
                                        {
                                            dbProvider.RollbackTransaction();
                                            throw new Exception();
                                        }
                                    }
                                    
                                    */
                                    // End Commento Mev Cessione Diritti e Mantieni diritti di scrittura
                                    //

                                    //inserimento nuovo utente proprietario in security
                                    if (!this.insertSecurity(IDObject, objTrasm.cessione.idPeopleNewPropr, "0", "NULL", "P"))
                                    {
                                        dbProvider.RollbackTransaction();
                                        throw new Exception();
                                    }

                                    //cancellazione di altri record preesistenti in security (di tipo ACQUISITO) del nuovo utente proprietario 
                                    if (!this.deleteSecurity(IDObject, objTrasm.cessione.idPeopleNewPropr, "0", true, true))
                                    {
                                        dbProvider.RollbackTransaction();
                                        throw new Exception();
                                    }

                                    
                                    // ##################################################
                                    // #              FASCICOLO    ROOT                 #
                                    // ##################################################
                                    if (tipoOggettoTrasm.Equals("F"))
                                    {
                                        // prende il fascicolo Root (cha_tipo_proj = 'F')
                                        IDObjectFolder = this.GetIDFolderRoot(IDObject);

                                        logger.DebugFormat("Cessione Diritti di Proprietà Utente per Fascicolo; IDObjectFolder: {0}", IDObjectFolder);

                                        if (IDObjectFolder != string.Empty)
                                        {
                                            logger.Debug("inizio cessione diritti ulteriori fascicolo");
                                            //rimozione dalla security
                                            if (!this.deleteSecurity(IDObjectFolder, objTrasm.cessione.idPeople, "0", false, false))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }

                                            ////inserimento nella deleted_security
                                            //if (!this.insertDeletedSecurity(objTrasm.cessione.userId, IDObjectFolder, objTrasm.cessione.idPeople, "0", "NULL", "P", objTrasm.cessione.idPeopleNewPropr, objTrasm.cessione.idRuoloNewPropr))
                                            //{
                                            //    dbProvider.RollbackTransaction();
                                            //    throw new Exception();
                                            //}

                                            //inserimento nuovo utente proprietario in security
                                            if (!this.insertSecurity(IDObjectFolder, objTrasm.cessione.idPeopleNewPropr, "0", "NULL", "P"))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }
                                            logger.Debug("end cessione diritti ulteriori fascicolo");
                                        }
                                    }
                                    // ##################################################
                                    logger.Debug("End cessione diritti di proprietà documento/fascicolo come Utente");

                                    //Se il ruolo dell'utente cedente è diverso dal ruolo del nuovo proprietario
                                    //si elimina anche il ruolo dalla security e si aggiunge in deleted_security.
                                    //Si aggiunge il ruolo del nuovo ruolo proprietario in security.
                                    //Si aggiunge il ruolo solo se non si tratta di doc. personali.
                                    if (objTrasm.cessione.idRuolo != objTrasm.cessione.idRuoloNewPropr)
                                    {
                                        if (!docPersonale)
                                        {
                                            logger.DebugFormat("Cessione Diritti di Proprietà Ruolo con Mantieni Diritti per Documento/fascicolo: Scrittura: {0}, Lettura: {1}", objTrasm.mantieniScrittura.ToString(), objTrasm.mantieniLettura.ToString());
                                            logger.Debug("Inizio cessione diritti di proprietà documento/fascicolo come Ruolo");
                                            //rimozione dalla security del ruolo cedente 
                                            if (!this.deleteSecurity(IDObject, objTrasm.cessione.idRuolo, "255", false, false))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }
                                            //
                                            // Mev Cessione Diritti, Mantieni Diritto Scrittura

                                            //Se abilitato il mantenimento dei diritti di scrittura non inserisco nella deleteSecurity
                                            if (objTrasm.mantieniScrittura == true)
                                            {
                                                //Lascio i diritti di scrittura al vecchio proprietario
                                                if (!this.insertSecurity(IDObject, objTrasm.cessione.idRuolo, "63", objTrasm.cessione.idRuolo, "A"))
                                                {
                                                    dbProvider.RollbackTransaction();
                                                    throw new Exception();
                                                }
                                                objTrasm.dirittiCeduti = true;
                                            }
                                            else 
                                            {
                                                //Se abilitato il mantenimento dei diritti di lettura non inserisco nella deleteSecurity
                                                if (objTrasm.mantieniLettura == true)
                                                {
                                                    //Lascio i diritti di lettura al vecchio proprietario
                                                    if (!this.insertSecurity(IDObject, objTrasm.cessione.idRuolo, "45", objTrasm.cessione.idRuolo, "A"))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }
                                                    objTrasm.dirittiCeduti = true;
                                                }
                                                else
                                                {
                                                    //inserimento nella deleted_security del ruolo cedente - Tengo traccia del vecchio proprietario
                                                    if (!this.insertDeletedSecurity(objTrasm.cessione.userId, IDObject, objTrasm.cessione.idRuolo, "255", objTrasm.cessione.idRuolo, "P", objTrasm.cessione.idPeopleNewPropr, objTrasm.cessione.idRuoloNewPropr))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }
                                                    else
                                                    {
                                                        //
                                                        // MEV ACL - siamo nel caso di cessione diritti e Non mantiene alcun diritto.
                                                        objTrasm.dirittiCeduti = true;
                                                    }
                                                }
                                            }
                                            // End Mev Cessione Diritti
                                            //

                                            //
                                            // Commentato per Mev Cessione Diritti Mantieni Diritti di Scrittura
                                            /*

                                            //Se abilitato il mantenimento dei diritti di lettura non inserisco nella deleteSecurity
                                            if (objTrasm.mantieniLettura == true)
                                            {
                                                //Lascio i diritti di lettura al vecchio proprietario
                                                if (!this.insertSecurity(IDObject, objTrasm.cessione.idRuolo, "45", objTrasm.cessione.idRuolo, "A"))
                                                {
                                                    dbProvider.RollbackTransaction();
                                                    throw new Exception();
                                                }
                                                objTrasm.dirittiCeduti = true;
                                            }
                                            else
                                            {
                                                //inserimento nella deleted_security del ruolo cedente
                                                if (!this.insertDeletedSecurity(objTrasm.cessione.userId, IDObject, objTrasm.cessione.idRuolo, "255", objTrasm.cessione.idRuolo, "P", objTrasm.cessione.idPeopleNewPropr, objTrasm.cessione.idRuoloNewPropr))
                                                {
                                                    dbProvider.RollbackTransaction();
                                                    throw new Exception();
                                                }
                                            }

                                            */
                                            // End Commento Mev Cessione Diritti e Mantieni diritti di scrittura
                                            //

                                            //se si tratta di cessione diritti ad utente di un documento privato allora trascuriamo la parte di security sul nuovo 
                                            //ruolo proprietario
                                            if (!string.IsNullOrEmpty(objTrasm.cessione.idRuoloNewPropr))
                                            {
                                                //inserimento nuovo ruolo proprietario in security
                                                if (!this.insertSecurity(IDObject, objTrasm.cessione.idRuoloNewPropr, "255", objTrasm.cessione.idRuoloNewPropr, "P"))
                                                {
                                                    dbProvider.RollbackTransaction();
                                                    throw new Exception();
                                                }

                                                //cancellazione di altri record preesistenti in security (di tipo ACQUISITO) del nuovo ruolo proprietario 
                                                if (!this.deleteSecurity(IDObject, objTrasm.cessione.idRuoloNewPropr, "0,255", true, true))
                                                {
                                                    dbProvider.RollbackTransaction();
                                                    throw new Exception();
                                                }
                                            }
                                            else if(!docPrivate)
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception("Non è presente il nuovo ruolo proprietario");
                                            }

                                            // ##################################################
                                            // #              FASCICOLO    ROOT                 #
                                            // ##################################################
                                            if (tipoOggettoTrasm.Equals("F"))
                                            {
                                                // prende il fascicolo Root (cha_tipo_proj = 'F')
                                                IDObjectFolder = this.GetIDFolderRoot(IDObject);

                                                logger.DebugFormat("Cessione Diritti di Proprietà Ruolo per Fascicolo; IDObjectFolder: {0}", IDObjectFolder);

                                                if (IDObjectFolder != string.Empty)
                                                {
                                                    logger.Debug("inizio cessione diritti aggiuntivi fascicolo");
                                                    //rimozione dalla security del ruolo cedente 
                                                    if (!this.deleteSecurity(IDObjectFolder, objTrasm.cessione.idRuolo, "255", false, false))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }

                                                    ////inserimento nella deleted_security del ruolo cedente
                                                    //if (!this.insertDeletedSecurity(objTrasm.cessione.userId, IDObjectFolder, objTrasm.cessione.idRuolo, "255", objTrasm.cessione.idRuolo, "P", objTrasm.cessione.idPeopleNewPropr, objTrasm.cessione.idRuoloNewPropr))
                                                    //{
                                                    //    dbProvider.RollbackTransaction();
                                                    //    throw new Exception();
                                                    //}

                                                    //inserimento nuovo ruolo proprietario in security
                                                    if (!this.insertSecurity(IDObjectFolder, objTrasm.cessione.idRuoloNewPropr, "255", objTrasm.cessione.idRuoloNewPropr, "P"))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }

                                                    //cancellazione di altri record preesistenti in security (di tipo ACQUISITO) del nuovo ruolo proprietario 
                                                    if (!this.deleteSecurity(IDObjectFolder, objTrasm.cessione.idRuoloNewPropr, "0,255", true, true))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }
                                                    logger.Debug("end cessione diritti aggiuntivi fascicolo");
                                                }
                                            }
                                            // ##################################################
                                            logger.Debug("End cessione diritti di proprietà documento come Ruolo");
                                        }
                                        else
                                        {
                                            // se è doc personale, allora elimino altri record preesistenti in security (di tipo ACQUISITO) del nuovo ruolo proprietario se esistente
                                            if (objTrasm.cessione.idRuoloNewPropr != null && objTrasm.cessione.idRuoloNewPropr != "")
                                            {
                                                if (!this.deleteSecurity(IDObject, objTrasm.cessione.idRuoloNewPropr, "0,255", true, true))
                                                {
                                                    dbProvider.RollbackTransaction();
                                                    throw new Exception();
                                                }
                                            }
                                        }
                                    }
                                    cessioneEffettuata = true;
                                }
                            }
                            else
                            {
                                //CEDE diritti ACQUISITI 
                                //-----------------------------------------------------------------------------------------------------
                                tipoDirittoLog = "acquisiti";

                                logger.DebugFormat("Cessione Diritti; Tipo Diritto: {0}, isPersonOwner: {1}, isGroupOwner: {2}", tipoDirittoLog, isPersonOwner, isGroupOwner);
                                logger.DebugFormat("Valore idPeopleNewPropr: {0}, idRuoloNewPropr: {1}", objTrasm.cessione.idPeopleNewPropr, objTrasm.cessione.idRuoloNewPropr);

                                if ((objTrasm.cessione.idPeople != null &&
                                    objTrasm.cessione.idPeople != "" &&
                                    objTrasm.cessione.idPeople != string.Empty)
                                    &&
                                   (objTrasm.cessione.idRuolo != null &&
                                    objTrasm.cessione.idRuolo != "" &&
                                    objTrasm.cessione.idRuolo != string.Empty))
                                {

                                    bool cedeAcqUtente = false;
                                    bool cedeAcqRuolo = false;

                                    idPersonOrGroup = objTrasm.cessione.idPeople;
                                    // verifica l'esistenza dei record in Security acquisiti come UTENTE
                                    this.selectSecurity(out accessRights, out idGruppoTrasm, out tipoDiritto, IDObject, idPersonOrGroup, "NOT IN (0,255)");
                                    cedeAcqUtente = (!accessRights.Equals("") && !accessRights.Equals("0") && !accessRights.Equals("255"));

                                    logger.DebugFormat("Cessione Diritti Acquisiti come Utente. Valore cedeAcqUtente: {0}", cedeAcqUtente.ToString());

                                    if (cedeAcqUtente)
                                    {
                                        logger.Debug("Cessione Diritti Acquisiti come Utente");
                                        //
                                        // Mev Cessione Diritti, Mantieni diritti di scrittura
                                        //
                                        // I possibili diritti da mantenere sono di Scrittura o Lettura.
                                        // Se non mantengo i diritti di Scrittura, potrei avere i diritti di Lettura
                                        // CASI POSSIBILI:
                                        // 1. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di Scrittura e si possiedono solo quelli
                                        //    la cessione del nuovo utente nn viene eseguita.
                                        // 2. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di lettura e si possiedono solo quelli 
                                        //    la cessione del nuovo utente nn viene eseguita
                                        // 3. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di lettura e si possiedono quelli di Scrittura
                                        //    deve avvenire la cessione del nuovo utente

                                        // 1. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di Scrittura e si possiedono solo quelli
                                        //    la cessione del nuovo utente nn viene eseguita.
                                        if ((objTrasm.mantieniScrittura && !accessRights.Equals("63")) || (!objTrasm.mantieniScrittura && !objTrasm.mantieniLettura))
                                        {
                                            logger.Debug("Inizio caso 1. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di Scrittura ma si possiedono solo quelli la cessione del nuovo utente nn viene eseguita");
                                            logger.DebugFormat("Cessione Diritti Acquisiti come Utente per il Documento/fascicolo. Mantieni Scrittura: {0}, Mantieni Lettura: {1}, AccessRight: {2}", objTrasm.mantieniScrittura.ToString(), objTrasm.mantieniLettura.ToString(), accessRights);

                                            //rimozione dalla security del cedente (Cedo Il Diritto)
                                            if (!this.deleteSecurity(IDObject, idPersonOrGroup, accessRights, false, false))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }

                                            //inserimento nella deleted_security del cedente
                                            if (!this.insertDeletedSecurity(objTrasm.cessione.userId, IDObject, idPersonOrGroup, accessRights, idGruppoTrasm, tipoDiritto, objTrasm.cessione.idPeopleNewPropr, objTrasm.cessione.idRuoloNewPropr))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }

                                            //rimozione della todolist per il ruolo cedente
                                            if (!this.RimuoviToDoList(idPersonOrGroup, IDObject))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }

                                            // ##################################################
                                            // #              FASCICOLO    ROOT                 #
                                            // ##################################################
                                            if (tipoOggettoTrasm.Equals("F"))
                                            {
                                                // prende il fascicolo Root (cha_tipo_proj = 'F')
                                                IDObjectFolder = this.GetIDFolderRoot(IDObject);

                                                logger.DebugFormat("Cessione Diritti Acquisiti come Utente per Fascicolo. IDObjectFolder: {0}, Mantieni Scrittura: {1}, Mantieni Lettura: {2}, AccessRight: {3}", IDObjectFolder, objTrasm.mantieniScrittura.ToString(), objTrasm.mantieniLettura.ToString(), accessRights);

                                                if (IDObjectFolder != string.Empty)
                                                {
                                                    logger.Debug("Inizio Cessione Diritti Acquisiti aggiuntivi Fascicolo");
                                                    //rimozione dalla security del cedente
                                                    if (!this.deleteSecurity(IDObjectFolder, idPersonOrGroup, accessRights, false, false))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }

                                                    //rimozione della todolist per il ruolo cedente
                                                    if (!this.RimuoviToDoList(idPersonOrGroup, IDObjectFolder))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }
                                                    logger.Debug("End Cessione Diritti Acquisiti aggiuntivi Fascicolo");
                                                }
                                            }
                                            // ##################################################

                                            cessioneEffettuata = true;
                                            objTrasm.dirittiCeduti = true;
                                            logger.Debug("End cessione caso 1 Utente");
                                        }

                                        // 2. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di lettura e si possiedono solo quelli 
                                        //    la cessione del nuovo utente nn viene eseguita
                                        
                                        if ((objTrasm.mantieniLettura && !objTrasm.mantieniScrittura && !accessRights.Equals("45") && !accessRights.Equals("63")) || (!objTrasm.mantieniLettura && !objTrasm.mantieniScrittura))
                                        {
                                            logger.Debug("Inizio caso 2. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di Lettura ma si possiedono solo quelli la cessione del nuovo utente nn viene eseguita");
                                            logger.DebugFormat("Cessione Diritti Acquisiti come Utente per il Documento/fascicolo. Mantieni Scrittura: {0}, Mantieni Lettura: {1}, AccessRight: {2}", objTrasm.mantieniScrittura.ToString(), objTrasm.mantieniLettura.ToString(), accessRights);
                                            
                                            //rimozione dalla security del cedente
                                            if (!this.deleteSecurity(IDObject, idPersonOrGroup, accessRights, false, false))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }

                                            //inserimento nella deleted_security del cedente
                                            if (!this.insertDeletedSecurity(objTrasm.cessione.userId, IDObject, idPersonOrGroup, accessRights, idGruppoTrasm, tipoDiritto, objTrasm.cessione.idPeopleNewPropr, objTrasm.cessione.idRuoloNewPropr))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }

                                            //rimozione della todolist per il ruolo cedente
                                            if (!this.RimuoviToDoList(idPersonOrGroup, IDObject))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }


                                            // ##################################################
                                            // #              FASCICOLO    ROOT                 #
                                            // ##################################################
                                            if (tipoOggettoTrasm.Equals("F"))
                                            {
                                                // prende il fascicolo Root (cha_tipo_proj = 'F')
                                                IDObjectFolder = this.GetIDFolderRoot(IDObject);

                                                logger.DebugFormat("Cessione Diritti Acquisiti come Utente per Fascicolo. IDObjectFolder: {0}, Mantieni Scrittura: {1}, Mantieni Lettura: {2}, AccessRight: {3}", IDObjectFolder, objTrasm.mantieniScrittura.ToString(), objTrasm.mantieniLettura.ToString(), accessRights);

                                                if (IDObjectFolder != string.Empty)
                                                {
                                                    logger.Debug("Inizio Cessione Diritti Acquisiti aggiuntivi Fascicolo");
                                                    //rimozione dalla security del cedente
                                                    if (!this.deleteSecurity(IDObjectFolder, idPersonOrGroup, accessRights, false, false))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }

                                                    //rimozione della todolist per il ruolo cedente
                                                    if (!this.RimuoviToDoList(idPersonOrGroup, IDObjectFolder))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }
                                                    logger.Debug("end Cessione Diritti Acquisiti aggiuntivi Fascicolo");
                                                }
                                            }
                                            // ##################################################

                                            cessioneEffettuata = true;
                                            objTrasm.dirittiCeduti = true;
                                            logger.Debug("End cessione caso 2 Utente");
                                        }

                                        // 3. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di lettura e si possiedono quelli di Scrittura
                                        //    deve avvenire la cessione del nuovo utente
                                        if (objTrasm.mantieniLettura && accessRights.Equals("63"))
                                        {
                                            logger.Debug("Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di Lettura e si possiedono quelli di Scrittura deve avvenire la cessione del nuovo utente");
                                            logger.DebugFormat("Cessione Diritti Acquisiti come Utente per il Documento/fascicolo. Mantieni Scrittura: {0}, Mantieni Lettura: {1}, AccessRight: {2}", objTrasm.mantieniScrittura.ToString(), objTrasm.mantieniLettura.ToString(), accessRights);
                                            logger.DebugFormat("Se l'utente a cui cedere il diritto è presente, avviene la cessione. Utente a cui cedere il diritto: {0}", objTrasm.cessione.idPeopleNewPropr);
                                            
                                            if (!string.IsNullOrEmpty(objTrasm.cessione.idPeopleNewPropr))
                                            {
                                                logger.Debug("Inizio caso 3. cessione del diritto acquisito Utente per il documento/fascicolo.");

                                                //rimozione dalla security del cedente
                                                if (!this.deleteSecurity(IDObject, idPersonOrGroup, accessRights, false, false))
                                                {
                                                    dbProvider.RollbackTransaction();
                                                    throw new Exception();
                                                }

                                                //Lascio il permesso di lettura al vecchio proprietario
                                                if (!this.insertSecurity(IDObject, objTrasm.cessione.idPeople, "45", "NULL", "A"))
                                                {
                                                    dbProvider.RollbackTransaction();
                                                    throw new Exception();
                                                }

                                                //inserimento nuovo utente in security
                                                if (!this.insertSecurity(IDObject, objTrasm.cessione.idPeopleNewPropr, accessRights, "NULL", "A"))
                                                {
                                                    dbProvider.RollbackTransaction();
                                                    throw new Exception();
                                                }

                                                //cancellazione di altri record preesistenti in security (di tipo ACQUISITO) del nuovo utente 
                                                if (!this.deleteSecurity(IDObject, objTrasm.cessione.idPeopleNewPropr, accessRights, true, true))
                                                {
                                                    dbProvider.RollbackTransaction();
                                                    throw new Exception();
                                                }
                                            
                                                // ##################################################
                                                // #              FASCICOLO    ROOT                 #
                                                // ##################################################
                                                if (tipoOggettoTrasm.Equals("F"))
                                                {
                                                    // prende il fascicolo Root (cha_tipo_proj = 'F')
                                                    IDObjectFolder = this.GetIDFolderRoot(IDObject);

                                                    logger.DebugFormat("Cessione Diritti Acquisiti come Utente per Fascicolo. IDObjectFolder: {0}, Mantieni Scrittura: {1}, Mantieni Lettura: {2}, AccessRight: {3}", IDObjectFolder, objTrasm.mantieniScrittura.ToString(), objTrasm.mantieniLettura.ToString(), accessRights);

                                                    if (IDObjectFolder != string.Empty)
                                                    {
                                                        logger.Debug("Inizio Cessione Diritti Acquisiti aggiuntivi Fascicolo");
                                                        //rimozione dalla security del cedente
                                                        if (!this.deleteSecurity(IDObjectFolder, idPersonOrGroup, accessRights, false, false))
                                                        {
                                                            dbProvider.RollbackTransaction();
                                                            throw new Exception();
                                                        }

                                                        //rimozione della todolist per il ruolo cedente
                                                        if (!this.RimuoviToDoList(idPersonOrGroup, IDObjectFolder))
                                                        {
                                                            dbProvider.RollbackTransaction();
                                                            throw new Exception();
                                                        }
                                                        logger.Debug("end Cessione Diritti Acquisiti aggiuntivi Fascicolo");
                                                    }
                                                }
                                                // ##################################################

                                                cessioneEffettuata = true;
                                                objTrasm.dirittiCeduti = true;
                                                logger.Debug("End cessione caso 3 Utente");
                                            }
                                        }

                                        // End Mev Cessione Diritti, Mantieni diritti di scrittura
                                        //

                                        //
                                        // Commentato per Mev Cessione Diritti, Mantieni diritti di Scrittura
                                        /*
                                        
                                        //Se attiva l'opzione di mantenere i diritti di lettura e si possiedono solo quelli 
                                        //la cessione nn viene eseguita
                                        if ((objTrasm.mantieniLettura && !accessRights.Equals("45")) || !objTrasm.mantieniLettura)
                                        {
                                            //rimozione dalla security del cedente
                                            if (!this.deleteSecurity(IDObject, idPersonOrGroup, accessRights, false, false))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }

                                            //inserimento nella deleted_security del cedente
                                            if (!this.insertDeletedSecurity(objTrasm.cessione.userId, IDObject, idPersonOrGroup, accessRights, idGruppoTrasm, tipoDiritto, objTrasm.cessione.idPeopleNewPropr, objTrasm.cessione.idRuoloNewPropr))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }

                                            //rimozione della todolist per il ruolo cedente
                                            if (!this.RimuoviToDoList(idPersonOrGroup, IDObject))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }


                                            // ##################################################
                                            // #              FASCICOLO    ROOT                 #
                                            // ##################################################
                                            if (tipoOggettoTrasm.Equals("F"))
                                            {
                                                // prende il fascicolo Root (cha_tipo_proj = 'F')
                                                IDObjectFolder = this.GetIDFolderRoot(IDObject);

                                                if (IDObjectFolder != string.Empty)
                                                {
                                                    //rimozione dalla security del cedente
                                                    if (!this.deleteSecurity(IDObjectFolder, idPersonOrGroup, accessRights, false, false))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }

                                                    ////inserimento nella deleted_security del cedente
                                                    //if (!this.insertDeletedSecurity(objTrasm.cessione.userId, IDObjectFolder, idPersonOrGroup, accessRights, idGruppoTrasm, tipoDiritto, objTrasm.cessione.idPeopleNewPropr, objTrasm.cessione.idRuoloNewPropr))
                                                    //{
                                                    //    dbProvider.RollbackTransaction();
                                                    //    throw new Exception();
                                                    //}

                                                    //rimozione della todolist per il ruolo cedente
                                                    if (!this.RimuoviToDoList(idPersonOrGroup, IDObjectFolder))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }
                                                }
                                            }
                                            // ##################################################


                                            cessioneEffettuata = true;
                                            objTrasm.dirittiCeduti = true;
                                        }
                                        
                                        */
                                        // End Commento Mev Cessione Diritti, Mantieni diritti di Scrittura
                                        //
                                    }

                                    // verifica l'esistenza dei record in Security acquisiti come RUOLO
                                    idPersonOrGroup = objTrasm.cessione.idRuolo;
                                    this.selectSecurity(out accessRights, out idGruppoTrasm, out tipoDiritto, IDObject, idPersonOrGroup, "NOT IN (0,255)");
                                    cedeAcqRuolo = (!accessRights.Equals("") && !accessRights.Equals("0") && !accessRights.Equals("255"));

                                    logger.DebugFormat("Cessione Diritti Acquisiti come Ruolo. Valore cedeAcqRuolo: {0}", cedeAcqRuolo.ToString());

                                    if (cedeAcqRuolo)
                                    {
                                        logger.Debug("Cessione Diritti Acquisiti come Ruolo");
                                        //
                                        // Mev Cessione Diritti, Mantieni diritti di scrittura
                                        //
                                        // I possibili diritti da mantenere sono di Scrittura o Lettura.
                                        // Se non mantengo i diritti di Scrittura, potrei avere i diritti di Lettura
                                        // CASI POSSIBILI:
                                        // 1. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di Scrittura e si possiedono solo quelli
                                        //    la cessione del nuovo ruolo nn viene eseguita.
                                        // 2. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di lettura e si possiedono solo quelli 
                                        //    la cessione del nuovo ruolo nn viene eseguita
                                        // 3. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di lettura e si possiedono quelli di Scrittura
                                        //    deve avvenire la cessione del nuovo ruolo

                                        // 1. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di Scrittura e si possiedono solo quelli
                                        //    la cessione del nuovo ruolo nn viene eseguita.
                                        if ((objTrasm.mantieniScrittura && !accessRights.Equals("63")) || (!objTrasm.mantieniScrittura && !objTrasm.mantieniLettura))
                                        {
                                            logger.Debug("Inizio caso 1. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di Scrittura ma si possiedono solo quelli la cessione del nuovo ruolo nn viene eseguita");
                                            logger.DebugFormat("Cessione Diritti Acquisiti come Ruolo per il Documento/fascicolo. Mantieni Scrittura: {0}, Mantieni Lettura: {1}, AccessRight: {2}", objTrasm.mantieniScrittura.ToString(), objTrasm.mantieniLettura.ToString(), accessRights);

                                            //rimozione dalla security del cedente
                                            if (!this.deleteSecurity(IDObject, idPersonOrGroup, accessRights, false, false))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }

                                            //inserimento nella deleted_security del cedente
                                            if (!this.insertDeletedSecurity(objTrasm.cessione.userId, IDObject, idPersonOrGroup, accessRights, idGruppoTrasm, tipoDiritto, objTrasm.cessione.idPeopleNewPropr, objTrasm.cessione.idRuoloNewPropr))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }

                                            //rimozione della todolist per il ruolo cedente
                                            if (!this.RimuoviToDoList(idPersonOrGroup, IDObject))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }


                                            // ##################################################
                                            // #              FASCICOLO    ROOT                 #
                                            // ##################################################
                                            if (tipoOggettoTrasm.Equals("F"))
                                            {
                                                // prende il fascicolo Root (cha_tipo_proj = 'F')
                                                IDObjectFolder = this.GetIDFolderRoot(IDObject);

                                                logger.DebugFormat("Cessione Diritti Acquisiti come Ruolo per Fascicolo. IDObjectFolder: {0}, Mantieni Scrittura: {1}, Mantieni Lettura: {2}, AccessRight: {3}", IDObjectFolder, objTrasm.mantieniScrittura.ToString(), objTrasm.mantieniLettura.ToString(), accessRights);

                                                if (IDObjectFolder != string.Empty)
                                                {
                                                    logger.Debug("Inizio Cessione Diritti Acquisiti aggiuntivi Fascicolo");
                                                    //rimozione dalla security del cedente
                                                    if (!this.deleteSecurity(IDObjectFolder, idPersonOrGroup, accessRights, false, false))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }

                                                    //rimozione della todolist per il ruolo cedente
                                                    if (!this.RimuoviToDoList(idPersonOrGroup, IDObjectFolder))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }
                                                    logger.Debug("end Cessione Diritti Acquisiti aggiuntivi Fascicolo");
                                                }
                                            }
                                            // ##################################################

                                            cessioneEffettuata = true;
                                            objTrasm.dirittiCeduti = true;
                                            logger.Debug("End cessione caso 1 Ruolo");
                                        }

                                        // 2. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di lettura e si possiedono solo quelli 
                                        //    la cessione del nuovo ruolo nn viene eseguita
                                        if ((objTrasm.mantieniLettura && !objTrasm.mantieniScrittura && !accessRights.Equals("45") && !accessRights.Equals("63")) || (!objTrasm.mantieniLettura && !objTrasm.mantieniScrittura))
                                        {
                                            logger.Debug("Inizio caso 2. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di Lettura ma si possiedono solo quelli la cessione del nuovo ruolo nn viene eseguita");
                                            logger.DebugFormat("Cessione Diritti Acquisiti come Ruolo per il documento/fascicolo. Mantieni Scrittura: {0}, Mantieni Lettura: {1}, AccessRight: {2}", objTrasm.mantieniScrittura.ToString(), objTrasm.mantieniLettura.ToString(), accessRights);

                                            //rimozione dalla security del cedente
                                            if (!this.deleteSecurity(IDObject, idPersonOrGroup, accessRights, false, false))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }

                                            //inserimento nella deleted_security del cedente
                                            if (!this.insertDeletedSecurity(objTrasm.cessione.userId, IDObject, idPersonOrGroup, accessRights, idGruppoTrasm, tipoDiritto, objTrasm.cessione.idPeopleNewPropr, objTrasm.cessione.idRuoloNewPropr))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }

                                            //rimozione della todolist per il ruolo cedente
                                            if (!this.RimuoviToDoList(idPersonOrGroup, IDObject))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }


                                            // ##################################################
                                            // #              FASCICOLO    ROOT                 #
                                            // ##################################################
                                            if (tipoOggettoTrasm.Equals("F"))
                                            {
                                                // prende il fascicolo Root (cha_tipo_proj = 'F')
                                                IDObjectFolder = this.GetIDFolderRoot(IDObject);

                                                logger.DebugFormat("Cessione Diritti Acquisiti come Ruolo per Fascicolo. IDObjectFolder: {0}, Mantieni Scrittura: {1}, Mantieni Lettura: {2}, AccessRight: {3}", IDObjectFolder, objTrasm.mantieniScrittura.ToString(), objTrasm.mantieniLettura.ToString(), accessRights);

                                                if (IDObjectFolder != string.Empty)
                                                {
                                                    logger.Debug("Inizio Cessione Diritti Acquisiti aggiuntivi Fascicolo");
                                                    //rimozione dalla security del cedente
                                                    if (!this.deleteSecurity(IDObjectFolder, idPersonOrGroup, accessRights, false, false))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }

                                                    //rimozione della todolist per il ruolo cedente
                                                    if (!this.RimuoviToDoList(idPersonOrGroup, IDObjectFolder))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }
                                                    logger.Debug("end Cessione Diritti Acquisiti aggiuntivi Fascicolo");
                                                }
                                            }
                                            // ##################################################

                                            cessioneEffettuata = true;
                                            objTrasm.dirittiCeduti = true;
                                            logger.Debug("End cessione caso 2 Ruolo");
                                        }

                                        // 3. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di lettura e si possiedono quelli di Scrittura
                                        //    deve avvenire la cessione del nuovo ruolo
                                        if (objTrasm.mantieniLettura && !objTrasm.mantieniScrittura && accessRights.Equals("63"))
                                        {
                                            logger.Debug("Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di Lettura e si possiedono quelli di Scrittura deve avvenire la cessione del nuovo ruolo");
                                            logger.DebugFormat("Cessione Diritti Acquisiti come Ruolo per il Documento/fascicolo. Mantieni Scrittura: {0}, Mantieni Lettura: {1}, AccessRight: {2}", objTrasm.mantieniScrittura.ToString(), objTrasm.mantieniLettura.ToString(), accessRights);
                                            logger.DebugFormat("Se l'utente a cui cedere il diritto è presente, avviene la cessione. Utente a cui cedere il diritto: {0}", objTrasm.cessione.idPeopleNewPropr);

                                            if (!string.IsNullOrEmpty(objTrasm.cessione.idRuoloNewPropr))
                                            {
                                                logger.Debug("Inizio caso 3. cessione del diritto acquisito Ruolo per il documento/fascicolo.");

                                                //rimozione dalla security del cedente
                                                if (!this.deleteSecurity(IDObject, idPersonOrGroup, accessRights, false, false))
                                                {
                                                    dbProvider.RollbackTransaction();
                                                    throw new Exception();
                                                }

                                                //Lascio il permesso di lettura al vecchio ruolo
                                                if (!this.insertSecurity(IDObject, objTrasm.cessione.idRuolo, "45", "NULL", "A"))
                                                {
                                                    dbProvider.RollbackTransaction();
                                                    throw new Exception();
                                                }


                                                string accessRightsDest = string.Empty;
                                                this.selectSecurity(out accessRightsDest, out idGruppoTrasm, out tipoDiritto, IDObject, objTrasm.cessione.idRuoloNewPropr, "");
                                                isGroupOwner = (!accessRights.Equals("") && accessRights.Equals("255"));
                                                if (Int32.Parse(accessRights) > Int32.Parse(accessRightsDest))
                                                {
                                                    //inserimento nuovo ruolo in security
                                                    if (!this.insertSecurity(IDObject, objTrasm.cessione.idRuoloNewPropr, accessRights, "NULL", "A"))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }

                                                    //cancellazione di altri record preesistenti in security (di tipo ACQUISITO) del nuovo ruolo 
                                                    if (!this.deleteSecurity(IDObject, objTrasm.cessione.idRuoloNewPropr, accessRights, true, true))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }
                                                }
                                            
                                                // ##################################################
                                                // #              FASCICOLO    ROOT                 #
                                                // ##################################################
                                                if (tipoOggettoTrasm.Equals("F"))
                                                {
                                                    // prende il fascicolo Root (cha_tipo_proj = 'F')
                                                    IDObjectFolder = this.GetIDFolderRoot(IDObject);

                                                    logger.DebugFormat("Cessione Diritti Acquisiti come Ruolo per Fascicolo. IDObjectFolder: {0}, Mantieni Scrittura: {1}, Mantieni Lettura: {2}, AccessRight: {3}", IDObjectFolder, objTrasm.mantieniScrittura.ToString(), objTrasm.mantieniLettura.ToString(), accessRights);

                                                    if (IDObjectFolder != string.Empty)
                                                    {
                                                        logger.Debug("Inizio Cessione Diritti Acquisiti aggiuntivi Fascicolo");
                                                        //rimozione dalla security del cedente
                                                        if (!this.deleteSecurity(IDObjectFolder, idPersonOrGroup, accessRights, false, false))
                                                        {
                                                            dbProvider.RollbackTransaction();
                                                            throw new Exception();
                                                        }

                                                        //rimozione della todolist per il ruolo cedente
                                                        if (!this.RimuoviToDoList(idPersonOrGroup, IDObjectFolder))
                                                        {
                                                            dbProvider.RollbackTransaction();
                                                            throw new Exception();
                                                        }
                                                        logger.Debug("End Cessione Diritti Acquisiti aggiuntivi Fascicolo");
                                                    }
                                                }
                                                // ##################################################

                                                cessioneEffettuata = true;
                                                objTrasm.dirittiCeduti = true;
                                                logger.Debug("End cessione caso 3 Ruolo");
                                            }
                                        }

                                        // End Mev Cessione Diritti, Mantieni diritti di scrittura
                                        //

                                        //
                                        // Commentato per Mev Cessione Diritti, Mantieni diritti di Scrittura
                                        /*

                                        //Se attiva l'opzione di mantenere i diritti di lettura e si possiedono solo quelli 
                                        //la trasmissione nn viene eseguita
                                        if ((objTrasm.mantieniLettura && !accessRights.Equals("45")) || !objTrasm.mantieniLettura)
                                        {
                                            //rimozione dalla security del cedente
                                            if (!this.deleteSecurity(IDObject, idPersonOrGroup, accessRights, false, false))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }

                                            //inserimento nella deleted_security del cedente
                                            if (!this.insertDeletedSecurity(objTrasm.cessione.userId, IDObject, idPersonOrGroup, accessRights, idGruppoTrasm, tipoDiritto, objTrasm.cessione.idPeopleNewPropr, objTrasm.cessione.idRuoloNewPropr))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }

                                            //rimozione della todolist per il ruolo cedente
                                            if (!this.RimuoviToDoList(idPersonOrGroup, IDObject))
                                            {
                                                dbProvider.RollbackTransaction();
                                                throw new Exception();
                                            }


                                            // ##################################################
                                            // #              FASCICOLO    ROOT                 #
                                            // ##################################################
                                            if (tipoOggettoTrasm.Equals("F"))
                                            {
                                                // prende il fascicolo Root (cha_tipo_proj = 'F')
                                                IDObjectFolder = this.GetIDFolderRoot(IDObject);

                                                if (IDObjectFolder != string.Empty)
                                                {
                                                    //rimozione dalla security del cedente
                                                    if (!this.deleteSecurity(IDObjectFolder, idPersonOrGroup, accessRights, false, false))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }

                                                    ////inserimento nella deleted_security del cedente
                                                    //if (!this.insertDeletedSecurity(objTrasm.cessione.userId, IDObjectFolder, idPersonOrGroup, accessRights, idGruppoTrasm, tipoDiritto, objTrasm.cessione.idPeopleNewPropr, objTrasm.cessione.idRuoloNewPropr))
                                                    //{
                                                    //    dbProvider.RollbackTransaction();
                                                    //    throw new Exception();
                                                    //}

                                                    //rimozione della todolist per il ruolo cedente
                                                    if (!this.RimuoviToDoList(idPersonOrGroup, IDObjectFolder))
                                                    {
                                                        dbProvider.RollbackTransaction();
                                                        throw new Exception();
                                                    }
                                                }
                                            }
                                            // ##################################################

                                            cessioneEffettuata = true;
                                            objTrasm.dirittiCeduti = true;
                                        }

                                        */
                                        // End Commento Mev Cessione Diritti, Mantieni diritti di Scrittura
                                        //
                                    }
                                }
                            }
                        }

                        if (cessioneEffettuata)
                        {
                             // gestione LOG /STORICO
                            DocsPaVO.Logger.CodAzione.infoOggetto InfoOggetto;
                            InfoOggetto = Log.getInfoOggetto("EXECSENDERRIGTHS_" + tipoOggettoTrasm, objTrasm.utente.idAmministrazione);
                            if (InfoOggetto.Attivo == 1)
                            {
                                Log.InsertLog(objTrasm.cessione.userId,
                                                            objTrasm.cessione.idPeople,
                                                            objTrasm.cessione.idRuolo,
                                                            objTrasm.utente.idAmministrazione,
                                                            InfoOggetto.Oggetto.Trim(),
                                                            IDObject,
                                                            "Cede diritti " + tipoDirittoLog,
                                                            InfoOggetto.Codice.Trim(),
                                                            InfoOggetto.Descrizione.Trim(),
                                                            DocsPaVO.Logger.CodAzione.Esito.OK, null, string.Empty);
                            }

                            //***************************************************************************************
                            //Modifica Iacozzilli Giordano 30/07/2012
                            //Modifica per il LOG della cessione dei diritti da parte 
                            //di un utente che non è l'Owner del doc.
                            //LOG APPLICATIVO
                            DocsPaVO.Logger.CodAzione.infoOggetto InfoOggettoCessioneNoOwner;
                            InfoOggettoCessioneNoOwner = Log.getInfoOggetto("EXECSENDERRIGTHSNOOWNER", objTrasm.utente.idAmministrazione);
                            if (InfoOggettoCessioneNoOwner.Attivo == 1)
                            {
                                Log.InsertLog(objTrasm.cessione.userId,
                                                            objTrasm.cessione.idPeople,
                                                            objTrasm.cessione.idRuolo,
                                                            objTrasm.utente.idAmministrazione,
                                                            InfoOggettoCessioneNoOwner.Oggetto.Trim(),
                                                            IDObject,
                                                            "Cessione diritti :" + "\\n"
                                                                                        + "Documento Num Prot -- : " + objTrasm.infoDocumento.numProt
                                                                                        + "\\n" + "Proprietario del documento -- IDPEOPLE : " + objTrasm.cessione.idPeople
                                                                                        + "\\n" + "Utente Cedente -- IDPEOPLE : " + objTrasm.utente.idPeople
                                                                                        + "\\n" + "Nuovo Proprietario -- IDPEOPLE : " + objTrasm.cessione.idPeopleNewPropr,
                                                            InfoOggettoCessioneNoOwner.Codice.Trim(),
                                                            InfoOggettoCessioneNoOwner.Descrizione.Trim(),
                                                            DocsPaVO.Logger.CodAzione.Esito.OK, null, string.Empty);
                            }
                            //FINE
                            //***********************************************************************************************************************
                        }
                    }
                    catch
                    {
                        logger.Debug("Errore nella procedura di cessione dei diritti!!");
                    }
                }
            }
            return cessioneEffettuata;
            logger.Debug("Fine procedura di cessione dei diritti");

        }

        /// <summary>
        /// Permette la query SELECT sulla tabella Security
        /// </summary>
        /// <param name="accessRights"></param>
        /// <param name="idGruppoTrasm"></param>
        /// <param name="tipoDiritto"></param>
        /// <param name="thing"></param>
        /// <param name="personOrgroup"></param>
        /// <param name="accessRightsToTest"></param>
        /// <returns></returns>
        public bool selectSecurity(out string accessRights, out string idGruppoTrasm, out string tipoDiritto, string thing, string personOrgroup, string accessRightsToTest)
        {
            accessRights = "";
            idGruppoTrasm = "";
            tipoDiritto = "";

            string queryAccessRightsToTest = string.Empty;
            IDataReader dr = null;
            bool retValue = false;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SECURITY_GENERIC");

                    if (accessRightsToTest != null && accessRightsToTest != string.Empty)
                        queryAccessRightsToTest = " AND ACCESSRIGHTS " + accessRightsToTest;

                    q.setParam("param1", "ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO");
                    q.setParam("param2", "WHERE THING = " + thing + " AND PERSONORGROUP = " + personOrgroup + queryAccessRightsToTest);
                    string queryString = q.getSQL();
                    logger.Debug(queryString);

                    using (dr = dbProvider.ExecuteReader(queryString))
                    {
                        if (dr == null)
                        {
                            return false;
                        }
                        if (dr != null && dr.FieldCount > 0)
                        {
                            while (dr.Read())
                            {
                                accessRights = dr.GetValue(0).ToString();
                                idGruppoTrasm = dr.GetValue(1).ToString();
                                tipoDiritto = dr.GetValue(2).ToString();
                            }
                            retValue = true;
                        }
                    }
                }
                if (dr != null && (!dr.IsClosed))
                    dr.Close();
            }
            catch
            {
                return false;
            }
            return retValue;
        }

        /// <summary>
        /// Permette l'inserimento di un record sulla tabella Security
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="personOrGroup"></param>
        /// <param name="accessRights"></param>
        /// <param name="idGruppoTrasm"></param>
        /// <param name="tipoDiritto"></param>
        /// <returns></returns>
        private bool insertSecurity(string thing, string personOrGroup, string accessRights, string idGruppoTrasm, string tipoDiritto)
        {
            bool retValue = false;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_Security");
                    q.setParam("param1", thing + "," + personOrGroup + "," + accessRights + "," + idGruppoTrasm + ",'" + tipoDiritto + "', NULL");
                    string queryString = q.getSQL();
                    logger.Debug(queryString);
                    retValue = dbProvider.ExecuteNonQuery(queryString);
                }
            }
            catch
            {
                return false;
            }

            return retValue;
        }

        /// <summary>
        /// Permette la cancellazione di record sulla tabella Security
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="idPersonOrGroup"></param>
        /// <param name="accessRights"></param>
        /// <param name="isAccessRightsIN"></param>
        /// <param name="NotIN"></param>
        /// <returns></returns>
        private bool deleteSecurity(string thing, string idPersonOrGroup, string accessRights, bool isAccessRightsIN, bool NotIN)
        {
            bool retValue = false;
            string queryAccessrights = string.Empty;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_SECURITY");

                    queryAccessrights = (isAccessRightsIN ? (NotIN ? "NOT " : "") + "IN (" + accessRights + ")" : "= " + accessRights);

                    q.setParam("param1", "THING = " + thing + " AND PERSONORGROUP = " + idPersonOrGroup + " AND ACCESSRIGHTS " + queryAccessrights);
                    string queryString = q.getSQL();
                    logger.Debug(queryString);
                    retValue = dbProvider.ExecuteNonQuery(queryString);
                }
            }
            catch
            {
                return false;
            }

            return retValue;
        }

        /// <summary>
        /// Permette l'inserimento di un record sulla tabella Deleted_Security
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="thing"></param>
        /// <param name="idPersonOrGroup"></param>
        /// <param name="accessRights"></param>
        /// <param name="idGruppoTrasm"></param>
        /// <param name="tipoDiritto"></param>
        /// <param name="idPerson"></param>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        private bool insertDeletedSecurity(string userID, string thing, string idPersonOrGroup, string accessRights, string idGruppoTrasm, string tipoDiritto, string idPerson, string idRuolo)
        {
            bool retValue = false;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    // prima di inserire il record, vengono eliminati eventuali record esistenti
                    // per non incorrere nell'errore di CONSTRAINTS violato su [THING, PERSONORGROUP, ACCESSRIGHTS]
                    string queryString = "DELETE FROM DELETED_SECURITY WHERE THING = " + thing + " AND PERSONORGROUP = " + idPersonOrGroup + " AND ACCESSRIGHTS = " + accessRights;
                    logger.Debug(queryString);
                    if (dbProvider.ExecuteNonQuery(queryString))
                    {
                        // quindi inserisce il record...
                        DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DELETED_SECURITY");
                        string note = "Diritto ceduto da: " + userID;
                        q.setParam("param1", thing + "," + idPersonOrGroup + "," + accessRights + "," + idGruppoTrasm + ",'" + tipoDiritto + "','" + note + "'," + DocsPaDbManagement.Functions.Functions.GetDate() + ",'" + idPerson + "','" + idRuolo + "', NULL");
                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        retValue = dbProvider.ExecuteNonQuery(queryString);
                    }
                }
            }
            catch
            {
                return false;
            }

            return retValue;
        }

        private DocsPaVO.documento.TipoDiritto getTipoDiritto(string tipoDiritto)
        {
            DocsPaVO.documento.TipoDiritto diritto = new DocsPaVO.documento.TipoDiritto();
            if (tipoDiritto.Equals("P"))
            {
                diritto = DocsPaVO.documento.TipoDiritto.TIPO_PROPRIETARIO;

            }
            if (tipoDiritto.Equals("T"))
            {
                diritto = DocsPaVO.documento.TipoDiritto.TIPO_TRASMISSIONE;
            }
            if (tipoDiritto.Equals("F"))
            {
                diritto = DocsPaVO.documento.TipoDiritto.TIPO_TRASMISSIONE_IN_FASCICOLO;
            }
            if (tipoDiritto.Equals("S"))
            {
                diritto = DocsPaVO.documento.TipoDiritto.TIPO_SOSPESO;
            }
            if (tipoDiritto.Equals("A"))
            {
                diritto = DocsPaVO.documento.TipoDiritto.TIPO_ACQUISITO;
            }
            if (tipoDiritto.Equals("D"))
            {
                diritto = DocsPaVO.documento.TipoDiritto.TIPO_DELEGATO;
            }
            return diritto;
        }

        /// <summary>
        /// Ricava la lista delle query in base al tipo destinatario
        /// </summary>
        /// <param name="currentTrasmissione"></param>
        /// <param name="objTrasm"></param>
        /// <param name="idRegistro"></param>
        /// <param name="idNodoTitolario"></param>
        /// <param name="idObj"></param>
        /// <param name="idPeopleGroup"></param>
        /// <param name="codiceACL"></param>
        /// <param name="tipoDiritto"></param>
        /// <returns></returns>
        public ArrayList GetQueryTrasm(DocsPaVO.trasmissione.TrasmissioneSingola currentTrasmissione, DocsPaVO.trasmissione.Trasmissione objTrasm, string idRegistro, string idNodoTitolario, string idObj, string idPeopleGroup, string codiceACL, string tipoDiritto)
        {
            System.Data.DataSet dataSet = new System.Data.DataSet();
            ArrayList returnList = new ArrayList();
            string checkString;
            ArrayList insertQueries = new ArrayList();
            ArrayList listaInfoSec = new ArrayList();

            //CASO 1: DESTINATARIO RUOLO
            if (currentTrasmissione.tipoDest == DocsPaVO.trasmissione.TipoDestinatario.RUOLO)
            {
                checkString = "SELECT PERSONORGROUP, ACCESSRIGHTS, CHA_TIPO_DIRITTO FROM SECURITY WHERE THING = " + idObj + " AND PERSONORGROUP = " + idPeopleGroup;
                logger.Debug(checkString);
                this.ExecuteQuery(dataSet, "DIRITTI", checkString);

                DataRow[] dirittiRuolo = dataSet.Tables["DIRITTI"].Select("PERSONORGROUP=" + idPeopleGroup);
                if (dirittiRuolo.Length == 0)
                {
                    DocsPaVO.trasmissione.infoSecurity infoSec = new DocsPaVO.trasmissione.infoSecurity();

                    infoSec.thing = idObj;
                    infoSec.accessRights = codiceACL;
                    infoSec.chaTipoDiritto = tipoDiritto;
                    infoSec.personOrGroup = idPeopleGroup;
                    infoSec.idGruppoTrasm = objTrasm.ruolo.idGruppo;
                    infoSec.hideDocPreviousVersions = currentTrasmissione.hideDocumentPreviousVersions;
                    infoSec.tipoQuery = "I";    //insert

                    listaInfoSec.Add(infoSec);
                }
                else
                {
                    if (Int32.Parse(dirittiRuolo[0]["ACCESSRIGHTS"].ToString()) < Int32.Parse(codiceACL))
                    {
                        DocsPaVO.trasmissione.infoSecurity infoSec = new DocsPaVO.trasmissione.infoSecurity();

                        infoSec.thing = idObj;
                        infoSec.accessRights = codiceACL;
                        infoSec.chaTipoDiritto = choseRight(dirittiRuolo[0]["CHA_TIPO_DIRITTO"].ToString(), tipoDiritto);
                        infoSec.personOrGroup = idPeopleGroup;
                        infoSec.idGruppoTrasm = objTrasm.ruolo.idGruppo;
                        infoSec.hideDocPreviousVersions = currentTrasmissione.hideDocumentPreviousVersions;
                        infoSec.tipoQuery = "U";	//update

                        listaInfoSec.Add(infoSec);
                    }
                    else
                        logger.Debug("nessuna modifica");
                }
            }

            //CASO 2: DESTINATARIO UTENTE
            if (currentTrasmissione.tipoDest == DocsPaVO.trasmissione.TipoDestinatario.UTENTE)
            {
                //si controlla se l'utente ha diritti sull'oggetto
                checkString = "SELECT * FROM SECURITY WHERE THING='" + idObj + "' AND PERSONORGROUP=" + ((DocsPaVO.trasmissione.TrasmissioneUtente)currentTrasmissione.trasmissioneUtente[0]).utente.idPeople;

                this.ExecuteQuery(dataSet, "DIRITTI", checkString);

                if (dataSet.Tables["DIRITTI"].Rows.Count == 0)
                {
                    DocsPaVO.trasmissione.infoSecurity infoSec = new DocsPaVO.trasmissione.infoSecurity();

                    infoSec.thing = idObj;
                    infoSec.accessRights = codiceACL;
                    infoSec.chaTipoDiritto = tipoDiritto;
                    infoSec.personOrGroup = ((DocsPaVO.trasmissione.TrasmissioneUtente)currentTrasmissione.trasmissioneUtente[0]).utente.idPeople;
                    infoSec.idGruppoTrasm = objTrasm.ruolo.idGruppo;
                    infoSec.hideDocPreviousVersions = currentTrasmissione.hideDocumentPreviousVersions;
                    infoSec.tipoQuery = "I";
                    listaInfoSec.Add(infoSec);
                }
                else
                {
                    int oldAccessRight = Int32.Parse(dataSet.Tables["DIRITTI"].Rows[0]["ACCESSRIGHTS"].ToString());
                    //if (oldAccessRight < Int32.Parse(codiceACL) && oldAccessRight != 0)
                    if (oldAccessRight < Int32.Parse(codiceACL))
                    {
                        DocsPaVO.trasmissione.infoSecurity infoSec = new DocsPaVO.trasmissione.infoSecurity();

                        infoSec.thing = idObj;
                        infoSec.accessRights = codiceACL;
                        infoSec.chaTipoDiritto = choseRight(dataSet.Tables["DIRITTI"].Rows[0]["CHA_TIPO_DIRITTO"].ToString(), tipoDiritto);
                        infoSec.personOrGroup = ((DocsPaVO.trasmissione.TrasmissioneUtente)currentTrasmissione.trasmissioneUtente[0]).utente.idPeople;
                        infoSec.idGruppoTrasm = objTrasm.ruolo.idGruppo;
                        infoSec.hideDocPreviousVersions = currentTrasmissione.hideDocumentPreviousVersions;
                        if (oldAccessRight != 0)
                        {
                            infoSec.tipoQuery = "U";
                        }
                        else
                        {
                            infoSec.tipoQuery = "I";
                            infoSec.chaTipoDiritto = "T";
                        }
                        listaInfoSec.Add(infoSec);
                    }
                }
            }

            return listaInfoSec;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldRight"></param>
        /// <param name="newRight"></param>
        /// <returns></returns>
        private string choseRight(string oldRight, string newRight)
        {
            System.Collections.ArrayList rights = new System.Collections.ArrayList();
            rights.Add("F".ToString());
            rights.Add("A".ToString());
            rights.Add("T".ToString());
            rights.Add("P".ToString());
            string res;
            if (rights.IndexOf(oldRight) <= rights.IndexOf(newRight))
            {
                res = newRight;
            }
            else
            {
                res = oldRight;
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateTrasmUtente"></param>
        /// <param name="updateTrasmUtente2"></param>
        public void execACCRifMeth(string updateTrasmUtente)
        {
            bool result = false;
            logger.Info("BEGIN");
            int rowsAff;
            try
            {
                this.BeginTransaction();

                //this.ExecuteNonQuery(updateTrasmUtente);
                result = this.ExecuteNonQuery(updateTrasmUtente, out rowsAff);

                logger.Debug("RowsAffected=" + Convert.ToString(rowsAff));

                this.CommitTransaction();
            }
            catch
            {
                this.RollbackTransaction();

                throw new Exception();
            }
            logger.Info("END");
        }

        #endregion

        #region Query di QueryTrasmManager.cs
        /*
		public void getQueryTrasmissioni(DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso)
		{
		
			/*DocsPaUtils.Query q = 
				DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TRASMISSIONE_DPA_TRASM_SINGOLA_DPA_TRASM_UTENTE_ORDER");			

			q.setParam("param1",DocsPaWS.Utils.dbControl.toChar("A.DTA_INVIO",false)) ;
			q.setParam("param2",DocsPaWS.Utils.dbControl.toChar("B.DTA_SCADENZA",false)) ;
			q.setParam("param3",DocsPaWS.Utils.dbControl.toChar("C.DTA_VISTA",false)) ;
			q.setParam("param4",DocsPaWS.Utils.dbControl.toChar("C.DTA_ACCETTATA",false));
			q.setParam("param5",DocsPaWS.Utils.dbControl.toChar("C.DTA_RIFIUTATA",false));


			if (objOggettoTrasmesso == null)
				return;
			string whereStr = null;
			bool doc = false;

			// condizione sui documenti
			if (objOggettoTrasmesso.infoDocumento != null) 
			{
				whereStr = "A.ID_PROFILE=" + objOggettoTrasmesso.infoDocumento.idProfile;
				doc = true;
			}

			//condizione sui fascicoli
			if (objOggettoTrasmesso.infoFascicolo != null) 
			{
				if (doc) whereStr += " OR ";
				whereStr += "A.ID_PROJECT=" + objOggettoTrasmesso.infoFascicolo.idFascicolo;
			}

			if(whereStr != null) 
			{
				string query_Where = " AND ";
				if (doc) query_Where += "(";
				query_Where += whereStr;
				if (doc) query_Where += ") ";

				q.setParam("param6",query_Where);
			}


			return q.getSQL();		
		}
		*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryWhere"></param>
        /// <param name="objOggettoTrasmesso"></param>
        public DocsPaUtils.Query QueryTrasm(ref string queryWhere, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASMISSIONE__TRASM_SINGOLA__TRASM_UTENTE");

            string UserDB = string.Empty;

            if (dbType.ToUpper() == "SQL")
            {
                UserDB = getUserDB();
                q.setParam("dbuser", UserDB);
            }

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO", false));
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("B.DTA_SCADENZA", false));
            q.setParam("param3", DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_VISTA", false));
            q.setParam("param4", DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_ACCETTATA", false));
            q.setParam("param5", DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_RIFIUTATA", false));

            if (objOggettoTrasmesso == null)
            {
                return q;
            }

            string whereStr = null;
            bool doc = false;

            // condizione sui documenti
            if (objOggettoTrasmesso.infoDocumento != null)
            {
                whereStr = "A.ID_PROFILE=" + objOggettoTrasmesso.infoDocumento.idProfile;
                doc = true;
            }

            //condizione sui fascicoli
            if (objOggettoTrasmesso.infoFascicolo != null)
            {
                if (doc) whereStr += " OR ";
                whereStr += "A.ID_PROJECT=" + objOggettoTrasmesso.infoFascicolo.idFascicolo;
            }
            if (whereStr != null)
            {
                queryWhere += " AND ";
                if (doc)
                {
                    queryWhere += "(";
                }
                queryWhere += whereStr;
                if (doc)
                {
                    queryWhere += ") ";
                }
            }
            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public string getQueryEffMet1(string idPeople)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlob11");

            q.setParam("param1", idPeople);

            string sql = q.getSQL();

            string idCorrispondente;
            this.ExecuteScalar(out idCorrispondente, sql);
            return idCorrispondente;
        }

        //		public void GetQueryTrasmEffettuate(out DataSet dataSet, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.utente.Utente objUtente, string idCorrispondente,ref bool repeatQuery)
        //		{
        //			string queryWhere = "";
        //			string queryJoin = "";
        //			//string ruoliInf = getListaRuoliInf(objOggettoTrasmesso, objRuolo, cercaInf(objListaFiltri));
        //
        //			//DocsPaUtils.Query q = QueryTrasm(ref queryWhere, objOggettoTrasmesso);
        //			/*----> QueryTrasm <-------*/
        //			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASMISSIONE__TRASM_SINGOLA__TRASM_UTENTE");
        //			q.setParam("param1",DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO",false));
        //			q.setParam("param2",DocsPaDbManagement.Functions.Functions.ToChar("B.DTA_SCADENZA",false));
        //			q.setParam("param3",DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_VISTA",false));
        //			q.setParam("param4",DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_ACCETTATA",false));
        //			q.setParam("param5",DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_RIFIUTATA",false));
        //
        //			//vedere
        //			/*if (objOggettoTrasmesso == null)
        //			{
        //				return q;
        //			}*/
        //			//nella versione 2 c'è questo controllo invece di quello sopra	
        //			//if (objOggettoTrasmesso == null ||(objOggettoTrasmesso.infoDocumento == null && objOggettoTrasmesso.infoFascicolo == null))
        //			//	return;
        //		
        //			//string whereStr = null;		
        //			//bool doc = false;
        //
        //			// condizione sui documenti
        //			/*if (objOggettoTrasmesso.infoDocumento != null) 
        //			{
        //				whereStr = "A.ID_PROFILE=" + objOggettoTrasmesso.infoDocumento.idProfile;
        //				doc = true;
        //			}*/
        //
        //			// condizione sui documenti da docspa20
        //			string tipo = "";
        //			if (objOggettoTrasmesso.infoDocumento != null) 
        //			{
        //				queryWhere += " AND (";
        //				queryWhere += "A.ID_PROFILE = " + objOggettoTrasmesso.infoDocumento.idProfile + " AND ";
        //				queryWhere += "A.CHA_TIPO_OGGETTO = 'D' )";				
        //				tipo = "OR";
        //			}
        //
        //			//condizione sui fascicoli
        //			/*if (objOggettoTrasmesso.infoFascicolo != null) 
        //			{
        //				if (doc) whereStr += " OR ";
        //				whereStr += "A.ID_PROJECT=" + objOggettoTrasmesso.infoFascicolo.idFascicolo;
        //			}
        //			if(whereStr != null) 
        //			{			
        //				queryWhere += " AND ";
        //				if (doc) 
        //				{
        //					queryWhere += "(";
        //				}
        //				queryWhere += whereStr;
        //				if (doc) 
        //				{				
        //					queryWhere += ") ";
        //				}
        //			}
        //			return q;*/
        //			//condizione sui fascicoli da docspa20
        //			if (objOggettoTrasmesso.infoFascicolo != null) 
        //			{
        //				queryWhere += tipo + " (";
        //				queryWhere += "A.ID_DOC_PROG = " + objOggettoTrasmesso.infoFascicolo.idFascicolo;
        //				queryWhere += "AND A.CHA_TIPO_OGGETTO = 'P' )";				
        //			}
        //			/*fine QueryTrasm */
        //		
        //			repeatQuery = getCondFiltri(ref queryWhere, ref queryJoin, objListaFiltri);
        //			bool inf = cercaInf(objListaFiltri);
        //			//queryWhere += " AND ((";
        //
        //			//ricevute
        //			/*queryWhere +=
        //				" C.CHA_VALIDA='1' AND A.DTA_INVIO IS NOT NULL) AND " +
        //				" (A.SYSTEM_ID IN (SELECT B.ID_TRASMISSIONE FROM DPA_TRASM_SINGOLA B, DPA_TRASM_UTENTE C WHERE B.SYSTEM_ID=C.ID_TRASM_SINGOLA AND (B.ID_CORR_GLOBALE = " + objRuolo.systemId + " AND C.ID_PEOPLE=" + objUtente.idPeople + ") OR B.ID_CORR_GLOBALE=" + idCorrispondente;*/
        //
        //			queryWhere += " AND C.CHA_VALIDA='1' AND A.DTA_INVIO IS NOT NULL AND (";
        //				
        //			//2004-02-13 mdigregorio:modificato per errore su tipoRuolo di ultimo liv 
        //			/*if (inf && !ruoliInf.Equals("()"))
        //			{
        //				queryWhere += " OR B.ID_CORR_GLOBALE IN " + ruoliInf;				
        //			}
        //			queryWhere += "))) OR (";*/
        //			//effettuate
        //			getCondVisibilitaEffettuate(ref queryWhere, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);
        //			queryWhere += ")";
        //					
        //			/*q.setParam("param1000",queryJoin);
        //			q.setParam("param2000",queryWhere);*/
        //			q.setParam("param6", queryWhere);
        //			string queryString = q.getSQL();
        //
        //			logger.Debug(queryString);
        //			this.ExecuteQuery(out dataSet,"TRASMISSIONI",queryString);
        //		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="objListaFiltri"></param>
        /// <param name="ruoliInf"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objUtente"></param>
        /// <param name="idCorrispondente"></param>
        /// <param name="repeatQuery"></param>
        public void getQueryTrasmEff(out DataSet dataSet, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.utente.Utente objUtente, string idCorrispondente, ref bool repeatQuery)
        {
            string queryWhere = "";
            string queryJoin = "";

            string ruoliInf = getListaRuoliInf(objOggettoTrasmesso, objRuolo, cercaInf(objListaFiltri));

            DocsPaUtils.Query q = QueryTrasm(ref queryWhere, objOggettoTrasmesso);
            repeatQuery = getCondFiltri(ref queryWhere, ref queryJoin, objListaFiltri);
            bool inf = cercaInf(objListaFiltri);
            queryWhere += " AND ((";

            //ricevute
            queryWhere +=
                " C.CHA_VALIDA='1' AND A.DTA_INVIO IS NOT NULL) AND " +
                " (A.SYSTEM_ID IN (SELECT B.ID_TRASMISSIONE FROM DPA_TRASM_SINGOLA B, DPA_TRASM_UTENTE C WHERE B.SYSTEM_ID=C.ID_TRASM_SINGOLA AND (B.ID_CORR_GLOBALE = " + objRuolo.systemId + " AND C.ID_PEOPLE=" + objUtente.idPeople + ") OR B.ID_CORR_GLOBALE=" + idCorrispondente;

            //2004-02-13 mdigregorio:modificato per errore su tipoRuolo di ultimo liv 
            if (inf && !ruoliInf.Equals("()"))
            {
                queryWhere += " OR B.ID_CORR_GLOBALE IN " + ruoliInf;
            }
            queryWhere += "))) OR (";
            //effettuate
            getCondizioniVisibilitaEffettuate(ref queryWhere, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);
            queryWhere += ")";

            /*q.setParam("param1000",queryJoin);
            q.setParam("param2000",queryWhere);*/
            q.setParam("param6", queryWhere);
            string queryString = q.getSQL();

            logger.Debug(queryString);

            this.ExecuteQuery(out dataSet, "TRASMISSIONI", queryString);
        }




        //aggiunto veronica 29102004
        public void GetInfoFasc(out DataSet dataSet, int systemId)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project0");
            q.setParam("param1", "VAR_CODICE, DESCRIPTION," + DocsPaDbManagement.Functions.Functions.ToChar("DTA_APERTURA", false) + "AS DTA_APERTURA, ID_REGISTRO");
            q.setParam("param2", "WHERE SYSTEM_ID = " + systemId);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.ExecuteQuery(out dataSet, "FASCICOLI", queryString);


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objListaFiltri"></param>
        /// <returns></returns>
        private bool getCondFiltri(ref string queryWhere, ref string queryFrom, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            // ret è false se i filtri coninvolgono le tabelle B e C e quindi la query va ripetuta
            bool ret = false;

            string filtroProfilazioneTipo = null;
            string filtroProfilazioneAtto = null;
            string filtroProfilazioneCompleto = null;
            string filtroProfilazioneDiagramma = null;
            string queryProfilazione = " AND A.ID_PROFILE IN (SELECT A.SYSTEM_ID FROM ";

            if (objListaFiltri == null)
                return ret;

            string peopleID = string.Empty;

            DocsPaVO.filtri.FiltroRicerca f;
            for (int i = 0; i < objListaFiltri.Length; i++)
            {
                f = objListaFiltri[i];
                if (f.valore != null && !f.valore.Equals(""))
                {
                    switch (f.argomento)
                    {
                        case "TRASMISSIONE_IL":
                            queryWhere += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO", false) + "= '" + f.valore + "'";
                            break;
                        case "TRASMISSIONE_SUCCESSIVA_AL":
                            queryWhere += " AND A.DTA_INVIO>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "TRASMISSIONE_PRECEDENTE_IL":
                            queryWhere += " AND A.DTA_INVIO<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "TRASMISSIONE_TODAY":
                            //if (!dbType.ToUpper().Equals("SQL"))
                            //{
                            //    queryWhere += " AND to_char(A.DTA_INVIO, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            //}
                            //else
                            //{
                            //    queryWhere += " AND DATEDIFF(DD, A.DTA_INVIO, GETDATE()) = 0 ";
                            //}
                                if (!dbType.ToUpper().Equals("SQL"))
                                    queryWhere += " AND A.DTA_INVIO between trunc(sysdate ,'DD') and sysdate ";
                                else
                                    queryWhere += " AND DATEDIFF(DD, A.DTA_INVIO, GETDATE()) = 0 ";
                            break;
                        case "TRASMISSIONE_SC":
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                queryWhere += " AND A.DTA_INVIO>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND A.DTA_INVIO<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            }
                            else
                            {
                                queryWhere += " AND A.DTA_INVIO>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND A.DTA_INVIO<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            }
                            break;
                        case "TRASMISSIONE_MC":
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                queryWhere += " AND A.DTA_INVIO>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND A.DTA_INVIO<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            }
                            else
                            {
                                queryWhere += " AND A.DTA_INVIO>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND A.DTA_INVIO<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            }
                            break;
                        case "MANCANZA_IMMAGINE":
                            ret = true;
                            queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE CHA_IMG = '0')";// AND "+DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO","0")+"='0'";
                            break;
                        case "CON_IMMAGINE":
                            ret = true;
                            queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE CHA_IMG = '1')";// AND "+DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO","0")+"='0'";
                            break;
                        case "MANCANZA_FASCICOLAZIONE":
                            ret = true;
                            queryWhere += " AND not EXISTS (SELECT 'x' FROM PROJECT_COMPONENTS T WHERE T.LINK = A.ID_PROFILE )";
                            break;
                        case "DA_PROTOCOLLARE":
                            ret = true;
                            queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE CHA_DA_PROTO = '1')";
                            break;
                        case "SCADENZA_IL":
                            ret = true;
                            queryWhere += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("B.DTA_SCADENZA", false) + "= '" + f.valore + "'";
                            break;
                        case "SCADENZA_SUCCESSIVA_AL":
                            ret = true;
                            queryWhere += " AND B.DTA_SCADENZA>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "SCADENZA_PRECEDENTE_IL":
                            ret = true;
                            queryWhere += " AND B.DTA_SCADENZA<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "RISPOSTA_IL":
                            ret = true;
                            queryWhere += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_RISPOSTA", false) + "= '" + f.valore + "'";
                            break;
                        case "RISPOSTA_SUCCESSIVA_AL":
                            ret = true;
                            queryWhere += " AND C.DTA_RISPOSTA>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "RISPOSTA_PRECEDENTE_IL":
                            queryWhere += " AND C.DTA_RISPOSTA<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "NOTE_GENERALI":
                            queryWhere += " AND UPPER(A.VAR_NOTE_GENERALI) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                            break;
                        case "NOTE_INDIVIDUALI":
                            ret = true;
                            queryWhere += " AND UPPER(B.VAR_NOTE_SING) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                            break;
                        case "RAGIONE":
                            ret = true;
                            if (!queryFrom.Contains(",DPA_RAGIONE_TRASM D"))
                                queryFrom += " ,DPA_RAGIONE_TRASM D";
                            queryWhere += " AND B.ID_RAGIONE=D.SYSTEM_ID";

                            // queryWhere += " AND UPPER(D.VAR_DESC_RAGIONE) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                            queryWhere += " AND UPPER(D.VAR_DESC_RAGIONE) = '" + f.valore.ToUpper().Replace("'", "''") + "'";
                            break;
                        case "STATO":
                            ret = true;
                            if (f.valore.Equals("A"))
                            {
                                queryWhere += " AND C.CHA_ACCETTATA ='1'";
                            }
                            else
                            {
                                if (f.valore.Equals("R"))
                                {
                                    queryWhere += " AND C.CHA_RIFIUTATA ='1'";
                                }
                            }
                            break;
                        case "TIPO_OGGETTO":
                            queryWhere += " AND A.CHA_TIPO_OGGETTO = '" + f.valore + "'";
                            //per profilazione dinamica
                            if (f.valore.Equals("D"))
                            {
                                filtroProfilazioneTipo = "PROFILE A";
                            }
                            else
                            {
                                filtroProfilazioneTipo = "PROJECT A";
                                queryProfilazione = " AND A.ID_PROJECT IN (SELECT A.SYSTEM_ID FROM ";
                            }
                            break;
                        case "DESTINATARIO_UTENTE":
                            queryWhere += " AND B.CHA_TIPO_DEST='U' AND B.ID_CORR_GLOBALE IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%')";
                            break;
                        case "DESTINATARIO_RUOLO":
                            queryWhere += " AND B.CHA_TIPO_DEST='R' AND B.ID_CORR_GLOBALE IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%')";
                            break;
                        case "ID_TRASMISSIONE":
                            queryWhere += " AND A.SYSTEM_ID = " + f.valore;
                            break;
                        //nuovi filtri
                        case "COD_RUBR_DEST_UTENTE":
                            queryWhere += " AND C.ID_PEOPLE IN (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '" + f.valore.ToUpper().Replace("'", "''") + "')";
                            break;
                        case "COD_RUBR_DEST_RUOLO":
                            queryWhere += " AND B.CHA_TIPO_DEST='R' AND B.ID_CORR_GLOBALE IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '" + f.valore.ToUpper().Replace("'", "''") + "')";
                            break;
                        case "COD_RUBR_MITT_UTENTE":
                            if (peopleID.Equals(string.Empty))
                                peopleID = GetPeopleID(f.valore);

                            queryWhere += " AND A.ID_PEOPLE IN (" + peopleID + ")";
                            break;
                        case "COD_RUBR_MITT_RUOLO":
                            queryWhere += " AND A.ID_RUOLO_IN_UO IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '" + f.valore.ToUpper().Replace("'", "''") + "')";
                            break;
                        case "MITTENTE_UTENTE":
                            //ERRATA: CONFRONTA SYSTEM_ID DELLA DPA_CORR_GLOBALI CON ID_PEOPLE DELLA DPA_CPRR_GLOBALI !!!
                            //17 nov 2005 - per risoluzione BUG 1496
                            //queryWhere += " AND A.ID_PEOPLE IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%"+f.valore.ToUpper().Replace("'","''")+"%')";
                            queryWhere += " AND A.ID_PEOPLE IN (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%')";

                            //CONCETTUALMENTE SBAGLIATO: il metodo GetPeopleID ritorna la system_id della dpa_corr_globali
                            //dove il VAR_COD_RUBRICA = f.valore. Ma questo in questo caso non centra nulla poichè
                            //in questo CASE ci entra quando si è digitato qualcosa nel campo DESCRIZIONE DEL corrispondente.
                            //in più la CLAUSOLA IN non deve contenere una sola system_id poichè IL CAMPO VAR_DESC_CORR non è univoco.
                            //							if (peopleID.Equals(string.Empty))
                            //								peopleID=GetPeopleID(f.valore);
                            //	
                            //							queryWhere += " AND A.ID_PEOPLE IN (" + peopleID + ")";

                            break;
                        case "MITTENTE_RUOLO":
                            queryWhere += " AND A.ID_RUOLO_IN_UO IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%')";
                            break;
                        case "ID_UO_DEST":
                            queryWhere += " AND B.CHA_TIPO_DEST='R' AND B.ID_CORR_GLOBALE IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND ID_UO =" + f.valore + ")";
                            break;
                        case "ID_UO_MITT":
                            queryWhere += " AND A.ID_RUOLO_IN_UO IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND ID_UO =" + f.valore + ")";
                            break;

                        //Sezione per la ricerca delle trasmissioni : Accettate Rifiutate in data o intervalli di data specifici
                        case "ACCETTATA_RIFIUTATA":
                            if (f.valore == "A")
                            {
                                queryWhere += " AND C.DTA_ACCETTATA > " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753") + " AND C.CHA_VALIDA = '0'  AND b.id_ragione in (select system_id from dpa_ragione_trasm where CHA_TIPO_RAGIONE = 'W') ";
                            }
                            if (f.valore == "R")
                            {
                                queryWhere += " AND C.DTA_RIFIUTATA > " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753") + " AND C.CHA_VALIDA = '0'  AND b.id_ragione in (select system_id from dpa_ragione_trasm where CHA_TIPO_RAGIONE = 'W') ";
                            }
                            if (f.valore == "A_R")
                            {
                                queryWhere += " AND (C.DTA_ACCETTATA > " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753") + " OR C.DTA_RIFIUTATA > " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753") + ") AND C.CHA_VALIDA = '0'  AND b.id_ragione in (select system_id from dpa_ragione_trasm where CHA_TIPO_RAGIONE = 'W') ";
                            }
                            break;
                        case "DATA_ACCETTAZIONE":
                            queryWhere += " AND C.DTA_ACCETTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND C.DTA_ACCETTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "DATA_RIFIUTO":
                            queryWhere += " AND C.DTA_RIFIUTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND C.DTA_RIFIUTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "DATA_ACCETTAZIONE_DA":
                            queryWhere += " AND C.DTA_ACCETTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "DATA_ACCETTAZIONE_A":
                            queryWhere += " AND C.DTA_ACCETTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "DATA_ACCETTAZIONE_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND to_char(C.DTA_ACCETTATA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            else
                                queryWhere += " AND DATEDIFF(DD, C.DTA_ACCETTATA, GETDATE()) = 0 ";
                            break;
                        case "DATA_ACCETTAZIONE_SC":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND C.DTA_ACCETTATA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND C.DTA_ACCETTATA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            else
                                queryWhere += " AND C.DTA_ACCETTATA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND C.DTA_ACCETTATA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            break;
                        case "DATA_ACCETTAZIONE_MC":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND C.DTA_ACCETTATA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND C.DTA_ACCETTATA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryWhere += " AND C.DTA_ACCETTATA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND C.DTA_ACCETTATA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            break;
                        case "DATA_RIFIUTO_DA":
                            queryWhere += " AND C.DTA_RIFIUTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "DATA_RIFIUTO_A":
                            queryWhere += " AND C.DTA_RIFIUTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "DATA_ACC_RIF":
                            queryWhere += " AND ( (C.DTA_ACCETTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND C.DTA_ACCETTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false) + ") ";
                            queryWhere += " OR (C.DTA_RIFIUTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND C.DTA_RIFIUTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false) + ") ) ";
                            break;
                        case "DATA_RIFIUTO_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND to_char(C.DTA_RIFIUTATA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            else
                                queryWhere += " AND DATEDIFF(DD, C.DTA_RIFIUTATA, GETDATE()) = 0 ";
                            break;
                        case "DATA_RIFIUTO_SC":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND C.DTA_RIFIUTATA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND C.DTA_RIFIUTATA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            else
                                queryWhere += " AND C.DTA_RIFIUTATA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND C.DTA_RIFIUTATA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            break;
                        case "DATA_RIFIUTO_MC":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND C.DTA_RIFIUTATA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND C.DTA_RIFIUTATA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryWhere += " AND C.DTA_RIFIUTATA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND C.DTA_RIFIUTATA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            break;
                        case "DATA_ACC_RIF_DA":
                            queryWhere += " AND (C.DTA_ACCETTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            queryWhere += " OR C.DTA_RIFIUTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + ") ";
                            break;
                        case "DATA_ACC_RIF_A":
                            queryWhere += " AND (C.DTA_ACCETTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            queryWhere += " OR C.DTA_RIFIUTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false) + ") ";
                            break;
                        case "DATA_ACC_RIF_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                queryWhere += " AND (to_char(C.DTA_ACCETTATA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                                queryWhere += " OR to_char(C.DTA_RIFIUTATA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)) ";
                            }
                            else
                            {
                                queryWhere += " AND (DATEDIFF(DD, C.DTA_ACCETTATA, GETDATE()) = 0 ";
                                queryWhere += " OR DATEDIFF(DD, C.DTA_RIFIUTATA, GETDATE()) = 0) ";
                            }
                            break;
                        case "DATA_ACC_RIF_SC":
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                queryWhere += " AND (C.DTA_ACCETTATA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND C.DTA_ACCETTATA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                                queryWhere += " OR C.DTA_RIFIUTATA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND C.DTA_RIFIUTATA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual)) ";
                            }
                            else
                            {
                                queryWhere += " AND (C.DTA_ACCETTATA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND C.DTA_ACCETTATA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                                queryWhere += " OR C.DTA_RIFIUTATA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND C.DTA_RIFIUTATA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE()))) ";
                            }
                            break;
                        case "DATA_ACC_RIF_MC":
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                queryWhere += " AND (C.DTA_ACCETTATA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND C.DTA_ACCETTATA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                                queryWhere += " OR C.DTA_RIFIUTATA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND C.DTA_RIFIUTATA<(select to_date(last_day(sysdate)+1) as DAY from dual)) ";
                            }
                            else
                            {
                                queryWhere += " AND (C.DTA_ACCETTATA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND C.DTA_ACCETTATA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                                queryWhere += " OR C.DTA_RIFIUTATA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND C.DTA_RIFIUTATA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate())))) ";
                            }
                            break;
                        //Fine sezione per la ricerca delle trasmissioni : Accettate Rifiutate in data o intervalli di data specifici                        

                        case "OGGETTO_DOCUMENTO_TRASMESSO":
                            // Gestione filtro su oggetto del documento trasmesso

                            // Inserimento nella query della tabella profile e della join con la tabella delle trasmissioni
                            queryFrom += " ,PROFILE P";
                            queryWhere += " AND P.SYSTEM_ID = A.ID_PROFILE" +
                                          " AND UPPER(P.VAR_PROF_OGGETTO) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                            //List<SearchTextItem> items = new List<SearchTextItem>();
                            //string value = DocsPaUtils.Functions.Functions.ReplaceApexes(f.valore).ToUpper();
                            //foreach (string filter in new System.Text.RegularExpressions.Regex("&&").Split(value))
                            //    items.Add(new SearchTextItem(filter, SearchTextOptionsEnum.InitWithWord));
                            //queryWhere += " AND " + DocsPaDbManagement.Functions.Functions.GetContainsTextQuery("P.VAR_PROF_OGGETTO", items.ToArray());
                            break;
                        case "OGGETTO_FASCICOLO_TRASMESSO":
                            // Gestione filtro su oggetto del fascicolo trasmesso
                            queryFrom += " ,PROJECT PJ";
                            queryWhere += " AND PJ.SYSTEM_ID = A.ID_PROJECT" +
                                          " AND UPPER(PJ.DESCRIPTION) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                            break;
                        case "TIPO_ATTO":
                            DocsPaDB.Query_DocsPAWS.Model md = new Model();
                            DocsPaVO.ProfilazioneDinamica.Templates profilo = md.getTemplateById(f.valore);

                            if (profilo != null && profilo.IPER_FASC_DOC == "1")
                            {

                            }
                            else
                            {
                                filtroProfilazioneAtto = "A.ID_TIPO_ATTO=" + f.valore;
                            }
                            break;
                        case "PROFILAZIONE_DINAMICA":
                            DocsPaDB.Query_DocsPAWS.Model model = new Model();
                            filtroProfilazioneCompleto = model.getSeriePerRicercaProfilazione(f.template, "");
                            break;
                        case "DIAGRAMMA_STATO_DOC":
                            filtroProfilazioneTipo += " ,DPA_DIAGRAMMI ";
                            filtroProfilazioneDiagramma = f.valore;
                            break;
                        //PROFILAZIONE FASCICOLI
                        case "TIPOLOGIA_FASCICOLO":
                            DocsPaDB.Query_DocsPAWS.ModelFasc mdFasc = new ModelFasc();
                            DocsPaVO.ProfilazioneDinamica.Templates profiloDin = mdFasc.getTemplateFascById(f.valore);
                            if (profiloDin != null && profiloDin.IPER_FASC_DOC != "1")
                            {
                                filtroProfilazioneAtto += "id_tipo_fasc = " + profiloDin.SYSTEM_ID.ToString();
                            }
                            break;
                        case "DIAGRAMMA_STATO_FASC":
                            filtroProfilazioneDiagramma = "  AND A.SYSTEM_ID IN (SELECT ID_PROJECT FROM DPA_DIAGRAMMI WHERE DPA_DIAGRAMMI.ID_STATO = " + f.valore + ") ";
                            break;
                    }
                }

                // filtri indipendenti dal valore
                switch (f.argomento)
                {
                    case "ASSEGNAZIONI_PENDENTI":
                        ret = true;
                        break;
                    case "TODO_LIST":
                        ret = true;
                        //queryFrom += " ,DPA_RAGIONE_TRASM D";
                        //queryWhere += " AND B.ID_RAGIONE=D.SYSTEM_ID";
                        //queryWhere += 
                        //    " AND ((D.CHA_TIPO_RAGIONE='W' AND C.CHA_ACCETTATA='0' AND C.CHA_RIFIUTATA='0' AND C.CHA_VALIDA='1')" + 
                        //    " OR ((D.CHA_TIPO_RAGIONE='N' OR D.CHA_TIPO_RAGIONE='I') AND C.CHA_VISTA='0'))";
                        queryWhere += " AND C.CHA_VALIDA='1' AND C.CHA_IN_TODOLIST = '1'";
                        break;
                    case "IN_RISPOSTA":
                        ret = true;
                        //query.addJoin("DPA_RAGIONE_TRASM D","B.ID_RAGIONE=D.SYSTEM_ID");
                        if (!queryFrom.Contains(",DPA_RAGIONE_TRASM D"))
                            queryFrom += " ,DPA_RAGIONE_TRASM D";
                        queryWhere += " AND B.ID_RAGIONE=D.SYSTEM_ID";

                        //query.Where += " AND D.CHA_RISPOSTA='1' AND C.CHA_ACCETTATA='1' AND C.ID_TRASM_RISP_SING IS NULL";
                        queryWhere += " AND D.CHA_RISPOSTA='1' AND C.CHA_ACCETTATA='1' AND C.ID_TRASM_RISP_SING IS NULL";
                        break;
                    case "ATTIVITA_NON_CONCLUSE":
                        ret = true;
                        //query.addJoin("DPA_RAGIONE_TRASM D","B.ID_RAGIONE=D.SYSTEM_ID");
                        if (!queryFrom.Contains(",DPA_RAGIONE_TRASM D"))
                            queryFrom += " ,DPA_RAGIONE_TRASM D";
                        queryWhere += " AND B.ID_RAGIONE=D.SYSTEM_ID";

                        //query.Where += " AND D.CHA_RISPOSTA='1'";	
                        queryWhere += " AND D.CHA_RISPOSTA='1'";
                        //condizione sui documenti e non sulla trasmissione
                        //query.Where += 
                        //	" AND " + getNomeColonnaOggetto(objListaFiltri) + 
                        //	" NOT IN (" + getQueryOggettiCompletati(objListaFiltri) + ")";
                        queryWhere +=
                            " AND " + getNomeColonnaOggetto(objListaFiltri) +
                            " NOT IN (" + getQueryOggettiCompletati(objListaFiltri) + ")";
                        break;
                    case "TRASMISSIONE_DOC_PROTOCOLLATI":  //aggiunti il 20/09/2004 per liguria
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL) ";
                        break;
                    case "TRASMISSIONE_DOC_PROT_ARRIVO":
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_TIPO_PROTO = 'A') ";
                        break;
                    case "TRASMISSIONE_DOC_PROT_PARTENZA":
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_TIPO_PROTO = 'P') ";
                        break;
                    case "TRASMISSIONE_DOC_PROT_INTERNO":
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_TIPO_PROTO = 'I') ";
                        break;
                    case "TRASMISSIONE_DOC_NON_PROTOCOLLATI":
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NULL OR DTA_PROTO IS NULL )  ";
                        break;
                    case "TRASMISSIONE_DOC_TUTTI":
                        // queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE " + DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO", "0") + "='0' )  ";
                        //queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE " + DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO", "0") + "='0' )  ";
                        break;

                    case "PENDENTI":
                        queryWhere += " AND C.DTA_ACCETTATA IS NULL AND C.DTA_RIFIUTATA IS NULL AND C.CHA_VALIDA = '1' AND b.id_ragione in (select system_id from dpa_ragione_trasm where CHA_TIPO_RAGIONE = 'W') ";
                        break;

                    case "FIRMATO":
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE ";
                        if (f.valore == "1")
                        {
                            if (dbType.ToUpper() == "SQL")
                                queryWhere += getUserDB() + ".getchafirmato(DOCNUMBER) = '1')";
                            else
                                queryWhere += "getchafirmato(DOCNUMBER) = '1')";
                        }
                        else
                        {
                            if (f.valore == "0")
                            {
                                if (dbType.ToUpper() == "SQL")
                                    queryWhere += getUserDB() + ".getchafirmato(DOCNUMBER) = '0')";
                                else
                                    queryWhere += "getchafirmato(DOCNUMBER) = '0')";
                            }
                            else
                            {
                                if (dbType.ToUpper() == "SQL")
                                {
                                    queryWhere += getUserDB() + ".getchaimg(SYSTEM_ID)<>'0')";
                                }
                                else
                                {
                                    queryWhere += "getchaimg(SYSTEM_ID)<>'0')";
                                }
                            }
                        }
                        break;

                    case "TIPO_FILE_ACQUISITO":
                        //   queryWhere += " AND A.ID_PROFILE IN (SELECT FROM COMPONENTS WHERE UPPER(COMPONENTS.EXT)='" + f.valore.ToUpper() + "' AND COMPONENTS.VERSION_ID=(select max(versions.version_id)  from versions, components where" + " versions.version_id=components.version_id AND versions.docnumber=DOCNUMBER)";
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE ";
                        if (dbType.ToUpper() == "SQL")
                        {
                            queryWhere += "@dbuser@.getchaimg(DOCNUMBER) ='" + f.valore.ToUpper() + "')";
                        }
                        else
                        {
                            queryWhere += "getchaimg(DOCNUMBER) = '" + f.valore.ToUpper() + "')";
                        }
                        break;
                }
            }

            //PER DOCUMENTI PROFILATI
            if (filtroProfilazioneAtto != null)
            {
                queryProfilazione = queryProfilazione + filtroProfilazioneTipo + " WHERE " + filtroProfilazioneAtto + filtroProfilazioneCompleto;
                if (filtroProfilazioneCompleto != null)
                {
                    queryProfilazione += filtroProfilazioneCompleto;
                }
                if (filtroProfilazioneDiagramma != null)
                {
                    queryProfilazione += filtroProfilazioneDiagramma;
                }
                queryWhere += queryProfilazione + ")";
            }


            return ret;
        }


        private bool getCondFiltri2(ref string queryWhere, ref string queryFrom, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo)
        {
            // ret è false se i filtri coninvolgono le tabelle B e C e quindi la query va ripetuta
            bool ret = false;

            string filtroProfilazioneTipo = null;
            string filtroProfilazioneAtto = null;
            string filtroProfilazioneCompleto = null;
            string filtroProfilazioneDiagramma = null;
            string queryProfilazione = " AND A.ID_PROFILE IN (SELECT A.SYSTEM_ID FROM ";

            if (objListaFiltri == null)
                return ret;

            string peopleID = string.Empty;

            DocsPaVO.filtri.FiltroRicerca f;
            for (int i = 0; i < objListaFiltri.Length; i++)
            {
                f = objListaFiltri[i];
                if (f.valore != null && !f.valore.Equals(""))
                {
                    switch (f.argomento)
                    {
                        case "TRASMISSIONE_IL":
                            queryWhere += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO", false) + "= '" + f.valore + "'";
                            break;
                        case "TRASMISSIONE_SUCCESSIVA_AL":
                            queryWhere += " AND A.DTA_INVIO>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "TRASMISSIONE_PRECEDENTE_IL":
                            queryWhere += " AND A.DTA_INVIO<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "TRASMISSIONE_TODAY":
                                if (!dbType.ToUpper().Equals("SQL"))
                                    queryWhere += " AND A.DTA_INVIO between trunc(sysdate ,'DD') and sysdate ";
                                else
                                    queryWhere += " AND DATEDIFF(DD, A.DTA_INVIO, GETDATE()) = 0 ";
                            //if (!dbType.ToUpper().Equals("SQL"))
                            //{
                            //    queryWhere += " AND to_char(A.DTA_INVIO, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            //}
                            //else
                            //{
                            //    queryWhere += " AND DATEDIFF(DD, A.DTA_INVIO, GETDATE()) = 0 ";
                            //}
                            break;
                        case "TRASMISSIONE_SC":
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                queryWhere += " AND A.DTA_INVIO>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND A.DTA_INVIO<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            }
                            else
                            {
                                queryWhere += " AND A.DTA_INVIO>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND A.DTA_INVIO<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            }
                            break;
                        case "TRASMISSIONE_MC":
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                queryWhere += " AND A.DTA_INVIO>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND A.DTA_INVIO<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            }
                            else
                            {
                                queryWhere += " AND A.DTA_INVIO>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND A.DTA_INVIO<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            }
                            break;
                        case "TRASMISSIONE_IERI":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND to_date(to_char(A.DTA_INVIO,'dd/mm/yyyy'),'dd/mm/yyyy') = trunc(sysdate -1 ,'DD') ";
                            else
                                queryWhere += " AND DATEDIFF(DD, A.DTA_INVIO, GETDATE() -1) = 0 ";
                            break;
                        case "TRASMISSIONE_ULTIMI_SETTE_GIORNI":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND A.DTA_INVIO>=(select to_date(to_char(sysdate - 7)) from dual) ";
                            else
                                queryWhere += " AND A.DTA_INVIO>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) ";
                            break;
                        case "TRASMISSIONE_ULTMI_TRENTUNO_GIORNI":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND A.DTA_INVIO>=(select to_date(to_char(sysdate - 31)) from dual) ";
                            else
                                queryWhere += " AND A.DTA_INVIO>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) ";
                            break;
                        case "MANCANZA_IMMAGINE":
                            ret = true;
                            queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE CHA_IMG = '0')";// AND "+DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO","0")+"='0'";
                            break;
                        case "CON_IMMAGINE":
                            ret = true;
                            queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE CHA_IMG = '1')";// AND "+DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO","0")+"='0'";
                            break;
                        case "MANCANZA_FASCICOLAZIONE":
                            ret = true;
                            queryWhere += " AND not EXISTS (SELECT 'x' FROM PROJECT_COMPONENTS T WHERE T.LINK = A.ID_PROFILE )";
                            break;
                        case "DA_PROTOCOLLARE":
                            ret = true;
                            queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE CHA_DA_PROTO = '1')";
                            break;
                        case "SCADENZA_IL":
                            ret = true;
                            queryWhere += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("B.DTA_SCADENZA", false) + "= '" + f.valore + "'";
                            break;
                        case "SCADENZA_SUCCESSIVA_AL":
                            ret = true;
                            queryWhere += " AND B.DTA_SCADENZA>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "SCADENZA_PRECEDENTE_IL":
                            ret = true;
                            queryWhere += " AND B.DTA_SCADENZA<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "RISPOSTA_IL":
                            ret = true;
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            queryWhere += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_RISPOSTA", false) + "= '" + f.valore + "'";
                            break;
                        case "RISPOSTA_SUCCESSIVA_AL":
                            ret = true;
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            queryWhere += " AND C.DTA_RISPOSTA>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "RISPOSTA_PRECEDENTE_IL":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            queryWhere += " AND C.DTA_RISPOSTA<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "NOTE_GENERALI":
                            queryWhere += " AND UPPER(A.VAR_NOTE_GENERALI) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                            break;
                        case "NOTE_INDIVIDUALI":
                            ret = true;
                            queryWhere += " AND UPPER(B.VAR_NOTE_SING) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                            break;
                        case "RAGIONE":
                            ret = true;
                            if (!queryFrom.Contains(",DPA_RAGIONE_TRASM D"))
                                queryFrom += " ,DPA_RAGIONE_TRASM D";
                            queryWhere += " AND B.ID_RAGIONE=D.SYSTEM_ID";

                            // queryWhere += " AND UPPER(D.VAR_DESC_RAGIONE) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                            queryWhere += " AND UPPER(D.VAR_DESC_RAGIONE) = '" + f.valore.ToUpper().Replace("'", "''") + "'";
                            break;
                        case "STATO":
                            ret = true;
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            if (f.valore.Equals("A"))
                            {
                                queryWhere += " AND C.CHA_ACCETTATA ='1'";
                            }
                            else
                            {
                                if (f.valore.Equals("R"))
                                {
                                    queryWhere += " AND C.CHA_RIFIUTATA ='1'";
                                }
                            }
                            break;
                        case "TIPO_OGGETTO":
                            queryWhere += " AND A.CHA_TIPO_OGGETTO = '" + f.valore + "'";
                            //per profilazione dinamica
                            if (f.valore.Equals("D"))
                            {
                                filtroProfilazioneTipo = "PROFILE A";
                            }
                            else
                            {
                                filtroProfilazioneTipo = "PROJECT A";
                                queryProfilazione = " AND A.ID_PROJECT IN (SELECT A.SYSTEM_ID FROM ";
                            }
                            break;
                        case "DESTINATARIO_UTENTE":
                            queryWhere += " AND B.CHA_TIPO_DEST='U' AND B.ID_CORR_GLOBALE IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%')";
                            break;
                        case "DESTINATARIO_RUOLO":
                            StringBuilder filterDescDestCond = new StringBuilder(" AND B.ID_CORR_GLOBALE IN (");

                            FiltroRicerca filterDescDest = objListaFiltri.Where(e => e.argomento == listaArgomentiNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString()).FirstOrDefault();
                            if (filterDescDest != null && Convert.ToBoolean(filterDescDest.valore))
                                filterDescDestCond.AppendFormat("SELECT system_id FROM dpa_corr_globali WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' START WITH upper(var_desc_corr) LIKE '%{0}%' CONNECT BY PRIOR id_old = system_id)", f.valore.ToUpper().Replace("'", "''"));
                            else
                                filterDescDestCond.AppendFormat("SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%{0}%')", f.valore.ToUpper().Replace("'", "''"));

                            queryWhere += filterDescDestCond.ToString();
                            break;
                        case "ID_TRASMISSIONE":
                            queryWhere += " AND A.SYSTEM_ID = " + f.valore;
                            break;
                        //nuovi filtri
                        case "COD_RUBR_DEST_UTENTE":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            queryWhere += " AND C.ID_PEOPLE IN (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '" + f.valore.ToUpper().Replace("'", "''") + "')";
                            break;
                        case "COD_RUBR_DEST_RUOLO":

                            StringBuilder filterCond = new StringBuilder(" AND B.CHA_TIPO_DEST='R' AND B.ID_CORR_GLOBALE IN (");

                            FiltroRicerca filter = objListaFiltri.Where(e => e.argomento == listaArgomentiNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString()).FirstOrDefault();
                            if (filter != null && Convert.ToBoolean(filter.valore))
                            {
                                filterCond.AppendFormat("SELECT system_id FROM dpa_corr_globali WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' START WITH upper(var_cod_rubrica) = '{0}' CONNECT BY PRIOR id_old = system_id)", f.valore.ToUpper().Replace("'", "''"));
                            }
                            else
                                filterCond.AppendFormat("SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '{0}')", f.valore.ToUpper().Replace("'", "''"));

                            queryWhere += filterCond.ToString();

                            break;
                        case "COD_RUBR_MITT_UTENTE":
                            if (peopleID.Equals(string.Empty))
                                peopleID = GetPeopleID(f.valore);

                            queryWhere += " AND A.ID_PEOPLE IN (" + peopleID + ")";
                            break;
                        case "COD_RUBR_MITT_RUOLO":

                            StringBuilder filterRubMittCond = new StringBuilder(" AND A.ID_RUOLO_IN_UO IN (");

                            FiltroRicerca filterRubMitt = objListaFiltri.Where(e => e.argomento == listaArgomentiNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString()).FirstOrDefault();
                            if (filterRubMitt != null && Convert.ToBoolean(filterRubMitt.valore))
                            {
                                filterRubMittCond.AppendFormat("SELECT system_id FROM dpa_corr_globali WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' START WITH upper(var_cod_rubrica) = '{0}' CONNECT BY PRIOR id_old = system_id)", f.valore.ToUpper().Replace("'", "''"));
                            }
                            else
                                filterRubMittCond.AppendFormat("SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '{0}')", f.valore.ToUpper().Replace("'", "''"));

                            queryWhere += filterRubMittCond.ToString();
                            break;
                        case "MITTENTE_UTENTE":
                            //ERRATA: CONFRONTA SYSTEM_ID DELLA DPA_CORR_GLOBALI CON ID_PEOPLE DELLA DPA_CPRR_GLOBALI !!!
                            //17 nov 2005 - per risoluzione BUG 1496
                            //queryWhere += " AND A.ID_PEOPLE IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%"+f.valore.ToUpper().Replace("'","''")+"%')";
                            queryWhere += " AND A.ID_PEOPLE IN (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%')";

                            //CONCETTUALMENTE SBAGLIATO: il metodo GetPeopleID ritorna la system_id della dpa_corr_globali
                            //dove il VAR_COD_RUBRICA = f.valore. Ma questo in questo caso non centra nulla poichè
                            //in questo CASE ci entra quando si è digitato qualcosa nel campo DESCRIZIONE DEL corrispondente.
                            //in più la CLAUSOLA IN non deve contenere una sola system_id poichè IL CAMPO VAR_DESC_CORR non è univoco.
                            //							if (peopleID.Equals(string.Empty))
                            //								peopleID=GetPeopleID(f.valore);
                            //	
                            //							queryWhere += " AND A.ID_PEOPLE IN (" + peopleID + ")";

                            break;
                        case "MITTENTE_RUOLO":
                            StringBuilder filterDescMittCond = new StringBuilder(" AND A.ID_RUOLO_IN_UO IN (");

                            FiltroRicerca filterDescMitt = objListaFiltri.Where(e => e.argomento == listaArgomentiNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString()).FirstOrDefault();
                            if (filterDescMitt != null && Convert.ToBoolean(filterDescMitt.valore))
                                filterDescMittCond.AppendFormat("SELECT system_id FROM dpa_corr_globali WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' START WITH upper(var_desc_corr) LIKE '%{0}%' CONNECT BY PRIOR id_old = system_id)", f.valore.ToUpper().Replace("'", "''"));
                            else
                                filterDescMittCond.AppendFormat("SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%{0}%')", f.valore.ToUpper().Replace("'", "''"));

                            queryWhere += filterDescMittCond.ToString();

                            break;
                        case "ID_UO_DEST":
                            queryWhere += " AND B.CHA_TIPO_DEST='R' AND B.ID_CORR_GLOBALE IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND ID_UO =" + f.valore + ")";
                            break;
                        case "ID_UO_MITT":
                            queryWhere += " AND A.ID_RUOLO_IN_UO IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND ID_UO =" + f.valore + ")";
                            break;

                        //Sezione per la ricerca delle trasmissioni : Accettate Rifiutate in data o intervalli di data specifici
                        case "ACCETTATA_RIFIUTATA":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            if (f.valore == "A")
                            {
                                queryWhere += " AND C.DTA_ACCETTATA > " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753") + " AND C.CHA_VALIDA = '0'  AND b.id_ragione in (select system_id from dpa_ragione_trasm where CHA_TIPO_RAGIONE = 'W') ";
                            }
                            if (f.valore == "R")
                            {
                                queryWhere += " AND C.DTA_RIFIUTATA > " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753") + " AND C.CHA_VALIDA = '0'  AND b.id_ragione in (select system_id from dpa_ragione_trasm where CHA_TIPO_RAGIONE = 'W') ";
                            }
                            if (f.valore == "A_R")
                            {
                                queryWhere += " AND (C.DTA_ACCETTATA > " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753") + " OR C.DTA_RIFIUTATA > " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753") + ") AND C.CHA_VALIDA = '0'  AND b.id_ragione in (select system_id from dpa_ragione_trasm where CHA_TIPO_RAGIONE = 'W') ";
                            }
                            break;
                        case "DATA_ACCETTAZIONE":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            queryWhere += " AND C.DTA_ACCETTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND C.DTA_ACCETTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "DATA_RIFIUTO":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            queryWhere += " AND C.DTA_RIFIUTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND C.DTA_RIFIUTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "DATA_ACCETTAZIONE_DA":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            queryWhere += " AND C.DTA_ACCETTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "DATA_ACCETTAZIONE_A":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            queryWhere += " AND C.DTA_ACCETTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "DATA_ACCETTAZIONE_TODAY":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND to_char(C.DTA_ACCETTATA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            else
                                queryWhere += " AND DATEDIFF(DD, C.DTA_ACCETTATA, GETDATE()) = 0 ";
                            break;
                        case "DATA_ACCETTAZIONE_SC":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND C.DTA_ACCETTATA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND C.DTA_ACCETTATA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            else
                                queryWhere += " AND C.DTA_ACCETTATA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND C.DTA_ACCETTATA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            break;
                        case "DATA_ACCETTAZIONE_MC":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND C.DTA_ACCETTATA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND C.DTA_ACCETTATA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryWhere += " AND C.DTA_ACCETTATA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND C.DTA_ACCETTATA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            break;
                        case "DATA_RIFIUTO_DA":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            queryWhere += " AND C.DTA_RIFIUTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "DATA_RIFIUTO_A":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            queryWhere += " AND C.DTA_RIFIUTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "DATA_ACC_RIF":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            queryWhere += " AND ( (C.DTA_ACCETTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND C.DTA_ACCETTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false) + ") ";
                            queryWhere += " OR (C.DTA_RIFIUTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND C.DTA_RIFIUTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false) + ") ) ";
                            break;
                        case "DATA_RIFIUTO_TODAY":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND to_char(C.DTA_RIFIUTATA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            else
                                queryWhere += " AND DATEDIFF(DD, C.DTA_RIFIUTATA, GETDATE()) = 0 ";
                            break;
                        case "DATA_RIFIUTO_SC":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND C.DTA_RIFIUTATA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND C.DTA_RIFIUTATA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            else
                                queryWhere += " AND C.DTA_RIFIUTATA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND C.DTA_RIFIUTATA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            break;
                        case "DATA_RIFIUTO_MC":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND C.DTA_RIFIUTATA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND C.DTA_RIFIUTATA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryWhere += " AND C.DTA_RIFIUTATA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND C.DTA_RIFIUTATA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            break;
                        case "DATA_ACC_RIF_DA":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            queryWhere += " AND (C.DTA_ACCETTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            queryWhere += " OR C.DTA_RIFIUTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + ") ";
                            break;
                        case "DATA_ACC_RIF_A":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            queryWhere += " AND (C.DTA_ACCETTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            queryWhere += " OR C.DTA_RIFIUTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false) + ") ";
                            break;
                        case "DATA_ACC_RIF_TODAY":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                queryWhere += " AND (to_char(C.DTA_ACCETTATA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                                queryWhere += " OR to_char(C.DTA_RIFIUTATA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)) ";
                            }
                            else
                            {
                                queryWhere += " AND (DATEDIFF(DD, C.DTA_ACCETTATA, GETDATE()) = 0 ";
                                queryWhere += " OR DATEDIFF(DD, C.DTA_RIFIUTATA, GETDATE()) = 0) ";
                            }
                            break;
                        case "DATA_ACC_RIF_SC":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                queryWhere += " AND (C.DTA_ACCETTATA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND C.DTA_ACCETTATA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                                queryWhere += " OR C.DTA_RIFIUTATA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND C.DTA_RIFIUTATA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual)) ";
                            }
                            else
                            {
                                queryWhere += " AND (C.DTA_ACCETTATA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND C.DTA_ACCETTATA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                                queryWhere += " OR C.DTA_RIFIUTATA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND C.DTA_RIFIUTATA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE()))) ";
                            }
                            break;
                        case "DATA_ACC_RIF_MC":
                            if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                            {
                                queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                                queryFrom += " ,DPA_TRASM_UTENTE C";
                            }
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                queryWhere += " AND (C.DTA_ACCETTATA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND C.DTA_ACCETTATA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                                queryWhere += " OR C.DTA_RIFIUTATA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND C.DTA_RIFIUTATA<(select to_date(last_day(sysdate)+1) as DAY from dual)) ";
                            }
                            else
                            {
                                queryWhere += " AND (C.DTA_ACCETTATA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND C.DTA_ACCETTATA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                                queryWhere += " OR C.DTA_RIFIUTATA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND C.DTA_RIFIUTATA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate())))) ";
                            }
                            break;
                        //Fine sezione per la ricerca delle trasmissioni : Accettate Rifiutate in data o intervalli di data specifici                        
                        case "NON_LAVORATE_DA_UTENTE_NOTIFICATO":
                            queryWhere += " AND (  CASE WHEN (SELECT CHA_TIPO_RAGIONE FROM DPA_RAGIONE_TRASM WHERE SYSTEM_ID = B.ID_RAGIONE) = 'W' AND C.DTA_ACCETTATA IS NULL AND C.DTA_RIFIUTATA IS NULL THEN 1 " +
                                                      " WHEN (SELECT CHA_TIPO_RAGIONE FROM DPA_RAGIONE_TRASM WHERE SYSTEM_ID = B.ID_RAGIONE) <> 'W' AND C.DTA_VISTA IS NULL THEN 1 " + // NOT EXISTS (SELECT 'X' FROM DPA_LOG WHERE ID_TRASM_SINGOLA = B.SYSTEM_ID AND (VAR_COD_AZIONE = 'CHECK_TRASM_DOCUMENT' OR VAR_COD_AZIONE = 'CHECK_TRASM_FOLDER') AND ID_PEOPLE_OPERATORE = C.ID_PEOPLE) AND NOT EXISTS (SELECT 'X' FROM DPA_LOG_STORICO WHERE ID_TRASM_SINGOLA = B.SYSTEM_ID AND (VAR_COD_AZIONE = 'CHECK_TRASM_DOCUMENT' OR VAR_COD_AZIONE = 'CHECK_TRASM_FOLDER') AND ID_PEOPLE_OPERATORE = C.ID_PEOPLE) THEN 1" +
                                                       " ELSE 0 END = 1 AND C.ID_PEOPLE = " + f.valore + " AND C.CHA_IN_TODOLIST = '0' ) ";
                            break;
                        case "OGGETTO_DOCUMENTO_TRASMESSO":
                            // Gestione filtro su oggetto del documento trasmesso

                            // Inserimento nella query della tabella profile e della join con la tabella delle trasmissioni
                            queryFrom += " ,PROFILE P";
                            queryWhere += " AND P.SYSTEM_ID = A.ID_PROFILE" +
                                          " AND UPPER(P.VAR_PROF_OGGETTO) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                            //List<SearchTextItem> items = new List<SearchTextItem>();
                            //string value = DocsPaUtils.Functions.Functions.ReplaceApexes(f.valore).ToUpper();
                            //foreach (string filter in new System.Text.RegularExpressions.Regex("&&").Split(value))
                            //    items.Add(new SearchTextItem(filter, SearchTextOptionsEnum.InitWithWord));
                            //queryWhere += " AND " + DocsPaDbManagement.Functions.Functions.GetContainsTextQuery("P.VAR_PROF_OGGETTO", items.ToArray());
                            break;
                        case "OGGETTO_FASCICOLO_TRASMESSO":
                            // Gestione filtro su oggetto del fascicolo trasmesso
                            queryFrom += " ,PROJECT PJ";
                            queryWhere += " AND PJ.SYSTEM_ID = A.ID_PROJECT" +
                                          " AND UPPER(PJ.DESCRIPTION) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                            break;
                        case "TIPO_ATTO":
                            DocsPaDB.Query_DocsPAWS.Model md = new Model();
                            DocsPaVO.ProfilazioneDinamica.Templates profilo = md.getTemplateById(f.valore);

                            if (profilo != null && profilo.IPER_FASC_DOC == "1")
                            {

                            }
                            else
                            {
                                filtroProfilazioneAtto = "A.ID_TIPO_ATTO=" + f.valore;
                            }
                            break;
                        case "PROFILAZIONE_DINAMICA":
                            DocsPaDB.Query_DocsPAWS.Model model = new Model();
                            filtroProfilazioneCompleto = model.getSeriePerRicercaProfilazione(f.template, "");
                            break;
                        case "DIAGRAMMA_STATO_DOC":
                            filtroProfilazioneTipo += " ,DPA_DIAGRAMMI ";
                            filtroProfilazioneDiagramma = f.valore;
                            break;
                        //PROFILAZIONE FASCICOLI
                        case "TIPOLOGIA_FASCICOLO":
                            DocsPaDB.Query_DocsPAWS.ModelFasc mdFasc = new ModelFasc();
                            DocsPaVO.ProfilazioneDinamica.Templates profiloDin = mdFasc.getTemplateFascById(f.valore);
                            if (profiloDin != null && profiloDin.IPER_FASC_DOC != "1")
                            {
                                filtroProfilazioneAtto += "id_tipo_fasc = " + profiloDin.SYSTEM_ID.ToString();
                            }
                            break;
                        case "DIAGRAMMA_STATO_FASC":
                            filtroProfilazioneDiagramma = "  AND A.SYSTEM_ID IN (SELECT ID_PROJECT FROM DPA_DIAGRAMMI WHERE DPA_DIAGRAMMI.ID_STATO = " + f.valore + ") ";
                            break;
                    }
                }

                // filtri indipendenti dal valore
                switch (f.argomento)
                {
                    case "ASSEGNAZIONI_PENDENTI":
                        ret = true;
                        break;
                    case "TODO_LIST":
                        ret = true;
                        if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                        {
                            queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                            queryFrom += " ,DPA_TRASM_UTENTE C";
                        }
                        //queryFrom += " ,DPA_RAGIONE_TRASM D";
                        //queryWhere += " AND B.ID_RAGIONE=D.SYSTEM_ID";
                        //queryWhere += 
                        //    " AND ((D.CHA_TIPO_RAGIONE='W' AND C.CHA_ACCETTATA='0' AND C.CHA_RIFIUTATA='0' AND C.CHA_VALIDA='1')" + 
                        //    " OR ((D.CHA_TIPO_RAGIONE='N' OR D.CHA_TIPO_RAGIONE='I') AND C.CHA_VISTA='0'))";
                        queryWhere += " AND C.CHA_VALIDA='1' AND C.CHA_IN_TODOLIST = '1'";
                        break;
                    case "IN_RISPOSTA":
                        ret = true;
                        if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                        {
                            queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                            queryFrom += " ,DPA_TRASM_UTENTE C";
                        }
                        //query.addJoin("DPA_RAGIONE_TRASM D","B.ID_RAGIONE=D.SYSTEM_ID");
                        if (!queryFrom.Contains(",DPA_RAGIONE_TRASM D"))
                            queryFrom += " ,DPA_RAGIONE_TRASM D";
                        queryWhere += " AND B.ID_RAGIONE=D.SYSTEM_ID";

                        //query.Where += " AND D.CHA_RISPOSTA='1' AND C.CHA_ACCETTATA='1' AND C.ID_TRASM_RISP_SING IS NULL";
                        queryWhere += " AND D.CHA_RISPOSTA='1' AND C.CHA_ACCETTATA='1' AND C.ID_TRASM_RISP_SING IS NULL";
                        break;
                    case "ATTIVITA_NON_CONCLUSE":
                        ret = true;
                        //query.addJoin("DPA_RAGIONE_TRASM D","B.ID_RAGIONE=D.SYSTEM_ID");
                        if (!queryFrom.Contains(",DPA_RAGIONE_TRASM D"))
                            queryFrom += " ,DPA_RAGIONE_TRASM D";
                        queryWhere += " AND B.ID_RAGIONE=D.SYSTEM_ID";

                        //query.Where += " AND D.CHA_RISPOSTA='1'";	
                        queryWhere += " AND D.CHA_RISPOSTA='1'";
                        //condizione sui documenti e non sulla trasmissione
                        //query.Where += 
                        //	" AND " + getNomeColonnaOggetto(objListaFiltri) + 
                        //	" NOT IN (" + getQueryOggettiCompletati(objListaFiltri) + ")";
                        queryWhere +=
                            " AND " + getNomeColonnaOggetto(objListaFiltri) +
                            " NOT IN (" + getQueryOggettiCompletati(objListaFiltri) + ")";
                        break;
                    case "TRASMISSIONE_DOC_PROTOCOLLATI":  //aggiunti il 20/09/2004 per liguria
                        //queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_IN_CESTINO in (null, '0')) ";
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND " + DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO", "'0'") + "='0' ) ";
                        break;
                    case "TRASMISSIONE_DOC_PROT_ARRIVO":
                        //queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_TIPO_PROTO = 'A' AND CHA_IN_CESTINO in (null, '0')) ";
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_TIPO_PROTO = 'A' AND " + DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO", "'0'") + "='0' ) ";
                        break;
                    case "TRASMISSIONE_DOC_PROT_PARTENZA":
                        //queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_TIPO_PROTO = 'P' AND CHA_IN_CESTINO in (null, '0')) ";
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_TIPO_PROTO = 'P' AND " + DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO", "'0'") + "='0' ) ";
                        break;
                    case "TRASMISSIONE_DOC_PROT_INTERNO":
                        //queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_TIPO_PROTO = 'I') AND CHA_IN_CESTINO in (null, '0') ";
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_TIPO_PROTO = 'I' AND " + DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO", "'0'") + "='0' ) ";
                        break;
                    case "TRASMISSIONE_DOC_NON_PROTOCOLLATI":
                        //queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NULL OR DTA_PROTO IS NULL  AND CHA_IN_CESTINO in (null, '0'))  ";
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NULL OR DTA_PROTO IS NULL  AND " + DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO", "'0'") + "='0' ) ";
                        break;
                    case "TRASMISSIONE_DOC_TUTTI":
                        // queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE " + DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO", "0") + "='0' )  ";
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE " + DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO", "'0'") + "='0' )  ";
                        break;

                    case "PENDENTI":
                        if (!queryFrom.Contains(",DPA_TRASM_UTENTE C"))
                        {
                            queryWhere += " AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
                            queryFrom += " ,DPA_TRASM_UTENTE C";
                        }
                        queryWhere += " AND C.DTA_ACCETTATA IS NULL AND C.DTA_RIFIUTATA IS NULL AND C.CHA_VALIDA = '1' AND b.id_ragione in (select system_id from dpa_ragione_trasm where CHA_TIPO_RAGIONE = 'W') ";
                        break;

                    case "FIRMATO":
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE ";
                        if (f.valore == "1")
                        {
                            if (dbType.ToUpper() == "SQL")
                                queryWhere += getUserDB() + ".getchafirmato(DOCNUMBER) = '1')";
                            else
                                queryWhere += "getchafirmato(DOCNUMBER) = '1')";
                        }
                        else
                        {
                            if (f.valore == "0")
                            {
                                if (dbType.ToUpper() == "SQL")
                                    queryWhere += getUserDB() + ".getchafirmato(DOCNUMBER) = '0')";
                                else
                                    queryWhere += "getchafirmato(DOCNUMBER) = '0')";
                            }
                            else
                            {
                                if (dbType.ToUpper() == "SQL")
                                {
                                    queryWhere += getUserDB() + ".getchaimg(SYSTEM_ID)<>'0')";
                                }
                                else
                                {
                                    queryWhere += "getchaimg(SYSTEM_ID)<>'0')";
                                }
                            }
                        }
                        break;

                    case "TIPO_FILE_ACQUISITO":
                        //   queryWhere += " AND A.ID_PROFILE IN (SELECT FROM COMPONENTS WHERE UPPER(COMPONENTS.EXT)='" + f.valore.ToUpper() + "' AND COMPONENTS.VERSION_ID=(select max(versions.version_id)  from versions, components where" + " versions.version_id=components.version_id AND versions.docnumber=DOCNUMBER)";
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE ";
                        if (dbType.ToUpper() == "SQL")
                        {
                            queryWhere += "@dbuser@.getchaimg(DOCNUMBER) ='" + f.valore.ToUpper() + "')";
                        }
                        else
                        {
                            queryWhere += "getchaimg(DOCNUMBER) = '" + f.valore.ToUpper() + "')";
                        }
                        break;
                }
            }

            //PER DOCUMENTI PROFILATI
            if (filtroProfilazioneAtto != null)
            {
                queryProfilazione = queryProfilazione + filtroProfilazioneTipo + " WHERE " + filtroProfilazioneAtto + filtroProfilazioneCompleto;
                if (filtroProfilazioneCompleto != null)
                {
                    queryProfilazione += filtroProfilazioneCompleto;
                }
                if (filtroProfilazioneDiagramma != null)
                {
                    queryProfilazione += filtroProfilazioneDiagramma;
                }
                queryWhere += queryProfilazione + ")";
            }


            return ret;
        }

        private bool getCondFiltri(ref string queryWhere, ref string queryFrom, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo)
        {
            // ret è false se i filtri coninvolgono le tabelle B e C e quindi la query va ripetuta
            bool ret = false;

            string filtroProfilazioneTipo = null;
            string filtroProfilazioneAtto = null;
            string filtroProfilazioneCompleto = null;
            string filtroProfilazioneDiagramma = null;
            string queryProfilazione = " AND A.ID_PROFILE IN (SELECT A.SYSTEM_ID FROM ";

            if (objListaFiltri == null)
                return ret;

            string peopleID = string.Empty;

            DocsPaVO.filtri.FiltroRicerca f;
            for (int i = 0; i < objListaFiltri.Length; i++)
            {
                f = objListaFiltri[i];
                if (f.valore != null && !f.valore.Equals(""))
                {
                    switch (f.argomento)
                    {
                        case "TRASMISSIONE_IL":
                            queryWhere += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO", false) + "= '" + f.valore + "'";
                            break;
                        case "TRASMISSIONE_SUCCESSIVA_AL":
                            queryWhere += " AND A.DTA_INVIO>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "TRASMISSIONE_PRECEDENTE_IL":
                            queryWhere += " AND A.DTA_INVIO<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "TRASMISSIONE_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND A.DTA_INVIO between trunc(sysdate ,'DD') and sysdate ";
                            else
                                queryWhere += " AND DATEDIFF(DD, A.DTA_INVIO, GETDATE()) = 0 ";
                            //if (!dbType.ToUpper().Equals("SQL"))
                            //{
                            //    queryWhere += " AND to_char(A.DTA_INVIO, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            //}
                            //else
                            //{
                            //    queryWhere += " AND DATEDIFF(DD, A.DTA_INVIO, GETDATE()) = 0 ";
                            //}
                            break;
                        case "TRASMISSIONE_SC":
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                queryWhere += " AND A.DTA_INVIO>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND A.DTA_INVIO<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            }
                            else
                            {
                                queryWhere += " AND A.DTA_INVIO>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND A.DTA_INVIO<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            }
                            break;
                        case "TRASMISSIONE_MC":
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                queryWhere += " AND A.DTA_INVIO>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND A.DTA_INVIO<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            }
                            else
                            {
                                queryWhere += " AND A.DTA_INVIO>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND A.DTA_INVIO<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            }
                            break;
                        case "TRASMISSIONE_IERI":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND to_date(to_char(A.DTA_INVIO,'dd/mm/yyyy'),'dd/mm/yyyy') = trunc(sysdate -1 ,'DD') ";
                            else
                                queryWhere += " AND DATEDIFF(DD, A.DTA_INVIO, GETDATE() -1) = 0 ";
                            break;
                        case "TRASMISSIONE_ULTIMI_SETTE_GIORNI":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND A.DTA_INVIO>=(select to_date(to_char(sysdate - 7)) from dual) ";
                            else
                                queryWhere += " AND A.DTA_INVIO>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) ";
                            break;
                        case "TRASMISSIONE_ULTMI_TRENTUNO_GIORNI":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND A.DTA_INVIO>=(select to_date(to_char(sysdate - 31)) from dual) ";
                            else
                                queryWhere += " AND A.DTA_INVIO>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) ";
                            break;
                        case "MANCANZA_IMMAGINE":
                            ret = true;
                            queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE CHA_IMG = '0')";// AND "+DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO","0")+"='0'";
                            break;
                        case "CON_IMMAGINE":
                            ret = true;
                            queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE CHA_IMG = '1')";// AND "+DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO","0")+"='0'";
                            break;
                        case "MANCANZA_FASCICOLAZIONE":
                            ret = true;
                            queryWhere += " AND not EXISTS (SELECT 'x' FROM PROJECT_COMPONENTS T WHERE T.LINK = A.ID_PROFILE )";
                            break;
                        case "DA_PROTOCOLLARE":
                            ret = true;
                            queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE CHA_DA_PROTO = '1')";
                            break;
                        case "SCADENZA_IL":
                            ret = true;
                            queryWhere += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("B.DTA_SCADENZA", false) + "= '" + f.valore + "'";
                            break;
                        case "SCADENZA_SUCCESSIVA_AL":
                            ret = true;
                            queryWhere += " AND B.DTA_SCADENZA>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "SCADENZA_PRECEDENTE_IL":
                            ret = true;
                            queryWhere += " AND B.DTA_SCADENZA<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "RISPOSTA_IL":
                            ret = true;
                            queryWhere += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_RISPOSTA", false) + "= '" + f.valore + "'";
                            break;
                        case "RISPOSTA_SUCCESSIVA_AL":
                            ret = true;
                            queryWhere += " AND C.DTA_RISPOSTA>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "RISPOSTA_PRECEDENTE_IL":
                            queryWhere += " AND C.DTA_RISPOSTA<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "NOTE_GENERALI":
                            queryWhere += " AND UPPER(A.VAR_NOTE_GENERALI) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                            break;
                        case "NOTE_INDIVIDUALI":
                            ret = true;
                            queryWhere += " AND UPPER(B.VAR_NOTE_SING) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                            break;
                        case "RAGIONE":
                            ret = true;
                            if (!queryFrom.Contains(",DPA_RAGIONE_TRASM D"))
                                queryFrom += " ,DPA_RAGIONE_TRASM D";
                            queryWhere += " AND B.ID_RAGIONE=D.SYSTEM_ID";

                            // queryWhere += " AND UPPER(D.VAR_DESC_RAGIONE) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                            queryWhere += " AND UPPER(D.VAR_DESC_RAGIONE) = '" + f.valore.ToUpper().Replace("'", "''") + "'";
                            break;
                        case "STATO":
                            ret = true;
                            if (f.valore.Equals("A"))
                            {
                                queryWhere += " AND C.CHA_ACCETTATA ='1'";
                            }
                            else
                            {
                                if (f.valore.Equals("R"))
                                {
                                    queryWhere += " AND C.CHA_RIFIUTATA ='1'";
                                }
                            }
                            break;
                        case "TIPO_OGGETTO":
                            queryWhere += " AND A.CHA_TIPO_OGGETTO = '" + f.valore + "'";
                            //per profilazione dinamica
                            if (f.valore.Equals("D"))
                            {
                                filtroProfilazioneTipo = "PROFILE A";
                            }
                            else
                            {
                                filtroProfilazioneTipo = "PROJECT A";
                                queryProfilazione = " AND A.ID_PROJECT IN (SELECT A.SYSTEM_ID FROM ";
                            }
                            break;
                        case "DESTINATARIO_UTENTE":
                            queryWhere += " AND B.CHA_TIPO_DEST='U' AND B.ID_CORR_GLOBALE IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%')";
                            break;
                        case "DESTINATARIO_RUOLO":
                            StringBuilder filterDescDestCond = new StringBuilder(" AND B.ID_CORR_GLOBALE IN (");

                            FiltroRicerca filterDescDest = objListaFiltri.Where(e => e.argomento == listaArgomentiNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString()).FirstOrDefault();
                            if (filterDescDest != null && Convert.ToBoolean(filterDescDest.valore))
                                filterDescDestCond.AppendFormat("SELECT system_id FROM dpa_corr_globali WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' START WITH upper(var_desc_corr) LIKE '%{0}%' CONNECT BY PRIOR id_old = system_id)", f.valore.ToUpper().Replace("'", "''"));
                            else
                                filterDescDestCond.AppendFormat("SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%{0}%')", f.valore.ToUpper().Replace("'", "''"));

                            queryWhere += filterDescDestCond.ToString();
                            break;
                        case "ID_TRASMISSIONE":
                            queryWhere += " AND A.SYSTEM_ID = " + f.valore;
                            break;
                        //nuovi filtri
                        case "COD_RUBR_DEST_UTENTE":
                            queryWhere += " AND C.ID_PEOPLE IN (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '" + f.valore.ToUpper().Replace("'", "''") + "')";
                            break;
                        case "COD_RUBR_DEST_RUOLO":

                            StringBuilder filterCond = new StringBuilder(" AND B.CHA_TIPO_DEST='R' AND B.ID_CORR_GLOBALE IN (");

                            FiltroRicerca filter = objListaFiltri.Where(e => e.argomento == listaArgomentiNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString()).FirstOrDefault();
                            if (filter != null && Convert.ToBoolean(filter.valore))
                            {
                                filterCond.AppendFormat("SELECT system_id FROM dpa_corr_globali WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' START WITH upper(var_cod_rubrica) = '{0}' CONNECT BY PRIOR id_old = system_id)", f.valore.ToUpper().Replace("'", "''"));
                            }
                            else
                                filterCond.AppendFormat("SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '{0}')", f.valore.ToUpper().Replace("'", "''"));

                            queryWhere += filterCond.ToString();

                            break;
                        case "COD_RUBR_MITT_UTENTE":
                            if (peopleID.Equals(string.Empty))
                                peopleID = GetPeopleID(f.valore);

                            queryWhere += " AND A.ID_PEOPLE IN (" + peopleID + ")";
                            break;
                        case "COD_RUBR_MITT_RUOLO":

                            StringBuilder filterRubMittCond = new StringBuilder(" AND A.ID_RUOLO_IN_UO IN (");

                            FiltroRicerca filterRubMitt = objListaFiltri.Where(e => e.argomento == listaArgomentiNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString()).FirstOrDefault();
                            if (filterRubMitt != null && Convert.ToBoolean(filterRubMitt.valore))
                            {
                                filterRubMittCond.AppendFormat("SELECT system_id FROM dpa_corr_globali WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' START WITH upper(var_cod_rubrica) = '{0}' CONNECT BY PRIOR id_old = system_id)", f.valore.ToUpper().Replace("'", "''"));
                            }
                            else
                                filterRubMittCond.AppendFormat("SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '{0}')", f.valore.ToUpper().Replace("'", "''"));

                            queryWhere += filterRubMittCond.ToString();
                            break;
                        case "MITTENTE_UTENTE":
                            //ERRATA: CONFRONTA SYSTEM_ID DELLA DPA_CORR_GLOBALI CON ID_PEOPLE DELLA DPA_CPRR_GLOBALI !!!
                            //17 nov 2005 - per risoluzione BUG 1496
                            //queryWhere += " AND A.ID_PEOPLE IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%"+f.valore.ToUpper().Replace("'","''")+"%')";
                            queryWhere += " AND A.ID_PEOPLE IN (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%')";

                            //CONCETTUALMENTE SBAGLIATO: il metodo GetPeopleID ritorna la system_id della dpa_corr_globali
                            //dove il VAR_COD_RUBRICA = f.valore. Ma questo in questo caso non centra nulla poichè
                            //in questo CASE ci entra quando si è digitato qualcosa nel campo DESCRIZIONE DEL corrispondente.
                            //in più la CLAUSOLA IN non deve contenere una sola system_id poichè IL CAMPO VAR_DESC_CORR non è univoco.
                            //							if (peopleID.Equals(string.Empty))
                            //								peopleID=GetPeopleID(f.valore);
                            //	
                            //							queryWhere += " AND A.ID_PEOPLE IN (" + peopleID + ")";

                            break;
                        case "MITTENTE_RUOLO":
                            StringBuilder filterDescMittCond = new StringBuilder(" AND A.ID_RUOLO_IN_UO IN (");

                            FiltroRicerca filterDescMitt = objListaFiltri.Where(e => e.argomento == listaArgomentiNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString()).FirstOrDefault();
                            if (filterDescMitt != null && Convert.ToBoolean(filterDescMitt.valore))
                                filterDescMittCond.AppendFormat("SELECT system_id FROM dpa_corr_globali WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' START WITH upper(var_desc_corr) LIKE '%{0}%' CONNECT BY PRIOR id_old = system_id)", f.valore.ToUpper().Replace("'", "''"));
                            else
                                filterDescMittCond.AppendFormat("SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%{0}%')", f.valore.ToUpper().Replace("'", "''"));

                            queryWhere += filterDescMittCond.ToString();

                            break;
                        case "ID_UO_DEST":
                            queryWhere += " AND B.CHA_TIPO_DEST='R' AND B.ID_CORR_GLOBALE IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND ID_UO =" + f.valore + ")";
                            break;
                        case "ID_UO_MITT":
                            queryWhere += " AND A.ID_RUOLO_IN_UO IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND ID_UO =" + f.valore + ")";
                            break;

                        //Sezione per la ricerca delle trasmissioni : Accettate Rifiutate in data o intervalli di data specifici
                        case "ACCETTATA_RIFIUTATA":
                            if (f.valore == "A")
                            {
                                queryWhere += " AND C.DTA_ACCETTATA > " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753") + " AND C.CHA_VALIDA = '0'  AND b.id_ragione in (select system_id from dpa_ragione_trasm where CHA_TIPO_RAGIONE = 'W') ";
                            }
                            if (f.valore == "R")
                            {
                                queryWhere += " AND C.DTA_RIFIUTATA > " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753") + " AND C.CHA_VALIDA = '0'  AND b.id_ragione in (select system_id from dpa_ragione_trasm where CHA_TIPO_RAGIONE = 'W') ";
                            }
                            if (f.valore == "A_R")
                            {
                                queryWhere += " AND (C.DTA_ACCETTATA > " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753") + " OR C.DTA_RIFIUTATA > " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753") + ") AND C.CHA_VALIDA = '0'  AND b.id_ragione in (select system_id from dpa_ragione_trasm where CHA_TIPO_RAGIONE = 'W') ";
                            }
                            break;
                        case "DATA_ACCETTAZIONE":
                            queryWhere += " AND C.DTA_ACCETTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND C.DTA_ACCETTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "DATA_RIFIUTO":
                            queryWhere += " AND C.DTA_RIFIUTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND C.DTA_RIFIUTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "DATA_ACCETTAZIONE_DA":
                            queryWhere += " AND C.DTA_ACCETTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "DATA_ACCETTAZIONE_A":
                            queryWhere += " AND C.DTA_ACCETTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "DATA_ACCETTAZIONE_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND to_char(C.DTA_ACCETTATA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            else
                                queryWhere += " AND DATEDIFF(DD, C.DTA_ACCETTATA, GETDATE()) = 0 ";
                            break;
                        case "DATA_ACCETTAZIONE_SC":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND C.DTA_ACCETTATA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND C.DTA_ACCETTATA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            else
                                queryWhere += " AND C.DTA_ACCETTATA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND C.DTA_ACCETTATA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            break;
                        case "DATA_ACCETTAZIONE_MC":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND C.DTA_ACCETTATA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND C.DTA_ACCETTATA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryWhere += " AND C.DTA_ACCETTATA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND C.DTA_ACCETTATA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            break;
                        case "DATA_RIFIUTO_DA":
                            queryWhere += " AND C.DTA_RIFIUTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "DATA_RIFIUTO_A":
                            queryWhere += " AND C.DTA_RIFIUTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "DATA_ACC_RIF":
                            queryWhere += " AND ( (C.DTA_ACCETTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND C.DTA_ACCETTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false) + ") ";
                            queryWhere += " OR (C.DTA_RIFIUTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND C.DTA_RIFIUTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false) + ") ) ";
                            break;
                        case "DATA_RIFIUTO_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND to_char(C.DTA_RIFIUTATA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            else
                                queryWhere += " AND DATEDIFF(DD, C.DTA_RIFIUTATA, GETDATE()) = 0 ";
                            break;
                        case "DATA_RIFIUTO_SC":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND C.DTA_RIFIUTATA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND C.DTA_RIFIUTATA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            else
                                queryWhere += " AND C.DTA_RIFIUTATA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND C.DTA_RIFIUTATA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            break;
                        case "DATA_RIFIUTO_MC":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryWhere += " AND C.DTA_RIFIUTATA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND C.DTA_RIFIUTATA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryWhere += " AND C.DTA_RIFIUTATA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND C.DTA_RIFIUTATA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            break;
                        case "DATA_ACC_RIF_DA":
                            queryWhere += " AND (C.DTA_ACCETTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            queryWhere += " OR C.DTA_RIFIUTATA >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + ") ";
                            break;
                        case "DATA_ACC_RIF_A":
                            queryWhere += " AND (C.DTA_ACCETTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            queryWhere += " OR C.DTA_RIFIUTATA <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false) + ") ";
                            break;
                        case "DATA_ACC_RIF_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                queryWhere += " AND (to_char(C.DTA_ACCETTATA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                                queryWhere += " OR to_char(C.DTA_RIFIUTATA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)) ";
                            }
                            else
                            {
                                queryWhere += " AND (DATEDIFF(DD, C.DTA_ACCETTATA, GETDATE()) = 0 ";
                                queryWhere += " OR DATEDIFF(DD, C.DTA_RIFIUTATA, GETDATE()) = 0) ";
                            }
                            break;
                        case "DATA_ACC_RIF_SC":
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                queryWhere += " AND (C.DTA_ACCETTATA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND C.DTA_ACCETTATA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                                queryWhere += " OR C.DTA_RIFIUTATA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND C.DTA_RIFIUTATA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual)) ";
                            }
                            else
                            {
                                queryWhere += " AND (C.DTA_ACCETTATA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND C.DTA_ACCETTATA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                                queryWhere += " OR C.DTA_RIFIUTATA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND C.DTA_RIFIUTATA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE()))) ";
                            }
                            break;
                        case "DATA_ACC_RIF_MC":
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                queryWhere += " AND (C.DTA_ACCETTATA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND C.DTA_ACCETTATA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                                queryWhere += " OR C.DTA_RIFIUTATA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND C.DTA_RIFIUTATA<(select to_date(last_day(sysdate)+1) as DAY from dual)) ";
                            }
                            else
                            {
                                queryWhere += " AND (C.DTA_ACCETTATA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND C.DTA_ACCETTATA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                                queryWhere += " OR C.DTA_RIFIUTATA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND C.DTA_RIFIUTATA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate())))) ";
                            }
                            break;
                        //Fine sezione per la ricerca delle trasmissioni : Accettate Rifiutate in data o intervalli di data specifici                        
                        case "NON_LAVORATE_DA_UTENTE_NOTIFICATO":
                            queryWhere += " AND (  CASE WHEN (SELECT CHA_TIPO_RAGIONE FROM DPA_RAGIONE_TRASM WHERE SYSTEM_ID = B.ID_RAGIONE) = 'W' AND C.DTA_ACCETTATA IS NULL AND C.DTA_RIFIUTATA IS NULL THEN 1 " +
                                                      " WHEN (SELECT CHA_TIPO_RAGIONE FROM DPA_RAGIONE_TRASM WHERE SYSTEM_ID = B.ID_RAGIONE) <> 'W' AND C.DTA_VISTA IS NULL THEN 1 " + // NOT EXISTS (SELECT 'X' FROM DPA_LOG WHERE ID_TRASM_SINGOLA = B.SYSTEM_ID AND (VAR_COD_AZIONE = 'CHECK_TRASM_DOCUMENT' OR VAR_COD_AZIONE = 'CHECK_TRASM_FOLDER') AND ID_PEOPLE_OPERATORE = C.ID_PEOPLE) AND NOT EXISTS (SELECT 'X' FROM DPA_LOG_STORICO WHERE ID_TRASM_SINGOLA = B.SYSTEM_ID AND (VAR_COD_AZIONE = 'CHECK_TRASM_DOCUMENT' OR VAR_COD_AZIONE = 'CHECK_TRASM_FOLDER') AND ID_PEOPLE_OPERATORE = C.ID_PEOPLE) THEN 1" +
                                                       " ELSE 0 END = 1 AND C.ID_PEOPLE = " + f.valore + " AND C.CHA_IN_TODOLIST = '0' ) ";
                            break;
                        case "OGGETTO_DOCUMENTO_TRASMESSO":
                            // Gestione filtro su oggetto del documento trasmesso

                            // Inserimento nella query della tabella profile e della join con la tabella delle trasmissioni
                            queryFrom += " ,PROFILE P";
                            queryWhere += " AND P.SYSTEM_ID = A.ID_PROFILE" +
                                          " AND UPPER(P.VAR_PROF_OGGETTO) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                            //List<SearchTextItem> items = new List<SearchTextItem>();
                            //string value = DocsPaUtils.Functions.Functions.ReplaceApexes(f.valore).ToUpper();
                            //foreach (string filter in new System.Text.RegularExpressions.Regex("&&").Split(value))
                            //    items.Add(new SearchTextItem(filter, SearchTextOptionsEnum.InitWithWord));
                            //queryWhere += " AND " + DocsPaDbManagement.Functions.Functions.GetContainsTextQuery("P.VAR_PROF_OGGETTO", items.ToArray());
                            break;
                        case "OGGETTO_FASCICOLO_TRASMESSO":
                            // Gestione filtro su oggetto del fascicolo trasmesso
                            queryFrom += " ,PROJECT PJ";
                            queryWhere += " AND PJ.SYSTEM_ID = A.ID_PROJECT" +
                                          " AND UPPER(PJ.DESCRIPTION) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                            break;
                        case "TIPO_ATTO":
                            DocsPaDB.Query_DocsPAWS.Model md = new Model();
                            DocsPaVO.ProfilazioneDinamica.Templates profilo = md.getTemplateById(f.valore);

                            if (profilo != null && profilo.IPER_FASC_DOC == "1")
                            {

                            }
                            else
                            {
                                filtroProfilazioneAtto = "A.ID_TIPO_ATTO=" + f.valore;
                            }
                            break;
                        case "PROFILAZIONE_DINAMICA":
                            DocsPaDB.Query_DocsPAWS.Model model = new Model();
                            filtroProfilazioneCompleto = model.getSeriePerRicercaProfilazione(f.template, "");
                            break;
                        case "DIAGRAMMA_STATO_DOC":
                            filtroProfilazioneTipo += " ,DPA_DIAGRAMMI ";
                            filtroProfilazioneDiagramma = f.valore;
                            break;
                        //PROFILAZIONE FASCICOLI
                        case "TIPOLOGIA_FASCICOLO":
                            DocsPaDB.Query_DocsPAWS.ModelFasc mdFasc = new ModelFasc();
                            DocsPaVO.ProfilazioneDinamica.Templates profiloDin = mdFasc.getTemplateFascById(f.valore);
                            if (profiloDin != null && profiloDin.IPER_FASC_DOC != "1")
                            {
                                filtroProfilazioneAtto += "id_tipo_fasc = " + profiloDin.SYSTEM_ID.ToString();
                            }
                            break;
                        case "DIAGRAMMA_STATO_FASC":
                            filtroProfilazioneDiagramma = "  AND A.SYSTEM_ID IN (SELECT ID_PROJECT FROM DPA_DIAGRAMMI WHERE DPA_DIAGRAMMI.ID_STATO = " + f.valore + ") ";
                            break;
                    }
                }

                // filtri indipendenti dal valore
                switch (f.argomento)
                {
                    case "ASSEGNAZIONI_PENDENTI":
                        ret = true;
                        break;
                    case "TODO_LIST":
                        ret = true;
                        //queryFrom += " ,DPA_RAGIONE_TRASM D";
                        //queryWhere += " AND B.ID_RAGIONE=D.SYSTEM_ID";
                        //queryWhere += 
                        //    " AND ((D.CHA_TIPO_RAGIONE='W' AND C.CHA_ACCETTATA='0' AND C.CHA_RIFIUTATA='0' AND C.CHA_VALIDA='1')" + 
                        //    " OR ((D.CHA_TIPO_RAGIONE='N' OR D.CHA_TIPO_RAGIONE='I') AND C.CHA_VISTA='0'))";
                        queryWhere += " AND C.CHA_VALIDA='1' AND C.CHA_IN_TODOLIST = '1'";
                        break;
                    case "IN_RISPOSTA":
                        ret = true;
                        //query.addJoin("DPA_RAGIONE_TRASM D","B.ID_RAGIONE=D.SYSTEM_ID");
                        if (!queryFrom.Contains(",DPA_RAGIONE_TRASM D"))
                            queryFrom += " ,DPA_RAGIONE_TRASM D";
                        queryWhere += " AND B.ID_RAGIONE=D.SYSTEM_ID";

                        //query.Where += " AND D.CHA_RISPOSTA='1' AND C.CHA_ACCETTATA='1' AND C.ID_TRASM_RISP_SING IS NULL";
                        queryWhere += " AND D.CHA_RISPOSTA='1' AND C.CHA_ACCETTATA='1' AND C.ID_TRASM_RISP_SING IS NULL";
                        break;
                    case "ATTIVITA_NON_CONCLUSE":
                        ret = true;
                        //query.addJoin("DPA_RAGIONE_TRASM D","B.ID_RAGIONE=D.SYSTEM_ID");
                        if (!queryFrom.Contains(",DPA_RAGIONE_TRASM D"))
                            queryFrom += " ,DPA_RAGIONE_TRASM D";
                        queryWhere += " AND B.ID_RAGIONE=D.SYSTEM_ID";

                        //query.Where += " AND D.CHA_RISPOSTA='1'";	
                        queryWhere += " AND D.CHA_RISPOSTA='1'";
                        //condizione sui documenti e non sulla trasmissione
                        //query.Where += 
                        //	" AND " + getNomeColonnaOggetto(objListaFiltri) + 
                        //	" NOT IN (" + getQueryOggettiCompletati(objListaFiltri) + ")";
                        queryWhere +=
                            " AND " + getNomeColonnaOggetto(objListaFiltri) +
                            " NOT IN (" + getQueryOggettiCompletati(objListaFiltri) + ")";
                        break;
                    case "TRASMISSIONE_DOC_PROTOCOLLATI":  //aggiunti il 20/09/2004 per liguria
                        //queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_IN_CESTINO in (null, '0')) ";
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND " + DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO", "'0'") + "='0' ) ";
                        break;
                    case "TRASMISSIONE_DOC_PROT_ARRIVO":
                        //queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_TIPO_PROTO = 'A' AND CHA_IN_CESTINO in (null, '0')) ";
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_TIPO_PROTO = 'A' AND " + DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO", "'0'") + "='0' ) ";
                        break;
                    case "TRASMISSIONE_DOC_PROT_PARTENZA":
                        //queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_TIPO_PROTO = 'P' AND CHA_IN_CESTINO in (null, '0')) ";
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_TIPO_PROTO = 'P' AND " + DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO", "'0'") + "='0' ) ";
                        break;
                    case "TRASMISSIONE_DOC_PROT_INTERNO":
                        //queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_TIPO_PROTO = 'I') AND CHA_IN_CESTINO in (null, '0') ";
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NOT NULL AND DTA_PROTO IS NOT NULL AND CHA_TIPO_PROTO = 'I' AND " + DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO", "'0'") + "='0' ) ";
                        break;
                    case "TRASMISSIONE_DOC_NON_PROTOCOLLATI":
                        //queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NULL OR DTA_PROTO IS NULL  AND CHA_IN_CESTINO in (null, '0'))  ";
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE NUM_PROTO IS NULL AND DTA_PROTO IS NULL  AND " + DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO", "'0'") + "='0' ) ";
                        break;
                    case "TRASMISSIONE_DOC_TUTTI":
                        // queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE " + DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO", "0") + "='0' )  ";
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE " + DocsPaDbManagement.Functions.Functions.getNVL("CHA_IN_CESTINO", "'0'") + "='0' )  ";
                        break;

                    case "PENDENTI":
                        queryWhere += " AND C.DTA_ACCETTATA IS NULL AND C.DTA_RIFIUTATA IS NULL AND C.CHA_VALIDA = '1' AND b.id_ragione in (select system_id from dpa_ragione_trasm where CHA_TIPO_RAGIONE = 'W') ";
                        break;

                    case "FIRMATO":
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE ";
                        if (f.valore == "1")
                        {
                            if (dbType.ToUpper() == "SQL")
                                queryWhere += getUserDB() + ".getchafirmato(DOCNUMBER) = '1')";
                            else
                                queryWhere += "getchafirmato(DOCNUMBER) = '1')";
                        }
                        else
                        {
                            if (f.valore == "0")
                            {
                                if (dbType.ToUpper() == "SQL")
                                    queryWhere += getUserDB() + ".getchafirmato(DOCNUMBER) = '0')";
                                else
                                    queryWhere += "getchafirmato(DOCNUMBER) = '0')";
                            }
                            else
                            {
                                if (dbType.ToUpper() == "SQL")
                                {
                                    queryWhere += getUserDB() + ".getchaimg(SYSTEM_ID)<>'0')";
                                }
                                else
                                {
                                    queryWhere += "getchaimg(SYSTEM_ID)<>'0')";
                                }
                            }
                        }
                        break;

                    case "TIPO_FILE_ACQUISITO":
                        //   queryWhere += " AND A.ID_PROFILE IN (SELECT FROM COMPONENTS WHERE UPPER(COMPONENTS.EXT)='" + f.valore.ToUpper() + "' AND COMPONENTS.VERSION_ID=(select max(versions.version_id)  from versions, components where" + " versions.version_id=components.version_id AND versions.docnumber=DOCNUMBER)";
                        queryWhere += " AND A.ID_PROFILE IN (SELECT SYSTEM_ID FROM PROFILE WHERE ";
                        if (dbType.ToUpper() == "SQL")
                        {
                            queryWhere += "@dbuser@.getchaimg(DOCNUMBER) ='" + f.valore.ToUpper() + "')";
                        }
                        else
                        {
                            queryWhere += "getchaimg(DOCNUMBER) = '" + f.valore.ToUpper() + "')";
                        }
                        break;
                }
            }

            //PER DOCUMENTI PROFILATI
            if (filtroProfilazioneAtto != null)
            {
                queryProfilazione = queryProfilazione + filtroProfilazioneTipo + " WHERE " + filtroProfilazioneAtto + filtroProfilazioneCompleto;
                if (filtroProfilazioneCompleto != null)
                {
                    queryProfilazione += filtroProfilazioneCompleto;
                }
                if (filtroProfilazioneDiagramma != null)
                {
                    queryProfilazione += filtroProfilazioneDiagramma;
                }
                queryWhere += queryProfilazione + ")";
            }


            return ret;
        }

        /// <summary>
        /// Reperimento valore system_id dalla tabella DPA_CORR_GLOBALI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetPeopleID(string name)
        {
            string commandText = "SELECT ID_PEOPLE " +
                "FROM DPA_CORR_GLOBALI " +
                "WHERE CHA_TIPO_URP='P' AND " +
                "CHA_TIPO_IE='I' AND " +
                "UPPER(VAR_COD_RUBRICA) = '" + name.ToUpper().Replace("'", "''") + "'";

            logger.Debug(commandText);

            string retValue;
            this.ExecuteScalar(out retValue, commandText);
            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objListaFiltri"></param>
        /// <returns></returns>
        private string getNomeColonnaOggetto(DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            for (int i = 0; i < objListaFiltri.Length; i++)
            {
                if (objListaFiltri[i].argomento.Equals("TIPO_OGGETTO"))
                {
                    if (objListaFiltri[i].valore.Equals("F"))
                        return "A.ID_PROJECT";
                    else
                        return "A.ID_PROFILE";
                }
            }
            return "A.ID_PROFILE";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objListaFiltri"></param>
        /// <returns></returns>
        private string getQueryOggettiCompletati(DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            string col = getNomeColonnaOggetto(objListaFiltri);
            /*string queryStr = 
                "SELECT " + col + " FROM DPA_TRASMISSIONE A, DPA_TRASM_SINGOLA B, DPA_RAGIONE_TRASM D " +
                "WHERE A.SYSTEM_ID=B.ID_TRASMISSIONE AND B.ID_RAGIONE=D.SYSTEM_ID AND D.CHA_TIPO_RISPOSTA='C'";
            */
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASMISSIONE__TRASM_SINGOLA__RAGIONE_TRASM");

            q.setParam("param1", col);

            string queryString = q.getSQL();

            return queryString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objListaFiltri"></param>
        /// <returns></returns>
        private bool cercaInf(DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            if (objListaFiltri != null)
            {
                for (int i = 0; i < objListaFiltri.Length; i++)
                {
                    if (objListaFiltri[i].argomento.Equals("NO_CERCA_INFERIORI") && objListaFiltri[i].valore.ToUpper().Equals("TRUE"))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Reperimento condizioni di visibilità per le trasmissioni ricevute
        /// </summary>
        /// <param name="queryWhere"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objListaFiltri"></param>
        private void getCondizioniVisibilitaRicevute(ref string queryWhere, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            string filter = string.Empty;

            // Reperimento idcorrispondente
            string idCorrispondente = getQueryEffMet1(objUtente.idPeople);

            Boolean a_me_stesso = false;
            Boolean al_mio_ruolo = false;
            String ruolo_sottoposto = null;
            String persona_sottoposta = null;
            string aggiungiUtRuoloInit = null;
            string aggiungiUtRuoloFine = null;
            bool tab_trasmissioni = false;
            String filterHistoricized = null;

            DocsPaVO.filtri.FiltroRicerca f;
            if (objListaFiltri != null)
            {
                for (int i = 0; i < objListaFiltri.Length; i++)
                {
                    f = objListaFiltri[i];
                    if (f.argomento.Equals("A_ME_STESSO"))
                    {
                        a_me_stesso = true;
                    }
                    if (f.argomento.Equals("AL_MIO_RUOLO"))
                    {
                        al_mio_ruolo = true;
                    }
                    if (f.argomento.Equals("RUOLO_SOTTOPOSTO"))
                    {
                        ruolo_sottoposto = f.valore;
                    }
                    if (f.argomento.Equals("PERSONA_SOTTOPOSTA"))
                    {
                        persona_sottoposta = f.valore;
                    }
                    if (f.argomento.Equals("TAB_TRASMISSIONI"))
                    {
                        tab_trasmissioni = true;
                    }

                }
            }

            if (tab_trasmissioni == false)
            {
                // Filtro che indica di reperire le trasmissioni visibili all'utente
                if (a_me_stesso == true && al_mio_ruolo == false)
                {
                    aggiungiUtRuoloInit = "(";
                    aggiungiUtRuoloFine = " AND B.CHA_TIPO_DEST = 'U') ";

                    filter = string.Format("(C.ID_PEOPLE = {0} OR B.ID_CORR_GLOBALE = {1})", objUtente.idPeople, idCorrispondente);
                    filter = aggiungiUtRuoloInit + filter + aggiungiUtRuoloFine;
                }

                FiltroRicerca extendToHist = objListaFiltri.Where(e => e.argomento == listaArgomentiNascosti.RUOLO_EXTEND_TO_HISTORICIZED.ToString()).FirstOrDefault();

                if (extendToHist != null && Convert.ToBoolean(extendToHist.valore))
                {
                    Query q = null;
                    Query sqlDb = null;

                    if (dbType == "SQL")
                    {
                        q = InitQuery.getInstance().getQuery("S_GET_ROLE_CHAIN_ID_CORR_GLOBALI_IN_CLAUSOLE");

                        sqlDb = InitQuery.getInstance().getQuery("S_GET_ROLE_CHAIN_ID_CORR_GLOBALI_OUT_CLAUSOLE");
                        sqlDb.setParam("idCorrGlob", ruolo_sottoposto);
                        chaiTableDef = sqlDb.getSQL();

                    }
                    else
                    {
                        q = InitQuery.getInstance().getQuery("S_GET_ROLE_CHAIN_ID_CORR_GLOBALI");
                        q.setParam("idCorrGlob", ruolo_sottoposto);
                    }

                    filterHistoricized = String.Format(" IN ({0})", q.getSQL());
                }
                else
                    filterHistoricized = " = " + ruolo_sottoposto;


                // Filtro che indica di reperire le trasmissioni visibili solo al ruolo
                if (a_me_stesso == false && al_mio_ruolo == true)
                {
                    if (persona_sottoposta.Equals("tutti")) //Tutti gli utenti del ruolo
                    {
                        this.eliminaTramissioniMultiple = true;

                        filter = string.Format("(B.ID_CORR_GLOBALE {2} AND B.CHA_TIPO_DEST = 'R')", objUtente.idPeople, idCorrispondente, filterHistoricized);

                    }
                    else
                    {
                        if (persona_sottoposta.Equals("altri")) //Solo gli altri utenti tranne me
                        {
                            this.eliminaTramissioniMultiple = true;
                            filter = string.Format("(B.ID_CORR_GLOBALE {2} AND B.CHA_TIPO_DEST = 'R' AND NOT EXISTS (SELECT f.system_id FROM dpa_trasm_singola f, dpa_trasm_utente p WHERE p.id_people = {0} AND p.id_trasm_singola = b.system_id AND b.id_corr_globale {2}))", objUtente.idPeople, idCorrispondente, filterHistoricized);
                        }
                        else //singolo utente selezionato
                        {
                            this.eliminaTramissioniMultiple = true;
                            filter = string.Format("(B.ID_CORR_GLOBALE {2} AND C.ID_PEOPLE = {3} AND B.CHA_TIPO_DEST = 'R')", objUtente.idPeople, idCorrispondente, filterHistoricized, persona_sottoposta);
                        }
                    }
                }

                // Filtro che indica di reperire le trasmissioni visibili all'utente e al ruolo
                if (a_me_stesso == true && al_mio_ruolo == true)
                {
                    if (persona_sottoposta != null)
                    {
                        if (persona_sottoposta.Equals("tutti")) //Tutti gli utenti del ruolo
                        {
                            this.eliminaTramissioniMultiple = true;
                            //filter = string.Format("(( (C.ID_PEOPLE = {0} OR B.ID_CORR_GLOBALE = {1}) AND B.CHA_TIPO_DEST = 'U') OR B.ID_CORR_GLOBALE {2} AND B.CHA_TIPO_DEST = 'R')", objUtente.idPeople, idCorrispondente, filterHistoricized);
                            filter = string.Format("(( B.ID_CORR_GLOBALE = {1} AND B.CHA_TIPO_DEST = 'U') OR B.ID_CORR_GLOBALE {2} AND B.CHA_TIPO_DEST = 'R')", objUtente.idPeople, idCorrispondente, filterHistoricized);
                        }
                        else
                        {
                            if (persona_sottoposta.Equals("altri")) //Solo gli altri utenti tranne me
                            {
                                this.eliminaTramissioniMultiple = true;
                                //filter = string.Format("(( (C.ID_PEOPLE = {0} OR B.ID_CORR_GLOBALE = {1}) AND B.CHA_TIPO_DEST = 'U') OR (B.ID_CORR_GLOBALE {2} AND C.ID_PEOPLE NOT IN {0} AND B.CHA_TIPO_DEST = 'R') AND NOT EXISTS (SELECT f.system_id FROM dpa_trasm_singola f, dpa_trasm_utente p WHERE p.id_people = {0} AND p.id_trasm_singola = b.system_id AND b.id_corr_globale {2}))", objUtente.idPeople, idCorrispondente, filterHistoricized);
                                filter = string.Format("((B.ID_CORR_GLOBALE = {1} AND B.CHA_TIPO_DEST = 'U') OR (B.ID_CORR_GLOBALE {2} AND C.ID_PEOPLE NOT IN {0} AND B.CHA_TIPO_DEST = 'R') AND NOT EXISTS (SELECT f.system_id FROM dpa_trasm_singola f, dpa_trasm_utente p WHERE p.id_people = {0} AND p.id_trasm_singola = b.system_id AND b.id_corr_globale {2}))", objUtente.idPeople, idCorrispondente, filterHistoricized);
                            }
                            else //singolo utente selezionato
                            {
                                this.eliminaTramissioniMultiple = true;
                                //filter = string.Format("(( (C.ID_PEOPLE = {0} OR B.ID_CORR_GLOBALE = {1}) AND B.CHA_TIPO_DEST = 'U') OR (B.ID_CORR_GLOBALE {2} AND C.ID_PEOPLE = {3}))", objUtente.idPeople, idCorrispondente, filterHistoricized, persona_sottoposta);
                                filter = string.Format("((B.ID_CORR_GLOBALE = {1} AND B.CHA_TIPO_DEST = 'U') OR (B.ID_CORR_GLOBALE {2} AND C.ID_PEOPLE = {3}))", objUtente.idPeople, idCorrispondente, filterHistoricized, persona_sottoposta);
                            }
                        }
                    }
                    else
                    {
                        this.eliminaTramissioniMultiple = true;
                        filter = string.Format("(( (C.ID_PEOPLE = {0} OR B.ID_CORR_GLOBALE = {1}) AND B.CHA_TIPO_DEST = 'U') OR (B.ID_CORR_GLOBALE = {2} AND C.ID_PEOPLE = {3}))", objUtente.idPeople, idCorrispondente, objRuolo.systemId, objUtente.idPeople);
                    }
                }


                //MODIFICA

                // Indica se cercare o meno le trasmissioni ricevute dai ruoli inferiori
                bool cercaInferiori = cercaInf(objListaFiltri);

                if (cercaInferiori)
                {
                    this.eliminaTramissioniMultiple = true;

                    // Se cerco solo i sottoposti al ruolo corrente
                    if (al_mio_ruolo == false && a_me_stesso == false)
                    {
                        string ruoliInf = getListaRuoliInf(objOggettoTrasmesso, objRuolo, cercaInferiori);
                        if (!(ruoliInf.Equals("()")))
                        {
                            filter = string.Format("( B.ID_CORR_GLOBALE IN {0}", ruoliInf);
                        }
                        else
                        {
                            //Per non trovare niente se non ho ruoli sottoposti e si seleziona il flag
                            filter = " a.system_id is null ";
                        }
                    }
                    else
                    {
                        DocsPaVO.utente.Ruolo newRuolo = new DocsPaVO.utente.Ruolo();
                        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                        if (al_mio_ruolo == false)
                        {
                            //Prendi il ruolo corrente
                            newRuolo = objRuolo;
                        }
                        else
                        {
                            //Prendi il ruolo selezionato
                            newRuolo = utenti.getRuoloById(ruolo_sottoposto);
                            newRuolo = utenti.GetRuoloByIdGruppo(newRuolo.idGruppo);
                        }
                        string ruoliInf = getListaRuoliInf(objOggettoTrasmesso, newRuolo, cercaInferiori);
                        if (!(ruoliInf.Equals("()")))
                        {
                            //ruoliInf.Substring(0, ruoliInf.Length - 1);
                            string aggiungi = string.Format("( B.ID_CORR_GLOBALE IN {0})", ruoliInf);
                            filter = "((" + filter + ") OR " + aggiungi;
                        }
                    }
                }
            }
            else
            {
                filter = string.Format("( (C.ID_PEOPLE = {0} AND B.ID_CORR_GLOBALE = {1}) OR B.ID_CORR_GLOBALE = {2})", objUtente.idPeople, objRuolo.systemId, idCorrispondente);
            }

            if (!queryWhere.Trim().StartsWith(" AND "))
            {
                queryWhere += " AND ";
            }

            queryWhere += filter;

        }

        /// <summary>
        /// Reperimento condizioni di visibilità per le trasmissioni effettuate
        /// </summary>
        /// <param name="queryWhere"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objListaFiltri"></param>
        private void getCondizioniVisibilitaEffettuate(ref string queryWhere, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            bool cercaInferiori = cercaInf(objListaFiltri);
            string ruolo_sottoposto = string.Empty;
            string persona_sottoposta = string.Empty;
            string and = "";
            bool al_mio_ruolo = false;
            String filterHistoricized = null;

            string idCorrispondente = getQueryEffMet1(objUtente.idPeople);

            if (!queryWhere.Trim().StartsWith(" AND "))
            {
                and = " AND ";
            }

            DocsPaVO.filtri.FiltroRicerca f;
            for (int i = 0; i < objListaFiltri.Length; i++)
            {
                f = objListaFiltri[i];
                if (f.argomento.Equals("RUOLO_SOTTOPOSTO"))
                {
                    ruolo_sottoposto = f.valore;
                }
                if (f.argomento.Equals("PERSONA_SOTTOPOSTA"))
                {
                    persona_sottoposta = f.valore;
                }
                if (f.argomento.Equals("AL_MIO_RUOLO"))
                {
                    al_mio_ruolo = true;
                }
            }

            FiltroRicerca extendToHist = objListaFiltri.Where(e => e.argomento == listaArgomentiNascosti.RUOLO_EXTEND_TO_HISTORICIZED.ToString()).FirstOrDefault();

            if (extendToHist != null && Convert.ToBoolean(extendToHist.valore))
            {
                Query q = InitQuery.getInstance().getQuery("S_GET_ROLE_CHAIN_ID_CORR_GLOBALI");
                q.setParam("idCorrGlob", ruolo_sottoposto);
                filterHistoricized = String.Format(" IN ({0})", q.getSQL());
            }
            else
                filterHistoricized = " = " + ruolo_sottoposto;



            // Filtro che indica di reperire le trasmissioni visibili al ruolo
            if (persona_sottoposta.Equals("tutti")) //Tutti gli utenti del ruolo
            {
                queryWhere += string.Format("(A.ID_RUOLO_IN_UO {2})", objUtente.idPeople, idCorrispondente, filterHistoricized);
            }
            else
            {
                if (persona_sottoposta.Equals("altri")) //Solo gli altri utenti tranne me
                {
                    queryWhere += string.Format("(A.ID_RUOLO_IN_UO {2} AND A.ID_PEOPLE NOT IN {0})", objUtente.idPeople, idCorrispondente, filterHistoricized);
                }
                else //singolo utente selezionato
                {
                    queryWhere += string.Format("(A.ID_RUOLO_IN_UO {2} AND A.ID_PEOPLE = {3})", objUtente.idPeople, idCorrispondente, filterHistoricized, persona_sottoposta);
                }
            }

            // Se cerco solo i sottoposti al ruolo corrente
            if (al_mio_ruolo == false)
            {
                string ruoliInf = getListaRuoliInf(objOggettoTrasmesso, objRuolo, cercaInferiori);
                if (!(ruoliInf.Equals("()")))
                {
                    queryWhere = string.Format("( A.ID_RUOLO_IN_UO IN {0}", ruoliInf);
                }
                else
                {
                    //Per non trovare niente se non ho ruoli sottoposti e si seleziona il flag
                    queryWhere += " a.system_id is null ";
                }
            }
            else
            {
                DocsPaVO.utente.Ruolo newRuolo = new DocsPaVO.utente.Ruolo();
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                if (al_mio_ruolo == false)
                {
                    //Prendi il ruolo corrente
                    newRuolo = objRuolo;
                }
                else
                {
                    //Prendi il ruolo selezionato
                    newRuolo = utenti.getRuoloById(ruolo_sottoposto);
                    newRuolo = utenti.GetRuoloByIdGruppo(newRuolo.idGruppo);
                }
                string ruoliInf = getListaRuoliInf(objOggettoTrasmesso, newRuolo, cercaInferiori);
                if (!(ruoliInf.Equals("()")))
                {
                    //ruoliInf.Substring(0, ruoliInf.Length - 1);
                    string aggiungi = string.Format("( A.ID_RUOLO_IN_UO IN {0})", ruoliInf);
                    queryWhere = "((" + queryWhere + ") OR " + aggiungi;
                }
            }

            queryWhere = and + queryWhere;

            //queryWhere += ")";
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objRuolo"></param>
        /// <param name="cercaInf"></param>
        /// <returns></returns>
        private string getListaRuoliInf(DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Ruolo objRuolo, bool cercaInf)
        {
            logger.Debug("getListaRuoliInf");
            string ret = "(";
            int i;
            logger.Debug("cercaInf = " + cercaInf);

            if (cercaInf)
            {
                ArrayList listaRuoliInf;

                DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();

                if (objOggettoTrasmesso != null && objOggettoTrasmesso.infoDocumento != null)
                    listaRuoliInf = gerarchia.getGerarchiaInf(objRuolo, objOggettoTrasmesso.infoDocumento.idRegistro, null, DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO);
                else if (objOggettoTrasmesso != null && objOggettoTrasmesso.infoFascicolo != null)
                    listaRuoliInf = gerarchia.getGerarchiaInf(objRuolo, objOggettoTrasmesso.infoFascicolo.idRegistro, objOggettoTrasmesso.infoFascicolo.idClassificazione, DocsPaVO.trasmissione.TipoOggetto.FASCICOLO);
                else
                    listaRuoliInf = gerarchia.getGerarchiaInf(objRuolo, null, null, DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO);

                if (listaRuoliInf == null)
                {
                    logger.Debug("Errore nella gestione delle trasmissioni (Query - getListaRuoliInf)");
                    throw new Exception("Errore in : getListaRuoliInf");
                }
                for (i = 0; i < listaRuoliInf.Count; i++)
                {
                    ret = ret + ((DocsPaVO.utente.Ruolo)listaRuoliInf[i]).systemId;
                    if (i < listaRuoliInf.Count - 1)
                    {
                        if (i % 998 == 0 && i > 0)
                        {
                            //queryString=queryString+") OR A.ID_UO IN (";
                            ret = ret + ") OR A.ID_RUOLO_IN_UO IN (";
                        }
                        else
                        {
                            ret += ", ";
                        }
                    }
                    else
                    {
                        ret += ")";
                    }
                }

            }
            ret += ")";
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repeatQuery"></param>
        /// <param name="dataSet"></param>
        /// <param name="idPeople"></param>
        /// <param name="systemId"></param>
        /// <param name="objListaFiltri"></param>
        public void getQueryRicMet(ref bool repeatQuery, out DataSet dataSet, string idPeople, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso)
        {
            //			string queryWhere = "";
            //			string queryFrom = "";
            //			string queryColumn = "";			
            //			getQueryTrasmissioni(objOggettoTrasmesso, ref queryWhere, ref queryFrom, ref queryColumn);
            //			string idCorrispondente = getQueryEffMet1(idPeople);
            //
            //			queryWhere +=
            //				" AND C.CHA_VALIDA='1' AND A.DTA_INVIO IS NOT NULL " +
            //				"AND ((B.ID_CORR_GLOBALE = " + ruolo.systemId + " AND C.ID_PEOPLE=" + idPeople + ") OR B.ID_CORR_GLOBALE=" + idCorrispondente; 
            //			

            //			queryWhere += ")";
            //			
            //			//DocsPaUtils.Query q = QueryTrasm(ref queryWhere,objOggettoTrasmesso);
            //			repeatQuery = getCondFiltri(ref queryWhere, ref queryFrom, objListaFiltri);
            //			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASMISSIONE__TRASM_SINGOLA__TRASM_UTENTE2");
            //			q.setParam("param1", queryColumn);
            //			q.setParam("param2", queryFrom);			
            //			q.setParam("param3",queryWhere);
            //			
            //			string queryString = q.getSQL();
            //
            //			logger.Debug(queryString);

            string queryString = this.getDefinitionQueryRicevute(ref repeatQuery, idPeople, ruolo, objListaFiltri, objOggettoTrasmesso);

            logger.Debug(queryString);

            this.ExecuteQuery(out dataSet, "TRASMISSIONI", queryString);
        }

        /// <summary>
        /// Preparazione stringa di query per le trasmissioni ricevute
        /// </summary>
        /// <param name="repeatQuery"></param>
        /// <param name="idPeople"></param>
        /// <param name="ruolo"></param>
        /// <param name="objListaFiltri"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <returns></returns>
        private string getDefinitionQueryRicevute(ref bool repeatQuery,
                                                string idPeople,
                                                DocsPaVO.utente.Ruolo ruolo,
                                                DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso)
        {
            string queryWhere = "";
            string queryFrom = "";
            string queryColumn = "";
            getQueryTrasmissioni(objOggettoTrasmesso, ref queryWhere, ref queryFrom, ref queryColumn);
            string idCorrispondente = getQueryEffMet1(idPeople);

            //luluciani 3.7.7: export in excel dava meno dati della ricerca ricevute.
            //queryWhere +=
            //    " AND C.CHA_VALIDA='1' AND A.DTA_INVIO IS NOT NULL " +
            //    "AND ((B.ID_CORR_GLOBALE = " + ruolo.systemId + " AND C.ID_PEOPLE=" + idPeople + ") OR B.ID_CORR_GLOBALE=" + idCorrispondente; 


            DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
            utente.idPeople = idPeople;
            queryWhere +=
                " AND A.DTA_INVIO IS NOT NULL ";

            if (utente != null && ruolo != null)
                getCondizioniVisibilitaRicevute(ref queryWhere, objOggettoTrasmesso, utente, ruolo, objListaFiltri);

            //    "AND ((B.ID_CORR_GLOBALE = " + ruolo.systemId + " AND C.ID_PEOPLE=" + idPeople + ") OR B.ID_CORR_GLOBALE=" + idCorrispondente;

            repeatQuery = getCondFiltri(ref queryWhere, ref queryFrom, objListaFiltri);





            //PER ORA COMMENTO sab - era stato messo per la ricerca ai ruoli sottoposti anche per le trasm ricevute--
            //inf = false;
            //			inf = cercaInf(objListaFiltri);	
            //			string ruoliInf = getListaRuoliInf(objOggettoTrasmesso, ruolo, inf);
            //			if(inf && !(ruoliInf.Equals("()"))) 
            //			{
            //				queryWhere += " OR (B.ID_CORR_GLOBALE IN " + ruoliInf + " AND B.CHA_TIPO_DEST = 'R') ";
            //			}

            //  queryWhere += ")";

            //DocsPaUtils.Query q = QueryTrasm(ref queryWhere,objOggettoTrasmesso);
            // repeatQuery = getCondFiltri(ref queryWhere, ref queryFrom, objListaFiltri);
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASMISSIONE__TRASM_SINGOLA__TRASM_UTENTE3");

            string UserDB = string.Empty;

            if (dbType.ToUpper() == "SQL")
            {
                UserDB = getUserDB();
                q.setParam("dbuser", UserDB);
            }

            q.setParam("param1", queryColumn);
            q.setParam("param2", queryFrom);
            q.setParam("param3", queryWhere);

            #region Ordinamento

            // Recupero dei filtri di ricerca relarivi all'ordinamento
            FiltroRicerca oracleField = objListaFiltri.Where(e => e.argomento == "ORACLE_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca sqlField = objListaFiltri.Where(e => e.argomento == "SQL_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca profilationField = objListaFiltri.Where(e => e.argomento == "PROFILATION_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca orderDirection = objListaFiltri.Where(e => e.argomento == "ORDER_DIRECTION").FirstOrDefault();

            // Se non è valorizzata la direzione di ordinamento, viene creato un filtro di tipo ORDER_DIRECTION
            if (orderDirection == null)
            {
                orderDirection = new FiltroRicerca()
                {
                    argomento = "ORDER_DIRECTION",
                    valore = "DESC"

                };

            }

            // Function da utilizzare per estrarre i valori del campo profilato da utilizzare per l'ordinamento
            String extractFieldValue = String.Empty, orderCondition = String.Empty;

            if (this.dbType == "SQL")
            {
                // DB SQL Server
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", Convert(int, @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0})) AS CUSTOM_FIELD", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0}) AS CUSTOM_FIELD", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("CUSTOM_FIELD {0}", orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (sqlField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Format(", {0} AS var_desc_ragione", sqlField.valore);
                        orderCondition = String.Format("var_desc_ragione {0}", orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("Convert(datetime, A.DTA_INVIO) {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }

            }
            else
            {
                // DB ORACLE
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", to_number(getValCampoProfDoc(A.DOCNUMBER, {0}))", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", getValCampoProfDoc(A.DOCNUMBER, {0})", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("getValCampoProfDoc(A.DOCNUMBER, {0}) {1}", profilationField.valore, orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (oracleField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("{0} {1}", oracleField.valore, orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("TO_DATE (a.dta_invio, 'dd/MM/yyyy') {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }
            }

            // Impostazione del parametro di ordinamento
            q.setParam("orderCondition", orderCondition);

            #endregion

            string queryStringAll = q.getSQL();

            if (dbType == "SQL" && !string.IsNullOrEmpty(chaiTableDef))
            {
                queryStringAll = chaiTableDef + queryStringAll;
                chaiTableDef = string.Empty;
            }

            return queryStringAll;
        }
        //////
        //////		public void getQueryRicMet(ref bool repeatQuery, 
        //////									out DataSet dataSet, 
        //////									string idPeople, 
        //////									DocsPaVO.utente.Ruolo ruolo, 
        //////									DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
        //////									int pageNumber,
        //////									out int totalPageNumber,
        //////									out int recordCount)
        //////		{
        //////			string queryString=this.getDefinitionQueryRicevute(ref repeatQuery,idPeople,ruolo,objListaFiltri,objOggettoTrasmesso);
        //////
        //////			logger.Debug(queryString);
        //////
        //////			this.ExecutePaging(out dataSet,out totalPageNumber,out recordCount,pageNumber,20,queryString,"");
        //////
        //////			dataSet.Tables[0].TableName="TRASMISSIONI";
        //////		}

        internal void getQueryTrasmissioni(DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, ref string queryWhere, ref string queryFrom, ref string queryColumn)
        {
            // trasmissioni
            queryColumn +=
                "A.ID_RUOLO_IN_UO, A.ID_PEOPLE, A.CHA_TIPO_OGGETTO, " +
                "A.ID_PROFILE, A.ID_PROJECT, " +
                DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO", false) + " AS DTA_INVIO_F, " +
                "A.VAR_NOTE_GENERALI, A.CHA_SALVATA_CON_CESSIONE, ";
            // trasmissioni singole
            queryColumn +=
                "B.ID_RAGIONE, B.ID_TRASMISSIONE, B.ID_TRASM_UTENTE, B.CHA_TIPO_DEST, " +
                "B.ID_CORR_GLOBALE, B.VAR_NOTE_SING, B.CHA_TIPO_TRASM, " +
                DocsPaDbManagement.Functions.Functions.ToChar("B.DTA_SCADENZA", false) + " AS DTA_SCADENZA, B.HIDE_DOC_VERSIONS,";
            // trasmissioni utente
            queryColumn +=
                "C.SYSTEM_ID AS ID_TRASMISSIONE_UTENTE, C.ID_PEOPLE AS ID_DESTINATARIO, " +
                DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_VISTA", false) + " AS DTA_VISTA,CHA_VISTA, " +
                DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_ACCETTATA", false) + " AS DTA_ACCETTATA,CHA_ACCETTATA, " +
                DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_RIFIUTATA", false) + " AS DTA_RIFIUTATA,CHA_RIFIUTATA, " +
                "C.VAR_NOTE_RIF, C.VAR_NOTE_ACC, C.ID_TRASM_SINGOLA, C.CHA_VALIDA, C.DTA_RIMOZIONE_TODOLIST AS DTA_RIMOSSA_TDL, A.ID_PEOPLE_DELEGATO, " +
                " A.ID_PEOPLE_DELEGATO AS ID_PEOPLE_DELEGATO, C.ID_PEOPLE_DELEGATO AS DELEGATO_UTENTE, C.CHA_ACCETTATA_DELEGATO AS ACCETTATA_DELEGATO, C.CHA_VISTA_DELEGATO AS VISTA_DELEGATO, C.CHA_RIFIUTATA_DELEGATO AS RIFIUTATA_DELEGATO";

            queryWhere =
                "A.SYSTEM_ID=B.ID_TRASMISSIONE AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";

            if (objOggettoTrasmesso == null)
                return;
            string whereStr = null;
            bool doc = false;

            // condizione sui documenti
            if (objOggettoTrasmesso.infoDocumento != null)
            {
                whereStr = "A.ID_PROFILE=" + objOggettoTrasmesso.infoDocumento.idProfile;
                doc = true;
            }

            //condizione sui fascicoli
            if (objOggettoTrasmesso.infoFascicolo != null)
            {
                if (doc) whereStr += " OR ";
                whereStr += "A.ID_PROJECT=" + objOggettoTrasmesso.infoFascicolo.idFascicolo;
            }

            if (whereStr != null)
            {
                queryWhere += " AND ";
                if (doc)
                {
                    queryWhere += "(";
                }
                queryWhere += whereStr;
                if (doc)
                {
                    queryWhere += ") ";
                }
            }
        }

        internal void getQueryTrasmissioniWithoutTrasmUtente(DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, ref string queryWhere, ref string queryFrom, ref string queryColumn)
        {
            // trasmissioni
            queryColumn +=
                "A.ID_RUOLO_IN_UO, A.ID_PEOPLE, A.CHA_TIPO_OGGETTO, " +
                "A.ID_PROFILE, A.ID_PROJECT, " +
                DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO", false) + " AS DTA_INVIO_F, " +
                "A.VAR_NOTE_GENERALI, A.CHA_SALVATA_CON_CESSIONE, A.ID_PEOPLE_DELEGATO AS ID_PEOPLE_DELEGATO,";
            // trasmissioni singole
            queryColumn +=
                "B.ID_RAGIONE, B.ID_TRASMISSIONE, B.ID_TRASM_UTENTE, B.SYSTEM_ID AS ID_TRASM_SINGOLA,  B.CHA_TIPO_DEST, " +
                "B.ID_CORR_GLOBALE, B.VAR_NOTE_SING, B.CHA_TIPO_TRASM, " +
                DocsPaDbManagement.Functions.Functions.ToChar("B.DTA_SCADENZA", false) + " AS DTA_SCADENZA, B.HIDE_DOC_VERSIONS ";

            queryWhere =
                "A.SYSTEM_ID=B.ID_TRASMISSIONE AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";

            if (objOggettoTrasmesso == null)
                return;
            string whereStr = null;
            bool doc = false;

            // condizione sui documenti
            if (objOggettoTrasmesso.infoDocumento != null)
            {
                whereStr = "A.ID_PROFILE=" + objOggettoTrasmesso.infoDocumento.idProfile;
                doc = true;
            }

            //condizione sui fascicoli
            if (objOggettoTrasmesso.infoFascicolo != null)
            {
                if (doc) whereStr += " OR ";
                whereStr += "A.ID_PROJECT=" + objOggettoTrasmesso.infoFascicolo.idFascicolo;
            }

            if (whereStr != null)
            {
                queryWhere += " AND ";
                if (doc)
                {
                    queryWhere += "(";
                }
                queryWhere += whereStr;
                if (doc)
                {
                    queryWhere += ") ";
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="repeatQuery"></param>
        /// <param name="dataSet"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objListaFiltri"></param>
        public void getQueryEffMetdef(ref bool repeatQuery, out DataSet dataSet, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            //string queryWhere = " AND ";
            string queryWhere = "";
            DocsPaUtils.Query q = QueryTrasmExport(ref queryWhere, objOggettoTrasmesso);

            getCondizioniVisibilitaEffettuate(ref queryWhere, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);

            string queryJoin = "";
            repeatQuery = getCondFiltri(ref queryWhere, ref queryJoin, objListaFiltri, objUtente, objRuolo);

            q.setParam("param6", queryWhere);
            q.setParam("param7", queryJoin);

            #region Ordinamento

            // Recupero dei filtri di ricerca relarivi all'ordinamento
            FiltroRicerca oracleField = null;
            FiltroRicerca sqlField = null;
            FiltroRicerca profilationField = null;
            FiltroRicerca orderDirection = null;
            if (objListaFiltri != null)
            {
                oracleField = objListaFiltri.Where(e => e.argomento == "ORACLE_FIELD_FOR_ORDER").FirstOrDefault();
                sqlField = objListaFiltri.Where(e => e.argomento == "SQL_FIELD_FOR_ORDER").FirstOrDefault();
                profilationField = objListaFiltri.Where(e => e.argomento == "PROFILATION_FIELD_FOR_ORDER").FirstOrDefault();
                orderDirection = objListaFiltri.Where(e => e.argomento == "ORDER_DIRECTION").FirstOrDefault();
            }
            // Se non è valorizzata la direzione di ordinamento, viene creato un filtro di tipo ORDER_DIRECTION
            if (orderDirection == null)
            {
                orderDirection = new FiltroRicerca()
                {
                    argomento = "ORDER_DIRECTION",
                    valore = "DESC"

                };

            }

            // Function da utilizzare per estrarre i valori del campo profilato da utilizzare per l'ordinamento
            String extractFieldValue = String.Empty, orderCondition = String.Empty;

            if (this.dbType == "SQL")
            {
                // DB SQL Server
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", Convert(int, @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0})) AS CUSTOM_FIELD", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0}) AS CUSTOM_FIELD", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("CUSTOM_FIELD {0}", orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (sqlField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Format(", {0} AS var_desc_ragione", sqlField.valore);
                        orderCondition = String.Format("var_desc_ragione {0}", orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("DTA_INVIO {0}", orderDirection.valore);
                    }
                }

            }
            else
            {
                // DB ORACLE
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", to_number(getValCampoProfDoc(A.DOCNUMBER, {0}))", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", getValCampoProfDoc(A.DOCNUMBER, {0})", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("getValCampoProfDoc(A.DOCNUMBER, {0}) {1}", profilationField.valore, orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (oracleField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("{0} {1}", oracleField.valore, orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("TO_DATE (a.dta_invio, 'dd/MM/yyyy') {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }
            }

            q.setParam("orderCondition", orderCondition);

            #endregion

            string queryString = q.getSQL();

            logger.Debug(queryString);

            this.ExecuteQuery(out dataSet, "TRASMISSIONI", queryString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryWhere"></param>
        /// <param name="dataSet"></param>
        /// <param name="objOggettoTrasmesso"></param>
        public void repeatQueryTrasm(ref DataSet dataSet, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, ArrayList idTrasmissioneList, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            string inStr = "(";
            for (int i = 0; i < idTrasmissioneList.Count; i++)
            {
                inStr = inStr + (string)idTrasmissioneList[i];
                if (i < idTrasmissioneList.Count - 1)
                {
                    if (i % 998 == 0 && i > 0)
                    {
                        //queryString=queryString+") OR A.ID_UO IN (";
                        inStr = inStr + ") OR B.ID_TRASMISSIONE IN (";
                    }
                    else
                    {
                        inStr += ", ";
                    }
                }
                else
                {
                    inStr += ")";
                }
            }

            dataSet.Tables.Remove("TRASMISSIONI");
            string queryWhere = " AND B.ID_TRASMISSIONE IN " + inStr;

            DocsPaUtils.Query q = QueryTrasm(ref queryWhere, objOggettoTrasmesso);
            string ragioneDescr = getSeFiltroRagione(objListaFiltri);
            if (ragioneDescr != "")
            {
                string userdb = DocsPaDbManagement.Functions.Functions.GetDbUserSession();
                if (userdb != "")
                    userdb = userdb + ".";
                queryWhere += queryWhere + " " + "AND " + userdb + "VARDESCRIBE(B.ID_RAGIONE,'RAGIONETRASM')='" + ragioneDescr.ToUpper() + "'";

            }
            q.setParam("param6", queryWhere);

            string queryString = q.getSQL();

            logger.Debug(queryString);

            this.ExecuteQuery(out dataSet, "TRASMISSIONI", queryString);
        }

        public string getSeFiltroRagione(DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            string rtn = string.Empty;
            for (int i = 0; i < objListaFiltri.Length; i++)
            {
                if (objListaFiltri[i].argomento == DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.RAGIONE.ToString())
                {
                    return rtn = objListaFiltri[i].valore.ToUpper();
                }
            }
            return rtn;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="idTrasmissione"></param>
        public void getTrasm(out DataSet dataSet, string idTrasmissione, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso)
        {
            string queryWhere = " AND A.SYSTEM_ID=" + idTrasmissione;

            DocsPaUtils.Query q = QueryTrasm(ref queryWhere, objOggettoTrasmesso);

            q.setParam("param6", queryWhere);
            //q.setParam("param2000",queryWhere);

            string queryString = q.getSQL();

            logger.Debug(queryString);

            this.ExecuteQuery(out dataSet, "TRASMISSIONI", queryString);
        }

        #endregion

        #region Query di RagioniManager.cs

        public ArrayList GetListRag(DocsPaVO.trasmissione.Diritti objDiritti, bool flgDaRicercaTrasm, bool sysExt=false)
        {
            System.Collections.ArrayList objListaRagioni = new System.Collections.ArrayList();
            //DocsPa_V15_Utils.database.SqlServerAgent db = new DocsPa_V15_Utils.database.SqlServerAgent();
            //db.openConnection();
            try
            {
                string queryString = "";
                if (flgDaRicercaTrasm)
                {
                    queryString = GetQueryTutteLeRagione();
                    if (objDiritti != null)
                        queryString += " WHERE ID_AMM = " + objDiritti.idAmministrazione;
                }
                else
                {
                    queryString = GetQueryRagione(null,sysExt);
                    //queryString += " AND (ID_AMM IS NULL ";
                    if (objDiritti != null)
                        //queryString += " OR ID_AMM = " + objDiritti.idAmministrazione;
                        queryString += " AND ID_AMM = " + objDiritti.idAmministrazione;
                    //queryString += ")";

                }


                try
                {
                    if (objDiritti != null && objDiritti.accessRights != null && objDiritti.accessRights.Equals("45"))
                    {
                        queryString += " AND CHA_TIPO_DIRITTI IN ('R', 'C') ";
                    }
                }
                catch
                {
                }

                // prima era così ora i PM richiedono order per var_NOTE
                //queryString += " ORDER BY VAR_DESC_RAGIONE ASC";
                queryString += " ORDER BY VAR_NOTE ASC";

                logger.Debug(queryString);
                /*
                IDataReader dr = db.executeReader(queryString); 
                while (dr.Read()) 
                {					
                    objListaRagioni.Add(getDatiRagione(dr));
                }
                dr.Close();
                */
                DataSet dataSet;

                this.ExecuteQuery(out dataSet, queryString);

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    objListaRagioni.Add(getDatiRagione(row));
                }
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                logger.Debug("Errore nella gestione delle trasmissioni (Query - GetListRag)", e);
                throw new Exception(e.Message);
            }
            // chiudo le connessioni
            //db.closeConnection();
            return objListaRagioni;
        }

        public ArrayList GetListRagATutti(DocsPaVO.trasmissione.Diritti objDiritti)
        {
            System.Collections.ArrayList objListaRagioni = new System.Collections.ArrayList();
            try
            {

                string queryString = GetQueryRagione(null);

                if (objDiritti != null)
                {
                    queryString += " AND ID_AMM = " + objDiritti.idAmministrazione;
                }

                queryString += " AND CHA_TIPO_DEST IN ('T', 'P') ";



                // prima era così ora i PM richiedono order per var_NOTE
                //queryString += " ORDER BY VAR_DESC_RAGIONE ASC";
                queryString += " ORDER BY VAR_NOTE ASC";

                logger.Debug(queryString);

                DataSet dataSet;

                this.ExecuteQuery(out dataSet, queryString);

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    objListaRagioni.Add(getDatiRagione(row));
                }
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                logger.Debug("Errore nella gestione delle trasmissioni (Query - GetListRag)", e);
                throw new Exception(e.Message);
            }
            // chiudo le connessioni
            //db.closeConnection();
            return objListaRagioni;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private DocsPaVO.trasmissione.RagioneTrasmissione getDatiRagione(DataRow dr)
        {
            DocsPaVO.trasmissione.RagioneTrasmissione objRagione = new DocsPaVO.trasmissione.RagioneTrasmissione();

            objRagione.systemId = dr[0].ToString();
            objRagione.descrizione = dr[1].ToString();
            objRagione.tipo = dr[2].ToString();
            objRagione.tipoDiritti = (DocsPaVO.trasmissione.TipoDiritto)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoDirittoStringa, dr[3].ToString());
            objRagione.risposta = dr[4].ToString();
            objRagione.tipoDestinatario = (DocsPaVO.trasmissione.TipoGerarchia)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa, dr[5].ToString());
            objRagione.note = dr[6].ToString();
            objRagione.eredita = dr[7].ToString();
            if (dr[8] != null)
            {
                objRagione.tipoRisposta = dr[8].ToString();
            }
            objRagione.notifica = dr[9].ToString();

            if (dr[10] != null)
                objRagione.testoMsgNotificaDoc = dr[10].ToString();

            if (dr[11] != null)
                objRagione.testoMsgNotificaFasc = dr[11].ToString();

            if (dr[12] != null)
                objRagione.prevedeCessione = dr[12].ToString();
            else
                objRagione.prevedeCessione = "N";
            if (dr[13] != null)
                objRagione.mantieniLettura = dr[13].ToString();
            else
                objRagione.mantieniLettura = "false";
            
            //
            // Mev Cessione Diritti - Mantieni Scrittura
            //if (dr[14] != null)
            if (dr.Table.Columns.Contains("CHA_MANTIENI_SCRITT"))
                objRagione.mantieniScrittura = dr[14].ToString();
            else
                objRagione.mantieniScrittura = "false";
            // End Mev Cessione Diritti - Mantieni Scrittura
            //
            
            return objRagione;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="objRagione"></param>
        public void GetRag(string queryString, ref DocsPaVO.trasmissione.RagioneTrasmissione objRagione)
        {
            DataSet dataSet;

            this.ExecuteQuery(out dataSet, queryString);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                objRagione = getDatiRagione(row);
            }
            if (dataSet != null && dataSet.Tables[0].Rows.Count == 0)
                objRagione = null;

            dataSet.Dispose();

        }

        public void getRagioneNotifica(string idAmm, ref DocsPaVO.trasmissione.RagioneTrasmissione objRagione)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPARagNotifica");
                q.setParam("idAmm", idAmm);
                string commandText = q.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getRagioneNotifica - QUERY : " + commandText);
                logger.Debug("SQL - getRagioneNotifica - QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    objRagione.systemId = ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString();
                    objRagione.descrizione = ds.Tables[0].Rows[0]["VAR_DESC_RAGIONE"].ToString();
                    objRagione.tipo = ds.Tables[0].Rows[0]["CHA_TIPO_RAGIONE"].ToString();
                    objRagione.tipoDiritti = (DocsPaVO.trasmissione.TipoDiritto)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoDirittoStringa, ds.Tables[0].Rows[0]["CHA_TIPO_DIRITTI"].ToString());
                    objRagione.risposta = ds.Tables[0].Rows[0]["CHA_RISPOSTA"].ToString();
                    objRagione.tipoDestinatario = (DocsPaVO.trasmissione.TipoGerarchia)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa, ds.Tables[0].Rows[0]["CHA_TIPO_DEST"].ToString());
                    objRagione.note = ds.Tables[0].Rows[0]["VAR_NOTE"].ToString();
                    objRagione.eredita = ds.Tables[0].Rows[0]["CHA_EREDITA"].ToString();
                    if (ds.Tables[0].Rows[0]["VAR_DESC_RAGIONE"].ToString() != null)
                    {
                        objRagione.tipoRisposta = ds.Tables[0].Rows[0]["CHA_TIPO_RISPOSTA"].ToString();
                    }
                    objRagione.notifica = ds.Tables[0].Rows[0]["VAR_NOTIFICA_TRASM"].ToString();
                    objRagione.prevedeCessione = ds.Tables[0].Rows[0]["CHA_CEDE_DIRITTI"].ToString();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public string GetQueryTutteLeRagione()
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_All_DPARagTrasm");
            return q.getSQL();
        }

        public string GetQueryRagione(string idRagione, bool sysExt =false)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPARagTrasm3");

            if (idRagione != null)
            {
                q.setParam("param1", " SYSTEM_ID = " + idRagione);
            }
            else if (!sysExt)
            {
                q.setParam("param1", " DPA_RAGIONE_TRASM.CHA_VIS = '1'");
            }
            else
            {
                q.setParam("param1", " DPA_RAGIONE_TRASM.CHA_VIS != '0'");
            }
            return q.getSQL();
        }

        public string GetQueryRagioneByCodice(string idAmm, string codice)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPARagTrasm3");

            q.setParam("param1", "VAR_DESC_RAGIONE = '" + codice + "' AND ID_AMM = " + idAmm);

            return q.getSQL();
        }

        public string GetQueryRagione(string tipoDest, string idAmm)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPARagTrasm5");
            q.setParam("param1", idAmm);
            q.setParam("param2", tipoDest);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            return queryString;
        }

        public DocsPaVO.trasmissione.RagioneTrasmissione GetRagioneByTipoOperazione(string tipoOperazione, string idAmm)
        {
            logger.Debug("Inizio Metodo GetRagioneByTipoOperazione in DocsPaDb.Query_DocsPAWS.LibroFirma");
            DocsPaVO.trasmissione.RagioneTrasmissione ragione = new DocsPaVO.trasmissione.RagioneTrasmissione();
            try
            {
                String query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_RAGIONE_TRASM_BY_PROC_RES");
                q.setParam("tipoOperazione", tipoOperazione);
                q.setParam("idAmm", idAmm);
                query = q.getSQL();
                logger.Debug("GetRagioneByTipoOperazione: " + query);

                if (this.ExecuteQuery(out ds, "ragione", query))
                {
                    if (ds.Tables["ragione"] != null && ds.Tables["ragione"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            ragione = getDatiRagione(row);
                        }
                    }
                }
                else
                {
                    throw new Exception("Errore durante l'estrazione della ragione di trasmissione: " + query);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetRagioneByTipoOperazione", ex);
                return null;
            }
            logger.Debug("Fine Metodo GetRagioneByTipoOperazione in DocsPaDb.Query_DocsPAWS.LibroFirma");
            return ragione;
        }

        #endregion

        #region Query di TrasmProtoIntManager.cs
        /// <summary>
        /// Controlla che un'unita' organizzativa abbia un ruolo di riferimento.
        /// </summary>
        /// <param name="uo"></param>
        /// <returns></returns>
        public bool UOHasReferenceRole(string idUO)
        {
            bool result = true;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("UO_HAS_REFERENCE_ROLE");
                q.setParam("param1", idUO);
                string queryString = q.getSQL();
                DataSet dataSet;
                ExecuteQuery(out dataSet, queryString);

                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                {
                    result = false;
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        #endregion

        #region verifica presenza ragioni di trasmissione TO, CC, REFERENTE
        /// <summary>
        /// Verifica se esistono le ragioni di trasmissione per i protocolli interni
        /// </summary>
        /// <param name="idAmm">System_id dell'amministrazione</param>
        /// <param name="isEnableRef"> booleano che identifica se è stata abilitata la gestione
        /// dell'ufficio referente</param>
        /// <returns>bool</returns>
        public bool VerificaRagProtInt(string idAmm, bool isEnableRef)
        {
            bool result = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPARagTrasm4");
            q.setParam("param1", idAmm);

            string queryString = q.getSQL();
            if (isEnableRef)
            {
                queryString = queryString + " OR (UPPER(a.VAR_RAGIONE_REFERENTE) = UPPER(b.VAR_DESC_RAGIONE))";
            }
            queryString = queryString + ")";
            logger.Debug(queryString);
            string count;
            ExecuteScalar(out count, queryString);

            // la query deve trovare 2 record di ragioni di trasmissione!
            if (isEnableRef == false)
            {
                if (count == "2") result = true;
            }

            if (isEnableRef == true)
            {
                if (count == "3") result = true;
            }
            return result;

        }

        public bool VerificaRagProtIntBis(string idAmm, bool isEnableRef, string tipoProto, bool destTO, bool destCC, out string message)
        {
            bool result = false;
            message = "";
            string ragioneTO = "";
            string ragioneCC = "";
            string ragReferente = "";

            DataSet dataset = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPARagTrasm4Bis");
            q.setParam("param1", idAmm);

            if (isEnableRef)
            {
                q.setParam("param2", " , ID_RAGIONE_REFERENTE");
            }

            string queryString = q.getSQL();

            if (isEnableRef)
            {
                queryString = queryString + " OR (a.ID_RAGIONE_REFERENTE = b.SYSTEM_ID)";
            }
            queryString = queryString + ")";
            logger.Debug(queryString);

            ExecuteQuery(dataset, "RAGIONE_TRASM", queryString);
            /* La protocollazione in INGRESSO trasmette solo all'ufficio referente se abilitato*/
            if (tipoProto.Equals("A"))
            {
                if (dataset != null && dataset.Tables[0].Rows.Count > 0)
                {
                    if (isEnableRef == true)	//se è abilitato l'ufficio referente
                    {
                        ragReferente = dataset.Tables[0].Rows[0]["ID_RAGIONE_REFERENTE"].ToString();
                        if (ragReferente == null || ragReferente.Equals(""))
                        {
                            message = "UFFICIO REFERENTE";
                            return false;
                        }
                        result = true;
                    }

                }
                else
                {
                    message = "UFFICIO REFERENTE";
                }
            }
            /* La protocollazione in USCITA trasmette:
             * - ai destinatari in TO;
             * - ai destinatari in CC;
             * - all'ufficio referente se abilitato */
            if (tipoProto.Equals("P") || tipoProto.Equals("I"))
            {

                if (dataset != null && dataset.Tables[0].Rows.Count > 0)
                {
                    if (destTO)//se sono specificati i dest in TO
                    {
                        ragioneTO = dataset.Tables[0].Rows[0]["ID_RAGIONE_TO"].ToString();
                        if (ragioneTO == null || ragioneTO == "") //Se la ragione in TO è assente
                        {
                            message = "DESTINATARI IN TO";

                            if (destCC)//se sono specificati i dest in CC
                            {
                                ragioneCC = dataset.Tables[0].Rows[0]["ID_RAGIONE_CC"].ToString();
                                if (ragioneCC == null || ragioneCC == "") //Se la ragione in CC è assente
                                {
                                    if (message != "")
                                        message = message + " , ";
                                    message = message + "DESTINATARI IN CC";
                                }
                            }
                            if (isEnableRef == true)	//se è specificato l'ufficio referente
                            {
                                ragReferente = dataset.Tables[0].Rows[0]["ID_RAGIONE_REFERENTE"].ToString();
                                if (ragReferente == null || ragReferente.Equals("")) //Se la ragione REFERENTE è assente
                                {
                                    if (message != "")
                                        message = message + " , ";
                                    message = message + "UFFICIO REFERENTE";
                                }
                            }
                            return false;
                        }
                    }
                    if (destCC)//se sono specificati i dest in CC
                    {
                        ragioneCC = dataset.Tables[0].Rows[0]["ID_RAGIONE_CC"].ToString();
                        if (ragioneCC == null || ragioneCC == "") //Se la ragione in CC è assente
                        {
                            if (message != "")
                            {
                                message = message + " , ";
                            }
                            message = message + "DESTINATARI IN CC";
                            if (isEnableRef == true)	//se è specificato l'ufficio referente
                            {
                                ragReferente = dataset.Tables[0].Rows[0]["ID_RAGIONE_REFERENTE"].ToString();
                                if (ragReferente == null || ragReferente.Equals("")) //Se la ragione REFERENTE è assente
                                {
                                    if (message != "")
                                        message = message + " , ";
                                    message = message + "UFFICIO REFERENTE";
                                }
                            }
                            return false;
                        }
                    }
                    if (isEnableRef == true)	//se è specificato l'ufficio referente
                    {
                        ragReferente = dataset.Tables[0].Rows[0]["ID_RAGIONE_REFERENTE"].ToString();
                        if (ragReferente == null || ragReferente.Equals("")) //Se la ragione REFERENTE è assente
                        {
                            message = "UFFICIO REFERENTE";
                            return false;
                        }
                    }
                    result = true;
                }
                else
                {
                    if (destTO)//se sono specificati i dest in TO
                    {
                        message = "DESTINATARI IN TO";
                    }
                    if (destCC)//se sono specificati i dest in CC
                    {
                        if (message != "")
                        {
                            message = message + " , ";
                        }
                        message = message + "DESTINATARI IN CC";
                    }
                    if (isEnableRef == true)	//se è specificato l'ufficio referente
                    {
                        if (message != "")
                            message = message + " , ";
                        message = message + "UFFICIO REFERENTE";
                    }
                }

            }

            return result;
        }

        /// <summary>
        /// Verifica se esistono le ragioni di trasmissione per la trasmissione 
        /// all'ufficio referente
        /// </summary>
        /// <param name="idAmm">System_id dell'amministrazione</param>
        /// <returns>bool</returns>
        public bool VerificaRagTrasmUffRef(string idAmm)
        {
            bool result = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPARagTrasm6");
            q.setParam("param1", idAmm);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            string count;
            ExecuteScalar(out count, queryString);

            // la query deve trovare 1 solo record (VAR_RAGIONE_REFERENTE)
            if (count != null && count != "")
                result = true;

            return result;
        }
        #endregion

        #region Query di TemplateTrasmManager.cs

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private static DocsPaVO.trasmissione.TemplateTrasmissione getDatiTemplate(DataRow dr)
        {
            DocsPaVO.trasmissione.TemplateTrasmissione template = new DocsPaVO.trasmissione.TemplateTrasmissione();
            template.systemId = dr[0].ToString();
            template.idTrasmissione = dr[1].ToString();
            template.descrizione = dr[2].ToString();
            return template;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="listaTemplate"></param>
        public void getListTempl(string queryString, ArrayList listaTemplate)
        {
            /*
                IDataReader dr = db.executeReader(queryString); 

                while (dr.Read()) 
                {					
                    listaTemplate.Add(getDatiTemplate(dr));
                }
				
                dr.Close();
            */
            DataSet dataSet;

            this.ExecuteQuery(out dataSet, queryString);

            if (dataSet != null)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    listaTemplate.Add(getDatiTemplate(row));
                }
                dataSet.Dispose();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public DocsPaVO.trasmissione.TemplateTrasmissione addTemplate(DocsPaVO.trasmissione.TemplateTrasmissione template)
        {
            logger.Debug("inserimentoTemplate");
            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
            //db.openConnection();		
            this.OpenConnection();
            string numPar = checkTemplate(template);
            if (!numPar.Equals("0"))
            {
                return null;
            }
            try
            {
                /*string insertString = 
                    "INSERT INTO DPA_TEMPL_TRASM " +
                    "(" + DocsPaWS.Utils.dbControl.getSystemIdColName() + " ID_TRASMISSIONE, VAR_TEMPLATE ) " +
                    " VALUES (" + DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_TEMPL_TRASM") +
                    template.idTrasmissione + ", '" + template.descrizione.Replace("'", "''") + "')";*/
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPATempltrasm");
                q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName() + " ID_TRASMISSIONE, VAR_TEMPLATE");
                //DocsPaWS.Utils.dbControl.getSystemIdColName() + " ID_TRASMISSIONE, VAR_TEMPLATE");
                q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_TEMPL_TRASM") +
                    template.idTrasmissione + ", '" + template.descrizione.Replace("'", "''") + "'");
                //DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_TEMPL_TRASM") +

                string sql = q.getSQL();
                logger.Debug(sql);
                string res;
                this.InsertLocked(out res, sql, "DPA_TEMPL_TRASM");
                template.systemId = res;

                //db.closeConnection();
                this.CloseConnection();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                this.CloseConnection();
                //throw new Exception("F_System");
                logger.Debug("Errore nella gestione delle trasmissioni (Query - addTemplate)", e);
                throw new Exception("F_System");
            }
            return template;
        }

        public string checkTemplate(DocsPaVO.trasmissione.TemplateTrasmissione template)
        {
            //si verifica se la parola chiave è già presente
            /*string selectString =
                "SELECT COUNT(*) FROM DPA_TEMPL_TRASM WHERE upper(VAR_TEMPLATE)='"+ template.descrizione.ToUpper() +"'" + 
                " AND ID_TRASMISSIONE = " + template.idTrasmissione;*/
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPATemplTrasm");
            q.setParam("param1", "upper(VAR_TEMPLATE)='" + template.descrizione.Replace("'", "''").ToUpper() + "'" +
                " AND ID_TRASMISSIONE = " + template.idTrasmissione);
            string sql = q.getSQL();
            logger.Debug(sql);
            string numPar;
            this.ExecuteScalar(out numPar, sql);
            return numPar;
        }


        public string getQueryTemplate(string idPeople, string idRuoloInUO, string tipo, string nomeTemplate)
        {
            /*string queryString =
                "SELECT A.SYSTEM_ID, A.ID_TRASMISSIONE, A.VAR_TEMPLATE FROM DPA_TEMPL_TRASM A, DPA_TRASMISSIONE B " +
                "WHERE A.ID_TRASMISSIONE = B.SYSTEM_ID " +
                "AND B.ID_PEOPLE = " + idPeople + " AND B.ID_RUOLO_IN_UO = " + idRuoloInUO ; */
            //per ora non gestiamo doc e fascicoli !!
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASMISSIONE__TEMPL_TRASM");
            string param = "B.ID_PEOPLE = " + idPeople + " AND B.ID_RUOLO_IN_UO = " + idRuoloInUO;
            if (tipo != null && !tipo.Equals(""))
            {
                param += " AND B.CHA_TIPO_OGGETTO ='" + tipo + "'";
            }

            if (nomeTemplate != null && !nomeTemplate.Equals(""))
            {
                param += " AND UPPER(A.VAR_TEMPLATE) ='" + nomeTemplate.Replace("'", "''").ToUpper() + "'";
            }
            q.setParam("param1", param);
            q.setParam("param2", "A.VAR_TEMPLATE");
            string queryString = q.getSQL();
            return queryString;
        }

        public bool DeleteTemplate(DocsPaVO.trasmissione.TemplateTrasmissione template)
        {
            bool result = true;
            logger.Debug("cancellaTemplate");
            //DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
            bool dbOpen = false;
            try
            {
                this.OpenConnection();
                dbOpen = true;
                //costruzione della query
                //string deleteString="DELETE FROM DPA_TEMPL_TRASM WHERE SYSTEM_ID="+template.systemId;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPATempltrasm");
                q.setParam("param1", "SYSTEM_ID=" + template.systemId);
                string sql = q.getSQL();
                this.ExecuteNonQuery(sql);
                this.CloseConnection();
            }
            catch (Exception)
            {
                if (dbOpen)
                {
                    this.CloseConnection();
                }
                //throw e;
                result = false;
            }
            return result;
        }

        public bool UpdateTemplate(DocsPaVO.trasmissione.TemplateTrasmissione template)
        {
            bool result = true;
            logger.Debug("update Template");
            //DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
            System.Data.DataSet dataSet = new System.Data.DataSet();
            try
            {
                this.OpenConnection();
                /*string updateString="UPDATE DPA_TEMPL_TRASM SET " +
                    " VAR_TEMPLATE ='" + template.descrizione + "' " +
                    " WHERE System_ID = " + template.systemId;*/
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPATempltrasm");
                q.setParam("param1", " VAR_TEMPLATE ='" + template.descrizione + "' ");
                q.setParam("param2", " System_ID = " + template.systemId);
                string sql = q.getSQL();
                logger.Debug(sql);
                this.ExecuteNonQuery(sql);
                this.CloseConnection();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                this.RollbackTransaction();
                this.CloseConnection();
                //throw e;
                result = false;
            }
            return result;
        }
        #endregion

        #region Query di TrasmManager.cs

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objTrasm"></param>
        public void SetAssegnazione(DocsPaVO.trasmissione.Trasmissione objTrasm)
        {

            if (objTrasm == null || objTrasm.infoDocumento == null)
                return;
            string idProfile = objTrasm.infoDocumento.idProfile;
            if (!(idProfile != null && !idProfile.Equals("")))
                return;
            for (int i = 0; i < objTrasm.trasmissioniSingole.Count; i++)
            {
                DocsPaVO.trasmissione.TrasmissioneSingola trs = ((DocsPaVO.trasmissione.TrasmissioneSingola)objTrasm.trasmissioniSingole[i]);
                //DocsPaVO.trasmissione.RagioneTrasmissione ragione = ((DocsPaVO.trasmissione.TrasmissioneSingola)objTrasm.trasmissioniSingole[i]).ragione;
                if (trs != null && trs.ragione != null && trs.ragione.tipo.Equals("W") && trs.ragione.tipoDestinatario == DocsPaVO.trasmissione.TipoGerarchia.INFERIORE)
                {
                    string updateString = "";

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROFILE_CHA_ASSEGNATO_1_NOT_CHA_ASSEGNATO");
                    q.setParam("param1", idProfile);
                    updateString = q.getSQL();
                    this.ExecuteNonQuery(updateString);
                    return;
                }
            }
        }

        public void DeleteTrasmSingola(DocsPaVO.trasmissione.Trasmissione objTrasmissione)
        {
            string sqlString = "delete from dpa_trasm_utente where id_trasm_singola in (select system_id from dpa_trasm_singola where id_trasmissione = " + objTrasmissione.systemId + ")";
            logger.Debug(sqlString);
            this.ExecuteNonQuery(sqlString);

            sqlString = "delete from dpa_trasm_singola where id_trasmissione = " + objTrasmissione.systemId;
            logger.Debug(sqlString);
            this.ExecuteNonQuery(sqlString);
        }

        public void DeleteTrasmSingola(DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola)
        {
            string sqlString = "";

            //	"DELETE DPA_TRASM_UTENTE WHERE ID_TRASM_SINGOLA=" + objTrasmSingola.systemId;
            DocsPaUtils.Query qTrasmUtente = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_TRASM_UTENTE_ID_TRASM_SINGOLA");
            qTrasmUtente.setParam("param1", objTrasmSingola.systemId);
            sqlString = qTrasmUtente.getSQL();

            this.ExecuteNonQuery(sqlString);

            //	"DELETE DPA_TRASM_SINGOLA WHERE SYSTEM_ID=" + objTrasmSingola.systemId;
            sqlString = "";
            DocsPaUtils.Query qTrasmSingola = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_TRASM_SINGOLA_SYSTEM_ID");
            qTrasmSingola.setParam("param1", objTrasmSingola.systemId);
            sqlString = qTrasmSingola.getSQL();

            this.ExecuteNonQuery(sqlString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objTrasmissione"></param>
        public void DeleteTrasmissione(DocsPaVO.trasmissione.Trasmissione objTrasmissione)
        {
            string sqlString = "";

            //		"DELETE DPA_TRASMISSIONE WHERE SYSTEM_ID=" + objTrasmissione.systemId;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_TRASMISSIONE_WHERE_SYSTEMID");
            q.setParam("param1", objTrasmissione.systemId);
            sqlString = q.getSQL();
            this.ExecuteNonQuery(sqlString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="idTrasmRispSingola"></param>
        public void UpdateTrasmRispSing(string systemId, string idTrasmRispSingola)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASM_UTENTE_updateTrasmRispSing");

            q.setParam("param", systemId);
            q.setParam("param", idTrasmRispSingola);

            this.ExecuteNonQuery(q.getSQL());

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objTrasmUtente"></param>
        /// <returns></returns>
        public DocsPaVO.trasmissione.TrasmissioneUtente UpdateTrasmUtente(DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente)
        {

            if (!objTrasmUtente.daAggiornare)
                return objTrasmUtente;
            //string sqlString = "";
            /*"UPDATE DPA_TRASM_UTENTE SET ID_TRASM_SINGOLA = " + notNull(objTrasmUtente.idTrasmRispSing,false) + 				
                ", DTA_VISTA = " + DocsPaWS.Utils.dbControl.toDate(objTrasmUtente.dataVista,false) +
                ", DTA_ACCETTATA = " + DocsPaWS.Utils.dbControl.toDate(objTrasmUtente.dataAccettata,false) +
                ", DTA_RIFIUTATA = " + DocsPaWS.Utils.dbControl.toDate(objTrasmUtente.dataRifiutata,false) +
                ", DTA_RISPOSTA = " + DocsPaWS.Utils.dbControl.toDate(objTrasmUtente.dataRisposta,false) +
                ", VAR_NOTE_ACC = '" + notNull(objTrasmUtente.noteAccettazione,true) + "'" +
                ", VAR_NOTE_RIF = '" + notNull(objTrasmUtente.noteRifiuto,true) + "'";*/

            DocsPaUtils.Query q =
                DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASM_UTENTE_updateTrasmUtente");

            q.setParam("param1", notNull(objTrasmUtente.idTrasmRispSing, false));
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToDate(objTrasmUtente.dataAccettata));
            //DocsPaWS.Utils.dbControl.toDate(objTrasmUtente.dataVista,false));
            q.setParam("param3", DocsPaDbManagement.Functions.Functions.ToDate(objTrasmUtente.dataAccettata));
            //DocsPaWS.Utils.dbControl.toDate(objTrasmUtente.dataAccettata,false));
            q.setParam("param4", DocsPaDbManagement.Functions.Functions.ToDate(objTrasmUtente.dataRifiutata));
            //DocsPaWS.Utils.dbControl.toDate(objTrasmUtente.dataRifiutata,false));
            q.setParam("param5", DocsPaDbManagement.Functions.Functions.ToDate(objTrasmUtente.dataRisposta));
            //DocsPaWS.Utils.dbControl.toDate(objTrasmUtente.dataRisposta,false));
            q.setParam("param6", notNull(objTrasmUtente.noteAccettazione, true));
            q.setParam("param7", notNull(objTrasmUtente.noteRifiuto, true));


            if (!(objTrasmUtente.dataAccettata != null && !objTrasmUtente.dataAccettata.Equals("")))
                //sqlString += ", CHA_ACCETTATA='1'";
                q.setParam("param8", ", CHA_ACCETTATA='1'");
            else
                q.setParam("param8", "");
            if (!(objTrasmUtente.dataRifiutata != null && !objTrasmUtente.dataRifiutata.Equals("")))
                //sqlString += ", CHA_RIFIUTATA='1'";
                q.setParam("param9", ", CHA_RIFIUTATA='1'");
            else
                q.setParam("param9", "");
            if (!(objTrasmUtente.dataVista != null && !objTrasmUtente.dataVista.Equals("")))
                //sqlString += ", CHA_VISTA='1'";
                q.setParam("param10", ", CHA_VISTA='1'");
            else
                q.setParam("param10", "");

            q.setParam("param11", objTrasmUtente.systemId);


            //db.executeNonQuery(sqlString);
            this.ExecuteNonQuery(q.getSQL());
            objTrasmUtente.daAggiornare = false;
            return objTrasmUtente;


        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="objTrasmSingola"></param>
        /// <returns></returns>
        public DocsPaVO.trasmissione.TrasmissioneSingola updateTrasmSingola(DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola)
        {
            if (!objTrasmSingola.daAggiornare)
                return objTrasmSingola;
            string sqlString = string.Empty;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASM_UTENTE_updateTrasmSingola");

            q.setParam("param1", notNull(objTrasmSingola.ragione.systemId, false));
            q.setParam("param2", notNull((string)DocsPaVO.trasmissione.TrasmissioneSingola.tipoDestStringa[objTrasmSingola.tipoDest], true));
            q.setParam("param3", notNull(objTrasmSingola.corrispondenteInterno.systemId, false));
            q.setParam("param4", notNull(objTrasmSingola.noteSingole, true));
            q.setParam("param5", notNull(objTrasmSingola.tipoTrasm, true));
            q.setParam("param6", DocsPaDbManagement.Functions.Functions.ToDate(objTrasmSingola.dataScadenza));
            q.setParam("param7", objTrasmSingola.systemId);
            q.setParam("param8", (objTrasmSingola.hideDocumentPreviousVersions ? "1" : "0"));

            sqlString = q.getSQL();
            logger.Debug(sqlString);
            this.ExecuteNonQuery(sqlString);
            objTrasmSingola.daAggiornare = false;
            return objTrasmSingola;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objTrasm"></param>
        /// <returns></returns>
        public DocsPaVO.trasmissione.Trasmissione updateTrasmissione(DocsPaVO.trasmissione.Trasmissione objTrasm)
        {
            logger.Info("BEGIN");
            if (!objTrasm.daAggiornare)
                return objTrasm;
            //string sqlString = ""; 
            /*"UPDATE DPA_TRASMISSIONE SET CHA_TIPO_OGGETTO = '" + 
                 * notNull((string)DocsPaVO.trasmissione.Trasmissione.oggettoStringa[objTrasm.tipoOggetto],true) + 
                "', VAR_NOTE_GENERALI = '" + 
                notNull(objTrasm.noteGenerali,true) + "' 
                WHERE SYSTEM_ID=" + objTrasm.systemId;*/

            //db.executeNonQuery(sqlString);

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TRASMISSIONE_updateTrasmissione");
            q.setParam("param1", notNull((string)DocsPaVO.trasmissione.Trasmissione.oggettoStringa[objTrasm.tipoOggetto], true));
            q.setParam("param2", notNull(objTrasm.noteGenerali, true));
            q.setParam("param3", objTrasm.systemId);

            if (objTrasm.salvataConCessione)
                q.setParam("cessione", "1");
            else
                q.setParam("cessione", "0");

            this.ExecuteNonQuery(q.getSQL());
            logger.Info("END");
            return objTrasm;


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objTrasmUtente"></param>
        /// <param name="idTrasmSingola"></param>
        /// <returns></returns>
        public DocsPaVO.trasmissione.TrasmissioneUtente insertTrasmUtente(/*DocsPaWS.Utils.Database db,*/DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente, string idTrasmSingola)
        {
            //string sqlString = "" ;
            /*"INSERT INTO DPA_TRASM_UTENTE (" + DocsPaWS.Utils.dbControl.getSystemIdColName() + 
                "ID_PEOPLE,ID_TRASM_RISP_SING,ID_TRASM_SINGOLA,CHA_VISTA,CHA_ACCETTATA,CHA_RIFIUTATA,CHA_VALIDA) " +
                "VALUES ("+DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_TRASM_UTENTE") + 
                notNull(objTrasmUtente.utente.idPeople,false) + "," + notNull(objTrasmUtente.idTrasmRispSing,false)+ "," +
                idTrasmSingola + ",'0','0','0','1')";*/
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_TRASM_UTENTE_insertTrasmUtente");

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            //DocsPaWS.Utils.dbControl.getSystemIdColName());			
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_TRASM_UTENTE"));
            //DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_TRASM_UTENTE"));			
            q.setParam("param3", notNull(objTrasmUtente.utente.idPeople, false));
            q.setParam("param4", notNull(objTrasmUtente.idTrasmRispSing, false));
            q.setParam("param5", idTrasmSingola);

            string res;
            this.InsertLocked(out res, q.getSQL(), "DPA_TRASM_UTENTE");

            objTrasmUtente.systemId = res;
            return objTrasmUtente;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="objTrasmSingola"></param>
        /// <param name="idTrasmissione"></param>
        /// <returns></returns>
        public string insertTrasmSingola(/*DocsPaWS.Utils.Database db,*/ DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola, string idTrasmissione)
        {
            #region codice originale
            /*logger.Debug("insertTrasmSingola");
			string sqlString = "INSERT INTO DPA_TRASM_SINGOLA (" + 
				DocsPaWS.Utils.dbControl.getSystemIdColName() + 
				"ID_RAGIONE, ID_TRASMISSIONE, CHA_TIPO_DEST, ID_CORR_GLOBALE, VAR_NOTE_SING, CHA_TIPO_TRASM, DTA_SCADENZA, ID_TRASM_UTENTE) " +
				"VALUES ("+
				DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_TRASM_SINGOLA") + 
				notNull(objTrasmSingola.ragione.systemId,false) + "," + 
				notNull(idTrasmissione,false) + ",'" + 
				notNull((string)DocsPaVO.trasmissione.TrasmissioneSingola.tipoDestStringa[objTrasmSingola.tipoDest],true) + "'," + 
				notNull(objTrasmSingola.corrispondenteInterno.systemId,false) + ",'" +
				notNull(objTrasmSingola.noteSingole,true) + "','" + 
				objTrasmSingola.tipoTrasm + "'," +
				DocsPaWS.Utils.dbControl.toDate(objTrasmSingola.dataScadenza,false) + "," + 
				notNull(objTrasmSingola.idTrasmUtente,false) + ")";
			logger.Debug(sqlString);						
			String idTrasmSingola = db.insertLocked(sqlString, "DPA_TRASM_SINGOLA");*/
            #endregion
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_TRASM_SINGOLA_insertTrasmSingola");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_TRASM_SINGOLA"));
            q.setParam("param3", notNull(objTrasmSingola.ragione.systemId, false));
            q.setParam("param4", notNull(idTrasmissione, false));
            q.setParam("param5", notNull((string)DocsPaVO.trasmissione.TrasmissioneSingola.tipoDestStringa[objTrasmSingola.tipoDest], true));
            q.setParam("param6", notNull(objTrasmSingola.corrispondenteInterno.systemId, false));
            q.setParam("param7", notNull(objTrasmSingola.noteSingole, true));
            q.setParam("param8", objTrasmSingola.tipoTrasm);
            q.setParam("param9", DocsPaDbManagement.Functions.Functions.ToDate(objTrasmSingola.dataScadenza));
            q.setParam("param10", notNull(objTrasmSingola.idTrasmUtente, false));
            q.setParam("param11", objTrasmSingola.ragione.eredita);
            q.setParam("param12", (objTrasmSingola.hideDocumentPreviousVersions ? "1" : "0"));
            String idTrasmSingola;
            this.InsertLocked(out idTrasmSingola, q.getSQL(), "DPA_TRASM_SINGOLA");
            return idTrasmSingola;
        }



        public string insertTrasmissione(/*DocsPaWS.Utils.Database db,*/ DocsPaVO.trasmissione.Trasmissione objTrasm)
        {
            logger.Info("BEGIN");
            //string sqlString = "" ;

            DocsPaUtils.Query q = null;

            //inizio 3.11.5 3/6/2010: settando la data invio durante l'insert non scatta il trigger che è solo sull'update.
            //in questo modo ottengo trasmissioni senza notifiche in TDL. naturalmente è necessario modificare anche executeTrasm affinchè non faccia 
            // lui l'update che fa scattare il trigger..
            if (!string.IsNullOrEmpty(objTrasm.NO_NOTIFY) && objTrasm.NO_NOTIFY.Equals("1"))
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_TRASMISSIONE_insertTrasmissioneDtaInvio");
                q.setParam("dta_invio", DocsPaDbManagement.Functions.Functions.GetDate(true));
            }
            else
                q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_TRASMISSIONE_insertTrasmissione");

            //fine 3.11.5 3/6/2010

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            //DocsPaWS.Utils.dbControl.getSystemIdColName());
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_TRASMISSIONE"));
            //DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_TRASMISSIONE"));
            q.setParam("param3", notNull(objTrasm.ruolo.systemId, false));
            q.setParam("param4", notNull(objTrasm.utente.idPeople, false));
            q.setParam("param5", notNull((string)DocsPaVO.trasmissione.Trasmissione.oggettoStringa[objTrasm.tipoOggetto], true));
            q.setParam("param6", notNull(getIdProfile(objTrasm), false));
            q.setParam("param7", notNull(getIdProject(objTrasm), false));
            q.setParam("param8", notNull(objTrasm.noteGenerali, true));
            q.setParam("delegato", notNull(objTrasm.delegato, false));
            if (objTrasm.salvataConCessione)
                q.setParam("cessione", "1");
            else
                q.setParam("cessione", "0");

            String idTrasmissione;
            string mySql = q.getSQL();
            logger.Debug(mySql);
            this.InsertLocked(out idTrasmissione, mySql, "DPA_TRASMISSIONE");
            logger.Info("END");
            return idTrasmissione;

        }



        private static string getIdProfile(DocsPaVO.trasmissione.Trasmissione objTrasm)
        {
            if (objTrasm.infoDocumento != null)
                return objTrasm.infoDocumento.idProfile;
            else return null;
        }

        private static string getIdProject(DocsPaVO.trasmissione.Trasmissione objTrasm)
        {
            if (objTrasm.infoFascicolo != null)
                return objTrasm.infoFascicolo.idFascicolo;
            else return null;
        }

        #endregion

        #region utility
        private static string notNull(string val, bool str)
        {
            if (val != null)
            {
                if (str || !val.Equals(""))
                    return val.Replace("'", "''");
                else
                    return "null";
            }
            else if (str)
                return "";
            else
                return "null";
        }


        #endregion

        #region QueryReport
        public void getTrasmissioniDocFasc(out DataSet dataSet, DocsPaVO.trasmissione.OggettoTrasm obj, string tipoDest)
        {
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASM__DPA_RAGIONE__DPA_CORR_GLOBALI");
                q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO", false));
                q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_VISTA", false));
                q.setParam("param3", DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_ACCETTATA", false));
                q.setParam("param4", DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_RIFIUTATA", false));
                string whereStr = null;

                // condizione sui documenti
                if (obj.infoDocumento != null)
                    whereStr = "A.ID_PROFILE=" + obj.infoDocumento.idProfile + " AND A.CHA_TIPO_OGGETTO = 'D'";
                // condizione sui fascicoli
                else if (obj.infoFascicolo != null)
                    whereStr += "A.ID_PROJECT=" + obj.infoFascicolo.idFascicolo + " AND A.CHA_TIPO_OGGETTO = 'F'";

                //trasmissioni a utenti o a ruoli
                whereStr += " AND B.CHA_TIPO_DEST = '" + tipoDest + "'";


                q.setParam("param5", whereStr);
                string queryString = q.getSQL();

                logger.Debug(queryString);
                ExecuteQuery(out dataSet, "REP_TRASMISSIONI", queryString);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("Errore nella gestione delle trasmissioni (Query - getTrasmissioniDocFasc)", e);
                throw e;
            }
        }

        public void getTrasmissioniUO(out DataSet dataSet, DocsPaVO.filtri.FiltroRicerca[] filtriTrasm, string tipoTrasm)
        {
            string nomeQuery = "S_J_DPA_TRASM__DPA_RAGIONE__DPA_CORR_GLOBALI__UO_D";	//D=doc - F=fasc	
            string whereStr = null;
            bool res = true;
            try
            {
                DocsPaVO.filtri.FiltroRicerca f;
                for (int i = 0; i < filtriTrasm.Length; i++)
                {
                    f = filtriTrasm[i];
                    if (f.valore != null && !f.valore.Equals(""))
                    {
                        switch (f.argomento)
                        {
                            case "TRASMISSIONE_IL":
                                whereStr += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO", false) + "= '" + f.valore + "'";
                                break;
                            case "TRASMISSIONE_SUCCESSIVA_AL":
                                whereStr += " AND A.DTA_INVIO>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                                break;
                            case "TRASMISSIONE_PRECEDENTE_IL":
                                whereStr += " AND A.DTA_INVIO<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                                break;
                            case "RAGIONE":
                                whereStr += " AND E.SYSTEM_ID =" + f.valore;
                                break;
                            case "ID_UO":
                                if (tipoTrasm.Equals("E"))
                                    whereStr += " AND G.ID_UO =" + f.valore;
                                else if (tipoTrasm.Equals("R"))
                                    whereStr += " AND B.CHA_TIPO_DEST = 'R' AND D.ID_UO =" + f.valore;
                                break;
                            case "TIPO_OGGETTO":
                                //query.Where += " AND A.CHA_TIPO_OGGETTO = '" + f.valore + "'";
                                nomeQuery = "S_J_DPA_TRASM__DPA_RAGIONE__DPA_CORR_GLOBALI__UO_" + f.valore;
                                break;
                        }
                    }
                }
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery(nomeQuery);
                q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO", false));
                q.setParam("param2", whereStr);
                q.setParam("param5", "DOCSADM");
                string queryString = q.getSQL();

                logger.Debug(queryString);
                res = ExecuteQuery(out dataSet, "REP_TRASMISSIONI", queryString);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("Errore nella gestione delle trasmissioni (Query - getTrasmissioniUO)", e);
                throw e;
            }
        }

        #endregion

        #region QueryTrasmissioniPaginate

        #region TrasmissioniEffettuate

        public void getQueryTrasmEffPaging(ref bool repeatQuery,
                                            out DataSet dataSet,
                                            DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                            DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                            int pageNumber,
                                            string idAmministrazione,
                                            out int totalPageNumber,
                                            out int recordCount)
        {
            this.getQueryTrasmEffPaging(ref repeatQuery,
                                                out dataSet,
                                                objOggettoTrasmesso,
                                                null,
                                                null,
                                                objListaFiltri,
                                                pageNumber,
                                                out totalPageNumber,
                                                out recordCount,
                                                idAmministrazione);
        }
        /// <summary>
        /// Esecuzione query paginata per il reperimento delle trasmissioni effettuate
        /// </summary>
        /// <param name="repeatQuery"></param>
        /// <param name="dataSet"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objListaFiltri"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalPageNumber"></param>
        /// <param name="recordCount"></param>
        public void getQueryTrasmEffPaging(ref bool repeatQuery,
                                            out DataSet dataSet,
                                            DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                            DocsPaVO.utente.Utente objUtente,
                                            DocsPaVO.utente.Ruolo objRuolo,
                                            DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                            int pageNumber,
                                            out int totalPageNumber,
                                            out int recordCount,
                                            String idAmministrazione)
        {
            string queryWhere = "";
            DocsPaUtils.Query q = QueryTrasmExport(ref queryWhere, objOggettoTrasmesso);

            if (objUtente != null && objRuolo != null)
                getCondizioniVisibilitaEffettuate(ref queryWhere, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);

            string queryJoin = "";
            repeatQuery = getCondFiltri(ref queryWhere, ref queryJoin, objListaFiltri, objUtente, objRuolo);

            ////CONDIZIONE per non trovare documenti con visibilità rimossa:
            //if (objOggettoTrasmesso!=null && objOggettoTrasmesso.infoDocumento != null && objUtente != null && objRuolo != null)
            //     queryWhere += " AND  EXISTS (SELECT 'x' FROM SECURITY D WHERE D.PERSONORGROUP  IN (" + objRuolo.idGruppo + "," + objUtente.idPeople + ") AND D.THING=A.ID_PROFILE )   ";

            // Reperimento array contenente tutti gli IDTrasmissione della pagina richiesta
            string[] idTrasmArray = this.GetSystemIDTrasmEffettuate(queryWhere, queryJoin, string.Empty, pageNumber, out totalPageNumber, out recordCount, objListaFiltri, idAmministrazione);

            if (idTrasmArray.Length > 0)
            {
                string trasmFilter = "";

                foreach (string idTrasm in idTrasmArray)
                {
                    if (trasmFilter != "")
                        trasmFilter += ", ";
                    trasmFilter += idTrasm;
                }

                queryWhere += " AND A.SYSTEM_ID IN (" + trasmFilter + ")";
            }

            q.setParam("param6", queryWhere);
            q.setParam("param7", queryJoin);

            #region Ordinamento

            // Recupero dei filtri di ricerca relarivi all'ordinamento
            FiltroRicerca oracleField = null;
            FiltroRicerca sqlField = null;
            FiltroRicerca profilationField = null;
            FiltroRicerca orderDirection = null;
            if (objListaFiltri != null)
            {
                oracleField = objListaFiltri.Where(e => e.argomento == "ORACLE_FIELD_FOR_ORDER").FirstOrDefault();
                sqlField = objListaFiltri.Where(e => e.argomento == "SQL_FIELD_FOR_ORDER").FirstOrDefault();
                profilationField = objListaFiltri.Where(e => e.argomento == "PROFILATION_FIELD_FOR_ORDER").FirstOrDefault();
                orderDirection = objListaFiltri.Where(e => e.argomento == "ORDER_DIRECTION").FirstOrDefault();
            }
            // Se non è valorizzata la direzione di ordinamento, viene creato un filtro di tipo ORDER_DIRECTION
            if (orderDirection == null)
            {
                orderDirection = new FiltroRicerca()
                {
                    argomento = "ORDER_DIRECTION",
                    valore = "DESC"

                };

            }

            // Function da utilizzare per estrarre i valori del campo profilato da utilizzare per l'ordinamento
            String extractFieldValue = String.Empty, orderCondition = String.Empty;

            if (this.dbType == "SQL")
            {
                // DB SQL Server
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", Convert(int, @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0})) AS CUSTOM_FIELD", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0}) AS CUSTOM_FIELD", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("CUSTOM_FIELD {0}", orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (sqlField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Format(", {0} AS var_desc_ragione", sqlField.valore);
                        orderCondition = String.Format("var_desc_ragione {0}", orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("DTA_INVIO {0}", orderDirection.valore);
                    }
                }

            }
            else
            {
                // DB ORACLE
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", to_number(getValCampoProfDoc(A.DOCNUMBER, {0}))", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", getValCampoProfDoc(A.DOCNUMBER, {0})", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("getValCampoProfDoc(A.DOCNUMBER, {0}) {1}", profilationField.valore, orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (oracleField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("{0} {1}", oracleField.valore, orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("TO_DATE (a.dta_invio, 'dd/MM/yyyy') {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }
            }

            q.setParam("orderCondition", orderCondition);

            #endregion


            string queryString = q.getSQL();

            logger.Debug(queryString);

            this.ExecuteQuery(out dataSet, "TRASMISSIONI", queryString);
        }


        /// <summary>
        /// Esecuzione query paginata per il reperimento delle trasmissioni effettuate
        /// </summary>
        /// <param name="repeatQuery"></param>
        /// <param name="dataSet"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objListaFiltri"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalPageNumber"></param>
        /// <param name="recordCount"></param>
        public void getQueryTrasmissioneById(out DataSet dataSet,
                                            DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                            string systemId)
        {
            string queryWhere = "";
            DocsPaUtils.Query q = QueryTrasmExport(ref queryWhere, objOggettoTrasmesso);
            queryWhere += " AND A.SYSTEM_ID = " + systemId + " ";

            string queryJoin = "";

            q.setParam("param6", queryWhere);
            q.setParam("param7", queryJoin);

            #region Ordinamento

            // Recupero dei filtri di ricerca relarivi all'ordinamento
            FiltroRicerca orderDirection = orderDirection = new FiltroRicerca()
            {
                argomento = "ORDER_DIRECTION",
                valore = "DESC"
            };

            String orderCondition = String.Empty;
            if (this.dbType == "SQL")
            {
                // DB SQL Server
                orderCondition = String.Format("DTA_INVIO {0}", orderDirection.valore);
            }
            else
            {
                // DB ORACLE
                orderCondition = String.Format("a.dta_invio {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
            }

            q.setParam("orderCondition", orderCondition);

            #endregion


            string queryString = q.getSQL();

            logger.Debug(queryString);

            this.ExecuteQuery(out dataSet, "TRASMISSIONI", queryString);
        }

        /// <summary>
        /// Restituisce il dettaglio della trasmissione partendo dall'id della trasmissione singola
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="idTrasmSing"></param>
        public void getQueryTrasmissioneByidTrasmSing(out DataSet dataSet,
                                            DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                            string idTrasmSing)
        {
            string queryWhere = "";
            DocsPaUtils.Query q = QueryTrasmByidTrasmSing(ref queryWhere, objOggettoTrasmesso, idTrasmSing);

            string queryJoin = "";

            q.setParam("param6", queryWhere);
            q.setParam("param7", queryJoin);

            #region Ordinamento

            // Recupero dei filtri di ricerca relarivi all'ordinamento
            FiltroRicerca orderDirection = orderDirection = new FiltroRicerca()
            {
                argomento = "ORDER_DIRECTION",
                valore = "DESC"
            };

            String orderCondition = String.Empty;
            if (this.dbType == "SQL")
            {
                // DB SQL Server
                orderCondition = String.Format("DTA_INVIO {0}", orderDirection.valore);
            }
            else
            {
                // DB ORACLE
                orderCondition = String.Format("TO_DATE (a.dta_invio, 'dd/MM/yyyy') {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
            }

            q.setParam("orderCondition", orderCondition);

            #endregion


            string queryString = q.getSQL();

            logger.Debug(queryString);

            this.ExecuteQuery(out dataSet, "TRASMISSIONI", queryString);
        }


        /// <summary>
        /// Esecuzione query per il reperimento di tutti gli idtrasmissione
        /// relativamente alla pagina richiesta.
        /// La query è costruita dinamicamente in base al filtro selezionato
        /// dall'utente, e all'ordinamento
        /// </summary>
        /// <param name="queryWhere"></param>
        /// <param name="queryJoin"></param>
        /// <param name="requestedPageNumber"></param>
        /// <param name="totalPageNumber"></param>
        /// <param name="totalRecordCount"></param>
        /// <returns></returns>
        private string[] GetSystemIDTrasmEffettuate(string queryWhere,
                                                    string queryJoin,
                                                    string orderCriteria,
                                                    int requestedPageNumber,
                                                    out int totalPageNumber,
                                                    out int totalRecordCount,
                                                    FiltroRicerca[] objListaFiltri,
                                                    string idAmministrazione)
        {
            //string fieldDefinition="A.SYSTEM_ID AS ID_TRASM, A.DTA_INVIO, B.ID_TRASMISSIONE AS ID_TRASM_SINGOLA ";
            string tables = "DPA_TRASMISSIONE A, DPA_TRASM_SINGOLA B, DPA_TRASM_UTENTE C " + queryJoin;
            queryWhere = "A.SYSTEM_ID=B.ID_TRASMISSIONE AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA " + queryWhere;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TRASM_SyIDTrEff");
            //q.setParam("param1", fieldDefinition);
            q.setParam("param2", tables);
            q.setParam("param3", queryWhere);

            #region Ordinamento

            // Recupero dei filtri di ricerca relarivi all'ordinamento
            FiltroRicerca oracleField = null;
            FiltroRicerca sqlField = null;
            FiltroRicerca profilationField = null;
            FiltroRicerca orderDirection = null;
            if (objListaFiltri != null)
            {
                oracleField = objListaFiltri.Where(e => e.argomento == "ORACLE_FIELD_FOR_ORDER").FirstOrDefault();
                sqlField = objListaFiltri.Where(e => e.argomento == "SQL_FIELD_FOR_ORDER").FirstOrDefault();
                profilationField = objListaFiltri.Where(e => e.argomento == "PROFILATION_FIELD_FOR_ORDER").FirstOrDefault();
                orderDirection = objListaFiltri.Where(e => e.argomento == "ORDER_DIRECTION").FirstOrDefault();
            }
            // Se non è valorizzata la direzione di ordinamento, viene creato un filtro di tipo ORDER_DIRECTION
            if (orderDirection == null)
            {
                orderDirection = new FiltroRicerca()
                {
                    argomento = "ORDER_DIRECTION",
                    valore = "DESC"

                };

            }

            // Function da utilizzare per estrarre i valori del campo profilato da utilizzare per l'ordinamento
            String extractFieldValue = String.Empty, orderCondition = String.Empty;

            if (this.dbType == "SQL")
            {
                // DB SQL Server
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", Convert(int, @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0})) AS CUSTOM_FIELD", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0}) AS CUSTOM_FIELD", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("CUSTOM_FIELD {0}", orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (sqlField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Format(", {0} AS var_desc_ragione", sqlField.valore);
                        orderCondition = String.Format("var_desc_ragione {0}", orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("DTA_INVIO {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }

            }
            else
            {
                // DB ORACLE
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", to_number(getValCampoProfDoc(A.DOCNUMBER, {0}))", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", getValCampoProfDoc(A.DOCNUMBER, {0})", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("getValCampoProfDoc(A.DOCNUMBER, {0}) {1}", profilationField.valore, orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (oracleField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("{0} {1}", oracleField.valore, orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        // Andrea - Errore di escuzione della query per data null e data con anno valorizzato a 2000
                        //orderCondition = String.Format("TO_DATE (a.dta_invio, 'dd/MM/yyyy') {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                        orderCondition = String.Format("a.dta_invio {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }
            }

            q.setParam("orderCondition", orderCondition);

            #endregion

            string sqlDefinition = q.getSQL();

            logger.Debug(sqlDefinition);

            DataSet ds = null;
            this.ExecuteQuery(out ds, sqlDefinition);

            return this.GetArraySystemIDTrasmissioni(ds.Tables[0], requestedPageNumber, out totalPageNumber, out totalRecordCount, idAmministrazione);
        }

        /// <summary>
        /// Esecuzione query per il reperimento di tutti gli idtrasmissione
        /// relativamente alla pagina richiesta.
        /// La query è costruita dinamicamente in base al filtro selezionato
        /// dall'utente, e all'ordinamento
        /// </summary>
        /// <param name="queryWhere"></param>
        /// <param name="queryJoin"></param>
        /// <param name="requestedPageNumber"></param>
        /// <param name="totalPageNumber"></param>
        /// <param name="totalRecordCount"></param>
        /// <returns></returns>
        private string[] GetSystemIDTrasmEffettuateLite(string queryWhere,
                                                    string queryJoin,
                                                    string orderCriteria,
                                                    int requestedPageNumber,
                                                    bool excel,
                                                    int pageSize,
                                                    out int totalPageNumber,
                                                    out int totalRecordCount,
                                                    FiltroRicerca[] objListaFiltri,
                                                    string idAmministrazione)
        {
            //string fieldDefinition="A.SYSTEM_ID AS ID_TRASM, A.DTA_INVIO, B.ID_TRASMISSIONE AS ID_TRASM_SINGOLA ";
            string tables = "DPA_TRASMISSIONE A, DPA_TRASM_SINGOLA B " + queryJoin;
            queryWhere = "A.SYSTEM_ID=B.ID_TRASMISSIONE " + queryWhere;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TRASM_SyIDTrEff_LITE");
            //q.setParam("param1", fieldDefinition);
            q.setParam("param2", tables);
            q.setParam("param3", queryWhere);

            #region Ordinamento

            // Recupero dei filtri di ricerca relarivi all'ordinamento
            FiltroRicerca oracleField = null;
            FiltroRicerca sqlField = null;
            FiltroRicerca profilationField = null;
            FiltroRicerca orderDirection = null;
            if (objListaFiltri != null)
            {
                oracleField = objListaFiltri.Where(e => e.argomento == "ORACLE_FIELD_FOR_ORDER").FirstOrDefault();
                sqlField = objListaFiltri.Where(e => e.argomento == "SQL_FIELD_FOR_ORDER").FirstOrDefault();
                profilationField = objListaFiltri.Where(e => e.argomento == "PROFILATION_FIELD_FOR_ORDER").FirstOrDefault();
                orderDirection = objListaFiltri.Where(e => e.argomento == "ORDER_DIRECTION").FirstOrDefault();
            }
            // Se non è valorizzata la direzione di ordinamento, viene creato un filtro di tipo ORDER_DIRECTION
            if (orderDirection == null)
            {
                orderDirection = new FiltroRicerca()
                {
                    argomento = "ORDER_DIRECTION",
                    valore = "DESC"

                };

            }

            // Function da utilizzare per estrarre i valori del campo profilato da utilizzare per l'ordinamento
            String extractFieldValue = String.Empty, orderCondition = String.Empty;

            if (this.dbType == "SQL")
            {
                // DB SQL Server
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", Convert(int, @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0})) AS CUSTOM_FIELD", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0}) AS CUSTOM_FIELD", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("CUSTOM_FIELD {0}", orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (sqlField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Format(", {0} AS var_desc_ragione", sqlField.valore);
                        orderCondition = String.Format("var_desc_ragione {0}", orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("DTA_INVIO {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }

            }
            else
            {
                // DB ORACLE
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", to_number(getValCampoProfDoc(A.DOCNUMBER, {0}))", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", getValCampoProfDoc(A.DOCNUMBER, {0})", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("getValCampoProfDoc(A.DOCNUMBER, {0}) {1}", profilationField.valore, orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (oracleField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("{0} {1}", oracleField.valore, orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        // Andrea - Errore in esecuzione query per data null o data con anno valorizzato a 2000
                        //orderCondition = String.Format("TO_DATE (a.dta_invio, 'dd/MM/yyyy') {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                        orderCondition = String.Format("a.dta_invio {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }
            }

            q.setParam("orderCondition", orderCondition);

            #endregion

            string sqlDefinition = q.getSQL();

            logger.Debug(sqlDefinition);

            DataSet ds = null;
            this.ExecuteQuery(out ds, sqlDefinition);

            totalPageNumber = 0;
            totalRecordCount = 0;

            int startRecord = (requestedPageNumber * pageSize) - pageSize;
            // DataRow row = null;

            List<string> list = new List<string>();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow row = ds.Tables[0].Rows[i];

                string idTrasm = row["ID_TRASM"].ToString();

                if (!list.Contains(idTrasm))
                {
                    if (i >= startRecord && list.Count < pageSize)
                    {
                        list.Add(idTrasm);
                    }

                    totalRecordCount++;
                }
            }

            /* ABBATANGELI GIANLUIGI
             * Aggiunto il valore di configurazione MAX_ROW_SEARCHABLE
             * che determina il numero massimo di righe accettatte
             * come risultato di una ricerca trasmissioni */
            int maxRowSearchable = Cfg_MAX_ROW_SEARCHABLE(idAmministrazione);





            if (maxRowSearchable == 0 || totalRecordCount <= maxRowSearchable)
            {
                // Calcolo numero pagine totali
                totalPageNumber = (totalRecordCount / pageSize);
                if (totalPageNumber * pageSize < totalRecordCount)
                    totalPageNumber++;
            }
            else
            {
                /* ABBATANGELI GIANLUIGI
                * Non carico i documenti perchè raggiunto il numero massimo 
                * di righe per la ricerca ed imposto numTotPage = -2. */
                totalPageNumber = -2;
            }
            return list.ToArray();
        }

        /// <summary>
        /// Esecuzione query paginata per il reperimento delle trasmissioni effettuate
        /// </summary>
        /// <param name="repeatQuery"></param>
        /// <param name="dataSet"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objListaFiltri"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalPageNumber"></param>
        /// <param name="recordCount"></param>
        public void getQueryTrasmEffPagingLite(ref bool repeatQuery,
                                            out DataSet dataSet,
                                            DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                            DocsPaVO.utente.Utente objUtente,
                                            DocsPaVO.utente.Ruolo objRuolo,
                                            DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                            int pageNumber,
                                            bool excel,
                                            int pageSize,
                                            out int totalPageNumber,
                                            out int recordCount)
        {
            string queryWhere = "";
            DocsPaUtils.Query q = QueryTrasmExportLite(ref queryWhere, objOggettoTrasmesso, objListaFiltri);

            if (objUtente != null && objRuolo != null)
                getCondizioniVisibilitaEffettuate(ref queryWhere, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);

            string queryJoin = "";
            repeatQuery = getCondFiltri(ref queryWhere, ref queryJoin, objListaFiltri, objUtente, objRuolo);

            // Reperimento array contenente tutti gli IDTrasmissione della pagina richiesta
            string[] idTrasmArray = this.GetSystemIDTrasmEffettuateLite(queryWhere, queryJoin, string.Empty, pageNumber, excel, pageSize, out totalPageNumber, out recordCount, objListaFiltri, objUtente.idAmministrazione);

            if (!excel)
            {
                if (idTrasmArray.Length > 0)
                {
                    string trasmFilter = "";

                    foreach (string idTrasm in idTrasmArray)
                    {
                        if (trasmFilter != "")
                            trasmFilter += ", ";
                        trasmFilter += idTrasm;
                    }

                    queryWhere += " AND A.SYSTEM_ID IN (" + trasmFilter + ")";
                }
            }

            q.setParam("param6", queryWhere);
            q.setParam("param7", queryJoin);

            #region Ordinamento

            // Recupero dei filtri di ricerca relarivi all'ordinamento
            FiltroRicerca oracleField = null;
            FiltroRicerca sqlField = null;
            FiltroRicerca profilationField = null;
            FiltroRicerca orderDirection = null;
            if (objListaFiltri != null)
            {
                oracleField = objListaFiltri.Where(e => e.argomento == "ORACLE_FIELD_FOR_ORDER").FirstOrDefault();
                sqlField = objListaFiltri.Where(e => e.argomento == "SQL_FIELD_FOR_ORDER").FirstOrDefault();
                profilationField = objListaFiltri.Where(e => e.argomento == "PROFILATION_FIELD_FOR_ORDER").FirstOrDefault();
                orderDirection = objListaFiltri.Where(e => e.argomento == "ORDER_DIRECTION").FirstOrDefault();
            }
            // Se non è valorizzata la direzione di ordinamento, viene creato un filtro di tipo ORDER_DIRECTION
            if (orderDirection == null)
            {
                orderDirection = new FiltroRicerca()
                {
                    argomento = "ORDER_DIRECTION",
                    valore = "DESC"

                };

            }

            // Function da utilizzare per estrarre i valori del campo profilato da utilizzare per l'ordinamento
            String extractFieldValue = String.Empty, orderCondition = String.Empty;

            if (this.dbType == "SQL")
            {
                // DB SQL Server
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", Convert(int, @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0})) AS CUSTOM_FIELD", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0}) AS CUSTOM_FIELD", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("CUSTOM_FIELD {0}", orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (sqlField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Format(", {0} AS var_desc_ragione", sqlField.valore);
                        orderCondition = String.Format("var_desc_ragione {0}", orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("DTA_INVIO {0}", orderDirection.valore);
                    }
                }

            }
            else
            {
                // DB ORACLE
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", to_number(getValCampoProfDoc(A.DOCNUMBER, {0}))", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", getValCampoProfDoc(A.DOCNUMBER, {0})", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("getValCampoProfDoc(A.DOCNUMBER, {0}) {1}", profilationField.valore, orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (oracleField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("{0} {1}", oracleField.valore, orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("TO_DATE (a.dta_invio, 'dd/MM/yyyy') {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }
            }

            q.setParam("orderCondition", orderCondition);

            #endregion


            string queryString = q.getSQL();

            logger.Debug(queryString);

            this.ExecuteQuery(out dataSet, "TRASMISSIONI", queryString);
        }



        /// <summary>
        /// Esecuzione query paginata per il reperimento delle trasmissioni effettuate
        /// </summary>
        /// <param name="repeatQuery"></param>
        /// <param name="dataSet"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objListaFiltri"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalPageNumber"></param>
        /// <param name="recordCount"></param>
        public void getQueryTrasmEffPagingLiteWithoutTrasmUtente(ref bool repeatQuery,
                                            out DataSet dataSet,
                                            DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                            DocsPaVO.utente.Utente objUtente,
                                            DocsPaVO.utente.Ruolo objRuolo,
                                            DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                            int pageNumber,
                                            bool excel,
                                            int pageSize,
                                            out int totalPageNumber,
                                            out int recordCount)
        {
            string queryWhere = "";
            DocsPaUtils.Query q = QueryTrasmExportLiteWithoutTrasmUtente(ref queryWhere, objOggettoTrasmesso, objListaFiltri, objUtente);

            if (objUtente != null && objRuolo != null)
                getCondizioniVisibilitaEffettuate(ref queryWhere, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);

            string queryJoin = "";
            repeatQuery = getCondFiltri2(ref queryWhere, ref queryJoin, objListaFiltri, objUtente, objRuolo);

            // Reperimento array contenente tutti gli IDTrasmissione della pagina richiesta
            string[] idTrasmArray = this.GetSystemIDTrasmEffettuateLite(queryWhere, queryJoin, string.Empty, pageNumber, excel, pageSize, out totalPageNumber, out recordCount, objListaFiltri, objUtente.idAmministrazione);

            if (!excel)
            {
                if (idTrasmArray.Length > 0)
                {
                    string trasmFilter = "";

                    foreach (string idTrasm in idTrasmArray)
                    {
                        if (trasmFilter != "")
                            trasmFilter += ", ";
                        trasmFilter += idTrasm;
                    }

                    queryWhere += " AND A.SYSTEM_ID IN (" + trasmFilter + ")";
                }
            }

            q.setParam("param6", queryWhere);
            q.setParam("param7", queryJoin);

            #region Ordinamento

            // Recupero dei filtri di ricerca relarivi all'ordinamento
            FiltroRicerca oracleField = null;
            FiltroRicerca sqlField = null;
            FiltroRicerca profilationField = null;
            FiltroRicerca orderDirection = null;
            if (objListaFiltri != null)
            {
                oracleField = objListaFiltri.Where(e => e.argomento == "ORACLE_FIELD_FOR_ORDER").FirstOrDefault();
                sqlField = objListaFiltri.Where(e => e.argomento == "SQL_FIELD_FOR_ORDER").FirstOrDefault();
                profilationField = objListaFiltri.Where(e => e.argomento == "PROFILATION_FIELD_FOR_ORDER").FirstOrDefault();
                orderDirection = objListaFiltri.Where(e => e.argomento == "ORDER_DIRECTION").FirstOrDefault();
            }
            // Se non è valorizzata la direzione di ordinamento, viene creato un filtro di tipo ORDER_DIRECTION
            if (orderDirection == null)
            {
                orderDirection = new FiltroRicerca()
                {
                    argomento = "ORDER_DIRECTION",
                    valore = "DESC"

                };

            }

            // Function da utilizzare per estrarre i valori del campo profilato da utilizzare per l'ordinamento
            String extractFieldValue = String.Empty, orderCondition = String.Empty;

            if (this.dbType == "SQL")
            {
                // DB SQL Server
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", Convert(int, @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0})) AS CUSTOM_FIELD", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0}) AS CUSTOM_FIELD", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("CUSTOM_FIELD {0}", orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (sqlField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Format(", {0} AS var_desc_ragione", sqlField.valore);
                        orderCondition = String.Format("var_desc_ragione {0}", orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("DTA_INVIO {0}", orderDirection.valore);
                    }
                }

            }
            else
            {
                // DB ORACLE
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", to_number(getValCampoProfDoc(A.DOCNUMBER, {0}))", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", getValCampoProfDoc(A.DOCNUMBER, {0})", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("getValCampoProfDoc(A.DOCNUMBER, {0}) {1}", profilationField.valore, orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (oracleField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("{0} {1}", oracleField.valore, orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("TO_DATE (a.dta_invio, 'dd/MM/yyyy') {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }
            }

            q.setParam("orderCondition", orderCondition);

            #endregion


            string queryString = q.getSQL();

            logger.Debug(queryString);

            this.ExecuteQuery(out dataSet, "TRASMISSIONI", queryString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryWhere"></param>
        /// <param name="objOggettoTrasmesso"></param>
        public DocsPaUtils.Query QueryTrasmExportLite(ref string queryWhere, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASMISSIONE__TRASM_SINGOLA__TRASM_UTENTE_EXPORT2");

            string UserDB = string.Empty;
            string otherField = string.Empty;
            string otherTable = string.Empty;
            string otherFilter = string.Empty;


            if (dbType.ToUpper() == "SQL")
            {
                UserDB = getUserDB();
                q.setParam("dbuser", UserDB);
            }

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO", false));
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("B.DTA_SCADENZA", false));
            q.setParam("param3", DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_VISTA", false));
            q.setParam("param4", DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_ACCETTATA", false));
            q.setParam("param5", DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_RIFIUTATA", false));

            if (objOggettoTrasmesso == null)
            {
                return q;
            }

            //è un documento
            FiltroRicerca documentOrFolder = objListaFiltri.Where(e => e.argomento == "TIPO_OGGETTO").FirstOrDefault();

            if (documentOrFolder != null)
            {
                if (documentOrFolder.valore == "D")
                {
                    if (dbType.ToUpper() == "SQL")
                    {
                        UserDB = getUserDB();
                        otherField = " ," + UserDB + ".corrcatbytipo(pr.system_id,pr.cha_tipo_proto,'M') as mittenti_proto, " + UserDB + ".corrcatbytipo(pr.system_id,pr.cha_tipo_proto,'D') as destinatari_proto, pr.VAR_PROF_OGGETTO as oggetto_proto, CONVERT(datetime,pr.dta_proto,109) as data_proto, pr.NUM_PROTO as num_proto, pr.DOCNUMBER, pr.VAR_SEGNATURA, pr.ID_REGISTRO, pr.cha_tipo_proto ";
                    }
                    else
                    {
                        otherField = " ,corrcatbytipo(pr.system_id,pr.cha_tipo_proto,'M') as mittenti_proto, corrcatbytipo(pr.system_id,pr.cha_tipo_proto,'D') as destinatari_proto, pr.VAR_PROF_OGGETTO as oggetto_proto, TO_CHAR (pr.dta_proto, 'dd/mm/yyyy') as data_proto, pr.NUM_PROTO as num_proto, pr.DOCNUMBER, pr.VAR_SEGNATURA, pr.ID_REGISTRO, pr.cha_tipo_proto ";
                    }
                    otherTable = " ,profile pr ";
                    otherFilter = " AND pr.system_id = a.id_profile ";
                }
                else
                {
                    if (dbType.ToUpper() == "SQL")
                    {
                        otherField = " ,pj.var_codice, pj.description,CONVERT(datetime,pj.dta_apertura,109) AS dta_apertura, pj.id_registro";
                    }
                    else
                    {
                        otherField = " ,pj.var_codice, pj.description,TO_CHAR (pj.dta_apertura, 'dd/mm/yyyy') AS dta_apertura, pj.id_registro";
                    }

                    otherTable = " ,project pj ";
                    otherFilter = " AND pj.system_id = a.ID_PROJECT ";
                }
            }

            q.setParam("otherField", otherField);
            q.setParam("otherTable", otherTable);
            q.setParam("otherFilter", otherFilter);

            string whereStr = null;
            bool doc = false;

            // condizione sui documenti
            if (objOggettoTrasmesso.infoDocumento != null)
            {
                whereStr = "A.ID_PROFILE=" + objOggettoTrasmesso.infoDocumento.idProfile;
                doc = true;
            }

            //condizione sui fascicoli
            if (objOggettoTrasmesso.infoFascicolo != null)
            {
                if (doc) whereStr += " OR ";
                whereStr += "A.ID_PROJECT=" + objOggettoTrasmesso.infoFascicolo.idFascicolo;
            }
            if (whereStr != null)
            {
                queryWhere += " AND ";
                if (doc)
                {
                    queryWhere += "A.ID_PROFILE in (select system_id from profile where cha_in_cestino in (null, '0')) AND (";
                }
                queryWhere += whereStr;
                if (doc)
                {
                    queryWhere += ") ";
                }
            }

            return q;
        }


        public DocsPaUtils.Query QueryTrasmExportLiteWithoutTrasmUtente(ref string queryWhere, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, DocsPaVO.utente.Utente utente)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TRASMISSIONE_EXPORT2");

            string UserDB = string.Empty;
            string otherField = string.Empty;
            string otherTable = string.Empty;
            string otherFilter = string.Empty;


            if (dbType.ToUpper() == "SQL")
            {
                UserDB = getUserDB();
                q.setParam("dbuser", UserDB);
            }

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO", false));
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("B.DTA_SCADENZA", false));

            if (objOggettoTrasmesso == null)
            {
                return q;
            }

            //è un documento
            FiltroRicerca documentOrFolder = objListaFiltri.Where(e => e.argomento == "TIPO_OGGETTO").FirstOrDefault();

            if (documentOrFolder != null)
            {
                if (documentOrFolder.valore == "D")
                {
                    if (dbType.ToUpper() == "SQL")
                    {
                        UserDB = getUserDB();
                        otherField = " ," + UserDB + ".corrcatbytipo(pr.system_id,pr.cha_tipo_proto,'M') as mittenti_proto, " + UserDB + ".corrcatbytipo(pr.system_id,pr.cha_tipo_proto,'D') as destinatari_proto, pr.VAR_PROF_OGGETTO as oggetto_proto, CONVERT(datetime,pr.dta_proto,109) as data_proto, pr.NUM_PROTO as num_proto, pr.DOCNUMBER, pr.VAR_SEGNATURA, pr.ID_REGISTRO, pr.cha_tipo_proto ";
                        otherField += ", " + UserDB + ".GETSEGNATURAREPERTORIO(pr.system_id," + utente.idAmministrazione + ") as COUNTER_REPERTORY";
                    }
                    else
                    {
                        otherField = " ,corrcatbytipo(pr.system_id,pr.cha_tipo_proto,'M') as mittenti_proto, corrcatbytipo(pr.system_id,pr.cha_tipo_proto,'D') as destinatari_proto, pr.VAR_PROF_OGGETTO as oggetto_proto, TO_CHAR (pr.dta_proto, 'dd/mm/yyyy') as data_proto, pr.NUM_PROTO as num_proto, pr.DOCNUMBER, pr.VAR_SEGNATURA, pr.ID_REGISTRO, pr.cha_tipo_proto ";
                        otherField += ", GETSEGNATURAREPERTORIO(pr.system_id," + utente.idAmministrazione + ") as COUNTER_REPERTORY";
                    }
                    otherTable = " ,profile pr ";
                    otherFilter = " AND pr.system_id = a.id_profile ";
                }
                else
                {
                    if (dbType.ToUpper() == "SQL")
                    {
                        otherField = " ,pj.var_codice, pj.description,CONVERT(datetime,pj.dta_apertura,109) AS dta_apertura, pj.id_registro";
                    }
                    else
                    {
                        otherField = " ,pj.var_codice, pj.description,TO_CHAR (pj.dta_apertura, 'dd/mm/yyyy') AS dta_apertura, pj.id_registro";
                    }

                    otherTable = " ,project pj ";
                    otherFilter = " AND pj.system_id = a.ID_PROJECT ";
                }
            }

            q.setParam("otherField", otherField);
            q.setParam("otherTable", otherTable);
            q.setParam("otherFilter", otherFilter);

            string whereStr = null;
            bool doc = false;

            // condizione sui documenti
            if (objOggettoTrasmesso.infoDocumento != null)
            {
                whereStr = "A.ID_PROFILE=" + objOggettoTrasmesso.infoDocumento.idProfile;
                doc = true;
            }

            //condizione sui fascicoli
            if (objOggettoTrasmesso.infoFascicolo != null)
            {
                if (doc) whereStr += " OR ";
                whereStr += "A.ID_PROJECT=" + objOggettoTrasmesso.infoFascicolo.idFascicolo;
            }
            if (whereStr != null)
            {
                queryWhere += " AND ";
                if (doc)
                {
                    queryWhere += "A.ID_PROFILE in (select system_id from profile where cha_in_cestino in (null, '0')) AND (";
                }
                queryWhere += whereStr;
                if (doc)
                {
                    queryWhere += ") ";
                }
            }

            return q;
        }

        #endregion


        #region TrasmissioniRicevute

        public void getQueryTrasmRicPaging(ref bool repeatQuery,
                                            out DataSet dataSet,
                                            DocsPaVO.utente.Utente utente,
                                            DocsPaVO.utente.Ruolo ruolo,
                                            DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                            int pageNumber,
                                            out int totalPageNumber,
                                            out int recordCount)
        {
            dataSet = null;
            totalPageNumber = 0;
            recordCount = 0;

            string queryWhere = "";
            string queryFrom = "";
            string queryColumn = "";
            getQueryTrasmissioni(objOggettoTrasmesso, ref queryWhere, ref queryFrom, ref queryColumn);
            ///string idCorrispondente = getQueryEffMet1(utente.idPeople);

            queryWhere +=
                " AND A.DTA_INVIO IS NOT NULL "; // +
            // "AND ((B.ID_CORR_GLOBALE = " + ruolo.systemId + " AND C.ID_PEOPLE=" + utente.idPeople + ") OR B.ID_CORR_GLOBALE=" + idCorrispondente;

            //queryWhere += ")";


            if (utente != null && ruolo != null)
                getCondizioniVisibilitaRicevute(ref queryWhere, objOggettoTrasmesso, utente, ruolo, objListaFiltri);

            //DocsPaUtils.Query q = QueryTrasm(ref queryWhere,objOggettoTrasmesso);
            repeatQuery = getCondFiltri(ref queryWhere, ref queryFrom, objListaFiltri, utente, ruolo);

            string[] idTrasmArray;
            idTrasmArray = this.GetSystemIDTrasmRicevute(queryWhere, queryFrom, string.Empty, pageNumber, out totalPageNumber, out recordCount, objListaFiltri, utente.idAmministrazione);
            if (idTrasmArray.Length > 0)
            {
                string trasmFilter = "";

                foreach (string idTrasm in idTrasmArray)
                {
                    if (trasmFilter != "")
                        trasmFilter += ", ";
                    trasmFilter += "" + idTrasm + "";
                }

                queryWhere += " AND A.SYSTEM_ID IN (" + trasmFilter + ")";
            }

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASMISSIONE__TRASM_SINGOLA__TRASM_UTENTE3");

            string UserDB = string.Empty;

            if (dbType.ToUpper() == "SQL")
            {
                UserDB = getUserDB();
                q.setParam("dbuser", UserDB);
            }

            q.setParam("param1", queryColumn);
            q.setParam("param2", queryFrom);
            q.setParam("param3", queryWhere);

            #region Ordinamento

            // Recupero dei filtri di ricerca relarivi all'ordinamento
            FiltroRicerca oracleField = objListaFiltri.Where(e => e.argomento == "ORACLE_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca sqlField = objListaFiltri.Where(e => e.argomento == "SQL_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca profilationField = objListaFiltri.Where(e => e.argomento == "PROFILATION_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca orderDirection = objListaFiltri.Where(e => e.argomento == "ORDER_DIRECTION").FirstOrDefault();

            // Se non è valorizzata la direzione di ordinamento, viene creato un filtro di tipo ORDER_DIRECTION
            if (orderDirection == null)
            {
                orderDirection = new FiltroRicerca()
                {
                    argomento = "ORDER_DIRECTION",
                    valore = "DESC"

                };

            }

            // Function da utilizzare per estrarre i valori del campo profilato da utilizzare per l'ordinamento
            String extractFieldValue = String.Empty, orderCondition = String.Empty;

            if (this.dbType == "SQL")
            {
                // DB SQL Server
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", Convert(int, @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0})) AS CUSTOM_FIELD", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0}) AS CUSTOM_FIELD", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("CUSTOM_FIELD {0}", orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (sqlField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Format(", {0} AS var_desc_ragione", sqlField.valore);
                        orderCondition = String.Format("var_desc_ragione {0}", orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("Convert(datetime, A.DTA_INVIO) {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }

            }
            else
            {
                // DB ORACLE
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", to_number(getValCampoProfDoc(A.DOCNUMBER, {0}))", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", getValCampoProfDoc(A.DOCNUMBER, {0})", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("getValCampoProfDoc(A.DOCNUMBER, {0}) {1}", profilationField.valore, orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (oracleField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("{0} {1}", oracleField.valore, orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("TO_DATE (a.dta_invio, 'dd/MM/yyyy') {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }
            }

            q.setParam("orderCondition", orderCondition);

            #endregion

            string queryString = q.getSQL();

            if (dbType == "SQL" && !string.IsNullOrEmpty(chaiTableDef))
            {
                queryString = chaiTableDef + queryString;
                chaiTableDef = string.Empty;
            }

            logger.Debug(queryString);

            this.ExecuteQuery(out dataSet, "TRASMISSIONI", queryString);
        }

        private string[] GetSystemIDTrasmRicevute(string queryWhere,
                                                    string queryJoin,
                                                    string orderCriteria,
                                                    int requestedPageNumber,
                                                    out int totalPageNumber,
                                                    out int totalRecordCount,
                                                    FiltroRicerca[] objListaFiltri,
                                                    string idAmministrazione)
        {
            string fieldDefinition = "A.SYSTEM_ID AS ID_TRASM";
            string tables = "DPA_TRASMISSIONE A, DPA_TRASM_SINGOLA B, DPA_TRASM_UTENTE C " + queryJoin;
            //queryWhere="A.SYSTEM_ID=B.ID_TRASMISSIONE AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA " + queryWhere;

            //	ODL:
            //			string sqlDefinition="SELECT " + fieldDefinition + " " +
            //									"FROM " + tables + " " +
            //									"WHERE " + queryWhere + " " +
            //									"ORDER BY A.DTA_INVIO DESC,B.ID_TRASMISSIONE DESC";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TRASM_SyIDTrRic");
            q.setParam("param2", tables);
            q.setParam("param3", queryWhere);


            #region Ordinamento

            // Recupero dei filtri di ricerca relarivi all'ordinamento
            FiltroRicerca oracleField = objListaFiltri.Where(e => e.argomento == "ORACLE_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca sqlField = objListaFiltri.Where(e => e.argomento == "SQL_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca profilationField = objListaFiltri.Where(e => e.argomento == "PROFILATION_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca orderDirection = objListaFiltri.Where(e => e.argomento == "ORDER_DIRECTION").FirstOrDefault();

            // Se non è valorizzata la direzione di ordinamento, viene creato un filtro di tipo ORDER_DIRECTION
            if (orderDirection == null)
            {
                orderDirection = new FiltroRicerca()
                {
                    argomento = "ORDER_DIRECTION",
                    valore = "DESC"

                };

            }

            // Function da utilizzare per estrarre i valori del campo profilato da utilizzare per l'ordinamento
            String extractFieldValue = String.Empty, orderCondition = String.Empty;

            if (this.dbType == "SQL")
            {
                // DB SQL Server
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", Convert(int, @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0})) AS CUSTOM_FIELD", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0}) AS CUSTOM_FIELD", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("CUSTOM_FIELD {0}", orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (sqlField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Format(", {0} AS ORDER_STANDARD", sqlField.valore);
                        orderCondition = String.Format("ORDER_STANDARD {0}", orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("Convert(datetime, A.DTA_INVIO) {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }

            }
            else
            {
                // DB ORACLE
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", to_number(getValCampoProfDoc(A.DOCNUMBER, {0}))", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", getValCampoProfDoc(A.DOCNUMBER, {0})", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("getValCampoProfDoc(A.DOCNUMBER, {0}) {1}", profilationField.valore, orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (oracleField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("{0} {1}", oracleField.valore, orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        // Andrea - Errore in esecuzione query per data null o data con anno valorizzato a 2000
                        //orderCondition = String.Format("TO_DATE (a.dta_invio, 'dd/MM/yyyy') {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                        orderCondition = String.Format("a.dta_invio {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }
            }

            fieldDefinition += extractFieldValue;
            q.setParam("param1", fieldDefinition);

            // Impostazione dell'utente amministratore di db
            q.setParam("dbUser", this.getUserDB());
            // Impostazione del parametro di ordinamento
            q.setParam("orderCondition", orderCondition);

            #endregion


            string sqlDefinition = q.getSQL();

            logger.Debug(sqlDefinition);

            DataSet ds = null;

            this.ExecuteQuery(out ds, sqlDefinition);

            if (eliminaTramissioniMultiple)
            {

                DataSet dsTemp = new DataSet();
                // Creo una datatable
                DataTable tableTemp = new DataTable();
                tableTemp.Columns.Add("ID_TRASM", typeof(string));
                // Aggiungo il data table al data set
                dsTemp.Tables.Add(tableTemp);

                string idTrasmTemp = null;

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow row = ds.Tables[0].Rows[i];

                    string idTrasmIns = row["ID_TRASM"].ToString();

                    if (idTrasmIns != idTrasmTemp)
                    {
                        DataRow rowTemp = tableTemp.NewRow();
                        rowTemp["ID_TRASM"] = idTrasmIns;
                        tableTemp.Rows.Add(rowTemp);
                        idTrasmTemp = idTrasmIns;
                    }
                }
                ds = dsTemp;
            }

            return this.GetArraySystemIDTrasmissioni(ds.Tables[0], requestedPageNumber, out totalPageNumber, out totalRecordCount, idAmministrazione);
        }


        #endregion

        #region QueryDettaglioTrasmissione
        public void getDettaglioTrasmissione(ref bool repeatQuery, out DataSet dataSet, string idPeople,
                                        DocsPaVO.utente.Ruolo ruolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                        DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, string systemIDTrasm)
        {
            dataSet = null;

            string queryWhere = "";
            string queryFrom = "";
            string queryColumn = "";
            getQueryTrasmissioni(objOggettoTrasmesso, ref queryWhere, ref queryFrom, ref queryColumn);
            string idCorrispondente = getQueryEffMet1(idPeople);

            queryWhere +=
                " AND A.DTA_INVIO IS NOT NULL " +
                "AND ((B.ID_CORR_GLOBALE = " + ruolo.systemId + " AND C.ID_PEOPLE=" + idPeople + ") OR B.ID_CORR_GLOBALE=" + idCorrispondente;

            queryWhere += ")";
            repeatQuery = getCondFiltri(ref queryWhere, ref queryFrom, objListaFiltri);

            queryWhere += " AND A.SYSTEM_ID = " + systemIDTrasm;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASMISSIONE__TRASM_SINGOLA__TRASM_UTENTE2");

            string UserDB = string.Empty;

            if (dbType.ToUpper() == "SQL")
            {
                UserDB = getUserDB();
                q.setParam("dbuser", UserDB);
            }

            q.setParam("param1", queryColumn);
            q.setParam("param2", queryFrom);
            q.setParam("param3", queryWhere);

            string queryString = q.getSQL();

            logger.Debug(queryString);

            this.ExecuteQuery(out dataSet, "TRASMISSIONI", queryString);
        }

        #endregion

        private string[] GetArraySystemIDTrasmissioni(DataTable tableTrasmissioni,
                                                        int requestedPageNumber,
                                                        out int totalPageNumber,
                                                        out int totalRecordCount,
                                                        string idAmministrazione)
        {
            // Dimensione della pagina
            const int PAGE_SIZE = 8;

            totalPageNumber = 0;
            totalRecordCount = 0;

            int startRecord = (requestedPageNumber * PAGE_SIZE) - PAGE_SIZE;
            // DataRow row = null;

            List<string> list = new List<string>();

            for (int i = 0; i < tableTrasmissioni.Rows.Count; i++)
            {
                DataRow row = tableTrasmissioni.Rows[i];

                string idTrasm = row["ID_TRASM"].ToString();

                if (!list.Contains(idTrasm))
                {
                    if (i >= startRecord && list.Count < PAGE_SIZE)
                    {
                        list.Add(idTrasm);
                    }

                    totalRecordCount++;
                }
            }

            //if (tableTrasmissioni.Rows.Count > 0)
            //{
            //    for (int index = startRecord; index < startRecord + PAGE_SIZE; index++)
            //    {
            //        if (index >= tableTrasmissioni.Rows.Count)
            //            break;

            //        row = tableTrasmissioni.Rows[index];
            //        list.Add(row["ID_TRASM"].ToString());
            //        row = null;
            //    }
            //}

            // Restituzione numero totale dei record
            // totalRecordCount = tableTrasmissioni.Rows.Count;

            /* ABBATANGELI GIANLUIGI
             * Aggiunto il valore di configurazione MAX_ROW_SEARCHABLE
             * che determina il numero massimo di righe accettatte
             * come risultato di una ricerca trasmissioni */
            int maxRowSearchable = Cfg_MAX_ROW_SEARCHABLE(idAmministrazione);

            if (maxRowSearchable == 0 || totalRecordCount <= maxRowSearchable)
            {
                // Calcolo numero pagine totali
                totalPageNumber = (totalRecordCount / PAGE_SIZE);
                if (totalPageNumber * PAGE_SIZE < totalRecordCount)
                    totalPageNumber++;
            }
            else
            {
                /* ABBATANGELI GIANLUIGI
                 * Non carico i documenti perchè raggiunto il numero massimo 
                 * di righe per la ricerca ed imposto numTotPage = -2. */
                totalPageNumber = -2;
            }
            return list.ToArray();
        }

        /* ABBATANGELI GIANLUIGI
         * Caricamento dal database del valore int relativo al 
         * numero massimo di righe accettate come risultato di ricerca */
        /// <summary>
        /// return, numero massimo di righe per le ricerche.
        /// </summary>
        public int Cfg_MAX_ROW_SEARCHABLE(string idAmministrazione)
        {
            int result = 0;
            string value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmministrazione, "MAX_ROW_SEARCHABLE");

            if (string.IsNullOrEmpty(value))
            {
                value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "MAX_ROW_SEARCHABLE");
            }
            if (!string.IsNullOrEmpty(value))
            {
                result = Convert.ToInt32(value);
            }
            return result;
        }
        private string[] GetArraySystemIDTrasmissioniRuolo(DataTable tableTrasmissioni)
        {
            List<string> list = new List<string>();

            for (int i = 0; i < tableTrasmissioni.Rows.Count; i++)
            {
                DataRow row = tableTrasmissioni.Rows[i];

                string idTrasm = row["ID_TRASM"].ToString();

                if (!list.Contains(idTrasm))
                {
                    list.Add(idTrasm);

                    i++;
                }
            }
            return list.ToArray();
        }
        #endregion

        #region svuotamento TDL

        public bool isNoticeActivedTDL(out string noticeDays, string idAmm)
        {
            /*
                SELECT 
                   CHA_ATTIVA_GG_PERM_TODOLIST,NUM_GG_PERM_TODOLIST
                FROM DPA_AMMINISTRA 
                WHERE system_id = idAmm
            */

            bool retValue = false;
            noticeDays = string.Empty;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAmministra2");
            q.setParam("param1", "CHA_ATTIVA_GG_PERM_TODOLIST,NUM_GG_PERM_TODOLIST");
            q.setParam("param2", idAmm);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            System.Data.DataSet ds;
            this.ExecuteQuery(out ds, "AMM", queryString);

            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables["AMM"].Rows[0]["CHA_ATTIVA_GG_PERM_TODOLIST"].ToString() != null &&
                    (ds.Tables["AMM"].Rows[0]["CHA_ATTIVA_GG_PERM_TODOLIST"].ToString().Equals("0") ||
                     ds.Tables["AMM"].Rows[0]["CHA_ATTIVA_GG_PERM_TODOLIST"].ToString().Equals("1")))
                    retValue = Convert.ToBoolean(Convert.ToByte(ds.Tables["AMM"].Rows[0]["CHA_ATTIVA_GG_PERM_TODOLIST"].ToString()));

                noticeDays = ds.Tables["AMM"].Rows[0]["NUM_GG_PERM_TODOLIST"].ToString();
            }
            return retValue;
        }

        public string countTxtOverNoticeDays(string noticeDays, string idPeopleUtente, string idCorrGlobRuolo, string tipoTrasm)
        {
            string retValue = string.Empty;

            /*
                SELECT COUNT(A.SYSTEM_ID)
                FROM DPA_TRASMISSIONE A, DPA_TRASM_SINGOLA B, DPA_TRASM_UTENTE C 
                WHERE A.SYSTEM_ID=B.ID_TRASMISSIONE AND 
	                  B.SYSTEM_ID=C.ID_TRASM_SINGOLA AND					 
	                  A.DTA_INVIO IS NOT NULL AND 
	                  ((B.ID_CORR_GLOBALE = idCorrGlobRuolo AND C.ID_PEOPLE = idPeopleUtente) OR B.ID_CORR_GLOBALE = idCorrGlobUtente) AND 
	                  A.CHA_TIPO_OGGETTO = 'tipoTrasm' AND
	                  C.CHA_IN_TODOLIST = '1' AND
	             --oracle:     TO_DATE(A.DTA_INVIO,'DD/MM/YYYY') < TO_DATE((SYSDATE-NUMTODSINTERVAL(noticeDays,'DAY')),'DD/MM/YYYY')
                 --sqlserver:  DATEDIFF(DAY,A.DTA_INVIO,GETDATE()) > noticeDays
             */

            string idCorrGlobUtente = this.getQueryEffMet1(idPeopleUtente);

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONTA_TRASM_TDL_OLTRE_GG_AVVISO");
            q.setParam("param1", idCorrGlobRuolo);
            q.setParam("param2", idPeopleUtente);
            // q.setParam("param3", idCorrGlobUtente);
            //  q.setParam("param4", tipoTrasm);
            q.setParam("param5", noticeDays);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            this.ExecuteScalar(out retValue, queryString);

            return retValue;
        }

        public string getDatePost(string daysLessTo)
        {
            string retValue = string.Empty;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DATEDIFF_FROM_DUAL");
            q.setParam("param1", daysLessTo);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            this.ExecuteScalar(out retValue, queryString);

            return retValue;
        }

        public bool svuotaTDL(string dataImpostata, string idPeopleUtente, string idGruppoUtente, string idCorrGlobRuolo, string tipoTrasm, bool noWF, DocsPaVO.filtri.FiltroRicerca[] filtri)
        {
            bool retValue;
            //IDataReader dr = null;
            //ArrayList list_ID_TRASM = new ArrayList();
            using (DBProvider dbProvider = new DBProvider())
            {
                //Nuova rimozione... considera anche i filtri
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_ID_TRASM_UTENTE_TODOLIST");
                string filterWhere = string.Empty;
                if (filtri != null && filtri.Length > 2)
                {
                    //filterWhere = " and ";
                    filterWhere = getwhereFilter(filtri);
                    query.setParam("docORFasc", "");
                }
                else
                    query.setParam("docORFasc", "(ID_PROFILE > 0 or ID_PROJECT > 0) AND");
                query.setParam("idPeople", idPeopleUtente);
                query.setParam("idGruppo", idGruppoUtente);
                string userDB = getUserDB();
                if (!string.IsNullOrEmpty(userDB))
                    query.setParam("dbuser", userDB);

                query.setParam("filters", filterWhere);

                string commandText = query.getSQL();
                logger.Debug(commandText);
                //DataSet ds = new DataSet();

                //using (dr = dbProvider.ExecuteReader(commandText))
                //{
                //    if (dr == null)
                //    {
                //        return false;
                //    }
                //    if (dr != null && dr.FieldCount > 0)
                //    {
                //        while (dr.Read())
                //        {
                //            list_ID_TRASM.Add(dr.GetValue(0).ToString());
                //        }
                //    }
                //}
                //if (dr != null && (!dr.IsClosed))
                //    dr.Close();

                string queryName = "U_TDL_OLTRE_GG_AVVISO";

                //A.CHA_TIPO_OGGETTO = '@param4@' AND
                //tolto nella query per todolist unificata
                #region commento
                /*
             
             elimina tutte indistintamente...
            
             UPDATE DPA_TRASM_UTENTE SET CHA_IN_TODOLIST = '0', DTA_RIMOZIONE_TODOLIST = TO_DATE(SYSDATE,'DD/MM/YYYY')
             WHERE ID_TRASM_SINGOLA IN (
                            SELECT B.SYSTEM_ID
                            FROM DPA_TRASMISSIONE A, DPA_TRASM_SINGOLA B, DPA_TRASM_UTENTE C 
                            WHERE A.SYSTEM_ID=B.ID_TRASMISSIONE AND 
                                  B.SYSTEM_ID=C.ID_TRASM_SINGOLA AND					 
                                  A.DTA_INVIO IS NOT NULL AND 
                                  ((B.ID_CORR_GLOBALE = idCorrGlobRuolo AND C.ID_PEOPLE = idPeopleUtente) OR B.ID_CORR_GLOBALE = idCorrGlobUtente) AND 
                                  A.CHA_TIPO_OGGETTO = 'tipoTrasm' AND
                                  C.CHA_IN_TODOLIST = '1' AND
                                  A.DTA_INVIO < dataImpostata
                             )  
             
             oppure tutte quelle che non necessitano accettazione / rifiuto...
             
             UPDATE DPA_TRASM_UTENTE SET CHA_IN_TODOLIST = '0', DTA_RIMOZIONE_TODOLIST = TO_DATE(SYSDATE,'DD/MM/YYYY')
             WHERE ID_TRASM_SINGOLA IN (
                            SELECT B.SYSTEM_ID
                            FROM DPA_TRASMISSIONE A, DPA_TRASM_SINGOLA B, DPA_TRASM_UTENTE C, DPA_RAGIONE_TRASM D
                            WHERE A.SYSTEM_ID=B.ID_TRASMISSIONE AND 
                                  B.SYSTEM_ID=C.ID_TRASM_SINGOLA AND
                                  B.ID_RAGIONE=D.SYSTEM_ID AND
                                  D.CHA_TIPO_RAGIONE NOT IN ('W') AND
                                  A.DTA_INVIO IS NOT NULL AND 
                                  ((B.ID_CORR_GLOBALE = idCorrGlobRuolo AND C.ID_PEOPLE = idPeopleUtente) OR B.ID_CORR_GLOBALE = idCorrGlobUtente) AND 
                                  A.CHA_TIPO_OGGETTO = 'tipoTrasm' AND
                                  C.CHA_IN_TODOLIST = '1' AND
                                  A.DTA_INVIO < dataImpostata
                             )  
             
            */
                #endregion
                string idCorrGlobUtente = this.getQueryEffMet1(idPeopleUtente);

                if (noWF)
                    queryName = "U_TDL_OLTRE_GG_AVVISO_NO_WF";

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);

                q.setParam("param1", idCorrGlobRuolo);
                q.setParam("param2", idPeopleUtente);
                q.setParam("param3", idCorrGlobUtente);
                //q.setParam("param4", tipoTrasm);
                q.setParam("param5", DocsPaDbManagement.Functions.Functions.ToDate(dataImpostata, false));


                //if (list_ID_TRASM != null && list_ID_TRASM.Count > 0)
                //{
                //string idTrasm = "(";
                //for (int i = 0; i < list_ID_TRASM.Count; i++)
                //{
                //    idTrasm += list_ID_TRASM[i].ToString() + ",";
                //}
                //idTrasm = idTrasm.Substring(0, idTrasm.Length - 1);
                //idTrasm = idTrasm + ")";
                //}
                //else
                //    q.setParam("paramTrasm", "");

                string idTrasm = "(" + commandText + ")";
                q.setParam("paramTrasm", " and system_id in " + idTrasm);

                string queryString = q.getSQL();
                logger.Debug(queryString);

                //retValue = this.ExecuteNonQuery(queryString);
                int rowsAffected;
                retValue = this.ExecuteNonQuery(queryString, out rowsAffected);

                //*****************************ELIMINAZIONE DAL CENTRO NOTIFICHE**********************
                if(retValue)
                {
                    if (noWF)
                    {
                        List<string> idNotify = new List<string>();
                        DataSet ds = new DataSet();
                        string query2 = string.Empty;
                        string condition = string.Empty;
                        DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_NOTIFY_NO_WF");
                        q2.setParam("param1", idPeopleUtente);
                        q2.setParam("param2", idGruppoUtente);
                        q2.setParam("param3", DocsPaDbManagement.Functions.Functions.ToDate(dataImpostata, false));
                        query2 = q2.getSQL();
                        if (this.ExecuteQuery(out ds, "notify", query2))
                        {
                            if (ds.Tables["notify"] != null && ds.Tables["notify"].Rows.Count > 0)
                            {
                                foreach (DataRow row in ds.Tables["notify"].Rows)
                                {
                                    q2 = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_NOTIFY_HISTORY_MULTIPLE");
                                    condition = "SYSTEM_ID = " + row["SYSTEM_ID"].ToString();
                                    q2.setParam("condition", condition);
                                    if(this.ExecuteNonQuery(q2.getSQL()))
                                    {
                                        q2 = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_NOTIFY");
                                        q2.setParam("idNotification", row["SYSTEM_ID"].ToString());
                                        this.ExecuteNonQuery(q2.getSQL());
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string condition;
                        DataSet ds;
                        DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_NOTIFY");
                        q1.setParam("idGroup", idGruppoUtente);
                        q1.setParam("idPeople", idPeopleUtente);
                        if (this.ExecuteQuery(out ds, "notify", q1.getSQL()))
                        {
                            if (ds.Tables["notify"] != null && ds.Tables["notify"].Rows.Count > 0)
                            {
                                foreach (DataRow row in ds.Tables["notify"].Rows)
                                {
                                    if (Convert.ToDateTime(row["dta_notify"]).CompareTo(Convert.ToDateTime(dataImpostata)) < 0)
                                    {
                                        q1 = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_NOTIFY_HISTORY_MULTIPLE");
                                        condition = "SYSTEM_ID = " + row["ID_NOTIFY"].ToString();
                                        q1.setParam("condition", condition);
                                        if (this.ExecuteNonQuery(q1.getSQL()))
                                        {
                                            q1 = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_NOTIFY");
                                            q1.setParam("idNotification", row["ID_NOTIFY"].ToString());
                                            this.ExecuteNonQuery(q1.getSQL());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //*****************************FINE ELIMINAZIONE DAL CENTRO NOTIFICHE**********************

                logger.Debug("Trasmissioni eliminate: " + Convert.ToString(rowsAffected));
            }

            return retValue;
        }
        #endregion

        public DataSet checkScadenzeDoc(string param1)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TRASM_SCADENZE_DOC");
                queryMng.setParam("param1", param1);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - checkScadenze - Trasmissione.cs - QUERY : " + commandText);
                logger.Debug("SQL - checkScadenze - Trasmissione.cs - QUERY : " + commandText);

                DataSet dsDocInScadenza = new DataSet();
                dbProvider.ExecuteQuery(dsDocInScadenza, commandText);

                return dsDocInScadenza;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataSet checkScadenzeMitt(string docNumber, string param1)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TRASM_SCADENZE_MITT");
                queryMng.setParam("docNumber", docNumber);
                queryMng.setParam("param1", param1);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - checkScadenzeMitt - Trasmissione.cs - QUERY : " + commandText);
                logger.Debug("SQL - checkScadenzeMitt - Trasmissione.cs - QUERY : " + commandText);

                DataSet dsScadenzeMitt = new DataSet();
                dbProvider.ExecuteQuery(dsScadenzeMitt, commandText);

                return dsScadenzeMitt;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataSet checkScadenzeDest(string docNumber, string param1)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TRASM_SCADENZE_DEST");
                queryMng.setParam("docNumber", docNumber);
                queryMng.setParam("param1", param1);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - checkScadenzeDest - Trasmissione.cs - QUERY : " + commandText);
                logger.Debug("SQL - checkScadenzeDest - Trasmissione.cs - QUERY : " + commandText);

                DataSet dsScadenzeDest = new DataSet();
                dbProvider.ExecuteQuery(dsScadenzeDest, commandText);

                return dsScadenzeDest;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        #region new_todolist
        /// <summary>
        /// Reperimento numero totale di righe todolist
        /// </summary>
        /// <param name="docOrFasc"></param>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <param name="registri"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private int getCountMyDocTodoList(string docOrFasc,
                                    string idPeople, string idGruppo,
                                    string registri, DocsPaVO.filtri.FiltroRicerca[] filter)
        {
            int retValue = 0;

            try
            {
                DocsPaUtils.Query queryMng = null;

                if (docOrFasc == "D")
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ROWS_TODO_LIST_DOCUMENTI");
                else if (docOrFasc == "F")
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ROWS_TODO_LIST_FASCICOLI");
                else
                    throw new ApplicationException(string.Format("Tipologia di ricerca non riconosciuta: {0}", docOrFasc));

                // non è necessario usare i filtri se non vengono popolati
                //dal momento che vengono cmq istanziati e non popolati
                //per non rimuovere la logica costruita controllo che
                // la dimensione sia superiore a 3 in quanto quel numero
                //rappresenta i filtri minimi caricati in argomento ma senza valore!!!
                string filterWhere = string.Empty;
                if (filter != null && filter.Length > 3)
                    filterWhere = getwhereFilter(filter);

                queryMng.setParam("idPeople", idPeople);
                queryMng.setParam("idGruppo", idGruppo);
                queryMng.setParam("registri", registri);
                queryMng.setParam("filters", filterWhere);

                string commandText = queryMng.getSQL();
                logger.Debug(commandText);

                using (DBProvider dbProvider = new DBProvider())
                {
                    string field;
                    if (dbProvider.ExecuteScalar(out field, commandText))
                        retValue = Convert.ToInt32(field);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore in getCountMyDocTodoList:\n{0}", ex.Message));
            }

            return retValue;
        }

        private int getCountMyAllItemTodoList(string idPeople, string idGruppo,
                                    string registri, DocsPaVO.filtri.FiltroRicerca[] filter)
        {
            int retValue = 0;

            try
            {
                DocsPaUtils.Query queryMng = null;

                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ROWS_TODOLIST");

                //non è necessario usare i filtri se non vengono popolati
                //dal momento che vengono cmq istanziati e non popolati
                //per non rimuovere la logica costruita controllo che
                //la dimensione sia superiore a 3 in quanto quel numero
                //rappresenta i filtri minimi caricati in argomento ma senza valore!!!
                string filterWhere = string.Empty;
                if (filter != null)// && filter.Length > 2)
                {
                    filterWhere = getwhereFilter(filter);

                    if (string.IsNullOrEmpty(filterWhere))
                        if (!string.IsNullOrEmpty(registri))
                            queryMng.setParam("docORfasc", "( (ID_PROFILE > 0 and TDL.id_registro in (" + registri + ")) or ID_PROJECT > 0) AND");
                        else
                            queryMng.setParam("docORfasc", "(ID_PROFILE > 0 or ID_PROJECT > 0) AND");
                    else
                        queryMng.setParam("docORfasc", "");
                }
                else
                    if (!string.IsNullOrEmpty(registri))
                        queryMng.setParam("docORfasc", "( (ID_PROFILE > 0 and TDL.id_registro in (" + registri + ") ) or ID_PROJECT > 0) AND");
                    else
                        queryMng.setParam("docORfasc", "(ID_PROFILE > 0 or ID_PROJECT > 0) AND");

                //queryMng.setParam("docORfasc", "(ID_PROFILE > 0 or ID_PROJECT > 0) AND");

                queryMng.setParam("idPeople", idPeople);
                queryMng.setParam("idGruppo", idGruppo);
                //queryMng.setParam("registri", " AND ID_REGISTRO IN (" + registri + ")");
                queryMng.setParam("filters", filterWhere);
                string commandText = queryMng.getSQL();
                logger.Debug(commandText);

                using (DBProvider dbProvider = new DBProvider())
                {
                    string field;
                    if (dbProvider.ExecuteScalar(out field, commandText))
                        retValue = Convert.ToInt32(field);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore in getCountMyAllItemTodoList:\n{0}", ex.Message));
            }

            return retValue;
        }

        //restituisce la lista delle trasmissioni in todolist non ancora lette
        private int getCountTrasmNonLette(string idPeople, string idGruppo,
                                    string registri, DocsPaVO.filtri.FiltroRicerca[] filter)
        {
            int retValue = 0;

            try
            {
                DocsPaUtils.Query queryMng = null;

                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ROWS_TODOLIST");

                // non è necessario usare i filtri se non vengono popolati
                //dal momento che vengono cmq istanziati e non popolati
                //per non rimuovere la logica costruita controllo che
                // la dimensione sia superiore a 3 in quanto quel numero
                //rappresenta i filtri minimi caricati in argomento ma senza valore!!!
                string filterWhere = string.Empty;
                if (filter != null && filter.Length > 2)
                {
                    filterWhere = getwhereFilter(filter);
                    if (string.IsNullOrEmpty(filterWhere))
                        if (!string.IsNullOrEmpty(registri))
                            queryMng.setParam("docORfasc", "( (ID_PROFILE > 0 and TDL.id_registro in (" + registri + ") ) or ID_PROJECT > 0) AND");
                        else
                            queryMng.setParam("docORfasc", "(ID_PROFILE > 0 or ID_PROJECT > 0) AND");
                    else
                        queryMng.setParam("docORfasc", "");
                    //queryMng.setParam("docORfasc", "");
                }
                else
                    if (!string.IsNullOrEmpty(registri))
                        queryMng.setParam("docORfasc", "( (ID_PROFILE > 0 and TDL.id_registro in (" + registri + ") ) or ID_PROJECT > 0) AND");
                    else
                        queryMng.setParam("docORfasc", "(ID_PROFILE > 0 or ID_PROJECT > 0) AND");

                //queryMng.setParam("docORfasc", "(ID_PROFILE > 0 or ID_PROJECT > 0) AND");

                filterWhere += "AND dta_vista = " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753");
                queryMng.setParam("idPeople", idPeople);
                queryMng.setParam("idGruppo", idGruppo);
                //queryMng.setParam("registri", " AND ID_REGISTRO IN (" + registri + ")");
                queryMng.setParam("filters", filterWhere);
                string commandText = queryMng.getSQL();
                logger.Debug(commandText);

                using (DBProvider dbProvider = new DBProvider())
                {
                    string field;
                    if (dbProvider.ExecuteScalar(out field, commandText))
                        retValue = Convert.ToInt32(field);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore in getCountTrasmNonLette:\n{0}", ex.Message));
            }

            return retValue;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="docOrFasc"></param>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <param name="registri"></param>
        /// <param name="filter"></param>
        /// <param name="requestedPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecordCount"></param>
        /// <returns></returns>
        public ArrayList getMyTodoList(string docOrFasc,
                                        string idPeople, string idGruppo, string registri,
                                        DocsPaVO.filtri.FiltroRicerca[] filter,
                                        int requestedPage, int pageSize, out int totalRecordCount)
        {
            ArrayList list = new ArrayList();
            totalRecordCount = 0;

            try
            {
                // Reperimento numero totale di record estratti dalla query
                totalRecordCount = this.getCountMyAllItemTodoList(idPeople, idGruppo, registri, filter);
                //totalRecordCount = this.getCountMyDocTodoList(docOrFasc, idPeople, idGruppo, registri, filter);

                DocsPaUtils.Query queryMng = null;

                if (docOrFasc == "D")
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_TODO_LIST_DOCUMENTI");
                else if (docOrFasc == "F")
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_TODO_LIST_FASCICOLI");
                else
                    throw new ApplicationException(string.Format("Tipologia di ricerca non riconosciuta: {0}", docOrFasc));

                // non è necessario usare i filtri se non vengono popolati
                //dal momento che vengono cmq istanziati e non popolati
                //per non rimuovere la logica costruita controllo che
                // la dimensione sia superiore a 3 in quanto quel numero
                //rappresenta i filtri minimi caricati in argomento ma senza valore!!!
                string filterWhere = string.Empty;
                if (filter != null && filter.Length > 3)
                    filterWhere = getwhereFilter(filter);

                queryMng.setParam("idPeople", idPeople);
                queryMng.setParam("idGruppo", idGruppo);

                if (docOrFasc == "D")
                {
                    // Parametro previsto solamente per la ricerca dei documenti
                    queryMng.setParam("registri", registri);
                }

                string userDB = getUserDB();
                if (!string.IsNullOrEmpty(userDB))
                    queryMng.setParam("dbuser", userDB);

                queryMng.setParam("filters", filterWhere);

                // Reperimento ordinamento query in base al filtro inserito
                string orderCriteria = this.getorderFilter(filter, docOrFasc);
                queryMng.setParam("order", orderCriteria);

                // Determina il num di pagine totali

                int numTotPage = (totalRecordCount / pageSize);
                if (numTotPage != 0)
                {
                    if ((totalRecordCount % numTotPage) > 0) numTotPage++;
                }
                else numTotPage = 1;

                int startRow = ((requestedPage * pageSize) - pageSize) + 1;
                int endRow = (startRow - 1) + pageSize;

                queryMng.setParam("startRow", startRow.ToString());
                queryMng.setParam("endRow", endRow.ToString());

                // INIZIO - Parametri specifici per SqlServer
                // il numero totale di righe da estrarre equivale 
                // al limite inferiore dell'ultima riga da estrarre
                int pageSizeSqlServer = pageSize;
                int totalRowsSqlServer = (requestedPage * pageSize);
                if ((totalRecordCount - totalRowsSqlServer) <= 0)
                {
                    pageSizeSqlServer -= System.Math.Abs(totalRecordCount - totalRowsSqlServer);
                    totalRowsSqlServer = totalRecordCount;
                }

                queryMng.setParam("pageSize", pageSizeSqlServer.ToString()); // Dimensione pagina
                queryMng.setParam("totalRows", totalRowsSqlServer.ToString());

                // Swap ordinamento:
                string reverseOrderCriteria = orderCriteria.Replace("ASC", "SWAP");
                reverseOrderCriteria = reverseOrderCriteria.Replace("DESC", "ASC");
                reverseOrderCriteria = reverseOrderCriteria.Replace("SWAP", "DESC");
                queryMng.setParam("reverseOrder", reverseOrderCriteria);
                // FINE - Parametri specifici per SqlServer

                string commandText = queryMng.getSQL();
                logger.Debug(commandText);

                DataSet ds = new DataSet();

                using (DBProvider dbProvider = new DBProvider())
                    dbProvider.ExecuteQuery(ds, commandText);

                if (ds != null && ds.Tables[0] != null)
                    list = this.GetListTrasmissioni(ds.Tables[0], docOrFasc, idPeople, idGruppo, orderCriteria);
                else
                    logger.Debug("Errore in getMyTodoList");
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore in getMyDocTodoList:\n{0}", ex.Message));
            }

            return list;
        }

        //nuovo metodo per la todolist unificata (documenti e fascicoli insieme)
        public ArrayList getMyNewTodoList(string idPeople, string idGruppo, string registri,
                                        DocsPaVO.filtri.FiltroRicerca[] filter,
                                        int requestedPage, int pageSize, out int totalRecordCount, out int totalTrasmNonViste)
        {
            ArrayList list = new ArrayList();
            totalRecordCount = 0;
            totalTrasmNonViste = 0;
            try
            {
                DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();
                if (obj.isFiltroAooEnabled())
                    registri = "";
                // Reperimento numero totale di record estratti dalla query
                totalRecordCount = this.getCountMyAllItemTodoList(idPeople, idGruppo, registri, filter);
                //nuovo requisito: nel totale delle trasmissioni si vuole sapere quante di queste non sono 
                //state ancora lette
                totalTrasmNonViste = this.getCountTrasmNonLette(idPeople, idGruppo, registri, filter);
                DocsPaUtils.Query queryMng = null;

                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_TODOLIST");

                // non è necessario usare i filtri se non vengono popolati
                //dal momento che vengono cmq istanziati e non popolati
                //per non rimuovere la logica costruita controllo che
                // la dimensione sia superiore a 3 in quanto quel numero
                //rappresenta i filtri minimi caricati in argomento ma senza valore!!!
                string filterWhere = string.Empty;
                if (filter != null && filter.Length > 2)
                {
                    filterWhere = getwhereFilter(filter);
                    if (string.IsNullOrEmpty(filterWhere))
                        if (!string.IsNullOrEmpty(registri))
                            queryMng.setParam("docORFasc", "( (ID_PROFILE > 0 and id_registro in (" + registri + ")  ) or ID_PROJECT > 0) AND");
                        else
                            queryMng.setParam("docORFasc", "(ID_PROFILE > 0 or ID_PROJECT > 0) AND");
                    else
                        queryMng.setParam("docORFasc", "");

                }
                else
                    if (!string.IsNullOrEmpty(registri))
                        queryMng.setParam("docORFasc", "( (ID_PROFILE > 0 and id_registro in (" + registri + ")  ) or ID_PROJECT > 0) AND");
                    else
                        queryMng.setParam("docORFasc", "(ID_PROFILE > 0 or ID_PROJECT > 0) AND");
                queryMng.setParam("idPeople", idPeople);
                queryMng.setParam("idGruppo", idGruppo);


                queryMng.setParam("registri", registri);

                string userDB = getUserDB();
                if (!string.IsNullOrEmpty(userDB))
                    queryMng.setParam("dbuser", userDB);

                queryMng.setParam("filters", filterWhere);

                // Reperimento ordinamento query in base al filtro inserito
                string orderCriteria = this.getNewOrderFilter(filter);
                queryMng.setParam("order", orderCriteria);

                // Determina il num di pagine totali



                int numTotPage = (totalRecordCount / pageSize);
                if (numTotPage != 0)
                {
                    if ((totalRecordCount % numTotPage) > 0) numTotPage++;
                }
                else numTotPage = 1;

                int startRow = ((requestedPage * pageSize) - pageSize) + 1;
                int endRow = (startRow - 1) + pageSize;

                queryMng.setParam("startRow", startRow.ToString());
                queryMng.setParam("endRow", endRow.ToString());

                // INIZIO - Parametri specifici per SqlServer
                // il numero totale di righe da estrarre equivale 
                // al limite inferiore dell'ultima riga da estrarre
                int pageSizeSqlServer = pageSize;
                int totalRowsSqlServer = (requestedPage * pageSize);
                if ((totalRecordCount - totalRowsSqlServer) <= 0)
                {
                    pageSizeSqlServer -= System.Math.Abs(totalRecordCount - totalRowsSqlServer);
                    totalRowsSqlServer = totalRecordCount;
                }

                queryMng.setParam("pageSize", pageSizeSqlServer.ToString()); // Dimensione pagina
                queryMng.setParam("totalRows", totalRowsSqlServer.ToString());

                // Swap ordinamento:
                string reverseOrderCriteria = orderCriteria.Replace("ASC", "SWAP");
                reverseOrderCriteria = reverseOrderCriteria.Replace("DESC", "ASC");
                reverseOrderCriteria = reverseOrderCriteria.Replace("SWAP", "DESC");
                queryMng.setParam("reverseOrder", reverseOrderCriteria);

                // FINE - Parametri specifici per SqlServer


                string commandText = queryMng.getSQL();
                logger.Debug(commandText);

                DataSet ds = new DataSet();

                using (DBProvider dbProvider = new DBProvider())
                    dbProvider.ExecuteQuery(ds, commandText);

                if (ds != null && ds.Tables[0] != null)
                    list = this.GetListTrasmissioni(ds.Tables[0], "T", idPeople, idGruppo, orderCriteria);
                else
                    logger.Debug("Errore in getMyTodoList");
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore in getMyDocTodoList:\n{0}", ex.Message));
            }

            return list;
        }


        public ArrayList getMyNewTodoListMigrazione(string idPeople, string idGruppo)
        {
            ArrayList list = new ArrayList();


            DocsPaUtils.Query queryMng = null;

            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_TODOLIST_MIGRAZIONE");

            queryMng.setParam("docORFasc", "(ID_PROFILE > 0 or ID_PROJECT > 0) AND");
            queryMng.setParam("idPeople", idPeople);
            queryMng.setParam("idGruppo", idGruppo);

            string userDB = getUserDB();
            if (!string.IsNullOrEmpty(userDB))
                queryMng.setParam("dbuser", userDB);


            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            DataSet ds = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
                dbProvider.ExecuteQuery(ds, commandText);

            if (ds != null && ds.Tables[0] != null)
                list = this.GetListTrasmissioni(ds.Tables[0], "T", idPeople, idGruppo, string.Empty);
            else
                logger.Debug("Errore in getMyTodoList");


            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docOrFasc"></param>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <param name="registri"></param>
        /// <param name="filter"></param>
        /// <param name="objectId">Lista dei systemid degli elementi selezionati</param>
        /// <returns></returns>
        public ArrayList getTodoList(string docOrFasc,
                                        string idPeople, string idGruppo, string registri,
                                        DocsPaVO.filtri.FiltroRicerca[] filter,
                                        String[] objectId)
        {
            ArrayList list = new ArrayList();

            try
            {
                DocsPaUtils.Query queryMng = null;

                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_EXPORT_TODO_LIST");

                // non è necessario usare i filtri se non vengono popolati
                //dal momento che vengono cmq istanziati e non popolati
                //per non rimuovere la logica costruita controllo che
                // la dimensione sia superiore a 3 in quanto quel numero
                //rappresenta i filtri minimi caricati in argomento ma senza valore!!!
                string filterWhere = string.Empty;
                if (filter != null && filter.Length > 2)
                {
                    filterWhere = getwhereFilter(filter);

                    queryMng.setParam("docORFasc", "");
                }
                else
                    queryMng.setParam("docORFasc", "(ID_PROFILE > 0 or ID_PROJECT > 0) AND");
                queryMng.setParam("idPeople", idPeople);
                queryMng.setParam("idGruppo", idGruppo);

                queryMng.setParam("registri", registri);

                string userDB = getUserDB();
                if (!string.IsNullOrEmpty(userDB))
                    queryMng.setParam("dbuser", userDB);

                queryMng.setParam("filters", filterWhere);

                // Reperimento ordinamento query in base al filtro inserito
                string orderCriteria = this.getorderFilter(filter, docOrFasc);
                queryMng.setParam("order", orderCriteria);

                string commandText = queryMng.getSQL();

                // Se ci sono elementi selezionati, viene aggiunto un filtro sui system id
                if (objectId != null &&
                    objectId.Length > 0)
                {
                    commandText = "SELECT * FROM (" + commandText + ") WHERE ";

                    if (docOrFasc.Equals("doc"))
                        filterWhere += "ID_PROFILE IN (";
                    else
                        filterWhere += "ID_PROJECT IN (";

                    foreach (string id in objectId)
                        filterWhere += id + ",";

                    filterWhere = filterWhere.Remove(filterWhere.Length - 1);
                    filterWhere += ")";

                }

                logger.Debug(commandText);
                logger.Debug(commandText);
                DataSet ds = new DataSet();

                using (DBProvider dbProvider = new DBProvider())
                    dbProvider.ExecuteQuery(ds, commandText);

                if (ds != null && ds.Tables[0] != null)
                    list = this.GetListTrasmissioni(ds.Tables[0], "T", idPeople, idGruppo, orderCriteria);
                else
                    logger.Debug("Errore in getTodoList");
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore in getTodoList:\n{0}", ex.Message));
            }

            return list;
        }

        //Vecchio metodo con gestione  non unificata di documenti e fascicoli
        public ArrayList getTodoListOLD(string docOrFasc,
                                        string idPeople, string idGruppo, string registri,
                                        DocsPaVO.filtri.FiltroRicerca[] filter)
        {
            ArrayList list = new ArrayList();

            try
            {
                DocsPaUtils.Query queryMng = null;

                if (docOrFasc == "D")
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_EXPORT_TODO_LIST_DOCUMENTI");
                else if (docOrFasc == "F")
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_EXPORT_TODO_LIST_FASCICOLI");
                else
                    throw new ApplicationException(string.Format("Tipologia di ricerca non riconosciuta: {0}", docOrFasc));

                // non è necessario usare i filtri se non vengono popolati
                //dal momento che vengono cmq istanziati e non popolati
                //per non rimuovere la logica costruita controllo che
                // la dimensione sia superiore a 3 in quanto quel numero
                //rappresenta i filtri minimi caricati in argomento ma senza valore!!!
                string filterWhere = string.Empty;
                if (filter != null && filter.Length > 3)
                    filterWhere = getwhereFilter(filter);

                queryMng.setParam("idPeople", idPeople);
                queryMng.setParam("idGruppo", idGruppo);

                if (docOrFasc == "D")
                {
                    // Parametro previsto solamente per la ricerca dei documenti
                    queryMng.setParam("registri", registri);
                }

                string userDB = getUserDB();
                if (!string.IsNullOrEmpty(userDB))
                    queryMng.setParam("dbuser", userDB);

                queryMng.setParam("filters", filterWhere);

                // Reperimento ordinamento query in base al filtro inserito
                string orderCriteria = this.getorderFilter(filter, docOrFasc);
                queryMng.setParam("order", orderCriteria);

                string commandText = queryMng.getSQL();
                logger.Debug(commandText);

                DataSet ds = new DataSet();

                using (DBProvider dbProvider = new DBProvider())
                    dbProvider.ExecuteQuery(ds, commandText);

                if (ds != null && ds.Tables[0] != null)
                    list = this.GetListTrasmissioni(ds.Tables[0], docOrFasc, idPeople, idGruppo, orderCriteria);
                else
                    logger.Debug("Errore in getMyTodoList");
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore in getTodoListOLD:\n{0}", ex.Message));
            }

            return list;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="tipoTrasmissione"></param>
        /// <param name="currentIdPeople"></param>
        /// <param name="currentIdGruppo"></param>
        /// <param name="filterOrder"></param>
        /// <returns></returns>
        public ArrayList GetListTrasmissioni(DataTable dataTable, string tipoTrasmissione, string currentIdPeople, string currentIdGruppo, string filterOrder)
        {
            logger.Info("BEGIN");
            ArrayList todolist = new ArrayList();
            string sort = string.Empty;

            // Creazione dataview e impostazione ordinamento definitivo
            DataView view = dataTable.DefaultView;
            if (filterOrder != "")
            {
                sort = filterOrder + ",";
            }
            else
                //default
                sort = " DTA_INVIO DESC, ID_PROFILE DESC,";

            if (tipoTrasmissione == "D")
                view.Sort = sort + "  ID_RAGIONE_TRASM ASC, ID_PEOPLE_DEST ASC, ID_RUOLO_DEST DESC";
            else if (tipoTrasmissione == "F" || tipoTrasmissione == "T")
                view.Sort = sort + "  ID_PEOPLE_DEST ASC, ID_RUOLO_DEST DESC";
            logger.Debug("view.Sort: " + view.Sort);
            List<string> includedList = new List<string>();

            foreach (DataRowView drv in view)
            {
                if (drv.Row.Table.Columns.Contains("SEGNA_DOCNUMBER"))
                {
                    logger.Debug("SEGNA_DOCNUMBER: " + drv.Row["SEGNA_DOCNUMBER"].ToString());
                    if (!string.IsNullOrEmpty(drv.Row["SEGNA_DOCNUMBER"].ToString()))
                    {
                        Caching cache = new Caching();
                        string docnumber = cache.GET_DOCNUMER_BY_SEGNATURA(drv.Row["SEGNA_DOCNUMBER"].ToString());
                        if (!string.IsNullOrEmpty(docnumber))
                        {
                            DocsPaVO.Caching.InfoFileCaching info = cache.getFileCache(docnumber);
                            if (info != null &&
                                !string.IsNullOrEmpty(info.ext))
                            {
                                drv.Row["CHA_IMG"] = info.ext;
                            }
                        }
                    }
                }
                logger.Debug("calcolo key");

                string key = drv.Row["ID_TRASMISSIONE"].ToString() + "_" +
                                drv.Row["ID_RAGIONE_TRASM"].ToString();

                bool canAdd = false;
                logger.Debug("key: " + key);
                if (!includedList.Contains(key))
                {
                    if (drv.Row["ID_RUOLO_DEST"].ToString() == currentIdGruppo)
                        canAdd = true;

                    else if (drv.Row["ID_RUOLO_DEST"].ToString() == "0")
                        canAdd = true;

                    if (canAdd)
                    {
                        // La trasmissione è sicuramente visualizzata
                        if (tipoTrasmissione == "D")
                            todolist.Add(CreateInfoDocumento(drv.Row, currentIdGruppo, currentIdPeople));
                        if (tipoTrasmissione == "F")
                            todolist.Add(CreateInfoFascicolo(drv.Row, currentIdGruppo, currentIdPeople));
                        if (tipoTrasmissione == "T")
                            todolist.Add(CreateInfoTodoList(drv.Row, currentIdGruppo, currentIdPeople));

                        includedList.Add(key);
                    }
                }
            }
            logger.Info("END");
            return todolist;
        }


        #region old_method
        //public ArrayList getMyDocTodoList(string idPeople, string idGruppo, string registri, DocsPaVO.filtri.FiltroRicerca[] filter)
        //{
        //    DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
        //    ArrayList dati = new ArrayList();
        //    string commandText = String.Empty;
        //    string filterWhere = String.Empty;
        //    string filterOrder = String.Empty;
        //    string userDB = String.Empty;

        //    DocsPaUtils.Query queryMng = null;
        //    try
        //    {
        //        if (dbType.ToUpper() == "SQL")
        //        {
        //            userDB = getUserDB();
        //        }

        //        System.Data.DataSet dsout = new DataSet();
        //        // non è necessario usare i filtri se non vengono popolati
        //        //dal momento che vengono cmq istanziati e non popolati
        //        //per non rimuovere la logica costruita controllo che
        //        // la dimensione sia superiore a 3 in quanto quel numero
        //        //rappresenta i filtri minimi caricati in argomento ma senza valore!!!
        //        if ((filter != null) && (filter.Length > 3))
        //        {
        //            filterWhere = getwhereFilter(filter);
        //            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_MY_DOC_TODOLIST_FILTERED");
        //            queryMng.setParam("idPeople", idPeople);
        //            queryMng.setParam("idGruppo", idGruppo);
        //            queryMng.setParam("registri", registri);
        //            queryMng.setParam("filtri", filterWhere);
        //            //necessita di utente db SQL per la gestione della funzione vardescribe
        //            if ((userDB != null) && (userDB != ""))
        //            {
        //                queryMng.setParam("dbuser", userDB);
        //            }
        //            // commandText = queryMng.getSQL();
        //            //   logger.Debug("SQL -  getMyDocTodoListWithFilter - Trasmissione.cs - QUERY : " + commandText);

        //        }
        //        else
        //        {

        //            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_MY_DOC_TODOLIST");
        //            queryMng.setParam("idPeople", idPeople);
        //            queryMng.setParam("idGruppo", idGruppo);
        //            queryMng.setParam("registri", registri);
        //            if ((userDB != null) && (userDB != ""))
        //            {
        //                queryMng.setParam("dbuser", userDB);
        //            }
        //            // commandText = queryMng.getSQL();
        //        }
        //        //set order
        //        filterOrder = getorderFilter(filter, "D");
        //        queryMng.setParam("order", filterOrder);

        //        commandText = queryMng.getSQL();
        //        logger.Debug("SQL -  getMyDocTodoListNoFilter - Trasmissione.cs - QUERY : " + commandText);

        //        dbProvider.ExecuteQuery(dsout, commandText);


        //        return this.GetListTrasmissioni(dsout.Tables[0], "D", idPeople, idGruppo, filterOrder);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        //public ArrayList getMyFascTodoList(string idPeople, string idGruppo, DocsPaVO.filtri.FiltroRicerca[] filter)
        //{
        //    DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
        //    ArrayList dati = new ArrayList();
        //    string commandText = String.Empty;
        //    string filterWhere = String.Empty;
        //    string filterOrder = String.Empty;
        //    string userDB = String.Empty;

        //    DocsPaUtils.Query queryMng = null;
        //    try
        //    {
        //        if (dbType.ToUpper() == "SQL")
        //        {
        //            userDB = getUserDB();
        //        }
        //        System.Data.DataSet dsout = new DataSet();
        //        // non è necessario usare i filtri se non vengono popolati
        //        //dal momento che vengono cmq istanziati e non popolati
        //        //per non rimuovere la logica costruita controllo che
        //        // la dimensione sia superiore a 3 in quanto quel numero
        //        //rappresenta i filtri minimi caricati in argomento ma senza valore!!!
        //        if ((filter != null) && (filter.Length > 3))
        //        {
        //            filterWhere = getwhereFilter(filter);
        //            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_MY_FASC_TODOLIST_FILTERED");
        //            queryMng.setParam("idPeople", idPeople);
        //            queryMng.setParam("idGruppo", idGruppo);
        //            queryMng.setParam("filtri", filterWhere);
        //            //necessita di utente db SQL per la gestione della funzione vardescribe
        //            if ((userDB != null) && (userDB != ""))
        //            {
        //                queryMng.setParam("dbuser", userDB);
        //            }
        //            //commandText = queryMng.getSQL();
        //            // logger.Debug("SQL -  getMyFascTodoListWithFilter - Trasmissione.cs - QUERY : " + commandText);
        //        }
        //        else
        //        {
        //            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_MY_FASC_TODOLIST");
        //            queryMng.setParam("idPeople", idPeople);
        //            queryMng.setParam("idGruppo", idGruppo);
        //            //necessita di utente db SQL per la gestione della funzione vardescribe
        //            if ((userDB != null) && (userDB != ""))
        //            {
        //                queryMng.setParam("dbuser", userDB);
        //            }
        //            //commandText = queryMng.getSQL();
        //        }

        //        //set order
        //        filterOrder = getorderFilter(filter, "F");
        //        queryMng.setParam("order", filterOrder);

        //        commandText = queryMng.getSQL();

        //        logger.Debug("SQL -  getMyFascTodoListNoFilter - Trasmissione.cs - QUERY : " + commandText);

        //        dbProvider.ExecuteQuery(dsout, commandText);


        //        return this.GetListTrasmissioni(dsout.Tables[0], "F", idPeople, idGruppo, filterOrder);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        //public ArrayList GetListTrasmissioni(DataTable dataTable, string tipoTrasmissione, string currentIdPeople, string currentIdGruppo, string filterOrder)
        //{
        //    ArrayList todolist = new ArrayList();
        //    string sort = string.Empty;

        //    // Creazione dataview e impostazione ordinamento definitivo
        //    DataView view = dataTable.DefaultView;
        //    if (filterOrder != "")
        //    {

        //        sort = filterOrder + ",";
        //    }
        //    else
        //        //default
        //        sort = " DTAINVIO DESC, ID_PROFILE DESC,";

        //    if (tipoTrasmissione == "D")
        //        view.Sort = sort + "  ID_RAGIONE_TRASM ASC, ID_PEOPLE_DEST ASC, ID_RUOLO_DEST DESC";
        //    else if (tipoTrasmissione == "F")
        //        view.Sort = sort + "  ID_PEOPLE_DEST ASC, ID_RUOLO_DEST DESC";

        //    List<string> includedList = new List<string>();

        //    foreach (DataRowView drv in view)
        //    {
        //        string key = drv.Row["ID_TRASMISSIONE"].ToString() + "_" +
        //                        drv.Row["ID_RAGIONE_TRASM"].ToString();

        //        bool canAdd = false;

        //        if (!includedList.Contains(key))
        //        {
        //            if (drv.Row["ID_RUOLO_DEST"].ToString() == currentIdGruppo)
        //                canAdd = true;

        //            else if (drv.Row["ID_RUOLO_DEST"].ToString() == "0")
        //                canAdd = true;

        //            if (canAdd)
        //            {
        //                // La trasmissione è sicuramente visualizzata
        //                if (tipoTrasmissione == "D")
        //                    todolist.Add(CreateInfoDocumento(drv.Row));
        //                else
        //                    todolist.Add(CreateInfoFascicolo(drv.Row));

        //                includedList.Add(key);
        //            }
        //        }
        //    }

        //    return todolist;
        //}

        //utils
        //public string getwhereFilter(DocsPaVO.filtri.FiltroRicerca[] filter)
        //{
        //    string filterWhere = String.Empty;
        //    DocsPaVO.filtri.FiltroRicerca f;
        //    string UserDB = String.Empty;
        //    // verifica dbms
        //    if (dbType.ToUpper() == "SQL") UserDB = getUserDB();

        //    if (filter != null)
        //    {
        //        for (int i = 0; i < filter.Length; i++)
        //        {
        //            f = filter[i];
        //            if (f.valore != null && !f.valore.Equals(""))
        //            {
        //                switch (f.argomento)
        //                {
        //                    //GESTIONE OGGETTO DEL DOCUMENTO
        //                    case "OGGETTO_DOCUMENTO_TRASMESSO":
        //                        if (!string.IsNullOrEmpty(UserDB))
        //                            filterWhere += " AND UPPER(" + UserDB + ".Vardescribe(ID_PROFILE, 'DESC_OGGETTO')) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
        //                        else
        //                            filterWhere += " AND UPPER(Vardescribe(ID_PROFILE, 'DESC_OGGETTO')) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
        //                        break;
        //                    //GESTIONE OGGETTO DEL FASCICOLO
        //                    case "OGGETTO_FASCICOLO_TRASMESSO":
        //                        if (!string.IsNullOrEmpty(UserDB))
        //                            filterWhere += " AND UPPER(" + UserDB + ".Vardescribe(ID_PROJECT, 'DESC_FASC')) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
        //                        else
        //                            filterWhere += " AND UPPER(Vardescribe(ID_PROJECT, 'DESC_FASC')) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
        //                        break;
        //                    //GESTIONE FILTRO RAGIONE DI TRASMISSIONE
        //                    case "RAGIONE":
        //                        if (!string.IsNullOrEmpty(UserDB))
        //                            filterWhere += " AND UPPER(" + UserDB + ".Vardescribe(ID_RAGIONE_TRASM, 'RAGIONETRASM')) = '" + f.valore.ToString().ToUpper() + "'";
        //                        else
        //                            filterWhere += " AND UPPER(Vardescribe(ID_RAGIONE_TRASM, 'RAGIONETRASM')) = '" + f.valore.ToString().ToUpper() + "'";
        //                        break;
        //                    //GESTIONE FILTRI DATA DI TRASMISSIONE
        //                    case "TRASMISSIONE_IL":
        //                        filterWhere += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("DTA_INVIO", false) + "= '" + f.valore + "'";
        //                        break;
        //                    case "TRASMISSIONE_SUCCESSIVA_AL":
        //                        filterWhere += " AND DTA_INVIO>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
        //                        break;
        //                    case "TRASMISSIONE_PRECEDENTE_IL":
        //                        filterWhere += " AND DTA_INVIO<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
        //                        break;
        //                    //GESTIONE FILTRI SCADENZA
        //                    case "SCADENZA_IL":
        //                        filterWhere += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("DTA_SCADENZA", false) + "= '" + f.valore + "'";
        //                        break;
        //                    case "SCADENZA_SUCCESSIVA_AL":
        //                        filterWhere += " AND DTA_SCADENZA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
        //                        break;
        //                    case "SCADENZA_PRECEDENTE_IL":
        //                        filterWhere += " AND DTA_SCADENZA<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
        //                        break;
        //                    //GESTIONE FILTRI MITTENTE TRASMISSIONE DA RUBRICA
        //                    case "COD_RUBR_MITT_RUOLO":
        //                        filterWhere += " AND ID_RUOLO_MITT IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '" + f.valore.ToUpper().Replace("'", "''") + "')";
        //                        break;
        //                    case "COD_RUBR_MITT_UTENTE":
        //                        filterWhere += " AND ID_PEOPLE_MITT = (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '" + f.valore.ToUpper().Replace("'", "''") + "')";
        //                        break;
        //                    case "ID_UO_MITT":
        //                        filterWhere += " AND ID_RUOLO_MITT IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND ID_UO =" + f.valore + ")";
        //                        break;
        //                    //GESTIONE FILTRI MITTENTE TRASMISSIONE DA INSERIMENTO TESTUALE
        //                    case "MITTENTE_RUOLO":
        //                        filterWhere += " AND ID_RUOLO_MITT IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%')";
        //                        break;
        //                    case "MITTENTE_UTENTE":
        //                        filterWhere += " AND ID_PEOPLE_MITT IN (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%')";
        //                        break;
        //                    case "MITTENTE_UO":
        //                        filterWhere += " AND ID_RUOLO_MITT IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND ID_UO =(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE UPPER(VAR_DESC_CORR) IN ('" + f.valore.ToUpper().Replace("'", "''") + "')))";
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //    return filterWhere;
        //}
        /// <summary>
        /// Ritorna la clausola ORDER BY da applicare parametricamente sulle query della todolist e di smistamento.
        /// 
        /// se filter=null,
        /// si applica l'order primario di default DTAINVIO DESC
        /// se type="", sia applica l'order secondario di default ID_PROFILE DESC
        /// type [D = documenti; F=fascicoli; S=smistamento]
        ///// </summary>
        ///// <param name="filter"></param>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //public string getorderFilter(DocsPaVO.filtri.FiltroRicerca[] filter, string type)
        //{
        //    string filterOrder = String.Empty;
        //    DocsPaVO.filtri.FiltroRicerca f;

        //    if (filter != null)
        //    {
        //        for (int i = 0; i < filter.Length; i++)
        //        {
        //            f = filter[i];
        //            if (f.valore != null && !f.valore.Equals(""))
        //            {
        //                switch (f.argomento)
        //                {
        //                    //GESTIONE OGGETTO DEL DOCUMENTO
        //                    case "DTA_INVIO_DESC":
        //                        filterOrder += " DTAINVIO DESC";
        //                        break;
        //                    case "DTA_SCAD_DESC":
        //                        filterOrder += " DTA_SCAD_DESC DESC";
        //                        break;
        //                    case "DTA_SCAD_ASC":
        //                        filterOrder += " DTA_SCAD_ASC ASC";
        //                        break;
        //                    case "DTA_INVIO_ASC":
        //                        filterOrder += " DTAINVIO ASC";
        //                        break;

        //                }
        //            }
        //        }
        //    }

        //    //per settare un ordinamento di default
        //    if (filterOrder == "")
        //        filterOrder += " DTAINVIO DESC";

        //    //POI VERIFICO SE FASC "F" O DOC "D" e aggiungo order necessario:

        //    switch (type)
        //    {
        //        case "D":
        //            filterOrder += ",ID_PROFILE DESC";
        //            break;
        //        case "F":
        //            filterOrder += ",ID_PROJECT DESC";
        //            break;
        //        case "S": //SMISTAMENTO
        //            filterOrder += ",ID_TRASMISSIONE DESC";
        //            break;
        //        //non dovrebbe essere utile.
        //        default:
        //            filterOrder += ",ID_PROFILE DESC";
        //            break;
        //    }


        //    return filterOrder;
        //}


        #endregion

        /// <summary>
        /// Rimozione del documento per cui si sono perduti i diritti di visibilità dalla todolist
        /// </summary>
        /// <param name="idTrasmUtente"></param>
        /// <param name="idTrasmSingola"></param>
        /// <param name="idPeople"></param>
        /// <returns>True o False</returns>
        public bool RimuoviToDoListACL(string idTrasmUtente, string idTrasmSingola, string idPeople)
        {
            DocsPaUtils.Query queryMng = null;
            string commandText;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //Si deve capire se è stato rimosso il diritto di visibilità o il documento
                    System.Data.DataSet dataSet = new DataSet();
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_TODOLIST_ACL");
                    queryMng.setParam("param3", idTrasmUtente);
                    queryMng.setParam("param5", " SELECT id_trasmissione FROM DPA_TRASM_SINGOLA WHERE system_id=" + idTrasmSingola + " ");
                    queryMng.setParam("param4", idPeople);

                    commandText = queryMng.getSQL();
                    logger.Debug("SQL -  RimuoviToDoListACL - Trasmissione.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                    dbProvider.CommitTransaction();
                }
            }
            catch (Exception e)
            {
                logger.Debug("F_System");
                throw new Exception(e.Message);
                return false;
            }
            return true;
        }

        public bool getPredInTodoList(DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruoloCorr)
        {
            DocsPaUtils.Query queryMng = null;
            string commandText;
            bool retValue = false;
            int totalRecordCount = 0;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ROWSNONVISTE_PRED_TODOLIST");
                    queryMng.setParam("idPeople", utente.idPeople);
                    queryMng.setParam("idGruppo", ruoloCorr.idGruppo);
                    queryMng.setParam("filters", "AND (A.dta_vista = " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753") + ")");// or A.id_trasm_utente in (select system_id from dpa_trasm_utente where cha_accettata='0'))");
                    commandText = queryMng.getSQL();
                    logger.Debug("DOCUMENTI PREDISPOSTI NON VISTI O NON ANCORA ACCETTATI: " + commandText);

                    string field;
                    if (dbProvider.ExecuteScalar(out field, commandText))
                        totalRecordCount = Convert.ToInt32(field);

                    if (totalRecordCount > 0)
                    {
                        retValue = true;
                        return retValue;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("F_System");
                throw new Exception(e.Message);
                return false;
            }
            return retValue;
        }

        public bool getAllTodoList(DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruoloCorr)
        {
            DocsPaUtils.Query queryMng = null;
            string commandText;
            bool retValue = false;
            int totalRecordCount = 0;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {

                    for (int i = 0; i < utente.ruoli.Count; i++)
                    {
                        if (ruoloCorr.idGruppo != ((DocsPaVO.utente.Ruolo)utente.ruoli[i]).idGruppo)
                        {
                            //Verifica che ci sia almeno una trasmissione non letta in un ruolo dell'utente
                            //che non sia quello corrente
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ROWSNONVISTE_TODOLIST");
                            queryMng.setParam("idPeople", utente.idPeople);
                            queryMng.setParam("idGruppo", ((DocsPaVO.utente.Ruolo)utente.ruoli[i]).idGruppo);
                            queryMng.setParam("filters", "AND (dta_vista = " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753") + " or id_trasm_utente in (select system_id from dpa_trasm_utente where cha_accettata='0' ))");
                            commandText = queryMng.getSQL();
                            logger.Debug("DOCUMENTI NON VISTI O NON ANCORA ACCETTATI: " + commandText);

                            string field;
                            if (dbProvider.ExecuteScalar(out field, commandText))
                                totalRecordCount = Convert.ToInt32(field);

                            if (totalRecordCount > 0)
                            {
                                retValue = true;
                                return retValue;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("F_System");
                throw new Exception(e.Message);
                return false;
            }
            return retValue;
        }

        public ArrayList getDettagliAllTodolistOLD(DocsPaVO.utente.Utente utente)
        {
            DocsPaUtils.Query queryMng = null;
            string commandText;
            string field;
            ArrayList allTodolist = new ArrayList();
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {

                    for (int i = 0; i < utente.ruoli.Count; i++)
                    {
                        DocsPaVO.trasmissione.AllToDoList todolist = new DocsPaVO.trasmissione.AllToDoList();
                        todolist.ruoloDesc = ((DocsPaVO.utente.Ruolo)utente.ruoli[i]).descrizione;
                        todolist.ruoloId = ((DocsPaVO.utente.Ruolo)utente.ruoli[i]).idGruppo;

                        ArrayList listRegistri = ((DocsPaVO.utente.Ruolo)utente.ruoli[i]).registri;
                        string registri = "0,";
                        for (int reg = 0; reg < listRegistri.Count; reg++)
                        {
                            string registro = ((DocsPaVO.utente.Registro)listRegistri[reg]).systemId;
                            registri = registri + registro + ",";
                        }
                        registri = registri.Substring(0, registri.Length - 1);


                        //Verifica per il dato ruolo:
                        //numero documenti presenti in todolist
                        //numero documenti non letti in todolist
                        //numero documenti non accettati
                        //numero fascicoli presenti in todolist
                        //numero fascicoli non letti in todolist
                        //numero fascicoli non accettati
                        //numero documenti predisposti

                        //numero documenti presenti in todolist
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ROWS_TODOLIST");
                        queryMng.setParam("idPeople", utente.idPeople);
                        queryMng.setParam("idGruppo", ((DocsPaVO.utente.Ruolo)utente.ruoli[i]).idGruppo);
                        queryMng.setParam("filters", "");
                        queryMng.setParam("docORfasc", "ID_PROFILE > 0 and");
                        queryMng.setParam("registri", " AND TDL.ID_REGISTRO IN (" + registri + ")");
                        //queryMng.setParam("registri", registri);
                        commandText = queryMng.getSQL();
                        logger.Debug("Numero doc. tot.: ");
                        logger.Debug(commandText);
                        if (dbProvider.ExecuteScalar(out field, commandText))
                            todolist.trasmDocTot = Convert.ToInt32(field);

                        //numero documenti non letti in todolist
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ROWS_TODOLIST");
                        queryMng.setParam("idPeople", utente.idPeople);
                        queryMng.setParam("idGruppo", ((DocsPaVO.utente.Ruolo)utente.ruoli[i]).idGruppo);
                        queryMng.setParam("filters", " AND dta_vista = " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753"));
                        queryMng.setParam("docORfasc", "ID_PROFILE > 0 and");
                        //queryMng.setParam("registri", registri);
                        queryMng.setParam("registri", " AND TDL.ID_REGISTRO IN (" + registri + ")");
                        commandText = queryMng.getSQL();
                        logger.Debug("Numero doc. tot. non letti: ");
                        logger.Debug(commandText);
                        if (dbProvider.ExecuteScalar(out field, commandText))
                            todolist.trasmDocNonLetti = Convert.ToInt32(field);

                        //numero documenti non ancora accettati in todolist
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ROWS_TODOLIST");
                        queryMng.setParam("idPeople", utente.idPeople);
                        queryMng.setParam("idGruppo", ((DocsPaVO.utente.Ruolo)utente.ruoli[i]).idGruppo);
                        queryMng.setParam("filters", "");
                        queryMng.setParam("docORfasc", "ID_PROFILE > 0 and");
                        //queryMng.setParam("registri", registri);
                        queryMng.setParam("registri", " AND TDL.ID_REGISTRO IN (" + registri + ")" + " and id_trasm_utente in (select system_id from dpa_trasm_utente where cha_accettata='0' )");
                        commandText = queryMng.getSQL();
                        logger.Debug("Numero doc. tot. non accettai: ");
                        logger.Debug(commandText);
                        if (dbProvider.ExecuteScalar(out field, commandText))
                            todolist.trasmDocNonAccettati = Convert.ToInt32(field);

                        //numero fascicoli presenti in todolist
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ROWS_TODOLIST");
                        queryMng.setParam("idPeople", utente.idPeople);
                        queryMng.setParam("idGruppo", ((DocsPaVO.utente.Ruolo)utente.ruoli[i]).idGruppo);
                        queryMng.setParam("filters", "");
                        queryMng.setParam("docORfasc", "ID_PROJECT > 0 and");
                        queryMng.setParam("registri", "");
                        commandText = queryMng.getSQL();
                        logger.Debug("Numero fasc. tot.: ");
                        logger.Debug(commandText);
                        if (dbProvider.ExecuteScalar(out field, commandText))
                            todolist.trasmFascTot = Convert.ToInt32(field);

                        //numero fascicoli non letti in todolist
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ROWS_TODOLIST");
                        queryMng.setParam("idPeople", utente.idPeople);
                        queryMng.setParam("idGruppo", ((DocsPaVO.utente.Ruolo)utente.ruoli[i]).idGruppo);
                        queryMng.setParam("filters", " AND dta_vista = " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753"));
                        queryMng.setParam("docORfasc", "ID_PROJECT > 0 and");
                        queryMng.setParam("registri", "");
                        commandText = queryMng.getSQL();
                        logger.Debug("Numero fasc. tot. non letti: ");
                        logger.Debug(commandText);
                        if (dbProvider.ExecuteScalar(out field, commandText))
                            todolist.trasmFascNonLetti = Convert.ToInt32(field);

                        //numero documenti non ancora accettati in todolist
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ROWS_TODOLIST");
                        queryMng.setParam("idPeople", utente.idPeople);
                        queryMng.setParam("idGruppo", ((DocsPaVO.utente.Ruolo)utente.ruoli[i]).idGruppo);
                        queryMng.setParam("filters", "");
                        queryMng.setParam("docORfasc", "ID_PROJECT > 0 and");
                        //queryMng.setParam("registri", registri);
                        queryMng.setParam("registri", " AND TDL.ID_REGISTRO IN (" + registri + ")" + " and id_trasm_utente in (select system_id from dpa_trasm_utente where cha_accettata='0' )");
                        commandText = queryMng.getSQL();
                        logger.Debug("Numero fasc. tot. non accettati: ");
                        logger.Debug(commandText);
                        if (dbProvider.ExecuteScalar(out field, commandText))
                            todolist.trasmFascNonAccettati = Convert.ToInt32(field);

                        //numero documenti predisposti
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ROWS_TODOLIST");
                        queryMng.setParam("idPeople", utente.idPeople);
                        queryMng.setParam("idGruppo", ((DocsPaVO.utente.Ruolo)utente.ruoli[i]).idGruppo);
                        queryMng.setParam("filters", "");
                        queryMng.setParam("docORfasc", "ID_PROFILE > 0 and id_profile in (select system_id from profile where cha_da_proto = '1') and ");
                        queryMng.setParam("registri", " AND TDL.ID_REGISTRO IN (" + registri + ")");
                        commandText = queryMng.getSQL();
                        logger.Debug("Numero doc. predisposti: ");
                        logger.Debug(commandText);
                        if (dbProvider.ExecuteScalar(out field, commandText))
                            todolist.docPredisposti = Convert.ToInt32(field);

                        allTodolist.Add(todolist);
                    }

                }


            }
            catch (Exception e)
            {
                allTodolist = null;
                logger.Debug("F_System");
                throw new Exception(e.Message);
            }
            return allTodolist;
        }

        public ArrayList getDettagliAllTodolist(DocsPaVO.utente.Utente utente)
        {
            DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();

            DocsPaUtils.Query queryMng = null;
            string commandText = "";
            string timeStamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").Replace(".", ":");
            ArrayList allTodolist = new ArrayList();
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    for (int i = 0; i < utente.ruoli.Count; i++)
                    {
                        //Verifica per il dato ruolo:
                        //numero documenti presenti in todolist
                        //numero documenti non letti in todolist
                        //numero documenti non accettati
                        //numero fascicoli presenti in todolist
                        //numero fascicoli non letti in todolist
                        //numero fascicoli non accettati
                        //numero documenti predisposti
                        DataSet ds = new DataSet();
                        string _objDBType = dbProvider.DBType.ToUpper().ToString();
                        switch (_objDBType)
                        {
                            case "SQL":
                                ArrayList _parametri = new ArrayList();
                                _parametri.Add(new DocsPaUtils.Data.ParameterSP("idPeople", utente.idPeople));
                                _parametri.Add(new DocsPaUtils.Data.ParameterSP("idGroup", ((DocsPaVO.utente.Ruolo)utente.ruoli[i]).idGruppo));
                                if (!obj.isFiltroAooEnabled())
                                {
                                    logger.Debug("TODOLIST - SP: COUNT_TODOLIST");
                                    dbProvider.ExecuteStoredProcedure("SP_COUNT_TODOLIST", _parametri, ds);
                                }
                                else
                                {
                                    logger.Debug("TODOLIST - SP: COUNT_TODOLIST_NO_REG");
                                    dbProvider.ExecuteStoredProcedure("SP_COUNT_TODOLIST_NO_REG", _parametri, ds);
                                }
                                break;
                            case "ORACLE":
                                if (!obj.isFiltroAooEnabled())
                                {
                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("SET_DETTAGLI_ALL_TODOLIST");
                                }
                                else
                                {
                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("SET_DETTAGLI_ALL_TODOLIST_NO_REG");
                                }
                                queryMng.setParam("IDPEOPLE", utente.idPeople);
                                queryMng.setParam("IDGRUPPO", ((DocsPaVO.utente.Ruolo)utente.ruoli[i]).idGruppo);
                                queryMng.setParam("TIMESTAMP", timeStamp);
                                commandText = queryMng.getSQL();
                                logger.Debug("TODOLIST - SET_DETTAGLI_ALL_TODOLIST: " + commandText);
                                dbProvider.ExecuteNonQuery(commandText);
                                //prelevo i dati appena inseriti
                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("GET_DETTAGLI_ALL_TODOLIST");
                                queryMng.setParam("IDPEOPLE", utente.idPeople);
                                queryMng.setParam("TIMESTAMP", timeStamp);
                                commandText = queryMng.getSQL();
                                logger.Debug("TODOLIST - GET_DETTAGLI_ALL_TODOLIST: " + commandText);
                                dbProvider.ExecuteQuery(ds, commandText);
                                break;
                        }
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            DocsPaVO.trasmissione.AllToDoList todolist = new DocsPaVO.trasmissione.AllToDoList();
                            todolist.ruoloDesc = ((DocsPaVO.utente.Ruolo)utente.ruoli[i]).descrizione;
                            todolist.ruoloId = ((DocsPaVO.utente.Ruolo)utente.ruoli[i]).idGruppo;
                            todolist.trasmDocTot = Convert.ToInt32(row["TOT_DOC"].ToString());
                            todolist.trasmDocNonLetti = Convert.ToInt32(row["TOT_DOC_NO_LETTI"].ToString());
                            todolist.trasmDocNonAccettati = Convert.ToInt32(row["TOT_DOC_NO_ACCETTATI"].ToString());
                            todolist.trasmFascTot = Convert.ToInt32(row["TOT_FASC"].ToString());
                            todolist.trasmFascNonLetti = Convert.ToInt32(row["TOT_FASC_NO_LETTI"].ToString());
                            todolist.trasmFascNonAccettati = Convert.ToInt32(row["TOT_FASC_NO_ACCETTATI"].ToString());
                            todolist.docPredisposti = Convert.ToInt32(row["TOT_DOC_PREDISPOSTI"].ToString());
                            allTodolist.Add(todolist);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("F_System");
                throw new Exception(e.Message);
                return null;
            }
            return allTodolist;
        }


        //Creazione oggetto infoTodoList per i documenti e i fascicoli trasmessi
        private DocsPaVO.trasmissione.infoToDoList CreateInfoTodoList(DataRow row, string currentIdGruppo, string currentIdPeople)
        {
            logger.Info("BEGIN");
            // L'oggetto della trasmissione (documento = D, fascicolo = F)
            string tipoOggetto = string.Empty;

            string patternVideo = "<br>" + " ------- " + "<br>";

            DocsPaVO.trasmissione.infoToDoList myTodo = new DocsPaVO.trasmissione.infoToDoList();

            // dati presenti per obbligatorietà.
            this.FetchCommonFieldInfoToDoList(row, myTodo);

            //aggiungo il dato video concatenato
            if (!string.IsNullOrEmpty(myTodo.ut_delegato))
                myTodo.videoMittRuolo = myTodo.ut_delegato + "<br>Delegato da " + myTodo.utenteMittente + "<br>(" + myTodo.ruoloMittente + ")";
            else
                myTodo.videoMittRuolo = myTodo.utenteMittente + "<br>" + "(" + myTodo.ruoloMittente + ")";

            myTodo.utenteDestinatario = row["UT_DEST"].ToString();
            myTodo.ragione = row["RAG_TRASM"].ToString();

            // dati opzionali
            if (row["VAR_NOTE_GEN"] != DBNull.Value && row["VAR_NOTE_GEN"].ToString() != "")
            {
                myTodo.noteGenerali = row["VAR_NOTE_GEN"].ToString();
                if (row["VAR_NOTE_SING"] != DBNull.Value && row["VAR_NOTE_SING"].ToString() != "")
                {
                    myTodo.noteGenerali += patternVideo + row["VAR_NOTE_SING"].ToString();
                }
            }
            else
            {
                if (row["VAR_NOTE_SING"] != DBNull.Value && row["VAR_NOTE_SING"].ToString() != "")
                {
                    myTodo.noteGenerali += patternVideo + row["VAR_NOTE_SING"].ToString();
                }
            }
            if (row["VAR_NOTE_SING"] != DBNull.Value)
            {
                if (row["VAR_NOTE_SING"].ToString() != "")
                    myTodo.noteIndividuali = row["VAR_NOTE_SING"].ToString();
            }
            if (row["DTA_SCADENZA"] != DBNull.Value)
            {
                if (row["DTA_SCAD_CHAR"].ToString() != "")
                    myTodo.dataScadenza = row["DTA_SCAD_CHAR"].ToString();
            }
            if (row["DTA_SCAD_CHAR"] != DBNull.Value)
            {
                if (row["DTA_SCAD_CHAR"].ToString() != "")
                    myTodo.dataScadenza = row["DTA_SCAD_CHAR"].ToString();
            }
            if (row["DTA_VISTA"] != DBNull.Value)
            {
                if (row["DTA_VISTA"].ToString() != "")
                {
                    string dataVista = row["DTA_VISTA"].ToString();
                    dataVista = dataVista.Substring(0, dataVista.IndexOf(" "));
                    //modifica
                    myTodo.isVista = "0";

                    if (!dataVista.Equals("01/01/1753"))
                        myTodo.isVista = "1";
                    //fine modifica
                    //DateTime date = DateTime.Parse("01/01/1753");
                    //DateTime dateVista = Convert.ToDateTime(dataVista);

                    //                    int valore = dataVista.Equals("01/01/1753");

                    //                    if (valore == 1)
                    //                        myTodo.isVista = "1";
                    //                    else
                    //                        myTodo.isVista = "0";
                }
            }



            #region documenti
            if (row["ID_PROFILE"] != DBNull.Value)
            {
                // Tipo oggetto è documento
                tipoOggetto = "D";

                myTodo.sysIdDoc = row["ID_PROFILE"].ToString();


                myTodo.infoDoc = row["SEGNA_DOCNUMBER"].ToString();
                myTodo.dataDoc = row["DTA_DOC"].ToString();

                if (row["TIPO_PROTO"] != DBNull.Value)
                    myTodo.tipoProto = row["TIPO_PROTO"].ToString();

                if (row["NUM_PROTO"] != DBNull.Value)
                {
                    if (row["NUM_PROTO"].ToString() != "")
                        myTodo.numProto = row["NUM_PROTO"].ToString();
                    else
                        myTodo.numProto = ""; //row["SEGNA_DOCNUMBER"].ToString();
                }
                else myTodo.numProto = ""; //row["SEGNA_DOCNUMBER"].ToString();

                if (row["OGGETTO_MITTENTE"] != DBNull.Value)
                {
                    if (row["OGGETTO_MITTENTE"].ToString() != "")
                    {
                        // formato  OGGETTO_MITTENTE:
                        //se protocollo:
                        //OGGETTO || '@@' || VAR_DESC_MITT QUINDI LUNGHEZZA 3
                        //char[] pattern = { '@', '@' };
                        //string[] oggMitt = row["OGGETTO_MITTENTE"].ToString().Split(pattern);

                        string[] pattern = { "@@" };
                        string[] oggMitt = row["OGGETTO_MITTENTE"].ToString().Split(pattern, StringSplitOptions.RemoveEmptyEntries);

                        // è un doc grigio
                        if (myTodo.tipoProto == "G")
                        {
                            myTodo.oggetto = oggMitt[0].ToString();
                            //aggiungo il dato video concatenato
                            myTodo.videoOggMitt = oggMitt[0].ToString() + patternVideo;
                            myTodo.videoDocInfo = row["SEGNA_DOCNUMBER"].ToString() + "<br>" + myTodo.dataDoc;
                            myTodo.infoDoc = "IdDoc: " + row["SEGNA_DOCNUMBER"].ToString();

                        }
                        //è un protocollo
                        if ((myTodo.tipoProto == "A" || myTodo.tipoProto == "P"
                            || myTodo.tipoProto == "I") && myTodo.numProto != "")
                        {
                            myTodo.oggetto = oggMitt[0].ToString();
                            // se uscita o interno il mittente può essere anche vuoto, decideremo poi se mettere destinatari

                            //ANOMALIA MITTENTI CON @
                            //if (oggMitt.Length == 3 && oggMitt[2] != null)
                            //{
                            //    myTodo.mittente = oggMitt[2].ToString();
                            //    myTodo.videoOggMitt = oggMitt[0].ToString() + patternVideo + oggMitt[2].ToString();

                            //}
                            if (oggMitt.Length == 2 && oggMitt[1] != null)
                            {
                                myTodo.mittente = oggMitt[1].ToString();
                                myTodo.videoOggMitt = oggMitt[0].ToString() + patternVideo + oggMitt[1].ToString();
                            }                           
                            else
                            {
                                myTodo.oggetto = oggMitt[0].ToString();
                                //aggiungo il dato video concatenato
                                myTodo.videoOggMitt = oggMitt[0].ToString() + patternVideo;
                            }
                            myTodo.videoDocInfo = myTodo.numProto + "<br>" + myTodo.dataDoc;
                            myTodo.infoDoc = row["SEGNA_DOCNUMBER"].ToString();
                        }

                        if ((myTodo.tipoProto == "A" || myTodo.tipoProto == "P"
                            || myTodo.tipoProto == "I") && myTodo.numProto == "") //predisposto 
                        {
                            myTodo.oggetto = oggMitt[0].ToString();
                            myTodo.oggetto = oggMitt[0].ToString();
                            // se uscita o interno il mittente può essere anche vuoto, decideremo poi se mettere destinatari.

                            //ANOMALIA MITTENTE con @ nella descrizione
                            //if (oggMitt.Length == 3 && oggMitt[2] != null)
                            //{
                            //    myTodo.mittente = oggMitt[2].ToString();
                            //    myTodo.videoOggMitt = oggMitt[0].ToString() + patternVideo + oggMitt[2].ToString();

                            //}
                            if (oggMitt.Length == 2 && oggMitt[1] != null)
                            {
                                myTodo.mittente = oggMitt[1].ToString();
                                myTodo.videoOggMitt = oggMitt[0].ToString() + patternVideo + oggMitt[1].ToString();

                            }
                            else
                            {
                                myTodo.oggetto = oggMitt[0].ToString();
                                //aggiungo il dato video concatenato
                                myTodo.videoOggMitt = oggMitt[0].ToString() + patternVideo;
                            }
                            myTodo.videoDocInfo = row["SEGNA_DOCNUMBER"].ToString() + "<br>" + myTodo.dataDoc;
                            myTodo.infoDoc = "IdDoc: " + row["SEGNA_DOCNUMBER"].ToString();

                        }
                    }

                }
                if (row["NUM_PROTO"] != DBNull.Value)
                {
                    if (row["NUM_PROTO"].ToString() != "")
                        myTodo.numProto = row["NUM_PROTO"].ToString();
                    else
                        myTodo.numProto = row["SEGNA_DOCNUMBER"].ToString();
                }
                else myTodo.numProto = row["SEGNA_DOCNUMBER"].ToString();

                if (row["CHA_IMG"] != DBNull.Value)
                {
                    myTodo.cha_img = row["CHA_IMG"].ToString();
                }

                if (row["CHA_FIRMATO"] != DBNull.Value)
                {
                    if (row["CHA_FIRMATO"].ToString() != "")
                    {
                        myTodo.isFirmato = row["CHA_FIRMATO"].ToString();
                    }
                }

                Model model = new Model();
                myTodo.VideoTipology = model.GetTipologyDescriptionByIdProfile(myTodo.sysIdDoc);

            }
            #endregion





            #region fascicoli
            string numFascicolo = "";
            string annoCreazione = "";
            string codFasc = "";
            if (row["ID_PROJECT"] != DBNull.Value)
            {
                // Tipo oggetto è fascicolo
                tipoOggetto = "F";

                if (row["ID_PROJECT"].ToString() != "")
                    myTodo.sysIdFasc = row["ID_PROJECT"].ToString();


                if (row["DESC_FASC"] != DBNull.Value)
                {
                    if (row["DESC_FASC"].ToString() != "")
                    {
                        myTodo.oggetto = row["DESC_FASC"].ToString();
                        myTodo.videoOggMitt = myTodo.oggetto;
                    }
                }

                if (row["COD_FASC"] != DBNull.Value)
                {
                    if (row["COD_FASC"].ToString() != "")
                    {
                        codFasc = row["COD_FASC"].ToString();
                        myTodo.infoDoc = "Fasc: " + codFasc;
                    }
                }

                //Numero Fascicolo + anno di creazione del fascicolo + codice classificazione
                if (row["NUM_FASC"] != DBNull.Value)
                {
                    if (row["NUM_FASC"].ToString() != "")
                        numFascicolo = row["NUM_FASC"].ToString();
                }
                if (row["DTA_CREAZ"] != DBNull.Value)
                {
                    if (row["DTA_CREAZ"].ToString() != "")
                        annoCreazione = row["DTA_CREAZ"].ToString();
                }
                myTodo.videoDocInfo = codFasc; // +" - <br>" + annoCreazione + " - " + numFascicolo;

                ModelFasc model = new ModelFasc();
                myTodo.VideoTipology = model.GetTipologyDescriptionByIdProject(myTodo.sysIdFasc);

            }
            #endregion

            string docFasc = string.Empty;
            string msg;
            int resACL = 0;

            // Determinazione della tipologia di oggetto della trasmissione e dei diritti di accesso
            // all'oggetto della trasmissione
            if (tipoOggetto.Equals("D"))
            {
                resACL = new Documenti().VerificaACL(tipoOggetto, myTodo.sysIdDoc,
                            currentIdPeople, currentIdGruppo, out msg);

                docFasc = "documento";
            }
            else
            {
                resACL = new Documenti().VerificaACL(tipoOggetto, myTodo.sysIdFasc,
                            currentIdPeople, currentIdGruppo, out msg);

                docFasc = "fascicolo";
            }


            // Se l'utente ha i diritti necessari di visibilità sul 
            // documento
            if (resACL != 2)
            {
                myTodo.videoOggMitt = String.Format("<b>Non si possiedono i diritti per la visualizzazione delle informazioni sul {0}</b>",
                    docFasc);
                myTodo.oggetto = String.Format("<b>Non si possiedono i diritti per la visualizzazione delle informazioni sul {0}</b>",
                    docFasc);
            }
            logger.Info("END");
            return myTodo;
        }

        // Creazione oggetto infoToDoList per il fascicolo trasmesso
        private DocsPaVO.trasmissione.infoToDoList CreateInfoFascicolo(DataRow row, string currentIdGruppo, string currentIdPeople)
        {
            logger.Info("BEGIN");
            string patternVideo = "<br>" + " ------- " + "<br>";
            DocsPaVO.trasmissione.infoToDoList myTodo = new DocsPaVO.trasmissione.infoToDoList();

            // dati presenti per obligatorietà.
            this.FetchCommonFieldInfoToDoList(row, myTodo);

            //aggiungo il dato video concatenato
            if (!string.IsNullOrEmpty(myTodo.ut_delegato))
                myTodo.videoMittRuolo = myTodo.ut_delegato + "<br>Delegato da " + myTodo.utenteMittente + "<br>(" + myTodo.ruoloMittente + ")";
            else
                myTodo.videoMittRuolo = myTodo.utenteMittente + "<br>" + "(" + myTodo.ruoloMittente + ")";

            myTodo.utenteDestinatario = row["UT_DEST"].ToString();
            myTodo.ragione = row["RAG_TRASM"].ToString();

            // dati opzionali
            if (row["VAR_NOTE_GEN"] != DBNull.Value && row["VAR_NOTE_GEN"].ToString() != "")
            {
                myTodo.noteGenerali = row["VAR_NOTE_GEN"].ToString();
                if (row["VAR_NOTE_SING"] != DBNull.Value && row["VAR_NOTE_SING"].ToString() != "")
                {
                    myTodo.noteGenerali += patternVideo + row["VAR_NOTE_SING"].ToString();
                }
            }
            else
            {
                if (row["VAR_NOTE_SING"] != DBNull.Value && row["VAR_NOTE_SING"].ToString() != "")
                {
                    myTodo.noteGenerali += patternVideo + row["VAR_NOTE_SING"].ToString();
                }
            }
            if (row["VAR_NOTE_SING"] != DBNull.Value)
            {
                if (row["VAR_NOTE_SING"].ToString() != "")
                    myTodo.noteIndividuali = row["VAR_NOTE_SING"].ToString();
            }
            if (row["DTA_SCADENZA"] != DBNull.Value)
            {
                if (row["DTA_SCAD_CHAR"].ToString() != "")
                    myTodo.dataScadenza = row["DTA_SCAD_CHAR"].ToString();
            }

            if (row["ID_PROJECT"] != DBNull.Value)
            {
                if (row["ID_PROJECT"].ToString() != "")
                    myTodo.sysIdFasc = row["ID_PROJECT"].ToString();
            }

            if (row["DESC_FASC"] != DBNull.Value)
            {
                if (row["DESC_FASC"].ToString() != "")
                    myTodo.oggetto = row["DESC_FASC"].ToString();
            }

            if (row["COD_FASC"] != DBNull.Value)
            {
                if (row["COD_FASC"].ToString() != "")
                    myTodo.infoDoc = row["COD_FASC"].ToString();
            }

            string msg;
            // Se l'utente ha i diritti necessari di visibilità sul 
            // documento
            if (new Documenti().VerificaACL("F", myTodo.sysIdFasc,
                currentIdPeople, currentIdGruppo, out msg) != 2)
                myTodo.oggetto = "<b>Non si possiedono i diritti per la visualizzazione delle informazioni sul fascicolo</b>";

            ModelFasc model = new ModelFasc();
            myTodo.VideoTipology = model.GetTipologyDescriptionByIdProject(myTodo.sysIdFasc);

            logger.Info("END");
            return myTodo;
        }


        //Fetch dei dati comuni di todolist
        private void FetchCommonFieldInfoToDoList(DataRow row, DocsPaVO.trasmissione.infoToDoList info)
        {
            // dati presenti per obligatorietà.
            info.sysIdTrasm = row["ID_TRASMISSIONE"].ToString();
            info.sysIdTrasmSing = row["ID_TRASM_SINGOLA"].ToString();
            info.sysIdTrasmUt = row["ID_TRASM_UTENTE"].ToString();
            info.dataInvio = row["DTAINVIO"].ToString();
            info.utenteMittente = row["UT_MITT"].ToString();
            info.ruoloMittente = row["RUOLO_MITT"].ToString();
            info.idPeopleMitt = row["ID_PEOPLE_MITT"].ToString();
            info.idRuoloInUo = row["ID_RUOLO_MITT"].ToString();
            info.idRagione = row["ID_RAGIONE_TRASM"].ToString();
            info.idPeopleDest = row["ID_PEOPLE_DEST"].ToString();
            info.idRuoloDest = row["ID_RUOLO_DEST"].ToString();
            info.TipoTrasm = row["CHA_TIPO_TRASM"].ToString();

            if (row.Table.Columns.Contains("ID_PEOPLE_DELEGATO"))
                if (row["ID_PEOPLE_DELEGATO"] != DBNull.Value)
                {
                    if (row["ID_PEOPLE_DELEGATO"].ToString() != "")
                    {
                        Utenti utente = new Utenti();
                        info.ut_delegato = utente.GetUtente(row["ID_PEOPLE_DELEGATO"].ToString()).descrizione;

                    }
                }

        }

        // Creazione oggetto infoToDoList per il documento trasmesso
        private DocsPaVO.trasmissione.infoToDoList CreateInfoDocumento(DataRow row, string currentIdGruppo, string currentIdPeople)
        {
            logger.Info("BEGIN");
            string patternVideo = "<br>" + " ------- " + "<br>";

            DocsPaVO.trasmissione.infoToDoList myTodo = new DocsPaVO.trasmissione.infoToDoList();

            // dati presenti per obbligatorietà.
            this.FetchCommonFieldInfoToDoList(row, myTodo);

            //aggiungo il dato video concatenato
            if (!string.IsNullOrEmpty(myTodo.ut_delegato))
                myTodo.videoMittRuolo = myTodo.ut_delegato + "<br>Delegato da " + myTodo.utenteMittente + "<br>(" + myTodo.ruoloMittente + ")";
            else
                myTodo.videoMittRuolo = myTodo.utenteMittente + "<br>" + "(" + myTodo.ruoloMittente + ")";

            myTodo.utenteDestinatario = row["UT_DEST"].ToString();
            myTodo.ragione = row["RAG_TRASM"].ToString();
            //da verificare per i fascicoli
            myTodo.infoDoc = row["SEGNA_DOCNUMBER"].ToString();
            myTodo.dataDoc = row["DTA_DOC"].ToString();

            if (row["TIPO_PROTO"] != DBNull.Value)
                myTodo.tipoProto = row["TIPO_PROTO"].ToString();

            // dati opzionali
            if (row["VAR_NOTE_GEN"] != DBNull.Value && row["VAR_NOTE_GEN"].ToString() != "")
            {
                myTodo.noteGenerali = row["VAR_NOTE_GEN"].ToString();
                if (row["VAR_NOTE_SING"] != DBNull.Value && row["VAR_NOTE_SING"].ToString() != "")
                {
                    myTodo.noteGenerali += patternVideo + row["VAR_NOTE_SING"].ToString();
                }
            }
            else
            {
                if (row["VAR_NOTE_SING"] != DBNull.Value && row["VAR_NOTE_SING"].ToString() != "")
                {
                    myTodo.noteGenerali += patternVideo + row["VAR_NOTE_SING"].ToString();
                }
            }
            if (row["VAR_NOTE_SING"] != DBNull.Value)
            {
                myTodo.noteIndividuali = row["VAR_NOTE_SING"].ToString();
            }
            if (row["DTA_SCAD_CHAR"] != DBNull.Value)
            {
                if (row["DTA_SCAD_CHAR"].ToString() != "")
                    myTodo.dataScadenza = row["DTA_SCAD_CHAR"].ToString();
            }
            if (row["ID_PROFILE"] != DBNull.Value)
            {
                myTodo.sysIdDoc = row["ID_PROFILE"].ToString();
            }

            if (row["NUM_PROTO"] != DBNull.Value)
            {
                if (row["NUM_PROTO"].ToString() != "")
                    myTodo.numProto = row["NUM_PROTO"].ToString();
                else
                    myTodo.numProto = ""; //row["SEGNA_DOCNUMBER"].ToString();
            }
            else myTodo.numProto = ""; //row["SEGNA_DOCNUMBER"].ToString();

            if (row["OGGETTO_MITTENTE"] != DBNull.Value)
            {
                if (row["OGGETTO_MITTENTE"].ToString() != "")
                {
                    // formato  OGGETTO_MITTENTE:
                    //se protocollo:
                    //OGGETTO || '@@' || VAR_DESC_MITT QUINDI LUNGHEZZA 3
                    //char[] pattern = { '@', '@' };
                    //string[] oggMitt = row["OGGETTO_MITTENTE"].ToString().Split(pattern);

                    string[] pattern = { "@@" };
                    string[] oggMitt = row["OGGETTO_MITTENTE"].ToString().Split(pattern, StringSplitOptions.RemoveEmptyEntries);
       
                    // è un doc grigio
                    if (myTodo.tipoProto == "G")
                    {
                        myTodo.oggetto = oggMitt[0].ToString();
                        //aggiungo il dato video concatenato
                        myTodo.videoOggMitt = oggMitt[0].ToString() + patternVideo;
                        myTodo.videoDocInfo = row["SEGNA_DOCNUMBER"].ToString() + "<br>" + myTodo.dataDoc;
                        myTodo.infoDoc = "IdDoc: " + row["SEGNA_DOCNUMBER"].ToString();

                    }
                    //è un protocollo
                    if ((myTodo.tipoProto == "A" || myTodo.tipoProto == "P"
                        || myTodo.tipoProto == "I") && myTodo.numProto != "")
                    {
                        myTodo.oggetto = oggMitt[0].ToString();

                        //ANOMALIA MITTENTI CON @
                        //if (oggMitt.Length == 3 && oggMitt[2] != null)
                        //{
                        //    myTodo.mittente = oggMitt[2].ToString();
                        //    myTodo.videoOggMitt = oggMitt[0].ToString() + patternVideo + oggMitt[2].ToString();

                        //}
                        // se uscita o interno il mittente può essere anche vuoto, decideremo poi se mettere destinatari
                        if (oggMitt.Length == 2 && oggMitt[1] != null)
                        {
                            myTodo.mittente = oggMitt[1].ToString();
                            myTodo.videoOggMitt = oggMitt[0].ToString() + patternVideo + oggMitt[1].ToString();

                        }
                        else
                        {
                            myTodo.oggetto = oggMitt[0].ToString();
                            //aggiungo il dato video concatenato
                            myTodo.videoOggMitt = oggMitt[0].ToString() + patternVideo;
                        }
                        myTodo.videoDocInfo = myTodo.numProto + "<br>" + myTodo.dataDoc;
                        myTodo.infoDoc = row["SEGNA_DOCNUMBER"].ToString();
                    }

                    if ((myTodo.tipoProto == "A" || myTodo.tipoProto == "P"
                        || myTodo.tipoProto == "I") && myTodo.numProto == "") //predisposto 
                    {
                        myTodo.oggetto = oggMitt[0].ToString();
                        myTodo.oggetto = oggMitt[0].ToString();
                        // se uscita o interno il mittente può essere anche vuoto, decideremo poi se mettere destinatari.

                        //ANOMALIA MITTENTI CON @
                        //// se uscita o interno il mittente può essere anche vuoto, decideremo poi se mettere destinatari.
                        //if (oggMitt.Length == 3 && oggMitt[2] != null)
                        //{
                        //    myTodo.mittente = oggMitt[2].ToString();
                        //    myTodo.videoOggMitt = oggMitt[0].ToString() + patternVideo + oggMitt[2].ToString();

                        //}
                        // se uscita o interno il mittente può essere anche vuoto, decideremo poi se mettere destinatari.
                        if (oggMitt.Length == 2 && oggMitt[1] != null)
                        {
                            myTodo.mittente = oggMitt[1].ToString();
                            myTodo.videoOggMitt = oggMitt[0].ToString() + patternVideo + oggMitt[1].ToString();

                        }
                        else
                        {
                            myTodo.oggetto = oggMitt[0].ToString();
                            //aggiungo il dato video concatenato
                            myTodo.videoOggMitt = oggMitt[0].ToString() + patternVideo;
                        }
                        myTodo.videoDocInfo = row["SEGNA_DOCNUMBER"].ToString() + "<br>" + myTodo.dataDoc;
                        myTodo.infoDoc = "IdDoc: " + row["SEGNA_DOCNUMBER"].ToString();
                    }
                }
            }
            if (row["NUM_PROTO"] != DBNull.Value)
            {
                if (row["NUM_PROTO"].ToString() != "")
                    myTodo.numProto = row["NUM_PROTO"].ToString();
                else
                    myTodo.numProto = row["SEGNA_DOCNUMBER"].ToString();
            }
            else myTodo.numProto = row["SEGNA_DOCNUMBER"].ToString();

            if (row["CHA_IMG"] != DBNull.Value)
            {
                myTodo.cha_img = row["CHA_IMG"].ToString();
            }

            string msg;
            // Se l'utente ha i diritti necessari di visibilità sul 
            // documento
            if (new Documenti().VerificaACL("D", myTodo.sysIdDoc,
                currentIdPeople, currentIdGruppo, out msg) != 2)
                myTodo.videoOggMitt = "<b>Non si possiedono i diritti per la visualizzazione delle informazioni sul documento</b>";

            // Aggiunta dell'eventuale tipologia associata al documento
            Model model = new Model();
            myTodo.VideoTipology = model.GetTipologyDescriptionByIdProfile(myTodo.sysIdDoc);

            logger.Info("END");
            return myTodo;
        }

        //utils
        public string getwhereFilter(DocsPaVO.filtri.FiltroRicerca[] filter)
        {
            string filterWhere = String.Empty;
            string filterWhereFasc = string.Empty;
            string filterWhereDoc = string.Empty;
            DocsPaVO.filtri.FiltroRicerca f;
            string UserDB = String.Empty;
            string docOrFasc = "";
            bool tipoRicerca = getTipoRicerca(filter);
            // verifica dbms
            if (dbType.ToUpper() == "SQL") UserDB = getUserDB();

            if (filter != null)
            {
                for (int i = 0; i < filter.Length; i++)
                {
                    f = filter[i];
                    if (f.valore != null && !f.valore.Equals(""))
                    {
                        switch (f.argomento)
                        {
                            case "TIPO_OGGETTO":
                                string valore = f.valore;
                                if (f.valore == "D")
                                    filterWhere += " and ID_PROFILE > 0";
                                if (f.valore == "F")
                                    filterWhere += " and ID_PROJECT > 0";
                                //if (f.valore == "T")
                                //filterWhere += " and (ID_PROFILE > 0 OR ID_PROJECT > 0)";
                                docOrFasc = f.valore;
                                break;
                            //GESTIONE OGGETTO DEL DOCUMENTO
                            case "OGGETTO_DOCUMENTO_TRASMESSO":
                                if (!string.IsNullOrEmpty(UserDB))
                                    filterWhereDoc += " AND UPPER(" + UserDB + ".Vardescribe(ID_PROFILE, 'DESC_OGGETTO')) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                                else
                                    filterWhereDoc += " AND UPPER(Vardescribe(ID_PROFILE, 'DESC_OGGETTO')) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                                break;
                            //GESTIONE OGGETTO DEL FASCICOLO
                            case "OGGETTO_FASCICOLO_TRASMESSO":
                                if (!string.IsNullOrEmpty(UserDB))
                                    filterWhereFasc += " AND UPPER(" + UserDB + ".Vardescribe(ID_PROJECT, 'DESC_FASC')) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                                else
                                    filterWhereFasc += " AND UPPER(Vardescribe(ID_PROJECT, 'DESC_FASC')) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                                break;
                            //GESTIONE FILTRO RAGIONE DI TRASMISSIONE
                            case "RAGIONE":
                                if (!string.IsNullOrEmpty(UserDB))
                                    filterWhere += " AND UPPER(" + UserDB + ".Vardescribe(ID_RAGIONE_TRASM, 'RAGIONETRASM')) = '" + f.valore.ToString().ToUpper() + "'";
                                else
                                    filterWhere += " AND UPPER(Vardescribe(ID_RAGIONE_TRASM, 'RAGIONETRASM')) = '" + f.valore.ToString().ToUpper() + "'";
                                break;
                            //GESTIONE FILTRI DATA DI TRASMISSIONE
                            case "TRASMISSIONE_IL":
                                filterWhere += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("DTA_INVIO", false) + "= '" + f.valore + "'";
                                break;
                            case "TRASMISSIONE_SUCCESSIVA_AL":
                                filterWhere += " AND DTA_INVIO>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                                break;
                            case "TRASMISSIONE_PRECEDENTE_IL":
                                filterWhere += " AND DTA_INVIO<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                                break;
                            //GESTIONE FILTRI SCADENZA
                            case "SCADENZA_IL":
                                filterWhere += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("DTA_SCADENZA", false) + "= '" + f.valore + "'";
                                break;
                            case "SCADENZA_SUCCESSIVA_AL":
                                filterWhere += " AND DTA_SCADENZA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                                break;
                            case "SCADENZA_PRECEDENTE_IL":
                                filterWhere += " AND DTA_SCADENZA<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                                break;
                            //GESTIONE FILTRI MITTENTE TRASMISSIONE DA RUBRICA
                            case "COD_RUBR_MITT_RUOLO":
                                filterWhere += " AND ID_RUOLO_MITT IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '" + f.valore.ToUpper().Replace("'", "''") + "')";
                                break;
                            case "COD_RUBR_MITT_UTENTE":
                                filterWhere += " AND ID_PEOPLE_MITT = (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '" + f.valore.ToUpper().Replace("'", "''") + "')";
                                break;
                            case "ID_UO_MITT":
                                filterWhere += " AND ID_RUOLO_MITT IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND ID_UO =" + f.valore + ")";
                                break;
                            //GESTIONE FILTRI MITTENTE TRASMISSIONE DA INSERIMENTO TESTUALE
                            case "MITTENTE_RUOLO":
                                filterWhere += " AND ID_RUOLO_MITT IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%')";
                                break;
                            case "MITTENTE_UTENTE":
                                filterWhere += " AND ID_PEOPLE_MITT IN (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%')";
                                break;
                            case "MITTENTE_UO":
                                filterWhere += " AND ID_RUOLO_MITT IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND ID_UO =(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE UPPER(VAR_DESC_CORR) IN ('" + f.valore.ToUpper().Replace("'", "''") + "')))";
                                break;
                            case "TIPO":
                                if (tipoRicerca)
                                {
                                    filterWhereDoc += " AND ";
                                    if (f.valore.Equals("T"))
                                    {
                                        if (!string.IsNullOrEmpty(UserDB))
                                        {
                                            filterWhereDoc += " UPPER(" + UserDB + ".Vardescribe(ID_PROFILE, 'CHA_TIPO_PROTO')) IN ('A', 'P', 'I','G') ";
                                        }
                                        else
                                        {
                                            filterWhereDoc += " UPPER(Vardescribe(ID_PROFILE, 'CHA_TIPO_PROTO')) IN  ('A', 'P', 'I','G') ";
                                        }
                                    }
                                    else
                                        if (f.valore.Equals("PR"))
                                        {
                                            filterWhereDoc += " id_profile in (select system_id from profile where cha_da_proto = '1') ";
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(UserDB))
                                            {
                                                filterWhereDoc += " UPPER(" + UserDB + ".Vardescribe(ID_PROFILE, 'CHA_TIPO_PROTO')) = '" + f.valore.ToUpper() + "'";
                                            }
                                            else
                                            {
                                                filterWhereDoc += " UPPER(Vardescribe(ID_PROFILE, 'CHA_TIPO_PROTO')) = '" + f.valore.ToUpper() + "'";
                                            }


                                        }
                                }
                                break;
                            case "ELEMENTI_NON_VISTI":
                                if (f.valore.Equals("1"))
                                    filterWhere += " AND dta_vista = " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753");
                                break;
                            case "DOCUMENTI_ACQUISITI":
                                if (f.valore.Equals("1"))
                                    filterWhereDoc += " AND UPPER(Vardescribe(ID_PROFILE, 'PROFILE_CHA_IMG')) <> '0'";
                                break;
                            case "DOCUMENTI_FIRMATI":
                                if (f.valore.Equals("1"))
                                {
                                    if (!string.IsNullOrEmpty(UserDB))
                                    {
                                        filterWhereDoc += " AND UPPER(" + UserDB + ".Vardescribe(ID_PROFILE, 'COMPONENTS_CHA_FIRMATO')) = '" + f.valore.ToUpper() + "'";
                                    }
                                    else
                                    {
                                        filterWhereDoc += " AND UPPER(Vardescribe(ID_PROFILE, 'COMPONENTS_CHA_FIRMATO')) = '" + f.valore.ToUpper() + "'";
                                    }
                                }
                                else
                                {
                                    if (f.valore.Equals("0"))
                                    {
                                        if (!string.IsNullOrEmpty(UserDB))
                                        {
                                            filterWhereDoc += " AND UPPER(" + UserDB + ".Vardescribe(ID_PROFILE, 'COMPONENTS_CHA_FIRMATO')) = '" + f.valore.ToUpper() + "'";
                                        }
                                        else
                                        {
                                            filterWhereDoc += " AND UPPER(Vardescribe(ID_PROFILE, 'COMPONENTS_CHA_FIRMATO')) = '" + f.valore.ToUpper() + "'";
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(UserDB))
                                        {
                                            filterWhereDoc += " AND UPPER(" + UserDB + ".Vardescribe(ID_PROFILE, 'PROFILE_CHA_IMG')) <> '0'";
                                        }
                                        else
                                        {
                                            filterWhereDoc += " AND UPPER(Vardescribe(ID_PROFILE, 'PROFILE_CHA_IMG')) <> '0'";
                                        }
                                    }
                                }
                                break;
                            case "TIPO_FILE_ACQUISITO":
                                if (!string.IsNullOrEmpty(UserDB))
                                {
                                    filterWhereDoc += " AND" + UserDB + ".vardescribe(ID_PROFILE, 'PROFILE_CHA_IMG')='" + f.valore.ToUpper() + "'";
                                }
                                else
                                {
                                    filterWhereDoc += " AND vardescribe(ID_PROFILE, 'PROFILE_CHA_IMG')='" + f.valore.ToUpper() + "'";
                                }
                                break;
                            case "TRASMISSIONI_ACCETTATE":
                                if (f.valore.Equals("1"))
                                    //filterWhereDoc += " and id_trasm_utente in (select system_id from dpa_trasm_utente where cha_accettata='0' )";
                                    filterWhereDoc += " and exists (select dpa_ragione_trasm.system_id from dpa_ragione_trasm where id_ragione_trasm = dpa_ragione_trasm.system_id and dpa_ragione_trasm.CHA_TIPO_RAGIONE = 'W' ) AND dta_vista = " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753");
                                break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(docOrFasc))
                {
                    if (docOrFasc == "D")
                        filterWhere += filterWhereDoc;
                    if (docOrFasc == "F")
                        filterWhere += filterWhereFasc;
                    if (docOrFasc == "T")
                    {
                        filterWhere += "and ( (ID_PROFILE > 0 " + filterWhereDoc + ") or (ID_PROJECT > 0 " + filterWhereFasc + ") )";

                    }
                }
                else
                    filterWhere += filterWhereDoc + filterWhereFasc;
            }
            return filterWhere;
        }

        //Metodo necessario per non effettuare la ricerca per "TIPO" nel metodo "getwhereFilter" 
        //quando il "TIPO_OGGETTO" è un fascicolo.        
        public bool getTipoRicerca(DocsPaVO.filtri.FiltroRicerca[] filter)
        {
            for (int i = 0; i < filter.Length; i++)
            {
                DocsPaVO.filtri.FiltroRicerca f = filter[i];
                if (f.argomento == "TIPO_OGGETTO" && f.valore == "F")
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Ritorna la clausola ORDER BY da applicare parametricamente sulle query della todolist e di smistamento.
        /// 
        /// se filter=null,
        /// si applica l'order primario di default DTAINVIO DESC
        /// se type="", sia applica l'order secondario di default ID_PROFILE DESC
        /// type [D = documenti; F=fascicoli; S=smistamento]
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string getorderFilter(DocsPaVO.filtri.FiltroRicerca[] filter, string type)
        {
            string filterOrder = String.Empty;
            DocsPaVO.filtri.FiltroRicerca f;

            if (filter != null)
            {
                for (int i = 0; i < filter.Length; i++)
                {
                    f = filter[i];
                    if (f.valore != null && !f.valore.Equals(""))
                    {
                        switch (f.argomento)
                        {
                            //GESTIONE OGGETTO DEL DOCUMENTO
                            case "DTA_INVIO_DESC":
                                filterOrder += " DTA_INVIO DESC";
                                break;
                            case "DTA_SCAD_DESC":
                                filterOrder += " DTA_SCAD_DESC DESC";
                                break;
                            case "DTA_SCAD_ASC":
                                filterOrder += " DTA_SCAD_ASC ASC";
                                break;
                            case "DTA_INVIO_ASC":
                                filterOrder += " DTA_INVIO ASC";
                                break;

                        }
                    }
                }
            }

            //per settare un ordinamento di default
            if (filterOrder == "")
                filterOrder += " DTA_INVIO DESC";

            //POI VERIFICO SE FASC "F" O DOC "D" e aggiungo order necessario:

            switch (type)
            {
                case "D":
                    filterOrder += ",ID_PROFILE DESC";
                    break;
                case "F":
                    filterOrder += ",ID_PROJECT DESC";
                    break;
                case "S": //SMISTAMENTO
                    filterOrder += ",ID_TRASMISSIONE DESC";
                    break;
                //non dovrebbe essere utile.
                default:
                    filterOrder += ",ID_PROFILE DESC";
                    break;
            }


            return filterOrder;
        }


        public string getNewOrderFilter(DocsPaVO.filtri.FiltroRicerca[] filter)
        {
            string filterOrder = String.Empty;
            DocsPaVO.filtri.FiltroRicerca f;

            if (filter != null)
            {
                for (int i = 0; i < filter.Length; i++)
                {
                    f = filter[i];
                    if (f.valore != null && !f.valore.Equals(""))
                    {
                        switch (f.argomento)
                        {
                            //GESTIONE OGGETTO DEL DOCUMENTO
                            case "DTA_INVIO_DESC":
                                filterOrder += " DTA_INVIO DESC";
                                break;
                            case "DTA_SCAD_DESC":
                                filterOrder += " DTA_SCAD_DESC DESC";
                                break;
                            case "DTA_SCAD_ASC":
                                filterOrder += " DTA_SCAD_ASC ASC";
                                break;
                            case "DTA_INVIO_ASC":
                                filterOrder += " DTA_INVIO ASC";
                                break;

                        }
                    }
                }
            }

            //per settare un ordinamento di default
            if (filterOrder == "")
                filterOrder += " DTA_INVIO DESC";

            //POI VERIFICO SE FASC "F" O DOC "D" e aggiungo order necessario:
            //Nuova gestione unificata non ho più doc o fasc, ma insieme
            //switch (type)
            //{
            //    case "D":
            //        filterOrder += ",ID_PROFILE DESC";
            //        break;
            //    case "F":
            //        filterOrder += ",ID_PROJECT DESC";
            //        break;
            //    case "S": //SMISTAMENTO
            //        filterOrder += ",ID_TRASMISSIONE DESC";
            //        break;
            //    //non dovrebbe essere utile.
            //    default:
            //        filterOrder += ",ID_PROFILE DESC";
            //        break;
            //}


            return filterOrder;
        }


        public string getUserDB()
        {
            return Functions.GetDbUserSession();
        }

        #endregion

        #region InfoTrasmissioni

        #region Trasmissioni effettuate

        /// <summary>
        /// Reperimento delle trasmissioni effettuate di un documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocOrFasc"></param>
        /// <param name="docOrFasc"></param>
        /// <param name="pagingContext">
        /// Contesto paginazione
        /// </param>
        /// <returns></returns>
        public DocsPaVO.trasmissione.InfoTrasmissione[] GetInfoTrasmissioniEffettuate(
                DocsPaVO.utente.InfoUtente infoUtente,
                string idDocOrFasc, string docOrFasc,
                DocsPaVO.ricerche.SearchPagingContext pagingContext)
        {
            List<DocsPaVO.trasmissione.InfoTrasmissione> list = new List<DocsPaVO.trasmissione.InfoTrasmissione>();

            // Reperimento numero di record totali
            pagingContext.SetRecordCount(this.GetCountInfoTrasmissioniEffettuate(infoUtente, idDocOrFasc, docOrFasc));

            if (pagingContext.RecordCount > 0)
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_INFO_TRASMISSIONI_EFFETTUATE");

                queryDef.setParam("dtaInvio", DocsPaDbManagement.Functions.Functions.ToChar("dta_invio", false));

                string filters = string.Empty;
                if (docOrFasc == "D")
                    filters = string.Format("a.id_profile = {0}", idDocOrFasc);
                else if (docOrFasc == "F")
                    filters = string.Format("a.id_project = {0}", idDocOrFasc);
                queryDef.setParam("filters", filters);

                // Impostazione parametri paginazione
                queryDef.setParam("startRow", pagingContext.StartRow.ToString());
                queryDef.setParam("endRow", pagingContext.EndRow.ToString());

                // INIZIO - Parametri specifici per SqlServer
                // il numero totale di righe da estrarre equivale 
                // al limite inferiore dell'ultima riga da estrarre
                int pageSizeSqlServer = pagingContext.PageSize;
                int totalRowsSqlServer = (pagingContext.Page * pagingContext.PageSize);
                if ((pagingContext.RecordCount - totalRowsSqlServer) <= 0)
                {
                    pageSizeSqlServer -= System.Math.Abs(pagingContext.RecordCount - totalRowsSqlServer);
                    totalRowsSqlServer = pagingContext.RecordCount;
                }
                queryDef.setParam("pageSize", pageSizeSqlServer.ToString()); // Dimensione pagina
                queryDef.setParam("totalRows", totalRowsSqlServer.ToString());

                queryDef.setParam("dbUser", this.getUserDB());
                // FINE - Parametri specifici per SqlServer

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                            list.Add(this.CreateInfoTrasmissione(reader));
                    }
                }
            }

            return list.ToArray();
        }

        #endregion

        #region Trasmissioni ricevute

        /// <summary>
        /// Reperimento delle trasmissioni effettuate di un documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocOrFasc"></param>
        /// <param name="idObject"></param>
        /// <param name="pagingContext">
        /// Contesto paginazione
        /// </param>
        /// <returns></returns>
        public DocsPaVO.trasmissione.InfoTrasmissione[] GetInfoTrasmissioniRicevute(
                DocsPaVO.utente.InfoUtente infoUtente,
                string idDocOrFasc, string docOrFasc,
                DocsPaVO.ricerche.SearchPagingContext pagingContext)
        {
            List<DocsPaVO.trasmissione.InfoTrasmissione> list = new List<DocsPaVO.trasmissione.InfoTrasmissione>();

            // Reperimento id della persona in corrglobali
            string idPeopleCorrGlobale = this.getQueryEffMet1(infoUtente.idPeople);

            // Reperimento numero di record totali
            pagingContext.SetRecordCount(this.GetCountInfoTrasmissioniRicevute(infoUtente, idPeopleCorrGlobale, idDocOrFasc, docOrFasc));

            if (pagingContext.RecordCount > 0)
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_INFO_TRASMISSIONI_RICEVUTE");

                queryDef.setParam("dtaInvio", DocsPaDbManagement.Functions.Functions.ToChar("dta_invio", false));
                queryDef.setParam("dtaScadenza", DocsPaDbManagement.Functions.Functions.ToChar("dta_Scadenza", false));

                string filters = string.Empty;
                if (docOrFasc == "D")
                    filters = string.Format("t.id_profile = {0}", idDocOrFasc);
                else if (docOrFasc == "F")
                    filters = string.Format("t.id_project = {0}", idDocOrFasc);
                queryDef.setParam("filters", filters);

                queryDef.setParam("idCorrGlobale", infoUtente.idCorrGlobali);
                queryDef.setParam("idPeople", infoUtente.idPeople);
                queryDef.setParam("idPeopleCorrGlobale", idPeopleCorrGlobale);

                // Impostazione parametri paginazione
                queryDef.setParam("startRow", pagingContext.StartRow.ToString());
                queryDef.setParam("endRow", pagingContext.EndRow.ToString());

                // INIZIO - Parametri specifici per SqlServer
                // il numero totale di righe da estrarre equivale 
                // al limite inferiore dell'ultima riga da estrarre
                int pageSizeSqlServer = pagingContext.PageSize;
                int totalRowsSqlServer = (pagingContext.Page * pagingContext.PageSize);
                if ((pagingContext.RecordCount - totalRowsSqlServer) <= 0)
                {
                    pageSizeSqlServer -= System.Math.Abs(pagingContext.RecordCount - totalRowsSqlServer);
                    totalRowsSqlServer = pagingContext.RecordCount;
                }
                queryDef.setParam("pageSize", pageSizeSqlServer.ToString()); // Dimensione pagina
                queryDef.setParam("totalRows", totalRowsSqlServer.ToString());

                queryDef.setParam("dbUser", this.getUserDB());
                // FINE - Parametri specifici per SqlServer

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            list.Add(this.CreateInfoTrasmissioneRicevuta(reader));
                        }
                    }
                }
            }

            return list.ToArray();
        }

        #endregion

        /// <summary>
        /// Reperimento delle trasmissioni filtrate di un documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocOrFasc"></param>
        /// <param name="docOrFasc"></param>
        /// <param name="pagingContext">
        /// Contesto paginazione
        /// </param>
        /// <returns></returns>
        public DocsPaVO.trasmissione.InfoTrasmissione[] GetInfoTrasmissioniFiltered(
                DocsPaVO.utente.InfoUtente infoUtente
                , string idDocOrFasc, string docOrFasc
                , DocsPaVO.filtri.FiltroRicerca[] objListaFiltri
                , ref DocsPaVO.ricerche.SearchPagingContext pagingContext)
        {
            List<DocsPaVO.trasmissione.InfoTrasmissione> list = new List<DocsPaVO.trasmissione.InfoTrasmissione>();

            // Reperimento numero di record totali
            pagingContext.SetRecordCount(this.GetCountInfoTrasmissioniFiltered(infoUtente, idDocOrFasc, docOrFasc, objListaFiltri));

            if (pagingContext.RecordCount > 0)
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_INFO_TRASMISSIONI_FILTERED");

                queryDef.setParam("dtaInvio", DocsPaDbManagement.Functions.Functions.ToChar("dta_invio", true));

                string filters = string.Empty;

                switch (docOrFasc)
                {
                    case "D":
                        filters = string.Format("a.id_profile = {0}", idDocOrFasc);
                        break;
                    case "F":
                        filters = string.Format("a.id_project = {0}", idDocOrFasc);
                        break;
                }

                bool extendFromStruct = false;
                bool extendSingleTransmission = false;
                DocsPaVO.filtri.FiltroRicerca f;
                string filtersInOr = string.Empty, filtersInOr2 = string.Empty;
                bool orCondition = false;
                bool orCondition2 = false;
                for (int i = 0; i < objListaFiltri.Length; i++)
                {
                    f = objListaFiltri[i];
                    if (f.valore != null && !f.valore.Equals(""))
                    {
                        switch (f.argomento)
                        {
                            case "MY_RECEIVED_TRANSMISSIONS":
                                if (orCondition)
                                    filtersInOr += " OR ";
                                filtersInOr += " ((ts.id_corr_globale = " + f.valore.Split('|')[0] + " AND tu.id_people = " + f.valore.Split('|')[1] + ") OR (ts.id_corr_globale = " + f.valore.Split('|')[2] + ")) ";
                                extendFromStruct = true;
                                extendSingleTransmission = true;
                                orCondition = true;
                                break;
                            case "MITTENTE_RUOLO":
                                if (orCondition)
                                    filtersInOr += " AND ";
                                filtersInOr += "(a.id_ruolo_in_uo IN (";
                                 FiltroRicerca filterRubMitt = objListaFiltri.Where(e => e.argomento == listaArgomentiNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString()).FirstOrDefault();
                                if (filterRubMitt != null && Convert.ToBoolean(filterRubMitt.valore))
                                {
                                    filtersInOr += "SELECT system_id FROM dpa_corr_globali WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' START WITH SYSTEM_ID = '" + f.valore.ToUpper().Replace("'", "''") + "' CONNECT BY PRIOR id_old = system_id))";
                                }
                                else
                                    filtersInOr += f.valore + "))";
                                orCondition = true;
                                break;
                            case "EFFETTUATE_RUOLI_IN_RF":
                                if (orCondition)
                                    filtersInOr += " OR ";
                                filtersInOr += "(a.id_ruolo_in_uo in (SELECT id_ruolo_in_uo from dpa_l_ruolo_reg reg where reg.id_registro = " + f.valore + "))";
                                orCondition = true;
                                break;
                            case "MITTENTE_UTENTE":
                                filters += " AND a.id_people = " + f.valore + " ";
                                break;
                            case "DESTINATARIO_RUOLO":
                                //filters += " AND ts.id_corr_globale = " + f.valore + " ";
                                filters += " AND ts.id_corr_globale IN (";
                                 FiltroRicerca filterRubDest = objListaFiltri.Where(e => e.argomento == listaArgomentiNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString()).FirstOrDefault();
                                 if (filterRubDest != null && Convert.ToBoolean(filterRubDest.valore))
                                {
                                    filters += "SELECT system_id FROM dpa_corr_globali WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' START WITH SYSTEM_ID = '" + f.valore.ToUpper().Replace("'", "''") + "' CONNECT BY PRIOR id_old = system_id)";
                                }
                                else
                                     filters += f.valore + ")";
                                //extendFromStruct = true;
                                extendSingleTransmission = true;
                                break;
                            case "DESTINATARIO_UTENTE":
                                filters += " AND tu.id_people = " + f.valore + " ";
                                extendSingleTransmission = true;
                                extendFromStruct = true;
                                break;
                            case "RAGIONE":
                                filters += " AND ts.id_ragione = " + f.valore + " ";
                                extendSingleTransmission = true;
                                break;
                            case "DTA_INVIO":
                                filters += " AND a.dta_invio between to_date('" + f.valore + "','DD/MM/YYYY') and to_date('" + f.valore + "','DD/MM/YYYY')+1 ";
                                break;
                            case "DATA_ACCETTAZIONE_DA":
                                if (orCondition2)
                                    filtersInOr2 += " OR ";
                                //filtersInOr2 += "(tu.dta_accettata>to_date('01/01/1900','DD/MM/YYYY'))";
                                //extendFromStruct = true;
                                //extendSingleTransmission = true;


                                filtersInOr2 += "EXISTS(SELECT 'X' FROM DPA_TRASM_UTENTE tu, DPA_TRASM_SINGOLA ts WHERE ts.id_trasmissione= a.system_id and tu.id_trasm_singola= ts.system_id and tu.dta_accettata>to_date('01/01/1900','DD/MM/YYYY'))";
                                orCondition2 = true;
                                break;
                            case "DATA_RIFIUTO_DA":
                                if (orCondition2)
                                    filtersInOr2 += " OR ";
                                //filtersInOr2 += "(tu.dta_rifiutata>to_date('01/01/1900','DD/MM/YYYY'))";
                                //extendFromStruct = true;
                                //extendSingleTransmission = true;
                                filtersInOr2 += "EXISTS(SELECT 'X' FROM DPA_TRASM_UTENTE tu, DPA_TRASM_SINGOLA ts WHERE ts.id_trasmissione= a.system_id and tu.id_trasm_singola= ts.system_id and tu.dta_rifiutata>to_date('01/01/1900','DD/MM/YYYY'))";
                                

                                orCondition2 = true;
                                break;
                            case "VISTE":
                                if (orCondition2)
                                    filtersInOr2 += " OR ";
                                //filtersInOr2 += "(tu.dta_vista>to_date('01/01/1900','DD/MM/YYYY'))";
                                //extendFromStruct = true;
                                //extendSingleTransmission = true;
                                filtersInOr2 += "EXISTS(SELECT 'X' FROM DPA_TRASM_UTENTE tu, DPA_TRASM_SINGOLA ts WHERE ts.id_trasmissione= a.system_id and tu.id_trasm_singola= ts.system_id and tu.dta_vista>to_date('01/01/1900','DD/MM/YYYY'))";
                                

                                orCondition2 = true;
                                break;
                            case "PENDENTI":
                                if (orCondition2)
                                    filtersInOr2 += " OR ";
                                //filtersInOr2 += "(tu.dta_accettata IS NULL AND tu.dta_rifiutata IS NULL AND exists (select 'x' from dpa_ragione_trasm r where r.cha_tipo_ragione='W' and ts.id_ragione = r.system_id ))";
                                //extendFromStruct = true;
                                //extendSingleTransmission = true;
                                
                            //NOT EXISTS(SELECT 'X' FROM DPA_TRASM_UTENTE tu, DPA_TRASM_SINGOLA ts WHERE tu.id_trasm_singola= ts.system_id and (tu.dta_accettata IS NOT NULL OR tu.dta_rifiutata IS NOT NULL) AND ts.ID_TRASMISSIONE=A.SYSTEM_ID  AND EXISTS (select 'Y' from dpa_ragione_trasm r where r.cha_tipo_ragione='W' and ts.id_ragione = r.system_id ))
                                filtersInOr2 += "NOT EXISTS(SELECT 'X' FROM DPA_TRASM_UTENTE tu, DPA_TRASM_SINGOLA ts WHERE tu.id_trasm_singola= ts.system_id and (tu.dta_accettata IS NOT NULL OR tu.dta_rifiutata IS NOT NULL) AND ts.ID_TRASMISSIONE=A.SYSTEM_ID  AND EXISTS (select 'Y' from dpa_ragione_trasm r where r.cha_tipo_ragione='W' and ts.id_ragione = r.system_id ))";

                                orCondition2 = true;
                                break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(filtersInOr))
                    filters += " AND(" + filtersInOr + ") ";
                if (!string.IsNullOrEmpty(filtersInOr2))
                    filters += " AND(" + filtersInOr2 + ") ";
                queryDef.setParam("filters", filters);

                string sqlFrom = string.Empty;

                if (extendSingleTransmission)
                {
                    sqlFrom += "INNER JOIN dpa_trasm_singola ts on a.system_id = ts.id_trasmissione ";
                }


                if (extendFromStruct)
                {
                    sqlFrom += "INNER JOIN dpa_trasm_utente tu ON ts.system_id = tu.id_trasm_singola ";

                }


                queryDef.setParam("from", sqlFrom);

                // Impostazione parametri paginazione
                queryDef.setParam("startRow", pagingContext.StartRow.ToString());
                queryDef.setParam("endRow", pagingContext.EndRow.ToString());

                // INIZIO - Parametri specifici per SqlServer
                // il numero totale di righe da estrarre equivale 
                // al limite inferiore dell'ultima riga da estrarre
                int pageSizeSqlServer = pagingContext.PageSize;
                int totalRowsSqlServer = (pagingContext.Page * pagingContext.PageSize);
                if ((pagingContext.RecordCount - totalRowsSqlServer) <= 0)
                {
                    pageSizeSqlServer -= System.Math.Abs(pagingContext.RecordCount - totalRowsSqlServer);
                    totalRowsSqlServer = pagingContext.RecordCount;
                }
                queryDef.setParam("pageSize", pageSizeSqlServer.ToString()); // Dimensione pagina
                queryDef.setParam("totalRows", totalRowsSqlServer.ToString());

                queryDef.setParam("dbUser", this.getUserDB());
                // FINE - Parametri specifici per SqlServer

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                            list.Add(this.CreateInfoTrasmissione(reader));
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocOrFasc"></param>
        /// <param name="docOrFasc"></param>
        /// <returns></returns>
        public int GetCountInfoTrasmissioniFiltered(DocsPaVO.utente.InfoUtente infoUtente, string idDocOrFasc, string docOrFasc, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            int count = 0;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_INFO_TRASMISSIONI_FILTERED");

            string filters = string.Empty;
            string filtersInOr = string.Empty, filtersInOr2 = string.Empty;
            bool orCondition = false;
            bool orCondition2 = false;
            switch (docOrFasc)
            {
                case "D":
                    filters = string.Format("a.id_profile = {0}", idDocOrFasc);
                    break;
                case "F":
                    filters = string.Format("a.id_project = {0}", idDocOrFasc);
                    break;
            }

            bool extendFromStruct = false;
            DocsPaVO.filtri.FiltroRicerca f;

            for (int i = 0; i < objListaFiltri.Length; i++)
            {
                f = objListaFiltri[i];
                if (f.valore != null && !f.valore.Equals(""))
                {
                    switch (f.argomento)
                    {
                        case "MY_RECEIVED_TRANSMISSIONS":
                            if (orCondition)
                                filtersInOr += " OR ";
                            filtersInOr += " ((ts.id_corr_globale = " + f.valore.Split('|')[0] + " AND tu.id_people = " + f.valore.Split('|')[1] + ") OR (ts.id_corr_globale = " + f.valore.Split('|')[2] + ")) ";
                            extendFromStruct = true;
                            orCondition = true;
                            break;
                        case "MITTENTE_RUOLO":
                            if (orCondition)
                                filtersInOr += " OR ";
                            filtersInOr += "(a.id_ruolo_in_uo IN (";
                                 FiltroRicerca filterRubMitt = objListaFiltri.Where(e => e.argomento == listaArgomentiNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString()).FirstOrDefault();
                                if (filterRubMitt != null && Convert.ToBoolean(filterRubMitt.valore))
                                {
                                    filtersInOr += "SELECT system_id FROM dpa_corr_globali WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' START WITH SYSTEM_ID = '" + f.valore.ToUpper().Replace("'", "''") + "' CONNECT BY PRIOR id_old = system_id))";
                                }
                                else
                                    filtersInOr += f.valore + "))";
                            orCondition = true;
                            break;
                        case "EFFETTUATE_RUOLI_IN_RF":
                            if (orCondition)
                                filtersInOr += " OR ";
                            filtersInOr += "(a.id_ruolo_in_uo in (SELECT id_ruolo_in_uo from dpa_l_ruolo_reg reg where reg.id_registro = " + f.valore + "))";
                            orCondition = true;
                            break;
                        case "MITTENTE_UTENTE":
                            filters += " AND a.id_people = " + f.valore + " ";
                            break;
                        case "DESTINATARIO_RUOLO":
                            filters += " AND ts.id_corr_globale IN (";
                            FiltroRicerca filterRubDest = objListaFiltri.Where(e => e.argomento == listaArgomentiNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString()).FirstOrDefault();
                            if (filterRubDest != null && Convert.ToBoolean(filterRubDest.valore))
                            {
                                filters += "SELECT system_id FROM dpa_corr_globali WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' START WITH SYSTEM_ID = '" + f.valore.ToUpper().Replace("'", "''") + "' CONNECT BY PRIOR id_old = system_id)";
                            }
                            else
                                filters += f.valore + ")";
                            extendFromStruct = true;
                            break;
                        case "DESTINATARIO_UTENTE":
                            filters += " AND tu.id_people = " + f.valore + " ";
                            extendFromStruct = true;
                            break;
                        case "RAGIONE":
                            filters += " AND ts.id_ragione = " + f.valore + " ";
                            extendFromStruct = true;
                            break;
                        case "DTA_INVIO":
                            filters += " AND a.dta_invio between to_date('" + f.valore + "','DD/MM/YYYY') and to_date('" + f.valore + "','DD/MM/YYYY')+1 ";
                            break;
                        case "DATA_ACCETTAZIONE_DA":
                            if (orCondition2)
                                filtersInOr2 += " OR ";
                            filtersInOr2 += "(tu.dta_accettata>to_date('01/01/1900','DD/MM/YYYY'))";
                            extendFromStruct = true;
                            orCondition2 = true;
                            break;
                        case "DATA_RIFIUTO_DA":
                            if (orCondition2)
                                filtersInOr2 += " OR ";
                            filtersInOr2 += "(tu.dta_rifiutata>to_date('01/01/1900','DD/MM/YYYY'))";
                            extendFromStruct = true;
                            orCondition2 = true;
                            break;
                        case "VISTE":
                            if (orCondition2)
                                filtersInOr2 += " OR ";
                            filtersInOr2 += "(tu.dta_vista>to_date('01/01/1900','DD/MM/YYYY'))";
                            extendFromStruct = true;
                            orCondition2 = true;
                            break;
                        case "PENDENTI":
                            if (orCondition2)
                                filtersInOr2 += " OR ";
                            filtersInOr2 += "(tu.dta_accettata IS NULL AND tu.dta_rifiutata IS NULL AND exists (select 'x' from dpa_ragione_trasm r where r.cha_tipo_ragione='W' and ts.id_ragione = r.system_id ))";
                            extendFromStruct = true;
                            orCondition2 = true;
                            break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(filtersInOr))
                filters += " AND(" + filtersInOr + ") ";
            if (!string.IsNullOrEmpty(filtersInOr2))
                filters += " AND(" + filtersInOr2 + ") ";
            queryDef.setParam("filters", filters);

            string sqlFrom = string.Empty;
            if (extendFromStruct)
            {
                sqlFrom = "INNER JOIN dpa_trasm_singola ts on a.system_id = ts.id_trasmissione "
                        + "INNER JOIN dpa_trasm_utente tu ON ts.system_id = tu.id_trasm_singola ";
            }
            queryDef.setParam("from", sqlFrom);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    count = Convert.ToInt32(field);
            }

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="docOrFasc"></param>
        /// <param name="idDocOrFasc"></param>
        /// <returns></returns>
        private int GetCountInfoTrasmissioniEffettuate(DocsPaVO.utente.InfoUtente infoUtente, string idDocOrFasc, string docOrFasc)
        {
            int count = 0;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_INFO_TRASMISSIONI_EFFETTUATE");

            string filters = string.Empty;
            if (docOrFasc == "D")
                filters = string.Format("id_profile = {0}", idDocOrFasc);
            else if (docOrFasc == "F")
                filters = string.Format("id_project = {0}", idDocOrFasc);
            queryDef.setParam("filters", filters);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    count = Convert.ToInt32(field);
            }

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="docOrFasc"></param>
        /// <param name="idDocOrFasc"></param>
        /// <param name="queryName"></param>
        /// <returns></returns>
        private int GetCountInfoTrasmissioniRicevute(DocsPaVO.utente.InfoUtente infoUtente, string idPeopleCorrGlobali, string idDocOrFasc, string docOrFasc)
        {
            int count = 0;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_INFO_TRASMISSIONI_RICEVUTE");

            string filters = string.Empty;
            if (docOrFasc == "D")
                filters = string.Format("id_profile = {0}", idDocOrFasc);
            else if (docOrFasc == "F")
                filters = string.Format("id_project = {0}", idDocOrFasc);
            queryDef.setParam("filters", filters);

            queryDef.setParam("idCorrGlobale", infoUtente.idCorrGlobali);
            queryDef.setParam("idPeople", infoUtente.idPeople);
            queryDef.setParam("idPeopleCorrGlobale", idPeopleCorrGlobali);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    count = Convert.ToInt32(field);
            }

            return count;
        }

        /// <summary>
        /// Creazione oggetto InfoTrasmissione
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private DocsPaVO.trasmissione.InfoTrasmissione CreateInfoTrasmissione(IDataReader reader)
        {
            DocsPaVO.trasmissione.InfoTrasmissione info = new DocsPaVO.trasmissione.InfoTrasmissione();

            info.IdTrasmissione = reader.GetValue(reader.GetOrdinal("system_id")).ToString();
            info.IdRuolo = reader.GetValue(reader.GetOrdinal("ruolo_system_id")).ToString();
            info.Ruolo = reader.GetValue(reader.GetOrdinal("ruolo")).ToString();
            info.IdUtente = reader.GetValue(reader.GetOrdinal("user_system_id")).ToString();
            info.Utente = reader.GetValue(reader.GetOrdinal("user_id")).ToString();
            info.Tipo = reader.GetValue(reader.GetOrdinal("cha_tipo_oggetto")).ToString();
            info.IdDocumento = reader.GetValue(reader.GetOrdinal("id_profile")).ToString();
            info.IdFascicolo = reader.GetValue(reader.GetOrdinal("id_project")).ToString();
            info.DataInvio = reader.GetValue(reader.GetOrdinal("dta_invio")).ToString();
            info.NoteGenerali = reader.GetValue(reader.GetOrdinal("var_note_generali")).ToString();
            info.SalvataConCessione = reader.GetValue(reader.GetOrdinal("cha_salvata_con_cessione")).ToString();
            string utDelegato = reader.GetValue(reader.GetOrdinal("id_people_delegato")).ToString();
            info.UtenteDelegato = "";
            if (!string.IsNullOrEmpty(utDelegato))
            {
                Utenti utente = new Utenti();
                DocsPaVO.utente.Utente delegato = utente.GetUtenteNoFiltroDisabled(utDelegato);
                //Uso il metodo GetUtenteNoFiltroDisabled e non GetUtente perchè nel caso in cui il delegato è disabilitato non ritorna nulla
                //info.UtenteDelegato = utente.GetUtente(utDelegato).descrizione;

                info.UtenteDelegato = (delegato != null) ? delegato.descrizione : string.Empty;
            }
            return info;
        }


        /// <summary>
        /// Creazione oggetto InfoTrasmissione
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private DocsPaVO.trasmissione.InfoTrasmissione CreateInfoTrasmissioneRicevuta(IDataReader reader)
        {
            DocsPaVO.trasmissione.InfoTrasmissione info = this.CreateInfoTrasmissione(reader);

            info.IdRagioneTrasmissione = reader.GetValue(reader.GetOrdinal("id_ragione")).ToString();
            info.RagioneTrasmissione = reader.GetValue(reader.GetOrdinal("ragione")).ToString();
            info.DataScadenza = reader.GetValue(reader.GetOrdinal("dta_scadenza")).ToString();
            return info;
        }

        #endregion

        #region GESTIONE VISIBILITA' GERARCHICA
        /// <summary>
        /// Ricava la lista delle query in base al tipo destinatario per estendere la visibilità gerarchica
        /// </summary>
        /// <param name="currentTrasmissione"></param>
        /// <param name="objTrasm"></param>
        /// <param name="idRegistro"></param>
        /// <param name="idNodoTitolario"></param>
        /// <param name="idObj"></param>
        /// <param name="idPeopleGroup"></param>
        /// <param name="codiceACL"></param>
        /// <param name="tipoDiritto"></param>
        /// <returns></returns>
        public ArrayList GetQueryTrasmEstVisibSup(DocsPaVO.trasmissione.TrasmissioneSingola currentTrasmissione, DocsPaVO.trasmissione.Trasmissione objTrasm, string idRegistro, string idNodoTitolario, string idObj, string idPeopleGroup, string codiceACL, string tipoDiritto, ref ArrayList ruoliSuperiori)
        {
            System.Data.DataSet dataSet = new System.Data.DataSet();
            ArrayList returnList = new ArrayList();
            string checkString = string.Empty;
            ArrayList insertQueries = new ArrayList();
            ArrayList listaInfoSec = new ArrayList();
            ArrayList ruoliSup = new ArrayList();

            if (currentTrasmissione.corrispondenteInterno.GetType() == typeof(DocsPaVO.utente.Ruolo))
            {
                if (ruoliSuperiori != null && ruoliSuperiori.Count == 0)
                {
                    DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                    ruoliSup = gerarchia.getGerarchiaSup((DocsPaVO.utente.Ruolo)currentTrasmissione.corrispondenteInterno, idRegistro, idNodoTitolario, objTrasm.tipoOggetto);
                    ruoliSuperiori = ruoliSup;
                }
                else
                {
                    DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                    ruoliSup = gerarchia.getGerarchiaSup((DocsPaVO.utente.Ruolo)currentTrasmissione.corrispondenteInterno, idRegistro, idNodoTitolario, objTrasm.tipoOggetto);
                    if (ruoliSup != null && ruoliSup.Count > 0)
                    {
                        for (int i = 0; i < ruoliSup.Count; i++)
                        {
                            ruoliSuperiori.Add(ruoliSup[i]);
                        }
                    }
                }
            }

            if (ruoliSup == null)
            {
                logger.Debug("Errore nella gestione delle trasmissioni (Query - GetQueryTrasm)");
                throw new Exception("Errore in : getQueryTrasm");
            }

            if (ruoliSup.Count > 0)
            {
                checkString = "SELECT PERSONORGROUP, ACCESSRIGHTS, CHA_TIPO_DIRITTO FROM SECURITY WHERE THING = " + idObj + " AND PERSONORGROUP IN (";

                for (int l = 0; l < ruoliSup.Count; l++)
                {
                    checkString = checkString + ((DocsPaVO.utente.Ruolo)ruoliSup[l]).idGruppo;
                    if (l < ruoliSup.Count - 1)
                        checkString += ",";
                }

                checkString += ") ORDER BY ACCESSRIGHTS DESC";

                logger.Debug(checkString);
                this.ExecuteQuery(dataSet, "DIRITTI", checkString);

                //ArrayList ruoliConDiritti = new ArrayList();
                Hashtable ruoliConDiritti = new Hashtable();

                foreach (DataRow ruoloRow in dataSet.Tables["DIRITTI"].Rows)
                {
                    bool isContainsK = false;
                    try
                    {
                        isContainsK = ruoliConDiritti.ContainsKey(ruoloRow["PERSONORGROUP"].ToString());
                    }
                    catch
                    {
                        isContainsK = true;
                    }
                    if (!isContainsK)
                    {
                        ruoliConDiritti.Add(ruoloRow["PERSONORGROUP"].ToString(), ruoloRow["ACCESSRIGHTS"].ToString());
                        logger.Debug("Aggiunto " + ruoloRow["PERSONORGROUP"].ToString());
                    }
                    //else
                    //{
                    //    string newACL = dataSet.Tables["ACCESSRIGHTS"].Rows[0].ToString();
                    //    string oldACL = (string)ruoliConDiritti[ruoloRow["PERSONORGROUP"].ToString()];
                    //    if(Convert.ToInt32(newACL) > Convert.ToInt32(oldACL))
                    //    {
                    //        ruoliConDiritti.Remove(ruoloRow["PERSONORGROUP"].ToString());
                    //        ruoliConDiritti.Add(ruoloRow["PERSONORGROUP"].ToString(), newACL);
                    //    }
                    //}
                }

                for (int m = 0; m < ruoliSup.Count; m++)
                {
                    DocsPaVO.utente.Ruolo ruoloCorrente = (DocsPaVO.utente.Ruolo)ruoliSup[m];
                    //if (!ruoliConDiritti.Contains(ruoloCorrente.idGruppo))
                    bool isContainsKey = false;
                    try
                    {
                        isContainsKey = ruoliConDiritti.ContainsKey(ruoloCorrente.idGruppo);
                    }
                    catch
                    {
                        isContainsKey = true;
                    }
                    if (!isContainsKey)
                    {
                        DocsPaVO.trasmissione.infoSecurity infoSec = new DocsPaVO.trasmissione.infoSecurity();

                        infoSec.thing = idObj;
                        infoSec.accessRights = codiceACL;
                        infoSec.chaTipoDiritto = "A";
                        infoSec.personOrGroup = ruoloCorrente.idGruppo;
                        infoSec.idGruppoTrasm = objTrasm.ruolo.idGruppo;
                        infoSec.hideDocPreviousVersions = currentTrasmissione.hideDocumentPreviousVersions;
                        infoSec.tipoQuery = "I";

                        listaInfoSec.Add(infoSec);
                    }
                    else
                    {
                        string oldACL = string.Empty;
                        foreach (DictionaryEntry entry in ruoliConDiritti)
                        {
                            if ((string)entry.Key == ruoloCorrente.idGruppo)
                                oldACL = (string)entry.Value;
                        }
                        if (!string.IsNullOrEmpty(oldACL) && Convert.ToInt32(oldACL) < Convert.ToInt32(codiceACL))
                        {
                            DocsPaVO.trasmissione.infoSecurity infoSec = new DocsPaVO.trasmissione.infoSecurity();

                            infoSec.thing = idObj;
                            infoSec.accessRights = codiceACL;
                            infoSec.chaTipoDiritto = "A";
                            infoSec.personOrGroup = ruoloCorrente.idGruppo;
                            infoSec.idGruppoTrasm = objTrasm.ruolo.idGruppo;
                            infoSec.hideDocPreviousVersions = currentTrasmissione.hideDocumentPreviousVersions;
                            infoSec.tipoQuery = "U";

                            listaInfoSec.Add(infoSec);
                        }
                    }
                }
            }

            return listaInfoSec;
        }

        /// <summary>
        /// Elimina la visibilità dell'oggetto all'utente o al ruolo
        /// </summary>
        /// <param name="idDestinatario">ID utente o ruolo destinatario</param>
        /// <param name="idOggetto">ID oggetto trasmesso</param>
        /// <returns>true o false</returns>
        public bool EliminaVisibilitaRuolo_Utente(string idDestinatario, string idOggetto)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            bool retValue = false;

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("D_SECURITY");
                queryMng.setParam("param1", "THING = " + idOggetto + " AND PERSONORGROUP = " + idDestinatario + " AND CHA_TIPO_DIRITTO NOT IN ('P')");
                string commandText = queryMng.getSQL();
                logger.Debug("EliminaVisibilitaRuolo_Utente QUERY : " + commandText);

                retValue = dbProvider.ExecuteNonQuery(commandText);
            }
            catch
            {
                retValue = false;
            }

            return retValue;
        }

        /// <summary>
        /// Elimina la visibilità degli oggetti in stato: attesa di accettazione
        /// </summary>
        /// <param name="thing">ID oggetto (doc o fasc)</param>
        /// <param name="personorgroup">ID Utente/Gruppo</param>     
        /// <param name="isFascicolo">True se l'oggetto è un fascicolo</param>
        /// <returns>true o false</returns>
        public bool EliminaAttesaAccettazione(string thing, string personorgroup, bool isFascicolo)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            bool retValue = false;

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("D_SECURITY");

                queryMng.setParam("param1", "THING = " + thing + " AND PERSONORGROUP = " + personorgroup + " AND ACCESSRIGHTS <= 45");

                string commandText = queryMng.getSQL();
                logger.Debug("EliminaAttesaAccettazione QUERY : " + commandText);

                retValue = dbProvider.ExecuteNonQuery(commandText);

                if (retValue && isFascicolo)
                    retValue = EliminaAttesaAccettazione_Fascicolo(thing, personorgroup);
            }
            catch
            {
                retValue = false;
            }

            return retValue;
        }

        public bool EliminaAttesaAccettazione_Fascicolo(string thing, string personorgroup)
        {
            bool retValue = true;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            string idFolderC = GetIdFolderC(thing);

            if (!string.IsNullOrEmpty(idFolderC))
            {
                // elimina il tipo fascicolo "C"
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("D_SECURITY");
                queryMng.setParam("param1", "THING = " + idFolderC + " AND PERSONORGROUP = " + personorgroup + " AND ACCESSRIGHTS <= 45");
                string commandText = queryMng.getSQL();
                logger.Debug(commandText);

                retValue = dbProvider.ExecuteNonQuery(commandText);

                if (retValue)
                {
                    // elimina i documenti in folder "C"
                    string condizione = " AND THING IN (SELECT LINK FROM PROJECT_COMPONENTS WHERE PROJECT_ID = " + idFolderC + ")";
                    commandText = "DELETE FROM SECURITY WHERE PERSONORGROUP = " + personorgroup + " AND ACCESSRIGHTS <= 45" + condizione;
                    logger.Debug(commandText);

                    retValue = dbProvider.ExecuteNonQuery(commandText);

                    if (retValue)
                    {
                        // cicla per tutte le sotto cartelle (ri-esegue questo metodo)
                        retValue = EliminaAttesaAccettazione_Fascicolo(idFolderC, personorgroup);
                    }
                }
            }

            return retValue;
        }

        public string GetIdFolderC(string idF)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            string retValue = string.Empty;
            string idFolderC = string.Empty;

            string commandText = "SELECT SYSTEM_ID FROM PROJECT WHERE CHA_TIPO_PROJ = 'C' AND ID_PARENT = " + idF;
            logger.Debug(commandText);

            bool returnDB = dbProvider.ExecuteScalar(out idFolderC, commandText);

            if (returnDB && (!string.IsNullOrEmpty(idFolderC)))
                retValue = idFolderC;

            return retValue;
        }

        #endregion

        /// <summary>
        /// Reperisce tutti i dati di una trasmissione con ID conosciuto 
        /// </summary>
        /// <param name="idTrasmissione">system_id della trasmissione</param>
        /// <returns></returns>
        public DataSet GetDatiTrasmissioneByID(string idTrasmissione)
        {
            DataSet dsDati;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPATrasmissione");
            queryDef.setParam("param1", "SYSTEM_ID = " + idTrasmissione);

            string commandText = queryDef.getSQL();
            commandText = commandText.Replace("SELECT SYSTEM_ID", "SELECT *");
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                dbProvider.ExecuteQuery(out dsDati, commandText);

            return dsDati;
        }

        /// <summary>
        /// Reperisce tutte le trasmissioni singole (con le relative ragioni di trasmissione) di una trasmissione con ID conosciuto 
        /// </summary>
        /// <param name="idTrasmissione">system_id della trasmissione</param>
        /// <returns></returns>
        public DataSet GetTrasmissioniSingoleByIDTrasmissione(string idTrasmissione)
        {
            DataSet dsDati;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("GET_TRASMISSIONISINGOLE");
            queryDef.setParam("param1", idTrasmissione);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                dbProvider.ExecuteQuery(out dsDati, commandText);

            return dsDati;
        }

        /// <summary>
        /// Reperisce tutte le trasmissioni utente di una trasmissione singola con ID conosciuto
        /// </summary>
        /// <param name="idTrasmSingola">system_id della trasmissione singola</param>
        /// <returns></returns>
        public DataSet GetTrasmissioniUtenteByIDTrasmSingola(string idTrasmSingola)
        {
            DataSet dsDati;

            string commandText = "SELECT * FROM DPA_TRASM_UTENTE WHERE ID_TRASM_SINGOLA = " + idTrasmSingola;
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                dbProvider.ExecuteQuery(out dsDati, commandText);

            return dsDati;
        }

        /// <summary>  
        /// Verifica se una trasmissione di tipo UNO è già stata accettata o rifiutata
        /// da un utente del ruolo
        /// </summary>
        /// <param name="idTrasmSingola"></param>
        /// <returns>False se è già stata accettata o rifiutata</returns>
        public bool checkTrasm_UNO_AccettataRifiutata(string idTrasmSingola)
        {
            bool retValue = true;
            string campo = string.Empty;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_TRASM_UNO_TUTTI_ACC_RIF");
            queryMng.setParam("queryWhere", "AND (DTA_ACCETTATA IS NOT NULL OR DTA_RIFIUTATA IS NOT NULL)");
            queryMng.setParam("idTrasmSingola", idTrasmSingola);
            string commandText = queryMng.getSQL();
            logger.Debug(commandText);
            dbProvider.ExecuteScalar(out campo, commandText);
            if (campo != null && Convert.ToByte(campo) > 0)
                retValue = false;

            return retValue;
        }

        /// <summary>
        /// Verifica se una trasmissione di tipo TUTTI è già stata accettata o rifiutata da tutti
        /// </summary>
        /// <param name="idTrasmSingola"></param>
        /// <returns>True se è già stata accettata o rifiutata da tutti</returns>
        public bool checkTrasm_TUTTI_AccettataRifiutata(string idTrasmSingola)
        {
            bool retValue = false;
            string quanteTrasmUtente = string.Empty;
            string quanteDaAccRif = string.Empty;
            string commandText = string.Empty;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            DocsPaUtils.Query queryMng;
            // conta prima quante sono le trasm. utente
            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_TRASM_UNO_TUTTI_ACC_RIF");
            queryMng.setParam("queryWhere", "");
            queryMng.setParam("idTrasmSingola", idTrasmSingola);
            commandText = queryMng.getSQL();
            logger.Debug(commandText);
            dbProvider.ExecuteScalar(out quanteTrasmUtente, commandText);

            // conta quante trasm utente non sono state ancora acc./rif.
            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_TRASM_UNO_TUTTI_ACC_RIF");
            queryMng.setParam("queryWhere", "AND ((DTA_ACCETTATA IS NULL AND DTA_RIFIUTATA IS NOT NULL) OR (DTA_ACCETTATA IS NOT NULL AND DTA_RIFIUTATA IS NULL))");
            queryMng.setParam("idTrasmSingola", idTrasmSingola);
            commandText = queryMng.getSQL();
            logger.Debug(commandText);
            dbProvider.ExecuteScalar(out quanteDaAccRif, commandText);

            if (Convert.ToByte(quanteTrasmUtente) > Convert.ToByte(quanteDaAccRif))
                retValue = true;

            return retValue;
        }

        /// <summary>
        /// Rimozione del documento nella todolist degli utenti del ruolo
        /// </summary>

        /// <returns>True o false</returns>
        public bool RimuoviToDoList(string idGruppo, string idOggetto)
        {
            bool retValue = false;

            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_TRASM_UTENTE_TOGLIE_TODOLIST");
            queryMng.setParam("data_rimozione", DocsPaDbManagement.Functions.Functions.GetDate(true));
            queryMng.setParam("idgruppo", idGruppo);
            queryMng.setParam("idoggetto", idOggetto);
            string commandText = queryMng.getSQL();
            logger.Debug(commandText);
            retValue = dbProvider.ExecuteNonQuery(commandText);

            return retValue;
        }

        public int ContaTrasm_Da_ACC_Per_OggettoRuoloUtente(string tipoOggetto, string IDOggetto, string IDGroup, string IDPeople)
        {
            int retValue = 0;
            string campoOggetto = string.Empty;
            string outValue = string.Empty;

            switch (tipoOggetto)
            {
                case "D":
                    campoOggetto = "ID_PROFILE";
                    break;
                case "F":
                    campoOggetto = "ID_PROJECT";
                    break;
            }

            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONTA_TRASM_DA_ACC");
            queryMng.setParam("campoOggetto", campoOggetto);
            queryMng.setParam("IDOggetto", IDOggetto);
            queryMng.setParam("IDPeople", IDPeople);
            queryMng.setParam("IDGroup", IDGroup);

            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            dbProvider.ExecuteScalar(out outValue, commandText);

            retValue = Convert.ToInt32(outValue);

            return retValue;
        }

        public bool verificaSecurityPerEliminaVisibRuolo_Utente(string IDOggetto, string IDGroup, string IDPeople)
        {
            bool retValue = false;
            string outValue = string.Empty;

            string queryWhere = "THING = " + IDOggetto + " AND PERSONORGROUP IN (" + IDGroup + "," + IDPeople + ") AND ACCESSRIGHTS >= 45 AND CHA_TIPO_DIRITTO IN ('A','F','T')";

            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_ACL");
            queryMng.setParam("param1", queryWhere);

            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            dbProvider.ExecuteScalar(out outValue, commandText);

            retValue = (outValue.Equals("0"));

            return retValue;
        }

        /// <summary>
        /// Update di TIPO DIRITTO nella tabella SECURITY
        /// </summary>
        /// <param name="dirittoToSet">Valore di CHA_TIPO_DIRITTO da impostare</param>
        /// <param name="condizione">Condizione della query. NOTA: ometti clausola WHERE e spazio iniziale!</param>
        /// <returns></returns>
        public bool impostaTipoDirittoInSecurity(string dirittoToSet, string condizione)
        {
            bool retValue = false;
            int rowsAffected = 0;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            string commandText = "UPDATE SECURITY SET CHA_TIPO_DIRITTO = '" + dirittoToSet + "' WHERE " + condizione;
            logger.Debug(commandText);

            retValue = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

            if (retValue)
                logger.Debug("Rows affected: " + rowsAffected.ToString());

            return retValue;
        }

        /// <summary>
        /// Update di TIPO DIRITTO e ACCESSRIGHTS nella tabella SECURITY
        /// </summary>
        /// <param name="dirittoToSet">Valore di CHA_TIPO_DIRITTO da impostare</param>
        /// <param name="accessRightsToSet">Valore di ACCESSRIGHTS da impostare</param>
        /// <param name="condizione">Condizione della query. NOTA: ometti clausola WHERE e spazio iniziale!</param>
        /// <returns></returns>
        public bool impostaDirittoEAggiornaAccessrights(string dirittoToSet, string condizione, string accessRightsToSet)
        {
            bool retValue = false;
            int rowsAffected = 0;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            string commandText = "UPDATE SECURITY SET CHA_TIPO_DIRITTO = '" + dirittoToSet + "', ACCESSRIGHTS = " + accessRightsToSet + " WHERE " + condizione;
            logger.Debug(commandText);

            retValue = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

            if (retValue)
                logger.Debug("Rows affected: " + rowsAffected.ToString());

            return retValue;
        }

        /// <summary>
        /// Update di ACCESSRIGHTS nella tabella SECURITY;
        /// se una trasmissione viene inviata con "Workflow", all'inizio viene
        /// impostato il campo ACCESSRIGHTS della tabella security con un valore minore 
        /// al diritto di sola lettura (significa: attesa di accettazione/rifiuto).
        /// 
        /// Dopo l'accettazione della trasm.ne, il campo ACCESSRIGHTS deve essere aggiornato al valore
        /// corretto rispetto alla ragione di trasmissione con la quale era stata inviata la trasm.ne.
        /// </summary>
        /// <param name="dirittoToSet">Valore di ACCESSRIGHTS da impostare</param>
        /// <param name="condizione">Condizione della query. NOTA: ometti clausola WHERE e spazio iniziale!</param>
        /// <returns></returns>
        public bool aggiornaAccessRights_Waiting(string thing, string accessRightsToSet, string condizione)
        {
            bool retValue = false;
            int rowsAffected = 0;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            //controllo se è in stato finale, devo lasciare security a lettura in ogni caso.
            DocsPaDB.Query_DocsPAWS.DiagrammiStato ds = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
            DocsPaVO.DiagrammaStato.Stato stato = ds.getStatoDoc(thing);
            if (stato != null && stato.STATO_FINALE)
                accessRightsToSet = "45";
            string commandText = "UPDATE SECURITY SET ACCESSRIGHTS = " + accessRightsToSet + " WHERE " + condizione;
            logger.Debug(commandText);

            retValue = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

            if (retValue)
                logger.Debug("Rows affected: " + rowsAffected.ToString());

            return retValue;
        }

        /// <summary>
        /// Update di ACCESSRIGHTS nella tabella SECURITY quando l'oggetto è di tipo FASCICOLO
        /// </summary>
        /// <param name="thing">ID Fascicolo (Tipo "F")</param>
        /// <param name="idGroup">ID Gruppo da ricercare nella query</param>
        /// <param name="idPeople">ID People da ricercare nella query</param>
        /// <param name="accessRightsAttuale">ACCESSRIGHTS attuale da ricercare nella query</param>
        /// <param name="accessRightsToSet">ACCESSRIGHTS da impostare</param>
        /// <returns></returns>
        public bool aggiornaAccessRights_DocsInFascicolo(string thing, string idGroup, string idPeople, string accessRightsAttuale, string accessRightsLettura, string accessRightsToSet)
        {
            bool retValue = false;
            int rowsAffected = 0;
            string idFolderC = string.Empty;

            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            // prende il tipo project "C"
            idFolderC = GetIdFolderC(thing);

            if (!string.IsNullOrEmpty(idFolderC))
            {
                // esegue l'update in SECURITY del tipo project "C"
                string condizione = "THING = " + idFolderC + " AND PERSONORGROUP IN (" + idGroup + "," + idPeople + ") AND ACCESSRIGHTS IN (" + accessRightsAttuale + "," + accessRightsLettura + ")";
                string commandText = "UPDATE SECURITY SET ACCESSRIGHTS = " + accessRightsToSet + " WHERE " + condizione;
                logger.Debug(commandText);

                retValue = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                if (retValue)
                {
                    logger.Debug("Rows affected: " + rowsAffected.ToString());

                    // esegue l'update in SECURITY dei documenti in folder C
                    condizione = " AND THING IN (SELECT LINK FROM PROJECT_COMPONENTS WHERE PROJECT_ID = " + idFolderC + ")";
                    commandText = "UPDATE SECURITY SET ACCESSRIGHTS = " + accessRightsToSet + " WHERE PERSONORGROUP IN (" + idGroup + "," + idPeople + ") AND ACCESSRIGHTS IN (" + accessRightsAttuale + "," + accessRightsLettura + ") " + condizione;
                    logger.Debug(commandText);

                    retValue = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                    if (retValue)
                    {
                        logger.Debug("Rows affected: " + rowsAffected.ToString());

                        // cicla per tutte le sotto cartelle (ri-esegue questo metodo)
                        retValue = this.aggiornaAccessRights_DocsInFascicolo(idFolderC, idGroup, idPeople, accessRightsAttuale, accessRightsLettura, accessRightsToSet);
                    }
                }
            }

            return retValue;
        }

        public int ContaTrasm_UT_inTDL(string tipoOggetto, string IDOggetto, string IDPeople)
        {
            int retValue = 0;
            string campoOggetto = string.Empty;
            string outValue = string.Empty;

            switch (tipoOggetto)
            {
                case "D":
                    campoOggetto = "ID_PROFILE";
                    break;
                case "F":
                    campoOggetto = "ID_PROJECT";
                    break;
            }

            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONTA_TRASM_CON_WF_IN_TDL_PER_UT");
            queryMng.setParam("campoOggetto", campoOggetto);
            queryMng.setParam("IDOggetto", IDOggetto);
            queryMng.setParam("IDPeople", IDPeople);

            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            dbProvider.ExecuteScalar(out outValue, commandText);

            retValue = Convert.ToInt32(outValue);

            return retValue;
        }

        public int ContaTrasm_RUOLO_inTDL(string tipoOggetto, string IDOggetto, string IDGruppo)
        {
            int retValue = 0;
            string campoOggetto = string.Empty;
            string outValue = string.Empty;

            switch (tipoOggetto)
            {
                case "D":
                    campoOggetto = "ID_PROFILE";
                    break;
                case "F":
                    campoOggetto = "ID_PROJECT";
                    break;
            }

            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONTA_TRASM_CON_WF_IN_TDL_PER_RUOLO");
            queryMng.setParam("campoOggetto", campoOggetto);
            queryMng.setParam("IDOggetto", IDOggetto);
            queryMng.setParam("IDGruppo", IDGruppo);

            string commandText = queryMng.getSQL();
            logger.Debug(commandText);

            dbProvider.ExecuteScalar(out outValue, commandText);

            retValue = Convert.ToInt32(outValue);

            return retValue;
        }

        public DataSet checkScadenzeFasc(string param1)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TRASM_SCADENZE_FASC");
                queryMng.setParam("param1", param1);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - checkScadenzeFasc - Trasmissione.cs - QUERY : " + commandText);
                logger.Debug("SQL - checkScadenzeFasc - Trasmissione.cs - QUERY : " + commandText);

                DataSet dsFascInScadenza = new DataSet();
                dbProvider.ExecuteQuery(dsFascInScadenza, commandText);

                return dsFascInScadenza;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataSet checkScadenzeMittFasc(string idProject, string param1)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TRASM_SCADENZE_MITT_FASC");
                queryMng.setParam("idProject", idProject);
                queryMng.setParam("param1", param1);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - checkScadenzeMittFasc - Trasmissione.cs - QUERY : " + commandText);
                logger.Debug("SQL - checkScadenzeMittFasc - Trasmissione.cs - QUERY : " + commandText);

                DataSet dsScadenzeMittFasc = new DataSet();
                dbProvider.ExecuteQuery(dsScadenzeMittFasc, commandText);

                return dsScadenzeMittFasc;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public DataSet checkScadenzeDestFasc(string idProject, string param1)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("TRASM_SCADENZE_DEST_FASC");
                queryMng.setParam("idProject", idProject);
                queryMng.setParam("param1", param1);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - checkScadenzeDestFasc - Trasmissione.cs - QUERY : " + commandText);
                logger.Debug("SQL - checkScadenzeDestFasc - Trasmissione.cs - QUERY : " + commandText);

                DataSet dsScadenzeDestFasc = new DataSet();
                dbProvider.ExecuteQuery(dsScadenzeDestFasc, commandText);

                return dsScadenzeDestFasc;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool isPersonOrGroupOwner(string thing, string idPeople, string idGroup)
        {
            bool retValue = false;

            IDataReader dr = null;
            string accessRights = string.Empty;
            string personGroup = string.Empty;

            bool peopleOwner = false;
            bool groupOwner = false;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SECURITY_GENERIC");
                    q.setParam("param1", "ACCESSRIGHTS, PERSONORGROUP");
                    q.setParam("param2", "WHERE THING = " + thing + " AND PERSONORGROUP IN (" + idPeople + "," + idGroup + ") AND ACCESSRIGHTS IN (0,255)");
                    string queryString = q.getSQL();
                    logger.Debug(queryString);

                    using (dr = dbProvider.ExecuteReader(queryString))
                    {
                        if (dr == null)
                        {
                            return false;
                        }
                        if (dr != null && dr.FieldCount > 1)
                        {
                            while (dr.Read())
                            {
                                accessRights = dr.GetValue(0).ToString();
                                personGroup = dr.GetValue(1).ToString();

                                //proprietà di ruolo
                                if (accessRights.Equals("0") && personGroup.Equals(idPeople))
                                    peopleOwner = true;

                                //proprietà di ruolo
                                if (accessRights.Equals("255") && personGroup.Equals(idGroup))
                                    groupOwner = true;
                            }
                        }
                    }
                }
                if (dr != null && (!dr.IsClosed))
                    dr.Close();
            }
            catch
            {
                return false;
            }

            retValue = (peopleOwner && groupOwner);

            return retValue;
        }

        public bool isGroupNoOwner(string thing, string idGroup)
        {
            bool retValue = false;

            IDataReader dr = null;
            //string accessRights = string.Empty;            

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SECURITY_GENERIC");
                    q.setParam("param1", "ACCESSRIGHTS");
                    q.setParam("param2", "WHERE THING = " + thing + " AND PERSONORGROUP = " + idGroup + " AND ACCESSRIGHTS NOT IN (0,255)");
                    string queryString = q.getSQL();
                    logger.Debug(queryString);

                    using (dr = dbProvider.ExecuteReader(queryString))
                    {
                        if (dr == null)
                        {
                            return false;
                        }
                        if (dr != null && dr.FieldCount > 1)
                        {
                            retValue = true;
                        }
                    }
                }
                if (dr != null && (!dr.IsClosed))
                    dr.Close();
            }
            catch
            {
                return false;
            }

            return retValue;
        }

        public string GetIDFolderRoot(string idC)
        {
            string retValue = string.Empty;
            IDataReader dr = null;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project10");
                    q.setParam("param1", idC);
                    string queryString = q.getSQL();
                    logger.Debug(queryString);

                    using (dr = dbProvider.ExecuteReader(queryString))
                    {
                        if (dr == null)
                        {
                            return retValue;
                        }
                        if (dr != null && dr.FieldCount > 0)
                        {
                            while (dr.Read())
                            {
                                retValue = dr.GetValue(0).ToString();
                            }
                        }
                    }
                }
            }
            catch
            {
                return retValue;
            }

            return retValue;
        }

        /// <summary>
        /// Se nelle ragioni delle trasmissioni c'è almeno una ragione con il valore 1 nel campo eredita,
        /// tale metodo restituisce true altrimenti false.
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="idModello"></param>
        /// <returns></returns>
        public bool GetEstensioneVisibilita(string idAmm, string idModello)
        {
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
            string query = string.Empty;
            string Eredita = string.Empty;
            string err = string.Empty;

            if (dbType.ToUpper() == "SQL")
            {
                query = "SELECT " + DocsPaDbManagement.Functions.Functions.GetDbUserSession() + ".getVisibilita(" + idAmm + ", " + idModello + ") as VISIBILITA";
            }
            else
            {
                query = "SELECT " + DocsPaDbManagement.Functions.Functions.GetDbUserSession() + "getVisibilita(" + idAmm + ", " + idModello + ") as VISIBILITA FROM DUAL";
            }
            logger.Debug(query);
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(query))
                    {
                        while (reader.Read())
                        {
                            Eredita = reader.GetValue(reader.GetOrdinal("VISIBILITA")).ToString();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                err = exc.Message;
                logger.Debug(err);
            }
            if (Eredita != "1")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Reperimento dell'ultima data in cui è stato trasmesso dall'utente un documento o un fascicolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="tipoOggettoTrasmesso"></param>
        /// <param name="idOggettoTrasmesso"></param>
        /// <param name="idCorrGlobaliDestinatario">
        /// Id del destinatario in tabella DPA_CORR_GLOBALI
        /// </param>
        /// <returns></returns>
        public string GetDataUltimaTrasmissioneEffettuata(DocsPaVO.utente.InfoUtente infoUtente, string tipoOggettoTrasmesso, string idOggettoTrasmesso, string idCorrGlobaliDestinatario)
        {
            using (DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_DATA_ULTIMA_TRASMISSIONE_EFFETTUATA");

                queryDef.setParam("tipoOggetto", tipoOggettoTrasmesso);
                if (tipoOggettoTrasmesso == "D")
                    queryDef.setParam("filtroIdOggetto", string.Format("id_profile = {0}", idOggettoTrasmesso));
                else if (tipoOggettoTrasmesso == "F")
                    queryDef.setParam("filtroIdOggetto", string.Format("id_project = {0}", idOggettoTrasmesso));
                queryDef.setParam("idDestinatario", idCorrGlobaliDestinatario);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                string field;
                dbProvider.ExecuteScalar(out field, commandText);
                return field;
            }
        }

        /// <summary>
        /// Reperimento delle informazioni di stato relative ad una trasmissione utente
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idTrasmissioneUtente"></param>
        /// <returns></returns>
        public DocsPaVO.trasmissione.StatoTrasmissioneUtente getStatoTrasmissioneUtente(DocsPaVO.utente.InfoUtente infoUtente, string idTrasmissioneUtente)
        {
            try
            {
                DocsPaVO.trasmissione.StatoTrasmissioneUtente stato = new DocsPaVO.trasmissione.StatoTrasmissioneUtente();

                using (DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_STATO_TRASMISSIONE_UTENTE");
                    queryDef.setParam("idTrasmissioneSingola", idTrasmissioneUtente);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                        {
                            stato.Vista = (reader.GetValue(reader.GetOrdinal("VISTA")).ToString() == "1");
                            stato.Accettata = (reader.GetValue(reader.GetOrdinal("ACCETTATA")).ToString() == "1");
                            stato.Rifiutata = (reader.GetValue(reader.GetOrdinal("RIFIUTATA")).ToString() == "1");
                            stato.InTodoList = (reader.GetValue(reader.GetOrdinal("IN_TODOLIST")).ToString() == "1");
                        }
                    }
                }

                return stato;
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nel reperimento delle informazioni di relative alla trasmissione singola con id '{0}'", idTrasmissioneUtente);
                logger.Debug(errorMessage, ex);

                throw new ApplicationException(errorMessage, ex);
            }

        }

        /// <summary>
        /// Dato l'id di una trasmissione singola e l'utente, restituisce una trasmissione utente se presente, array vuoto altrimenti (Fabio)
        /// </summary>
        public DocsPaVO.trasmissione.TrasmissioneUtente[] getTrasmissioneUtenteInRuolo(DocsPaVO.utente.InfoUtente infoUtente, string trasmissione, DocsPaVO.utente.Utente utente)
        {

            List<DocsPaVO.trasmissione.TrasmissioneUtente> list = new List<DocsPaVO.trasmissione.TrasmissioneUtente>();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {

                string idUtente = infoUtente.idPeople;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_TRASM_UTENTE_DA_SINGOLA");
                q.setParam("idTrasmSingola", trasmissione);
                q.setParam("idPeople", idUtente);

                string commandText = q.getSQL();
                logger.Debug(commandText);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {

                    while (reader.Read())
                    {
                        DocsPaVO.trasmissione.TrasmissioneUtente item = new DocsPaVO.trasmissione.TrasmissioneUtente();

                        item.systemId = reader["SYSTEM_ID"].ToString();
                        //MANCA ID TRASM SINGOLA NELLA CLASSE TRASMISSIONE UTENTE ("ID_TRASM_SINGOLA") 
                        item.utente = utente;
                        item.dataVista = reader["DTA_VISTA"].ToString();
                        item.dataAccettata = reader["DTA_ACCETTATA"].ToString();
                        item.dataRifiutata = reader["DTA_RIFIUTATA"].ToString();
                        item.dataRisposta = reader["DTA_RISPOSTA"].ToString();
                        //MANCA CHA_VISTA NELLA CLASSE TRASMISSIONE UTENTE ("CHA_VISTA") 
                        //MANCA CHA_ACCETTATA NELLA CLASSE TRASMISSIONE UTENTE ("CHA_ACCETTATA")
                        //MANCA CHA_RIFIUTATA NELLA CLASSE TRASMISSIONE UTENTE ("CHA_RIFIUATATA")
                        item.noteAccettazione = reader["VAR_NOTE_ACC"].ToString();
                        item.noteRifiuto = reader["VAR_NOTE_RIF"].ToString();
                        item.valida = reader["CHA_VALIDA"].ToString();
                        item.idTrasmRispSing = reader["ID_TRASM_RISP_SING"].ToString();
                        //MANCA CHA_IN_TODOLIST NELLA CLASSE TRASMISSIONE UTENTE ("CHA_IN_TODOLIST")
                        item.dataRimossaTDL = reader["DTA_RIMOZIONE_TODOLIST"].ToString();
                        item.idPeopleDelegato = reader["ID_PEOPLE_DELEGATO"].ToString();
                        item.cha_accettata_delegato = reader["CHA_ACCETTATA_DELEGATO"].ToString();
                        item.cha_vista_delegato = reader["CHA_VISTA_DELEGATO"].ToString();
                        item.cha_rifiutata_delegato = reader["CHA_RIFIUTATA_DELEGATO"].ToString();

                        list.Add(item);
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Dato l'id di una trasmissione singola restituisce tutte le trasmissioni utente)
        /// </summary>
        public DocsPaVO.trasmissione.TrasmissioneUtente[] getTrasmissioniUtenteDaSingola(string trasmissione)
        {

            List<DocsPaVO.trasmissione.TrasmissioneUtente> list = new List<DocsPaVO.trasmissione.TrasmissioneUtente>();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_TRASM_TUTTE_UTENTE_DA_SINGOLA");
                q.setParam("idTrasmSingola", trasmissione);

                string commandText = q.getSQL();
                logger.Debug(commandText);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {

                    while (reader.Read())
                    {
                        DocsPaVO.trasmissione.TrasmissioneUtente item = new DocsPaVO.trasmissione.TrasmissioneUtente();

                        string idPeople = reader.GetInt32(reader.GetOrdinal("ID_PEOPLE")).ToString();

                        DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();

                        DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();

                        utente = ut.getUtenteById(idPeople);

                        item.systemId = reader["SYSTEM_ID"].ToString();
                        //MANCA ID TRASM SINGOLA NELLA CLASSE TRASMISSIONE UTENTE ("ID_TRASM_SINGOLA") 
                        item.utente = utente;
                        item.dataVista = reader["DTA_VISTA"].ToString();
                        item.dataAccettata = reader["DTA_ACCETTATA"].ToString();
                        item.dataRifiutata = reader["DTA_RIFIUTATA"].ToString();
                        item.dataRisposta = reader["DTA_RISPOSTA"].ToString();
                        //MANCA CHA_VISTA NELLA CLASSE TRASMISSIONE UTENTE ("CHA_VISTA") 
                        //MANCA CHA_ACCETTATA NELLA CLASSE TRASMISSIONE UTENTE ("CHA_ACCETTATA")
                        //MANCA CHA_RIFIUTATA NELLA CLASSE TRASMISSIONE UTENTE ("CHA_RIFIUATATA")
                        item.noteAccettazione = reader["VAR_NOTE_ACC"].ToString();
                        item.noteRifiuto = reader["VAR_NOTE_RIF"].ToString();
                        item.valida = reader["CHA_VALIDA"].ToString();
                        item.idTrasmRispSing = reader["ID_TRASM_RISP_SING"].ToString();
                        //MANCA CHA_IN_TODOLIST NELLA CLASSE TRASMISSIONE UTENTE ("CHA_IN_TODOLIST")
                        item.dataRimossaTDL = reader["DTA_RIMOZIONE_TODOLIST"].ToString();
                        item.idPeopleDelegato = reader["ID_PEOPLE_DELEGATO"].ToString();
                        item.cha_accettata_delegato = reader["CHA_ACCETTATA_DELEGATO"].ToString();
                        item.cha_vista_delegato = reader["CHA_VISTA_DELEGATO"].ToString();
                        item.cha_rifiutata_delegato = reader["CHA_RIFIUTATA_DELEGATO"].ToString();

                        list.Add(item);
                    }
                }
            }

            return list.ToArray();
        }

        private string tipoContatoreTemplates(string idTemplate)
        {
            Model model = new Model();
            DocsPaVO.ProfilazioneDinamica.Templates template = model.getTemplateById(idTemplate);

            if (template != null && template.ELENCO_OGGETTI != null)
            {

                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                {
                    if (oggettoCustom.DA_VISUALIZZARE_RICERCA.Equals("1"))
                        return oggettoCustom.TIPO_CONTATORE;
                }
            }
            return string.Empty;
        }

        public System.Data.DataSet getTrasmissioneUtenteById(string idTrasmissioneUtente)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                string commandText = "SELECT * FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID = " + idTrasmissioneUtente;
                System.Diagnostics.Debug.WriteLine("SQL - getTrasmissioneUtenteById - Trasmissione.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTrasmissioneUtenteById - Trasmissione.cs - QUERY : " + commandText);

                DataSet trasmissioneUtente = new DataSet();
                dbProvider.ExecuteQuery(trasmissioneUtente, commandText);
                return trasmissioneUtente;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public System.Data.DataSet getTrasmissioneSingolaById(string idTrasmissioneSingola)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                string commandText = "SELECT * FROM DPA_TRASM_SINGOLA WHERE SYSTEM_ID = " + idTrasmissioneSingola;
                System.Diagnostics.Debug.WriteLine("SQL - getTrasmissioneSingolaById - Trasmissione.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTrasmissioneSingolaById - Trasmissione.cs - QUERY : " + commandText);

                DataSet trasmissioneSingola = new DataSet();
                dbProvider.ExecuteQuery(trasmissioneSingola, commandText);
                return trasmissioneSingola;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Reperisce tutti i dati di una trasmissione con ID conosciuto 
        /// </summary>
        /// <param name="idTrasmissione">system_id della trasmissione</param>
        /// <returns></returns>
        public DataSet GetDatiTrasmissioneByIDLite(string idTrasmissione, string idTrasmissioneUtente)
        {
            DataSet dsDati;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_TRASMISSIONE_SINGOLA_LITE");
            queryDef.setParam("param1", idTrasmissione);
            queryDef.setParam("param2", idTrasmissioneUtente);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                dbProvider.ExecuteQuery(out dsDati, commandText);

            return dsDati;
        }

        /// <summary>
        /// Dato l'id di una trasmissione singola restituisce tutti i destinatari delle trasmissioni utente)
        /// </summary>
        public string getDestinatariTrasmissioniUtenteDaSingola(string trasmissione)
        {
            string destinatari = string.Empty;
            List<DocsPaVO.trasmissione.TrasmissioneUtente> list = new List<DocsPaVO.trasmissione.TrasmissioneUtente>();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_TRASM_TUTTE_DEST_UTENTE_DA_SINGOLA");
                q.setParam("idTrasmSingola", trasmissione);

                string commandText = q.getSQL();
                logger.Debug(commandText);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        destinatari += reader["FULL_NAME"].ToString() + ", ";
                    }
                }

                if (destinatari.EndsWith(", "))
                    destinatari = destinatari.Remove((destinatari.Length - 2), 2);

            }

            return destinatari;
        }

        /// <summary>
        /// Dato l'id di una trasmissione singola restituisce il tipo di destinatario (Ruolo o Utente)
        /// </summary>
        public string getTipoTrasmSing(string idTrasmSingola)
        {
            string tipo_destinatario = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_TIPO_DEST_FROM_TRASM_SING");
                q.setParam("id_trasm", idTrasmSingola);

                string commandText = q.getSQL();
                logger.Debug(commandText);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        tipo_destinatario += reader["CHA_TIPO_DEST"].ToString();
                    }
                }

            }

            return tipo_destinatario;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryWhere"></param>
        /// <param name="objOggettoTrasmesso"></param>
        public DocsPaUtils.Query QueryTrasmExport(ref string queryWhere, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASMISSIONE__TRASM_SINGOLA__TRASM_UTENTE_EXPORT");

            string UserDB = string.Empty;

            if (dbType.ToUpper() == "SQL")
            {
                UserDB = getUserDB();
                q.setParam("dbuser", UserDB);
            }

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO", false));
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("B.DTA_SCADENZA", false));
            q.setParam("param3", DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_VISTA", false));
            q.setParam("param4", DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_ACCETTATA", false));
            q.setParam("param5", DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_RIFIUTATA", false));

            if (objOggettoTrasmesso == null)
            {
                return q;
            }

            string whereStr = null;
            bool doc = false;

            // condizione sui documenti
            if (objOggettoTrasmesso.infoDocumento != null)
            {
                whereStr = "A.ID_PROFILE=" + objOggettoTrasmesso.infoDocumento.idProfile;
                doc = true;
            }

            //condizione sui fascicoli
            if (objOggettoTrasmesso.infoFascicolo != null)
            {
                if (doc) whereStr += " OR ";
                whereStr += "A.ID_PROJECT=" + objOggettoTrasmesso.infoFascicolo.idFascicolo;
            }
            if (whereStr != null)
            {
                queryWhere += " AND ";
                if (doc)
                {
                    queryWhere += "(";
                }
                queryWhere += whereStr;
                if (doc)
                {
                    queryWhere += ") ";
                }
            }

            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryWhere"></param>
        /// <param name="objOggettoTrasmesso"></param>
        public DocsPaUtils.Query QueryTrasmByidTrasmSing(ref string queryWhere, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, string idTrasmSing)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASMISSIONE__TRASM_SINGOLA__TRASM_UTENTE");

            string UserDB = string.Empty;

            if (dbType.ToUpper() == "SQL")
            {
                UserDB = getUserDB();
                q.setParam("dbuser", UserDB);
            }

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO", false));
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("B.DTA_SCADENZA", false));
            q.setParam("param3", DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_VISTA", false));
            q.setParam("param4", DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_ACCETTATA", false));
            q.setParam("param5", DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_RIFIUTATA", false));

            if (objOggettoTrasmesso == null)
            {
                return q;
            }

            string whereStr = null;
            bool doc = false;

            // condizione sui documenti
            if (objOggettoTrasmesso.infoDocumento != null)
            {
                whereStr = "A.ID_PROFILE=" + objOggettoTrasmesso.infoDocumento.idProfile;
                doc = true;
            }

            //condizione sui fascicoli
            if (objOggettoTrasmesso.infoFascicolo != null)
            {
                if (doc) whereStr += " OR ";
                whereStr += "A.ID_PROJECT=" + objOggettoTrasmesso.infoFascicolo.idFascicolo;
            }
            
            if(!string.IsNullOrEmpty(whereStr))
                whereStr += " AND ";

            whereStr += "A.SYSTEM_ID = (select distinct id_trasmissione from dpa_trasm_singola where system_id = " + idTrasmSing + ")";

            if (whereStr != null)
            {
                queryWhere += " AND ";
                if (doc)
                {
                    queryWhere += "(";
                }
                queryWhere += whereStr;
                if (doc)
                {
                    queryWhere += ") ";
                }
            }

            return q;
        }

      
        /// <summary>
        /// Metodo utilizzato per verificare se la to do list di un ruolo contiene trasmissioni rivolte a ruolo
        /// pendenti
        /// </summary>
        /// <param name="roleIdCorrGlob">Id corr globali del ruolo da verificare</param>
        /// <returns>True se ci sono trasmissioni a ruolo pendenti</returns>
        public bool CheckIfUserHaveRoleTransmission(String roleIdCorrGlob)
        {
            String retVal = String.Empty;
            using (DBProvider dbProvider = new DBProvider())
            {
                // Recupero della query da eseguire
                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("CHECK_IF_ROLE_HAS_TRANSMISSIONS");
                query.setParam("roleIdCorrGlobali", roleIdCorrGlob);

                // Esecuzione query e impostazione del risultato da restituire
                dbProvider.ExecuteScalar(out retVal, query.getSQL());
            }

            return Convert.ToInt32(retVal) > 0;

        }

        public void getQueryTrasmRicPagingLite(ref bool repeatQuery,
                                            out DataSet dataSet,
                                            DocsPaVO.utente.Utente utente,
                                            DocsPaVO.utente.Ruolo ruolo,
                                            DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                            int pageNumber,
                                            bool excel,
                                            int pageSize,
                                            out int totalPageNumber,
                                            out int recordCount)
        {
            dataSet = null;
            totalPageNumber = 0;
            recordCount = 0;

            string queryWhere = "";
            string queryFrom = "";
            string queryColumn = "";
            string otherField = string.Empty;
            string otherTable = string.Empty;
            string otherFilter = string.Empty;

            getQueryTrasmissioni(objOggettoTrasmesso, ref queryWhere, ref queryFrom, ref queryColumn);

            queryWhere +=
                " AND A.DTA_INVIO IS NOT NULL "; // +


            if (utente != null && ruolo != null)
                getCondizioniVisibilitaRicevute(ref queryWhere, objOggettoTrasmesso, utente, ruolo, objListaFiltri);

            repeatQuery = getCondFiltri(ref queryWhere, ref queryFrom, objListaFiltri, utente, ruolo);

            string[] idTrasmArray;
            idTrasmArray = this.GetSystemIDTrasmRicevuteLite(queryWhere, queryFrom, string.Empty, pageNumber, excel, pageSize, out totalPageNumber, out recordCount, objListaFiltri, utente.idAmministrazione);
            if (!excel)
            {
                if (idTrasmArray.Length > 0)
                {
                    string trasmFilter = "";

                    foreach (string idTrasm in idTrasmArray)
                    {
                        if (trasmFilter != "")
                            trasmFilter += ", ";
                        trasmFilter += "" + idTrasm + "";
                    }

                    queryWhere += " AND A.SYSTEM_ID IN (" + trasmFilter + ")";
                }
            }

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASMISSIONE__TRASM_SINGOLA__TRASM_UTENTE4");

            string UserDB = string.Empty;

            if (dbType.ToUpper() == "SQL")
            {
                UserDB = getUserDB();
                q.setParam("dbuser", UserDB);
            }

            //è un documento
            FiltroRicerca documentOrFolder = objListaFiltri.Where(e => e.argomento == "TIPO_OGGETTO").FirstOrDefault();

            if (documentOrFolder != null)
            {
                if (documentOrFolder.valore == "D")
                {
                    if (dbType.ToUpper() == "SQL")
                    {
                        UserDB = getUserDB();
                        otherField = " ," + UserDB + ".corrcatbytipo(pr.system_id,pr.cha_tipo_proto,'M') as mittenti_proto, " + UserDB + ".corrcatbytipo(pr.system_id,pr.cha_tipo_proto,'D') as destinatari_proto, pr.VAR_PROF_OGGETTO as oggetto_proto, CONVERT(datetime,pr.dta_proto,109) as data_proto, pr.NUM_PROTO as num_proto, pr.DOCNUMBER, pr.VAR_SEGNATURA, pr.ID_REGISTRO, pr.cha_tipo_proto ";
                    }
                    else
                    {
                        otherField = " ,corrcatbytipo(pr.system_id,pr.cha_tipo_proto,'M') as mittenti_proto, corrcatbytipo(pr.system_id,pr.cha_tipo_proto,'D') as destinatari_proto, pr.VAR_PROF_OGGETTO as oggetto_proto, TO_CHAR (pr.dta_proto, 'dd/mm/yyyy') as data_proto, pr.NUM_PROTO as num_proto, pr.DOCNUMBER, pr.VAR_SEGNATURA, pr.ID_REGISTRO, pr.cha_tipo_proto ";
                    }
                    otherTable = " ,profile pr ";
                    otherFilter = " AND pr.system_id = a.id_profile ";
                }
                else
                {
                    if (dbType.ToUpper() == "SQL")
                    {
                        otherField = " ,pj.var_codice, pj.description,CONVERT(datetime,pj.dta_apertura,109) AS dta_apertura, pj.id_registro";
                    }
                    else
                    {
                        otherField = " ,pj.var_codice, pj.description,TO_CHAR (pj.dta_apertura, 'dd/mm/yyyy') AS dta_apertura, pj.id_registro";
                    }

                    otherTable = " ,project pj ";
                    otherFilter = " AND pj.system_id = a.ID_PROJECT ";
                }
            }

            q.setParam("param1", queryColumn);
            q.setParam("param2", queryFrom);
            q.setParam("param3", queryWhere);

            q.setParam("otherField", otherField);
            q.setParam("otherTable", otherTable);
            q.setParam("otherFilter", otherFilter);

            #region Ordinamento

            // Recupero dei filtri di ricerca relarivi all'ordinamento
            FiltroRicerca oracleField = objListaFiltri.Where(e => e.argomento == "ORACLE_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca sqlField = objListaFiltri.Where(e => e.argomento == "SQL_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca profilationField = objListaFiltri.Where(e => e.argomento == "PROFILATION_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca orderDirection = objListaFiltri.Where(e => e.argomento == "ORDER_DIRECTION").FirstOrDefault();

            // Se non è valorizzata la direzione di ordinamento, viene creato un filtro di tipo ORDER_DIRECTION
            if (orderDirection == null)
            {
                orderDirection = new FiltroRicerca()
                {
                    argomento = "ORDER_DIRECTION",
                    valore = "DESC"

                };

            }

            // Function da utilizzare per estrarre i valori del campo profilato da utilizzare per l'ordinamento
            String extractFieldValue = String.Empty, orderCondition = String.Empty;

            if (this.dbType == "SQL")
            {
                // DB SQL Server
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", Convert(int, @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0})) AS CUSTOM_FIELD", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0}) AS CUSTOM_FIELD", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("CUSTOM_FIELD {0}", orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (sqlField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Format(", {0} AS var_desc_ragione", sqlField.valore);
                        orderCondition = String.Format("var_desc_ragione {0}", orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("Convert(datetime, A.DTA_INVIO) {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }

            }
            else
            {
                // DB ORACLE
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", to_number(getValCampoProfDoc(A.DOCNUMBER, {0}))", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", getValCampoProfDoc(A.DOCNUMBER, {0})", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("getValCampoProfDoc(A.DOCNUMBER, {0}) {1}", profilationField.valore, orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (oracleField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("{0} {1}", oracleField.valore, orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("TO_DATE (a.dta_invio, 'dd/MM/yyyy') {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }
            }

            q.setParam("orderCondition", orderCondition);

            #endregion

            string queryString = q.getSQL();

            if (dbType == "SQL" && !string.IsNullOrEmpty(chaiTableDef))
            {
                queryString = chaiTableDef + queryString;
                chaiTableDef = string.Empty;
            }

            logger.Debug(queryString);

            this.ExecuteQuery(out dataSet, "TRASMISSIONI", queryString);
        }


        public void getQueryTrasmRicPagingLiteWithoutTrasmUtente(ref bool repeatQuery,
                                            out DataSet dataSet,
                                            DocsPaVO.utente.Utente utente,
                                            DocsPaVO.utente.Ruolo ruolo,
                                            DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                            int pageNumber,
                                            bool excel,
                                            int pageSize,
                                            out int totalPageNumber,
                                            out int recordCount)
        {
            dataSet = null;
            totalPageNumber = 0;
            recordCount = 0;

            string queryWhere = "";
            string queryFrom = "";
            string queryColumn = "";
            string otherField = string.Empty;
            string otherTable = string.Empty;
            string otherFilter = string.Empty;

            getQueryTrasmissioniWithoutTrasmUtente(objOggettoTrasmesso, ref queryWhere, ref queryFrom, ref queryColumn);

            queryWhere +=
                " AND A.DTA_INVIO IS NOT NULL "; // +


            if (utente != null && ruolo != null)
                getCondizioniVisibilitaRicevute(ref queryWhere, objOggettoTrasmesso, utente, ruolo, objListaFiltri);

            repeatQuery = getCondFiltri(ref queryWhere, ref queryFrom, objListaFiltri, utente, ruolo);

            string[] idTrasmArray;
            idTrasmArray = this.GetSystemIDTrasmRicevuteLite(queryWhere, queryFrom, string.Empty, pageNumber, excel, pageSize, out totalPageNumber, out recordCount, objListaFiltri, utente.idAmministrazione);
            if (!excel)
            {
                if (idTrasmArray.Length > 0)
                {
                    string trasmFilter = "";

                    foreach (string idTrasm in idTrasmArray)
                    {
                        if (trasmFilter != "")
                            trasmFilter += ", ";
                        trasmFilter += "" + idTrasm + "";
                    }

                    queryWhere += " AND A.SYSTEM_ID IN (" + trasmFilter + ")";
                }
            }

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_TRASMISSIONE__TRASM_SINGOLA__TRASM_UTENTE5");

            string UserDB = string.Empty;

            if (dbType.ToUpper() == "SQL")
            {
                UserDB = getUserDB();
                q.setParam("dbuser", UserDB);
            }

            //è un documento
            FiltroRicerca documentOrFolder = objListaFiltri.Where(e => e.argomento == "TIPO_OGGETTO").FirstOrDefault();

            if (documentOrFolder != null)
            {
                if (documentOrFolder.valore == "D")
                {
                    if (dbType.ToUpper() == "SQL")
                    {
                        UserDB = getUserDB();
                        otherField = " ," + UserDB + ".corrcatbytipo(pr.system_id,pr.cha_tipo_proto,'M') as mittenti_proto, " + UserDB + ".corrcatbytipo(pr.system_id,pr.cha_tipo_proto,'D') as destinatari_proto, pr.VAR_PROF_OGGETTO as oggetto_proto, CONVERT(datetime,pr.dta_proto,109) as data_proto, pr.NUM_PROTO as num_proto, pr.DOCNUMBER, pr.VAR_SEGNATURA, pr.ID_REGISTRO, pr.cha_tipo_proto ";
                        queryColumn += ", " + UserDB + ".GETSEGNATURAREPERTORIO(pr.system_id," + utente.idAmministrazione + ") as COUNTER_REPERTORY";
                    }
                    else
                    {
                        otherField = " ,corrcatbytipo(pr.system_id,pr.cha_tipo_proto,'M') as mittenti_proto, corrcatbytipo(pr.system_id,pr.cha_tipo_proto,'D') as destinatari_proto, pr.VAR_PROF_OGGETTO as oggetto_proto, TO_CHAR (pr.dta_proto, 'dd/mm/yyyy') as data_proto, pr.NUM_PROTO as num_proto, pr.DOCNUMBER, pr.VAR_SEGNATURA, pr.ID_REGISTRO, pr.cha_tipo_proto ";
                        queryColumn += ", GETSEGNATURAREPERTORIO(pr.system_id," + utente.idAmministrazione + ") as COUNTER_REPERTORY";
                    }
                    otherTable = " ,profile pr ";
                    otherFilter = " AND pr.system_id = a.id_profile ";
                }
                else
                {
                    if (dbType.ToUpper() == "SQL")
                    {
                        otherField = " ,pj.var_codice, pj.description,CONVERT(datetime,pj.dta_apertura,109) AS dta_apertura, pj.id_registro";
                    }
                    else
                    {
                        otherField = " ,pj.var_codice, pj.description,TO_CHAR (pj.dta_apertura, 'dd/mm/yyyy') AS dta_apertura, pj.id_registro";
                    }

                    otherTable = " ,project pj ";
                    otherFilter = " AND pj.system_id = a.ID_PROJECT ";
                }
            }

            q.setParam("param1", queryColumn);
            q.setParam("param2", queryFrom);
            q.setParam("param3", queryWhere);

            q.setParam("otherField", otherField);
            q.setParam("otherTable", otherTable);
            q.setParam("otherFilter", otherFilter);

            #region Ordinamento

            // Recupero dei filtri di ricerca relarivi all'ordinamento
            FiltroRicerca oracleField = objListaFiltri.Where(e => e.argomento == "ORACLE_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca sqlField = objListaFiltri.Where(e => e.argomento == "SQL_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca profilationField = objListaFiltri.Where(e => e.argomento == "PROFILATION_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca orderDirection = objListaFiltri.Where(e => e.argomento == "ORDER_DIRECTION").FirstOrDefault();

            // Se non è valorizzata la direzione di ordinamento, viene creato un filtro di tipo ORDER_DIRECTION
            if (orderDirection == null)
            {
                orderDirection = new FiltroRicerca()
                {
                    argomento = "ORDER_DIRECTION",
                    valore = "DESC"

                };

            }

            // Function da utilizzare per estrarre i valori del campo profilato da utilizzare per l'ordinamento
            String extractFieldValue = String.Empty, orderCondition = String.Empty;

            if (this.dbType == "SQL")
            {
                // DB SQL Server
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", Convert(int, @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0})) AS CUSTOM_FIELD", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0}) AS CUSTOM_FIELD", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("CUSTOM_FIELD {0}", orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (sqlField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Format(", {0} AS var_desc_ragione", sqlField.valore);
                        orderCondition = String.Format("var_desc_ragione {0}", orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("Convert(datetime, A.DTA_INVIO) {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }

            }
            else
            {
                // DB ORACLE
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", to_number(getValCampoProfDoc(A.DOCNUMBER, {0}))", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", getValCampoProfDoc(A.DOCNUMBER, {0})", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("getValCampoProfDoc(A.DOCNUMBER, {0}) {1}", profilationField.valore, orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (oracleField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("{0} {1}", oracleField.valore, orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("TO_DATE (a.dta_invio, 'dd/MM/yyyy') {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }
            }

            q.setParam("orderCondition", orderCondition);

            #endregion

            string queryString = q.getSQL();

            if (dbType == "SQL" && !string.IsNullOrEmpty(chaiTableDef))
            {
                queryString = chaiTableDef + queryString;
                chaiTableDef = string.Empty;
            }

            logger.Debug(queryString);

            this.ExecuteQuery(out dataSet, "TRASMISSIONI", queryString);
        }


        private string[] GetSystemIDTrasmRicevuteLite(string queryWhere,
                                                    string queryJoin,
                                                    string orderCriteria,
                                                    int requestedPageNumber,
                                                    bool excel,
                                                    int pageSize,
                                                    out int totalPageNumber,
                                                    out int totalRecordCount,
                                                    FiltroRicerca[] objListaFiltri,
                                                    string idAmministrazione)
        {
            List<string> list = new List<string>();

            totalPageNumber = 0;
            totalRecordCount = 0;

            int startRecord = (requestedPageNumber * pageSize) - pageSize;

            string fieldDefinition = "A.SYSTEM_ID AS ID_TRASM";
            string tables = "DPA_TRASMISSIONE A, DPA_TRASM_SINGOLA B, DPA_TRASM_UTENTE C " + queryJoin;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TRASM_SyIDTrRic_LITE");
            q.setParam("param2", tables);
            q.setParam("param3", queryWhere);


            #region Ordinamento

            // Recupero dei filtri di ricerca relarivi all'ordinamento
            FiltroRicerca oracleField = objListaFiltri.Where(e => e.argomento == "ORACLE_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca sqlField = objListaFiltri.Where(e => e.argomento == "SQL_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca profilationField = objListaFiltri.Where(e => e.argomento == "PROFILATION_FIELD_FOR_ORDER").FirstOrDefault();
            FiltroRicerca orderDirection = objListaFiltri.Where(e => e.argomento == "ORDER_DIRECTION").FirstOrDefault();

            // Se non è valorizzata la direzione di ordinamento, viene creato un filtro di tipo ORDER_DIRECTION
            if (orderDirection == null)
            {
                orderDirection = new FiltroRicerca()
                {
                    argomento = "ORDER_DIRECTION",
                    valore = "DESC"

                };

            }

            // Function da utilizzare per estrarre i valori del campo profilato da utilizzare per l'ordinamento
            String extractFieldValue = String.Empty, orderCondition = String.Empty;

            if (this.dbType == "SQL")
            {
                // DB SQL Server
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", Convert(int, @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0})) AS CUSTOM_FIELD", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", @dbuser@.getValCampoProfDoc(A.DOCNUMBER, {0}) AS CUSTOM_FIELD", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("CUSTOM_FIELD {0}", orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (sqlField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Format(", {0} AS ORDER_STANDARD", sqlField.valore);
                        orderCondition = String.Format("ORDER_STANDARD {0}", orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("Convert(datetime, A.DTA_INVIO) {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }

            }
            else
            {
                // DB ORACLE
                // Se bisogna ordinare per campo custom...
                if (profilationField != null)
                {
                    // ...recupero del dettaglio dell'oggetto custom
                    OggettoCustom obj = new Model().getOggettoById(profilationField.valore);

                    if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", to_number(getValCampoProfDoc(A.DOCNUMBER, {0}))", profilationField.valore);
                    else
                        // ...viene preparata la funzione per estrarre il valore del campo profilato
                        extractFieldValue = String.Format(", getValCampoProfDoc(A.DOCNUMBER, {0})", profilationField.valore);

                    // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                    orderCondition = String.Format("getValCampoProfDoc(A.DOCNUMBER, {0}) {1}", profilationField.valore, orderDirection.valore);
                }
                else
                {
                    // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                    if (oracleField != null)
                    {
                        // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                        extractFieldValue = String.Empty;
                        orderCondition = String.Format("{0} {1}", oracleField.valore, orderDirection.valore);
                    }
                    else
                    {
                        // Altrimenti viene creato il filtro standard
                        extractFieldValue = String.Empty;
                        // Andrea - Errore in esecuzione query per data null o data con anno valorizzato a 2000
                        //orderCondition = String.Format("TO_DATE (a.dta_invio, 'dd/MM/yyyy') {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                        orderCondition = String.Format("a.dta_invio {0}, B.ID_TRASMISSIONE {0}", orderDirection.valore);
                    }
                }
            }

            fieldDefinition += extractFieldValue;
            q.setParam("param1", fieldDefinition);

            // Impostazione dell'utente amministratore di db
            q.setParam("dbUser", this.getUserDB());
            // Impostazione del parametro di ordinamento
            q.setParam("orderCondition", orderCondition);

            #endregion


            string sqlDefinition = q.getSQL();

            if (dbType == "SQL" && !string.IsNullOrEmpty(chaiTableDef))
            {
                sqlDefinition = chaiTableDef + sqlDefinition;
            }

            logger.Debug(sqlDefinition);

            DataSet ds = null;

            this.ExecuteQuery(out ds, sqlDefinition);

            if (eliminaTramissioniMultiple)
            {

                DataSet dsTemp = new DataSet();
                // Creo una datatable
                DataTable tableTemp = new DataTable();
                tableTemp.Columns.Add("ID_TRASM", typeof(string));
                // Aggiungo il data table al data set
                dsTemp.Tables.Add(tableTemp);

                string idTrasmTemp = null;

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow row = ds.Tables[0].Rows[i];

                    string idTrasmIns = row["ID_TRASM"].ToString();

                    if (idTrasmIns != idTrasmTemp)
                    {
                        DataRow rowTemp = tableTemp.NewRow();
                        rowTemp["ID_TRASM"] = idTrasmIns;
                        tableTemp.Rows.Add(rowTemp);
                        idTrasmTemp = idTrasmIns;
                    }
                }
                ds = dsTemp;
            }

            totalPageNumber = 0;
            totalRecordCount = 0;

            startRecord = (requestedPageNumber * pageSize) - pageSize;
            // DataRow row = null;

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow row = ds.Tables[0].Rows[i];

                string idTrasm = row["ID_TRASM"].ToString();

                if (!list.Contains(idTrasm))
                {
                    if (!excel)
                    {
                        if (i >= startRecord && list.Count < pageSize)
                        {
                            list.Add(idTrasm);
                        }
                    }
                    else
                    {
                        list.Add(idTrasm);
                    }

                    totalRecordCount++;
                }
            }
            /* ABBATANGELI GIANLUIGI
            * Aggiunto il valore di configurazione MAX_ROW_SEARCHABLE
            * che determina il numero massimo di righe accettatte
            * come risultato di una ricerca trasmissioni */
            int maxRowSearchable = Cfg_MAX_ROW_SEARCHABLE(idAmministrazione);

            if (maxRowSearchable == 0 || totalRecordCount <= maxRowSearchable)
            {
                // Calcolo numero pagine totali
                totalPageNumber = (totalRecordCount / pageSize);
                if (totalPageNumber * pageSize < totalRecordCount)
                    totalPageNumber++;
            }
            else
            {
                /* ABBATANGELI GIANLUIGI
                * Non carico i documenti perchè raggiunto il numero massimo 
                * di righe per la ricerca ed imposto numTotPage = -2. */
                totalPageNumber = -2;
            }

            return list.ToArray();
        }

        /// <summary>
        /// Dato l'id di una trasmissione singola restituisce tutti i destinatari delle trasmissioni utente)
        /// </summary>
        public Dictionary<string, string> getDestinatariTrasmByListaTrasm(ArrayList trasmissioni)
        {
            Dictionary<string, string> destTrasm = new Dictionary<string, string>();
            string listaTrasm = string.Empty;
            string destinatari = string.Empty;
            string idTrasmSingola = string.Empty;

            if (trasmissioni != null && trasmissioni.Count > 0)
            {
                bool pass = false;
                foreach (DocsPaVO.trasmissione.Trasmissione trasmissione in trasmissioni)
                {
                    foreach (DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSing in trasmissione.trasmissioniSingole)
                    {
                        if (!pass)
                        {
                            listaTrasm += " a.id_trasm_singola = " + trasmissioneSing.systemId;
                            pass = true;
                        }
                        else
                        {
                            listaTrasm += " or a.id_trasm_singola = " + trasmissioneSing.systemId;
                        }
                    }

                }
            }


            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                string UserDB = string.Empty;

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ALL_DESTINATARI_TRASMISSIONI");


                q.setParam("listaTrasm", listaTrasm);

                if (dbType.ToUpper() == "SQL")
                {
                    UserDB = getUserDB();
                    q.setParam("dbuser", UserDB);
                }

                string commandText = q.getSQL();
                logger.Debug(commandText);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        idTrasmSingola = reader["system_id"].ToString();
                        if (destTrasm.ContainsKey(idTrasmSingola))
                        {
                            destinatari = destTrasm[idTrasmSingola];
                            destinatari = destinatari + ", " + reader["FULL_NAME"].ToString();
                            destTrasm.Remove(idTrasmSingola);
                            destTrasm.Add(idTrasmSingola, destinatari);
                        }
                        else
                        {
                            // cerco la descrizione del ruolo destinatario della trasmissione singola
                            using (DocsPaDB.DBProvider dbProvider1 = new DBProvider())
                            {
                                DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TRASM_SING_CORR_GLOBALI");
                                q1.setParam("param1", " and a.system_id = " + idTrasmSingola);
                                string descRuolo = string.Empty;
                                dbProvider1.ExecuteScalar(out descRuolo, q1.getSQL());
                                if (!string.IsNullOrEmpty(descRuolo))
                                    destinatari = "(" + descRuolo + ")";
                                else
                                    destinatari = "";
                            }
                            destinatari = destinatari + " " + reader["FULL_NAME"].ToString();
                            destTrasm.Add(idTrasmSingola, destinatari);
                        }
                    }
                }

            }

            return destTrasm;
        }

        public void UpdateTipoDirittoTrasm(string idPeopleOrGroups, string idDocOrFasc)
        {
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_CHA_TIPO_DIRITTO_SECURITY");
                q.setParam("personorgroup", idPeopleOrGroups);
                q.setParam("thing", idDocOrFasc);

                string commandText = q.getSQL();
                logger.Debug(commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
        }

        public void UpdateTipoDirittoTrasmFolder(string idPeopleOrGroups, string idFascicolo)
        {
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_CHA_TIPO_DIRITTO_SECURITY_DOC_IN_FOLDER");
                q.setParam("personorgroup", idPeopleOrGroups);
                q.setParam("idFascicolo", idFascicolo);

                string commandText = q.getSQL();
                logger.Debug(commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
        }

        //Lnr 09/06/3013
        public string getorderNotifyFilter(DocsPaVO.filtri.FiltroRicerca[] filter, string type)
        {
            string filterOrder = String.Empty;
            DocsPaVO.filtri.FiltroRicerca f;

            if (filter != null)
            {
                for (int i = 0; i < filter.Length; i++)
                {
                    f = filter[i];
                    if (f.valore != null && !f.valore.Equals(""))
                    {
                        switch (f.argomento)
                        {
                            //GESTIONE OGGETTO DEL DOCUMENTO
                            case "DTA_INVIO_DESC":
                                filterOrder += " DTA_INVIO DESC";
                                break;
                            case "DTA_SCAD_DESC":
                                filterOrder += " DTA_SCAD_DESC DESC";
                                break;
                            case "DTA_SCAD_ASC":
                                filterOrder += " DTA_SCAD_ASC ASC";
                                break;
                            case "DTA_INVIO_ASC":
                                filterOrder += " DTA_INVIO ASC";
                                break;

                        }
                    }
                }
            }

            //per settare un ordinamento di default
            if (filterOrder == "")
                filterOrder += " DTA_INVIO DESC";

            //POI VERIFICO SE FASC "F" O DOC "D" e aggiungo order necessario:

            switch (type)
            {
                case "D":
                    filterOrder += ",ID_PROFILE DESC";
                    break;
                case "F":
                    filterOrder += ",ID_PROJECT DESC";
                    break;
                case "S": //SMISTAMENTO
                    filterOrder += ",ID_TRASMISSIONE DESC";
                    break;
                //non dovrebbe essere utile.
                default:
                    filterOrder += ",ID_PROFILE DESC";
                    break;
            }


            return filterOrder;
        }

        public string getwhereNotifyFilter(DocsPaVO.filtri.FiltroRicerca[] filter)
        {
            string filterWhere = String.Empty;
            string filterWhereFasc = string.Empty;
            string filterWhereDoc = string.Empty;
            DocsPaVO.filtri.FiltroRicerca f;
            string UserDB = String.Empty;
            string docOrFasc = "";
            bool tipoRicerca = getTipoRicerca(filter);
            // verifica dbms
            if (dbType.ToUpper() == "SQL") UserDB = getUserDB();

            if (filter != null)
            {
                for (int i = 0; i < filter.Length; i++)
                {
                    f = filter[i];
                    if (f.valore != null && !f.valore.Equals(""))
                    {
                        switch (f.argomento)
                        {
                            case "TIPO_OGGETTO":
                                string valore = f.valore;
                                if (f.valore == "D")
                                    filterWhere += " and ID_PROFILE > 0";
                                if (f.valore == "F")
                                    filterWhere += " and ID_PROJECT > 0";
                                //if (f.valore == "T")
                                //filterWhere += " and (ID_PROFILE > 0 OR ID_PROJECT > 0)";
                                docOrFasc = f.valore;
                                break;
                            //GESTIONE OGGETTO DEL DOCUMENTO
                            case "OGGETTO_DOCUMENTO_TRASMESSO":
                                if (!string.IsNullOrEmpty(UserDB))
                                    filterWhereDoc += " AND UPPER(" + UserDB + ".Vardescribe(ID_PROFILE, 'DESC_OGGETTO')) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                                else
                                    filterWhereDoc += " AND UPPER(Vardescribe(ID_PROFILE, 'DESC_OGGETTO')) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                                break;
                            //GESTIONE OGGETTO DEL FASCICOLO
                            case "OGGETTO_FASCICOLO_TRASMESSO":
                                if (!string.IsNullOrEmpty(UserDB))
                                    filterWhereFasc += " AND UPPER(" + UserDB + ".Vardescribe(ID_PROJECT, 'DESC_FASC')) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                                else
                                    filterWhereFasc += " AND UPPER(Vardescribe(ID_PROJECT, 'DESC_FASC')) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%'";
                                break;
                            //GESTIONE FILTRO RAGIONE DI TRASMISSIONE
                            case "RAGIONE":
                                if (!string.IsNullOrEmpty(UserDB))
                                    filterWhere += " AND UPPER(" + UserDB + ".Vardescribe(ID_RAGIONE_TRASM, 'RAGIONETRASM')) = '" + f.valore.ToString().ToUpper() + "'";
                                else
                                    filterWhere += " AND UPPER(Vardescribe(ID_RAGIONE_TRASM, 'RAGIONETRASM')) = '" + f.valore.ToString().ToUpper() + "'";
                                break;
                            //GESTIONE FILTRI DATA DI TRASMISSIONE
                            case "TRASMISSIONE_IL":
                                filterWhere += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("DTA_INVIO", false) + "= '" + f.valore + "'";
                                break;
                            case "TRASMISSIONE_SUCCESSIVA_AL":
                                filterWhere += " AND DTA_INVIO>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                                break;
                            case "TRASMISSIONE_PRECEDENTE_IL":
                                filterWhere += " AND DTA_INVIO<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                                break;
                            //GESTIONE FILTRI SCADENZA
                            case "SCADENZA_IL":
                                filterWhere += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("DTA_SCADENZA", false) + "= '" + f.valore + "'";
                                break;
                            case "SCADENZA_SUCCESSIVA_AL":
                                filterWhere += " AND TS.DTA_SCADENZA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                                break;
                            case "SCADENZA_PRECEDENTE_IL":
                                filterWhere += " AND TS.DTA_SCADENZA<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                                break;
                            //GESTIONE FILTRI MITTENTE TRASMISSIONE DA RUBRICA
                            case "COD_RUBR_MITT_RUOLO":
                                filterWhere += " AND T.ID_RUOLO_IN_UO IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '" + f.valore.ToUpper().Replace("'", "''") + "')";
                                break;
                            case "COD_RUBR_MITT_UTENTE":
                                //filterWhere += " AND ID_PEOPLE_MITT = (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '" + f.valore.ToUpper().Replace("'", "''") + "')";
                                filterWhere += " AND T.ID_PEOPLE = (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_IE='I' AND UPPER(VAR_COD_RUBRICA) = '" + f.valore.ToUpper().Replace("'", "''") + "')";
                                break;
                            case "ID_UO_MITT":
                                //filterWhere += " AND ID_RUOLO_MITT IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND ID_UO =" + f.valore + ")";
                                filterWhere += " AND T.ID_RUOLO_IN_UO IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND ID_UO in (select system_id from dpa_corr_globali where var_cod_rubrica = '" + f.valore + "'))";
                                break;
                            //GESTIONE FILTRI MITTENTE TRASMISSIONE DA INSERIMENTO TESTUALE
                            case "MITTENTE_RUOLO":
                                //filterWhere += " AND ID_RUOLO_MITT IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%')";
                                filterWhere += " AND T.ID_RUOLO_IN_UO IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%')";
                                break;
                            case "MITTENTE_UTENTE":
                                //filterWhere += " AND ID_PEOPLE_MITT IN (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%')";
                                filterWhere += " AND T.ID_PEOPLE IN (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='P' AND CHA_TIPO_IE='I' AND UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%')";
                                break;
                            case "MITTENTE_UO":
                                //filterWhere += " AND ID_RUOLO_MITT IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND ID_UO =(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE UPPER(VAR_DESC_CORR) IN ('" + f.valore.ToUpper().Replace("'", "''") + "')))";
                                filterWhere += " AND T.ID_RUOLO_IN_UO IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND ID_UO =(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE UPPER(VAR_DESC_CORR) IN ('" + f.valore.ToUpper().Replace("'", "''") + "')))";
                                break;
                            case "TIPO":
                                if (tipoRicerca)
                                {
                                    filterWhereDoc += " AND ";
                                    if (f.valore.Equals("T"))
                                    {
                                        if (!string.IsNullOrEmpty(UserDB))
                                        {
                                            filterWhereDoc += " UPPER(" + UserDB + ".Vardescribe(ID_PROFILE, 'CHA_TIPO_PROTO')) IN ('A', 'P', 'I','G') ";
                                        }
                                        else
                                        {
                                            filterWhereDoc += " UPPER(Vardescribe(ID_PROFILE, 'CHA_TIPO_PROTO')) IN  ('A', 'P', 'I','G') ";
                                        }
                                    }
                                    else
                                        if (f.valore.Equals("PR"))
                                        {
                                            filterWhereDoc += " id_profile in (select system_id from profile where cha_da_proto = '1') ";
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(UserDB))
                                            {
                                                filterWhereDoc += " UPPER(" + UserDB + ".Vardescribe(ID_PROFILE, 'CHA_TIPO_PROTO')) = '" + f.valore.ToUpper() + "'";
                                            }
                                            else
                                            {
                                                filterWhereDoc += " UPPER(Vardescribe(ID_PROFILE, 'CHA_TIPO_PROTO')) = '" + f.valore.ToUpper() + "'";
                                            }


                                        }
                                }
                                break;
                            case "ELEMENTI_NON_VISTI":
                                if (f.valore.Equals("1"))
                                    filterWhere += " AND dta_vista = " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753");
                                break;
                            case "DOCUMENTI_ACQUISITI":

                                if (f.valore.Equals("1"))
                                {
                                    if (!string.IsNullOrEmpty(UserDB))
                                        filterWhereDoc += " AND UPPER(" + UserDB + ".Vardescribe(ID_PROFILE, 'PROFILE_CHA_IMG')) <> '0'";
                                    else
                                        filterWhereDoc += " AND UPPER(Vardescribe(ID_PROFILE, 'PROFILE_CHA_IMG')) <> '0'";
                                }
                                break;
                            case "DOCUMENTI_FIRMATI":
                                if (f.valore.Equals("1"))
                                {
                                    if (!string.IsNullOrEmpty(UserDB))
                                    {
                                        filterWhereDoc += " AND UPPER(" + UserDB + ".Vardescribe(ID_PROFILE, 'COMPONENTS_CHA_FIRMATO')) = '" + f.valore.ToUpper() + "'";
                                    }
                                    else
                                    {
                                        filterWhereDoc += " AND UPPER(Vardescribe(ID_PROFILE, 'COMPONENTS_CHA_FIRMATO')) = '" + f.valore.ToUpper() + "'";
                                    }
                                }
                                else
                                {
                                    if (f.valore.Equals("0"))
                                    {
                                        if (!string.IsNullOrEmpty(UserDB))
                                        {
                                            filterWhereDoc += " AND UPPER(" + UserDB + ".Vardescribe(ID_PROFILE, 'COMPONENTS_CHA_FIRMATO')) = '" + f.valore.ToUpper() + "'";
                                        }
                                        else
                                        {
                                            filterWhereDoc += " AND UPPER(Vardescribe(ID_PROFILE, 'COMPONENTS_CHA_FIRMATO')) = '" + f.valore.ToUpper() + "'";
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(UserDB))
                                        {
                                            filterWhereDoc += " AND UPPER(" + UserDB + ".Vardescribe(ID_PROFILE, 'PROFILE_CHA_IMG')) <> '0'";
                                        }
                                        else
                                        {
                                            filterWhereDoc += " AND UPPER(Vardescribe(ID_PROFILE, 'PROFILE_CHA_IMG')) <> '0'";
                                        }
                                    }
                                }
                                break;
                            case "TIPO_FILE_ACQUISITO":
                                if (!string.IsNullOrEmpty(UserDB))
                                {
                                    filterWhereDoc += " AND " + UserDB + ".vardescribe(ID_PROFILE, 'PROFILE_CHA_IMG')='" + f.valore.ToUpper() + "'";
                                }
                                else
                                {
                                    filterWhereDoc += " AND vardescribe(ID_PROFILE, 'PROFILE_CHA_IMG')='" + f.valore.ToUpper() + "'";
                                }
                                break;
                            case "TRASMISSIONI_ACCETTATE":
                                if (f.valore.Equals("1"))
                                    //filterWhereDoc += " and id_trasm_utente in (select system_id from dpa_trasm_utente where cha_accettata='0' )";
                                    filterWhereDoc += " and exists (select system_id from dpa_ragione_trasm where system_id = TS.id_ragione and CHA_TIPO_RAGIONE = 'W' ) AND dta_vista = " + DocsPaDbManagement.Functions.Functions.ToDate("01/01/1753");
                                break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(docOrFasc))
                {
                    if (docOrFasc == "D")
                        filterWhere += filterWhereDoc;
                    if (docOrFasc == "F")
                        filterWhere += filterWhereFasc;
                    if (docOrFasc == "T")
                    {
                        filterWhere += "and ( (ID_PROFILE > 0 " + filterWhereDoc + ") or (ID_PROJECT > 0 " + filterWhereFasc + ") )";

                    }
                }
                else
                    filterWhere += filterWhereDoc + filterWhereFasc;
            }
            return filterWhere;
        }

        #region Autenticazione Sistemi Esterni

        public bool cessioneProprietaSistemaEsterno(string idOggetto, string idRuoloDest, string idUtenteDest, string idSysExt, string idUtSysExt)
        {
            bool retval = false;

            retval = deleteSecurity(idOggetto, idUtenteDest, "0", true, true);
            if (retval)
            {
                retval = deleteSecurity(idOggetto, idRuoloDest, "0", true, true);
                if (retval)
                {
                    retval = insertSecurity(idOggetto, idUtenteDest, "0", "null", "P");
                    if (retval)
                    {
                        retval = insertSecurity(idOggetto, idRuoloDest, "255", idRuoloDest, "P");
                        if (retval)
                        {
                            retval = impostaDirittoEAggiornaAccessrights("A", " PERSONORGROUP IN (" + idSysExt + "," + idUtSysExt + ") AND THING = " + idOggetto, "63");
                            if (retval)
                            {
                                retval = insertDeletedSecurity("Sistema esterno",idOggetto,idUtSysExt,"0","null","P",idUtenteDest,idRuoloDest);
                                retval = insertDeletedSecurity("Sistema esterno", idOggetto, idSysExt, "255", "null", "P", idUtenteDest, idRuoloDest);
                                retval = insertDeletedSecurity("Sistema esterno", idOggetto, idUtenteDest, "20", "null", "T", idUtenteDest, idRuoloDest);
                                retval = insertDeletedSecurity("Sistema esterno", idOggetto, idRuoloDest, "20", "null", "T", idUtenteDest, idRuoloDest);

                                
                            }
                        }
                    }
                }
            }
            return retval;
        }

        public bool AggDirittiSysExtInDelSec(string idOggetto, string idUtSysExt, string idSysExt, string accessRights)
        {
            bool retval = false;
            retval = insertDeletedSecurity("Sistema esterno", idOggetto, idUtSysExt, accessRights, "null", "A", idUtSysExt, idSysExt);
            retval = insertDeletedSecurity("Sistema esterno", idOggetto, idSysExt, accessRights, "null", "A", idUtSysExt, idSysExt);
                                 
            return retval;
        }

        public bool CessioneTotaleDirittiSysExt(string idOggetto, string idUtSysExt, string idSysExt, string accessRights)
        {
            bool retval = false;
            retval = deleteSecurity(idOggetto, idUtSysExt, "0", true, true);
            retval = deleteSecurity(idOggetto, idSysExt, "0", true, true);
            retval = insertDeletedSecurity("Sistema esterno", idOggetto, idUtSysExt, accessRights, "null", "A", idUtSysExt, idSysExt);
            retval = insertDeletedSecurity("Sistema esterno", idOggetto, idSysExt, accessRights, "null", "A", idUtSysExt, idSysExt);
            
            return retval;
        }

        public bool CleanRightsExtSysAfterCreation(string idOggetto, string idUtSysExt, string idSysExt)
        {
            bool retval = false;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_CLEAN_RIGHTS_AFTER_CREATION_SYS_EXT");

                    q.setParam("param1", idOggetto);
                    q.setParam("param2", string.Format("{0},{1}",idSysExt,idUtSysExt));
                    
                    string queryString = q.getSQL();
                    logger.Debug(queryString);
                    retval = dbProvider.ExecuteNonQuery(queryString);
                }
            }
            catch
            {
                return false;
            }
            return retval;
        }
        #endregion

        /// <summary>
        /// Controllo dell'avvenuta trasmissione di un protocollo interno.
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public bool DocumentAlreadyTransmitted_Opt(string idDocument)
        {
            bool retval = false;
            DataSet ds = new DataSet();
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_DOCUMENTALREADY_TRASMITTED_OPT");
            queryDef.setParam("param1", idDocument);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);
            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteQuery(out ds, "SPEDS", commandText);
                if (ds.Tables["SPEDS"].Rows.Count > 0)
                    retval = true;
            }

            return retval;
        }


        /// <summary>
        /// Metodo per il recupero del tipo associato all'ultima trasmissione effettuata per un dato documento
        /// </summary>
        /// <param name="documentId">Id del documento</param>
        /// <returns>T se è di tipo tutti; S se è singola</returns>
        public char GetLastTrasmTypeForDocument(String documentId)
        {
            // Query per il recupero dell'ultima trasmissione effettuata per il documento
            String query = String.Format(
                "select distinct(cha_tipo_trasm) from dpa_trasm_singola where id_trasmissione in ( select max(system_id) from dpa_trasmissione where id_profile = {0} )",
                documentId);

            // Lettura del tipo di trasmissione
            char tipoTrasm = 'S';
            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query))
                {
                    while (dataReader.Read())
                    {
                        tipoTrasm = dataReader[0].ToString()[0];
                    }
                }
            }

            return tipoTrasm;

        }

        public string GetIdTrasmissioneByIdTrasmSingola(string idTrasmissioneSingola)
        {
            string idTrasmissione = string.Empty;

            logger.Info("Inizio Metodo GetNextIstanzaPasso in DocsPaDb.Query_DocsPAWS.LibroFirma");
            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TRASM_SINGOLA_ID_TRASMISSIONE");
                q.setParam("idTrasmissioneSingola", idTrasmissioneSingola);
                query = q.getSQL();
                logger.Debug("GetIdTrasmissioneByIdTrasmSingola: " + query);

                if (this.ExecuteQuery(out ds, "idTrasmissione", query))
                {
                    if (ds.Tables["idTrasmissione"] != null && ds.Tables["idTrasmissione"].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["idTrasmissione"].Rows[0];
                        idTrasmissione = row["ID_TRASMISSIONE"].ToString();
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error("Errore in DocsPaDb.Query_DocsPAWS.LibroFirma - Metodo GetIdTrasmissioneByIdTrasmSingola", exc);
                return null;
            }
            logger.Info("Fine Metodo GetIdTrasmissioneByIdTrasmSingola in DocsPaDb.Query_DocsPAWS.LibroFirma");

            return idTrasmissione;
        }

        public string GetTipoRagioneByIdTrasmSingola(string idTrasmSingola)
        {
            string tipoRagione = string.Empty;

            try
            {
                string query;
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_TIPORAG_TRASMSING");
                q.setParam("idTrasmSing", idTrasmSingola);
                query = q.getSQL();
                logger.Debug(query);

                if (this.ExecuteQuery(out ds, "TIPORAG", query))
                {
                    if (ds.Tables["TIPORAG"] != null && ds.Tables["TIPORAG"].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["TIPORAG"].Rows[0];
                        tipoRagione = row["TIPORAGIONE"].ToString();
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(exc);
                return null;
            }
            
            return tipoRagione;
        }
    }
}
