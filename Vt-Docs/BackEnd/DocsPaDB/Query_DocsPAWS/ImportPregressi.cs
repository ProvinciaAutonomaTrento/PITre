using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaUtils;
using System.Data;
using log4net;
using DocsPaVO.utente;
using DocsPaVO.Import.Pregressi;

namespace DocsPaDB.Query_DocsPAWS
{
    public class ImportPregressi
    {
        private static ILog logger = LogManager.GetLogger(typeof(ImportPregressi));

        public DocsPaVO.Import.Pregressi.ReportPregressi GetReportPregressi(string sysId, bool getItems)
        {
            DocsPaVO.Import.Pregressi.ReportPregressi result = null;
            try
            {
                DataSet ds_report = new DataSet();


                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //Prendo il Report in base al system_id
                    Query query_report = InitQuery.getInstance().getQuery("S_GET_REPORT_PREGRESSO_BY_SYSTEMID");
                    query_report.setParam("system_id", sysId);

                    string commandText = query_report.getSQL();
                    logger.Debug(commandText);

                    dbProvider.ExecuteQuery(ds_report, "ReportById", commandText);
                    //dbProvider.ExecuteQuery(ds_report, query_report.ToString());
                    //Fine Query

                    if (ds_report != null && ds_report.Tables[0].Rows.Count > 0)
                    {
                        result = new DocsPaVO.Import.Pregressi.ReportPregressi();
                        result.itemPregressi = new List<DocsPaVO.Import.Pregressi.ItemReportPregressi>();
                        foreach (DataRow row in ds_report.Tables[0].Rows)
                        {
                            result.systemId = row["SYSTEM_ID"].ToString();
                            result.idAmm = row["ID_AMM"].ToString();
                            result.idUtenteCreatore = row["ID_UTENTE_CREATORE"].ToString();
                            result.idRuoloCreatore = row["ID_RUOLO_CREATORE"].ToString();
                            result.dataEsecuzione = row["DATA_ESECUZIONE"].ToString();
                            result.dataFine = row["DATA_FINE"].ToString();
                            result.numDoc = row["NUM_DOC"].ToString();

                            if (getItems)
                            {
                                DataSet ds_item = new DataSet();

                                //Query per il popolamento della lista di itemReportPregressi
                                //Query:
                                Query query_item = InitQuery.getInstance().getQuery("S_GET_ITEM_REPORT_PREGRESSO_BY_SYSTEMID");
                                query_item.setParam("id_report", sysId);

                                string commandText_item = query_item.getSQL();
                                logger.Debug(commandText_item);

                                dbProvider.ExecuteQuery(ds_item, "ItemReport", commandText_item);
                                //dbProvider.ExecuteQuery(ds_item, query_item.ToString());

                                foreach (DataRow row_item in ds_item.Tables[0].Rows)
                                {
                                    DocsPaVO.Import.Pregressi.ItemReportPregressi item = new DocsPaVO.Import.Pregressi.ItemReportPregressi();
                                    //Popolamento:
                                    item.systemId = row_item["SYSTEM_ID"].ToString();
                                    item.idPregresso = row_item["ID_PREGRESSO"].ToString();
                                    item.idRegistro = row_item["ID_REGISTRO"].ToString();
                                    item.idDocumento = row_item["ID_DOCUMENTO"].ToString();
                                    item.idUtente = row_item["ID_UTENTE"].ToString();
                                    item.idRuolo = row_item["ID_RUOLO"].ToString();
                                    item.tipoOperazione = row_item["TIPO_OPERAZIONE"].ToString();
                                    item.data = row_item["DATA"].ToString();
                                    item.errore = row_item["ERRORE"].ToString();
                                    item.esito = row_item["ESITO"].ToString();
                                    item.idNumProtocolloExcel = row_item["ID_NUM_PROTO_EXCEL"].ToString();

                                    //Get degli Allegati
                                    item.Allegati = GetAllegatiByIdItem(item.systemId);

                                    //Inserimento:
                                    result.itemPregressi.Add(item);
                                    //result.AddItemReportPregressi(item);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Andrea De Marco - get di tutti i report degli import pregressi
        /// Il booleano serve per abilitare o meno la get degli item collegati ai report
        /// Bool daAmm: Se false filtro in query (DPA_PREGRESSI.ID_UTENTE_CREATORE = infoutente.idPeople AND DPA_PREGRESSI.ID_RUOLO_CREATORE = infoutente.idGruppo)
        /// RICHIESTA DEL CLIENTE: non è più necessario specificare il bool daAmm. Impostare un bool qualsiasi (true/false)
        /// </summary>
        /// <param name="getItem"></param>
        /// <returns></returns>
        public DocsPaVO.Import.Pregressi.ReportPregressi[] GetReports(bool getItems, InfoUtente infoUtente, bool daAmm)
        {
            DocsPaVO.Import.Pregressi.ReportPregressi[] result = null;

            try
            {
                DataSet ds_all_report = new DataSet();
                //DataSet ds_item = new DataSet();

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //Commentato per richiesta del cliente
                    //if (daAmm)
                    //{
                        //Prendo tutti i report
                        Query query_report = InitQuery.getInstance().getQuery("S_GET_ALL_REPORT_PREGRESSO");
                        query_report.setParam("id_amm", infoUtente.idAmministrazione);
                        query_report.setParam("daAmm", string.Empty);
                        
                        string commandText = query_report.getSQL();
                        logger.Debug(commandText);

                        dbProvider.ExecuteQuery(ds_all_report, "Report", commandText);


                        //dbProvider.ExecuteQuery(ds_all_report, query_report.ToString());
                        //Fine Query

                        if (ds_all_report != null && ds_all_report.Tables[0].Rows.Count > 0)
                        {
                            result = new DocsPaVO.Import.Pregressi.ReportPregressi[ds_all_report.Tables[0].Rows.Count];

                            for (int i = 0; i < ds_all_report.Tables[0].Rows.Count; i++)
                            {

                                DocsPaVO.Import.Pregressi.ReportPregressi rp = new DocsPaVO.Import.Pregressi.ReportPregressi();

                                rp.itemPregressi = new List<DocsPaVO.Import.Pregressi.ItemReportPregressi>();

                                rp.systemId = ds_all_report.Tables[0].Rows[i]["SYSTEM_ID"].ToString();
                                rp.idAmm = ds_all_report.Tables[0].Rows[i]["ID_AMM"].ToString();
                                rp.idUtenteCreatore = ds_all_report.Tables[0].Rows[i]["ID_UTENTE_CREATORE"].ToString();
                                rp.idRuoloCreatore = ds_all_report.Tables[0].Rows[i]["ID_RUOLO_CREATORE"].ToString();
                                rp.dataEsecuzione = ds_all_report.Tables[0].Rows[i]["DATA_ESECUZIONE"].ToString();
                                rp.dataFine = ds_all_report.Tables[0].Rows[i]["DATA_FINE"].ToString();
                                rp.numDoc = ds_all_report.Tables[0].Rows[i]["NUM_DOC"].ToString();
                                rp.descrizione = ds_all_report.Tables[0].Rows[i]["DESCRIZIONE"].ToString();
                                rp.numeroElaborati = ds_all_report.Tables[0].Rows[i]["ELABORATI"].ToString();

                                rp.inError = ds_all_report.Tables[0].Rows[i]["ERRORI"].ToString();

                                //result[i] = rp;

                                if (getItems)
                                {
                                    DataSet ds_item = new DataSet();

                                    //Query per il popolamento della lista di itemReportPregressi
                                    //Query:
                                    Query query_item = InitQuery.getInstance().getQuery("S_GET_ITEM_REPORT_PREGRESSO_BY_SYSTEMID");
                                    query_item.setParam("id_report", rp.systemId);

                                    string commandText_item = query_item.getSQL();
                                    logger.Debug(commandText_item);

                                    dbProvider.ExecuteQuery(ds_item, "Item Report", commandText_item);
                                    //dbProvider.ExecuteQuery(ds_item, query_item.ToString());

                                    if (ds_item != null && ds_item.Tables[0].Rows.Count > 0)
                                    {
                                        foreach (DataRow row_item in ds_item.Tables[0].Rows)
                                        {
                                            DocsPaVO.Import.Pregressi.ItemReportPregressi item = new DocsPaVO.Import.Pregressi.ItemReportPregressi();

                                            //Popolamento:
                                            item.systemId = row_item["SYSTEM_ID"].ToString();
                                            item.idPregresso = row_item["ID_PREGRESSO"].ToString();
                                            item.idRegistro = row_item["ID_REGISTRO"].ToString();
                                            item.idDocumento = row_item["ID_DOCUMENTO"].ToString();
                                            item.idUtente = row_item["ID_UTENTE"].ToString();
                                            item.tipoOperazione = row_item["TIPO_OPERAZIONE"].ToString();
                                            item.data = row_item["DATA"].ToString();
                                            item.errore = row_item["ERRORE"].ToString();
                                            item.esito = row_item["ESITO"].ToString();
                                            //item.esito = (bool)row_item["ESITO"];
                                            item.idNumProtocolloExcel = row_item["ID_NUM_PROTO_EXCEL"].ToString();

                                            //Inserimento:
                                            rp.itemPregressi.Add(item);
                                            //result[i].itemPregressi.Add(item);
                                            //result[i].AddItemReportPregressi(item);
                                        }
                                    }
                                }

                                result[i] = rp;

                            }
                        }
                    //}     //Commentato per richiesta del cliente
                    //else 
                    //daAmm=false
                    //{
                    //    //Prendo tutti i report
                    //    Query query_report = InitQuery.getInstance().getQuery("S_GET_ALL_REPORT_PREGRESSO");
                    //    query_report.setParam("id_amm", infoUtente.idAmministrazione);
                    //    query_report.setParam("daAmm",  " and p.ID_UTENTE_CREATORE =" + infoUtente.idPeople +" and p.ID_RUOLO_CREATORE =" +infoUtente.idGruppo);
                        

                    //    string commandText = query_report.getSQL();
                    //    logger.Debug(commandText);

                    //    dbProvider.ExecuteQuery(ds_all_report, "Report", commandText);


                    //    //dbProvider.ExecuteQuery(ds_all_report, query_report.ToString());
                    //    //Fine Query

                    //    if (ds_all_report != null && ds_all_report.Tables[0].Rows.Count > 0)
                    //    {
                    //        result = new DocsPaVO.Import.Pregressi.ReportPregressi[ds_all_report.Tables[0].Rows.Count];

                    //        for (int i = 0; i < ds_all_report.Tables[0].Rows.Count; i++)
                    //        {

                    //            DocsPaVO.Import.Pregressi.ReportPregressi rp = new DocsPaVO.Import.Pregressi.ReportPregressi();

                    //            rp.itemPregressi = new List<DocsPaVO.Import.Pregressi.ItemReportPregressi>();

                    //            rp.systemId = ds_all_report.Tables[0].Rows[i]["SYSTEM_ID"].ToString();
                    //            rp.idAmm = ds_all_report.Tables[0].Rows[i]["ID_AMM"].ToString();
                    //            rp.idUtenteCreatore = ds_all_report.Tables[0].Rows[i]["ID_UTENTE_CREATORE"].ToString();
                    //            rp.idRuoloCreatore = ds_all_report.Tables[0].Rows[i]["ID_RUOLO_CREATORE"].ToString();
                    //            rp.dataEsecuzione = ds_all_report.Tables[0].Rows[i]["DATA_ESECUZIONE"].ToString();
                    //            rp.dataFine = ds_all_report.Tables[0].Rows[i]["DATA_FINE"].ToString();
                    //            rp.numDoc = ds_all_report.Tables[0].Rows[i]["NUM_DOC"].ToString();
                    //            rp.descrizione = ds_all_report.Tables[0].Rows[i]["DESCRIZIONE"].ToString();
                    //            rp.numeroElaborati = ds_all_report.Tables[0].Rows[i]["ELABORATI"].ToString();
                                
                    //            rp.inError = ds_all_report.Tables[0].Rows[i]["ERRORI"].ToString();
                                
                    //            //result[i] = rp;

                    //            if (getItems)
                    //            {
                    //                DataSet ds_item = new DataSet();

                    //                //Query per il popolamento della lista di itemReportPregressi
                    //                //Query:
                    //                Query query_item = InitQuery.getInstance().getQuery("S_GET_ITEM_REPORT_PREGRESSO_BY_SYSTEMID");
                    //                query_item.setParam("id_report", rp.systemId);

                    //                string commandText_item = query_item.getSQL();
                    //                logger.Debug(commandText_item);

                    //                dbProvider.ExecuteQuery(ds_item, "Item Report", commandText_item);
                    //                //dbProvider.ExecuteQuery(ds_item, query_item.ToString());

                    //                if (ds_item != null && ds_item.Tables[0].Rows.Count > 0)
                    //                {
                    //                    foreach (DataRow row_item in ds_item.Tables[0].Rows)
                    //                    {
                    //                        DocsPaVO.Import.Pregressi.ItemReportPregressi item = new DocsPaVO.Import.Pregressi.ItemReportPregressi();

                    //                        //Popolamento:
                    //                        item.systemId = row_item["SYSTEM_ID"].ToString();
                    //                        item.idPregresso = row_item["ID_PREGRESSO"].ToString();
                    //                        item.idRegistro = row_item["ID_REGISTRO"].ToString();
                    //                        item.idDocumento = row_item["ID_DOCUMENTO"].ToString();
                    //                        item.idUtente = row_item["ID_UTENTE"].ToString();
                    //                        item.tipoOperazione = row_item["TIPO_OPERAZIONE"].ToString();
                    //                        item.data = row_item["DATA"].ToString();
                    //                        item.errore = row_item["ERRORE"].ToString();
                    //                        item.esito = row_item["ESITO"].ToString();
                    //                        //item.esito = (bool)row_item["ESITO"];
                    //                        item.idNumProtocolloExcel = row_item["ID_NUM_PROTO_EXCEL"].ToString();

                    //                        //Inserimento:
                    //                        rp.itemPregressi.Add(item);
                    //                        //result[i].itemPregressi.Add(item);
                    //                        //result[i].AddItemReportPregressi(item);
                    //                    }
                    //                }
                    //            }

                    //            result[i] = rp;

                    //        }
                    //    }
                    //}
                }

            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Andrea De Marco - Get degli allegati di un item per SystemId
        /// </summary>
        /// <param name="getItems"></param>
        /// <returns></returns>
        public List<DocsPaVO.Import.Pregressi.Allegati> GetAllegatiByIdItem(string idItem)
        {
            List<DocsPaVO.Import.Pregressi.Allegati> result = null;

            try
            {
                DataSet ds_allegati = new DataSet();

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    //Prendo gli allegati in base al id_item
                    Query query_allegati = InitQuery.getInstance().getQuery("S_GET_ALLEGATI_ITEM_REPORT_PREGRESSI_BY_SYSTEMID");
                    query_allegati.setParam("id_item", idItem);

                    string commandText = query_allegati.getSQL();
                    logger.Debug(commandText);

                    dbProvider.ExecuteQuery(ds_allegati, "allegati", commandText);
                    //dbProvider.ExecuteQuery(ds_allegati, query_allegati.ToString());
                    //Fine Query

                    if (ds_allegati != null && ds_allegati.Tables[0].Rows.Count > 0)
                    {
                        result = new List<DocsPaVO.Import.Pregressi.Allegati>();

                        for (int i = 0; i < ds_allegati.Tables[0].Rows.Count; i++)
                        {

                            DocsPaVO.Import.Pregressi.Allegati allegato = new DocsPaVO.Import.Pregressi.Allegati();
                            //Popolamento:
                            allegato.systemId = ds_allegati.Tables[0].Rows[i]["SYSTEM_ID"].ToString();
                            allegato.idItem = ds_allegati.Tables[0].Rows[i]["ID_ITEM"].ToString();
                            allegato.idItem = ds_allegati.Tables[0].Rows[i]["ERRORE"].ToString();
                            allegato.idItem = ds_allegati.Tables[0].Rows[i]["ESITO"].ToString();
                            //Inserimento:
                            result.Add(allegato);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                result = null;
            }

            return result;
        }


        //Andrea De Marco - Insert del Report Import Pregressi
        public string InsertReportPregressi(DocsPaVO.Import.Pregressi.ReportPregressi report)
        {
            string system_id = string.Empty;
            try
            {

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.BeginTransaction();

                    Query query = InitQuery.getInstance().getQuery("I_DPA_PREGRESSI");

                    query.setParam("paramA", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    query.setParam("paramB", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_PREGRESSI"));
                    query.setParam("id_amm", report.idAmm);
                    query.setParam("id_utente_creatore", report.idUtenteCreatore);
                    query.setParam("id_ruolo_creatore", report.idRuoloCreatore);
                    query.setParam("data_esecuzione", DocsPaDbManagement.Functions.Functions.GetDate());
                    query.setParam("data_fine", DocsPaDbManagement.Functions.Functions.ToDate(report.dataFine));
                    query.setParam("num_doc", report.itemPregressi.Count().ToString());
                    //Andrea - Aggiunta descrizione report
                    string desc = string.Empty;              
                    if (string.IsNullOrEmpty(report.descrizione))
                    {
                        query.setParam("descrizione", "--");
                    }
                    else
                    {
                        desc = report.descrizione.Replace("'", "''");
                        query.setParam("descrizione", report.descrizione.ToString());
                    }
                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - InsertReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + commandText);
                    logger.Debug("SQL - InsertReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + commandText);

                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                    else
                    {
                        // Recupero systemid appena inserito
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_PREGRESSI");
                        System.Diagnostics.Debug.WriteLine("SQL - InsertReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + sql);
                        logger.Debug("SQL - InsertReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + sql);
                        dbProvider.ExecuteScalar(out system_id, sql);

                        dbProvider.CommitTransaction();
                    }
                }


            }
            catch (Exception e)
            {
                system_id = string.Empty;
                logger.Debug(e.Message);
            }

            return system_id;
        }



        //Andrea De Marco - Insert del Item Report - Import Pregressi - /*item.id_pregresso = report.system_id*/
        public string InsertItemReportPregressi(DocsPaVO.Import.Pregressi.ItemReportPregressi item, string idReport)
        {
            string id_item = string.Empty;
            try
            {

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.BeginTransaction();

                    Query query = InitQuery.getInstance().getQuery("I_DPA_ASS_PREGRESSI");

                    query.setParam("paramA", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    query.setParam("paramB", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_PREGRESSI"));
                    query.setParam("id_pregresso", idReport);
                    if (!string.IsNullOrEmpty(item.idRegistro))
                    {
                        query.setParam("id_registro", item.idRegistro);
                    }
                    else
                    {
                        query.setParam("id_registro", "null");
                    }
                   
                    //query.setParam("id_documento", item.idDocumento);
                    if (!string.IsNullOrEmpty(item.idUtente))
                    {
                        query.setParam("id_utente", item.idUtente);
                    }
                    else
                    {
                        query.setParam("id_utente", "null");
                    }
                    if (!string.IsNullOrEmpty(item.idRuolo))
                    {
                        query.setParam("id_ruolo", item.idRuolo);
                    }
                    else
                    {
                        query.setParam("id_ruolo", "null");
                    }
                    query.setParam("tipo_operazione", item.tipoOperazione);
                    query.setParam("data", DocsPaDbManagement.Functions.Functions.ToDate(item.data));
                    query.setParam("id_num_proto_excel", item.idNumProtocolloExcel);

                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - InsertItemReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + commandText);
                    logger.Debug("SQL - InsertItemReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + commandText);
                    //dbProvider.ExecuteNonQuery(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                    else
                    {
                        // Recupero IdItem appena inserito
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_ASS_PREGRESSI");
                        System.Diagnostics.Debug.WriteLine("SQL - InsertItemReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + sql);
                        logger.Debug("SQL - InsertItemReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + sql);
                        dbProvider.ExecuteScalar(out id_item, sql);

                        dbProvider.CommitTransaction();
                    }
                }


            }
            catch (Exception e)
            {
                id_item = string.Empty;
                logger.Debug(e.Message);
            }

            return id_item;
        }


        /// <summary>
        /// Andrea De Marco - Insert del Allegato di un Item Report - Import Pregressi
        /// </summary>
        /// <param name="item"></param>
        /// <param name="idReport"></param>
        /// <returns></returns>
        public string InsertAllegatoItemReportPregressi(DocsPaVO.Import.Pregressi.Allegati allegato, string idItem)
        {
            string result = string.Empty;
            try
            {

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.BeginTransaction();

                    Query query = InitQuery.getInstance().getQuery("I_DPA_ASS_ALLEGATO");

                    query.setParam("paramA", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    query.setParam("paramB", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_ALLEGATO"));
                    query.setParam("id_item", idItem);
                    query.setParam("errore", allegato.errore);
                    query.setParam("esito", allegato.esito);

                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - InsertAllegatoItemReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + commandText);
                    logger.Debug("SQL - InsertAllegatoItemReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + commandText);
                    //dbProvider.ExecuteNonQuery(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                    else
                    {
                        string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_ASS_ALLEGATO");
                        System.Diagnostics.Debug.WriteLine("SQL - InsertItemReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + sql);
                        logger.Debug("SQL - InsertItemReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + sql);
                        dbProvider.ExecuteScalar(out result, sql);

                        dbProvider.CommitTransaction();
                    }
                }


            }
            catch (Exception e)
            {
                result = string.Empty;
                logger.Debug(e.Message);
            }

            return result;
        }

        //Andrea De Marco - Delete del Item Report e Report - Import Pregressi
        public bool DeleteReport(string idReport)
        {
            bool result = true;
            DocsPaDB.DBProvider dbProvider = new DBProvider();

            dbProvider.BeginTransaction();

            try
            {

                DataSet ds = new DataSet();

                using (dbProvider)
                {
                    //Select che recupera idItem a partire dall'idReport
                    Query queryIdItem = InitQuery.getInstance().getQuery("S_GET_ID_ITEM_DPA_ASS_PREGRESSI");
                    queryIdItem.setParam("id_report", idReport);

                    string commandText_queryIdItem = queryIdItem.getSQL();
                    logger.Debug(commandText_queryIdItem);

                    dbProvider.ExecuteQuery(ds, "Id Item Report", commandText_queryIdItem);
                    //dbProvider.ExecuteQuery(ds, queryIdItem.ToString());

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {

                        Query queryAlleg = InitQuery.getInstance().getQuery("D_DELETE_ALLEGATO_ITEM_REPORT_PREGRESSI");

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            string idItem = ds.Tables[0].Rows[i].ToString();
                            queryAlleg.setParam("id_item", idItem);
                            dbProvider.ExecuteNonQuery(queryAlleg.getSQL());

                        }

                        Query query = InitQuery.getInstance().getQuery("D_DELETE_ITEM_REPORT_PREGRESSI");
                        query.setParam("id_pregresso", idReport);
                        dbProvider.ExecuteNonQuery(query.getSQL());

                    }

                    Query q = InitQuery.getInstance().getQuery("D_DELETE_REPORT_PREGRESSI");
                    q.setParam("system_id", idReport);
                    dbProvider.ExecuteNonQuery(q.getSQL());

                    dbProvider.CommitTransaction();

                }
            }
            catch (Exception e)
            {
                result = false;
                dbProvider.RollbackTransaction();
                logger.Debug(e.Message);
            }

            return result;
        }

        /// <summary>
        /// Andrea De Marco - Update dell'item Import Pregressi
        /// </summary>
        /// <returns></returns>
        public bool UpdateItemReportPregressi(DocsPaVO.Import.Pregressi.ItemReportPregressi item, string sysId)
        {
            bool result = true;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {

                    dbProvider.BeginTransaction();

                    Query query = InitQuery.getInstance().getQuery("U_ITEM_REPORT_PREGRESSI");

                    if (!string.IsNullOrEmpty(item.idDocumento))
                    {
                        query.setParam("id_documento", item.idDocumento);
                    }
                    else
                    {
                        query.setParam("id_documento", "null");
                    }

                    string errore = string.Empty;
                    if(!string.IsNullOrEmpty(item.errore))
                    {
                        errore = item.errore.Replace("'","''");
                    }

                    query.setParam("errore", errore);
                    query.setParam("esito", item.esito);
                    query.setParam("system_id", sysId);

                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - UpdateItemReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + commandText);
                    logger.Debug("SQL - UpdateItemReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + commandText);
                    //dbProvider.ExecuteNonQuery(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                    else
                    {
                        dbProvider.CommitTransaction();
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }

            return result;

        }

        /// <summary>
        /// Andrea De Marco - Update della Data Fine del Report Import Pregressi
        /// </summary>
        /// <returns></returns>
        public bool UpdateReportPregressi(string sysId)
        {
            bool result = true;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.BeginTransaction();
                    Query query = InitQuery.getInstance().getQuery("U_REPORT_PREGRESSI");

                    query.setParam("data_fine", DocsPaDbManagement.Functions.Functions.GetDate());
                    query.setParam("system_id", sysId);

                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - UpdateReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + commandText);
                    logger.Debug("SQL - UpdateReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + commandText);
                    //dbProvider.ExecuteNonQuery(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                    else
                    {
                        dbProvider.CommitTransaction();
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }

            return result;

        }


        /// <summary>
        /// Andrea De Marco - Update dell'allegato Import Pregressi
        /// </summary>
        /// <returns></returns>
        public bool UpdateAllegatoItemReportPregressi(DocsPaVO.Import.Pregressi.Allegati allegato, string sysId)
        {
            bool result = true;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.BeginTransaction();
                    Query query = InitQuery.getInstance().getQuery("U_ALLEGATO_ITEM_REPORT_PREGRESSI");

                    query.setParam("id_item", allegato.idItem);
                    query.setParam("errore", allegato.errore);
                    query.setParam("esito", allegato.esito);
                    query.setParam("system_id", sysId);

                    string commandText = query.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - UpdateAllegatoItemReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + commandText);
                    logger.Debug("SQL - UpdateAllegatoItemReportPregressi - DocsPaDB/ImportPregressi.cs - QUERY : " + commandText);
                    //dbProvider.ExecuteNonQuery(commandText);
                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                    else
                    {
                        dbProvider.CommitTransaction();
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }

            return result;

        }

        public bool NumeroProtocolloUnivocoPerReg(string numproto, string codreg)
        {
            bool result = true;
            DocsPaUtils.Query q;
            string queryString;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("V_PROTO_PREGRESSI");

                    q.setParam("param1", codreg);
                    q.setParam("param2", " = " + numproto);

                    queryString = q.getSQL();
                    logger.Debug("Inserimento nuova applicazione: ");
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                    {
                        result = false;
                        throw new Exception();
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            return result;
        }


        public List<string> GetExistingProtocolNumber(string idRegistro, string whereClausole, string regYear)
        {
            List<string> result = null;
            DocsPaUtils.Query q;
            string queryString;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("V_PROTO_PREGRESSI");

                    q.setParam("param1", idRegistro);
                    q.setParam("param2", whereClausole);
                    q.setParam("param3", regYear);

                    queryString = q.getSQL();
                    logger.Debug("Verifica univocità numero protocolli in import pregressi: ");
                    logger.Debug(queryString);
                    DataSet dataSet;
                    if (dbProvider.ExecuteQuery(out dataSet, "PROFILE", queryString))
                    {
                        result = new List<string>();

                        foreach (DataRow dataRow in dataSet.Tables["PROFILE"].Rows)
                        {
                            result.Add(dataRow["PROTOCOLLATO"].ToString());
                        }
                        dataSet.Dispose();
                    }
                    else
                    {
                        logger.Debug("Errore nell'esecuzione della query in 'GetExistingProtocolNumber'");
                        throw new ApplicationException("Errore nell'esecuzione della query in 'GetExistingProtocolNumber'");
                    }
                }
                return result;

            }
            catch (Exception e)
            {
                result = null;
                logger.Debug(e.Message);
            }
            return result;
        }

        public List<string> GetInvalidProtocolNumber(List<string> functionParameters)
        {
            List<string> result = null;
            string queryString = string.Empty;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
 
                    string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
                    string sqlFunction = (dbType.ToUpper() == "SQL"? DocsPaDbManagement.Functions.Functions.GetDbUserSession() + "." : "") + "CHECKPROTODATA";
                    int k = 0;

                    foreach (string parametro in functionParameters)
                    {
                        if (k < functionParameters.Count)
                            {
                                if (k > 0)
                                {
                                    queryString += ",";
                                }
                                else
                                {
                                    queryString = "SELECT ";
                                }
                            }

                        queryString += sqlFunction + parametro + " AS " + "A_" + k;
                        k++;
                    }

                    if (dbType.ToUpper() == "ORACLE")
                        queryString += " FROM DUAL";

                    logger.Debug("Verifica progressione numerico-temporale protocolli in import pregressi: ");
                    logger.Debug(queryString);
                    DataSet dataSet;
                    if (dbProvider.ExecuteQuery(out dataSet, "PROFILE", queryString))
                    {
                        result = new List<string>();

                        foreach (DataRow dataRow in dataSet.Tables["PROFILE"].Rows)
                        {
                            for(int i = 0; i<dataSet.Tables["PROFILE"].Columns.Count;i++)
                            {
                                string idprofile = dataRow[i].ToString().Trim();
                                if (idprofile!="0")
                                    result.Add(idprofile);
                            }
                        }

                        dataSet.Dispose();
                    }
                    else
                    {
                        logger.Debug("Errore nell'esecuzione della query in 'GetInvalidProtocolNumber'");
                        throw new ApplicationException("Errore nell'esecuzione della query in 'GetInvalidProtocolNumber'");
                    }
                }
                return result;

            }
            catch (Exception e)
            {
                result = null;
                logger.Debug(e.Message);
            }
            return result;
        }

        public List<string> GetIdPeopleIdRuoloFromCodici(string cod_utente, string cod_ruolo, string id_amm)
        {
            List<string> result = null;
            DocsPaUtils.Query q;
            string queryString;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_UTE_RUOL_BY_COD");

                    q.setParam("param1", cod_utente);
                    q.setParam("param2", cod_ruolo);
                    q.setParam("param3", id_amm);

                    queryString = q.getSQL();
                    logger.Debug("Reperimento id_People e id_Ruolo da codice utente e codice ruolo per amministrazione in import pregressi: ");
                    logger.Debug(queryString);
                    DataSet dataSet;
                    if (dbProvider.ExecuteQuery(out dataSet, "S_UTE_RUOL_BY_COD", queryString))
                    {
                        result = new List<string>();

                        foreach (DataRow dataRow in dataSet.Tables["S_UTE_RUOL_BY_COD"].Rows)
                        {
                            result.Add(dataRow["ID_RUOLO"].ToString());
                            result.Add(dataRow["ID_PEOPLE"].ToString());
                        }
                        dataSet.Dispose();
                    }
                    else
                    {
                        logger.Debug("Errore nell'esecuzione della query in 'GetIdPeopleIdRuoloFromCodici'");
                        throw new ApplicationException("Errore nell'esecuzione della query in 'GetIdPeopleIdRuoloFromCodici'");
                    }
                }
                return result;

            }
            catch (Exception e)
            {
                result = null;
                logger.Debug(e.Message);
            }
            return result;
        }

        public List<string> GetExistingProtocolNumber(string whereClausole)
        {
            List<string> result = null;
            DocsPaUtils.Query q;
            string queryString;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("V_IDVECCHIO_PREGRESSI");

                    q.setParam("param1", whereClausole);

                    queryString = q.getSQL();
                    logger.Debug("Verifica univocità id vecchio documento in import pregressi: ");
                    logger.Debug(queryString);
                    DataSet dataSet;
                    if (dbProvider.ExecuteQuery(out dataSet, "PROFILE", queryString))
                    {
                        result = new List<string>();

                        foreach (DataRow dataRow in dataSet.Tables["PROFILE"].Rows)
                        {
                            result.Add(dataRow["PROTOCOLLATO"].ToString());
                        }
                        dataSet.Dispose();
                    }
                    else
                    {
                        logger.Debug("Errore nell'esecuzione della query in 'GetExistingProtocolNumber'");
                        throw new ApplicationException("Errore nell'esecuzione della query in 'GetExistingProtocolNumber'");
                    }
                }
                return result;

            }
            catch (Exception e)
            {
                result = null;
                logger.Debug(e.Message);
            }
            return result;
        }

        public List<string> GetInvalidOldNumber(List<string> functionParameters)
        {
            List<string> result = null;
            string queryString = string.Empty;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {

                    string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
                    string sqlFunction = (dbType.ToUpper() == "SQL" ? DocsPaDbManagement.Functions.Functions.GetDbUserSession() + "." : "") + "CHECKPROFILEDATA";
                    int k = 0;

                    foreach (string parametro in functionParameters)
                    {
                        if (k < functionParameters.Count)
                        {
                            if (k > 0)
                            {
                                queryString += ",";
                            }
                            else
                            {
                                queryString = "SELECT ";
                            }
                        }

                        queryString += sqlFunction + parametro + " AS " + "A_" + k;
                        k++;
                    }

                    if (dbType.ToUpper() == "ORACLE")
                        queryString += " FROM DUAL";

                    logger.Debug("Verifica progressione numerico-temporale documenti non protocollati in import pregressi: ");
                    logger.Debug(queryString);
                    DataSet dataSet;
                    if (dbProvider.ExecuteQuery(out dataSet, "PROFILE", queryString))
                    {
                        result = new List<string>();

                        foreach (DataRow dataRow in dataSet.Tables["PROFILE"].Rows)
                        {
                            for (int i = 0; i < dataSet.Tables["PROFILE"].Columns.Count; i++)
                            {
                                string idprofile = dataRow[i].ToString().Trim();
                                if (idprofile != "0")
                                    result.Add(idprofile);
                            }
                        }

                        dataSet.Dispose();
                    }
                    else
                    {
                        logger.Debug("Errore nell'esecuzione della query in 'GetInvalidProtocolNumber'");
                        throw new ApplicationException("Errore nell'esecuzione della query in 'GetInvalidProtocolNumber'");
                    }
                }
                return result;

            }
            catch (Exception e)
            {
                result = null;
                logger.Debug(e.Message);
            }
            return result;
        }
    }
}
