using System;
using System.Collections;
using System.ComponentModel;
using System.Web.Services;
using System.Xml.Serialization;
using System.Xml;
using System.Configuration;
using System.IO;
using System.Data;
using System.Threading;
using DocsPaUtils;
using log4net;
using System.Collections.Generic;
using System.Linq;
using DocsPaUtils.Data;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.ProfilazioneDinamicaLite;

namespace DocsPaDB.Query_DocsPAWS
{
    public class Model
    {
        private ILog logger = LogManager.GetLogger(typeof(Model));

        public Model() { }

        public ArrayList getIdModelliTrasmAssociati(string idTipoDoc, string idDiagramma, string idStato)
        {
            ArrayList idModelliTrasmSelezionati = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                if (idStato != "")
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_ID_MODELLO_TIPODOC_DS");
                    queryMng.setParam("idTipoDoc", idTipoDoc);
                    queryMng.setParam("idDiagramma", idDiagramma);
                    queryMng.setParam("idStato", idStato);
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getIdModelliTrasmAssociati - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getIdModelliTrasmAssociati - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm obj = new DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm();
                        obj.SYTSEM_ID = ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString();
                        obj.ID_TIPO_DOC = ds.Tables[0].Rows[i]["Id_tipo_doc"].ToString();
                        obj.ID_DIAGRAMMA = ds.Tables[0].Rows[i]["Id_diagramma"].ToString();
                        obj.ID_TEMPLATE = ds.Tables[0].Rows[i]["Id_Mod_Trasm"].ToString();
                        obj.ID_STATO = ds.Tables[0].Rows[i]["Id_stato"].ToString();
                        obj.TRASM_AUT = ds.Tables[0].Rows[i]["Trasm_aut"].ToString();
                        idModelliTrasmSelezionati.Add(obj);
                    }
                }
                else
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_ID_MODELLO_TIPODOC_DS_1");
                    queryMng.setParam("idTipoDoc", idTipoDoc);
                    queryMng.setParam("idDiagramma", idDiagramma);
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getIdModelliTrasmAssociati - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getIdModelliTrasmAssociati - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm obj = new DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm();
                        obj.SYTSEM_ID = ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString();
                        obj.ID_TIPO_DOC = ds.Tables[0].Rows[i]["Id_tipo_doc"].ToString();
                        obj.ID_DIAGRAMMA = ds.Tables[0].Rows[i]["Id_diagramma"].ToString();
                        obj.ID_TEMPLATE = ds.Tables[0].Rows[i]["Id_Mod_Trasm"].ToString();
                        obj.ID_STATO = ds.Tables[0].Rows[i]["Id_stato"].ToString();
                        obj.TRASM_AUT = ds.Tables[0].Rows[i]["Trasm_aut"].ToString();
                        idModelliTrasmSelezionati.Add(obj);
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }
            return idModelliTrasmSelezionati;
        }

        public DocsPaVO.ProfilazioneDinamica.OggettoCustom getOggettoById(string idOggetto)
        {
            DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto = null;
            DocsPaDB.DBProvider dbProvider = null;
            try
            {
                dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query qry = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_OGGETTO_BY_ID");
                qry.setParam("param1", idOggetto);

                string commandText = qry.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getOggettoById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getOggettoById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
                if (dbProvider.ExecuteQuery(ds, commandText) && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    oggetto = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                    setOggettoCustom(ref oggetto, dr);
                    DocsPaVO.ProfilazioneDinamica.TipoOggetto tipo = new DocsPaVO.ProfilazioneDinamica.TipoOggetto();
                    setTipoOggetto(ref tipo, dr);
                    oggetto.TIPO = tipo;

                    //campo CLOB di configurazione
                    if (oggetto.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                    {
                        string config = dbProvider.GetLargeText("DPA_OGGETTI_CUSTOM", oggetto.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST");
                        oggetto.CONFIG_OBJ_EST = config;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SQL - getOggettoById - ProfilazioneDinamica/Database/model.cs - Eccezione : " + ex.Message);
                logger.Debug("SQL - getOggettoById - ProfilazioneDinamica/Database/model.cs - Eccezione : " + ex.Message);
            }
            finally
            {
                try
                {
                    dbProvider.Dispose();
                }
                catch { }
            }
            return oggetto;
        }

        public void salvaAssociazioneModelli(string idTipoDoc, string idDiagramma, ArrayList modelliSelezionati, string idStato)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                if (idStato == "")
                {
                    if (idDiagramma == "0")
                    {
                        DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_DISASSOCIA_TIPO_DOC_DIAGRAMMA_1");
                        queryMng.setParam("idTipoDoc", idTipoDoc);
                        string commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaAssociazioneModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                    }
                    else
                    {
                        DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_DISASSOCIA_TIPO_DOC_DIAGRAMMA_2");
                        queryMng.setParam("idDiagramma", idDiagramma);
                        queryMng.setParam("idTipoDoc", idTipoDoc);
                        string commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaAssociazioneModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_DISASSOCIA_TIPO_DOC_DIAGRAMMA_1");
                        queryMng.setParam("idTipoDoc", idTipoDoc);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaAssociazioneModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                    }

                    for (int i = 0; i < modelliSelezionati.Count; i++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm obj = (DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm)modelliSelezionati[i];
                        if (idDiagramma == "0")
                        {
                            bool retValue = false;
                            int rowsAffected;
                            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_ASSOCIA_TIPO_DOC_DIAGRAMMA_1");
                            queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                            queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_DIAGRAMMI"));
                            queryMng.setParam("idTipoDoc", obj.ID_TIPO_DOC);
                            queryMng.setParam("idTemplate", obj.ID_TEMPLATE);

                            string commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - salvaAssociazioneModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);
                        }
                        else
                        {
                            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_ASSOCIA_TIPO_DOC_DIAGRAMMA_2");
                            queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                            queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_DIAGRAMMI"));
                            queryMng.setParam("idTipoDoc", obj.ID_TIPO_DOC);
                            queryMng.setParam("idDiagramma", obj.ID_DIAGRAMMA);
                            queryMng.setParam("idTemplate", obj.ID_TEMPLATE);

                            string commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - salvaAssociazioneModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);
                        }
                    }
                }
                else
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_DISASSOCIA_TIPO_DOC_DIAGRAMMA_3");
                    queryMng.setParam("idDiagramma", idDiagramma);
                    queryMng.setParam("idTipoDoc", idTipoDoc);
                    queryMng.setParam("idStato", idStato);
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaAssociazioneModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);

                    for (int i = 0; i < modelliSelezionati.Count; i++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm obj = (DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm)modelliSelezionati[i];

                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_ASSOCIA_TIPO_DOC_DIAGRAMMA_3");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_DIAGRAMMI"));
                        queryMng.setParam("idTipoDoc", obj.ID_TIPO_DOC);
                        queryMng.setParam("idDiagramma", obj.ID_DIAGRAMMA);
                        queryMng.setParam("idTemplate", obj.ID_TEMPLATE);
                        queryMng.setParam("idStato", obj.ID_STATO);
                        queryMng.setParam("trasmAut", obj.TRASM_AUT);

                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaAssociazioneModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                    }
                }
            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public string getIdAmmByCod(string codiceAmministrazione)
        {
            DataSet ds = new DataSet();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_ID_AMM_BY_COD");
                queryMng.setParam("codiceAmministrazione", codiceAmministrazione);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getIdAmmByCod - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getIdAmmByCod - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteQuery(ds, commandText);
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString();
        }

        public ArrayList getTipiOggetto()
        {
            DataSet ds = new DataSet();
            ArrayList result = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TIPI_OGGETTO");
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTipiOggetto - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTipiOggetto - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteQuery(ds, commandText);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    result.Add((string)ds.Tables[0].Rows[i]["TIPO"]);
                }

            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return result;
        }

        public ArrayList getTipiDocumento(string idAmministrazione)
        {
            DataSet ds = new DataSet();
            ArrayList result = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TIPI_DOCUMENTO");
                queryMng.setParam("idAmministrazione", idAmministrazione);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTipiDocumento - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTipiDocumento - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteQuery(ds, commandText);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    result.Add((string)ds.Tables[0].Rows[i]["VAR_DESC_ATTO"]);
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }
            return result;
        }

        public bool salvaTemplate(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione)
        {
            template.gestisciCaratteriSpeciali();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            bool retValue = false;
            int rowsAffected;

            try
            {
                //Inserimento DPA_TEMPLATES
                string system_id_templates = string.Empty;
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_TEMPLATES");
                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_TIPO_ATTO"));
                queryMng.setParam("Descrizione", template.DESCRIZIONE);
                queryMng.setParam("id_amm", idAmministrazione);
                queryMng.setParam("Abilitato_SI_NO", "1");
                queryMng.setParam("In_Esercizio", "NO");
                queryMng.setParam("CHA_PRIVATO", template.PRIVATO);
                queryMng.setParam("NUM_MESI_CONSERVAZIONE", template.NUM_MESI_CONSERVAZIONE);
                queryMng.setParam("CHA_CONSERVAZIONE", template.INVIO_CONSERVAZIONE);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                if (dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                {
                    retValue = (rowsAffected > 0);
                    if (retValue)
                    {
                        // Reperimento systemid
                        commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_TIPO_ATTO");
                        dbProvider.ExecuteScalar(out system_id_templates, commandText);
                    }
                    else
                    {
                        dbProvider.RollbackTransaction();
                        return false;
                    }
                }

                //Inserimento DPA_OGGETTI_CUSTOM
                for (int i = 0; i < template.ELENCO_OGGETTI.Count; i++)
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[i];
                    oggettoCustom.gestisciCaratteriSpeciali();
                    //Cerco l'ID_TIPO_OGGETTO per l'oggettoCustom in questione
                    int system_id_tipo_oggetto = 0;
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TIPO_OGGETTO");
                    queryMng.setParam("Descrizione", oggettoCustom.TIPO.DESCRIZIONE_TIPO);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        system_id_tipo_oggetto = Convert.ToInt32(ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                    }
                    else
                    {
                        logger.Debug("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - ERRRORE : system_id TIPO OGGETTO non trovata");
                        dbProvider.RollbackTransaction();
                        return false;
                    }

                    //Inserimento oggettoCustom
                    string system_id_oggettoCustom = string.Empty;
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_OGGETTO_CUSTOM");
                    queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_OGGETTI_CUSTOM"));
                    queryMng.setParam("Descrizione", oggettoCustom.DESCRIZIONE);
                    queryMng.setParam("Orizzontale_Verticale", oggettoCustom.ORIZZONTALE_VERTICALE);
                    queryMng.setParam("Campo_Obbligatorio", oggettoCustom.CAMPO_OBBLIGATORIO);
                    queryMng.setParam("Multilinea", oggettoCustom.MULTILINEA);
                    queryMng.setParam("Numero_Di_Linee", oggettoCustom.NUMERO_DI_LINEE);
                    queryMng.setParam("Numero_Di_Caratteri", oggettoCustom.NUMERO_DI_CARATTERI);
                    queryMng.setParam("Campo_Di_Ricerca", oggettoCustom.CAMPO_DI_RICERCA);
                    queryMng.setParam("ID_TIPO_OGGETTO", system_id_tipo_oggetto.ToString());
                    queryMng.setParam("Reset_Anno", oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO);
                    queryMng.setParam("Formato_Contatore", oggettoCustom.FORMATO_CONTATORE);
                    queryMng.setParam("ID_R_DEFAULT", oggettoCustom.ID_RUOLO_DEFAULT);
                    queryMng.setParam("RICERCA_CORR", oggettoCustom.TIPO_RICERCA_CORR);
                    queryMng.setParam("tipoContatore", oggettoCustom.TIPO_CONTATORE);
                    queryMng.setParam("formatoOra", oggettoCustom.FORMATO_ORA);


                    if (oggettoCustom.CAMPO_COMUNE != null && oggettoCustom.CAMPO_COMUNE == "1")
                        queryMng.setParam("CAMPO_COMUNE", oggettoCustom.CAMPO_COMUNE);
                    else
                        queryMng.setParam("CAMPO_COMUNE", "null");

                    if (oggettoCustom.CONTA_DOPO != null && oggettoCustom.CONTA_DOPO == "1")
                        queryMng.setParam("contaDopo", "1");
                    else
                        queryMng.setParam("contaDopo", "0");

                    if (oggettoCustom.REPERTORIO != null && oggettoCustom.REPERTORIO == "1")
                    {
                        queryMng.setParam("repertorio", "1");
                        if (oggettoCustom.CONS_REPERTORIO != null && oggettoCustom.CONS_REPERTORIO == "1")
                            queryMng.setParam("cons_repertorio", "1");
                        else
                            queryMng.setParam("cons_repertorio", "0");
                    }
                    else
                    {
                        queryMng.setParam("repertorio", "0");
                        queryMng.setParam("cons_repertorio", "0");
                    }
                    // INTEGRAZIONE PITRE-PARER
                    if (oggettoCustom.CONSOLIDAMENTO != null && oggettoCustom.CONSOLIDAMENTO == "1")
                        queryMng.setParam("consolidamento", "1");
                    else
                        queryMng.setParam("consolidamento", "0");
                    if (oggettoCustom.CONSERVAZIONE != null && oggettoCustom.CONSERVAZIONE == "1")
                        queryMng.setParam("conservazione", "1");
                    else
                        queryMng.setParam("conservazione", "0");

                    if (oggettoCustom.DA_VISUALIZZARE_RICERCA != null && oggettoCustom.DA_VISUALIZZARE_RICERCA == "1")
                        queryMng.setParam("da_visualizzare_ricerca", "1");
                    else
                        queryMng.setParam("da_visualizzare_ricerca", "0");

                    if (!string.IsNullOrEmpty(oggettoCustom.TIPO_LINK))
                        queryMng.setParam("TIPO_LINK", oggettoCustom.TIPO_LINK);
                    else
                        queryMng.setParam("TIPO_LINK", "");

                    if (!string.IsNullOrEmpty(oggettoCustom.TIPO_OBJ_LINK))
                        queryMng.setParam("TIPO_OBJ_LINK", oggettoCustom.TIPO_OBJ_LINK);
                    else
                        queryMng.setParam("TIPO_OBJ_LINK", "");

                    if (string.IsNullOrEmpty(oggettoCustom.MODULO_SOTTOCONTATORE))
                        queryMng.setParam("moduloSottocontatore", "0");
                    else
                        queryMng.setParam("moduloSottocontatore", oggettoCustom.MODULO_SOTTOCONTATORE);

                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                    if (dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    {
                        retValue = (rowsAffected > 0);

                        if (retValue)
                        {
                            // Reperimento systemid
                            commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_OGGETTI_CUSTOM");
                            dbProvider.ExecuteScalar(out system_id_oggettoCustom, commandText);

                            if (!string.IsNullOrEmpty(oggettoCustom.DATA_INIZIO) && !string.IsNullOrEmpty(oggettoCustom.DATA_FINE))
                            {
                                //salvo intervallo di date utilizzando data_fine e data_inizio

                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_DATE_CONT_CUSTOM_DOC");
                                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONT_CUSTOM_DOC"));
                                queryMng.setParam("dataFine", DocsPaDbManagement.Functions.Functions.ToDate(oggettoCustom.DATA_FINE));
                                queryMng.setParam("idOgg", system_id_oggettoCustom);
                                queryMng.setParam("dataInizio", DocsPaDbManagement.Functions.Functions.ToDate(oggettoCustom.DATA_INIZIO));


                                commandText = queryMng.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - salva contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                logger.Debug("SQL - salva contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                bool result = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                            }
                        }
                        else
                        {
                            dbProvider.RollbackTransaction();
                            return false;
                        }
                    }


                    if (!string.IsNullOrEmpty(oggettoCustom.CONFIG_OBJ_EST))
                    {
                        /*string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_OGGETTI_CUSTOM");
                        string id = string.Empty;
                        System.Diagnostics.Debug.WriteLine("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + sql);
                        logger.Debug("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + sql);
                        dbProvider.ExecuteScalar(out id, sql);*/

                        if (!string.IsNullOrEmpty(system_id_oggettoCustom))
                        {
                            dbProvider.SetLargeText("DPA_OGGETTI_CUSTOM", system_id_oggettoCustom, "CONFIG_OBJ_EST", oggettoCustom.CONFIG_OBJ_EST);
                        }
                    }

                    /*
                    //Poichè sto inserendo un nuovo oggetto della tipologia devo dare la visibilità a tutti i ruoli dell'amministrazione
                    ArrayList listaRuoli = this.getRuoliByAmm(idAmministrazione, "", "");
                    DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                    assDocFascRuoli.ID_TIPO_DOC_FASC = system_id_templates;
                    assDocFascRuoli.ID_OGGETTO_CUSTOM = system_id_oggettoCustom;
                    assDocFascRuoli.VIS_OGG_CUSTOM = "1";
                    assDocFascRuoli.INS_MOD_OGG_CUSTOM = "1";
                    ArrayList listaCampi = new ArrayList();
                    listaCampi.Add(assDocFascRuoli);
                    this.estendiDirittiCampiARuoliDoc(listaCampi, listaRuoli);
                    */

                    //Inserimento DPA_OGG_CUSTOM_COMP
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INSERT_DPA_OGG_CUSTOM_COMP");
                    queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_OGG_CUSTOM_COMP"));
                    queryMng.setParam("idTemplate", system_id_templates);
                    queryMng.setParam("idOggettoCustom", system_id_oggettoCustom);
                    queryMng.setParam("posizione", oggettoCustom.POSIZIONE);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);

                    //Inserimento DPA_ASSOCIAZIONE_TEMPLATES
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASSOCIAZIONE_TEMPLATES");
                    queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASSOCIAZIONE_TEMPLATES"));
                    queryMng.setParam("ID_OGGETTO", system_id_oggettoCustom);
                    queryMng.setParam("ID_TEMPLATE", system_id_templates);
                    queryMng.setParam("Doc_Number", "");
                    if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
                    {
                        queryMng.setParam("Valore_Oggetto_Db", oggettoCustom.VALORE_DATABASE);
                    }
                    else
                        queryMng.setParam("Valore_Oggetto_Db", "");
                    queryMng.setParam("CODICE_DB", "");
                    queryMng.setParam("MANUAL_INSERT", "0");
                    queryMng.setParam("Anno", System.DateTime.Now.Year.ToString());
                    if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                        queryMng.setParam("idAooRf", oggettoCustom.ID_AOO_RF);
                    else
                        queryMng.setParam("idAooRf", "0");

                    //queryMng.setParam("valoreSc", "0");
                    //queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.GetDate());
                    queryMng.setParam("valoreSc", "NULL");
                    queryMng.setParam("dtaIns", "NULL");
                    queryMng.setParam("anno_acc", "");

                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);

                    // Se l'oggetto inserito è un contatore di repertorio, viene inserito nell'anagrafica
                    if (oggettoCustom.REPERTORIO == "1")
                        this.AddCounterToManagementTable(system_id_templates, system_id_oggettoCustom, oggettoCustom.TIPO_CONTATORE);

                    //Inserimento DPA_ASSOCIAZIONE_VALORI
                    for (int j = 0; j < oggettoCustom.ELENCO_VALORI.Count; j++)
                    {
                        DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = (DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[j];
                        valoreOggetto.gestisciCaratteriSpeciali();
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASSOCIAZIONE_VALORI");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASSOCIAZIONE_VALORI"));
                        queryMng.setParam("Descrizione_Valore", valoreOggetto.DESCRIZIONE_VALORE);
                        queryMng.setParam("Valore", valoreOggetto.VALORE);
                        queryMng.setParam("Valore_Di_Default", valoreOggetto.VALORE_DI_DEFAULT);
                        queryMng.setParam("ID_OGGETTO_CUSTOM", system_id_oggettoCustom);
                        queryMng.setParam("abilitato", valoreOggetto.ABILITATO.ToString());
                        queryMng.setParam("colorBg", valoreOggetto.COLOR_BG != null ? valoreOggetto.COLOR_BG.ToString() : "null");
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                    }
                }
            }
            catch
            {
                dbProvider.RollbackTransaction();
                return false;
            }
            finally
            {
                dbProvider.Dispose();
            }
            return true;
        }

        public bool eliminaOggettoCustomDaDB(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            //Il seguente metodo elimina l'oggettoCustom dal Database, non è stato possibile realizzare
            //l'eliminazione nel metodo "aggiornaTemplate" impostando il tipo di operazione, perchè si
            //presentavano particolari complicazione nella gestione dei dataGrid lato frontEnd-toolAmministrazione
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_OGGETTI_CUSTOM_DA_DB");
                queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - eliminaOggettoCustomDaDB - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - eliminaOggettoCustomDaDB - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);

                if (template.IN_ESERCIZIO == "NO")
                {
                    //Cancellazione valori
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_ASSOCIAZIONE_VALORI");
                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - eliminaOggettoCustomDaDB - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - eliminaOggettoCustomDaDB - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);

                    //Cancellazione posizione
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_OGG_CUSTOM_COMP");
                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - eliminaOggettoCustomDaDB - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - eliminaOggettoCustomDaDB - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);

                    //Cancellazione oggetto
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_OGGETTI_CUSTOM");
                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - eliminaOggettoCustomDaDB - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - eliminaOggettoCustomDaDB - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);

                    //Cancellazione visibilità oggetto
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DEL_A_R_CAMPI_DOC");
                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - eliminaOggettoCustomDaDB - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - eliminaOggettoCustomDaDB - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                }

                //Controllo che l'oggettoCustom non sia un contatore, in caso affermativo, 
                //disabilito non cancello il contatore dalla tabella DPA_CONTATORI_DOC
                if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "Contatore")
                {
                    //controllo se il contatore è di custom,ossia se esiste un record all'interno della
                    //DPA_CONT_CUSTOM relativo al system_id dell'oggetto custom
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CONT_CUSTOM_DOC_BY_IDOGG");
                    queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - cerca contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - cerca contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    DataSet ds_datiContatoreCustom = new DataSet(); ;
                    dbProvider.ExecuteQuery(ds_datiContatoreCustom, commandText);
                    if (ds_datiContatoreCustom.Tables[0].Rows.Count != 0)
                    {

                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_CONT_CUSTOM_DOC");
                        queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - eliminaOggettoCustomDaDB - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - eliminaOggettoCustomDaDB - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                    }
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DISABLE_CONT_DOC");
                    queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - eliminaOggettoCustomDaDB - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - eliminaOggettoCustomDaDB - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);



                }
            }
            catch
            {
                dbProvider.RollbackTransaction();
                return false;
            }
            finally
            {
                dbProvider.Dispose();
            }
            return true;
        }

        /// <summary>
        /// Il seguente metodo opera sulla proprieta' TIPO_OPERAZIONE dell'oggettoCustom, a seconda
        /// del tipo di operazione associata all'oggetto viene effettuata l'aggiornamento
        /// o l'inserimento di quest'ultimo.
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public bool aggiornaTemplate(DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng;
                string commandText;
                bool retValue = false;
                int rowsAffected;

                for (int i = 0; i < template.ELENCO_OGGETTI.Count; i++)
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[i];
                    oggettoCustom.gestisciCaratteriSpeciali();

                    #region DaAggiungere
                    if (oggettoCustom.TIPO_OPERAZIONE.Equals("DaAggiungere"))
                    {
                        //Cerco l'ID_TIPO_OGGETTO per l'oggettoCustom in questione
                        int system_id_tipo_oggetto = 0;
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TIPO_OGGETTO");
                        queryMng.setParam("Descrizione", oggettoCustom.TIPO.DESCRIZIONE_TIPO);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        DataSet ds = new DataSet();
                        dbProvider.ExecuteQuery(ds, commandText);
                        system_id_tipo_oggetto = Convert.ToInt32(ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString());

                        //Inserimento oggettoCustom
                        string system_id_oggettoCustom = string.Empty;
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_OGGETTO_CUSTOM");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_OGGETTI_CUSTOM"));
                        queryMng.setParam("Descrizione", oggettoCustom.DESCRIZIONE);
                        queryMng.setParam("Orizzontale_Verticale", oggettoCustom.ORIZZONTALE_VERTICALE);
                        queryMng.setParam("Campo_Obbligatorio", oggettoCustom.CAMPO_OBBLIGATORIO);
                        queryMng.setParam("Multilinea", oggettoCustom.MULTILINEA);
                        queryMng.setParam("Numero_Di_Linee", oggettoCustom.NUMERO_DI_LINEE);
                        queryMng.setParam("Numero_Di_Caratteri", oggettoCustom.NUMERO_DI_CARATTERI);
                        queryMng.setParam("Campo_Di_Ricerca", oggettoCustom.CAMPO_DI_RICERCA);
                        queryMng.setParam("ID_TIPO_OGGETTO", system_id_tipo_oggetto.ToString());
                        queryMng.setParam("Reset_Anno", oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO);
                        queryMng.setParam("Formato_Contatore", oggettoCustom.FORMATO_CONTATORE);
                        queryMng.setParam("ID_R_DEFAULT", oggettoCustom.ID_RUOLO_DEFAULT);
                        queryMng.setParam("RICERCA_CORR", oggettoCustom.TIPO_RICERCA_CORR);
                        if (!string.IsNullOrEmpty(oggettoCustom.TIPO_LINK))
                        {
                            queryMng.setParam("TIPO_LINK", oggettoCustom.TIPO_LINK);
                        }
                        else
                        {
                            queryMng.setParam("TIPO_LINK", "");
                        }
                        if (!string.IsNullOrEmpty(oggettoCustom.TIPO_OBJ_LINK))
                        {
                            queryMng.setParam("TIPO_OBJ_LINK", oggettoCustom.TIPO_OBJ_LINK);
                        }
                        else
                        {
                            queryMng.setParam("TIPO_OBJ_LINK", "");
                        }
                        if (oggettoCustom.CAMPO_COMUNE != null && oggettoCustom.CAMPO_COMUNE == "1")
                            queryMng.setParam("CAMPO_COMUNE", oggettoCustom.CAMPO_COMUNE);
                        else
                            queryMng.setParam("CAMPO_COMUNE", "null");
                        queryMng.setParam("tipoContatore", oggettoCustom.TIPO_CONTATORE);
                        if (oggettoCustom.CONTA_DOPO != null && oggettoCustom.CONTA_DOPO == "1")
                            queryMng.setParam("contaDopo", "1");
                        else
                            queryMng.setParam("contaDopo", "0");
                        if (oggettoCustom.REPERTORIO != null && oggettoCustom.REPERTORIO == "1")
                        {
                            queryMng.setParam("repertorio", "1");
                            if (oggettoCustom.CONS_REPERTORIO != null && oggettoCustom.CONS_REPERTORIO == "1")
                                queryMng.setParam("cons_repertorio", "1");
                            else
                                queryMng.setParam("cons_repertorio", "0");
                        }
                        else
                        {
                            queryMng.setParam("repertorio", "0");
                            queryMng.setParam("cons_repertorio", "0");
                        }
                        // INTEGRAZIONE PITRE-PARER
                        if (oggettoCustom.CONSOLIDAMENTO != null && oggettoCustom.CONSOLIDAMENTO == "1")
                        {
                            queryMng.setParam("consolidamento", "1");
                            if (oggettoCustom.CONSERVAZIONE != null && oggettoCustom.CONSERVAZIONE == "1")
                                queryMng.setParam("conservazione", "1");
                            else
                                queryMng.setParam("conservazione", "0");
                        }
                        else
                        {
                            queryMng.setParam("consolidamento", "0");
                            queryMng.setParam("conservazione", "0");
                        }

                        if (oggettoCustom.DA_VISUALIZZARE_RICERCA != null && oggettoCustom.DA_VISUALIZZARE_RICERCA == "1")
                            queryMng.setParam("da_visualizzare_ricerca", "1");
                        else
                            queryMng.setParam("da_visualizzare_ricerca", "0");

                        queryMng.setParam("formatoOra", oggettoCustom.FORMATO_ORA);

                        if (string.IsNullOrEmpty(oggettoCustom.MODULO_SOTTOCONTATORE))
                            queryMng.setParam("moduloSottocontatore", "0");
                        else
                            queryMng.setParam("moduloSottocontatore", oggettoCustom.MODULO_SOTTOCONTATORE);

                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        if (dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                        {
                            retValue = (rowsAffected > 0);
                            if (retValue)
                            {
                                // Reperimento systemid
                                commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_OGGETTI_CUSTOM");
                                dbProvider.ExecuteScalar(out system_id_oggettoCustom, commandText);
                                if (!string.IsNullOrEmpty(oggettoCustom.DATA_INIZIO) && !string.IsNullOrEmpty(oggettoCustom.DATA_FINE))
                                {

                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_DATE_CONT_CUSTOM_DOC");
                                    queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                    queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONT_CUSTOM_DOC"));
                                    queryMng.setParam("dataFine", DocsPaDbManagement.Functions.Functions.ToDate(oggettoCustom.DATA_FINE));
                                    queryMng.setParam("idOgg", system_id_oggettoCustom);
                                    queryMng.setParam("dataInizio", DocsPaDbManagement.Functions.Functions.ToDate(oggettoCustom.DATA_INIZIO));

                                    commandText = queryMng.getSQL();
                                    System.Diagnostics.Debug.WriteLine("SQL - salva contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                    logger.Debug("SQL - salva contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                    bool result = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                                }

                            }
                            else
                            {
                                dbProvider.RollbackTransaction();
                                return false;
                            }
                        }

                        if (!string.IsNullOrEmpty(oggettoCustom.CONFIG_OBJ_EST))
                        {
                            /*string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_OGGETTI_CUSTOM");
                            string id = string.Empty;
                            System.Diagnostics.Debug.WriteLine("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + sql);
                            logger.Debug("SQL - saveTSR - DocsPaDB/TimestampDoc.cs - QUERY : " + sql);
                            dbProvider.ExecuteScalar(out id, sql);*/

                            if (!string.IsNullOrEmpty(system_id_oggettoCustom))
                            {
                                dbProvider.SetLargeText("DPA_OGGETTI_CUSTOM", system_id_oggettoCustom, "CONFIG_OBJ_EST", oggettoCustom.CONFIG_OBJ_EST);
                            }
                        }

                        /*
                        //Poichè sto inserendo un nuovo oggetto della tipologia devo dare la visibilità a tutti i ruoli dell'amministrazione
                        ArrayList listaRuoli = this.getRuoliByAmm(template.ID_AMMINISTRAZIONE, "", "");
                        DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                        assDocFascRuoli.ID_TIPO_DOC_FASC = template.SYSTEM_ID.ToString();
                        assDocFascRuoli.ID_OGGETTO_CUSTOM = system_id_oggettoCustom;
                        assDocFascRuoli.VIS_OGG_CUSTOM = "1";
                        assDocFascRuoli.INS_MOD_OGG_CUSTOM = "1";
                        ArrayList listaCampi = new ArrayList();
                        listaCampi.Add(assDocFascRuoli);
                        this.estendiDirittiCampiARuoliDoc(listaCampi, listaRuoli);
                        */

                        // INTEGRAZIONE PITRE-PARER
                        // se ho impostato conservazione repertorio=1 devo attivare il campo invio in conservazione
                        // nel template
                        if (oggettoCustom.CONS_REPERTORIO.Equals("1"))
                            this.UpdateInvioConsTipoDoc(template.SYSTEM_ID, "1");

                        //Inserimento DPA_OGG_CUSTOM_COMP
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INSERT_DPA_OGG_CUSTOM_COMP");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_OGG_CUSTOM_COMP"));
                        queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                        queryMng.setParam("idOggettoCustom", system_id_oggettoCustom);
                        queryMng.setParam("posizione", oggettoCustom.POSIZIONE);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                        //Inserimento DPA_ASSOCIAZIONE_TEMPLATES
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASSOCIAZIONE_TEMPLATES");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASSOCIAZIONE_TEMPLATES"));
                        queryMng.setParam("ID_OGGETTO", system_id_oggettoCustom);
                        queryMng.setParam("ID_TEMPLATE", template.SYSTEM_ID.ToString());
                        queryMng.setParam("Doc_Number", "");
                        if(!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
                            queryMng.setParam("Valore_Oggetto_Db", oggettoCustom.VALORE_DATABASE);
                        else
                            queryMng.setParam("Valore_Oggetto_Db", "");
                        queryMng.setParam("CODICE_DB", "");
                        queryMng.setParam("MANUAL_INSERT", "0");
                        queryMng.setParam("Anno", oggettoCustom.ANNO);
                        if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                            queryMng.setParam("idAooRf", oggettoCustom.ID_AOO_RF);
                        else
                            queryMng.setParam("idAooRf", "0");

                        //queryMng.setParam("valoreSc", "0");
                        //queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.GetDate());
                        queryMng.setParam("valoreSc", "NULL");
                        queryMng.setParam("dtaIns", "NULL");
                        queryMng.setParam("anno_acc", "");

                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                        if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROPAGAZIONE_CAMPI_A_PREGRESSI")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROPAGAZIONE_CAMPI_A_PREGRESSI") == "1")
                        {
                            // Propagazione aggiunta campi ai pregressi
                            logger.Debug("Store Procedure: SP_EST_CAMPI_DOC_PREGRESSI");
                            System.Collections.ArrayList parameters = new System.Collections.ArrayList();
                            parameters.Add(new DocsPaUtils.Data.ParameterSP("id_oggetto_in", system_id_oggettoCustom));
                            parameters.Add(new DocsPaUtils.Data.ParameterSP("id_template_in", template.SYSTEM_ID.ToString()));
                            parameters.Add(new DocsPaUtils.Data.ParameterSP("anno_in", oggettoCustom.ANNO));
                            if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("id_aoo_rf_in", oggettoCustom.ID_AOO_RF));
                            else
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("id_aoo_rf_in", "0"));

                            if (dbProvider.DBType == "SQL")
                                parameters.Add(new DocsPaUtils.Data.ParameterSP("returnvalue", "", DirectionParameter.ParamOutput));

                            int resultSP = dbProvider.ExecuteStoreProcedure("SP_EST_CAMPI_DOC_PREGRESSI", parameters);
                            if (resultSP == 0)
                            {
                                logger.Debug("STORE PROCEDURE SP_EST_CAMPI_DOC_PREGRESSI: esito positivo");
                            }
                            else
                            {
                                logger.Error("ERRORE: STORE PROCEDURE SP_EST_CAMPI_DOC_PREGRESSI: esito negativo");
                            }
                        }

                        if (!string.IsNullOrEmpty(oggettoCustom.CONFIG_OBJ_EST))
                        {
                            dbProvider.SetLargeText("DPA_OGGETTI_CUSTOM", oggettoCustom.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST", oggettoCustom.CONFIG_OBJ_EST);
                        }

                        //Inserimento DPA_ASSOCIAZIONE_VALORI
                        for (int j = 0; j < oggettoCustom.ELENCO_VALORI.Count; j++)
                        {
                            DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = (DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[j];
                            valoreOggetto.gestisciCaratteriSpeciali();
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASSOCIAZIONE_VALORI");
                            queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                            queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASSOCIAZIONE_VALORI"));
                            queryMng.setParam("Descrizione_Valore", valoreOggetto.DESCRIZIONE_VALORE);
                            queryMng.setParam("Valore", valoreOggetto.VALORE);
                            queryMng.setParam("Valore_Di_Default", valoreOggetto.VALORE_DI_DEFAULT);
                            queryMng.setParam("ID_OGGETTO_CUSTOM", system_id_oggettoCustom);
                            queryMng.setParam("abilitato", valoreOggetto.ABILITATO.ToString());
                            queryMng.setParam("colorBg", valoreOggetto.COLOR_BG != null ? valoreOggetto.COLOR_BG.ToString() : "null");
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);
                        }

                        // Se l'oggetto inserito è un contatore di repertorio, viene inserito nell'anagrafica
                        if (oggettoCustom.REPERTORIO == "1")
                            this.AddCounterToManagementTable(template.SYSTEM_ID.ToString(), system_id_oggettoCustom, oggettoCustom.TIPO_CONTATORE);

                    }
                    #endregion

                    #region DaAggiornare
                    if (oggettoCustom.TIPO_OPERAZIONE.Equals("DaAggiornare"))
                    {
                        //controllo se il contatore è di tipo custom, in caso positivo aggiorno la tabella
                        // DPA_CONT_CUSTOM_DOC passandogli il system_id dell'oggetto custom corrente in sessione
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CONT_CUSTOM_DOC_BY_IDOGG");
                        queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - cerca contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - cerca contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        DataSet ds_datiContatoreCustom = new DataSet(); ;
                        dbProvider.ExecuteQuery(ds_datiContatoreCustom, commandText);
                        if (ds_datiContatoreCustom.Tables[0].Rows.Count != 0)
                        {

                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONT_CUSTOM_DOC");
                            queryMng.setParam("dataFine", DocsPaDbManagement.Functions.Functions.ToDate(oggettoCustom.DATA_FINE));
                            queryMng.setParam("dataInizio", DocsPaDbManagement.Functions.Functions.ToDate(oggettoCustom.DATA_INIZIO));
                            queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());

                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - salva contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - salva contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            bool result = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                        }

                        //Aggiornamento della DPA_OGGETTI_CUSTOM
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_AGGIORNA_DPA_OGGETTI_CUSTOM");
                        queryMng.setParam("descrizioneOggettoCustom", oggettoCustom.DESCRIZIONE);
                        queryMng.setParam("orizzontaleVerticaleOggettoCustom", oggettoCustom.ORIZZONTALE_VERTICALE);
                        queryMng.setParam("campoObbligatorioOggettoCustom", oggettoCustom.CAMPO_OBBLIGATORIO);
                        queryMng.setParam("multilineaOggettoCustom", oggettoCustom.MULTILINEA);
                        queryMng.setParam("numeroLineeOggettoCustom", oggettoCustom.NUMERO_DI_LINEE);
                        queryMng.setParam("numeroCaratteriOggettoCustom", oggettoCustom.NUMERO_DI_CARATTERI);
                        queryMng.setParam("campoRicercaOggettoCustom", oggettoCustom.CAMPO_DI_RICERCA);
                        queryMng.setParam("posizioneOggettoCustom", oggettoCustom.POSIZIONE);
                        queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                        queryMng.setParam("Reset_Anno", oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO);
                        queryMng.setParam("Formato_Contatore", oggettoCustom.FORMATO_CONTATORE);
                        queryMng.setParam("ID_R_DEFAULT", oggettoCustom.ID_RUOLO_DEFAULT);
                        queryMng.setParam("RICERCA_CORR", oggettoCustom.TIPO_RICERCA_CORR);
                        queryMng.setParam("CAMPO_COMUNE", oggettoCustom.CAMPO_COMUNE);
                        queryMng.setParam("tipoContatore", oggettoCustom.TIPO_CONTATORE);
                        queryMng.setParam("contaDopo", oggettoCustom.CONTA_DOPO);
                        queryMng.setParam("repertorio", oggettoCustom.REPERTORIO);
                        queryMng.setParam("cons_repertorio", oggettoCustom.CONS_REPERTORIO);
                        queryMng.setParam("consolidamento", oggettoCustom.CONSOLIDAMENTO);
                        queryMng.setParam("conservazione", oggettoCustom.CONSERVAZIONE);
                        queryMng.setParam("formatoOra", oggettoCustom.FORMATO_ORA);

                        if (!string.IsNullOrEmpty(oggettoCustom.TIPO_LINK))
                            queryMng.setParam("TIPO_LINK", oggettoCustom.TIPO_LINK);
                        else
                            queryMng.setParam("TIPO_LINK", "");

                        if (!string.IsNullOrEmpty(oggettoCustom.TIPO_OBJ_LINK))
                            queryMng.setParam("TIPO_OBJ_LINK", oggettoCustom.TIPO_OBJ_LINK);
                        else
                            queryMng.setParam("TIPO_OBJ_LINK", "");

                        if (string.IsNullOrEmpty(oggettoCustom.MODULO_SOTTOCONTATORE))
                            queryMng.setParam("moduloSottocontatore", "0");
                        else
                            queryMng.setParam("moduloSottocontatore", oggettoCustom.MODULO_SOTTOCONTATORE);

                        queryMng.setParam("da_visualizzare_ricerca", oggettoCustom.DA_VISUALIZZARE_RICERCA);

                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                        // INTEGRAZIONE PITRE-PARER
                        // se ho impostato conservazione repertorio=1 devo attivare il campo invio in conservazione
                        // nel template
                        if (oggettoCustom.CONS_REPERTORIO.Equals("1"))
                            this.UpdateInvioConsTipoDoc(template.SYSTEM_ID, "1");

                        if (!string.IsNullOrEmpty(oggettoCustom.CONFIG_OBJ_EST))
                        {
                            dbProvider.SetLargeText("DPA_OGGETTI_CUSTOM", oggettoCustom.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST", oggettoCustom.CONFIG_OBJ_EST);
                        }
                        // settaggio valore di default
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_U_ASSOCIAZIONE_TEMPLATES_DEFAULT");
                        queryMng.setParam("idtemplate", template.SYSTEM_ID.ToString());
                        queryMng.setParam("idoggetto", oggettoCustom.SYSTEM_ID.ToString());
                        if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
                            queryMng.setParam("valore_default", oggettoCustom.VALORE_DATABASE);
                        else
                            queryMng.setParam("valore_default", "");
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                        //Cerco il valoreOggetto per valore e id_oggettCustom
                        //Se esiste non faccio niente altrimenti effettuo un inserimento
                        for (int j = 0; j < oggettoCustom.ELENCO_VALORI.Count; j++)
                        {
                            DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = (DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[j];
                            valoreOggetto.gestisciCaratteriSpeciali();

                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI_1");
                            queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                            queryMng.setParam("valore", valoreOggetto.VALORE);
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                            DataSet ds = new DataSet();
                            dbProvider.ExecuteQuery(ds, commandText);
                            if (ds.Tables[0].Rows.Count == 0)
                            {
                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASSOCIAZIONE_VALORI");
                                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASSOCIAZIONE_VALORI"));
                                queryMng.setParam("Descrizione_Valore", valoreOggetto.DESCRIZIONE_VALORE);
                                queryMng.setParam("Valore", valoreOggetto.VALORE);
                                queryMng.setParam("Valore_Di_Default", valoreOggetto.VALORE_DI_DEFAULT);
                                queryMng.setParam("ID_OGGETTO_CUSTOM", oggettoCustom.SYSTEM_ID.ToString());
                                queryMng.setParam("abilitato", valoreOggetto.ABILITATO.ToString());
                                queryMng.setParam("colorBg", valoreOggetto.COLOR_BG != null ? valoreOggetto.COLOR_BG.ToString() : "null");
                                commandText = queryMng.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                logger.Debug("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                dbProvider.ExecuteNonQuery(commandText);
                            }
                        }

                        //Verifico eventuali cancellazioni
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI");
                        queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        DataSet ds_1 = new DataSet();
                        dbProvider.ExecuteQuery(ds_1, commandText);
                        if (ds_1.Tables[0].Rows.Count != 0)
                        {
                            for (int k = 0; k < ds_1.Tables[0].Rows.Count; k++)
                            {
                                bool result = false;
                                for (int j = 0; j < oggettoCustom.ELENCO_VALORI.Count; j++)
                                {
                                    DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = (DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[j];
                                    if (ds_1.Tables[0].Rows[k]["Valore"].ToString() == valoreOggetto.VALORE)
                                        result = true;
                                }
                                if (!result)
                                {
                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_ASSOCIAZIONE_VALORI_1");
                                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                                    queryMng.setParam("valore", ds_1.Tables[0].Rows[k]["Valore"].ToString());
                                    commandText = queryMng.getSQL();
                                    System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                    logger.Debug("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                    dbProvider.ExecuteNonQuery(commandText);
                                }
                            }
                        }

                        //Ordino le Descrizioni valore
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI");
                        queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        DataSet ds_2 = new DataSet();
                        dbProvider.ExecuteQuery(ds_2, commandText);
                        if (ds_2.Tables[0].Rows.Count != 0)
                        {
                            for (int y = 0; y < ds_2.Tables[0].Rows.Count; y++)
                            {
                                foreach (DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto in oggettoCustom.ELENCO_VALORI)
                                {
                                    if (valoreOggetto.VALORE == ds_2.Tables[0].Rows[y]["VALORE"].ToString())
                                    {
                                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_DPA_ASSOCIAZIONE_VALORI");
                                        queryMng.setParam("idOggettoCustom", ds_2.Tables[0].Rows[y]["ID_OGGETTO_CUSTOM"].ToString());
                                        queryMng.setParam("valore", ds_2.Tables[0].Rows[y]["Valore"].ToString());
                                        queryMng.setParam("valoreDiDefault", valoreOggetto.VALORE_DI_DEFAULT);
                                        queryMng.setParam("descrizione", "Valore" + (y + 1));
                                        queryMng.setParam("abilitato", valoreOggetto.ABILITATO.ToString());
                                        queryMng.setParam("colorBg", valoreOggetto.COLOR_BG != null ? valoreOggetto.COLOR_BG.ToString() : "null");
                                        commandText = queryMng.getSQL();
                                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                        logger.Debug("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                        dbProvider.ExecuteNonQuery(commandText);
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            catch
            {
                dbProvider.RollbackTransaction();
                return false;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return true;
        }

        /// <summary>
        /// Metodo per l'inserimento dei dati relativi ad un repertorio nell'anagrafica.
        /// </summary>
        /// <param name="tipologyId">Id della tipologia</param>
        /// <param name="counterId">Id del contatore</param>
        /// <param name="counterType">Tipo del contatore</param>
        public void AddCounterToManagementTable(String tipologyId, String counterId, String counterType)
        {
            ArrayList parameters = new ArrayList();
            parameters.Add(new ParameterSP("tipologyId", tipologyId));
            parameters.Add(new ParameterSP("counterId", counterId));
            parameters.Add(new ParameterSP("counterType", counterType));

            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoreProcedure("InsertRepertorioFromCode", parameters);
            }
        }

        public DocsPaVO.ProfilazioneDinamica.Templates getTemplateById(string idTemplate)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();

            try
            {
                //Recupero i dati del template
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TEMPLATE_DOC");
                queryMng.setParam("idTemplate", idTemplate);
                //queryMng.setParam("docNumber", "''");
                queryMng.setParam("docNumber", " AND (DPA_ASSOCIAZIONE_TEMPLATES.DOC_NUMBER  = '' OR DPA_ASSOCIAZIONE_TEMPLATES.DOC_NUMBER IS NULL) ");
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplateById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplateById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_template = new DataSet();
                dbProvider.ExecuteQuery(ds_template, commandText);

                //Se il template non ha oggetti custom vengono restituite solo le proprietà del template
                if (ds_template.Tables[0].Rows.Count == 0)
                {
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE_2");
                    queryMng.setParam("idTemplate", idTemplate);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    ds_template = new DataSet();
                    dbProvider.ExecuteQuery(ds_template, commandText);

                    setTemplate(ref template, ds_template, 0);
                    return template;
                }

                setTemplate(ref template, ds_template, 0);

                //Cerco gli oggetti custom associati al template
                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;
                for (int j = 0; j < ds_template.Tables[0].Rows.Count; j++)
                {
                    //Imposto i valori degli oggetti custom dei tipi oggetto
                    oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                    setOggettoCustom(ref oggettoCustom, ds_template, j);

                    DocsPaVO.ProfilazioneDinamica.TipoOggetto tipoOggetto = new DocsPaVO.ProfilazioneDinamica.TipoOggetto();
                    setTipoOggetto(ref tipoOggetto, ds_template, j);

                    //Aggiungo il tipo oggetto all'oggettoCustom
                    oggettoCustom.TIPO = tipoOggetto;

                    //campo CLOB di configurazione
                    if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                    {
                        string config = dbProvider.GetLargeText("DPA_OGGETTI_CUSTOM", oggettoCustom.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST");
                        oggettoCustom.CONFIG_OBJ_EST = config;
                    }

                    //Selezioni i valori per l'oggettoCustom
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI");
                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplates - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                    DataSet ds_valoriOggetto = new DataSet();
                    dbProvider.ExecuteQuery(ds_valoriOggetto, commandText);

                    for (int k = 0; k < ds_valoriOggetto.Tables[0].Rows.Count; k++)
                    {
                        DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto();
                        setValoreOggetto(ref valoreOggetto, ds_valoriOggetto, k);
                        oggettoCustom.ELENCO_VALORI.Add(valoreOggetto);
                        oggettoCustom.VALORI_SELEZIONATI.Add("");
                    }
                    template.ELENCO_OGGETTI.Add(oggettoCustom);
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return template;
        }

        public ArrayList getTemplates(string idAmministrazione)
        {
            ArrayList templates = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //Selezione dalla DPA_TEMPLATES in relazione con la DPA_TEMPLATES_COMPONENT
                //per la selezione dei template associati ad una specifica amministrazione
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE");
                queryMng.setParam("idAmministrazione", idAmministrazione);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplates - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplates - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                DataSet ds_templates = new DataSet();
                dbProvider.ExecuteQuery(ds_templates, commandText);

                for (int i = 0; i < ds_templates.Tables[0].Rows.Count; i++)
                {
                    DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();
                    setTemplate(ref template, ds_templates, i);
                    templates.Add(template);
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }

            //Prima di restituire l'array devo preoccuparmi di mettere in testa tutte le tipologie
            //che rappresentano un iperfascicolo cio' sono un insieme di campi comuni
            for (int i = 0; i < templates.Count; i++)
            {
                if (((DocsPaVO.ProfilazioneDinamica.Templates)templates[i]).IPER_FASC_DOC == "1")
                {
                    DocsPaVO.ProfilazioneDinamica.Templates tempApp = (DocsPaVO.ProfilazioneDinamica.Templates)templates[i];
                    templates.RemoveAt(i);
                    templates.Insert(0, tempApp);
                }
            }

            return templates;
        }

        public void salvaInserimentoUtenteProfDim(DocsPaVO.ProfilazioneDinamica.Templates template, string docNumber)
        {
            logger.Info("BEGIN");
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //Verifico l'esistenza del docNumber per decidere se effettuare un inserimento o un aggiornamento
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DOCNUMBER");
                queryMng.setParam("docNumber", docNumber);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_docNumber = new DataSet();
                dbProvider.ExecuteQuery(ds_docNumber, commandText);

                string anno = System.DateTime.Now.Year.ToString();

                if (ds_docNumber.Tables[0].Rows.Count == 0)
                {
                    //INSERIMENTO
                    for (int i = 0; i < template.ELENCO_OGGETTI.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[i];
                        oggettoCustom.ANNO = anno;

                        switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                        {
                            case "Contatore":
                            case "ContatoreSottocontatore":
                                //Il parametro booleano è :
                                //true = se l'oggetto è da inserire - come in questo caso
                                //false= se l'oggetto è da aggiornare
                                if (oggettoCustom.CAMPO_COMUNE != "1")
                                    calcolaContatore(template, ref oggettoCustom, true);
                                else
                                    calcolaContatoreComune(template, ref oggettoCustom, true);
                              
                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASSOCIAZIONE_TEMPLATES");
                                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASSOCIAZIONE_TEMPLATES"));
                                queryMng.setParam("ID_OGGETTO", oggettoCustom.SYSTEM_ID.ToString());
                                queryMng.setParam("ID_TEMPLATE", template.SYSTEM_ID.ToString());
                                queryMng.setParam("Doc_Number", docNumber);
                                queryMng.setParam("Valore_Oggetto_Db", oggettoCustom.VALORE_DATABASE);
                                queryMng.setParam("CODICE_DB", oggettoCustom.CODICE_DB);
                                string manual = (oggettoCustom.MANUAL_INSERT) ? "1" : "0";
                                queryMng.setParam("MANUAL_INSERT", manual);
                                queryMng.setParam("Anno", oggettoCustom.ANNO);

                                if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                                {
                                    queryMng.setParam("idAooRf", oggettoCustom.ID_AOO_RF);
                                }
                                else
                                {
                                    queryMng.setParam("idAooRf", "0");
                                }

                                if (!string.IsNullOrEmpty(oggettoCustom.VALORE_SOTTOCONTATORE))
                                {
                                    queryMng.setParam("valoreSc", oggettoCustom.VALORE_SOTTOCONTATORE);
                                }
                                else
                                {
                                    queryMng.setParam("valoreSc", "0");
                                }
                                //Perché è stata messa questo inserimento che dipende dal culture info del pc?
                                //queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.ToDate(oggettoCustom.DATA_INSERIMENTO));

                                queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.GetDate());
                                    
                                queryMng.setParam("anno_acc", oggettoCustom.ANNO_ACC);
                                

                                commandText = queryMng.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                logger.Debug("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                if (!dbProvider.ExecuteNonQuery(commandText))
                                {
                                    logger.Error("Errore nell'inserimento nella DPA_ASSOCIAZIONE_TEMPLATES - QUERY: " + commandText);
                                    throw new Exception("Errore nell'inserimento nella DPA_ASSOCIAZIONE_TEMPLATES");
                                }
                                break;

                            case "CasellaDiSelezione":
                                oggettoCustom.gestisciCaratteriSpeciali();
                                for (int j = 0; j < oggettoCustom.ELENCO_VALORI.Count; j++)
                                {
                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASSOCIAZIONE_TEMPLATES");
                                    queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                    queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASSOCIAZIONE_TEMPLATES"));
                                    queryMng.setParam("ID_OGGETTO", oggettoCustom.SYSTEM_ID.ToString());
                                    queryMng.setParam("ID_TEMPLATE", template.SYSTEM_ID.ToString());
                                    queryMng.setParam("Doc_Number", docNumber);
                                    queryMng.setParam("CODICE_DB", oggettoCustom.CODICE_DB);
                                    string manual1 = (oggettoCustom.MANUAL_INSERT) ? "1" : "0";
                                    queryMng.setParam("MANUAL_INSERT", manual1);
                                    if (oggettoCustom.VALORI_SELEZIONATI.Count == 0)
                                    {
                                        queryMng.setParam("Valore_Oggetto_Db", "");
                                    }
                                    else
                                    {
                                        if (oggettoCustom.VALORI_SELEZIONATI[j] != null && (string)oggettoCustom.VALORI_SELEZIONATI[j] != "")
                                            queryMng.setParam("Valore_Oggetto_Db", oggettoCustom.VALORI_SELEZIONATI[j].ToString().Replace("'", "''"));
                                        else
                                            queryMng.setParam("Valore_Oggetto_Db", "");
                                    }
                                    queryMng.setParam("Anno", oggettoCustom.ANNO);
                                    if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                                        queryMng.setParam("idAooRf", oggettoCustom.ID_AOO_RF);
                                    else
                                        queryMng.setParam("idAooRf", "0");

                                    queryMng.setParam("valoreSc", "0");
                                    queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.GetDate());
                                    queryMng.setParam("anno_acc", "");

                                    commandText = queryMng.getSQL();
                                    System.Diagnostics.Debug.WriteLine("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                    logger.Debug("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                    if (!dbProvider.ExecuteNonQuery(commandText))
                                    {
                                        logger.Error("Errore nell'inserimento nella DPA_ASSOCIAZIONE_TEMPLATES - QUERY: " + commandText);
                                        throw new Exception("Errore nell'inserimento nella DPA_ASSOCIAZIONE_TEMPLATES");
                                    }
                                }
                                oggettoCustom.ritornaCaratteriSpeciali();
                                break;

                            default:
                                oggettoCustom.gestisciCaratteriSpeciali();
                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASSOCIAZIONE_TEMPLATES");
                                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASSOCIAZIONE_TEMPLATES"));
                                queryMng.setParam("ID_OGGETTO", oggettoCustom.SYSTEM_ID.ToString());
                                queryMng.setParam("ID_TEMPLATE", template.SYSTEM_ID.ToString());
                                queryMng.setParam("Doc_Number", docNumber);
                                queryMng.setParam("CODICE_DB", oggettoCustom.CODICE_DB);
                                string manual2 = (oggettoCustom.MANUAL_INSERT) ? "1" : "0";
                                queryMng.setParam("MANUAL_INSERT", manual2);
                                //if(oggettoCustom.VALORE_DATABASE.Length > 254)
                                //    queryMng.setParam("Valore_Oggetto_Db", oggettoCustom.VALORE_DATABASE.Substring(0,254));
                                //else
                                queryMng.setParam("Valore_Oggetto_Db", oggettoCustom.VALORE_DATABASE);
                                queryMng.setParam("Anno", oggettoCustom.ANNO);
                                if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                                    queryMng.setParam("idAooRf", oggettoCustom.ID_AOO_RF);
                                else
                                    queryMng.setParam("idAooRf", "0");

                                queryMng.setParam("valoreSc", "0");
                                queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.GetDate());
                                queryMng.setParam("anno_acc", "");

                                commandText = queryMng.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                logger.Debug("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                if (!dbProvider.ExecuteNonQuery(commandText))
                                {
                                    logger.Error("Errore nell'inserimento nella DPA_ASSOCIAZIONE_TEMPLATES - QUERY: " + commandText);
                                    throw new Exception("Errore nell'inserimento nella DPA_ASSOCIAZIONE_TEMPLATES");
                                }
                                oggettoCustom.ritornaCaratteriSpeciali();
                                break;
                        }
                    }
                    template.OLD_OGG_CUSTOM.Clear();
                }
                else
                {
                    //elenco dei vecchi valori dei campi profilati da salvare nello storico
                    ArrayList oldOggCustom = template.OLD_OGG_CUSTOM;
                    //var utilizzate per lo storico dei campi profilati
                    int indexOldObj = -1, rowsAffected;
                    //AGGIORNAMENTO
                    int indiceSystem_id_DpaAssTemplates = 0;


                    for (int i = 0; i < template.ELENCO_OGGETTI.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[i];

                        bool modifica = true;
                        if (!string.IsNullOrEmpty(oggettoCustom.CAMPO_XML_ASSOC) && oggettoCustom.CAMPO_XML_ASSOC.Equals("DI_SISTEMA"))
                            modifica = false;

                        if (modifica)
                        {
                            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                            {
                                case "CasellaDiSelezione":
                                    int modif_cas_selez = 0; //booleano che mi informa se è cambiato lo stato della casella di selezione 
                                    oggettoCustom.gestisciCaratteriSpeciali();
                                    for (int j = 0; j < oggettoCustom.ELENCO_VALORI.Count; j++)
                                    {
                                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_DPA_ASSOCIAZIONE_TEMPLATES");
                                        if (oggettoCustom.VALORI_SELEZIONATI[j] != null && (string)oggettoCustom.VALORI_SELEZIONATI[j] != "")
                                            queryMng.setParam("valoreDbOggettoCustom", oggettoCustom.VALORI_SELEZIONATI[j].ToString().Replace("'", "''"));
                                        else
                                            queryMng.setParam("valoreDbOggettoCustom", "");
                                        queryMng.setParam("idAooRf", "0");
                                        queryMng.setParam("docNumber", docNumber);
                                        queryMng.setParam("CODICE_DB", oggettoCustom.CODICE_DB);
                                        string manual = (oggettoCustom.MANUAL_INSERT) ? "1" : "0";
                                        queryMng.setParam("MANUAL_INSERT", manual);
                                        queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                                        queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                                        queryMng.setParam("system_id", ds_docNumber.Tables[0].Rows[indiceSystem_id_DpaAssTemplates]["SYSTEM_ID"].ToString());
                                        queryMng.setParam("valoreSc", "0");
                                        queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.GetDate());
                                        queryMng.setParam("anno", oggettoCustom.ANNO);
                                        commandText = queryMng.getSQL();
                                        System.Diagnostics.Debug.WriteLine("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                        logger.Debug("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                                        indiceSystem_id_DpaAssTemplates++;
                                        if (rowsAffected > 0)
                                            modif_cas_selez = 1;
                                    }
                                    oggettoCustom.ritornaCaratteriSpeciali();
                                    insertInStorico(oggettoCustom, oldOggCustom, modif_cas_selez, ref indexOldObj);
                                    break;

                                case "Contatore":
                                case "ContatoreSottocontatore":
                                    //Iacozzilli 17/12/2013
                                    //Modifica per l'update del contatore da SP via PIS.
                                    //OLD CODE:
                                    //if (oggettoCustom.VALORE_DATABASE == null || oggettoCustom.VALORE_DATABASE == "")
                                    //NEW CODE
                                    if (oggettoCustom.VALORE_DATABASE == null || oggettoCustom.VALORE_DATABASE == "" || oggettoCustom.VALORE_DATABASE == "0")
                                    {
                                        //verifico se nel contatore ci sia già impostato un valore numerico: in questo caso non faccio update
                                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_IS_VALUE_IN_USE");
                                        queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString() + " AND system_id = " + ds_docNumber.Tables[0].Rows[indiceSystem_id_DpaAssTemplates]["SYSTEM_ID"].ToString());
                                        queryMng.setParam("idOggetto", oggettoCustom.SYSTEM_ID.ToString() + " AND doc_number = " + docNumber);
                                        string field = string.Empty;
                                        dbProvider.ExecuteScalar(out field, queryMng.getSQL());
                                        bool continua = false;
                                        try
                                        {
                                            //Iacozzilli 17/12/2013
                                            //Modifica per l'update del contatore da SP via PIS.
                                            //OLD CODE:
                                            //Convert.ToInt32(field);
                                            //continua = false;
                                            //NEW CODE:
                                            int code = 0;
                                            continua = !Int32.TryParse(field, out code);
                                        }
                                        catch (Exception excp)
                                        {
                                            logger.Debug("Il contatore non presenta un valore numerico e quindi si può effettuare l'aggiornamento");
                                            continua = true;
                                        }

                                        //if (continua)
                                        //{
                                        //Il parametro booleano è :
                                        //true = se l'oggetto è da inserire 
                                        //false= se l'oggetto è da aggiornare - come in questo caso
                                        if (oggettoCustom.CAMPO_COMUNE != "1")
                                            calcolaContatore(template, ref oggettoCustom, continua);
                                        else
                                            calcolaContatoreComune(template, ref oggettoCustom, continua);

                                        if (!continua)
                                            oggettoCustom.VALORE_DATABASE = field;

                                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_DPA_ASSOCIAZIONE_TEMPLATES");
                                        if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                                            queryMng.setParam("idAooRf", oggettoCustom.ID_AOO_RF);
                                        else
                                            queryMng.setParam("idAooRf", "0");
                                        queryMng.setParam("docNumber", docNumber);
                                        queryMng.setParam("CODICE_DB", oggettoCustom.CODICE_DB);
                                        string manual1 = (oggettoCustom.MANUAL_INSERT) ? "1" : "0";
                                        queryMng.setParam("MANUAL_INSERT", manual1);
                                        queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                                        queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                                        queryMng.setParam("system_id", ds_docNumber.Tables[0].Rows[indiceSystem_id_DpaAssTemplates]["SYSTEM_ID"].ToString());
                                        if (!string.IsNullOrEmpty(oggettoCustom.VALORE_SOTTOCONTATORE))
                                            queryMng.setParam("valoreSc", oggettoCustom.VALORE_SOTTOCONTATORE);
                                        else
                                            queryMng.setParam("valoreSc", "0");

                                        //In alcuni casi impostava la data di inserimento vuota
                                        if (!string.IsNullOrEmpty(DocsPaDbManagement.Functions.Functions.ToDate(oggettoCustom.DATA_INSERIMENTO)))
                                            queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.ToDate(oggettoCustom.DATA_INSERIMENTO));
                                        else
                                            queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.GetDate());

                                        queryMng.setParam("anno", oggettoCustom.ANNO);
                                        queryMng.setParam("valoreDbOggettoCustom", oggettoCustom.VALORE_DATABASE);
                                        commandText = queryMng.getSQL();
                                        System.Diagnostics.Debug.WriteLine("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                        logger.Debug("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                        int rowsAff = 0;
                                        dbProvider.ExecuteNonQuery(commandText, out rowsAff);
                                        //}
                                    }
                                    indiceSystem_id_DpaAssTemplates++;
                                    break;

                                default:
                                    oggettoCustom.gestisciCaratteriSpeciali();
                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_DPA_ASSOCIAZIONE_TEMPLATES");
                                    //if(oggettoCustom.VALORE_DATABASE.Length > 254)
                                    //    queryMng.setParam("valoreDbOggettoCustom", oggettoCustom.VALORE_DATABASE.Substring(0,254));
                                    //else
                                    queryMng.setParam("valoreDbOggettoCustom", oggettoCustom.VALORE_DATABASE);
                                    if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                                        queryMng.setParam("idAooRf", oggettoCustom.ID_AOO_RF);
                                    else
                                        queryMng.setParam("idAooRf", "0");
                                    queryMng.setParam("docNumber", docNumber);
                                    queryMng.setParam("CODICE_DB", oggettoCustom.CODICE_DB);
                                    string manual2 = (oggettoCustom.MANUAL_INSERT) ? "1" : "0";
                                    queryMng.setParam("MANUAL_INSERT", manual2);
                                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                                    queryMng.setParam("system_id", ds_docNumber.Tables[0].Rows[indiceSystem_id_DpaAssTemplates]["SYSTEM_ID"].ToString());
                                    queryMng.setParam("valoreSc", "0");
                                    queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.GetDate());
                                    queryMng.setParam("anno", oggettoCustom.ANNO);
                                    commandText = queryMng.getSQL();
                                    System.Diagnostics.Debug.WriteLine("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                    logger.Debug("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                                    indiceSystem_id_DpaAssTemplates++;
                                    oggettoCustom.ritornaCaratteriSpeciali();
                                    //storicizzo il campo profilato
                                    insertInStorico(oggettoCustom, oldOggCustom, rowsAffected, ref indexOldObj);
                                    break;
                            }
                        }
                        else
                        {
                            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                            {
                                case "CasellaDiSelezione":
                                    for (int j = 0; j < oggettoCustom.ELENCO_VALORI.Count; j++)
                                        indiceSystem_id_DpaAssTemplates++;
                                    break;
                                default:
                                    indiceSystem_id_DpaAssTemplates++;
                                    break;
                            }
                        }
                    }
                    template.OLD_OGG_CUSTOM.Clear();
                }
            }
            catch (Exception ex)
            {
                dbProvider.RollbackTransaction();
                logger.Debug("Errore nel salvataggio dei campi profilati - DOCUMENTO DocNumber = " + docNumber + " TIPOLOGIA = " + template.DESCRIZIONE + " ERRORE = " + ex.Message);
                throw ex;
            }
            finally
            {
                dbProvider.Dispose();
            }
            logger.Info("END");
        }

        public bool disabilitaTemplate(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione, string serverPath, string codiceAmministrazione)
        {
            template.gestisciCaratteriSpeciali();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                string commandText;
                DocsPaUtils.Query queryMng;

                #region Codice Commetato - Non viene più eliminato il template ma solo disabilitato
                /*
                if (template.IN_ESERCIZIO.Equals("NO"))
                {
                    //Cancello l'associazione del template
                    template = this.getTemplateById(template.ID_TIPO_ATTO);
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_ASSOCIAZIONE_TEMPLATES");
                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - disabilitaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);

                    //Cancello eventuali Valori degli Oggetti Custom e gli Oggetti Custom 
                    for (int i = 0; i < template.ELENCO_OGGETTI.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[i];

                        //Cancellazione posizione
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_OGG_CUSTOM_COMP");
                        queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                        queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                        //Se è un oggetto comune non devo cancellarlo correttamente ho precedentemente
                        //cacellato la sua posizione nel template che vado a rimuovere ma essendo un oggetto
                        //condiviso da altri template non va cancellato
                        if (oggettoCustom.CAMPO_COMUNE != "1")
                        {
                            //Cancellazione valori
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_ASSOCIAZIONE_VALORI");
                            queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - disabilitaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);

                            //Cancellazione oggetto
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_OGGETTI_CUSTOM");
                            queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - disabilitaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);

                            //Cancellazione visibilità oggetto
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DEL_A_R_CAMPI_DOC");
                            queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                            queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - disabilitaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);
                        }
                    }

                    //Elimino la visibilità sul template
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_ASS_DOC_RUOLI");
                    queryMng.setParam("idTipoDoc", template.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);

                    //Elimino il tipo documento
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_TEMPLATE");
                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - disabilitaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                }
                else
                {
                    //Disabilito il template
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DISABILITA_TEMPLATE");
                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - disabilitaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                }
                */
                #endregion Codice Commetato - Non viene più eliminato il template ma solo disabilitato

                //Disabilito il template
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DISABILITA_TEMPLATE");
                queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - disabilitaTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);

                //Controllo se ci sono dei modelli da cancellare
                if (Directory.Exists(serverPath + "\\Modelli\\" + codiceAmministrazione + "\\" + template.DESCRIZIONE + "\\"))
                {
                    Directory.Delete(serverPath + "\\Modelli\\" + codiceAmministrazione + "\\" + template.DESCRIZIONE, true);
                }
            }
            catch
            {
                dbProvider.RollbackTransaction();
                return false;
            }
            finally
            {
                dbProvider.Dispose();
            }
            return true;
        }

        public void messaInEsercizioTemplate(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_MESSA_IN_ESERCIZIO");
                queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());

                if (template.IN_ESERCIZIO.ToUpper().Equals("SI"))
                    queryMng.setParam("paramInEsercizio", "NO");
                else
                    queryMng.setParam("paramInEsercizio", "SI");

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - messaInEsercizioTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - messaInEsercizioTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);

                // Se il template per cui cambiare la messa in esercizio contiene un contatore di repertorio
                // bisogna aprire / chiudere tutte le istanze del repertorio
                OggettoCustom obj = ((OggettoCustom[])template.ELENCO_OGGETTI.ToArray(typeof(OggettoCustom))).Where(o => o.TIPO.DESCRIZIONE_TIPO.ToLower() == "contatore" && o.REPERTORIO == "1").FirstOrDefault();
                if (obj != null)
                {
                    RegistriRepertorioPrintManager manager = new RegistriRepertorioPrintManager();
                    manager.SaveCounterStateInAllInstances(
                        obj.SYSTEM_ID.ToString(),
                        template.IN_ESERCIZIO.ToUpper().Equals("SI") ?
                            DocsPaVO.utente.Repertori.RegistroRepertorioSingleSettings.RepertorioState.C :
                            DocsPaVO.utente.Repertori.RegistroRepertorioSingleSettings.RepertorioState.O);
                }
                //codice aggiunto da C.Fuccia per la gestione manuale della sospensione dei contatori custom
                OggettoCustom cc =((OggettoCustom[])template.ELENCO_OGGETTI.ToArray(typeof(OggettoCustom))).Where(c => c.TIPO.DESCRIZIONE_TIPO.ToLower() == "contatore" && !string.IsNullOrEmpty(c.DATA_INIZIO) && !string.IsNullOrEmpty(c.DATA_FINE) ).FirstOrDefault();
                if (cc != null)
                {
                    DocsPaUtils.Query queryMngcc  = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GESTIONE_SOSPENSIONE");
                    queryMngcc.setParam("id_ogg", cc.SYSTEM_ID.ToString());
                    if (template.IN_ESERCIZIO.ToUpper().Equals("SI"))
                        queryMngcc.setParam("sospensione", "SI");
                    else
                        queryMngcc.setParam("sospensione", "NO");
                    string commandTextcc = queryMngcc.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - messaInEsercizioTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandTextcc);
                    logger.Debug("SQL - messaInEsercizioTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandTextcc);
                    dbProvider.ExecuteNonQuery(commandTextcc);

                }

            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public void aggiornaPosizioni(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom_1, DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom_2, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_AGGIORNA_POSIZIONE");
                queryMng.setParam("posizione", oggettoCustom_1.POSIZIONE);
                queryMng.setParam("idOggettoCustom", oggettoCustom_1.SYSTEM_ID.ToString());
                queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - aggiornaPosizioni - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - aggiornaPosizioni - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);

                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_AGGIORNA_POSIZIONE");
                queryMng.setParam("posizione", oggettoCustom_2.POSIZIONE);
                queryMng.setParam("idOggettoCustom", oggettoCustom_2.SYSTEM_ID.ToString());
                queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - aggiornaPosizioni - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - aggiornaPosizioni - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public void aggiornaPosizione(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_AGGIORNA_POSIZIONE");
                queryMng.setParam("posizione", oggettoCustom.POSIZIONE);
                queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - aggiornaPosizione - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - aggiornaPosizione - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public DocsPaVO.ProfilazioneDinamica.Templates getTemplatePerRicerca(string idAmministrazione, string tipoAtto)
        {
            DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE_1");
                queryMng.setParam("tipoAtto", tipoAtto);
                queryMng.setParam("idAmministrazione", idAmministrazione);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_template = new DataSet();
                dbProvider.ExecuteQuery(ds_template, commandText);

                //In questo caso verifico se esiste un template per l'amministrazione e il tipo di atto desiderato
                if (ds_template.Tables[0].Rows.Count == 0)
                {
                    return null;
                }
                else
                {
                    setTemplate(ref template, ds_template, 0);
                    template.DOC_NUMBER = "";

                    //Recupero i dati del template
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TEMPLATE_DOC");
                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                    //queryMng.setParam("docNumber", "''");
                    queryMng.setParam("docNumber", " AND (DPA_ASSOCIAZIONE_TEMPLATES.DOC_NUMBER  = '' OR DPA_ASSOCIAZIONE_TEMPLATES.DOC_NUMBER IS NULL) ");
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    DataSet ds_templateCompleto = new DataSet();
                    dbProvider.ExecuteQuery(ds_templateCompleto, commandText);

                    setTemplate(ref template, ds_templateCompleto, 0);
                    template.DOC_NUMBER = "";

                    //Cerco gli oggetti custom associati al template che sono campi di ricerca
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;
                    for (int j = 0; j < ds_templateCompleto.Tables[0].Rows.Count; j++)
                    {
                        oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                        setOggettoCustom(ref oggettoCustom, ds_templateCompleto, j);

                        if (oggettoCustom.CAMPO_DI_RICERCA == "SI")
                        {
                            DocsPaVO.ProfilazioneDinamica.TipoOggetto tipoOggetto = new DocsPaVO.ProfilazioneDinamica.TipoOggetto();
                            setTipoOggetto(ref tipoOggetto, ds_templateCompleto, j);

                            //Aggiungo il tipo oggetto all'oggettoCustom
                            oggettoCustom.TIPO = tipoOggetto;

                            //campo CLOB di configurazione
                            if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                            {
                                string config = dbProvider.GetLargeText("DPA_OGGETTI_CUSTOM", oggettoCustom.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST");
                                oggettoCustom.CONFIG_OBJ_EST = config;
                            }

                            //Selezioni i valori per l'oggettoCustom
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI");
                            queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                            DataSet ds_valoriOggetto = new DataSet();
                            dbProvider.ExecuteQuery(ds_valoriOggetto, commandText);
                            for (int k = 0; k < ds_valoriOggetto.Tables[0].Rows.Count; k++)
                            {
                                DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto();
                                setValoreOggetto(ref valoreOggetto, ds_valoriOggetto, k);
                                oggettoCustom.ELENCO_VALORI.Add(valoreOggetto);
                            }
                            template.ELENCO_OGGETTI.Add(oggettoCustom);
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return template;
        }

        public string getIdTemplate(string docNumber)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            string idTemplate = "";

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_ID_TEMPLATE");
                queryMng.setParam("docNumber", docNumber);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getIdTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getIdTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    idTemplate = ds.Tables[0].Rows[0]["ID_TEMPLATE"].ToString();
                }
                return idTemplate;
            }
            catch
            {
                return idTemplate;
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public bool isValueInUse(string idOggetto, string idTemplate, string valoreOggettoDB)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_IS_VALUE_IN_USE");
                queryMng.setParam("idOggetto", idOggetto);
                queryMng.setParam("idTemplate", idTemplate);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - isValueInUse - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - isValueInUse - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string valore = ds.Tables[0].Rows[i]["Valore_Oggetto_db"].ToString();
                        if (valore.IndexOf(valoreOggettoDB) != -1)
                            return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public void salvaModelli(byte[] dati, string nomeProfilo, string codiceAmministrazione, string nomeFile, string estensione, string serverPath, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                // Modifica introdotta per consentire la gestione del modello M/Text
                if (dati != null)
                    this.CreateFile(serverPath, codiceAmministrazione, nomeProfilo, dati, nomeFile);

                //Update dei path e le estensioni dei modelli
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_PATH_MODELLI");
                if (template.PATH_MODELLO_1 != "")
                {
                    if (template.PATH_MODELLO_1.IndexOf(':') != -1 || estensione.Equals("MTXT"))
                        queryMng.setParam("pathModUno", template.PATH_MODELLO_1);
                    else
                        queryMng.setParam("pathModUno", serverPath + "\\" + template.PATH_MODELLO_1);
                    queryMng.setParam("extModUno", estensione);
                }
                else
                {
                    queryMng.setParam("pathModUno", "");
                    queryMng.setParam("extModUno", "");
                }

                if (template.PATH_MODELLO_2 != "")
                {
                    if (template.PATH_MODELLO_2.IndexOf(':') != -1 || estensione.Equals("MTXT"))
                        queryMng.setParam("pathModDue", template.PATH_MODELLO_2);
                    else
                        queryMng.setParam("pathModDue", serverPath + "\\" + template.PATH_MODELLO_2);
                    queryMng.setParam("extModDue", estensione);
                }
                else
                {
                    queryMng.setParam("pathModDue", "");
                    queryMng.setParam("extModDue", "");
                }

                if (!string.IsNullOrEmpty(template.PATH_MODELLO_STAMPA_UNIONE))
                {
                    if (template.PATH_MODELLO_STAMPA_UNIONE.IndexOf(':') != -1)
                        queryMng.setParam("pathModSU", template.PATH_MODELLO_STAMPA_UNIONE);
                    else
                        queryMng.setParam("pathModSU", serverPath + "\\" + template.PATH_MODELLO_STAMPA_UNIONE);
                }
                else
                {
                    queryMng.setParam("pathModSU", "");
                }

                if (template.PATH_ALLEGATO_1 != "")
                {
                    if (template.PATH_ALLEGATO_1.IndexOf(':') != -1)
                        queryMng.setParam("pathAllUno", template.PATH_ALLEGATO_1);
                    else
                        queryMng.setParam("pathAllUno", serverPath + "\\" + template.PATH_ALLEGATO_1);
                    queryMng.setParam("extAllUno", estensione);
                }
                else
                {
                    queryMng.setParam("pathAllUno", "");
                    queryMng.setParam("extAllUno", "");
                }

                if (!string.IsNullOrEmpty(template.PATH_MODELLO_EXCEL))
                {
                    if (template.PATH_MODELLO_EXCEL.IndexOf(':') != -1)
                        queryMng.setParam("pathmodexc", template.PATH_MODELLO_EXCEL);
                    else
                        queryMng.setParam("pathmodexc", serverPath + "\\" + template.PATH_MODELLO_EXCEL);
                }
                else
                {
                    queryMng.setParam("pathmodexc", "");
                }

                queryMng.setParam("idTemplate", Convert.ToString(template.SYSTEM_ID));

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - salvaModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - salvaModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        private void CreateFile(string serverPath, string codiceAmministrazione, string nomeProfilo, byte[] dati, String nomeFile)
        {
            if (Directory.Exists(serverPath + "\\Modelli\\" + codiceAmministrazione + "\\" + nomeProfilo + "\\"))
            {
                FileStream fs1 = new FileStream(serverPath + "\\Modelli\\" + codiceAmministrazione + "\\" + nomeProfilo + "\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.Write);
                fs1.Write(dati, 0, dati.Length);
                fs1.Close();
            }
            else
            {
                Directory.CreateDirectory(serverPath + "\\Modelli\\" + codiceAmministrazione + "\\" + nomeProfilo + "\\");
                FileStream fs1 = new FileStream(serverPath + "\\Modelli\\" + codiceAmministrazione + "\\" + nomeProfilo + "\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.Write);
                fs1.Write(dati, 0, dati.Length);
                fs1.Close();
            }
        }

        public void eliminaModelli(string nomeProfilo, string codiceAmministrazione, string nomeFile, string estensione, string serverPath, DocsPaVO.ProfilazioneDinamica.Templates template)
        {

            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //Elimino il file
                if (Directory.Exists(serverPath + "\\Modelli\\" + codiceAmministrazione + "\\" + nomeProfilo + "\\"))
                {
                    File.Delete(serverPath + "\\Modelli\\" + codiceAmministrazione + "\\" + nomeProfilo + "\\" + nomeFile);
                    string[] files = Directory.GetFiles(serverPath + "\\Modelli\\" + codiceAmministrazione + "\\" + nomeProfilo + "\\");
                    if (files.Length == 0)
                        Directory.Delete(serverPath + "\\Modelli\\" + codiceAmministrazione + "\\" + nomeProfilo + "\\");
                }

                //Update dei path e le estensioni dei modelli
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_PATH_MODELLI");
                if (template.PATH_MODELLO_1 != "")
                {
                    if (template.PATH_MODELLO_1.IndexOf(':') != -1)
                        queryMng.setParam("pathModUno", template.PATH_MODELLO_1);
                    else
                        queryMng.setParam("pathModUno", serverPath + "\\" + template.PATH_MODELLO_1);
                    queryMng.setParam("extModUno", estensione);
                }
                else
                {
                    queryMng.setParam("pathModUno", "");
                    queryMng.setParam("extModUno", "");
                }

                if (template.PATH_MODELLO_2 != "")
                {
                    if (template.PATH_MODELLO_2.IndexOf(':') != -1)
                        queryMng.setParam("pathModDue", template.PATH_MODELLO_2);
                    else
                        queryMng.setParam("pathModDue", serverPath + "\\" + template.PATH_MODELLO_2);
                    queryMng.setParam("extModDue", estensione);
                }
                else
                {
                    queryMng.setParam("pathModDue", "");
                    queryMng.setParam("extModDue", "");
                }

                if (!string.IsNullOrEmpty(template.PATH_MODELLO_STAMPA_UNIONE))
                {
                    if (template.PATH_MODELLO_STAMPA_UNIONE.IndexOf(':') != -1)
                        queryMng.setParam("pathModSU", template.PATH_MODELLO_STAMPA_UNIONE);
                    else
                        queryMng.setParam("pathModSU", serverPath + "\\" + template.PATH_MODELLO_STAMPA_UNIONE);
                }
                else
                {
                    queryMng.setParam("pathModSU", "");
                }

                if (template.PATH_ALLEGATO_1 != "")
                {
                    if (template.PATH_ALLEGATO_1.IndexOf(':') != -1)
                        queryMng.setParam("pathAllUno", template.PATH_ALLEGATO_1);
                    else
                        queryMng.setParam("pathAllUno", serverPath + "\\" + template.PATH_ALLEGATO_1);
                    queryMng.setParam("extAllUno", estensione);
                }
                else
                {
                    queryMng.setParam("pathAllUno", "");
                    queryMng.setParam("extAllUno", "");
                }

                if (!string.IsNullOrEmpty(template.PATH_MODELLO_EXCEL))
                {
                    if (template.PATH_MODELLO_EXCEL.IndexOf(':') != -1)
                        queryMng.setParam("pathmodexc", template.PATH_MODELLO_EXCEL);
                    else
                        queryMng.setParam("pathmodexc", serverPath + "\\" + template.PATH_MODELLO_EXCEL);

                }
                else
                    queryMng.setParam("pathmodexc", "");

                queryMng.setParam("idTemplate", Convert.ToString(template.SYSTEM_ID));

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - salvaModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - salvaModelli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public void updateScadenzeTipoDoc(int systemId_template, string scadenza, string preScadenza)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_SCADENZE");
                queryMng.setParam("idTemplate", systemId_template.ToString());
                if (scadenza != null && scadenza != "")
                    queryMng.setParam("scadenza", scadenza);
                else
                    queryMng.setParam("scadenza", "0");
                if (preScadenza != null && preScadenza != "")
                    queryMng.setParam("preScadenza", preScadenza);
                else
                    queryMng.setParam("preScadenza", "0");

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - updateScadenzeTipoDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - updateScadenzeTipoDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
            catch (Exception e)
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public void UpdatePrivatoTipoDoc(int systemId_template, string privato)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_PRIVATO_DOC");
                queryMng.setParam("idTemplate", systemId_template.ToString());
                if (privato != "")
                    queryMng.setParam("privato", privato);
                else
                    queryMng.setParam("privato", "0");

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - updatePrivatoTipoDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - updatePrivatoTipoDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
            catch (Exception e)
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public void UpdateMesiConsTipoDoc(int systemId_template, string mesiCons)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_MESICONS_DOC");
                queryMng.setParam("idTemplate", systemId_template.ToString());
                if (!string.IsNullOrEmpty(mesiCons))
                    queryMng.setParam("NUM_MESI_CONSERVAZIONE", mesiCons);
                else
                    queryMng.setParam("NUM_MESI_CONSERVAZIONE", "0");

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - UpdateMesiConsTipoDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - UpdateMesiConsTipoDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
            catch (Exception e)
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        #region INTEGRAZIONE PITRE-PARER

        public void UpdateInvioConsTipoDoc(int systemId_template, string invioCons)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_INVIOCONS_DOC");
                queryMng.setParam("idTemplate", systemId_template.ToString());
                if (!string.IsNullOrEmpty(invioCons))
                    queryMng.setParam("INVIO_CONSERVAZIONE", invioCons);
                else
                    queryMng.setParam("INVIO_CONSERVAZIONE", "0");

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - UpdateInvioConsTipoDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - UpdateInvioConsTipoDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);

                // se sto disattivando il flag devo rimuovere anche i flag sottostanti
                // (conservazione oggetti del template, conservazione contatori di repertorio)
                if (invioCons.Equals("0"))
                {
                    DocsPaVO.ProfilazioneDinamica.Templates template = this.getTemplateById(systemId_template.ToString());
                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggCustom in template.ELENCO_OGGETTI)
                    {
                        if (oggCustom.CONSERVAZIONE.Equals("1"))
                            //this.UpdateConsolidaCampo(oggCustom.SYSTEM_ID, "0", string.Empty);
                            this.UpdateConservaCampo(oggCustom.SYSTEM_ID, "0", string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public void UpdateConsolidaCampo(int systemId, string consolida, string systemId_template)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONSOLIDA_CAMPO");
                queryMng.setParam("idOggCustom", systemId.ToString());
                if (!string.IsNullOrEmpty(consolida))
                    queryMng.setParam("CHA_CONSOLIDAMENTO", consolida);
                else
                {
                    queryMng.setParam("CHA_CONSOLIDAMENTO", "0");
                    consolida = "0";
                }
                // se si disattiva il flag consolidamento è necessario rimuovere anche i flag conservazione e conservazione repertorio
                if(consolida.Equals("0"))
                    queryMng.setParam("CONSERVAZIONE", ", CHA_CONSERVAZIONE=0, CHA_CONS_REPERTORIO=0");
                else
                    queryMng.setParam("CONSERVAZIONE", string.Empty);

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - UpdateConsolidaCampo - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - UpdateConsolidaCampo - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);

                // se il parametro systemId_template non è null devo attivare il campo invio conservazione per il template
                /*
                if (!string.IsNullOrEmpty(systemId_template) && consolida.Equals("1"))
                {
                    this.UpdateInvioConsTipoDoc(Convert.ToInt32(systemId_template), "1");
                }
                */
                
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public void UpdateConservaCampo(int systemId, string conserva, string systemId_template)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONSERVA_CAMPO");
                queryMng.setParam("idOggCustom", systemId.ToString());
                if (!string.IsNullOrEmpty(conserva))
                    queryMng.setParam("CHA_CONSERVAZIONE", conserva);
                else
                {
                    queryMng.setParam("CHA_CONSERVAZIONE", "0");
                    conserva = "0";
                }
                // se si attiva il flag consolidamento è necessario attivare anche il flag conservazione
                // se invece si disattiva, è necessario disattivare il flag conserva repertorio
                if (conserva.Equals("1"))
                    queryMng.setParam("CONSOLIDAMENTO", ", CHA_CONSOLIDAMENTO=1");
                else
                    queryMng.setParam("CONSOLIDAMENTO", ", CHA_CONS_REPERTORIO=0");


                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - UpdateConservaCampo - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - UpdateConservaCampo - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);

                // se il parametro systemId_template non è null devo attivare il campo invio conservazione per il template
                if (!string.IsNullOrEmpty(systemId_template))
                {
                    this.UpdateInvioConsTipoDoc(Convert.ToInt32(systemId_template), "1");
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                dbProvider.RollbackTransaction();
            }
            finally
            {

                dbProvider.Dispose();
            }
        }

        #endregion

        public int countDocTipoDoc(string tipo_atto, string codiceAmm)
        {
            int numDoc = 0;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            string numeroDocumenti;
            try
            {
                string idAmm = getIdAmmByCod(codiceAmm);
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CONTATORE");
                queryMng.setParam("idAmministrazione", idAmm);
                queryMng.setParam("idTipoAtto", tipo_atto);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - countDocTipoDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - countDocTipoDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteScalar(out numeroDocumenti, commandText);
                numDoc = Convert.ToInt32(numeroDocumenti);
            }
            catch
            {
                return numDoc;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return numDoc;
        }

        public ArrayList getRuoliByAmm(string idAmm, string codiceRicerca, string tipoRicerca)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            ArrayList listaRuoli = new ArrayList();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_RUOLI_BY_AMM");
                if (codiceRicerca != "")
                {
                    switch (tipoRicerca)
                    {
                        case "COD_RUOLO":
                            queryMng.setParam("tableParam", " ");
                            queryMng.setParam("param", " AND DPA_CORR_GLOBALI.ID_AMM = " + idAmm + " AND UPPER(DPA_CORR_GLOBALI.VAR_COD_RUBRICA) like UPPER('%" + codiceRicerca.Replace("'", "''") + "%') ");
                            break;

                        case "DES_RUOLO":
                            queryMng.setParam("tableParam", " ");
                            queryMng.setParam("param", " AND DPA_CORR_GLOBALI.ID_AMM = " + idAmm + " AND UPPER(DPA_CORR_GLOBALI.VAR_DESC_CORR) like UPPER('%" + codiceRicerca.Replace("'", "''") + "%') ");
                            break;

                        case "COD_RF":
                            queryMng.setParam("tableParam", " , DPA_EL_REGISTRI , DPA_L_RUOLO_REG ");
                            string paramCodRF = " AND DPA_CORR_GLOBALI.ID_AMM = " + idAmm +
                                                " AND DPA_CORR_GLOBALI.SYSTEM_ID = DPA_L_RUOLO_REG.ID_RUOLO_IN_UO " +
                                                " AND DPA_L_RUOLO_REG.ID_REGISTRO = DPA_EL_REGISTRI.SYSTEM_ID " +
                                                " AND DPA_EL_REGISTRI.CHA_RF = 1 " +
                                                " AND UPPER(DPA_EL_REGISTRI.VAR_CODICE) = UPPER('" + codiceRicerca.Replace("'", "''") + "') ";
                            queryMng.setParam("param", paramCodRF);
                            break;

                        case "DES_RF":
                            queryMng.setParam("tableParam", " , DPA_EL_REGISTRI , DPA_L_RUOLO_REG ");
                            string paramDesRF = " AND DPA_CORR_GLOBALI.ID_AMM = " + idAmm +
                                                " AND DPA_CORR_GLOBALI.SYSTEM_ID = DPA_L_RUOLO_REG.ID_RUOLO_IN_UO " +
                                                " AND DPA_L_RUOLO_REG.ID_REGISTRO = DPA_EL_REGISTRI.SYSTEM_ID " +
                                                " AND DPA_EL_REGISTRI.CHA_RF = 1 " +
                                                " AND UPPER(DPA_EL_REGISTRI.VAR_DESC_REGISTRO) like UPPER('%" + codiceRicerca.Replace("'", "''") + "%') ";
                            queryMng.setParam("param", paramDesRF);
                            break;

                        case "COD_AOO":
                            queryMng.setParam("tableParam", " , DPA_EL_REGISTRI , DPA_L_RUOLO_REG ");
                            string paramCodAOO = " AND DPA_CORR_GLOBALI.ID_AMM = " + idAmm +
                                                    " AND DPA_CORR_GLOBALI.SYSTEM_ID = DPA_L_RUOLO_REG.ID_RUOLO_IN_UO " +
                                                    " AND DPA_L_RUOLO_REG.ID_REGISTRO = DPA_EL_REGISTRI.SYSTEM_ID " +
                                                    " AND DPA_EL_REGISTRI.CHA_RF = 0 " +
                                                    " AND UPPER(DPA_EL_REGISTRI.VAR_CODICE) = UPPER('" + codiceRicerca.Replace("'", "''") + "') ";
                            queryMng.setParam("param", paramCodAOO);
                            break;

                        case "DES_AOO":
                            queryMng.setParam("tableParam", " , DPA_EL_REGISTRI , DPA_L_RUOLO_REG ");
                            string paramDesAOO = " AND DPA_CORR_GLOBALI.ID_AMM = " + idAmm +
                                                    " AND DPA_CORR_GLOBALI.SYSTEM_ID = DPA_L_RUOLO_REG.ID_RUOLO_IN_UO " +
                                                    " AND DPA_L_RUOLO_REG.ID_REGISTRO = DPA_EL_REGISTRI.SYSTEM_ID " +
                                                    " AND DPA_EL_REGISTRI.CHA_RF = 0 " +
                                                    " AND UPPER(DPA_EL_REGISTRI.VAR_DESC_REGISTRO) like UPPER('%" + codiceRicerca.Replace("'", "''") + "%') ";
                            queryMng.setParam("param", paramDesAOO);
                            break;

                        case "COD_UO":
                            queryMng.setParam("tableParam", " ");
                            string paramCodUO = " AND DPA_CORR_GLOBALI.ID_AMM = " + idAmm +
                                                " AND DPA_CORR_GLOBALI.ID_UO IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE UPPER(DPA_CORR_GLOBALI.VAR_COD_RUBRICA) = UPPER('" + codiceRicerca.Replace("'", "''") + "') AND CHA_TIPO_URP = 'U') ";
                            queryMng.setParam("param", paramCodUO);
                            break;

                        case "DES_UO":
                            queryMng.setParam("tableParam", " ");
                            string paramDesUO = " AND DPA_CORR_GLOBALI.ID_AMM = " + idAmm +
                                                " AND DPA_CORR_GLOBALI.ID_UO IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE UPPER(DPA_CORR_GLOBALI.VAR_DESC_CORR) like UPPER('%" + codiceRicerca.Replace("'", "''") + "%') AND CHA_TIPO_URP = 'U') ";
                            queryMng.setParam("param", paramDesUO);
                            break;
                        case "COD_TIPO":
                            queryMng.setParam("tableParam", " , DPA_TIPO_RUOLO ");
                            string paramCodTipo = " AND DPA_CORR_GLOBALI.ID_AMM = " + idAmm +
                                                    " AND DPA_CORR_GLOBALI.ID_TIPO_RUOLO = DPA_TIPO_RUOLO.SYSTEM_ID " +
                                                    " AND UPPER(DPA_TIPO_RUOLO.VAR_CODICE) = UPPER('" + codiceRicerca.Replace("'", "''") + "') ";
                            queryMng.setParam("param", paramCodTipo);
                            break;
                        case "DES_TIPO":
                            queryMng.setParam("tableParam", " , DPA_TIPO_RUOLO ");
                            string paramDesTipo = " AND DPA_CORR_GLOBALI.ID_AMM = " + idAmm +
                                                    " AND DPA_CORR_GLOBALI.ID_TIPO_RUOLO = DPA_TIPO_RUOLO.SYSTEM_ID " +
                                                    " AND UPPER(DPA_TIPO_RUOLO.VAR_DESC_RUOLO) like UPPER('%" + codiceRicerca.Replace("'", "''") + "%') ";
                            queryMng.setParam("param", paramDesTipo);
                            break;
                    }
                }
                else
                {
                    queryMng.setParam("tableParam", " ");
                    queryMng.setParam("param", " AND DPA_CORR_GLOBALI.ID_AMM = " + idAmm + " ");
                }
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getRuoliByAmm - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getRuoliByAmm - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                DataSet ds_listaRuoli = new DataSet();
                dbProvider.ExecuteQuery(ds_listaRuoli, commandText);

                if (ds_listaRuoli.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds_listaRuoli.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.utente.Ruolo r = new DocsPaVO.utente.Ruolo();
                        r.systemId = ds_listaRuoli.Tables[0].Rows[i]["SYSTEM_ID"].ToString();
                        r.idGruppo = ds_listaRuoli.Tables[0].Rows[i]["ID_GRUPPO"].ToString();
                        r.descrizione = ds_listaRuoli.Tables[0].Rows[i]["VAR_DESC_CORR"].ToString();
                        r.codice = ds_listaRuoli.Tables[0].Rows[i]["VAR_COD_RUBRICA"].ToString();
                        listaRuoli.Add(r);
                    }
                }

                return listaRuoli;
            }
            catch
            {
                return listaRuoli;
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public void salvaAssociazioneDocRuoli(ArrayList assDocRuoli)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                foreach (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli in assDocRuoli)
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_RUOLI_TIPO_DOC");
                    queryMng.setParam("idTipoDoc", assDocFascRuoli.ID_TIPO_DOC_FASC);
                    queryMng.setParam("idRuolo", assDocFascRuoli.ID_GRUPPO);
                    queryMng.setParam("diritti", assDocFascRuoli.DIRITTI_TIPOLOGIA);
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneDocRuoli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaAssociazioneDocRuoli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    int rowsAffected;
                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                    if (rowsAffected == 0)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INSERT_ASS_DOC_RUOLI");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_VIS_TIPO_DOC"));
                        queryMng.setParam("idTipoDoc", assDocFascRuoli.ID_TIPO_DOC_FASC);
                        queryMng.setParam("idRuolo", assDocFascRuoli.ID_GRUPPO);
                        queryMng.setParam("diritti", assDocFascRuoli.DIRITTI_TIPOLOGIA);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneDocRuoli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaAssociazioneDocRuoli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                    }
                }
            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public ArrayList getRuoliTipoDoc(string idTipoDoc)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            ArrayList listaAssRuoli = new ArrayList();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_RUOLI_TIPO_DOC");
                queryMng.setParam("idTipoDoc", idTipoDoc);

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getRuoliTipoDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getRuoliTipoDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                DataSet ds_listaAssRuoli = new DataSet();
                dbProvider.ExecuteQuery(ds_listaAssRuoli, commandText);

                if (ds_listaAssRuoli.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds_listaAssRuoli.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli obj = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                        setAssDocFascRuoli(ref obj, ds_listaAssRuoli, i);
                        listaAssRuoli.Add(obj);
                    }
                }
                return listaAssRuoli;
            }
            catch
            {
                return listaAssRuoli;
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public ArrayList getRuoliTipoAtto(string idTipoDoc, string testo, string tipoRicerca)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            ArrayList listaAssRuoli = new ArrayList();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_RUOLI_TIPO_ATTO");
                if (testo != "")
                {
                    if (tipoRicerca.Equals("C"))
                        queryMng.setParam("idTipoDoc", idTipoDoc + " AND UPPER(B.VAR_COD_RUBRICA) like UPPER('" + testo.Replace("'", "''") + "%')");
                    else
                        queryMng.setParam("idTipoDoc", idTipoDoc + " AND UPPER(B.VAR_DESC_CORR) like UPPER('%" + testo.Replace("'", "''") + "%')");
                }
                else
                    queryMng.setParam("idTipoDoc", idTipoDoc);

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getRuoliTipoDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getRuoliTipoDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                DataSet ds_listaAssRuoli = new DataSet();
                dbProvider.ExecuteQuery(ds_listaAssRuoli, commandText);

                if (ds_listaAssRuoli.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds_listaAssRuoli.Tables[0].Rows.Count; i++)
                    {
                        listaAssRuoli.Add(ds_listaAssRuoli.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                    }
                }
                return listaAssRuoli;
            }
            catch
            {
                return listaAssRuoli;
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        //questa funzione controlla se è associato un record nella DPA_CONT_CUSTOM_DOC
        //relativo al system_id dell'oggettocustom corrente
        private void ControllaCustom(ref DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaUtils.Query queryMng;
            string commandText = string.Empty;
            //controllo se esiste un record nella DPA_CONT_CUSTOM_DOC associato
            //al system id dell'oggetto custom corrente
            try
            {
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CONT_CUSTOM_DOC_BY_IDOGG");
                queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());
                commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - cerca contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - cerca contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_datiContatoreCustom = new DataSet(); ;
                dbProvider.ExecuteQuery(ds_datiContatoreCustom, commandText);
                if (ds_datiContatoreCustom.Tables[0].Rows.Count != 0)
                {
                    oggettoCustom.DATA_INIZIO = ds_datiContatoreCustom.Tables[0].Rows[0]["DATA_INIZIO"].ToString();
                    oggettoCustom.DATA_FINE = ds_datiContatoreCustom.Tables[0].Rows[0]["DATA_FINE"].ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Controlla Custom Exception message: ", ex);
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        private void calcolaContatore(DocsPaVO.ProfilazioneDinamica.Templates template, ref DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, bool inserimentoAggiornamento)
        {

            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            oggettoCustom.VALORE_DATABASE = string.Empty;
            DocsPaUtils.Query queryMng;
            string commandText = string.Empty;
            //controllo se esiste un record nella DPA_CONT_CUSTOM_DOC associato
            //al system id dell'oggetto custom corrente
            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CONT_CUSTOM_DOC_BY_IDOGG");
            queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());
            commandText = queryMng.getSQL();
            System.Diagnostics.Debug.WriteLine("SQL - cerca contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
            logger.Debug("SQL - cerca contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
            DataSet ds_datiContatoreCustom = new DataSet(); ;
            dbProvider.ExecuteQuery(ds_datiContatoreCustom, commandText);
            if (ds_datiContatoreCustom.Tables[0].Rows.Count != 0)
            {
                if (oggettoCustom.CONTATORE_DA_FAR_SCATTARE)
                {
                    {
                        //Verifico che il contatore esiste, se si lo recupero altrimenti lo inserisco e ne recupero i dati
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_EXIST_CONT_DOC");
                        queryMng.setParam("idOggetto", oggettoCustom.SYSTEM_ID.ToString());
                        queryMng.setParam("idTipologia", template.SYSTEM_ID.ToString());

                        switch (oggettoCustom.TIPO_CONTATORE)
                        {
                            case "T":
                                queryMng.setParam("param1", "");
                                break;

                            case "A":
                                queryMng.setParam("param1", " AND ID_AOO = " + oggettoCustom.ID_AOO_RF.ToString());
                                break;

                            case "R":
                                queryMng.setParam("param1", " AND ID_RF =  " + oggettoCustom.ID_AOO_RF.ToString());
                                break;

                            default:
                                queryMng.setParam("param1", "");
                                break;
                        }
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - cercaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - cercaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        DataSet ds_datiContatore = new DataSet();
                        dbProvider.ExecuteQuery(ds_datiContatore, commandText);

                        //Contatore non esistente 
                        if (ds_datiContatore.Tables[0].Rows.Count == 0)
                        {
                            //Inserisco il contatore
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INSERT_CONT_DOC");
                            queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                            queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONTATORI_DOC"));
                            queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());
                            queryMng.setParam("idTipologia", template.SYSTEM_ID.ToString());
                            /*
                            if (oggettoCustom.CAMPO_COMUNE.Equals("1"))
                            {
                                queryMng.setParam("idTipologia", "0");
                            }
                            else
                            {
                                queryMng.setParam("idTipologia", template.SYSTEM_ID.ToString());
                            }
                             * */
                            switch (oggettoCustom.TIPO_CONTATORE)
                            {
                                case "T":
                                    queryMng.setParam("idAoo", "0");
                                    queryMng.setParam("idRf", "0");
                                    break;

                                case "A":
                                    queryMng.setParam("idAoo", oggettoCustom.ID_AOO_RF);
                                    queryMng.setParam("idRf", "0");
                                    break;

                                case "R":
                                    queryMng.setParam("idAoo", "0");
                                    queryMng.setParam("idRf", oggettoCustom.ID_AOO_RF);
                                    break;

                                default:
                                    queryMng.setParam("idAoo", "0");
                                    queryMng.setParam("idRf", "0");
                                    break;
                            }
                            queryMng.setParam("valore", "0");
                            queryMng.setParam("valoreSottocontatore", "0");
                            queryMng.setParam("abilitato", "1");
                            queryMng.setParam("anno", oggettoCustom.ANNO);
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - inserisciContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - inserisciContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);

                            //Reperimento systemid
                            commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_CONTATORI_DOC");
                            string idContatoreInserito = string.Empty;
                            dbProvider.ExecuteScalar(out idContatoreInserito, commandText);

                            //Recupero i dati del contatore
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CONT_DOC_BY_ID");
                            queryMng.setParam("systemId", idContatoreInserito);
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - cercaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - cercaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            dbProvider.ExecuteQuery(ds_datiContatore, commandText);



                        }

                        if (ds_datiContatore.Tables[0].Rows.Count != 0)
                        {
                            int annoContatore = 0;
                            int annoOggetto = 0;
                            int valoreContatore = 0;
                            int valoreSottocontatore = 0;
                            string valoreDataInizio = string.Empty;
                            string valoreDataFine = string.Empty;
                            int annoCorrente = System.DateTime.Now.Year;
                            if (ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString() != null && ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString() != "")
                                annoContatore = Convert.ToInt32(ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString());
                            if (oggettoCustom.ANNO != null && oggettoCustom.ANNO != "")
                                annoOggetto = Convert.ToInt32(oggettoCustom.ANNO);
                            if (ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString() != null && ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString() != "")
                                valoreContatore = Convert.ToInt32(ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString());
                            if (ds_datiContatore.Tables[0].Columns.Contains("VALORE_SC") && !string.IsNullOrEmpty(ds_datiContatore.Tables[0].Rows[0]["VALORE_SC"].ToString()))
                                valoreSottocontatore = Convert.ToInt32(ds_datiContatore.Tables[0].Rows[0]["VALORE_SC"].ToString());
                            if (ds_datiContatoreCustom.Tables[0].Columns.Contains("DATA_INIZIO") && !string.IsNullOrEmpty(ds_datiContatoreCustom.Tables[0].Rows[0]["DATA_INIZIO"].ToString()))
                                valoreDataInizio = Convert.ToDateTime(ds_datiContatoreCustom.Tables[0].Rows[0]["DATA_INIZIO"]).Year.ToString();
                            if (ds_datiContatoreCustom.Tables[0].Columns.Contains("DATA_FINE") && !string.IsNullOrEmpty(ds_datiContatoreCustom.Tables[0].Rows[0]["DATA_FINE"].ToString()))
                                valoreDataFine = Convert.ToDateTime(ds_datiContatoreCustom.Tables[0].Rows[0]["DATA_FINE"]).Year.ToString();



                            //Incremento il valore del contatore e lo aggiorno
                            //if (inserimentoAggiornamento || (oggettoCustom.CONTA_DOPO == "1" && oggettoCustom.CONTATORE_DA_FAR_SCATTARE))
                            if (inserimentoAggiornamento || oggettoCustom.CONTATORE_DA_FAR_SCATTARE)
                            {
                                //Caso di un contatore con sottocontatore
                                if (!string.IsNullOrEmpty(oggettoCustom.MODULO_SOTTOCONTATORE) && !oggettoCustom.MODULO_SOTTOCONTATORE.Equals("0"))
                                {
                                    valoreSottocontatore++;
                                    if (valoreContatore == 0)
                                        valoreContatore++;
                                    //if (Convert.ToInt32(oggettoCustom.MODULO_SOTTOCONTATORE) == valoreSottocontatore)
                                    if ((valoreSottocontatore - 1) >= Convert.ToInt32(oggettoCustom.MODULO_SOTTOCONTATORE))  //SAB
                                    {
                                        valoreSottocontatore = 1;
                                        valoreContatore++;
                                    }
                                }
                                //Caso di un contatore normale
                                else
                                {
                                    valoreContatore++;
                                }

                                oggettoCustom.VALORE_DATABASE = valoreContatore.ToString();
                                oggettoCustom.VALORE_SOTTOCONTATORE = valoreSottocontatore.ToString();
                                oggettoCustom.DATA_INSERIMENTO = dbProvider.GetSysdate();

                                if (inserimentoAggiornamento)
                                {
                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONT_DOC");
                                    queryMng.setParam("idContatore", ds_datiContatore.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                                    queryMng.setParam("valoreContatore", valoreContatore.ToString());
                                    queryMng.setParam("valoreSottocontatore", valoreSottocontatore.ToString());
                                    queryMng.setParam("anno", oggettoCustom.ANNO);
                                    commandText = queryMng.getSQL();
                                    System.Diagnostics.Debug.WriteLine("SQL - aggiornaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                    logger.Debug("SQL - aggiornaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                    dbProvider.ExecuteNonQuery(commandText);
                                }
                            }
                            //Caso in cui non ci troviamo nella situazione di inserire un nuovo oggettoCustom perchè già esiste
                            //e inoltre non dovendo far scattere il conta dopo, non si deve incementare nulla  a meno che il contatore non è stato inserito ora 
                            //e quindi va inizializzato
                            else
                            {
                                if (valoreContatore == 0)
                                    valoreContatore++;
                                oggettoCustom.VALORE_DATABASE = valoreContatore.ToString();
                            }
                        }
                    }



                }

            }
            else
            {
                if (ds_datiContatoreCustom.Tables[0].Rows.Count == 0)
                {   //Verifico se deve scattare il contatore
                    //OggettoCustom.CONTA_DOPO = 1 AND OggettoCustom.CONTATORE_DA_FAR_SCATTARE = true - SI 
                    //OggettoCustom.CONTA_DOPO != 1 - SI 
                    //In tutti gli altri casi il contatore non deve scattare
                    //ma va comunque inserita una riga nella DPA_ASSOCIAZIONE_TEMPLATES con valore vuoto
                    //perchè posso decidere di far scattare il contatore in momenti diversi

                    //if ((oggettoCustom.CONTA_DOPO == "1" && oggettoCustom.CONTATORE_DA_FAR_SCATTARE) || oggettoCustom.CONTA_DOPO != "1")
                    if (oggettoCustom.CONTATORE_DA_FAR_SCATTARE)
                    {
                        //Verifico che il contatore esiste, se si lo recupero altrimenti lo inserisco e ne recupero i dati
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_EXIST_CONT_DOC");
                        queryMng.setParam("idOggetto", oggettoCustom.SYSTEM_ID.ToString());
                        queryMng.setParam("idTipologia", template.SYSTEM_ID.ToString());
                        /*
                        if (oggettoCustom.CAMPO_COMUNE.Equals("1"))
                        {
                            queryMng.setParam("idOggetto", oggettoCustom.SYSTEM_ID.ToString());
                            queryMng.setParam("idTipologia", " ");
                        }
                        else
                        {
                            queryMng.setParam("idOggetto", oggettoCustom.SYSTEM_ID.ToString());
                            queryMng.setParam("idTipologia", " AND ID_TIPOLOGIA = " + template.SYSTEM_ID.ToString() + " ");
                        }
                         */
                        switch (oggettoCustom.TIPO_CONTATORE)
                        {
                            case "T":
                                queryMng.setParam("param1", "");
                                break;

                            case "A":
                                queryMng.setParam("param1", " AND ID_AOO = " + oggettoCustom.ID_AOO_RF.ToString());
                                break;

                            case "R":
                                queryMng.setParam("param1", " AND ID_RF =  " + oggettoCustom.ID_AOO_RF.ToString());
                                break;

                            default:
                                queryMng.setParam("param1", "");
                                break;
                        }
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - cercaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - cercaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        DataSet ds_datiContatore = new DataSet();
                        dbProvider.ExecuteQuery(ds_datiContatore, commandText);

                        //Contatore non esistente 
                        if (ds_datiContatore.Tables[0].Rows.Count == 0)
                        {
                            //Inserisco il contatore
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INSERT_CONT_DOC");
                            queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                            queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONTATORI_DOC"));
                            queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());
                            queryMng.setParam("idTipologia", template.SYSTEM_ID.ToString());
                            /*
                            if (oggettoCustom.CAMPO_COMUNE.Equals("1"))
                            {
                                queryMng.setParam("idTipologia", "0");
                            }
                            else
                            {
                                queryMng.setParam("idTipologia", template.SYSTEM_ID.ToString());
                            }
                             * */
                            switch (oggettoCustom.TIPO_CONTATORE)
                            {
                                case "T":
                                    queryMng.setParam("idAoo", "0");
                                    queryMng.setParam("idRf", "0");
                                    break;

                                case "A":
                                    queryMng.setParam("idAoo", oggettoCustom.ID_AOO_RF);
                                    queryMng.setParam("idRf", "0");
                                    break;

                                case "R":
                                    queryMng.setParam("idAoo", "0");
                                    queryMng.setParam("idRf", oggettoCustom.ID_AOO_RF);
                                    break;

                                default:
                                    queryMng.setParam("idAoo", "0");
                                    queryMng.setParam("idRf", "0");
                                    break;
                            }
                            queryMng.setParam("valore", "0");
                            queryMng.setParam("valoreSottocontatore", "0");
                            queryMng.setParam("abilitato", "1");
                            queryMng.setParam("anno", oggettoCustom.ANNO);
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - inserisciContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - inserisciContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);

                            //Reperimento systemid
                            commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_CONTATORI_DOC");
                            string idContatoreInserito = string.Empty;
                            dbProvider.ExecuteScalar(out idContatoreInserito, commandText);

                            //Recupero i dati del contatore
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CONT_DOC_BY_ID");
                            queryMng.setParam("systemId", idContatoreInserito);
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - cercaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - cercaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            dbProvider.ExecuteQuery(ds_datiContatore, commandText);
                        }

                        if (ds_datiContatore.Tables[0].Rows.Count != 0)
                        {
                            int annoContatore = 0;
                            int annoOggetto = 0;
                            int valoreContatore = 0;
                            int valoreSottocontatore = 0;
                            int annoCorrente = System.DateTime.Now.Year;
                            if (ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString() != null && ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString() != "")
                                annoContatore = Convert.ToInt32(ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString());
                            if (oggettoCustom.ANNO != null && oggettoCustom.ANNO != "")
                                annoOggetto = Convert.ToInt32(oggettoCustom.ANNO);
                            if (ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString() != null && ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString() != "")
                                valoreContatore = Convert.ToInt32(ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString());
                            if (ds_datiContatore.Tables[0].Columns.Contains("VALORE_SC") && !string.IsNullOrEmpty(ds_datiContatore.Tables[0].Rows[0]["VALORE_SC"].ToString()))
                                valoreSottocontatore = Convert.ToInt32(ds_datiContatore.Tables[0].Rows[0]["VALORE_SC"].ToString());

                            if (oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI")
                            {
                                //if (annoContatore < annoOggetto)
                                if (annoContatore < annoCorrente)
                                {
                                    //Reinizializzo il contatore
                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONT_DOC");
                                    queryMng.setParam("idContatore", ds_datiContatore.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                                    queryMng.setParam("valoreContatore", "1");
                                    queryMng.setParam("valoreSottocontatore", "1");
                                    //queryMng.setParam("anno", oggettoCustom.ANNO);
                                    queryMng.setParam("anno", annoCorrente.ToString());
                                    commandText = queryMng.getSQL();
                                    System.Diagnostics.Debug.WriteLine("SQL - aggiornaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                    logger.Debug("SQL - aggiornaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                    dbProvider.ExecuteNonQuery(commandText);
                                    oggettoCustom.VALORE_DATABASE = "1";
                                    oggettoCustom.VALORE_SOTTOCONTATORE = "1";
                                    oggettoCustom.DATA_INSERIMENTO = dbProvider.GetSysdate();
                                    oggettoCustom.ANNO = annoCorrente.ToString();
                                }
                                else
                                {
                                    //Incremento il valore del contatore e lo aggiorno
                                    //if (inserimentoAggiornamento || (oggettoCustom.CONTA_DOPO == "1" && oggettoCustom.CONTATORE_DA_FAR_SCATTARE))
                                    if (inserimentoAggiornamento || oggettoCustom.CONTATORE_DA_FAR_SCATTARE)
                                    {
                                        //Caso di un contatore con sottocontatore
                                        if (!string.IsNullOrEmpty(oggettoCustom.MODULO_SOTTOCONTATORE) && !oggettoCustom.MODULO_SOTTOCONTATORE.Equals("0"))
                                        {
                                            valoreSottocontatore++;
                                            if (valoreContatore == 0)
                                                valoreContatore++;
                                            //if (Convert.ToInt32(oggettoCustom.MODULO_SOTTOCONTATORE) == valoreSottocontatore)
                                            if ((valoreSottocontatore - 1) >= Convert.ToInt32(oggettoCustom.MODULO_SOTTOCONTATORE))  //SAB
                                            {
                                                valoreSottocontatore = 1;
                                                valoreContatore++;
                                            }
                                        }
                                        //Caso di un contatore normale
                                        else
                                        {
                                            valoreContatore++;
                                        }

                                        oggettoCustom.VALORE_DATABASE = valoreContatore.ToString();
                                        oggettoCustom.VALORE_SOTTOCONTATORE = valoreSottocontatore.ToString();

                                        DocsPaDB.Query_Utils.Utils utils = new DocsPaDB.Query_Utils.Utils();
                                        oggettoCustom.DATA_INSERIMENTO = utils.GetDBDate(true);
                                        oggettoCustom.ANNO = annoContatore.ToString();

                                        if (inserimentoAggiornamento)
                                        {
                                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONT_DOC");
                                            queryMng.setParam("idContatore", ds_datiContatore.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                                            queryMng.setParam("valoreContatore", valoreContatore.ToString());
                                            queryMng.setParam("valoreSottocontatore", valoreSottocontatore.ToString());
                                            //queryMng.setParam("anno", oggettoCustom.ANNO);
                                            queryMng.setParam("anno", annoContatore.ToString());
                                            commandText = queryMng.getSQL();
                                            System.Diagnostics.Debug.WriteLine("SQL - aggiornaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                            logger.Debug("SQL - aggiornaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                            dbProvider.ExecuteNonQuery(commandText);
                                        }
                                    }
                                    //Caso in cui non ci troviamo nella situazione di inserire un nuovo oggettoCustom perchè già esiste
                                    //e inoltre non dovendo far scattere il conta dopo, non si deve incementare nulla  a meno che il contatore non è stato inserito ora 
                                    //e quindi va inizializzato
                                    else
                                    {
                                        if (valoreContatore == 0)
                                            valoreContatore++;
                                        oggettoCustom.VALORE_DATABASE = valoreContatore.ToString();
                                    }
                                }
                            }
                            else
                            {
                                //Incremento il valore del contatore e lo aggiorno
                                //if (inserimentoAggiornamento || (oggettoCustom.CONTA_DOPO == "1" && oggettoCustom.CONTATORE_DA_FAR_SCATTARE))
                                if (inserimentoAggiornamento || oggettoCustom.CONTATORE_DA_FAR_SCATTARE)
                                {
                                    //Caso di un contatore con sottocontatore
                                    if (!string.IsNullOrEmpty(oggettoCustom.MODULO_SOTTOCONTATORE) && !oggettoCustom.MODULO_SOTTOCONTATORE.Equals("0"))
                                    {
                                        valoreSottocontatore++;
                                        if (valoreContatore == 0)
                                            valoreContatore++;
                                        //if (Convert.ToInt32(oggettoCustom.MODULO_SOTTOCONTATORE) == valoreSottocontatore)
                                        if ((valoreSottocontatore - 1) >= Convert.ToInt32(oggettoCustom.MODULO_SOTTOCONTATORE))  //SAB
                                        {
                                            valoreSottocontatore = 1;
                                            valoreContatore++;
                                        }
                                    }
                                    //Caso di un contatore normale
                                    else
                                    {
                                        valoreContatore++;
                                    }

                                    oggettoCustom.VALORE_DATABASE = valoreContatore.ToString();
                                    oggettoCustom.VALORE_SOTTOCONTATORE = valoreSottocontatore.ToString();
                                    //oggettoCustom.DATA_INSERIMENTO = dbProvider.GetSysdate();

                                    DocsPaDB.Query_Utils.Utils utils = new DocsPaDB.Query_Utils.Utils();
                                    oggettoCustom.DATA_INSERIMENTO = utils.GetDBDate(true);

                                    if (inserimentoAggiornamento)
                                    {
                                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONT_DOC");
                                        queryMng.setParam("idContatore", ds_datiContatore.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                                        queryMng.setParam("valoreContatore", valoreContatore.ToString());
                                        queryMng.setParam("valoreSottocontatore", valoreSottocontatore.ToString());
                                        queryMng.setParam("anno", oggettoCustom.ANNO);
                                        commandText = queryMng.getSQL();
                                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                        logger.Debug("SQL - aggiornaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                        dbProvider.ExecuteNonQuery(commandText);
                                    }
                                }
                                //Caso in cui non ci troviamo nella situazione di inserire un nuovo oggettoCustom perchè già esiste
                                //e inoltre non dovendo far scattere il conta dopo, non si deve incementare nulla  a meno che il contatore non è stato inserito ora 
                                //e quindi va inizializzato
                                else
                                {
                                    if (valoreContatore == 0)
                                        valoreContatore++;
                                    oggettoCustom.VALORE_DATABASE = valoreContatore.ToString();
                                }
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(oggettoCustom.DATA_INIZIO) && !string.IsNullOrEmpty(oggettoCustom.DATA_FINE))
            {
                string AnnoAccademico = oggettoCustom.DATA_INIZIO.ToString().Substring(6, 4) + oggettoCustom.DATA_FINE.ToString().Substring(5, 5);
                oggettoCustom.ANNO_ACC = AnnoAccademico;
            }
        }

        private void calcolaContatoreComune(DocsPaVO.ProfilazioneDinamica.Templates template, ref DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, bool inserimentoAggiornamento)
        {

            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            oggettoCustom.VALORE_DATABASE = string.Empty;
            DocsPaUtils.Query queryMng;
            string commandText = string.Empty;
            //controllo se esiste un record nella DPA_CONT_CUSTOM_DOC associato
            //al system id dell'oggetto custom corrente
            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CONT_CUSTOM_DOC_BY_IDOGG");
            queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());
            commandText = queryMng.getSQL();
            System.Diagnostics.Debug.WriteLine("SQL - cerca contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
            logger.Debug("SQL - cerca contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
            DataSet ds_datiContatoreCustom = new DataSet(); ;
            dbProvider.ExecuteQuery(ds_datiContatoreCustom, commandText);
            if (ds_datiContatoreCustom.Tables[0].Rows.Count != 0)
            {
                if (oggettoCustom.CONTATORE_DA_FAR_SCATTARE)
                {
                    {
                        //Verifico che il contatore esiste, se si lo recupero altrimenti lo inserisco e ne recupero i dati
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_EXIST_CONT_DOC_COMUNE");
                        queryMng.setParam("idOggetto", oggettoCustom.SYSTEM_ID.ToString());
                        //queryMng.setParam("idTipologia", template.SYSTEM_ID.ToString());

                        switch (oggettoCustom.TIPO_CONTATORE)
                        {
                            case "T":
                                queryMng.setParam("param1", "");
                                break;

                            case "A":
                                queryMng.setParam("param1", " AND ID_AOO = " + oggettoCustom.ID_AOO_RF.ToString());
                                break;

                            case "R":
                                queryMng.setParam("param1", " AND ID_RF =  " + oggettoCustom.ID_AOO_RF.ToString());
                                break;

                            default:
                                queryMng.setParam("param1", "");
                                break;
                        }
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - cercaContatoreComune - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - cercaContatoreComune - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        DataSet ds_datiContatore = new DataSet();
                        dbProvider.ExecuteQuery(ds_datiContatore, commandText);

                        //Contatore non esistente 
                        if (ds_datiContatore.Tables[0].Rows.Count == 0)
                        {
                            //Inserisco il contatore
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INSERT_CONT_DOC");
                            queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                            queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONTATORI_DOC"));
                            queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());
                            queryMng.setParam("idTipologia", "0");
                            /*
                            if (oggettoCustom.CAMPO_COMUNE.Equals("1"))
                            {
                                queryMng.setParam("idTipologia", "0");
                            }
                            else
                            {
                                queryMng.setParam("idTipologia", template.SYSTEM_ID.ToString());
                            }
                             * */
                            switch (oggettoCustom.TIPO_CONTATORE)
                            {
                                case "T":
                                    queryMng.setParam("idAoo", "0");
                                    queryMng.setParam("idRf", "0");
                                    break;

                                case "A":
                                    queryMng.setParam("idAoo", oggettoCustom.ID_AOO_RF);
                                    queryMng.setParam("idRf", "0");
                                    break;

                                case "R":
                                    queryMng.setParam("idAoo", "0");
                                    queryMng.setParam("idRf", oggettoCustom.ID_AOO_RF);
                                    break;

                                default:
                                    queryMng.setParam("idAoo", "0");
                                    queryMng.setParam("idRf", "0");
                                    break;
                            }
                            queryMng.setParam("valore", "0");
                            queryMng.setParam("valoreSottocontatore", "0");
                            queryMng.setParam("abilitato", "1");
                            queryMng.setParam("anno", oggettoCustom.ANNO);
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - inserisciContatoreComune - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - inserisciContatoreComune - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);

                            //Reperimento systemid
                            commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_CONTATORI_DOC");
                            string idContatoreInserito = string.Empty;
                            dbProvider.ExecuteScalar(out idContatoreInserito, commandText);

                            //Recupero i dati del contatore
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_EXIST_CONT_DOC_COMUNE");
                            queryMng.setParam("idOggetto", oggettoCustom.SYSTEM_ID.ToString());
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - cercaContatoreComune - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - cercaContatoreComune - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            dbProvider.ExecuteQuery(ds_datiContatore, commandText);



                        }

                        if (ds_datiContatore.Tables[0].Rows.Count != 0)
                        {
                            int annoContatore = 0;
                            int annoOggetto = 0;
                            int valoreContatore = 0;
                            int valoreSottocontatore = 0;
                            string valoreDataInizio = string.Empty;
                            string valoreDataFine = string.Empty;
                            int annoCorrente = System.DateTime.Now.Year;
                            if (ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString() != null && ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString() != "")
                                annoContatore = Convert.ToInt32(ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString());
                            if (oggettoCustom.ANNO != null && oggettoCustom.ANNO != "")
                                annoOggetto = Convert.ToInt32(oggettoCustom.ANNO);
                            if (ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString() != null && ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString() != "")
                                valoreContatore = Convert.ToInt32(ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString());
                            if (ds_datiContatore.Tables[0].Columns.Contains("VALORE_SC") && !string.IsNullOrEmpty(ds_datiContatore.Tables[0].Rows[0]["VALORE_SC"].ToString()))
                                valoreSottocontatore = Convert.ToInt32(ds_datiContatore.Tables[0].Rows[0]["VALORE_SC"].ToString());
                            if (ds_datiContatoreCustom.Tables[0].Columns.Contains("DATA_INIZIO") && !string.IsNullOrEmpty(ds_datiContatoreCustom.Tables[0].Rows[0]["DATA_INIZIO"].ToString()))
                                valoreDataInizio = Convert.ToDateTime(ds_datiContatoreCustom.Tables[0].Rows[0]["DATA_INIZIO"]).Year.ToString();
                            if (ds_datiContatoreCustom.Tables[0].Columns.Contains("DATA_FINE") && !string.IsNullOrEmpty(ds_datiContatoreCustom.Tables[0].Rows[0]["DATA_FINE"].ToString()))
                                valoreDataFine = Convert.ToDateTime(ds_datiContatoreCustom.Tables[0].Rows[0]["DATA_FINE"]).Year.ToString();



                            //Incremento il valore del contatore e lo aggiorno
                            //if (inserimentoAggiornamento || (oggettoCustom.CONTA_DOPO == "1" && oggettoCustom.CONTATORE_DA_FAR_SCATTARE))
                            if (inserimentoAggiornamento || oggettoCustom.CONTATORE_DA_FAR_SCATTARE)
                            {
                                //Caso di un contatore con sottocontatore
                                if (!string.IsNullOrEmpty(oggettoCustom.MODULO_SOTTOCONTATORE) && !oggettoCustom.MODULO_SOTTOCONTATORE.Equals("0"))
                                {
                                    valoreSottocontatore++;
                                    if (valoreContatore == 0)
                                        valoreContatore++;
                                    //if (Convert.ToInt32(oggettoCustom.MODULO_SOTTOCONTATORE) == valoreSottocontatore)
                                    if ((valoreSottocontatore - 1) >= Convert.ToInt32(oggettoCustom.MODULO_SOTTOCONTATORE))  //SAB
                                    {
                                        valoreSottocontatore = 1;
                                        valoreContatore++;
                                    }
                                }
                                //Caso di un contatore normale
                                else
                                {
                                    valoreContatore++;
                                }

                                oggettoCustom.VALORE_DATABASE = valoreContatore.ToString();
                                oggettoCustom.VALORE_SOTTOCONTATORE = valoreSottocontatore.ToString();
                                oggettoCustom.DATA_INSERIMENTO = dbProvider.GetSysdate();

                                if (inserimentoAggiornamento)
                                {
                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONT_DOC_COMUNE");
                                    queryMng.setParam("idOggContatore", oggettoCustom.SYSTEM_ID.ToString());
                                    queryMng.setParam("valoreContatore", valoreContatore.ToString());
                                    queryMng.setParam("valoreSottocontatore", valoreSottocontatore.ToString());
                                    queryMng.setParam("anno", oggettoCustom.ANNO);
                                    commandText = queryMng.getSQL();
                                    System.Diagnostics.Debug.WriteLine("SQL - aggiornaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                    logger.Debug("SQL - aggiornaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                    dbProvider.ExecuteNonQuery(commandText);
                                }
                            }
                            //Caso in cui non ci troviamo nella situazione di inserire un nuovo oggettoCustom perchè già esiste
                            //e inoltre non dovendo far scattere il conta dopo, non si deve incementare nulla  a meno che il contatore non è stato inserito ora 
                            //e quindi va inizializzato
                            else
                            {
                                if (valoreContatore == 0)
                                    valoreContatore++;
                                oggettoCustom.VALORE_DATABASE = valoreContatore.ToString();
                            }
                        }
                    }



                }

            }
            else
            {
                if (ds_datiContatoreCustom.Tables[0].Rows.Count == 0)
                {   //Verifico se deve scattare il contatore
                    //OggettoCustom.CONTA_DOPO = 1 AND OggettoCustom.CONTATORE_DA_FAR_SCATTARE = true - SI 
                    //OggettoCustom.CONTA_DOPO != 1 - SI 
                    //In tutti gli altri casi il contatore non deve scattare
                    //ma va comunque inserita una riga nella DPA_ASSOCIAZIONE_TEMPLATES con valore vuoto
                    //perchè posso decidere di far scattare il contatore in momenti diversi

                    //if ((oggettoCustom.CONTA_DOPO == "1" && oggettoCustom.CONTATORE_DA_FAR_SCATTARE) || oggettoCustom.CONTA_DOPO != "1")
                    if (oggettoCustom.CONTATORE_DA_FAR_SCATTARE)
                    {
                        //Verifico che il contatore esiste, se si lo recupero altrimenti lo inserisco e ne recupero i dati
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_EXIST_CONT_DOC_COMUNE");
                        queryMng.setParam("idOggetto", oggettoCustom.SYSTEM_ID.ToString());
                        //queryMng.setParam("idTipologia", template.SYSTEM_ID.ToString());
                        /*
                        if (oggettoCustom.CAMPO_COMUNE.Equals("1"))
                        {
                            queryMng.setParam("idOggetto", oggettoCustom.SYSTEM_ID.ToString());
                            queryMng.setParam("idTipologia", " ");
                        }
                        else
                        {
                            queryMng.setParam("idOggetto", oggettoCustom.SYSTEM_ID.ToString());
                            queryMng.setParam("idTipologia", " AND ID_TIPOLOGIA = " + template.SYSTEM_ID.ToString() + " ");
                        }
                         */
                        switch (oggettoCustom.TIPO_CONTATORE)
                        {
                            case "T":
                                queryMng.setParam("param1", "");
                                break;

                            case "A":
                                queryMng.setParam("param1", " AND ID_AOO = " + oggettoCustom.ID_AOO_RF.ToString());
                                break;

                            case "R":
                                queryMng.setParam("param1", " AND ID_RF =  " + oggettoCustom.ID_AOO_RF.ToString());
                                break;

                            default:
                                queryMng.setParam("param1", "");
                                break;
                        }
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - cercaContatoreComune - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - cercaContatoreComune - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        DataSet ds_datiContatore = new DataSet();
                        dbProvider.ExecuteQuery(ds_datiContatore, commandText);

                        //Contatore non esistente 
                        if (ds_datiContatore.Tables[0].Rows.Count == 0)
                        {
                            //Inserisco il contatore
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INSERT_CONT_DOC");
                            queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                            queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONTATORI_DOC"));
                            queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());
                            queryMng.setParam("idTipologia", "0");
                            /*
                            if (oggettoCustom.CAMPO_COMUNE.Equals("1"))
                            {
                                queryMng.setParam("idTipologia", "0");
                            }
                            else
                            {
                                queryMng.setParam("idTipologia", template.SYSTEM_ID.ToString());
                            }
                             * */
                            switch (oggettoCustom.TIPO_CONTATORE)
                            {
                                case "T":
                                    queryMng.setParam("idAoo", "0");
                                    queryMng.setParam("idRf", "0");
                                    break;

                                case "A":
                                    queryMng.setParam("idAoo", oggettoCustom.ID_AOO_RF);
                                    queryMng.setParam("idRf", "0");
                                    break;

                                case "R":
                                    queryMng.setParam("idAoo", "0");
                                    queryMng.setParam("idRf", oggettoCustom.ID_AOO_RF);
                                    break;

                                default:
                                    queryMng.setParam("idAoo", "0");
                                    queryMng.setParam("idRf", "0");
                                    break;
                            }
                            queryMng.setParam("valore", "0");
                            queryMng.setParam("valoreSottocontatore", "0");
                            queryMng.setParam("abilitato", "1");
                            queryMng.setParam("anno", oggettoCustom.ANNO);
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - inserisciContatoreComune - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - inserisciContatoreComune - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);

                            //Reperimento systemid
                            commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_CONTATORI_DOC");
                            string idContatoreInserito = string.Empty;
                            dbProvider.ExecuteScalar(out idContatoreInserito, commandText);

                            //Recupero i dati del contatore
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_EXIST_CONT_DOC_COMUNE");
                            queryMng.setParam("idOggetto", oggettoCustom.SYSTEM_ID.ToString());
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - cercaContatoreComune - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - cercaContatoreComune - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            dbProvider.ExecuteQuery(ds_datiContatore, commandText);
                        }

                        if (ds_datiContatore.Tables[0].Rows.Count != 0)
                        {
                            int annoContatore = 0;
                            int annoOggetto = 0;
                            int valoreContatore = 0;
                            int valoreSottocontatore = 0;
                            int annoCorrente = System.DateTime.Now.Year;
                            if (ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString() != null && ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString() != "")
                                annoContatore = Convert.ToInt32(ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString());
                            if (oggettoCustom.ANNO != null && oggettoCustom.ANNO != "")
                                annoOggetto = Convert.ToInt32(oggettoCustom.ANNO);
                            if (ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString() != null && ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString() != "")
                                valoreContatore = Convert.ToInt32(ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString());
                            if (ds_datiContatore.Tables[0].Columns.Contains("VALORE_SC") && !string.IsNullOrEmpty(ds_datiContatore.Tables[0].Rows[0]["VALORE_SC"].ToString()))
                                valoreSottocontatore = Convert.ToInt32(ds_datiContatore.Tables[0].Rows[0]["VALORE_SC"].ToString());

                            if (oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI")
                            {
                                //if (annoContatore < annoOggetto)
                                if (annoContatore < annoCorrente)
                                {
                                    //Reinizializzo il contatore
                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONT_DOC");
                                    queryMng.setParam("idContatore", ds_datiContatore.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                                    queryMng.setParam("valoreContatore", "1");
                                    queryMng.setParam("valoreSottocontatore", "1");
                                    //queryMng.setParam("anno", oggettoCustom.ANNO);
                                    queryMng.setParam("anno", annoCorrente.ToString());
                                    commandText = queryMng.getSQL();
                                    System.Diagnostics.Debug.WriteLine("SQL - aggiornaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                    logger.Debug("SQL - aggiornaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                    dbProvider.ExecuteNonQuery(commandText);
                                    oggettoCustom.VALORE_DATABASE = "1";
                                    oggettoCustom.VALORE_SOTTOCONTATORE = "1";
                                    oggettoCustom.DATA_INSERIMENTO = dbProvider.GetSysdate();
                                    oggettoCustom.ANNO = annoCorrente.ToString();
                                }
                                else
                                {
                                    //Incremento il valore del contatore e lo aggiorno
                                    //if (inserimentoAggiornamento || (oggettoCustom.CONTA_DOPO == "1" && oggettoCustom.CONTATORE_DA_FAR_SCATTARE))
                                    if (inserimentoAggiornamento || oggettoCustom.CONTATORE_DA_FAR_SCATTARE)
                                    {
                                        //Caso di un contatore con sottocontatore
                                        if (!string.IsNullOrEmpty(oggettoCustom.MODULO_SOTTOCONTATORE) && !oggettoCustom.MODULO_SOTTOCONTATORE.Equals("0"))
                                        {
                                            valoreSottocontatore++;
                                            if (valoreContatore == 0)
                                                valoreContatore++;
                                            //if (Convert.ToInt32(oggettoCustom.MODULO_SOTTOCONTATORE) == valoreSottocontatore)
                                            if ((valoreSottocontatore - 1) >= Convert.ToInt32(oggettoCustom.MODULO_SOTTOCONTATORE))  //SAB
                                            {
                                                valoreSottocontatore = 1;
                                                valoreContatore++;
                                            }
                                        }
                                        //Caso di un contatore normale
                                        else
                                        {
                                            valoreContatore++;
                                        }

                                        oggettoCustom.VALORE_DATABASE = valoreContatore.ToString();
                                        oggettoCustom.VALORE_SOTTOCONTATORE = valoreSottocontatore.ToString();
                                        oggettoCustom.DATA_INSERIMENTO = dbProvider.GetSysdate();
                                        oggettoCustom.ANNO = annoContatore.ToString();

                                        if (inserimentoAggiornamento)
                                        {
                                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONT_DOC");
                                            queryMng.setParam("idContatore", ds_datiContatore.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                                            queryMng.setParam("valoreContatore", valoreContatore.ToString());
                                            queryMng.setParam("valoreSottocontatore", valoreSottocontatore.ToString());
                                            //queryMng.setParam("anno", oggettoCustom.ANNO);
                                            queryMng.setParam("anno", annoContatore.ToString());
                                            commandText = queryMng.getSQL();
                                            System.Diagnostics.Debug.WriteLine("SQL - aggiornaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                            logger.Debug("SQL - aggiornaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                            dbProvider.ExecuteNonQuery(commandText);
                                        }
                                    }
                                    //Caso in cui non ci troviamo nella situazione di inserire un nuovo oggettoCustom perchè già esiste
                                    //e inoltre non dovendo far scattere il conta dopo, non si deve incementare nulla  a meno che il contatore non è stato inserito ora 
                                    //e quindi va inizializzato
                                    else
                                    {
                                        if (valoreContatore == 0)
                                            valoreContatore++;
                                        oggettoCustom.VALORE_DATABASE = valoreContatore.ToString();
                                    }
                                }
                            }
                            else
                            {
                                //Incremento il valore del contatore e lo aggiorno
                                //if (inserimentoAggiornamento || (oggettoCustom.CONTA_DOPO == "1" && oggettoCustom.CONTATORE_DA_FAR_SCATTARE))
                                if (inserimentoAggiornamento || oggettoCustom.CONTATORE_DA_FAR_SCATTARE)
                                {
                                    //Caso di un contatore con sottocontatore
                                    if (!string.IsNullOrEmpty(oggettoCustom.MODULO_SOTTOCONTATORE) && !oggettoCustom.MODULO_SOTTOCONTATORE.Equals("0"))
                                    {
                                        valoreSottocontatore++;
                                        if (valoreContatore == 0)
                                            valoreContatore++;
                                        //if (Convert.ToInt32(oggettoCustom.MODULO_SOTTOCONTATORE) == valoreSottocontatore)
                                        if ((valoreSottocontatore - 1) >= Convert.ToInt32(oggettoCustom.MODULO_SOTTOCONTATORE))  //SAB
                                        {
                                            valoreSottocontatore = 1;
                                            valoreContatore++;
                                        }
                                    }
                                    //Caso di un contatore normale
                                    else
                                    {
                                        valoreContatore++;
                                    }

                                    oggettoCustom.VALORE_DATABASE = valoreContatore.ToString();
                                    oggettoCustom.VALORE_SOTTOCONTATORE = valoreSottocontatore.ToString();
                                    oggettoCustom.DATA_INSERIMENTO = dbProvider.GetSysdate();

                                    if (inserimentoAggiornamento)
                                    {
                                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONT_DOC_COMUNE");
                                        queryMng.setParam("idOggContatore", oggettoCustom.SYSTEM_ID.ToString());
                                        queryMng.setParam("valoreContatore", valoreContatore.ToString());
                                        queryMng.setParam("valoreSottocontatore", valoreSottocontatore.ToString());
                                        queryMng.setParam("anno", oggettoCustom.ANNO);
                                        commandText = queryMng.getSQL();
                                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                        logger.Debug("SQL - aggiornaContatore - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                        dbProvider.ExecuteNonQuery(commandText);
                                    }
                                }
                                //Caso in cui non ci troviamo nella situazione di inserire un nuovo oggettoCustom perchè già esiste
                                //e inoltre non dovendo far scattere il conta dopo, non si deve incementare nulla  a meno che il contatore non è stato inserito ora 
                                //e quindi va inizializzato
                                else
                                {
                                    if (valoreContatore == 0)
                                        valoreContatore++;
                                    oggettoCustom.VALORE_DATABASE = valoreContatore.ToString();
                                }
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(oggettoCustom.DATA_INIZIO) && !string.IsNullOrEmpty(oggettoCustom.DATA_FINE))
            {
                string AnnoAccademico = oggettoCustom.DATA_INIZIO.ToString().Substring(6, 4) + oggettoCustom.DATA_FINE.ToString().Substring(5, 5);
                oggettoCustom.ANNO_ACC = AnnoAccademico;
            }
        }


        public bool isDocRepertoriato(string docNumber, string idTipoAtto)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_IS_DOC_REPERTORIATO");
                queryMng.setParam("docNumber", docNumber);
                queryMng.setParam("idTipoAtto", idTipoAtto);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - isDocRepertoriato - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - isDocRepertoriato - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0].Rows.Count != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        protected string getOperatoreQueryOggettoCustomStringa(DocsPaVO.ProfilazioneDinamica.TipoRicercaStringaEnum tipoRicerca)
        {
            if (tipoRicerca == DocsPaVO.ProfilazioneDinamica.TipoRicercaStringaEnum.PARTE_DELLA_PAROLA ||
                tipoRicerca == DocsPaVO.ProfilazioneDinamica.TipoRicercaStringaEnum.PAROLA_INIZIA_CON)
                return "LIKE";
            else if (tipoRicerca == DocsPaVO.ProfilazioneDinamica.TipoRicercaStringaEnum.PAROLA_INTERA)
                return "=";
            else
                return null;
        }

        protected string getValoreQueryOggettoCustomStringa(DocsPaVO.ProfilazioneDinamica.TipoRicercaStringaEnum tipoRicerca, string valore)
        {
            string retValue = string.Empty;

            valore = valore.Replace("'", "''");

            if (tipoRicerca == DocsPaVO.ProfilazioneDinamica.TipoRicercaStringaEnum.PARTE_DELLA_PAROLA)
                retValue = string.Format("%{0}%", valore);
            else if (tipoRicerca == DocsPaVO.ProfilazioneDinamica.TipoRicercaStringaEnum.PAROLA_INTERA)
                retValue = valore;
            else if (tipoRicerca == DocsPaVO.ProfilazioneDinamica.TipoRicercaStringaEnum.PAROLA_INIZIA_CON)
                retValue = string.Format("{0}%", valore);

            return retValue;
        }

        
        public string GetIdCorrespondentForTemplate(DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            string corrId = string.Empty;
            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto in template.ELENCO_OGGETTI)
            {
                if(oggetto.TIPO.DESCRIZIONE_TIPO.Equals("Corrispondente"))
                {
                    corrId = oggetto.VALORE_DATABASE.ToString();
                    break;
                }
            }
            return corrId;
        }
         

        public string getSeriePerRicercaProfilazione(DocsPaVO.ProfilazioneDinamica.Templates template, string anno_prof)
        {
            string filterString = string.Empty;

            if (template != null && template.ELENCO_OGGETTI != null)
            {
                //Imposto il filtro per la ricercare solo sui campi profilati che sono campi di ricerca
                //filterString += "A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0, dpa_oggetti_custom dpa1 where dpa0.id_oggetto =  dpa1.system_id and dpa1.campo_di_ricerca = 'SI') ";

                bool firstFilter = true;

                if (template.ELENCO_OGGETTI.Count == 0)
                {
                    filterString += "A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where dpa0.id_template = " + template.ID_TIPO_ATTO + ")";
                }
                else
                {
                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto in template.ELENCO_OGGETTI)
                    {
                        string operatoreRicerca = string.Empty;

                        if (!firstFilter && !string.IsNullOrEmpty(filterString))
                            operatoreRicerca = template.OPERATORE_RICERCA_OGGETTI.ToString();

                        switch (oggetto.TIPO.DESCRIZIONE_TIPO)
                        {
                            case "CampoDiTesto":
                                if (oggetto.VALORE_DATABASE != null && oggetto.VALORE_DATABASE != "")
                                {
                                    filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                    if (!string.IsNullOrEmpty(anno_prof))
                                        filterString += " AND dpa0.ANNO = " + anno_prof;
                                    filterString += " AND UPPER(dpa0.valore_oggetto_db) " + this.getOperatoreQueryOggettoCustomStringa(oggetto.TIPO_RICERCA_STRINGA) + " UPPER('" + this.getValoreQueryOggettoCustomStringa(oggetto.TIPO_RICERCA_STRINGA, oggetto.VALORE_DATABASE) + "') ) ) ";
                                }
                                break;

                            case "CasellaDiSelezione":
                                for (int k = 0; k < oggetto.VALORI_SELEZIONATI.Count; k++)
                                {
                                    if (oggetto.VALORI_SELEZIONATI[k] != null && (string)oggetto.VALORI_SELEZIONATI[k] != "")
                                    {
                                        filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                        if (!string.IsNullOrEmpty(anno_prof))
                                            filterString += " AND dpa0.ANNO = " + anno_prof;
                                        filterString += " AND UPPER(dpa0.valore_oggetto_db) = UPPER('" + oggetto.VALORI_SELEZIONATI[k].ToString().Replace("'", "''") + "')) ) ";
                                        operatoreRicerca = template.OPERATORE_RICERCA_OGGETTI.ToString().ToLowerInvariant();
                                        firstFilter = false;
                                    }
                                }
                                break;

                            case "MenuATendina":
                                if (oggetto.VALORE_DATABASE != null && oggetto.VALORE_DATABASE != "")
                                {
                                    filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                    if (!string.IsNullOrEmpty(anno_prof))
                                        filterString += " AND dpa0.ANNO = " + anno_prof;
                                    filterString += " AND UPPER(dpa0.valore_oggetto_db) = UPPER('" + oggetto.VALORE_DATABASE.Replace("'", "''") + "')) ) ";
                                }
                                break;

                            case "SelezioneEsclusiva":
                                if (oggetto.VALORE_DATABASE != null && oggetto.VALORE_DATABASE != "")
                                {
                                    filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                    if (!string.IsNullOrEmpty(anno_prof))
                                        filterString += " AND dpa0.ANNO = " + anno_prof;
                                    filterString += " AND UPPER(dpa0.valore_oggetto_db) = UPPER('" + oggetto.VALORE_DATABASE.Replace("'", "''") + "')) ) ";
                                }
                                break;

                            case "Contatore":
                                string filterDataRepertorio = string.Empty;
                                if(!string.IsNullOrEmpty(oggetto.DATA_INSERIMENTO))
                                {
                                    if (oggetto.DATA_INSERIMENTO.IndexOf('@') != -1)
                                    {
                                        string[] dataInserimento = oggetto.DATA_INSERIMENTO.Split('@');
                                        filterDataRepertorio += " AND dpa0.DTA_INS >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(dataInserimento[0], true) + " AND dpa0.DTA_INS <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(dataInserimento[1], false);
                                    }
                                    else 
                                    {
                                        filterDataRepertorio += " AND dpa0.DTA_INS >=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(oggetto.DATA_INSERIMENTO, true) + " AND dpa0.DTA_INS <=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(oggetto.DATA_INSERIMENTO, false);
                                    }
                                }
                                switch (oggetto.TIPO_CONTATORE)
                                {
                                    case "T":
                                        if (!oggetto.VALORE_DATABASE.Equals(""))
                                        {
                                            if (oggetto.VALORE_DATABASE.IndexOf('@') != -1)
                                            {
                                                string[] contatore = oggetto.VALORE_DATABASE.Split('@');
                                                filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                                if (!string.IsNullOrEmpty(anno_prof))
                                                    filterString += " AND dpa0.ANNO = " + anno_prof;
                                                if (!string.IsNullOrEmpty(filterDataRepertorio))
                                                    filterString += filterDataRepertorio;
                                                filterString += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[1], false) + " ) )";
                                            }
                                            else
                                            {
                                                filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                                if (!string.IsNullOrEmpty(anno_prof))
                                                    filterString += " AND dpa0.ANNO = " + anno_prof;
                                                if (!string.IsNullOrEmpty(filterDataRepertorio))
                                                    filterString += filterDataRepertorio;
                                                filterString += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_DATABASE, false) + " ) )";
                                            }
                                        }
                                        else if(!string.IsNullOrEmpty(filterDataRepertorio))
                                        {
                                            filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + filterDataRepertorio + " ) )";
                                        }
                                        break;

                                    //O è di tipo "A" o di tipo "R"
                                    default:
                                        //if (oggetto.ID_AOO_RF != null && oggetto.ID_AOO_RF != "")
                                        if (!string.IsNullOrEmpty(oggetto.ID_AOO_RF) && oggetto.ID_AOO_RF != "0")
                                        {
                                            if (!oggetto.VALORE_DATABASE.Equals(""))
                                            {
                                                //Nr.Contatore SI - Aoo/Rf SI
                                                if (oggetto.VALORE_DATABASE.IndexOf('@') != -1)
                                                {
                                                    string[] contatore = oggetto.VALORE_DATABASE.Split('@');
                                                    filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                                    if (!string.IsNullOrEmpty(anno_prof))
                                                        filterString += " AND dpa0.ANNO = " + anno_prof;
                                                    if (!string.IsNullOrEmpty(filterDataRepertorio))
                                                        filterString += filterDataRepertorio;
                                                    filterString += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[1], false) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ) )";
                                                }
                                                else
                                                {
                                                    filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                                    if (!string.IsNullOrEmpty(anno_prof))
                                                        filterString += " AND dpa0.ANNO = " + anno_prof;
                                                    if (!string.IsNullOrEmpty(filterDataRepertorio))
                                                        filterString += filterDataRepertorio;
                                                    filterString += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_DATABASE, false) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ) )";
                                                }
                                            }
                                            else
                                            {
                                                //Nr.Contatore NO - Aoo/Rf SI
                                                filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                                if (!string.IsNullOrEmpty(anno_prof))
                                                    filterString += " AND dpa0.ANNO = " + anno_prof;
                                                if (!string.IsNullOrEmpty(filterDataRepertorio))
                                                    filterString += filterDataRepertorio;
                                                filterString += " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ) )";
                                            }
                                        }
                                        else
                                        {
                                            if (!oggetto.VALORE_DATABASE.Equals(""))
                                            {
                                                //Nr.Contatore SI - Aoo/Rf NO
                                                if (oggetto.VALORE_DATABASE.IndexOf('@') != -1)
                                                {
                                                    string[] contatore = oggetto.VALORE_DATABASE.Split('@');
                                                    filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                                    if (!string.IsNullOrEmpty(anno_prof))
                                                        filterString += " AND dpa0.ANNO = " + anno_prof;
                                                    if (!string.IsNullOrEmpty(filterDataRepertorio))
                                                        filterString += filterDataRepertorio;
                                                    filterString += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[1], false) + " ) )";
                                                }
                                                else
                                                {
                                                    filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                                    if (!string.IsNullOrEmpty(anno_prof))
                                                        filterString += " AND dpa0.ANNO = " + anno_prof;
                                                    if (!string.IsNullOrEmpty(filterDataRepertorio))
                                                        filterString += filterDataRepertorio;
                                                    filterString += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_DATABASE, false) + " ) )";
                                                }
                                            }
                                            else if (!string.IsNullOrEmpty(filterDataRepertorio))
                                            {
                                                filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + filterDataRepertorio + " ) )";
                                            }
                                        }
                                        break;
                                }
                                break;

                            case "Data":
                                if (oggetto.VALORE_DATABASE != null && oggetto.VALORE_DATABASE != "")
                                {
                                    if (oggetto.VALORE_DATABASE.IndexOf('@') != -1)
                                    {
                                        string[] date = oggetto.VALORE_DATABASE.Split('@');
                                        filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                        if (!string.IsNullOrEmpty(anno_prof))
                                            filterString += " AND dpa0.ANNO = " + anno_prof;
                                        filterString += " AND " + DocsPaDbManagement.Functions.Functions.ToDateColumn("dpa0.valore_oggetto_db") + " >= " + DocsPaDbManagement.Functions.Functions.ToDate(date[0]) + " AND " + DocsPaDbManagement.Functions.Functions.ToDateColumn("dpa0.valore_oggetto_db") + " <= " + DocsPaDbManagement.Functions.Functions.ToDate(date[1]) + " ) ) ";
                                    }
                                    else if (oggetto.VALORE_DATABASE.IndexOf('|') != -1)
                                    {
                                        // Ricerca per data con la possibilità di passare l'operatore di ricerca
                                        string[] pair = oggetto.VALORE_DATABASE.Split('|');

                                        string op = pair[0];
                                        string value = pair[1];

                                        filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                        if (!string.IsNullOrEmpty(anno_prof))
                                            filterString += " AND dpa0.ANNO = " + anno_prof;
                                        filterString += " AND " + DocsPaDbManagement.Functions.Functions.ToDateColumn("dpa0.valore_oggetto_db") + " " + op + " " + DocsPaDbManagement.Functions.Functions.ToDate(value) + " ) ) ";
                                    }
                                    else
                                    {
                                        filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                        if (!string.IsNullOrEmpty(anno_prof))
                                            filterString += " AND dpa0.ANNO = " + anno_prof;
                                        filterString += " AND " + DocsPaDbManagement.Functions.Functions.ToDateColumn("dpa0.valore_oggetto_db") + " >= " + DocsPaDbManagement.Functions.Functions.ToDate(oggetto.VALORE_DATABASE) + " ) ) ";
                                    }
                                }
                                break;

                            case "Corrispondente":
                                if (oggetto.VALORE_DATABASE != null && oggetto.VALORE_DATABASE != "")
                                {
                                    int codiceCorrispondente;
                                    if (Int32.TryParse(oggetto.VALORE_DATABASE, out codiceCorrispondente))
                                    {
                                        filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                        if (!string.IsNullOrEmpty(anno_prof))
                                            filterString += " AND dpa0.ANNO = " + anno_prof;
                                        if (oggetto.ESTENDI_STORICIZZATI)
                                        {
                                            if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "SQL")
                                            {
                                                filterString += " AND upper(dpa0.valore_oggetto_db) in( select upper(SYSTEM_ID) from DPA_CORR_GLOBALI where SYSTEM_ID IN (SELECT system_id FROM N))))";
                                            }
                                            else
                                            { 
                                                filterString += " AND upper(dpa0.valore_oggetto_db) in( select upper(SYSTEM_ID) from DPA_CORR_GLOBALI where SYSTEM_ID IN (SELECT system_id FROM dpa_corr_globali START WITH system_id = " + codiceCorrispondente + " CONNECT BY PRIOR id_old = system_id))))";
                                            }
                                        }
                                        else
                                        {
                                            filterString += " AND upper(dpa0.valore_oggetto_db) in( select upper(SYSTEM_ID) from DPA_CORR_GLOBALI where SYSTEM_ID = " + codiceCorrispondente + ")))";
                                        }
                                    }
                                    else
                                    {
                                        filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                        if (!string.IsNullOrEmpty(anno_prof))
                                            filterString += " AND dpa0.ANNO = " + anno_prof;
                                        filterString += " AND upper(dpa0.valore_oggetto_db) in( select SYSTEM_ID from DPA_CORR_GLOBALI where UPPER(VAR_DESC_CORR) " + this.getOperatoreQueryOggettoCustomStringa(oggetto.TIPO_RICERCA_STRINGA) + " UPPER('" + this.getValoreQueryOggettoCustomStringa(oggetto.TIPO_RICERCA_STRINGA, oggetto.VALORE_DATABASE) + "') )))";
                                    }
                                }
                                break;
                            case "ContatoreSottocontatore":
                                string filterSottocontatore = operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " ";
                                if (!string.IsNullOrEmpty(anno_prof))
                                    filterSottocontatore += "AND dpa0.ANNO = " + anno_prof + " ";
                                bool addCondition = false;
                                switch (oggetto.TIPO_CONTATORE)
                                {
                                    case "T":
                                        //Contatore
                                        if (!oggetto.VALORE_DATABASE.Equals(""))
                                        {
                                            if (oggetto.VALORE_DATABASE.IndexOf('@') != -1)
                                            {
                                                string[] contatore = oggetto.VALORE_DATABASE.Split('@');
                                                //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[1], false) + " ) )";
                                                filterSottocontatore += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[1], false) + " ";
                                                addCondition = true;
                                            }
                                            else
                                            {
                                                //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_DATABASE, false) + " ) )";
                                                filterSottocontatore += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_DATABASE, false) + " ";
                                                addCondition = true;
                                            }
                                        }
                                        //Sottocontatore
                                        if (!oggetto.VALORE_SOTTOCONTATORE.Equals(""))
                                        {
                                            if (oggetto.VALORE_SOTTOCONTATORE.IndexOf('@') != -1)
                                            {
                                                string[] sottocontatore = oggetto.VALORE_SOTTOCONTATORE.Split('@');
                                                //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(sottocontatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(sottocontatore[1], false) + " ) )";
                                                filterSottocontatore += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(sottocontatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(sottocontatore[1], false) + " ";
                                                addCondition = true;
                                            }
                                            else
                                            {
                                                //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_SOTTOCONTATORE, false) + " ) )";
                                                filterSottocontatore += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_SOTTOCONTATORE, false) + " ";
                                                addCondition = true;
                                            }
                                        }
                                        //Data inserimento sottocontatore
                                        if (oggetto.DATA_INSERIMENTO != null && oggetto.DATA_INSERIMENTO != "")
                                        {
                                            if (oggetto.DATA_INSERIMENTO.IndexOf('@') != -1)
                                            {
                                                string[] dateIns = oggetto.DATA_INSERIMENTO.Split('@');
                                                //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND dpa0.dta_ins >= " + DocsPaDbManagement.Functions.Functions.ToDate(dateIns[0]) + " AND dpa0.dta_ins <= " + DocsPaDbManagement.Functions.Functions.ToDate(dateIns[1]) + " ) ) ";
                                                filterSottocontatore += " AND dpa0.dta_ins >= " + DocsPaDbManagement.Functions.Functions.ToDate(dateIns[0]) + " AND dpa0.dta_ins <= " + DocsPaDbManagement.Functions.Functions.ToDate(dateIns[1]) + " ";
                                                addCondition = true;
                                            }
                                            else
                                            {
                                                //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND dpa0.dta_ins >= " + DocsPaDbManagement.Functions.Functions.ToDate(oggetto.DATA_INSERIMENTO) + " ) ) ";
                                                filterSottocontatore += " AND dpa0.dta_ins >= " + DocsPaDbManagement.Functions.Functions.ToDate(oggetto.DATA_INSERIMENTO) + " ";
                                                addCondition = true;
                                            }
                                        }
                                        break;

                                    //O è di tipo "A" o di tipo "R"
                                    default:
                                        //if (oggetto.ID_AOO_RF != null && oggetto.ID_AOO_RF != "")
                                        if (!string.IsNullOrEmpty(oggetto.ID_AOO_RF) && oggetto.ID_AOO_RF != "0")
                                        {
                                            //Contatore
                                            if (!oggetto.VALORE_DATABASE.Equals(""))
                                            {
                                                //Nr.Contatore SI - Aoo/Rf SI
                                                if (oggetto.VALORE_DATABASE.IndexOf('@') != -1)
                                                {
                                                    string[] contatore = oggetto.VALORE_DATABASE.Split('@');
                                                    //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[1], false) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ) )";
                                                    filterSottocontatore += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[1], false) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ";
                                                    addCondition = true;
                                                }
                                                else
                                                {
                                                    //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_DATABASE, false) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ) )";
                                                    filterSottocontatore += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_DATABASE, false) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ";
                                                    addCondition = true;
                                                }
                                            }

                                            //Sottocontatore
                                            if (!oggetto.VALORE_SOTTOCONTATORE.Equals(""))
                                            {
                                                //Nr.Sottocontatore SI - Aoo/Rf SI
                                                if (oggetto.VALORE_SOTTOCONTATORE.IndexOf('@') != -1)
                                                {
                                                    string[] sottocontatore = oggetto.VALORE_SOTTOCONTATORE.Split('@');
                                                    //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(sottocontatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(sottocontatore[1], false) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ) )";
                                                    filterSottocontatore += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(sottocontatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(sottocontatore[1], false) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ";
                                                    addCondition = true;
                                                }
                                                else
                                                {
                                                    //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_SOTTOCONTATORE, false) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ) )";
                                                    filterSottocontatore += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_SOTTOCONTATORE, false) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ";
                                                    addCondition = true;
                                                }
                                            }

                                            //Dta. Inserimento sottocontatore
                                            if (!oggetto.DATA_INSERIMENTO.Equals(""))
                                            {
                                                //Dta. Inserimento sottocontatore SI - Aoo/Rf SI
                                                if (oggetto.DATA_INSERIMENTO.IndexOf('@') != -1)
                                                {
                                                    string[] dateIns = oggetto.DATA_INSERIMENTO.Split('@');
                                                    //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND dpa0.dta_ins >= " + DocsPaDbManagement.Functions.Functions.ToDate(dateIns[0]) + " AND dpa0.dta_ins <= " + DocsPaDbManagement.Functions.Functions.ToDate(dateIns[1]) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ) )";
                                                    filterSottocontatore += " AND dpa0.dta_ins >= " + DocsPaDbManagement.Functions.Functions.ToDate(dateIns[0]) + " AND dpa0.dta_ins <= " + DocsPaDbManagement.Functions.Functions.ToDate(dateIns[1]) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ";
                                                    addCondition = true;
                                                }
                                                else
                                                {
                                                    //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND dpa0.dta_ins >= " + DocsPaDbManagement.Functions.Functions.ToDate(oggetto.DATA_INSERIMENTO) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ) )";
                                                    filterSottocontatore += " AND dpa0.dta_ins >= " + DocsPaDbManagement.Functions.Functions.ToDate(oggetto.DATA_INSERIMENTO) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ";
                                                    addCondition = true;
                                                }
                                            }

                                            //Contatore, Sottocontatore e Dta. Inserimento sottocontatore NO - Aoo/Rf SI
                                            if (oggetto.VALORE_DATABASE.Equals("") && oggetto.VALORE_SOTTOCONTATORE.Equals("") && oggetto.DATA_INSERIMENTO.Equals(""))
                                            {
                                                //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ) )";
                                                filterSottocontatore += " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ";
                                                addCondition = true;
                                            }
                                        }
                                        else
                                        {
                                            //Nr.Contatore SI - Aoo/Rf NO
                                            if (!oggetto.VALORE_DATABASE.Equals(""))
                                            {
                                                if (oggetto.VALORE_DATABASE.IndexOf('@') != -1)
                                                {
                                                    string[] contatore = oggetto.VALORE_DATABASE.Split('@');
                                                    //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[1], false) + " ) )";
                                                    filterSottocontatore += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[1], false) + " ";
                                                    addCondition = true;
                                                }
                                                else
                                                {
                                                    //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_DATABASE, false) + " ) )";
                                                    filterSottocontatore += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_DATABASE, false) + " ";
                                                    addCondition = true;
                                                }
                                            }

                                            //Nr.Sottocontatore SI - Aoo/Rf NO
                                            if (!oggetto.VALORE_SOTTOCONTATORE.Equals(""))
                                            {
                                                if (oggetto.VALORE_SOTTOCONTATORE.IndexOf('@') != -1)
                                                {
                                                    string[] sottocontatore = oggetto.VALORE_SOTTOCONTATORE.Split('@');
                                                    //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(sottocontatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(sottocontatore[1], false) + " ) )";
                                                    filterSottocontatore += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(sottocontatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(sottocontatore[1], false) + " ";
                                                    addCondition = true;
                                                }
                                                else
                                                {
                                                    //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_SOTTOCONTATORE, false) + " ) )";
                                                    filterSottocontatore += " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_sc", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_SOTTOCONTATORE, false) + " ";
                                                    addCondition = true;
                                                }
                                            }

                                            //Dta. Inserimento Sottocontatore SI - Aoo/Rf NO
                                            if (!oggetto.DATA_INSERIMENTO.Equals(""))
                                            {
                                                if (oggetto.DATA_INSERIMENTO.IndexOf('@') != -1)
                                                {
                                                    string[] dateIns = oggetto.DATA_INSERIMENTO.Split('@');
                                                    //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND dpa0.dta_ins >= " + DocsPaDbManagement.Functions.Functions.ToDate(dateIns[0]) + " AND dpa0.dta_ins <= " + DocsPaDbManagement.Functions.Functions.ToDate(dateIns[1]) + " ) )";
                                                    filterSottocontatore += " AND dpa0.dta_ins >= " + DocsPaDbManagement.Functions.Functions.ToDate(dateIns[0]) + " AND dpa0.dta_ins <= " + DocsPaDbManagement.Functions.Functions.ToDate(dateIns[1]) + " ";
                                                    addCondition = true;
                                                }
                                                else
                                                {
                                                    //filterString += operatoreRicerca + " A.system_id in  (SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND dpa0.dta_ins >= " + DocsPaDbManagement.Functions.Functions.ToDate(oggetto.DATA_INSERIMENTO) + " ) )";
                                                    filterSottocontatore += " AND dpa0.dta_ins >= " + DocsPaDbManagement.Functions.Functions.ToDate(oggetto.DATA_INSERIMENTO) + " ";
                                                    addCondition = true;
                                                }
                                            }
                                        }
                                        break;
                                }

                                //Verifico se sono state aggiunte condizioni di ricerca per questo tipo di campo 
                                if (addCondition)
                                    filterSottocontatore += " )) ";
                                else
                                    filterSottocontatore = string.Empty;

                                filterString += filterSottocontatore;
                                break;
                            case "OggettoEsterno":
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE) || !string.IsNullOrEmpty(oggetto.CODICE_DB))
                                {
                                    string assQuery = "SELECT doc_number FROM dpa_associazione_templates dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                    if (!string.IsNullOrEmpty(anno_prof))
                                        assQuery += " AND dpa0.ANNO = " + anno_prof;
                                    if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                    {
                                        assQuery = assQuery + " AND UPPER(dpa0.valore_oggetto_db) LIKE UPPER('%" + oggetto.VALORE_DATABASE.Replace("'", "''") + "%')";
                                    }
                                    if (!string.IsNullOrEmpty(oggetto.CODICE_DB))
                                    {
                                        assQuery = assQuery + " AND UPPER(dpa0.codice_db) = UPPER('" + oggetto.CODICE_DB.Replace("'", "''") + "')";
                                    }
                                    assQuery = assQuery + ")";
                                    filterString += operatoreRicerca + " A.system_id in  (" + assQuery + ")";
                                }
                                break;
                        }

                        if (firstFilter)
                            firstFilter = false;
                    }
                }

                if (!string.IsNullOrEmpty(filterString))
                {
                    filterString = string.Format(" {0} ({1})",
                        DocsPaVO.ProfilazioneDinamica.OperatoriRicercaOggettiCustomEnum.And.ToString().ToLowerInvariant(), filterString);
                }
            }

            return filterString;
        }

        public DocsPaVO.ProfilazioneDinamica.Templates getTemplate(string docNumber)
        {
            DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();
            string idTemplate = null;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //Ricerco un determinato idTemplate a partire dal docNumber: prima la query era PD_GET_ID_TEMPLATE_FROM_DOC_NUMBER
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_ID_TEMPLATE_FROM_PROFILE");
                queryMng.setParam("docNumber", docNumber);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_template = new DataSet();
                dbProvider.ExecuteQuery(ds_template, commandText);

                //Verifico, se esiste un idTemplate per quel docNumber lo carico, altrimenti carico un template vuoto
                //per il tipoAtto e l'amministrazione richiesti
                if (ds_template.Tables[0].Rows.Count != 0)
                {
                    idTemplate = ds_template.Tables[0].Rows[0]["ID_TEMPLATE"].ToString();
                    template.SYSTEM_ID = Convert.ToInt32(idTemplate);

                    //Recupero la descrizione del template
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DESCRIZIONE_TEMPLATE");
                    queryMng.setParam("idTemplate", idTemplate);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplate - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                    DataSet ds_template_descrizione = new DataSet();
                    dbProvider.ExecuteQuery(ds_template_descrizione, commandText);

                    //Carico il template associato allo specifico docNumber
                    setTemplate(ref template, ds_template_descrizione, 0);
                    template.DOC_NUMBER = docNumber;
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return template;
        }

        public DocsPaVO.ProfilazioneDinamica.Templates getTemplateDettagli(string docNumber)
        {
            logger.Info("BEGIN");
            DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();
            string idTemplate = null;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //Ricerco un determinato idTemplate a partire dal docNumber
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_ID_TEMPLATE_FROM_DOC_NUMBER");
                queryMng.setParam("docNumber", docNumber);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplateDettagli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplateDettagli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_template = new DataSet();
                dbProvider.ExecuteQuery(ds_template, commandText);

                //Verifico, se esiste un idTemplate per quel docNumber lo carico, altrimenti carico un template vuoto
                //per il tipoAtto e l'amministrazione richiesti
                if (ds_template.Tables[0].Rows.Count != 0)
                {
                    idTemplate = ds_template.Tables[0].Rows[0]["ID_TEMPLATE"].ToString();
                    template.SYSTEM_ID = Convert.ToInt32(idTemplate);

                    //Recupero i dati del template
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TEMPLATE_DOC");
                    queryMng.setParam("idTemplate", idTemplate);
                    //queryMng.setParam("docNumber", docNumber);
                    queryMng.setParam("docNumber", " AND DPA_ASSOCIAZIONE_TEMPLATES.DOC_NUMBER  = " + docNumber + " ");
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateDettagli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateDettagli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    DataSet ds_templateCompleto = new DataSet();
                    dbProvider.ExecuteQuery(ds_templateCompleto, commandText);

                    //Se il template non ha oggetti custom vengono restituite solo le proprietà del template
                    if (ds_templateCompleto.Tables[0].Rows.Count == 0)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE_2");
                        queryMng.setParam("idTemplate", idTemplate);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - getTemplateById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - getTemplateById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        ds_template = new DataSet();
                        dbProvider.ExecuteQuery(ds_template, commandText);

                        setTemplate(ref template, ds_template, 0);
                        return template;
                    }

                    setTemplate(ref template, ds_templateCompleto, 0);

                    //Cerco gli oggetti custom associati al template
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateDettagli - ProfilazioneDinamica/Database/model.cs - Impostazione valori oggetti custom");
                    logger.Debug("SQL - getTemplateDettagli - ProfilazioneDinamica/Database/model.cs - Impostazione valori oggetti custom");

                    for (int j = 0; j < ds_templateCompleto.Tables[0].Rows.Count; j++)
                    {
                        oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                        setOggettoCustom(ref oggettoCustom, ds_templateCompleto, j);

                        DocsPaVO.ProfilazioneDinamica.TipoOggetto tipoOggetto = new DocsPaVO.ProfilazioneDinamica.TipoOggetto();
                        setTipoOggetto(ref tipoOggetto, ds_templateCompleto, j);

                        //Aggiungo il tipo oggetto all'oggettoCustom
                        oggettoCustom.TIPO = tipoOggetto;

                        //campo CLOB di configurazione
                        if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                        {
                            string config = dbProvider.GetLargeText("DPA_OGGETTI_CUSTOM", oggettoCustom.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST");
                            oggettoCustom.CONFIG_OBJ_EST = config;
                        }

                        if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione"))
                        {
                            //Recupero il valoreDb dell'oggetto
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_VALORE_DB_OGGETTO_CUSTOM_1");
                            queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                            queryMng.setParam("docNumber", docNumber);
                            commandText = queryMng.getSQL();
                            //System.Diagnostics.Debug.WriteLine("SQL - getTemplateDettagli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            //logger.Debug("SQL - getTemplateDettagli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                            DataSet ds_valore_database = new DataSet();
                            dbProvider.ExecuteQuery(ds_valore_database, commandText);

                            if (ds_valore_database.Tables[0].Rows.Count != 0)
                            {
                                oggettoCustom.VALORE_DATABASE = ds_valore_database.Tables[0].Rows[0]["Valore_Oggetto_Db"].ToString();
                                oggettoCustom.ANNO = ds_valore_database.Tables[0].Rows[0]["Anno"].ToString();
                                if (ds_valore_database.Tables[0].Rows[0]["ID_AOO_RF"].ToString() != null && ds_valore_database.Tables[0].Rows[0]["ID_AOO_RF"].ToString() != "")
                                    oggettoCustom.ID_AOO_RF = ds_valore_database.Tables[0].Rows[0]["ID_AOO_RF"].ToString();
                                else
                                    oggettoCustom.ID_AOO_RF = "0";
                                for (int i = 0; i < ds_valore_database.Tables[0].Rows.Count; i++)
                                {
                                    oggettoCustom.VALORI_SELEZIONATI.Add(ds_valore_database.Tables[0].Rows[i]["Valore_Oggetto_Db"].ToString());
                                }
                            }
                        }

                        if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione") ||
                            oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("MenuATendina") ||
                            oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("SelezioneEsclusiva"))
                        {
                            //Selezioni i valori per l'oggettoCustom
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI");
                            queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                            commandText = queryMng.getSQL();
                            //System.Diagnostics.Debug.WriteLine("SQL - getTemplateDettagli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            //logger.Debug("SQL - getTemplateDettagli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                            DataSet ds_valoriOggetto = new DataSet();
                            dbProvider.ExecuteQuery(ds_valoriOggetto, commandText);

                            for (int k = 0; k < ds_valoriOggetto.Tables[0].Rows.Count; k++)
                            {
                                if (oggettoCustom != null && !string.IsNullOrEmpty(oggettoCustom.TIPO.DESCRIZIONE_TIPO))
                                {
                                    DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto();

                                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                                    {
                                        case "CasellaDiSelezione":
                                            if (k < oggettoCustom.VALORI_SELEZIONATI.Count)
                                            {
                                                setValoreOggetto(ref valoreOggetto, ds_valoriOggetto, k);
                                                oggettoCustom.ELENCO_VALORI.Add(valoreOggetto);
                                            }
                                            break;

                                        default:
                                            setValoreOggetto(ref valoreOggetto, ds_valoriOggetto, k);
                                            oggettoCustom.ELENCO_VALORI.Add(valoreOggetto);
                                            break;
                                    }
                                }
                            }
                        }

                        template.ELENCO_OGGETTI.Add(oggettoCustom);
                    }
                }
                else
                {
                    return getTemplate(docNumber);
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                logger.Info("END");
                dbProvider.Dispose();
            }
            return template;
        }

        public ArrayList getTemplatesArchivioDeposito(string idAmm, bool seRepertorio)
        {
            ArrayList templates = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TEMPLATES_ARCHIVIO_DEPOSITO");
                queryMng.setParam("idAmm", idAmm);
                if (seRepertorio)
                    queryMng.setParam("seRepertorio", " and dpa_oggetti_custom.REPERTORIO = 1");
                else
                    queryMng.setParam("seRepertorio", "");

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplatesArchivioDeposito - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplatesArchivioDeposito - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                DataSet ds_templates = new DataSet();
                dbProvider.ExecuteQuery(ds_templates, commandText);

                for (int i = 0; i < ds_templates.Tables[0].Rows.Count; i++)
                {
                    DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();
                    setTemplate(ref template, ds_templates, i);
                    templates.Add(template);
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return templates;
        }

        public bool getContatoreTemplates(string idTemplate)
        {
            bool result = false;
            DocsPaVO.ProfilazioneDinamica.Templates template = getTemplateById(idTemplate);
            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto in template.ELENCO_OGGETTI)
            {
                switch (oggetto.TIPO.DESCRIZIONE_TIPO)
                {
                    case "Contatore":
                        result = true;
                        break;
                }
            }
            return result;
        }

        public ArrayList getDirittiCampiTipologiaDoc(string idRuolo, string idTemplate)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            ArrayList listaDirittiCampi = new ArrayList();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_A_R_CAMPI_DOC");
                queryMng.setParam("idTemplate", idTemplate);
                queryMng.setParam("idRuolo", idRuolo);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getDirittiCampiTipologiaDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getDirittiCampiTipologiaDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_dirittiCampi = new DataSet();
                dbProvider.ExecuteQuery(ds_dirittiCampi, commandText);

                if (ds_dirittiCampi.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds_dirittiCampi.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                        setAssDocFascRuoli(ref assDocFascRuoli, ds_dirittiCampi, i);
                        listaDirittiCampi.Add(assDocFascRuoli);
                    }
                }
                else
                {
                    DocsPaVO.ProfilazioneDinamica.Templates template = this.getTemplateById(idTemplate);
                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                    {
                        DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                        assDocFascRuoli.ID_GRUPPO = idRuolo;
                        assDocFascRuoli.ID_TIPO_DOC_FASC = idTemplate;
                        assDocFascRuoli.ID_OGGETTO_CUSTOM = oggettoCustom.SYSTEM_ID.ToString();
                        assDocFascRuoli.INS_MOD_OGG_CUSTOM = "0";
                        if (!String.IsNullOrEmpty(template.IPER_FASC_DOC) && template.IPER_FASC_DOC.Equals("1"))
                            assDocFascRuoli.VIS_OGG_CUSTOM = "1";
                        else
                            assDocFascRuoli.VIS_OGG_CUSTOM = "0";
                        assDocFascRuoli.ANNULLA_REPERTORIO = "0";
                        listaDirittiCampi.Add(assDocFascRuoli);
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return listaDirittiCampi;
        }

        public void salvaDirittiCampiTipologiaDoc(ArrayList listaDirittiCampiSelezionati)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                foreach (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli in listaDirittiCampiSelezionati)
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_A_R_CAMPI_DOC");
                    queryMng.setParam("idTemplate", assDocFascRuoli.ID_TIPO_DOC_FASC);
                    queryMng.setParam("idOggettoCustom", assDocFascRuoli.ID_OGGETTO_CUSTOM);
                    queryMng.setParam("idRuolo", assDocFascRuoli.ID_GRUPPO);
                    queryMng.setParam("insMod", assDocFascRuoli.INS_MOD_OGG_CUSTOM);
                    queryMng.setParam("vis", assDocFascRuoli.VIS_OGG_CUSTOM);
                    queryMng.setParam("delRep", assDocFascRuoli.ANNULLA_REPERTORIO);
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - salvaDirittiCampiTipologiaDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaDirittiCampiTipologiaDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    int rowsAffected;
                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                    if (rowsAffected == 0)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INS_A_R_CAMPI_DOC");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_A_R_OGG_CUSTOM_DOC"));
                        queryMng.setParam("idTemplate", assDocFascRuoli.ID_TIPO_DOC_FASC);
                        queryMng.setParam("idOggettoCustom", assDocFascRuoli.ID_OGGETTO_CUSTOM);
                        queryMng.setParam("idRuolo", assDocFascRuoli.ID_GRUPPO);
                        queryMng.setParam("inserimento", assDocFascRuoli.INS_MOD_OGG_CUSTOM);
                        queryMng.setParam("visibilita", assDocFascRuoli.VIS_OGG_CUSTOM);
                        queryMng.setParam("delRep", assDocFascRuoli.ANNULLA_REPERTORIO);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaDirittiCampiTipologiaDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaDirittiCampiTipologiaDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                    }
                }
            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public void estendiDirittiCampiARuoliDoc(ArrayList listaDirittiCampiSelezionati, ArrayList listaRuoli)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                foreach (DocsPaVO.utente.Ruolo ruolo in listaRuoli)
                {
                    foreach (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli in listaDirittiCampiSelezionati)
                    {
                        DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_A_R_CAMPI_DOC");
                        queryMng.setParam("idTemplate", assDocFascRuoli.ID_TIPO_DOC_FASC);
                        queryMng.setParam("idOggettoCustom", assDocFascRuoli.ID_OGGETTO_CUSTOM);
                        queryMng.setParam("idRuolo", ruolo.idGruppo);
                        queryMng.setParam("insMod", assDocFascRuoli.INS_MOD_OGG_CUSTOM);
                        queryMng.setParam("vis", assDocFascRuoli.VIS_OGG_CUSTOM);
                        queryMng.setParam("delRep", assDocFascRuoli.ANNULLA_REPERTORIO);
                        string commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - estendiDirittiCampiARuoliDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - estendiDirittiCampiARuoliDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        int rowsAffected;
                        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                        if (rowsAffected == 0)
                        {
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INS_A_R_CAMPI_DOC");
                            queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                            queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_A_R_OGG_CUSTOM_DOC"));
                            queryMng.setParam("idTemplate", assDocFascRuoli.ID_TIPO_DOC_FASC);
                            queryMng.setParam("idOggettoCustom", assDocFascRuoli.ID_OGGETTO_CUSTOM);
                            queryMng.setParam("idRuolo", ruolo.idGruppo);
                            queryMng.setParam("inserimento", assDocFascRuoli.INS_MOD_OGG_CUSTOM);
                            queryMng.setParam("visibilita", assDocFascRuoli.VIS_OGG_CUSTOM);
                            queryMng.setParam("delRep", assDocFascRuoli.ANNULLA_REPERTORIO);
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - estendiDirittiCampiARuoli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - estendiDirittiCampiARuoli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);
                        }
                    }
                }
            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli getDirittiCampoTipologiaDoc(string idRuolo, string idTemplate, string idOggettoCustom)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoliResult = null;
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_A_R_CAMPO_DOC");
                queryMng.setParam("idTemplate", idTemplate);
                queryMng.setParam("idOggettoCustom", idOggettoCustom);
                queryMng.setParam("idRuolo", idRuolo);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getDirittiCampoTipologiaDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getDirittiCampoTipologiaDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_dirittiCampo = new DataSet();
                dbProvider.ExecuteQuery(ds_dirittiCampo, commandText);

                if (ds_dirittiCampo.Tables[0].Rows.Count != 0)
                {
                    assDocFascRuoliResult = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                    setAssDocFascRuoli(ref assDocFascRuoliResult, ds_dirittiCampo, 0);
                }
                else
                {
                    DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                    assDocFascRuoli.ID_GRUPPO = idRuolo;
                    assDocFascRuoli.ID_TIPO_DOC_FASC = idTemplate;
                    assDocFascRuoli.ID_OGGETTO_CUSTOM = idOggettoCustom;
                    assDocFascRuoli.INS_MOD_OGG_CUSTOM = "0";
                    assDocFascRuoli.VIS_OGG_CUSTOM = "0";
                    assDocFascRuoli.ANNULLA_REPERTORIO = "0";
                    assDocFascRuoliResult = assDocFascRuoli;
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return assDocFascRuoliResult;
        }

        public void estendiDirittiRuoloACampiDoc(ArrayList listaDirittiRuoli, ArrayList listaCampi)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                foreach (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli in listaDirittiRuoli)
                {
                    //Se il diritto sulla tipologia è 0, allora ai campi non viene dato nessun diritto
                    if (assDocFascRuoli.DIRITTI_TIPOLOGIA == "0")
                    {
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in listaCampi)
                        {
                            DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocsFascRuoliNew = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                            assDocsFascRuoliNew.ID_TIPO_DOC_FASC = assDocFascRuoli.ID_TIPO_DOC_FASC;
                            assDocsFascRuoliNew.ID_OGGETTO_CUSTOM = oggettoCustom.SYSTEM_ID.ToString();
                            assDocsFascRuoliNew.ID_GRUPPO = assDocFascRuoli.ID_GRUPPO;
                            assDocsFascRuoliNew.INS_MOD_OGG_CUSTOM = "0";
                            assDocsFascRuoliNew.VIS_OGG_CUSTOM = "0";
                            assDocsFascRuoliNew.ANNULLA_REPERTORIO = "0";
                            ArrayList listaDirittiCampiSelezionati = new ArrayList();
                            listaDirittiCampiSelezionati.Add(assDocsFascRuoliNew);
                            this.salvaDirittiCampiTipologiaDoc(listaDirittiCampiSelezionati);
                        }
                    }

                    //Se il diritto sulla tipologia è 1, allora ai campi viene data solo la visibilità
                    if (assDocFascRuoli.DIRITTI_TIPOLOGIA == "1")
                    {
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in listaCampi)
                        {
                            DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocsFascRuoliNew = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                            assDocsFascRuoliNew.ID_TIPO_DOC_FASC = assDocFascRuoli.ID_TIPO_DOC_FASC;
                            assDocsFascRuoliNew.ID_OGGETTO_CUSTOM = oggettoCustom.SYSTEM_ID.ToString();
                            assDocsFascRuoliNew.ID_GRUPPO = assDocFascRuoli.ID_GRUPPO;
                            assDocsFascRuoliNew.INS_MOD_OGG_CUSTOM = "0";
                            assDocsFascRuoliNew.VIS_OGG_CUSTOM = "1";
                            assDocsFascRuoliNew.ANNULLA_REPERTORIO = "0";
                            ArrayList listaDirittiCampiSelezionati = new ArrayList();
                            listaDirittiCampiSelezionati.Add(assDocsFascRuoliNew);
                            this.salvaDirittiCampiTipologiaDoc(listaDirittiCampiSelezionati);
                        }
                    }

                    //Se il diritto sulla tipologia è 2, allora ai campi vengono dati pieni diritti
                    if (assDocFascRuoli.DIRITTI_TIPOLOGIA == "2")
                    {
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in listaCampi)
                        {
                            DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocsFascRuoliNew = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                            assDocsFascRuoliNew.ID_TIPO_DOC_FASC = assDocFascRuoli.ID_TIPO_DOC_FASC;
                            assDocsFascRuoliNew.ID_OGGETTO_CUSTOM = oggettoCustom.SYSTEM_ID.ToString();
                            assDocsFascRuoliNew.ID_GRUPPO = assDocFascRuoli.ID_GRUPPO;
                            assDocsFascRuoliNew.INS_MOD_OGG_CUSTOM = "1";
                            assDocsFascRuoliNew.VIS_OGG_CUSTOM = "1";
                            assDocsFascRuoliNew.ANNULLA_REPERTORIO = "0";
                            ArrayList listaDirittiCampiSelezionati = new ArrayList();
                            listaDirittiCampiSelezionati.Add(assDocsFascRuoliNew);
                            this.salvaDirittiCampiTipologiaDoc(listaDirittiCampiSelezionati);
                        }
                    }
                }
            }
            catch
            {
                dbProvider.RollbackTransaction();
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public ArrayList getRuoliFromOggettoCustomDoc(string idTemplate, string idOggettoCustom)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            ArrayList ruoliFromOggettoCustom = new ArrayList();
            //Utenti ut = new Utenti();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_A_R_RUOLI_FROM_OGG_CUSTOM_DOC");
                queryMng.setParam("idTemplate", idTemplate);
                queryMng.setParam("idOggettoCustom", idOggettoCustom);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getRuoliFromOggettoCustomDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getRuoliFromOggettoCustomDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_ruoliFromCampi = new DataSet();
                dbProvider.ExecuteQuery(ds_ruoliFromCampi, commandText);

                if (ds_ruoliFromCampi.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds_ruoliFromCampi.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                        setAssDocFascRuoli(ref assDocFascRuoli, ds_ruoliFromCampi, i);
                        ruoliFromOggettoCustom.Add(assDocFascRuoli);
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return ruoliFromOggettoCustom;
        }

        private void setTemplate(ref DocsPaVO.ProfilazioneDinamica.Templates template, DataSet dataSet, int rowNumber)
        {
            try
            {
                if (dataSet.Tables[0].Columns.Contains("SYSTEM_ID"))
                {
                    template.SYSTEM_ID = Convert.ToInt32(dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID"].ToString());
                    template.ID_TIPO_ATTO = dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID"].ToString();
                }
                if (dataSet.Tables[0].Columns.Contains("SYSTEM_ID_TEMPLATE"))
                {
                    template.SYSTEM_ID = Convert.ToInt32(dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID_TEMPLATE"].ToString());
                    template.ID_TIPO_ATTO = dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID_TEMPLATE"].ToString();
                }
                if (dataSet.Tables[0].Columns.Contains("VAR_DESC_ATTO"))
                    template.DESCRIZIONE = dataSet.Tables[0].Rows[rowNumber]["VAR_DESC_ATTO"].ToString();
                if (dataSet.Tables[0].Columns.Contains("DOC_NUMBER"))
                    template.DOC_NUMBER = dataSet.Tables[0].Rows[rowNumber]["DOC_NUMBER"].ToString();
                if (dataSet.Tables[0].Columns.Contains("ABILITATO_SI_NO"))
                    template.ABILITATO_SI_NO = dataSet.Tables[0].Rows[rowNumber]["ABILITATO_SI_NO"].ToString();
                if (dataSet.Tables[0].Columns.Contains("IN_ESERCIZIO"))
                    template.IN_ESERCIZIO = dataSet.Tables[0].Rows[rowNumber]["IN_ESERCIZIO"].ToString();
                if (dataSet.Tables[0].Columns.Contains("PATH_MOD_1"))
                    template.PATH_MODELLO_1 = dataSet.Tables[0].Rows[rowNumber]["PATH_MOD_1"].ToString();
                if (dataSet.Tables[0].Columns.Contains("PATH_MOD_2"))
                    template.PATH_MODELLO_2 = dataSet.Tables[0].Rows[rowNumber]["PATH_MOD_2"].ToString();
                if (dataSet.Tables[0].Columns.Contains("Ext_Mod_1"))
                    template.PATH_MODELLO_1_EXT = dataSet.Tables[0].Rows[rowNumber]["Ext_Mod_1"].ToString().Trim();
                if (dataSet.Tables[0].Columns.Contains("Ext_Mod_2"))
                    template.PATH_MODELLO_2_EXT = dataSet.Tables[0].Rows[rowNumber]["Ext_Mod_2"].ToString().Trim();
                if (dataSet.Tables[0].Columns.Contains("PATH_MOD_SU"))
                    template.PATH_MODELLO_STAMPA_UNIONE = dataSet.Tables[0].Rows[rowNumber]["PATH_MOD_SU"].ToString();
                if (dataSet.Tables[0].Columns.Contains("PATH_MOD_EXC"))
                    template.PATH_MODELLO_EXCEL = dataSet.Tables[0].Rows[rowNumber]["PATH_MOD_EXC"].ToString();
                // Modifica Elaborazione XML da PIS req.1
                if (dataSet.Tables[0].Columns.Contains("PATH_XSD_ASSOCIATO"))
                    template.PATH_XSD_ASSOCIATO = dataSet.Tables[0].Rows[rowNumber]["PATH_XSD_ASSOCIATO"].ToString();
                
                if (dataSet.Tables[0].Columns.Contains("PATH_ALL_1"))
                    template.PATH_ALLEGATO_1 = dataSet.Tables[0].Rows[rowNumber]["PATH_ALL_1"].ToString();
                if (dataSet.Tables[0].Columns.Contains("GG_SCADENZA"))
                    template.SCADENZA = dataSet.Tables[0].Rows[rowNumber]["GG_SCADENZA"].ToString();
                if (dataSet.Tables[0].Columns.Contains("GG_PRE_SCADENZA"))
                    template.PRE_SCADENZA = dataSet.Tables[0].Rows[rowNumber]["GG_PRE_SCADENZA"].ToString();
                if (dataSet.Tables[0].Columns.Contains("CHA_PRIVATO"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["CHA_PRIVATO"].ToString()))
                        template.PRIVATO = dataSet.Tables[0].Rows[rowNumber]["CHA_PRIVATO"].ToString();
                    else
                        template.PRIVATO = "0";
                }
                if (dataSet.Tables[0].Columns.Contains("ID_AMM"))
                    template.ID_AMMINISTRAZIONE = dataSet.Tables[0].Rows[rowNumber]["ID_AMM"].ToString();
                if (dataSet.Tables[0].Columns.Contains("COD_CLASS"))
                    template.CODICE_CLASSIFICA = dataSet.Tables[0].Rows[rowNumber]["COD_CLASS"].ToString();
                if (dataSet.Tables[0].Columns.Contains("COD_MOD_TRASM"))
                    template.CODICE_MODELLO_TRASM = dataSet.Tables[0].Rows[rowNumber]["COD_MOD_TRASM"].ToString();
                if (dataSet.Tables[0].Columns.Contains("IPERDOCUMENTO"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["IPERDOCUMENTO"].ToString()) && dataSet.Tables[0].Rows[rowNumber]["IPERDOCUMENTO"].ToString() == "1")
                        template.IPER_FASC_DOC = "1";
                    else
                        template.IPER_FASC_DOC = "0";
                }
                if (dataSet.Tables[0].Columns.Contains("NUM_MESI_CONSERVAZIONE"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["NUM_MESI_CONSERVAZIONE"].ToString()))
                        template.NUM_MESI_CONSERVAZIONE = dataSet.Tables[0].Rows[rowNumber]["NUM_MESI_CONSERVAZIONE"].ToString();
                    else
                        template.NUM_MESI_CONSERVAZIONE = "0";


                }
                if (dataSet.Tables[0].Columns.Contains("IS_TYPE_INSTANCE"))
                {
                    template.IS_TYPE_INSTANCE = string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["IS_TYPE_INSTANCE"].ToString()) || dataSet.Tables[0].Rows[rowNumber]["IS_TYPE_INSTANCE"].ToString() == "0" ? '0' : '1';
                }

                // INTEGRAZIONE PITRE-PARER
                if (dataSet.Tables[0].Columns.Contains("CHA_INVIO_CONSERVAZIONE"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["CHA_INVIO_CONSERVAZIONE"].ToString()))
                        template.INVIO_CONSERVAZIONE = dataSet.Tables[0].Rows[rowNumber]["CHA_INVIO_CONSERVAZIONE"].ToString();
                    else
                        template.INVIO_CONSERVAZIONE = "0";
                }

                if (dataSet.Tables[0].Columns.Contains("CHA_ASSOC_MANUALE"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["CHA_ASSOC_MANUALE"].ToString()))
                        template.CHA_ASSOC_MANUALE = dataSet.Tables[0].Rows[rowNumber]["CHA_ASSOC_MANUALE"].ToString();
                    else
                        template.CHA_ASSOC_MANUALE = "0";
                }
                if (dataSet.Tables[0].Columns.Contains("ID_CONTESTO_PROCEDURALE"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["ID_CONTESTO_PROCEDURALE"].ToString()))
                        template.ID_CONTESTO_PROCEDURALE = dataSet.Tables[0].Rows[rowNumber]["ID_CONTESTO_PROCEDURALE"].ToString();
                }
            }
            catch (Exception ex)
            {
                logger.DebugFormat("SQL - setTemplate - ProfilazioneDinamica/Database/model.cs - Exception : {0} \r\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private void setOggettoCustom(ref DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, DataSet dataSet, int rowNumber)
        {
            try
            {
                if (dataSet.Tables[0].Columns.Contains("SYSTEM_ID"))
                    oggettoCustom.SYSTEM_ID = Convert.ToInt32(dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID"].ToString());
                if (dataSet.Tables[0].Columns.Contains("SYSTEM_ID_OGG_CUSTOM"))
                    oggettoCustom.SYSTEM_ID = Convert.ToInt32(dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID_OGG_CUSTOM"].ToString());
                if (dataSet.Tables[0].Columns.Contains("CAMPO_DI_RICERCA"))
                    oggettoCustom.CAMPO_DI_RICERCA = dataSet.Tables[0].Rows[rowNumber]["CAMPO_DI_RICERCA"].ToString();
                if (dataSet.Tables[0].Columns.Contains("CAMPO_OBBLIGATORIO"))
                {
                    oggettoCustom.CAMPO_OBBLIGATORIO = dataSet.Tables[0].Rows[rowNumber]["CAMPO_OBBLIGATORIO"].ToString();
                    oggettoCustom.ASTERISCO_OBBLIGATORIETA = dataSet.Tables[0].Rows[rowNumber]["CAMPO_OBBLIGATORIO"].ToString();
                }
                if (dataSet.Tables[0].Columns.Contains("DESCRIZIONE"))
                    oggettoCustom.DESCRIZIONE = dataSet.Tables[0].Rows[rowNumber]["DESCRIZIONE"].ToString();
                if (dataSet.Tables[0].Columns.Contains("MULTILINEA"))
                    oggettoCustom.MULTILINEA = dataSet.Tables[0].Rows[rowNumber]["MULTILINEA"].ToString();
                if (dataSet.Tables[0].Columns.Contains("NUMERO_DI_CARATTERI"))
                    oggettoCustom.NUMERO_DI_CARATTERI = dataSet.Tables[0].Rows[rowNumber]["NUMERO_DI_CARATTERI"].ToString();
                if (dataSet.Tables[0].Columns.Contains("NUMERO_DI_LINEE"))
                    oggettoCustom.NUMERO_DI_LINEE = dataSet.Tables[0].Rows[rowNumber]["NUMERO_DI_LINEE"].ToString();
                if (dataSet.Tables[0].Columns.Contains("ORIZZONTALE_VERTICALE"))
                    oggettoCustom.ORIZZONTALE_VERTICALE = dataSet.Tables[0].Rows[rowNumber]["ORIZZONTALE_VERTICALE"].ToString();
                if (dataSet.Tables[0].Columns.Contains("POSIZIONE"))
                    oggettoCustom.POSIZIONE = dataSet.Tables[0].Rows[rowNumber]["POSIZIONE"].ToString();
                // Modifica Elaborazione XML da PIS req.1
                if (dataSet.Tables[0].Columns.Contains("CAMPO_XML_ASSOC"))
                    oggettoCustom.CAMPO_XML_ASSOC = dataSet.Tables[0].Rows[rowNumber]["CAMPO_XML_ASSOC"].ToString();
                if (dataSet.Tables[0].Columns.Contains("OPZIONI_XML_ASSOC"))
                    oggettoCustom.OPZIONI_XML_ASSOC = dataSet.Tables[0].Rows[rowNumber]["OPZIONI_XML_ASSOC"].ToString();
                if (dataSet.Tables[0].Columns.Contains("RESET_ANNO"))
                    oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO = dataSet.Tables[0].Rows[rowNumber]["RESET_ANNO"].ToString();
                if (dataSet.Tables[0].Columns.Contains("FORMATO_CONTATORE"))
                    oggettoCustom.FORMATO_CONTATORE = dataSet.Tables[0].Rows[rowNumber]["FORMATO_CONTATORE"].ToString();
                if (dataSet.Tables[0].Columns.Contains("RICERCA_CORR"))
                    oggettoCustom.TIPO_RICERCA_CORR = dataSet.Tables[0].Rows[rowNumber]["RICERCA_CORR"].ToString();
                if (dataSet.Tables[0].Columns.Contains("ID_R_DEFAULT"))
                    oggettoCustom.ID_RUOLO_DEFAULT = dataSet.Tables[0].Rows[rowNumber]["ID_R_DEFAULT"].ToString();
                if (dataSet.Tables[0].Columns.Contains("CAMPO_COMUNE"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["CAMPO_COMUNE"].ToString()) && dataSet.Tables[0].Rows[rowNumber]["CAMPO_COMUNE"].ToString() == "1")
                        oggettoCustom.CAMPO_COMUNE = "1";
                    else
                        oggettoCustom.CAMPO_COMUNE = "0";
                }
                if (dataSet.Tables[0].Columns.Contains("CHA_TIPO_TAR"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["CHA_TIPO_TAR"].ToString()))
                        oggettoCustom.TIPO_CONTATORE = dataSet.Tables[0].Rows[rowNumber]["CHA_TIPO_TAR"].ToString();
                }
                
                if (dataSet.Tables[0].Columns.Contains("CONTA_DOPO"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["CONTA_DOPO"].ToString()) && dataSet.Tables[0].Rows[rowNumber]["CONTA_DOPO"].ToString() == "1")
                        oggettoCustom.CONTA_DOPO = "1";
                    else
                        oggettoCustom.CONTA_DOPO = "0";
                }
                if (dataSet.Tables[0].Columns.Contains("REPERTORIO"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["REPERTORIO"].ToString()) && dataSet.Tables[0].Rows[rowNumber]["REPERTORIO"].ToString() == "1")
                        oggettoCustom.REPERTORIO = "1";
                    else
                        oggettoCustom.REPERTORIO = "0";
                }
                if (dataSet.Tables[0].Columns.Contains("CHA_CONS_REPERTORIO"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["CHA_CONS_REPERTORIO"].ToString()) && dataSet.Tables[0].Rows[rowNumber]["CHA_CONS_REPERTORIO"].ToString() == "1")
                        oggettoCustom.CONS_REPERTORIO = "1";
                    else
                        oggettoCustom.CONS_REPERTORIO = "0";
                }

                if (dataSet.Tables[0].Columns.Contains("DA_VISUALIZZARE_RICERCA"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["DA_VISUALIZZARE_RICERCA"].ToString()) && dataSet.Tables[0].Rows[rowNumber]["DA_VISUALIZZARE_RICERCA"].ToString() == "1")
                        oggettoCustom.DA_VISUALIZZARE_RICERCA = "1";
                    else
                        oggettoCustom.DA_VISUALIZZARE_RICERCA = "0";
                }
                if (dataSet.Tables[0].Columns.Contains("ANNO"))
                    oggettoCustom.ANNO = dataSet.Tables[0].Rows[rowNumber]["ANNO"].ToString();
                if (dataSet.Tables[0].Columns.Contains("VALORE_OGGETTO_DB"))
                    oggettoCustom.VALORE_DATABASE = dataSet.Tables[0].Rows[rowNumber]["VALORE_OGGETTO_DB"].ToString();
                if (dataSet.Tables[0].Columns.Contains("ID_AOO_RF"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["ID_AOO_RF"].ToString()))
                        oggettoCustom.ID_AOO_RF = dataSet.Tables[0].Rows[rowNumber]["ID_AOO_RF"].ToString();
                    else
                        oggettoCustom.ID_AOO_RF = "0";
                }
                if (dataSet.Tables[0].Columns.Contains("FORMATO_ORA"))
                    oggettoCustom.FORMATO_ORA = dataSet.Tables[0].Rows[rowNumber]["FORMATO_ORA"].ToString();
                if (dataSet.Tables[0].Columns.Contains("TIPO_LINK"))
                    oggettoCustom.TIPO_LINK = dataSet.Tables[0].Rows[rowNumber]["TIPO_LINK"].ToString();
                if (dataSet.Tables[0].Columns.Contains("TIPO_OBJ_LINK"))
                    oggettoCustom.TIPO_OBJ_LINK = dataSet.Tables[0].Rows[rowNumber]["TIPO_OBJ_LINK"].ToString();
                if (dataSet.Tables[0].Columns.Contains("MODULO_SOTTOCONTATORE"))
                    oggettoCustom.MODULO_SOTTOCONTATORE = dataSet.Tables[0].Rows[rowNumber]["MODULO_SOTTOCONTATORE"].ToString();
                if (dataSet.Tables[0].Columns.Contains("VALORE_SC"))
                    oggettoCustom.VALORE_SOTTOCONTATORE = dataSet.Tables[0].Rows[rowNumber]["VALORE_SC"].ToString();
                if (dataSet.Tables[0].Columns.Contains("DTA_INS"))
                    oggettoCustom.DATA_INSERIMENTO = dataSet.Tables[0].Rows[rowNumber]["DTA_INS"].ToString();
                if (dataSet.Tables[0].Columns.Contains("DTA_ANNULLAMENTO"))
                    oggettoCustom.DATA_ANNULLAMENTO = dataSet.Tables[0].Rows[rowNumber]["DTA_ANNULLAMENTO"].ToString();
                if (dataSet.Tables[0].Columns.Contains("CODICE_DB"))
                    oggettoCustom.CODICE_DB = dataSet.Tables[0].Rows[rowNumber]["CODICE_DB"].ToString();
                if (dataSet.Tables[0].Columns.Contains("MANUAL_INSERT"))
                {
                    string value = dataSet.Tables[0].Rows[rowNumber]["MANUAL_INSERT"].ToString();
                    if (!string.IsNullOrEmpty(value) && "1".Equals(value))
                    {
                        oggettoCustom.MANUAL_INSERT = true;
                    }
                }
                if (dataSet.Tables[0].Columns.Contains("ANNO_ACC"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["ANNO_ACC"].ToString()))
                        oggettoCustom.ANNO_ACC = dataSet.Tables[0].Rows[rowNumber]["ANNO_ACC"].ToString();
                }
                // INTEGRAZIONE PITRE-PARER
                if (dataSet.Tables[0].Columns.Contains("CHA_CONSOLIDAMENTO"))
                {
                    string value = dataSet.Tables[0].Rows[rowNumber]["CHA_CONSOLIDAMENTO"].ToString();
                    if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                        oggettoCustom.CONSOLIDAMENTO = "1";
                    else
                        oggettoCustom.CONSOLIDAMENTO = "0";
                }
                if (dataSet.Tables[0].Columns.Contains("CHA_CONSERVAZIONE"))
                {
                    string value = dataSet.Tables[0].Rows[rowNumber]["CHA_CONSERVAZIONE"].ToString();
                    if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                        oggettoCustom.CONSERVAZIONE = "1";
                    else
                        oggettoCustom.CONSERVAZIONE = "0";
                }
                if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
                    this.ControllaCustom(ref oggettoCustom);

                
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - setOggettoCustom - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
            }
        }


        private void setOggettoCustom(ref DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, DataRow dataRow)
        {
            try
            {
                if (dataRow.Table.Columns.Contains("SYSTEM_ID"))
                    oggettoCustom.SYSTEM_ID = Convert.ToInt32(dataRow["SYSTEM_ID"].ToString());
                if (dataRow.Table.Columns.Contains("SYSTEM_ID_OGG_CUSTOM"))
                    oggettoCustom.SYSTEM_ID = Convert.ToInt32(dataRow["SYSTEM_ID_OGG_CUSTOM"].ToString());
                if (dataRow.Table.Columns.Contains("CAMPO_DI_RICERCA"))
                    oggettoCustom.CAMPO_DI_RICERCA = dataRow["CAMPO_DI_RICERCA"].ToString();
                if (dataRow.Table.Columns.Contains("CAMPO_OBBLIGATORIO"))
                {
                    oggettoCustom.CAMPO_OBBLIGATORIO = dataRow["CAMPO_OBBLIGATORIO"].ToString();
                    oggettoCustom.ASTERISCO_OBBLIGATORIETA = dataRow["CAMPO_OBBLIGATORIO"].ToString();
                }
                if (dataRow.Table.Columns.Contains("DESCRIZIONE"))
                    oggettoCustom.DESCRIZIONE = dataRow["DESCRIZIONE"].ToString();
                if (dataRow.Table.Columns.Contains("MULTILINEA"))
                    oggettoCustom.MULTILINEA = dataRow["MULTILINEA"].ToString();
                if (dataRow.Table.Columns.Contains("NUMERO_DI_CARATTERI"))
                    oggettoCustom.NUMERO_DI_CARATTERI = dataRow["NUMERO_DI_CARATTERI"].ToString();
                if (dataRow.Table.Columns.Contains("NUMERO_DI_LINEE"))
                    oggettoCustom.NUMERO_DI_LINEE = dataRow["NUMERO_DI_LINEE"].ToString();
                if (dataRow.Table.Columns.Contains("ORIZZONTALE_VERTICALE"))
                    oggettoCustom.ORIZZONTALE_VERTICALE = dataRow["ORIZZONTALE_VERTICALE"].ToString();
                if (dataRow.Table.Columns.Contains("POSIZIONE"))
                    oggettoCustom.POSIZIONE = dataRow["POSIZIONE"].ToString();
                if (dataRow.Table.Columns.Contains("RESET_ANNO"))
                    oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO = dataRow["RESET_ANNO"].ToString();
                if (dataRow.Table.Columns.Contains("FORMATO_CONTATORE"))
                    oggettoCustom.FORMATO_CONTATORE = dataRow["FORMATO_CONTATORE"].ToString();
                if (dataRow.Table.Columns.Contains("RICERCA_CORR"))
                    oggettoCustom.TIPO_RICERCA_CORR = dataRow["RICERCA_CORR"].ToString();
                if (dataRow.Table.Columns.Contains("ID_R_DEFAULT"))
                    oggettoCustom.ID_RUOLO_DEFAULT = dataRow["ID_R_DEFAULT"].ToString();
                if (dataRow.Table.Columns.Contains("CAMPO_COMUNE"))
                {
                    if (!string.IsNullOrEmpty(dataRow["CAMPO_COMUNE"].ToString()) && dataRow["CAMPO_COMUNE"].ToString() == "1")
                        oggettoCustom.CAMPO_COMUNE = "1";
                    else
                        oggettoCustom.CAMPO_COMUNE = "0";
                }
                if (dataRow.Table.Columns.Contains("CHA_TIPO_TAR"))
                {
                    if (!string.IsNullOrEmpty(dataRow["CHA_TIPO_TAR"].ToString()))
                        oggettoCustom.TIPO_CONTATORE = dataRow["CHA_TIPO_TAR"].ToString();
                }
                
                if (dataRow.Table.Columns.Contains("CONTA_DOPO"))
                {
                    if (!string.IsNullOrEmpty(dataRow["CONTA_DOPO"].ToString()) && dataRow["CONTA_DOPO"].ToString() == "1")
                        oggettoCustom.CONTA_DOPO = "1";
                    else
                        oggettoCustom.CONTA_DOPO = "0";
                }
                if (dataRow.Table.Columns.Contains("REPERTORIO"))
                {
                    if (!string.IsNullOrEmpty(dataRow["REPERTORIO"].ToString()) && dataRow["REPERTORIO"].ToString() == "1")
                        oggettoCustom.REPERTORIO = "1";
                    else
                        oggettoCustom.REPERTORIO = "0";
                }
                if (dataRow.Table.Columns.Contains("CHA_CONS_REPERTORIO"))
                {
                    if (!string.IsNullOrEmpty(dataRow["CHA_CONS_REPERTORIO"].ToString()) && dataRow["CHA_CONS_REPERTORIO"].ToString() == "1")
                        oggettoCustom.CONS_REPERTORIO = "1";
                    else
                        oggettoCustom.CONS_REPERTORIO = "0";
                }
                if (dataRow.Table.Columns.Contains("DA_VISUALIZZARE_RICERCA"))
                {
                    if (!string.IsNullOrEmpty(dataRow["DA_VISUALIZZARE_RICERCA"].ToString()) && dataRow["DA_VISUALIZZARE_RICERCA"].ToString() == "1")
                        oggettoCustom.DA_VISUALIZZARE_RICERCA = "1";
                    else
                        oggettoCustom.DA_VISUALIZZARE_RICERCA = "0";
                }
                if (dataRow.Table.Columns.Contains("ANNO"))
                    oggettoCustom.ANNO = dataRow["ANNO"].ToString();
                if (dataRow.Table.Columns.Contains("VALORE_OGGETTO_DB"))
                    oggettoCustom.VALORE_DATABASE = dataRow["VALORE_OGGETTO_DB"].ToString();
                if (dataRow.Table.Columns.Contains("ID_AOO_RF"))
                {
                    if (!string.IsNullOrEmpty(dataRow["ID_AOO_RF"].ToString()))
                        oggettoCustom.ID_AOO_RF = dataRow["ID_AOO_RF"].ToString();
                    else
                        oggettoCustom.ID_AOO_RF = "0";
                }
                if (dataRow.Table.Columns.Contains("FORMATO_ORA"))
                    oggettoCustom.FORMATO_ORA = dataRow["FORMATO_ORA"].ToString();
                if (dataRow.Table.Columns.Contains("TIPO_LINK"))
                    oggettoCustom.TIPO_LINK = dataRow["TIPO_LINK"].ToString();
                if (dataRow.Table.Columns.Contains("TIPO_OBJ_LINK"))
                    oggettoCustom.TIPO_OBJ_LINK = dataRow["TIPO_OBJ_LINK"].ToString();
                if (dataRow.Table.Columns.Contains("MODULO_SOTTOCONTATORE"))
                    oggettoCustom.MODULO_SOTTOCONTATORE = dataRow["MODULO_SOTTOCONTATORE"].ToString();
                if (dataRow.Table.Columns.Contains("VALORE_SC"))
                    oggettoCustom.VALORE_SOTTOCONTATORE = dataRow["VALORE_SC"].ToString();
                if (dataRow.Table.Columns.Contains("DTA_INS"))
                    oggettoCustom.DATA_INSERIMENTO = dataRow["DTA_INS"].ToString();
                if (dataRow.Table.Columns.Contains("DTA_ANNULLAMENTO"))
                    oggettoCustom.DATA_ANNULLAMENTO = dataRow["DTA_ANNULLAMENTO"].ToString();
                if (dataRow.Table.Columns.Contains("CODICE_DB"))
                    oggettoCustom.CODICE_DB = dataRow["CODICE_DB"].ToString();
                if (dataRow.Table.Columns.Contains("MANUAL_INSERT"))
                {
                    string value = dataRow["MANUAL_INSERT"].ToString();
                    if (!string.IsNullOrEmpty(value) && "1".Equals(value))
                    {
                        oggettoCustom.MANUAL_INSERT = true;
                    }
                }
                if (dataRow.Table.Columns.Contains("ANNO_ACC"))
                {
                    if (!string.IsNullOrEmpty(dataRow["ANNO_ACC"].ToString()))
                        oggettoCustom.ANNO_ACC = dataRow["ANNO_ACC"].ToString();
                }
                if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE))
                    this.ControllaCustom(ref oggettoCustom);
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - setOggettoCustom - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
            }
        }

        private void setTipoOggetto(ref DocsPaVO.ProfilazioneDinamica.TipoOggetto tipoOggetto, DataSet dataSet, int rowNumber)
        {
            try
            {
                if (dataSet.Tables[0].Columns.Contains("SYSTEM_ID"))
                    tipoOggetto.SYSTEM_ID = Convert.ToInt32(dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID"].ToString());
                if (dataSet.Tables[0].Columns.Contains("SYSTEM_ID_TIPO_OGGETTO"))
                    tipoOggetto.SYSTEM_ID = Convert.ToInt32(dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID_TIPO_OGGETTO"].ToString());
                if (dataSet.Tables[0].Columns.Contains("DESCRIZIONE"))
                    tipoOggetto.DESCRIZIONE_TIPO = dataSet.Tables[0].Rows[rowNumber]["DESCRIZIONE"].ToString();
                if (dataSet.Tables[0].Columns.Contains("DESCRIZIONE_TIPO"))
                    tipoOggetto.DESCRIZIONE_TIPO = dataSet.Tables[0].Rows[rowNumber]["DESCRIZIONE_TIPO"].ToString();
                if (dataSet.Tables[0].Columns.Contains("ID_TIPO"))
                    tipoOggetto.SYSTEM_ID = Convert.ToInt32(dataSet.Tables[0].Rows[rowNumber]["ID_TIPO"].ToString());
                if (dataSet.Tables[0].Columns.Contains("DESCR_TIPO"))
                    tipoOggetto.DESCRIZIONE_TIPO = dataSet.Tables[0].Rows[rowNumber]["DESCR_TIPO"].ToString();
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - setTipoOggetto - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
            }
        }

        private void setTipoOggetto(ref DocsPaVO.ProfilazioneDinamica.TipoOggetto tipoOggetto, DataRow dataRow)
        {
            try
            {
                if (dataRow.Table.Columns.Contains("SYSTEM_ID") || dataRow.Table.Columns.Contains("SYSTEM_ID_TIPO_OGGETTO"))
                    tipoOggetto.SYSTEM_ID = Int32.Parse(dataRow["SYSTEM_ID"].ToString());
                if (dataRow.Table.Columns.Contains("DESCRIZIONE"))
                    tipoOggetto.DESCRIZIONE_TIPO = dataRow["DESCRIZIONE"].ToString();
                if (dataRow.Table.Columns.Contains("DESCRIZIONE_TIPO"))
                    tipoOggetto.DESCRIZIONE_TIPO = dataRow["DESCRIZIONE_TIPO"].ToString();
                if (dataRow.Table.Columns.Contains("ID_TIPO"))
                    tipoOggetto.SYSTEM_ID = Int32.Parse(dataRow["ID_TIPO"].ToString());
                if (dataRow.Table.Columns.Contains("DESCR_TIPO"))
                    tipoOggetto.DESCRIZIONE_TIPO = dataRow["DESCR_TIPO"].ToString();
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - setTipoOggetto - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
            }
        }

        private void setValoreOggetto(ref DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto, DataSet dataSet, int rowNumber)
        {
            try
            {
                if (dataSet.Tables[0].Columns.Contains("SYSTEM_ID"))
                    valoreOggetto.SYSTEM_ID = Convert.ToInt32(dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID"].ToString());
                if (dataSet.Tables[0].Columns.Contains("DESCRIZIONE_VALORE"))
                    valoreOggetto.DESCRIZIONE_VALORE = dataSet.Tables[0].Rows[rowNumber]["DESCRIZIONE_VALORE"].ToString();
                if (dataSet.Tables[0].Columns.Contains("VALORE"))
                    valoreOggetto.VALORE = dataSet.Tables[0].Rows[rowNumber]["VALORE"].ToString();
                if (dataSet.Tables[0].Columns.Contains("VALORE_DI_DEFAULT"))
                    valoreOggetto.VALORE_DI_DEFAULT = dataSet.Tables[0].Rows[rowNumber]["VALORE_DI_DEFAULT"].ToString();
                if (dataSet.Tables[0].Columns.Contains("COLOR_BG"))
                    valoreOggetto.COLOR_BG = dataSet.Tables[0].Rows[rowNumber]["COLOR_BG"].ToString();
                if (dataSet.Tables[0].Columns.Contains("ABILITATO"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["ABILITATO"].ToString()))
                        valoreOggetto.ABILITATO = Convert.ToInt32(dataSet.Tables[0].Rows[rowNumber]["ABILITATO"]);
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - setValoreOggetto - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
            }
        }

        private void setAssDocFascRuoli(ref DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli, DataSet dataSet, int rowNumber)
        {
            try
            {
                if (dataSet.Tables[0].Columns.Contains("ID_RUOLO"))
                    assDocFascRuoli.ID_GRUPPO = dataSet.Tables[0].Rows[rowNumber]["ID_RUOLO"].ToString();
                if (dataSet.Tables[0].Columns.Contains("ID_TEMPLATE"))
                    assDocFascRuoli.ID_TIPO_DOC_FASC = dataSet.Tables[0].Rows[rowNumber]["ID_TEMPLATE"].ToString();
                if (dataSet.Tables[0].Columns.Contains("ID_TIPO_DOC"))
                    assDocFascRuoli.ID_TIPO_DOC_FASC = dataSet.Tables[0].Rows[rowNumber]["ID_TIPO_DOC"].ToString();
                if (dataSet.Tables[0].Columns.Contains("ID_OGGETTO_CUSTOM"))
                    assDocFascRuoli.ID_OGGETTO_CUSTOM = dataSet.Tables[0].Rows[rowNumber]["ID_OGGETTO_CUSTOM"].ToString();
                if (dataSet.Tables[0].Columns.Contains("INS_MOD"))
                    assDocFascRuoli.INS_MOD_OGG_CUSTOM = dataSet.Tables[0].Rows[rowNumber]["INS_MOD"].ToString();
                if (dataSet.Tables[0].Columns.Contains("VIS"))
                    assDocFascRuoli.VIS_OGG_CUSTOM = dataSet.Tables[0].Rows[rowNumber]["VIS"].ToString();
                if (dataSet.Tables[0].Columns.Contains("DIRITTI"))
                    assDocFascRuoli.DIRITTI_TIPOLOGIA = dataSet.Tables[0].Rows[rowNumber]["DIRITTI"].ToString();
                if (dataSet.Tables[0].Columns.Contains("DEL_REP") && !String.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["DEL_REP"].ToString()))
                    assDocFascRuoli.ANNULLA_REPERTORIO = dataSet.Tables[0].Rows[rowNumber]["DEL_REP"].ToString();

            }
            catch (Exception ex)
            {
                logger.Debug("SQL - setAssDocFascRuoli - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
            }
        }

        public bool isInUseCampoComuneDoc(string idTemplate, string idCampoComune)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_IS_IN_USE_CAMPO_COMUNE_DOC");
                queryMng.setParam("idTemplate", idTemplate);
                queryMng.setParam("idOggetto", idCampoComune);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - isInUseCampoComuneDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - isInUseCampoComuneFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_useCampiComuni = new DataSet();
                dbProvider.ExecuteQuery(ds_useCampiComuni, commandText);
                if (ds_useCampiComuni.Tables[0].Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        public DocsPaVO.ProfilazioneDinamica.Templates impostaCampiComuniDoc(DocsPaVO.ProfilazioneDinamica.Templates modello, ArrayList campiComuni)
        {

            //Quando vengono associati uno o più campi comuni, vengono ricalcolte le posizioni dei campi non comuni
            //e i campi comuni selezionati vengono reinseriti sempre in coda alla lista
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //Prima di tutto verifico se il modello ha degli oggetti custom da salvare nel db, se si li inserisco 
                //in quanto mi serve un modello aggiornato per poter in seguito impostare le posizioni degli oggetti in modo corretto
                for (int i = 0; i < modello.ELENCO_OGGETTI.Count; i++)
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)modello.ELENCO_OGGETTI[i];
                    if (ogg.SYSTEM_ID == 0 || ogg.SYSTEM_ID == null)
                    {
                        //Ci sono oggetti nel modello non ancora inseriti nel db
                        //allora aggiorno il modello ed esco dal ciclo, mi basta fare l'aggiornamento una sola volta
                        aggiornaTemplate(modello);
                        break;
                    }

                }

                //Seleziono i system_id degli oggetti_custom comuni per questo specifico template
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CAMPI_COMUNI_DOC");
                queryMng.setParam("idTemplate", modello.SYSTEM_ID.ToString());
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - impostaCampiComuniDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - impostaCampiComuniDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_idCampiComuni = new DataSet();
                dbProvider.ExecuteQuery(ds_idCampiComuni, commandText);


                if (ds_idCampiComuni.Tables[0].Rows.Count != 0)
                {
                    //Rimuovo gli oggetti_custom comuni per questo specifico template
                    for (int i = 0; i < ds_idCampiComuni.Tables[0].Rows.Count; i++)
                    {
                        //Rimozione dalla PDA_ASSOCIAZIONE_TEMPLATES
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_CAMPI_COMUNI_DOC");
                        queryMng.setParam("idTemplate", modello.SYSTEM_ID.ToString());
                        queryMng.setParam("idOggettoCustom", ds_idCampiComuni.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - impostaCampiComuniDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - impostaCampiComuniDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                        //Rimozione dalla DPA_OGG_CUSTOM_COMP
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_OGG_CUSTOM_COMP");
                        queryMng.setParam("idTemplate", modello.SYSTEM_ID.ToString());
                        queryMng.setParam("idOggettoCustom", ds_idCampiComuni.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - impostaCampiComuniDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - impostaCampiComuniDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                    }
                }

                //Recupero il modello aggiornato per impostare correttamente le posizioni dei campi comuni
                DocsPaVO.ProfilazioneDinamica.Templates modelloAggiornato = this.getTemplateById(modello.SYSTEM_ID.ToString());
                int numeroElementiModello = modelloAggiornato.ELENCO_OGGETTI.Count;
                ////Reimposto le posizioni degli oggetti non comuni
                for (int i = 0; i < modelloAggiornato.ELENCO_OGGETTI.Count; i++)
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)modelloAggiornato.ELENCO_OGGETTI[i];
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_DPA_OGG_CUSTOM_COMP");
                    int newPosition = (i + 1);
                    queryMng.setParam("posizione", newPosition.ToString());
                    queryMng.setParam("idTemplate", modelloAggiornato.SYSTEM_ID.ToString());
                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - impostaCampiComuniDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - impostaCampiComuniDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                }

                //Inserisco gli oggetti_custom comuni selezionati per questo template
                for (int i = 0; i < campiComuni.Count; i++)
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)campiComuni[i];
                    numeroElementiModello++;

                    //Inserimento nella DPA_ASSOCIAZIONE_TEMPLATES
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASSOCIAZIONE_TEMPLATES");
                    queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASSOCIAZIONE_TEMPLATES"));
                    queryMng.setParam("ID_OGGETTO", oggCustom.SYSTEM_ID.ToString());
                    queryMng.setParam("ID_TEMPLATE", modello.SYSTEM_ID.ToString());
                    queryMng.setParam("Doc_Number", "");
                    queryMng.setParam("Valore_Oggetto_Db", "");
                    queryMng.setParam("CODICE_DB", "");
                    queryMng.setParam("MANUAL_INSERT", "0");
                    queryMng.setParam("Anno", System.DateTime.Now.Year.ToString());
                    if (oggCustom.ID_AOO_RF != null && oggCustom.ID_AOO_RF != "")
                        queryMng.setParam("idAooRf", oggCustom.ID_AOO_RF);
                    else
                        queryMng.setParam("idAooRf", "0");

                    //queryMng.setParam("valoreSc", "0");
                    //queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.GetDate());
                    queryMng.setParam("valoreSc", "NULL");
                    queryMng.setParam("dtaIns", "NULL");
                    queryMng.setParam("anno_acc", "");

                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - impostaCampiComuniDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - impostaCampiComuniDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);

                    //Verifico se il campo comune è già presente nella DPA_OGG_CUSTOM_COMP per il template interessato,
                    //in caso affermativo non devo inserirlo
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_OGGETTI_CUSTOM");
                    queryMng.setParam("idTemplate", modello.SYSTEM_ID.ToString());
                    queryMng.setParam("idOggettoCustom", oggCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - impostaCampiComuniDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - impostaCampiComuniDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    DataSet ds_campiComuniInPosizione = new DataSet();
                    dbProvider.ExecuteQuery(ds_campiComuniInPosizione, commandText);

                    if (ds_campiComuniInPosizione.Tables[0].Rows.Count == 0)
                    {
                        //Inserimento DPA_OGG_CUSTOM_COMP
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INSERT_DPA_OGG_CUSTOM_COMP");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_OGG_CUSTOM_COMP"));
                        queryMng.setParam("idTemplate", modello.SYSTEM_ID.ToString());
                        queryMng.setParam("idOggettoCustom", oggCustom.SYSTEM_ID.ToString());
                        queryMng.setParam("posizione", numeroElementiModello.ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - impostaCampiComuniDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - impostaCampiComuniDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                    }
                }


                //Recupero il modello completamente aggiornato da restituire
                DocsPaVO.ProfilazioneDinamica.Templates template = this.getTemplateById(modello.SYSTEM_ID.ToString());

                return template;
            }
            catch
            {
                dbProvider.RollbackTransaction();
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }

        }

        public DocsPaVO.ProfilazioneDinamica.Templates getTemplateCampiComuniById(DocsPaVO.utente.InfoUtente infoUtente, string idTemplate)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();

            try
            {
                //Selezione template
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE_2");
                queryMng.setParam("idTemplate", idTemplate);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplateCampiComuniById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplateCampiComuniById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_template = new DataSet();
                dbProvider.ExecuteQuery(ds_template, commandText);

                setTemplate(ref template, ds_template, 0);

                //Seleziono i SYSTEM_ID per gli oggettiCustom associati al template dalla DPA_ASSOCIAZIONE_TEMPLATES
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SYSTEM_ID_OGGETTI_CUSTOM_CAMPI_COMUNI");
                queryMng.setParam("idTemplate", idTemplate);
                queryMng.setParam("idAmm", infoUtente.idAmministrazione);
                queryMng.setParam("idRuolo", infoUtente.idGruppo);
                commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplateCampiComuniById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplateCampiComuniById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_systemId_oggettiCustom = new DataSet();
                dbProvider.ExecuteQuery(ds_systemId_oggettiCustom, commandText);

                //Cerco gli oggetti custom associati al template
                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;
                for (int j = 0; j < ds_systemId_oggettiCustom.Tables[0].Rows.Count; j++)
                {
                    oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_OGGETTI_CUSTOM");
                    queryMng.setParam("idOggettoCustom", ds_systemId_oggettiCustom.Tables[0].Rows[j]["ID_OGGETTO"].ToString());
                    queryMng.setParam("idTemplate", idTemplate);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateCampiComuniById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateCampiComuniById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                    DataSet ds_oggettoCustom = new DataSet();
                    dbProvider.ExecuteQuery(ds_oggettoCustom, commandText);
                    setOggettoCustom(ref oggettoCustom, ds_oggettoCustom, 0);

                    //Seleziono il tipo di oggetto
                    DocsPaVO.ProfilazioneDinamica.TipoOggetto tipoOggetto = new DocsPaVO.ProfilazioneDinamica.TipoOggetto();
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TIPI_OGGETTO_1");
                    queryMng.setParam("idOggettoCustom", ds_oggettoCustom.Tables[0].Rows[0]["ID_TIPO_OGGETTO"].ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateCampiComuniById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateCampiComuniById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                    DataSet ds_tipoOggetto = new DataSet();
                    dbProvider.ExecuteQuery(ds_tipoOggetto, commandText);
                    setTipoOggetto(ref tipoOggetto, ds_tipoOggetto, 0);

                    //Aggiungo il tipo oggetto all'oggettoCustom
                    oggettoCustom.TIPO = tipoOggetto;

                    //campo CLOB di configurazione
                    if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                    {
                        string config = dbProvider.GetLargeText("DPA_OGGETTI_CUSTOM", oggettoCustom.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST");
                        oggettoCustom.CONFIG_OBJ_EST = config;
                    }

                    //Seleziono i valori per l'oggettoCustom
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI");
                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateCampiComuniById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateCampiComuniById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                    DataSet ds_valoriOggetto = new DataSet();
                    dbProvider.ExecuteQuery(ds_valoriOggetto, commandText);

                    for (int k = 0; k < ds_valoriOggetto.Tables[0].Rows.Count; k++)
                    {
                        DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto();
                        setValoreOggetto(ref valoreOggetto, ds_valoriOggetto, k);
                        oggettoCustom.ELENCO_VALORI.Add(valoreOggetto);
                        oggettoCustom.VALORI_SELEZIONATI.Add("");
                    }
                    template.ELENCO_OGGETTI.Add(oggettoCustom);
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return template;
        }

        public DocsPaVO.ProfilazioneDinamica.OggettoCustom[] GetValuesDropDownList(string oggettoCustomId, string tipo)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaVO.ProfilazioneDinamica.OggettoCustom[] listaOgg = null;

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI");
                if (tipo.ToUpper().Equals("F"))
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI_FASC");
                queryMng.setParam("idOggettoCustom", oggettoCustomId);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - GetValuesDropDownList - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - GetValuesDropDownList - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_valuesDDL = new DataSet();
                dbProvider.ExecuteQuery(ds_valuesDDL, commandText);
                if (ds_valuesDDL.Tables[0].Rows.Count > 0)
                {
                    listaOgg = new DocsPaVO.ProfilazioneDinamica.OggettoCustom[ds_valuesDDL.Tables[0].Rows.Count];
                    for (int i = 0; i < ds_valuesDDL.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom oggCus = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                        oggCus.SYSTEM_ID = Convert.ToInt32(ds_valuesDDL.Tables[0].Rows[i]["SYSTEM_ID"]);
                        oggCus.VALORE_DATABASE = ds_valuesDDL.Tables[0].Rows[i]["VALORE"].ToString();
                        listaOgg[i] = oggCus;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - GetValuesDropDownList - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
                return null;
            }

            return listaOgg;
        }

        public DocsPaVO.ProfilazioneDinamica.Contatore[] GetValuesContatoriDoc(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaVO.ProfilazioneDinamica.Contatore[] listaContatori = null;

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_VALUES_CONTATORI_DOC");
                queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - GetValuesContatoriDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - GetValuesContatoriDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_valuesContatoriDoc = new DataSet();
                dbProvider.ExecuteQuery(ds_valuesContatoriDoc, commandText);
                if (ds_valuesContatoriDoc.Tables[0].Rows.Count > 0)
                {
                    listaContatori = new DocsPaVO.ProfilazioneDinamica.Contatore[ds_valuesContatoriDoc.Tables[0].Rows.Count];
                    for (int i = 0; i < ds_valuesContatoriDoc.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamica.Contatore contatore = new DocsPaVO.ProfilazioneDinamica.Contatore();
                        contatore.SYSTEM_ID = ds_valuesContatoriDoc.Tables[0].Rows[i]["SYSTEM_ID"].ToString();
                        contatore.ID_OGG = ds_valuesContatoriDoc.Tables[0].Rows[i]["ID_OGG"].ToString();
                        contatore.ID_TIPOLOGIA = ds_valuesContatoriDoc.Tables[0].Rows[i]["ID_TIPOLOGIA"].ToString();
                        contatore.ID_AOO = ds_valuesContatoriDoc.Tables[0].Rows[i]["ID_AOO"].ToString();
                        contatore.ID_RF = ds_valuesContatoriDoc.Tables[0].Rows[i]["ID_RF"].ToString();
                        contatore.VALORE = ds_valuesContatoriDoc.Tables[0].Rows[i]["VALORE"].ToString();
                        contatore.ABILITATO = ds_valuesContatoriDoc.Tables[0].Rows[i]["ABILITATO"].ToString();
                        contatore.ANNO = ds_valuesContatoriDoc.Tables[0].Rows[i]["ANNO"].ToString();
                        contatore.VALORE_SC = ds_valuesContatoriDoc.Tables[0].Rows[i]["VALORE_SC"].ToString();
                        contatore.CODICE_RF_AOO = ds_valuesContatoriDoc.Tables[0].Rows[i]["CODICE_RF_AOO"].ToString();
                        contatore.DESC_RF_AOO = ds_valuesContatoriDoc.Tables[0].Rows[i]["DESC_RF_AOO"].ToString();

                        listaContatori[i] = contatore;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - GetValuesContatoriDoc - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
                return null;
            }

            return listaContatori;
        }

        public void SetValuesContatoreDoc(DocsPaVO.ProfilazioneDinamica.Contatore contatore)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONT_DOC");
                queryMng.setParam("idContatore", contatore.SYSTEM_ID);
                queryMng.setParam("valoreContatore", contatore.VALORE);
                queryMng.setParam("valoreSottocontatore", contatore.VALORE_SC);
                queryMng.setParam("anno", contatore.ANNO);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - SetValuesContatoreDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - SetValuesContatoreDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - SetValuesContatoreDoc - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
            }
        }

        public void DeleteValueContatoreDoc(DocsPaVO.ProfilazioneDinamica.Contatore contatore)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_CONT_DOC");
                queryMng.setParam("system_id", contatore.SYSTEM_ID);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - DeleteValueContatoreDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - DeleteValueContatoreDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - DeleteValueContatoreDoc - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
            }
        }

        public void InsertValuesContatoreDoc(DocsPaVO.ProfilazioneDinamica.Contatore contatore)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_CONT_DOC");
                queryMng.setParam("system_id", contatore.SYSTEM_ID);

                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INSERT_CONT_DOC");
                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONTATORI_DOC"));
                queryMng.setParam("idOgg", contatore.ID_OGG);
                queryMng.setParam("idTipologia", contatore.ID_TIPOLOGIA);
                queryMng.setParam("idAoo", contatore.ID_AOO);
                queryMng.setParam("idRf", contatore.ID_RF);
                queryMng.setParam("valore", contatore.VALORE);
                queryMng.setParam("valoreSottocontatore", contatore.VALORE_SC);
                queryMng.setParam("abilitato", contatore.ABILITATO);
                queryMng.setParam("anno", contatore.ANNO);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - InsertValuesContatoreDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - InsertValuesContatoreDoc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - InsertValuesContatoreDoc - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
            }
        }

        /// <summary>
        /// Funzione per il recupero del nome di un modello a partire dal suo system id
        /// </summary>
        /// <param name="modelId">Id del modello di cui caricare il nome</param>
        /// <returns>Nome del modello</returns>
        public String GetModelNameById(String modelId)
        {
            // Valore da restituire
            String toReturn = String.Empty;

            using (DBProvider dbProvider = new DBProvider())
            {
                // Caricamento della query da eseguire
                Query query = DocsPaUtils.InitQuery.getInstance().getQuery("GET_DOC_MODEL_NAME");

                // Impostazione dell'id del modello
                query.setParam("modelId", modelId);

                // Dataset in cui verranno inseriti i risultati della query
                DataSet dataSet = new DataSet();

                // Esecuzione query
                if (dbProvider.ExecuteQuery(dataSet, query.getSQL()) &&
                    dataSet.Tables.Count == 1 &&
                    dataSet.Tables[0].Rows.Count == 1)
                    // Lettura del risultato
                    toReturn = dataSet.Tables[0].Rows[0][0].ToString();

            }

            // Restituzione del nome del template
            return toReturn;

        }

        public bool getContatoreTemplates(DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            bool result = false;

            if (template != null && template.ELENCO_OGGETTI != null)
            {
                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto in template.ELENCO_OGGETTI)
                {
                    switch (oggetto.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "Contatore":
                            result = true;
                            break;
                    }
                }
            }

            return result;
        }

        public DocsPaVO.ProfilazioneDinamica.Templates getTemplateDettagliFilterObjects(string docNumber, string idTipoAtto, string[] visibleFields)
        {
            logger.Info("BEGIN");
            DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();
            string idTemplate = null;
            string commandText = null;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DataSet ds_template = new DataSet();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_ID_TEMPLATE_FROM_DOC_NUMBER");
                queryMng.setParam("docNumber", docNumber);

                if (!string.IsNullOrEmpty(idTipoAtto))
                {
                    idTemplate = idTipoAtto;

                }
                else
                {
                    //Ricerco un determinato idTemplate a partire dal docNumber
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateDettagli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateDettagliFilterObjects - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    dbProvider.ExecuteQuery(ds_template, commandText);
                    if (ds_template.Tables[0].Rows.Count != 0)
                    {
                        idTemplate = ds_template.Tables[0].Rows[0]["ID_TEMPLATE"].ToString();
                    }
                }

                //Verifico, se esiste un idTemplate per quel docNumber lo carico, altrimenti carico un template vuoto
                //per il tipoAtto e l'amministrazione richiesti
                if (idTemplate != null)
                {
                    template.SYSTEM_ID = Convert.ToInt32(idTemplate);
                    //Recupero i dati del template
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TEMPLATE_DOC");
                    queryMng.setParam("idTemplate", idTemplate);
                    //queryMng.setParam("docNumber", docNumber);
                    queryMng.setParam("docNumber", " AND DPA_ASSOCIAZIONE_TEMPLATES.DOC_NUMBER  = " + docNumber + " ");
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateDettagliFilterObjects - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateDettagli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    DataSet ds_templateCompleto = new DataSet();
                    dbProvider.ExecuteQuery(ds_templateCompleto, commandText);

                    //Se il template non ha oggetti custom vengono restituite solo le proprietà del template
                    if (ds_templateCompleto.Tables[0].Rows.Count == 0)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE_2");
                        queryMng.setParam("idTemplate", idTemplate);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - getTemplateById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - getTemplateById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        ds_template = new DataSet();
                        dbProvider.ExecuteQuery(ds_template, commandText);

                        setTemplate(ref template, ds_template, 0);
                        return template;
                    }

                    setTemplate(ref template, ds_templateCompleto, 0);

                    //Cerco gli oggetti custom associati al template
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;

                    for (int j = 0; j < ds_templateCompleto.Tables[0].Rows.Count; j++)
                    {
                        oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                        setOggettoCustom(ref oggettoCustom, ds_templateCompleto, j);

                        if (visibleFields.Contains((oggettoCustom.SYSTEM_ID).ToString()))
                        {

                            DocsPaVO.ProfilazioneDinamica.TipoOggetto tipoOggetto = new DocsPaVO.ProfilazioneDinamica.TipoOggetto();
                            setTipoOggetto(ref tipoOggetto, ds_templateCompleto, j);

                            //Aggiungo il tipo oggetto all'oggettoCustom
                            oggettoCustom.TIPO = tipoOggetto;

                            //campo CLOB di configurazione
                            if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                            {
                                string config = dbProvider.GetLargeText("DPA_OGGETTI_CUSTOM", oggettoCustom.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST");
                                oggettoCustom.CONFIG_OBJ_EST = config;
                            }

                            if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione"))
                            {
                                //Recupero il valoreDb dell'oggetto
                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_VALORE_DB_OGGETTO_CUSTOM_1");
                                queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                                queryMng.setParam("docNumber", docNumber);
                                commandText = queryMng.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - getTemplateDettagli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                logger.Debug("SQL - getTemplateDettagli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                                DataSet ds_valore_database = new DataSet();
                                dbProvider.ExecuteQuery(ds_valore_database, commandText);

                                if (ds_valore_database.Tables[0].Rows.Count != 0)
                                {
                                    oggettoCustom.VALORE_DATABASE = ds_valore_database.Tables[0].Rows[0]["Valore_Oggetto_Db"].ToString();
                                    oggettoCustom.ANNO = ds_valore_database.Tables[0].Rows[0]["Anno"].ToString();
                                    if (ds_valore_database.Tables[0].Rows[0]["ID_AOO_RF"].ToString() != null && ds_valore_database.Tables[0].Rows[0]["ID_AOO_RF"].ToString() != "")
                                        oggettoCustom.ID_AOO_RF = ds_valore_database.Tables[0].Rows[0]["ID_AOO_RF"].ToString();
                                    else
                                        oggettoCustom.ID_AOO_RF = "0";
                                    for (int i = 0; i < ds_valore_database.Tables[0].Rows.Count; i++)
                                    {
                                        oggettoCustom.VALORI_SELEZIONATI.Add(ds_valore_database.Tables[0].Rows[i]["Valore_Oggetto_Db"].ToString());
                                    }
                                }
                            }

                            if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione") ||
                                oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("MenuATendina") ||
                                oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("SelezioneEsclusiva"))
                            {
                                //Selezioni i valori per l'oggettoCustom
                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI");
                                queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                                commandText = queryMng.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - getTemplateDettagli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                                logger.Debug("SQL - getTemplateDettagli - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                                DataSet ds_valoriOggetto = new DataSet();
                                dbProvider.ExecuteQuery(ds_valoriOggetto, commandText);

                                for (int k = 0; k < ds_valoriOggetto.Tables[0].Rows.Count; k++)
                                {
                                    if (oggettoCustom != null && !string.IsNullOrEmpty(oggettoCustom.TIPO.DESCRIZIONE_TIPO))
                                    {
                                        DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto();

                                        switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                                        {
                                            case "CasellaDiSelezione":
                                                if (k < oggettoCustom.VALORI_SELEZIONATI.Count)
                                                {
                                                    setValoreOggetto(ref valoreOggetto, ds_valoriOggetto, k);
                                                    oggettoCustom.ELENCO_VALORI.Add(valoreOggetto);
                                                }
                                                break;

                                            default:
                                                setValoreOggetto(ref valoreOggetto, ds_valoriOggetto, k);
                                                oggettoCustom.ELENCO_VALORI.Add(valoreOggetto);
                                                break;
                                        }
                                    }
                                }
                            }

                            template.ELENCO_OGGETTI.Add(oggettoCustom);
                        }
                    }
                }
                else
                {
                    return getTemplate(docNumber);
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                logger.Info("END");
                dbProvider.Dispose();
            }
            return template;
        }

        public DocsPaVO.ProfilazioneDinamica.Templates getTemplatePerRicercaById(string idAtto)
        {
            DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                ////DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE_1");
                //queryMng.setParam("idAtto", idAtto);
                ////string commandText = queryMng.getSQL();
                ////System.Diagnostics.Debug.WriteLine("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                ////logger.Debug("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                ////DataSet ds_template = new DataSet();
                ////dbProvider.ExecuteQuery(ds_template, commandText);



                //Recupero i dati del template
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TEMPLATE_DOC");
                queryMng.setParam("idTemplate", idAtto);
                //queryMng.setParam("docNumber", "''");
                queryMng.setParam("docNumber", " AND (DPA_ASSOCIAZIONE_TEMPLATES.DOC_NUMBER  = '' OR DPA_ASSOCIAZIONE_TEMPLATES.DOC_NUMBER IS NULL) ");
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_templateCompleto = new DataSet();
                dbProvider.ExecuteQuery(ds_templateCompleto, commandText);

                setTemplate(ref template, ds_templateCompleto, 0);
                template.DOC_NUMBER = "";

                //Cerco gli oggetti custom associati al template che sono campi di ricerca
                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;
                for (int j = 0; j < ds_templateCompleto.Tables[0].Rows.Count; j++)
                {
                    oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                    setOggettoCustom(ref oggettoCustom, ds_templateCompleto, j);

                    if (oggettoCustom.CAMPO_DI_RICERCA == "SI")
                    {
                        DocsPaVO.ProfilazioneDinamica.TipoOggetto tipoOggetto = new DocsPaVO.ProfilazioneDinamica.TipoOggetto();
                        setTipoOggetto(ref tipoOggetto, ds_templateCompleto, j);

                        //Aggiungo il tipo oggetto all'oggettoCustom
                        oggettoCustom.TIPO = tipoOggetto;

                        //campo CLOB di configurazione
                        if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                        {
                            string config = dbProvider.GetLargeText("DPA_OGGETTI_CUSTOM", oggettoCustom.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST");
                            oggettoCustom.CONFIG_OBJ_EST = config;
                        }

                        //Selezioni i valori per l'oggettoCustom
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI");
                        queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                        DataSet ds_valoriOggetto = new DataSet();
                        dbProvider.ExecuteQuery(ds_valoriOggetto, commandText);
                        for (int k = 0; k < ds_valoriOggetto.Tables[0].Rows.Count; k++)
                        {
                            DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto();
                            setValoreOggetto(ref valoreOggetto, ds_valoriOggetto, k);
                            oggettoCustom.ELENCO_VALORI.Add(valoreOggetto);
                        }
                        template.ELENCO_OGGETTI.Add(oggettoCustom);
                    }

                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return template;
        }

        /// <summary>
        /// Metodo che preleva le informazioni basilari dei templates per documenti in un'amministrazione
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public TemplateLite[] getListTemplatesLite(string idAmministrazione)
        {
            TemplateLite[] result = null;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //Selezione dalla DPA_TEMPLATES in relazione con la DPA_TEMPLATES_COMPONENT
                //per la selezione dei template associati ad una specifica amministrazione
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE_WITH_DIAGRAMMA");
                queryMng.setParam("idAmministrazione", idAmministrazione);
                queryMng.setParam("doc", "LEFT JOIN dpa_ass_diagrammi di ON di.ID_TIPO_DOC= d.SYSTEM_ID");
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getListTemplatesLite - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplates - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                DataSet ds_templates = new DataSet();
                dbProvider.ExecuteQuery(ds_templates, commandText);

                if (ds_templates != null && ds_templates.Tables[0].Rows.Count > 0)
                {
                    result = new TemplateLite[ds_templates.Tables[0].Rows.Count];

                    for (int i = 0; i < ds_templates.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamicaLite.TemplateLite template = new DocsPaVO.ProfilazioneDinamicaLite.TemplateLite();
                        setTemplateLite(ref template, ds_templates, i);
                        result[i] = template;
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }


            return result;
        }

        /// <summary>
        /// Metodo che preleva le informazioni basilari dei templates per documenti, visibili da un determinato ruolo.
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public TemplateLite[] getListTemplatesLiteByRole(string idAmministrazione, string idRuolo)
        {
            TemplateLite[] result = null;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TEMPLATES_DOC_LITE_BY_ROLE");
                queryMng.setParam("idAmm", idAmministrazione);
                queryMng.setParam("idRuolo", idRuolo);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getListTemplatesLiteByRole - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getListTemplatesLiteByRole - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                DataSet ds_templates = new DataSet();
                dbProvider.ExecuteQuery(ds_templates, commandText);

                if (ds_templates != null && ds_templates.Tables[0].Rows.Count > 0)
                {
                    result = new TemplateLite[ds_templates.Tables[0].Rows.Count];

                    for (int i = 0; i < ds_templates.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamicaLite.TemplateLite template = new DocsPaVO.ProfilazioneDinamicaLite.TemplateLite();
                        setTemplateLite(ref template, ds_templates, i);
                        result[i] = template;
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }


            return result;
        }

        public TemplateLite[] getListTemplatesLiteFasc(string idAmministrazione)
        {
            TemplateLite[] result = null;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //Selezione dalla DPA_TEMPLATES in relazione con la DPA_TEMPLATES_COMPONENT
                //per la selezione dei template associati ad una specifica amministrazione
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE_FASC_WITH_DIAGRAMMA");
                queryMng.setParam("idAmministrazione", idAmministrazione);
                queryMng.setParam("doc", "LEFT JOIN dpa_ass_diagrammi di ON di.ID_TIPO_FASC= d.SYSTEM_ID");
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getListTemplatesLiteFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplates - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                DataSet ds_templates = new DataSet();
                dbProvider.ExecuteQuery(ds_templates, commandText);

                if (ds_templates != null && ds_templates.Tables[0].Rows.Count > 0)
                {
                    result = new TemplateLite[ds_templates.Tables[0].Rows.Count];

                    for (int i = 0; i < ds_templates.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamicaLite.TemplateLite template = new DocsPaVO.ProfilazioneDinamicaLite.TemplateLite();
                        setTemplateFascLite(ref template, ds_templates, i);
                        result[i] = template;
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }


            return result;
        }

        /// <summary>
        /// Metodo che preleva le informazioni basilari dei templates per fascicoli, visibili da un determinato ruolo.
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public TemplateLite[] getListTemplatesLiteFascByRole(string idAmministrazione, string idRuolo)
        {
            TemplateLite[] result = null;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TEMPLATES_FASC_LITE_BY_ROLE");
                queryMng.setParam("idAmm", idAmministrazione);
                queryMng.setParam("idRuolo", idRuolo);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getListTemplatesLiteFascByRole - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getListTemplatesLiteFascByRole - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                DataSet ds_templates = new DataSet();
                dbProvider.ExecuteQuery(ds_templates, commandText);

                if (ds_templates != null && ds_templates.Tables[0].Rows.Count > 0)
                {
                    result = new TemplateLite[ds_templates.Tables[0].Rows.Count];

                    for (int i = 0; i < ds_templates.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamicaLite.TemplateLite template = new DocsPaVO.ProfilazioneDinamicaLite.TemplateLite();
                        setTemplateFascLite(ref template, ds_templates, i);
                        result[i] = template;
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }


            return result;
        }

        private void setTemplateLite(ref DocsPaVO.ProfilazioneDinamicaLite.TemplateLite template, DataSet dataSet, int rowNumber)
        {
            try
            {
                if (dataSet.Tables[0].Columns.Contains("SYSTEM_ID"))
                {
                    template.system_id = dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID"].ToString();
                }
                if (dataSet.Tables[0].Columns.Contains("VAR_DESC_ATTO"))
                {
                    template.name = dataSet.Tables[0].Rows[rowNumber]["VAR_DESC_ATTO"].ToString();
                }
                if (dataSet.Tables[0].Columns.Contains("ID_DIAGRAMMA"))
                {
                    template.idDiagram = dataSet.Tables[0].Rows[rowNumber]["ID_DIAGRAMMA"].ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - setTemplate - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
            }
        }

        private void setTemplateFascLite(ref DocsPaVO.ProfilazioneDinamicaLite.TemplateLite template, DataSet dataSet, int rowNumber)
        {
            try
            {
                if (dataSet.Tables[0].Columns.Contains("SYSTEM_ID"))
                {
                    template.system_id = dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID"].ToString();
                }
                if (dataSet.Tables[0].Columns.Contains("VAR_DESC_FASC"))
                {
                    template.name = dataSet.Tables[0].Rows[rowNumber]["VAR_DESC_FASC"].ToString();
                }
                if (dataSet.Tables[0].Columns.Contains("ID_DIAGRAMMA"))
                {
                    template.idDiagram = dataSet.Tables[0].Rows[rowNumber]["ID_DIAGRAMMA"].ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - setTemplate - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
            }
        }

        #region STORICO
        /// <summary>
        /// Metodo di storicizzazione dei campi profilati
        /// </summary>
        /// <param name="oggettoCustom"></param>
        /// <param name="listOldObj"></param>
        /// <param name="rowsEffected"></param>
        /// <param name="indexListOldObj"></param>
        public void insertInStorico(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, ArrayList listOldObj, int rowsEffected, ref int indexListOldObj)
        {
            logger.Info("BEGIN");
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                {
                    case "CampoDiTesto":
                    case "MenuATendina":
                    case "SelezioneEsclusiva":
                    case "Data":
                    case "Corrispondente":
                    case "CasellaDiSelezione":
                        indexListOldObj++;
                        if (rowsEffected > 0)
                        {
                            //aggiungo il vecchio valore del campo profilato nello storico
                            //string commandText = string.Empty;
                            DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue ogg = (DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue)listOldObj[indexListOldObj];
                            //DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_PROFIL_STO");
                            //if (dbProvider.DBType.ToUpper().Equals("ORACLE"))
                            //    queryMng.setParam("param0", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_PROFIL_STO"));
                            //queryMng.setParam("param1", ogg.IDTemplate);
                            //queryMng.setParam("param2", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                            //queryMng.setParam("param3", ogg.ID_Doc_Fasc);
                            //queryMng.setParam("param4", ogg.ID_Oggetto);
                            //queryMng.setParam("param5", ogg.ID_People);
                            //queryMng.setParam("param6", ogg.ID_Ruolo_In_UO);
                            //queryMng.setParam("param7", ogg.Valore);
                            //commandText = queryMng.getSQL();
                            //System.Diagnostics.Debug.WriteLine("SQL - insertInStorico - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            //logger.Debug("SQL - insertInStorico - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            //dbProvider.ExecuteNonQuery(commandText);

                            bool canInsert = false;
                            // Il valore va inserito nello storico solo se il campo ha subito modifiche
                            if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "CasellaDiSelezione")
                            {
                                var flattenValue = ((String[])oggettoCustom.VALORI_SELEZIONATI.ToArray(typeof(String))).Aggregate((a, b) => String.Format("{0}*#?{1}", a, b)).ToString();
                                canInsert = !ogg.Valore.Equals(flattenValue);
                            }
                            else
                                canInsert = !ogg.Valore.Equals(oggettoCustom.VALORE_DATABASE);

                            if (canInsert)
                            {
                                ArrayList parameters = new ArrayList();
                                parameters.Add(new ParameterSP("objType", "D", DirectionParameter.ParamInput));
                                parameters.Add(new ParameterSP("Idtemplate", ogg.IDTemplate, DirectionParameter.ParamInput));
                                parameters.Add(new ParameterSP("idDocOrFasc", ogg.ID_Doc_Fasc, DirectionParameter.ParamInput));
                                parameters.Add(new ParameterSP("Idoggcustom", ogg.ID_Oggetto, DirectionParameter.ParamInput));
                                parameters.Add(new ParameterSP("Idpeople", ogg.ID_People, DirectionParameter.ParamInput));
                                parameters.Add(new ParameterSP("Idruoloinuo", ogg.ID_Ruolo_In_UO, DirectionParameter.ParamInput));
                                parameters.Add(new ParameterSP("Descmodifica", ogg.Valore, DirectionParameter.ParamInput));

                                dbProvider.ExecuteStoreProcedure("InsertDataInHistoryProf", parameters);
                            }

                        }
                        break;
                }
            }
            catch { }
            finally
            {
                dbProvider.Dispose();
                logger.Info("END");
            }
        }

        /// <summary>
        /// Funzione per il recupero dello storico di un modello inserito in un documento
        /// </summary>
        public ArrayList getListaStoricoAtto(string id_tipo_atto, string doc_number, string idGroup)
        {
            ArrayList storico = new ArrayList();
            using (DBProvider dbProvider = new DBProvider())
            {
                // Caricamento della query da eseguire
                Query query = DocsPaUtils.InitQuery.getInstance().getQuery("GET_STO_DOCUMENTO");

                // Impostazione dell'id del modello
                query.setParam("id_template", id_tipo_atto);
                //impostazione dell'id del documento al quale è associato il modello
                query.setParam("id_profile", doc_number);
                if (dbProvider.DBType.ToLower().Equals("oracle"))
                    query.setParam("id_ruolo", idGroup);
                // Dataset in cui verranno inseriti i risultati della query
                DataSet dataSet = new DataSet();
                dbProvider.ExecuteQuery(dataSet, query.getSQL());
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamica.StoricoProfilati recordStorico = new DocsPaVO.ProfilazioneDinamica.StoricoProfilati();
                        Utenti ut = new Utenti();
                        recordStorico.dta_modifica = dataSet.Tables[0].Rows[i]["dta_modifica"].ToString();
                        recordStorico.utente = ut.GetUtente(dataSet.Tables[0].Rows[i]["id_people"].ToString());
                        recordStorico.ruolo = ut.GetRuolo(dataSet.Tables[0].Rows[i]["id_ruolo_in_uo"].ToString());
                        recordStorico.oggetto = getOggettoById(dataSet.Tables[0].Rows[i]["id_ogg_custom"].ToString());
                        recordStorico.var_desc_modifica = dataSet.Tables[0].Rows[i]["var_desc_modifica"].ToString();
                        storico.Add(recordStorico);
                    }
                }

                // Restituzione dei campi modificati
                return storico;
            }
        }

        /// <summary>
        /// Metodo per l'attivazione dello storico per determinati campi profilati
        /// </summary>
        /// <param name="templateId">Id del template cui appartengono i campi da gestire</param>
        /// <param name="selectedFiels">Lista con informazioni sui campi per cui abilitare / disabilitare lo storico</param>
        /// <returns>Esito dell'operazione</returns>
        public bool ActiveSelectiveHistory(String templateId, List<CustomObjHistoryState> selectedFiels)
        {
            bool retVal = false;

            using (DBProvider dbProvider = new DBProvider())
            {
                // Disattivazione di tutti gli storici relativi alla tipologia
                if (this.DeactivateAllHistory(templateId))
                {
                    // Attivazione dello storico per specifici campi
                    Query query = InitQuery.getInstance().getQuery("U_ENABLE_DISABLE_PROF_HISTORY_SELECTIVE");
                    query.setParam("fasc", String.Empty);
                    query.setParam("value", "1");

                    //Emanuela 13-02-2015: aggiunto la condizione seguente per eccezione nel caso in cui selectedFiels = 0
                    if (selectedFiels.Count > 0)
                    {
                        String[] idList = selectedFiels.Select(f => f.FieldId.ToString()).ToArray<String>();
                        query.setParam("idList", idList.Aggregate((a, b) => a + ", " + b).ToString());

                        retVal = dbProvider.ExecuteNonQuery(query.getSQL());
                    }
                    else
                    {
                        return true;
                    }
                }
                return retVal;
            }

        }



        /// <summary>
        /// Metodo per l'attivazione dello storico per tutti i campi profilati di una tipologia
        /// </summary>
        /// <param name="templateId">Id del template cui appartengono i campi da gestire</param>
        /// <returns>Esito dell'operazione</returns>
        public bool ActiveSelectiveHistory(String templateId)
        {
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("U_DISABLE_ENABLE_ALL_PROF_HISTORY");
                query.setParam("fasc", String.Empty);
                query.setParam("value", "1");
                query.setParam("idTemplate", templateId);

                return dbProvider.ExecuteNonQuery(query.getSQL());
            }

        }

        public bool ActiveSelectiveHistoryFasc(String templateId)
        {
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("U_DISABLE_ENABLE_ALL_PROF_HISTORY_FASC");
                query.setParam("value", "1");
                query.setParam("idTemplate", templateId);

                return dbProvider.ExecuteNonQuery(query.getSQL());
            }

        }

        public bool ActiveSelectiveHistoryFasc(String templateId, List<CustomObjHistoryState> selectedFiels)
        {
            bool retVal = false;

            using (DBProvider dbProvider = new DBProvider())
            {
                // Disattivazione di tutti gli storici relativi alla tipologia
                if (this.DeactivateAllHistoryFasc(templateId))
                {
                    // Attivazione dello storico per specifici campi
                    Query query = InitQuery.getInstance().getQuery("U_ENABLE_DISABLE_PROF_HISTORY_SELECTIVE_FASC");
                    query.setParam("fasc", String.Empty);
                    query.setParam("value", "1");

                    //Emanuela 13-02-2015: aggiunto la condizione seguente per eccezione nel caso in cui selectedFiels = 0
                    if (selectedFiels.Count > 0)
                    {
                        String[] idList = selectedFiels.Select(f => f.FieldId.ToString()).ToArray<String>();
                        query.setParam("idList", idList.Aggregate((a, b) => a + ", " + b).ToString());

                        retVal = dbProvider.ExecuteNonQuery(query.getSQL());
                    }
                    else
                    {
                        return true;
                    }
                }
                return retVal;
            }

        }
        /// <summary>
        /// Metodo per la disabilitazione dello storico relativo a tutti i campi di una tipologia
        /// </summary>
        /// <param name="templateId">Id della tipologia per cui disabilitare lo storico</param>
        /// <returns>Esito dell'operazione</returns>
        public bool DeactivateAllHistory(String templateId)
        {
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("U_DISABLE_ENABLE_ALL_PROF_HISTORY");
                query.setParam("fasc", String.Empty);
                query.setParam("value", "0");
                query.setParam("idTemplate", templateId);

                return dbProvider.ExecuteNonQuery(query.getSQL());
            }
        }

        public bool DeactivateAllHistoryFasc(String templateId)
        {
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("U_DISABLE_ENABLE_ALL_PROF_HISTORY_FASC");
                query.setParam("fasc", String.Empty);
                query.setParam("value", "0");
                query.setParam("idTemplate", templateId);

                return dbProvider.ExecuteNonQuery(query.getSQL());
            }
        }

        /// <summary>
        /// Metodo per il recupero della lista con le informazioni relative allo
        /// stato di abilitazione dello storico per i campi di una determinata tipologia
        /// </summary>
        /// <param name="templateId">Id della tipologia di cui caricare le informazioni</param>
        /// <returns>Lista di oggetti con le informazioni sullo stato di abilitazione dello storico per i campi che compongono la tipologia</returns>
        public List<CustomObjHistoryState> GetCustomHistoryList(String templateId)
        {
            List<CustomObjHistoryState> result = new List<CustomObjHistoryState>();

            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_HISTORY_PROF_STATE");
                query.setParam("fasc", String.Empty);
                query.setParam("idTemplate", templateId);
                string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
                if (dbType.ToUpper() == "SQL")
                    query.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL());

                while (dataReader.Read())
                {
                    string descrizione = dataReader["descrizione"].ToString();
                    if (dataReader["attivo"].ToString().Equals("0"))
                        descrizione += " (disabilitato)";
                    result.Add(new CustomObjHistoryState()
                    {
                        FieldId = Convert.ToInt32(dataReader["system_id"]),
                        Description = descrizione,
                        Enabled = dataReader["EnabledHistory"] != DBNull.Value && Convert.ToInt32(dataReader["EnabledHistory"]) == 1
                    });
                }
            }

            return result;

        }

        public List<CustomObjHistoryState> GetCustomHistoryListFasc(String templateId)
        {
            List<CustomObjHistoryState> result = new List<CustomObjHistoryState>();

            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_HISTORY_PROF_STATE_FASC");
                query.setParam("idTemplate", templateId);
                string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
                if (dbType.ToUpper() == "SQL")
                    query.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL());

                while (dataReader.Read())
                {
                    string descrizione = dataReader["descrizione"].ToString();
                    if (dataReader["attivo"].ToString().Equals("0"))
                        descrizione += " (disabilitato)";
                    result.Add(new CustomObjHistoryState()
                    {
                        FieldId = Convert.ToInt32(dataReader["system_id"]),
                        Description = descrizione,
                        Enabled = dataReader["EnabledHistory"] != DBNull.Value && Convert.ToInt32(dataReader["EnabledHistory"]) == 1
                    });
                }
            }

            return result;

        }

        #endregion

        public void AnnullaContatoreDiRepertorio(string idOggetto, string docNumber)
        {
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SET_DTA_ANNULLAMENTO_REPERTORIO_DOC");
                queryMng.setParam("idOggetto", idOggetto);
                queryMng.setParam("docNumber", docNumber);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - AnnullaContatoreDiRepertorio - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - AnnullaContatoreDiRepertorio - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - AnnullaContatoreDiRepertorio - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
            }
        }

        public void Storicizza(DocsPaVO.ProfilazioneDinamica.Storicizzazione storico)
        {
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INS_STORICO_DOC");
                //queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryMng.setParam("colID", "SYSTEMID,");
                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_PROFIL_STO"));
                queryMng.setParam("idTemplate", storico.ID_TEMPLATE);
                queryMng.setParam("dtaModifica", DocsPaDbManagement.Functions.Functions.ToDate(storico.DATA_MODIFICA));
                queryMng.setParam("idProfile", storico.ID_PROFILE);
                queryMng.setParam("idOggCustom", storico.ID_OGG_CUSTOM);
                queryMng.setParam("idPeople", storico.ID_PEOPLE);
                queryMng.setParam("idRuoloInUo", storico.ID_RUOLO_IN_UO);
                queryMng.setParam("varDescModifica", "Motivo annullamento: " + storico.DESC_MODIFICA);
                string commandText = queryMng.getSQL();

                System.Diagnostics.Debug.WriteLine("SQL - Storicizza - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - Storicizza - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                dbProvider.ExecuteNonQuery(commandText);
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - Storicizza - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
            }
        }

        public String GetTipologyDescriptionByDocNumber(String docNumber)
        {
            Query query = InitQuery.getInstance().getQuery("S_DOC_TIPOLOGY_DESC_BY_DOC_NUMBER");
            query.setParam("docNumber", docNumber);

            String retVal = String.Empty;
            using (DBProvider dbProvider = new DBProvider())
            {
                IDataReader reader = dbProvider.ExecuteReader(query.getSQL());
                while (reader.Read())
                    retVal = reader[0].ToString();
            }

            return retVal;
        }

        public String GetTipologyDescriptionByIdProfile(String idProfile)
        {
            Query query = InitQuery.getInstance().getQuery("S_DOC_TIPOLOGY_DESC_BY_ID_PROFILE");
            query.setParam("idProfile", idProfile);

            String retVal = String.Empty;
            using (DBProvider dbProvider = new DBProvider())
            {
                IDataReader reader = dbProvider.ExecuteReader(query.getSQL());
                while (reader.Read())
                    retVal = reader[0].ToString();
            }

            return retVal;
        }

        public void RemoveTipologyDoc(DocsPaVO.utente.InfoUtente info, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    string command = "UPDATE PROFILE SET ID_TIPO_ATTO = NULL WHERE DOCNUMBER = " + schedaDocumento.docNumber;
                    int rowsAffected = 0;
                    logger.Debug("Eliminazione ID_TIPO_ATTO per il documento con DOCNUMBER : " + schedaDocumento.docNumber);
                    logger.Debug("RemoveTipologyDoc QUERY : " + command);
                    dbProvider.ExecuteNonQuery(command, out rowsAffected);
                    if (rowsAffected != 0)
                    {
                        command = "DELETE DPA_ASSOCIAZIONE_TEMPLATES WHERE DOC_NUMBER = " + schedaDocumento.docNumber;
                        logger.Debug("Eliminazione dei campi di tipologia per il documento con DOCNUMBER : " + schedaDocumento.docNumber);
                        logger.Debug("RemoveTipologyDoc QUERY : " + command);
                        dbProvider.ExecuteNonQuery(command);
                    }
                }
                catch (Exception ex)
                {
                    logger.Debug("RemoveTipologyDoc - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
                }
                finally
                {
                    dbProvider.Dispose();
                }
            }
        }

        public DocsPaVO.ProfilazioneDinamica.Templates getTemplateByDescrizione(string descrizioneTemplate, string idAmm)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();

            try
            {
                //Recupero i dati del template
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TEMPLATE_DOC_BY_DESC");
                queryMng.setParam("descrizioneTemplate", descrizioneTemplate);
                //queryMng.setParam("docNumber", "''");
                queryMng.setParam("docNumber", " AND (DPA_ASSOCIAZIONE_TEMPLATES.DOC_NUMBER  = '' OR DPA_ASSOCIAZIONE_TEMPLATES.DOC_NUMBER IS NULL) AND DPA_TIPO_ATTO.ID_AMM = "+idAmm+" ");
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplateById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplateById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                DataSet ds_template = new DataSet();
                dbProvider.ExecuteQuery(ds_template, commandText);

                //Se il template non ha oggetti custom vengono restituite solo le proprietà del template
                if (ds_template.Tables[0].Rows.Count == 0)
                {
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE_2_BY_DESC");
                    queryMng.setParam("descrizioneTemplate", descrizioneTemplate);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    ds_template = new DataSet();
                    dbProvider.ExecuteQuery(ds_template, commandText);

                    setTemplate(ref template, ds_template, 0);
                    return template;
                }

                setTemplate(ref template, ds_template, 0);

                //Cerco gli oggetti custom associati al template
                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;
                for (int j = 0; j < ds_template.Tables[0].Rows.Count; j++)
                {
                    //Imposto i valori degli oggetti custome dei tipi oggetto
                    oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                    setOggettoCustom(ref oggettoCustom, ds_template, j);

                    DocsPaVO.ProfilazioneDinamica.TipoOggetto tipoOggetto = new DocsPaVO.ProfilazioneDinamica.TipoOggetto();
                    setTipoOggetto(ref tipoOggetto, ds_template, j);

                    //Aggiungo il tipo oggetto all'oggettoCustom
                    oggettoCustom.TIPO = tipoOggetto;

                    //campo CLOB di configurazione
                    if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                    {
                        string config = dbProvider.GetLargeText("DPA_OGGETTI_CUSTOM", oggettoCustom.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST");
                        oggettoCustom.CONFIG_OBJ_EST = config;
                    }

                    //Selezioni i valori per l'oggettoCustom
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI");
                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateById - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplates - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);

                    DataSet ds_valoriOggetto = new DataSet();
                    dbProvider.ExecuteQuery(ds_valoriOggetto, commandText);

                    for (int k = 0; k < ds_valoriOggetto.Tables[0].Rows.Count; k++)
                    {
                        DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = new DocsPaVO.ProfilazioneDinamica.ValoreOggetto();
                        setValoreOggetto(ref valoreOggetto, ds_valoriOggetto, k);
                        oggettoCustom.ELENCO_VALORI.Add(valoreOggetto);
                        oggettoCustom.VALORI_SELEZIONATI.Add("");
                    }
                    template.ELENCO_OGGETTI.Add(oggettoCustom);
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return template;
        }

        public bool UpdateIsTypeInstance(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione)
        {
            bool result = true;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TIPO_ATTO_TYPE_INSTANCE");
                queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                queryMng.setParam("idAmm", idAmministrazione);
                queryMng.setParam("paramIsTypeInstance", template.IS_TYPE_INSTANCE.ToString());
                string commandText = queryMng.getSQL();
                result = dbProvider.ExecuteNonQuery(commandText);
                if (result && template.IS_TYPE_INSTANCE == '1')
                {
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_TIPO_ATTO_NO_TYPE_INSTANCE");
                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                    queryMng.setParam("idAmm", idAmministrazione);
                    commandText = queryMng.getSQL();
                    if (!dbProvider.ExecuteNonQuery(commandText))
                        result = false;
                }
            }
            catch (Exception exc)
            {
                logger.Info("Errore in DocsPaDb.Query_DocsPAWS.Models - Metodo UpdateIsTypeInstance", exc);
                return false;
            }
            return result;
        }

        public ArrayList getIdDocByAssTemplates(string idOggetto, string valoreDB, string ordine)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            ArrayList retVal = new ArrayList();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_IDDOC_BY_ASS_TEMPLATES");
                q.setParam("id_ogg_custom", idOggetto);
                q.setParam("valore_oggetto_db", valoreDB);
                if (string.IsNullOrEmpty(ordine) || ordine.ToUpper() != "DESC")
                    q.setParam("ordine", "ASC");
                else q.setParam("ordine", "DESC");

                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataset = new DataSet();
                dbProvider.ExecuteQuery(out dataset, "IDDOCS", queryString);
                if (dataset.Tables["IDDOCS"] != null && dataset.Tables["IDDOCS"].Rows.Count > 0)
                {
                    foreach (DataRow r in dataset.Tables["IDDOCS"].Rows)
                    {
                        retVal.Add(r["DOC_NUMBER"].ToString());
                    }
                }
                if (retVal.Count < 1) retVal = null;

            }
            catch (Exception ex)
            {
                retVal = null;
                logger.Error(ex);
            }

            return retVal;

        }

        public ArrayList getIdDocByAssTemplates(string idOggetto, string valoreDB, string ordine, string idTemplate)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            ArrayList retVal = new ArrayList();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_IDDOC_BY_ASS_TEMPLATES_2");
                q.setParam("id_ogg_custom", idOggetto);
                q.setParam("id_template", idTemplate);
                q.setParam("valore_oggetto_db", valoreDB);
                if (string.IsNullOrEmpty(ordine) || ordine.ToUpper() != "DESC")
                    q.setParam("ordine", "ASC");
                else q.setParam("ordine", "DESC");

                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataset = new DataSet();
                dbProvider.ExecuteQuery(out dataset, "IDDOCS", queryString);
                if (dataset.Tables["IDDOCS"] != null && dataset.Tables["IDDOCS"].Rows.Count > 0)
                {
                    foreach (DataRow r in dataset.Tables["IDDOCS"].Rows)
                    {
                        retVal.Add(r["DOC_NUMBER"].ToString());
                    }
                }
                if (retVal.Count < 1) retVal = null;

            }
            catch (Exception ex)
            {
                retVal = null;
                logger.Error(ex);
            }

            return retVal;

        }

        public DocsPaVO.FlussoAutomatico.ContestoProcedurale GetContestoProceduraleById(string idContesto)
        {
            DocsPaVO.FlussoAutomatico.ContestoProcedurale contestoProcedurale = new DocsPaVO.FlussoAutomatico.ContestoProcedurale();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_CONTESTO_PROCEDURALE_BY_ID");
                q.setParam("idContesto", idContesto);

                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataset = new DataSet();
                dbProvider.ExecuteQuery(out dataset, "CONTESTO_PROCEDURALE", queryString);
                if (dataset.Tables["CONTESTO_PROCEDURALE"] != null && dataset.Tables["CONTESTO_PROCEDURALE"].Rows.Count > 0)
                {
                    DataRow row = dataset.Tables["CONTESTO_PROCEDURALE"].Rows[0];
                    contestoProcedurale.SYSTEM_ID = row["SYSTEM_ID"].ToString();
                    contestoProcedurale.TIPO_CONTESTO_PROCEDURALE = row["TIPO_CONTESTO_PROCEDURALE"].ToString();
                    contestoProcedurale.NOME = row["NOME"].ToString();
                    contestoProcedurale.FAMIGLIA = row["FAMIGLIA"].ToString();
                    contestoProcedurale.VERSIONE = row["VERSIONE"].ToString();
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetContestoProceduraleById " + e.Message);
            }

            return contestoProcedurale;
        }
    }
}
