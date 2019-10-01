using System;
using System.Data;
using DocsPaUtils;
using System.Configuration;
using System.Collections;
using System.Globalization;
using DocsPaVO;
using System.Collections.Generic;
using log4net;
using DocsPaVO.utente;
using System.Linq;
using DocsPaDbManagement.Functions;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Summary description for Utenti.
    /// </summary>
    public class Utenti : DBProvider
    {
        private static ILog logger = LogManager.GetLogger(typeof(Utenti));
        #region addressBookManager
        public void InsertCanalePref(string sysId, DocsPaVO.utente.Canale canalePref)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPATCanaleCorr");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("") + "'" + sysId + "','" + canalePref.systemId + "','1'");
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteNonQuery(sql);
        }

        public bool InsertCanalePrefMail(string idCorrGlobale)
        {
            bool result = true;

            try
            {
                logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> InsertCanalePrefMail");

                //si ricava la systemId del canale preferenziale MAIL
                string idCanaleMail = "";
                DocsPaUtils.Query q;

                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DocumentTypes2");
                string queryString = q.getSQL();
                logger.Debug(queryString);
                this.ExecuteScalar(out idCanaleMail, queryString);

                if (idCanaleMail != null && idCanaleMail != String.Empty)
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPATCanCorr2");
                    q.setParam("param10", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    q.setParam("param1", idCorrGlobale);
                    q.setParam("param2", idCanaleMail);
                    q.setParam("param20", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(""));
                    string insertString = q.getSQL();
                    logger.Debug(insertString);

                    this.ExecuteNonQuery(insertString);
                }

                logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> setCanalePrefMail");
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nell'inserimento del corrispondente:" + ex.Message);
                result = false;
            }
            return result;
        }
        /// <summary>
        /// controlla se i dettagli di un corrispondente occassionale sono già esistenti.
        /// </summary>
        /// <returns></returns>
        public bool CheckExistDettagliCorr(string idCorr)
        {
            bool result = false;
            logger.Debug("CheckExistDettagliCorr");
            try
            {
                DocsPaUtils.Query q = new Query("SELECT COUNT(ID_CORR_GLOBALI) AS TOT FROM DPA_DETT_GLOBALI WHERE ID_CORR_GLOBALI=" + idCorr);
                string sql = q.getSQL();
                logger.Debug(sql);

                IDataReader dr = (IDataReader)this.ExecuteReader(sql);
                if (dr == null)
                    throw new Exception();

                if (dr.Read())
                    result = (Convert.ToDecimal(dr[0]) != 0);

                if (!dr.IsClosed)
                    dr.Close();


            }
            catch (Exception)
            {
                //throw e;
                result = false;
            }
            return result;

        }
        public bool InsertDettagli(string systemId, DocsPaVO.addressbook.DettagliCorrispondente dettagli)
        {
            bool result = true;
            logger.Debug("insertDettagli");
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPADettGlobali");
                q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("") + systemId + ",'" + CorrectApici(dettagli.Corrispondente[0].indirizzo) + "','" + CorrectApici(dettagli.Corrispondente[0].cap) + "','" + CorrectApici(dettagli.Corrispondente[0].citta) + "','" + CorrectApici(dettagli.Corrispondente[0].provincia) + "','" + CorrectApici(dettagli.Corrispondente[0].nazione) + "','" +
                    CorrectApici(dettagli.Corrispondente[0].telefono) + "','" + CorrectApici(dettagli.Corrispondente[0].telefono2) + "','" + CorrectApici(dettagli.Corrispondente[0].fax) + "','" + CorrectApici(dettagli.Corrispondente[0].codiceFiscale) + "','" + CorrectApici(dettagli.Corrispondente[0].note) + "','" + CorrectApici(dettagli.Corrispondente[0].localita) + "','" + CorrectApici(dettagli.Corrispondente[0].luogoNascita) + "','" + CorrectApici(dettagli.Corrispondente[0].dataNascita) + "','" + CorrectApici(dettagli.Corrispondente[0].titolo) + "','" + CorrectApici(dettagli.Corrispondente[0].partitaIva) + "'");// + ",'" + CorrectApici(dettagli.Corrispondente[0].codiceIpa) + "'");
                string sql = q.getSQL();
                logger.Debug(sql);
                //database.executeNonQuery(insertDettagliQuery);
                result = this.ExecuteNonQuery(sql);
            }
            catch (Exception)
            {
                //throw e;
                result = false;
            }
            return result;
        }

        public bool CompletaInserimentoCorrispPerInterop(string IdCorrGlobale)
        {
            bool result = true;
            logger.Debug("insertDettagli");
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPADettGlobali2");
                q.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                q.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CORR_GLOBALI"));
                //q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(""));
                q.setParam("param3", IdCorrGlobale);
                string sql = q.getSQL();
                logger.Debug(sql);

                this.ExecuteNonQuery(sql);

                if (!InsertCanalePrefMail(IdCorrGlobale))
                {
                    throw new Exception();
                }

            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public string UpdateCodRubrica(string systemId, string prefix, string idAmm)
        {
            logger.Debug("UPDATE CODICE RUBRICA E VAR CODICE");
            string res = null;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlobali");
                q.setParam("param1", "VAR_COD_RUBRICA='" + prefix + systemId + "' , VAR_CODICE = '" + prefix + systemId + "'");
                q.setParam("param2", "SYSTEM_ID=" + systemId + " AND ID_AMM=" + idAmm);
                //	" AND NOT EXISTS (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI A WHERE ID_AMM="+idAmm+" AND VAR_COD_RUBRICA='"+prefix+systemId+"')");
                string sql = q.getSQL();
                logger.Debug(sql);
                this.ExecuteNonQuery(sql);

                /*if(resultUpdate==false)
                {
                    throw new Exception("Update del codice rubrica non effettuato");
                }*/
                res = prefix + systemId;
            }
            catch (Exception)
            {
                res = null;
            }
            return res;

        }

        public void IsCodRubricaPresente(string codRubrica, string tipoCorr, string idAmm, string idReg, bool inRubricaComune, out DataSet dataSet)
        {
            //bool result = true;

            logger.Debug("isCodRubricaPresente");
            //string sql="SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI A WHERE VAR_COD_RUBRICA='"+codRubrica+"' AND (ID_AMM="+idAmm+" OR ID_AMM IS NULL)";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
            q.setParam("param1", "A.SYSTEM_ID,A.CHA_TIPO_IE,A.CHA_TIPO_CORR");

            string param2Value = string.Format("UPPER(A.VAR_COD_RUBRICA)='{0}'", codRubrica.ToUpper());
            if (!string.IsNullOrEmpty(idAmm))
                param2Value = string.Concat(param2Value, string.Format(" AND (A.ID_AMM = {0} OR A.ID_AMM IS NULL)", idAmm));

            // Il pezzo di codice che controlla l'id registro era stato commentato. Ora è stato decommentato e modificato
            // in modo che renda possibile inserire un corrispondente su TUTTI e su un registro / RF (l'univocià quindi è
            // su Codice rubrica + Id registro. 
            if (!String.IsNullOrEmpty(idReg))
                param2Value = String.Concat(param2Value, string.Format(" AND ((A.ID_REGISTRO = {0}) OR (A.ID_REGISTRO IS NULL AND A.CHA_TIPO_IE='I'))", idReg));
            else
                param2Value = string.Concat(param2Value, string.Format(" AND A.ID_REGISTRO IS NULL"));

            param2Value = string.Concat(param2Value, " AND A.DTA_FINE IS NULL");
            /* Emanuela 26/06/2014: commentato perchè nel verificarsi di ciascuna condizione quello che fa è aggiungere sempre AND A.DTA_FINE IS NULL
            if (inRubricaComune != null)
            {

                if (!inRubricaComune)
                {
                    if (!string.IsNullOrEmpty(idReg))
                    {
                        param2Value = string.Concat(param2Value, string.Format(" AND (A.ID_REGISTRO = {0}) AND A.DTA_FINE IS NULL ", idReg));
                    }
                    else
                    {
                        param2Value = string.Concat(param2Value, " AND A.DTA_FINE IS NULL ");
                    }
                }
                else
                    param2Value = string.Concat(param2Value, " AND A.DTA_FINE IS NULL");
            }
            else
            {
                logger.Debug("ERRORE in IsCodRubricaPresente il tipoCorr è null");
                param2Value = string.Concat(param2Value, string.Format(" AND A.DTA_FINE IS NULL"));
            }
             * */
            q.setParam("param2", param2Value);

            string sql = q.getSQL();
            logger.Debug(sql);
            //database.fillTable(codiceString,dataSet,"CODICE");
            this.ExecuteQuery(out dataSet, "CODICE", sql);
            if (dataSet.Tables["CODICE"].Rows.Count > 0)
            {
                if (inRubricaComune)
                {
                    foreach (DataRow row in dataSet.Tables["CODICE"].Rows)
                    {
                        if (row["CHA_TIPO_IE"].ToString() != "E" || row["CHA_TIPO_CORR"].ToString() == "C")
                        {
                            logger.Debug("Codice già presente in rubrica (Query - IsCodRubricaPresente)");
                            throw new Exception("Codice già presente in rubrica");
                        }
                        else
                        {
                            string dataFine = DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US")));
                            string newCod = codRubrica + "_" + row["SYSTEM_ID"].ToString();
                            invalidaCorr(dataFine, row["SYSTEM_ID"].ToString(), newCod);
                        }
                    }
                }
                else
                {
                    foreach (DataRow row in dataSet.Tables["CODICE"].Rows)
                    {
                        if (row["CHA_TIPO_CORR"].ToString() == "C")
                        {
                            logger.Debug("Codice già presente in Rubrica Comune (Query - IsCodRubricaPresente)");
                            throw new Exception("Corrispondente già presente in Rubrica Comune");

                        }
                        else
                        {
                            logger.Debug("Codice già presente in rubrica (Query - IsCodRubricaPresente)");
                            throw new Exception("Codice già presente in rubrica");
                        }
                    }
                }
                //result = false;
            }

            //Emanuela 27-01-2014: controllo che non esista una lista con lo stesso codice rubrica
            if (!inRubricaComune)
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("LS_IS_UNIQUE_COD_LISTA");
                queryMng.setParam("param1", codRubrica.ToUpper());
                queryMng.setParam("param2", idAmm);
                string commandText = queryMng.getSQL();
                DataSet ds = new DataSet();
                this.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    logger.Debug("Esiste una lista con lo stesso codice rubrica (Query - IsCodRubricaPresente)");
                    throw new Exception("Esiste una lista con lo stesso codice rubrica");
                }
            }
            //return result;			
        }

        public DocsPaVO.utente.Canale GetCanalePreferenzialeByIdCorr(string systemId)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Amministrazione> GetCanalePreferenzialeByIdCorr");
            DocsPaVO.utente.Canale canalePref = null;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPATCanaleCorr1");
            q.setParam("param1", systemId);
            string queryString = q.getSQL();

            logger.Debug(queryString);



            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                canalePref = new DocsPaVO.utente.Canale();
                canalePref.systemId = row[0].ToString();
                canalePref.descrizione = row[1].ToString();
                canalePref.typeId = row[2].ToString();
            }

            dataSet.Dispose();

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Amministrazione> GetCanalePreferenzialeByIdCorr");
            return canalePref;
        }

        public void GetCanali(out DataSet dataSet)
        {
            //string sql="SELECT SYSTEM_ID,TYPE_ID,DESCRIPTION FROM DOCUMENTTYPES";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DocumentTypes3");
            string sql = q.getSQL();
            this.ExecuteQuery(out dataSet, "CANALI", sql);
        }

        public void GetRuoliSuperiori(out DataSet dataSet, DocsPaVO.utente.Ruolo ruolo)
        {
            //string queryString="SELECT A.SYSTEM_ID, A.VAR_COD_RUBRICA, A.ID_UO, A.ID_GRUPPO, A.VAR_CODICE, A.ID_PARENT, B.VAR_DESC_RUOLO, B.NUM_LIVELLO FROM DPA_CORR_GLOBALI A, DPA_TIPO_RUOLO B WHERE CHA_TIPO_URP='R' AND ID_UO="+ruolo.uo.systemId+" AND B.NUM_LIVELLO < "+ruolo.livello+" AND B.SYSTEM_ID=A.ID_TIPO_RUOLO";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__TIPO_RUOLO");
            q.setParam("param1", "A.SYSTEM_ID, A.VAR_COD_RUBRICA, A.ID_UO, A.ID_GRUPPO, A.VAR_CODICE, A.ID_PARENT, B.VAR_DESC_RUOLO, B.NUM_LIVELLO AS NUM_LIV_B, A.VAR_DESC_CORR");
            q.setParam("param2", "CHA_TIPO_URP='R' AND ID_UO=" + ruolo.uo.systemId + " AND B.NUM_LIVELLO < " + ruolo.livello + " AND B.SYSTEM_ID=A.ID_TIPO_RUOLO AND A.DTA_FINE IS NULL");
            string sql = q.getSQL();
            logger.Debug(sql);

            //database.fillTable(queryString,dataSet,"RUOLI_SUP");
            this.ExecuteQuery(out dataSet, "RUOLI_SUP", sql);
        }


        public void InsertRUOEUtente(string sysId, DocsPaVO.utente.Corrispondente parent)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPARuoeUtente");
            /*string insertRuoeUtente="INSERT INTO DPA_RUOE_UTENTE ";
            insertRuoeUtente=insertRuoeUtente+"(SYSTEM_ID, ID_UTENTE_EST,ID_RUOE) VALUES (1,";
            insertRuoeUtente=insertRuoeUtente+sysId+",";
            insertRuoeUtente=insertRuoeUtente+parent.systemId+")";*/
            q.setParam("param1", sysId + "," + parent.systemId);
            string sql = q.getSQL();
            logger.Debug(sql);

            string res;
            this.InsertLocked(out res, sql, "DPA_RUOE_UTENTE");
        }

        public string InsertUtente(DocsPaVO.utente.Corrispondente corr)
        {

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPACorrglobali");
            if (corr.idRegistro == null || (corr.idRegistro != null && corr.idRegistro == string.Empty))
            {
                corr.idRegistro = "NULL";
            }
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName() + " CHA_TIPO_IE, ID_REGISTRO, ID_AMM, VAR_DESC_CORR, ID_OLD, DTA_INIZIO, VAR_CODICE, CHA_TIPO_CORR, CHA_TIPO_URP, VAR_COD_RUBRICA, CHA_DETTAGLI, VAR_EMAIL, VAR_COGNOME, VAR_NOME,VAR_CODICE_AMM,VAR_CODICE_AOO,CHA_PA");

            string lastParam = string.Empty;
            if (!string.IsNullOrEmpty(CorrectApici(((DocsPaVO.utente.Utente)corr).titolo)))
                lastParam = DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("") + "'E'," +
                corr.idRegistro + "," + corr.idAmministrazione + "," + "'" + CorrectApici(((DocsPaVO.utente.Utente)corr).titolo) + " " + CorrectApici(((DocsPaVO.utente.Utente)corr).cognome) + " " +
                CorrectApici(((DocsPaVO.utente.Utente)corr).nome) + "'," + "0," + DocsPaDbManagement.Functions.Functions.GetDate() + "," +     //DocsPaWS.Utils.dbControl.getDate()+"," +
                "'" + CorrectApici(corr.codiceCorrispondente) + "'," + "'S'," + "'P',";
            else
                lastParam = DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("") + "'E'," +
                corr.idRegistro + "," + corr.idAmministrazione + "," + "'" + CorrectApici(((DocsPaVO.utente.Utente)corr).cognome) + " " +
                CorrectApici(((DocsPaVO.utente.Utente)corr).nome) + "'," + "0," + DocsPaDbManagement.Functions.Functions.GetDate() + "," +     //DocsPaWS.Utils.dbControl.getDate()+"," +
                "'" + CorrectApici(corr.codiceCorrispondente) + "'," + "'S'," + "'P',";
            //DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_CORR_GLOBALI") +"'E'," +
            if (corr.codiceRubrica != null && !corr.codiceRubrica.Equals(""))
            {
                lastParam = lastParam + "'" + CorrectApici(corr.codiceRubrica) + "',";
            }
            else
            {
                lastParam = lastParam + "'',";
            }
            if (corr.dettagli)
            {
                lastParam = lastParam + "'1',";
            }
            else
            {
                lastParam = lastParam + "'0',";
            }
            lastParam = lastParam + "'" + CorrectApici(((DocsPaVO.utente.Utente)corr).email) + "',";
            lastParam = lastParam + "'" + CorrectApici(((DocsPaVO.utente.Utente)corr).cognome) + "',";
            lastParam = lastParam + "'" + CorrectApici(((DocsPaVO.utente.Utente)corr).nome) + "'";
            lastParam = lastParam + ",'" + corr.codiceAmm + "','" + corr.codiceAOO + "'";
            lastParam = lastParam + ",'1'";
            q.setParam("param2", lastParam);
            logger.Debug("param2=" + lastParam);
            string sql = q.getSQL();
            logger.Debug(sql);

            string res;
            this.InsertLocked(out res, sql, "DPA_CORR_GLOBALI");
            return res;
        }

        public bool InsertOccasionale(DocsPaVO.utente.DatiModificaCorr datiModifica, out string newIdCorrGlobali, out string message)
        {
            bool result = true;
            string prefix = System.Configuration.ConfigurationManager.AppSettings["prefissoCorrOccasionale"];
            //DocsPaDB.DBProvider db=new DBProvider();
            // Creazione parametri SP
            ArrayList sp_params = new ArrayList();
            DocsPaUtils.Data.ParameterSP res;

            res = new DocsPaUtils.Data.ParameterSP("RESULT", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("ID_REG", 0));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("IDAMM", 0));

            sp_params.Add(new DocsPaUtils.Data.ParameterSP("Prefix_cod_rub", prefix));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("DESC_CORR", datiModifica.descCorr));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("CHA_DETTAGLI", "0"));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("ID_CORR_GLOBALI", datiModifica.idCorrGlobali));

            sp_params.Add(res);
            //this.BeginTransaction();
            int resultStore = this.ExecuteStoredProcedure("INS_OCC", sp_params, null);

            if (res.Valore != null && res.Valore.ToString() != "" && resultStore != -1 && resultStore != 0)
            {
                newIdCorrGlobali = res.Valore.ToString();
                //this.CommitTransaction();
            }
            else
            {
                throw new Exception();
            }
            //else this.RollbackTransaction();
            if (!string.IsNullOrEmpty(newIdCorrGlobali))
                message = "OK";
            else
            {
                message = "KO";
                result = false;
            }
            return result;

        }


        public string InsertRuolo(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.Corrispondente parent)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPACorrglobali");
            if (corr.idRegistro == null || (corr.idRegistro != null && corr.idRegistro == string.Empty))
            {
                corr.idRegistro = "NULL";
            }
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName() + " CHA_TIPO_IE, ID_REGISTRO, ID_AMM, VAR_DESC_CORR, ID_OLD, DTA_INIZIO, ID_UO, VAR_CODICE, CHA_TIPO_CORR, CHA_TIPO_URP, VAR_COD_RUBRICA, CHA_DETTAGLI,var_email,VAR_CODICE_AMM,VAR_CODICE_AOO,CHA_PA");
            string param = DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("") + "'E'," +  //DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_CORR_GLOBALI") +"'E'," +
                corr.idRegistro + "," + corr.idAmministrazione + "," + "'" + CorrectApici(corr.descrizione) +
                " " + CorrectApici(((DocsPaVO.utente.UnitaOrganizzativa)parent).descrizione) + "'," + "0," +
                DocsPaDbManagement.Functions.Functions.GetDate() + "," + parent.systemId + "," +  //DocsPaWS.Utils.dbControl.getDate()+"," + parent.systemId+"," +
                "'" + CorrectApici(corr.codiceCorrispondente) + "'," + "'S'," + "'R',";
            if (corr.codiceRubrica != null && !corr.codiceRubrica.Equals(""))
            {
                param = param + "'" + CorrectApici(corr.codiceRubrica) + "',";
            }
            else
            {
                param = param + "'',";
            }
            if (corr.dettagli)
            {
                param = param + "'1'";
            }
            else
            {
                param = param + "'0'";
            }
            param = param + ",'" + corr.email + "','" + corr.codiceAmm + "','" + corr.codiceAOO + "'";
            param = param + ",'1'";
            q.setParam("param2", param);
            string sql = q.getSQL();
            logger.Debug(sql);

            string res;
            this.InsertLocked(out res, sql, "DPA_CORR_GLOBALI");
            return res;
        }

        public string GetSignTypePreference(string idPeople)
        {
            logger.Debug("GetSignTypePreference");

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_UTENTE");
            q.setParam("idUtente", idPeople);
            logger.Debug(q.getSQL());
            DataSet ds = new DataSet();

            try
            {
                if (this.ExecuteQuery(out ds, "signpreference", q.getSQL()))
                {
                    if (ds.Tables["signpreference"].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables["signpreference"].Rows[0];
                        string preference = (string)dr["CHA_TIPO_FIRMA"].ToString();

                        return preference;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nel caricamento della preferenza tipo firma per utente (" + idPeople + ")", e);
                return null;
            }
            return null;
        }

        public bool SetSignTypePreference(string idPeople, string chaPreference)
        {
            bool result = false;

            DataSet dataSet = new DataSet();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_SIGN_PREFERENCE");
                q.setParam("param1", idPeople);
                q.setParam("param2", chaPreference);
                string sql = q.getSQL();

                result = this.ExecuteNonQuery(sql);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
            }
            return result;
        }

        public DocsPaVO.utente.Corrispondente GetCorrispondenteByCodRubricaNotDisabled(string id_amm, string cod_rubrica, DocsPaVO.addressbook.TipoUtente tipoIE)
        {
            logger.Debug("getCorrispondente");

            string ie_where = "";
            switch (tipoIE)
            {
                case DocsPaVO.addressbook.TipoUtente.INTERNO:
                    ie_where = " and cha_tipo_ie='I'";
                    break;
                case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                    ie_where = " and cha_tipo_ie='E'";
                    break;
                case DocsPaVO.addressbook.TipoUtente.GLOBALE:
                    ie_where = "";
                    break;
            }

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
            q.setParam("param1", "SYSTEM_ID,CHA_TIPO_URP,ID_GRUPPO,ID_PEOPLE,CHA_TIPO_IE");
            q.setParam("param2", "UPPER(VAR_COD_RUBRICA)='" + cod_rubrica.ToUpper().Replace("'", "''") + "'" + ie_where + " AND ID_AMM=" + id_amm + " AND DTA_FINE IS  NULL ");
            logger.Debug(q.getSQL());
            DataSet ds = new DataSet();

            try
            {
                if (this.ExecuteQuery(out ds, "corrispondenti", q.getSQL()))
                {
                    DocsPaVO.utente.Corrispondente corrispondente = new DocsPaVO.utente.Corrispondente();
                    if (ds.Tables["corrispondenti"].Rows.Count >= 1)
                    {
                        DataRow dr = ds.Tables["corrispondenti"].Rows[0];
                        string sid = (string)dr["SYSTEM_ID"].ToString();
                        string tipo = (string)dr["CHA_TIPO_URP"].ToString();
                        string id_gruppo = (string)dr["ID_GRUPPO"].ToString();
                        string id_people = (string)dr["ID_PEOPLE"].ToString();

                        bool isInterno = (dr["CHA_TIPO_IE"].ToString() == "I");

                        DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                        DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                        qco.systemId = sid;
                        qco.idAmministrazione = id_amm;
                        qco.fineValidita = true;

                        if (isInterno)
                            return u.GetCorrispondenteInt(qco);
                        else
                            return u.GetCorrispondenteEst(qco);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella ricerca di uno specifico corrispondente (" + cod_rubrica + ")", e);
                return null;
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="parent"></param>
        /// <param name="risIns"></param>
        /// <returns></returns>
        public bool InsertCorrispondente(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.Corrispondente parent, string idAmm, ref DocsPaVO.utente.Corrispondente risIns)
        {
            bool result = true;

            logger.Debug("insertCorrispondente");
            DataSet dataSet = new DataSet();
            string insertPrefix = null;

            try
            {
                insertPrefix = (string)System.Configuration.ConfigurationManager.AppSettings["prefissoInsertCorr"];

                try
                {
                    string ammId = corr.idAmministrazione;
                    if (string.IsNullOrEmpty(ammId))
                        ammId = idAmm;
                    IsCodRubricaPresente(CorrectApici(corr.codiceRubrica), corr.tipoCorrispondente, ammId, corr.idRegistro, corr.inRubricaComune, out dataSet);

                    IsCodeCorrect(CorrectApici(corr.codiceRubrica));
                }
                catch (Exception e)
                {
                    risIns.errore = e.Message;
                    result = false;
                    return result;
                }

                //si esegue l'inserimento
                logger.Debug(corr.GetType().FullName);

                if (corr.GetType().Equals(typeof(DocsPaVO.utente.UnitaOrganizzativa)))
                {
                    if (string.IsNullOrEmpty(corr.oldDescrizione))
                    {
                        corr.oldDescrizione = corr.descrizione;
                    }
                    string sysId = string.Empty;
                    sysId = InsertCorr(corr, parent);

                    if (corr.codiceRubrica == null || corr.codiceRubrica.Equals(""))
                    {
                        corr.codiceRubrica = UpdateCodRubrica(sysId, insertPrefix + "U_", corr.idAmministrazione);

                        if (corr.codiceRubrica == null)
                        {
                            result = false;
                        }

                    }
                    if (((DocsPaVO.utente.UnitaOrganizzativa)corr).info != null)
                    {
                        DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();
                        DocsPaUtils.Data.TypedDataSetManager.MakeTyped(((DocsPaVO.utente.UnitaOrganizzativa)corr).info, dettagliCorrispondente.Corrispondente.DataSet);

                        if (!InsertDettagli(sysId, dettagliCorrispondente))
                        {
                            result = false;
                        }
                    }

                    //inserimento del canale preferenziale
                    if (corr.canalePref != null)
                    {
                        if (corr.tipoIE != "I" && string.IsNullOrEmpty(corr.canalePref.systemId))
                            corr.canalePref.systemId = this.GetSystemIDCanale(); //imposto lettera di default se vuoto
                        InsertCanalePref(sysId, corr.canalePref);
                    }

                    //Aggiorno le eventuali liste di distribuzione con l'id del nuovo corrispondente
                    if (!string.IsNullOrEmpty(sysId) && dataSet != null && dataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in dataSet.Tables[0].Rows)
                        {
                            string idCorrOld = row["SYSTEM_ID"].ToString();
                            string idCorrNew = sysId;

                            AggiornaListeDistribuzione(idCorrNew, idCorrOld);
                        }
                    }

                    corr.systemId = sysId;
                }

                if (corr.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
                {
                    string sysId = InsertRuolo(corr, parent);

                    if (corr.codiceRubrica == null || corr.codiceRubrica.Equals(""))
                    {
                        corr.codiceRubrica = UpdateCodRubrica(sysId, insertPrefix + "R_", corr.idAmministrazione);
                        if (corr.codiceRubrica == null)
                        {
                            result = false;
                        }
                    }

                    //inserimento del canale preferenziale
                    if (corr.canalePref != null)
                    {
                        if (corr.tipoIE != "I" && string.IsNullOrEmpty(corr.canalePref.systemId))
                            corr.canalePref.systemId = this.GetSystemIDCanale(); //imposto lettera di default se vuoto
                        InsertCanalePref(sysId, corr.canalePref);
                    }

                    //Aggiorno le eventuali liste di distribuzione con l'id del nuovo corrispondente
                    if (!string.IsNullOrEmpty(sysId) && dataSet != null && dataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in dataSet.Tables[0].Rows)
                        {
                            string idCorrOld = row["SYSTEM_ID"].ToString();
                            string idCorrNew = sysId;

                            AggiornaListeDistribuzione(idCorrNew, idCorrOld);
                        }
                    }

                    corr.systemId = sysId;
                }

                if (corr.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
                {
                    string sysId = InsertUtente(corr);

                    if (corr.codiceRubrica == null || corr.codiceRubrica.Equals(""))
                    {
                        corr.codiceRubrica = UpdateCodRubrica(sysId, insertPrefix + "U_", corr.idAmministrazione);
                        if (corr.codiceRubrica == null)
                        {
                            result = false;
                        }
                    }
                    if (((DocsPaVO.utente.Utente)corr).info != null)
                    {
                        DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();
                        DocsPaUtils.Data.TypedDataSetManager.MakeTyped(((DocsPaVO.utente.Utente)corr).info, dettagliCorrispondente.Corrispondente.DataSet);

                        if (!InsertDettagli(sysId, dettagliCorrispondente))
                        {
                            result = false;
                        }
                    }
                    //insert nella tabella dpa_ruoe_utente
                    if (parent != null && parent.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
                    {
                        InsertRUOEUtente(sysId, parent);
                    }
                    //inserimento del canale preferenziale
                    if (corr.canalePref != null)
                    {
                        if (corr.tipoIE != "I" && string.IsNullOrEmpty(corr.canalePref.systemId))
                            corr.canalePref.systemId = this.GetSystemIDCanale(); //imposto lettera di default se vuoto
                        InsertCanalePref(sysId, corr.canalePref);
                    }

                    //Aggiorno le eventuali liste di distribuzione con l'id del nuovo corrispondente
                    if (!string.IsNullOrEmpty(sysId) && dataSet != null && dataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in dataSet.Tables[0].Rows)
                        {
                            string idCorrOld = row["SYSTEM_ID"].ToString();
                            string idCorrNew = sysId;

                            AggiornaListeDistribuzione(idCorrNew, idCorrOld);
                        }
                    }

                    corr.systemId = sysId;
                }

                if (corr.GetType().Equals(typeof(DocsPaVO.utente.RaggruppamentoFunzionale)))
                {
                    if (!string.IsNullOrEmpty(corr.descrizione))
                    {
                        corr.oldDescrizione = corr.descrizione;
                    }
                    string sysId = string.Empty;
                    sysId = InsertCorr(corr, parent);

                    if (corr.codiceRubrica == null || corr.codiceRubrica.Equals(""))
                    {
                        corr.codiceRubrica = UpdateCodRubrica(sysId, insertPrefix + "RF_", corr.idAmministrazione);

                        if (corr.codiceRubrica == null)
                        {
                            result = false;
                        }

                    }

                    if (((DocsPaVO.utente.RaggruppamentoFunzionale)corr).info != null)
                    {
                        DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();
                        DocsPaUtils.Data.TypedDataSetManager.MakeTyped(((DocsPaVO.utente.RaggruppamentoFunzionale)corr).info, dettagliCorrispondente.Corrispondente.DataSet);

                        if (!InsertDettagli(sysId, dettagliCorrispondente))
                        {
                            result = false;
                        }
                    }

                    //inserimento del canale preferenziale
                    if (corr.canalePref != null)
                    {
                        if (corr.tipoIE != "I" && string.IsNullOrEmpty(corr.canalePref.systemId))
                            corr.canalePref.systemId = this.GetSystemIDCanale(); //imposto lettera di default se vuoto
                        InsertCanalePref(sysId, corr.canalePref);
                    }

                    //Aggiorno le eventuali liste di distribuzione con l'id del nuovo corrispondente
                    if (!string.IsNullOrEmpty(sysId) && dataSet != null && dataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in dataSet.Tables[0].Rows)
                        {
                            string idCorrOld = row["SYSTEM_ID"].ToString();
                            string idCorrNew = sysId;

                            AggiornaListeDistribuzione(idCorrNew, idCorrOld);
                        }
                    }
                    corr.systemId = sysId;
                }
               
                logger.Debug("Transazione effettuata");
            }
            catch (Exception e)
            {
                logger.Debug("Eccezione: " + e);

                result = false;
            }

            risIns = corr;

            return result;
        }

        private bool AggiornaListeDistribuzione(string idCorrNew, string idCorrOld)
        {
            bool result = true;
            try
            {
                DocsPaUtils.Query q;

                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_LISTE_DISTR");
                q.setParam("idCorrNew", idCorrNew);
                q.setParam("idCorrOld", idCorrOld);

                string query = q.getSQL();
                logger.Debug("AggiornaListeDistribuzione: " + query);
                int rows = 0;
                if (!ExecuteNonQuery(query, out rows))
                {
                    result = false;
                    throw new Exception("Errore durante l'aggiornamento delle liste di distribuzione: " + query);
                }
            }
            catch(Exception e)
            {
                logger.Error("Errore in AggiornaListeDistribuzione " + e.Message);
            }
            return result;
        }

        private void IsCodeCorrect(string codRubrica)
        {
            logger.Debug("isCodeCorrect");
            char[] separator = { ';' };
            String[] prefissi = System.Configuration.ConfigurationManager.AppSettings["prefissiRubricaRiservati"].Split(separator);
            for (int i = 0; i < prefissi.Length; i++)
            {
                if (codRubrica.ToUpper().StartsWith(prefissi[i]))
                {
                    logger.Debug("Il codice rubrica inserito inizia per " + prefissi[i]);

                    logger.Debug("Errore nella gestione delle trasmissioni (Query - IsCodCorrect)");
                    throw new Exception("Il codice rubrica inserito non può iniziare per " + prefissi[i]);
                }
            }
        }

        public string InsertCorr(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.Corrispondente parent)
        {
            string res = string.Empty;

            using (DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPACorrglobali");
                q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName() + " NUM_LIVELLO, CHA_TIPO_IE, ID_REGISTRO, ID_AMM, VAR_DESC_CORR, ID_OLD, DTA_INIZIO, ID_PARENT, VAR_CODICE, CHA_TIPO_CORR, CHA_TIPO_URP, CHA_PA, VAR_CODICE_AOO, VAR_COD_RUBRICA, CHA_DETTAGLI, VAR_EMAIL, VAR_CODICE_AMM, VAR_CODICE_ISTAT,VAR_DESC_CORR_OLD, InteropUrl");

                string param = DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("");
                //DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_CORR_GLOBALI");
                if (parent == null)
                {
                    param += "0,";
                }
                else
                {
                    int livello = Int32.Parse(((DocsPaVO.utente.UnitaOrganizzativa)parent).livello) + 1;
                    param += livello.ToString() + ",";
                }
                param += "'E',";


                if (corr.idRegistro == null || corr.idRegistro == string.Empty)
                {
                    param += " NULL ,";
                }
                else
                {
                    param += corr.idRegistro + ",";
                }

                if (!string.IsNullOrEmpty(corr.idAmministrazione))
                    param += corr.idAmministrazione + ",";
                else
                    param += " NULL, ";

                param += "'" + CorrectApici(corr.descrizione) + "',";
                param += "0,";
                param += DocsPaDbManagement.Functions.Functions.GetDate() + ",";   //DocsPaWS.Utils.dbControl.getDate()+",";
                if (parent == null)
                {
                    param += "0,";
                }
                else
                {
                    param += parent.systemId + ",";
                }
                if (!string.IsNullOrEmpty(corr.codiceCorrispondente))
                    param += "'" + CorrectApici(corr.codiceCorrispondente) + "',";
                else
                    param += "'" + CorrectApici(corr.codiceRubrica) + "',";

                if (corr.inRubricaComune)
                    // Se si sta inserendo un corrispondente proveniente da rubrica comune,
                    // viene impostato 'C' (comune) come valore per il campo tipo corrispondente
                    param += "'C',";
                else
                    param += "'S',";

                // Il tipo sarà "F" se il corrispondente, di rubrica comune, è un RF altrimenti sarà "U"
                //param += String.IsNullOrEmpty(corr.tipoCorrispondente) || corr.tipoCorrispondente != "RF" ? "'U'," : "'RF',";
                //***********************************
                if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                    param += String.Format("'{0}',", "U");
                else if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
                    param += String.Format("'{0}',", "R");
                else if (corr.GetType() == typeof(DocsPaVO.utente.Utente))
                    param += String.Format("'{0}',", "P");
                else if (corr.GetType() == typeof(DocsPaVO.utente.RaggruppamentoFunzionale))
                    param += String.Format("'{0}',", "F");
                param += "'1',";
                //param += "'" + ((DocsPaVO.utente.UnitaOrganizzativa)corr).codiceAOO + "',";
                param += "'" + corr.codiceAOO + "',";
                if (corr.codiceRubrica != null && !corr.codiceRubrica.Equals(""))
                {
                    param += "'" + CorrectApici(corr.codiceRubrica) + "',";
                }
                else
                {
                    param += "'',";
                }
                if (corr.dettagli)
                {
                    param += "'1',";
                }
                else
                {
                    param += "'0',";
                }

                //param += "'" + CorrectApici(((DocsPaVO.utente.UnitaOrganizzativa)corr).email) + "',";
                //param += "'" + CorrectApici(((DocsPaVO.utente.UnitaOrganizzativa)corr).codiceAmm) + "',";
                //param += "'" + CorrectApici(((DocsPaVO.utente.UnitaOrganizzativa)corr).codiceIstat) + "'";
                param += "'" + CorrectApici(corr.email) + "',";
                param += "'" + CorrectApici(corr.codiceAmm) + "',";

                if (corr is RaggruppamentoFunzionale)
                    param += "''";
                else
                    param += "'" + CorrectApici(((DocsPaVO.utente.UnitaOrganizzativa)corr).codiceIstat) + "'";

                if (!string.IsNullOrEmpty(corr.oldDescrizione))
                {
                    param += ",'" + CorrectApici(corr.oldDescrizione) + "'";

                }
                else
                {
                    param += ",null";
                }

                // Salvataggio dell'URL se valorizzato
                param += corr.Url != null && corr.Url.Count > 0 && !String.IsNullOrEmpty(corr.Url[0].Url) ? String.Format(",'{0}'", corr.Url[0].Url) : ",null";

                //string[] splitted = corr.note.Split('#');
                //if (splitted.Length > 1)
                //    param += ",'" + splitted[1] + "'";
                //else
                //{
                //    param += ",NULL";
                //}
                logger.Debug("param = " + param);

                q.setParam("param2", param);
                string sql = q.getSQL();
                logger.Debug(sql);

                int rowsAffected;
                dbProvider.ExecuteNonQuery(sql, out rowsAffected);

                if (rowsAffected > 0)
                {
                    // Reperimento id del corrispondente appena inserito
                    sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted();
                    logger.Debug(sql);

                    dbProvider.ExecuteScalar(out res, sql);
                }
            }

            return res;
        }

        /*private string correctApici(string str)
        {
            if(str!=null)
            {
                return str.Replace("'","''");
            }
            else
            {
                return str;
            }
        }*/


        public void DettagliCorrispondenti(out DataSet dataSet, DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPADettGlob5");
            q.setParam("param1", "ID_CORR_GLOBALI=" + objQueryCorrispondente.systemId);
            //string commandString1="SELECT * FROM DPA_DETT_GLOBALI WHERE ID_CORR_GLOBALI="+objQueryCorrispondente.systemId;
            string sql = q.getSQL();
            logger.Debug(sql);

            //database.fillTable(commandString1,dataSet,"DETTAGLI");
            this.ExecuteQuery(out dataSet, "DETTAGLI", sql);
        }

        private string GetQueryChildrenGruppo(string codiceRubrica)
        {
            string sql = "";
            DocsPaUtils.Query q;
            try
            {
                DataSet dataSet;
                //string parentString="SELECT CHA_TIPO_URP, CHA_TIPO_IE FROM DPA_CORR_GLOBALI A WHERE VAR_COD_RUBRICA='"+codiceRubrica+"' AND CHA_TIPO_CORR='G'"; 
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                q.setParam("param1", "A.CHA_TIPO_URP, A.CHA_TIPO_IE");
                q.setParam("param2", "UPPER(A.VAR_COD_RUBRICA)='" + codiceRubrica.ToUpper() + "' AND A.CHA_TIPO_CORR='G'");
                sql = q.getSQL();

                //database.fillTable(parentString, dataSet, "PARENT");
                this.ExecuteQuery(out dataSet, "PARENT", sql);
                if (dataSet.Tables["PARENT"].Rows.Count == 0)
                {
                    return sql;
                }
                if (dataSet.Tables["PARENT"].Rows[0]["CHA_TIPO_URP"].ToString().Equals("P"))
                {
                    //il gruppo contiene utenti
                    if (dataSet.Tables["PARENT"].Rows[0]["CHA_TIPO_IE"].ToString().Equals("E"))
                    {
                        /*queryString="SELECT A.SYSTEM_ID, A.CHA_TIPO_IE, A.ID_REGISTRO, A.VAR_EMAIL, A.VAR_COGNOME, A.VAR_NOME, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP FROM  DPA_CORR_GLOBALI A,DPA_CORR_GRUPPO B, DPA_CORR_GLOBALI C ";
                        queryString=queryString+" WHERE A.SYSTEM_ID=B.ID_COMP_GRUPPO AND C.SYSTEM_ID=B.ID_GRUPPO AND C.VAR_COD_RUBRICA='"+codiceRubrica+"'";*/
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__CORR_GRUPPO__CORR_GLOBALI");
                        q.setParam("param1", "A.SYSTEM_ID, A.CHA_TIPO_IE, A.ID_REGISTRO, A.VAR_EMAIL, A.VAR_COGNOME, A.VAR_NOME, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP");
                        q.setParam("param2", "UPPER(C.VAR_COD_RUBRICA)='" + codiceRubrica.ToUpper() + "'");
                        sql = q.getSQL();
                    }
                    else
                    {
                        /*queryString="SELECT A.SYSTEM_ID, A.ID_PEOPLE, A.ID_REGISTRO, A.CHA_TIPO_IE, E.VAR_COGNOME, E.VAR_NOME, E.EMAIL_ADDRESS, E.CHA_NOTIFICA, E.VAR_TELEFONO, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP";
                        queryString=queryString+" FROM DPA_CORR_GLOBALI A , DPA_CORR_GLOBALI C, PEOPLE E, DPA_CORR_GRUPPO F WHERE A.ID_PEOPLE = E.SYSTEM_ID AND  A.SYSTEM_ID = F.ID_COMP_GRUPPO";
                        queryString=queryString+" AND F.ID_GRUPPO = C.SYSTEM_ID AND C.CHA_TIPO_CORR = 'G' AND C.VAR_COD_RUBRICA = '"+codiceRubrica+"'";*/
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__CORR_GLOBALI__PEOPLE__CORR_GRUPPO");
                        q.setParam("param1", "A.SYSTEM_ID, A.ID_PEOPLE, A.ID_REGISTRO, A.CHA_TIPO_IE, E.VAR_COGNOME, E.VAR_NOME, E.EMAIL_ADDRESS, E.CHA_NOTIFICA, E.VAR_TELEFONO, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP");
                        q.setParam("param2", " C.CHA_TIPO_CORR = 'G' AND UPPER(C.VAR_COD_RUBRICA) = '" + codiceRubrica.ToUpper() + "'");
                        sql = q.getSQL();
                    }
                }
                else
                {
                    //il gruppo contiene UO
                    /*queryString="SELECT A.SYSTEM_ID, A.CHA_TIPO_URP, A.ID_REGISTRO, A.NUM_LIVELLO, A.VAR_EMAIL, A.CHA_PA, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_AMM FROM DPA_CORR_GLOBALI A, DPA_CORR_GRUPPO B, DPA_CORR_GLOBALI C";
                    queryString=queryString+" WHERE A.SYSTEM_ID=B.ID_COMP_GRUPPO AND C.SYSTEM_ID=B.ID_GRUPPO AND C.VAR_COD_RUBRICA='"+codiceRubrica+"' AND C.CHA_TIPO_CORR='G'";*/
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__CORR_GRUPPO__CORR_GLOBALI");
                    q.setParam("param1", "A.SYSTEM_ID, A.CHA_TIPO_URP, A.ID_REGISTRO, A.NUM_LIVELLO, A.VAR_EMAIL, A.CHA_PA, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_AMM");
                    q.setParam("param2", "C.VAR_COD_RUBRICA='" + codiceRubrica + "' AND C.CHA_TIPO_CORR='G'");
                    sql = q.getSQL();

                }
            }
            catch (Exception)
            {
                //throw e;
                sql = null;
                return sql;
            }
            return sql;
        }

        //elisa 
        public DataSet GetRuoliUtenteInterno(string codiceRubrica)
        {

            DataSet dataSet = new DataSet();
            try
            {
                logger.Debug("start > GetRuoliUtenteInterno");

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Ruoli_Utente_Interno");
                //				q.setParam("param1", "e.var_cognome, r.system_id AS ruolo_system_id,b.var_desc_ruolo AS ruolo_desc, r.var_codice AS ruolo_codice,r.var_cod_rubrica AS ruolo_cod_rubrica,U.VAR_DESC_CORR as descrizioneUO,pg.cha_preferito as cha_preferito");
                q.setParam("param1", "e.var_cognome, r.system_id AS ruolo_system_id,r.var_desc_corr AS ruolo_desc, r.var_codice AS ruolo_codice,r.var_cod_rubrica AS ruolo_cod_rubrica,U.VAR_DESC_CORR as descrizioneUO,pg.cha_preferito as cha_preferito");
                q.setParam("param2", " UPPER(p.var_cod_rubrica)='" + codiceRubrica.ToUpper() + "'");
                q.setParam("param3", "ORDER BY cha_preferito desc");
                string sql = q.getSQL();
                logger.Debug(sql);

                this.ExecuteQuery(dataSet, "RUOLI", sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataSet;
        }

        public ArrayList GetCorrGruppi(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            logger.Debug("getCorrGruppiMethod");
            //DocsPaWS.Utils.Database database=DocsPaWS.Utils.dbControl.getDatabase();
            DocsPaVO.addressbook.QueryCorrispondente objQuery = CorrectApiciQuery(qco);
            ArrayList result = new ArrayList();
            System.Collections.ArrayList lista = new System.Collections.ArrayList();

            //database.openConnection();
            this.OpenConnection();

            DataSet dataSet = new DataSet();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");

                string queryString;
                if (objQuery.codiceRubrica != null)
                {
                    if (objQuery.getChildren)
                    {
                        queryString = GetQueryChildrenGruppo(objQuery.codiceRubrica);
                        if (queryString == null)
                        {
                            result = null;
                        }
                    }
                    else
                    {
                        //queryString="SELECT SYSTEM_ID, ID_REGISTRO, VAR_EMAIL, CHA_PA, VAR_DESC_CORR, VAR_CODICE, VAR_COD_RUBRICA, VAR_SMTP, NUM_PORTA_SMTP, ID_AMM FROM DPA_CORR_GLOBALI A WHERE CHA_TIPO_CORR='G' AND VAR_COD_RUBRICA='"+objQuery.codiceRubrica+"'";
                        q.setParam("param1", "A.SYSTEM_ID, A.ID_REGISTRO, A.VAR_EMAIL, A.CHA_PA, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_AMM, A.VAR_CODICE_AOO, A.VAR_CODICE_AMM");
                        q.setParam("param2", "A.CHA_TIPO_CORR='G' AND UPPER(A.VAR_COD_RUBRICA)='" + objQuery.codiceRubrica.ToUpper() + "'");
                        queryString = q.getSQL();
                    }
                }
                else
                {
                    q.setParam("param1", "A.SYSTEM_ID, A.ID_REGISTRO, A.VAR_EMAIL, A.NUM_LIVELLO, A.CHA_PA, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_AMM, A.VAR_CODICE_A00, A.VAR_CODICE_AMM");
                    string param = "A.CHA_TIPO_CORR='G' ";
                    if (objQuery.codiceGruppo != null)
                    {
                        param = param + " AND A.VAR_CODICE='" + objQuery.codiceGruppo + "'";
                    }
                    else
                    {
                        param = param + " AND UPPER(A.VAR_DESC_CORR) LIKE '%" + objQuery.descrizioneGruppo.ToUpper().Replace("'", "''") + "%'";
                    }
                    if (objQuery.tipoUtente == DocsPaVO.addressbook.TipoUtente.ESTERNO)
                    {
                        param = param + " AND A.CHA_TIPO_IE='E'";
                    }
                    if (objQuery.tipoUtente == DocsPaVO.addressbook.TipoUtente.INTERNO)
                    {
                        param = param + " AND A.CHA_TIPO_IE='I'";
                    }

                    if (objQuery.fineValidita)
                        param = param + " AND A.DTA_FINE IS NULL";

                    q.setParam("param2", param);
                    queryString = q.getSQL();
                }
                logger.Debug(queryString);

                //database.fillTable(queryString,dataSet,"CORR_GRUPPI");
                this.ExecuteQuery(out dataSet, "CORR_GRUPPI", queryString);
                if (objQuery.codiceRubrica != null && objQuery.getChildren)
                {
                    //nel caso in cui si ottengono gli appartenenti al gruppo, che sono utente o UO
                    foreach (DataRow dr in dataSet.Tables["CORR_GRUPPI"].Rows)
                    {
                        if (dr["CHA_TIPO_URP"].Equals("U"))
                        {
                            DocsPaVO.utente.UnitaOrganizzativa uo = new DocsPaVO.utente.UnitaOrganizzativa();
                            uo.codiceCorrispondente = dr["VAR_CODICE"].ToString();
                            uo.codiceRubrica = dr["VAR_COD_RUBRICA"].ToString();
                            uo.descrizione = dr["VAR_DESC_CORR"].ToString();
                            uo.idAmministrazione = dr["ID_AMM"].ToString();
                            if (dr["ID_REGISTRO"] != null)
                            {
                                uo.idRegistro = dr["ID_REGISTRO"].ToString();
                            }
                            uo.email = dr["VAR_EMAIL"].ToString();
                            uo.interoperante = FromCharToBool(dr["CHA_PA"].ToString());
                            uo.systemId = dr["SYSTEM_ID"].ToString();
                            DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                            sp.serverSMTP = dr["VAR_SMTP"].ToString();
                            sp.portaSMTP = dr["NUM_PORTA_SMTP"].ToString();
                            uo.serverPosta = sp;
                            uo.livello = dr["NUM_LIVELLO"].ToString();
                            lista.Add(uo);
                        }
                        if (dr["CHA_TIPO_URP"].Equals("P"))
                        {
                            DocsPaVO.utente.Utente ut = new DocsPaVO.utente.Utente();
                            if (dr["CHA_TIPO_IE"].Equals("E"))
                            {
                                ut.systemId = dr["SYSTEM_ID"].ToString();
                                ut.descrizione = dr["VAR_COGNOME"].ToString() + " " + dr["VAR_NOME"].ToString();
                                ut.codiceCorrispondente = dr["VAR_CODICE"].ToString();
                                ut.codiceRubrica = dr["VAR_COD_RUBRICA"].ToString();
                                ut.dettagli = FromCharToBool(dr["CHA_DETTAGLI"].ToString());
                                if (dr["ID_REGISTRO"] != null)
                                {
                                    ut.idRegistro = dr["ID_REGISTRO"].ToString();
                                }
                                ut.email = dr["VAR_EMAIL"].ToString();
                            }
                            else
                            {
                                ut.systemId = dr["SYSTEM_ID"].ToString();
                                ut.idPeople = dr["ID_PEOPLE"].ToString();
                                if (dr["ID_REGISTRO"] != null)
                                {
                                    ut.idRegistro = dr["ID_REGISTRO"].ToString();
                                }
                                ut.descrizione = dr["VAR_COGNOME"].ToString() + " " + dr["VAR_NOME"].ToString();
                                ut.codiceCorrispondente = dr["VAR_CODICE"].ToString();
                                ut.codiceRubrica = dr["VAR_COD_RUBRICA"].ToString();
                                ut.dettagli = FromCharToBool(dr["CHA_DETTAGLI"].ToString());
                                ut.email = dr["EMAIL_ADDRESS"].ToString();
                                ut.notifica = dr["CHA_NOTIFICA"].ToString();
                                ut.telefono = dr["VAR_TELEFONO"].ToString();
                            }
                            lista.Add(ut);
                        }
                    }
                }
                else
                {
                    foreach (DataRow dr in dataSet.Tables["CORR_GRUPPI"].Rows)
                    {
                        DocsPaVO.utente.Corrispondente gruppo = new DocsPaVO.utente.Corrispondente();
                        gruppo.codiceCorrispondente = dr["VAR_CODICE"].ToString();
                        gruppo.codiceRubrica = dr["VAR_COD_RUBRICA"].ToString();
                        gruppo.descrizione = dr["VAR_DESC_CORR"].ToString();
                        gruppo.idAmministrazione = dr["ID_AMM"].ToString();
                        gruppo.systemId = dr["SYSTEM_ID"].ToString();
                        if (dr["ID_REGISTRO"] != null)
                        {
                            gruppo.idRegistro = dr["ID_REGISTRO"].ToString();
                        }
                        lista.Add(gruppo);
                    }
                }
                result = lista;

                //database.closeConnection();
                this.CloseConnection();
            }
            catch (Exception e)
            {
                logger.Debug("eccezione: " + e);
                //database.closeConnection();
                this.CloseConnection();
                //throw e;
                result = null;
            }
            return result;
        }

        public void GetRootUO(out DataSet dataSet, DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente)
        {
            string generalCondition = "((ID_AMM IS NULL) OR (ID_REGISTRO IS NULL AND ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')";
            generalCondition = generalCondition + RegistriCondition(objQueryCorrispondente, null) + ") ";
            //string queryUO="SELECT * FROM DPA_CORR_GLOBALI A WHERE A.DTA_FINE IS NULL AND CHA_TIPO_URP='U' AND NUM_LIVELLO='0' ";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
            q.setParam("param1", "*");
            string param = "A.DTA_FINE IS NULL AND A.CHA_TIPO_URP='U' AND A.NUM_LIVELLO='0' ";

            if (objQueryCorrispondente.tipoUtente == DocsPaVO.addressbook.TipoUtente.ESTERNO)
            {
                param = param + " AND A.CHA_TIPO_IE='E'";
            }
            if (objQueryCorrispondente.tipoUtente == DocsPaVO.addressbook.TipoUtente.INTERNO)
            {
                param = param + " AND A.CHA_TIPO_IE='I'";
            }
            param = param + " AND " + generalCondition;
            q.setParam("param2", param);
            string sql = q.getSQL();
            logger.Debug(sql);

            //database.fillTable(queryUO,ds,"UO");
            this.ExecuteQuery(out dataSet, "UO", sql);
        }

        public ArrayList ListaUtentiSciolti(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            //DocsPaWS.Utils.Database database=DocsPaWS.Utils.dbControl.getDatabase();
            System.Data.DataSet dataSet = new System.Data.DataSet();
            System.Collections.ArrayList listaCorr = new System.Collections.ArrayList();
            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);
            try
            {

                if (objQueryCorrispondente.isUODefined() || objQueryCorrispondente.isRuoloDefined())
                {
                    return listaCorr;
                }

                string commandString1;
                DocsPaUtils.Query q;
                string sql;

                string generalCondition = "((A.ID_AMM IS NULL) OR (A.ID_REGISTRO IS NULL AND A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')";

                generalCondition = generalCondition + RegistriCondition(objQueryCorrispondente, null) + ") ";
                logger.Debug("generalCondition=" + generalCondition);

                if (objQueryCorrispondente.codiceRubrica != null)
                {
                    string generalConditionCR = " ((A.ID_AMM IS NULL) OR (A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "'))";

                    if (objQueryCorrispondente.fineValidita)
                        generalConditionCR += " AND A.DTA_FINE IS NULL";
                    //if (RegistriCondition(objQueryCorrispondente, null) != "")
                    //{
                    //    generalCondition = generalCondition + " AND (ID_REGISTRO IS NULL " + RegistriCondition(objQueryCorrispondente, null) + ") ";
                    //}
                    //string parentString="SELECT * FROM DPA_CORR_GLOBALI A WHERE VAR_COD_RUBRICA='"+objQueryCorrispondente.codiceRubrica+"' and cha_tipo_ie='E' AND CHA_TIPO_CORR='S' AND "+generalConditionCR;
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                    q.setParam("param1", "*");
                    q.setParam("param2", "UPPER(A.VAR_COD_RUBRICA)='" + objQueryCorrispondente.codiceRubrica.ToUpper() + "' and A.cha_tipo_ie='E' AND A.CHA_TIPO_CORR='S' AND " + generalConditionCR);
                    sql = q.getSQL();
                    logger.Debug("sql=" + sql);

                    //db.fillTable(parentString,dataSet,"PARENT");
                    this.ExecuteQuery(out dataSet, "PARENT", sql);

                    //nel caso non esiste oggetto parent con tale codice rubrica
                    if (dataSet.Tables["PARENT"].Rows.Count == 0)
                    {
                        return listaCorr;
                    }

                    if (objQueryCorrispondente.getChildren == false)
                    {
                        if (dataSet.Tables["PARENT"].Rows[0]["CHA_TIPO_URP"].Equals("P"))
                        {
                            //è un utente:
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                            q.setParam("param1", "A.SYSTEM_ID, A.VAR_COGNOME, A.VAR_NOME, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP, A.ID_REGISTRO, A.VAR_EMAIL, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.VAR_CODICE_AMM, A.VAR_CODICE_AOO");
                            commandString1 = q.getSQL() + "UPPER(A.VAR_COD_RUBRICA)='" + objQueryCorrispondente.codiceRubrica.ToUpper() + "'" +
                                " AND ((A.ID_AMM IS NULL) OR (A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')) AND A.CHA_TIPO_CORR='S'";
                            //if (RegistriCondition(objQueryCorrispondente, "A") != "")
                            //{
                            //    commandString1 = commandString1 + " AND (ID_REGISTRO IS NULL " + RegistriCondition(objQueryCorrispondente, "A") + ") ";
                            //}
                        }
                        else
                        {
                            return listaCorr;
                        }
                    }
                    else
                    {
                        //non ci possono essere figli
                        return listaCorr;
                    }

                }
                else
                {
                    //la query viene fatta in base all'UO, al ruolo e all'utente
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                    q.setParam("param1", "*");
                    q.setParam("param2", generalCondition);
                    //string commandString_begin="SELECT * FROM DPA_CORR_GLOBALI A WHERE "+generalCondition;
                    string commandString_begin = q.getSQL();
                    logger.Debug("commandString_begin=" + commandString_begin);

                    string commandString_utente = null;

                    string generalCondition_utente = "((A.ID_AMM IS NULL) OR (A.ID_REGISTRO IS NULL AND A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')" + RegistriCondition(objQueryCorrispondente, "A") + ")";

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                    q.setParam("param1", "distinct A.SYSTEM_ID, A.VAR_NOME, A.VAR_COGNOME, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP, A.ID_REGISTRO, A.VAR_EMAIL, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.VAR_CODICE_AOO, A.VAR_CODICE_AMM");
                    q.setParam("param2", generalCondition_utente);
                    //string commandString_utente_begin="SELECT A.SYSTEM_ID, A.VAR_NOME, A.VAR_COGNOME, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP, A.ID_REGISTRO, A.VAR_EMAIL, A.VAR_SMTP, A.NUM_PORTA_SMTP FROM  DPA_CORR_GLOBALI A WHERE "+generalCondition_utente;
                    string commandString_utente_begin = q.getSQL();
                    logger.Debug("commandString_utente_begin=" + commandString_utente_begin);

                    if (objQueryCorrispondente.nomeUtente != null)
                    {
                        //Celeste
                        //commandString_utente=commandString_utente_begin+" AND UPPER(A.VAR_NOME) LIKE '%"+objQueryCorrispondente.nomeUtente.ToUpper()+"%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='E' AND A.CHA_TIPO_CORR='S'";
                        commandString_utente = commandString_utente_begin + " AND UPPER(A.VAR_NOME) LIKE '" + objQueryCorrispondente.nomeUtente.ToUpper() + "%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='E' AND A.CHA_TIPO_CORR='S'";
                    }
                    else
                    {
                        //Celeste
                        //commandString_utente=commandString_utente_begin+" AND UPPER(A.VAR_COGNOME) LIKE '%"+objQueryCorrispondente.cognomeUtente.ToUpper()+"%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='E' AND A.CHA_TIPO_CORR='S'";
                        commandString_utente = commandString_utente_begin + " AND UPPER(A.VAR_COGNOME) LIKE '" + objQueryCorrispondente.cognomeUtente.ToUpper() + "%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='E' AND A.CHA_TIPO_CORR='S'";
                    }

                    commandString1 = commandString_utente;
                }

                //Fabio
                if (objQueryCorrispondente.fineValidita)
                    commandString1 += " AND A.DTA_FINE IS NULL";

                logger.Debug("commandString1=" + commandString1);

                //database.fillTable(commandString1,dataSet,"CORRISPONDENTI");
                this.ExecuteQuery(out dataSet, "CORRISPONDENTI", commandString1);

                foreach (DataRow corrRow in dataSet.Tables["CORRISPONDENTI"].Rows)
                {
                    if (corrRow["CHA_TIPO_URP"].ToString().Equals("P"))
                    {
                        DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();
                        corrispondenteUtente.systemId = corrRow["SYSTEM_ID"].ToString();
                        corrispondenteUtente.descrizione = corrRow["VAR_COGNOME"].ToString() + " " + corrRow["VAR_NOME"].ToString();
                        corrispondenteUtente.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                        corrispondenteUtente.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                        corrispondenteUtente.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                        DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                        sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                        corrispondenteUtente.serverPosta = sp;
                        corrispondenteUtente.tipoIE = "E";
                        if (corrRow["ID_REGISTRO"] != null)
                        {
                            corrispondenteUtente.idRegistro = corrRow["ID_REGISTRO"].ToString();
                        }
                        corrispondenteUtente.email = corrRow["VAR_EMAIL"].ToString();
                        corrispondenteUtente.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                        corrispondenteUtente.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();

                        listaCorr.Add(corrispondenteUtente);
                    }
                }
            }
            catch (Exception)
            {
                logger.Debug("error: listaUtentiScioltiMethod");

                //database.closeConnection();
                //this.CloseConnection();
                //throw e;
                listaCorr = null;
            }
            return listaCorr;
        }

        public void RicercaUOParentInt(DataSet dataSet, DataRow corrRow)
        {
            string sql = "";
            DocsPaUtils.Query q = null;

            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "USA_CONNECTBYPRIOR_OR_WITH")) && (DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "USA_CONNECTBYPRIOR_OR_WITH")).Equals("1"))
            {
                if (corrRow["CHA_TIPO_URP"].ToString().Equals("U"))
                {
                    //commandString2="SELECT SYSTEM_ID, VAR_EMAIL, CHA_PA, ID_REGISTRO, VAR_DESC_CORR, NUM_LIVELLO, CHA_DETTAGLI, VAR_CODICE, VAR_COD_RUBRICA, VAR_CODICE_AMM, VAR_CODICE_ISTAT, VAR_CODICE_AOO, VAR_SMTP, NUM_PORTA_SMTP, ID_PARENT FROM DPA_CORR_GLOBALI A WHERE CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND NUM_LIVELLO <="+corrRow["NUM_LIVELLO"].ToString();
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                    q.setParam("param1", "A.SYSTEM_ID, A.ID_AMM, A.VAR_EMAIL, A.CHA_PA, A.ID_REGISTRO, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT ");
                    // q.setParam("param2", "A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND A.NUM_LIVELLO <=" + corrRow["NUM_LIVELLO"].ToString() + " AND A.ID_AMM = " + corrRow["ID_AMM"].ToString() + " ORDER BY A.VAR_DESC_CORR ASC");


                    if (dbType.ToUpper().Equals("ORACLE"))
                        q.setParam("param2", "A.cha_tipo_urp = 'U' AND A.cha_tipo_ie = 'I'   AND A.id_amm = " + corrRow["ID_AMM"].ToString() + " connect by prior A.id_parent=a.system_id start with A.system_id = " + corrRow["SYSTEM_ID"].ToString() + "  ORDER BY A.var_desc_corr ASC");
                    else if (dbType.ToUpper().Equals("SQL"))
                    {
                        q.setParam("param2", "A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND A.NUM_LIVELLO <=" + corrRow["NUM_LIVELLO"].ToString() + " AND A.ID_AMM = " + corrRow["ID_AMM"].ToString() + " ORDER BY A.VAR_DESC_CORR ASC");


                    }
                    sql = q.getSQL();
                }
                if (corrRow["CHA_TIPO_URP"].ToString().Equals("R"))
                {
                    //commandString2="SELECT A.SYSTEM_ID, A.VAR_EMAIL, A.CHA_PA, A.ID_REGISTRO, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT FROM DPA_CORR_GLOBALI A, DPA_CORR_GLOBALI B WHERE A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND B.SYSTEM_ID='"+corrRow["ID_UO"].ToString()+"' AND A.NUM_LIVELLO <= B.NUM_LIVELLO";
                    //old  q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__CORR_GLOBALI");

                    // old  q.setParam("param2", "A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND B.SYSTEM_ID='" + corrRow["ID_UO"].ToString() + "' AND A.NUM_LIVELLO <= B.NUM_LIVELLO" + " AND A.ID_AMM = " + corrRow["ID_AMM"].ToString() + " ORDER BY A.VAR_DESC_CORR ASC");


                    if (dbType.ToUpper().Equals("ORACLE"))
                    {
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");

                        q.setParam("param1", "A.SYSTEM_ID, A.ID_AMM, A.VAR_EMAIL, A.CHA_PA, A.ID_REGISTRO, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT");

                        q.setParam("param2", "A.cha_tipo_urp = 'U' AND A.cha_tipo_ie = 'I'   AND A.id_amm = " + corrRow["ID_AMM"].ToString() + " connect by prior A.id_parent=a.system_id start with A.system_id = " + corrRow["ID_UO"].ToString() + "  ORDER BY A.var_desc_corr ASC");
                    }
                    else if (dbType.ToUpper().Equals("SQL"))
                    {
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__CORR_GLOBALI");
                        q.setParam("param1", "A.SYSTEM_ID, A.ID_AMM, A.VAR_EMAIL, A.CHA_PA, A.ID_REGISTRO, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT");

                        //per adesso è come la precedente dopo l'ottimizzaremo con la clausola WITH
                        q.setParam("param2", "A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND B.SYSTEM_ID='" + corrRow["ID_UO"].ToString() + "' AND A.NUM_LIVELLO <= B.NUM_LIVELLO" + " AND A.ID_AMM = " + corrRow["ID_AMM"].ToString() + " ORDER BY A.VAR_DESC_CORR ASC");
                    }
                    sql = q.getSQL();
                }
                if (corrRow["CHA_TIPO_URP"].ToString().Equals("P"))
                {
                    //old
                    // //commandString2="SELECT A.SYSTEM_ID, A.VAR_EMAIL, A.CHA_PA, A.ID_REGISTRO, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT FROM DPA_CORR_GLOBALI A, DPA_CORR_GLOBALI B WHERE A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND B.SYSTEM_ID='"+corrRow["RUOLO_ID_UO"].ToString()+"' AND A.NUM_LIVELLO <= B.NUM_LIVELLO";
                    //q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__CORR_GLOBALI");
                    //// q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                    // q.setParam("param1", "A.SYSTEM_ID, A.ID_AMM, A.VAR_EMAIL, A.CHA_PA, A.ID_REGISTRO, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT");
                    // q.setParam("param2", "A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND B.SYSTEM_ID=" + corrRow["RUOLO_ID_UO"].ToString() + " AND A.NUM_LIVELLO <= B.NUM_LIVELLO" + " AND A.ID_AMM = " + corrRow["ID_AMM"].ToString() + " ORDER BY A.VAR_DESC_CORR ASC");

                    // sql = q.getSQL();

                    //commandString2="SELECT SYSTEM_ID, VAR_EMAIL, CHA_PA, ID_REGISTRO, VAR_DESC_CORR, NUM_LIVELLO, CHA_DETTAGLI, VAR_CODICE, VAR_COD_RUBRICA, VAR_CODICE_AMM, VAR_CODICE_ISTAT, VAR_CODICE_AOO, VAR_SMTP, NUM_PORTA_SMTP, ID_PARENT FROM DPA_CORR_GLOBALI A WHERE CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND NUM_LIVELLO <="+corrRow["NUM_LIVELLO"].ToString();
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                    q.setParam("param1", "A.SYSTEM_ID, A.ID_AMM, A.VAR_EMAIL, A.CHA_PA, A.ID_REGISTRO, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT ");
                    // q.setParam("param2", "A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND A.NUM_LIVELLO <=" + corrRow["NUM_LIVELLO"].ToString() + " AND A.ID_AMM = " + corrRow["ID_AMM"].ToString() + " ORDER BY A.VAR_DESC_CORR ASC");


                    if (dbType.ToUpper().Equals("ORACLE"))
                        q.setParam("param2", "A.cha_tipo_urp = 'U' AND A.cha_tipo_ie = 'I'   AND A.id_amm = " + corrRow["ID_AMM"].ToString() + " connect by prior A.id_parent=a.system_id start with A.system_id = " + corrRow["RUOLO_ID_UO"].ToString() + "  ORDER BY A.var_desc_corr ASC");
                    else if (dbType.ToUpper().Equals("SQL"))
                    {
                        //  q.setParam("param2", "A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND B.SYSTEM_ID=" + corrRow["RUOLO_ID_UO"].ToString() + " AND A.NUM_LIVELLO <=B.NUM_LIVELLO AND A.ID_AMM = " + corrRow["ID_AMM"].ToString() + " ORDER BY A.VAR_DESC_CORR ASC");
                        q.setParam("param2", "A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND A.NUM_LIVELLO <= (select B.NUM_LIVELLO FROM DPA_CORR_GLOBALI B where B.SYSTEM_ID=" + corrRow["RUOLO_ID_UO"].ToString() + ") AND A.ID_AMM = " + corrRow["ID_AMM"].ToString() + " ORDER BY A.VAR_DESC_CORR ASC");

                    }
                    sql = q.getSQL();
                }


            }
            else
            {
                if (corrRow["CHA_TIPO_URP"].ToString().Equals("U"))
                {
                    //commandString2="SELECT SYSTEM_ID, VAR_EMAIL, CHA_PA, ID_REGISTRO, VAR_DESC_CORR, NUM_LIVELLO, CHA_DETTAGLI, VAR_CODICE, VAR_COD_RUBRICA, VAR_CODICE_AMM, VAR_CODICE_ISTAT, VAR_CODICE_AOO, VAR_SMTP, NUM_PORTA_SMTP, ID_PARENT FROM DPA_CORR_GLOBALI A WHERE CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND NUM_LIVELLO <="+corrRow["NUM_LIVELLO"].ToString();
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                    q.setParam("param1", "A.SYSTEM_ID, A.ID_AMM, A.VAR_EMAIL, A.CHA_PA, A.ID_REGISTRO, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT ");
                    q.setParam("param2", "A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND A.NUM_LIVELLO <=" + corrRow["NUM_LIVELLO"].ToString() + " AND A.ID_AMM = " + corrRow["ID_AMM"].ToString() + " ORDER BY A.VAR_DESC_CORR ASC");
                    sql = q.getSQL();
                }
                if (corrRow["CHA_TIPO_URP"].ToString().Equals("R"))
                {
                    //commandString2="SELECT A.SYSTEM_ID, A.VAR_EMAIL, A.CHA_PA, A.ID_REGISTRO, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT FROM DPA_CORR_GLOBALI A, DPA_CORR_GLOBALI B WHERE A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND B.SYSTEM_ID='"+corrRow["ID_UO"].ToString()+"' AND A.NUM_LIVELLO <= B.NUM_LIVELLO";
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__CORR_GLOBALI");

                    q.setParam("param1", "A.SYSTEM_ID, A.ID_AMM, A.VAR_EMAIL, A.CHA_PA, A.ID_REGISTRO, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT");
                    q.setParam("param2", "A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND B.SYSTEM_ID='" + corrRow["ID_UO"].ToString() + "' AND A.NUM_LIVELLO <= B.NUM_LIVELLO" + " AND A.ID_AMM = " + corrRow["ID_AMM"].ToString() + " ORDER BY A.VAR_DESC_CORR ASC");
                    sql = q.getSQL();
                }
                if (corrRow["CHA_TIPO_URP"].ToString().Equals("P"))
                {
                    //commandString2="SELECT A.SYSTEM_ID, A.VAR_EMAIL, A.CHA_PA, A.ID_REGISTRO, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT FROM DPA_CORR_GLOBALI A, DPA_CORR_GLOBALI B WHERE A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND B.SYSTEM_ID='"+corrRow["RUOLO_ID_UO"].ToString()+"' AND A.NUM_LIVELLO <= B.NUM_LIVELLO";
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__CORR_GLOBALI");
                    q.setParam("param1", "A.SYSTEM_ID, A.ID_AMM, A.VAR_EMAIL, A.CHA_PA, A.ID_REGISTRO, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT");
                    q.setParam("param2", "A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND B.SYSTEM_ID=" + corrRow["RUOLO_ID_UO"].ToString() + " AND A.NUM_LIVELLO <= B.NUM_LIVELLO" + " AND A.ID_AMM = " + corrRow["ID_AMM"].ToString() + " ORDER BY A.VAR_DESC_CORR ASC");
                    sql = q.getSQL();
                }


            }
            logger.Debug(sql);
            //database.fillTable(commandString2,dataSet,"UO");
            this.ExecuteQuery(dataSet, "UO", sql);
        }

        public ArrayList ListaCorrispondentiInt(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Utenti > ListaCorrispondentiInt");
            //DocsPaWS.Utils.Database database=DocsPaWS.Utils.dbControl.getDatabase();
            //this.OpenConnection();
            //database.openConnection();
            DataSet dataSet = new DataSet();
            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);
            ArrayList listaCorr = new ArrayList();
            try
            {
                //costruzione della query in base alla richiesta ricevuta
                string commandString1 = "";
                string commandString_begin = "";
                string commandString_UO = "";
                string commandString_ruolo = "";
                string commandString_utente = "";
                DocsPaUtils.Query q;

                string generalCondition = "((A.ID_AMM IS NULL) OR (A.ID_REGISTRO IS NULL AND A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')" + RegistriCondition(objQueryCorrispondente, "A") + ")";

                if (objQueryCorrispondente.codiceRubrica != null)
                {
                    string generalConditionCR = " ((A.ID_AMM IS NULL) OR (A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "'))";
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                    q.setParam("param1", "*");
                    string whereCond = "UPPER(A.VAR_COD_RUBRICA)='" + objQueryCorrispondente.codiceRubrica.ToUpper() + "' and A.cha_tipo_ie='I' AND A.CHA_TIPO_CORR='S' AND " + generalConditionCR;
                    if (objQueryCorrispondente.fineValidita)
                        whereCond += " AND A.DTA_FINE IS NULL";
                    whereCond += " ORDER BY VAR_DESC_CORR ASC";
                    q.setParam("param2", whereCond);

                    string parentString = q.getSQL();
                    logger.Debug("parentString = " + parentString);

                    //database.fillTable(parentString,dataSet,"PARENT");
                    this.ExecuteQuery(out dataSet, "PARENT", parentString);

                    //nel caso non esiste oggetto parent con tale codice rubrica
                    if (dataSet.Tables["PARENT"].Rows != null && dataSet.Tables["PARENT"].Rows.Count == 0)
                        return listaCorr;
                    if (objQueryCorrispondente.getChildren == false)
                    {
                        if (dataSet.Tables["PARENT"].Rows[0]["CHA_TIPO_URP"].Equals("P"))
                        {
                            //commandString1="SELECT A.SYSTEM_ID,A.ID_PEOPLE,A.ID_REGISTRO, A.ID_AMM, E.VAR_NOME, E.VAR_COGNOME, E.EMAIL_ADDRESS, E.CHA_NOTIFICA, E.VAR_TELEFONO, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP, A.VAR_SMTP, A.NUM_PORTA_SMTP, C.SYSTEM_ID AS RUOLO_SYSTEM_ID, D.VAR_DESC_RUOLO AS RUOLO_DESC, C.VAR_CODICE AS RUOLO_CODICE, C.ID_UO AS RUOLO_ID_UO, C.VAR_COD_RUBRICA AS RUOLO_COD_RUBRICA, C.CHA_DETTAGLI AS RUOLO_DETTAGLI  FROM  DPA_CORR_GLOBALI A, PEOPLEGROUPS B, DPA_CORR_GLOBALI C, PEOPLE E, DPA_TIPO_RUOLO D WHERE B.PEOPLE_SYSTEM_ID=A.ID_PEOPLE AND C.ID_GRUPPO=B.GROUPS_SYSTEM_ID AND A.VAR_COD_RUBRICA='"+objQueryCorrispondente.codiceRubrica+"' AND D.SYSTEM_ID=C.ID_TIPO_RUOLO AND E.SYSTEM_ID=A.ID_PEOPLE ";
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLEGROUPS__CORR_GLOBALI__PEOPLE__TIPO_RUOLO");
                            string param = "UPPER(A.VAR_COD_RUBRICA)='" + objQueryCorrispondente.codiceRubrica.ToUpper() + "'";
                            param = param + " AND ((A.ID_AMM IS NULL) OR (A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')) AND A.CHA_TIPO_CORR='S'";

                            if (objQueryCorrispondente.fineValidita)
                                param += " AND B.DTA_FINE IS NULL";

                            q.setParam("param1", param);
                            commandString1 = q.getSQL();
                        }
                        else if (dataSet.Tables["PARENT"].Rows[0]["CHA_TIPO_URP"].Equals("R"))
                        {
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__TIPO_RUOLO");
                            q.setParam("param1", "A.SYSTEM_ID, A.ID_REGISTRO, A.ID_AMM, A.VAR_DESC_CORR,A.VAR_COGNOME, B.VAR_DESC_RUOLO, B.NUM_LIVELLO AS NUM_LIV_B, A.ID_GRUPPO, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_TIPO_URP, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_UO, A.ID_PARENT, A.CHA_DETTAGLI, A.VAR_CODICE_AOO, A.VAR_CODICE_AMM, A.DTA_FINE");
                            string lastParam = "A.ID_TIPO_RUOLO=B.SYSTEM_ID AND UPPER(A.VAR_COD_RUBRICA)='" + objQueryCorrispondente.codiceRubrica.ToUpper() + "'";
                            lastParam += " AND ((A.ID_AMM IS NULL) OR (A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')) AND A.CHA_TIPO_CORR='S'";

                            if (objQueryCorrispondente.fineValidita)
                                lastParam += " AND A.DTA_FINE IS NULL";

                            q.setParam("param2", lastParam);
                            commandString1 = q.getSQL();
                        }
                        else
                        {
                            commandString1 = parentString;
                        }
                        logger.Debug("1) " + commandString1);
                    }
                    else
                    {
                        DataRow parentRow = dataSet.Tables["PARENT"].Rows[0];

                        //se il parent è un utente si restituisce la lista vuota
                        if (parentRow["CHA_TIPO_URP"].ToString().Equals("P"))
                            return listaCorr;

                        //se il parent è una UO
                        if (parentRow["CHA_TIPO_URP"].ToString().Equals("U"))
                        {
                            //commandString1="SELECT A.SYSTEM_ID, A.VAR_DESC_CORR, A.ID_REGISTRO, A.ID_AMM, A.VAR_EMAIL, A.CHA_PA, B.VAR_DESC_RUOLO, B.NUM_LIVELLO AS RUOLO_LIVELLO, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.CHA_TIPO_URP, A.ID_UO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT, A.NUM_LIVELLO, A.ID_GRUPPO, A.CHA_DETTAGLI FROM DPA_CORR_GLOBALI A, DPA_TIPO_RUOLO B WHERE (A.ID_TIPO_RUOLO=B.SYSTEM_ID OR A.ID_TIPO_RUOLO IS NULL) AND (A.ID_UO='"+parentRow["SYSTEM_ID"].ToString()+"' OR A.ID_PARENT='"+parentRow["SYSTEM_ID"].ToString()+"') and a.cha_tipo_ie='I' AND A.CHA_TIPO_CORR='S'";
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__TIPO_RUOLO");
                            q.setParam("param1", "A.SYSTEM_ID, A.VAR_DESC_CORR,A.VAR_COGNOME, A.ID_REGISTRO, A.ID_AMM, A.VAR_EMAIL, A.CHA_PA, B.VAR_DESC_RUOLO, B.NUM_LIVELLO AS NUM_LIV_B, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.CHA_TIPO_URP, A.ID_UO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT, A.NUM_LIVELLO, A.ID_GRUPPO, A.CHA_DETTAGLI, A.DTA_FINE");
                            q.setParam("param2", "(A.ID_TIPO_RUOLO=B.SYSTEM_ID OR A.ID_TIPO_RUOLO IS NULL) AND (A.ID_UO='" + parentRow["SYSTEM_ID"].ToString() + "' OR A.ID_PARENT='" + parentRow["SYSTEM_ID"].ToString() + "') and a.cha_tipo_ie='I' AND A.CHA_TIPO_CORR='S'" + (objQueryCorrispondente.fineValidita ? " AND A.DTA_FINE IS NULL" : ""));
                            commandString1 = q.getSQL();
                        }

                        //se il parent è un ruolo
                        if (parentRow["CHA_TIPO_URP"].ToString().Equals("R"))
                        {
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLEGROUPS__CORR_GLOBALI__PEOPLE__TIPO_RUOLO");
                            q.setParam("param1", "B.GROUPS_SYSTEM_ID='" + parentRow["ID_GRUPPO"].ToString() + "' AND a.cha_tipo_ie='I' AND A.CHA_TIPO_CORR='S'" + (objQueryCorrispondente.fineValidita ? " AND B.DTA_FINE IS NULL AND C.DTA_FINE IS NULL AND A.DTA_FINE IS NULL " : ""));
                            commandString1 = q.getSQL();
                        }
                        logger.Debug("2) " + commandString1);
                    }

                }
                else
                {
                    //la query viene fatta in base all'UO, al ruolo e all'utente
                    //commandString_begin="SELECT * FROM DPA_CORR_GLOBALI A WHERE "+generalCondition;
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                    q.setParam("param1", "*");
                    q.setParam("param2", generalCondition + (objQueryCorrispondente.fineValidita ? " AND A.DTA_FINE IS NULL" : ""));
                    commandString_begin = q.getSQL();
                    logger.Debug("commandString_begin=" + commandString_begin);

                    //se la UO non è nulla
                    if (objQueryCorrispondente.codiceUO != null)
                    {
                        commandString_UO = commandString_begin + " AND UPPER(A.VAR_CODICE)='" + objQueryCorrispondente.codiceUO.ToUpper() + "' AND CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND CHA_TIPO_CORR='S'";
                    }
                    if (objQueryCorrispondente.descrizioneUO != null)
                    {
                        commandString_UO = commandString_begin + " AND CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND CHA_TIPO_CORR='S' AND (";
                        char[] separator = ConfigurationManager.AppSettings["separator"].ToCharArray();
                        string[] uo_list = objQueryCorrispondente.descrizioneUO.Split(separator);
                        for (int i = 0; i < uo_list.Length; i++)
                        {
                            commandString_UO = commandString_UO + "UPPER(VAR_DESC_CORR) LIKE UPPER('%" + uo_list[i].Replace(" ", "%") + "%')";
                            if (i < uo_list.Length - 1) { commandString_UO = commandString_UO + " OR "; }
                        }
                        commandString_UO = commandString_UO + ")";
                    }
                    commandString1 = commandString_UO;

                    //se il ruolo non è nullo
                    if (objQueryCorrispondente.isRuoloDefined())
                    {
                        //commandString_ruolo="SELECT A.*, B.VAR_DESC_RUOLO, B.NUM_LIVELLO AS RUOLO_LIVELLO FROM DPA_CORR_GLOBALI A, DPA_TIPO_RUOLO B WHERE A.ID_TIPO_RUOLO=B.SYSTEM_ID AND B.VAR_DESC_RUOLO LIKE '%"+objQueryCorrispondente.descrizioneRuolo+"%' AND CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' ";
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__TIPO_RUOLO");
                        q.setParam("param1", "A.SYSTEM_ID,A.VAR_DESC_CORR,A.VAR_COGNOME,A.ID_OLD,A.DTA_INIZIO,DTA_FINE,A.ID_PARENT,A.VAR_CODICE,A.ID_TIPO_RUOLO,A.VAR_NOME,A.ID_PEOPLE,A.CHA_TIPO_URP,A.CHA_TIPO_IE,A.CHA_TIPO_CORR,A.CHA_PA,A.VAR_COD_RUBRICA,A.ID_AMM,A.ID_GRUPPO,A.VAR_CODICE_AOO,A.VAR_CODICE_AMM,A.VAR_SMTP,A.NUM_PORTA_SMTP,A.ID_REGISTRO,A.CHA_DETTAGLI,A.ID_UO,A.CHA_RIFERIMENTO,A.VAR_EMAIL,A.VAR_CODICE_ISTAT,B.NUM_LIVELLO AS NUM_LIV_B,B.VAR_DESC_RUOLO");
                        q.setParam("param2", "A.ID_TIPO_RUOLO=B.SYSTEM_ID AND UPPER(A.VAR_DESC_CORR) LIKE '%" + objQueryCorrispondente.descrizioneRuolo.ToUpper() + "%' AND CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND A.ID_AMM = " + objQueryCorrispondente.idAmministrazione + (objQueryCorrispondente.fineValidita ? " AND A.DTA_FINE IS NULL" : ""));
                        commandString_ruolo = q.getSQL();
                        logger.Debug("commandString_ruolo=" + commandString_ruolo);

                        commandString1 = commandString_ruolo;
                    }

                    //se l'utente non è nullo
                    if (objQueryCorrispondente.isUtenteDefined())
                    {
                        string generalCondition_utente = "((A.ID_AMM IS NULL) OR (A.ID_REGISTRO IS NULL AND A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')" + RegistriCondition(objQueryCorrispondente, "A") + ") ";

                        if (objQueryCorrispondente.fineValidita)
                            generalCondition_utente += " AND B.DTA_FINE IS NULL";

                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLEGROUPS__CORR_GLOBALI__PEOPLE__TIPO_RUOLO");
                        q.setParam("param1", generalCondition_utente);

                        string commandString_utente_begin = q.getSQL();
                        if (objQueryCorrispondente.nomeUtente != null)
                        {
                            commandString_utente = commandString_utente_begin + " AND UPPER(E.VAR_NOME) LIKE '" + objQueryCorrispondente.nomeUtente.ToUpper() + "%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='I' AND A.CHA_TIPO_CORR='S'";
                        }
                        else
                        {
                            commandString_utente = commandString_utente_begin + " AND UPPER(E.VAR_COGNOME) LIKE '" + objQueryCorrispondente.cognomeUtente.ToUpper().Replace("'", "''") + "%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='I' AND A.CHA_TIPO_CORR='S'";
                        }

                        //controllo se la query ha condizioni sui ruoli
                        if (objQueryCorrispondente.isRuoloDefined())
                        {
                            commandString_utente = commandString_utente + " AND UPPER(D.VAR_DESC_RUOLO) LIKE '%" + objQueryCorrispondente.descrizioneRuolo.ToUpper().Replace("'", "''") + "%'";
                        }
                        commandString1 = commandString_utente;
                    }
                }

                if (!commandString1.Contains("ORDER BY"))
                    if (commandString1.Contains("A.VAR_COGNOME"))
                        commandString1 += " ORDER BY A.VAR_COGNOME ASC";
                    else
                        if (commandString1.Contains("E.VAR_COGNOME"))
                            commandString1 += " ORDER BY E.VAR_COGNOME ASC";
                //commandString1 += " ORDER BY A.VAR_DESC_CORR ASC";

                commandString1 = DocsPaDbManagement.Functions.Functions.SelectTop(commandString1);
                //questa modifica server per avere la lista corrispondenti senza nomi ripetuti
                //non potendo intervenire direttamente sul codice sql ,in quanto costruito dinamicamente
                //commandString1 = commandString1.Insert(7, "distinct ");

                //commandString1 += " ORDER BY VAR_COGNOME";

                //DocsPaWS.Utils.dbControl.selectTop(commandString1);
                logger.Debug("3) " + commandString1);

                //database.fillTable(commandString1,dataSet,"CORRISPONDENTI");
                this.ExecuteQuery(out dataSet, "CORRISPONDENTI", commandString1);
                ArrayList list = new ArrayList();
                //logger.Debug(commandString1);
                //ArrayList ruoli=new ArrayList();
                foreach (DataRow corrRow in dataSet.Tables["CORRISPONDENTI"].Rows)
                {
                    RicercaUOParentInt(dataSet, corrRow);
                    if (dataSet != null)
                    {
                        if (corrRow["CHA_TIPO_URP"].ToString().Equals("U"))
                        {
                            //si verifica se è replicata nella lista
                            int replies = 0;
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (((DocsPaVO.utente.Corrispondente)list[i]).systemId.Equals(corrRow["SYSTEM_ID"].ToString())) replies = replies + 1;
                            }
                            if (replies == 0)
                            {
                                DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                                corrispondenteUO.systemId = corrRow["SYSTEM_ID"].ToString();
                                corrispondenteUO.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                                corrispondenteUO.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                                corrispondenteUO.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                                //DTA_FINE, 02 FEB 2007
                                corrispondenteUO.dta_fine = corrRow["DTA_FINE"].ToString();
                                if (corrRow["ID_REGISTRO"] != null)
                                {
                                    corrispondenteUO.idRegistro = corrRow["ID_REGISTRO"].ToString();
                                }
                                corrispondenteUO.email = corrRow["VAR_EMAIL"].ToString();
                                corrispondenteUO.interoperante = FromCharToBool(corrRow["CHA_PA"].ToString());
                                corrispondenteUO.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                                corrispondenteUO.livello = corrRow["NUM_LIVELLO"].ToString();
                                DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                                sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                                corrispondenteUO.serverPosta = sp;
                                corrispondenteUO.idAmministrazione = corrRow["ID_AMM"].ToString();
                                corrispondenteUO.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                                corrispondenteUO.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                                corrispondenteUO.codiceIstat = corrRow["VAR_CODICE_ISTAT"].ToString();
                                corrispondenteUO.tipoIE = "I";
                                corrispondenteUO.tipoCorrispondente = "U";
                                //qui si ritrova la parentela
                                if (!string.IsNullOrEmpty(corrRow["ID_PARENT"].ToString())
                                    && !corrRow["ID_PARENT"].ToString().Equals("0"))
                                {
                                    corrispondenteUO.parent = GetParents(corrRow["ID_PARENT"].ToString(), dataSet.Tables["UO"]);
                                }
                                list.Add(corrispondenteUO);
                            }
                        }
                        if (corrRow["CHA_TIPO_URP"].ToString().Equals("R"))
                        {
                            DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                            corrispondenteRuolo.systemId = corrRow["SYSTEM_ID"].ToString();
                            //corrispondenteRuolo.descrizione=corrRow["VAR_DESC_RUOLO"].ToString();
                            corrispondenteRuolo.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                            corrispondenteRuolo.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                            corrispondenteRuolo.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                            //DTA_FINE, 02 FEB 2007
                            corrispondenteRuolo.dta_fine = corrRow["DTA_FINE"].ToString();

                            corrispondenteRuolo.idAmministrazione = corrRow["ID_AMM"].ToString();
                            //corrispondenteRuolo.livello=corrRow["RUOLO_LIVELLO"].ToString(); 
                            corrispondenteRuolo.livello = corrRow["NUM_LIV_B"].ToString();
                            corrispondenteRuolo.idGruppo = corrRow["ID_GRUPPO"].ToString();
                            corrispondenteRuolo.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                            corrispondenteRuolo.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                            DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                            sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                            sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                            corrispondenteRuolo.serverPosta = sp;
                            corrispondenteRuolo.tipoIE = "I";
                            corrispondenteRuolo.tipoCorrispondente = "R";
                            if (corrRow["ID_REGISTRO"] != null)
                            {
                                corrispondenteRuolo.idRegistro = corrRow["ID_REGISTRO"].ToString();
                            }
                            corrispondenteRuolo.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                            //qui si ritrova la parentela (con filtro o no)
                            if (objQueryCorrispondente.isUODefined())
                            {
                                if ((objQueryCorrispondente.descrizioneUO != null && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])))
                                {
                                    corrispondenteRuolo.uo = GetParents(corrRow["ID_UO"].ToString(), dataSet.Tables["UO"]);
                                    list.Add(corrispondenteRuolo);
                                }
                            }
                            else
                            {
                                corrispondenteRuolo.uo = GetParents(corrRow["ID_UO"].ToString(), dataSet.Tables["UO"]);
                                list.Add(corrispondenteRuolo);
                            }


                            // S. Furnari - 17/01/2013 - Sviluppo interop. I test automatici utilizzano questo metodo che 
                            // non imposta il parametro tipoIE della UO parent del ruolo. L'ho aggiunto così almeno riesco
                            // a spedire.
                            if (corrispondenteRuolo.uo != null)
                                corrispondenteRuolo.uo.tipoIE = "I";
                        }
                        if (corrRow["CHA_TIPO_URP"].ToString().Equals("P"))
                        {
                            DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();
                            corrispondenteUtente.systemId = corrRow["SYSTEM_ID"].ToString();
                            corrispondenteUtente.idPeople = corrRow["ID_PEOPLE"].ToString();
                            corrispondenteUtente.descrizione = corrRow["VAR_NOME"].ToString() + " " + corrRow["VAR_COGNOME"].ToString();
                            corrispondenteUtente.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                            corrispondenteUtente.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                            corrispondenteUtente.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                            //DTA_FINE, 02 FEB 2007
                            corrispondenteUtente.dta_fine = corrRow["DTA_FINE"].ToString();
                            corrispondenteUtente.disabilitato = corrRow["DISABLED"].ToString();
                            corrispondenteUtente.idAmministrazione = corrRow["ID_AMM"].ToString();
                            corrispondenteUtente.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                            corrispondenteUtente.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                            if (corrRow["ID_REGISTRO"] != null)
                            {
                                corrispondenteUtente.idRegistro = corrRow["ID_REGISTRO"].ToString();
                            }
                            corrispondenteUtente.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                            DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                            sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                            sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                            corrispondenteUtente.serverPosta = sp;
                            corrispondenteUtente.email = corrRow["EMAIL_ADDRESS"].ToString();
                            corrispondenteUtente.notifica = corrRow["CHA_NOTIFICA"].ToString();
                            corrispondenteUtente.telefono = corrRow["VAR_TELEFONO"].ToString();
                            corrispondenteUtente.tipoIE = "I";
                            corrispondenteUtente.tipoCorrispondente = "P";
                            if (corrRow["CHA_NOTIFICA_CON_ALLEGATO"] != null)
                            {
                                if (corrRow["CHA_NOTIFICA_CON_ALLEGATO"].ToString().Equals("1"))
                                {
                                    corrispondenteUtente.notificaConAllegato = true;
                                }
                                else
                                {
                                    corrispondenteUtente.notificaConAllegato = false;
                                }
                            }

                            if (corrRow.Table.Columns.Contains("var_sede") && corrRow["var_sede"] != null)
                            {
                                corrispondenteUtente.sede = corrRow["var_sede"].ToString();
                            }

                            //si trova il ruolo corrispondente
                            DocsPaVO.utente.Ruolo ruoloUtente = new DocsPaVO.utente.Ruolo();
                            ruoloUtente.systemId = corrRow["RUOLO_SYSTEM_ID"].ToString();
                            ruoloUtente.descrizione = corrRow["RUOLO_DESC"].ToString();
                            ruoloUtente.codiceCorrispondente = corrRow["RUOLO_CODICE"].ToString();
                            ruoloUtente.codiceRubrica = corrRow["RUOLO_COD_RUBRICA"].ToString();
                            ruoloUtente.dettagli = FromCharToBool(corrRow["RUOLO_DETTAGLI"].ToString());

                            //qui si trova la parentela (con filtro o no)
                            if (objQueryCorrispondente.isUODefined())
                            {
                                if ((objQueryCorrispondente.descrizioneUO != null && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, corrRow["RUOLO_ID_UO"].ToString(), dataSet.Tables["UO"])) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, corrRow["RUOLO_ID_UO"].ToString(), dataSet.Tables["UO"])))
                                {
                                    ruoloUtente.uo = GetParents(corrRow["RUOLO_ID_UO"].ToString(), dataSet.Tables["UO"]);

                                    ArrayList ruoli = new ArrayList();
                                    ruoli.Add(ruoloUtente);
                                    corrispondenteUtente.ruoli = ruoli;
                                    list.Add(corrispondenteUtente);
                                }
                            }
                            else
                            {
                                ruoloUtente.uo = GetParents(corrRow["RUOLO_ID_UO"].ToString(), dataSet.Tables["UO"]);

                                ArrayList ruoli = new ArrayList();
                                ruoli.Add(ruoloUtente);
                                corrispondenteUtente.ruoli = ruoli;
                                list.Add(corrispondenteUtente);
                            }
                        }
                    }

                }
                listaCorr = list;

                //database.closeConnection();
                //this.CloseConnection();				
            }
            catch (Exception e)
            {
                logger.Debug("ERROR : ListaCorrispondentiInt", e);

                //database.closeConnection();
                //this.CloseConnection();
                //throw e;
                listaCorr = null;
            }

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Utenti > ListaCorrispondentiInt");
            return listaCorr;
        }

        public ArrayList ListaCorrispondentiInt_Aut(DocsPaVO.addressbook.QueryCorrispondente qco, DocsPaVO.trasmissione.RagioneTrasmissione ragioneTrasm)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Utenti > ListaCorrispondentiInt");

            DataSet dataSet = new DataSet();
            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);
            ArrayList listaCorr = new ArrayList();

            try
            {
                string commandString1 = "";
                string commandString_begin = "";
                string commandString_UO = "";
                string commandString_ruolo = "";
                string commandString_utente = "";
                DocsPaUtils.Query q;


                if (objQueryCorrispondente.codiceRubrica != null)
                {
                    string generalConditionCR = " ((A.ID_AMM IS NULL) OR (A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "'))";
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                    q.setParam("param1", "*");
                    string whereCond = "UPPER(A.VAR_COD_RUBRICA)='" + objQueryCorrispondente.codiceRubrica.ToUpper() + "' and A.cha_tipo_ie='I' AND A.CHA_TIPO_CORR='S' AND " + generalConditionCR;
                    if (objQueryCorrispondente.fineValidita)
                        whereCond = whereCond + " AND A.DTA_FINE IS NULL";
                    q.setParam("param2", whereCond);

                    string parentString = q.getSQL();
                    logger.Debug("parentString = " + parentString);

                    this.ExecuteQuery(out dataSet, "PARENT", parentString);

                    //nel caso non esiste oggetto parent con tale codice rubrica
                    if (dataSet.Tables["PARENT"].Rows != null && dataSet.Tables["PARENT"].Rows.Count == 0)
                        return listaCorr;
                    if (objQueryCorrispondente.getChildren == false)
                    {
                        if (dataSet.Tables["PARENT"].Rows[0]["CHA_TIPO_URP"].Equals("P"))
                        {
                            string generalCondition = "P.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "' AND R.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "'";// AND R.CHA_RIFERIMENTO='1'";
                            generalCondition = generalCondition + " AND R.ID_TIPO_RUOLO=B.SYSTEM_ID AND R.SYSTEM_ID=C.ID_RUOLO_IN_UO AND " + RegistriCondition(objQueryCorrispondente, "C", "true");
                            generalCondition = generalCondition + " AND P.CHA_TIPO_URP='P' AND P.CHA_TIPO_IE='I' AND P.CHA_TIPO_CORR='S' AND R.CHA_TIPO_URP='R' AND R.CHA_TIPO_IE='I' AND R.CHA_TIPO_CORR='S'";
                            generalCondition = generalCondition + " AND E.SYSTEM_ID=P.ID_PEOPLE AND PG.GROUPS_SYSTEM_ID=R.ID_GRUPPO AND PG.PEOPLE_SYSTEM_ID=P.ID_PEOPLE AND PG.DTA_FINE IS NULL";

                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali_ProtoInt_P");
                            q.setParam("param1", "P.SYSTEM_ID,P.ID_PEOPLE,P.ID_REGISTRO,P.ID_AMM,E.VAR_NOME,E.VAR_COGNOME,E.EMAIL_ADDRESS,E.CHA_NOTIFICA,E.VAR_TELEFONO,P.VAR_DESC_CORR,P.VAR_CODICE,P.VAR_COD_RUBRICA,P.CHA_DETTAGLI,P.CHA_TIPO_URP,P.VAR_SMTP,P.NUM_PORTA_SMTP,P.VAR_CODICE_AMM,P.VAR_CODICE_AOO,R.SYSTEM_ID AS RUOLO_SYSTEM_ID,B.VAR_DESC_RUOLO AS RUOLO_DESC,R.VAR_CODICE AS RUOLO_CODICE,R.ID_UO AS RUOLO_ID_UO,R.VAR_COD_RUBRICA AS RUOLO_COD_RUBRICA,R.CHA_DETTAGLI AS RUOLO_DETTAGLI,E.CHA_NOTIFICA_CON_ALLEGATO");
                            q.setParam("param2", generalCondition);
                            commandString_begin = q.getSQL();
                            logger.Debug("commandString_begin=" + commandString_begin);
                            commandString1 = commandString_begin + " AND UPPER(P.VAR_COD_RUBRICA) = '" + objQueryCorrispondente.codiceRubrica.ToUpper() + "'";
                        }
                        else if (dataSet.Tables["PARENT"].Rows[0]["CHA_TIPO_URP"].Equals("R"))
                        {
                            string generalCondition = "R.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "'"; //AND R.CHA_RIFERIMENTO='1'";
                            generalCondition = generalCondition + " AND R.ID_TIPO_RUOLO=B.SYSTEM_ID AND R.SYSTEM_ID=C.ID_RUOLO_IN_UO AND " + RegistriCondition(objQueryCorrispondente, "C", "true") + " AND C.DTA_FINE IS NULL AND R.DTA_FINE IS NULL";

                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali_ProtoInt_R");
                            q.setParam("param1", "R.SYSTEM_ID,R.VAR_DESC_CORR,R.ID_OLD,R.DTA_INIZIO,R.DTA_FINE,R.ID_PARENT,R.VAR_CODICE,R.ID_TIPO_RUOLO,R.VAR_NOME,R.VAR_COGNOME,R.ID_PEOPLE,R.CHA_TIPO_URP,R.CHA_TIPO_IE,R.CHA_TIPO_CORR,R.CHA_PA,R.VAR_COD_RUBRICA,R.ID_AMM,R.ID_GRUPPO,R.VAR_CODICE_AOO,R.VAR_CODICE_AMM,R.VAR_SMTP,R.NUM_PORTA_SMTP,R.ID_REGISTRO,R.CHA_DETTAGLI,R.ID_UO,R.CHA_RIFERIMENTO,R.VAR_EMAIL,R.VAR_CODICE_ISTAT,B.NUM_LIVELLO AS NUM_LIV_B,B.VAR_DESC_RUOLO");
                            q.setParam("param2", generalCondition);
                            commandString_begin = q.getSQL();
                            logger.Debug("commandString_begin=" + commandString_begin);

                            commandString_ruolo = commandString_begin + " AND R.CHA_TIPO_IE='I' AND R.CHA_TIPO_URP = 'R'";
                            //commandString1=commandString_ruolo+" AND UPPER(R.VAR_DESC_CORR) LIKE '%"+objQueryCorrispondente.descrizioneRuolo.ToUpper() +"%'";
                            commandString1 = commandString_ruolo + " AND UPPER(R.VAR_DESC_CORR) LIKE '%" + dataSet.Tables["PARENT"].Rows[0]["VAR_DESC_CORR"].ToString().ToUpper().Replace("'", "''") + "%'" + " ORDER BY R.DTA_INIZIO DESC";

                        }
                        else if (dataSet.Tables["PARENT"].Rows[0]["CHA_TIPO_URP"].Equals("U"))
                        {
                            //string generalCondition=" AND U.ID_AMM='"+objQueryCorrispondente.idAmministrazione+"' AND R.ID_AMM='"+objQueryCorrispondente.idAmministrazione+"' AND R.CHA_RIFERIMENTO='1'";
                            //Commentato il 17/07/2006: perchè altrimenti la ricerca per codice corrisp da dei risultati diversi dalla
                            //rubrica che non gestisce la ricerca per uo che possiedono ruoli di riferimento autorizzati su un registro 
                            string generalCondition = " AND U.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "' AND R.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "'";
                            generalCondition = generalCondition + "  AND " + RegistriCondition(objQueryCorrispondente, "C", "true") + " AND C.DTA_FINE IS NULL AND R.DTA_FINE IS NULL";
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali_ProtoInt_U");
                            q.setParam("param1", "U.*,C.id_registro as IdRegSel");
                            q.setParam("param2", generalCondition);
                            commandString_begin = q.getSQL();

                            commandString_UO = commandString_begin + " AND UPPER(U.VAR_COD_RUBRICA)='" + objQueryCorrispondente.codiceRubrica.ToUpper() + "'";
                            logger.Debug("commandString_UO=" + commandString_UO);
                            commandString1 = commandString_UO;
                        }
                        logger.Debug("1) " + commandString1);
                    }
                    else
                    {
                        DataRow parentRow = dataSet.Tables["PARENT"].Rows[0];

                        //se il parent è un utente si restituisce la lista vuota
                        if (parentRow["CHA_TIPO_URP"].ToString().Equals("P"))
                            return listaCorr;

                        //se il parent è una UO
                        if (parentRow["CHA_TIPO_URP"].ToString().Equals("U"))
                        {
                            string generalCondition = " AND U.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "' AND R.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "' AND R.CHA_RIFERIMENTO='1'";
                            generalCondition = generalCondition + "  AND " + RegistriCondition(objQueryCorrispondente, "C", "true");
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali_ProtoInt_U");
                            q.setParam("param1", "U.*,C.id_registro as IdRegSel");
                            q.setParam("param2", generalCondition);
                            commandString_begin = q.getSQL();
                            logger.Debug("commandString_begin=" + commandString_begin);
                            commandString1 = commandString_begin + " AND U.ID_UO='" + parentRow["SYSTEM_ID"].ToString() + "' OR U.ID_PARENT='" + parentRow["SYSTEM_ID"].ToString() + "'";
                        }

                        //se il parent è un ruolo
                        if (parentRow["CHA_TIPO_URP"].ToString().Equals("R"))
                        {
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLEGROUPS__CORR_GLOBALI__PEOPLE__TIPO_RUOLO");
                            q.setParam("param1", "B.GROUPS_SYSTEM_ID='" + parentRow["ID_GRUPPO"].ToString() + "' AND a.cha_tipo_ie='I' AND A.CHA_TIPO_CORR='S'");
                            commandString1 = q.getSQL();
                        }
                        logger.Debug("2) " + commandString1);
                    }

                }
                else
                {
                    //la query viene fatta in base all'UO, al ruolo e all'utente
                    #region UO
                    if (objQueryCorrispondente.isUODefined())
                    {
                        string generalCondition = " AND U.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "' AND R.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "' AND R.CHA_RIFERIMENTO='1'";
                        generalCondition = generalCondition + "  AND " + RegistriCondition(objQueryCorrispondente, "C", "true");
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali_ProtoInt_U");
                        q.setParam("param1", "U.*,C.id_registro as IdRegSel");
                        q.setParam("param2", generalCondition);
                        commandString_begin = q.getSQL();
                        logger.Debug("commandString_begin=" + commandString_begin);


                        //se la UO non è nulla
                        if (objQueryCorrispondente.codiceUO != null)
                        {
                            commandString_UO = commandString_begin + " AND UPPER(U.VAR_CODICE)='" + objQueryCorrispondente.codiceUO.ToUpper() + "'";

                            if (objQueryCorrispondente.fineValidita)
                                commandString_UO += " AND U.DTA_FINE IS NULL AND R.DTA_FINE IS NULL";
                        }
                        if (objQueryCorrispondente.descrizioneUO != null)
                        {
                            char[] separator = ConfigurationManager.AppSettings["separator"].ToCharArray();
                            string[] uo_list = objQueryCorrispondente.descrizioneUO.Split(separator);
                            for (int i = 0; i < uo_list.Length; i++)
                            {
                                commandString_UO = commandString_begin + " AND (UPPER(U.VAR_DESC_CORR) LIKE UPPER('%" + uo_list[i].Replace(" ", "%") + "%')";
                                if (i < uo_list.Length - 1) { commandString_UO = commandString_UO + " OR "; }
                            }
                            commandString_UO = commandString_UO + ")";
                            if (objQueryCorrispondente.fineValidita)
                                commandString_UO += " AND U.DTA_FINE IS NULL AND R.DTA_FINE IS NULL";
                        }
                        commandString1 = commandString_UO;
                    }
                    #endregion

                    #region RUOLO
                    //se il ruolo non è nullo
                    if (objQueryCorrispondente.isRuoloDefined())
                    {

                        string generalCondition = "R.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "'"; //AND R.CHA_RIFERIMENTO='1'";
                        generalCondition = generalCondition + " AND R.ID_TIPO_RUOLO=B.SYSTEM_ID AND R.SYSTEM_ID=C.ID_RUOLO_IN_UO AND " + RegistriCondition(objQueryCorrispondente, "C", "true");

                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali_ProtoInt_R");
                        q.setParam("param1", "R.SYSTEM_ID,R.VAR_DESC_CORR,R.ID_OLD,R.DTA_INIZIO,R.DTA_FINE,R.ID_PARENT,R.VAR_CODICE,R.ID_TIPO_RUOLO,R.VAR_NOME,R.VAR_COGNOME,R.ID_PEOPLE,R.CHA_TIPO_URP,R.CHA_TIPO_IE,R.CHA_TIPO_CORR,R.CHA_PA,R.VAR_COD_RUBRICA,R.ID_AMM,R.ID_GRUPPO,R.VAR_CODICE_AOO,R.VAR_CODICE_AMM,R.VAR_SMTP,R.NUM_PORTA_SMTP,R.ID_REGISTRO,R.CHA_DETTAGLI,R.ID_UO,R.CHA_RIFERIMENTO,R.VAR_EMAIL,R.VAR_CODICE_ISTAT,B.NUM_LIVELLO AS NUM_LIV_B,B.VAR_DESC_RUOLO");
                        q.setParam("param2", generalCondition);
                        commandString_begin = q.getSQL();
                        logger.Debug("commandString_begin=" + commandString_begin);

                        commandString_ruolo = commandString_begin + " AND R.CHA_TIPO_IE='I' AND R.CHA_TIPO_URP = 'R'";
                        commandString_ruolo = commandString_ruolo + " AND UPPER(R.VAR_DESC_CORR) LIKE '%" + objQueryCorrispondente.descrizioneRuolo.ToUpper().Replace("'", "''") + "%'";

                        if (objQueryCorrispondente.fineValidita)
                            commandString_ruolo += " AND R.DTA_FINE IS NULL";
                        logger.Debug("commandString_ruolo=" + commandString_ruolo);

                        commandString1 = commandString_ruolo;
                    }
                    #endregion

                    #region Utente
                    //se l'utente non è nullo
                    if (objQueryCorrispondente.isUtenteDefined())
                    {
                        string generalCondition = "P.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "' AND R.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "'";// AND R.CHA_RIFERIMENTO='1'";
                        generalCondition = generalCondition + " AND R.ID_TIPO_RUOLO=B.SYSTEM_ID AND R.SYSTEM_ID=C.ID_RUOLO_IN_UO AND " + RegistriCondition(objQueryCorrispondente, "C", "true");
                        generalCondition = generalCondition + " AND P.CHA_TIPO_URP='P' AND P.CHA_TIPO_IE='I' AND P.CHA_TIPO_CORR='S' AND R.CHA_TIPO_URP='R' AND R.CHA_TIPO_IE='I' AND R.CHA_TIPO_CORR='S'";
                        generalCondition = generalCondition + " AND E.SYSTEM_ID=P.ID_PEOPLE AND PG.GROUPS_SYSTEM_ID=R.ID_GRUPPO AND PG.PEOPLE_SYSTEM_ID=P.ID_PEOPLE AND PG.DTA_FINE IS NULL";

                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali_ProtoInt_P");
                        q.setParam("param1", "P.SYSTEM_ID,P.ID_PEOPLE,P.ID_REGISTRO,P.ID_AMM,E.VAR_NOME,E.VAR_COGNOME,E.EMAIL_ADDRESS,E.CHA_NOTIFICA,E.VAR_TELEFONO,P.VAR_DESC_CORR,P.VAR_CODICE,P.VAR_COD_RUBRICA,P.CHA_DETTAGLI,P.CHA_TIPO_URP,P.VAR_SMTP,P.NUM_PORTA_SMTP,P.VAR_CODICE_AMM,P.VAR_CODICE_AOO,R.SYSTEM_ID AS RUOLO_SYSTEM_ID,B.VAR_DESC_RUOLO AS RUOLO_DESC,R.VAR_CODICE AS RUOLO_CODICE,R.ID_UO AS RUOLO_ID_UO,R.VAR_COD_RUBRICA AS RUOLO_COD_RUBRICA,R.CHA_DETTAGLI AS RUOLO_DETTAGLI");
                        q.setParam("param2", generalCondition);
                        commandString_begin = q.getSQL();
                        logger.Debug("commandString_begin=" + commandString_begin);

                        if (objQueryCorrispondente.nomeUtente != null)
                        {
                            commandString_utente = commandString_begin + " AND UPPER(E.VAR_NOME) LIKE '" + objQueryCorrispondente.nomeUtente.ToUpper() + "%'";
                        }
                        else
                        {
                            commandString_utente = commandString_begin + " AND UPPER(E.VAR_COGNOME) LIKE '" + objQueryCorrispondente.cognomeUtente.ToUpper() + "%'";
                        }

                        //controllo se la query ha condizioni sui ruoli
                        if (objQueryCorrispondente.isRuoloDefined())
                        {

                            commandString_utente = commandString_utente + " AND UPPER(B.VAR_DESC_RUOLO) LIKE '%" + objQueryCorrispondente.descrizioneRuolo.ToUpper() + "%'";
                        }
                        if (objQueryCorrispondente.fineValidita)
                            commandString_utente += " AND R.DTA_FINE IS NULL AND P.DTA_FINE IS NULL";
                        commandString1 = commandString_utente;

                    }
                    #endregion

                }


                if (commandString1 != "")
                {

                    commandString1 = DocsPaDbManagement.Functions.Functions.SelectTop(commandString1);
                    //questa modifica server per avere la lista corrispondenti senza nomi ripetuti
                    //non potendo intervenire direttamente sul codice sql ,in quanto costruito dinamicamente
                    commandString1 = commandString1.Insert(7, "distinct ");
                    logger.Debug("3) " + commandString1);

                    this.ExecuteQuery(out dataSet, "CORRISPONDENTI", commandString1);
                    ArrayList list = new ArrayList();
                    if (dataSet.Tables["CORRISPONDENTI"].Rows.Count > 0)
                    {
                        foreach (DataRow corrRow in dataSet.Tables["CORRISPONDENTI"].Rows)
                        {
                            RicercaUOParentInt(dataSet, corrRow);
                            if (dataSet != null)
                            {
                                if (corrRow["CHA_TIPO_URP"].ToString().Equals("U"))
                                {
                                    //si verifica se è replicata nella lista
                                    int replies = 0;
                                    for (int i = 0; i < list.Count; i++)
                                    {
                                        if (((DocsPaVO.utente.Corrispondente)list[i]).systemId.Equals(corrRow["SYSTEM_ID"].ToString())) replies = replies + 1;
                                    }
                                    if (replies == 0)
                                    {
                                        DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                                        corrispondenteUO.systemId = corrRow["SYSTEM_ID"].ToString();
                                        corrispondenteUO.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                                        corrispondenteUO.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                                        corrispondenteUO.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                                        if (corrRow["ID_REGISTRO"] != null)
                                        {
                                            corrispondenteUO.idRegistro = corrRow["ID_REGISTRO"].ToString();
                                        }
                                        corrispondenteUO.email = corrRow["VAR_EMAIL"].ToString();
                                        corrispondenteUO.interoperante = FromCharToBool(corrRow["CHA_PA"].ToString());
                                        corrispondenteUO.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                                        corrispondenteUO.livello = corrRow["NUM_LIVELLO"].ToString();
                                        DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                        sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                                        sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                                        corrispondenteUO.serverPosta = sp;
                                        corrispondenteUO.idAmministrazione = corrRow["ID_AMM"].ToString();
                                        corrispondenteUO.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                                        corrispondenteUO.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                                        corrispondenteUO.codiceIstat = corrRow["VAR_CODICE_ISTAT"].ToString();
                                        corrispondenteUO.tipoIE = "I";
                                        //corrispondenteUO.tipoCorrispondente = corrRow["CHA_TIPO_URP"].ToString();
                                        //qui si ritrova la parentela
                                        if (!corrRow["ID_PARENT"].ToString().Equals("0"))
                                        {
                                            corrispondenteUO.parent = GetParents(corrRow["ID_PARENT"].ToString(), dataSet.Tables["UO"]);
                                        }
                                        list.Add(corrispondenteUO);
                                    }
                                }
                                if (corrRow["CHA_TIPO_URP"].ToString().Equals("R"))
                                {
                                    DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                                    corrispondenteRuolo.systemId = corrRow["SYSTEM_ID"].ToString();
                                    //corrispondenteRuolo.descrizione=corrRow["VAR_DESC_RUOLO"].ToString();
                                    corrispondenteRuolo.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                                    corrispondenteRuolo.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                                    corrispondenteRuolo.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                                    corrispondenteRuolo.idAmministrazione = corrRow["ID_AMM"].ToString();
                                    //corrispondenteRuolo.livello=corrRow["RUOLO_LIVELLO"].ToString(); 
                                    corrispondenteRuolo.livello = corrRow["NUM_LIV_B"].ToString();
                                    corrispondenteRuolo.idGruppo = corrRow["ID_GRUPPO"].ToString();
                                    corrispondenteRuolo.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                                    corrispondenteRuolo.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                                    DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                    sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                                    sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                                    corrispondenteRuolo.serverPosta = sp;
                                    corrispondenteRuolo.tipoIE = "I";
                                    //corrispondenteRuolo.tipoCorrispondente = corrRow["CHA_TIPO_URP"].ToString();
                                    if (corrRow["ID_REGISTRO"] != null)
                                    {
                                        corrispondenteRuolo.idRegistro = corrRow["ID_REGISTRO"].ToString();
                                    }
                                    corrispondenteRuolo.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                                    //qui si ritrova la parentela (con filtro o no)
                                    if (objQueryCorrispondente.isUODefined())
                                    {
                                        if ((objQueryCorrispondente.descrizioneUO != null && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])))
                                        {
                                            corrispondenteRuolo.uo = GetParents(corrRow["ID_UO"].ToString(), dataSet.Tables["UO"]);
                                            list.Add(corrispondenteRuolo);
                                        }
                                    }
                                    else
                                    {
                                        corrispondenteRuolo.uo = GetParents(corrRow["ID_UO"].ToString(), dataSet.Tables["UO"]);
                                        list.Add(corrispondenteRuolo);
                                    }
                                }
                                if (corrRow["CHA_TIPO_URP"].ToString().Equals("P"))
                                {
                                    DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();
                                    corrispondenteUtente.systemId = corrRow["SYSTEM_ID"].ToString();
                                    corrispondenteUtente.idPeople = corrRow["ID_PEOPLE"].ToString();
                                    corrispondenteUtente.descrizione = corrRow["VAR_NOME"].ToString() + " " + corrRow["VAR_COGNOME"].ToString();
                                    corrispondenteUtente.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                                    corrispondenteUtente.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                                    corrispondenteUtente.idAmministrazione = corrRow["ID_AMM"].ToString();
                                    corrispondenteUtente.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                                    corrispondenteUtente.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                                    if (corrRow["ID_REGISTRO"] != null)
                                    {
                                        corrispondenteUtente.idRegistro = corrRow["ID_REGISTRO"].ToString();
                                    }
                                    corrispondenteUtente.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                                    DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                    sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                                    sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                                    corrispondenteUtente.serverPosta = sp;
                                    corrispondenteUtente.email = corrRow["EMAIL_ADDRESS"].ToString();
                                    corrispondenteUtente.notifica = corrRow["CHA_NOTIFICA"].ToString();
                                    corrispondenteUtente.telefono = corrRow["VAR_TELEFONO"].ToString();
                                    corrispondenteUtente.tipoIE = "I";
                                    //corrispondenteUtente.tipoCorrispondente = corrRow["CHA_TIPO_URP"].ToString();
                                    if (corrRow["CHA_NOTIFICA_CON_ALLEGATO"] != null)
                                    {
                                        if (corrRow["CHA_NOTIFICA_CON_ALLEGATO"].ToString().Equals("1"))
                                        {
                                            corrispondenteUtente.notificaConAllegato = true;
                                        }
                                        else
                                        {
                                            corrispondenteUtente.notificaConAllegato = false;
                                        }
                                    }
                                    //si trova il ruolo corrispondente
                                    DocsPaVO.utente.Ruolo ruoloUtente = new DocsPaVO.utente.Ruolo();
                                    ruoloUtente.systemId = corrRow["RUOLO_SYSTEM_ID"].ToString();
                                    ruoloUtente.descrizione = corrRow["RUOLO_DESC"].ToString();
                                    ruoloUtente.codiceCorrispondente = corrRow["RUOLO_CODICE"].ToString();
                                    ruoloUtente.codiceRubrica = corrRow["RUOLO_COD_RUBRICA"].ToString();
                                    ruoloUtente.dettagli = FromCharToBool(corrRow["RUOLO_DETTAGLI"].ToString());

                                    //qui si trova la parentela (con filtro o no)
                                    if (objQueryCorrispondente.isUODefined())
                                    {
                                        if ((objQueryCorrispondente.descrizioneUO != null && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, corrRow["RUOLO_ID_UO"].ToString(), dataSet.Tables["UO"])) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, corrRow["RUOLO_ID_UO"].ToString(), dataSet.Tables["UO"])))
                                        {
                                            ruoloUtente.uo = GetParents(corrRow["RUOLO_ID_UO"].ToString(), dataSet.Tables["UO"]);

                                            ArrayList ruoli = new ArrayList();
                                            ruoli.Add(ruoloUtente);
                                            corrispondenteUtente.ruoli = ruoli;
                                            list.Add(corrispondenteUtente);
                                        }
                                    }
                                    else
                                    {
                                        ruoloUtente.uo = GetParents(corrRow["RUOLO_ID_UO"].ToString(), dataSet.Tables["UO"]);

                                        ArrayList ruoli = new ArrayList();
                                        ruoli.Add(ruoloUtente);
                                        corrispondenteUtente.ruoli = ruoli;
                                        list.Add(corrispondenteUtente);
                                    }
                                }
                            }

                        }
                    }
                    listaCorr = list;
                }
            }
            catch (Exception e)
            {
                logger.Debug("ERROR : ListaCorrispondentiInt_Aut", e);
                listaCorr = null;
            }

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Utenti > ListaCorrispondentiInt_Aut");
            return listaCorr;
        }


        /// <summary>
        /// ritorna un corrispodente. In caso di utente ritorna solo se non disabilitato.
        /// </summary>
        /// <param name="qco"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Corrispondente GetCorrispondenteInt(DocsPaVO.addressbook.QueryCorrispondente qco)
        {

            return GetCorrispondenteInt(qco, true);
        }



        public ArrayList ListaCorrispondentiIntInterop(DocsPaVO.addressbook.QueryCorrispondente qco)
        {

            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);
            string commandString = "";
            DataSet dataSet = new DataSet();
            DocsPaUtils.Query q;
            ArrayList listaCorr = new ArrayList();
            //ricavo il ruolo mittente del protocollo ricevuto per interoperabilità
            if (objQueryCorrispondente.isRuoloDefined())
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ID_UO_INTEROP");
                q.setParam("param1", objQueryCorrispondente.descrizioneUO.ToUpper());
                q.setParam("param2", objQueryCorrispondente.idAmministrazione);
                commandString = q.getSQL();
                string inString = "";
                this.ExecuteQuery(out dataSet, "ID_UO", commandString);
                foreach (DataRow rowIdUo in dataSet.Tables["ID_UO"].Rows)
                {
                    if (!inString.Equals(string.Empty))
                        inString += ", ";

                    inString = rowIdUo["SYSTEM_ID"].ToString();
                }

                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__TIPO_RUOLO");
                q.setParam("param1", "A.SYSTEM_ID,A.VAR_DESC_CORR,A.ID_OLD,A.DTA_INIZIO,DTA_FINE,A.ID_PARENT,A.VAR_CODICE,A.ID_TIPO_RUOLO,A.VAR_NOME,A.VAR_COGNOME,A.ID_PEOPLE,A.CHA_TIPO_URP,A.CHA_TIPO_IE,A.CHA_TIPO_CORR,A.CHA_PA,A.VAR_COD_RUBRICA,A.ID_AMM,A.ID_GRUPPO,A.VAR_CODICE_AOO,A.VAR_CODICE_AMM,A.VAR_SMTP,A.NUM_PORTA_SMTP,A.ID_REGISTRO,A.CHA_DETTAGLI,A.ID_UO,A.CHA_RIFERIMENTO,A.VAR_EMAIL,A.VAR_CODICE_ISTAT,B.NUM_LIVELLO AS NUM_LIV_B,B.VAR_DESC_RUOLO");
                q.setParam("param2", "A.ID_TIPO_RUOLO=B.SYSTEM_ID AND UPPER(A.VAR_DESC_CORR) LIKE '%" + objQueryCorrispondente.descrizioneRuolo.ToUpper() + "%' AND A.ID_UO IN (" + inString + ") AND CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND A.ID_AMM = '" + objQueryCorrispondente.idAmministrazione + "' ");
                commandString = q.getSQL();
                logger.Debug("commandString_ruolo=" + commandString);
                this.ExecuteQuery(out dataSet, "CORRISPONDENTI", commandString);
                ArrayList list = new ArrayList();
                foreach (DataRow corrRow in dataSet.Tables["CORRISPONDENTI"].Rows)
                {
                    RicercaUOParentInt(dataSet, corrRow);
                    if (dataSet != null)
                    {
                        if (corrRow["CHA_TIPO_URP"].ToString().Equals("R"))
                        {
                            DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                            corrispondenteRuolo.systemId = corrRow["SYSTEM_ID"].ToString();

                            corrispondenteRuolo.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                            corrispondenteRuolo.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                            corrispondenteRuolo.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                            corrispondenteRuolo.idAmministrazione = corrRow["ID_AMM"].ToString();

                            corrispondenteRuolo.livello = corrRow["NUM_LIV_B"].ToString();
                            corrispondenteRuolo.idGruppo = corrRow["ID_GRUPPO"].ToString();
                            corrispondenteRuolo.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                            corrispondenteRuolo.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                            DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                            sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                            sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                            corrispondenteRuolo.serverPosta = sp;
                            corrispondenteRuolo.tipoIE = "I";
                            if (corrRow["ID_REGISTRO"] != null)
                            {
                                corrispondenteRuolo.idRegistro = corrRow["ID_REGISTRO"].ToString();
                            }
                            //viene calcolata la parentela del mittente del protocollo ricevuto per interoperabilità
                            if (objQueryCorrispondente.isUODefined())
                            {
                                if ((objQueryCorrispondente.descrizioneUO != null && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])))
                                {
                                    corrispondenteRuolo.uo = GetParents(corrRow["ID_UO"].ToString(), dataSet.Tables["UO"]);
                                    list.Add(corrispondenteRuolo);
                                }
                            }
                            else
                            {
                                corrispondenteRuolo.uo = GetParents(corrRow["ID_UO"].ToString(), dataSet.Tables["UO"]);
                                list.Add(corrispondenteRuolo);
                            }
                        }
                    }

                    listaCorr = list;
                }
            }

            return listaCorr;
        }


        DocsPaVO.utente.UnitaOrganizzativa get_uo_by_system_id(string id_amm, string system_id)
        {
            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
            qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
            qco.systemId = system_id;
            qco.idAmministrazione = id_amm;
            qco.fineValidita = true;

            return (DocsPaVO.utente.UnitaOrganizzativa)GetCorrispondenteInt(qco);
        }

        /// <summary>
        /// ritorna un corrispondente interno. In caso di utente, se il parametro notDisabled=true ritornano solo utenti non disabilitati.
        /// </summary>
        /// <param name="qco"></param>
        /// <param name="disabled"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Corrispondente GetCorrispondenteInt(DocsPaVO.addressbook.QueryCorrispondente qco, bool notDisabled)
        {
            DataSet dataSet;
            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);
            DocsPaVO.utente.Corrispondente result = new DocsPaVO.utente.Corrispondente();
            DocsPaVO.utente.ServerPosta sp = null;
            try
            {
                DocsPaUtils.Query q;
                q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_CORRISPONDENTE_SEMPLICE");
                q.setParam("param1", qco.systemId);
                q.setParam("param2", qco.idAmministrazione);
                string command = q.getSQL();

                if (!this.ExecuteQuery(out dataSet, command)) throw new Exception();

                if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow corrRow = dataSet.Tables[0].Rows[0];

                    switch (corrRow["CHA_TIPO_URP"].ToString())
                    {
                        case "U": // UO
                            DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                            corrispondenteUO.systemId = corrRow["SYSTEM_ID"].ToString();
                            corrispondenteUO.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                            corrispondenteUO.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                            corrispondenteUO.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                            String url = corrRow["InteropUrl"].ToString();
                            corrispondenteUO.Url = new List<Corrispondente.UrlInfo>();
                            if (!String.IsNullOrEmpty(url))
                                corrispondenteUO.Url.Add(new Corrispondente.UrlInfo() { Url = url });

                            if (corrRow["ID_REGISTRO"] != null)
                            {
                                corrispondenteUO.idRegistro = corrRow["ID_REGISTRO"].ToString();
                            }
                            corrispondenteUO.email = corrRow["VAR_EMAIL"].ToString();
                            corrispondenteUO.interoperante = FromCharToBool(corrRow["CHA_PA"].ToString());
                            corrispondenteUO.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                            corrispondenteUO.livello = corrRow["NUM_LIVELLO"].ToString();
                            sp = new DocsPaVO.utente.ServerPosta();
                            sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                            sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                            corrispondenteUO.serverPosta = sp;
                            corrispondenteUO.idAmministrazione = corrRow["ID_AMM"].ToString();
                            corrispondenteUO.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                            corrispondenteUO.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                            corrispondenteUO.codiceIstat = corrRow["VAR_CODICE_ISTAT"].ToString();
                            corrispondenteUO.tipoIE = "I";
                            if (corrRow["DTA_FINE"] != null)
                                corrispondenteUO.dta_fine = corrRow["DTA_FINE"].ToString();//Fede 24 nov 05
                            //qui si ritrova la parentela
                            if (!corrRow["ID_PARENT"].ToString().Equals("0"))
                            {
                                DocsPaVO.utente.UnitaOrganizzativa uon = new DocsPaVO.utente.UnitaOrganizzativa();
                                uon.systemId = corrRow["ID_PARENT"].ToString();
                                corrispondenteUO.parent = uon;//=GetParents(corrRow["ID_PARENT"].ToString(),dataSet.Tables["UO"]);
                            }

                            result = (DocsPaVO.utente.Corrispondente)corrispondenteUO;
                            result.tipoCorrispondente = "U";

                            //popolo il canale preferenziale per l'UO
                            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                            corrispondenteUO.canalePref = amm.GetDatiCanPref(corrispondenteUO);

                            if (corrRow.Table.Columns.Contains("CHA_DISABLED_TRASM") && corrRow["CHA_DISABLED_TRASM"] != null && corrRow["CHA_DISABLED_TRASM"].ToString() == "1")
                            {
                                corrispondenteUO.disabledTrasm = true;
                            }

                            break;
                        case "R": // Ruolo
                            DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                            corrispondenteRuolo.systemId = corrRow["SYSTEM_ID"].ToString();
                            //corrispondenteRuolo.descrizione=corrRow["VAR_DESC_RUOLO"].ToString();
                            corrispondenteRuolo.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                            corrispondenteRuolo.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                            corrispondenteRuolo.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                            corrispondenteRuolo.idAmministrazione = corrRow["ID_AMM"].ToString();
                            //corrispondenteRuolo.livello=corrRow["RUOLO_LIVELLO"].ToString(); 

                            corrispondenteRuolo.idGruppo = corrRow["ID_GRUPPO"].ToString();
                            corrispondenteRuolo.livello = (new Amministrazione()).GetDatiTipoRuolo(corrRow["ID_TIPO_RUOLO"].ToString()).livello;

                            corrispondenteRuolo.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                            corrispondenteRuolo.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                            sp = new DocsPaVO.utente.ServerPosta();
                            sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                            sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                            corrispondenteRuolo.serverPosta = sp;
                            corrispondenteRuolo.tipoIE = "I";
                            if (corrRow["DTA_FINE"] != null)
                                corrispondenteRuolo.dta_fine = corrRow["DTA_FINE"].ToString();//Fede 24 nov 05
                            if (corrRow["ID_REGISTRO"] != null)
                            {
                                corrispondenteRuolo.idRegistro = corrRow["ID_REGISTRO"].ToString();
                            }
                            corrispondenteRuolo.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                            //qui si ritrova la parentela (con filtro o no)
                            if (objQueryCorrispondente.isUODefined())
                            {
                                if ((objQueryCorrispondente.descrizioneUO != null && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])))
                                {
                                    corrispondenteRuolo.uo = get_uo_by_system_id(corrRow["ID_AMM"].ToString(), corrRow["ID_UO"].ToString());
                                    result = (DocsPaVO.utente.Corrispondente)corrispondenteRuolo;
                                    result.tipoCorrispondente = "R";
                                }
                            }
                            else
                            {
                                corrispondenteRuolo.uo = get_uo_by_system_id(corrRow["ID_AMM"].ToString(), corrRow["ID_UO"].ToString());
                                result = (DocsPaVO.utente.Corrispondente)corrispondenteRuolo;
                                result.tipoCorrispondente = "R";
                            }


                            if (corrRow.Table.Columns.Contains("CHA_DISABLED_TRASM") && corrRow["CHA_DISABLED_TRASM"] != null && corrRow["CHA_DISABLED_TRASM"].ToString() == "1")
                            {
                                corrispondenteRuolo.disabledTrasm = true;
                            }

                            break;
                        case "P": // Persona
                            DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();
                            corrispondenteUtente.systemId = corrRow["SYSTEM_ID"].ToString();
                            corrispondenteUtente.idPeople = corrRow["ID_PEOPLE"].ToString();
                            corrispondenteUtente.descrizione = corrRow["VAR_COGNOME"].ToString() + " " + corrRow["VAR_NOME"].ToString();
                            corrispondenteUtente.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                            corrispondenteUtente.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                            corrispondenteUtente.idAmministrazione = corrRow["ID_AMM"].ToString();
                            corrispondenteUtente.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                            corrispondenteUtente.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                            corrispondenteUtente.nome = corrRow["VAR_NOME"].ToString();
                            corrispondenteUtente.cognome = corrRow["VAR_COGNOME"].ToString();
                            if (corrRow["DTA_FINE"] != null)
                            {
                                corrispondenteUtente.dta_fine = corrRow["DTA_FINE"].ToString();//Fede 24 nov 05
                            }
                            if (corrRow["ID_REGISTRO"] != null)
                            {
                                corrispondenteUtente.idRegistro = corrRow["ID_REGISTRO"].ToString();
                            }

                            corrispondenteUtente.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                            sp = new DocsPaVO.utente.ServerPosta();
                            sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                            sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                            corrispondenteUtente.serverPosta = sp;
                            corrispondenteUtente.email = corrRow["VAR_EMAIL"].ToString();
                            DocsPaVO.utente.Utente ut = null;
                            if (notDisabled) //ricerco solo utenti non disabilitati.
                                ut = GetUtente(corrRow["ID_PEOPLE"].ToString());
                            else //ricerco tutti gli utenti.
                                ut = GetUtenteNoFiltroDisabled(corrRow["ID_PEOPLE"].ToString());
                            corrispondenteUtente.notifica = ut.notifica;
                            corrispondenteUtente.notificaConAllegato = ut.notificaConAllegato;

                            corrispondenteUtente.nome = ut.nome;
                            corrispondenteUtente.cognome = ut.cognome;

                            corrispondenteUtente.tipoIE = "I";
                            result = (DocsPaVO.utente.Corrispondente)corrispondenteUtente;
                            result.tipoCorrispondente = "P";
                            result.nome = corrispondenteUtente.nome;
                            result.cognome = corrispondenteUtente.cognome;

                            if (corrRow.Table.Columns.Contains("CHA_DISABLED_TRASM") && corrRow["CHA_DISABLED_TRASM"] != null && corrRow["CHA_DISABLED_TRASM"].ToString() == "1")
                            {
                                corrispondenteUtente.disabledTrasm = true;
                            }

                            break;
                    }
                }

            }
            catch (Exception e)
            {
                logger.Debug("ERROR : ListaCorrispondentiInt", e);
                result = null;
            }

            return result;
        }

        public DocsPaVO.utente.Corrispondente GetCorrispondenteInt_NEW(DataRow corrRow, DocsPaVO.addressbook.QueryCorrispondente qco, bool notDisabled, DataSet dataSet)
        {
            //DataSet dataSet;
            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);
            DocsPaVO.utente.Corrispondente result = new DocsPaVO.utente.Corrispondente();
            DocsPaVO.utente.ServerPosta sp = null;
            try
            {
                switch (corrRow["CHA_TIPO_URP"].ToString())
                {
                    case "U": // UO
                        DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                        corrispondenteUO.systemId = corrRow["SYSTEM_ID"].ToString();
                        corrispondenteUO.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                        corrispondenteUO.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                        corrispondenteUO.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                        if (corrRow["ID_REGISTRO"] != null)
                        {
                            corrispondenteUO.idRegistro = corrRow["ID_REGISTRO"].ToString();
                        }
                        corrispondenteUO.email = corrRow["VAR_EMAIL"].ToString();
                        corrispondenteUO.interoperante = FromCharToBool(corrRow["CHA_PA"].ToString());
                        corrispondenteUO.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                        corrispondenteUO.livello = corrRow["NUM_LIVELLO"].ToString();
                        sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                        sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                        corrispondenteUO.serverPosta = sp;
                        corrispondenteUO.idAmministrazione = corrRow["ID_AMM"].ToString();
                        corrispondenteUO.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                        corrispondenteUO.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                        corrispondenteUO.codiceIstat = corrRow["VAR_CODICE_ISTAT"].ToString();
                        corrispondenteUO.tipoIE = "I";
                        if (corrRow["DTA_FINE"] != null)
                            corrispondenteUO.dta_fine = corrRow["DTA_FINE"].ToString();//Fede 24 nov 05
                        //qui si ritrova la parentela
                        if (!corrRow["ID_PARENT"].ToString().Equals("0"))
                        {
                            DocsPaVO.utente.UnitaOrganizzativa uon = new DocsPaVO.utente.UnitaOrganizzativa();
                            uon.systemId = corrRow["ID_PARENT"].ToString();
                            corrispondenteUO.parent = uon;//=GetParents(corrRow["ID_PARENT"].ToString(),dataSet.Tables["UO"]);
                        }
                        result = (DocsPaVO.utente.Corrispondente)corrispondenteUO;
                        result.tipoCorrispondente = "U";

                        if (corrRow.Table.Columns.Contains("CHA_DISABLED_TRASM") && corrRow["CHA_DISABLED_TRASM"] != null && corrRow["CHA_DISABLED_TRASM"].ToString() == "1")
                        {
                            corrispondenteUO.disabledTrasm = true;
                        }

                        break;
                    case "R": // Ruolo
                        DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                        corrispondenteRuolo.systemId = corrRow["SYSTEM_ID"].ToString();
                        corrispondenteRuolo.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                        corrispondenteRuolo.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                        corrispondenteRuolo.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                        corrispondenteRuolo.idAmministrazione = corrRow["ID_AMM"].ToString();
                        corrispondenteRuolo.idGruppo = corrRow["ID_GRUPPO"].ToString();
                        corrispondenteRuolo.livello = (new Amministrazione()).GetDatiTipoRuolo(corrRow["ID_TIPO_RUOLO"].ToString()).livello;
                        corrispondenteRuolo.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                        corrispondenteRuolo.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                        sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                        sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                        corrispondenteRuolo.serverPosta = sp;
                        corrispondenteRuolo.tipoIE = "I";
                        if (corrRow["DTA_FINE"] != null)
                            corrispondenteRuolo.dta_fine = corrRow["DTA_FINE"].ToString();//Fede 24 nov 05
                        if (corrRow["ID_REGISTRO"] != null)
                        {
                            corrispondenteRuolo.idRegistro = corrRow["ID_REGISTRO"].ToString();
                        }
                        corrispondenteRuolo.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                        //qui si ritrova la parentela (con filtro o no)
                        if (objQueryCorrispondente.isUODefined())
                        {
                            if ((objQueryCorrispondente.descrizioneUO != null && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])))
                            {
                                corrispondenteRuolo.uo = get_uo_by_system_id(corrRow["ID_AMM"].ToString(), corrRow["ID_UO"].ToString());
                                result = (DocsPaVO.utente.Corrispondente)corrispondenteRuolo;
                                result.tipoCorrispondente = "R";
                            }
                        }
                        else
                        {
                            corrispondenteRuolo.uo = get_uo_by_system_id(corrRow["ID_AMM"].ToString(), corrRow["ID_UO"].ToString());
                            result = (DocsPaVO.utente.Corrispondente)corrispondenteRuolo;
                            result.tipoCorrispondente = "R";
                        }

                        if (corrRow.Table.Columns.Contains("CHA_DISABLED_TRASM") && corrRow["CHA_DISABLED_TRASM"] != null && corrRow["CHA_DISABLED_TRASM"].ToString() == "1")
                        {
                            corrispondenteRuolo.disabledTrasm = true;
                        }

                        break;
                    case "P": // Persona
                        DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();
                        corrispondenteUtente.systemId = corrRow["SYSTEM_ID"].ToString();
                        corrispondenteUtente.idPeople = corrRow["ID_PEOPLE"].ToString();
                        corrispondenteUtente.descrizione = corrRow["VAR_COGNOME"].ToString() + " " + corrRow["VAR_NOME"].ToString();
                        corrispondenteUtente.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                        corrispondenteUtente.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                        corrispondenteUtente.idAmministrazione = corrRow["ID_AMM"].ToString();
                        corrispondenteUtente.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                        corrispondenteUtente.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                        corrispondenteUtente.nome = corrRow["VAR_NOME"].ToString();
                        corrispondenteUtente.cognome = corrRow["VAR_COGNOME"].ToString();
                        if (corrRow["DTA_FINE"] != null)
                        {
                            corrispondenteUtente.dta_fine = corrRow["DTA_FINE"].ToString();//Fede 24 nov 05
                        }
                        if (corrRow["ID_REGISTRO"] != null)
                        {
                            corrispondenteUtente.idRegistro = corrRow["ID_REGISTRO"].ToString();
                        }

                        corrispondenteUtente.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                        sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                        sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                        corrispondenteUtente.serverPosta = sp;
                        corrispondenteUtente.email = corrRow["VAR_EMAIL"].ToString();
                        DocsPaVO.utente.Utente ut = null;
                        if (notDisabled) //ricerco solo utenti non disabilitati.
                            ut = GetUtente(corrRow["ID_PEOPLE"].ToString());
                        else //ricerco tutti gli utenti.
                            ut = GetUtenteNoFiltroDisabled(corrRow["ID_PEOPLE"].ToString());
                        corrispondenteUtente.notifica = ut.notifica;
                        corrispondenteUtente.notificaConAllegato = ut.notificaConAllegato;

                        corrispondenteUtente.tipoIE = "I";
                        result = (DocsPaVO.utente.Corrispondente)corrispondenteUtente;
                        result.tipoCorrispondente = "P";
                        result.nome = corrispondenteUtente.nome;
                        result.cognome = corrispondenteUtente.cognome;

                        if (corrRow.Table.Columns.Contains("CHA_DISABLED_TRASM") && corrRow["CHA_DISABLED_TRASM"] != null && corrRow["CHA_DISABLED_TRASM"].ToString() == "1")
                        {
                            corrispondenteUtente.disabledTrasm = true;
                        }
                        break;

                }


            }
            catch (Exception e)
            {
                logger.Debug("ERROR : ListaCorrispondentiInt", e);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Creazione oggetto UnitaOrganizzativa
        /// </summary>
        /// <param name="corrRow"></param>
        /// <returns></returns>
        private DocsPaVO.utente.UnitaOrganizzativa GetUO(DataRow corrRow)
        {
            DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();

            corrispondenteUO.tipoCorrispondente = "U";
            corrispondenteUO.systemId = corrRow["SYSTEM_ID"].ToString();
            corrispondenteUO.descrizione = corrRow["VAR_DESC_CORR"].ToString();
            corrispondenteUO.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
            corrispondenteUO.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
            if (corrRow["ID_REGISTRO"] != null)
            {
                corrispondenteUO.idRegistro = corrRow["ID_REGISTRO"].ToString();
            }
            corrispondenteUO.email = corrRow["VAR_EMAIL"].ToString();
            corrispondenteUO.interoperante = FromCharToBool(corrRow["CHA_PA"].ToString());
            corrispondenteUO.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
            corrispondenteUO.livello = corrRow["NUM_LIVELLO"].ToString();
            DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
            sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
            sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
            corrispondenteUO.serverPosta = sp;
            corrispondenteUO.idAmministrazione = corrRow["ID_AMM"].ToString();
            corrispondenteUO.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
            corrispondenteUO.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
            corrispondenteUO.codiceIstat = corrRow["VAR_CODICE_ISTAT"].ToString();
            //18 nov 2005 - BUG ANAS 1501: corrispondenteUO.tipoIE="I";
            corrispondenteUO.tipoIE = corrRow["CHA_TIPO_IE"].ToString();
            //corrispondenteUO.tipoIE = "E";

            // Indica se il corrispondente è proveniente da rubrica comune
            corrispondenteUO.inRubricaComune = (corrRow["CHA_TIPO_CORR"] != DBNull.Value && corrRow["CHA_TIPO_CORR"].ToString() == "C");

            //popolo il canale preferenziale per il corrisp Esterno
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            corrispondenteUO.canalePref = amm.GetDatiCanPref(corrispondenteUO);


            //DATA FINE PER CORRISPONDENTI ESTERNI
            if (corrRow.Table.Columns.Contains("ID_OLD") && corrRow["ID_OLD"] != DBNull.Value)
                corrispondenteUO.idOld = corrRow["ID_OLD"].ToString();

            corrispondenteUO.dta_fine = corrRow["DTA_FINE"].ToString();
            //
            //qui si ritrova la parentela
            if (!corrRow["ID_PARENT"].ToString().Equals("0"))
            {
                DocsPaVO.utente.UnitaOrganizzativa uon = new DocsPaVO.utente.UnitaOrganizzativa();
                uon.systemId = corrRow["ID_PARENT"].ToString();
                corrispondenteUO.parent = uon;//=GetParents(corrRow["ID_PARENT"].ToString(),dataSet.Tables["UO"]);
            }

            // Impostazione dell'url per l'interoperabilità semplificata
            String url = corrRow["InteropUrl"].ToString();
            corrispondenteUO.Url = new List<Corrispondente.UrlInfo>();
            if (!String.IsNullOrEmpty(url))
                corrispondenteUO.Url.Add(new Corrispondente.UrlInfo() { Url = url });

            return corrispondenteUO;
        }

        /// <summary>
        /// Creazione oggetto RF
        /// </summary>
        /// <param name="corrRow"></param>
        /// <returns></returns>
        private DocsPaVO.utente.Corrispondente GetRF(DataRow corrRow)
        {
            DocsPaVO.utente.Corrispondente corrispondenteRF = new RaggruppamentoFunzionale();

            corrispondenteRF.tipoCorrispondente = "F";
            corrispondenteRF.systemId = corrRow["SYSTEM_ID"].ToString();
            corrispondenteRF.descrizione = corrRow["VAR_DESC_CORR"].ToString();
            corrispondenteRF.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
            corrispondenteRF.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
            if (corrRow["ID_REGISTRO"] != null)
            {
                corrispondenteRF.idRegistro = corrRow["ID_REGISTRO"].ToString();
            }
            corrispondenteRF.email = corrRow["VAR_EMAIL"].ToString();
            corrispondenteRF.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
            DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
            sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
            sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
            corrispondenteRF.serverPosta = sp;
            corrispondenteRF.idAmministrazione = corrRow["ID_AMM"].ToString();
            corrispondenteRF.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
            corrispondenteRF.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
            //18 nov 2005 - BUG ANAS 1501: corrispondenteUO.tipoIE="I";
            corrispondenteRF.tipoIE = corrRow["CHA_TIPO_IE"].ToString();
            //corrispondenteUO.tipoIE = "E";

            // Indica se il corrispondente è proveniente da rubrica comune
            corrispondenteRF.inRubricaComune = (corrRow["CHA_TIPO_CORR"] != DBNull.Value && corrRow["CHA_TIPO_CORR"].ToString() == "C");

            //popolo il canale preferenziale per il corrisp Esterno
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            corrispondenteRF.canalePref = amm.GetDatiCanPref(corrispondenteRF);


            //DATA FINE PER CORRISPONDENTI ESTERNI
            if (corrRow.Table.Columns.Contains("ID_OLD") && corrRow["ID_OLD"] != DBNull.Value)
                corrispondenteRF.idOld = corrRow["ID_OLD"].ToString();

            corrispondenteRF.dta_fine = corrRow["DTA_FINE"].ToString();

            // Impostazione dell'url per l'interoperabilità semplificata
            corrispondenteRF.Url = new List<Corrispondente.UrlInfo>();
            String url = corrRow["InteropUrl"].ToString();
            if (!String.IsNullOrEmpty(url))
                corrispondenteRF.Url.Add(new Corrispondente.UrlInfo() { Url = url });

            //DataSet emails = this.GetMailCorr(corrispondenteRF.systemId);
            //corrispondenteRF.Emails = new List<MailCorrispondente>();
            //if(emails != null && emails.Tables != null && emails.Tables[0].Rows.Count > 0)
            //    foreach (DataRow item in emails.Tables[0].Rows)
            //    {
            //        corrispondenteRF.Emails.Add(new MailCorrispondente()
            //        {
            //            Email = item["Email"].ToString(),
            //            Note = item["Note"].ToString(),
            //            Principale = item["Principale"].ToString(),
            //            systemId = item["SystemId"].ToString()
            //        });

            //    }

            return corrispondenteRF;
        }

        public DocsPaVO.utente.Corrispondente GetCorrispondenteRF(DocsPaVO.addressbook.QueryCorrispondente qco, string tipo)
        {
            DataSet dataSet;
            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);
            DocsPaVO.utente.Corrispondente result = new DocsPaVO.utente.Corrispondente();
            try
            {
                DocsPaUtils.Query q;
                q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_CORRISPONDENTE_RF");
                q.setParam("param1", qco.systemId);
                q.setParam("param2", qco.idAmministrazione);

                string command = q.getSQL();

                if (!this.ExecuteQuery(out dataSet, command)) throw new Exception();

                if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow corrRow = dataSet.Tables[0].Rows[0];
                    DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                    corrispondenteRuolo.systemId = corrRow["SYSTEM_ID"].ToString();
                    corrispondenteRuolo.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                    corrispondenteRuolo.codice = corrRow["VAR_CODICE"].ToString();
                    corrispondenteRuolo.idRegistro = corrRow["ID_RF"].ToString();
                    corrispondenteRuolo.codiceAOO = corrRow["ID_REGISTRO"].ToString();
                    corrispondenteRuolo.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                    result = (DocsPaVO.utente.Corrispondente)corrispondenteRuolo;
                    result.tipoCorrispondente = tipo.ToUpper();
                }
            }
            catch (Exception e)
            {
                logger.Debug("ERROR : ListaCorrispondentiInt", e);
                result = null;
            }

            return result;
        }

        public DocsPaVO.utente.Corrispondente GetCorrispondenteEst(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            DataSet dataSet;
            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);
            DocsPaVO.utente.Corrispondente result = new DocsPaVO.utente.Corrispondente();
            DocsPaVO.utente.ServerPosta sp = null;
            try
            {
                DocsPaUtils.Query q;
                q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_CORRISPONDENTE_SEMPLICE_EST");
                q.setParam("param1", qco.systemId);
                q.setParam("param2", qco.idAmministrazione);

                string command = q.getSQL();

                if (!this.ExecuteQuery(out dataSet, command)) throw new Exception();

                if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow corrRow = dataSet.Tables[0].Rows[0];

                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

                    switch (corrRow["CHA_TIPO_URP"].ToString())
                    {
                        case "U": // UO
                            result = GetUO(corrRow);

                            break;
                        case "R": // Ruolo
                            DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                            corrispondenteRuolo.systemId = corrRow["SYSTEM_ID"].ToString();
                            //corrispondenteRuolo.descrizione=corrRow["VAR_DESC_RUOLO"].ToString();
                            corrispondenteRuolo.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                            corrispondenteRuolo.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                            corrispondenteRuolo.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                            corrispondenteRuolo.idAmministrazione = corrRow["ID_AMM"].ToString();
                            //corrispondenteRuolo.livello=corrRow["RUOLO_LIVELLO"].ToString(); 
                            corrispondenteRuolo.livello = corrRow["NUM_LIVELLO"].ToString();
                            corrispondenteRuolo.idGruppo = corrRow["ID_GRUPPO"].ToString();
                            corrispondenteRuolo.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                            corrispondenteRuolo.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                            sp = new DocsPaVO.utente.ServerPosta();
                            sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                            sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                            corrispondenteRuolo.serverPosta = sp;

                            //18 nov 2005 - BUG ANAS 1501: corrispondenteRuolo.tipoIE="I";
                            corrispondenteRuolo.tipoIE = "E";

                            //popolo il canale preferenziale per il corrisp Esterno
                            corrispondenteRuolo.canalePref = amm.GetDatiCanPref(corrispondenteRuolo);

                            //DATA FINE PER CORRISPONDENTI ESTERNI
                            corrispondenteRuolo.dta_fine = corrRow["DTA_FINE"].ToString();

                            if (corrRow["ID_REGISTRO"] != null)
                            {
                                corrispondenteRuolo.idRegistro = corrRow["ID_REGISTRO"].ToString();
                            }
                            corrispondenteRuolo.email = corrRow["VAR_EMAIL"].ToString();
                            corrispondenteRuolo.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                            //qui si ritrova la parentela (con filtro o no)
                            if (objQueryCorrispondente.isUODefined())
                            {
                                if ((objQueryCorrispondente.descrizioneUO != null && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])))
                                {
                                    DocsPaVO.utente.UnitaOrganizzativa uon = new DocsPaVO.utente.UnitaOrganizzativa();
                                    uon.systemId = corrRow["ID_PARENT"].ToString();
                                    corrispondenteRuolo.uo = uon;
                                    result = (DocsPaVO.utente.Corrispondente)corrispondenteRuolo;
                                    result.tipoCorrispondente = "R";
                                }
                            }
                            else
                            {
                                DocsPaVO.utente.UnitaOrganizzativa uon = new DocsPaVO.utente.UnitaOrganizzativa();
                                uon.systemId = corrRow["ID_PARENT"].ToString();
                                corrispondenteRuolo.uo = uon;
                                result = (DocsPaVO.utente.Corrispondente)corrispondenteRuolo;
                                result.tipoCorrispondente = "R";
                            }

                            break;
                        case "P": // Persona
                            DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();
                            corrispondenteUtente.systemId = corrRow["SYSTEM_ID"].ToString();

                            //la descrizione cambia nel caso in cui sia presente la qualifica dell'utente
                            corrispondenteUtente.idPeople = corrRow["ID_PEOPLE"].ToString();
                            if (corrRow["VAR_DESC_CORR"] != null && !corrRow["VAR_DESC_CORR"].ToString().StartsWith(corrRow["VAR_COGNOME"].ToString()))
                            {
                                corrispondenteUtente.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                            }
                            else
                            {
                                corrispondenteUtente.descrizione = corrRow["VAR_COGNOME"].ToString() + " " + corrRow["VAR_NOME"].ToString();
                            }
                            corrispondenteUtente.cognome = corrRow["VAR_COGNOME"].ToString();
                            corrispondenteUtente.nome = corrRow["VAR_NOME"].ToString();
                            corrispondenteUtente.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                            corrispondenteUtente.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                            corrispondenteUtente.idAmministrazione = corrRow["ID_AMM"].ToString();
                            corrispondenteUtente.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                            corrispondenteUtente.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();

                            if (corrRow["ID_REGISTRO"] != null)
                            {
                                corrispondenteUtente.idRegistro = corrRow["ID_REGISTRO"].ToString();
                            }

                            corrispondenteUtente.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                            sp = new DocsPaVO.utente.ServerPosta();
                            sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                            sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                            corrispondenteUtente.serverPosta = sp;
                            corrispondenteUtente.email = corrRow["VAR_EMAIL"].ToString();

                            //18 nov 2005 - BUG ANAS 1501: corrispondenteUtente.tipoIE="I";
                            corrispondenteUtente.tipoIE = "E";

                            //popolo il canale preferenziale per il corrisp Esterno
                            corrispondenteUtente.canalePref = amm.GetDatiCanPref(corrispondenteUtente);

                            //DATA FINE PER CORRISPONDENTI ESTERNI
                            corrispondenteUtente.dta_fine = corrRow["DTA_FINE"].ToString();

                            result = (DocsPaVO.utente.Corrispondente)corrispondenteUtente;
                            result.tipoCorrispondente = "P";
                            result.nome = corrispondenteUtente.nome;
                            result.cognome = corrispondenteUtente.cognome;
                            break;
                        case "F":  // Corrispondente esterno di tipo RF 
                            result = GetRF(corrRow);
                            break;

                    }

                    //recupero l'elenco delle mail associate al corrispondente
                    DataSet ds = GetMailCorr(result.systemId);
                    if (ds != null && ds.Tables["CASELLE_CORRISPONDENTE"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["CASELLE_CORRISPONDENTE"].Rows)
                        {
                            DocsPaVO.utente.MailCorrispondente mailCorr = new DocsPaVO.utente.MailCorrispondente();
                            mailCorr.systemId = row["SystemId"].ToString();
                            mailCorr.Email = row["Email"].ToString();
                            mailCorr.Principale = row["Principale"].ToString();
                            mailCorr.Note = row["Note"].ToString();
                            result.Emails.Add(mailCorr);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("ERROR : ListaCorrispondentiInt", e);
                result = null;
            }

            return result;
        }

        public DocsPaVO.utente.Corrispondente GetCorrispondenteEst_NEW(DataRow corrRow, DocsPaVO.addressbook.QueryCorrispondente qco, DataSet dataSet)
        {
            //DataSet dataSet;
            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);
            DocsPaVO.utente.Corrispondente result = new DocsPaVO.utente.Corrispondente();
            DocsPaVO.utente.ServerPosta sp = null;
            try
            {
                //DocsPaUtils.Query q;
                //q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_CORRISPONDENTE_SEMPLICE_EST");
                //q.setParam("param1", qco.systemId);
                //q.setParam("param2", qco.idAmministrazione);

                //string command = q.getSQL();

                //if (!this.ExecuteQuery(out dataSet, command)) throw new Exception();

                //if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
                //{
                //    DataRow corrRow = dataSet.Tables[0].Rows[0];

                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

                switch (corrRow["CHA_TIPO_URP"].ToString())
                {
                    case "U": // UO
                        result = GetUO(corrRow);

                        break;
                    case "F":   // RF
                        result = GetRF(corrRow);
                        break;
                    case "R": // Ruolo
                        DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                        corrispondenteRuolo.systemId = corrRow["SYSTEM_ID"].ToString();
                        //corrispondenteRuolo.descrizione=corrRow["VAR_DESC_RUOLO"].ToString();
                        corrispondenteRuolo.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                        corrispondenteRuolo.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                        corrispondenteRuolo.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                        corrispondenteRuolo.idAmministrazione = corrRow["ID_AMM"].ToString();
                        //corrispondenteRuolo.livello=corrRow["RUOLO_LIVELLO"].ToString(); 
                        corrispondenteRuolo.livello = corrRow["NUM_LIVELLO"].ToString();
                        corrispondenteRuolo.idGruppo = corrRow["ID_GRUPPO"].ToString();
                        corrispondenteRuolo.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                        corrispondenteRuolo.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                        sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                        sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                        corrispondenteRuolo.serverPosta = sp;

                        //18 nov 2005 - BUG ANAS 1501: corrispondenteRuolo.tipoIE="I";
                        corrispondenteRuolo.tipoIE = "E";

                        //popolo il canale preferenziale per il corrisp Esterno
                        corrispondenteRuolo.canalePref = amm.GetDatiCanPref(corrispondenteRuolo);

                        //DATA FINE PER CORRISPONDENTI ESTERNI
                        corrispondenteRuolo.dta_fine = corrRow["DTA_FINE"].ToString();

                        if (corrRow["ID_REGISTRO"] != null)
                        {
                            corrispondenteRuolo.idRegistro = corrRow["ID_REGISTRO"].ToString();
                        }
                        corrispondenteRuolo.email = corrRow["VAR_EMAIL"].ToString();
                        corrispondenteRuolo.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                        //qui si ritrova la parentela (con filtro o no)
                        if (objQueryCorrispondente.isUODefined())
                        {
                            if ((objQueryCorrispondente.descrizioneUO != null && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])))
                            {
                                DocsPaVO.utente.UnitaOrganizzativa uon = new DocsPaVO.utente.UnitaOrganizzativa();
                                uon.systemId = corrRow["ID_PARENT"].ToString();
                                corrispondenteRuolo.uo = uon;
                                result = (DocsPaVO.utente.Corrispondente)corrispondenteRuolo;
                                result.tipoCorrispondente = "R";
                            }
                        }
                        else
                        {
                            DocsPaVO.utente.UnitaOrganizzativa uon = new DocsPaVO.utente.UnitaOrganizzativa();
                            uon.systemId = corrRow["ID_PARENT"].ToString();
                            corrispondenteRuolo.uo = uon;
                            result = (DocsPaVO.utente.Corrispondente)corrispondenteRuolo;
                            result.tipoCorrispondente = "R";
                        }

                        break;
                    case "P": // Persona
                        DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();
                        corrispondenteUtente.systemId = corrRow["SYSTEM_ID"].ToString();
                        corrispondenteUtente.idPeople = corrRow["ID_PEOPLE"].ToString();
                        corrispondenteUtente.descrizione = corrRow["VAR_COGNOME"].ToString() + " " + corrRow["VAR_NOME"].ToString();
                        corrispondenteUtente.cognome = corrRow["VAR_COGNOME"].ToString();
                        corrispondenteUtente.nome = corrRow["VAR_NOME"].ToString();
                        corrispondenteUtente.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                        corrispondenteUtente.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                        corrispondenteUtente.idAmministrazione = corrRow["ID_AMM"].ToString();
                        corrispondenteUtente.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                        corrispondenteUtente.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();

                        if (corrRow["ID_REGISTRO"] != null)
                        {
                            corrispondenteUtente.idRegistro = corrRow["ID_REGISTRO"].ToString();
                        }

                        corrispondenteUtente.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                        sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                        sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                        corrispondenteUtente.serverPosta = sp;
                        corrispondenteUtente.email = corrRow["VAR_EMAIL"].ToString();

                        //18 nov 2005 - BUG ANAS 1501: corrispondenteUtente.tipoIE="I";
                        corrispondenteUtente.tipoIE = "E";

                        //popolo il canale preferenziale per il corrisp Esterno
                        corrispondenteUtente.canalePref = amm.GetDatiCanPref(corrispondenteUtente);

                        //DATA FINE PER CORRISPONDENTI ESTERNI
                        corrispondenteUtente.dta_fine = corrRow["DTA_FINE"].ToString();

                        result = (DocsPaVO.utente.Corrispondente)corrispondenteUtente;
                        result.tipoCorrispondente = "P";
                        break;
                }

                //recupero l'elenco delle mail associate al corrispondente
                DataSet ds = GetMailCorr(result.systemId);
                if (ds != null && ds.Tables["CASELLE_CORRISPONDENTE"].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables["CASELLE_CORRISPONDENTE"].Rows)
                    {
                        DocsPaVO.utente.MailCorrispondente mailCorr = new DocsPaVO.utente.MailCorrispondente();
                        mailCorr.systemId = row["SystemId"].ToString();
                        mailCorr.Email = row["Email"].ToString();
                        mailCorr.Principale = row["Principale"].ToString();
                        mailCorr.Note = row["Note"].ToString();
                        result.Emails.Add(mailCorr);
                    }
                }
                //}
            }
            catch (Exception e)
            {
                logger.Debug("ERROR : ListaCorrispondentiInt", e);

                //database.closeConnection();
                //this.CloseConnection();
                //throw e;
                //				listaCorr = null;
                result = null;
            }

            //			logger.Debug("END : DocsPaDB > Query_DocsPAWS > Utenti > ListaCorrispondentiInt");
            //			return listaCorr;
            return result;
        }

        public DocsPaVO.utente.Corrispondente GetCorrispondente_Experimental(DataRow corrRow, DocsPaVO.addressbook.QueryCorrispondente qco, DataSet dataSet)
        {
            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);

            DocsPaVO.utente.Corrispondente result = new DocsPaVO.utente.Corrispondente();
            DocsPaVO.utente.ServerPosta sp = null;
            try
            {
                //DocsPaUtils.Query q;
                //q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_CORRISPONDENTE_SEMPLICE_EST");
                //q.setParam("param1", qco.systemId);
                //q.setParam("param2", qco.idAmministrazione);

                //string command = q.getSQL();

                //if (!this.ExecuteQuery(out dataSet, command)) throw new Exception();

                //if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
                //{
                //    DataRow corrRow = dataSet.Tables[0].Rows[0];

                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                if (!string.IsNullOrEmpty(corrRow["CHA_TIPO_IE"].ToString()) && corrRow["CHA_TIPO_IE"].ToString().Equals("E"))
                {
                    switch (corrRow["CHA_TIPO_URP"].ToString())
                    {
                        case "U": // UO
                            //result = GetUO(corrRow);
                            DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();

                            corrispondenteUO.tipoCorrispondente = "U";
                            corrispondenteUO.systemId = corrRow["SYSTEM_ID"].ToString();
                            corrispondenteUO.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                            corrispondenteUO.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                            corrispondenteUO.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                            if (corrRow["ID_REGISTRO"] != null)
                            {
                                corrispondenteUO.idRegistro = corrRow["ID_REGISTRO"].ToString();
                            }
                            corrispondenteUO.email = corrRow["VAR_EMAIL"].ToString();
                            corrispondenteUO.interoperante = FromCharToBool(corrRow["CHA_PA"].ToString());
                            corrispondenteUO.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                            corrispondenteUO.livello = corrRow["NUM_LIVELLO"].ToString();
                            DocsPaVO.utente.ServerPosta sp_E_UO = new DocsPaVO.utente.ServerPosta();
                            sp_E_UO.serverSMTP = corrRow["VAR_SMTP"].ToString();
                            sp_E_UO.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                            corrispondenteUO.serverPosta = sp_E_UO;
                            corrispondenteUO.idAmministrazione = corrRow["ID_AMM"].ToString();
                            corrispondenteUO.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                            corrispondenteUO.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                            corrispondenteUO.codiceIstat = corrRow["VAR_CODICE_ISTAT"].ToString();
                            //18 nov 2005 - BUG ANAS 1501: corrispondenteUO.tipoIE="I";
                            corrispondenteUO.tipoIE = corrRow["CHA_TIPO_IE"].ToString();
                            //corrispondenteUO.tipoIE = "E";

                            // Indica se il corrispondente è proveniente da rubrica comune
                            corrispondenteUO.inRubricaComune = (corrRow["CHA_TIPO_CORR"] != DBNull.Value && corrRow["CHA_TIPO_CORR"].ToString() == "C");

                            //popolo il canale preferenziale per il corrisp Esterno
                            //DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                            //corrispondenteUO.canalePref = amm.GetDatiCanPref(corrispondenteUO);


                            //DATA FINE PER CORRISPONDENTI ESTERNI
                            if (corrRow.Table.Columns.Contains("ID_OLD") && corrRow["ID_OLD"] != DBNull.Value)
                                corrispondenteUO.idOld = corrRow["ID_OLD"].ToString();

                            corrispondenteUO.dta_fine = corrRow["DTA_FINE"].ToString();
                            //
                            //qui si ritrova la parentela
                            if (!corrRow["ID_PARENT"].ToString().Equals("0"))
                            {
                                DocsPaVO.utente.UnitaOrganizzativa uon = new DocsPaVO.utente.UnitaOrganizzativa();
                                uon.systemId = corrRow["ID_PARENT"].ToString();
                                corrispondenteUO.parent = uon;//=GetParents(corrRow["ID_PARENT"].ToString(),dataSet.Tables["UO"]);
                            }

                            // Impostazione dell'url per l'interoperabilità semplificata
                            String url = corrRow["InteropUrl"].ToString();
                            corrispondenteUO.Url = new List<Corrispondente.UrlInfo>();
                            if (!String.IsNullOrEmpty(url))
                                corrispondenteUO.Url.Add(new Corrispondente.UrlInfo() { Url = url });
                            result = corrispondenteUO;
                            break;
                        case "F":   // RF
                            //result = GetRF(corrRow);
                            DocsPaVO.utente.Corrispondente corrispondenteRF = new RaggruppamentoFunzionale();

                            corrispondenteRF.tipoCorrispondente = "F";
                            corrispondenteRF.systemId = corrRow["SYSTEM_ID"].ToString();
                            corrispondenteRF.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                            corrispondenteRF.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                            corrispondenteRF.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                            if (corrRow["ID_REGISTRO"] != null)
                            {
                                corrispondenteRF.idRegistro = corrRow["ID_REGISTRO"].ToString();
                            }
                            corrispondenteRF.email = corrRow["VAR_EMAIL"].ToString();
                            corrispondenteRF.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                            DocsPaVO.utente.ServerPosta sp_E_RF = new DocsPaVO.utente.ServerPosta();
                            sp_E_RF.serverSMTP = corrRow["VAR_SMTP"].ToString();
                            sp_E_RF.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                            corrispondenteRF.serverPosta = sp_E_RF;
                            corrispondenteRF.idAmministrazione = corrRow["ID_AMM"].ToString();
                            corrispondenteRF.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                            corrispondenteRF.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                            //18 nov 2005 - BUG ANAS 1501: corrispondenteUO.tipoIE="I";
                            corrispondenteRF.tipoIE = corrRow["CHA_TIPO_IE"].ToString();
                            //corrispondenteUO.tipoIE = "E";

                            // Indica se il corrispondente è proveniente da rubrica comune
                            corrispondenteRF.inRubricaComune = (corrRow["CHA_TIPO_CORR"] != DBNull.Value && corrRow["CHA_TIPO_CORR"].ToString() == "C");

                            //popolo il canale preferenziale per il corrisp Esterno
                            //DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                            //corrispondenteRF.canalePref = amm.GetDatiCanPref(corrispondenteRF);


                            //DATA FINE PER CORRISPONDENTI ESTERNI
                            if (corrRow.Table.Columns.Contains("ID_OLD") && corrRow["ID_OLD"] != DBNull.Value)
                                corrispondenteRF.idOld = corrRow["ID_OLD"].ToString();

                            corrispondenteRF.dta_fine = corrRow["DTA_FINE"].ToString();

                            // Impostazione dell'url per l'interoperabilità semplificata
                            corrispondenteRF.Url = new List<Corrispondente.UrlInfo>();
                            String url_E_RF = corrRow["InteropUrl"].ToString();
                            if (!String.IsNullOrEmpty(url_E_RF))
                                corrispondenteRF.Url.Add(new Corrispondente.UrlInfo() { Url = url_E_RF });
                            result = corrispondenteRF;

                            break;
                        case "R": // Ruolo
                            DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                            corrispondenteRuolo.systemId = corrRow["SYSTEM_ID"].ToString();
                            //corrispondenteRuolo.descrizione=corrRow["VAR_DESC_RUOLO"].ToString();
                            corrispondenteRuolo.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                            corrispondenteRuolo.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                            corrispondenteRuolo.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                            corrispondenteRuolo.idAmministrazione = corrRow["ID_AMM"].ToString();
                            //corrispondenteRuolo.livello=corrRow["RUOLO_LIVELLO"].ToString(); 
                            corrispondenteRuolo.livello = corrRow["NUM_LIVELLO"].ToString();
                            corrispondenteRuolo.idGruppo = corrRow["ID_GRUPPO"].ToString();
                            corrispondenteRuolo.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                            corrispondenteRuolo.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                            sp = new DocsPaVO.utente.ServerPosta();
                            sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                            sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                            corrispondenteRuolo.serverPosta = sp;

                            //18 nov 2005 - BUG ANAS 1501: corrispondenteRuolo.tipoIE="I";
                            corrispondenteRuolo.tipoIE = "E";

                            //popolo il canale preferenziale per il corrisp Esterno
                            //corrispondenteRuolo.canalePref = amm.GetDatiCanPref(corrispondenteRuolo);

                            //DATA FINE PER CORRISPONDENTI ESTERNI
                            corrispondenteRuolo.dta_fine = corrRow["DTA_FINE"].ToString();

                            if (corrRow["ID_REGISTRO"] != null)
                            {
                                corrispondenteRuolo.idRegistro = corrRow["ID_REGISTRO"].ToString();
                            }
                            corrispondenteRuolo.email = corrRow["VAR_EMAIL"].ToString();
                            corrispondenteRuolo.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                            //qui si ritrova la parentela (con filtro o no)
                            if (objQueryCorrispondente.isUODefined())
                            {
                                if ((objQueryCorrispondente.descrizioneUO != null && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])))
                                {
                                    DocsPaVO.utente.UnitaOrganizzativa uon = new DocsPaVO.utente.UnitaOrganizzativa();
                                    uon.systemId = corrRow["ID_PARENT"].ToString();
                                    corrispondenteRuolo.uo = uon;
                                    result = (DocsPaVO.utente.Corrispondente)corrispondenteRuolo;
                                    result.tipoCorrispondente = "R";
                                }
                            }
                            else
                            {
                                DocsPaVO.utente.UnitaOrganizzativa uon = new DocsPaVO.utente.UnitaOrganizzativa();
                                uon.systemId = corrRow["ID_PARENT"].ToString();
                                corrispondenteRuolo.uo = uon;
                                result = (DocsPaVO.utente.Corrispondente)corrispondenteRuolo;
                                result.tipoCorrispondente = "R";
                            }

                            break;
                        case "P": // Persona
                            DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();
                            corrispondenteUtente.systemId = corrRow["SYSTEM_ID"].ToString();
                            corrispondenteUtente.idPeople = corrRow["ID_PEOPLE"].ToString();
                            corrispondenteUtente.descrizione = corrRow["VAR_COGNOME"].ToString() + " " + corrRow["VAR_NOME"].ToString();
                            corrispondenteUtente.cognome = corrRow["VAR_COGNOME"].ToString();
                            corrispondenteUtente.nome = corrRow["VAR_NOME"].ToString();
                            corrispondenteUtente.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                            corrispondenteUtente.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                            corrispondenteUtente.idAmministrazione = corrRow["ID_AMM"].ToString();
                            corrispondenteUtente.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                            corrispondenteUtente.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();

                            if (corrRow["ID_REGISTRO"] != null)
                            {
                                corrispondenteUtente.idRegistro = corrRow["ID_REGISTRO"].ToString();
                            }

                            corrispondenteUtente.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                            sp = new DocsPaVO.utente.ServerPosta();
                            sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                            sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                            corrispondenteUtente.serverPosta = sp;
                            corrispondenteUtente.email = corrRow["VAR_EMAIL"].ToString();

                            //18 nov 2005 - BUG ANAS 1501: corrispondenteUtente.tipoIE="I";
                            corrispondenteUtente.tipoIE = "E";

                            //popolo il canale preferenziale per il corrisp Esterno
                            //corrispondenteUtente.canalePref = amm.GetDatiCanPref(corrispondenteUtente);

                            //DATA FINE PER CORRISPONDENTI ESTERNI
                            corrispondenteUtente.dta_fine = corrRow["DTA_FINE"].ToString();

                            result = (DocsPaVO.utente.Corrispondente)corrispondenteUtente;
                            result.tipoCorrispondente = "P";
                            break;
                    }
                }
                else if (!string.IsNullOrEmpty(corrRow["CHA_TIPO_IE"].ToString()) && corrRow["CHA_TIPO_IE"].ToString().Equals("I"))
                {
                    switch (corrRow["CHA_TIPO_URP"].ToString())
                    {
                        case "U": // UO
                            DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                            corrispondenteUO.systemId = corrRow["SYSTEM_ID"].ToString();
                            corrispondenteUO.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                            corrispondenteUO.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                            corrispondenteUO.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                            if (corrRow["ID_REGISTRO"] != null)
                            {
                                corrispondenteUO.idRegistro = corrRow["ID_REGISTRO"].ToString();
                            }
                            corrispondenteUO.email = corrRow["VAR_EMAIL"].ToString();
                            corrispondenteUO.interoperante = FromCharToBool(corrRow["CHA_PA"].ToString());
                            corrispondenteUO.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                            corrispondenteUO.livello = corrRow["NUM_LIVELLO"].ToString();
                            sp = new DocsPaVO.utente.ServerPosta();
                            sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                            sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                            corrispondenteUO.serverPosta = sp;
                            corrispondenteUO.idAmministrazione = corrRow["ID_AMM"].ToString();
                            corrispondenteUO.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                            corrispondenteUO.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                            corrispondenteUO.codiceIstat = corrRow["VAR_CODICE_ISTAT"].ToString();
                            corrispondenteUO.tipoIE = "I";
                            if (corrRow["DTA_FINE"] != null)
                                corrispondenteUO.dta_fine = corrRow["DTA_FINE"].ToString();//Fede 24 nov 05
                            //qui si ritrova la parentela
                            if (!corrRow["ID_PARENT"].ToString().Equals("0"))
                            {
                                DocsPaVO.utente.UnitaOrganizzativa uon = new DocsPaVO.utente.UnitaOrganizzativa();
                                uon.systemId = corrRow["ID_PARENT"].ToString();
                                corrispondenteUO.parent = uon;//=GetParents(corrRow["ID_PARENT"].ToString(),dataSet.Tables["UO"]);
                            }
                            result = (DocsPaVO.utente.Corrispondente)corrispondenteUO;
                            result.tipoCorrispondente = "U";

                            if (corrRow.Table.Columns.Contains("CHA_DISABLED_TRASM") && corrRow["CHA_DISABLED_TRASM"] != null && corrRow["CHA_DISABLED_TRASM"].ToString() == "1")
                            {
                                corrispondenteUO.disabledTrasm = true;
                            }

                            break;
                        case "R": // Ruolo
                            DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                            corrispondenteRuolo.systemId = corrRow["SYSTEM_ID"].ToString();
                            corrispondenteRuolo.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                            corrispondenteRuolo.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                            corrispondenteRuolo.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                            corrispondenteRuolo.idAmministrazione = corrRow["ID_AMM"].ToString();
                            corrispondenteRuolo.idGruppo = corrRow["ID_GRUPPO"].ToString();
                            corrispondenteRuolo.livello = (new Amministrazione()).GetDatiTipoRuolo(corrRow["ID_TIPO_RUOLO"].ToString()).livello;
                            corrispondenteRuolo.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                            corrispondenteRuolo.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                            sp = new DocsPaVO.utente.ServerPosta();
                            sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                            sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                            corrispondenteRuolo.serverPosta = sp;
                            corrispondenteRuolo.tipoIE = "I";
                            if (corrRow["DTA_FINE"] != null)
                                corrispondenteRuolo.dta_fine = corrRow["DTA_FINE"].ToString();//Fede 24 nov 05
                            if (corrRow["ID_REGISTRO"] != null)
                            {
                                corrispondenteRuolo.idRegistro = corrRow["ID_REGISTRO"].ToString();
                            }
                            corrispondenteRuolo.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                            //qui si ritrova la parentela (con filtro o no)
                            if (objQueryCorrispondente.isUODefined())
                            {
                                if ((objQueryCorrispondente.descrizioneUO != null && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])))
                                {
                                    corrispondenteRuolo.uo = get_uo_by_system_id(corrRow["ID_AMM"].ToString(), corrRow["ID_UO"].ToString());
                                    result = (DocsPaVO.utente.Corrispondente)corrispondenteRuolo;
                                    result.tipoCorrispondente = "R";
                                }
                            }
                            else
                            {
                                corrispondenteRuolo.uo = get_uo_by_system_id(corrRow["ID_AMM"].ToString(), corrRow["ID_UO"].ToString());
                                result = (DocsPaVO.utente.Corrispondente)corrispondenteRuolo;
                                result.tipoCorrispondente = "R";
                            }

                            if (corrRow.Table.Columns.Contains("CHA_DISABLED_TRASM") && corrRow["CHA_DISABLED_TRASM"] != null && corrRow["CHA_DISABLED_TRASM"].ToString() == "1")
                            {
                                corrispondenteRuolo.disabledTrasm = true;
                            }

                            break;
                        case "P": // Persona
                            DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();
                            corrispondenteUtente.systemId = corrRow["SYSTEM_ID"].ToString();
                            corrispondenteUtente.idPeople = corrRow["ID_PEOPLE"].ToString();
                            corrispondenteUtente.descrizione = corrRow["VAR_COGNOME"].ToString() + " " + corrRow["VAR_NOME"].ToString();
                            corrispondenteUtente.nome = corrRow["VAR_NOME"].ToString();
                            corrispondenteUtente.cognome = corrRow["VAR_COGNOME"].ToString();
                            corrispondenteUtente.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                            corrispondenteUtente.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                            corrispondenteUtente.idAmministrazione = corrRow["ID_AMM"].ToString();
                            corrispondenteUtente.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                            corrispondenteUtente.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                            if (corrRow["DTA_FINE"] != null)
                            {
                                corrispondenteUtente.dta_fine = corrRow["DTA_FINE"].ToString();//Fede 24 nov 05
                            }
                            if (corrRow["ID_REGISTRO"] != null)
                            {
                                corrispondenteUtente.idRegistro = corrRow["ID_REGISTRO"].ToString();
                            }

                            corrispondenteUtente.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                            sp = new DocsPaVO.utente.ServerPosta();
                            sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                            sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                            corrispondenteUtente.serverPosta = sp;
                            corrispondenteUtente.email = corrRow["VAR_EMAIL"].ToString();
                            //DocsPaVO.utente.Utente ut = null;
                            //07/03/2017: ottimizzazione, estratte le informazioni sotto commentate nella query superiore
                            //ut = GetUtente(corrRow["ID_PEOPLE"].ToString());
                            //if (ut != null)
                            //{
                            //    corrispondenteUtente.notifica = ut.notifica;
                            //    corrispondenteUtente.notificaConAllegato = ut.notificaConAllegato;
                            //}
                            corrispondenteUtente.notifica = corrRow["CHA_NOTIFICA"].ToString();
                            corrispondenteUtente.notificaConAllegato = FromCharToBool(corrRow["CHA_NOTIFICA_CON_ALLEGATO"].ToString());
                            corrispondenteUtente.tipoIE = "I";
                            result = (DocsPaVO.utente.Corrispondente)corrispondenteUtente;
                            result.tipoCorrispondente = "P";

                            if (corrRow.Table.Columns.Contains("CHA_DISABLED_TRASM") && corrRow["CHA_DISABLED_TRASM"] != null && corrRow["CHA_DISABLED_TRASM"].ToString() == "1")
                            {
                                corrispondenteUtente.disabledTrasm = true;
                            }
                            break;

                    }
                }


            }
            catch (Exception e)
            {
                logger.Debug("ERROR : ListaCorrispondentiInt", e);

                //database.closeConnection();
                //this.CloseConnection();
                //throw e;
                //				listaCorr = null;
                result = null;
            }

            //			logger.Debug("END : DocsPaDB > Query_DocsPAWS > Utenti > ListaCorrispondentiInt");
            //			return listaCorr;
            return result;
        }

        public void RicercaUOParent(DataSet dataSet, DataRow corrRow)
        {
            string sql = "";
            DocsPaUtils.Query q;
            if (corrRow["CHA_TIPO_URP"].ToString().Equals("U"))
            {
                //commandString2="SELECT SYSTEM_ID, ID_REGISTRO, VAR_EMAIL, CHA_PA, VAR_DESC_CORR, NUM_LIVELLO, CHA_DETTAGLI, VAR_CODICE, VAR_COD_RUBRICA, VAR_CODICE_AMM, VAR_CODICE_ISTAT, VAR_CODICE_AOO, VAR_SMTP, NUM_PORTA_SMTP, ID_PARENT FROM DPA_CORR_GLOBALI A WHERE CHA_TIPO_URP='U' AND CHA_TIPO_IE='E' AND NUM_LIVELLO <="+corrRow["NUM_LIVELLO"].ToString();
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                //FEDERICA -AGGIUNTO ID AMM
                q.setParam("param1", "A.SYSTEM_ID, A.ID_REGISTRO, A.ID_AMM,  A.VAR_EMAIL, A.CHA_PA, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT");
                q.setParam("param2", "A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='E' AND A.NUM_LIVELLO <=" + corrRow["NUM_LIVELLO"].ToString());
                sql = q.getSQL();
            }
            if (corrRow["CHA_TIPO_URP"].ToString().Equals("R"))
            {
                //commandString2="SELECT A.SYSTEM_ID, A.ID_REGISTRO, A.VAR_EMAIL, A.CHA_PA, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT FROM DPA_CORR_GLOBALI A, DPA_CORR_GLOBALI B WHERE A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='E' AND B.SYSTEM_ID='"+corrRow["ID_UO"].ToString()+"' AND A.NUM_LIVELLO <= B.NUM_LIVELLO";
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__CORR_GLOBALI");
                //FEDERICA -AGGIUNTO ID AMM
                q.setParam("param1", "A.SYSTEM_ID, A.ID_REGISTRO, A.ID_AMM,  A.VAR_EMAIL, A.CHA_PA, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT");
                q.setParam("param2", "A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='E' AND B.SYSTEM_ID='" + corrRow["ID_UO"].ToString() + "' AND A.NUM_LIVELLO <= B.NUM_LIVELLO");
                sql = q.getSQL();
            }
            if (corrRow["CHA_TIPO_URP"].ToString().Equals("P"))
            {
                //commandString2="SELECT A.SYSTEM_ID, A.ID_REGISTRO, A.VAR_EMAIL, A.CHA_PA, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT FROM DPA_CORR_GLOBALI A, DPA_CORR_GLOBALI B WHERE A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='E' AND B.SYSTEM_ID='"+corrRow["RUOLO_ID_UO"].ToString()+"' AND A.NUM_LIVELLO <= B.NUM_LIVELLO";
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__CORR_GLOBALI");
                //FEDERICA - AGGIUNTO ID_AMM
                q.setParam("param1", "A.SYSTEM_ID, A.ID_REGISTRO, A.ID_AMM, A.VAR_EMAIL, A.CHA_PA, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT");
                q.setParam("param2", "A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='E' AND B.SYSTEM_ID='" + corrRow["RUOLO_ID_UO"].ToString() + "' AND A.NUM_LIVELLO <= B.NUM_LIVELLO");
                sql = q.getSQL();
            }
            logger.Debug(sql);

            //database.fillTable(commandString2,dataSet,"UO");
            this.ExecuteQuery(dataSet, "UO", sql);
        }


        public ArrayList ListaCorrispondentiOcc(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            logger.Debug("listaCorrispondentiEstMethod");
            //DocsPaWS.Utils.Database database = DocsPaWS.Utils.dbControl.getDatabase();
            //database.openConnection();
            DataSet dataSet = new DataSet();
            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);
            ArrayList listaCorr = new ArrayList();
            try
            {
                //costruzione della query in base alla richiesta ricevuta
                DocsPaUtils.Query q;

                string generalCondition = "((A.ID_AMM IS NULL) OR (A.ID_REGISTRO IS NULL AND A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')";
                generalCondition = generalCondition + RegistriCondition(objQueryCorrispondente, null) + ") ";

                if (objQueryCorrispondente.codiceRubrica != null)
                {
                    string generalConditionCR = " ((A.ID_AMM IS NULL) OR (A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "'))";
                    //string parentString="SELECT * FROM DPA_CORR_GLOBALI A WHERE VAR_COD_RUBRICA='"+objQueryCorrispondente.codiceRubrica+"' and cha_tipo_ie='E' AND CHA_TIPO_CORR='S' AND "+generalConditionCR;
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                    q.setParam("param1", "*");


                    string whereCond = "UPPER(A.VAR_COD_RUBRICA)='" + objQueryCorrispondente.codiceRubrica.ToUpper() + "'";
                    string parentString = q.getSQL() + whereCond;
                    logger.Debug("lista corrispongenti occ: " + parentString);
                    this.ExecuteQuery(dataSet, "PARENT", parentString);

                    //nel caso non esiste oggetto parent con tale codice rubrica
                    if (dataSet.Tables["PARENT"].Rows.Count == 0)
                        return listaCorr;
                    else
                    {
                        DataRow parentRow = dataSet.Tables["PARENT"].Rows[0];
                        listaCorr.Add(parentRow["VAR_EMAIL"].ToString());
                        return listaCorr;
                    }

                }
                return listaCorr;
            }
            catch (Exception e)
            {
                logger.Debug("errore nel reperimento di un mittente occasionale: " + e.Message);
                return listaCorr;
            }
        }
        public ArrayList ListaCorrispondentiEst(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            logger.Debug("listaCorrispondentiEstMethod");
            //DocsPaWS.Utils.Database database = DocsPaWS.Utils.dbControl.getDatabase();
            //database.openConnection();
            DataSet dataSet = new DataSet();
            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);
            ArrayList listaCorr = new ArrayList();
            try
            {
                //costruzione della query in base alla richiesta ricevuta
                DocsPaUtils.Query q;
                string commandString1 = "";
                string commandString_begin = "";
                string commandString_UO = "";
                string commandString_ruolo = "";
                string commandString_utente = "";

                string generalCondition = "((A.ID_AMM IS NULL) OR (A.ID_REGISTRO IS NULL AND A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')";
                generalCondition = generalCondition + RegistriCondition(objQueryCorrispondente, null) + ") ";

                if (objQueryCorrispondente.codiceRubrica != null)
                {
                    string generalConditionCR = " ((A.ID_AMM IS NULL) OR (A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "'))";
                    //string parentString="SELECT * FROM DPA_CORR_GLOBALI A WHERE VAR_COD_RUBRICA='"+objQueryCorrispondente.codiceRubrica+"' and cha_tipo_ie='E' AND CHA_TIPO_CORR='S' AND "+generalConditionCR;
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                    q.setParam("param1", "*");


                    string whereCond = "UPPER(A.VAR_COD_RUBRICA)='" + objQueryCorrispondente.codiceRubrica.ToUpper() + "' and A.cha_tipo_ie='E' AND A.CHA_TIPO_CORR in ('S','C') AND " + generalConditionCR;
                    //if (RegistriCondition(objQueryCorrispondente, "A") != "")
                    //{
                    //    whereCond = whereCond + "AND (A.ID_REGISTRO IS NULL " + RegistriCondition(objQueryCorrispondente, "A") + ") ";
                    //}
                    if (objQueryCorrispondente.fineValidita)
                        whereCond += " AND A.DTA_FINE IS NULL";
                    q.setParam("param2", whereCond);
                    string parentString = q.getSQL();

                    //db.fillTable(parentString,dataSet,"PARENT");
                    this.ExecuteQuery(dataSet, "PARENT", parentString);

                    //nel caso non esiste oggetto parent con tale codice rubrica
                    if (dataSet.Tables["PARENT"].Rows.Count == 0)
                        return listaCorr;

                    if (objQueryCorrispondente.getChildren == false)
                    {
                        if (dataSet.Tables["PARENT"].Rows[0]["CHA_TIPO_URP"].Equals("P"))
                        {
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__RUOE_UTENTE__CORR_GLOBALI");
                            string param = "UPPER(A.VAR_COD_RUBRICA)='" + objQueryCorrispondente.codiceRubrica.ToUpper() + "'";
                            param = param + " AND ((A.ID_AMM IS NULL) OR (A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')) and A.CHA_TIPO_CORR in ('S','C') ";
                            //if (RegistriCondition(objQueryCorrispondente, "A") != "")
                            //{
                            //    param = param + "AND (A.ID_REGISTRO IS NULL " + RegistriCondition(objQueryCorrispondente, "A") + ") ";
                            //}
                            q.setParam("param1", param);
                            commandString1 = q.getSQL();

                        }
                        else
                        {
                            commandString1 = parentString;
                        }
                    }
                    else
                    {
                        DataRow parentRow = dataSet.Tables["PARENT"].Rows[0];

                        //se il parent è un utente si restituisce la lista vuota
                        if (parentRow["CHA_TIPO_URP"].ToString().Equals("P"))
                            return listaCorr;

                        //se il parent è una UO
                        if (parentRow["CHA_TIPO_URP"].ToString().Equals("U"))
                        {
                            //commandString1="SELECT SYSTEM_ID, ID_REGISTRO, VAR_EMAIL, CHA_PA, VAR_DESC_CORR, VAR_CODICE, VAR_COD_RUBRICA, VAR_CODICE_AMM, VAR_CODICE_ISTAT, VAR_CODICE_AOO, CHA_TIPO_URP, ID_UO, ID_PARENT, NUM_LIVELLO, CHA_DETTAGLI, VAR_SMTP, NUM_PORTA_SMTP FROM DPA_CORR_GLOBALI A WHERE (ID_UO='"+parentRow["SYSTEM_ID"].ToString()+"' OR ID_PARENT='"+parentRow["SYSTEM_ID"].ToString()+"') and cha_tipo_ie='E'";
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                            q.setParam("param1", "SYSTEM_ID, ID_REGISTRO, ID_AMM, VAR_EMAIL, CHA_PA, VAR_DESC_CORR, VAR_CODICE, VAR_COD_RUBRICA, VAR_CODICE_AMM, VAR_CODICE_ISTAT, VAR_CODICE_AOO, CHA_TIPO_URP, ID_UO, ID_PARENT, NUM_LIVELLO, CHA_DETTAGLI, VAR_SMTP, NUM_PORTA_SMTP");
                            q.setParam("param2", "(ID_UO='" + parentRow["SYSTEM_ID"].ToString() + "' OR ID_PARENT='" + parentRow["SYSTEM_ID"].ToString() + "') and cha_tipo_ie='E'");
                            commandString1 = q.getSQL();
                        }

                        //se il parent è un ruolo
                        if (parentRow["CHA_TIPO_URP"].ToString().Equals("R"))
                        {
                            //commandString1="SELECT A.SYSTEM_ID, A.ID_REGISTRO, A.VAR_COGNOME, A.VAR_NOME, A.VAR_DESC_CORR, A.VAR_EMAIL, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP, A.VAR_SMTP, A.NUM_PORTA_SMTP, C.SYSTEM_ID AS RUOLO_SYSTEM_ID, C.VAR_DESC_CORR AS RUOLO_DESC, C.VAR_CODICE AS RUOLO_CODICE, C.VAR_COD_RUBRICA AS RUOLO_COD_RUBRICA , C.CHA_DETTAGLI AS RUOLO_DETTAGLI, C.ID_UO AS RUOLO_ID_UO FROM  DPA_CORR_GLOBALI A,DPA_RUOE_UTENTE B, DPA_CORR_GLOBALI C WHERE A.SYSTEM_ID=B.ID_UTENTE_EST AND C.SYSTEM_ID=B.ID_RUOE AND B.ID_RUOE='"+parentRow["SYSTEM_ID"].ToString()+"' and a.cha_tipo_ie='E'";
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__RUOE_UTENTE__CORR_GLOBALI");
                            q.setParam("param1", "B.ID_RUOE='" + parentRow["SYSTEM_ID"].ToString() + "' and a.cha_tipo_ie='E'");
                            commandString1 = q.getSQL();
                        }
                    }

                }
                else
                {
                    //la query viene fatta in base all'UO, al ruolo e all'utente
                    //commandString_begin="SELECT * FROM DPA_CORR_GLOBALI A WHERE "+generalCondition;
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                    q.setParam("param1", "*");
                    q.setParam("param2", generalCondition);
                    commandString_begin = q.getSQL();

                    //se la UO non è nulla
                    if (objQueryCorrispondente.codiceUO != null)
                    {
                        commandString_UO = commandString_begin + " AND UPPER(VAR_CODICE)LIKE UPPER('%" + objQueryCorrispondente.codiceUO + "%') AND CHA_TIPO_URP='U' AND CHA_TIPO_IE='E' and A.CHA_TIPO_CORR in ('S','C') ";
                    }
                    if (objQueryCorrispondente.descrizioneUO != null)
                    {
                        commandString_UO = commandString_begin + " AND CHA_TIPO_URP='U' AND CHA_TIPO_IE='E' AND A.CHA_TIPO_CORR in ('S','C') AND (";
                        char[] separator = ConfigurationManager.AppSettings["separator"].ToCharArray();
                        string[] uo_list = objQueryCorrispondente.descrizioneUO.Split(separator);
                        for (int i = 0; i < uo_list.Length; i++)
                        {
                            commandString_UO = commandString_UO + "UPPER(VAR_DESC_CORR) LIKE UPPER('%" + uo_list[i].Replace(" ", "%") + "%')";
                            if (i < uo_list.Length - 1)
                            {
                                commandString_UO = commandString_UO + " OR ";
                            }
                        }
                        commandString_UO = commandString_UO + ")";
                    }
                    commandString1 = commandString_UO;

                    //se il ruolo non è nullo
                    if (objQueryCorrispondente.isRuoloDefined())
                    {
                        commandString_ruolo = commandString_begin + " AND UPPER(VAR_DESC_CORR) LIKE '%" + objQueryCorrispondente.descrizioneRuolo.ToUpper() + "%' AND CHA_TIPO_URP='R' AND CHA_TIPO_IE='E' ";
                        commandString1 = commandString_ruolo;
                    }

                    //se l'utente non è nullo
                    if (objQueryCorrispondente.isUtenteDefined())
                    {
                        string generalCondition_utente = "((A.ID_AMM IS NULL) OR (A.ID_REGISTRO IS NULL AND A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')" + RegistriCondition(objQueryCorrispondente, "A") + ")";
                        /*if(objQueryCorrispondente.fineValidita)
                        {
                            generalCondition_utente=generalCondition_utente+" AND A.DTA_FINE IS NULL";
                        };*/
                        //string commandString_utente_begin="SELECT A.SYSTEM_ID, A.VAR_NOME, A.VAR_COGNOME, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP, A.ID_REGISTRO, A.VAR_EMAIL, A.VAR_SMTP, A.NUM_PORTA_SMTP, C.SYSTEM_ID AS RUOLO_SYSTEM_ID, C.VAR_DESC_CORR AS RUOLO_DESC, C.VAR_CODICE AS RUOLO_CODICE, C.ID_UO AS RUOLO_ID_UO, C.VAR_COD_RUBRICA AS RUOLO_COD_RUBRICA, C.CHA_DETTAGLI AS RUOLO_DETTAGLI  FROM  DPA_CORR_GLOBALI A,DPA_RUOE_UTENTE B, DPA_CORR_GLOBALI C WHERE "+generalCondition_utente+" AND B.ID_UTENTE_EST=A.SYSTEM_ID AND C.SYSTEM_ID=ID_RUOE";
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__RUOE_UTENTE__CORR_GLOBALI");
                        q.setParam("param1", generalCondition_utente);
                        q.setParam("param2", "");
                        string commandString_utente_begin = q.getSQL();
                        if (objQueryCorrispondente.nomeUtente != null)
                        {
                            //Celeste
                            //commandString_utente=commandString_utente_begin+" AND UPPER(A.VAR_NOME) LIKE '%"+objQueryCorrispondente.nomeUtente.ToUpper()+"%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='E' AND A.CHA_TIPO_CORR='S'";
                            commandString_utente = commandString_utente_begin + " AND UPPER(A.VAR_NOME) LIKE '" + objQueryCorrispondente.nomeUtente.ToUpper() + "%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='E' AND A.CHA_TIPO_CORR in ('S','C')";
                        }
                        else
                        {
                            //Celeste
                            //commandString_utente=commandString_utente_begin+" AND UPPER(A.VAR_COGNOME) LIKE '%"+objQueryCorrispondente.cognomeUtente.ToUpper()+"%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='E' AND A.CHA_TIPO_CORR='S'";
                            commandString_utente = commandString_utente_begin + " AND UPPER(A.VAR_COGNOME) LIKE '" + objQueryCorrispondente.cognomeUtente.ToUpper() + "%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='E' and A.CHA_TIPO_CORR in ('S','C')";
                        }

                        //controllo se la query ha condizioni sui ruoli
                        if (objQueryCorrispondente.isRuoloDefined())
                        {
                            commandString_utente = commandString_utente + " AND UPPER(C.VAR_DESC_CORR) LIKE '%" + objQueryCorrispondente.descrizioneRuolo.ToUpper() + "%'";
                        }
                        commandString1 = commandString_utente;
                    }
                }

                //Fabio
                if (objQueryCorrispondente.fineValidita)
                    commandString1 += " AND A.DTA_FINE IS NULL";

                if (!string.IsNullOrEmpty(objQueryCorrispondente.email))
                    commandString1 += " AND upper(A.VAR_EMAIL)='" + objQueryCorrispondente.email.ToUpper() + "'";
                logger.Debug(commandString1);

                //database.fillTable(commandString1,dataSet,"CORRISPONDENTI");
                this.ExecuteQuery(dataSet, "CORRISPONDENTI", commandString1);
                ArrayList list = new ArrayList();

                //riempimento dell'oggetto finale
                foreach (DataRow corrRow in dataSet.Tables["CORRISPONDENTI"].Rows)
                {
                    //ricerca delle UO parent: viene riempita una tabella ottimizzata
                    /*string commandString2="";
                    if(corrRow["CHA_TIPO_URP"].ToString().Equals("U"))
                    {
                        commandString2="SELECT SYSTEM_ID, ID_REGISTRO, VAR_EMAIL, CHA_PA, VAR_DESC_CORR, NUM_LIVELLO, CHA_DETTAGLI, VAR_CODICE, VAR_COD_RUBRICA, VAR_CODICE_AMM, VAR_CODICE_ISTAT, VAR_CODICE_AOO, VAR_SMTP, NUM_PORTA_SMTP, ID_PARENT FROM DPA_CORR_GLOBALI A WHERE CHA_TIPO_URP='U' AND CHA_TIPO_IE='E' AND NUM_LIVELLO <="+corrRow["NUM_LIVELLO"].ToString();
                    }

                    if(corrRow["CHA_TIPO_URP"].ToString().Equals("R"))
                    {
                        commandString2="SELECT A.SYSTEM_ID, A.ID_REGISTRO, A.VAR_EMAIL, A.CHA_PA, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT FROM DPA_CORR_GLOBALI A, DPA_CORR_GLOBALI B WHERE A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='E' AND B.SYSTEM_ID='"+corrRow["ID_UO"].ToString()+"' AND A.NUM_LIVELLO <= B.NUM_LIVELLO";
                    }
                    if(corrRow["CHA_TIPO_URP"].ToString().Equals("P"))
                    {
                        commandString2="SELECT A.SYSTEM_ID, A.ID_REGISTRO, A.VAR_EMAIL, A.CHA_PA, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT FROM DPA_CORR_GLOBALI A, DPA_CORR_GLOBALI B WHERE A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='E' AND B.SYSTEM_ID='"+corrRow["RUOLO_ID_UO"].ToString()+"' AND A.NUM_LIVELLO <= B.NUM_LIVELLO";
                    }
                    logger.Debug(commandString2);
					
                    database.fillTable(commandString2,dataSet,"UO");*/

                    //RicercaUOParent(dataSet,corrRow);

                    //l'oggetto viene riempito in base alla sua tipologia
                    if (corrRow["CHA_TIPO_URP"].ToString().Equals("U"))
                    {
                        DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                        corrispondenteUO.systemId = corrRow["SYSTEM_ID"].ToString();
                        corrispondenteUO.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                        corrispondenteUO.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                        corrispondenteUO.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                        if (corrRow["ID_REGISTRO"] != null)
                        {
                            corrispondenteUO.idRegistro = corrRow["ID_REGISTRO"].ToString();
                        }
                        corrispondenteUO.email = corrRow["VAR_EMAIL"].ToString();
                        corrispondenteUO.interoperante = FromCharToBool(corrRow["CHA_PA"].ToString());
                        corrispondenteUO.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                        DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                        sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                        corrispondenteUO.serverPosta = sp;
                        corrispondenteUO.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                        corrispondenteUO.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                        corrispondenteUO.codiceIstat = corrRow["VAR_CODICE_ISTAT"].ToString();
                        corrispondenteUO.livello = corrRow["NUM_LIVELLO"].ToString();
                        corrispondenteUO.tipoIE = "E";
                        corrispondenteUO.tipoCorrispondente = corrRow["CHA_TIPO_URP"].ToString();
                        //qui si ritrova la parentela
                        if (!corrRow["ID_PARENT"].ToString().Equals("0")
                            && !string.IsNullOrEmpty(corrRow["ID_PARENT"].ToString()))
                        {
                            logger.Debug("IDPAR: " + corrRow["ID_PARENT"].ToString());
                            //RicercaUOParent(dataSet, corrRow);
                            //corrispondenteUO.parent = GetParents(corrRow["ID_PARENT"].ToString(), dataSet.Tables["UO"]);
                        }
                        list.Add(corrispondenteUO);
                    }
                    if (corrRow["CHA_TIPO_URP"].ToString().Equals("R"))
                    {
                        DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                        corrispondenteRuolo.systemId = corrRow["SYSTEM_ID"].ToString();
                        corrispondenteRuolo.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                        corrispondenteRuolo.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                        corrispondenteRuolo.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                        corrispondenteRuolo.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                        corrispondenteRuolo.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                        corrispondenteRuolo.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                        corrispondenteRuolo.idAmministrazione = corrRow["ID_AMM"].ToString();
                        corrispondenteRuolo.email = corrRow["var_email"].ToString();
                        DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                        sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                        corrispondenteRuolo.serverPosta = sp;
                        corrispondenteRuolo.tipoIE = "E";
                        corrispondenteRuolo.tipoCorrispondente = corrRow["CHA_TIPO_URP"].ToString();
                        if (corrRow["ID_REGISTRO"] != null)
                        {
                            corrispondenteRuolo.idRegistro = corrRow["ID_REGISTRO"].ToString();
                        }
                        //qui si ritrova la parentela (con filtro o no)
                        //RicercaUOParent(dataSet, corrRow);
                        if (objQueryCorrispondente.isUODefined())
                        {
                            if ((objQueryCorrispondente.descrizioneUO != null && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])))
                            {
                                corrispondenteRuolo.uo = GetParents(corrRow["ID_UO"].ToString(), dataSet.Tables["UO"]);
                                list.Add(corrispondenteRuolo);
                            }
                        }
                        else
                        {
                            if (dataSet.Tables["UO"] != null)
                                corrispondenteRuolo.uo = GetParents(corrRow["ID_UO"].ToString(), dataSet.Tables["UO"]);
                            list.Add(corrispondenteRuolo);
                        }
                    }
                    if (corrRow["CHA_TIPO_URP"].ToString().Equals("P"))
                    {
                        DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();
                        corrispondenteUtente.systemId = corrRow["SYSTEM_ID"].ToString();
                        corrispondenteUtente.descrizione = corrRow["VAR_COGNOME"].ToString() + " " + corrRow["VAR_NOME"].ToString();
                        corrispondenteUtente.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                        corrispondenteUtente.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                        corrispondenteUtente.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                        corrispondenteUtente.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                        corrispondenteUtente.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                        corrispondenteUtente.tipoCorrispondente = corrRow["CHA_TIPO_URP"].ToString();
                        DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                        sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                        corrispondenteUtente.serverPosta = sp;
                        corrispondenteUtente.tipoIE = "E";
                        if (corrRow["ID_REGISTRO"] != null)
                        {
                            corrispondenteUtente.idRegistro = corrRow["ID_REGISTRO"].ToString();
                        }
                        corrispondenteUtente.email = corrRow["VAR_EMAIL"].ToString();
                        //si trova il ruolo corrispondente
                        logger.Debug("riempimento ruolo");
                        DocsPaVO.utente.Ruolo ruoloUtente = new DocsPaVO.utente.Ruolo();
                        ruoloUtente.systemId = corrRow.Table.Columns.Contains("RUOLO_SYSTEM_ID") ? corrRow["RUOLO_SYSTEM_ID"].ToString() : string.Empty;
                        ruoloUtente.descrizione = corrRow.Table.Columns.Contains("RUOLO_DESC") ? corrRow["RUOLO_DESC"].ToString() : string.Empty;
                        ruoloUtente.codiceCorrispondente = corrRow.Table.Columns.Contains("RUOLO_CODICE") ? corrRow["RUOLO_CODICE"].ToString() : string.Empty;
                        ruoloUtente.codiceRubrica = corrRow.Table.Columns.Contains("RUOLO_COD_RUBRICA") ? corrRow["RUOLO_COD_RUBRICA"].ToString() : string.Empty;
                        ruoloUtente.dettagli = corrRow.Table.Columns.Contains("RUOLO_DETTAGLI") ? FromCharToBool(corrRow["RUOLO_DETTAGLI"].ToString()) : false;
                        //qui si ritrova la parentela (con filtro o no)
                        //RicercaUOParent(dataSet, corrRow);
                        logger.Debug("Riempimento UO");
                        if (objQueryCorrispondente.isUODefined())
                        {
                            if ((objQueryCorrispondente.descrizioneUO != null && corrRow.Table.Columns.Contains("RUOLO_ID_UO") && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, corrRow["RUOLO_ID_UO"].ToString(), dataSet.Tables["UO"])) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, corrRow["RUOLO_ID_UO"].ToString(), dataSet.Tables["UO"])))
                            {
                                ruoloUtente.uo = GetParents(corrRow["RUOLO_ID_UO"].ToString(), dataSet.Tables["UO"]);

                                ArrayList ruoli = new ArrayList();
                                ruoli.Add(ruoloUtente);
                                corrispondenteUtente.ruoli = ruoli;
                                list.Add(corrispondenteUtente);
                            }
                        }
                        else
                        {
                            if (corrRow.Table.Columns.Contains("RUOLO_ID_UO"))
                                ruoloUtente.uo = GetParents(corrRow["RUOLO_ID_UO"].ToString(), dataSet.Tables["UO"]);

                            ArrayList ruoli = new ArrayList();
                            ruoli.Add(ruoloUtente);
                            corrispondenteUtente.ruoli = ruoli;
                            list.Add(corrispondenteUtente);
                        }

                    }
                }
                listaCorr = list;

                //database.closeConnection();
                //this.CloseConnection();

            }
            catch (Exception e)
            {
                logger.Debug("Eccezione: " + e);
                //database.closeConnection();
                //this.CloseConnection();
                //throw e;
                listaCorr = null;
            }
            return listaCorr;
        }

        /// <summary>
        /// In questa seconda versione del metodo elimino la query sulla tabella DPA_RUOE_UTENTE, sempre vuota
        /// </summary>
        /// <param name="qco"></param>
        /// <returns></returns>
        public ArrayList ListaCorrispondentiEst2(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            logger.Debug("listaCorrispondentiEstMethod");
            //DocsPaWS.Utils.Database database = DocsPaWS.Utils.dbControl.getDatabase();
            //database.openConnection();
            DataSet dataSet = new DataSet();
            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);
            ArrayList listaCorr = new ArrayList();
            try
            {
                //costruzione della query in base alla richiesta ricevuta
                DocsPaUtils.Query q;
                string commandString1 = "";
                string commandString_begin = "";
                string commandString_UO = "";
                string commandString_ruolo = "";
                string commandString_utente = "";

                string generalCondition = "((A.ID_AMM IS NULL) OR (A.ID_REGISTRO IS NULL AND A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')";
                generalCondition = generalCondition + RegistriCondition(objQueryCorrispondente, null) + ") ";

                if (objQueryCorrispondente.codiceRubrica != null)
                {
                    string generalConditionCR = " ((A.ID_AMM IS NULL) OR (A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "'))";
                    //string parentString="SELECT * FROM DPA_CORR_GLOBALI A WHERE VAR_COD_RUBRICA='"+objQueryCorrispondente.codiceRubrica+"' and cha_tipo_ie='E' AND CHA_TIPO_CORR='S' AND "+generalConditionCR;
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                    q.setParam("param1", "*");


                    string whereCond = "UPPER(A.VAR_COD_RUBRICA)='" + objQueryCorrispondente.codiceRubrica.ToUpper() + "' and A.cha_tipo_ie='E' AND A.CHA_TIPO_CORR in ('S','C') AND " + generalConditionCR + " AND A.SYSTEM_ID = " + objQueryCorrispondente.systemId;
                    //if (RegistriCondition(objQueryCorrispondente, "A") != "")
                    //{
                    //    whereCond = whereCond + "AND (A.ID_REGISTRO IS NULL " + RegistriCondition(objQueryCorrispondente, "A") + ") ";
                    //}
                    if (objQueryCorrispondente.fineValidita)
                        whereCond += " AND A.DTA_FINE IS NULL";
                    q.setParam("param2", whereCond);
                    string parentString = q.getSQL();

                    //db.fillTable(parentString,dataSet,"PARENT");
                    this.ExecuteQuery(dataSet, "PARENT", parentString);

                    //nel caso non esiste oggetto parent con tale codice rubrica
                    if (dataSet.Tables["PARENT"].Rows.Count == 0)
                        return listaCorr;

                    if (objQueryCorrispondente.getChildren == false)
                    {
                        commandString1 = parentString;
                    }
                    else
                    {
                        DataRow parentRow = dataSet.Tables["PARENT"].Rows[0];

                        //se il parent è un utente si restituisce la lista vuota
                        if (parentRow["CHA_TIPO_URP"].ToString().Equals("P"))
                            return listaCorr;

                        //se il parent è una UO
                        if (parentRow["CHA_TIPO_URP"].ToString().Equals("U"))
                        {
                            //commandString1="SELECT SYSTEM_ID, ID_REGISTRO, VAR_EMAIL, CHA_PA, VAR_DESC_CORR, VAR_CODICE, VAR_COD_RUBRICA, VAR_CODICE_AMM, VAR_CODICE_ISTAT, VAR_CODICE_AOO, CHA_TIPO_URP, ID_UO, ID_PARENT, NUM_LIVELLO, CHA_DETTAGLI, VAR_SMTP, NUM_PORTA_SMTP FROM DPA_CORR_GLOBALI A WHERE (ID_UO='"+parentRow["SYSTEM_ID"].ToString()+"' OR ID_PARENT='"+parentRow["SYSTEM_ID"].ToString()+"') and cha_tipo_ie='E'";
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                            q.setParam("param1", "SYSTEM_ID, ID_REGISTRO, ID_AMM, VAR_EMAIL, CHA_PA, VAR_DESC_CORR, VAR_CODICE, VAR_COD_RUBRICA, VAR_CODICE_AMM, VAR_CODICE_ISTAT, VAR_CODICE_AOO, CHA_TIPO_URP, ID_UO, ID_PARENT, NUM_LIVELLO, CHA_DETTAGLI, VAR_SMTP, NUM_PORTA_SMTP");
                            q.setParam("param2", "(ID_UO='" + parentRow["SYSTEM_ID"].ToString() + "' OR ID_PARENT='" + parentRow["SYSTEM_ID"].ToString() + "') and cha_tipo_ie='E'");
                            commandString1 = q.getSQL();
                        }

                        //se il parent è un ruolo
                        if (parentRow["CHA_TIPO_URP"].ToString().Equals("R"))
                        {
                            //commandString1="SELECT A.SYSTEM_ID, A.ID_REGISTRO, A.VAR_COGNOME, A.VAR_NOME, A.VAR_DESC_CORR, A.VAR_EMAIL, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP, A.VAR_SMTP, A.NUM_PORTA_SMTP, C.SYSTEM_ID AS RUOLO_SYSTEM_ID, C.VAR_DESC_CORR AS RUOLO_DESC, C.VAR_CODICE AS RUOLO_CODICE, C.VAR_COD_RUBRICA AS RUOLO_COD_RUBRICA , C.CHA_DETTAGLI AS RUOLO_DETTAGLI, C.ID_UO AS RUOLO_ID_UO FROM  DPA_CORR_GLOBALI A,DPA_RUOE_UTENTE B, DPA_CORR_GLOBALI C WHERE A.SYSTEM_ID=B.ID_UTENTE_EST AND C.SYSTEM_ID=B.ID_RUOE AND B.ID_RUOE='"+parentRow["SYSTEM_ID"].ToString()+"' and a.cha_tipo_ie='E'";
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__RUOE_UTENTE__CORR_GLOBALI");
                            q.setParam("param1", "B.ID_RUOE='" + parentRow["SYSTEM_ID"].ToString() + "' and a.cha_tipo_ie='E'");
                            commandString1 = q.getSQL();
                        }
                    }

                }
                else
                {
                    //la query viene fatta in base all'UO, al ruolo e all'utente
                    //commandString_begin="SELECT * FROM DPA_CORR_GLOBALI A WHERE "+generalCondition;
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                    q.setParam("param1", "*");
                    q.setParam("param2", generalCondition);
                    commandString_begin = q.getSQL();

                    //se la UO non è nulla
                    if (objQueryCorrispondente.codiceUO != null)
                    {
                        commandString_UO = commandString_begin + " AND UPPER(VAR_CODICE)LIKE UPPER('%" + objQueryCorrispondente.codiceUO + "%') AND CHA_TIPO_URP='U' AND CHA_TIPO_IE='E' and A.CHA_TIPO_CORR in ('S','C') ";
                    }
                    if (objQueryCorrispondente.descrizioneUO != null)
                    {
                        commandString_UO = commandString_begin + " AND CHA_TIPO_URP='U' AND CHA_TIPO_IE='E' AND A.CHA_TIPO_CORR in ('S','C') AND (";
                        char[] separator = ConfigurationManager.AppSettings["separator"].ToCharArray();
                        string[] uo_list = objQueryCorrispondente.descrizioneUO.Split(separator);
                        for (int i = 0; i < uo_list.Length; i++)
                        {
                            commandString_UO = commandString_UO + "UPPER(VAR_DESC_CORR) LIKE UPPER('%" + uo_list[i].Replace(" ", "%") + "%')";
                            if (i < uo_list.Length - 1)
                            {
                                commandString_UO = commandString_UO + " OR ";
                            }
                        }
                        commandString_UO = commandString_UO + ")";
                    }
                    commandString1 = commandString_UO;

                    //se il ruolo non è nullo
                    if (objQueryCorrispondente.isRuoloDefined())
                    {
                        commandString_ruolo = commandString_begin + " AND UPPER(VAR_DESC_CORR) LIKE '%" + objQueryCorrispondente.descrizioneRuolo.ToUpper() + "%' AND CHA_TIPO_URP='R' AND CHA_TIPO_IE='E' ";
                        commandString1 = commandString_ruolo;
                    }

                    //se l'utente non è nullo
                    if (objQueryCorrispondente.isUtenteDefined())
                    {
                        string generalCondition_utente = "((A.ID_AMM IS NULL) OR (A.ID_REGISTRO IS NULL AND A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')" + RegistriCondition(objQueryCorrispondente, "A") + ")";
                        /*if(objQueryCorrispondente.fineValidita)
                        {
                            generalCondition_utente=generalCondition_utente+" AND A.DTA_FINE IS NULL";
                        };*/
                        //string commandString_utente_begin="SELECT A.SYSTEM_ID, A.VAR_NOME, A.VAR_COGNOME, A.VAR_DESC_CORR, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.CHA_DETTAGLI, A.CHA_TIPO_URP, A.ID_REGISTRO, A.VAR_EMAIL, A.VAR_SMTP, A.NUM_PORTA_SMTP, C.SYSTEM_ID AS RUOLO_SYSTEM_ID, C.VAR_DESC_CORR AS RUOLO_DESC, C.VAR_CODICE AS RUOLO_CODICE, C.ID_UO AS RUOLO_ID_UO, C.VAR_COD_RUBRICA AS RUOLO_COD_RUBRICA, C.CHA_DETTAGLI AS RUOLO_DETTAGLI  FROM  DPA_CORR_GLOBALI A,DPA_RUOE_UTENTE B, DPA_CORR_GLOBALI C WHERE "+generalCondition_utente+" AND B.ID_UTENTE_EST=A.SYSTEM_ID AND C.SYSTEM_ID=ID_RUOE";
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__RUOE_UTENTE__CORR_GLOBALI");
                        q.setParam("param1", generalCondition_utente);
                        q.setParam("param2", "");
                        string commandString_utente_begin = q.getSQL();
                        if (objQueryCorrispondente.nomeUtente != null)
                        {
                            //Celeste
                            //commandString_utente=commandString_utente_begin+" AND UPPER(A.VAR_NOME) LIKE '%"+objQueryCorrispondente.nomeUtente.ToUpper()+"%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='E' AND A.CHA_TIPO_CORR='S'";
                            commandString_utente = commandString_utente_begin + " AND UPPER(A.VAR_NOME) LIKE '" + objQueryCorrispondente.nomeUtente.ToUpper() + "%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='E' AND A.CHA_TIPO_CORR in ('S','C')";
                        }
                        else
                        {
                            //Celeste
                            //commandString_utente=commandString_utente_begin+" AND UPPER(A.VAR_COGNOME) LIKE '%"+objQueryCorrispondente.cognomeUtente.ToUpper()+"%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='E' AND A.CHA_TIPO_CORR='S'";
                            commandString_utente = commandString_utente_begin + " AND UPPER(A.VAR_COGNOME) LIKE '" + objQueryCorrispondente.cognomeUtente.ToUpper() + "%' AND A.CHA_TIPO_URP='P' AND A.CHA_TIPO_IE='E' and A.CHA_TIPO_CORR in ('S','C')";
                        }

                        //controllo se la query ha condizioni sui ruoli
                        if (objQueryCorrispondente.isRuoloDefined())
                        {
                            commandString_utente = commandString_utente + " AND UPPER(C.VAR_DESC_CORR) LIKE '%" + objQueryCorrispondente.descrizioneRuolo.ToUpper() + "%'";
                        }
                        commandString1 = commandString_utente;
                    }
                }

                //Fabio
                if (objQueryCorrispondente.fineValidita)
                    commandString1 += " AND A.DTA_FINE IS NULL";

                if (!string.IsNullOrEmpty(objQueryCorrispondente.email))
                    commandString1 += " AND upper(A.VAR_EMAIL)='" + objQueryCorrispondente.email.ToUpper() + "'";
                logger.Debug(commandString1);

                //database.fillTable(commandString1,dataSet,"CORRISPONDENTI");
                this.ExecuteQuery(dataSet, "CORRISPONDENTI", commandString1);
                ArrayList list = new ArrayList();

                //riempimento dell'oggetto finale
                foreach (DataRow corrRow in dataSet.Tables["CORRISPONDENTI"].Rows)
                {
                    //ricerca delle UO parent: viene riempita una tabella ottimizzata
                    /*string commandString2="";
                    if(corrRow["CHA_TIPO_URP"].ToString().Equals("U"))
                    {
                        commandString2="SELECT SYSTEM_ID, ID_REGISTRO, VAR_EMAIL, CHA_PA, VAR_DESC_CORR, NUM_LIVELLO, CHA_DETTAGLI, VAR_CODICE, VAR_COD_RUBRICA, VAR_CODICE_AMM, VAR_CODICE_ISTAT, VAR_CODICE_AOO, VAR_SMTP, NUM_PORTA_SMTP, ID_PARENT FROM DPA_CORR_GLOBALI A WHERE CHA_TIPO_URP='U' AND CHA_TIPO_IE='E' AND NUM_LIVELLO <="+corrRow["NUM_LIVELLO"].ToString();
                    }

                    if(corrRow["CHA_TIPO_URP"].ToString().Equals("R"))
                    {
                        commandString2="SELECT A.SYSTEM_ID, A.ID_REGISTRO, A.VAR_EMAIL, A.CHA_PA, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT FROM DPA_CORR_GLOBALI A, DPA_CORR_GLOBALI B WHERE A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='E' AND B.SYSTEM_ID='"+corrRow["ID_UO"].ToString()+"' AND A.NUM_LIVELLO <= B.NUM_LIVELLO";
                    }
                    if(corrRow["CHA_TIPO_URP"].ToString().Equals("P"))
                    {
                        commandString2="SELECT A.SYSTEM_ID, A.ID_REGISTRO, A.VAR_EMAIL, A.CHA_PA, A.VAR_DESC_CORR, A.NUM_LIVELLO, A.CHA_DETTAGLI, A.VAR_CODICE, A.VAR_COD_RUBRICA, A.VAR_CODICE_AMM, A.VAR_CODICE_ISTAT, A.VAR_CODICE_AOO, A.VAR_SMTP, A.NUM_PORTA_SMTP, A.ID_PARENT FROM DPA_CORR_GLOBALI A, DPA_CORR_GLOBALI B WHERE A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='E' AND B.SYSTEM_ID='"+corrRow["RUOLO_ID_UO"].ToString()+"' AND A.NUM_LIVELLO <= B.NUM_LIVELLO";
                    }
                    logger.Debug(commandString2);
					
                    database.fillTable(commandString2,dataSet,"UO");*/

                    //RicercaUOParent(dataSet,corrRow);

                    //l'oggetto viene riempito in base alla sua tipologia
                    if (corrRow["CHA_TIPO_URP"].ToString().Equals("U"))
                    {
                        DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                        corrispondenteUO.systemId = corrRow["SYSTEM_ID"].ToString();
                        corrispondenteUO.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                        corrispondenteUO.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                        corrispondenteUO.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                        if (corrRow["ID_REGISTRO"] != null)
                        {
                            corrispondenteUO.idRegistro = corrRow["ID_REGISTRO"].ToString();
                        }
                        corrispondenteUO.email = corrRow["VAR_EMAIL"].ToString();
                        corrispondenteUO.interoperante = FromCharToBool(corrRow["CHA_PA"].ToString());
                        corrispondenteUO.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                        DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                        sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                        corrispondenteUO.serverPosta = sp;
                        corrispondenteUO.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                        corrispondenteUO.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                        corrispondenteUO.codiceIstat = corrRow["VAR_CODICE_ISTAT"].ToString();
                        corrispondenteUO.livello = corrRow["NUM_LIVELLO"].ToString();
                        corrispondenteUO.tipoIE = "E";
                        corrispondenteUO.tipoCorrispondente = corrRow["CHA_TIPO_URP"].ToString();
                        //qui si ritrova la parentela
                        if (!corrRow["ID_PARENT"].ToString().Equals("0")
                            && !string.IsNullOrEmpty(corrRow["ID_PARENT"].ToString()))
                        {
                            logger.Debug("IDPAR: " + corrRow["ID_PARENT"].ToString());
                            //RicercaUOParent(dataSet, corrRow);
                            //corrispondenteUO.parent = GetParents(corrRow["ID_PARENT"].ToString(), dataSet.Tables["UO"]);
                        }
                        list.Add(corrispondenteUO);
                    }
                    if (corrRow["CHA_TIPO_URP"].ToString().Equals("R"))
                    {
                        DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                        corrispondenteRuolo.systemId = corrRow["SYSTEM_ID"].ToString();
                        corrispondenteRuolo.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                        corrispondenteRuolo.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                        corrispondenteRuolo.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                        corrispondenteRuolo.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                        corrispondenteRuolo.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                        corrispondenteRuolo.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                        corrispondenteRuolo.idAmministrazione = corrRow["ID_AMM"].ToString();
                        corrispondenteRuolo.email = corrRow["var_email"].ToString();
                        DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                        sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                        corrispondenteRuolo.serverPosta = sp;
                        corrispondenteRuolo.tipoIE = "E";
                        corrispondenteRuolo.tipoCorrispondente = corrRow["CHA_TIPO_URP"].ToString();
                        if (corrRow["ID_REGISTRO"] != null)
                        {
                            corrispondenteRuolo.idRegistro = corrRow["ID_REGISTRO"].ToString();
                        }
                        //qui si ritrova la parentela (con filtro o no)
                        //RicercaUOParent(dataSet, corrRow);
                        if (objQueryCorrispondente.isUODefined())
                        {
                            if ((objQueryCorrispondente.descrizioneUO != null && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, corrRow["ID_UO"].ToString(), dataSet.Tables["UO"])))
                            {
                                corrispondenteRuolo.uo = GetParents(corrRow["ID_UO"].ToString(), dataSet.Tables["UO"]);
                                list.Add(corrispondenteRuolo);
                            }
                        }
                        else
                        {
                            if (dataSet.Tables["UO"] != null)
                                corrispondenteRuolo.uo = GetParents(corrRow["ID_UO"].ToString(), dataSet.Tables["UO"]);
                            list.Add(corrispondenteRuolo);
                        }
                    }
                    if (corrRow["CHA_TIPO_URP"].ToString().Equals("P"))
                    {
                        DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();
                        corrispondenteUtente.systemId = corrRow["SYSTEM_ID"].ToString();
                        corrispondenteUtente.descrizione = corrRow["VAR_COGNOME"].ToString() + " " + corrRow["VAR_NOME"].ToString();
                        corrispondenteUtente.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                        corrispondenteUtente.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                        corrispondenteUtente.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                        corrispondenteUtente.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                        corrispondenteUtente.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                        corrispondenteUtente.tipoCorrispondente = corrRow["CHA_TIPO_URP"].ToString();
                        DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                        sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                        sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                        corrispondenteUtente.serverPosta = sp;
                        corrispondenteUtente.tipoIE = "E";
                        if (corrRow["ID_REGISTRO"] != null)
                        {
                            corrispondenteUtente.idRegistro = corrRow["ID_REGISTRO"].ToString();
                        }
                        corrispondenteUtente.email = corrRow["VAR_EMAIL"].ToString();
                        //si trova il ruolo corrispondente
                        logger.Debug("riempimento ruolo");
                        DocsPaVO.utente.Ruolo ruoloUtente = new DocsPaVO.utente.Ruolo();
                        ruoloUtente.systemId = corrRow.Table.Columns.Contains("RUOLO_SYSTEM_ID") ? corrRow["RUOLO_SYSTEM_ID"].ToString() : string.Empty;
                        ruoloUtente.descrizione = corrRow.Table.Columns.Contains("RUOLO_DESC") ? corrRow["RUOLO_DESC"].ToString() : string.Empty;
                        ruoloUtente.codiceCorrispondente = corrRow.Table.Columns.Contains("RUOLO_CODICE") ? corrRow["RUOLO_CODICE"].ToString() : string.Empty;
                        ruoloUtente.codiceRubrica = corrRow.Table.Columns.Contains("RUOLO_COD_RUBRICA") ? corrRow["RUOLO_COD_RUBRICA"].ToString() : string.Empty;
                        ruoloUtente.dettagli = corrRow.Table.Columns.Contains("RUOLO_DETTAGLI") ? FromCharToBool(corrRow["RUOLO_DETTAGLI"].ToString()) : false;
                        //qui si ritrova la parentela (con filtro o no)
                        //RicercaUOParent(dataSet, corrRow);
                        logger.Debug("Riempimento UO");
                        if (objQueryCorrispondente.isUODefined())
                        {
                            if ((objQueryCorrispondente.descrizioneUO != null && corrRow.Table.Columns.Contains("RUOLO_ID_UO") && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, corrRow["RUOLO_ID_UO"].ToString(), dataSet.Tables["UO"])) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, corrRow["RUOLO_ID_UO"].ToString(), dataSet.Tables["UO"])))
                            {
                                ruoloUtente.uo = GetParents(corrRow["RUOLO_ID_UO"].ToString(), dataSet.Tables["UO"]);

                                ArrayList ruoli = new ArrayList();
                                ruoli.Add(ruoloUtente);
                                corrispondenteUtente.ruoli = ruoli;
                                list.Add(corrispondenteUtente);
                            }
                        }
                        else
                        {
                            if (corrRow.Table.Columns.Contains("RUOLO_ID_UO"))
                                ruoloUtente.uo = GetParents(corrRow["RUOLO_ID_UO"].ToString(), dataSet.Tables["UO"]);

                            ArrayList ruoli = new ArrayList();
                            ruoli.Add(ruoloUtente);
                            corrispondenteUtente.ruoli = ruoli;
                            list.Add(corrispondenteUtente);
                        }

                    }
                }
                listaCorr = list;

                //database.closeConnection();
                //this.CloseConnection();

            }
            catch (Exception e)
            {
                logger.Debug("Eccezione: " + e);
                //database.closeConnection();
                //this.CloseConnection();
                //throw e;
                listaCorr = null;
            }
            return listaCorr;
        }


        private static DocsPaVO.addressbook.QueryCorrispondente CorrectApiciQuery(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            DocsPaVO.addressbook.QueryCorrispondente res = new DocsPaVO.addressbook.QueryCorrispondente();
            res.codiceGruppo = CorrectApici(qco.codiceGruppo);
            res.codiceRubrica = CorrectApici(qco.codiceRubrica);
            res.codiceRuolo = CorrectApici(qco.codiceRuolo);
            res.codiceUO = CorrectApici(qco.codiceUO);
            res.cognomeUtente = CorrectApici(qco.cognomeUtente);
            res.descrizioneGruppo = CorrectApici(qco.descrizioneGruppo);
            res.descrizioneRuolo = CorrectApici(qco.descrizioneRuolo);
            res.descrizioneUO = CorrectApici(qco.descrizioneUO);
            res.fineValidita = qco.fineValidita;
            res.getChildren = qco.getChildren;
            res.idAmministrazione = qco.idAmministrazione;
            res.idRegistri = qco.idRegistri;
            res.nomeUtente = CorrectApici(qco.nomeUtente);
            res.systemId = qco.systemId;
            res.tipoUtente = qco.tipoUtente;
            res.email = qco.email;
            return res;
        }

        private static DocsPaVO.utente.UnitaOrganizzativa GetParents(string idParent, DataTable dt)
        {
            logger.Debug("getParents");

            DataRow[] parentRowList = dt.Select("SYSTEM_ID='" + idParent + "'");//[0];
            DataRow parentRow = null;
            DocsPaVO.utente.UnitaOrganizzativa parent = new DocsPaVO.utente.UnitaOrganizzativa();
            if (parentRowList.Length > 0)
            {
                parentRow = parentRowList[0];

                parent.systemId = parentRow["SYSTEM_ID"].ToString();
                parent.descrizione = parentRow["VAR_DESC_CORR"].ToString();
                parent.codiceCorrispondente = parentRow["VAR_CODICE"].ToString();
                parent.codiceRubrica = parentRow["VAR_COD_RUBRICA"].ToString();
                parent.livello = parentRow["NUM_LIVELLO"].ToString();
                parent.codiceAOO = parentRow["VAR_CODICE_AOO"].ToString();
                parent.codiceAmm = parentRow["VAR_CODICE_AMM"].ToString();
                parent.codiceIstat = parentRow["VAR_CODICE_ISTAT"].ToString();
                parent.idAmministrazione = parentRow["ID_AMM"].ToString();
                parent.dettagli = FromCharToBool(parentRow["CHA_DETTAGLI"].ToString());
                DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                sp.serverSMTP = parentRow["VAR_SMTP"].ToString();
                sp.portaSMTP = parentRow["NUM_PORTA_SMTP"].ToString();
                parent.serverPosta = sp;
                if (parentRow["ID_REGISTRO"] != null)
                {
                    parent.idRegistro = parentRow["ID_REGISTRO"].ToString();
                }
                parent.email = parentRow["VAR_EMAIL"].ToString();
                parent.interoperante = FromCharToBool(parentRow["CHA_PA"].ToString());

                if (parentRow.Table.Columns.Contains("CLASSIFICA_UO"))
                {
                    parent.classificaUO = (parentRow["CLASSIFICA_UO"] == DBNull.Value ?
                                    null :
                                    parentRow["CLASSIFICA_UO"].ToString());
                }

                logger.Debug("Parent: " + parentRow["VAR_DESC_CORR"].ToString() + " " + parentRow["ID_PARENT"].ToString());
                if (!parentRow["ID_PARENT"].ToString().Equals("0") && !String.IsNullOrEmpty(parentRow["ID_PARENT"].ToString()))
                {
                    parent.parent = GetParents(parentRow["ID_PARENT"].ToString(), dt);
                }
            }
            else
            {
                parent = null;

            }

            return parent;
        }

        private static bool HasDefinedUo(string val, int type, string idParent, DataTable dt)
        {
            bool ret = false;
            DataRow parentRow = dt.Select("SYSTEM_ID='" + idParent + "'")[0];
            if ((type == 1 && StringContain(val, parentRow["VAR_DESC_CORR"].ToString())) || (type == 2 && parentRow["VAR_CODICE"].ToString().ToUpper().Equals(val.ToUpper())))
            {
                return true;
            }
            else
            {
                if (!parentRow["ID_PARENT"].ToString().Equals("0"))
                {
                    ret = HasDefinedUo(val, type, parentRow["ID_PARENT"].ToString(), dt);
                }
            }
            return ret;
        }

        private static string CorrectApici(string str)
        {
            if (str != null)
            {
                return str.Replace("'", "''");
            }
            else
            {
                return str;
            }
        }

        private static bool StringContain(string query, string obj)
        {
            char[] separator_or = ConfigurationManager.AppSettings["separator"].ToCharArray();
            char[] separator_and = { ' ' };
            string query_up = query.ToUpper();
            string obj_up = obj.ToUpper();
            string[] query_array_or = query_up.Split(separator_or);
            for (int i = 0; i < query_array_or.Length; i++)
            {
                string[] query_array_and = query_array_or[i].Split(separator_and);
                int ind_true = 0;
                for (int j = 0; j < query_array_and.Length; j++)
                {
                    if (obj_up.IndexOf(query_array_and[j]) > -1)
                    {
                        ind_true = ind_true + 1;
                    }

                }
                if (ind_true == query_array_and.Length) return true;
            }
            return false;
        }

        private static string RegistriCondition(DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente, string nameTable)
        {
            string condition = "";
            string prefix = "";
            if (nameTable != null)
            {
                prefix = nameTable + ".";
            }
            if (objQueryCorrispondente.idRegistri != null && objQueryCorrispondente.idRegistri.Count > 0)
            {
                condition = condition + " OR ((";
                for (int i = 0; i < objQueryCorrispondente.idRegistri.Count; i++)
                {
                    condition = condition + prefix + "ID_REGISTRO='" + objQueryCorrispondente.idRegistri[i] + "'";
                    if (i < objQueryCorrispondente.idRegistri.Count - 1) condition = condition + " OR ";
                }
                condition = condition + ") AND " + prefix + "ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')";
            }
            return condition;
        }
        //elisa -- condizione sui registri senza quella sull'amministrazione
        private static string RegistriCondition(DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente, string nameTable, string noAmm)
        {
            string condition = "";
            string prefix = "";
            if (nameTable != null)
            {
                prefix = nameTable + ".";
            }
            if (objQueryCorrispondente.idRegistri != null && objQueryCorrispondente.idRegistri.Count > 0)
            {
                if (noAmm == "true")
                {
                    if (objQueryCorrispondente.idRegistri.Count > 1)
                    {
                        condition = condition + " OR (";
                    }
                    for (int i = 0; i < objQueryCorrispondente.idRegistri.Count; i++)
                    {
                        condition = condition + prefix + "ID_REGISTRO='" + objQueryCorrispondente.idRegistri[i] + "'";
                        if (i < objQueryCorrispondente.idRegistri.Count - 1) condition = condition + " OR ";
                    }
                    if (objQueryCorrispondente.idRegistri.Count > 1)
                    {
                        condition = condition + ")";
                    }
                }
            }
            return condition;
        }

        public ArrayList ListaCorrEstSciolti(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            logger.Debug("listaCorrEstSciolti");
            ArrayList listaCorr = ListaCorrispondentiEst(qco);
            if (listaCorr == null)
            {
                return listaCorr;
            }
            ArrayList listaSciolti = ListaUtentiSciolti(qco);
            if (listaSciolti == null)
            {
                listaCorr = null;
                return listaCorr;
            }
            ArrayList temp = new ArrayList();
            for (int i = 0; i < listaSciolti.Count; i++)
            {
                DocsPaVO.utente.Utente ut = (DocsPaVO.utente.Utente)listaSciolti[i];
                logger.Debug("Utente sciolto: " + ut.systemId);
                bool isInEst = false;
                for (int k = 0; k < listaCorr.Count; k++)
                {
                    if (ut.systemId.Equals(((DocsPaVO.utente.Corrispondente)listaCorr[k]).systemId)) isInEst = true;
                }
                if (!isInEst) temp.Add(ut);
            }
            for (int j = 0; j < temp.Count; j++)
            {
                logger.Debug("Aggiunto utente sciolto");
                listaCorr.Add(temp[j]);
            }
            return listaCorr;
        }

        //METODO AGGIUNTO PER LE TRASMISSIONI AD UO
        public ArrayList ListaUOAut(DocsPaVO.addressbook.QueryCorrispondente qco, ArrayList listaUO)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Utenti > ListaUOAut");
            ArrayList listaCorr = new ArrayList();
            try
            {
                DataSet dataSet = new DataSet();
                DocsPaUtils.Query q;
                DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);
                string commandString1 = "";
                string commandString_UO = "";
                string generalCondition = "((A.ID_AMM IS NULL) OR (A.ID_REGISTRO IS NULL AND A.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "')" + RegistriCondition(objQueryCorrispondente, "A") + ")";
                //condizioni sul tipo di corrispondente:
                generalCondition += " AND CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND CHA_TIPO_CORR='S'";
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                q.setParam("param1", "*");
                q.setParam("param2", generalCondition);
                string commandString_begin = q.getSQL();
                //logger.Debug("commandString_begin=" + commandString_begin);

                //se la UO non è nulla
                if (objQueryCorrispondente.codiceUO != null)
                    commandString_UO = commandString_begin + " AND UPPER(A.VAR_CODICE) = UPPER('" + objQueryCorrispondente.codiceUO + "')";
                if (objQueryCorrispondente.descrizioneUO != null)
                {
                    commandString_UO = commandString_begin + " AND (";
                    char[] separator = ConfigurationManager.AppSettings["separator"].ToCharArray();
                    string[] uo_list = objQueryCorrispondente.descrizioneUO.Split(separator);
                    for (int i = 0; i < uo_list.Length; i++)
                    {
                        commandString_UO = commandString_UO + "UPPER(VAR_DESC_CORR) LIKE UPPER('%" + uo_list[i].Replace(" ", "%") + "%')";
                        if (i < uo_list.Length - 1) { commandString_UO = commandString_UO + " OR "; }
                    }
                    commandString_UO = commandString_UO + ")";
                }
                commandString1 = commandString_UO;
                if (objQueryCorrispondente.fineValidita)
                    commandString1 += " AND A.DTA_FINE IS NULL";
                if (listaUO != null && listaUO.Count > 0)
                {
                    string queryString = " AND A.SYSTEM_ID IN (";
                    for (int i = 0; i < listaUO.Count; i++)
                    {
                        queryString = queryString + listaUO[i].ToString();
                        if (i < listaUO.Count - 1)
                            queryString = queryString + ",";
                    }
                    queryString = queryString + ")";
                    commandString1 += queryString;
                }

                commandString1 = DocsPaDbManagement.Functions.Functions.SelectTop(commandString1);

                logger.Debug(commandString1);
                this.ExecuteQuery(out dataSet, "UO_AUT", commandString1);
                foreach (DataRow corrRow in dataSet.Tables["UO_AUT"].Rows)
                {
                    DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                    corrispondenteUO.systemId = corrRow["SYSTEM_ID"].ToString();
                    corrispondenteUO.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                    corrispondenteUO.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                    corrispondenteUO.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                    if (corrRow["ID_REGISTRO"] != null)
                    {
                        corrispondenteUO.idRegistro = corrRow["ID_REGISTRO"].ToString();
                    }
                    corrispondenteUO.email = corrRow["VAR_EMAIL"].ToString();
                    corrispondenteUO.interoperante = FromCharToBool(corrRow["CHA_PA"].ToString());
                    corrispondenteUO.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                    corrispondenteUO.livello = corrRow["NUM_LIVELLO"].ToString();
                    DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                    sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                    sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                    corrispondenteUO.serverPosta = sp;
                    corrispondenteUO.idAmministrazione = corrRow["ID_AMM"].ToString();
                    corrispondenteUO.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                    corrispondenteUO.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                    corrispondenteUO.codiceIstat = corrRow["VAR_CODICE_ISTAT"].ToString();
                    corrispondenteUO.tipoIE = "I";
                    //qui si ritrova la parentela
                    //					if(!corrRow["ID_PARENT"].ToString().Equals("0")) 
                    //					{
                    //						corrispondenteUO.parent=GetParents(corrRow["ID_PARENT"].ToString(),dataSet.Tables["UO_AUT"]);
                    //					}
                    listaCorr.Add(corrispondenteUO);
                }
            }
            catch (Exception e)
            {
                logger.Debug("ERROR : ListaUOAut", e);
                listaCorr = null;
            }

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Utenti > ListaCorrispondentiInt");
            return listaCorr;
        }

        //elisa 17/10/2005
        //ricerca UO autorizzate per protocollo interno
        public ArrayList ListaUOAutInterne(DocsPaVO.addressbook.QueryCorrispondente qco, ArrayList listaUO)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Utenti > ListaUOAutInterne");
            ArrayList listaCorr = new ArrayList();
            try
            {
                DataSet dataSet = new DataSet();
                DocsPaUtils.Query q;
                DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);
                string commandString1 = "";
                string commandString_UO = "";

                string generalCondition = " AND U.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "' AND R.ID_AMM='" + objQueryCorrispondente.idAmministrazione + "' AND R.CHA_RIFERIMENTO='1'";
                generalCondition = generalCondition + "  AND " + RegistriCondition(objQueryCorrispondente, "C", "true");
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali_ProtoInt_U");
                q.setParam("param1", "U.*,C.id_registro as IdRegSel");
                q.setParam("param2", generalCondition);
                string commandString_begin = q.getSQL();
                logger.Debug("commandString_begin=" + commandString_begin);

                if (objQueryCorrispondente.codiceUO != null)
                {
                    commandString_UO = commandString_begin + " AND UPPER(U.VAR_CODICE)='" + objQueryCorrispondente.codiceUO.ToUpper() + "'";

                    if (objQueryCorrispondente.fineValidita)
                        commandString_UO += " AND U.DTA_FINE IS NULL AND R.DTA_FINE IS NULL";
                }
                if (objQueryCorrispondente.descrizioneUO != null)
                {

                    char[] separator = ConfigurationManager.AppSettings["separator"].ToCharArray();
                    string[] uo_list = objQueryCorrispondente.descrizioneUO.Split(separator);
                    for (int i = 0; i < uo_list.Length; i++)
                    {
                        commandString_UO = commandString_begin + " AND (UPPER(U.VAR_DESC_CORR) LIKE UPPER('%" + uo_list[i].Replace(" ", "%") + "%')";
                        if (i < uo_list.Length - 1) { commandString_UO = commandString_UO + " OR "; }
                    }
                    commandString_UO = commandString_UO + ")";
                    if (objQueryCorrispondente.fineValidita)
                        commandString_UO += " AND U.DTA_FINE IS NULL AND R.DTA_FINE IS NULL";
                }
                commandString1 = commandString_UO;

                if (listaUO != null && listaUO.Count > 0)
                {
                    string queryString = " AND U.SYSTEM_ID IN (";
                    for (int i = 0; i < listaUO.Count; i++)
                    {
                        queryString = queryString + listaUO[i].ToString();
                        if (i < listaUO.Count - 1)
                            queryString = queryString + ",";
                    }
                    queryString = queryString + ")";
                    commandString1 += queryString;
                }

                commandString1 = DocsPaDbManagement.Functions.Functions.SelectTop(commandString1);
                commandString1 = commandString1.Insert(7, "distinct ");

                logger.Debug("3) " + commandString1);

                this.ExecuteQuery(out dataSet, "UO_AUT", commandString1);
                foreach (DataRow corrRow in dataSet.Tables["UO_AUT"].Rows)
                {
                    DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                    corrispondenteUO.systemId = corrRow["SYSTEM_ID"].ToString();
                    corrispondenteUO.descrizione = corrRow["VAR_DESC_CORR"].ToString();
                    corrispondenteUO.codiceCorrispondente = corrRow["VAR_CODICE"].ToString();
                    corrispondenteUO.codiceRubrica = corrRow["VAR_COD_RUBRICA"].ToString();
                    if (corrRow["ID_REGISTRO"] != null)
                    {
                        corrispondenteUO.idRegistro = corrRow["ID_REGISTRO"].ToString();
                    }
                    corrispondenteUO.email = corrRow["VAR_EMAIL"].ToString();
                    corrispondenteUO.interoperante = FromCharToBool(corrRow["CHA_PA"].ToString());
                    corrispondenteUO.dettagli = FromCharToBool(corrRow["CHA_DETTAGLI"].ToString());
                    corrispondenteUO.livello = corrRow["NUM_LIVELLO"].ToString();
                    DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                    sp.serverSMTP = corrRow["VAR_SMTP"].ToString();
                    sp.portaSMTP = corrRow["NUM_PORTA_SMTP"].ToString();
                    corrispondenteUO.serverPosta = sp;
                    corrispondenteUO.idAmministrazione = corrRow["ID_AMM"].ToString();
                    corrispondenteUO.codiceAOO = corrRow["VAR_CODICE_AOO"].ToString();
                    corrispondenteUO.codiceAmm = corrRow["VAR_CODICE_AMM"].ToString();
                    corrispondenteUO.codiceIstat = corrRow["VAR_CODICE_ISTAT"].ToString();
                    corrispondenteUO.tipoIE = "I";
                    //qui si ritrova la parentela
                    //					if(!corrRow["ID_PARENT"].ToString().Equals("0")) 
                    //					{
                    //						corrispondenteUO.parent=GetParents(corrRow["ID_PARENT"].ToString(),dataSet.Tables["UO_AUT"]);
                    //					}
                    listaCorr.Add(corrispondenteUO);
                }
            }
            catch (Exception e)
            {
                logger.Debug("ERROR : ListaUOAut", e);
                listaCorr = null;
            }

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Utenti > ListaCorrispondentiInt");
            return listaCorr;
        }
        //

        public bool isCorrispondenteValido(string idCorrispondente)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali_VALID");
            bool result = false;
            q.setParam("param1", idCorrispondente);
            string sql = q.getSQL();
            logger.Debug(sql);
            DataSet dataSet;
            this.ExecuteQuery(out dataSet, sql);
            if (dataSet.Tables[0].Rows.Count > 0) result = true;
            //foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            //{		   
            //	result = true;
            //}
            dataSet.Dispose();
            return result;
        }
        #endregion


        #region DirittiManager
        public void GetListaDiritti(out DataSet dataSet, string idObj)
        {
            //query distinte per i ruoli e per gli utenti
            //query per i ruoli
            /*string sql =
                "SELECT DISTINCT A.SYSTEM_ID, A.VAR_COD_RUBRICA, A.ID_REGISTRO, " +
                "A.ID_AMM, C.VAR_DESC_RUOLO, B.CHA_TIPO_DIRITTO " +
                "FROM DPA_CORR_GLOBALI A, SECURITY B, DPA_TIPO_RUOLO C " +
                "WHERE B.THING=" + idObj + " AND (B.ACCESSRIGHTS > 0 OR B.CHA_TIPO_DIRITTO='S')" +
                "AND A.ID_GRUPPO=B.PERSONORGROUP AND C.SYSTEM_ID=A.ID_TIPO_RUOLO";*/
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__SECURITY__TIPO_RUOLO2");
            q.setParam("param1", "A.SYSTEM_ID, A.VAR_COD_RUBRICA, A.ID_REGISTRO, " +
                "A.ID_AMM, C.VAR_DESC_RUOLO, B.CHA_TIPO_DIRITTO, B.PERSONORGROUP, B.HIDE_DOC_VERSIONS");
            q.setParam("param2", "B.THING=" + idObj + " AND (B.ACCESSRIGHTS >= 0 OR B.CHA_TIPO_DIRITTO='P') ");
            string sql = q.getSQL();
            logger.Debug(sql);
            //db.fillTable(queryStringRuoli,dataSet,"DIRITTI_RUOLI");
            this.ExecuteQuery(out dataSet, "DIRITTI_RUOLI", sql);
        }

        public void GetListaRuoli(out DataSet dataSet, string idObj, string IDAMM)
        {
            //query distinte per i ruoli e per gli utenti
            //query per i ruoli
            /*string sql =
                "SELECT DISTINCT A.SYSTEM_ID, C.ID_REGISTRO, C.ID_AMM, A.VAR_NOME, " +
                "A.VAR_COGNOME, C.VAR_COD_RUBRICA, B.CHA_TIPO_DIRITTO " +
                "FROM PEOPLE A, SECURITY B, DPA_CORR_GLOBALI C " +
                "WHERE B.THING=" + idObj + " AND (B.ACCESSRIGHTS > 0 OR B.CHA_TIPO_DIRITTO='S') " +
                "AND A.SYSTEM_ID=B.PERSONORGROUP AND C.ID_PEOPLE=A.SYSTEM_ID";*/
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLE__SECURITY");
            q.setParam("param1", "A.SYSTEM_ID, C.ID_REGISTRO, C.ID_AMM, A.VAR_NOME, " +
                "A.VAR_COGNOME, C.VAR_COD_RUBRICA, B.CHA_TIPO_DIRITTO, B.ACCESSRIGHTS, B.PERSONORGROUP, B.HIDE_DOC_VERSIONS ");
            q.setParam("param2", "B.THING=" + idObj + " AND (B.ACCESSRIGHTS > 0 OR B.CHA_TIPO_DIRITTO='P') AND C.ID_AMM =" + IDAMM);
            string sql = q.getSQL();
            logger.Debug(sql);
            //db.fillTable(queryStringUtenti,dataSet,"DIRITTI_UTENTI");
            this.ExecuteQuery(out dataSet, "DIRITTI_UTENTI", sql);
        }

        public void GetListaDiritti_Deleted(out DataSet dataSet, string idObj)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__DELETED_SECURITY__TIPO_RUOLO");
            q.setParam("param1", "A.SYSTEM_ID, A.VAR_COD_RUBRICA, A.ID_REGISTRO, " +
                "A.ID_AMM, C.VAR_DESC_RUOLO, B.CHA_TIPO_DIRITTO, B.NOTE, B.PERSONORGROUP, B.HIDE_DOC_VERSIONS");
            q.setParam("param2", "B.THING=" + idObj + " AND (B.ACCESSRIGHTS >= 0 OR B.CHA_TIPO_DIRITTO='P')");
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteQuery(out dataSet, "DIRITTI_RUOLI_DELETED", sql);
        }

        public void GetListaRuoli_Deleted(out DataSet dataSet, string idObj, string IDAMM)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLE__DELETED_SECURITY");
            q.setParam("param1", "A.SYSTEM_ID, C.ID_REGISTRO, C.ID_AMM, A.VAR_NOME, " +
                "A.VAR_COGNOME, C.VAR_COD_RUBRICA, B.CHA_TIPO_DIRITTO, B.ACCESSRIGHTS, B.NOTE, B.PERSONORGROUP, B.HIDE_DOC_VERSIONS ");
            q.setParam("param2", "B.THING=" + idObj + " AND (B.ACCESSRIGHTS > 0 OR B.CHA_TIPO_DIRITTO='P') AND C.ID_AMM =" + IDAMM);
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteQuery(out dataSet, "DIRITTI_UTENTI_DELETED", sql);
        }

        public void GetListaStoriaDiritti(out DataSet dataSet, string idObj, string tipoObj)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__DPA_LOG");
            q.setParam("param1", idObj);
            if (tipoObj.Equals("D"))
            {
                q.setParam("param2", "in ('EDITING_ACL','CESSIONE_DOC')");
            }
            else
                q.setParam("param2", "in ('EDITING_FASC_ACL','CESSIONE_FASC')");
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteQuery(out dataSet, "STORIA_DIRITTI", sql);
        }
        #endregion

        #region Login

        public void GetLibrary(out string library)
        {
            logger.Debug("getLibrary");
            //string sql = "SELECT LIBRARY_NAME FROM REMOTE_LIBRARIES"; 
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RemoteLibraries");
            string sql = q.getSQL();
            logger.Debug(sql);
            library = null;
            try
            {
                this.ExecuteScalar(out library, sql);
                //library = db.executeScalar(sql).ToString();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //throw new Exception("F_WrongLogin");
                logger.Debug("Errore nella gestione delle trasmissioni (Query - GetLibrary)");
                throw new Exception("F_WrongLogin");

            }
        }

        public void GetDominio(out string dominio, string idPeople)
        {
            logger.Debug("GetDominio");
            dominio = System.String.Empty;

            //string sql = " SELECT NETWORK_ID FROM NETWORK_ALIASES WHERE PERSONORGROUP= " + idPeople;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_NetworkAliases");
            q.setParam("param1", idPeople);

            string sql = q.getSQL();
            //logger.Debug (sql);	 evito il log x sicurezza della password	
            try
            {
                //dominio = db.executeScalar(sql).ToString();
                if (!this.ExecuteScalar(out dominio, sql))
                    throw new System.Exception();
            }
            catch (Exception ex)
            {
                dominio = System.String.Empty;
                logger.Debug("Errore nella Verifica del Dominio Utente.", ex);
            }

        }

        /// <summary>
        /// Modifica password in cleartext
        /// </summary>
        /// <param name="userLogin"></param>
        /// <param name="oldPassword"></param>
        /// <returns></returns>
        public bool CambiaPassword(DocsPaVO.utente.UserLogin user, string oldPassword)
        {
            bool result = false;

            DataSet dataSet = new DataSet();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_People");
                q.setParam("param1", "USER_PASSWORD = '" + user.Password + "'");
                q.setParam("param2", "USER_ID = '" + user.UserName + "' AND ID_AMM = " + user.IdAmministrazione);
                string sql = q.getSQL();

                result = this.ExecuteNonQuery(sql);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
            }
            return result;
        }

        /// <summary>Esegue la login su ETDOC.
        /// ATTENZIONE: il metodo non esegue alcuna validazione dei parametri di input.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool UserLogin(out string peopleId, string userName, string password, string idAmm, string modulo)
        {
            bool result = false;
            peopleId = System.String.Empty;


            try
            {
                //old:
                //				string sql = "SELECT COUNT(*) FROM People WHERE user_Id='" + userName + "' AND user_password='" + password + "'";
                //				string count;
                //				
                //				if(this.ExecuteScalar(out count, sql))
                //				{
                //					result = Int32.Parse(count);
                //				}
                //new:
                //SELECT SYSTEM_ID FROM PEOPLE WHERE UPPER(user_Id)= UPPER(' userName ') AND user_password = 'password' AND DISABLED = '0'
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Login");
                q.setParam("param1", userName);
                q.setParam("param2", password);
                q.setParam("param3", idAmm);

                string command = q.getSQL();

                if (!this.ExecuteScalar(out peopleId, command))
                    throw new System.Exception();

                result = (!string.IsNullOrEmpty(peopleId));
            }
            catch (Exception ex)
            {
                result = false;
                peopleId = System.String.Empty;
                logger.Debug("Errore nella Login.", ex);
            }

            return result;
        }

        /// <summary>Esegue la verifica sulla People della USERID.
        /// ATTENZIONE: il metodo non esegue alcuna validazione dei parametri di input.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="modulo"></param>
        /// <returns></returns>
        public bool UserLogin(out string peopleId, string userName, string idAmm, string modulo)
        {
            bool result = true;
            peopleId = System.String.Empty;

            try
            {


                string queryName = string.Empty;

                if (modulo != null)
                {
                    if (DocsPaUtils.Moduli.ModuliAuthManager.IsModuloCentroServizi(modulo))
                    {
                        // login al modulo centro servizi della conservazione
                        queryName = "S_LOGIN_CENTRO_SERVIZI_CONSERVAZIONE";
                    }
                    else
                    {
                        // login al sistema di gestione documentale
                        queryName = "S_Login_verificaUser";
                    }
                }
                else
                {
                    // login al sistema di gestione documentale
                    queryName = "S_Login_verificaUser";
                }

                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);
                queryDef.setParam("param1", userName);
                queryDef.setParam("param2", idAmm);

                string command = queryDef.getSQL();

                logger.Debug(command);
                DataSet ds = null;

                if (!this.ExecuteQuery(out ds, command))
                    throw new System.Exception();
                else
                {
                    if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                        peopleId = ds.Tables[0].Rows[0][0].ToString();
                    }
                    else throw new System.Exception();

                }

                result = (!string.IsNullOrEmpty(peopleId));
            }
            catch (Exception ex)
            {
                result = false;
                peopleId = System.String.Empty;
                logger.Debug("Errore: Utente non valido.", ex);
            }

            return result;
        }




        #endregion

        #region RegistriManager
        public void GetRegistro(string idRegistro, ref DocsPaVO.utente.Registro reg)
        {
            /*string sql = 
                "SELECT A.SYSTEM_ID, A.VAR_CODICE, A.NUM_RIF, A.VAR_DESC_REGISTRO, " +
                "A.VAR_EMAIL_REGISTRO, A.CHA_STATO, A.ID_AMM, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_OPEN",false) + " AS DTA_OPEN, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_CLOSE",false) + " AS DTA_CLOSE, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_ULTIMO_PROTO",false) + " AS DTA_ULTIMO_PROTO " +
                "FROM DPA_EL_REGISTRI A WHERE A.SYSTEM_ID=" + idRegistro;*/
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAElRegistri");
            q.setParam("param1", "SYSTEM_ID, VAR_CODICE, NUM_RIF, VAR_DESC_REGISTRO, " +
                "VAR_EMAIL_REGISTRO, CHA_STATO, ID_AMM, " +
                DocsPaDbManagement.Functions.Functions.ToChar("DTA_OPEN", false) + "," + //DocsPaWS.Utils.dbControl.toChar("DTA_OPEN",false) + 
                DocsPaDbManagement.Functions.Functions.ToChar("DTA_CLOSE", false) + "," + //DocsPaWS.Utils.dbControl.toChar("DTA_CLOSE",false) + 
                DocsPaDbManagement.Functions.Functions.ToChar("DTA_ULTIMO_PROTO", false) + " , ID_RUOLO_AOO,ID_RUOLO_RESP,ID_PEOPLE_AOO, CHA_AUTO_INTEROP, CHA_RF, CHA_DISABILITATO, ID_AOO_COLLEGATA, INVIO_RICEVUTA_MANUALE, FLAG_WSPIA, VAR_PREG, ANNO_PREG, DIRITTO_RUOLO_AOO"); //DocsPaWS.Utils.dbControl.toChar("DTA_ULTIMO_PROTO",false));
            q.setParam("param2", "SYSTEM_ID=" + idRegistro);
            string sql = q.getSQL();
            logger.Debug(sql);
            DataSet dataSet;
            this.ExecuteQuery(out dataSet, sql);
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                reg = new DocsPaVO.utente.Registro();
                reg.systemId = dataRow[0].ToString();
                reg.codRegistro = dataRow[1].ToString();
                reg.codice = dataRow[2].ToString();
                reg.descrizione = dataRow[3].ToString();
                reg.email = dataRow[4].ToString();
                reg.stato = dataRow[5].ToString();
                reg.idAmministrazione = dataRow[6].ToString();
                reg.codAmministrazione = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                reg.dataApertura = dataRow[7].ToString();
                reg.dataChiusura = dataRow[8].ToString();
                reg.dataUltimoProtocollo = dataRow[9].ToString();
                reg.idRuoloAOO = dataRow[10].ToString();
                reg.idRuoloResp = dataRow[11].ToString();
                reg.idUtenteAOO = dataRow[12].ToString();
                reg.autoInterop = dataRow[13].ToString();
                reg.chaRF = dataRow[14].ToString();
                reg.rfDisabled = dataRow[15].ToString();
                if (dataRow[15].ToString().Equals("1"))
                    reg.Sospeso = true;
                else
                    reg.Sospeso = false;
                reg.idAOOCollegata = dataRow[16].ToString();
                reg.invioRicevutaManuale = dataRow[17].ToString();
                reg.FlagWspia = dataRow[18].ToString();
                //Andrea De Marco- campi VAR_PREG, ANNO_PREG aggiunti per Import Pregressi
                if (dataRow[19].ToString().Equals("1"))
                {
                    reg.flag_pregresso = true;
                }
                else
                {
                    reg.flag_pregresso = false;
                }
                reg.anno_pregresso = dataRow[20].ToString();
                reg.Diritto_Ruolo_AOO = dataRow[21].ToString();

            }
            dataSet.Dispose();
        }

        public string[] getidRuoloUtenteAOO(string codAOO)
        {
            string[] rtn = null;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPARegistriAOO1");
            q.setParam("param1", "UPPER(VAR_CODICE)=" + codAOO.ToUpper());
            string sql = q.getSQL();
            logger.Debug(sql);
            DataSet dataSet;
            this.ExecuteQuery(out dataSet, sql);
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                rtn = new string[2];
                rtn[0] = dataRow[0].ToString();
                rtn[1] = dataRow[1].ToString();
            }
            dataSet.Dispose();

            return rtn;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codAOO"></param>
        /// <param name="reg"></param>
        public void GetRegistroByCodAOO(string codAOO, string idAmm, ref DocsPaVO.utente.Registro reg)
        {
            /*string sql = 
                "SELECT A.SYSTEM_ID, A.VAR_CODICE, A.NUM_RIF, A.VAR_DESC_REGISTRO, " +
                "A.VAR_EMAIL_REGISTRO, A.CHA_STATO, A.ID_AMM, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_OPEN",false) + " AS DTA_OPEN, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_CLOSE",false) + " AS DTA_CLOSE, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_ULTIMO_PROTO",false) + " AS DTA_ULTIMO_PROTO " +
                "FROM DPA_EL_REGISTRI A WHERE A.SYSTEM_ID=" + idRegistro;*/
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAElRegistri");
            q.setParam("param1", "SYSTEM_ID, VAR_CODICE, NUM_RIF, VAR_DESC_REGISTRO, " +
                "VAR_EMAIL_REGISTRO, CHA_STATO, ID_AMM, " +
                DocsPaDbManagement.Functions.Functions.ToChar("DTA_OPEN", false) + "," + //DocsPaWS.Utils.dbControl.toChar("DTA_OPEN",false) + 
                DocsPaDbManagement.Functions.Functions.ToChar("DTA_CLOSE", false) + "," + //DocsPaWS.Utils.dbControl.toChar("DTA_CLOSE",false) + 
                DocsPaDbManagement.Functions.Functions.ToChar("DTA_ULTIMO_PROTO", false) + " , ID_RUOLO_AOO,ID_RUOLO_RESP,ID_PEOPLE_AOO,  CHA_AUTO_INTEROP, CHA_RF, CHA_DISABILITATO, ID_AOO_COLLEGATA, INVIO_RICEVUTA_MANUALE, VAR_PREG, ANNO_PREG, DIRITTO_RUOLO_AOO "); //DocsPaWS.Utils.dbControl.toChar("DTA_ULTIMO_PROTO",false));
            q.setParam("param2", "UPPER(VAR_CODICE)='" + codAOO.ToUpper() + "' and ID_AMM = " + idAmm);
            string sql = q.getSQL();
            logger.Debug(sql);
            DataSet dataSet;
            this.ExecuteQuery(out dataSet, sql);
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                reg = new DocsPaVO.utente.Registro();
                reg.systemId = dataRow[0].ToString();
                reg.codRegistro = dataRow[1].ToString();
                reg.codice = dataRow[2].ToString();
                reg.descrizione = dataRow[3].ToString();
                reg.email = dataRow[4].ToString();
                reg.stato = dataRow[5].ToString();
                reg.idAmministrazione = dataRow[6].ToString();
                reg.codAmministrazione = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                reg.dataApertura = dataRow[7].ToString();
                reg.dataChiusura = dataRow[8].ToString();
                reg.dataUltimoProtocollo = dataRow[9].ToString();
                reg.idRuoloAOO = dataRow[10].ToString();
                reg.idRuoloResp = dataRow[11].ToString();
                reg.idUtenteAOO = dataRow[12].ToString();
                reg.autoInterop = dataRow[13].ToString();
                reg.chaRF = dataRow[14].ToString();
                reg.rfDisabled = dataRow[15].ToString();
                reg.idAOOCollegata = dataRow[16].ToString();
                reg.invioRicevutaManuale = dataRow[17].ToString();
                if (dataRow[18].ToString().Equals("1"))
                {
                    reg.flag_pregresso = true;
                }
                else
                {
                    reg.flag_pregresso = false;
                }
                reg.anno_pregresso = dataRow[19].ToString();
                reg.Diritto_Ruolo_AOO = dataRow[20].ToString();
            }
            dataSet.Dispose();
        }

        public ArrayList GetRegistriRuolo(string idRuolo)
        {

            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Utenti> GetRegistriRuolo");

            ArrayList registri = new ArrayList();
            //ricerca dei registri associati al ruolo
            /*string sql= 
                "SELECT A.SYSTEM_ID, A.VAR_CODICE, A.NUM_RIF, A.VAR_DESC_REGISTRO, " +
                "A.VAR_EMAIL_REGISTRO, A.CHA_STATO, A.ID_AMM, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_OPEN",false) + " AS DTA_OPEN, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_CLOSE",false) + " AS DTA_CLOSE, " +
                DocsPaWS.Utils.dbControl.toChar("A.DTA_ULTIMO_PROTO",false) + " AS DTA_ULTIMO_PROTO " +
                "FROM DPA_L_RUOLO_REG B, DPA_EL_REGISTRI A " +
                "WHERE A.SYSTEM_ID=B.ID_REGISTRO AND ID_RUOLO_IN_UO=" + idRuolo +
                " ORDER BY B.CHA_PREFERITO DESC, A.VAR_DESC_REGISTRO";*/
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_L_RUOLO_REG__EL_REGISTRI");
            /*			q.setParam("param1", DocsPaWS.Utils.dbControl.toChar("A.DTA_OPEN",false) + " AS DTA_OPEN, " +
                            DocsPaWS.Utils.dbControl.toChar("A.DTA_CLOSE",false) + " AS DTA_CLOSE, " +
                            DocsPaWS.Utils.dbControl.toChar("A.DTA_ULTIMO_PROTO",false) + " AS DTA_ULTIMO_PROTO ");*/
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_OPEN", false) + " AS DTA_OPEN, " +
                DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CLOSE", false) + " AS DTA_CLOSE, " +
                DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_ULTIMO_PROTO", false) + " AS DTA_ULTIMO_PROTO " + ", A.ID_RUOLO_AOO,A.ID_RUOLO_RESP,A.ID_PEOPLE_AOO, A.CHA_AUTO_INTEROP, A.VAR_PREG, A.ANNO_PREG");
            q.setParam("param2", "ID_RUOLO_IN_UO=" + idRuolo);
            //Andrea - Aggiunto NVL(A.VAR_PREG,0) ASC, A.VAR_DESC_REGISTRO ASC per ordinare per primi i registri non di pregresso
            if (DBType.ToUpper().Equals("ORACLE"))
                q.setParam("param3", "B.CHA_PREFERITO DESC NULLS LAST,  NVL(A.VAR_PREG,0) ASC, A.VAR_CODICE, A.VAR_DESC_REGISTRO ASC");
            else
                q.setParam("param3", "COALESCE(B.CHA_PREFERITO,1),  ISNULL(A.VAR_PREG,0) ASC, A.VAR_CODICE, A.VAR_DESC_REGISTRO ASC");
            string sql = q.getSQL();
            logger.Debug(sql);

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, sql);
            if (dataSet != null)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro();

                    reg.systemId = row[0].ToString();
                    reg.codRegistro = row[1].ToString();
                    reg.codice = row[2].ToString();
                    reg.descrizione = row[3].ToString();
                    reg.email = row[4].ToString();
                    reg.stato = row[5].ToString();
                    reg.idAmministrazione = row[6].ToString();
                    reg.codAmministrazione = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                    reg.dataApertura = row[7].ToString().Trim();
                    reg.dataChiusura = row[8].ToString().Trim();
                    reg.dataUltimoProtocollo = row[9].ToString();
                    reg.idRuoloAOO = row[10].ToString();
                    reg.idRuoloResp = row[11].ToString();
                    reg.idUtenteAOO = row[12].ToString();
                    reg.autoInterop = row[13].ToString();
                    reg.Diritto_Ruolo_AOO = row["DIRITTO_RUOLO_AOO"].ToString();
                    if (!row["INVIO_RICEVUTA_MANUALE"].Equals(System.DBNull.Value))
                        reg.invioRicevutaManuale = (row["INVIO_RICEVUTA_MANUALE"].ToString().Equals("0")) ? "0" : "1";
                    else
                        reg.invioRicevutaManuale = "1";
                    if (row["FLAG_WSPIA"].Equals(System.DBNull.Value))
                        reg.FlagWspia = "0";
                    else reg.FlagWspia = row["FLAG_WSPIA"].ToString();
                    if (row.Table.Columns.Contains("VAR_PREG") && row.Table.Columns.Contains("ANNO_PREG"))
                    {
                        if (!string.IsNullOrEmpty(row["VAR_PREG"].ToString()))
                        {
                            reg.flag_pregresso = (row["VAR_PREG"].Equals("0") ? false : true);
                            reg.anno_pregresso = row["ANNO_PREG"].ToString();
                        }
                    }

                    registri.Add(reg);
                }
            }

            // leggo il numero di protocollo solo se lo stato del registro è chiuso
            for (int i = 0; i < registri.Count; i++)
            {
                DocsPaVO.utente.Registro reg = (DocsPaVO.utente.Registro)registri[i];
                if (reg.stato.Equals("C"))
                {
                    if (reg.dataUltimoProtocollo != null && reg.dataUltimoProtocollo != "" && reg.dataUltimoProtocollo.Substring(6, 4).Equals(DocsPaUtils.Functions.Functions.GetDate(false).Substring(6, 4)))
                    {
                        /*queryString = 
                            "SELECT NUM_RIF FROM DPA_REG_PROTO WHERE ID_REGISTRO = " + reg.systemId;
                        logger.Debug(queryString);
                        reg.ultimoNumeroProtocollo = db.executeScalar(queryString).ToString();*/
                        string res;
                        this.ExecuteScalar(out res, GetUltimoNumProto(reg.systemId));

                        reg.ultimoNumeroProtocollo = res;
                    }
                    else
                        reg.ultimoNumeroProtocollo = "1";
                    registri[i] = reg;
                }
            }
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Utenti> GetRegistriRuolo");
            return registri;
        }

        public ArrayList getRegistriNoFiltroAOO(string idAmm)
        {

            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Utenti> getRegistriNoFiltroAOO");

            ArrayList registri = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_LIST_REGISTRI");
            q.setParam("param1", idAmm);
            //Modificato l'ordinamento per far apparire nella ricerca prima i registri Non di Pregresso
            //q.setParam("order", "CODICE");
            if (DBType.ToUpper().Equals("ORACLE"))
                q.setParam("order", "nvl(VAR_PREG,-1) ASC, CODICE ASC");
            else
                q.setParam("order", "isnull(VAR_PREG,-1) ASC, CODICE ASC");
            string sql = q.getSQL();
            logger.Debug(sql);
            DataSet dataSet;
            this.ExecuteQuery(out dataSet, sql);
            if (dataSet != null)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro();
                    reg.systemId = row["IDREGISTRO"].ToString();
                    reg.codRegistro = row["CODICE"].ToString();
                    //reg.codice = row["CODICE"].ToString();
                    reg.descrizione = row["DESCRIZIONE"].ToString();
                    reg.idAmministrazione = row["IDAMMINISTRAZIONE"].ToString();
                    if (row.Table.Columns.Contains("VAR_PREG") && row.Table.Columns.Contains("ANNO_PREG"))
                    {
                        if (!string.IsNullOrEmpty(row["VAR_PREG"].ToString()))
                        {
                            reg.flag_pregresso = (row["VAR_PREG"].Equals("0") ? false : true);
                            reg.anno_pregresso = row["ANNO_PREG"].ToString();
                        }
                    }

                    registri.Add(reg);
                }
            }
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Utenti> getRegistriNoFiltroAOO");
            return registri;
        }

        public string GetUltimoNumProto(string idRegistro)
        {

            //string sql = 	"SELECT NUM_RIF FROM DPA_REG_PROTO WHERE ID_REGISTRO = " + idRegistro;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPARegProto");
            q.setParam("param1", "ID_REGISTRO = " + idRegistro);
            string sql = q.getSQL();
            logger.Debug(sql);
            return sql;
        }

        public string GetCodiceRegistroBySystemId(string idRegistro)
        {
            string codiceRegistro = "";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DescRegistroBySystemId");
            q.setParam("param1", idRegistro);
            string sql = q.getSQL();
            logger.Debug(sql);

            this.ExecuteScalar(out codiceRegistro, sql);
            return codiceRegistro;
        }

        public string GetEmailUtente(string idPeople)
        {
            string emailMittente = "";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_People_all");
            q.setParam("param1", "EMAIL_ADDRESS");
            q.setParam("param2", "SYSTEM_ID = " + idPeople);
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteScalar(out emailMittente, sql);

            return emailMittente;
        }

        public string GetFromEmailUtente(string idPeople)
        {
            string emailFromMittente = "";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_People_all");
            q.setParam("param1", "FROM_EMAIL_ADDRESS");
            q.setParam("param2", "SYSTEM_ID = " + idPeople);
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteScalar(out emailFromMittente, sql);

            return emailFromMittente;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registro"></param>
        /// <param name="infoUtente"></param>
        public void RegistroCambiaStato(string idPeople, string idCorrGlobali, ref DocsPaVO.utente.Registro registro)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Utenti > RegistroCambiaStato");
            //string sql = "UPDATE DPA_EL_REGISTRI SET ";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPAElregistri");
            string firstParam = "";
            if (registro.stato.Equals("A"))
            {
                registro.stato = "C";
                registro.dataChiusura = DocsPaUtils.Functions.Functions.GetDate(false);
                firstParam += " DTA_CLOSE = " + DocsPaDbManagement.Functions.Functions.GetDate();  //DocsPaWS.Utils.dbControl.getDate();
            }
            else
            {
                registro.stato = "A";
                registro.dataApertura = DocsPaUtils.Functions.Functions.GetDate(false);
                registro.dataChiusura = "";
                firstParam += " DTA_CLOSE = null, DTA_OPEN = " + DocsPaDbManagement.Functions.Functions.GetDate();//DocsPaWS.Utils.dbControl.getDate();
            }
            firstParam += ", CHA_STATO = '" + registro.stato + "'";
            q.setParam("param1", firstParam);
            q.setParam("param2", "SYSTEM_ID = " + registro.systemId);
            string sql = q.getSQL();
            try
            {
                //VIENE FATTO UN CONTROLLO AL CAMBIO DELL'ANNO E REIMPOSTATO AD 1 IL NUM. DI PROTOCOLLO 	
                if (!registro.stato.Equals("C"))
                {
                    #region commentato
                    /*string sqlString = 
						"UPDATE DPA_REG_PROTO SET NUM_RIF=1 " +
						"WHERE ID_REGISTRO = (SELECT SYSTEM_ID FROM DPA_EL_REGISTRI " +
						"WHERE SYSTEM_ID= " + registro.systemId + " AND " + DocsPaWS.Utils.dbControl.getYear(DocsPaWS.Utils.dbControl.getDate()) + "!=" + DocsPaWS.Utils.dbControl.getYear("DTA_OPEN") + ")";*/
                    //q.setParam("param2","ID_REGISTRO = (SELECT SYSTEM_ID FROM DPA_EL_REGISTRI " + "WHERE SYSTEM_ID= " + registro.systemId + " AND " +  DocsPaDbManagement.Functions.Functions.GetYear(DocsPaDbManagement.Functions.Functions.GetDate()) + "!=" + DocsPaDbManagement.Functions.Functions.GetYear("DTA_OPEN") + ")");
                    //DocsPaWS.Utils.dbControl.getYear(DocsPaWS.Utils.dbControl.getDate()) 
                    //+ "!=" + DocsPaWS.Utils.dbControl.getYear("DTA_OPEN") + ")");
                    #endregion

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPARegproto");
                    q.setParam("param1", " NUM_RIF=1");
                    q.setParam("param2", "ID_REGISTRO = (SELECT SYSTEM_ID FROM DPA_EL_REGISTRI " +
                        "WHERE SYSTEM_ID= " + registro.systemId + " AND " + DocsPaDbManagement.Functions.Functions.GetYear(DocsPaDbManagement.Functions.Functions.GetDate()) + "!=" + DocsPaDbManagement.Functions.Functions.GetYear("DTA_OPEN") + ")");

                    string sqlString = q.getSQL();
                    logger.Debug(sqlString);
                    this.ExecuteNonQuery(sqlString);
                }
                //VIENE AGGIORNATO LO STATO DEL REGISTRO				
                logger.Debug(sql);
                this.ExecuteNonQuery(sql);

                //SE IL REGISTRO E' CHIUSO VIENE STORICIZZATA LA MODIFICA
                if (registro.stato.Equals("C"))
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPARegProto");
                    q.setParam("param1", "ID_REGISTRO = " + registro.systemId);
                    string sqlQuery = q.getSQL();
                    string res;
                    this.ExecuteScalar(out res, sqlQuery);
                    registro.ultimoNumeroProtocollo = res;
                    AggiornaStorico(idPeople, idCorrGlobali, registro);
                }
                //this.CloseConnection();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //this.CloseConnection();
                //throw new Exception("F_System");
                logger.Debug("Errore nella gestione del registro (Query - RegistroCambiaStato)", e);
                throw new Exception("F_System");
            }
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Utenti > RegistroCambiaStato");
            //return registro;
        }

        public void RegistroModifica(ref DocsPaVO.utente.Registro registro, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaUtils.Query q;
            string sql;
            logger.Debug("modificaRegistro");
            if (registro.stato.Equals("A"))
            {
                logger.Debug("Errore nella gestione delle trasmissioni (Query - RegistroModifica). Il registro non è chiuso!");
                //throw new Exception("Il registro non è chiuso");
                throw new Exception("Il registro non è chiuso");
            }
            try
            {
                //this.OpenConnection();
                /*string sql = 
                    "UPDATE DPA_EL_REGISTRI SET " +
                    "VAR_EMAIL_REGISTRO = '" + registro.email + "'" +
                    ", VAR_DESC_REGISTRO = '" + registro.descrizione + "'" +
                    " WHERE SYSTEM_ID = " + registro.systemId;*/
                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPAElregistri");
                q.setParam("param1", "VAR_EMAIL_REGISTRO = '" + registro.email + "'" +
                    ", VAR_DESC_REGISTRO = '" + registro.descrizione + "'");
                q.setParam("param2", "SYSTEM_ID = " + registro.systemId);
                sql = q.getSQL();
                logger.Debug(sql);
                this.ExecuteNonQuery(sql);
                /*sql = 
                    "UPDATE DPA_REG_PROTO SET " +
                    "NUM_RIF = " + registro.ultimoNumeroProtocollo + 
                    " WHERE ID_REGISTRO = " + registro.systemId;*/
                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPARegproto");
                q.setParam("param1", "NUM_RIF = " + registro.ultimoNumeroProtocollo);
                q.setParam("param2", "ID_REGISTRO = " + registro.systemId);
                sql = q.getSQL();
                logger.Debug(sql);
                this.ExecuteNonQuery(sql);
                //this.CloseConnection();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //this.CloseConnection();
                //throw new Exception("F_System");
                logger.Debug("Errore nella gestione delle trasmissioni (Query - RegistroModifica)", e);
                throw new Exception("F_System");

            }
            //return registro;
        }

        private void AggiornaStorico(string idPeople, string idCorrGlobali, DocsPaVO.utente.Registro registro)
        {
            /*string sql =
                "INSERT INTO DPA_REGISTRO_STO " +
                "(" + DocsPaWS.Utils.dbControl.getSystemIdColName() + " ID_REGISTRO, DTA_OPEN, DTA_CLOSE, NUM_RIF, ID_PEOPLE, ID_RUOLO_IN_UO) " +
                "SELECT " + DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_REGISTRO_STO") + 
                "SYSTEM_ID, DTA_OPEN, DTA_CLOSE, " + registro.ultimoNumeroProtocollo + ", " + infoUtente.idPeople + "," + infoUtente.idCorrGlobali + 
                " FROM DPA_EL_REGISTRI WHERE SYSTEM_ID = " + registro.systemId;*/
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPARegistroSto");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName() + " ID_REGISTRO, DTA_OPEN, DTA_CLOSE, NUM_RIF, ID_PEOPLE, ID_RUOLO_IN_UO");
            //DocsPaWS.Utils.dbControl.getSystemIdColName() + " ID_REGISTRO, DTA_OPEN, DTA_CLOSE, NUM_RIF, ID_PEOPLE, ID_RUOLO_IN_UO");
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_REGISTRO_STO") +
                //DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_REGISTRO_STO") + 
                "SYSTEM_ID, DTA_OPEN, DTA_CLOSE, " + registro.ultimoNumeroProtocollo + ", " + idPeople + "," + idCorrGlobali);
            q.setParam("param3", "SYSTEM_ID = " + registro.systemId);
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteNonQuery(sql);
        }
        #endregion

        #region UserManager
        /// <summary>
        /// get DST da DPA_LOGIN.DST
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetDST(string userId)
        {

            string result = null;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_LOGIN_D");
                q.setParam("param1", "UPPER(USER_ID)='" + userId.ToUpper() + "'");
                q.setParam("param2", "DST");
                string queryString = q.getSQL();
                logger.Debug(queryString);

                IDataReader dr = this.ExecuteReader(queryString);

                if (dr == null)
                {
                    throw new Exception();
                }
                if (dr != null && dr.FieldCount > 0)
                {
                    while (dr.Read())
                    {
                        result = dr[0].ToString();
                    }
                }

                if (dr != null)
                {
                    dr.Dispose();
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                result = null;
            }
            finally
            {
                CloseConnection();
            }

            return result;

        }
        public string GetPassword(string idPeople)
        {
            string result = null;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PeopleP");
                q.setParam("param1", "USER_PASSWORD");
                q.setParam("param2", idPeople);
                string queryString = q.getSQL();
                logger.Debug(queryString);


                IDataReader dr = this.ExecuteReader(queryString);
                if (dr == null)
                {
                    throw new Exception("Errore nel metodo GetPassword");
                }
                if (dr != null && dr.FieldCount > 0)
                {
                    while (dr.Read())
                    {
                        result = dr[0].ToString();
                    }
                }
                if (dr != null)
                {
                    dr.Dispose();
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                result = null;
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        public string GetIDUtCorr(string idPeople)
        {
            string result = null;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_IDCorrGlobaliPeople");
            q.setParam("param1", idPeople);
            string queryString = q.getSQL();
            logger.Debug(queryString);

            this.ExecuteScalar(out result, queryString);

            return result;
        }

        public DocsPaVO.utente.Corrispondente GetCorrispondente(string idCorrispondente, bool isEnable)
        {
            logger.Debug("getCorrispondente");
            DocsPaVO.utente.Corrispondente corrispondente = new DocsPaVO.utente.Corrispondente();
            /*string queryString = 
                "SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI " +
                "WHERE SYSTEM_ID=" + idCorrispondente;*/
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
            q.setParam("param1", "ID_PEOPLE");
            q.setParam("param2", "SYSTEM_ID=" + idCorrispondente);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            string idPeople = "";
            try
            {
                this.ExecuteScalar(out idPeople, queryString);
            }
            catch (Exception) { }
            if (idPeople != null && !idPeople.Equals(""))
                //OLD: 11/02/2007
                //corrispondente = GetUtente(idPeople);
                corrispondente = GetUtenteNoFiltroDisabled(idPeople);
            else
            {
                if (!isEnable)
                {
                    corrispondente = GetRuolo(idCorrispondente);
                }
                else
                {
                    corrispondente = GetRuoloEnabledAndDisabled(idCorrispondente);
                }
            }
            if (corrispondente != null)
                corrispondente.systemId = idCorrispondente;
            //corrispondente.notificaConAllegato=true;
            return corrispondente;
        }


        /// <summary>
        /// Creato un override del metodo per inserire anche la condizione sul registro nella ricerca del corrispondente
        /// </summary>
        /// <param name="id_amm"></param>
        /// <param name="cod_rubrica"></param>
        /// <param name="tipoIE"></param>
        /// <param name="condRegistri"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Corrispondente GetCorrispondenteByCodRubrica(string id_amm, string cod_rubrica, DocsPaVO.addressbook.TipoUtente tipoIE, string condRegistri, bool storicizzato)
        {
            logger.Debug("getCorrispondente");

            string ie_where = "";
            switch (tipoIE)
            {
                case DocsPaVO.addressbook.TipoUtente.INTERNO:
                    ie_where = " and cha_tipo_ie='I'";
                    break;
                case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                    ie_where = " and cha_tipo_ie='E'";
                    break;
                case DocsPaVO.addressbook.TipoUtente.GLOBALE:
                    ie_where = "";
                    break;
            }

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
            q.setParam("param1", "SYSTEM_ID,CHA_TIPO_URP,ID_GRUPPO,ID_PEOPLE,CHA_TIPO_IE");
            string param2 = string.Empty;
            if (!storicizzato)
                param2 = "UPPER(VAR_COD_RUBRICA)='" + cod_rubrica.ToUpper().Replace("'", "''") + "'" + ie_where + " AND ID_AMM=" + id_amm;
            else
                param2 = "UPPER(VAR_COD_RUBRICA) like '" + cod_rubrica.ToUpper().Replace("'", "''") + "%'" + ie_where + " AND ID_AMM=" + id_amm;
            if (!string.IsNullOrEmpty(condRegistri))
                param2 += condRegistri;

            q.setParam("param2", param2);

            string commandText = q.getSQL();
            logger.Debug(commandText);
            DataSet ds = new DataSet();

            try
            {
                if (this.ExecuteQuery(out ds, "corrispondenti", commandText))
                {
                    DocsPaVO.utente.Corrispondente corrispondente = new DocsPaVO.utente.Corrispondente();
                    if (ds.Tables["corrispondenti"].Rows.Count >= 1)
                    {
                        DataRow dr = ds.Tables["corrispondenti"].Rows[0];
                        string sid = (string)dr["SYSTEM_ID"].ToString();
                        string tipo = (string)dr["CHA_TIPO_URP"].ToString();
                        string id_gruppo = (string)dr["ID_GRUPPO"].ToString();
                        string id_people = (string)dr["ID_PEOPLE"].ToString();

                        bool isInterno = (dr["CHA_TIPO_IE"].ToString() == "I");

                        DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                        DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                        qco.systemId = sid;
                        qco.idAmministrazione = id_amm;
                        qco.fineValidita = true;

                        if (tipo.ToUpper().Equals("F") || tipo.ToUpper().Equals("L"))
                            return u.GetCorrispondenteRF(qco, tipo);
                        else
                        {
                            if (isInterno)
                                return u.GetCorrispondenteInt(qco);
                            else
                                return u.GetCorrispondenteEst(qco);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella ricerca di uno specifico corrispondente (" + cod_rubrica + ")", e);
                return null;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_amm"></param>
        /// <param name="cod_rubrica"></param>
        /// <param name="tipoIE"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Corrispondente GetCorrispondenteByCodRubrica(string id_amm, string cod_rubrica, DocsPaVO.addressbook.TipoUtente tipoIE)
        {
            logger.Debug("getCorrispondente");

            string ie_where = "";
            switch (tipoIE)
            {
                case DocsPaVO.addressbook.TipoUtente.INTERNO:
                    ie_where = " and cha_tipo_ie='I'";
                    break;
                case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                    ie_where = " and cha_tipo_ie='E'";
                    break;
                case DocsPaVO.addressbook.TipoUtente.GLOBALE:
                    ie_where = "";
                    break;
            }

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
            q.setParam("param1", "SYSTEM_ID,CHA_TIPO_URP,ID_GRUPPO,ID_PEOPLE,CHA_TIPO_IE");
            q.setParam("param2", "UPPER(VAR_COD_RUBRICA)='" + cod_rubrica.ToUpper().Replace("'", "''") + "'" + ie_where + " AND ID_AMM=" + id_amm);

            string commandText = q.getSQL();
            logger.Debug(commandText);
            DataSet ds = new DataSet();

            try
            {
                if (this.ExecuteQuery(out ds, "corrispondenti", commandText))
                {
                    DocsPaVO.utente.Corrispondente corrispondente = new DocsPaVO.utente.Corrispondente();
                    if (ds.Tables["corrispondenti"].Rows.Count >= 1)
                    {
                        DataRow dr = ds.Tables["corrispondenti"].Rows[0];
                        string sid = (string)dr["SYSTEM_ID"].ToString();
                        string tipo = (string)dr["CHA_TIPO_URP"].ToString();
                        string id_gruppo = (string)dr["ID_GRUPPO"].ToString();
                        string id_people = (string)dr["ID_PEOPLE"].ToString();

                        bool isInterno = (dr["CHA_TIPO_IE"].ToString() == "I");

                        DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                        DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                        qco.systemId = sid;
                        qco.idAmministrazione = id_amm;
                        qco.fineValidita = true;

                        if (tipo.ToUpper().Equals("F") || tipo.ToUpper().Equals("L"))
                            return u.GetCorrispondenteRF(qco, tipo);
                        else
                        {
                            if (isInterno)
                                return u.GetCorrispondenteInt(qco);
                            else
                                return u.GetCorrispondenteEst(qco);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella ricerca di uno specifico corrispondente (" + cod_rubrica + ")", e);
                return null;
            }
            return null;
        }


        public DocsPaVO.utente.Corrispondente GetCorrispondenteByEmail(string id_amm, string mail, string id_reg)
        {
            logger.Debug("getCorrispondenteByMail start");

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_CORRISPONDENTI_BY_EMAIL");
            q.setParam("email", mail.Replace("'", "''"));
            q.setParam("idAmministrazione", id_amm);
            q.setParam("idRegistro", id_reg);

            string commandText = q.getSQL();
            logger.Debug(commandText);
            DataSet ds = new DataSet();

            try
            {
                if (this.ExecuteQuery(out ds, "corrispondenti", commandText))
                {
                    if (ds.Tables["corrispondenti"].Rows.Count >= 1)
                    {
                        Corrispondente corr = null;
                        foreach (DataRow dr in ds.Tables["corrispondenti"].Rows)
                        {
                            //DataRow dr = ds.Tables["corrispondenti"].Rows[0];
                            string sid = (string)dr["SYSTEM_ID"].ToString();
                            string tipo = (string)dr["CHA_TIPO_URP"].ToString();
                            string id_gruppo = (string)dr["ID_GRUPPO"].ToString();
                            string id_people = (string)dr["ID_PEOPLE"].ToString();
                            bool isInterno = (dr["CHA_TIPO_IE"].ToString() == "I");
                            DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                            qco.systemId = sid;
                            qco.idAmministrazione = id_amm;
                            qco.fineValidita = true;

                            logger.Debug("getCorrispondenteByMail end");
                            if (isInterno)
                            {
                                logger.Debug("richiamo il corrispondente interno");
                                corr = u.GetCorrispondenteInt(qco);
                                if (!corr.codiceRubrica.Contains("@") && (corr.codiceRubrica.Length < 8 || !corr.codiceRubrica.Substring(0, 8).ToUpper().Equals("INTEROP_")))
                                    return corr;
                            }
                            else
                            {
                                logger.Debug("richiamo il corrispondente esterno");
                                corr = u.GetCorrispondenteEst(qco);
                                if (!corr.codiceRubrica.Contains("@") && (corr.codiceRubrica.Length < 8 || !corr.codiceRubrica.Substring(0, 8).ToUpper().Equals("INTEROP_")))
                                    return corr;
                            }
                        }

                        return corr;
                    }

                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella ricerca di uno specifico corrispondente con mail: " + mail, e);
                return null;
            }
            return null;
        }

        public DocsPaVO.utente.Corrispondente getCorrispondenteByEmailCodiceAmmCodiceAoo(string id_amm, string mail, string id_reg, string codiceAmm, string codiceAoo)
        {
            logger.Debug("getCorrispondenteByMail start");

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_CORRISPONDENTI_BY_EMAIL");
            q.setParam("email", mail.Replace("'", "''"));
            q.setParam("idAmministrazione", id_amm);
            q.setParam("idRegistro", id_reg);

            string commandText = q.getSQL();
            logger.Debug(commandText);
            DataSet ds = new DataSet();

            try
            {
                if (this.ExecuteQuery(out ds, "corrispondenti", commandText))
                {
                    if (ds.Tables["corrispondenti"].Rows.Count >= 1)
                    {
                        Corrispondente corr = null;

                        //Se il risultato è maggiore di uno vado a dare la priorità a quello di RC con codiceAmm e CodiceAoo uguale a quello in input(quello della segnatura),
                        //se non ne trovo nessuno vado a vedere se ce ne è uno che non è di RC ma con CodiceAmm e CodiceAoo in Input
                        //Se anche in questo caso non trova nulla prende il primo che trova
                        if (ds.Tables["corrispondenti"].Rows.Count > 1)
                        {
                            DataRow[] drArray = new DataRow[ds.Tables["corrispondenti"].Rows.Count];
                            ds.Tables["corrispondenti"].Rows.CopyTo(drArray, 0);
                            DataRow dr1 = ((from row in drArray
                                            where !string.IsNullOrEmpty(row["CHA_TIPO_CORR"].ToString()) && row["CHA_TIPO_CORR"].ToString().Equals("C")
                                                 && row["VAR_CODICE_AMM"].ToString().ToUpper().Equals(codiceAmm.ToUpper())
                                                 && row["VAR_CODICE_AOO"].ToString().ToUpper().Equals(codiceAoo.ToUpper())
                                            select row)).FirstOrDefault();
                            if (dr1 == null)
                            {
                                dr1 = ((from row in drArray
                                        where row["VAR_CODICE_AMM"].ToString().ToUpper().Equals(codiceAmm.ToUpper())
                                                 && row["VAR_CODICE_AOO"].ToString().ToUpper().Equals(codiceAoo.ToUpper())
                                                 && (!row["VAR_COD_RUBRICA"].ToString().Contains("@") &&
                                                 (row["VAR_COD_RUBRICA"].ToString().Length < 8 || !row["VAR_COD_RUBRICA"].ToString().Substring(0, 8).ToUpper().Equals("INTEROP_")))
                                        select row)).FirstOrDefault();
                            }
                            if (dr1 != null)
                            {
                                string sid = (string)dr1["SYSTEM_ID"].ToString();
                                string tipo = (string)dr1["CHA_TIPO_URP"].ToString();
                                string id_gruppo = (string)dr1["ID_GRUPPO"].ToString();
                                string id_people = (string)dr1["ID_PEOPLE"].ToString();
                                bool isInterno = (dr1["CHA_TIPO_IE"].ToString() == "I");
                                DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                                qco.systemId = sid;
                                qco.idAmministrazione = id_amm;
                                qco.fineValidita = true;

                                logger.Debug("getCorrispondenteByMail end");
                                if (isInterno)
                                {
                                    logger.Debug("richiamo il corrispondente interno");
                                    corr = u.GetCorrispondenteInt(qco);
                                    if (!corr.codiceRubrica.Contains("@") && (corr.codiceRubrica.Length < 8 || !corr.codiceRubrica.Substring(0, 8).ToUpper().Equals("INTEROP_")))
                                        return corr;
                                }
                                else
                                {
                                    logger.Debug("richiamo il corrispondente esterno");
                                    corr = u.GetCorrispondenteEst(qco);
                                    if (!corr.codiceRubrica.Contains("@") && (corr.codiceRubrica.Length < 8 || !corr.codiceRubrica.Substring(0, 8).ToUpper().Equals("INTEROP_")))
                                        return corr;
                                }
                            }
                        }

                        foreach (DataRow dr in ds.Tables["corrispondenti"].Rows)
                        {
                            //DataRow dr = ds.Tables["corrispondenti"].Rows[0];
                            string sid = (string)dr["SYSTEM_ID"].ToString();
                            string tipo = (string)dr["CHA_TIPO_URP"].ToString();
                            string id_gruppo = (string)dr["ID_GRUPPO"].ToString();
                            string id_people = (string)dr["ID_PEOPLE"].ToString();
                            bool isInterno = (dr["CHA_TIPO_IE"].ToString() == "I");
                            DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                            qco.systemId = sid;
                            qco.idAmministrazione = id_amm;
                            qco.fineValidita = true;

                            logger.Debug("getCorrispondenteByMail end");
                            if (isInterno)
                            {
                                logger.Debug("richiamo il corrispondente interno");
                                corr = u.GetCorrispondenteInt(qco);
                                if (!corr.codiceRubrica.Contains("@") && (corr.codiceRubrica.Length < 8 || !corr.codiceRubrica.Substring(0, 8).ToUpper().Equals("INTEROP_")))
                                    return corr;
                            }
                            else
                            {
                                logger.Debug("richiamo il corrispondente esterno");
                                corr = u.GetCorrispondenteEst(qco);
                                if (!corr.codiceRubrica.Contains("@") && (corr.codiceRubrica.Length < 8 || !corr.codiceRubrica.Substring(0, 8).ToUpper().Equals("INTEROP_")))
                                    return corr;
                            }
                        }

                        return corr;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella ricerca di uno specifico corrispondente con mail: " + mail, e);
                return null;
            }
            return null;
        }

        public DocsPaVO.utente.Corrispondente GetCorrispondenteByEmail(string id_amm, string mail, string id_reg, out int rows)
        {
            logger.Debug("getCorrispondenteByMail start");
            rows = 0;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_CORRISPONDENTI_BY_EMAIL2");
            q.setParam("email", mail.Replace("'", "''"));
            q.setParam("idAmministrazione", id_amm);
            q.setParam("idRegistro", id_reg);

            string commandText = q.getSQL();
            logger.Debug(commandText);
            DataSet ds = new DataSet();

            try
            {
                if (this.ExecuteQuery(out ds, "corrispondenti", commandText))
                {
                    rows = ds.Tables["corrispondenti"].Rows.Count;
                    if (ds.Tables["corrispondenti"].Rows.Count >= 1)
                    {
                        //DataRow dr = ds.Tables["corrispondenti"].Rows[0];
                        Corrispondente corr = null;
                        foreach (DataRow dr in ds.Tables["corrispondenti"].Rows)
                        {
                            string sid = (string)dr["SYSTEM_ID"].ToString();
                            string tipo = (string)dr["CHA_TIPO_URP"].ToString();
                            string id_gruppo = (string)dr["ID_GRUPPO"].ToString();
                            string id_people = (string)dr["ID_PEOPLE"].ToString();
                            bool isInterno = (dr["CHA_TIPO_IE"].ToString() == "I");
                            DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                            qco.systemId = sid;
                            qco.idAmministrazione = id_amm;
                            qco.fineValidita = true;

                            logger.Debug("getCorrispondenteByMail end");
                            if (isInterno)
                            {
                                logger.Debug("richiamo il corrispondente interno");
                                corr = u.GetCorrispondenteInt(qco);
                                if (!corr.codiceRubrica.Contains("@") && (corr.codiceRubrica.Length < 8 || !corr.codiceRubrica.Substring(0, 8).ToUpper().Equals("INTEROP_")))
                                    return corr;
                            }
                            else
                            {
                                logger.Debug("richiamo il corrispondente esterno");
                                corr = u.GetCorrispondenteEst(qco);
                                if (!corr.codiceRubrica.Contains("@") && (corr.codiceRubrica.Length < 8 || !corr.codiceRubrica.Substring(0, 8).ToUpper().Equals("INTEROP_")))
                                    return corr;
                            }
                        }
                        return corr;
                    }


                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella ricerca di uno specifico corrispondente con mail: " + mail, e);
                return null;
            }
            return null;
        }



        public DocsPaVO.utente.Corrispondente GetCorrispondenteByEmailAndDescrizione(string id_amm, string mail, string id_reg, string descrizione)
        {
            logger.Debug("getCorrispondenteByMailAndDescrizione");

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali_byMail_Descrizone");
            q.setParam("mail", "'" + mail.ToUpper().Replace("'", "''") + "'");
            q.setParam("idamm", id_amm);
            q.setParam("id_reg", id_reg);
            q.setParam("descrizione", descrizione.ToUpper().Replace("'", "''"));
            q.setParam("descrizioneRC", string.Empty);
            string commandText = q.getSQL();
            logger.Debug(commandText);
            DataSet ds = new DataSet();

            try
            {
                if (this.ExecuteQuery(out ds, "corrispondenti", commandText))
                {
                    DocsPaVO.utente.Corrispondente corrispondente = new DocsPaVO.utente.Corrispondente();
                    if (ds.Tables["corrispondenti"].Rows.Count >= 1)
                    {
                        DataRow dr = ds.Tables["corrispondenti"].Rows[0];
                        string sid = (string)dr["SYSTEM_ID"].ToString();
                        string tipo = (string)dr["CHA_TIPO_URP"].ToString();
                        string id_gruppo = (string)dr["ID_GRUPPO"].ToString();
                        string id_people = (string)dr["ID_PEOPLE"].ToString();
                        bool isInterno = (dr["CHA_TIPO_IE"].ToString() == "I");
                        DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                        DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                        qco.systemId = sid;
                        qco.idAmministrazione = id_amm;
                        qco.fineValidita = true;

                        if (isInterno)
                            return u.GetCorrispondenteInt(qco);
                        else
                            return u.GetCorrispondenteEst(qco);

                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella ricerca di uno specifico corrispondente con mail: " + mail, e);
                return null;
            }
            return null;
        }


        public DocsPaVO.utente.Corrispondente GetCorrispondenteByEmailAndDescrizione(string id_amm, string mail, string id_reg, string descrizione, out string rows, string codiceAmm)
        {
            logger.Debug("getCorrispondenteByMailAndDescrizione");
            rows = "";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali_byMail_Descrizone");
            q.setParam("mail", "'" + mail.ToUpper().Replace("'", "''") + "'");
            q.setParam("idamm", id_amm);
            q.setParam("id_reg", id_reg);
            q.setParam("descrizione", descrizione.ToUpper().Replace("'", "''"));
            if (!string.IsNullOrEmpty(codiceAmm))
                q.setParam("descrizioneRC", " OR UPPER(VAR_DESC_CORR_OLD) = '" + codiceAmm.ToUpper().Replace("'", "''") + " - " + descrizione.ToUpper().Replace("'", "''") + "' ");
            else
                q.setParam("descrizioneRC", string.Empty);
            string commandText = q.getSQL();
            logger.Debug(commandText);
            DataSet ds = new DataSet();

            try
            {
                if (this.ExecuteQuery(out ds, "corrispondenti", commandText))
                {
                    rows = ds.Tables["corrispondenti"].Rows.Count.ToString();
                    DocsPaVO.utente.Corrispondente corrispondente = new DocsPaVO.utente.Corrispondente();
                    if (ds.Tables["corrispondenti"].Rows.Count >= 1)
                    {
                        DataRow[] drArray = new DataRow[ds.Tables["corrispondenti"].Rows.Count];
                        ds.Tables["corrispondenti"].Rows.CopyTo(drArray, 0);
                        Corrispondente corr = null;
                        foreach (DataRow dr2 in ds.Tables["corrispondenti"].Rows)
                        {
                            //se uno dei corrispondenti è di rubrica comune, restituisco o quest'ultimo
                            DataRow dr = ((from row in drArray
                                           where !string.IsNullOrEmpty(row["CHA_TIPO_CORR"].ToString()) && row["CHA_TIPO_CORR"].ToString().Equals("C")
                                           select row).Count() > 0) ? (from row in drArray
                                                                       where !string.IsNullOrEmpty(row["CHA_TIPO_CORR"].ToString()) && row["CHA_TIPO_CORR"].ToString().Equals("C")
                                                                       select row).First() : dr2;
                            string sid = (string)dr["SYSTEM_ID"].ToString();
                            string tipo = (string)dr["CHA_TIPO_URP"].ToString();
                            string id_gruppo = (string)dr["ID_GRUPPO"].ToString();
                            string id_people = (string)dr["ID_PEOPLE"].ToString();
                            bool isInterno = (dr["CHA_TIPO_IE"].ToString() == "I");
                            DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                            qco.systemId = sid;
                            qco.idAmministrazione = id_amm;
                            qco.fineValidita = true;

                            if (isInterno)
                            {
                                corr = u.GetCorrispondenteInt(qco);
                                if (corr.codiceRubrica.Length < 8 || !corr.codiceRubrica.Substring(0, 8).ToUpper().Equals("INTEROP_"))
                                    return corr;
                            }
                            else
                            {
                                corr = u.GetCorrispondenteEst(qco);
                                if (corr.codiceRubrica.Length < 8 || !corr.codiceRubrica.Substring(0, 8).ToUpper().Equals("INTEROP_"))
                                    return corr;
                            }
                        }
                        return corr;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella ricerca di uno specifico corrispondente con mail: " + mail, e);
                return null;
            }
            return null;
        }

        /// <summary>
        /// Questo metodo è analogo a GetCorrispondenteBySystemId ma estrare piu informazioni, ad esempio
        /// per i ruoli estrae anche le Uo a cui essi appatengono
        /// </summary>
        /// <param name="id_amm"></param>
        /// <param name="cod_rubrica"></param>
        /// <param name="tipoIE"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Corrispondente GetCorrispondenteCompletoBySystemId(string id_amm, string systemId, DocsPaVO.addressbook.TipoUtente tipoIE)
        {
            logger.Debug("getCorrispondente");

            string ie_where = "";
            switch (tipoIE)
            {
                case DocsPaVO.addressbook.TipoUtente.INTERNO:
                    ie_where = " and cha_tipo_ie='I'";
                    break;
                case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                    ie_where = " and cha_tipo_ie='E'";
                    break;
                case DocsPaVO.addressbook.TipoUtente.GLOBALE:
                    ie_where = "";
                    break;
            }

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
            q.setParam("param1", "SYSTEM_ID,CHA_TIPO_URP,ID_GRUPPO,ID_PEOPLE,CHA_TIPO_IE,DTA_FINE, VAR_DESC_CORR, VAR_COD_RUBRICA");
            q.setParam("param2", "SYSTEM_ID=" + systemId + " " + ie_where);
            logger.Debug(q.getSQL());
            DataSet ds = new DataSet();

            try
            {
                if (this.ExecuteQuery(out ds, "corrispondenti", q.getSQL()))
                {
                    DocsPaVO.utente.Corrispondente corrispondente = new DocsPaVO.utente.Corrispondente();
                    if (ds.Tables["corrispondenti"].Rows.Count >= 1)
                    {
                        DataRow dr = ds.Tables["corrispondenti"].Rows[0];
                        string sid = (string)dr["SYSTEM_ID"].ToString();
                        string tipo = (string)dr["CHA_TIPO_URP"].ToString();
                        string id_gruppo = (string)dr["ID_GRUPPO"].ToString();
                        string id_people = (string)dr["ID_PEOPLE"].ToString();
                        string dta_fine = (string)dr["DTA_FINE"].ToString();
                        string descrizione = (string)dr["VAR_DESC_CORR"].ToString();
                        string codRubrica = (string)dr["VAR_COD_RUBRICA"].ToString();
                        if (!string.IsNullOrEmpty(dta_fine))
                            corrispondente.dta_fine = dta_fine;


                        bool isInterno = (dr["CHA_TIPO_IE"].ToString() == "I");

                        DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                        DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                        qco.systemId = sid;
                        qco.idAmministrazione = id_amm;
                        qco.fineValidita = true;

                        if (isInterno)
                        {
                            if (!string.IsNullOrEmpty(dta_fine))
                                return u.GetCorrispondenteInt(qco, false);
                            else
                                return u.GetCorrispondenteInt(qco);
                        }
                        else
                        {
                            if (tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE)
                            {
                                corrispondente.systemId = sid;
                                corrispondente.codiceRubrica = codRubrica;
                                corrispondente.descrizione = descrizione;
                                corrispondente.tipoCorrispondente = tipo;
                                return corrispondente;
                            }
                            else
                                return u.GetCorrispondenteEst(qco);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella ricerca di uno specifico corrispondente (" + systemId + ")", e);
                return null;
            }
            return null;
        }

        /// <summary>
        /// Questo metodo è analogo a GetCorrispondenteBySystemId ma estrare piu informazioni, ad esempio
        /// per i ruoli estrae anche le Uo a cui essi appatengono
        /// </summary>
        /// <param name="id_amm"></param>
        /// <param name="cod_rubrica"></param>
        /// <param name="tipoIE"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Corrispondente GetCorrispondenteCompletoBySystemIdDisabled(string id_amm, string systemId, DocsPaVO.addressbook.TipoUtente tipoIE)
        {
            logger.Debug("GetCorrispondenteCompletoBySystemIdDisabled");

            string ie_where = "";
            switch (tipoIE)
            {
                case DocsPaVO.addressbook.TipoUtente.INTERNO:
                    ie_where = " and cha_tipo_ie='I'";
                    break;
                case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                    ie_where = " and cha_tipo_ie='E'";
                    break;
                case DocsPaVO.addressbook.TipoUtente.GLOBALE:
                    ie_where = "";
                    break;
            }

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
            q.setParam("param1", "SYSTEM_ID,CHA_TIPO_URP,ID_GRUPPO,ID_PEOPLE,CHA_TIPO_IE,DTA_FINE,VAR_COD_RUBRICA,VAR_DESC_CORR");
            q.setParam("param2", "SYSTEM_ID=" + systemId + " " + ie_where);
            logger.Debug(q.getSQL());
            DataSet ds = new DataSet();

            try
            {
                if (this.ExecuteQuery(out ds, "corrispondenti", q.getSQL()))
                {
                    DocsPaVO.utente.Corrispondente corrispondente = new DocsPaVO.utente.Corrispondente();
                    if (ds.Tables["corrispondenti"].Rows.Count >= 1)
                    {
                        DataRow dr = ds.Tables["corrispondenti"].Rows[0];
                        string sid = (string)dr["SYSTEM_ID"].ToString();
                        string tipo = (string)dr["CHA_TIPO_URP"].ToString();
                        string id_gruppo = (string)dr["ID_GRUPPO"].ToString();
                        string id_people = (string)dr["ID_PEOPLE"].ToString();
                        string dta_fine = (string)dr["DTA_FINE"].ToString();
                        string var_codice = "";
                        string var_desc = "";
                        if (!string.IsNullOrEmpty(dta_fine))
                            corrispondente.dta_fine = dta_fine;

                        if (dr["VAR_COD_RUBRICA"] != null && !string.IsNullOrEmpty((string)dr["VAR_COD_RUBRICA"].ToString()))
                        {
                            var_codice = (string)dr["VAR_COD_RUBRICA"].ToString();
                            corrispondente.codiceRubrica = var_codice;
                        }

                        if (dr["VAR_DESC_CORR"] != null && !string.IsNullOrEmpty((string)dr["VAR_DESC_CORR"].ToString()))
                        {
                            var_desc = (string)dr["VAR_DESC_CORR"].ToString();
                            corrispondente.descrizione = var_desc;
                        }

                        bool isInterno = (dr["CHA_TIPO_IE"].ToString() == "I");

                        DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                        DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                        qco.systemId = sid;
                        qco.idAmministrazione = id_amm;
                        qco.fineValidita = true;

                        if (isInterno)
                            return u.GetCorrispondenteInt(qco, false);
                        else
                        {
                            if (tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE)
                                return corrispondente;
                            else
                                return u.GetCorrispondenteEst(qco);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella ricerca di uno specifico corrispondente (" + systemId + ")", e);
                return null;
            }
            return null;
        }

        public DocsPaVO.utente.Corrispondente GetCorrispondenteBySystemID(string systemID)
        {
            logger.Debug("INIT - GetCorrispondenteBySystemID");

            DocsPaVO.utente.Corrispondente corrispondente = null;

            try
            {
                if (!string.IsNullOrEmpty(systemID))
                {
                    corrispondente = new DocsPaVO.utente.Corrispondente();

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");

                    q.setParam("param1", "id_amm, cha_tipo_ie");

                    if (Char.IsNumber(systemID, 0) || systemID.StartsWith("(")) //per checkScadenze
                        q.setParam("param2", " SYSTEM_ID=" + systemID);
                    else
                        q.setParam("param2", " UPPER(VAR_DESC_CORR)=UPPER('" + systemID + "')");

                    string commandText = q.getSQL();
                    logger.Debug(commandText);

                    bool existCorrispondente = false;

                    string idAmministrazione = string.Empty;
                    DocsPaVO.addressbook.TipoUtente tipoUtente = DocsPaVO.addressbook.TipoUtente.GLOBALE;

                    using (DBProvider dbProvider = new DBProvider())
                    {
                        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                        {
                            if (reader.Read())
                            {
                                // Corrispondente trovato
                                existCorrispondente = true;

                                if (!reader.IsDBNull(reader.GetOrdinal("id_amm")))
                                    idAmministrazione = reader.GetValue(reader.GetOrdinal("id_amm")).ToString();

                                if (!reader.IsDBNull(reader.GetOrdinal("cha_tipo_ie")))
                                {
                                    string tipoIE = reader.GetValue(reader.GetOrdinal("cha_tipo_ie")).ToString();

                                    if (!string.IsNullOrEmpty(tipoIE))
                                    {
                                        if (tipoIE.Equals("I"))
                                            tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                                        else if (tipoIE.Equals("E"))
                                            tipoUtente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                                    }
                                }
                            }
                        }
                    }

                    if (existCorrispondente)
                        // Reperimento dei metadati del corrisponente
                        corrispondente = GetCorrispondenteCompletoBySystemId(idAmministrazione, systemID, tipoUtente);
                    else
                        logger.Debug(string.Format("Corrispondente con ID '{0}' non trovato", systemID));

                    if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FE_ENABLE_FLUSSO_AUTOMATICO")) &&
                        DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FE_ENABLE_FLUSSO_AUTOMATICO").ToString().Equals("1"))
                    {
                        DocsPaDB.Query_DocsPAWS.FlussoAutomatico flusso = new FlussoAutomatico();
                        corrispondente.interoperanteRGS = flusso.CheckIsInteroperanteRGS(systemID);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Si è verificato un errore nella ricerca del corrispondente con ID '{0}'", systemID), ex);
            }
            finally
            {
                logger.Debug("END - GetCorrispondenteBySystemID");
            }

            return corrispondente;
        }

        public DocsPaVO.utente.Corrispondente GetCorrispondenteBySystemIDDisabled(string systemID)
        {
            logger.Debug("INIT - GetCorrispondenteBySystemID");

            DocsPaVO.utente.Corrispondente corrispondente = null;

            try
            {
                if (!string.IsNullOrEmpty(systemID))
                {
                    corrispondente = new DocsPaVO.utente.Corrispondente();

                    // Gabriele Melini 05-06-2014
                    // fix per stringhe contenenti apostrofi
                    if (systemID.Contains("\'"))
                    {
                        systemID = systemID.Replace("\'", "\'\'");
                    }

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");

                    q.setParam("param1", "id_amm, cha_tipo_ie");

                    if (Char.IsNumber(systemID, 0) || systemID.StartsWith("(")) //per checkScadenze
                        q.setParam("param2", " SYSTEM_ID=" + systemID);
                    else
                        q.setParam("param2", " UPPER(VAR_DESC_CORR)=UPPER('" + systemID + "')");

                    string commandText = q.getSQL();
                    logger.Debug(commandText);

                    bool existCorrispondente = false;

                    string idAmministrazione = string.Empty;
                    DocsPaVO.addressbook.TipoUtente tipoUtente = DocsPaVO.addressbook.TipoUtente.GLOBALE;

                    using (DBProvider dbProvider = new DBProvider())
                    {
                        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                        {
                            if (reader.Read())
                            {
                                // Corrispondente trovato
                                existCorrispondente = true;

                                if (!reader.IsDBNull(reader.GetOrdinal("id_amm")))
                                    idAmministrazione = reader.GetValue(reader.GetOrdinal("id_amm")).ToString();

                                if (!reader.IsDBNull(reader.GetOrdinal("cha_tipo_ie")))
                                {
                                    string tipoIE = reader.GetValue(reader.GetOrdinal("cha_tipo_ie")).ToString();

                                    if (!string.IsNullOrEmpty(tipoIE))
                                    {
                                        if (tipoIE.Equals("I"))
                                            tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                                        else if (tipoIE.Equals("E"))
                                            tipoUtente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                                    }
                                }
                            }
                        }
                    }

                    if (existCorrispondente)
                        // Reperimento dei metadati del corrisponente
                        corrispondente = GetCorrispondenteCompletoBySystemIdDisabled(idAmministrazione, systemID, tipoUtente);
                    else
                    {
                        logger.Debug(string.Format("Corrispondente con ID '{0}' non trovato", systemID));
                        // effettuo la modifica minima per un incident. Setto null il corrispondente quando non trovato.
                        corrispondente = null;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Si è verificato un errore nella ricerca del corrispondente con ID '{0}'", systemID), ex);
            }
            finally
            {
                logger.Debug("END - GetCorrispondenteBySystemID");
            }

            return corrispondente;
        }

        public string GetSystemIdCorrispondenteByIDPeople(string idPeople)
        {
            string systemId = "";
            logger.Debug("GetSystemIdCorrispondenteByIDPeople");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobaliByIdPeople");
            q.setParam("param1", idPeople);
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteScalar(out systemId, sql);
            return systemId;
        }


        public string GetIdAmm(string idPeople)
        {
            string idAmm = "";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_People_all");
            q.setParam("param1", "ID_AMM");
            q.setParam("param2", "SYSTEM_ID = " + idPeople);
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteScalar(out idAmm, sql);
            return idAmm;
        }

        public void GetIdAmmUtente(out string idAmm, string userName)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Utenti > GetIdAmmUtente");
            /*string sql =  "SELECT ID_AMM FROM PEOPLE WHERE USER_ID = '" + userName +"'";*/
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_People");
            q.setParam("param1", "ID_AMM");
            q.setParam("param2", "UPPER(USER_ID) = UPPER('" + userName + "')");
            string sql = q.getSQL();
            logger.Debug(sql);
            idAmm = null;
            try
            {
                this.ExecuteScalar(out idAmm, sql);
            }
            catch (Exception)
            { }
        }

        //aggiunto sabrina
        public void GetIdAmmUtente(out ArrayList List_idAmm, string userName)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Utenti > GetIdAmmUtente");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_People");
            q.setParam("param1", "ID_AMM");
            q.setParam("param2", "UPPER(USER_ID) = UPPER('" + userName + "')");
            string sql = q.getSQL();
            logger.Debug(sql);
            List_idAmm = null;
            DataSet ds = new DataSet();
            try
            {
                this.OpenConnection(); //alcune volte la connessione è chiusa inspegabilmente..
                //if (this.ExecuteQuery(out ds, "DPA_AMMINISTRA", sql))
                //{
                this.ExecuteQuery(out ds, "DPA_AMMINISTRA", sql);
                if (ds.Tables["DPA_AMMINISTRA"].Rows.Count > 0)
                {
                    List_idAmm = new ArrayList();
                    foreach (DataRow AmminRow in ds.Tables["DPA_AMMINISTRA"].Rows)
                    {
                        if (AmminRow["ID_AMM"] != DBNull.Value)
                            List_idAmm.Add(AmminRow["ID_AMM"].ToString());
                    }
                }
                ds.Dispose();
                //}
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nella query " + sql + " GetIdAmmUtente :" + ex);
                throw ex;
            }
        }

        public String[] GetMementoUtente(string idPeople, string IdAmministrazione)
        {
            //string sql ="SELECT SYSTEM_ID, USER_ID, ID_AMM, VAR_COGNOME, VAR_NOME, VAR_TELEFONO, EMAIL_ADDRESS, CHA_NOTIFICA, CHA_AMMINISTRATORE,CHA_NOTIFICA_CON_ALLEGATO  FROM PEOPLE WHERE SYSTEM_ID="+idPeople;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_People");
            q.setParam("param1", "SYSTEM_ID, VAR_MEMENTO");
            q.setParam("param2", "SYSTEM_ID=" + idPeople + " AND ID_AMM = " + IdAmministrazione);
            string sql = q.getSQL();
            logger.Debug(sql);
            DataSet dataSet;
            this.ExecuteQuery(out dataSet, "PEOPLE", sql);
            String retval = "";
            if (dataSet.Tables["PEOPLE"].Rows.Count > 0)
            {
                try
                {
                    DataRow utenteRow = dataSet.Tables["PEOPLE"].Rows[0];
                    retval = utenteRow["VAR_MEMENTO"].ToString();
                }
                catch (Exception e)
                {
                    logger.DebugFormat("Errore Recuperando il memento {0} {1}", e.Message, e.StackTrace);
                }
            }
            dataSet.Dispose();

            if (retval.Contains("§"))
            {
                return retval.Split('§');
            }
            else
            {
                return new String[] { retval };
            }

        }

        public bool SetMementoUtente(string idPeople, string IdAmministrazione, string dominio, string alias)
        {
            bool result = false;
            //tolgo i separatori
            dominio = dominio.Replace("§", "");
            alias = alias.Replace("§", "");

            string memento = String.Format("{0}§{1}", dominio, alias);

            DataSet dataSet = new DataSet();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_People");
                q.setParam("param1", "VAR_MEMENTO = '" + memento + "'");
                q.setParam("param2", "SYSTEM_ID=" + idPeople + " AND ID_AMM = " + IdAmministrazione);
                string sql = q.getSQL();

                result = this.ExecuteNonQuery(sql);
            }
            catch (Exception e)
            {
                logger.DebugFormat("Errore Impostanto il memento {0} {1}", e.Message, e.StackTrace);
            }
            return result;
        }

        public DocsPaVO.utente.Utente GetUtente(string idPeople)
        {
            //string sql ="SELECT SYSTEM_ID, USER_ID, ID_AMM, VAR_COGNOME, VAR_NOME, VAR_TELEFONO, EMAIL_ADDRESS, CHA_NOTIFICA, CHA_AMMINISTRATORE,CHA_NOTIFICA_CON_ALLEGATO  FROM PEOPLE WHERE SYSTEM_ID="+idPeople;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_People");
            q.setParam("param1", "SYSTEM_ID, USER_ID, ID_AMM, VAR_COGNOME, VAR_NOME, VAR_TELEFONO, EMAIL_ADDRESS, FROM_EMAIL_ADDRESS, CHA_NOTIFICA, CHA_AMMINISTRATORE,CHA_NOTIFICA_CON_ALLEGATO,VAR_SEDE,MATRICOLA");
            q.setParam("param2", "SYSTEM_ID=" + idPeople);
            string sql = q.getSQL();
            return GetUserData(sql);
        }

        public DocsPaVO.utente.Utente GetUtenteByEmail(string idAmm, string email)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_People");
            q.setParam("param1", "SYSTEM_ID, USER_ID, ID_AMM, VAR_COGNOME, VAR_NOME, VAR_TELEFONO, EMAIL_ADDRESS, CHA_NOTIFICA, CHA_AMMINISTRATORE,CHA_NOTIFICA_CON_ALLEGATO,VAR_SEDE,MATRICOLA");
            q.setParam("param2", "ID_AMM=" + idAmm + " AND EMAIL_ADDRESS='" + email + "'");
            string sql = q.getSQL();
            DocsPaVO.utente.Utente ut = GetUserData(sql);
            if (ut != null && ut.idPeople != null)
            {
                DocsPaUtils.Query q_1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                q_1.setParam("param1", " SYSTEM_ID, CHA_TIPO_IE");
                q_1.setParam("param2", " ID_PEOPLE = " + ut.idPeople);
                logger.Debug(q_1.getSQL());
                DataSet dataSet;
                this.ExecuteQuery(out dataSet, "IDCORRGLOBALI", q_1.getSQL());

                if (dataSet.Tables["IDCORRGLOBALI"].Rows.Count > 0)
                {
                    DataRow utenteRow = dataSet.Tables["IDCORRGLOBALI"].Rows[0];
                    ut.systemId = dataSet.Tables["IDCORRGLOBALI"].Rows[0]["SYSTEM_ID"].ToString(); ;
                    ut.tipoCorrispondente = dataSet.Tables["IDCORRGLOBALI"].Rows[0]["CHA_TIPO_IE"].ToString(); ;
                }
                dataSet.Dispose();
            }

            return ut;
        }

        public DocsPaVO.utente.Utente GetUtenteByEmailNoIdAmm(string email)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_People");
            q.setParam("param1", "SYSTEM_ID, USER_ID, ID_AMM, VAR_COGNOME, VAR_NOME, VAR_TELEFONO, EMAIL_ADDRESS, CHA_NOTIFICA, CHA_AMMINISTRATORE,CHA_NOTIFICA_CON_ALLEGATO,VAR_SEDE,MATRICOLA");
            q.setParam("param2", "UPPER(EMAIL_ADDRESS)='" + email + "'");
            string sql = q.getSQL();
            DocsPaVO.utente.Utente ut = GetUserData(sql);
            if (ut != null && ut.idPeople != null)
            {
                DocsPaUtils.Query q_1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                q_1.setParam("param1", " SYSTEM_ID, CHA_TIPO_IE");
                q_1.setParam("param2", " ID_PEOPLE = " + ut.idPeople);
                logger.Debug(q_1.getSQL());
                DataSet dataSet;
                this.ExecuteQuery(out dataSet, "IDCORRGLOBALI", q_1.getSQL());

                if (dataSet.Tables["IDCORRGLOBALI"].Rows.Count > 0)
                {
                    DataRow utenteRow = dataSet.Tables["IDCORRGLOBALI"].Rows[0];
                    ut.systemId = dataSet.Tables["IDCORRGLOBALI"].Rows[0]["SYSTEM_ID"].ToString(); ;
                    ut.tipoCorrispondente = dataSet.Tables["IDCORRGLOBALI"].Rows[0]["CHA_TIPO_IE"].ToString(); ;
                }
                dataSet.Dispose();
            }

            return ut;
        }

        /// <summary>
        /// Restituisce un oggetto Utente senza condizioni di filtro 
        /// relative all'abilitazione o meno
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Utente GetUtenteNoFiltroDisabled(string idPeople)
        {
            //string sql ="SELECT SYSTEM_ID, USER_ID, ID_AMM, VAR_COGNOME, VAR_NOME, VAR_TELEFONO, EMAIL_ADDRESS, CHA_NOTIFICA, CHA_AMMINISTRATORE,CHA_NOTIFICA_CON_ALLEGATO  FROM PEOPLE WHERE SYSTEM_ID="+idPeople;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_People_all");
            q.setParam("param1", "SYSTEM_ID, USER_ID, ID_AMM, VAR_COGNOME, VAR_NOME, VAR_TELEFONO, EMAIL_ADDRESS, CHA_NOTIFICA, CHA_AMMINISTRATORE,CHA_NOTIFICA_CON_ALLEGATO,VAR_SEDE,MATRICOLA");
            q.setParam("param2", "SYSTEM_ID=" + idPeople);
            string sql = q.getSQL();
            return GetUserData(sql);
        }

        public DocsPaVO.utente.Utente GetUtenteByMatricola(string matricola, string idAmministrazione, string modulo)
        {
            string queryName = string.Empty;

            if (DocsPaUtils.Moduli.ModuliAuthManager.IsModuloCentroServizi(modulo))
                queryName = "S_PEOPLE_CENTRO_SERVIZI";
            else
                queryName = "S_People";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);
            q.setParam("param1", "SYSTEM_ID, USER_ID, ID_AMM, VAR_COGNOME, VAR_NOME, VAR_TELEFONO, EMAIL_ADDRESS, CHA_NOTIFICA, CHA_AMMINISTRATORE,CHA_NOTIFICA_CON_ALLEGATO,VAR_SEDE,MATRICOLA");
            string param = "UPPER(MATRICOLA)='" + matricola.ToUpper() + "'";

            if (!string.IsNullOrEmpty(idAmministrazione))
                param += " AND ID_AMM = '" + idAmministrazione + "'";
            //else
            //    param += " AND (ID_AMM = '' OR ID_AMM IS NULL)";

            q.setParam("param2", param);

            string sql = q.getSQL();
            return GetUserData(sql);
        }

        public DocsPaVO.utente.Utente GetUtente(string userName, string idAmministrazione, string modulo)
        {
            string queryName = string.Empty;

            if (DocsPaUtils.Moduli.ModuliAuthManager.IsModuloCentroServizi(modulo))
                queryName = "S_PEOPLE_CENTRO_SERVIZI_GLOBALI";
            else
                queryName = "S_People_GLOBALI";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);
            q.setParam("param1", "P.SYSTEM_ID, P.USER_ID, P.ID_AMM, P.VAR_COGNOME, P.VAR_NOME, P.VAR_TELEFONO, P.EMAIL_ADDRESS, P.CHA_NOTIFICA, P.CHA_AMMINISTRATORE,P.CHA_NOTIFICA_CON_ALLEGATO,P.VAR_SEDE,MATRICOLA, A.SYSTEM_ID as IDCORRGLOBALI");
            string param = "UPPER(P.USER_ID)='" + userName.ToUpper() + "'";

            if (!string.IsNullOrEmpty(idAmministrazione) && !idAmministrazione.Equals("0"))
                param += " AND P.ID_AMM = '" + idAmministrazione + "'";
            //else
            //    param += " AND (ID_AMM = '' OR ID_AMM IS NULL)";

            q.setParam("param2", param);

            string sql = q.getSQL();
            return GetUserData(sql);
        }

        public DocsPaVO.utente.Utente GetUtenteByMatricola(string matricola, string idAmministrazione)
        {
            return this.GetUtenteByMatricola(matricola, idAmministrazione, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Utente GetUtente(string userName, string idAmministrazione)
        {
            return GetUtente(userName, idAmministrazione, string.Empty);
        }

        private DocsPaVO.utente.Utente GetUserData(string sql)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Utenti > GetUserData");

            DocsPaVO.utente.Utente utente = null;
            //ricerca delle caratteristiche utente nella tabella people
            logger.Debug(sql);
            DataSet dataSet;
            this.ExecuteQuery(out dataSet, "PEOPLE", sql);

            if (dataSet.Tables["PEOPLE"].Rows.Count > 0)
            {
                DataRow utenteRow = dataSet.Tables["PEOPLE"].Rows[0];

                utente = new DocsPaVO.utente.Utente();
                utente.idPeople = utenteRow["SYSTEM_ID"].ToString();
                utente.userId = utenteRow["USER_ID"].ToString();
                utente.descrizione = utenteRow["VAR_COGNOME"].ToString() + " " + utenteRow["VAR_NOME"].ToString();
                utente.telefono = utenteRow["VAR_TELEFONO"].ToString();
                utente.email = utenteRow["EMAIL_ADDRESS"].ToString();
                utente.notifica = utenteRow["CHA_NOTIFICA"].ToString();
                utente.amministratore = FromCharToBool(utenteRow["CHA_AMMINISTRATORE"].ToString());
                utente.assegnante = FromCharToBool(utenteRow["CHA_AMMINISTRATORE"].ToString());
                utente.assegnatario = FromCharToBool(utenteRow["CHA_AMMINISTRATORE"].ToString());
                utente.idAmministrazione = utenteRow["ID_AMM"].ToString();
                utente.notificaConAllegato = FromCharToBool(utenteRow["CHA_NOTIFICA_CON_ALLEGATO"].ToString());
                utente.sede = utenteRow["VAR_SEDE"].ToString();
                utente.matricola = (utenteRow["MATRICOLA"] != DBNull.Value ? utenteRow["MATRICOLA"].ToString() : null);
                utente.tipoCorrispondente = "P";
                //Abbatangeli Gianlugigi - carico le applicazioni relative all'utente
                //06/03/2017: commentata la riga sotto
                //utente.extApplications = this.getApplicazioniUtente(utente.idPeople);
                utente.cognome = utenteRow["VAR_COGNOME"].ToString();
                utente.nome = utenteRow["VAR_NOME"].ToString();
                if (utenteRow.Table.Columns.Contains("IDCORRGLOBALI"))
                {
                    utente.systemId = utenteRow["IDCORRGLOBALI"].ToString();
                }
            }

            dataSet.Dispose();
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Utenti > GetUserData");

            return utente;
        }

        /// <summary>
        /// Return Ruolo bY idCorrGlobali
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <param name="loadFunzioni">
        /// Indica se caricare o meno i dati delle funzioni associate al ruolo
        /// </param>
        /// <returns></returns>
        public DocsPaVO.utente.Ruolo GetRuolo(string idRuolo, bool loadFunzioni)
        {
            logger.Debug("getRuolo");
            DocsPaVO.utente.Ruolo objRuolo = null;
            DataSet dataSet = new DataSet();

            string sql = "";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLEGROUPS__TIPO_RUOLO__CORR_GLOBALI");
            //q.setParam("param1", "B.SYSTEM_ID = " + idRuolo);
            q.setParam("param1", String.Format("B.SYSTEM_ID in (select original_id from dpa_corr_globali where system_id = {0})", idRuolo));
            sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteQuery(dataSet, "RUOLI", sql);
            if (dataSet.Tables["RUOLI"].Rows.Count > 0)
            {
                // cerco il valore di livello più alto per le UO associate all'utente in modo
                // da limitare il dimensione della tabella delle UO.
                //string maxLivello = dataSet.Tables["RUOLI"].Select("","NUM_LIVELLO_UO desc")[0]["NUM_LIVELLO_UO"].ToString();
                string maxLivello = dataSet.Tables["RUOLI"].Select("", "NUM_LIVELLO desc")[0]["NUM_LIVELLO"].ToString();
                logger.Debug("GetRuolo: maxlivello=" + maxLivello);

                //Inizio modifica luluciani
                //NEW  
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "USA_CONNECTBYPRIOR_OR_WITH")) && (DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "USA_CONNECTBYPRIOR_OR_WITH")).Equals("1"))
                {
                    DataRow ruoloRow = dataSet.Tables["RUOLI"].Rows[0];
                    this.GetRuoliUtente(dataSet, ruoloRow, maxLivello, null, false);
                }
                //OLD  
                else
                {
                    GetRuoloMaxLivello(dataSet, maxLivello, false);
                }

                objRuolo = GetRuoloData(dataSet, dataSet.Tables["RUOLI"].Rows[0], loadFunzioni);
            }

            dataSet.Dispose();

            return objRuolo;
        }

        /// <summary>
        /// Return Ruolo bY idCorrGlobali
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Ruolo GetRuolo(string idRuolo)
        {
            return this.GetRuolo(idRuolo, true);
        }


        /// <summary>
        /// Return Ruolo bY idGruppo
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Ruolo GetRuoloByIdGruppo(string idGruppo)
        {
            logger.Debug("getRuolo");
            DocsPaVO.utente.Ruolo objRuolo = null;
            DataSet dataSet = new DataSet();
            string sql = "";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLEGROUPS__TIPO_RUOLO__CORR_GLOBALI");
            q.setParam("param1", "B.ID_GRUPPO = " + idGruppo);
            sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteQuery(dataSet, "RUOLI", sql);
            if (dataSet.Tables["RUOLI"].Rows.Count > 0)
            {
                // cerco il valore di livello più alto per le UO associate all'utente in modo
                // da limitare il dimensione della tabella delle UO.
                //string maxLivello = dataSet.Tables["RUOLI"].Select("","NUM_LIVELLO_UO desc")[0]["NUM_LIVELLO_UO"].ToString();
                string maxLivello = dataSet.Tables["RUOLI"].Select("", "NUM_LIVELLO desc")[0]["NUM_LIVELLO"].ToString();
                logger.Debug("GetRuolo: maxlivello=" + maxLivello);

                //Inizio modifica luluciani
                //NEW  
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "USA_CONNECTBYPRIOR_OR_WITH")) && (DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "USA_CONNECTBYPRIOR_OR_WITH")).Equals("1"))
                {
                    DataRow ruoloRow = dataSet.Tables["RUOLI"].Rows[0];
                    this.GetRuoliUtente(dataSet, ruoloRow, maxLivello, null, true);
                }
                //OLD  
                else
                {
                    GetRuoloMaxLivello(dataSet, maxLivello, true);
                }

                objRuolo = GetRuoloData(dataSet, dataSet.Tables["RUOLI"].Rows[0]);
            }

            dataSet.Dispose();

            return objRuolo;
        }


        /// <summary>
        /// Questo metodo ricerca il ruolo e lo ritorna anche se è disabilitato
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Ruolo GetRuoloEnabledAndDisabled(string idRuolo)
        {
            logger.Debug("getRuolo in trasmissione");
            DocsPaVO.utente.Ruolo objRuolo = null;
            DataSet dataSet = new DataSet();

            string sql = "";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLEGROUPS__TIPO_RUOLO__CORR_GLOBALI_E_D");
            q.setParam("param1", String.Format("B.SYSTEM_ID = {0}", idRuolo));
            sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteQuery(dataSet, "RUOLI", sql);
            if (dataSet.Tables["RUOLI"].Rows.Count > 0)
            {
                // cerco il valore di livello più alto per le UO associate all'utente in modo
                // da limitare il dimensione della tabella delle UO.
                string maxLivello = dataSet.Tables["RUOLI"].Select("", "NUM_LIVELLO desc")[0]["NUM_LIVELLO"].ToString();
                logger.Debug("GetRuolo: maxlivello=" + maxLivello);

                //Inizio modifica luluciani
                //NEW  
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "USA_CONNECTBYPRIOR_OR_WITH")) && (DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "USA_CONNECTBYPRIOR_OR_WITH")).Equals("1"))
                {
                    DataRow ruoloRow = dataSet.Tables["RUOLI"].Rows[0];
                    this.GetRuoliUtente(dataSet, ruoloRow, maxLivello, null, true);
                }
                //OLD  
                else
                {
                    GetRuoloMaxLivello(dataSet, maxLivello, true);
                }

                objRuolo = GetRuoloData(dataSet, dataSet.Tables["RUOLI"].Rows[0]);
            }

            dataSet.Dispose();

            return objRuolo;
        }

        /// <summary>
        /// Questo medodo prende il ruolo che ha il max livello,
        /// ma a seconda del valore del parametro booleano 'ruoliAbilitati'
        /// prendo e meno anche i ruoli che nel db risultano essere disabilitati
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="maxLivello"></param>
        /// <param name="getRuoliAbilitati">true=prende anche i ruoli sisabilitati, false = 
        /// prende solo i ruoli con dta_fine = null</param>
        public void GetRuoloMaxLivello(DataSet dataSet, string maxLivello, bool getRuoliAbilitati)
        {
            //DataSet dataSet = new DataSet();
            /*string sql =
                "SELECT * FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='U' " +
                "AND CHA_TIPO_IE='I' AND DTA_FINE IS NULL AND NUM_LIVELLO <=" + 
                maxLivello;*/
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
            q.setParam("param1", "*");
            if (!getRuoliAbilitati)
            {
                q.setParam("param2", "A.CHA_TIPO_URP='U' " +
                    "AND A.CHA_TIPO_IE='I' AND A.DTA_FINE IS NULL AND A.NUM_LIVELLO <=" +
                    maxLivello);
            }
            else
            {
                q.setParam("param2", "A.CHA_TIPO_URP='U' " +
                    "AND A.CHA_TIPO_IE='I' AND A.NUM_LIVELLO <=" +
                    maxLivello);
            }
            string sql = q.getSQL();
            logger.Debug(sql);
            //db.fillTable(queryString,dataSet,"UO");
            this.ExecuteQuery(dataSet, "UO", sql);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="ruoloRow"></param>
        /// <param name="loadFunzioni">
        /// Indica se caricare o meno i dati delle funzioni legate al ruolo
        /// </param>
        /// <returns></returns>
        public DocsPaVO.utente.Ruolo GetRuoloData(DataSet dataSet, DataRow ruoloRow, bool loadFunzioni)
        {
            //memorizzo l'id del ruolo per scriverla una sola volta
            string id = ruoloRow["SYSTEM_ID"].ToString();
            string idUO = ruoloRow["ID_UO"].ToString();
            DataRow[] dataRow = dataSet.Tables["UO"].Select("SYSTEM_ID='" + idUO + "'", "SYSTEM_ID");
            DataRow row = null;
            if (dataRow.Length > 0)
                row = dataRow[0];
            //DocsPaVO.utente.UnitaOrganizzativa uo = this.GetUnitaOrganizzativa(dataSet, dataSet.Tables["UO"].Select("SYSTEM_ID='"+ruoloRow["ID_UO"].ToString()+"'","SYSTEM_ID")[0]);
            DocsPaVO.utente.UnitaOrganizzativa uo = null;
            if (row != null)
                uo = this.GetUnitaOrganizzativa(dataSet, row);
            DocsPaVO.utente.TipoRuolo tipoRuo = new DocsPaVO.utente.TipoRuolo();
            if (ruoloRow.Table.Columns.Contains("VAR_CODICE"))
            {
                if (ruoloRow["VAR_CODICE"] != DBNull.Value)
                    tipoRuo.codice = ruoloRow["VAR_CODICE"].ToString();
            }
            tipoRuo.descrizione = ruoloRow["VAR_DESC_RUOLO"].ToString();

            ArrayList funzioni = null;
            if (loadFunzioni)
                funzioni = this.GetFunzioni(id);

            DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo(
                ruoloRow["SYSTEM_ID"].ToString(),
                //ruoloRow["VAR_DESC_RUOLO"].ToString()+" "+uo.descrizione,
                ruoloRow["VAR_DESC_CORR"].ToString(),
                ruoloRow["VAR_CODICE"].ToString(),
                ruoloRow["NUM_LIVELLO"].ToString(),
                ruoloRow["ID_GRUPPO"].ToString(),
                tipoRuo,
                funzioni);

            ruolo.uo = uo;
            ruolo.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();
            ruolo.idRegistro = ruoloRow["ID_REGISTRO"].ToString();
            ruolo.registri = GetRegistriRuolo(id);
            ruolo.idAmministrazione = ruoloRow["ID_AMM"].ToString();
            ruolo.tipoCorrispondente = "R";

            if (ruoloRow.Table.Columns.Contains("CHA_RESPONSABILE"))
            {
                if (ruoloRow["CHA_RESPONSABILE"] != DBNull.Value)
                    ruolo.Responsabile = (ruoloRow["CHA_RESPONSABILE"].ToString() == "1");
            }

            if (ruoloRow.Table.Columns.Contains("CHA_SEGRETARIO"))
            {
                if (ruoloRow["CHA_SEGRETARIO"] != DBNull.Value)
                    ruolo.Segretario = (ruoloRow["CHA_SEGRETARIO"].ToString() == "1");
            }

            if (ruoloRow.Table.Columns.Contains("CHA_PREFERITO"))
            {
                if (ruoloRow["CHA_PREFERITO"] != DBNull.Value)
                    ruolo.selezionato = (ruoloRow["CHA_PREFERITO"].ToString() == "1");
            }

            return ruolo;
        }

        public DocsPaVO.utente.Ruolo GetRuoloData(DataSet dataSet, DataRow ruoloRow)
        {
            return this.GetRuoloData(dataSet, ruoloRow, true);
        }

        //************************************************************************************
        // IACOZZILLI GIORDANO 03122013
        // Per il nuovo PIS GetRolesForEnabledActions devo aggiungere ul getruoli.
        //************************************************************************************
        public void GetRuoliUOUtenteForEnabledActions(DataSet dataSet, string idPeople, string codicetipofunzione, string IdAmm)
        {
            logger.Debug("start > GetRuoliUOUtenteForEnabledActions");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLEGROUPS__TIPO_RUOLO_COD_FUNC");
            q.setParam("param1", "upper(d.VAR_COD_TIPO) = upper('" + codicetipofunzione + "') AND A.PEOPLE_SYSTEM_ID=" + idPeople);
            q.setParam("param2", "d.ID_AMM = " + IdAmm);
            q.setParam("param3", "A.CHA_PREFERITO DESC");
            string sql = q.getSQL();
            logger.Debug(sql);
            //db.fillTable(commandString3,dataSet,"RUOLI");
            this.ExecuteQuery(dataSet, "RUOLIXFUNC", sql);
        }
        //*************************************************************************************
        //*************************************************************************************

        //************************************************************************************
        // ABBATANGELI GIANLUIGI 18112015
        // Per il nuovo PIS relativo ai big file.
        //************************************************************************************
        public void GetRuoliUOUtenteForEnabledSingleFunction(DataSet dataSet, string idPeople, string codicetipoMicrofunzione, string IdAmm)
        {
            logger.Debug("start > GetRuoliUOUtenteForEnabledSingleFunction");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLEGROUPS__TIPO_RUOLO_COD_MICROFUNC");
            q.setParam("codMicrofunzione", codicetipoMicrofunzione);
            q.setParam("idPeople", idPeople);
            q.setParam("idAmministrazione", IdAmm);
            q.setParam("param3", "A.CHA_PREFERITO DESC");
            string sql = q.getSQL();
            logger.Debug(sql);
            //db.fillTable(commandString3,dataSet,"RUOLI");
            this.ExecuteQuery(dataSet, "RUOLIXFUNC", sql);
        }
        //*************************************************************************************


        public void GetRuoliUtente(DataSet dataSet, string maxLivello, string idPeople)
        {
            logger.Debug("start > GetRuoliUtente");
            /*string commandString4="SELECT * FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND DTA_FINE IS NULL AND NUM_LIVELLO <=" + maxLivello;
            database.fillTable(commandString4,dataSet,"UO");*/
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
            q.setParam("param1", "*");
            q.setParam("param2", "A.CHA_TIPO_URP='U' AND A.CHA_TIPO_IE='I' AND A.DTA_FINE IS NULL AND A.NUM_LIVELLO <=" + maxLivello);
            string sql = q.getSQL();
            logger.Debug(sql);
            //db.fillTable(commandString3,dataSet,"RUOLI");
            this.ExecuteQuery(dataSet, "UO", sql);
        }


        public void GetRuoliUtente(DataSet dataSet, DataRow row, string maxLivello, string idPeople)
        {
            logger.Debug("start > GetRuoliUtente");
            /*string commandString4="SELECT * FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND DTA_FINE IS NULL AND NUM_LIVELLO <=" + maxLivello;
            database.fillTable(commandString4,dataSet,"UO");*/



            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobaliRuolo");
            // old q.setParam("param2", " AND A.NUM_LIVELLO <=" + maxLivello);

            if (dbType.ToUpper().Equals("ORACLE"))
                q.setParam("param2", " CONNECT BY PRIOR id_parent = system_id START WITH system_id =" + row["ID_UO"].ToString());
            else if (dbType.ToUpper().Equals("SQL"))
                q.setParam("param2", row["ID_UO"].ToString());

            string sql = q.getSQL();


            logger.Debug(sql);
            //db.fillTable(commandString3,dataSet,"RUOLI");
            this.ExecuteQuery(dataSet, "UO", sql);
        }

        public void GetRuoliUOUtente(DataSet dataSet, string idPeople)
        {
            logger.Debug("start > GetRuoliUOUtente");
            //DataSet dataSet = new DataSet();
            //string sql="SELECT A.PEOPLE_SYSTEM_ID, B.SYSTEM_ID, B.ID_GRUPPO, C.NUM_LIVELLO, C.VAR_CODICE, C.VAR_DESC_RUOLO, B.NUM_LIVELLO, B.ID_UO, B.VAR_COD_RUBRICA, B.ID_REGISTRO FROM PEOPLEGROUPS A, DPA_CORR_GLOBALI B, DPA_TIPO_RUOLO C where B.ID_GRUPPO=A.GROUPS_SYSTEM_ID and b.id_tipo_ruolo=c.system_id and A.PEOPLE_SYSTEM_ID="+idPeople+" ORDER BY A.CHA_PREFERITO DESC";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLEGROUPS__TIPO_RUOLO");
            q.setParam("param1", "A.PEOPLE_SYSTEM_ID=" + idPeople);
            q.setParam("param2", "A.CHA_PREFERITO DESC");
            string sql = q.getSQL();
            logger.Debug(sql);
            //db.fillTable(commandString3,dataSet,"RUOLI");
            this.ExecuteQuery(dataSet, "RUOLI", sql);
        }

        public void GetRuoliUOUtenteByIdCorr(DataSet dataSet, string idCorr)
        {
            logger.Debug("start > GetRuoliUOUtente");
            //DataSet dataSet = new DataSet();
            //string sql="SELECT A.PEOPLE_SYSTEM_ID, B.SYSTEM_ID, B.ID_GRUPPO, C.NUM_LIVELLO, C.VAR_CODICE, C.VAR_DESC_RUOLO, B.NUM_LIVELLO, B.ID_UO, B.VAR_COD_RUBRICA, B.ID_REGISTRO FROM PEOPLEGROUPS A, DPA_CORR_GLOBALI B, DPA_TIPO_RUOLO C where B.ID_GRUPPO=A.GROUPS_SYSTEM_ID and b.id_tipo_ruolo=c.system_id and A.PEOPLE_SYSTEM_ID="+idPeople+" ORDER BY A.CHA_PREFERITO DESC";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLEGROUPS__TIPO_RUOLO");
            q.setParam("param1", "A.PEOPLE_SYSTEM_ID=(SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID=" + idCorr + ")");
            q.setParam("param2", "A.CHA_PREFERITO DESC");
            string sql = q.getSQL();
            logger.Debug(sql);
            //db.fillTable(commandString3,dataSet,"RUOLI");
            this.ExecuteQuery(dataSet, "RUOLI", sql);
        }

        public ArrayList GetFunzioni(string idRuolo)
        {
            //ricerca delle funzioni associate al ruolo
            /*string queryString =
                "SELECT A.SYSTEM_ID, A.COD_FUNZIONE, A.VAR_DESC_FUNZIONE, " +
                "A.ID_TIPO_FUNZIONE, B.VAR_COD_TIPO, B.VAR_DESC_TIPO_FUN " +
                "FROM DPA_FUNZIONI A, DPA_TIPO_FUNZIONE B, DPA_TIPO_F_RUOLO C " +
                "WHERE A.ID_TIPO_FUNZIONE=B.SYSTEM_ID AND " +
                "B.SYSTEM_ID=C.ID_TIPO_FUNZ AND C.ID_RUOLO_IN_UO=" + idRuolo;*/
            DataSet dataSet;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_FUNZIONI__TIPO_FUNZIONE__TIPO_F_RUOLO");
            q.setParam("param1", "C.ID_RUOLO_IN_UO=" + idRuolo);
            string sql = q.getSQL();
            //db.fillTable(queryString,dataSet,"FUNZIONI");
            this.ExecuteQuery(out dataSet, "FUNZIONI", sql);
            ArrayList funzioni = new ArrayList();
            foreach (DataRow funzioneRow in dataSet.Tables["FUNZIONI"].Rows)
            {
                DocsPaVO.utente.Funzione funzione = new DocsPaVO.utente.Funzione(
                    funzioneRow["SYSTEM_ID"].ToString(),
                    funzioneRow["VAR_DESC_FUNZIONE"].ToString(),
                    funzioneRow["COD_FUNZIONE"].ToString(),
                    funzioneRow["ID_TIPO_FUNZIONE"].ToString(),
                    funzioneRow["VAR_COD_TIPO"].ToString(),
                    funzioneRow["VAR_DESC_TIPO_FUN"].ToString()
                    );
                funzioni.Add(funzione);
            }
            dataSet.Dispose();
            return funzioni;
        }

        //query che estrae i ruoli che hanno la visibilità sul documento selezionato dal portale
        public ArrayList VisibilitaRuolo(string docNumber, string idGruppi)
        {
            DataSet dataSet;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SECURITY_VISIBILITA_RUOLI");
            q.setParam("param1", idGruppi);
            q.setParam("param2", docNumber);
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteQuery(out dataSet, "VISIB_RUOLI", sql);
            ArrayList visRuolo = new ArrayList();
            foreach (DataRow visRuoloRow in dataSet.Tables["VISIB_RUOLI"].Rows)
            {
                visRuolo.Add(visRuoloRow["PERSONORGROUP"].ToString());
            }
            dataSet.Dispose();
            return visRuolo;
        }

        public ArrayList GetListaRuoliUtente(string idPeople)
        {
            ArrayList ruoli = new ArrayList();
            DataSet dataSet = new DataSet();

            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            //ricerca dei ruoli e relative UO dell'utente

            utenti.GetRuoliUOUtente(dataSet, idPeople);

            // cerco il valore di livello più alto per le UO associate all'utente in modo
            // da limitare la dimensione della tabella delle UO.
            if (dataSet.Tables["RUOLI"].Rows.Count > 0)
            {

                string maxLivello = dataSet.Tables["RUOLI"].Select("", "NUM_LIVELLO desc")[0]["NUM_LIVELLO"].ToString();

                utenti.GetRuoliUtente(dataSet, maxLivello, idPeople);

                foreach (DataRow ruoloRow in dataSet.Tables["RUOLI"].Rows)
                {
                    if (ruoloRow["CHA_PREFERITO"] != null)
                    {
                        if (ruoloRow["CHA_PREFERITO"].ToString() == "1")
                            ruoli.Add(utenti.GetRuoloData(dataSet, ruoloRow));
                    }
                }

                foreach (DataRow ruoloRow in dataSet.Tables["RUOLI"].Rows)
                {
                    if (ruoloRow["CHA_PREFERITO"] == null)
                    {
                        ruoli.Add(utenti.GetRuoloData(dataSet, ruoloRow));
                    }
                    else
                    {
                        if (ruoloRow["CHA_PREFERITO"].ToString() != "1")
                            ruoli.Add(utenti.GetRuoloData(dataSet, ruoloRow));
                    }
                }
            }

            return ruoli;
        }

        public DocsPaVO.utente.UnitaOrganizzativa GetUnitaOrganizzativa(DataSet dataSet, DataRow uoRow)
        {
            //ricerca delle UO

            DocsPaVO.utente.UnitaOrganizzativa uo = new DocsPaVO.utente.UnitaOrganizzativa();
            uo.systemId = uoRow["SYSTEM_ID"].ToString();
            uo.descrizione = uoRow["VAR_DESC_CORR"].ToString();
            uo.codice = uoRow["VAR_CODICE"].ToString();
            uo.idAmministrazione = uoRow["ID_AMM"].ToString();
            uo.livello = uoRow["NUM_LIVELLO"].ToString();
            uo.codiceRubrica = uoRow["VAR_COD_RUBRICA"].ToString();
            DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
            sp.serverSMTP = uoRow["VAR_SMTP"].ToString();
            sp.portaSMTP = uoRow["NUM_PORTA_SMTP"].ToString();
            uo.serverPosta = sp;
            uo.idRegistro = uoRow["ID_REGISTRO"].ToString();
            uo.interoperante = FromCharToBool(uoRow["CHA_PA"].ToString());
            uo.codiceAOO = uoRow["VAR_CODICE_AOO"].ToString();
            uo.codiceAmm = uoRow["VAR_CODICE_AMM"].ToString();
            uo.email = uoRow["VAR_EMAIL"].ToString();
            uo.tipoIE = uoRow["CHA_TIPO_IE"].ToString();
            uo.tipoCorrispondente = uoRow["CHA_TIPO_URP"].ToString();
            uo.classificaUO = (uoRow["CLASSIFICA_UO"] == DBNull.Value ? null : uoRow["CLASSIFICA_UO"].ToString());

            //si ricava la parentela
            if (!uoRow["ID_PARENT"].ToString().Equals(""))
            {
                if (!uoRow["ID_PARENT"].ToString().Equals("0"))
                {
                    uo.parent = GetParents(uoRow["ID_PARENT"].ToString(), dataSet.Tables["UO"]);
                }
            }
            //			else
            //			{
            //				uo.parent = null;
            //			}

            return uo;
        }

        private static bool FromCharToBool(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Equals("1"))
                return true;
            else
                return false;
        }
        #endregion


        public ArrayList getListaStoriciMittenti(string idProfile, string tipo)
        {

            ArrayList lista = new ArrayList();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_STORICO_MITT_1");

            q.setParam("param1", tipo);
            q.setParam("param2", idProfile);
            string sql = q.getSQL();
            logger.Debug(sql);

            try
            {

                DataSet dataSet = new DataSet();
                this.ExecuteQuery(out dataSet, "STORICO", sql);


                foreach (DataRow dataRow in dataSet.Tables["STORICO"].Rows)
                {
                    lista.Add(getStoricoMittenti(dataRow));
                }
                dataSet.Dispose();

            }
            catch (Exception e)
            {
                throw (e);
            }
            return lista;
        }

        protected DocsPaVO.documento.StoricoMittente getStoricoMittenti(DataRow dataRow)
        {
            DocsPaVO.documento.StoricoMittente storico = new DocsPaVO.documento.StoricoMittente();

            storico.systemId = dataRow["SYSTEM_ID"].ToString();
            storico.dataModifica = dataRow["DTA_MODIFICA"].ToString();
            if (dataRow["var_cod_rubrica"] != null)
                storico.cod_rubrica = dataRow["var_cod_rubrica"].ToString();
            storico.descrizione = dataRow["VAR_DESC_CORR"].ToString();


            storico.utente = GetUtenteNoFiltroDisabled(dataRow["ID_PEOPLE"].ToString());
            storico.ruolo = GetRuolo(dataRow["ID_RUOLO_IN_UO"].ToString());

            return storico;
        }

        #region Gestione login
        public bool CheckUserLogin(string userId, string idAmm)
        {
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CHECK_USER_LOGIN");
                q.setParam("param1", userId);
                q.setParam("param2", idAmm);
                string sql = q.getSQL();
                logger.Debug(sql);


                IDataReader dr = (IDataReader)this.ExecuteReader(sql);
                if (dr == null)
                    throw new Exception();

                if (dr.Read())
                    result = (Convert.ToDecimal(dr[0]) != 0);

                if (!dr.IsClosed)
                    dr.Close();
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante il controllo della sessione web.", exception);
                result = false;
            }
            finally
            {
                CloseConnection();
            }
            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webSessionId"></param>
        /// <returns></returns>
        //		public DocsPaVO.utente.UserLogin.ValidationResult CheckUserLogin(string userName, string idAmm,  string webSessionId)
        //		{
        //			DocsPaVO.utente.UserLogin.ValidationResult result = DocsPaVO.utente.UserLogin.ValidationResult.OK;
        //
        //			try
        //			{
        //				DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CHECK_USER_LOGIN_ON_SESSION_ID");
        //				q.setParam("param1", webSessionId);
        //				string sql = q.getSQL();
        //				//logger.Debug(sql);
        //				string valore = null;
        //
        //				
        //				if(!this.ExecuteScalar(out valore, sql)) 
        //					throw new Exception();
        //
        //				if(valore == "0") 
        //				{
        //					q = DocsPaUtils.InitQuery.getInstance().getQuery("CHECK_USER_LOGIN");
        //					q.setParam("param1", userName);
        //					q.setParam("param2", idAmm);
        //					sql = q.getSQL();
        //					logger.Debug(sql);
        //					valore = null;
        //
        //					if(!this.ExecuteScalar(out valore, sql)) throw new Exception();						
        //					
        //                    if(valore != "0") 
        //						result = DocsPaVO.utente.UserLogin.ValidationResult.SESSION_DROPPED;						
        //				}
        //			}
        //			catch(Exception exception)
        //			{
        //				logger.Debug("Errore durante la validazione della login.", exception); 
        //				result = DocsPaVO.utente.UserLogin.ValidationResult.APPLICATION_ERROR;				
        //			}
        //
        //			return result;
        //		}
        //

        public DocsPaVO.utente.UserLogin.ValidationResult CheckUserLogin(string userName, string idAmm, string webSessionId)
        {
            DocsPaVO.utente.UserLogin.ValidationResult result = DocsPaVO.utente.UserLogin.ValidationResult.OK;

            IDataReader dr = null;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CHECK_USER_LOGIN_ON_SESSION_ID");
                q.setParam("param1", webSessionId);
                q.setParam("userid", userName);
                q.setParam("idamm", idAmm);
                string sql = q.getSQL();
                //logger.Debug(sql);

                OpenConnection();
                dr = (IDataReader)this.ExecuteReader(sql);
                if (dr == null)
                    throw new Exception();

                bool session_id_exists = false;


                if (dr.Read())
                    session_id_exists = (Convert.ToDecimal(dr[0]) != 0);

                if (!dr.IsClosed)
                    dr.Close();

                if (!session_id_exists)
                {
                    // Session ID non valido, controlliamo se un altro utilizzatore
                    // ha effettuato un accesso con lo stesso USER_ID
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("CHECK_USER_LOGIN");
                    q.setParam("param1", userName);
                    q.setParam("param2", idAmm);
                    sql = q.getSQL();
                    logger.Debug(sql);

                    bool user_id_exists = false;
                    dr = (IDataReader)ExecuteReader(sql);
                    if (dr == null)
                        throw new Exception();

                    if (dr.Read())
                        user_id_exists = (Convert.ToDecimal(dr[0]) != 0);

                    if (!dr.IsClosed)
                        dr.Close();

                    if (user_id_exists)
                        result = DocsPaVO.utente.UserLogin.ValidationResult.SESSION_DROPPED;
                }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante la validazione della login.", exception);
                result = DocsPaVO.utente.UserLogin.ValidationResult.APPLICATION_ERROR;
            }
            finally
            {
                if (!dr.IsClosed)
                    dr.Close();

                CloseConnection();
            }

            return result;
        }


        public string GetUserIPAddress(string userid, string idamm)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_IP_ADDRESS_USER_LOGIN");
            q.setParam("param1", userid);
            q.setParam("param2", idamm);
            //logger.Debug("GetUserIPAddress");
            string sql = q.getSQL();
            logger.Debug(sql);
            string ipaddress = "";
            this.ExecuteScalar(out ipaddress, sql);
            return ipaddress;
        }

        public bool LockUserLogin(string userId, string idAmm, string webSessionId, string hostname, string dst)
        {
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("LOCK_USER_LOGIN");
                q.setParam("param1", userId);

                if (!string.IsNullOrEmpty(idAmm))
                    q.setParam("param2", idAmm);
                else
                    q.setParam("param2", "Null");

                if (!string.IsNullOrEmpty(hostname))
                    q.setParam("param3", hostname);
                else
                    q.setParam("param3", "Null");
                string valoreChiave;
                valoreChiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_ORARIO_CONN_UTE_H24");
                if (valoreChiave != null && valoreChiave.Equals("1"))
                    q.setParam("param4", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                else q.setParam("param4", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")));

                q.setParam("param5", webSessionId);
                q.setParam("param6", dst);

                string sql = q.getSQL();
                logger.Debug(sql);

                int rowsAffected;
                if (this.ExecuteNonQuery(sql, out rowsAffected))
                    result = (rowsAffected == 1);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;

        }

        public bool LockUserLoginDelegato(string userId, string idAmm, string webSessionId, string hostname, string dst, string userId_delegato, string id_delega)
        {
            string sql;
            DocsPaUtils.Query q;
            bool result = false;
            try
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("LOCK_USER_LOGIN_DELEGATO");
                q.setParam("param1", userId);

                if (!string.IsNullOrEmpty(idAmm))
                    q.setParam("param2", idAmm);
                else
                    q.setParam("param2", "Null");

                if (!string.IsNullOrEmpty(hostname))
                    q.setParam("param3", hostname);
                else
                    q.setParam("param3", "Null");
                string valoreChiave;
                valoreChiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_ORARIO_CONN_UTE_H24");
                if (valoreChiave != null && valoreChiave.Equals("1"))
                    q.setParam("param4", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                else q.setParam("param4", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")));
                q.setParam("param5", webSessionId);
                q.setParam("param6", dst);
                q.setParam("param7", userId_delegato);

                sql = q.getSQL();
                logger.Debug(sql);

                int rowsAffected;
                if (this.ExecuteNonQuery(sql, out rowsAffected))
                    result = (rowsAffected == 1);

                //U_MODIFICA_DELEGA set cha_in_esercizio=1
                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_MODIFICA_DELEGA");
                q.setParam("param1", "cha_in_esercizio = '1'");
                q.setParam("id_delega", id_delega);
                sql = q.getSQL();
                this.ExecuteNonQuery(sql);

            }
            catch (Exception)
            {
                result = false;
            }
            return result;

        }

        public bool UnlockUserLogin(string userId, string idAmm)
        {
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("UNLOCK_USER_LOGIN");
                q.setParam("param1", userId);
                if (idAmm != null && idAmm != "") //tanto la userId è univoca in Docspa, quindi non serve anche l'idAmm.
                    q.setParam("param2", " AND ID_AMM=" + idAmm);
                string sql = q.getSQL();
                logger.Debug(sql);
                if (this.ExecuteNonQuery(sql))
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;

        }

        public bool RDE_UnlockUserLogin(string userId)
        {
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("RDE_UNLOCK_USER_LOGIN");
                q.setParam("param1", userId);
                string sql = q.getSQL();
                logger.Debug(sql);
                if (this.ExecuteNonQuery(sql))
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;

        }

        public bool UnlockUserLogin(string userId, string idAmm, string sessionId)
        {
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("UNLOCK_USER_LOGIN_ON_SESSION_ID");
                q.setParam("param1", userId);
                if (!string.IsNullOrEmpty(idAmm))
                    q.setParam("param2", " AND ID_AMM=" + idAmm);
                else
                    q.setParam("param2", " ");
                if (!string.IsNullOrEmpty(sessionId))
                    q.setParam("param3", " AND SESSION_ID='" + sessionId + "'");
                else
                    q.setParam("param3", " ");
                string sql = q.getSQL();
                logger.Debug(sql);
                if (this.ExecuteNonQuery(sql))
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;

        }

        public DataSet GetUtentiConnessi(string codiceAmm)
        {
            DataSet result = null;
            try
            {
                //recupera id amministrazione
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazione = new AmministrazioneXml();
                string idAmm = amministrazione.GetAdminByName(codiceAmm);
                if (idAmm != null)
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_UTENTI_CONNESSI");
                    q.setParam("param1", idAmm);
                    string sql = q.getSQL();
                    logger.Debug(sql);
                    if (!this.ExecuteQuery(out result, sql))
                    {
                        result = null;
                    }
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;

        }

        public int GetNumUtentiConnessi(string codiceAmm)
        {
            int result = 0;
            IDataReader dr = null;
            try
            {
                //recupera id amministrazione
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazione = new AmministrazioneXml();
                string idAmm = amministrazione.GetAdminByName(codiceAmm);
                if (idAmm != null)
                {

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_NUM_UTENTI_CONNESSI");
                    q.setParam("param1", idAmm);
                    string sql = q.getSQL();
                    string outRes;
                    logger.Debug(sql);
                    this.ExecuteScalar(out outRes, sql);
                    if (!outRes.Equals("0")) result = Convert.ToInt32(outRes);
                }
            }
            catch (Exception)
            {
                result = 0;
            }
            return result;
        }

        public int GetNumUtentiAttivi(string codiceAmm)
        {
            int result = 0;
            IDataReader dr = null;
            try
            {
                //recupera id amministrazione
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazione = new AmministrazioneXml();
                string idAmm = amministrazione.GetAdminByName(codiceAmm);
                if (idAmm != null)
                {

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_NUM_UTENTI_ATTIVI");
                    q.setParam("param1", idAmm);
                    string sql = q.getSQL();
                    string outRes;
                    logger.Debug(sql);
                    this.ExecuteScalar(out outRes, sql);
                    if (!outRes.Equals("0")) result = Convert.ToInt32(outRes);
                }
            }
            catch (Exception)
            {
                result = 0;
            }
            return result;
        }


        public string GetNomeUtente(string userId)
        {
            string result = null;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_NOMEUTENTE_BY_USERID");
                q.setParam("param1", userId);
                string sql = q.getSQL();
                logger.Debug(sql);
                if (!this.ExecuteScalar(out result, sql))
                {
                    result = null;
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;

        }

        public string GetNomeCognomeUtente(string userId, string idAmm)
        {
            DataSet ds = new DataSet();
            string result = null;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_NOMECOGNOMEUTENTE");
                q.setParam("userId", userId);
                q.setParam("idAmm", idAmm);
                string sql = q.getSQL();
                logger.Debug(sql);
                if (this.ExecuteQuery(out ds, "utenti", sql))
                {
                    if (ds.Tables["utenti"].Rows.Count == 1)
                    {
                        DataRow dr = ds.Tables["utenti"].Rows[0];
                        result = dr["VAR_NOME"].ToString() + " " + dr["VAR_COGNOME"].ToString(); ;
                    }
                }
                else
                    result = null;
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }


        public bool DeleteADLUtente(string idPeople)
        {
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DELETE_ADL_UTENTE");
                q.setParam("param1", idPeople);
                string sql = q.getSQL();
                logger.Debug(sql);
                result = this.ExecuteNonQuery(sql);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public DataSet VerificaUtente(string userName)
        {
            DataSet ds;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("VERIFICA_UTENTE");
                q.setParam("param1", userName);
                string sql = q.getSQL();
                logger.Debug(sql);
                this.ExecuteQuery(out ds, sql);
            }
            catch (Exception e)
            {
                throw e;
            }

            return ds;
        }

        public bool IsUtenteDisabled(string userName, string idAmm)
        {
            string disabled = "";

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_People_Disabled");
                q.setParam("param1", userName);
                if (!string.IsNullOrEmpty(idAmm))
                    q.setParam("param2", " and ID_AMM = " + idAmm);
                else
                    q.setParam("param2", "");
                string sql = q.getSQL();
                logger.Debug(sql);
                this.ExecuteScalar(out disabled, sql);
                if (disabled == "Y")
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        #endregion
        /// <summary>
        /// verifica se il ruolo destinatario di un protocollo in uscita
        /// è autorizzato sul registro sul quale si sta protocollando un doc.
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <param name="idregistro"></param>
        /// <returns></returns>
        public bool VerificaRuoloAut(string idRuolo, string idregistro)
        {
            bool result = false;
            string valore = null;
            try
            {

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_L_RUOLO_REG");
                q.setParam("id_reg", idregistro);
                q.setParam("id_ruolo", idRuolo);
                string sql = q.getSQL();
                logger.Debug(sql);

                this.ExecuteScalar(out valore, sql);
                if (valore != null && valore != "")
                {
                    result = true;
                }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante VerificaRuoloAut.", exception);
                result = false;
            }
            return result;

        }

        public DocsPaVO.utente.Utente getUtenteById(string idPeople)
        {
            DocsPaVO.utente.Utente ut = null;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_UTENTE");
                q.setParam("idUtente", idPeople);
                string sql = q.getSQL();
                logger.Debug(sql);

                DataSet ds = new DataSet();

                ExecuteQuery(ds, sql);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    ut = new DocsPaVO.utente.Utente();
                    ut.systemId = ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString();
                    ut.userId = ds.Tables[0].Rows[0]["USER_ID"].ToString();
                    ut.descrizione = ds.Tables[0].Rows[0]["FULL_NAME"].ToString();
                    ut.telefono = ds.Tables[0].Rows[0]["PHONE"].ToString();
                    ut.email = ds.Tables[0].Rows[0]["EMAIL_ADDRESS"].ToString();
                    ut.notifica = ds.Tables[0].Rows[0]["CHA_NOTIFICA"].ToString();
                    ut.idAmministrazione = ds.Tables[0].Rows[0]["ID_AMM"].ToString();
                    ut.cognome = ds.Tables[0].Rows[0]["VAR_COGNOME"].ToString();
                    ut.nome = ds.Tables[0].Rows[0]["VAR_NOME"].ToString();
                    ut.sede = ds.Tables[0].Rows[0]["VAR_SEDE"].ToString();
                    ut.matricola = ds.Tables[0].Rows[0]["MATRICOLA"].ToString();
                    ut.idPeople = ut.systemId;
                    //Abbatangeli Gianlugigi - carico le applicazioni relative all'utente
                    ut.extApplications = this.getApplicazioniUtente(ut.idPeople);

                    //ut.notificaConAllegato = ds.Tables[0].Rows[0]["CHA_NOTIFICA_CON_ALLEGATO"].ToString();
                }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante getUtenteById.", exception);
            }
            return ut;
        }

        //Abbatangeli Gianluigi - Carica le applicazioni in un array per assegnarlo all'utente
        private ArrayList getApplicazioniUtente(string idPeople)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Utenti> GetListaApplicazioni");

            ArrayList listaApplicazioni = new ArrayList();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_UTENTE_APPLICAZIONI");
            q.setParam("idUtente", idPeople);
            string sql = q.getSQL();
            logger.Debug(sql);

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, sql);
            if (dataSet != null)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    DocsPaVO.utente.ExtApplication applicazione = new DocsPaVO.utente.ExtApplication();

                    applicazione.systemId = row[0].ToString();
                    applicazione.codice = row[1].ToString();
                    applicazione.descrizione = row[2].ToString();
                    listaApplicazioni.Add(applicazione);
                }
            }

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Utenti> GetListaApplicazioni");
            return listaApplicazioni;
        }

        /// <summary>
        /// attenzione restituisce ruolo.uo = null
        /// </summary>
        /// <param name="idCorrGlobali"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Ruolo getRuoloByCodice(string codice)
        {
            DocsPaVO.utente.Ruolo ruolo = null;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_RUOLO_BY_CODICE");
                q.setParam("codice", codice);
                string sql = q.getSQL();
                logger.Debug(sql);

                DataSet ds = new DataSet();

                ExecuteQuery(ds, sql);
                if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count != 0)
                {
                    ruolo = new DocsPaVO.utente.Ruolo();
                    ruolo.systemId = ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString();
                    ruolo.idRegistro = ds.Tables[0].Rows[0]["ID_REGISTRO"].ToString();
                    ruolo.idAmministrazione = ds.Tables[0].Rows[0]["ID_AMM"].ToString();
                    ruolo.codiceRubrica = ds.Tables[0].Rows[0]["VAR_COD_RUBRICA"].ToString();
                    ruolo.descrizione = ds.Tables[0].Rows[0]["VAR_DESC_CORR"].ToString();
                    ruolo.idOld = ds.Tables[0].Rows[0]["ID_OLD"].ToString();
                    ruolo.dta_fine = ds.Tables[0].Rows[0]["DTA_FINE"].ToString();
                    ruolo.codice = ds.Tables[0].Rows[0]["VAR_CODICE"].ToString();
                    ruolo.idGruppo = ds.Tables[0].Rows[0]["ID_GRUPPO"].ToString();
                    ruolo.tipoIE = ds.Tables[0].Rows[0]["CHA_TIPO_IE"].ToString();
                    ruolo.tipoCorrispondente = ds.Tables[0].Rows[0]["CHA_TIPO_URP"].ToString();
                    ruolo.codiceAOO = ds.Tables[0].Rows[0]["VAR_CODICE_AOO"].ToString();
                    ruolo.codiceAmm = ds.Tables[0].Rows[0]["VAR_CODICE_AMM"].ToString();
                    ruolo.codiceIstat = ds.Tables[0].Rows[0]["VAR_CODICE_ISTAT"].ToString();
                    ruolo.email = ds.Tables[0].Rows[0]["VAR_EMAIL"].ToString();
                    ruolo.tipoCorrispondente = "R";
                    if (ds.Tables[0].Columns.Contains("CHA_DISABLED_TRASM"))
                    {
                        if (ds.Tables[0].Rows[0]["CHA_DISABLED_TRASM"].ToString().Equals("1"))
                            ruolo.disabledTrasm = true;
                        else
                            ruolo.disabledTrasm = false;
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante getRuoloByCodice.", exception);
            }
            return ruolo;
        }

        /// <summary>
        /// attenzione restituisce ruolo.uo = null
        /// 
        /// Se la ID_UO non è null, restituisce solo ed esclusivamente ruolo.uo.system_id
        /// </summary>
        /// <param name="idCorrGlobali"></param>
        /// <returns></returns>
        public DocsPaVO.utente.Ruolo getRuoloById(string idCorrGlobali)
        {
            DocsPaVO.utente.Ruolo ruolo = null;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_RUOLO");
                q.setParam("idCorrGlobali", idCorrGlobali);
                string sql = q.getSQL();
                logger.Debug(sql);

                DataSet ds = new DataSet();

                ExecuteQuery(ds, sql);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    ruolo = new DocsPaVO.utente.Ruolo();
                    ruolo.systemId = ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString();
                    ruolo.idRegistro = ds.Tables[0].Rows[0]["ID_REGISTRO"].ToString();
                    ruolo.idAmministrazione = ds.Tables[0].Rows[0]["ID_AMM"].ToString();
                    ruolo.codiceRubrica = ds.Tables[0].Rows[0]["VAR_COD_RUBRICA"].ToString();
                    ruolo.descrizione = ds.Tables[0].Rows[0]["VAR_DESC_CORR"].ToString();
                    ruolo.idOld = ds.Tables[0].Rows[0]["ID_OLD"].ToString();
                    ruolo.dta_fine = ds.Tables[0].Rows[0]["DTA_FINE"].ToString();
                    ruolo.livello = ds.Tables[0].Rows[0]["NUM_LIVELLO"].ToString();
                    ruolo.codice = ds.Tables[0].Rows[0]["VAR_CODICE"].ToString();
                    ruolo.idGruppo = ds.Tables[0].Rows[0]["ID_GRUPPO"].ToString();
                    //ruolo.tipoCorrispondente = ds.Tables[0].Rows[0]["CHA_TIPO_CORR"].ToString();
                    ruolo.tipoIE = ds.Tables[0].Rows[0]["CHA_TIPO_IE"].ToString();
                    ruolo.tipoCorrispondente = ds.Tables[0].Rows[0]["CHA_TIPO_URP"].ToString();
                    ruolo.codiceAOO = ds.Tables[0].Rows[0]["VAR_CODICE_AOO"].ToString();
                    ruolo.codiceAmm = ds.Tables[0].Rows[0]["VAR_CODICE_AMM"].ToString();
                    ruolo.codiceIstat = ds.Tables[0].Rows[0]["VAR_CODICE_ISTAT"].ToString();
                    ruolo.email = ds.Tables[0].Rows[0]["VAR_EMAIL"].ToString();
                    // Modifica per stampe repertorio automatiche. Prelevo l'idUO.
                    if (ds.Tables[0].Rows[0]["ID_UO"] != null)
                    {
                        ruolo.uo = new UnitaOrganizzativa();
                        ruolo.uo.systemId = ds.Tables[0].Rows[0]["ID_UO"].ToString();
                    }

                    ruolo.tipoCorrispondente = "R";
                    if (ds.Tables[0].Columns.Contains("CHA_DISABLED_TRASM"))
                    {
                        if (ds.Tables[0].Rows[0]["CHA_DISABLED_TRASM"].ToString().Equals("1"))
                            ruolo.disabledTrasm = true;
                        else
                            ruolo.disabledTrasm = false;
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante getRuoloById.", exception);
            }
            return ruolo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="idGruppi"></param>
        /// <returns></returns>
        public string RemoveVisibilita(string docNumber, DocsPaVO.utente.Corrispondente corr)
        {
            DataSet dataSet;
            string personorgroup = "";
            bool rtn = false;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("DELETE_SECURITY_VISIBILITA");
            q.setParam("param1", docNumber);
            if (corr.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
                personorgroup = ((DocsPaVO.utente.Ruolo)corr).idGruppo;
            else if (corr.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
                personorgroup = ((DocsPaVO.utente.Utente)corr).idPeople;

            q.setParam("param2", personorgroup);
            string sql = q.getSQL();
            logger.Debug(sql);
            int rows = 0;
            this.BeginTransaction();
            rtn = this.ExecuteNonQuery(sql, out rows);
            if (!(rtn && rows > 0))
            {
                this.RollbackTransaction();
                throw new Exception("errore nell'esecuzione della query : " + sql);
            }
            else this.CommitTransaction();


            return rtn.ToString();
        }


        #region Query documentale Filenet

        public bool AddUserFilenet(string userID, string userGroup, string archivio, string userFullName)
        {
            int rows;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FILENET_I_ADD_USER");
            q.setParam("param1", userID);
            q.setParam("param2", userGroup);
            //q.setParam("param3", archivio);
            q.setParam("param4", userFullName);
            string sql = q.getSQL();
            logger.Debug(sql);

            try
            {
                this.BeginTransaction();
                this.ExecuteNonQuery(sql, out rows);
                if (rows == 0)
                    throw new Exception("Errore durante l'inserimento dell'utente Filenet: " + userID);

                logger.Debug("Metodo AddUserFilenet - righe inserite: " + rows);

                DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("FILENET_I_ADD_USERGROUP");
                q2.setParam("param1", "Administrators");
                q2.setParam("param2", userID);
                sql = q2.getSQL();
                logger.Debug(sql);
                this.ExecuteNonQuery(sql, out rows);
                if (rows == 0)
                    this.RollbackTransaction();
                else
                    this.CommitTransaction();

                logger.Debug("Metodo AddUserFilenet - righe inserite: " + rows);

                return true;
            }
            catch (Exception e)
            {
                this.RollbackTransaction();
                throw e;
            }
        }

        public bool DeleteUserFilenet(string username)
        {
            int rows;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FILENET_D_USER");
            q.setParam("param1", username);
            string sql = q.getSQL();
            logger.Debug(sql);

            DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("FILENET_D_GRP_USR");
            q2.setParam("param1", username);
            string sql2 = q2.getSQL();
            logger.Debug(sql2);

            try
            {
                this.BeginTransaction();
                this.ExecuteNonQuery(sql, out rows);
                if (rows == 0)
                    throw new Exception("Errore durante l'eliminazione dell'utente Filenet: " + username);

                logger.Debug("Metodo DeleteUserFilenet - righe eliminate: " + rows);

                this.ExecuteNonQuery(sql2, out rows);
                if (rows == 0)
                    this.RollbackTransaction();
                else
                    this.CommitTransaction();

                logger.Debug("Metodo DeleteUserFilenet - righe eliminate: " + rows);

                return true;
            }
            catch (Exception e)
            {
                this.RollbackTransaction();
                throw e;
            }

        }


        public bool DisableUserFilenet(string username)
        {
            int rows;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FILENET_DISABLE_PEOPLE");
            q.setParam("param1", username);
            string sql = q.getSQL();
            logger.Debug(sql);

            try
            {
                this.ExecuteNonQuery(sql, out rows);
                if (rows == 0)
                    throw new Exception("Errore durante l'eliminazione dell'utente Filenet: " + username);

                logger.Debug("Metodo DisableUserFilenet - righe interessate: " + rows);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool UpdateUserFilenet(string username, string userfullname)
        {
            int rows;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FILENET_M_USER");
            q.setParam("param1", username);
            q.setParam("param2", userfullname);
            q.setParam("param3", username);
            string sql = q.getSQL();
            logger.Debug(sql);

            try
            {
                this.ExecuteNonQuery(sql, out rows);
                if (rows == 0)
                    throw new Exception("Errore durante la modifica dell'utente Filenet: " + username);

                logger.Debug("Metodo UpdateUserFilenet - righe modificate: " + rows);

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string GetPasswordUserFilenet(string username)
        {
            string user_password = "";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FILENET_S_PEOPLE");
            q.setParam("param1", username);
            string sql = q.getSQL();
            logger.Debug(sql);

            try
            {
                this.ExecuteScalar(out user_password, sql);
                logger.Debug("Metodo GetPasswordUserFilenet");
                return user_password;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string GetPasswordUserFilenet(string systemID, bool fromSystemID)
        {
            string user_password = "";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FILENET_S_PEOPLE_2");
            q.setParam("param1", systemID);
            string sql = q.getSQL();
            logger.Debug(sql);

            try
            {
                this.ExecuteScalar(out user_password, sql);
                logger.Debug("Metodo GetPasswordUserFilenet");
                return user_password;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Eliminazione Corrispondente Esterno
        /// </summary>
        /// <param name="idCorrglobali">system_id del corrisoindente che deve essere rimosso/disabilitato</param>
        /// <param name="flagListe">1 se le liste sono abilitate, 0 altrimenti</param>
        /// <returns></returns>
        public bool DeleteCorrispondenteEsterno(string idCorrglobali, int flagListe, DocsPaVO.utente.InfoUtente user, out string message)
        {

            bool retValue = false;
            message = string.Empty;
            int retProc;
            try
            {
                this.BeginTransaction();

                // Creazione parametri per la Store Procedure
                ArrayList parameters = new ArrayList();
                if (idCorrglobali != null && idCorrglobali != "")
                {
                    parameters.Add(this.CreateParameter("IDCorrGlobale", idCorrglobali));
                    parameters.Add(this.CreateParameter("liste", flagListe));
                    parameters.Add(this.CreateParameter("IdPeople", user.idPeople));
                    parameters.Add(this.CreateParameter("IdGruppo", user.idGruppo));
                    //

                    if (dbType.ToUpper().Equals("SQL"))
                    {
                        //BUG cancellazione corrispondente per sql server
                        // Parametro di output relativo all'eventuale nuovo corrispondente inserito
                        DocsPaUtils.Data.ParameterSP outParam = new DocsPaUtils.Data.ParameterSP("ReturnValue", 0, DocsPaUtils.Data.DirectionParameter.ParamOutput);
                        outParam.Tipo = DbType.Int32;
                        parameters.Add(outParam);
                    }

                    retProc = this.ExecuteStoreProcedure("SP_DELETE_CORR_ESTERNO", parameters);
                    /*
                        VALORI RITORNATI
					
                        0: CANCELLAZIONE EFFETTUATA - operazione andata a buon fine
                        1: DISABILITAZIONE EFFETTUATA - il corrispondente è presente nella DPA_DOC_ARRIVO_PAR, quindi non viene cancellato

                        2: CORRISPONDENTE NON RIMOSSO - il corrispondente è presente nella lista di distribuzione e non posso rimuoverlo
	
                        3: ERRORE: la DELETE sulla dpa_corr_globali NON è andata a buon fine
                        4: ERRORE: la DELETE sulla dpa_dett_globali NON è andata a buon fine
                        5: ERRORE: l' UPDATE sulla dpa_corr_globali NON è andata a buon fine
                        6: ERRORE: la DELETE sulla dpa_liste_distr NON è andata a buon fine	
                        7: DISABILITAZIONE EFFETTUATA - il corrispondente non è stato usato come mittente/destinatario ma solo in campi profilati di documenti/fascicoli
										
                    */
                    logger.Debug("Chiamata SP 'SP_DELETE_CORR_ESTERNO'. Esito: " + Convert.ToString(retProc));

                    // Commit / rollback transazione
                    switch (retProc)
                    {
                        case 0:
                            this.CommitTransaction();
                            logger.Debug("Eseguita Commit alla Stored Procedure: SP_DELETE_CORR_ESTERNO - il corrispondente è stato cancellato");
                            retValue = true;
                            message = "OK";
                            break;
                        case 1:
                            this.CommitTransaction();
                            logger.Debug("Eseguita Commit alla Stored Procedure: SP_DELETE_CORR_ESTERNO - il corrispondente è stato disabilitato poichè è mitt o dest di documenti!");
                            retValue = true;
                            message = "OK";
                            break;
                        case 2:
                            this.RollbackTransaction();
                            logger.Debug("Eseguita Rollback alla Stored Procedure: SP_DELETE_CORR_ESTERNO - il corrispondente non viene rimosso poichè é presente in una lista di distribuzione");
                            message = "NOTOK";
                            break;
                        case 3:
                            this.RollbackTransaction();
                            logger.Debug("ERRORE - Eseguita Rollback alla Stored Procedure:  SP_DELETE_CORR_ESTERNO  - CAUSA : Delete sulla DPA_CORR_GLOBALI NON è andata a buon fine");
                            message = "ERROR3";
                            break;
                        case 4:
                            this.RollbackTransaction();
                            logger.Debug("ERRORE - Eseguita Rollback alla Stored Procedure:  SP_DELETE_CORR_ESTERNO  - CAUSA :  Delete sulla DPA_DETT_GLOBALI NON è andata a buon fine");
                            message = "ERROR4";
                            break;
                        case 5:
                            this.RollbackTransaction();
                            logger.Debug("ERRORE - Eseguita Rollback alla Stored Procedure:  SP_DELETE_CORR_ESTERNO - CAUSA : UPDATE sulla dpa_corr_globali NON è andata a buon fine");
                            message = "ERROR5";
                            break;
                        case 6:
                            this.RollbackTransaction();
                            logger.Debug("ERRORE - Eseguita Rollback alla Stored Procedure:  SP_DELETE_CORR_ESTERNO - CAUSA :  DELETE sulla dpa_liste_distr NON è andata a buon fine");
                            message = "ERROR6";
                            break;
                        case 7:
                            this.CommitTransaction();
                            logger.Debug("Eseguita Commit alla Stored Procedure: SP_DELETE_CORR_ESTERNO - il corrispondente è stato disabilitato poichè è stato usato in campi profilati!");
                            retValue = true;
                            message = "OK";
                            break;

                        default:
                            this.RollbackTransaction();
                            retValue = false;
                            message = "Errore durante l'eliminazione di un corrispondente: codice non gestito";
                            break;
                    }
                }

            }
            catch (Exception e)
            {
                this.RollbackTransaction();
                retValue = false;
                message = "Errore durante l'eliminazione di un corrispondente" + e.Message;
                logger.Debug(e.Message);
            }

            return retValue;
        }

        /// <summary>
        /// Modifica Corrispondente Esterno
        /// </summary>
        /// <param name="idCorrglobali">systemId del corrispondente che si vuole modificare</param>
        /// <param name="datiModifica">Oggetto che contiene tutti i dati del corrispondente</param>
        /// <param name="message">messaggio di output per esito procedura</param>
        /// <returns></returns>
        public bool
            ModifyCorrispondenteEsterno(DocsPaVO.utente.DatiModificaCorr datiModifica, InfoUtente user, out string newIdCorrGlobali, out string message)
        {
            bool retValue = false;
            newIdCorrGlobali = string.Empty;
            message = string.Empty;
            int retProc;

            try
            {
                this.BeginTransaction();

                // Creazione parametri per la Store Procedure
                ArrayList parameters = new ArrayList();
                if (datiModifica != null)
                {
                    parameters.Add(this.CreateParameter("IDCorrGlobale", datiModifica.idCorrGlobali));
                    parameters.Add(this.CreateParameter("desc_corr", datiModifica.descCorr));
                    //NOME
                    if (datiModifica.nome != null && datiModifica.nome != "")
                        parameters.Add(this.CreateParameter("nome", datiModifica.nome));
                    else
                        parameters.Add(this.CreateParameter("nome", DBNull.Value));
                    //COGNOME
                    if (datiModifica.cognome != null && datiModifica.cognome != "")
                        parameters.Add(this.CreateParameter("cognome", datiModifica.cognome));
                    else
                        parameters.Add(this.CreateParameter("cognome", DBNull.Value));
                    //CODICE AOO
                    if (datiModifica.codiceAoo != null && datiModifica.codiceAoo != "")
                        parameters.Add(this.CreateParameter("codice_aoo", datiModifica.codiceAoo));
                    else
                        parameters.Add(this.CreateParameter("codice_aoo", DBNull.Value));
                    //CODICE AMM
                    if (datiModifica.codiceAmm != null && datiModifica.codiceAmm != "")
                        parameters.Add(this.CreateParameter("codice_amm", datiModifica.codiceAmm));
                    else
                        parameters.Add(this.CreateParameter("codice_amm", DBNull.Value));
                    //EMAIL
                    if (datiModifica.email != null && datiModifica.email != "")
                        parameters.Add(this.CreateParameter("email", datiModifica.email));
                    else
                        parameters.Add(this.CreateParameter("email", DBNull.Value));
                    //INDIRIZZO
                    if (datiModifica.indirizzo != null && datiModifica.indirizzo != "")
                        parameters.Add(this.CreateParameter("indirizzo", datiModifica.indirizzo));
                    else
                        parameters.Add(this.CreateParameter("indirizzo", DBNull.Value));
                    //CAP
                    if (datiModifica.cap != null && datiModifica.cap != "")
                        parameters.Add(this.CreateParameter("cap", datiModifica.cap));
                    else
                        parameters.Add(this.CreateParameter("cap", DBNull.Value));
                    //PROVINCIA
                    if (datiModifica.provincia != null && datiModifica.provincia != "")
                        parameters.Add(this.CreateParameter("provincia", datiModifica.provincia));
                    else
                        parameters.Add(this.CreateParameter("provincia", DBNull.Value));
                    //NAZIONE
                    if (datiModifica.nazione != null && datiModifica.nazione != "")
                        parameters.Add(this.CreateParameter("nazione", datiModifica.nazione));
                    else
                        parameters.Add(this.CreateParameter("nazione", DBNull.Value));
                    //CITTA
                    if (datiModifica.citta != null && datiModifica.citta != "")
                        parameters.Add(this.CreateParameter("citta", datiModifica.citta));
                    else
                        parameters.Add(this.CreateParameter("citta", DBNull.Value));
                    //CODICE FISCALE
                    if (datiModifica.codFiscale != null && datiModifica.codFiscale != "")
                        parameters.Add(this.CreateParameter("cod_fiscale", datiModifica.codFiscale));
                    else
                        parameters.Add(this.CreateParameter("cod_fiscale", DBNull.Value));
                    //PARTITA IVA
                    if (datiModifica.partitaIva != null && datiModifica.partitaIva != "")
                        parameters.Add(this.CreateParameter("partita_iva", datiModifica.partitaIva));
                    else
                        parameters.Add(this.CreateParameter("partita_iva", DBNull.Value));
                    /*/CODICE IPA
                    if (datiModifica.codiceIpa != null && datiModifica.codiceIpa != "")
                        parameters.Add(this.CreateParameter("codice_ipa", datiModifica.codiceIpa));
                    else
                        parameters.Add(this.CreateParameter("codice_ipa", DBNull.Value));
                     */
                    //TELEFONO PRINC
                    if (datiModifica.telefono != null && datiModifica.telefono != "")
                        parameters.Add(this.CreateParameter("telefono", datiModifica.telefono));
                    else
                        parameters.Add(this.CreateParameter("telefono", DBNull.Value));
                    //TELEFONO SEC
                    if (datiModifica.telefono2 != null && datiModifica.telefono2 != "")
                        parameters.Add(this.CreateParameter("telefono2", datiModifica.telefono2));
                    else
                        parameters.Add(this.CreateParameter("telefono2", DBNull.Value));
                    //NOTE
                    if (datiModifica.note != null && datiModifica.note != "")
                        parameters.Add(this.CreateParameter("note", datiModifica.note));
                    else
                        parameters.Add(this.CreateParameter("note", DBNull.Value));
                    //FAX
                    if (datiModifica.fax != null && datiModifica.fax != "")
                        parameters.Add(this.CreateParameter("fax", datiModifica.fax));
                    else
                        parameters.Add(this.CreateParameter("fax", DBNull.Value));
                    //ID_CANALE_PREF
                    /*
                    if (datiModifica.idCanalePref != null)
                        parameters.Add(this.CreateParameter("var_idDocType", datiModifica.idCanalePref));
                    else
                        parameters.Add(this.CreateParameter("var_idDocType", DBNull.Value));
                    */
                    //27-01-2015 In modifica del corrispondente, se non selezionavo il canale preferenziale, sul db settava a null
                    if (!string.IsNullOrEmpty(datiModifica.idCanalePref))
                        parameters.Add(this.CreateParameter("var_idDocType", datiModifica.idCanalePref));
                    else
                    {
                        datiModifica.idCanalePref = this.GetSystemIDCanale(); //imposto lettera di default se vuoto
                        parameters.Add(this.CreateParameter("var_idDocType", datiModifica.idCanalePref));
                    }

                    // Corrispondente proveniente da rubrica comune
                    parameters.Add(this.CreateParameter("inRubricaComune", (datiModifica.inRubricaComune ? "1" : "0")));

                    // TIPO
                    if (!String.IsNullOrEmpty(datiModifica.tipoCorrispondente))
                        parameters.Add(this.CreateParameter("tipourp", datiModifica.tipoCorrispondente));
                    else
                    {
                        string tipourp = GetCorrispondenteBySystemID(datiModifica.idCorrGlobali).tipoCorrispondente;
                        if (string.IsNullOrEmpty(tipourp))
                            parameters.Add(this.CreateParameter("tipourp", DBNull.Value));
                        else
                            parameters.Add(this.CreateParameter("tipourp", tipourp));
                    }
                    //LOCALITA
                    if (datiModifica.localita != null && datiModifica.localita != "")
                        parameters.Add(this.CreateParameter("localita", datiModifica.localita));
                    else
                        parameters.Add(this.CreateParameter("localita", DBNull.Value));

                    //LUOGO DI NASCITA
                    if (datiModifica.luogoNascita != null && datiModifica.luogoNascita != "")
                        parameters.Add(this.CreateParameter("luogoNascita", datiModifica.luogoNascita));
                    else
                        parameters.Add(this.CreateParameter("luogoNascita", DBNull.Value));

                    //DATA DI NASCITA
                    if (datiModifica.dataNascita != null && datiModifica.dataNascita != "")
                        parameters.Add(this.CreateParameter("dataNascita", datiModifica.dataNascita));
                    else
                        parameters.Add(this.CreateParameter("dataNascita", DBNull.Value));

                    //TITOLO CORRISPONDENTE
                    if (datiModifica.titolo != null && datiModifica.titolo != "")
                        parameters.Add(this.CreateParameter("titolo", datiModifica.titolo));
                    else
                        parameters.Add(this.CreateParameter("titolo", DBNull.Value));

                    // Url per l'interoperabilità semplificata (attualmente uno solo)
                    if (datiModifica.Urls != null &&
                        datiModifica.Urls.Count > 0 &&
                        !String.IsNullOrEmpty(datiModifica.Urls[0].Url))
                        parameters.Add(this.CreateParameter("SimpInteropUrl", datiModifica.Urls[0].Url));
                    else
                        parameters.Add(this.CreateParameter("SimpInteropUrl", DBNull.Value));

                    DocsPaUtils.Data.ParameterSP par = this.CreateParameter("IdPeople", Convert.ToInt32(user.idPeople));
                    par.Tipo = DbType.Int32;
                    parameters.Add(par);
                    par = this.CreateParameter("IdGruppoPeople", Convert.ToInt32(user.idGruppo));
                    par.Tipo = DbType.Int32;

                    parameters.Add(par);

                    //REGISTRO
                    if (!string.IsNullOrEmpty(datiModifica.idRegistro))
                    {
                        parameters.Add(this.CreateParameter("idRegistro", Convert.ToInt32(datiModifica.idRegistro)));
                    }
                    else
                    {
                        parameters.Add(this.CreateParameter("idRegistro", DBNull.Value));
                    }
                    //if (!string.IsNullOrEmpty(datiModifica.codice))
                    //{
                    //    int v = 0;
                    //    if (Int32.TryParse(datiModifica.codice, out v))
                    //    {
                    //        parameters.Add(this.CreateParameter("idRegistro", Convert.ToInt32(datiModifica.codice)));
                    //    }
                    //}
                    //else
                    //{
                    //    parameters.Add(this.CreateParameter("idRegistro", DBNull.Value));
                    //}
                    // Parametro di output relativo all'eventuale nuovo corrispondente inserito
                    DocsPaUtils.Data.ParameterSP outParam = new DocsPaUtils.Data.ParameterSP("newId", 0, DocsPaUtils.Data.DirectionParameter.ParamOutput);
                    outParam.Tipo = DbType.Int32;
                    parameters.Add(outParam);

                    retProc = this.ExecuteStoreProcedure("SP_MODIFY_CORR_ESTERNO_IS", parameters);
                    /*
                        VALORI RITORNATI
					
                        0: ERRORE - L'operazione di modifica NON è andata a buon fine
                        1: MODIFICA EFFETTUATA CON SUCCESSO 
                            - il corrispondente è stato modificato con successo			
										
                    */
                    logger.Debug("Chiamata SP 'SP_MODIFY_CORR_ESTERNO_IS'. Esito: " + Convert.ToString(retProc));

                    // Commit / rollback transazione
                    switch (retProc)
                    {
                        /*
                    case 1:
                        this.CommitTransaction();
                        logger.Debug("Eseguita Commit alla Stored Procedure: SP_MODIFY_CORR_ESTERNO_IS - il corrispondente è stato modificato correttamente");
                        retValue = true;
                        message = "OK";

                        if (outParam.Valore != null)
                        {
                            // Reperimento dell'id dell'eventuale nuovo corrispondente inserito
                            newIdCorrGlobali = outParam.Valore.ToString();
                        }
                        break;
                         * */
                        case 10:
                        case 9:
                        case 8:
                        case 7:
                        case 6:
                        case 5:
                        case 4:
                        case 3:
                        case 2:
                        case 100:
                            this.RollbackTransaction();
                            logger.Debug("Eseguita Rollback della Stored Procedure: SP_MODIFY_CORR_ESTERNO_IS - errore durante la modifica di un corrispondente");
                            retValue = false;
                            message = "KO";
                            break;
                        case 0:
                        default:
                            this.CommitTransaction();
                            logger.Debug("Eseguita Commit alla Stored Procedure: SP_MODIFY_CORR_ESTERNO_IS - il corrispondente è stato modificato correttamente");
                            retValue = true;
                            message = "OK";
                            newIdCorrGlobali = retProc.ToString();
                            break;
                    }
                }

            }
            catch (Exception e)
            {
                this.RollbackTransaction();
                retValue = false;
                message = "Errore durante la modifica di un corrispondente" + e.Message;
                logger.Debug(e.Message);
            }

            return retValue;
        }

        private DocsPaUtils.Data.ParameterSP CreateParameter(string name, object value)
        {
            return new DocsPaUtils.Data.ParameterSP(name, value);
        }

        public string GetSystemIDCorr(string codRubrica, string idamm, string idRegistro)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_SYSTEM_ID_DPA_CORR_GLOBALI_PARAM");
                queryMng.setParam("param1", codRubrica.ToUpper());
                queryMng.setParam("param2", idamm);
                if (!string.IsNullOrEmpty(idRegistro))
                {
                    queryMng.setParam("params", "AND ID_REGISTRO=" + idRegistro);
                }
                else
                {
                    queryMng.setParam("params", "");
                }
                //queryMng.setParam("param3", codAOO.ToUpper());

                string commandText = queryMng.getSQL();
                string system_id;
                dbProvider.ExecuteScalar(out system_id, commandText);
                return system_id;
            }
            catch
            {
                return "0";
            }
            finally
            {
                dbProvider.Dispose();

            }
        }

        public string GetSystemIDCanale()
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                string system_id;
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_System_id__DocumentTypes");
                queryMng.setParam("param1", "TYPE_ID = 'LETTERA'");
                string commandText = queryMng.getSQL();
                dbProvider.ExecuteScalar(out system_id, commandText);
                return system_id;
            }
            catch
            {
                return "0";
            }
            finally
            {
                dbProvider.Dispose();

            }
        }


        public string GetSystemIDCanale(string DescrizioneCanale)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                string system_id;
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_System_id__DocumentTypes");
                queryMng.setParam("param1", "upper(TYPE_ID) = '" + DescrizioneCanale.ToUpper() + "'");
                string commandText = queryMng.getSQL();
                dbProvider.ExecuteScalar(out system_id, commandText);
                return system_id;
            }
            catch
            {
                return "0";
            }
            finally
            {
                dbProvider.Dispose();

            }
        }

        public string GetRegistroDaCodice(string codice)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                string system_id = "";
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAElRegistri");
                queryMng.setParam("param1", "SYSTEM_ID");
                queryMng.setParam("param2", "VAR_CODICE = '" + codice + "'");

                string commandText = queryMng.getSQL();
                dbProvider.ExecuteScalar(out system_id, commandText);
                return system_id;
            }
            catch
            {
                return "0";
            }
            finally
            {
                dbProvider.Dispose();

            }
        }

        #endregion

        public ArrayList GetListaCorrispondentiByDescrizione(string id_amm, string descrizione, string tipoIE)
        {

            logger.Debug("getCorrispondente");

            ArrayList corrispondenti = new ArrayList();

            string ie_where = "";

            DocsPaVO.addressbook.TipoUtente tipoUtente = DocsPaVO.addressbook.TipoUtente.GLOBALE;

            if (tipoIE.Equals("E"))
            {

                tipoUtente = DocsPaVO.addressbook.TipoUtente.ESTERNO;

            }

            else
            {

                if (tipoIE.Equals("I"))
                {

                    tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;

                }

            }


            switch (tipoUtente)
            {

                case DocsPaVO.addressbook.TipoUtente.INTERNO:

                    ie_where = " and cha_tipo_ie='I'";

                    break;

                case DocsPaVO.addressbook.TipoUtente.ESTERNO:

                    ie_where = " and cha_tipo_ie='E'";

                    break;

                case DocsPaVO.addressbook.TipoUtente.GLOBALE:

                    ie_where = "";

                    break;

            }

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");

            q.setParam("param1", "SYSTEM_ID,CHA_TIPO_URP,ID_GRUPPO,ID_PEOPLE,CHA_TIPO_IE");

            q.setParam("param2", "UPPER(VAR_DESC_CORR) LIKE '%" + descrizione.ToUpper().Replace("'", "''") + "%'" + ie_where);

            logger.Debug(q.getSQL());

            DataSet ds = new DataSet();

            try
            {

                if (this.ExecuteQuery(out ds, "corrispondenti", q.getSQL()))
                {

                    DocsPaVO.utente.Corrispondente corrispondente = new DocsPaVO.utente.Corrispondente();


                    if (ds.Tables["corrispondenti"].Rows.Count >= 1)
                    {

                        foreach (DataRow corrRow in ds.Tables["corrispondenti"].Rows)
                        {

                            string sid = corrRow["SYSTEM_ID"].ToString();

                            string tipo = corrRow["CHA_TIPO_URP"].ToString();

                            string id_gruppo = corrRow["ID_GRUPPO"].ToString();

                            string id_people = corrRow["ID_PEOPLE"].ToString();

                            bool isInterno = (corrRow["CHA_TIPO_IE"].ToString() == "I");

                            DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();

                            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();

                            qco.systemId = sid;

                            qco.idAmministrazione = id_amm;
                            qco.fineValidita = true;


                            if (isInterno)
                            {

                                corrispondente = u.GetCorrispondenteInt(qco);

                            }

                            else
                            {

                                corrispondente = u.GetCorrispondenteEst(qco);

                            }

                            corrispondenti.Add(corrispondente);
                        }

                    }

                }

            }

            catch (Exception e)
            {

                logger.Debug("Errore nella ricerca di uno specifico corrispondente (" + descrizione + ")", e);

                return null;

            }

            return corrispondenti;

        }

        /// <summary>
        /// Rimozione del record nella tabella UserLogin corrispondente al dst fornito
        /// </summary>
        /// <param name="dst"></param>
        /// <returns></returns>
        public bool RemoveUserLoginLock(string dst)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("REMOVE_USER_LOGIN_LOCK");
            queryDef.setParam("dst", dst);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            int rowsAffected;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

            return (rowsAffected > 0);
        }

        /// <summary>
        /// Ritorna la lista di registri e di RF del ruolo
        /// Se all == "1" vengono ritornati solo gli RF, in particolare solamente quegli RF che sono collegati al registro passato
        /// in ingresso (idAooColl)
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public ArrayList GetListaRegistriRfRuolo(string idRuolo, string all, string idAooColl)
        {

            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Utenti> GetListaRegistriRfRuolo");

            ArrayList listaRegRf = new ArrayList();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_L_RUOLO_REG_REGISTRI_E_RF");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_OPEN", false) + " AS DTA_OPEN, " +
            DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CLOSE", false) + " AS DTA_CLOSE, " +
            DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_ULTIMO_PROTO", false) + " AS DTA_ULTIMO_PROTO " + ", A.ID_RUOLO_AOO,A.ID_RUOLO_RESP,A.ID_PEOPLE_AOO, A.CHA_AUTO_INTEROP, A.CHA_RF, A.CHA_DISABILITATO, A.ID_AOO_COLLEGATA, A.INVIO_RICEVUTA_MANUALE, A.VAR_PREG");
            q.setParam("param2", "ID_RUOLO_IN_UO=" + idRuolo);

            if (all != null && all != "")
            {
                if (idAooColl != null && idAooColl != "" && all.Equals("1"))
                {
                    //prende solo gli RF collegati a un determinato registro
                    q.setParam("param4", " AND CHA_RF = '" + all + "' AND ID_AOO_COLLEGATA = " + idAooColl);
                }
                else
                {
                    //prende solo i registri associati ad un determinato ruolo
                    q.setParam("param4", " AND CHA_RF = '" + all + "'");
                }
            }
            else
            {
                q.setParam("param4", "");
            }

            //Laura 22 Febbraio 2013
            if (dbType.ToUpper() == "SQL")
                q.setParam("param3", "ISNULL(B.CHA_PREFERITO, 0) desc, A.VAR_CODICE");
            else
                q.setParam("param3", "B.CHA_PREFERITO DESC NULLS LAST, A.VAR_CODICE");
            string sql = q.getSQL();
            logger.Debug(sql);

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, sql);
            if (dataSet != null)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro();

                    reg.systemId = row[0].ToString();
                    reg.codRegistro = row[1].ToString();
                    reg.codice = row[2].ToString();
                    reg.descrizione = row[3].ToString();
                    reg.email = row[4].ToString();
                    reg.stato = row[5].ToString();
                    reg.idAmministrazione = row[6].ToString();
                    reg.codAmministrazione = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                    reg.dataApertura = row[7].ToString().Trim();
                    reg.dataChiusura = row[8].ToString().Trim();
                    reg.dataUltimoProtocollo = row[9].ToString();
                    reg.idRuoloAOO = row[10].ToString();
                    reg.idRuoloResp = row[11].ToString();
                    reg.idUtenteAOO = row[12].ToString();
                    reg.autoInterop = row[13].ToString();
                    reg.chaRF = row[14].ToString();
                    if (row[15] == null || row[15].ToString().Equals(""))
                        reg.rfDisabled = "0";
                    else
                        reg.rfDisabled = row[15].ToString();
                    if (row[15] != null && !row[15].ToString().Equals("1"))
                        reg.Sospeso = false;
                    else
                        reg.Sospeso = true;
                    reg.idAOOCollegata = row[16].ToString();
                    reg.invioRicevutaManuale = row[17].ToString();
                    if (row[18].ToString().Equals("1"))
                        reg.flag_pregresso = true;
                    else
                        reg.flag_pregresso = false;
                    listaRegRf.Add(reg);
                }
            }

            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Utenti> GetListaRegistriRfRuolo");
            return listaRegRf;
        }

        public void GetRegistroDaPec(string idProfile, ref DocsPaVO.utente.Registro reg)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REGISTRO_DA_PEC");
            q.setParam("param1", "E.SYSTEM_ID, E.VAR_CODICE, E.NUM_RIF, E.VAR_DESC_REGISTRO, " +
                "E.VAR_EMAIL_REGISTRO, E.CHA_STATO, E.ID_AMM, " +
                DocsPaDbManagement.Functions.Functions.ToChar("E.DTA_OPEN", false) + "," + //DocsPaWS.Utils.dbControl.toChar("DTA_OPEN",false) + 
                DocsPaDbManagement.Functions.Functions.ToChar("E.DTA_CLOSE", false) + "," + //DocsPaWS.Utils.dbControl.toChar("DTA_CLOSE",false) + 
                DocsPaDbManagement.Functions.Functions.ToChar("E.DTA_ULTIMO_PROTO", false) + " , E.ID_RUOLO_AOO,E.ID_RUOLO_RESP,E.ID_PEOPLE_AOO, E.CHA_AUTO_INTEROP, E.CHA_RF, E.CHA_DISABILITATO, E.ID_AOO_COLLEGATA, E.INVIO_RICEVUTA_MANUALE, E.VAR_PREG, E.ANNO_PREG, E.DIRITTO_RUOLO_AOO "); //DocsPaWS.Utils.dbControl.toChar("DTA_ULTIMO_PROTO",false));
            q.setParam("idProfile", idProfile);
            string sql = q.getSQL();
            logger.Debug(sql);
            DataSet dataSet;
            this.ExecuteQuery(out dataSet, sql);
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                reg = new DocsPaVO.utente.Registro();
                reg.systemId = dataRow[0].ToString();
                reg.codRegistro = dataRow[1].ToString();
                reg.codice = dataRow[2].ToString();
                reg.descrizione = dataRow[3].ToString();
                reg.email = dataRow[4].ToString();
                reg.stato = dataRow[5].ToString();
                reg.idAmministrazione = dataRow[6].ToString();
                reg.codAmministrazione = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                reg.dataApertura = dataRow[7].ToString();
                reg.dataChiusura = dataRow[8].ToString();
                reg.dataUltimoProtocollo = dataRow[9].ToString();
                reg.idRuoloAOO = dataRow[10].ToString();
                reg.idRuoloResp = dataRow[11].ToString();
                reg.idUtenteAOO = dataRow[12].ToString();
                reg.autoInterop = dataRow[13].ToString();
                reg.chaRF = dataRow[14].ToString();
                reg.rfDisabled = dataRow[15].ToString();
                reg.idAOOCollegata = dataRow[16].ToString();
                reg.invioRicevutaManuale = dataRow[17].ToString();
                if (dataRow[18].ToString().Equals("1"))
                    reg.flag_pregresso = true;
                else
                    reg.flag_pregresso = false;
                reg.anno_pregresso = dataRow[19].ToString();
                reg.Diritto_Ruolo_AOO = dataRow[20].ToString();
            }
            dataSet.Dispose();
        }

        //EM
        /// <summary>
        /// Ritorna la lista degli utenti per ruolo.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="idRuolo"></param>
        public ArrayList GetListaUtentiByIdRuolo(string idRuolo)
        {

            ArrayList utenti = new ArrayList();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_UTENTI_BY_GROUPS_SYSTEM_ID");

            q.setParam("param1", idRuolo);
            string sql = q.getSQL();
            logger.Debug(sql);
            DataSet dataSet;
            this.ExecuteQuery(out dataSet, sql);
            if (dataSet != null)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    DocsPaVO.utente.Utente ut = new DocsPaVO.utente.Utente();
                    ut.idPeople = row[0].ToString();
                    ut.userId = row[1].ToString();
                    ut.descrizione = row[2].ToString() + " (" + row[1].ToString() + ")";
                    ut.matricola = (row["Matricola"] != DBNull.Value ? row["Matricola"].ToString() : string.Empty);
                    utenti.Add(ut);
                }
            }

            return utenti;
        }

        /// <summary>
        /// Query su dpa_corr_globali quando bisogna estrarre un solo dato
        /// </summary>
        /// <param name="campo"></param>
        /// <param name="condizione"></param>
        /// <returns>Ritorna un solo campo di testo</returns>
        public string GetFromCorrGlobGeneric(string campo, string condizione)
        {
            string retValue;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_CORR_GLOB_GENERIC");
            q.setParam("param1", campo);
            q.setParam("param2", condizione);
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteScalar(out retValue, sql);

            return retValue;
        }

        public string GetCssAmministrazione(string idAmm)
        {
            string result = string.Empty;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_CSS_AMMINISTRA");
            q.setParam("idAmm", idAmm);
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteScalar(out result, sql);

            return result;
        }

        public string GetSegnAmm(string idAmm)
        {
            string result = "0";
            string res = string.Empty;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SEGN_AMMINISTRA");
            q.setParam("idAmm", idAmm);
            string sql = q.getSQL();
            logger.Debug(sql);
            DataSet dataSet;
            this.ExecuteQuery(out dataSet, sql);
            if (dataSet != null)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    if (!string.IsNullOrEmpty(row[0].ToString()) && Convert.ToInt16(row[0].ToString()) > 0)
                        result = row[0].ToString();
                }
            }
            return result;
        }

        #region Gestione corrispondente proveniente da rubrica comune

        /// <summary>
        /// Reperimento, dal codice rubrica, dell'id del corrispondente proveniente da rubrica comune e inserito in docspa
        /// </summary>
        /// <param name="codiceRubrica"></param>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public string GetIdCorrispondenteDaRubricaComune(string codiceRubrica, string idAmministrazione)
        {
            string id = string.Empty;

            using (DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                queryDef.setParam("param1", "A.SYSTEM_ID");

                string param2Value = string.Format("UPPER(A.VAR_COD_RUBRICA)='{0}' AND A.CHA_TIPO_CORR = 'C'", codiceRubrica.ToUpper());
                if (!string.IsNullOrEmpty(idAmministrazione))
                    param2Value = string.Concat(param2Value, string.Format(" AND A.ID_AMM = {0}", idAmministrazione));

                queryDef.setParam("param2", param2Value);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                dbProvider.ExecuteScalar(out id, commandText);
            }

            return id;
        }

        /// <summary>
        /// Verifica se, in docspa, è presente il corrispondente richiesto da rubrica comune
        /// </summary>
        /// <param name="codiceRubrica"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ContainsCorrispondenteDaRubricaComune(string codiceRubrica, string idAmministrazione, out string id)
        {
            id = GetIdCorrispondenteDaRubricaComune(codiceRubrica, idAmministrazione);

            return (!string.IsNullOrEmpty(id));
        }

        #endregion

        public DocsPaVO.utente.Corrispondente GetCorrispondenteByIdPeople(string id_amm, string idPeople, DocsPaVO.addressbook.TipoUtente tipoIE)
        {
            logger.Debug("getCorrispondenteByIdPeople");

            string ie_where = "";
            switch (tipoIE)
            {
                case DocsPaVO.addressbook.TipoUtente.INTERNO:
                    ie_where = " and cha_tipo_ie='I'";
                    break;
                case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                    ie_where = " and cha_tipo_ie='E'";
                    break;
                case DocsPaVO.addressbook.TipoUtente.GLOBALE:
                    ie_where = "";
                    break;
            }

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
            q.setParam("param1", "SYSTEM_ID,CHA_TIPO_URP,ID_GRUPPO,ID_PEOPLE,CHA_TIPO_IE");
            q.setParam("param2", "id_people=" + idPeople + ie_where + " AND DTA_FINE IS  NULL ");
            logger.Debug(q.getSQL());
            DataSet ds = new DataSet();

            try
            {
                if (this.ExecuteQuery(out ds, "corrispondenti", q.getSQL()))
                {
                    DocsPaVO.utente.Corrispondente corrispondente = new DocsPaVO.utente.Corrispondente();
                    if (ds.Tables["corrispondenti"].Rows.Count >= 1)
                    {
                        DataRow dr = ds.Tables["corrispondenti"].Rows[0];
                        string sid = (string)dr["SYSTEM_ID"].ToString();
                        string tipo = (string)dr["CHA_TIPO_URP"].ToString();
                        string id_gruppo = (string)dr["ID_GRUPPO"].ToString();
                        string id_people = (string)dr["ID_PEOPLE"].ToString();

                        bool isInterno = (dr["CHA_TIPO_IE"].ToString() == "I");

                        DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                        DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                        qco.systemId = sid;
                        qco.idAmministrazione = id_amm;
                        qco.fineValidita = true;

                        if (isInterno)
                            return u.GetCorrispondenteInt(qco);
                        else
                            return u.GetCorrispondenteEst(qco);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella ricerca di uno specifico corrispondente (" + idPeople + ")", e);
                return null;
            }
            return null;
        }

        public string getRuoloRespUoFromUo(string idCorrGlobaliUo, string tipoRuolo, string idCorr)
        {
            ArrayList parameters = new ArrayList();
            string idCorrGlobali_ruolo = String.Empty;
            DBProvider dbProvider = new DBProvider();
            try
            {
                parameters.Add(this.CreateParameter("id_UO", Convert.ToInt32(idCorrGlobaliUo)));
                parameters.Add(this.CreateParameter("tipo_ruolo", tipoRuolo));
                parameters.Add(this.CreateParameter("id_corr", idCorr));
                DocsPaUtils.Data.ParameterSP versionIdParam = new DocsPaUtils.Data.ParameterSP("result", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                parameters.Add(versionIdParam);
                DataSet ds = null;
                dbProvider.ExecuteStoredProcedure("SP_GET_RUOLO_RESP_UO_FROM_UO", parameters, ds);
                idCorrGlobali_ruolo = Convert.ToString(versionIdParam.Valore);
            }
            catch (Exception e)
            {
                logger.Debug("Errore nel richiamare la SP_GET_RUOLO_RESP_UO_FROM_UO", e);
                return "-1";
            }
            finally
            {
                dbProvider.Dispose();
            }
            return idCorrGlobali_ruolo;
        }

        public string getCodiceCorrispondente(string docnumber, string idOggettoCustom, string idTemplate)
        {

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("GETCODICECORRISPONDENTE");
                queryDef.setParam("IDTEMPLATE", idTemplate);
                queryDef.setParam("IDOGGETTO", idOggettoCustom);
                queryDef.setParam("DOCNUMBER", docnumber);
                logger.Debug(queryDef.getSQL());
                string commandText = queryDef.getSQL();

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                            return reader.GetString(reader.GetOrdinal("var_cod_rubrica"));

                        return string.Empty;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("errore nella query: " + e.Message);
                return string.Empty;
            }


        }


        public void invalidaCorr(string dataFine, string corrSystemId, string cod_rubrica)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Utenti> invalidaCorr");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlobaliStor");

            q.setParam("param1", dataFine);
            q.setParam("param2", "'" + cod_rubrica + "'");
            //q.setParam("param2",cod_rubrica);
            q.setParam("param3", "'" + cod_rubrica + "'");
            q.setParam("param4", corrSystemId);

            string insertString = q.getSQL();

            logger.Debug(insertString);
            int rowsAffected;
            this.ExecuteNonQuery(insertString, out rowsAffected);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Utenti> invalidaCorr");
            if (rowsAffected == 0)
            {
                throw new Exception();
            }

        }

        /// <summary>
        /// Dato l'id di un corrispondente resetta il campo VAR_INSERT_INTEROP ( Iacopino - 4/11/2011 )
        /// </summary>
        public int resetCorrVarInsertIterop(string corrSystemId, string val)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Utenti> resetCorrVarInsertIterop");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlobaliReset");

            q.setParam("param1", val);
            q.setParam("param2", corrSystemId);

            string resetString = q.getSQL();

            logger.Debug(resetString);
            int rowsAffected;
            this.ExecuteNonQuery(resetString, out rowsAffected);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Utenti> resetCorrVarInsertIterop");
            if (rowsAffected == 0)
            {
                throw new Exception();
            }
            return rowsAffected;

        }

        /// <summary>
        /// Dato l'id di un corrispondente resetta il campo Cod_Rubrica ( Iacopino - 12/11/2011 )
        /// </summary>
        public int resetCodRubCorrIterop(string corrSystemId, string val)
        {
            logger.Debug("START : DocsPaDB > Query_DocsPAWS > Utenti> resetCodRubCorrIterop");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlobaliResetCodRubrica");

            q.setParam("param1", val.Replace("'", "''"));
            q.setParam("param2", corrSystemId);

            string resetString = q.getSQL();

            logger.Debug(resetString);
            int rowsAffected;
            this.ExecuteNonQuery(resetString, out rowsAffected);
            logger.Debug("END : DocsPaDB > Query_DocsPAWS > Utenti> resetCodRubCorrIterop");
            if (rowsAffected == 0)
            {
                throw new Exception();
            }
            return rowsAffected;

        }

        /// <summary>
        /// Dato l'id di un ruolo restituisce tutti gli utenti appartenenti al ruolo (Fabio)
        /// </summary>
        public DataSet getUtentiInRuoloSottoposto(DocsPaVO.utente.InfoUtente infoUtente, string idRuolo)
        {
            DataSet ds = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_UTENTI_RUOLO_SOTTOPOSTO");
            q.setParam("idRuolo", idRuolo);

            string commandText = q.getSQL();
            logger.Debug(commandText);
            this.ExecuteQuery(ds, "UTENTI_RUOLO", commandText);

            return ds;
        }

        public DocsPaVO.utente.Corrispondente getDettagliIndirizzoCorrispondente(string systemId)
        {
            DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
            try
            {

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPADettGlob6");
                q.setParam("param1", "ID_CORR_GLOBALI= " + systemId);
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataSet;
                this.ExecuteQuery(out dataSet, queryString);
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    corr.indirizzo = row[0].ToString();
                    corr.cap = row[1].ToString();
                    corr.prov = row[2].ToString();
                    corr.citta = row[3].ToString();
                    corr.localita = row[4].ToString();
                    corr.telefono1 = row[5].ToString();
                    corr.telefono2 = row[6].ToString();
                    corr.note = row[7].ToString();
                }
                dataSet.Dispose();

            }
            catch (Exception e)
            {
                logger.Debug("errore nella query di ricerca dettagli indirizzo corrisponte con system-id: " + systemId);

            }
            return corr;
        }

        public List<string> getListaEmailUtentiAmm(string idAmm)
        {
            List<string> lista = new List<string>();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_LISTA_MAIL_UTENTI");
                q.setParam("idAmm", idAmm);
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataSet;
                this.ExecuteQuery(out dataSet, queryString);
                if (dataSet != null)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        string email = row[0].ToString();
                        lista.Add(email);
                    }
                }
                return lista;
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella ricerca delle email degli utenti dell'amministrazione + " + idAmm, e);
                return null;
            }
        }

        //metodo per il recupero dei titoli dei corrispondenti da visualizzare in rubrica
        public ArrayList getListaTitoli()
        {
            ArrayList listaTitoli = new ArrayList();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_LISTA_TITOLI");
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataSet;
                this.ExecuteQuery(out dataSet, queryString);
                if (dataSet != null)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        string titolo = row[0].ToString();
                        listaTitoli.Add(titolo);
                    }
                }

            }
            catch (Exception e)
            {
                logger.Debug("Errore nella ricerca dei titoli dei corrispondenti ", e);
            }
            return listaTitoli;
        }

        public void GetListaDirittiSemplificataRuoli(out DataSet dataSet, string idObj)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_SECURITY_PROFILE_SEMPL");
            q.setParam("param1", idObj);
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteQuery(out dataSet, "DIRITTI_RUOLI", sql);
        }

        public void GetListaDirittiSemplificataRuoliWithFilter(out DataSet dataSet, string idObj, DocsPaVO.filtri.FilterVisibility[] filters, DocsPaVO.utente.InfoUtente infoUtente) //, out bool proprietario)
        {
            string query1 = string.Empty;
            string query2 = string.Empty;

            if (filters != null && filters.Length > 0)
            {
                foreach (DocsPaVO.filtri.FilterVisibility filter in filters)
                {
                    if (filter != null)
                    {
                        if (filter.Type == DocsPaVO.filtri.TypeFilterVisibility.USER)
                        {
                            query1 += " and (b.personorgroup in (select x.GROUPS_SYSTEM_ID from peoplegroups x where x.PEOPLE_SYSTEM_ID in (select y.id_people from dpa_corr_globali y where y.system_id = " + filter.Value + ")) or a.system_id= " + filter.Value + ")";
                            query2 += " and (d.personorgroup in (select x.GROUPS_SYSTEM_ID from peoplegroups x where x.PEOPLE_SYSTEM_ID in (select y.id_people from dpa_corr_globali y where y.system_id = " + filter.Value + ")) or a.system_id= " + filter.Value + ")";
                        }

                        if (filter.Type == DocsPaVO.filtri.TypeFilterVisibility.ROLE)
                        {
                            query1 += " and a.system_id = " + filter.Value;
                            query2 += " and a.system_id = " + filter.Value;
                        }

                        if (filter.Type == DocsPaVO.filtri.TypeFilterVisibility.DATE)
                        {
                            query1 += " AND b.ts_inserimento >=" + DocsPaDbManagement.Functions.Functions.ToDate(filter.Value + " 00:00:00") +
                                " AND b.ts_inserimento <=" + DocsPaDbManagement.Functions.Functions.ToDate(filter.Value + " 23:59:59");
                            query2 += " AND  d.dta_revoca >=" + DocsPaDbManagement.Functions.Functions.ToDate(filter.Value + " 00:00:00") +
                                " AND  d.dta_revoca <=" + DocsPaDbManagement.Functions.Functions.ToDate(filter.Value + " 23:59:59");
                        }


                        if (filter.Type == DocsPaVO.filtri.TypeFilterVisibility.DATE_FROM)
                        {
                            query1 += " AND b.ts_inserimento >='" + DocsPaDbManagement.Functions.Functions.ToDate(filter.Value + " 00:00:00");
                            query2 += " AND d.dta_revoca >='" + DocsPaDbManagement.Functions.Functions.ToDate(filter.Value + " 00:00:00");
                        }

                        if (filter.Type == DocsPaVO.filtri.TypeFilterVisibility.DATE_TO)
                        {
                            query1 += " AND b.ts_inserimento <=" + DocsPaDbManagement.Functions.Functions.ToDate(filter.Value + " 23:59:59");
                            query2 += " AND d.dta_revoca <=" + DocsPaDbManagement.Functions.Functions.ToDate(filter.Value + " 23:59:59");
                        }

                        if (filter.Type == DocsPaVO.filtri.TypeFilterVisibility.DATE_WEEK)
                        {
                            query1 += " and b.ts_inserimento between trunc(sysdate,'WW') and sysdate ";
                            query2 += " and d.dta_revoca between trunc(sysdate,'WW') and sysdate ";
                        }

                        if (filter.Type == DocsPaVO.filtri.TypeFilterVisibility.DATE_MONTH)
                        {
                            query1 += " and b.ts_inserimento between trunc(sysdate,'MM') and sysdate ";
                            query2 += " and d.dta_revoca between trunc(sysdate,'MM') and sysdate ";
                        }

                        if (filter.Type == DocsPaVO.filtri.TypeFilterVisibility.TYPE)
                        {
                            if (filter.Value.Equals("R"))
                            {
                                query1 += " AND (b.accessrights = 45 OR b.accessrights = 20)";
                                query2 += " AND (d.accessrights = 45 OR d.accessrights = 20)";
                            }
                            else
                            {
                                query1 += " AND b.accessrights IN (255,63,0)";
                                query2 += " AND d.accessrights IN (255,63,0)";
                            }
                        }

                        if (filter.Type == DocsPaVO.filtri.TypeFilterVisibility.CAUSE)
                        {
                            switch (filter.Value)
                            {
                                case "UP":
                                    query1 += " AND a.cha_tipo_urp ='P' and b.cha_tipo_diritto = 'P'";
                                    query2 += " AND a.cha_tipo_urp ='P' and d.cha_tipo_diritto = 'P'";
                                    break;
                                case "RP":
                                    query1 += " AND a.cha_tipo_urp ='R' and b.cha_tipo_diritto = 'P'";
                                    query2 += " AND a.cha_tipo_urp ='R' and d.cha_tipo_diritto = 'P'";
                                    break;
                                case "AC":
                                    query1 += " AND b.cha_tipo_diritto ='A' AND b.Accessrights = 20";
                                    query2 += " AND d.cha_tipo_diritto ='A' AND d.Accessrights = 20";
                                    break;
                                case "A":
                                    query1 += " AND b.cha_tipo_diritto ='A' AND b.Accessrights <> 20";
                                    query2 += " AND d.cha_tipo_diritto ='A' AND d.Accessrights <> 20";
                                    break;
                                default:
                                    query1 += " AND b.cha_tipo_diritto ='" + filter.Value + "'";
                                    query2 += " AND d.cha_tipo_diritto ='" + filter.Value + "'";
                                    break;
                            }
                        }
                    }
                }
            }


            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_SECURITY_PROFILE_SEMPL_WITH_FILTERS");

            //Laura 22 Febbraio 2013
            string UserDB = string.Empty;

            if (dbType.ToUpper() == "SQL")
            {
                UserDB = getUserDB();
                q.setParam("dbUser", UserDB);
            }


            q.setParam("idProfile", idObj);
            q.setParam("query1", query1);
            q.setParam("query2", query2);
            q.setParam("idAmm", infoUtente.idAmministrazione);
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteQuery(out dataSet, "DIRITTI_RUOLI", sql);
        }

        public void GetListaRuoliSemplificato(out DataSet dataSet, string idObj, string IDAMM)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_SECURITY_PROFILE_SEMPL_PEOPLE");
            q.setParam("param1", idObj);
            q.setParam("param2", IDAMM);
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteQuery(out dataSet, "DIRITTI_UTENTI", sql);
        }

        public void GetListaDiritti_Deleted_Semplificato(out DataSet dataSet, string idObj)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_SECURITY_PROFILE_SEMPL_RUOLI_DELETE");
            q.setParam("param1", idObj);
            string sql = q.getSQL();
            logger.Debug(sql);
            this.ExecuteQuery(out dataSet, "DIRITTI_RUOLI_DELETED", sql);
        }

        public void GetRuoliUtente(DataSet dataSet, DataRow row, string maxLivello, string idPeople, bool ruoliAbilitati)
        {
            logger.Debug("start > GetRuoliUtente");
            /*string commandString4="SELECT * FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND DTA_FINE IS NULL AND NUM_LIVELLO <=" + maxLivello;
            database.fillTable(commandString4,dataSet,"UO");*/



            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobaliRuolo");
            // old q.setParam("param2", " AND A.NUM_LIVELLO <=" + maxLivello);

            if (dbType.ToUpper().Equals("ORACLE"))
                if (!ruoliAbilitati)
                    q.setParam("param2", " AND A.DTA_FINE IS NULL CONNECT BY PRIOR id_parent = system_id START WITH system_id =" + row["ID_UO"].ToString());
                else
                    q.setParam("param2", "  CONNECT BY PRIOR id_parent = system_id START WITH system_id =" + row["ID_UO"].ToString());
            else if (dbType.ToUpper().Equals("SQL"))
            {
                q.setParam("param2", row["ID_UO"].ToString());
                if (!ruoliAbilitati)
                {
                    q.setParam("paramdtaFine1", " AND cgbase.dta_fine IS NULL ");
                    q.setParam("paramdtafine2", " AND cg.dta_fine IS NULL ");
                }
                else
                {
                    q.setParam("paramdtaFine1", "");
                    q.setParam("paramdtafine2", "");
                }

            }

            string sql = q.getSQL();


            logger.Debug(sql);
            //db.fillTable(commandString3,dataSet,"RUOLI");
            this.ExecuteQuery(dataSet, "UO", sql);
        }

        #region Gestione storico modifiche ruolo

        /// <summary>
        /// Metodo per il recupero della storia delle modifiche apportate ad un ruolo
        /// </summary>
        /// <param name="idCorrGlobali">Id del ruolo di cui si vuole conoscere la storia</param>
        /// <returns>Lista delle istantanee della vista del ruolo</returns>
        public List<DocsPaVO.utente.RoleHistoryItem> GetRoleHistory(String idCorrGlobali)
        {
            List<DocsPaVO.utente.RoleHistoryItem> history = new List<DocsPaVO.utente.RoleHistoryItem>();
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_GET_ROLE_HISTORY");
                query.setParam("idCorrGlobali", idCorrGlobali);
                query.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                using (IDataReader reader = dbProvider.ExecuteReader(query.getSQL()))
                {
                    // Costruzione della storia del ruolo
                    while (reader.Read())
                        history.Add(new DocsPaVO.utente.RoleHistoryItem()
                        {
                            HistoryAction = (DocsPaVO.utente.RoleHistoryItem.AdmittedHistoryAction)Enum.Parse(typeof(DocsPaVO.utente.RoleHistoryItem.AdmittedHistoryAction), reader["Action"].ToString()),
                            ActionDate = DateTime.Parse(reader["action_date"].ToString()),
                            OriginalCorrId = Convert.ToInt32(reader["original_corr_id"]),
                            RoleDescription = reader["role_description"].ToString(),
                            RoleTypeDescription = reader["role_type_description_"].ToString(),
                            UoDescription = reader["uo_description_"].ToString(),
                            RoleCorrGlobId = reader["role_id"].ToString()
                        });


                }

            }

            // Restituzione storia del ruolo
            return history;

        }

        #endregion

        /// <summary>
        /// Metodo per il recupero dell'id UO di un ruolo a partire dall'id
        /// corr globali del ruolo
        /// </summary>
        /// <param name="idCorrGlobRole">Id corr globali del ruolo di cui recuperare l'id UO</param>
        /// <returns>Id della UO del ruolo</returns>
        /// <exception cref="ApplicationException">Sollevata nel caso in cui non venga rilevato codice UO per il ruolo</exception>
        public String GetUoIdFromRoleId(String idCorrGlobRole)
        {
            String retVal = String.Empty;
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_GET_UO_ID_FROM_ROLE_CORR_GLOB");
                query.setParam("idCorrGlobRole", idCorrGlobRole);

                if (!dbProvider.ExecuteScalar(out retVal, query.getSQL()))
                    throw new ApplicationException();
            }

            return retVal;
        }

        /// <summary>
        /// Metodo per verificare se un dato ruolo è disabilitato
        /// </summary>
        /// <param name="authorCorrGlob">Id group del ruolo da verificare</param>
        /// <returns>True se il ruolo è disabilitato</returns>
        public bool CheckIfRoleDisabled(String authorCorrGlob)
        {
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("CHECK_IF_ROLE_DISABLED");
                query.setParam("authorCorrGlob", authorCorrGlob);

                bool result = false;
                IDataReader reader = dbProvider.ExecuteReader(query.getSQL());
                while (reader.Read())
                    result |= reader[0] != null && !String.IsNullOrEmpty(reader[0].ToString());

                return result;
            }
        }

        public string GetRoleTypeId(String idGroup)
        {
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("GET_ROLE_TYPE_ID");
                query.setParam("idGroup", idGroup);

                String result = String.Empty;
                IDataReader reader = dbProvider.ExecuteReader(query.getSQL());
                while (reader.Read())
                    result = reader[0].ToString();

                return result;
            }

        }

        /// <summary>
        /// Metodo per il recupero della lista degli id corr globali relativi alla
        /// catena di storicizzazione di un ruolo
        /// </summary>
        /// <param name="idCorrGloRole">Id del ruolo di cui ricostruire la catena di id</param>
        /// <returns>Lista degli id dei ruoli della catena</returns>
        public List<String> GetRoleChain(String idCorrGloRole)
        {
            List<String> retVal = new List<string>();
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_GET_ROLE_CHAIN_ID_CORR_GLOBALI");
                query.setParam("idCorrGlob", idCorrGloRole);

                IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL());

                while (dataReader.Read())
                    retVal.Add(dataReader[0].ToString());

            }

            return retVal;
        }

        /// <summary>
        /// Metodo per il recupero di informaizoni minimali sugli utenti di un ruolo
        /// </summary>
        /// <param name="roleId">Id gruppo del ruolo di cui recuperare gli utenti</param>
        /// <returns>Lista con le informazioni di base sugli utenti del ruolo specificato</returns>
        public List<UserMinimalInfo> GetUsersInRoleMinimalInfo(String roleId)
        {
            List<UserMinimalInfo> users = new List<UserMinimalInfo>();

            Query query = InitQuery.getInstance().getQuery("S_USER_MIN_INFO");
            query.setParam("idGroup", roleId);

            using (DBProvider dbProvider = new DBProvider())
            {
                IDataReader reader = dbProvider.ExecuteReader(query.getSQL());
                while (reader.Read())
                    users.Add(new UserMinimalInfo() { SystemId = reader["system_id"].ToString(), Description = reader["full_name"].ToString() });
            }

            return users;

        }

        /// <summary>
        /// Metodo per il recupero di tutti i corrispondenti per una data mail e registro
        /// </summary>
        /// <param name="email"></param>
        /// <param name="idRegistri"></param>
        /// <returns></returns>

        public void GetCorrByEmail(string email, string idRegistri, string idAmm, out DataSet ds)
        {
            ds = new DataSet();

            Query query = InitQuery.getInstance().getQuery("S_RICERCA_CORR_GLOBAL_BY_EMAIL");
            query.setParam("param1", email);
            query.setParam("param2", idRegistri);
            if (!string.IsNullOrEmpty(idAmm))
                query.setParam("param3", " AND ID_AMM= " + idAmm);
            else
                query.setParam("param3", string.Empty);
            string sql = query.getSQL();
            logger.Debug(sql);

            this.ExecuteQuery(out ds, "CorrispondentiByMail", sql);
        }

        /// <summary>
        /// Metodo per il recupero di tutti i corrispondenti per una data mail e registro
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="descr">Descrizione</param>
        /// <param name="idRegistri">Registri</param>
        /// <returns></returns>

        public void GetCorrByEmailAndDescr(string email, string descr, string idRegistri, string idAmm, out DataSet ds)
        {
            ds = new DataSet();

            Query query = InitQuery.getInstance().getQuery("S_RICERCA_CORR_GLOBAL_BY_EMAIL_AND_DESCR");
            query.setParam("param1", email);
            query.setParam("param2", descr.ToUpper().Replace("'", "''"));
            query.setParam("param3", idRegistri);
            if (!string.IsNullOrEmpty(idAmm))
                query.setParam("param4", " AND ID_AMM= " + idAmm);
            else
                query.setParam("param4", string.Empty);
            string sql = query.getSQL();
            logger.Debug(sql);

            this.ExecuteQuery(out ds, "CorrispondentiByMailAndDescr", sql);
        }

        /// <summary>
        /// Verifica se esiste almeno un corrispondente nell'amministrazione del corrispondente passato in input al metodo
        /// </summary>
        /// <param name="codiceCorr"></param>
        /// <param name="idAmm"></param>
        /// <param name="codiceAmm"></param>
        /// <returns></returns>
        public bool GetNumCorrInAMM(string codiceCorr, string idAmm, string codiceAmm)
        {
            string countCorr = string.Empty;
            Query query = InitQuery.getInstance().getQuery("S_COUNT_NUM_CORR_IN_AMM");
            query.setParam("codiceCorr", "'" + codiceCorr + "'");
            query.setParam("idAmm", idAmm);
            query.setParam("codiceAmm", "'" + codiceAmm + "'");
            try
            {
                string sql = query.getSQL();
                logger.Debug(sql);
                this.ExecuteScalar(out countCorr, sql);
                if (string.IsNullOrEmpty(countCorr) || countCorr.Equals("0"))
                    return false;
                else
                    return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Metodo per aggiornate il mittente dei documenti di un corrispondente creato da interop
        /// </summary>
        /// <param name="oldCorrId">Vecchio Id mittente</param>
        /// <param name="newCorrId">Nuovo Id mittente</param>
        /// <param name="rows">Righe aggiornate</param>
        /// <return></return>

        public void updateDocArrivoFromInterop(string oldCorrId, string newCorrId, out int rows)
        {
            rows = 0;
            bool rtn = false;
            Query query = InitQuery.getInstance().getQuery("U_DOC_ARRIVO_MITTENTE_INTEROP");
            query.setParam("param1", newCorrId);
            query.setParam("param2", oldCorrId);

            string sql = query.getSQL();
            logger.Debug(sql);
            this.BeginTransaction();
            rtn = this.ExecuteNonQuery(sql, out rows);
            if (!(rtn && rows > 0))
            {
                this.RollbackTransaction();
                throw new Exception("errore nell'esecuzione della query : " + sql);
            }




            else this.CommitTransaction();
        }


        /// <summary>
        /// Metodo per aggiornate il mittente del di un corrispondente creato da interop
        /// </summary>
        /// <param name="oldCorrId">Id del documento</param>
        /// <param name="newCorrId">Nuovo Id mittente</param>
        /// <param name="rows">Righe aggiornate</param>
        /// <return></return>

        public void updateDocArrivoFromInteropOccasionale(string docId, string newCorrId, out int rows)
        {
            rows = 0;
            bool rtn = false;
            Query query = InitQuery.getInstance().getQuery("U_DOC_ARRIVO_MITTENTE_OCCASIONALE_INTEROP");
            query.setParam("param1", newCorrId);
            query.setParam("param2", docId);

            string sql = query.getSQL();
            logger.Debug(sql);
            this.BeginTransaction();
            rtn = this.ExecuteNonQuery(sql, out rows);
            if (!(rtn && rows > 0))
            {
                this.RollbackTransaction();
                throw new Exception("errore nell'esecuzione della query : " + sql);
            }




            else this.CommitTransaction();
        }



        public Utente[] getUserInRoleByIdCorrGlobali(string idCorrGloRole)
        {
            Utente[] result = null;
            DataSet ds = new DataSet(); ;
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_GET_USERS_IN_ROLE_BY_IDCORRGLOBALE");
                query.setParam("idCorrGlobali", idCorrGloRole);
                string commandText = query.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getUserInRoleByIdCorrGlobali - DocsPaDB/Utenti.cs - QUERY : " + commandText);
                logger.Debug("SQL - getUserInRoleByIdCorrGlobali - DocsPaDB/Utenti.cs - QUERY : " + commandText);
                dbProvider.ExecuteQuery(ds, query.getSQL());
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    result = new DocsPaVO.utente.Utente[ds.Tables[0].Rows.Count];
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.utente.Utente tempUtente = new DocsPaVO.utente.Utente();
                        tempUtente.systemId = ds.Tables[0].Rows[i]["ID_PEOPLE"].ToString();
                        tempUtente.descrizione = ds.Tables[0].Rows[i]["VAR_DESC_CORR"].ToString();
                        result[i] = tempUtente;
                    }
                }
            }

            return result;
        }

        public Utente[] getUserInRoleByIdGruppo(string idGruppo)
        {
            Utente[] result = null;
            DataSet ds = new DataSet(); ;
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_GET_USERS_IN_ROLE_BY_IDGRUPPO");
                query.setParam("idGruppo", idGruppo);
                string commandText = query.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getUserInRoleByIdGruppo - DocsPaDB/Utenti.cs - QUERY : " + commandText);
                logger.Debug("SQL - getUserInRoleByIdGruppo - DocsPaDB/Utenti.cs - QUERY : " + commandText);
                dbProvider.ExecuteQuery(ds, query.getSQL());
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    result = new DocsPaVO.utente.Utente[ds.Tables[0].Rows.Count];
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.utente.Utente tempUtente = new DocsPaVO.utente.Utente();
                        tempUtente.systemId = ds.Tables[0].Rows[i]["ID_PEOPLE"].ToString();
                        tempUtente.descrizione = ds.Tables[0].Rows[i]["VAR_DESC_CORR"].ToString();
                        result[i] = tempUtente;
                    }
                }
            }

            return result;
        }

        public String GetRoleDescriptionByIdGroup(String idGroup)
        {
            String returnValue = String.Empty;
            try
            {
                String query = String.Format("Select var_desc_corr From dpa_corr_globali Where id_gruppo = {0}", idGroup);
                logger.Debug("QUERY: " + query);
                using (DBProvider dbProvider = new DBProvider())
                {
                    IDataReader dataReader = dbProvider.ExecuteReader(query);
                    while (dataReader.Read())
                        returnValue = dataReader[0].ToString();
                    if (dataReader != null)
                    {
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel reperimento della descrizione del ruolo");
                return string.Empty;
            }

            return returnValue;
        }


        public string GetRoleDescriptionByIdCorrGlobali(string idCorrGlobali)
        {
            String returnValue = String.Empty;
            String query = String.Format("Select var_desc_corr From dpa_corr_globali Where system_id = {0}", idCorrGlobali);

            using (DBProvider dbProvider = new DBProvider())
            {
                IDataReader dataReader = dbProvider.ExecuteReader(query);
                while (dataReader.Read())
                    returnValue = dataReader[0].ToString();
                if (dataReader != null)
                {
                    dataReader.Close();
                    dataReader.Dispose();
                }
            }

            return returnValue;
        }


        //ottengo lista di tutte le qualifiche
        public List<DocsPaVO.Qualifica.Qualifica> GetQualifiche(int id_amm)
        {

            List<DocsPaVO.Qualifica.Qualifica> lista = new List<DocsPaVO.Qualifica.Qualifica>();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_QUALIF");
            q.setParam("param1", id_amm.ToString());
            string sql = q.getSQL();
            logger.Debug(sql);

            try
            {

                DataSet dataSet = new DataSet();
                this.ExecuteQuery(out dataSet, "QUALIFICHE", sql);

                foreach (DataRow dataRow in dataSet.Tables["QUALIFICHE"].Rows)
                {
                    DocsPaVO.Qualifica.Qualifica qualifica = new DocsPaVO.Qualifica.Qualifica();
                    qualifica.SYSTEM_ID = Convert.ToInt32(dataRow["SYSTEM_ID"]);
                    qualifica.CODICE = dataRow["CHA_COD"].ToString();
                    qualifica.DESCRIZIONE = dataRow["CHA_DESC"].ToString();
                    qualifica.ID_AMM = Convert.ToInt32(dataRow["ID_AMM"]);
                    lista.Add(qualifica);
                }
                dataSet.Dispose();

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return lista;
        }

        //verifico univocità del codice qualifica per l'inserimento
        public bool CheckUniqueCodiceQualifica(string codiceQualifica, string idAmministrazione)
        {
            bool retValue = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_CHECK_UNIQUE_CODICE_QUALIFICA");
            q.setParam("codiceQualifica", codiceQualifica);
            q.setParam("idAmministrazione", idAmministrazione);

            string sql = q.getSQL();
            logger.Debug(sql);

            string retParam;
            this.ExecuteScalar(out retParam, sql);

            try
            {
                retValue = (Convert.ToInt32(retParam) == 0);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return retValue;
        }


        //inserisco nuova qualifica
        public void InsertQual(string codice, string descrizione, string idAmministrazione)
        {

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_INSERT_QUALIFICA");
            q.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            q.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_QUALIFICHE"));
            q.setParam("codice", codice);
            q.setParam("descrizione", descrizione.Replace("'", "''"));
            q.setParam("idAmministrazione", idAmministrazione);

            string sql = q.getSQL();
            logger.Debug(sql);

            try
            {
                this.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }


        //aggiorno qualifica esistente
        public void UpdateQual(string idQualifica, string descrizione)
        {

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_UPDATE_QUALIFICA");
            q.setParam("id", idQualifica);
            q.setParam("descrizione", descrizione.Replace("'", "''"));

            string sql = q.getSQL();
            logger.Debug(sql);

            try
            {
                this.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }

        //cancello una qualifica
        public void DeleteQual(string idQualifica, string idAmministrazione)
        {

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_DELETE_QUALIFICA");
            q.setParam("idQualifica", idQualifica);
            q.setParam("idAmministrazione", idAmministrazione);

            string sql = q.getSQL();
            logger.Debug(sql);

            try
            {
                this.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }


        //ottengo lista di tutte le peoplegroups in base ai parametri
        public List<DocsPaVO.Qualifica.PeopleGroupsQualifiche> GetPeopleGroupsQualifiche(String idAmm, String idUo, String idGruppo, String idPeople)
        {

            List<DocsPaVO.Qualifica.PeopleGroupsQualifiche> lista = new List<DocsPaVO.Qualifica.PeopleGroupsQualifiche>();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_PEOPLEGROUPS_QUALIFICHE_BY_PARAMETERS");
            q.setParam("idAmm", idAmm);
            q.setParam("idUo", idUo);
            q.setParam("idGruppo", idGruppo);
            q.setParam("idPeople", idPeople);
            string sql = q.getSQL();
            logger.Debug(sql);

            try
            {

                DataSet dataSet = new DataSet();
                this.ExecuteQuery(out dataSet, "PEOPLEGROUPS_QUALIFICHE", sql);

                foreach (DataRow dataRow in dataSet.Tables["PEOPLEGROUPS_QUALIFICHE"].Rows)
                {
                    DocsPaVO.Qualifica.PeopleGroupsQualifiche pgq = new DocsPaVO.Qualifica.PeopleGroupsQualifiche();
                    pgq.SYSTEM_ID = Convert.ToInt32(dataRow["SYSTEM_ID"]);
                    pgq.ID_AMM = Convert.ToInt32(dataRow["ID_AMM"]);
                    pgq.ID_UO = Convert.ToInt32(dataRow["ID_UO"]);
                    pgq.ID_GRUPPO = Convert.ToInt32(dataRow["ID_GRUPPO"]);
                    pgq.ID_PEOPLE = Convert.ToInt32(dataRow["ID_PEOPLE"]);
                    pgq.CODICE = dataRow["CHA_COD"].ToString();
                    pgq.DESCRIZIONE = dataRow["CHA_DESC"].ToString();
                    lista.Add(pgq);
                }
                dataSet.Dispose();

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return lista;
        }

        //inserisco nuovo peoplegroups_qualifiche
        public void InsertPeopleGroupsQual(string idAmm, string idUo, string idGruppo, string idPeople, string idQualifica)
        {

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_INSERT_PEOPLEGROUPS_QUALIFICHE");
            q.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
            q.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_PEOPLEGROUPS_QUALIFICHE"));
            q.setParam("idAmm", idAmm);
            q.setParam("idUo", idUo);
            q.setParam("idGruppo", idGruppo);
            q.setParam("idPeople", idPeople);
            q.setParam("idQualifica", idQualifica);


            string sql = q.getSQL();
            logger.Debug(sql);

            try
            {
                this.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }

        //verifico univocità della tupla PeopleGroups che voglio inserire
        public bool CheckUniquePeopleGroups(string idAmm, string idUo, string idGruppo, string idPeople, string idQualifica)
        {
            bool retValue = false;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_CHECK_UNIQUE_PEOPLEGROUPS");
            q.setParam("idAmm", idAmm);
            q.setParam("idUo", idUo);
            q.setParam("idGruppo", idGruppo);
            q.setParam("idPeople", idPeople);
            q.setParam("idQualifica", idQualifica);

            string sql = q.getSQL();
            logger.Debug(sql);

            string retParam;
            this.ExecuteScalar(out retParam, sql);

            try
            {
                retValue = (Convert.ToInt32(retParam) == 0);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return retValue;
        }

        //cancello un peopleGroups
        public void DeletePeopleGroups(string idPeopleGroups)
        {

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_DELETE_PEOPLEGROUPS_QUALIFICHE");
            q.setParam("idPeopleGroups", idPeopleGroups);

            string sql = q.getSQL();
            logger.Debug(sql);

            try
            {
                this.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }

        /// <summary>
        /// Reperimento di tutti gli utenti che risultano associati ad una qualifica
        /// </summary>
        /// <param name="codiceQualifica"></param>
        /// <param name="idAmm"></param>
        /// <param name="idUo"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public List<DocsPaVO.Qualifica.PeopleQualifica> GetPeopleConQualifica(
                                                                string codiceQualifica,
                                                                string idAmm,
                                                                string idUo,
                                                                string idGruppo)
        {
            List<DocsPaVO.Qualifica.PeopleQualifica> list = new List<DocsPaVO.Qualifica.PeopleQualifica>();

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_PEOPLE_CON_QUALIFICA");
            queryDef.setParam("codiceQualifica", codiceQualifica);

            string customParams = string.Empty;

            if (!string.IsNullOrEmpty(idAmm))
            {
                customParams += string.Format("PQ.ID_AMM = {0}", idAmm);
            }

            if (!string.IsNullOrEmpty(idUo))
            {
                if (!string.IsNullOrEmpty(customParams))
                    customParams += " AND ";
                customParams += string.Format("PQ.ID_UO = {0}", idUo);
            }

            if (!string.IsNullOrEmpty(idGruppo))
            {
                if (!string.IsNullOrEmpty(customParams))
                    customParams += " AND ";
                customParams += string.Format("PQ.ID_GRUPPO = {0}", idGruppo);
            }

            queryDef.setParam("customParams", string.Format(" AND {0}", customParams));

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        list.Add(new DocsPaVO.Qualifica.PeopleQualifica
                        {
                            Id = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID_UTENTE"))),
                            IdPeopleGroups = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ID_PEOPLEGROUPS"))),
                            UserId = reader.GetString(reader.GetOrdinal("USER_NAME")),
                            Matricola = reader.GetString(reader.GetOrdinal("MATRICOLA")),
                            Cognome = reader.GetString(reader.GetOrdinal("COGNOME")),
                            Nome = reader.GetString(reader.GetOrdinal("NOME")),
                            NomeCompleto = reader.GetString(reader.GetOrdinal("NOME_COMPLETO"))
                        });
                    }
                }
            }

            return list;
        }

        //ottengo lista di tutte le peoplegroups in base ad idPeople
        public List<DocsPaVO.Qualifica.PeopleGroupsQualifiche> GetPeopleGroupsQualificheByIdPeople(String idPeople)
        {

            List<DocsPaVO.Qualifica.PeopleGroupsQualifiche> lista = new List<DocsPaVO.Qualifica.PeopleGroupsQualifiche>();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_PEOPLEGROUPS_QUALIFICHE_BY_IDPEOPLE");
            q.setParam("idPeople", idPeople);
            string sql = q.getSQL();
            logger.Debug(sql);

            try
            {

                DataSet dataSet = new DataSet();
                this.ExecuteQuery(out dataSet, "PEOPLEGROUPS_QUALIFICHE_BY_IDPEOPLE", sql);

                foreach (DataRow dataRow in dataSet.Tables["PEOPLEGROUPS_QUALIFICHE_BY_IDPEOPLE"].Rows)
                {
                    DocsPaVO.Qualifica.PeopleGroupsQualifiche pgq = new DocsPaVO.Qualifica.PeopleGroupsQualifiche();
                    pgq.SYSTEM_ID = Convert.ToInt32(dataRow["SYSTEM_ID"]);
                    pgq.ID_AMM = Convert.ToInt32(dataRow["ID_AMM"]);
                    pgq.ID_UO = Convert.ToInt32(dataRow["ID_UO"]);
                    pgq.ID_GRUPPO = Convert.ToInt32(dataRow["ID_GRUPPO"]);
                    pgq.ID_PEOPLE = Convert.ToInt32(dataRow["ID_PEOPLE"]);
                    pgq.ID_QUALIFICA = Convert.ToInt32(dataRow["ID_QUALIFICA"]);
                    pgq.CODICE = dataRow["CHA_COD"].ToString();
                    pgq.DESCRIZIONE = dataRow["CHA_DESC"].ToString();
                    lista.Add(pgq);
                }
                dataSet.Dispose();

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return lista;
        }


        public List<DocsPaVO.utente.Utente> GetUtentiByCodQualifica(String codQualifica)
        {

            List<DocsPaVO.utente.Utente> lista = new List<DocsPaVO.utente.Utente>();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_UTENTI_BY_CODQUALIFICA");
            q.setParam("codQualifica", codQualifica);
            string sql = q.getSQL();
            logger.Debug(sql);

            try
            {

                DataSet dataSet = new DataSet();
                this.ExecuteQuery(out dataSet, "UTENTI", sql);

                foreach (DataRow dataRow in dataSet.Tables["UTENTI"].Rows)
                {
                    DocsPaVO.utente.Utente ut = new DocsPaVO.utente.Utente();
                    ut.systemId = dataRow["SYSTEM_ID"].ToString();
                    ut.userId = dataRow["USER_ID"].ToString();
                    ut.descrizione = dataRow["FULL_NAME"].ToString();
                    ut.telefono = dataRow["PHONE"].ToString();
                    ut.email = dataRow["EMAIL_ADDRESS"].ToString();
                    ut.notifica = dataRow["CHA_NOTIFICA"].ToString();
                    ut.idAmministrazione = dataRow["ID_AMM"].ToString();
                    ut.cognome = dataRow["VAR_COGNOME"].ToString();
                    ut.nome = dataRow["VAR_NOME"].ToString();
                    ut.sede = dataRow["VAR_SEDE"].ToString();
                    ut.idPeople = ut.systemId;

                    lista.Add(ut);

                }

                dataSet.Dispose();

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return lista;
        }

        /// <summary>
        /// Reperimento della lista delle amministrazioni dell'utente censito per l'accesso ad un modulo
        /// </summary>
        /// <param name="List_idAmm"></param>
        /// <param name="userName"></param>
        /// <param name="modulo"></param>
        public void GetIdAmmUtente(out ArrayList List_idAmm, string userName, string modulo)
        {
            if (!string.IsNullOrEmpty(modulo))
            {
                if (DocsPaUtils.Moduli.ModuliAuthManager.IsModuloCentroServizi(modulo))
                {
                    GetIdAmmUtenteCentroServiziConservazione(out List_idAmm, userName);
                }
                else
                {
                    GetIdAmmUtente(out List_idAmm, userName);
                }
            }
            else
                GetIdAmmUtente(out List_idAmm, userName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="List_idAmm"></param>
        /// <param name="userName"></param>
        public void GetIdAmmUtenteCentroServiziConservazione(out ArrayList List_idAmm, string userName)
        {
            List_idAmm = new ArrayList();

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_PEOPLE_CENTRO_SERVIZI");

                queryDef.setParam("param1", "ID_AMM");
                queryDef.setParam("param2", "UPPER(USER_ID)='" + userName.ToUpper() + "'");

                string commandText = queryDef.getSQL();
                logger.InfoFormat("S_PEOPLE_CENTRO_SERVIZI: {0}", commandText);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("ID_AMM")))
                        {
                            List_idAmm.Add(reader.GetValue(reader.GetOrdinal("ID_AMM")).ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Verifica se l'utente è abilitato all'accesso al modulo Centro Servizi di conservazione
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool IsUtenteDisabledCentroServizi(string userName)
        {
            bool disabled = false;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_PEOPLE_DISABLED_CENTRO_SERVIZI");

                queryDef.setParam("userId", userName);

                string commandText = queryDef.getSQL();
                logger.InfoFormat("S_PEOPLE_DISABLED_CENTRO_SERVIZI: {0}", commandText);

                string field;
                if (!dbProvider.ExecuteScalar(out field, commandText))
                    throw new ApplicationException(dbProvider.LastExceptionMessage);

                disabled = string.IsNullOrEmpty(field);
            }

            return disabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="modulo"></param>
        /// <returns></returns>
        public bool IsUtenteDisabled(string userName, string modulo, string idAmm)
        {
            if (!string.IsNullOrEmpty(modulo))
            {
                if (DocsPaUtils.Moduli.ModuliAuthManager.IsModuloCentroServizi(modulo))
                    return IsUtenteDisabledCentroServizi(userName);
                else
                    return IsUtenteDisabled(userName, idAmm);
            }
            else
                return IsUtenteDisabled(userName, idAmm);
        }

        /// <summary>
        /// Restituisce il valore della var_desc_corr_old (necessario nei controlli K1, K2 per pec)
        /// </summary>
        /// <param name="idCorr"></param>
        /// <returns></returns>
        public string GetDescOldByCorr(string systemId)
        {
            string descOld = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(systemId))
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
                    q.setParam("param1", "var_desc_corr_old");
                    q.setParam("param2", " SYSTEM_ID=" + systemId);
                    string commandText = q.getSQL();
                    logger.Debug(commandText);
                    ExecuteScalar(out descOld, commandText);
                }
                return descOld;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        #region Multi Casella corrispondenti esterni
        public DataSet GetMailCorr(string idCorrispondente)
        {
            DataSet dataSet = new DataSet();
            try
            {
                if (!string.IsNullOrEmpty(idCorrispondente))
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_MAIL_CORR_ESTERNO");
                    q.setParam("idCorrispondente", idCorrispondente);
                    string commandText = q.getSQL();
                    logger.Debug(commandText);
                    ExecuteQuery(out dataSet, "CASELLE_CORRISPONDENTE", commandText);
                }
                return dataSet;

            }
            catch (Exception e)
            {
                return new DataSet();
            }
        }

        public DataSet GetMailsAllCorrProto(int idDoc)
        {
            DataSet dataSet = new DataSet();
            try
            {
                if (idDoc != null && idDoc > 0)
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ALL_MAIL_CORR_PROTO");
                    q.setParam("idDoc", idDoc.ToString());
                    string commandText = q.getSQL();
                    logger.Debug(commandText);
                    ExecuteQuery(out dataSet, "CASELLE_CORRISPONDENTE", commandText);
                }
                return dataSet;

            }
            catch (Exception e)
            {
                return new DataSet();
            }
        }

        public bool InsertMailCorr(List<MailCorrispondente> listCaselleCorr, string idCorrispondente)
        {
            try
            {
                DocsPaUtils.Query q;
                bool res = false;
                // Elimino tutte le caselle precedentemente associate al corrispondente esterno in DPA_MAIL_CORR_ESTERNI
                q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_MAIL_CORR_ESTERNO");
                q.setParam("idCorrispondente", idCorrispondente);
                BeginTransaction();
                string commandText = q.getSQL();
                logger.Debug(commandText);
                res = ExecuteNonQuery(commandText);
                if (!res)
                {
                    logger.Error("Errore nella cancellazione delle mail");
                    RollbackTransaction();
                }
                else
                    CommitTransaction();

                if (res)
                {
                    // Insert in DPA_MAIL_CORR_ESTERNI delle nuove caselle associate al corrispondente esterno 
                    BeginTransaction();
                    foreach (MailCorrispondente c in listCaselleCorr)
                    {
                        System.Text.StringBuilder recordInsert = new System.Text.StringBuilder();
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_MAIL_CORR_ESTERNO");
                        recordInsert.Append("(\n");
                        recordInsert.Append(
                            ((DBType.ToUpper().Equals("ORACLE")) ? DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MAIL_CORR_ESTERNI") + "\n" : "\n") +
                             idCorrispondente + ",\n" +
                            "'" + c.Email.Trim() + "',\n " +
                            "'" + c.Principale + "',\n" +
                            "'" + c.Note + "'");
                        recordInsert.Append(")");
                        q.setParam("value", recordInsert.ToString());
                        string commandText2 = q.getSQL();
                        logger.Debug(commandText2);

                        res = ExecuteNonQuery(commandText2);
                        if (!res)
                        {
                            logger.Error("Errore in inserimento delle mail");

                            RollbackTransaction();
                            return res;
                        }
                    }
                    CommitTransaction();
                }
                return res;
            }
            catch (Exception e)
            {
                logger.Error(e);
                RollbackTransaction();
                return false;
            }
        }

        public bool DeleteMailCorr(string idCorrispondente)
        {
            bool res = false;
            try
            {
                DocsPaUtils.Query q;
                // Elimino tutte le caselle associate al corrispondente appena eliminato dalla dpa_corr_globali
                q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_MAIL_CORR_ESTERNO");
                q.setParam("idCorrispondente", idCorrispondente);
                BeginTransaction();
                res = ExecuteNonQuery(q.getSQL());
                if (!res)
                    RollbackTransaction();
                else
                    CommitTransaction();
                return res;
            }
            catch (Exception e)
            {
                RollbackTransaction();
                return false;
            }
        }
        #endregion

        /// <summary>
        /// Metodo per il recupero del codice attuale di un corrispondente
        /// </summary>
        /// <param name="code">Codice del corrispondente</param>
        /// <returns>Codice del corrispondente nella sua ultima versione</returns>
        public String GetActualCorrCode(String code)
        {
            String corrCode = code;
            Query query = InitQuery.getInstance().getQuery("S_ACTUAL_CORR_CODE_BY_CODE");
            query.setParam("corrCode", code);

            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {
                    while (dataReader.Read())
                        corrCode = dataReader[0].ToString();

                }

            }

            return corrCode;
        }

        public void GetRegistroByCodice(string codice, ref DocsPaVO.utente.Registro reg)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_REGISTRO_BY_CODICE");
            q.setParam("param1", "SYSTEM_ID, VAR_CODICE, NUM_RIF, VAR_DESC_REGISTRO, " +
                "VAR_EMAIL_REGISTRO, CHA_STATO, ID_AMM, " +
                DocsPaDbManagement.Functions.Functions.ToChar("DTA_OPEN", false) + "," + //DocsPaWS.Utils.dbControl.toChar("DTA_OPEN",false) + 
                DocsPaDbManagement.Functions.Functions.ToChar("DTA_CLOSE", false) + "," + //DocsPaWS.Utils.dbControl.toChar("DTA_CLOSE",false) + 
                DocsPaDbManagement.Functions.Functions.ToChar("DTA_ULTIMO_PROTO", false) + " , ID_RUOLO_AOO,ID_RUOLO_RESP,ID_PEOPLE_AOO, CHA_AUTO_INTEROP, CHA_RF, CHA_DISABILITATO, ID_AOO_COLLEGATA, INVIO_RICEVUTA_MANUALE, FLAG_WSPIA, VAR_PREG, ANNO_PREG, DIRITTO_RUOLO_AOO"); //DocsPaWS.Utils.dbControl.toChar("DTA_ULTIMO_PROTO",false));
            q.setParam("param2", "UPPER(VAR_CODICE)='" + codice.ToUpper() + "'");
            string sql = q.getSQL();
            logger.Debug(sql);
            DataSet dataSet;
            this.ExecuteQuery(out dataSet, sql);
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                reg = new DocsPaVO.utente.Registro();
                reg.systemId = dataRow[0].ToString();
                reg.codRegistro = dataRow[1].ToString();
                reg.codice = dataRow[2].ToString();
                reg.descrizione = dataRow[3].ToString();
                reg.email = dataRow[4].ToString();
                reg.stato = dataRow[5].ToString();
                reg.idAmministrazione = dataRow[6].ToString();
                reg.codAmministrazione = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                reg.dataApertura = dataRow[7].ToString();
                reg.dataChiusura = dataRow[8].ToString();
                reg.dataUltimoProtocollo = dataRow[9].ToString();
                reg.idRuoloAOO = dataRow[10].ToString();
                reg.idRuoloResp = dataRow[11].ToString();
                reg.idUtenteAOO = dataRow[12].ToString();
                reg.autoInterop = dataRow[13].ToString();
                reg.chaRF = dataRow[14].ToString();
                reg.rfDisabled = dataRow[15].ToString();
                if (dataRow[15].ToString().Equals("1"))
                    reg.Sospeso = true;
                else
                    reg.Sospeso = false;
                reg.idAOOCollegata = dataRow[16].ToString();
                reg.invioRicevutaManuale = dataRow[17].ToString();
                reg.FlagWspia = dataRow[18].ToString();

                if (dataRow[19].ToString().Equals("1"))
                {
                    reg.flag_pregresso = true;
                }
                else
                {
                    reg.flag_pregresso = false;
                }
                reg.anno_pregresso = dataRow[20].ToString();
                reg.Diritto_Ruolo_AOO = dataRow[21].ToString();
            }
            dataSet.Dispose();
        }

        public DocsPaVO.utente.Utente getUtenteByCodice(string userName, string codiceAmm)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_People_PIS");
            q.setParam("userName", userName.ToUpper());
            q.setParam("codiceAmm", codiceAmm.ToUpper());
            string sql = q.getSQL();
            return GetUserData(sql);
        }

        public bool CodiceRubricaPresente(string codRubrica, string tipoCorr, string idAmm, string idReg, bool inRubricaComune)
        {
            bool result = false;

            logger.Debug("isCodRubricaPresente");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrGlobali");
            q.setParam("param1", "A.SYSTEM_ID,A.CHA_TIPO_IE,A.CHA_TIPO_CORR");

            string param2Value = string.Format("UPPER(A.VAR_COD_RUBRICA)='{0}'", codRubrica.ToUpper());
            if (!string.IsNullOrEmpty(idAmm))
                param2Value = string.Concat(param2Value, string.Format(" AND (A.ID_AMM = {0} OR A.ID_AMM IS NULL)", idAmm));

            if (inRubricaComune != null)
            {
                if (!inRubricaComune)
                {
                    if (!string.IsNullOrEmpty(idReg))
                    {
                        param2Value = string.Concat(param2Value, string.Format(" AND (A.ID_REGISTRO = {0} OR A.ID_REGISTRO IS NULL) AND A.DTA_FINE IS NULL ", idReg));
                    }
                    else
                    {
                        param2Value = string.Concat(param2Value, " AND A.DTA_FINE IS NULL ");
                    }
                }
                else
                    param2Value = string.Concat(param2Value, " AND A.DTA_FINE IS NULL");
            }
            else
            {
                logger.Debug("ERRORE in IsCodRubricaPresente il tipoCorr è null");
                param2Value = string.Concat(param2Value, string.Format(" AND A.DTA_FINE IS NULL"));
            }
            q.setParam("param2", param2Value);

            string sql = q.getSQL();
            logger.Debug(sql);
            DataSet dataSet = new DataSet();
            this.ExecuteQuery(out dataSet, "CODICE", sql);
            if (dataSet.Tables["CODICE"].Rows.Count > 0)
            {
                result = true;
            }
            return result;
        }

        public DocsPaVO.utente.Corrispondente GetDettaglioCorrispondente(string idCorrispondente)
        {
            logger.Debug("INIT - GetDettaglioCorrispondente");

            DocsPaVO.utente.Corrispondente corrispondente = null;

            try
            {
                if (!string.IsNullOrEmpty(idCorrispondente))
                {
                    corrispondente = new DocsPaVO.utente.Corrispondente();

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPADETTGLOBALIBYID");

                    q.setParam("idCorr", idCorrispondente);

                    string commandText = q.getSQL();
                    logger.Debug(commandText);

                    bool existCorrispondente = false;

                    using (DBProvider dbProvider = new DBProvider())
                    {
                        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                        {
                            if (reader.Read())
                            {

                                if (!reader.IsDBNull(reader.GetOrdinal("VAR_INDIRIZZO")))
                                    corrispondente.indirizzo = reader.GetValue(reader.GetOrdinal("VAR_INDIRIZZO")).ToString();

                                if (!reader.IsDBNull(reader.GetOrdinal("VAR_CAP")))
                                    corrispondente.cap = reader.GetValue(reader.GetOrdinal("VAR_CAP")).ToString();

                                if (!reader.IsDBNull(reader.GetOrdinal("VAR_PROVINCIA")))
                                    corrispondente.prov = reader.GetValue(reader.GetOrdinal("VAR_PROVINCIA")).ToString();

                                if (!reader.IsDBNull(reader.GetOrdinal("VAR_NAZIONE")))
                                    corrispondente.nazionalita = reader.GetValue(reader.GetOrdinal("VAR_NAZIONE")).ToString();

                                if (!reader.IsDBNull(reader.GetOrdinal("VAR_COD_FISC")))
                                    corrispondente.codfisc = reader.GetValue(reader.GetOrdinal("VAR_COD_FISC")).ToString();

                                if (!reader.IsDBNull(reader.GetOrdinal("VAR_TELEFONO")))
                                    corrispondente.telefono1 = reader.GetValue(reader.GetOrdinal("VAR_TELEFONO")).ToString();

                                if (!reader.IsDBNull(reader.GetOrdinal("VAR_FAX")))
                                    corrispondente.fax = reader.GetValue(reader.GetOrdinal("VAR_FAX")).ToString();

                                if (!reader.IsDBNull(reader.GetOrdinal("VAR_NOTE")))
                                    corrispondente.note = reader.GetValue(reader.GetOrdinal("VAR_NOTE")).ToString();

                                if (!reader.IsDBNull(reader.GetOrdinal("VAR_CITTA")))
                                    corrispondente.citta = reader.GetValue(reader.GetOrdinal("VAR_CITTA")).ToString();

                                if (!reader.IsDBNull(reader.GetOrdinal("VAR_COD_PI")))
                                    corrispondente.partitaiva = reader.GetValue(reader.GetOrdinal("VAR_COD_PI")).ToString();

                                if (!reader.IsDBNull(reader.GetOrdinal("VAR_LOCALITA")))
                                    corrispondente.localita = reader.GetValue(reader.GetOrdinal("VAR_LOCALITA")).ToString();
                                /*
                                if (!reader.IsDBNull(reader.GetOrdinal("VAR_COD_IPA")))
                                    corrispondente.codiceIpa = reader.GetValue(reader.GetOrdinal("VAR_COD_IPA")).ToString();
                                */
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Si è verificato un errore nella ricerca del corrispondente con ID '{0}'", idCorrispondente), ex);
            }
            finally
            {
                logger.Debug("END - GetDettaglioCorrispondente");
            }

            return corrispondente;
        }

        /// <summary>
        /// Metodo per il recupero di un attributo di un corrispondente
        /// </summary>
        /// <param name="codCorr">Codice del corrispondente</param>
        /// <param name="corrAttribute">Attributo del corrispondente</param>
        /// <returns>Attributo richiesto</returns>
        public String GetInternalCorrAttributeByCorrCode(String codCorr, CorrAttribute corrAttribute, String adminId)
        {
            String retVal = String.Empty;
            String query = String.Format("Select {0} From dpa_corr_globali Where dta_fine Is Null And cha_tipo_ie = 'I' And id_amm = {1} And upper(var_cod_rubrica) = '{2}'",
                corrAttribute,
                adminId,
                codCorr.ToUpperInvariant());

            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query))
                {
                    while (dataReader.Read())
                    {
                        retVal = dataReader[0].ToString();
                    }

                }

            }

            return retVal;

        }

        /// <summary>
        /// Attributo richiedibile per un corrispondente
        /// </summary>
        public enum CorrAttribute
        {
            cha_tipo_urp,
            id_people
        }

        /// <summary>
        /// Restituisce la lista dei 'destinatari' / 'destinatari in cc' associati ad un documento con il relativo canale preferenziale
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="typeDest">Può assumere D(estrai le info sui destinatari), C(estrati le info sui destinatari in CC)</param>
        /// <returns>List</returns>
        public List<DocsPaVO.utente.Corrispondente> GetPrefChannelAllDest(string idProfile, string typeDest)
        {
            logger.Debug("INIT - GetPrefChannelAllDest");
            DataSet ds = new DataSet();
            List<DocsPaVO.utente.Corrispondente> corrispondenti = new List<Corrispondente>();
            try
            {
                if (!string.IsNullOrEmpty(idProfile) && (!string.IsNullOrEmpty(typeDest)))
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPATCanaleAllCorrDoc");
                    q.setParam("idProfile", idProfile);
                    q.setParam("typeDest", typeDest);
                    ExecuteQuery(ds, q.getSQL());
                }
                logger.Debug("END - GetPrefChannelAllDest");
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        corrispondenti.Add(new Corrispondente()
                        {
                            systemId = row["id_mitt_dest"].ToString(),
                            canalePref = new Canale()
                            {
                                systemId = row["system_id"].ToString(),
                                descrizione = row["description"].ToString(),
                                typeId = row["type_id"].ToString()
                            }
                        });
                    }
                }

                return corrispondenti;
            }
            catch (Exception e)
            {
                logger.Debug(string.Format("Si è verificato un errore nel metodo GetPrefChannelAllDest.\nDettaglio errore: {0}"), e);
                logger.Debug("END - GetPrefChannelAllDest");
                return corrispondenti;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public DocsPaVO.addressbook.CorrespondentDetails getCorrespondentDetails(string idCorr)
        {
            DocsPaVO.addressbook.CorrespondentDetails corr = new DocsPaVO.addressbook.CorrespondentDetails();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_CORRESPONDENT_DETAILS");

            q.setParam("param1", idCorr);
            string sql = q.getSQL();
            logger.Debug(sql);

            try
            {

                DataSet dataSet = new DataSet();
                this.ExecuteQuery(out dataSet, "CORRESPONDENTS", sql);

                if (dataSet.Tables["CORRESPONDENTS"].Rows.Count > 0)
                {
                    DataRow dr = dataSet.Tables["CORRESPONDENTS"].Rows[0];
                    corr.SystemId = dr["SYSTEM_ID"].ToString();
                    corr.IdCorr = dr["ID_CORR_GLOBALI"].ToString();
                    corr.Address = dr["VAR_INDIRIZZO"].ToString();
                    corr.ZipCode = dr["VAR_CAP"].ToString();
                    corr.District = dr["VAR_PROVINCIA"].ToString();
                    corr.Country = dr["VAR_NAZIONE"].ToString();
                    corr.Phone = dr["VAR_TELEFONO"].ToString();
                    corr.Phone2 = dr["VAR_TELEFONO2"].ToString();
                    corr.Fax = dr["VAR_FAX"].ToString();
                    corr.Note = dr["VAR_NOTE"].ToString();
                    corr.TaxId = (string.IsNullOrEmpty(dr["VAR_COD_FISC"].ToString().Trim()) ? dr["VAR_COD_FISCALE"].ToString().Trim() : dr["VAR_COD_FISC"].ToString());
                    corr.City = dr["VAR_CITTA"].ToString();
                    corr.Place = dr["VAR_LOCALITA"].ToString();
                    corr.BirthPlace = dr["VAR_LUOGO_NASCITA"].ToString();
                    corr.BirthDay = dr["DTA_NASCITA"].ToString();
                    corr.Title = dr["VAR_TITOLO"].ToString();
                    corr.CommercialId = dr["VAR_COD_PI"].ToString();
                    //corr.IpaCode = dr["VAR_COD_IPA"].ToString();
                }
                dataSet.Dispose();

            }
            catch (Exception e)
            {
                throw (e);
            }
            return corr;
        }

        //Laura 22 Febbraio 2013
        public string getUserDB()
        {
            return Functions.GetDbUserSession();
        }

        public bool AddBrowserInfo(BrowserInfo browser, string idPeople, string userName, string ipAddress)
        {
            bool result = false;
            bool insert = true;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_UA_INFO");
                if (!string.IsNullOrEmpty(userName))
                    q.setParam("userId", userName.Replace("'", "''"));
                else
                    insert = false;
                if (!string.IsNullOrEmpty(idPeople))
                    q.setParam("idPeople", idPeople);
                else
                    insert = false;
                if (!string.IsNullOrEmpty(browser.ip) && !browser.ip.StartsWith(":"))
                    q.setParam("ip", browser.ip.Replace("'", "''"));
                else
                {
                    if (!string.IsNullOrEmpty(ipAddress))
                        q.setParam("ip", ipAddress.Replace("'", "''"));
                    else
                        insert = false;
                }
                q.setParam("browserType", browser.browserType.Replace("'", "''"));
                q.setParam("browserVersion", browser.browserVersion.Replace("'", "''"));
                if (!string.IsNullOrEmpty(browser.activex))
                {
                    if (browser.activex.ToLower().Equals("true"))
                        q.setParam("activex", "1");
                    else
                        q.setParam("activex", "0");
                }
                if (!string.IsNullOrEmpty(browser.javascript))
                {
                    if (browser.javascript.ToLower().Equals("true"))
                        q.setParam("javascript", "1");
                    else
                        q.setParam("javascript", "0");
                }
                if (!string.IsNullOrEmpty(browser.javaApplet))
                {
                    if (browser.javaApplet.ToLower().Equals("true"))
                        q.setParam("javaapplet", "1");
                    else
                        q.setParam("javaapplet", "0");
                }
                if (!string.IsNullOrEmpty(browser.userAgent))
                    q.setParam("userAgent", browser.userAgent.Replace("'", "''"));

                int res = 0;
                if (insert)
                {
                    string query;
                    query = q.getSQL();
                    this.ExecuteNonQuery(query, out res);
                    if (res > 0)
                        result = true;
                    else
                        result = false;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nella memorizzazione informazioni su DPA_UA_INFO" + ex.Message);
            }
            return result;
        }

        public bool UnlockUserLoginOtherSessions(string userId, string idAmm, string sessionId)
        {
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("UNLOCK_USER_LOGIN_ON_SESSION_ID");
                q.setParam("param1", userId);
                if (!string.IsNullOrEmpty(idAmm))
                    q.setParam("param2", " AND ID_AMM=" + idAmm);
                else
                    q.setParam("param2", " ");
                if (!string.IsNullOrEmpty(sessionId))
                    q.setParam("param3", " AND SESSION_ID<>'" + sessionId + "'");
                else
                    q.setParam("param3", " ");
                string sql = q.getSQL();
                logger.Debug(sql);
                if (this.ExecuteNonQuery(sql))
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;

        }

        public List<string> GetIdOpportunityList(string cod_people)
        {
            List<string> lista = new List<string>();

            logger.Debug("getIdOpportunityList");

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_OPPORTUNITY");
            if (!string.IsNullOrEmpty(cod_people))
                q.setParam("param1", " WHERE PEOPLE.USER_ID = '" + cod_people + "'");

            logger.Debug(q.getSQL());
            DataSet ds = new DataSet();

            try
            {
                if (this.ExecuteQuery(out ds, "opportunity", q.getSQL()))
                {
                    DocsPaVO.utente.Corrispondente corrispondente = new DocsPaVO.utente.Corrispondente();
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            string idop = (string)row["ID_OPPORTUNITY"].ToString();
                            lista.Add(idop);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nel caricare la lista degli Id Opportunity. ", e);
                return null;
            }

            return lista;
        }

        #region deposito

        public bool isUtArchivistaDeposito(string idPeople, string idGruppo)
        {

            //per ora l'unico controllo che faccio è sull'attivazione dell'ambiente di deposito

            //se idPeople null vuol dire che la security è associata al ruolo (tipo nodi di titolario, tipologia documento, ...)

            string eme = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ARCHIVIO_DEPOSITO");
            if (eme == null || !eme.Equals("1"))
                return false;

            //poi dovrei fare il controllo sull'utente e il ruolo archivista ...

            return isArchivistaDeposito(idGruppo);

        }


        private bool isArchivistaDeposito(string idGruppo)
        {
            logger.Debug("START : IsArchivistaDeposito");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RUOLO_ARCHIVISTA_DEPOSITO");

            q.setParam("param1", idGruppo);

            string selectString = q.getSQL();
            string ret;
            logger.Debug(selectString);
            this.ExecuteScalar(out ret, selectString);
            logger.Debug("END : IsArchivistaDeposito");
            if (string.IsNullOrEmpty(ret))
                return false;
            return true;
        }

        #endregion deposito

        #region HSMPIN

        public HSMPin SelectHSMPin(string idPeople, string idAmministrazione)
        {
            DataSet dataSet = null;
            String queryString = null;
            DocsPaUtils.Query q = null;
            HSMPin hsmParameters = null;
            DataRow dr = null;

            try
            {
                logger.Debug("START : DocsPaDB > Query_DocsPAWS > SelectHSMPin");

                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_HSMPIN");

                q.setParam("idPeople", idPeople);
                q.setParam("idAmministrazione", idAmministrazione);

                queryString = q.getSQL();

                logger.Debug(queryString);

                ExecuteQuery(out dataSet, queryString);

                if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    hsmParameters = new HSMPin();

                    dr = dataSet.Tables[0].Rows[0];
                    hsmParameters.pin = dr["PIN"].ToString();
                    hsmParameters.alias = dr["ALIAS"].ToString();
                    hsmParameters.idPeople = idPeople;
                    hsmParameters.idAmministrazione = idAmministrazione;
                }

                dataSet.Dispose();

                logger.Debug("END : DocsPaDB > Query_DocsPAWS > SelectHSMPin");
            }
            catch (Exception e)
            {
                logger.Error("Exception Utenti.SelectHSMPin sql: " + q.getSQL(), e);
                throw e;
            }
            finally
            {
                if (q != null) q = null;
                if (dataSet != null)
                {
                    dataSet.Clear();
                    dataSet = null;
                }
                queryString = null;
            }

            return hsmParameters;
        }

        public bool InsertHSMPin(HSMPin hsmPin)
        {
            String sql = null;
            String res = null;
            bool execute = false;
            DocsPaUtils.Query q = null;

            try
            {
                logger.Debug("START : DocsPaDB > Query_DocsPAWS > InsertHSMPin");

                q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_HSMPIN");

                q.setParam("idPeople", hsmPin.idPeople);
                q.setParam("idAmministrazione", hsmPin.idAmministrazione);
                q.setParam("pin", hsmPin.pin);
                q.setParam("alias", hsmPin.alias);

                sql = q.getSQL();

                logger.Debug(sql);

                BeginTransaction();

                execute = InsertLocked(out res, sql, "DPA_DPA_HSMPIN");

                if (!execute) RollbackTransaction();
                else CommitTransaction();

                logger.Debug("END : DocsPaDB > Query_DocsPAWS > InsertHSMPin");
            }
            catch (Exception e)
            {
                RollbackTransaction();
                execute = false;
                logger.Error("Exception Utenti.InsertHSMPin sql: " + q.getSQL(), e);
            }
            finally
            {
                if (q != null) q = null;
                if (sql != null) sql = null;
                if (res != null) res = null;
            }
            return execute;
        }

        public bool UpdateHSMPin(HSMPin hsmPin)
        {
            String sql = null;
            bool execute = false;
            DocsPaUtils.Query q = null;

            try
            {
                logger.Debug("START : DocsPaDB > Query_DocsPAWS > UpdateHSMPin");

                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_HSMPIN");

                q.setParam("idPeople", hsmPin.idPeople);
                q.setParam("idAmministrazione", hsmPin.idAmministrazione);
                q.setParam("pin", hsmPin.pin);
                q.setParam("alias", hsmPin.alias);

                sql = q.getSQL();

                BeginTransaction();

                execute = ExecuteNonQuery(q.getSQL());

                if (!execute) RollbackTransaction();
                else CommitTransaction();

                logger.Debug("END : DocsPaDB > Query_DocsPAWS > UpdateHSMPin");
            }
            catch (Exception e)
            {
                RollbackTransaction();
                execute = false;
                logger.Error("Exception Utenti.UpdateHSMPin sql: " + q.getSQL(), e);
            }
            finally
            {
                if (q != null) q = null;
                if (sql != null) sql = null;
            }
            return execute;
        }

        public bool DeleteHSMPin(String idPeople, String idAmministrazione)
        {
            bool execute = false;
            DocsPaUtils.Query q = null;

            try
            {
                logger.Debug("START : DocsPaDB > Query_DocsPAWS > DeleteHSMPin");

                q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_HSMPIN");
                q.setParam("idPeople", idPeople);
                q.setParam("idAmministrazione", idAmministrazione);

                BeginTransaction();

                execute = ExecuteNonQuery(q.getSQL());

                if (!execute) RollbackTransaction();
                else CommitTransaction();

                logger.Debug("END : DocsPaDB > Query_DocsPAWS > DeleteHSMPin");
            }
            catch (Exception e)
            {
                RollbackTransaction();
                execute = false;
                logger.Error("Exception Utenti.DeleteHSMPin sql: " + q.getSQL(), e);
            }
            finally
            {
                if (q != null) q = null;
            }
            return execute;
        }
        #endregion HSMPIN

        #region HSMPARAMETERS

        public HSMParameters SelectHSMParameters(string idAmministrazione)
        {
            DataSet dataSet = null;
            String queryString = null;
            DocsPaUtils.Query q = null;
            HSMParameters hsmParameters = null;
            DataRow dr = null;

            try
            {
                logger.Debug("START : DocsPaDB > Query_DocsPAWS > SelectHSMParameters");

                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_HSMPARAMETERS");

                q.setParam("idAmministrazione", idAmministrazione);

                queryString = q.getSQL();

                logger.Debug(queryString);

                ExecuteQuery(out dataSet, queryString);

                if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    hsmParameters = new HSMParameters();
                    dr = dataSet.Tables[0].Rows[0];
                    hsmParameters.id_amm = idAmministrazione;
                    hsmParameters.dominio = dr["DOMINIO"].ToString();
                    hsmParameters.passwordapplicativa = dr["PASSWORDAPPLICATIVA"].ToString();
                    hsmParameters.serverurl = dr["SERVERURL"].ToString();
                    hsmParameters.tsapassword = dr["TSAPASSWORD"].ToString();
                    hsmParameters.tsaurl = dr["TSAURL"].ToString();
                    hsmParameters.tsauser = dr["TSAUSER"].ToString();
                    hsmParameters.userapplicativa = dr["USERAPPLICATIVA"].ToString();
                }

                dataSet.Dispose();

                logger.Debug("END : DocsPaDB > Query_DocsPAWS > SelectHSMParameters");
            }
            catch (Exception e)
            {
                logger.Error("Exception Utenti.SelectHSMParameters sql: " + q.getSQL(), e);
                throw e;
            }
            finally
            {
                if (q != null) q = null;
                if (dataSet != null)
                {
                    dataSet.Clear();
                    dataSet = null;
                }
                queryString = null;
            }

            return hsmParameters;
        }

        public bool InsertHSMParameters(HSMParameters hsmParameters)
        {
            String sql = null;
            String res = null;
            bool execute = false;
            DocsPaUtils.Query q = null;

            try
            {
                logger.Debug("START : DocsPaDB > Query_DocsPAWS > InsertHSMParameters");

                q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_HSMPARAMETERS");

                q.setParam("idAmministrazione", hsmParameters.id_amm);
                q.setParam("passwordapplicativa", hsmParameters.passwordapplicativa);
                q.setParam("serverurl", hsmParameters.serverurl);
                q.setParam("dominio", hsmParameters.dominio);
                q.setParam("tsapassword", hsmParameters.tsapassword);
                q.setParam("tsaurl", hsmParameters.tsaurl);
                q.setParam("tsauser", hsmParameters.tsauser);
                q.setParam("userapplicativa", hsmParameters.userapplicativa);

                sql = q.getSQL();

                logger.Debug(sql);

                BeginTransaction();

                execute = InsertLocked(out res, sql, "DPA_HSMPARAMETERS");

                if (!execute) RollbackTransaction();
                else CommitTransaction();

                logger.Debug("END : DocsPaDB > Query_DocsPAWS > InsertHSMParameters");
            }
            catch (Exception e)
            {
                RollbackTransaction();
                execute = false;
                logger.Error("Exception Utenti.InsertHSMParameters sql: " + q.getSQL(), e);
            }
            finally
            {
                if (q != null) q = null;
                if (sql != null) sql = null;
                if (res != null) res = null;
            }
            return execute;
        }

        public bool UpdateHSMParameters(HSMParameters hsmParameters)
        {
            String sql = null;
            bool execute = false;
            DocsPaUtils.Query q = null;

            try
            {
                logger.Debug("START : DocsPaDB > Query_DocsPAWS > UpdateHSMParameters");

                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_HSMPARAMETERS");

                q.setParam("idAmministrazione", hsmParameters.id_amm);
                q.setParam("passwordapplicativa", hsmParameters.passwordapplicativa);
                q.setParam("serverurl", hsmParameters.serverurl);
                q.setParam("dominio", hsmParameters.dominio);
                q.setParam("tsapassword", hsmParameters.tsapassword);
                q.setParam("tsaurl", hsmParameters.tsaurl);
                q.setParam("tsauser", hsmParameters.tsauser);
                q.setParam("userapplicativa", hsmParameters.userapplicativa);

                sql = q.getSQL();

                BeginTransaction();

                execute = ExecuteNonQuery(q.getSQL());

                if (!execute) RollbackTransaction();
                else CommitTransaction();

                logger.Debug("END : DocsPaDB > Query_DocsPAWS > UpdateHSMParameters");
            }
            catch (Exception e)
            {
                RollbackTransaction();
                execute = false;
                logger.Error("Exception Utenti.UpdateHSMParameters sql: " + q.getSQL(), e);
            }
            finally
            {
                if (q != null) q = null;
                if (sql != null) sql = null;
            }
            return execute;
        }

        public bool DeleteHSMParameters(String idAmministrazione)
        {
            bool execute = false;
            DocsPaUtils.Query q = null;

            try
            {
                logger.Debug("START : DocsPaDB > Query_DocsPAWS > DeleteHSMParameters");

                q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_HSMPARAMETERS");
                q.setParam("idAmministrazione", idAmministrazione);

                BeginTransaction();

                execute = ExecuteNonQuery(q.getSQL());

                if (!execute) RollbackTransaction();
                else CommitTransaction();

                logger.Debug("END : DocsPaDB > Query_DocsPAWS > DeleteHSMParameters");
            }
            catch (Exception e)
            {
                RollbackTransaction();
                execute = false;
                logger.Error("Exception Utenti.DeleteHSMParameters sql: " + q.getSQL(), e);
            }
            finally
            {
                if (q != null) q = null;
            }
            return execute;
        }
        #endregion HSMPARAMETERS

        #region CAP_COMUNI

        public List<string> GetListaCapComuni(string prefixCap, string comune)
        {
            List<string> listaCap = new List<string>();
            logger.Debug("GetListaCap");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_CAP_COMUNI");
            string query;
            try
            {
                string conditionCap = string.IsNullOrEmpty(prefixCap) ? "" : " VAR_CAP LIKE '"+ prefixCap + "%' ";
                string conditionComune = string.IsNullOrEmpty(comune) ? "": " UPPER(VAR_COMUNE)= '" + comune.Replace("'", "''").ToUpper() + "' ";

                string condition = string.Empty;
                if(!string.IsNullOrEmpty(conditionCap) || !string.IsNullOrEmpty(conditionComune))
                {
                    condition += "WHERE " + conditionCap;
                    if (!string.IsNullOrEmpty(conditionComune))
                        condition += !string.IsNullOrEmpty(conditionCap) ? " AND " + conditionComune : conditionComune;
                }
                q.setParam("condition", condition);
                query = q.getSQL();
                logger.Debug("QUERY " + query);
                DataSet ds = new DataSet();

                if (this.ExecuteQuery(out ds, "cap", query))
                {
                    if(ds.Tables["cap"] != null && ds.Tables["cap"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["cap"].Rows)
                        {
                            listaCap.Add(row["VAR_CAP"].ToString() + " - " + row["VAR_COMUNE"].ToString());
                        }
                    }
                }
                else
                {
                    logger.Error("Errore durante l'estrazione del CAP " + query);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore durante l'estrazione del CAP " + e.Message);
            }

            return listaCap;
        }

        public InfoComune GetCapComuni(string cap, string comune)
        {
            InfoComune info = new InfoComune();
            logger.Debug("GetCapComuni");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_CAP");
            string query;
            try
            {
                q.setParam("cap", cap);
                q.setParam("condComune", string.IsNullOrEmpty(comune) ? "" : " AND UPPER(VAR_COMUNE)= '" + comune.Replace("'", "''").ToUpper() + "'");
                query = q.getSQL();
                logger.Debug("QUERY " + query);
                DataSet ds = new DataSet();

                if (this.ExecuteQuery(out ds, "cap", query))
                {
                    if (ds.Tables["cap"] != null && ds.Tables["cap"].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["cap"].Rows[0];
                        info.CAP = row["VAR_CAP"].ToString();
                        info.COMUNE = row["VAR_COMUNE"].ToString();
                        info.PROVINCIA = row["VAR_PROVINCIA"].ToString();
                    }
                }
                else
                {
                    logger.Error("Errore durante l'estrazione del CAP " + query);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore durante l'estrazione del CAP " + e.Message);
            }

            return info;
        }

        public InfoComune GetProvinciaComune(string comune)
        {
            InfoComune cap = new InfoComune();
            logger.Debug("GetProvinciaComune");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_PROVINCIA_BY_COMUNE");
            string query;
            try
            {
                q.setParam("comune", comune.ToUpper());

                query = q.getSQL();
                logger.Debug("QUERY " + query);
                DataSet ds = new DataSet();

                if (this.ExecuteQuery(out ds, "provincia", query))
                {
                    if (ds.Tables["provincia"] != null && ds.Tables["provincia"].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables["provincia"].Rows[0];
                        cap.COMUNE = row["VAR_COMUNE"].ToString();
                        cap.PROVINCIA = row["VAR_PROVINCIA"].ToString();
                    }
                }
                else
                {
                    logger.Error("Errore durante l'estrazione della provincia" + query);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore durante l'estrazione della provincia " + e.Message);
            }

            return cap;
        }

        public List<string> GetListaComuni(string prefixComune)
        {
            List<string> listaComuni = new List<string>();
            logger.Debug("GetListaComuni");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_CAP_COMUNI_BY_COMUNE");
            string query;
            try
            {
                q.setParam("comune", prefixComune.Replace("'","''").ToUpper());

                query = q.getSQL();
                logger.Debug("QUERY " + query);
                DataSet ds = new DataSet();

                if (this.ExecuteQuery(out ds, "comune", query))
                {
                    if (ds.Tables["comune"] != null && ds.Tables["comune"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["comune"].Rows)
                        {
                            listaComuni.Add(row["VAR_COMUNE"].ToString());
                        }
                    }
                }
                else
                {
                    logger.Error("Errore durante l'estrazione dei Comuni " + query);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore durante l'estrazione dei comuni " + e.Message);
            }

            return listaComuni;
        }
        #endregion

        public DocsPaVO.utente.Utente GetUtenteAutomatico(string idAmministrazione)
        {
            Utente utente = new Utente();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PEOPLE_AUTOMATICO");
                q.setParam("idAmm", idAmministrazione);

                string sql = q.getSQL();
                utente = GetUserData(sql);
            }
            catch(Exception e)
            {
                logger.Error("Errore in GetUtenteAutomatico " + e.Message);
                utente = null;
            }
            return utente;
        }
    }

}

