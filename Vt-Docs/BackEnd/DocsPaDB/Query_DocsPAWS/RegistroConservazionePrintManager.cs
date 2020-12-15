using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DocsPaDB;
using DocsPaUtils;
using DocsPaUtils.Data;
using DocsPaVO.documento;
using DocsPaVO.filtri;
using DocsPaVO.Report;
using DocsPaVO.utente;
using DocsPaVO.utente.RegistroConservazione;
using DocsPaVO.areaConservazione;
using log4net;



namespace DocsPaDB.Query_DocsPAWS
{


    /// <summary>
    /// Questa classe contiene i metodi utilizzati per la stampa del registro di conservazione.
    /// </summary>
    public class RegistroConservazionePrintManager
    {

        private static ILog logger = LogManager.GetLogger(typeof(RegistroConservazionePrintManager));

        /// <summary>
        /// Verifica se esistono amministrazioni aventi il registo abilitato.
        /// </summary>
        /// <returns>Numero amministrazioni con registro abilitato</returns>
        public string GetEnabledRegCons()
        {
            string retVal = "";
            Query query = InitQuery.getInstance().getQuery("S_REG_CONS_GET_NUM_ENABLED");

            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {
                    while (dataReader.Read())
                    {
                        retVal = dataReader["N_AMM"].ToString();
                    }

                }
            }

            return retVal;
        }

        /// <summary>
        /// Restituisce la lista di amministrazioni con registro di conservazione
        /// abilitato e le rispettive date in cui effettuare la prossima stampa.
        /// </summary>
        /// <returns></returns>
        public List<RegistroConservazionePrint> GetRegConsPrintRange()
        {

            List<RegistroConservazionePrint> retVal = new List<RegistroConservazionePrint>();
            using (DBProvider dbProvider = new DBProvider()) 
            {
                //creo la query da eseguire
                Query query = InitQuery.getInstance().getQuery("S_REG_CONS_GET_LIST_ENABLED_AMM");
                logger.Debug(query.getSQL());
                int i = 1;
                                
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {
                        while (dataReader.Read())
                        {
                            //per gestione eventuali valori null
                            string lastPrintedId;
                            string idAmm = string.Empty;
                            string userId;
                            string role;
                            int hPrint;
                            DateTime firstDate;
                            DateTime lastDate;

                            idAmm = dataReader["ID_AMM"].ToString();
                            lastPrintedId = dataReader["ID_LAST_PRINTED_EVENT"].Equals(DBNull.Value) ? "0" : dataReader["ID_LAST_PRINTED_EVENT"].ToString();
                            firstDate = dataReader["DTA_LAST_PRINT"].Equals(DBNull.Value) ? DateTime.Now : Convert.ToDateTime(dataReader["DTA_LAST_PRINT"].ToString());
                            lastDate = dataReader["DTA_NEXT_PRINT"].Equals(DBNull.Value) ? DateTime.Now : Convert.ToDateTime(dataReader["DTA_NEXT_PRINT"].ToString());
                            userId = dataReader["PRINTER_USER_ID"].Equals(DBNull.Value) ? string.Empty : dataReader["PRINTER_USER_ID"].ToString();
                            role = dataReader["PRINTER_ROLE_ID"].Equals(DBNull.Value) ? string.Empty : dataReader["PRINTER_ROLE_ID"].ToString();
                            hPrint = dataReader["PRINT_HOUR"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(dataReader["PRINT_HOUR"]);

                            logger.Debug(i + " - ID: " + idAmm + " - last id: " + lastPrintedId);
                            i++;

                            retVal.Add(new RegistroConservazionePrint()
                                {
                                    idAmministrazione = idAmm,
                                    printFreq = dataReader["PRINT_FREQ"].ToString(),
                                    idLastPrinted = lastPrintedId,
                                    idLastToPrint = dataReader["ID_LAST_EVENT_TO_PRINT"].ToString(),
                                    lastPrintDate = firstDate,
                                    nextPrintDate = lastDate,
                                    printHour = hPrint,
                                    print_userId = userId,
                                    print_role = role

                                });
                        }
                   }

                
            }

            return retVal;
        }

        /// <summary>
        /// Verifica se la data attuale è successiva a quella definita per la 
        /// prossima stampa.
        /// </summary>
        /// <param name="nextPrint">data di prossima stampa</param>
        /// <returns></returns>
        public bool VerifyNextPrint(DateTime nextPrint, string idAmm, string lastPrintedId, int startingHour)
        {
            bool retVal = false;
            int totId = 0;

            try
            {
                DateTime currentDate = DateTime.Now;
                int currentHour = Convert.ToInt32(currentDate.ToString("HH"));
                logger.Debug("data attuale: " + currentDate.ToString("dd/MM/yyyy"));
                logger.Debug("data di prossima stampa: " + nextPrint.ToString("dd/MM/yyyy"));
                logger.Debug("ora attuale: " + currentHour);
                logger.Debug("ora di stampa: " + startingHour);

                //controllo che la data attuale sia >= della data di prossima stampa
                //e che l'ora attuale sia >= dell'ora di stampa
                int value = DateTime.Compare(currentDate, nextPrint);
                if (value >= 0 && currentHour>=startingHour)
                {
                    logger.Debug("check su data di stampa OK");
                    //controllo aggiuntivo: sono presenti record successivi rispetto all'ultimo stampato?
                    using (DBProvider dbProvider = new DBProvider())
                    {
                        Query query = InitQuery.getInstance().getQuery("S_REG_CONS_VERIFY_SYS_ID");
                        query.setParam("id_amm", idAmm);
                        query.setParam("system_id", lastPrintedId);

                        using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                        {
                            while (dataReader.Read())
                            {
                                totId = Convert.ToInt32(dataReader["totId"].ToString());
                            }
                            if (totId > 0)
                            {
                                logger.Debug("check su record da stampare OK");
                                logger.Debug("modifica data di prossima stampa");
                                //GM 12-09-2013
                                //per impedire che il servizio venga invocato nuovamente se la stampa
                                //è in corso, imposto la data di prossima stampa                          
                                DateTime dummyDate = DateTime.Now.AddYears(1);
                                Query querycheck = InitQuery.getInstance().getQuery("U_REG_CONS_MOD_NEXT_PRINT_DATE");
                                querycheck.setParam("idAmm", idAmm);
                                querycheck.setParam("ndate", dummyDate.ToString("dd/MM/yyyy"));

                                string commandText = querycheck.getSQL();
                                logger.Debug(commandText);
                                dbProvider.ExecuteNonQuery(commandText);

                                retVal = true;
                            }
                        }
                    }
                }

                return retVal;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nel metodo VerifyNextPrint:", ex);
                return false;
            }
            //return retVal;
        }

        /// <summary>
        /// Restituisce le istanze da inserire nella stampa del registro di conservazione.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Lista delle istanze da inserire</returns>
        public List<string> GetListaIstanze(PrintReportRequest request)
        {
            List<string> retList = new List<string>();

            string idAmm = request.SearchFilters.Where(f => f.argomento == "id_amm").FirstOrDefault().valore;
            string nextSysId = request.SearchFilters.Where(f => f.argomento == "next_system_id").FirstOrDefault().valore;
            string lastSysId = request.SearchFilters.Where(f => f.argomento == "last_system_id").FirstOrDefault().valore;

            using (DBProvider dbProvider = new DBProvider())
            {

                Query query = InitQuery.getInstance().getQuery("S_REG_CONS_GET_LIST_ISTANZE");
                query.setParam("id_amm", idAmm);
                query.setParam("next_system_id", nextSysId);
                query.setParam("last_system_id", lastSysId);
                logger.Debug("GetListaIstanze: " + query.getSQL());

                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {
                    while (dataReader.Read())
                    {
                        retList.Add(dataReader["ID_ISTANZA"].ToString());

                    }

                }
            }

            return retList;
        }

        /// <summary>
        /// Restituisce la lista degli id dei documenti appartenenti ad un'istanza.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<string> GetListaDocumentiIstanze(PrintReportRequest request)
        {
            List<string> retList = new List<string>();

            string idAmm = request.SearchFilters.Where(f => f.argomento == "id_amm").FirstOrDefault().valore;
            string nextSysId = request.SearchFilters.Where(f => f.argomento == "next_system_id").FirstOrDefault().valore;
            string lastSysId = request.SearchFilters.Where(f => f.argomento == "last_system_id").FirstOrDefault().valore;
            string idIst = request.SearchFilters.Where(f => f.argomento == "id_istanza").FirstOrDefault().valore;

            using (DBProvider dbProvider = new DBProvider())
            {

                Query query = InitQuery.getInstance().getQuery("S_REG_CONS_GET_LIST_DOC_ISTANZE");
                query.setParam("id_amm", idAmm);
                query.setParam("next_system_id", nextSysId);
                query.setParam("last_system_id", lastSysId);
                query.setParam("id_istanza", idIst);

                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {
                    while (dataReader.Read())
                    {
                        retList.Add(dataReader["ID_OGGETTO"].ToString());

                    }

                }
            }

            return retList;
        }

        /// <summary>
        /// Metodo per l'aggiornamento della data di prossima stampa del registro di conservazione.
        /// L'aggiornamento avviene sulla base della frequenza impostata.
        /// Viene aggiornato anche l'ultimo ID stampato.
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public bool UpdateNextPrintDate(RegistroConservazionePrint reg)
        {
            //ricavo la data di prossima stampa
            string nextPrintDate = this.GetNextPrintDate(DateTime.Now, reg.printFreq);

            //imposto la data attuale come data di ultima stampa
            string lastPrintDate = DateTime.Now.ToString("dd/MM/yyyy");

            //recupero l'ultimo id stampato
            string idLastPrinted = reg.idLastToPrint;

            Query query = InitQuery.getInstance().getQuery("U_REG_CONS_UPDATE_CONFIG");
            query.setParam("nextPrintDate", nextPrintDate);
            query.setParam("lastPrintDate", lastPrintDate);
            query.setParam("idLastPrintedEvent", idLastPrinted);
            query.setParam("idAmm", reg.idAmministrazione);

           
            logger.Debug(query.getSQL());
            //esecuzione query
            bool retVal = false;
            using (DBProvider dbProvider = new DBProvider())
            {
                retVal = dbProvider.ExecuteNonQuery(query.getSQL());
            }

            return retVal;

        }

        /// <summary>
        /// Metodo per il salvataggio della stampa del registro di conservazione.
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public bool UpdateRegStampeCons(RegistroConservazionePrint reg, string docId)
        {

            bool retVal = false;

            //GM 22-7-2013
            //traccio il primo evento stampato nel registro
            //che corrisponde all'id a cui ci siamo fermati nella stampa precedente aumentato di 1
            string idFirstPrintedEvent = (Convert.ToInt32(reg.idLastPrinted) + 1).ToString();

            Query query = InitQuery.getInstance().getQuery("I_REG_CONS_SAVE_PRINT");
            query.setParam("id_amm", reg.idAmministrazione);
            query.setParam("last_system_id", idFirstPrintedEvent);
            query.setParam("next_system_id", reg.idLastToPrint);
            query.setParam("id_profile", docId); // ID del documento creato

            using (DBProvider dbProvider = new DBProvider())
            {
                retVal = dbProvider.ExecuteNonQuery(query.getSQL());
            }

            return retVal;
        }

        /// <summary>
        /// Metodo per l'aggiornamento del campo CHA_PRINTED del registro di conservazione.
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public bool UpdatePrintedRecords(RegistroConservazionePrint reg)
        {
            bool retVal = false;

            Query query = InitQuery.getInstance().getQuery("U_REG_CONS_PRINTED_ITEMS");
            query.setParam("id_amm", reg.idAmministrazione);
            query.setParam("next_system_id", reg.idLastPrinted);
            query.setParam("last_system_id", reg.idLastToPrint);

            using (DBProvider dbProvider = new DBProvider())
            {
                retVal = dbProvider.ExecuteNonQuery(query.getSQL());
            }

            return retVal;
        }

        /// <summary>
        /// Imposta la data di prossima stampa
        /// </summary>
        /// <param name="date"></param>
        /// <param name="freq"></param>
        /// <returns></returns>
        private string GetNextPrintDate(DateTime date, string freq)
        {

            DateTime retVal = date;
            logger.Debug(string.Format("DATA ULTIMA STAMPA: {0}", date.ToString("dd/MM/yyyy")));

            switch (freq)
            {

                //frequenza giornaliera
                case "10":
                    retVal = date.AddDays(1);
                    break;

                //frequenza settimanale
                case "20":
                    retVal = date.AddDays(7);
                    break;

                //frequenza mensile
                case "30":
                    retVal = date.AddMonths(1);
                    break;

                //frequenza annuale
                case "40":
                    retVal = date.AddYears(1);
                    break;

                default:
                    throw new Exception("errore nella frequenza di stampa");
            }

            logger.Debug(string.Format("DATA PROSSIMA STAMPA: {0}", retVal.ToString("dd/MM/yyyy")));
            return retVal.ToString("dd/MM/yyyy");


        }

        /// <summary>
        /// Converte la frequenza di stampa letta nella configurazione
        /// nel valore da inserire della stampa del registro.
        /// </summary>
        /// <param name="freq"></param>
        /// <returns></returns>
        public string GetPrintFreq(string freq)
        {
            string retVal = String.Empty;

            switch (freq)
            {
                case "10":
                    retVal = "giornaliera";
                    break;

                case "20":
                    retVal = "settimanale";
                    break;;

                case "30":
                    retVal = "mensile";
                    break;

                case "40":
                    retVal = "annuale";
                    break;
            }

            return retVal;

        }

        /// <summary>
        /// Metodo per il recupero delle informazioni necessarie alla scrittura
        /// del summary di un'istanza nella stampa del registro di conservazione.
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <returns></returns>
        public RegistroConservazioneSummary GetSummaryDataIst(string idIstanza)
        {

            RegistroConservazioneSummary retVal = new RegistroConservazioneSummary();
            Query query = InitQuery.getInstance().getQuery("S_REG_CONS_SUMMARY_DATA_IST");
            query.setParam("id_istanza", idIstanza);
            logger.Debug("GetSummaryDataIst: " + query.getSQL());

            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {
                    while (dataReader.Read())
                    {
                        //retVal.descrizione  = dataReader["var_descrizione"].ToString();
                        //retVal.creationDate = Convert.ToDateTime(dataReader["data_apertura"]);
                        //retVal.invioDate = Convert.ToDateTime(dataReader["data_invio"]);
                        //retVal.numDoc = dataReader["n_doc"].ToString();
                        //retVal.fileDim = dataReader["tot_size"].ToString();

                        retVal.descrizione = dataReader["var_descrizione"].Equals(DBNull.Value) ? string.Empty : dataReader["var_descrizione"].ToString();
                        retVal.creationDate = dataReader["data_apertura"].Equals(DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dataReader["data_apertura"]);
                        retVal.invioDate = dataReader["data_invio"].Equals(DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dataReader["data_invio"]);
                        retVal.numDoc = dataReader["n_doc"].Equals(DBNull.Value) ? string.Empty : dataReader["n_doc"].ToString();
                        retVal.fileDim = dataReader["tot_size"].Equals(DBNull.Value) ? string.Empty : dataReader["tot_size"].ToString();
                    }
                    
                }
                
            }

            return retVal;
        }


        /// <summary>
        /// Metodo per il recupero delle informazioni necessarie alla scrittura
        /// del summary di un documento nella stampa del registro di conservazione.
        /// </summary>
        /// <param name="idDoc"></param>
        /// <returns></returns>
        public RegistroConservazioneSummary GetSummaryDataDoc(string idDoc)
        {

            RegistroConservazioneSummary retVal = new RegistroConservazioneSummary();
            Query query = InitQuery.getInstance().getQuery("S_REG_CONS_SUMMARY_DATA_DOC");
            query.setParam("id_doc", idDoc);

            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {
                    while (dataReader.Read())
                    {
                        retVal.creationDate = dataReader["DATA_INS"].Equals(DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(dataReader["DATA_INS"]);
                        retVal.descrizione = dataReader["VAR_OGGETTO"].Equals(DBNull.Value) ? string.Empty : dataReader["VAR_OGGETTO"].ToString();
                        retVal.codiceFascicolo = dataReader["COD_FASC"].Equals(DBNull.Value) ? string.Empty : dataReader["COD_FASC"].ToString();
                        retVal.numDoc = dataReader["NUMERO_ALLEGATI"].Equals(DBNull.Value) ? string.Empty : dataReader["NUMERO_ALLEGATI"].ToString();
                        retVal.tipoFile = dataReader["VAR_TIPO_FILE"].Equals(DBNull.Value) ? string.Empty : dataReader["VAR_TIPO_FILE"].ToString();
                        retVal.fileDim = dataReader["SIZE_ITEM"].Equals(DBNull.Value) ? string.Empty : dataReader["SIZE_ITEM"].ToString();

                        //retVal.creationDate = Convert.ToDateTime(dataReader["DATA_INS"]);
                        //retVal.descrizione = dataReader["VAR_OGGETTO"].ToString();
                        //retVal.codiceFascicolo = dataReader["COD_FASC"].ToString();
                        //retVal.numDoc = dataReader["NUMERO_ALLEGATI"].ToString();
                        //retVal.tipoFile = dataReader["VAR_TIPO_FILE"].ToString();
                        //retVal.fileDim = dataReader["SIZE_ITEM"].ToString();
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Metodo per la modifica del tipo_proto della stampa del registro dopo
        /// il salvataggio. 
        /// </summary>
        /// <param name="docnumber"></param>
        /// <returns></returns>
        public bool UpdateTipoProto(string docnumber)
        {

            bool retVal = false;
            //tipo_proto da inserire:
            string tipoProto = "M";

            Query query = InitQuery.getInstance().getQuery("U_REG_CONS_TIPO_PROTO");
            query.setParam("docnumber", docnumber);
            query.setParam("tipo_proto", tipoProto);

            using (DBProvider dbProvider = new DBProvider())
            {
                retVal = dbProvider.ExecuteNonQuery(query.getSQL());
            }

            return retVal;

        }

        public string ConvertDocSize(string size)
        {

            double iSize = Convert.ToDouble(size);
            double conv = 1024;
            string retVal;

            if (iSize >= 1.0E9)
            {
                double fileSize = iSize/(conv*conv*conv);
                retVal = string.Format("{0} {1}", fileSize.ToString("F1"), "GB");
            }
            else if (iSize >= 1.0E6 && iSize < 1.0E9)
            {
                double fileSize = iSize/(conv * conv);
                retVal = string.Format("{0} {1}", fileSize.ToString("F1"), "MB");
            }
            else if (iSize >= 1.0E3 && iSize < 1.0E6)
            {
                double fileSize = iSize / conv;
                retVal = string.Format("{0} {1}", fileSize.ToString("F1"), "KB");
            }
            else
            {
                retVal = string.Format("{0} {1}", iSize.ToString(), "byte");
            }

            return retVal;


        }

        public List<StampaConservazione> RicercaStampaConservazione(FiltroRicerca[] filters)
        {

            string filtroFirma = string.Empty;
            string filtroData = string.Empty;
            string filtroAmm = string.Empty;

            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];

            List<FiltroRicerca> searchFilters = filters.Cast<FiltroRicerca>().ToList();
            foreach (FiltroRicerca f in searchFilters)
            {
                switch (f.argomento)
                {
                    case "FIRMATE":
                        filtroFirma = string.Format(" AND p.cha_firmato={0} ", f.valore);
                        break;

                    case "DATA_STAMPA_DA":
                        filtroData += " AND sc.print_date >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                        break;

                    case "DATA_STAMPA_A":
                        filtroData += " AND sc.print_date <" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                        break;

                    case "DATA_STAMPA_IL":
                        filtroData += " AND sc.print_date >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                        filtroData += " AND sc.print_date <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                        break;

                    case "DATA_STAMPA_TODAY":
                        DateTime date = DateTime.Now;
                        if (f.valore == "1")
                        {
                            filtroData += " AND sc.print_date >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(date.ToString("dd/MM/yyyy"), true);
                            filtroData += " AND sc.print_date <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(date.ToString("dd/MM/yyyy"), false);
                        }
                        break;

                    case "DATA_STAMPA_SC":
                        if (f.valore == "1")
                        {
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                filtroData += " AND sc.print_date >=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual)";
                            }
                            else
                            {
                                filtroData += " AND sc.print_date >=(select CAST(DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE()) as date))";
                            }
                        }
                        break;

                    case "DATA_STAMPA_MC":
                        if (f.valore == "1")
                        {
                            if (!dbType.ToUpper().Equals("SQL"))
                            {
                                filtroData += " AND sc.print_date >=TRUNC(SYSDATE,'MM') AND sc.print_date < (SYSDATE+1) ";
                            }
                            else
                            {
                                filtroData += " AND sc.print_date >=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND sc.print_date <(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            }
                        }
                        break;
                            
                    case "ID_AMM":
                        filtroAmm = f.valore;
                        break;
                }
            }

            List<StampaConservazione> retVal = new List<StampaConservazione>();

            //recupero filtri
            Query query = InitQuery.getInstance().getQuery("S_REG_CONS_RICERCA_STAMPE_CONS");
            query.setParam("p_firma", filtroFirma);
            query.setParam("p_data", filtroData);
            query.setParam("id_amm", filtroAmm);

            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {
                    while (dataReader.Read())
                    {
                        retVal.Add(new StampaConservazione
                        {
                            idDocumento = dataReader["SYSTEM_ID"].ToString(),
                            dataDocumento = (dataReader["PRINT_DATE"].ToString()).Substring(0,10),
                            oggettoDocumento = dataReader["VAR_PROF_OGGETTO"].ToString(),
                            idProfile = dataReader["ID_PROFILE"].ToString(),
                            chaFirmato = dataReader["CHA_FIRMATO"].ToString()
                        });
                    }
                }
            }

            return retVal;
        }


    }
}
