using System;
using System.Collections;
using System.Data;
using DocsPaDbManagement.Functions;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Classe contenente tutte le query (12) di DocsPAWS > report
    /// </summary>
    public class Report : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(Report));
        #region DocsPaWS.report.RegistriStampa (12)

        /// <summary>
        /// Query #1 per il metodo "stampaRegistro"
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="registro"></param>
        public void GetLastDateReg(out DataSet dataSet, DocsPaVO.utente.Registro registro)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAELRegistri3");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("DTA_CLOSE", false));
            //DocsPaWS.Utils.dbControl.toChar("DTA_CLOSE",false));
            q.setParam("param2", registro.systemId);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.ExecuteQuery(out dataSet, "REG", queryString);
        }

        /// <summary>
        /// Query #2 per il metodo "stampaRegistro"
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="regRow"></param>
        /// <param name="debug"></param>
        public void GetStampReg(DataSet dataSet, System.Data.DataRow regRow)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAStampaReg");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("DTA_STAMPA", true));
            //DocsPaWS.Utils.dbControl.toChar("DTA_STAMPA",true));
            q.setParam("param2", regRow["SYSTEM_ID"].ToString());
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.ExecuteQuery(dataSet, "STAMPAREG", queryString);
        }

        /// <summary>
        /// Query #3 per il metodo "stampaRegistro"
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="regRow"></param>
        /// <param name="debug"></param>
        public void GetProfile(DataSet dataSet, System.Data.DataRow regRow)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Profile1");
            q.setParam("param1", regRow["SYSTEM_ID"].ToString());
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.ExecuteQuery(dataSet, "NUM_ANNO_PROTO", queryString);
        }

        /// <summary>
        /// Query #1 per il metodo "generaFile"
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="idReg"></param>
        /// <param name="debug"></param>
        public void GetCodReg(out DataSet dataSet, string idReg)
        {
            //DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAELRegistri4");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAELRegistri_AMMINISTRA");
            q.setParam("param1", idReg);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.ExecuteQuery(out dataSet, "REG", queryString);
        }

        /// <summary>
        /// Query #2 per il metodo "generaFile"
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="anno"></param>
        /// <param name="protStart"></param>
        /// <param name="protEnd"></param>
        /// <param name="idReg"></param>
        /// <param name="debug"></param>
        public void GetProfOgg(DataSet dataSet, int anno, int protStart, int protEnd, string idReg)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROFILE__DPA_OGGETTARIO");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_PROTO", false));
            //DocsPaWS.Utils.dbControl.toChar("A.DTA_PROTO",false));			
            q.setParam("param2", anno.ToString());
            q.setParam("param3", ">=" + protStart.ToString());
            q.setParam("param4", "<=" + protEnd.ToString());
            q.setParam("param5", idReg);

            string queryString = q.getSQL();

            // Reperimento stringa SQL ed aggiunta della colonna "DATA_ANNULLAMENTO"
            int indexOfFrom = queryString.IndexOf("FROM");
            queryString = queryString.Substring(0, indexOfFrom) + ", " + Environment.NewLine +
                        DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_ANNULLA", false) + " AS DATA_ANNULLAMENTO " + Environment.NewLine +
                        queryString.Substring(indexOfFrom);

            logger.Debug(queryString);
            this.ExecuteQuery(dataSet, "PROFILE", queryString);
        }



        ///
        public void GetAmministrazioneReg(out DataSet dataSet, string idReg)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAELRegistri_AMMINISTRA");
            q.setParam("param1", idReg);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.ExecuteQuery(out dataSet, "AMM", queryString);
        }

        public void GetProfOgg(DataSet dataSet, string idRegistro, DocsPaVO.filtri.FiltroRicerca[][] filterList)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROFILE__DPA_OGGETTARIO_WITH_FILTERS");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_PROTO", false));
            q.setParam("param2", idRegistro);
            q.setParam("param3", this.GetProfOggFilterString(filterList));
            q.setParam("param4", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_ANNULLA", false));
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.ExecuteQuery(dataSet, "PROFILE", queryString);
        }

        /// <summary>
        /// Costruzione stringa di filtro utilizzabile per l'esecuzione
        /// della query "S_J_PROFILE__DPA_OGGETTARIO_WITH_FILTERS"
        /// </summary>
        /// <param name="filterList"></param>
        /// <returns></returns>
        private string GetProfOggFilterString(DocsPaVO.filtri.FiltroRicerca[][] filterList)
        {
            System.Collections.Hashtable filters = new System.Collections.Hashtable();

            for (int i = 0; i < filterList.Length; i++)
            {
                for (int j = 0; j < filterList[i].Length; j++)
                {
                    DocsPaVO.filtri.FiltroRicerca filterItem = filterList[i][j];
                    if (filterItem.valore != null && !filterItem.valore.Equals(""))
                    {
                        if (filterItem.valore == "ProfilazioneDinamica")
                            filters.Add(filterItem.argomento, filterItem.template);
                        else
                            filters.Add(filterItem.argomento, filterItem.valore);
                    }
                    filterItem = null;
                }
            }

            // Composizione stringa di filtro
            string filterString = "";

            if (filters.ContainsKey("NUM_PROTOCOLLO"))
            {
                filterString += "(A.NUM_PROTO = " + (string)filters["NUM_PROTOCOLLO"] + ")";
            }
            else
            {
                // Filtro range per numero protocollo
                if (filters.ContainsKey("NUM_PROTOCOLLO_DAL") && filters.ContainsKey("NUM_PROTOCOLLO_AL"))
                    filterString += "(A.NUM_PROTO >= " + (string)filters["NUM_PROTOCOLLO_DAL"] + " AND A.NUM_PROTO <= " + (string)filters["NUM_PROTOCOLLO_AL"] + ")";

                // Filtro per numero protocollo singolo
                if (filters.ContainsKey("NUM_PROTOCOLLO_DAL") && !filters.ContainsKey("NUM_PROTOCOLLO_AL"))
                {
                    if (filterString != "")
                        filterString += " AND ";

                    filterString += "(A.NUM_PROTO >= " + (string)filters["NUM_PROTOCOLLO_DAL"] + ")";

                }
                // Filtro per numero protocollo singolo
                if (!filters.ContainsKey("NUM_PROTOCOLLO_DAL") && filters.ContainsKey("NUM_PROTOCOLLO_AL"))
                {
                    if (filterString != "")
                        filterString += " AND ";

                    filterString += "(A.NUM_PROTO <= " + (string)filters["NUM_PROTOCOLLO_AL"] + ")";

                }
            }
            if (filters.ContainsKey("DATA_PROT_IL"))
            {
                if (filterString != "")
                    filterString += " AND ";

                filterString += "(A.DTA_PROTO>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween((string)filters["DATA_PROT_IL"], true) + " AND A.DTA_PROTO<= " + DocsPaDbManagement.Functions.Functions.ToDateBetween((string)filters["DATA_PROT_IL"], false) + ")";

            }
            else
            {

                // Filtro range per data protocollo
                if (filters.ContainsKey("DATA_PROT_SUCCESSIVA_AL") && filters.ContainsKey("DATA_PROT_PRECEDENTE_IL"))
                {
                    if (filterString != "")
                        filterString += " AND ";

                    filterString += "(A.DTA_PROTO >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween((string)filters["DATA_PROT_SUCCESSIVA_AL"], true) + " AND A.DTA_PROTO <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween((string)filters["DATA_PROT_PRECEDENTE_IL"], false) + ")";
                }

                // Filtro per data protocollo singola
                if (filters.ContainsKey("DATA_PROT_SUCCESSIVA_AL") && !filters.ContainsKey("DATA_PROT_PRECEDENTE_IL"))
                {
                    if (filterString != "")
                        filterString += " AND ";

                    //filterString += "(A.DTA_PROTO = " + DocsPaDbManagement.Functions.Functions.ToDate((string) filters["DATA_PROT_SUCCESSIVA_AL"]) + ")";
                    filterString += " (A.DTA_PROTO>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween((string)filters["DATA_PROT_SUCCESSIVA_AL"], true) + ")";
                }

                // Filtro per data protocollo singola
                if (!filters.ContainsKey("DATA_PROT_SUCCESSIVA_AL") && filters.ContainsKey("DATA_PROT_PRECEDENTE_IL"))
                {
                    if (filterString != "")
                        filterString += " AND ";

                    //filterString += "(A.DTA_PROTO = " + DocsPaDbManagement.Functions.Functions.ToDate((string) filters["DATA_PROT_SUCCESSIVA_AL"]) + ")";
                    filterString += " (A.DTA_PROTO<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween((string)filters["DATA_PROT_PRECEDENTE_IL"], false) + ")";
                }
            }

            // Filtro per anno protocollo
            if (filters.ContainsKey("ANNO_PROTOCOLLO"))
            {
                if (filterString != "")
                    filterString += " AND ";

                filterString += "(A.NUM_ANNO_PROTO = " + (string)filters["ANNO_PROTOCOLLO"] + ")";
            }
            //luluciani filtro UO protocollatrice
            // Filtro per uo 
            if (filters.ContainsKey("ID_UO_PROT"))
            {
                if (filterString != "")
                    filterString += " AND ";

                filterString += "(A.ID_UO_PROT = " + (string)filters["ID_UO_PROT"] + ")";
            }
            // Filtro per uo e sue figlie
            if (filters.ContainsKey("ID_UO_PROT_GERARCHIA"))
            {
                if (filterString != "")
                    filterString += " AND ";

                DocsPaDB.Utils.Gerarchia g = new DocsPaDB.Utils.Gerarchia();
                DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();

                DocsPaVO.utente.UnitaOrganizzativa u = (DocsPaVO.utente.UnitaOrganizzativa)ut.GetCorrispondenteBySystemID(filters["ID_UO_PROT_GERARCHIA"].ToString());


                System.Collections.ArrayList l = g.getChildrenUO(u);
                string queryString = "( A.ID_UO_PROT IN (" + u.systemId;
                for (int i = 0; l != null && i < l.Count; i++)
                {


                    if (i == 0)
                        queryString += ","; // c'è id della uo padre.

                    queryString += l[i].ToString();

                    if (i < l.Count - 1)
                    {
                        if (i % 997 == 0 && i > 0)
                        {
                            queryString = queryString + ") OR A.ID_UO_PROT IN (";
                        }
                        else
                        {
                            queryString = queryString + ",";
                        }
                    }
                    else
                    {
                        queryString = queryString + " )";
                    }
                }
                filterString += queryString + " ) ";

                if (l != null && l.Count == 0)
                {
                    filterString += " ) ";
                }
            }

            // Filtro per la Tipologia Atto
            if (filters.ContainsKey("TIPO_ATTO"))
            {
                if (filterString != "")
                    filterString += " AND ";

                filterString += " A.ID_TIPO_ATTO = " + (string)filters["TIPO_ATTO"];
            }

            // Filtro per la Profilazione Dinamica
            if (filters.ContainsKey("PROFILAZIONE_DINAMICA"))
            {
                DocsPaVO.ProfilazioneDinamica.Templates template = (DocsPaVO.ProfilazioneDinamica.Templates)filters["PROFILAZIONE_DINAMICA"];
                DocsPaDB.Query_DocsPAWS.Model model = new Model();
                filterString += model.getSeriePerRicercaProfilazione(template, "");
            }

            filters = null;

            if (filterString != "")
                filterString = " AND " + filterString;

            return filterString;
        }

        /// <summary>
        /// Query #3 per il metodo "generaFile"
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="anno"></param>
        /// <param name="protStart"></param>
        /// <param name="idReg"></param>
        /// <param name="debug"></param>
        public void GetProfOgg2(out DataSet dataSet, int anno, int protStart, string idReg, string dataUltimaStampa)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROFILE__DPA_OGGETTARIO2");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_PROTO", false));
            //DocsPaWS.Utils.dbControl.toChar("A.DTA_PROTO",false));			
            q.setParam("param2", anno.ToString());
            q.setParam("param3", "< " + protStart.ToString());
            q.setParam("param4", idReg);
            q.setParam("param5", "> " + DocsPaDbManagement.Functions.Functions.ToDate(dataUltimaStampa));
            //q.setParam("param5","> "+DocsPaDbManagement.Functions.Functions.dataUltimaStampa));


            q.setParam("session_user", getUserDB());


            //DocsPaWS.Utils.dbControl.toDate(DocsPaWS.Utils.DateControl.toDateFromDB(dataUltimaStampa),true));				
            string queryString = q.getSQL();

            // Reperimento stringa SQL ed aggiunta della colonna "DATA_ANNULLAMENTO"
            int indexOfFrom = queryString.IndexOf("FROM");
            queryString = queryString.Substring(0, indexOfFrom) + ", " + Environment.NewLine +
                DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_ANNULLA", false) + " AS DATA_ANNULLAMENTO " + Environment.NewLine +
                queryString.Substring(indexOfFrom);

            logger.Debug(queryString);
            this.ExecuteQuery(out dataSet, "PROTO_VARIATI", queryString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string getUserDB()
        {
            return Functions.GetDbUserSession();
        }




        /// <summary>
        /// Query per il metodo "maxNumProto"
        /// </summary>
        /// <param name="anno"></param>
        /// <param name="idReg"></param>
        /// <param name="debug"></param>
        public void MaxNrProt(out DataSet dataSet, int anno, string idReg)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Profile3");
            q.setParam("param1", idReg);
            q.setParam("param2", anno.ToString());
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.ExecuteQuery(out dataSet, "NUM_PROTO", queryString);
        }

        /// <summary>
        /// Query per il metodo "getHash"
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="docnumber"></param>
        public void GetImpVers(out DataSet dataSet, string docnumber)
        {
            string UserDB = string.Empty;

            //DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_COMPONENTS__VERSIONS");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_GETIMPRONTA_W_ALL");

            if (dbType.ToUpper() == "SQL")
            {
                UserDB = getUserDB();
                q.setParam("dbuser", UserDB);
            }
            q.setParam("param1", docnumber);
            string queryString = q.getSQL();
            this.ExecuteQuery(out dataSet, "HASH", queryString);
        }

        /// <summary>
        /// Query per il metodo "insertDocumentale"
        /// </summary>
        /// <param name="sd"></param>
        /// <param name="idReg"></param>
        /// <param name="docNumber"></param>
        public void UpdProf(DocsPaVO.documento.SchedaDocumento sd, string idReg, string docNumber)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_Profile1");
            q.setParam("param1", sd.oggetto.systemId);
            q.setParam("param2", idReg);
            q.setParam("param3", docNumber);
            q.setParam("param4", "'" + sd.oggetto.descrizione + "'");
            q.setParam("param5", "'Stampa registro: " + docNumber + "'");

            // La query U_Profile1 imposta tipo proto a R, per renderla compatibile con
            // le stampe repertori il valore viene passato da codice. Viene controllato
            // preventivamente se il tipo proto della scheda è null ed in questo caso
            // viene impostato a R
            if (String.IsNullOrEmpty(sd.tipoProto))
                sd.tipoProto = "R";

            q.setParam("tipoProto", sd.tipoProto);

            string queryString = q.getSQL();
            this.ExecuteNonQuery(queryString);
        }

        /// <summary>
        /// Query per il metodo "isVariato"
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public string GetDateOgg(System.Data.DataRow profile)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAOggSto");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("DTA_MODIFICA", true));
            //DocsPaWS.Utils.dbControl.toChar("DTA_MODIFICA",true));						
            q.setParam("param2", profile["SYSTEM_ID"].ToString());
            string queryString = q.getSQL();
            logger.Debug(queryString);
            string res = "";
            this.ExecuteScalar(out res, queryString);
            return res;
        }

        /// <summary>
        /// Query #2 per il metodo "isVariato"
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public string GetDateCorr(System.Data.DataRow profile)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPACorrSto");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("DTA_MODIFICA", true));
            //DocsPaWS.Utils.dbControl.toChar("DTA_MODIFICA",true));						
            q.setParam("param2", profile["SYSTEM_ID"].ToString());
            string queryString = q.getSQL();
            logger.Debug(queryString);
            string res = "";
            this.ExecuteScalar(out res, queryString);
            return res;
        }

        /// <summary>
        /// Query per il metodo "aggiornaProfile"
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="anno"></param>
        /// <param name="protStart"></param>
        /// <param name="protEnd"></param>
        /// <param name="idReg"></param>
        /// <param name="debug"></param>
        public void GetProfil(out DataSet dataSet, int anno, int protStart, int protEnd, string idReg)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Profile2");
            q.setParam("param1", anno.ToString());
            q.setParam("param2", ">=" + protStart.ToString());
            q.setParam("param3", "<=" + protEnd.ToString());
            q.setParam("param4", idReg);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.ExecuteQuery(out dataSet, "PROFILE", queryString);
        }

        public System.Collections.ArrayList GetCorrispondenti(string idDocProto)
        {
            logger.Debug("getCorrispondenti");
            System.Collections.ArrayList mittDest = new System.Collections.ArrayList();
            Documenti doc = new Documenti();
            string queryString = doc.GetQueryCorrispondente(idDocProto);
            //string queryString = DocManager.getQueryCorrispondente(idDocProto);	
            logger.Debug(queryString);
            /*IDataReader dr = db.executeReader(queryString);
            while (dr.Read()) 
            {		
                if(dr.GetValue(5).ToString().Equals("M")||dr.GetValue(5).ToString().Equals("D"))
                {
                    mittDest.Add(DocManager.getCorrispondente(dr).descrizione);
                }
                if(dr.GetValue(5).ToString().Equals("C"))
                {
                    mittDest.Add(DocManager.getCorrispondente(dr).descrizione+" (P.C.)");
                }
            }
            dr.Close();*/
            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);
            DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();

            try
            /*   OLD
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    if(row[5].ToString().Equals("M")||row[5].ToString().Equals("D"))
                    {
                        //mittDest.Add(DocManager.getCorrispondente(dr).descrizione);
                        if ((doc.GetCorrispondente(row).descrizione) == null)
                        {
                            return null;
                        }
                        else
                        {
                            mittDest.Add(doc.GetCorrispondente(row).descrizione);
                        }
                    }
                    if(row[5].ToString().Equals("C"))
                    {
                        //mittDest.Add(DocManager.getCorrispondente(dr).descrizione+" (P.C.)");
                        //mittDest.Add(doc.GetCorrispondente(row).descrizione+" (P.C.)");
                        if (doc.GetCorrispondente(row).descrizione+" (P.C.)" == null)
                        {
                            return null;
                        }
                        else
                        {
                            mittDest.Add(doc.GetCorrispondente(row).descrizione+" (P.C.)");
                        }
                    }
                }
            }   */

            // NEW
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    if ((doc.GetCorrispondente(row).descrizione == null)
                        || (doc.GetCorrispondente(row).descrizione + " (P.C.)" == null))
                    {
                        return null;
                    }

                    if (row["CHA_TIPO_PROTO"].Equals("A"))
                    {
                        if (row["CHA_TIPO_MITT_DEST"].Equals("M") || row["CHA_TIPO_MITT_DEST"].Equals("MD"))
                        {
                            mittDest.Add(doc.GetCorrispondente(row).descrizione);
                        }
                    }

                    if (row["CHA_TIPO_PROTO"].Equals("P"))
                    {
                        if (row["CHA_TIPO_MITT_DEST"].Equals("D"))
                        {
                            mittDest.Add(doc.GetCorrispondente(row).descrizione);
                        }

                        if (row["CHA_TIPO_MITT_DEST"].Equals("C"))
                        {
                            mittDest.Add(doc.GetCorrispondente(row).descrizione + " (P.C.)");
                        }
                    }

                    if (row["CHA_TIPO_PROTO"].Equals("I"))
                    {
                        if (row["CHA_TIPO_MITT_DEST"].Equals("M"))
                        {
                            mittDest.Add(doc.GetCorrispondente(row).descrizione);
                        }
                    }

                }
            }


            catch (Exception exception)
            {
                logger.Debug(exception.ToString());
            }

            return mittDest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idReg"></param>
        /// <param name="anno"></param>
        /// <param name="protStart"></param>
        /// <param name="protEnd"></param>
        public bool AggiornaProfile(string idReg, int anno, int protStart, int protEnd)
        {
            bool result = true;
            //logger.Debug("aggiornaProfile");
            //DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
            //System.Data.DataSet ds=new System.Data.DataSet();
            System.Data.DataSet ds;
            //bool dbOpen=false;
            //bool dbTrans=false;

            try
            {
                System.Collections.ArrayList updateQueries = new System.Collections.ArrayList();

                //db.openConnection();
                //dbOpen=true;
                /*
                string profileString ="SELECT DISTINCT A.SYSTEM_ID " +
                    "FROM PROFILE A "+
                    "WHERE " +
                    "A.CHA_TIPO_PROTO IN('A', 'P', 'I') " + 
                    "AND A.NUM_ANNO_PROTO="+anno+" AND A.NUM_PROTO>="+protStart+" AND A.NUM_PROTO<="+protEnd+" AND A.ID_REGISTRO="+idReg;		
                profileString += " ORDER BY A.SYSTEM_ID";
                logger.Debug(profileString);
                db.fillTable(profileString,ds,"PROFILE");
                */

                GetProfil(out ds, anno, protStart, protEnd, idReg);

                for (int i = 0; i < ds.Tables["PROFILE"].Rows.Count; i++)
                {
                    // TODO: vedere migliorie effettuate sulla versione 2.0
                    string updateQuery = "UPDATE PROFILE SET CHA_CONGELATO='1' WHERE SYSTEM_ID=" + ds.Tables["PROFILE"].Rows[i]["SYSTEM_ID"];
                    updateQueries.Add(updateQuery);
                    logger.Debug("Aggiunta query: " + updateQuery);
                }

                //si esegue l'update
                //db.beginTransaction();
                this.BeginTransaction();
                //dbTrans=true;

                for (int i = 0; i < updateQueries.Count; i++)
                {
                    this.ExecuteNonQuery((string)updateQueries[i]);
                    logger.Debug("Eseguito update " + i);
                }

                //db.commitTransaction();
                this.CommitTransaction();
            }
            catch (Exception)
            {
                /*
                if(dbTrans)
                {
                    db.rollbackTransaction();
                }
				
                if(dbOpen)
                {
                    db.closeConnection();
                }
                */
                this.RollbackTransaction();
                result = false;
                //throw e;
            }
            return result;
        }

        public bool AggiornaStampaReg(string idReg, int protStart, int protEnd, int anno, string docNumber)
        {
            bool result = true;
            bool dbOpen = false;
            try
            {
                this.OpenConnection();
                dbOpen = true;
                DateTime now = System.DateTime.Now;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPAStampaRegistri");
                q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName() +
                    "ID_REGISTRO,NUM_PROTO_START,NUM_PROTO_END,NUM_ANNO,NUM_ORD_FILE,DOCNUMBER,DTA_STAMPA");
                q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_STAMPAREGISTRI") + idReg + "," + protStart + "," + protEnd + "," + anno + ",1," + docNumber + "," + DocsPaDbManagement.Functions.Functions.GetDate());
                string queryString = q.getSQL();
                logger.Debug(queryString);
                this.ExecuteNonQuery(queryString);
            }
            catch (Exception)
            {
                if (dbOpen)
                {
                    this.CloseConnection();
                }
                result = false;
                //throw e;
            }
            return result;
        }

        public void getDocProtocollati(out DataSet ds, string idReg, int anno, int protStart, int protEnd)
        {
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("STAMPA_REGISTRI_01");
            q.setParam("param1", idReg);
            q.setParam("param2", Convert.ToString(protStart));
            q.setParam("param3", Convert.ToString(protEnd));
            q.setParam("param4", Convert.ToString(anno));


            //modifica
            //q.setParam("impronta_cache","ca.VAR_IMPRONTA as IMPRONTA_CACHE,");
            //q.setParam("left_join_cache", "LEFT JOIN DPA_CACHE ca on ca.docnumber=p.docnumber");
            //fine modifica


            q.setParam("param5", getUserDB());


            string query = q.getSQL();
            System.Diagnostics.Debug.WriteLine("Esecuzione STAMPA_REGISTRI_01 : " + query);
            this.ExecuteQuery(out ds, query);
        }

        ////modifica
        //public void getDocProtocollatiCache(out DataSet ds, string idReg, int anno, int protStart, int protEnd)
        //{
        //    string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
        //    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("STAMPA_REGISTRI_CACHE");
        //    q.setParam("param1", idReg);
        //    q.setParam("param2", Convert.ToString(protStart));
        //    q.setParam("param3", Convert.ToString(protEnd));
        //    q.setParam("param4", Convert.ToString(anno));
        //    q.setParam("param5", getUserDB());
        //    string query = q.getSQL();
        //    System.Diagnostics.Debug.WriteLine("Esecuzione STAMPA_REGISTRI_CACHE : " + query);
        //    this.ExecuteQuery(out ds, query);
        //}
        ////modifica

        public void getDocProtocollatiWithFilters(out DataSet ds, DocsPaVO.filtri.FiltroRicerca[][] filters, DocsPaVO.utente.Registro registro)
        {
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];

            string stringFilters = this.GetProfOggFilterString(filters);
            stringFilters = stringFilters.Replace("A.", "P.");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("STAMPA_RAPPORTI_01");
            q.setParam("param1", registro.systemId);
            q.setParam("param2", stringFilters);


            q.setParam("param5", getUserDB());


            string query = q.getSQL();
            System.Diagnostics.Debug.WriteLine("Esecuzione STAMPA_RAPPORTI_01 : " + query);
            this.ExecuteQuery(out ds, query);
        }

        public void getDestProtUscitaWithFilters(out DataSet ds, DocsPaVO.filtri.FiltroRicerca[][] filters, DocsPaVO.utente.Registro registro)
        {
            string stringFilters = this.GetProfOggFilterString(filters);
            stringFilters = stringFilters.Replace("A.", "P.");
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_STAMPA_BUSTE");
            q.setParam("param1", registro.systemId);
            q.setParam("param2", stringFilters);

            string query = q.getSQL();
            System.Diagnostics.Debug.WriteLine("Esecuzione S_STAMPA_BUSTE : " + query);
            this.ExecuteQuery(out ds, query);
        }

        public string getNomeAmmFromPeople(string idPeople)
        {
            string nomeAmministrazione = string.Empty;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_NOME_AMM_DA_PEOPLE");
            q.setParam("param1", idPeople);
            string query = q.getSQL();
            logger.Debug("getNomeAmmFromPeople : " + query);
            this.ExecuteScalar(out nomeAmministrazione, query);

            return nomeAmministrazione;
        }

        /// <summary>
        /// GADAMO - 22/05/2008
        /// Reperisce i dati per la generazione della stampa Report Avanzati in XLS
        /// 
        ///                         A  T  T  E  Z  I  O  N  E 
        ///                         
        ///             AL MOMENTO SONO STATE SVILUPPATE SOLO LE QUERY PER 
        ///             ORACLE POICHE' TALE FUNZIONALITA' E' STATA IMPLEMENTATA
        ///             A SEGUITO DELLE RICHIESTE DEL MINISTERO DELLA SANITA'
        ///             
        /// 
        /// </summary>
        /// <param name="obj">obj stampaReportXLS con i filtri impostati dall'utente</param>
        /// <returns>Arraylist dati</returns>
        public ArrayList GetQueryReportAvanzatiXLS(DocsPaVO.ExportData.ExportExcelClass objReport)
        {
            string condWhere = string.Empty;
            System.Data.DataSet dataSet = new System.Data.DataSet();
            ArrayList returnList = new ArrayList();
            DocsPaUtils.Query q = null;
            string commandText = string.Empty;
            DataSet ds = new DataSet();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaVO.ExportData.ExportDataExcel objDati;
            bool protoIntEnabled = false;

            switch (objReport.filtro.tipologiaReport)
            {
                case "TX_R":
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_1");
                    condWhere += "AND id_registro IN (" + objReport.filtro.idRegistro.Replace('_', ',') + ") ";
                    if (objReport.filtro.idRuolo != "0")
                        condWhere += "AND tx.id_ruolo_in_uo IN (select system_id from dpa_corr_globali where id_tipo_ruolo = " + objReport.filtro.idRuolo + " and cha_tipo_urp = 'R' and cha_tipo_ie = 'I') ";
                    if (objReport.filtro.idRagTrasm != "0")
                        condWhere += "AND ts.id_ragione = " + objReport.filtro.idRagTrasm + " ";
                    if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA != string.Empty)
                        condWhere += "AND tx.dta_invio BETWEEN" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataDa, true) + "AND" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataA, false);
                    else if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA.Equals(string.Empty))
                        condWhere += "AND" + DocsPaDbManagement.Functions.Functions.ToChar("tx.dta_invio", false) + "=" + DocsPaDbManagement.Functions.Functions.ToChar(DocsPaDbManagement.Functions.Functions.ToDate(objReport.filtro.dataDa, false), false);

                    q.setParam("condWhere", condWhere);
                    commandText = q.getSQL();
                    dbProvider.ExecuteQuery(ds, commandText);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        foreach (DataRow rowTable in ds.Tables[0].Rows)
                        {
                            objDati = new DocsPaVO.ExportData.ExportDataExcel();

                            objDati.etichetta_dato1 = "REGISTRO";
                            objDati.dato1 = rowTable["REGISTRO"].ToString();
                            objDati.etichetta_dato2 = "RUOLO";
                            objDati.dato2 = rowTable["RUOLO"].ToString();
                            objDati.etichetta_dato3 = "UTENTE";
                            objDati.dato3 = rowTable["UTENTE"].ToString();
                            objDati.etichetta_dato4 = "RAGIONE TRASM.";
                            objDati.dato4 = rowTable["RAGIONE"].ToString();
                            objDati.etichetta_dato5 = "TRASM. EFFETT.";
                            objDati.dato5 = rowTable["TOT"].ToString();

                            returnList.Add(objDati);
                        }
                    }

                    break;
                case "TX_P":
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_2");
                    condWhere += "AND id_registro IN (" + objReport.filtro.idRegistro.Replace('_', ',') + ") ";
                    if (objReport.filtro.idRuolo != "0")
                        condWhere += "AND id_ruolo_dest in (select id_gruppo from dpa_corr_globali where id_tipo_ruolo = " + objReport.filtro.idRuolo + ") ";
                    if (objReport.filtro.idRagTrasm != "0")
                        condWhere += "AND id_ragione_trasm = " + objReport.filtro.idRagTrasm + " ";
                    if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA != string.Empty)
                        condWhere += "AND dta_invio BETWEEN" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataDa, true) + "AND" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataA, false);
                    else if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA.Equals(string.Empty))
                        condWhere += "AND" + DocsPaDbManagement.Functions.Functions.ToChar("dta_invio", false) + "=" + DocsPaDbManagement.Functions.Functions.ToChar(DocsPaDbManagement.Functions.Functions.ToDate(objReport.filtro.dataDa, false), false);

                    q.setParam("condWhere", condWhere);
                    commandText = q.getSQL();
                    dbProvider.ExecuteQuery(ds, commandText);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        foreach (DataRow rowTable in ds.Tables[0].Rows)
                        {
                            objDati = new DocsPaVO.ExportData.ExportDataExcel();

                            objDati.etichetta_dato1 = "REGISTRO";
                            objDati.dato1 = rowTable["REGISTRO"].ToString();
                            objDati.etichetta_dato2 = "RUOLO";
                            objDati.dato2 = rowTable["RUOLO"].ToString();
                            objDati.etichetta_dato3 = "RAGIONE TRASM.";
                            objDati.dato3 = rowTable["RAGIONE"].ToString();
                            objDati.etichetta_dato4 = "TRASM. PENDENTI";
                            objDati.dato4 = rowTable["TOT"].ToString();

                            returnList.Add(objDati);
                        }
                    }

                    break;
                case "PR_REG":
                    protoIntEnabled = this.isProtoIntEnabled(objReport.filtro.idAmministrazione);

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_3");
                    condWhere += "AND id_registro IN (" + objReport.filtro.idRegistro.Replace('_', ',') + ") ";
                    if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA != string.Empty)
                        condWhere += "AND dta_proto BETWEEN" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataDa, true) + "AND" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataA, false);
                    else if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA.Equals(string.Empty))
                        condWhere += "AND" + DocsPaDbManagement.Functions.Functions.ToChar("dta_proto", false) + "=" + DocsPaDbManagement.Functions.Functions.ToChar(DocsPaDbManagement.Functions.Functions.ToDate(objReport.filtro.dataDa, false), false);

                    q.setParam("condWhere", condWhere);
                    if (protoIntEnabled)
                    {
                        q.setParam("protInterni", "sum(getprotocollato(system_id,'I','1')) AS INTERNI,");
                        q.setParam("abilProtInt", "1");
                    }
                    else
                    {
                        q.setParam("protInterni", "");
                        q.setParam("abilProtInt", "0");
                    }
                    commandText = q.getSQL();
                    dbProvider.ExecuteQuery(ds, commandText);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        foreach (DataRow rowTable in ds.Tables[0].Rows)
                        {
                            objDati = new DocsPaVO.ExportData.ExportDataExcel();

                            objDati.etichetta_dato1 = "REGISTRO";
                            objDati.dato1 = rowTable["REGISTRO"].ToString();
                            objDati.etichetta_dato2 = "PROTOCOLLATI";
                            objDati.dato2 = rowTable["PROTOCOLLATI"].ToString();
                            objDati.etichetta_dato3 = "ARRIVO";
                            objDati.dato3 = rowTable["ARRIVO"].ToString();
                            objDati.etichetta_dato4 = "PARTENZA";
                            objDati.dato4 = rowTable["PARTENZA"].ToString();
                            objDati.etichetta_dato5 = "FASCICOLATI";
                            objDati.dato5 = rowTable["FASCICOLATI"].ToString();
                            objDati.etichetta_dato6 = "ANNULLATI";
                            objDati.dato6 = rowTable["ANNULLATI"].ToString();
                            objDati.etichetta_dato7 = "IMMAGINE";
                            objDati.dato7 = rowTable["IMMAGINE"].ToString();

                            if (protoIntEnabled)
                            {
                                objDati.etichetta_dato10 = "INTERNI";
                                objDati.dato10 = rowTable["INTERNI"].ToString();
                            }

                            returnList.Add(objDati);
                        }
                    }

                    break;
                case "PR_REG_R":
                    protoIntEnabled = this.isProtoIntEnabled(objReport.filtro.idAmministrazione);

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_4");
                    condWhere += "AND id_registro IN (" + objReport.filtro.idRegistro.Replace('_', ',') + ") ";
                    if (objReport.filtro.idRuolo != "0")
                        condWhere += "AND id_ruolo_prot IN (select system_id from dpa_corr_globali where id_tipo_ruolo = " + objReport.filtro.idRuolo + " and cha_tipo_urp = 'R' and cha_tipo_ie = 'I') ";
                    if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA != string.Empty)
                        condWhere += "AND dta_proto BETWEEN" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataDa, true) + "AND" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataA, false);
                    else if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA.Equals(string.Empty))
                        condWhere += "AND" + DocsPaDbManagement.Functions.Functions.ToChar("dta_proto", false) + "=" + DocsPaDbManagement.Functions.Functions.ToChar(DocsPaDbManagement.Functions.Functions.ToDate(objReport.filtro.dataDa, false), false);

                    q.setParam("condWhere", condWhere);
                    if (protoIntEnabled)
                    {
                        q.setParam("protInterni", "sum(getprotocollato(system_id,'I','1')) AS INTERNI,");
                        q.setParam("abilProtInt", "1");
                    }
                    else
                    {
                        q.setParam("protInterni", "");
                        q.setParam("abilProtInt", "0");
                    }
                    commandText = q.getSQL();
                    dbProvider.ExecuteQuery(ds, commandText);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        foreach (DataRow rowTable in ds.Tables[0].Rows)
                        {
                            objDati = new DocsPaVO.ExportData.ExportDataExcel();

                            objDati.etichetta_dato1 = "REGISTRO";
                            objDati.dato1 = rowTable["REGISTRO"].ToString();
                            objDati.etichetta_dato2 = "RUOLO";
                            objDati.dato2 = rowTable["RUOLO"].ToString();
                            objDati.etichetta_dato3 = "PROTOCOLLATI";
                            objDati.dato3 = rowTable["PROTOCOLLATI"].ToString();
                            objDati.etichetta_dato4 = "ARRIVO";
                            objDati.dato4 = rowTable["ARRIVO"].ToString();
                            objDati.etichetta_dato5 = "PARTENZA";
                            objDati.dato5 = rowTable["PARTENZA"].ToString();
                            objDati.etichetta_dato6 = "FASCICOLATI";
                            objDati.dato6 = rowTable["FASCICOLATI"].ToString();
                            objDati.etichetta_dato7 = "ANNULLATI";
                            objDati.dato7 = rowTable["ANNULLATI"].ToString();
                            objDati.etichetta_dato8 = "IMMAGINE";
                            objDati.dato8 = rowTable["IMMAGINE"].ToString();

                            if (protoIntEnabled)
                            {
                                objDati.etichetta_dato10 = "INTERNI";
                                objDati.dato10 = rowTable["INTERNI"].ToString();
                            }

                            returnList.Add(objDati);
                        }
                    }
                    break;
                /********************************************************************************/
                //Giordano Iacozzilli: Gestire in report con una sola query è complesso, per celerità ho diviso 
                //la query in 4 sotto query che andrò a sistemare in una datatable.
                case "PFCNC_REG_R":
                    protoIntEnabled = this.isProtoIntEnabled(objReport.filtro.idAmministrazione);
                    /********************************************************************************/
                    //Il primo set di dati sono i protocolli in Uscita ed Entrata più gli altri campi utili valorizzati a ZERO
                    /********************************************************************************/
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_5_MAIN");
                    condWhere += "AND id_registro IN (" + objReport.filtro.idRegistro.Replace('_', ',') + ") ";
                    if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA != string.Empty)
                        condWhere += "AND dta_proto BETWEEN" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataDa, true) + "AND" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataA, false);
                    else if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA.Equals(string.Empty))
                        condWhere += "AND" + DocsPaDbManagement.Functions.Functions.ToChar("dta_proto", false) + "=" + DocsPaDbManagement.Functions.Functions.ToChar(DocsPaDbManagement.Functions.Functions.ToDate(objReport.filtro.dataDa, false), false);

                    q.setParam("condWhere", condWhere);

                    commandText = q.getSQL();
                    dbProvider.ExecuteQuery(ds, commandText);
                    DataTable _dtMAIN = new DataTable();
                    _dtMAIN = ds.Tables[0];

                    /********************************************************************************/
                    //Il secondo set di dati sono i Fascicolati dei protocolli in Uscita ed Entrata
                    /********************************************************************************/
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_5_FASCICOLATI");

                    q.setParam("destinazione", "A");
                    q.setParam("condWhere", condWhere);
                    commandText = q.getSQL();
                    ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    DataTable _dtFascicoliIN = new DataTable();
                    _dtFascicoliIN = ds.Tables[0];
                    _dtFascicoliIN.Columns[1].ColumnName = "FASCICOLATI IN ARRIVO";

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_5_FASCICOLATI");
                    q.setParam("destinazione", "P");
                    q.setParam("condWhere", condWhere);
                    commandText = q.getSQL();
                    ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    DataTable _dtFascicoliOUT = new DataTable();
                    _dtFascicoliOUT = ds.Tables[0];
                    _dtFascicoliOUT.Columns[1].ColumnName = "FASCICOLATI IN PARTENZA";
                    /********************************************************************************/
                    //Il terzo set di dati sono i Classificati dei protocolli in Uscita ed Entrata
                    /********************************************************************************/
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_5_CLASSIFICATI");

                    q.setParam("destinazione", "A");
                    q.setParam("condWhere", condWhere);
                    commandText = q.getSQL();
                    ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    DataTable _dtClassificatiIN = new DataTable();
                    _dtClassificatiIN = ds.Tables[0];
                    _dtClassificatiIN.Columns[1].ColumnName = "CLASSIFICATI IN ARRIVO";

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_5_CLASSIFICATI");
                    q.setParam("destinazione", "P");
                    q.setParam("condWhere", condWhere);
                    commandText = q.getSQL();
                    ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    DataTable _dtClassificatiOUT = new DataTable();
                    _dtClassificatiOUT = ds.Tables[0];
                    _dtClassificatiOUT.Columns[1].ColumnName = "CLASSIFICATI IN PARTENZA";

                    //MERGE ALL TABLE

                    _dtMAIN.PrimaryKey = new DataColumn[] { _dtMAIN.Columns["REGISTRO"] };

                    DataTable DtJoin4Tables = _dtMAIN.Copy();
                    DtJoin4Tables.Merge(_dtFascicoliIN, false, MissingSchemaAction.Add);
                    DtJoin4Tables.AcceptChanges();
                    DtJoin4Tables.Merge(_dtFascicoliOUT, false, MissingSchemaAction.Add);
                    DtJoin4Tables.AcceptChanges();
                    DtJoin4Tables.Merge(_dtClassificatiIN, false, MissingSchemaAction.Add);
                    DtJoin4Tables.AcceptChanges();
                    DtJoin4Tables.Merge(_dtClassificatiOUT, false, MissingSchemaAction.Add);
                    DtJoin4Tables.AcceptChanges();

                    if (DtJoin4Tables.Rows.Count != 0)
                    {
                        foreach (DataRow rowTable in DtJoin4Tables.Rows)
                        {
                            objDati = new DocsPaVO.ExportData.ExportDataExcel();

                            objDati.etichetta_dato1 = "REGISTRO";
                            objDati.dato1 = rowTable["REGISTRO"].ToString();

                            objDati.etichetta_dato2 = "PROTOCOLLATI IN ARRIVO";
                            objDati.dato2 = rowTable["PROTOCOLLATI IN ARRIVO"].ToString();

                            objDati.etichetta_dato3 = "FASCICOLATI IN ARRIVO";
                            objDati.dato3 = rowTable["FASCICOLATI IN ARRIVO"].ToString();

                            objDati.etichetta_dato4 = "CLASSIFICATI IN ARRIVO";
                            objDati.dato4 = rowTable["CLASSIFICATI IN ARRIVO"].ToString();

                            objDati.etichetta_dato5 = "PROTOCOLLATI IN PARTENZA";
                            objDati.dato5 = rowTable["PROTOCOLLATI IN PARTENZA"].ToString();

                            objDati.etichetta_dato6 = "FASCICOLATI IN PARTENZA";
                            objDati.dato6 = rowTable["FASCICOLATI IN PARTENZA"].ToString();

                            objDati.etichetta_dato7 = "CLASSIFICATI IN PARTENZA";
                            objDati.dato7 = rowTable["CLASSIFICATI IN PARTENZA"].ToString();

                            returnList.Add(objDati);
                        }
                    }
                    break;
                case "INTEROP_REG":
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_6_MAIN");
                    condWhere += "AND id_registro IN (" + objReport.filtro.idRegistro.Replace('_', ',') + ") ";
                    if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA != string.Empty)
                        condWhere += "AND dta_proto BETWEEN" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataDa, true) + "AND" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataA, false);
                    else if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA.Equals(string.Empty))
                        condWhere += "AND" + DocsPaDbManagement.Functions.Functions.ToChar("dta_proto", false) + "=" + DocsPaDbManagement.Functions.Functions.ToChar(DocsPaDbManagement.Functions.Functions.ToDate(objReport.filtro.dataDa, false), false);

                    q.setParam("condWhere", condWhere);

                    commandText = q.getSQL();
                    dbProvider.ExecuteQuery(ds, commandText);
                    DataTable _dtINTEROP_REGMAIN = new DataTable();
                    _dtINTEROP_REGMAIN = ds.Tables[0];

                    /********************************************************************************/
                    //Il secondo set di dati sono gli SPEDITI INETROPERABILITA INTERNA/ESTERNA
                    /********************************************************************************/
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_6_SPEDITI");
                    //reset where
                    condWhere = string.Empty;
                    condWhere += "AND id_registro IN (" + objReport.filtro.idRegistro.Replace('_', ',') + ") ";
                    if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA != string.Empty)
                        condWhere += "AND dta_spedizione BETWEEN" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataDa, true) + "AND" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataA, false);
                    else if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA.Equals(string.Empty))
                        condWhere += "AND" + DocsPaDbManagement.Functions.Functions.ToChar("dta_spedizione", false) + "=" + DocsPaDbManagement.Functions.Functions.ToChar(DocsPaDbManagement.Functions.Functions.ToDate(objReport.filtro.dataDa, false), false);


                    q.setParam("destinazione", "E");
                    q.setParam("condWhere", condWhere);
                    commandText = q.getSQL();
                    ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    DataTable _dtSPEDITI_INT_ESTERNA = new DataTable();
                    _dtSPEDITI_INT_ESTERNA = ds.Tables[0];
                    _dtSPEDITI_INT_ESTERNA.Columns[1].ColumnName = "SPEDITI INT. ESTERNA";

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_6_SPEDITI");
                    q.setParam("destinazione", "I");
                    q.setParam("condWhere", condWhere);
                    commandText = q.getSQL();
                    ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    DataTable _dtSPEDITI_INT_INTERNA = new DataTable();
                    _dtSPEDITI_INT_INTERNA = ds.Tables[0];
                    _dtSPEDITI_INT_INTERNA.Columns[1].ColumnName = "SPEDITI INT. INTERNA";
                    /********************************************************************************/
                    //Il terzo set di dati sono i RICEVUTI INTRNI/ESTERNI
                    /********************************************************************************/
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_6_RICEVUTI");
                    //reset where
                    condWhere = string.Empty;
                    condWhere += "AND id_registro IN (" + objReport.filtro.idRegistro.Replace('_', ',') + ") ";
                    if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA != string.Empty)
                        condWhere += "AND dta_proto BETWEEN" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataDa, true) + "AND" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataA, false);
                    else if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA.Equals(string.Empty))
                        condWhere += "AND" + DocsPaDbManagement.Functions.Functions.ToChar("dta_proto", false) + "=" + DocsPaDbManagement.Functions.Functions.ToChar(DocsPaDbManagement.Functions.Functions.ToDate(objReport.filtro.dataDa, false), false);

                    q.setParam("destinazione", "I");
                    q.setParam("condWhere", condWhere);
                    commandText = q.getSQL();
                    ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    DataTable _dtRICEVUTI_INT_INTERNA = new DataTable();
                    _dtRICEVUTI_INT_INTERNA = ds.Tables[0];
                    _dtRICEVUTI_INT_INTERNA.Columns[1].ColumnName = "RICEVUTI INT. INTERNA";

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_6_RICEVUTI");
                    q.setParam("destinazione", "S','E','P");
                    q.setParam("condWhere", condWhere);
                    commandText = q.getSQL();
                    ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    DataTable _dtRICEVUTI_INT_ESTERNA = new DataTable();
                    _dtRICEVUTI_INT_ESTERNA = ds.Tables[0];
                    _dtRICEVUTI_INT_ESTERNA.Columns[1].ColumnName = "RICEVUTI INT. ESTERNA";

                    //MERGE ALL TABLE
                    _dtINTEROP_REGMAIN.PrimaryKey = new DataColumn[] { _dtINTEROP_REGMAIN.Columns["REGISTRO"] };

                    DataTable DtJoin4TablesMainRep6 = _dtINTEROP_REGMAIN.Copy();
                    DtJoin4TablesMainRep6.Merge(_dtSPEDITI_INT_ESTERNA, false, MissingSchemaAction.Add);
                    DtJoin4TablesMainRep6.AcceptChanges();
                    DtJoin4TablesMainRep6.Merge(_dtSPEDITI_INT_INTERNA, false, MissingSchemaAction.Add);
                    DtJoin4TablesMainRep6.AcceptChanges();
                    DtJoin4TablesMainRep6.Merge(_dtRICEVUTI_INT_INTERNA, false, MissingSchemaAction.Add);
                    DtJoin4TablesMainRep6.AcceptChanges();
                    DtJoin4TablesMainRep6.Merge(_dtRICEVUTI_INT_ESTERNA, false, MissingSchemaAction.Add);
                    DtJoin4TablesMainRep6.AcceptChanges();

                    if (DtJoin4TablesMainRep6.Rows.Count != 0)
                    {
                        foreach (DataRow rowTable in DtJoin4TablesMainRep6.Rows)
                        {
                            objDati = new DocsPaVO.ExportData.ExportDataExcel();

                            objDati.etichetta_dato1 = "REGISTRO";
                            objDati.dato1 = rowTable["REGISTRO"].ToString();

                            objDati.etichetta_dato2 = "PROTOCOLLI IN INGRESSO";
                            objDati.dato2 = rowTable["PROTOCOLLI IN INGRESSO"].ToString();

                            objDati.etichetta_dato3 = "PROTOCOLLI IN USCITA";
                            objDati.dato3 = rowTable["PROTOCOLLI IN USCITA"].ToString();

                            objDati.etichetta_dato4 = "SPEDITI INT. ESTERNA";
                            objDati.dato4 = rowTable["SPEDITI INT. ESTERNA"].ToString();

                            objDati.etichetta_dato5 = "SPEDITI INT. INTERNA";
                            objDati.dato5 = rowTable["SPEDITI INT. INTERNA"].ToString();

                            objDati.etichetta_dato6 = "RICEVUTI INT. INTERNA";
                            objDati.dato6 = rowTable["RICEVUTI INT. INTERNA"].ToString();

                            objDati.etichetta_dato7 = "RICEVUTI INT. ESTERNA";
                            objDati.dato7 = rowTable["RICEVUTI INT. ESTERNA"].ToString();

                            returnList.Add(objDati);
                        }
                    }
                    break;
                case "PEC_REG":
                    ds = new DataSet();

                    condWhere = string.Empty;
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_7_MAIN");
                    condWhere += "AND id_registro IN (" + objReport.filtro.idRegistro.Replace('_', ',') + ") ";
                    if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA != string.Empty)
                        condWhere += "AND creation_date BETWEEN" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataDa, true) + "AND" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataA, false);
                    else if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA.Equals(string.Empty))
                        condWhere += "AND" + DocsPaDbManagement.Functions.Functions.ToChar("creation_date", false) + "=" + DocsPaDbManagement.Functions.Functions.ToChar(DocsPaDbManagement.Functions.Functions.ToDate(objReport.filtro.dataDa, false), false);

                    q.setParam("destinazione", "S','E','P");
                    q.setParam("condWhere", condWhere);
                    commandText = q.getSQL();

                    dbProvider.ExecuteQuery(ds, commandText);
                    DataTable _dsPec = new DataTable();
                    _dsPec = ds.Tables[0];
                    _dsPec.Columns["PROTO ENTRO X GIORNI"].ColumnName = "PROTO ENTRO " + objReport.filtro.giorniTrascorsiXProtocollazioneField + " GIORNI";

                    /********************************************************************************/
                    //Il  set di dati sono i RICEVUTI PEC
                    /********************************************************************************/
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_7_PROTO_ESTERNI");
                    //reset where
                    condWhere = string.Empty;
                    condWhere += "AND id_registro IN (" + objReport.filtro.idRegistro.Replace('_', ',') + ") ";
                    if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA != string.Empty)
                        condWhere += "AND creation_date BETWEEN" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataDa, true) + "AND" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataA, false);
                    else if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA.Equals(string.Empty))
                        condWhere += "AND" + DocsPaDbManagement.Functions.Functions.ToChar("creation_date", false) + "=" + DocsPaDbManagement.Functions.Functions.ToChar(DocsPaDbManagement.Functions.Functions.ToDate(objReport.filtro.dataDa, false), false);

                    q.setParam("destinazione", "S','E','P");
                    q.setParam("condWhere", condWhere);
                    commandText = q.getSQL();
                    ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    DataTable _dtRICEVUTI_PEC = new DataTable();
                    _dtRICEVUTI_PEC = ds.Tables[0];
                    _dtRICEVUTI_PEC.Columns[1].ColumnName = "PROTO MAIL ESTERNA";

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_7_PROTO_ENTRO_X_GG");
                    q.setParam("destinazione", "S','E','P");

                    q.setParam("condWhere", condWhere);
                    commandText = q.getSQL();
                    ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    DataTable _dtRICEVUTI_PEC_X_GG = new DataTable();
                    _dtRICEVUTI_PEC_X_GG = ds.Tables[0];
                    _dtRICEVUTI_PEC_X_GG.Columns[1].ColumnName = "PROTO ENTRO " + objReport.filtro.giorniTrascorsiXProtocollazioneField + " GIORNI";
                    //Qui tratto i GG lavorati in pratica li filtro da qui ;)
                    DataTable _dt2Copy = new DataTable();
                    _dt2Copy.Columns.Add("REGISTRO");
                    _dt2Copy.Columns.Add("PROTO ENTRO " + objReport.filtro.giorniTrascorsiXProtocollazioneField + " GIORNI");


                    foreach (DataRow _dr in _dtRICEVUTI_PEC_X_GG.Rows)
                    {
                        double _dbl = GetBusinessDays(Convert.ToDateTime(_dr["DATA PROTOCOLLAZIONE"]), Convert.ToDateTime(_dr["DATA CREAZIONE"]));
                        if (_dbl <= objReport.filtro.giorniTrascorsiXProtocollazioneField)
                        {
                            DataRow _dr2add = _dt2Copy.NewRow();
                            _dr2add["REGISTRO"] = _dr["REGISTRO"];
                            _dr2add["PROTO ENTRO " + objReport.filtro.giorniTrascorsiXProtocollazioneField + " GIORNI"] = _dr["PROTO ENTRO " + objReport.filtro.giorniTrascorsiXProtocollazioneField + " GIORNI"];
                            _dt2Copy.Rows.Add(_dr2add);
                            _dt2Copy.AcceptChanges();
                        }
                    }

                    //MERGE ALL TABLE
                    _dsPec.PrimaryKey = new DataColumn[] { _dsPec.Columns["REGISTRO"] };

                    DataTable DtJoin4TablesMainRep7 = _dsPec.Copy();
                    DtJoin4TablesMainRep7.Merge(_dtRICEVUTI_PEC, false, MissingSchemaAction.Add);
                    DtJoin4TablesMainRep7.AcceptChanges();

                    //Manipolo la datatable denormalizzata dei protocollati entro X Giorni.
                    DataTable DtManipolo = new DataTable();
                    DtManipolo.Columns.Add("REGISTRO");
                    DtManipolo.Columns.Add("PROTO ENTRO " + objReport.filtro.giorniTrascorsiXProtocollazioneField + " GIORNI", typeof(decimal));
                    string _codReg = string.Empty;
                    int _contoRecord = 0;

                    for (int i = 0; i < _dt2Copy.Rows.Count; i++)
                    {
                        _codReg = _dt2Copy.Rows[i]["REGISTRO"].ToString();
                        _contoRecord += Convert.ToInt16(_dt2Copy.Rows[i]["PROTO ENTRO " + objReport.filtro.giorniTrascorsiXProtocollazioneField + " GIORNI"].ToString());

                        //  (_dt2Copy.Rows.Count > 1) && (i + 1 = _dt2Copy.Rows.Count)
                        if (i + 1 == _dt2Copy.Rows.Count)
                        {
                            DataRow _drNew = DtManipolo.NewRow();
                            _drNew["REGISTRO"] = _codReg;
                            _drNew["PROTO ENTRO " + objReport.filtro.giorniTrascorsiXProtocollazioneField + " GIORNI"] = _contoRecord;
                            DtManipolo.Rows.Add(_drNew);
                            _contoRecord = 0;
                        }
                        else
                        {
                            if (_dt2Copy.Rows[i + 1]["REGISTRO"].ToString() != _codReg)
                            {
                                DataRow _drNew = DtManipolo.NewRow();
                                _drNew["REGISTRO"] = _codReg;
                                _drNew["PROTO ENTRO " + objReport.filtro.giorniTrascorsiXProtocollazioneField + " GIORNI"] = _contoRecord;
                                DtManipolo.Rows.Add(_drNew);
                                _contoRecord = 0;
                            }
                        }
                    }

                    if (DtManipolo.Rows.Count == 0 && _dtRICEVUTI_PEC_X_GG.Rows.Count != 0)
                    {
                        DataRow _drNew = DtManipolo.NewRow();
                        _drNew["REGISTRO"] = _codReg;
                        _drNew["PROTO ENTRO " + objReport.filtro.giorniTrascorsiXProtocollazioneField + " GIORNI"] = _contoRecord;
                        DtManipolo.Rows.Add(_drNew);
                    }


                    //Qui devo solo aggiungere all'ultima colonna 
                    DtJoin4TablesMainRep7.Merge(DtManipolo, false, MissingSchemaAction.Add);
                    DtJoin4TablesMainRep7.AcceptChanges();


                    if (DtJoin4TablesMainRep7.Rows.Count != 0)
                    {
                        foreach (DataRow rowTable in DtJoin4TablesMainRep7.Rows)
                        {
                            objDati = new DocsPaVO.ExportData.ExportDataExcel();

                            objDati.etichetta_dato1 = "REGISTRO";
                            objDati.dato1 = rowTable["REGISTRO"].ToString();

                            objDati.etichetta_dato2 = "RICEVUTI MAIL ESTERNA";
                            objDati.dato2 = rowTable["RICEVUTI MAIL ESTERNA"].ToString();

                            objDati.etichetta_dato3 = "PROTO MAIL ESTERNA";
                            objDati.dato3 = rowTable["PROTO MAIL ESTERNA"].ToString();

                            objDati.etichetta_dato4 = "PROTO ENTRO " + objReport.filtro.giorniTrascorsiXProtocollazioneField + " GIORNI";
                            objDati.dato4 = rowTable["PROTO ENTRO " + objReport.filtro.giorniTrascorsiXProtocollazioneField + " GIORNI"].ToString();

                            returnList.Add(objDati);
                        }
                    }
                    break;
                case "TRASM_EVI_RIF_REG":
                    //Tutte le trasmissioni
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_8");
                    condWhere += "AND pr.id_registro IN (" + objReport.filtro.idRegistro.Replace('_', ',') + ") ";
                    if (objReport.filtro.idRuolo != "0")
                        condWhere += "AND A.id_ruolo_in_uo IN (select system_id from dpa_corr_globali where id_tipo_ruolo = " + objReport.filtro.idRuolo + " and cha_tipo_urp = 'R' and cha_tipo_ie = 'I') ";
                    if (objReport.filtro.idRagTrasm != "0")
                        condWhere += "AND B.id_ragione = " + objReport.filtro.idRagTrasm + " ";
                    if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA != string.Empty)
                        condWhere += "AND A.dta_invio BETWEEN" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataDa, true) + "AND" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataA, false);
                    else if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA.Equals(string.Empty))
                        condWhere += "AND" + DocsPaDbManagement.Functions.Functions.ToChar("A.dta_invio", false) + "=" + DocsPaDbManagement.Functions.Functions.ToChar(DocsPaDbManagement.Functions.Functions.ToDate(objReport.filtro.dataDa, false), false);

                    q.setParam("condWhere", condWhere);
                    commandText = q.getSQL();
                    dbProvider.ExecuteQuery(ds, commandText);

                    DataTable _dtApppoggio = new DataTable();
                    _dtApppoggio = ds.Tables[0];

                    //Ciclo nell'appoggio ;)
                    if (_dtApppoggio.Rows.Count != 0)
                    {
                        for (int i = 0; i < _dtApppoggio.Rows.Count - 1; i++)
                        {
                            bool _stessoNome = false;
                            if ((_dtApppoggio.Rows.Count > 1) && (i + 1 < _dtApppoggio.Rows.Count))
                                if (_dtApppoggio.Rows[i]["UTENTE"].ToString() == _dtApppoggio.Rows[i + 1]["UTENTE"].ToString()
                                    && _dtApppoggio.Rows[i]["REGISTRO"].ToString() == _dtApppoggio.Rows[i + 1]["REGISTRO"].ToString()
                                     && _dtApppoggio.Rows[i]["RUOLO"].ToString() == _dtApppoggio.Rows[i + 1]["RUOLO"].ToString())
                                {
                                    _stessoNome = true;
                                }
                            if (_stessoNome)
                            {
                                objDati = new DocsPaVO.ExportData.ExportDataExcel();

                                objDati.etichetta_dato1 = "REGISTRO";
                                objDati.dato1 = _dtApppoggio.Rows[i]["REGISTRO"].ToString();

                                objDati.etichetta_dato2 = "UTENTE";
                                objDati.dato2 = _dtApppoggio.Rows[i]["UTENTE"].ToString();

                                objDati.etichetta_dato3 = "RUOLO";
                                objDati.dato3 = _dtApppoggio.Rows[i]["RUOLO"].ToString();

                                objDati.etichetta_dato4 = "NUMERO TRASM";
                                objDati.dato4 = (Convert.ToInt32(_dtApppoggio.Rows[i]["NUMERO TRASM"]) + Convert.ToInt32(_dtApppoggio.Rows[i + 1]["NUMERO TRASM"])).ToString();

                                objDati.etichetta_dato5 = "NUMERO TRASM RIF";
                                objDati.dato5 = _dtApppoggio.Rows[i + 1]["NUMERO TRASM"].ToString();

                                i++;
                            }
                            else
                            {
                                objDati = new DocsPaVO.ExportData.ExportDataExcel();

                                objDati.etichetta_dato1 = "REGISTRO";
                                objDati.dato1 = _dtApppoggio.Rows[i]["REGISTRO"].ToString();

                                objDati.etichetta_dato2 = "UTENTE";
                                objDati.dato2 = _dtApppoggio.Rows[i]["UTENTE"].ToString();

                                objDati.etichetta_dato3 = "RUOLO";
                                objDati.dato3 = _dtApppoggio.Rows[i]["RUOLO"].ToString();

                                objDati.etichetta_dato4 = "NUMERO TRASM";
                                objDati.dato4 = _dtApppoggio.Rows[i]["NUMERO TRASM"].ToString();

                                objDati.etichetta_dato5 = "NUMERO TRASM RIF";
                                objDati.dato5 = "0";
                            }
                            returnList.Add(objDati);
                        }
                    }

                    break;
                case "TX_R_RIC":
                    //Tutte le trasmissioni
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_AV_9");
                    condWhere += "AND pr.id_registro IN (" + objReport.filtro.idRegistro.Replace('_', ',') + ") ";
                    if (objReport.filtro.idRuolo != "0")
                        condWhere += "AND dcg.id_tipo_ruolo  = " + objReport.filtro.idRuolo;
                    if (objReport.filtro.idRagTrasm != "0")
                        condWhere += "AND B.id_ragione = " + objReport.filtro.idRagTrasm + " ";
                    if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA != string.Empty)
                        condWhere += "AND A.dta_invio BETWEEN" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataDa, true) + "AND" + DocsPaDbManagement.Functions.Functions.ToDateBetween(objReport.filtro.dataA, false);
                    else if (objReport.filtro.dataDa != string.Empty && objReport.filtro.dataA.Equals(string.Empty))
                        condWhere += "AND" + DocsPaDbManagement.Functions.Functions.ToChar("A.dta_invio", false) + "=" + DocsPaDbManagement.Functions.Functions.ToChar(DocsPaDbManagement.Functions.Functions.ToDate(objReport.filtro.dataDa, false), false);

                    q.setParam("condWhere", condWhere);
                    commandText = q.getSQL();
                    dbProvider.ExecuteQuery(ds, commandText);

                    DataTable _dtApppoggioRicevute = new DataTable();
                    _dtApppoggioRicevute = ds.Tables[0];

                    //Ciclo nell'appoggio ;)
                    if (_dtApppoggioRicevute.Rows.Count != 0)
                    {
                        for (int i = 0; i < _dtApppoggioRicevute.Rows.Count; i++)
                        {
                            bool _stessoNome = false;

                            if ((_dtApppoggioRicevute.Rows.Count > 1) && (i + 1 < _dtApppoggioRicevute.Rows.Count))
                                if (_dtApppoggioRicevute.Rows[i]["UTENTE"].ToString() == _dtApppoggioRicevute.Rows[i + 1]["UTENTE"].ToString()
                                    && _dtApppoggioRicevute.Rows[i]["REGISTRO"].ToString() == _dtApppoggioRicevute.Rows[i + 1]["REGISTRO"].ToString()
                                     && _dtApppoggioRicevute.Rows[i]["RUOLO"].ToString() == _dtApppoggioRicevute.Rows[i + 1]["RUOLO"].ToString())
                                {
                                    _stessoNome = true;
                                }
                            if (_stessoNome)
                            {
                                objDati = new DocsPaVO.ExportData.ExportDataExcel();

                                objDati.etichetta_dato1 = "REGISTRO";
                                objDati.dato1 = _dtApppoggioRicevute.Rows[i]["REGISTRO"].ToString();

                                objDati.etichetta_dato2 = "UTENTE";
                                objDati.dato2 = _dtApppoggioRicevute.Rows[i]["UTENTE"].ToString();

                                objDati.etichetta_dato3 = "RUOLO";
                                objDati.dato3 = _dtApppoggioRicevute.Rows[i]["RUOLO"].ToString();

                                objDati.etichetta_dato4 = "NUMERO TRASM";
                                objDati.dato4 = (Convert.ToInt32(_dtApppoggioRicevute.Rows[i]["NUMERO TRASM"]) + Convert.ToInt32(_dtApppoggioRicevute.Rows[i + 1]["NUMERO TRASM"])).ToString();

                                objDati.etichetta_dato5 = "NUMERO TRASM RIF";
                                objDati.dato5 = _dtApppoggioRicevute.Rows[i + 1]["NUMERO TRASM"].ToString();

                                i++;
                            }
                            else
                            {
                                objDati = new DocsPaVO.ExportData.ExportDataExcel();

                                objDati.etichetta_dato1 = "REGISTRO";
                                objDati.dato1 = _dtApppoggioRicevute.Rows[i]["REGISTRO"].ToString();

                                objDati.etichetta_dato2 = "UTENTE";
                                objDati.dato2 = _dtApppoggioRicevute.Rows[i]["UTENTE"].ToString();

                                objDati.etichetta_dato3 = "RUOLO";
                                objDati.dato3 = _dtApppoggioRicevute.Rows[i]["RUOLO"].ToString();

                                objDati.etichetta_dato4 = "NUMERO TRASM";
                                objDati.dato4 = _dtApppoggioRicevute.Rows[i]["NUMERO TRASM"].ToString();

                                objDati.etichetta_dato5 = "NUMERO TRASM RIF";
                                objDati.dato5 = "0";
                            }
                            returnList.Add(objDati);
                        }
                    }
                    break;
            }
            logger.Debug(commandText);

            return returnList;
        }

        /// <summary>
        /// Giordnao Iacozzilli
        /// Funzione che fa il get del numero di giorni Lavorativi su un range di date  
        /// </summary>
        /// <param name="startD">Data di partenza</param>
        /// <param name="endD">Data finale</param>
        /// <returns></returns>
        public static double GetBusinessDays(DateTime startD, DateTime endD)
        {

            double calcBusinessDays =

            1 + ((endD - startD).TotalDays * 5 -

            (startD.DayOfWeek - endD.DayOfWeek) * 2) / 7;

            if ((int)endD.DayOfWeek == 6) calcBusinessDays--;

            if ((int)startD.DayOfWeek == 0) calcBusinessDays--;

            return calcBusinessDays;

        }

        /// <summary>
        /// Verifica se l'amm.ne ha il protocollo interno
        /// </summary>
        /// <param name="idAmm">ID amm.ne</param>
        /// <returns>true o false</returns>
        private bool isProtoIntEnabled(string idAmm)
        {
            Documenti obj = new Documenti();
            return obj.IsEnabledProtoInt(idAmm);
        }
        #endregion

        #region StampaREGEXP
        
        public bool UpdStampaExp(string idprofile, string datastampa)
        {
            bool retval = false;

            //U_CONSERVAZIONE
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_CONSERVAZIONE");
            q.setParam("param1", "PROFILE");
            q.setParam("param2", "SET CREATION_DATE = TO_DATE('" + datastampa + " 23:00','dd/mm/yyyy hh24:mi'), CREATION_TIME = TO_DATE('" + datastampa + " 23:00','dd/mm/yyyy hh24:mi'),LAST_EDIT_DATE = TO_DATE('" + datastampa + " 23:00','dd/mm/yyyy hh24:mi')");
            q.setParam("param3", "WHERE SYSTEM_ID = "+idprofile);
            
            string queryString = q.getSQL();
            logger.Debug(queryString);
            retval = this.ExecuteNonQuery(queryString);

            DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("U_CONSERVAZIONE");
            q2.setParam("param1", "DPA_STAMPAREGISTRI");
            q2.setParam("param2", "SET DTA_STAMPA = TO_DATE('" + datastampa + " 23:00','dd/mm/yyyy hh24:mi')");
            q2.setParam("param3", "WHERE DOCNUMBER = " + idprofile);

            queryString = q2.getSQL();
            logger.Debug(queryString);
            retval = this.ExecuteNonQuery(queryString);

            DocsPaUtils.Query q3 = DocsPaUtils.InitQuery.getInstance().getQuery("U_CONSERVAZIONE");
            q3.setParam("param1", "COMPONENTS");
            q3.setParam("param2", "SET DTA_FILE_ACQUIRED = TO_DATE('" + datastampa + " 23:00','dd/mm/yyyy hh24:mi')");
            q3.setParam("param3", "WHERE DOCNUMBER = " + idprofile);

            queryString = q3.getSQL();
            logger.Debug(queryString);
            retval = this.ExecuteNonQuery(queryString);

            DocsPaUtils.Query q4 = DocsPaUtils.InitQuery.getInstance().getQuery("U_CONSERVAZIONE");
            q4.setParam("param1", "VERSIONS");
            q4.setParam("param2", "SET DTA_CREAZIONE = TO_DATE('" + datastampa + " 23:00','dd/mm/yyyy hh24:mi')");
            q4.setParam("param3", "WHERE DOCNUMBER = " + idprofile);

            queryString = q4.getSQL();
            logger.Debug(queryString);

            retval = this.ExecuteNonQuery(queryString);

            return retval;
        }

        public void getDatiStampaReg(string iddoc, out DataSet ds)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONSERVAZIONE");
            q.setParam("param1", "ID_REGISTRO, NUM_PROTO_START, NUM_PROTO_END, NUM_ANNO, DOCNUMBER, TO_CHAR(DTA_STAMPA,'dd/mm/yyyy') as DATASTAMPA");
            q.setParam("param2", "FROM DPA_STAMPAREGISTRI");
            q.setParam("param3", "WHERE DOCNUMBER = "+iddoc);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.ExecuteQuery(out ds, "REG", queryString);
        }

        #endregion
    }
}
