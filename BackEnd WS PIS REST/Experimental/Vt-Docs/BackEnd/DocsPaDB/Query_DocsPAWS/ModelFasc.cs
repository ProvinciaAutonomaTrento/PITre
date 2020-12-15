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
using System.Collections.Generic;
using System.Linq;
using log4net;
using DocsPaUtils.Data;
using DocsPaVO.ProfilazioneDinamica;

namespace DocsPaDB.Query_DocsPAWS
{
    public class ModelFasc
    {
        private ILog logger = LogManager.GetLogger(typeof(ModelFasc));
        public ModelFasc() { }

        public DocsPaVO.ProfilazioneDinamica.OggettoCustom getOggettoById(string idOggetto)
        {
            DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto = null;
            DocsPaDB.DBProvider dbProvider = null;
            try
            {
                dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query qry = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_OGGETTO_FASC_BY_ID");
                qry.setParam("param1", idOggetto);

                string commandText = qry.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getOggettoById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - getOggettoById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
                if (dbProvider.ExecuteQuery(ds, commandText) && ds.Tables[0].Rows.Count > 0)
                {
                    oggetto = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                    this.setOggettoCustom(ref oggetto, ds, 0);
                    DocsPaVO.ProfilazioneDinamica.TipoOggetto tipo = new DocsPaVO.ProfilazioneDinamica.TipoOggetto();
                    setTipoOggetto(ref tipo, ds,0);
                    oggetto.TIPO = tipo;

                    //campo CLOB di configurazione
                    if (oggetto.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                    {
                        string config = dbProvider.GetLargeText("DPA_OGGETTI_CUSTOM_FASC", oggetto.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST");
                        oggetto.CONFIG_OBJ_EST = config;
                    }                    
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SQL - getOggettoById - ProfilazioneDinamica/Database/modelFasc.cs - Eccezione : " + ex.Message);
                logger.Debug("SQL - getOggettoById - ProfilazioneDinamica/Database/modelFasc.cs - Eccezione : " + ex.Message);
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


        public ArrayList getTemplatesFasc(string idAmministrazione)
        {
            ArrayList templates = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //Selezione dalla DPA_TIPO_FASC dei template associati ad una specifica amministrazione
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE_FASC");
                queryMng.setParam("idAmministrazione", idAmministrazione);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplatesFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplates - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                DataSet ds_templates = new DataSet();
                dbProvider.ExecuteQuery(ds_templates, commandText);

                string key = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROJECT_STRUCTURE");

                for (int i = 0; i < ds_templates.Tables[0].Rows.Count; i++)
                {
                    DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();
                    setTemplate(ref template, ds_templates, i);

                    if (!string.IsNullOrEmpty(key) && key.Equals("1"))
                        SetTemplateStructure(ref template);
                        
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

        private void SetTemplateStructure(ref Templates template)
        {
            DataSet dataset = new DataSet();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                dbProvider.ExecuteStoredProcedure("SP_GET_REL_PROJECT_TEMPLATE", new ArrayList()
                {
                    new DocsPaUtils.Data.ParameterSP("ID_TEMPLATE", ""),
                    new DocsPaUtils.Data.ParameterSP("ID_FASCICOLO", template.SYSTEM_ID),
                    new DocsPaUtils.Data.ParameterSP("ID_TITOLARIO", "")
                }, dataset);

                if (dataset.Tables[0].Rows.Count > 0)
                    template.ID_TEMPLATE_STRUTTURA = Convert.ToString(dataset.Tables[0].Rows[0]["ID_TEMPLATE"]);
            }
            catch (Exception e)
            {
                logger.Error("SQL - SetTemplateStructure - ERRORE : " + e.Message);
            }
        }

        public bool eliminaOggettoCustomDaDBFasc(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            //Il seguente metodo elimina l'oggettoCustom dal Database, non è stato possibile realizzare
            //l'eliminazione nel metodo "aggiornaTemplate" impostando il tipo di operazione, perchè si
            //presentavano particolari complicazione nella gestione dei dataGrid lato frontEnd-toolAmministrazione
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_OGGETTI_CUSTOM_DA_DB_FASC");
                queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - eliminaOggettoCustomDaDBFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - eliminaOggettoCustomDaDBFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
                
                if (template.IN_ESERCIZIO == "NO" || template.IPER_FASC_DOC == "1")
                {
                    //Cancellazione valori
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_ASS_VALORI_FASC");
                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - eliminaOggettoCustomDaDBFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - eliminaOggettoCustomDaDBFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                    
                    //Cancellazione posizione
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_OGG_CUSTOM_COMP_FASC");
                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - eliminaOggettoCustomDaDBFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - eliminaOggettoCustomDaDBFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                    
                    //Cancellazione oggetto
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_OGGETTI_CUSTOM_FASC");
                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - eliminaOggettoCustomDaDBFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - eliminaOggettoCustomDaDBFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);

                    //Cancellazione visibilità oggetto
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DEL_A_R_CAMPI_FASC");
                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - eliminaOggettoCustomDaDBFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - eliminaOggettoCustomDaDBFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                }

                //Controllo che l'oggettoCustom non sia un contatore, in caso affermativo, 
                //disabilito non cancello il contatore dalla tabella DPA_CONTATORI_FASC
                if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "Contatore")
                {
                    //controllo se il contatore è di custom,ossia se esiste un record all'interno della
                    //DPA_CONT_CUSTOM_FASC relativo al system_id dell'oggetto custom
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CONT_CUSTOM_FASC_BY_IDOGG");
                    queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - cerca contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    logger.Debug("SQL - cerca contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                    DataSet ds_datiContatoreCustom = new DataSet(); ;
                    dbProvider.ExecuteQuery(ds_datiContatoreCustom, commandText);
                    if (ds_datiContatoreCustom.Tables[0].Rows.Count != 0)
                    {

                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_CONT_CUSTOM_FASC");
                        queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - eliminaOggettoCustomDaDB - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - eliminaOggettoCustomDaDB - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                    }
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DISABLE_CONT_FASC");
                    queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - eliminaOggettoCustomDaDBFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - eliminaOggettoCustomDaDBFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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

        public void aggiornaPosizioneFasc(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_AGGIORNA_POSIZIONE_FASC");
                queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                queryMng.setParam("posizione", oggettoCustom.POSIZIONE);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - aggiornaPosizioneFasc- ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - aggiornaPosizioneFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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
        
        public bool salvaTemplateFasc(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione)
        {
            template.gestisciCaratteriSpeciali();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            bool retValue = false;
            int rowsAffected;
                
            try
            {
                //Inserimento DPA_TIPO_FASC
                string system_id_templates = string.Empty;
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_TEMPLATES_FASC");
                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_TIPO_FASC"));
                queryMng.setParam("Descrizione", template.DESCRIZIONE);
                queryMng.setParam("id_amm", idAmministrazione);
                queryMng.setParam("Abilitato_SI_NO", "1");
                queryMng.setParam("In_Esercizio", "NO");
                queryMng.setParam("Privato", template.PRIVATO);
                queryMng.setParam("NUM_MESI_CONSERVAZIONE", template.NUM_MESI_CONSERVAZIONE);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                if (dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                {
                    retValue = (rowsAffected > 0);

                    if (retValue)
                    {
                        // Reperimento systemid
                        commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_TIPO_FASC");
                        dbProvider.ExecuteScalar(out system_id_templates, commandText);
                    }
                    else
                    {
                        dbProvider.RollbackTransaction();
                        return false;
                    }
                }

                //Inserimento DPA_OGGETTI_CUSTOM_FASC
                for (int i = 0; i < template.ELENCO_OGGETTI.Count; i++)
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[i];
                    oggettoCustom.gestisciCaratteriSpeciali();
                    //Cerco l'ID_TIPO_OGGETTO per l'oggettoCustom in questione
                    int system_id_tipo_oggetto = 0;
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TIPO_OGGETTO_FASC");
                    queryMng.setParam("Descrizione", oggettoCustom.TIPO.DESCRIZIONE_TIPO);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        system_id_tipo_oggetto = Convert.ToInt32(ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                    }
                    else
                    {
                        logger.Debug("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - ERRRORE : system_id TIPO OGGETTO non trovata");
                        dbProvider.RollbackTransaction();
                        return false;
                    }

                    //Inserimento oggettoCustom
                    string system_id_oggettoCustom = string.Empty;
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_OGGETTO_CUSTOM_FASC");
                    queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_OGGETTI_CUSTOM_FASC"));
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
                    if(oggettoCustom.CAMPO_COMUNE != null && oggettoCustom.CAMPO_COMUNE == "1")
                        queryMng.setParam("CAMPO_COMUNE", oggettoCustom.CAMPO_COMUNE);
                    else
                        queryMng.setParam("CAMPO_COMUNE", "null");
                    queryMng.setParam("tipoContatore", oggettoCustom.TIPO_CONTATORE);
                    if (oggettoCustom.CONTA_DOPO != null && oggettoCustom.CONTA_DOPO == "1")
                        queryMng.setParam("contaDopo", "1");
                    else
                        queryMng.setParam("contaDopo", "0");
                    if (oggettoCustom.REPERTORIO != null && oggettoCustom.REPERTORIO == "1")
                        queryMng.setParam("repertorio", "1");
                    else
                        queryMng.setParam("repertorio", "0");

                    //MODIFICA
                    if (oggettoCustom.DA_VISUALIZZARE_RICERCA != null && oggettoCustom.DA_VISUALIZZARE_RICERCA == "1")
                        queryMng.setParam("da_visualizzare_ricerca", "1");
                    else
                        queryMng.setParam("da_visualizzare_ricerca", "0");
                    //MODIFICA

                    //GESTIONE LINK
                    //GESTIONE LINK
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

                    queryMng.setParam("formatoOra", oggettoCustom.FORMATO_ORA);

                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    if (dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    {
                        retValue = (rowsAffected > 0);

                        if (retValue)
                        {
                            // Reperimento systemid
                            commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_OGGETTI_CUSTOM_FASC");
                            dbProvider.ExecuteScalar(out system_id_oggettoCustom, commandText);
                            if (!string.IsNullOrEmpty(oggettoCustom.DATA_INIZIO) && !string.IsNullOrEmpty(oggettoCustom.DATA_FINE))
                            {
                                //salvo intervallo di date utilizzando data_fine e data_inizio

                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_DATE_CONT_CUSTOM_FASC");
                                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONT_CUSTOM_FASC"));
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
                        /*string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_OGGETTI_CUSTOM_FASC");
                        string id = string.Empty;
                        System.Diagnostics.Debug.WriteLine("SQL - salvaTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + sql);
                        logger.Debug("SQL - salvaTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + sql);
                        dbProvider.ExecuteScalar(out id, sql);*/

                        if (!string.IsNullOrEmpty(system_id_oggettoCustom))
                        {
                            dbProvider.SetLargeText("DPA_OGGETTI_CUSTOM_FASC", system_id_oggettoCustom, "CONFIG_OBJ_EST", oggettoCustom.CONFIG_OBJ_EST);
                        }
                    }

                    /*
                    //Poichè sto inserendo un nuovo oggetto della tipologia devo dare la visibilità a tutti i ruoli dell'amministrazione
                    Model model = new Model();
                    ArrayList listaRuoli = model.getRuoliByAmm(idAmministrazione, "", "");
                    DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                    assDocFascRuoli.ID_TIPO_DOC_FASC = system_id_templates;
                    assDocFascRuoli.ID_OGGETTO_CUSTOM = system_id_oggettoCustom;
                    assDocFascRuoli.VIS_OGG_CUSTOM = "1";
                    assDocFascRuoli.INS_MOD_OGG_CUSTOM = "1";
                    ArrayList listaCampi = new ArrayList();
                    listaCampi.Add(assDocFascRuoli);
                    this.estendiDirittiCampiARuoliFasc(listaCampi, listaRuoli);
                    */

                    //Inserimento DPA_OGG_CUSTOM_COMP_FASC
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INSERT_DPA_OGG_CUSTOM_COMP_FASC");
                    queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_OGG_CUSTOM_COMP_FASC"));
                    queryMng.setParam("idTemplate",system_id_templates);
                    queryMng.setParam("idOggettoCustom",system_id_oggettoCustom);
                    queryMng.setParam("posizione", oggettoCustom.POSIZIONE);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                    
                    //Inserimento DPA_ASS_TEMPLATES_FASC
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASS_TEMPLATES_FASC");
                    queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_TEMPLATES_FASC"));
                    queryMng.setParam("ID_OGGETTO", system_id_oggettoCustom);
                    queryMng.setParam("ID_TEMPLATE", system_id_templates);
                    queryMng.setParam("Id_Project", "");
                    queryMng.setParam("Valore_Oggetto_Db", "");
                    queryMng.setParam("Anno", System.DateTime.Now.Year.ToString());
                    queryMng.setParam("CODICE_DB", "");
                    queryMng.setParam("MANUAL_INSERT","0");
                    if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                        queryMng.setParam("idAooRf", oggettoCustom.ID_AOO_RF);
                    else
                        queryMng.setParam("idAooRf", "0");
                    queryMng.setParam("dtaIns", "NULL");
                    queryMng.setParam("anno_acc", "");
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                    
                    //Inserimento DPA_ASS_VALORI_FASC
                    for (int j = 0; j < oggettoCustom.ELENCO_VALORI.Count; j++)
                    {
                        DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = (DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[j];
                        valoreOggetto.gestisciCaratteriSpeciali();
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASS_VALORI_FASC");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_VALORI_FASC"));
                        queryMng.setParam("Descrizione_Valore", valoreOggetto.DESCRIZIONE_VALORE);
                        queryMng.setParam("Valore", valoreOggetto.VALORE);
                        queryMng.setParam("Valore_Di_Default", valoreOggetto.VALORE_DI_DEFAULT);
                        queryMng.setParam("ID_OGGETTO_CUSTOM", system_id_oggettoCustom);
                        queryMng.setParam("abilitato", valoreOggetto.ABILITATO.ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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

        public bool aggiornaTemplateFasc(DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            //Il seguente metodo opera sulla proprieta' TIPO_OPERAZIONE dell'oggettoCustom, a seconda
            //del tipo di operazione associata all'oggetto viene effettuata l'aggiornamento
            //o l'inserimento di quest'ultimo.
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
                    if (oggettoCustom.TIPO_OPERAZIONE.Equals("DaAggiungere"))
                    {
                        //Cerco l'ID_TIPO_OGGETTO per l'oggettoCustom in questione
                        int system_id_tipo_oggetto = 0;
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TIPO_OGGETTO_FASC");
                        queryMng.setParam("Descrizione", oggettoCustom.TIPO.DESCRIZIONE_TIPO);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        DataSet ds = new DataSet();
                        dbProvider.ExecuteQuery(ds, commandText);
                        system_id_tipo_oggetto = Convert.ToInt32(ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                        
                        //Inserimento oggettoCustom
                        string system_id_oggettoCustom = string.Empty;
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_OGGETTO_CUSTOM_FASC");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_OGGETTI_CUSTOM_FASC"));
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
                            queryMng.setParam("repertorio", "1");
                        else
                            queryMng.setParam("repertorio", "0");

                        //MODIFICA
                        if (oggettoCustom.DA_VISUALIZZARE_RICERCA != null && oggettoCustom.DA_VISUALIZZARE_RICERCA == "1")
                            queryMng.setParam("da_visualizzare_ricerca", "1");
                        else
                            queryMng.setParam("da_visualizzare_ricerca", "0");
                        //MODIFICA

                        queryMng.setParam("formatoOra", oggettoCustom.FORMATO_ORA);
                        
                        //GESTIONE LINK
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
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        if (dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                        {
                            retValue = (rowsAffected > 0);

                            if (retValue)
                            {
                                // Reperimento systemid
                                commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_OGGETTI_CUSTOM_FASC");
                                dbProvider.ExecuteScalar(out system_id_oggettoCustom, commandText);
                                if (!string.IsNullOrEmpty(oggettoCustom.DATA_INIZIO) && !string.IsNullOrEmpty(oggettoCustom.DATA_FINE))
                                {

                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_DATE_CONT_CUSTOM_FASC");
                                    queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                    queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONT_CUSTOM_FASC"));
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
                            /*string sql = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_OGGETTI_CUSTOM_FASC");
                            string id = string.Empty;
                            System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + sql);
                            logger.Debug("SQL - aggiornaTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + sql);
                            dbProvider.ExecuteScalar(out id, sql);*/

                            if (!string.IsNullOrEmpty(system_id_oggettoCustom))
                            {
                                dbProvider.SetLargeText("DPA_OGGETTI_CUSTOM_FASC", system_id_oggettoCustom, "CONFIG_OBJ_EST", oggettoCustom.CONFIG_OBJ_EST);
                            }
                        }

                        /*
                        //Poichè sto inserendo un nuovo oggetto della tipologia devo dare la visibilità a tutti i ruoli dell'amministrazione
                        Model model = new Model();
                        ArrayList listaRuoli = model.getRuoliByAmm(template.ID_AMMINISTRAZIONE, "", "");
                        DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli = new DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli();
                        assDocFascRuoli.ID_TIPO_DOC_FASC = template.SYSTEM_ID.ToString();
                        assDocFascRuoli.ID_OGGETTO_CUSTOM = system_id_oggettoCustom;
                        assDocFascRuoli.VIS_OGG_CUSTOM = "1";
                        assDocFascRuoli.INS_MOD_OGG_CUSTOM = "1";
                        ArrayList listaCampi = new ArrayList();
                        listaCampi.Add(assDocFascRuoli);
                        this.estendiDirittiCampiARuoliFasc(listaCampi, listaRuoli);
                        */

                        //Inserimento DPA_OGG_CUSTOM_COMP_FASC
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INSERT_DPA_OGG_CUSTOM_COMP_FASC");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_OGG_CUSTOM_COMP_FASC"));
                        queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                        queryMng.setParam("idOggettoCustom", system_id_oggettoCustom);
                        queryMng.setParam("posizione", oggettoCustom.POSIZIONE);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                        
                        //Inserimento DPA_ASSOCIAZIONE_TEMPLATES
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASS_TEMPLATES_FASC");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_TEMPLATES_FASC"));
                        queryMng.setParam("ID_OGGETTO", system_id_oggettoCustom);
                        queryMng.setParam("ID_TEMPLATE", template.SYSTEM_ID.ToString());
                        queryMng.setParam("Id_Project", "");
                        queryMng.setParam("Valore_Oggetto_Db", "");
                        queryMng.setParam("Anno", oggettoCustom.ANNO);
                        queryMng.setParam("CODICE_DB","");
                        queryMng.setParam("MANUAL_INSERT", "0");
                        if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                            queryMng.setParam("idAooRf", oggettoCustom.ID_AOO_RF);
                        else
                            queryMng.setParam("idAooRf", "0");
                        queryMng.setParam("dtaIns", "NULL");
                        queryMng.setParam("anno_acc", "");
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                        if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROPAGAZIONE_CAMPI_A_PREGRESSI")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROPAGAZIONE_CAMPI_A_PREGRESSI") == "1")
                        {
                            // Propagazione aggiunta campi ai pregressi
                            logger.Debug("Store Procedure: SP_EST_CAMPI_FASC_PREGRESSI");
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

                            int resultSP = dbProvider.ExecuteStoreProcedure("SP_EST_CAMPI_FASC_PREGRESSI", parameters);
                            if (resultSP == 0)
                            {
                                logger.Debug("STORE PROCEDURE SP_EST_CAMPI_FASC_PREGRESSI: esito positivo");
                            }
                            else
                            {
                                logger.Debug("ERRORE: STORE PROCEDURE SP_EST_CAMPI_FASC_PREGRESSI: esito negativo");
                            }
                        }

                        if (!string.IsNullOrEmpty(oggettoCustom.CONFIG_OBJ_EST))
                        {
                            dbProvider.SetLargeText("DPA_OGGETTI_CUSTOM", oggettoCustom.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST", oggettoCustom.CONFIG_OBJ_EST);
                        }
                        
                        //Inserimento DPA_ASS_VALORI_FASC
                        for (int j = 0; j < oggettoCustom.ELENCO_VALORI.Count; j++)
                        {
                            DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = (DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[j];
                            valoreOggetto.gestisciCaratteriSpeciali();
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASS_VALORI_FASC");
                            queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                            queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_VALORI_FASC"));
                            queryMng.setParam("Descrizione_Valore", valoreOggetto.DESCRIZIONE_VALORE);
                            queryMng.setParam("Valore", valoreOggetto.VALORE);
                            queryMng.setParam("Valore_Di_Default", valoreOggetto.VALORE_DI_DEFAULT);
                            queryMng.setParam("ID_OGGETTO_CUSTOM", system_id_oggettoCustom);
                            queryMng.setParam("abilitato", valoreOggetto.ABILITATO.ToString());
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            logger.Debug("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);
                        }
                    }
                    if (oggettoCustom.TIPO_OPERAZIONE.Equals("DaAggiornare"))
                    {
                        //controllo se il contatore è di tipo custom, in caso positivo aggiorno la tabella
                        // DPA_CONT_CUSTOM_FASC passandogli il system_id dell'oggetto custom corrente in sessione
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CONT_CUSTOM_FASC_BY_IDOGG");
                        queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - cerca contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - cerca contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        DataSet ds_datiContatoreCustom = new DataSet(); ;
                        dbProvider.ExecuteQuery(ds_datiContatoreCustom, commandText);
                        if (ds_datiContatoreCustom.Tables[0].Rows.Count != 0)
                        {

                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONT_CUSTOM_FASC");
                            queryMng.setParam("dataFine", DocsPaDbManagement.Functions.Functions.ToDate(oggettoCustom.DATA_FINE));
                            queryMng.setParam("dataInizio", DocsPaDbManagement.Functions.Functions.ToDate(oggettoCustom.DATA_INIZIO));
                            queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());

                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - salva contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - salva contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            bool result = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                        }
                        //Aggiornamento della DPA_OGGETTI_CUSTOM_FASC
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_AGGIORNA_DPA_OGGETTI_CUSTOM_FASC");
                        queryMng.setParam("descrizioneOggettoCustom", oggettoCustom.DESCRIZIONE);
                        queryMng.setParam("orizzontaleVerticaleOggettoCustom", oggettoCustom.ORIZZONTALE_VERTICALE);
                        queryMng.setParam("campoObbligatorioOggettoCustom", oggettoCustom.CAMPO_OBBLIGATORIO);
                        queryMng.setParam("multilineaOggettoCustom", oggettoCustom.MULTILINEA);
                        queryMng.setParam("numeroLineeOggettoCustom", oggettoCustom.NUMERO_DI_LINEE);
                        queryMng.setParam("numeroCaratteriOggettoCustom", oggettoCustom.NUMERO_DI_CARATTERI);
                        queryMng.setParam("campoRicercaOggettoCustom", oggettoCustom.CAMPO_DI_RICERCA);
                        queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                        queryMng.setParam("Reset_Anno", oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO);
                        queryMng.setParam("Formato_Contatore", oggettoCustom.FORMATO_CONTATORE);
                        queryMng.setParam("ID_R_DEFAULT", oggettoCustom.ID_RUOLO_DEFAULT);
                        queryMng.setParam("RICERCA_CORR", oggettoCustom.TIPO_RICERCA_CORR);
                        queryMng.setParam("CAMPO_COMUNE", oggettoCustom.CAMPO_COMUNE);
                        queryMng.setParam("tipoContatore", oggettoCustom.TIPO_CONTATORE);
                        queryMng.setParam("contaDopo", oggettoCustom.CONTA_DOPO);
                        queryMng.setParam("repertorio", oggettoCustom.REPERTORIO);
                        queryMng.setParam("formatoOra", oggettoCustom.FORMATO_ORA);
                        //GESTIONE LINK
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

                        //modifica
                        queryMng.setParam("da_visualizzare_ricerca", oggettoCustom.DA_VISUALIZZARE_RICERCA);
                        //modifica
                        
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                        if (!string.IsNullOrEmpty(oggettoCustom.CONFIG_OBJ_EST))
                        {
                            dbProvider.SetLargeText("DPA_OGGETTI_CUSTOM_FASC", oggettoCustom.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST", oggettoCustom.CONFIG_OBJ_EST);
                        }
                        //Cerco il valoreOggetto per valore e id_oggettCustom
                        //Se esiste non faccio niente altrimenti effettuo un inserimento
                        for (int j = 0; j < oggettoCustom.ELENCO_VALORI.Count; j++)
                        {
                            DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoreOggetto = (DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[j];
                            valoreOggetto.gestisciCaratteriSpeciali();
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI_1_FASC");
                            queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                            queryMng.setParam("valore", valoreOggetto.VALORE);
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            logger.Debug("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            DataSet ds = new DataSet();
                            dbProvider.ExecuteQuery(ds, commandText);
                            if (ds.Tables[0].Rows.Count == 0)
                            {
                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASS_VALORI_FASC");
                                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_VALORI_FASC"));
                                queryMng.setParam("Descrizione_Valore", valoreOggetto.DESCRIZIONE_VALORE);
                                queryMng.setParam("Valore", valoreOggetto.VALORE);
                                queryMng.setParam("Valore_Di_Default", valoreOggetto.VALORE_DI_DEFAULT);
                                queryMng.setParam("ID_OGGETTO_CUSTOM", oggettoCustom.SYSTEM_ID.ToString());
                                queryMng.setParam("abilitato", valoreOggetto.ABILITATO.ToString());
                                commandText = queryMng.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                logger.Debug("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                dbProvider.ExecuteNonQuery(commandText);
                            }
                        }

                        //Verifico eventuali cancellazioni
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI_FASC");
                        queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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
                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_ASSOCIAZIONE_VALORI_1_FASC");
                                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                                    queryMng.setParam("valore", ds_1.Tables[0].Rows[k]["Valore"].ToString());
                                    commandText = queryMng.getSQL();
                                    System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                    logger.Debug("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                    dbProvider.ExecuteNonQuery(commandText);
                                }
                            }
                        }

                        //Ordino le Descrizioni valore
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI_FASC");
                        queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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
                                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_DPA_ASSOCIAZIONE_VALORI_FASC");
                                        queryMng.setParam("idOggettoCustom", ds_2.Tables[0].Rows[y]["ID_OGGETTO_CUSTOM"].ToString());
                                        queryMng.setParam("valore", ds_2.Tables[0].Rows[y]["Valore"].ToString());
                                        queryMng.setParam("valoreDiDefault", valoreOggetto.VALORE_DI_DEFAULT);
                                        queryMng.setParam("descrizione", "Valore" + (y + 1));
                                        queryMng.setParam("abilitato", valoreOggetto.ABILITATO.ToString());
                                        commandText = queryMng.getSQL();
                                        System.Diagnostics.Debug.WriteLine("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                        logger.Debug("SQL - aggiornaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                        dbProvider.ExecuteNonQuery(commandText);
                                    }
                                }
                            }
                        }
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

        public bool isValueInUseFasc(string idOggetto, string idTemplate, string valoreOggettoDB)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_IS_VALUE_IN_USE_FASC");
                queryMng.setParam("idOggetto", idOggetto);
                queryMng.setParam("idTemplate", idTemplate);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - isValueInUseFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - isValueInUseFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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

        public bool disabilitaTemplateFasc(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione, string serverPath, string codiceAmministrazione)
        {
            template.gestisciCaratteriSpeciali();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                bool retValue = false;
                int rowsAffected;
                string commandText;
                DocsPaUtils.Query queryMng;

                #region Codice Commetato - Non viene più eliminato il template ma solo disabilitato
                /*
                if (template.IN_ESERCIZIO.Equals("NO"))
                {
                    //Cancello l'associazione del template
                    ////template = this.getTemplateFascById(template.ID_TIPO_ATTO);
                    template = this.getTemplateFascById(template.ID_TIPO_FASC);
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_ASS_TEMPLATES_FASC");
                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplate Fasc- ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                    
                    //Cancello eventuali Valori degli Oggetti Custom e gli Oggetti Custom 
                    for (int i = 0; i < template.ELENCO_OGGETTI.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[i];

                        //Cancellazione posizione
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_OGG_CUSTOM_COMP_FASC");
                        queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                        queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                        
                        //Se è un oggetto comune non devo cancellarlo correttamente ho precedentemente
                        //cacellato la sua posizione nel template che vado a rimuovere ma essendo un oggetto
                        //condiviso da altri template non va cancellato
                        if (oggettoCustom.CAMPO_COMUNE != "1")
                        {
                            //Cancellazione valori
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_ASS_VALORI_FASC");
                            queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            logger.Debug("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);
                        
                            //Cancellazione oggetto
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_OGGETTI_CUSTOM_FASC");
                            queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            logger.Debug("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);

                            //Cancellazione visibilità oggetto
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DEL_A_R_CAMPI_FASC");
                            queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                            queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);
                        }
                    }

                    //Elimino la visibilità sul template
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_ASS_FASC_RUOLI");
                    queryMng.setParam("idTipoFasc", template.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                    
                    //Elimino il tipo documento
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_TEMPLATE_FASC");
                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                }
                else
                {
                    //Disabilito il template
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DISABILITA_TEMPLATE_FASC");
                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                }
                */
                #endregion Codice Commetato - Non viene più eliminato il template ma solo disabilitato

                //Disabilito il template
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DISABILITA_TEMPLATE_FASC");
                queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - disabilitaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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

        public DocsPaVO.ProfilazioneDinamica.Templates getTemplateFascById(string idTemplate)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();

            try
            {
                //Selezione template
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TEMPLATE_FASC");
                queryMng.setParam("idTemplate", idTemplate);
                //queryMng.setParam("docNumber", "''");
                queryMng.setParam("idProject", " AND (DPA_ASS_TEMPLATES_FASC.ID_PROJECT  = '' OR DPA_ASS_TEMPLATES_FASC.ID_PROJECT IS NULL) ");
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                DataSet ds_template = new DataSet();
                dbProvider.ExecuteQuery(ds_template, commandText);

                //Se il template non ha oggetti custom vengono restituite solo le proprietà del template
                if (ds_template.Tables[0].Rows.Count == 0)
                {
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE_2_FASC");
                    queryMng.setParam("idTemplate", idTemplate);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    ds_template = new DataSet();
                    dbProvider.ExecuteQuery(ds_template, commandText);

                    setTemplate(ref template, ds_template, 0);
                    string key = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROJECT_STRUCTURE");
                    if (!string.IsNullOrEmpty(key) && key.Equals("1"))
                        SetTemplateStructure(ref template);

                    return template;
                }

                setTemplate(ref template, ds_template, 0);

                string key2 = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROJECT_STRUCTURE");
                if (!string.IsNullOrEmpty(key2) && key2.Equals("1"))
                    SetTemplateStructure(ref template);



                /*
                //Selezione template
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE_2_FASC");
                queryMng.setParam("idTemplate", idTemplate);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                DataSet ds_template = new DataSet();
                dbProvider.ExecuteQuery(ds_template, commandText);

                setTemplate(ref template, ds_template, 0);
                
                //Seleziono i SYSTEM_ID per gli oggettiCustom associati al template dalla DPA_ASSOCIAZIONE_TEMPLATES
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SYSTEM_ID_OGGETTI_CUSTOM_FASC");
                queryMng.setParam("idTemplate", idTemplate);
                commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                DataSet ds_systemId_oggettiCustom = new DataSet();
                dbProvider.ExecuteQuery(ds_systemId_oggettiCustom, commandText);
                */

                //Cerco gli oggetti custom associati al template
                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;
                //for (int j = 0; j < ds_systemId_oggettiCustom.Tables[0].Rows.Count; j++)
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
                        string config = dbProvider.GetLargeText("DPA_OGGETTI_CUSTOM_FASC", oggettoCustom.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST");
                        oggettoCustom.CONFIG_OBJ_EST = config;
                    }
                    
                    /*
                    oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_OGGETTI_CUSTOM_FASC");
                    queryMng.setParam("idOggettoCustom", ds_systemId_oggettiCustom.Tables[0].Rows[j]["ID_OGGETTO"].ToString());
                    queryMng.setParam("idTemplate", idTemplate);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

                    DataSet ds_oggettoCustom = new DataSet();
                    dbProvider.ExecuteQuery(ds_oggettoCustom, commandText);
                    setOggettoCustom(ref oggettoCustom, ds_oggettoCustom, 0);
                    
                    //Seleziono il tipo di oggetto
                    DocsPaVO.ProfilazioneDinamica.TipoOggetto tipoOggetto = new DocsPaVO.ProfilazioneDinamica.TipoOggetto();
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TIPI_OGGETTO_1_FASC");
                    queryMng.setParam("idOggettoCustom", ds_oggettoCustom.Tables[0].Rows[0]["ID_TIPO_OGGETTO"].ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

                    DataSet ds_tipoOggetto = new DataSet();
                    dbProvider.ExecuteQuery(ds_tipoOggetto, commandText);
                    setTipoOggetto(ref tipoOggetto, ds_tipoOggetto, 0);
                    
                    //Aggiungo il tipo oggetto all'oggettoCustom
                    oggettoCustom.TIPO = tipoOggetto;
                    */

                    //Selezioni i valori per l'oggettoCustom
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI_FASC");
                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

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

        public DocsPaVO.ProfilazioneDinamica.Templates getTemplatePerRicerca(string idAmministrazione, string tipoFasc)
        {
            DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE_1_FASC");
                queryMng.setParam("tipoFasc", tipoFasc);
                queryMng.setParam("idAmministrazione", idAmministrazione);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/ModelFasc.cs - QUERY : " + commandText);
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
                    template.ID_PROJECT = "";

                    //Recupero i dati del template
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TEMPLATE_FASC");
                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                    //queryMng.setParam("docNumber", "''");
                    queryMng.setParam("idProject", " AND (DPA_ASS_TEMPLATES_FASC.ID_PROJECT  = '' OR DPA_ASS_TEMPLATES_FASC.ID_PROJECT IS NULL) ");
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/ModelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/ModelFasc.cs - QUERY : " + commandText);
                    DataSet ds_templateCompleto = new DataSet();
                    dbProvider.ExecuteQuery(ds_templateCompleto, commandText);

                    setTemplate(ref template, ds_templateCompleto, 0);
                    template.ID_PROJECT = "";

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
                                string config = dbProvider.GetLargeText("DPA_OGGETTI_CUSTOM_FASC", oggettoCustom.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST");
                                oggettoCustom.CONFIG_OBJ_EST = config;
                            }

                            //Selezioni i valori per l'oggettoCustom
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI");
                            queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - getTemplatePerRicerca - ProfilazioneDinamica/Database/ModelFasc.cs - QUERY : " + commandText);
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

        public void aggiornaPosizioniFasc(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom_1, DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom_2, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_AGGIORNA_POSIZIONE_FASC");
                queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                queryMng.setParam("idOggettoCustom", oggettoCustom_1.SYSTEM_ID.ToString());
                queryMng.setParam("posizione", oggettoCustom_1.POSIZIONE);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - aggiornaPosizioniFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - aggiornaPosizioniFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
                
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_AGGIORNA_POSIZIONE_FASC");
                queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                queryMng.setParam("idOggettoCustom", oggettoCustom_2.SYSTEM_ID.ToString());
                queryMng.setParam("posizione", oggettoCustom_2.POSIZIONE);
                commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - aggiornaPosizioniFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - aggiornaPosizioniFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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

        public void messaInEsercizioTemplateFasc(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_MESSA_IN_ESERCIZIO_FASC");
                queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());

                if (template.IN_ESERCIZIO.ToUpper().Equals("SI"))
                    queryMng.setParam("paramInEsercizio", "NO");
                else
                    queryMng.setParam("paramInEsercizio", "SI");

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - messaInEsercizioTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - messaInEsercizioTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                dbProvider.ExecuteNonQuery(commandText);
                //codice aggiunto da C.Fuccia per la gestione manuale della sospensione dei contatori custom
                OggettoCustom cc = ((OggettoCustom[])template.ELENCO_OGGETTI.ToArray(typeof(OggettoCustom))).Where(c => c.TIPO.DESCRIZIONE_TIPO.ToLower() == "contatore" && !string.IsNullOrEmpty(c.DATA_INIZIO) && !string.IsNullOrEmpty(c.DATA_FINE)).FirstOrDefault();
                if (cc != null)
                {
                    DocsPaUtils.Query queryMngcc = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GESTIONE_SOSPENSIONE_FASC");
                    queryMngcc.setParam("id_ogg_fasc", cc.SYSTEM_ID.ToString());
                    if (template.IN_ESERCIZIO.ToUpper().Equals("SI"))
                        queryMngcc.setParam("sospensione", "SI");
                    else
                        queryMngcc.setParam("sospensione", "NO");
                    string commandTextcc = queryMngcc.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - messaInEsercizioTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandTextcc);
                    logger.Debug("SQL - messaInEsercizioTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandTextcc);
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

        public ArrayList getTipoFasc(string idAmministrazione)
        {
            ArrayList listaTipiFasc = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TIPO_FASC");
                queryMng.setParam("idAmm", idAmministrazione);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTipoFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTipoFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

                DataSet ds_templates = new DataSet();
                dbProvider.ExecuteQuery(ds_templates, commandText);

                if (ds_templates.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds_templates.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();
                        setTemplate(ref template, ds_templates, i);
                        listaTipiFasc.Add(template);
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

            return listaTipiFasc;
        }

        public ArrayList getTipoFascFromRuolo(string idAmministrazione, string idRuolo, string diritti)
        {
            ArrayList listaTipiFasc = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_TIPO_FASC_FROM_RUOLO");
                queryMng.setParam("idAmm", idAmministrazione);
                queryMng.setParam("idRuolo", idRuolo);
                
                //Ricerca
                if (diritti == "1")
                {
                    queryMng.setParam("paramInEsercizio", "");
                    queryMng.setParam("diritti", " IN (1,2) ");
                }

                //Inserimento
                if (diritti == "2")
                {
                    queryMng.setParam("paramInEsercizio", " (In_Esercizio <> 'NO' OR In_Esercizio IS NULL) AND (Abilitato_SI_NO <> 0 OR Abilitato_SI_NO IS NULL) AND ");
                    queryMng.setParam("diritti", " IN (2) ");
                }

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTipoFascFromRuolo - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTipoFascFromRuolo - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

                DataSet ds_templates = new DataSet();
                dbProvider.ExecuteQuery(ds_templates, commandText);

                if (ds_templates.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds_templates.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();
                        setTemplate(ref template, ds_templates, i);
                        listaTipiFasc.Add(template);
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

            return listaTipiFasc;
        }

        public void salvaInserimentoUtenteProfDimFasc(DocsPaVO.ProfilazioneDinamica.Templates template, string idProject)
        {
            logger.Info("BEGIN");
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //Verifico l'esistenza dell' idProject per decidere se effettuare un inserimento o un aggiornamento
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_IDPROJECT");
                queryMng.setParam("idProject", idProject);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                DataSet ds_idProject = new DataSet();
                dbProvider.ExecuteQuery(ds_idProject, commandText);

                string anno = System.DateTime.Now.Year.ToString();

                if (ds_idProject.Tables[0].Rows.Count == 0)
                {
                    //INSERIMENTO
                    for (int i = 0; i < template.ELENCO_OGGETTI.Count; i++)
                    {
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[i];
                        oggettoCustom.ANNO = anno;
                        
                        switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                        {
                            case "Contatore":
                                //Il parametro booleano è :
                                //true = se l'oggetto è da inserire - come in questo caso
                                //false= se l'oggetto è da aggiornare
                                calcolaContatore(template, ref oggettoCustom, true);
                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASS_TEMPLATES_FASC");
                                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_TEMPLATES_FASC"));
                                queryMng.setParam("ID_OGGETTO", oggettoCustom.SYSTEM_ID.ToString());
                                queryMng.setParam("ID_TEMPLATE", template.SYSTEM_ID.ToString());
                                queryMng.setParam("Id_Project", idProject);
                                queryMng.setParam("Valore_Oggetto_Db", oggettoCustom.VALORE_DATABASE);
                                queryMng.setParam("CODICE_DB", oggettoCustom.CODICE_DB);
                                string manual=(oggettoCustom.MANUAL_INSERT)? "1":"0";
                                queryMng.setParam("MANUAL_INSERT", manual);
                                queryMng.setParam("Anno", oggettoCustom.ANNO);
                                if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                                    queryMng.setParam("idAooRf", oggettoCustom.ID_AOO_RF);
                                else
                                    queryMng.setParam("idAooRf", "0");
                                queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.ToDate(oggettoCustom.DATA_INSERIMENTO));
                                queryMng.setParam("anno_acc", oggettoCustom.ANNO_ACC);
                                commandText = queryMng.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - salvaInserimentoUtenteProfDimFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                logger.Debug("SQL - salvaInserimentoUtenteProfDimFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                dbProvider.ExecuteNonQuery(commandText);
                                break;

                            case "CasellaDiSelezione":
                                oggettoCustom.gestisciCaratteriSpeciali();
                                for (int j = 0; j < oggettoCustom.ELENCO_VALORI.Count; j++)
                                {
                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASS_TEMPLATES_FASC");
                                    queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                    queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_TEMPLATES_FASC"));
                                    queryMng.setParam("ID_OGGETTO", oggettoCustom.SYSTEM_ID.ToString());
                                    queryMng.setParam("ID_TEMPLATE", template.SYSTEM_ID.ToString());
                                    queryMng.setParam("Id_Project", idProject);
                                    if (oggettoCustom.VALORI_SELEZIONATI.Count == 0)
                                    {
                                        queryMng.setParam("Valore_Oggetto_Db", "");
                                    }
                                    else
                                    {
                                        if (oggettoCustom.VALORI_SELEZIONATI[j] != null && (string)oggettoCustom.VALORI_SELEZIONATI[j] != "")
                                            queryMng.setParam("Valore_Oggetto_Db", oggettoCustom.VALORI_SELEZIONATI[j].ToString());
                                        else
                                            queryMng.setParam("Valore_Oggetto_Db", "");
                                    }
                                    queryMng.setParam("Anno", oggettoCustom.ANNO);
                                    if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                                        queryMng.setParam("idAooRf", oggettoCustom.ID_AOO_RF);
                                    else
                                        queryMng.setParam("idAooRf", "0");
                                    queryMng.setParam("CODICE_DB", oggettoCustom.CODICE_DB);
                                    string manual2 = (oggettoCustom.MANUAL_INSERT) ? "1" : "0";
                                    queryMng.setParam("MANUAL_INSERT", manual2);
                                    queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.GetDate());
                                    queryMng.setParam("anno_acc", "");
                                    commandText = queryMng.getSQL();
                                    System.Diagnostics.Debug.WriteLine("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                    logger.Debug("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                    dbProvider.ExecuteNonQuery(commandText);
                                }
                                break;

                            default:
                                oggettoCustom.gestisciCaratteriSpeciali();
                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASS_TEMPLATES_FASC");
                                queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_TEMPLATES_FASC"));
                                queryMng.setParam("ID_OGGETTO", oggettoCustom.SYSTEM_ID.ToString());
                                queryMng.setParam("ID_TEMPLATE", template.SYSTEM_ID.ToString());
                                queryMng.setParam("Id_Project", idProject);
                                //if (oggettoCustom.VALORE_DATABASE.Length > 254)
                                //    queryMng.setParam("Valore_Oggetto_Db", oggettoCustom.VALORE_DATABASE.Substring(0,254));
                                //else
                                    queryMng.setParam("Valore_Oggetto_Db", oggettoCustom.VALORE_DATABASE);
                                queryMng.setParam("Anno", oggettoCustom.ANNO);
                                if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                                    queryMng.setParam("idAooRf", oggettoCustom.ID_AOO_RF);
                                else
                                    queryMng.setParam("idAooRf", "0");
                                queryMng.setParam("CODICE_DB", oggettoCustom.CODICE_DB);
                                string manual3=(oggettoCustom.MANUAL_INSERT)? "1":"0";
                                queryMng.setParam("MANUAL_INSERT", manual3);
                                queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.GetDate());
                                queryMng.setParam("anno_acc", "");
                                commandText = queryMng.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                logger.Debug("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                dbProvider.ExecuteNonQuery(commandText);
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
                        switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                        {
                            case "CasellaDiSelezione":
                                oggettoCustom.gestisciCaratteriSpeciali();
                                 int modif_cas_selez = 0; //booleano che mi informa se è cambiato lo stato della casella di selezione 
                                for (int j = 0; j < oggettoCustom.ELENCO_VALORI.Count; j++)
                                {
                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_DPA_ASS_TEMPLATES_FASC");
                                    if (oggettoCustom.VALORI_SELEZIONATI[j] != null && (string)oggettoCustom.VALORI_SELEZIONATI[j] != "")
                                        queryMng.setParam("valoreDbOggettoCustom", oggettoCustom.VALORI_SELEZIONATI[j].ToString().Replace("'","''"));
                                    else
                                        queryMng.setParam("valoreDbOggettoCustom", "");
                                    queryMng.setParam("idAooRf", "0");
                                    queryMng.setParam("idProject", idProject);
                                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                                    queryMng.setParam("system_id", ds_idProject.Tables[0].Rows[indiceSystem_id_DpaAssTemplates]["SYSTEM_ID"].ToString());
                                    queryMng.setParam("CODICE_DB", oggettoCustom.CODICE_DB);
                                    string manual = (oggettoCustom.MANUAL_INSERT) ? "1" : "0";
                                    queryMng.setParam("MANUAL_INSERT", manual);
                                    queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.GetDate());
                                    queryMng.setParam("anno", oggettoCustom.ANNO);
                                    commandText = queryMng.getSQL();
                                    System.Diagnostics.Debug.WriteLine("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                    logger.Debug("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                                    indiceSystem_id_DpaAssTemplates++;
                                    if (rowsAffected > 0)
                                        modif_cas_selez = 1;
                                }
                                insertInStorico(oggettoCustom, oldOggCustom, modif_cas_selez, ref indexOldObj);
                                break;

                            case "Contatore":
                                if (oggettoCustom.VALORE_DATABASE == null || oggettoCustom.VALORE_DATABASE == "")
                                {
                                    //verifico se nel contatore ci sia già impostato un valore numerico: in questo caso non faccio update
                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_IS_VALUE_IN_USE_FASC");
                                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString() + " AND system_id = " + ds_idProject.Tables[0].Rows[indiceSystem_id_DpaAssTemplates]["SYSTEM_ID"].ToString());
                                    queryMng.setParam("idOggetto", oggettoCustom.SYSTEM_ID.ToString() + " AND id_project = " + idProject);
                                    string field = string.Empty;
                                    dbProvider.ExecuteScalar(out field, queryMng.getSQL());
                                    bool continua = false;
                                    try
                                    {
                                        Convert.ToInt32(field);
                                        continua = false;
                                    }
                                    catch (Exception excp)
                                    {
                                        logger.Debug("Il contatore non presenta un valore numerico e quindi si può effettuare l'aggiornamento");
                                        continua = true;
                                    }
                                    
                                    //Il parametro booleano è :
                                    //true = se l'oggetto è da inserire 
                                    //false= se l'oggetto è da aggiornare - come in questo caso
                                    calcolaContatore(template, ref oggettoCustom, continua);
                                    if (!continua)
                                        oggettoCustom.VALORE_DATABASE = field;
                                }
                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_DPA_ASS_TEMPLATES_FASC");
                                queryMng.setParam("valoreDbOggettoCustom", oggettoCustom.VALORE_DATABASE);
                                if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                                    queryMng.setParam("idAooRf", oggettoCustom.ID_AOO_RF);
                                else
                                    queryMng.setParam("idAooRf", "0");
                                queryMng.setParam("idProject", idProject);
                                queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                                queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                                queryMng.setParam("system_id", ds_idProject.Tables[0].Rows[indiceSystem_id_DpaAssTemplates]["SYSTEM_ID"].ToString());
                                queryMng.setParam("CODICE_DB", oggettoCustom.CODICE_DB);
                                string manual1 = (oggettoCustom.MANUAL_INSERT) ? "1" : "0";
                                queryMng.setParam("MANUAL_INSERT", manual1);
                                queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.ToDate(oggettoCustom.DATA_INSERIMENTO));
                                queryMng.setParam("anno", oggettoCustom.ANNO);
                                commandText = queryMng.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - salvaInserimentoUtenteProfDimFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                logger.Debug("SQL - salvaInserimentoUtenteProfDimFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                int rowsAff = 0;
                                dbProvider.ExecuteNonQuery(commandText, out rowsAff);
                                indiceSystem_id_DpaAssTemplates++;
                                break;

                            default:
                                oggettoCustom.gestisciCaratteriSpeciali();
                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_DPA_ASS_TEMPLATES_FASC");
                                //if (oggettoCustom.VALORE_DATABASE.Length > 254)
                                //    queryMng.setParam("valoreDbOggettoCustom", oggettoCustom.VALORE_DATABASE.Substring(0, 254));
                                //else
                                    queryMng.setParam("valoreDbOggettoCustom", oggettoCustom.VALORE_DATABASE);
                                if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                                    queryMng.setParam("idAooRf", oggettoCustom.ID_AOO_RF);
                                else
                                    queryMng.setParam("idAooRf", "0");
                                queryMng.setParam("idProject", idProject);
                                queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                                queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                                queryMng.setParam("CODICE_DB", oggettoCustom.CODICE_DB);
                                string manual2 = (oggettoCustom.MANUAL_INSERT) ? "1" : "0";
                                queryMng.setParam("MANUAL_INSERT", manual2);
                                queryMng.setParam("system_id", ds_idProject.Tables[0].Rows[indiceSystem_id_DpaAssTemplates]["SYSTEM_ID"].ToString());
                                queryMng.setParam("dtaIns", DocsPaDbManagement.Functions.Functions.GetDate());
                                queryMng.setParam("anno", oggettoCustom.ANNO);
                                commandText = queryMng.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                logger.Debug("SQL - salvaInserimentoUtenteProfDim - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                                indiceSystem_id_DpaAssTemplates++;
                                //storicizzo il campo profilato
                                insertInStorico(oggettoCustom, oldOggCustom, rowsAffected, ref indexOldObj);
                                break;
                        }                        
                    }
                    template.OLD_OGG_CUSTOM.Clear();
                }
            }
            catch(Exception ex)
            {
                dbProvider.RollbackTransaction();
                logger.Debug("Errore nel salvataggio dei campi profilati - FASCICOLO IdProject = " + idProject + " TIPOLOGIA = " + template.DESCRIZIONE + " ERRORE = " + ex.Message);
                throw ex;
            }
            finally
            {
                dbProvider.Dispose();
            }

            logger.Info("END");
        }

        public DocsPaVO.ProfilazioneDinamica.Templates getTemplateFasc(string idProject)
        {
            DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();
            string idTemplate = null;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //Ricerco un determinato idTemplate a partire dall'idProject 
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_ID_TEMPLATE_FROM_PROJECT");
                queryMng.setParam("idProject", idProject);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                DataSet ds_template = new DataSet();
                dbProvider.ExecuteQuery(ds_template, commandText);

                //Verifico, se esiste un idTemplate per quell'idProject lo carico, altrimenti carico un template vuoto
                //per il tipoAtto e l'amministrazione richiesti
                if (ds_template.Tables[0].Rows.Count != 0)
                {
                    idTemplate = ds_template.Tables[0].Rows[0]["ID_TEMPLATE"].ToString();
                    template.SYSTEM_ID = Convert.ToInt32(idTemplate);

                    //Recupero la descrizione del template
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DESCRIZIONE_TEMPLATE_FASC");
                    queryMng.setParam("idTemplate", idTemplate);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

                    DataSet ds_template_descrizione = new DataSet();
                    dbProvider.ExecuteQuery(ds_template_descrizione, commandText);

                    setTemplate(ref template, ds_template_descrizione, 0);
                    template.ID_PROJECT = idProject;                    
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

        public DocsPaVO.ProfilazioneDinamica.Templates getTemplateFascDettagli(string idProject)
        {
            logger.Info("BEGIN");
            DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();
            string idTemplate = null;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                //Ricerco un determinato idTemplate a partire dall'idProject 
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_ID_TEMPLATE_FROM_ID_PROJECT");
                queryMng.setParam("idProject", idProject);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

                DataSet ds_template = new DataSet();
                dbProvider.ExecuteQuery(ds_template, commandText);

                //Verifico, se esiste un idTemplate per quell'idProject lo carico, altrimenti carico un template vuoto
                //per il tipoAtto e l'amministrazione richiesti
                if (ds_template.Tables[0].Rows.Count != 0)
                {
                    idTemplate = ds_template.Tables[0].Rows[0]["ID_TEMPLATE"].ToString();
                    template.SYSTEM_ID = Convert.ToInt32(idTemplate);

                    //Recupero i dati del template
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TEMPLATE_FASC");
                    queryMng.setParam("idTemplate", idTemplate);
                    //queryMng.setParam("docNumber", docNumber);
                    queryMng.setParam("idProject", " AND DPA_ASS_TEMPLATES_FASC.ID_PROJECT  = " + idProject + " ");
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascDettagli - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateFascDettagli - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    DataSet ds_templateCompleto = new DataSet();
                    dbProvider.ExecuteQuery(ds_templateCompleto, commandText);

                    //Se il template non ha oggetti custom vengono restituite solo le proprietà del template
                    if (ds_templateCompleto.Tables[0].Rows.Count == 0)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE_2_FASC");
                        queryMng.setParam("idTemplate", idTemplate);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        ds_template = new DataSet();
                        dbProvider.ExecuteQuery(ds_template, commandText);

                        setTemplate(ref template, ds_template, 0);
                        return template;
                    }

                    setTemplate(ref template, ds_templateCompleto, 0);

                    /*
                    //Recupero la descrizione del template
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DESCRIZIONE_TEMPLATE_FASC");
                    queryMng.setParam("idTemplate", idTemplate);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

                    DataSet ds_template_descrizione = new DataSet();
                    dbProvider.ExecuteQuery(ds_template_descrizione, commandText);

                    //Carico il template associato allo specifico idProject
                    setTemplate(ref template, ds_template_descrizione, 0);
                    template.ID_PROJECT = idProject;
                    
                    //Seleziono i SYSTEM_ID per gli oggettiCustom associati al template dalla DPA_ASSOCIAZIONE_TEMPLATES
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SYSTEM_ID_OGGETTI_CUSTOM_1_FASC");
                    queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                    queryMng.setParam("idProject", idProject);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

                    DataSet ds_systemId_oggettiCustom = new DataSet();
                    dbProvider.ExecuteQuery(ds_systemId_oggettiCustom, commandText);
                    */

                    //Cerco gli oggetti custom associati al template
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;
                    //for (int j = 0; j < ds_systemId_oggettiCustom.Tables[0].Rows.Count; j++)
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
                            string config = dbProvider.GetLargeText("DPA_OGGETTI_CUSTOM_FASC", oggettoCustom.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST");
                            oggettoCustom.CONFIG_OBJ_EST = config;
                        }

                        /*
                        oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_OGGETTI_CUSTOM_FASC");
                        queryMng.setParam("idOggettoCustom", ds_systemId_oggettiCustom.Tables[0].Rows[j]["ID_OGGETTO"].ToString());
                        queryMng.setParam("idTemplate", template.SYSTEM_ID.ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

                        DataSet ds_oggettoCustom = new DataSet();
                        dbProvider.ExecuteQuery(ds_oggettoCustom, commandText);
                        setOggettoCustom(ref oggettoCustom, ds_oggettoCustom, 0);
                        */

                        if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione"))
                        {
                            //Recupero il valoreDb dell'oggetto
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_VALORE_DB_OGGETTO_CUSTOM_1_FASC");
                            queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                            queryMng.setParam("idProject", idProject);
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            logger.Debug("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

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

                        /*
                        //Seleziono il tipo di oggetto
                        DocsPaVO.ProfilazioneDinamica.TipoOggetto tipoOggetto = new DocsPaVO.ProfilazioneDinamica.TipoOggetto();

                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TIPI_OGGETTO_1_FASC");
                        queryMng.setParam("idOggettoCustom", ds_oggettoCustom.Tables[0].Rows[0]["ID_TIPO_OGGETTO"].ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

                        DataSet ds_tipoOggetto = new DataSet();
                        dbProvider.ExecuteQuery(ds_tipoOggetto, commandText);
                        setTipoOggetto(ref tipoOggetto, ds_tipoOggetto, 0);
                        
                        //Aggiungo il tipo oggetto all'oggettoCustom
                        oggettoCustom.TIPO = tipoOggetto;
                        */

                        if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione") ||
                           oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("MenuATendina") ||
                           oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("SelezioneEsclusiva"))
                        {
                            //Selezioni i valori per l'oggettoCustom
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI_FASC");
                            queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            logger.Debug("SQL - getTemplate - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

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
                    return this.getTemplateFasc(idProject);
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

        public ArrayList getRuoliTipoFasc(string idTipoFasc)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            ArrayList listaAssRuoli = new ArrayList();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_RUOLI_TIPO_FASC");
                queryMng.setParam("idTipoFasc", idTipoFasc);

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getRuoliTipoFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - getRuoliTipoFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

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

        public void salvaAssociazioneFascRuoli(ArrayList assFascRuoli)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                foreach (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli in assFascRuoli)
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_RUOLI_TIPO_FASC");
                    queryMng.setParam("idTipoFasc", assDocFascRuoli.ID_TIPO_DOC_FASC);
                    queryMng.setParam("idRuolo", assDocFascRuoli.ID_GRUPPO);
                    queryMng.setParam("diritti", assDocFascRuoli.DIRITTI_TIPOLOGIA);
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneFascRuoli - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaAssociazioneFascRuoli - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    int rowsAffected;
                    dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                    if (rowsAffected == 0)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INSERT_ASS_FASC_RUOLI");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_VIS_TIPO_FASC"));
                        queryMng.setParam("idTipoFasc", assDocFascRuoli.ID_TIPO_DOC_FASC);
                        queryMng.setParam("idRuolo", assDocFascRuoli.ID_GRUPPO);
                        queryMng.setParam("diritti", assDocFascRuoli.DIRITTI_TIPOLOGIA);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneFascRuoli - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaAssociazioneFascRuoli - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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

        public bool existTemplatesFasc()
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("EXIST_TEMPLATES_FASC");
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - existTemplatesFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - existTemplatesFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

                DataSet ds_numberTemplates = new DataSet();
                dbProvider.ExecuteQuery(ds_numberTemplates, commandText);

                if (ds_numberTemplates.Tables[0].Rows.Count > 0)
                {
                    if(ds_numberTemplates.Tables[0].Rows[0]["NUMBER_TEMPLATES"].ToString().Equals("0"))
                        return false;
                    else
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

        public DocsPaVO.ProfilazioneDinamica.Templates impostaCampiComuniFasc(DocsPaVO.ProfilazioneDinamica.Templates modello, ArrayList campiComuni)
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
                        aggiornaTemplateFasc(modello);
                        break;
                    }
                        
                }

                //Seleziono i system_id degli oggetti_custom comuni per questo specifico template
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CAMPI_COMUNI_FASC");
                queryMng.setParam("idTemplate", modello.SYSTEM_ID.ToString());
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - impostaCampiComuniFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - impostaCampiComuniFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                DataSet ds_idCampiComuni = new DataSet();
                dbProvider.ExecuteQuery(ds_idCampiComuni, commandText);

                if (ds_idCampiComuni.Tables[0].Rows.Count != 0)
                {
                    //Rimuovo gli oggetti_custom comuni per questo specifico template
                    for (int i = 0; i < ds_idCampiComuni.Tables[0].Rows.Count; i++)
                    {
                        //Rimozione dalla PDA_ASS_TEMPLATES FASC
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_CAMPI_COMUNI_FASC");
                        queryMng.setParam("idTemplate", modello.SYSTEM_ID.ToString());
                        queryMng.setParam("idOggettoCustom", ds_idCampiComuni.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - impostaCampiComuniFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - impostaCampiComuniFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                
                        //Rimozione dalla DPA_OGG_CUSTOM_COMP_FASC
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_DELETE_DPA_OGG_CUSTOM_COMP_FASC");
                        queryMng.setParam("idTemplate", modello.SYSTEM_ID.ToString());
                        queryMng.setParam("idOggettoCustom", ds_idCampiComuni.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - impostaCampiComuniFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - impostaCampiComuniFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                    }
                }
                
                //Recupero il modello aggiornato per impostare correttamente le posizioni dei campi comuni
                DocsPaVO.ProfilazioneDinamica.Templates modelloAggiornato = this.getTemplateFascById(modello.SYSTEM_ID.ToString());
                int numeroElementiModello = modelloAggiornato.ELENCO_OGGETTI.Count;
                ////Reimposto le posizioni degli oggetti non comuni
                for (int i = 0; i < modelloAggiornato.ELENCO_OGGETTI.Count; i++)
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)modelloAggiornato.ELENCO_OGGETTI[i];
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_DPA_OGG_CUSTOM_COMP_FASC");
                    int newPosition = (i + 1);
                    queryMng.setParam("posizione", newPosition.ToString());
                    queryMng.setParam("idTemplate", modelloAggiornato.SYSTEM_ID.ToString());
                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - impostaCampiComuniFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - impostaCampiComuniFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                }


                //Inserisco gli oggetti_custom comuni selezionati per questo template
                for (int i = 0; i < campiComuni.Count; i++)
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom) campiComuni[i];
                    numeroElementiModello++;

                    //Inserimento nella DPA_ASS_TEMPLATES_FASC
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SALVA_ASS_TEMPLATES_FASC");
                    queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_TEMPLATES_FASC"));
                    queryMng.setParam("ID_OGGETTO", oggCustom.SYSTEM_ID.ToString());
                    queryMng.setParam("ID_TEMPLATE", modello.SYSTEM_ID.ToString());
                    queryMng.setParam("Id_Project", "");
                    queryMng.setParam("Valore_Oggetto_Db", "");
                    queryMng.setParam("CODICE_DB", "");
                    queryMng.setParam("MANUAL_INSERT", "0");
                    queryMng.setParam("Anno", System.DateTime.Now.Year.ToString());
                    if (oggCustom.ID_AOO_RF != null && oggCustom.ID_AOO_RF != "")
                        queryMng.setParam("idAooRf", oggCustom.ID_AOO_RF);
                    else
                        queryMng.setParam("idAooRf", "0");
                    queryMng.setParam("dtaIns", "NULL");
                    queryMng.setParam("anno_acc", "");
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - impostaCampiComuniFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - impostaCampiComuniFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                    
                    //Verifico se il campo comune è già presente nella DPA_OGG_CUSTOM_COMP_FASC per il template interessato,
                    //in caso affermativo non devo inserirlo
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_OGGETTI_CUSTOM_FASC");
                    queryMng.setParam("idTemplate", modello.SYSTEM_ID.ToString());
                    queryMng.setParam("idOggettoCustom", oggCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - impostaCampiComuniFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - impostaCampiComuniFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    DataSet ds_campiComuniInPosizione = new DataSet();
                    dbProvider.ExecuteQuery(ds_campiComuniInPosizione, commandText);

                    if (ds_campiComuniInPosizione.Tables[0].Rows.Count == 0)
                    {
                        //Inserimento DPA_OGG_CUSTOM_COMP_FASC
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INSERT_DPA_OGG_CUSTOM_COMP_FASC");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_OGG_CUSTOM_COMP_FASC"));
                        queryMng.setParam("idTemplate", modello.SYSTEM_ID.ToString());
                        queryMng.setParam("idOggettoCustom", oggCustom.SYSTEM_ID.ToString());
                        queryMng.setParam("posizione", numeroElementiModello.ToString());
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaTemplateFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);
                    }
                }

                //Recupero il modello completamente aggiornato da restituire
                DocsPaVO.ProfilazioneDinamica.Templates template = this.getTemplateFascById(modello.SYSTEM_ID.ToString());
                
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

        public bool isInUseCampoComuneFasc(string idTemplate, string idCampoComune)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_IS_IN_USE_CAMPO_COMUNE_FASC");
                queryMng.setParam("idTemplate", idTemplate);
                queryMng.setParam("idOggetto", idCampoComune);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - isInUseCampoComuneFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - isInUseCampoComuneFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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

        //questa funzione controlla se è associato un record nella DPA_CONT_CUSTOM_FASC
        //relativo al system_id dell'oggettocustom corrente
        private void ControllaCustom(ref DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaUtils.Query queryMng;
            string commandText = string.Empty;
            //controllo se esiste un record nella DPA_CONT_CUSTOM_FASC associato
            //al system id dell'oggetto custom corrente
            try
            {
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CONT_CUSTOM_FASC_BY_IDOGG");
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
            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CONT_CUSTOM_FASC_BY_IDOGG");
            queryMng.setParam("idOgg", oggettoCustom.SYSTEM_ID.ToString());
            commandText = queryMng.getSQL();
            System.Diagnostics.Debug.WriteLine("SQL - cerca contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
            logger.Debug("SQL - cerca contatore custom - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
            DataSet ds_datiContatoreCustom = new DataSet(); ;
            dbProvider.ExecuteQuery(ds_datiContatoreCustom, commandText);
            if (ds_datiContatoreCustom.Tables[0].Rows.Count != 0)
            {
                //if ((oggettoCustom.CONTA_DOPO == "1" && oggettoCustom.CONTATORE_DA_FAR_SCATTARE) || oggettoCustom.CONTA_DOPO != "1")
                if (oggettoCustom.CONTATORE_DA_FAR_SCATTARE)
                {
                    //Verifico che il contatore esiste, se si lo recupero altrimenti lo inserisco e ne recupero i dati
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_EXIST_CONT_FASC");
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
                    System.Diagnostics.Debug.WriteLine("SQL - cercaContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - cercaContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    DataSet ds_datiContatore = new DataSet();
                    dbProvider.ExecuteQuery(ds_datiContatore, commandText);

                    //Contatore non esistente 
                    if (ds_datiContatore.Tables[0].Rows.Count == 0)
                    {
                        //Inserisco il contatore
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INSERT_CONT_FASC");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONTATORI_FASC"));
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
                        */
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
                        queryMng.setParam("abilitato", "1");
                        queryMng.setParam("anno", oggettoCustom.ANNO);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - inserisciContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - inserisciContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                        //Reperimento systemid
                        commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_CONTATORI_FASC");
                        string idContatoreInserito = string.Empty;
                        dbProvider.ExecuteScalar(out idContatoreInserito, commandText);

                        //Recupero i dati del contatore
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CONT_FASC_BY_ID");
                        queryMng.setParam("systemId", idContatoreInserito);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - cercaContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - cercaContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        dbProvider.ExecuteQuery(ds_datiContatore, commandText);
                    }

                    if (ds_datiContatore.Tables[0].Rows.Count != 0)
                    {
                        int annoContatore = 0;
                        int annoOggetto = 0;
                        int valoreContatore = 0;
                        string valoreDataInizio = string.Empty;
                        string valoreDataFine = string.Empty;
                        int annoCorrente = System.DateTime.Now.Year;
                        if (ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString() != null && ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString() != "")
                            annoContatore = Convert.ToInt32(ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString());
                        if (oggettoCustom.ANNO != null && oggettoCustom.ANNO != "")
                            annoOggetto = Convert.ToInt32(oggettoCustom.ANNO);
                        if (ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString() != null && ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString() != "")
                            valoreContatore = Convert.ToInt32(ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString());
                        if (ds_datiContatoreCustom.Tables[0].Columns.Contains("DATA_INIZIO") && !string.IsNullOrEmpty(ds_datiContatoreCustom.Tables[0].Rows[0]["DATA_INIZIO"].ToString()))
                            valoreDataInizio = Convert.ToDateTime(ds_datiContatoreCustom.Tables[0].Rows[0]["DATA_INIZIO"]).Year.ToString();
                        if (ds_datiContatoreCustom.Tables[0].Columns.Contains("DATA_FINE") && !string.IsNullOrEmpty(ds_datiContatoreCustom.Tables[0].Rows[0]["DATA_FINE"].ToString()))
                            valoreDataFine = Convert.ToDateTime(ds_datiContatoreCustom.Tables[0].Rows[0]["DATA_FINE"]).Year.ToString();


                        //Incremento il valore del contatore e lo aggiorno
                        //if (inserimentoAggiornamento || (oggettoCustom.CONTA_DOPO == "1" && oggettoCustom.CONTATORE_DA_FAR_SCATTARE))
                        if (inserimentoAggiornamento || oggettoCustom.CONTATORE_DA_FAR_SCATTARE)
                        {
                            valoreContatore++;
                            oggettoCustom.VALORE_DATABASE = valoreContatore.ToString();
                            oggettoCustom.DATA_INSERIMENTO = dbProvider.GetSysdate();
                            oggettoCustom.ANNO = annoContatore.ToString();
                            if (inserimentoAggiornamento)
                            {
                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONT_FASC");
                                queryMng.setParam("idContatore", ds_datiContatore.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                                queryMng.setParam("valoreContatore", valoreContatore.ToString());
                                //queryMng.setParam("anno", oggettoCustom.ANNO);
                                queryMng.setParam("anno", annoContatore.ToString());
                                commandText = queryMng.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - aggiornaContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                logger.Debug("SQL - aggiornaContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                dbProvider.ExecuteNonQuery(commandText);
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
                        else
                        {
                            //Incremento il valore del contatore e lo aggiorno
                            //if (inserimentoAggiornamento || (oggettoCustom.CONTA_DOPO == "1" && oggettoCustom.CONTATORE_DA_FAR_SCATTARE))
                            if (inserimentoAggiornamento || oggettoCustom.CONTATORE_DA_FAR_SCATTARE)
                            {
                                valoreContatore++;
                                oggettoCustom.VALORE_DATABASE = valoreContatore.ToString();
                                oggettoCustom.DATA_INSERIMENTO = dbProvider.GetSysdate();
                                if (inserimentoAggiornamento)
                                {
                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONT_FASC");
                                    queryMng.setParam("idContatore", ds_datiContatore.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                                    queryMng.setParam("valoreContatore", valoreContatore.ToString());
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
                {

                    //Verifico se deve scattare il contatore
                    //OggettoCustom.CONTA_DOPO = 1 AND OggettoCustom.CONTATORE_DA_FAR_SCATTARE = true - SI 
                    //OggettoCustom.CONTA_DOPO != 1 - SI 
                    //In tutti gli altri casi il contatore non deve scattare
                    //ma va comunque inserita una riga nella DPA_ASSOCIAZIONE_TEMPLATES con valore vuoto
                    //perchè posso decidere di far scattare il contatore in momenti diversi

                    //if ((oggettoCustom.CONTA_DOPO == "1" && oggettoCustom.CONTATORE_DA_FAR_SCATTARE) || oggettoCustom.CONTA_DOPO != "1")
                    if (oggettoCustom.CONTATORE_DA_FAR_SCATTARE)
                    {
                        //Verifico che il contatore esiste, se si lo recupero altrimenti lo inserisco e ne recupero i dati
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_EXIST_CONT_FASC");
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
                        System.Diagnostics.Debug.WriteLine("SQL - cercaContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - cercaContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        DataSet ds_datiContatore = new DataSet();
                        dbProvider.ExecuteQuery(ds_datiContatore, commandText);

                        //Contatore non esistente 
                        if (ds_datiContatore.Tables[0].Rows.Count == 0)
                        {
                            //Inserisco il contatore
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INSERT_CONT_FASC");
                            queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                            queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_CONTATORI_FASC"));
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
                            */
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
                            queryMng.setParam("abilitato", "1");
                            queryMng.setParam("anno", oggettoCustom.ANNO);
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - inserisciContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            logger.Debug("SQL - inserisciContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);

                            //Reperimento systemid
                            commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted("DPA_CONTATORI_FASC");
                            string idContatoreInserito = string.Empty;
                            dbProvider.ExecuteScalar(out idContatoreInserito, commandText);

                            //Recupero i dati del contatore
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CONT_FASC_BY_ID");
                            queryMng.setParam("systemId", idContatoreInserito);
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - cercaContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            logger.Debug("SQL - cercaContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

                            dbProvider.ExecuteQuery(ds_datiContatore, commandText);
                        }

                        if (ds_datiContatore.Tables[0].Rows.Count != 0)
                        {
                            int annoContatore = 0;
                            int annoOggetto = 0;
                            int valoreContatore = 0;
                            int annoCorrente = System.DateTime.Now.Year;
                            if (ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString() != null && ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString() != "")
                                annoContatore = Convert.ToInt32(ds_datiContatore.Tables[0].Rows[0]["ANNO"].ToString());
                            if (oggettoCustom.ANNO != null && oggettoCustom.ANNO != "")
                                annoOggetto = Convert.ToInt32(oggettoCustom.ANNO);
                            if (ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString() != null && ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString() != "")
                                valoreContatore = Convert.ToInt32(ds_datiContatore.Tables[0].Rows[0]["VALORE"].ToString());

                            if (oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI")
                            {
                                //if (annoContatore < annoOggetto)
                                if (annoContatore < annoCorrente)
                                {
                                    //Reinizializzo il contatore
                                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONT_FASC");
                                    queryMng.setParam("idContatore", ds_datiContatore.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                                    queryMng.setParam("valoreContatore", "1");
                                    //queryMng.setParam("anno", oggettoCustom.ANNO);
                                    queryMng.setParam("anno", annoCorrente.ToString());
                                    commandText = queryMng.getSQL();
                                    System.Diagnostics.Debug.WriteLine("SQL - aggiornaContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                    logger.Debug("SQL - aggiornaContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                    dbProvider.ExecuteNonQuery(commandText);
                                    oggettoCustom.VALORE_DATABASE = "1";
                                    oggettoCustom.ANNO = annoCorrente.ToString();


                                }
                                else
                                {
                                    //Incremento il valore del contatore e lo aggiorno
                                    //if (inserimentoAggiornamento || (oggettoCustom.CONTA_DOPO == "1" && oggettoCustom.CONTATORE_DA_FAR_SCATTARE))
                                    if (inserimentoAggiornamento || oggettoCustom.CONTATORE_DA_FAR_SCATTARE)
                                    {
                                        valoreContatore++;
                                        oggettoCustom.VALORE_DATABASE = valoreContatore.ToString();
                                        oggettoCustom.DATA_INSERIMENTO = dbProvider.GetSysdate();
                                        oggettoCustom.ANNO = annoContatore.ToString();
                                        if (inserimentoAggiornamento)
                                        {
                                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONT_FASC");
                                            queryMng.setParam("idContatore", ds_datiContatore.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                                            queryMng.setParam("valoreContatore", valoreContatore.ToString());
                                            //queryMng.setParam("anno", oggettoCustom.ANNO);
                                            queryMng.setParam("anno", annoContatore.ToString());
                                            commandText = queryMng.getSQL();
                                            System.Diagnostics.Debug.WriteLine("SQL - aggiornaContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                                            logger.Debug("SQL - aggiornaContatore - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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
                                    valoreContatore++;
                                    oggettoCustom.VALORE_DATABASE = valoreContatore.ToString();
                                    oggettoCustom.DATA_INSERIMENTO = dbProvider.GetSysdate();
                                    if (inserimentoAggiornamento)
                                    {
                                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_CONT_FASC");
                                        queryMng.setParam("idContatore", ds_datiContatore.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
                                        queryMng.setParam("valoreContatore", valoreContatore.ToString());
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



        public void UpdatePrivatoTipoFasc(int systemId_template, string privato)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_PRIVATO_FASC");
                queryMng.setParam("idTemplate", systemId_template.ToString());
                if (privato != "")
                    queryMng.setParam("privato", privato);
                else
                    queryMng.setParam("privato", "0");
                
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - updatePrivatoTipoFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - updatePrivatoTipoFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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

        public void UpdateMesiConsTipoFasc(int systemId_template, string mesiCons)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_MESICONS_FASC");
                queryMng.setParam("idTemplate", systemId_template.ToString());
                if (!string.IsNullOrEmpty(mesiCons))
                    queryMng.setParam("NUM_MESI_CONSERVAZIONE", mesiCons);
                else
                    queryMng.setParam("NUM_MESI_CONSERVAZIONE", "0");

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - updateMesiConsTipoFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - updateMesiConsTipoFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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

        public int countFascTipoFasc(string tipo_fasc, string codiceAmm)
        {
            int numFasc = 0;
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            string numeroFascicoli;
            try
            {
                string idAmm = getIdAmmByCod(codiceAmm);
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_CONTATORE_FASC");
                queryMng.setParam("idAmministrazione", idAmm);
                queryMng.setParam("idTipoFasc", tipo_fasc);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - countFascTipoFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - countFascTipoFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                dbProvider.ExecuteScalar(out numeroFascicoli, commandText);
                numFasc = Convert.ToInt32(numeroFascicoli);
            }
            catch
            {
                return numFasc;
            }
            finally
            {
                dbProvider.Dispose();
            }

            return numFasc;
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
                System.Diagnostics.Debug.WriteLine("SQL - getIdAmmByCod - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - getIdAmmByCod - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipoRicerca"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipoRicerca"></param>
        /// <param name="valore"></param>
        /// <returns></returns>
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

        public string getSeriePerRicercaProfilazione(DocsPaVO.ProfilazioneDinamica.Templates template, string anno_prof)
        {   
            string queryStr = string.Empty;
            
            if (template != null && template.ELENCO_OGGETTI != null)
            {
                //Imposto il filtro per la ricercare solo sui campi profilati che sono campi di ricerca
                //queryStr += "A.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0, dpa_oggetti_custom_fasc dpa1 where dpa0.id_oggetto =  dpa1.system_id and dpa1.campo_di_ricerca = 'SI') ";

                bool firstFilter = true;

                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto in template.ELENCO_OGGETTI)
                {
                    string operatoreRicerca = string.Empty;

                    if (!firstFilter && !string.IsNullOrEmpty(queryStr))
                        operatoreRicerca = template.OPERATORE_RICERCA_OGGETTI.ToString();

                    switch (oggetto.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CampoDiTesto":
                            if (oggetto.VALORE_DATABASE != null && oggetto.VALORE_DATABASE != "")
                            {
                                queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND UPPER(dpa0.valore_oggetto_db) " + this.getOperatoreQueryOggettoCustomStringa(oggetto.TIPO_RICERCA_STRINGA) + " UPPER('" + this.getValoreQueryOggettoCustomStringa(oggetto.TIPO_RICERCA_STRINGA, oggetto.VALORE_DATABASE) + "') ) ) ";
                            }
                            break;

                        case "CasellaDiSelezione":
                            for (int j = 0; j < oggetto.VALORI_SELEZIONATI.Count; j++)
                            {
                                if (oggetto.VALORI_SELEZIONATI[j] != null && (string)oggetto.VALORI_SELEZIONATI[j] != "")
                                {
                                    queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND UPPER(dpa0.valore_oggetto_db) = UPPER('" + oggetto.VALORI_SELEZIONATI[j].ToString().Replace("'", "''") + "')) ) ";
                                    operatoreRicerca = template.OPERATORE_RICERCA_OGGETTI.ToString().ToLowerInvariant();
                                    firstFilter = false;
                                }
                                //else
                                //{
                                //    queryStr += operatoreRicerca + " A.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND ( dpa0.valore_oggetto_db = '' OR dpa0.valore_oggetto_db is null )) ) ";
                                //}
                            }
                            break;

                        case "MenuATendina":
                            if (oggetto.VALORE_DATABASE != null && oggetto.VALORE_DATABASE != "")
                            {
                                queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND UPPER(dpa0.valore_oggetto_db) = UPPER('" + oggetto.VALORE_DATABASE.Replace("'", "''") + "')) ) ";
                            }
                            break;

                        case "SelezioneEsclusiva":
                            if (oggetto.VALORE_DATABASE != null && oggetto.VALORE_DATABASE != "")
                            {
                                queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND UPPER(dpa0.valore_oggetto_db) = UPPER('" + oggetto.VALORE_DATABASE.Replace("'", "''") + "')) ) ";
                            }
                            break;

                        case "Contatore":
                            switch (oggetto.TIPO_CONTATORE)
                            {
                                case "T":
                                    if (!oggetto.VALORE_DATABASE.Equals(""))
                                    {
                                        if (oggetto.VALORE_DATABASE.IndexOf('@') != -1)
                                        {
                                            string[] contatore = oggetto.VALORE_DATABASE.Split('@');
                                            queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[1], false) + " ) )";
                                        }
                                        else
                                        {
                                            queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_DATABASE, false) + " ) )";
                                        }
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
                                                queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[1], false) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ) )";
                                            }
                                            else
                                            {
                                                queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_DATABASE, false) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ) )";
                                            }
                                        }
                                        else
                                        {
                                            //Nr.Contatore NO - Aoo/Rf SI
                                            queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ) )";
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
                                                queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[1], false) + " ) )";
                                            }
                                            else
                                            {
                                                queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_DATABASE, false) + " ) )";
                                            }
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
                                    queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToDateColumn("dpa0.valore_oggetto_db") + " >= " + DocsPaDbManagement.Functions.Functions.ToDate(date[0]) + " AND " + DocsPaDbManagement.Functions.Functions.ToDateColumn("dpa0.valore_oggetto_db") + " <= " + DocsPaDbManagement.Functions.Functions.ToDate(date[1]) + " ) ) ";
                                }
                                else
                                {
                                    queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToDateColumn("dpa0.valore_oggetto_db") + " >= " + DocsPaDbManagement.Functions.Functions.ToDate(oggetto.VALORE_DATABASE) + " ) ) ";
                                }
                            }
                            break;

                        case "Corrispondente":
                            if (oggetto.VALORE_DATABASE != null && oggetto.VALORE_DATABASE != "")
                            {
                                int codiceCorrispondente;
                                if (Int32.TryParse(oggetto.VALORE_DATABASE, out codiceCorrispondente))
                                {
                                    //queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND upper(dpa0.valore_oggetto_db) in( select upper(SYSTEM_ID) from DPA_CORR_GLOBALI where SYSTEM_ID = " + codiceCorrispondente + ")))";
                                    if (oggetto.ESTENDI_STORICIZZATI)
                                    {
                                        queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND upper(dpa0.valore_oggetto_db) in( select upper(SYSTEM_ID) from DPA_CORR_GLOBALI where SYSTEM_ID IN (SELECT system_id FROM dpa_corr_globali START WITH system_id = " + codiceCorrispondente + " CONNECT BY PRIOR id_old = system_id))))";
                                    }
                                    else
                                    {
                                        queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND upper(dpa0.valore_oggetto_db) in( select upper(SYSTEM_ID) from DPA_CORR_GLOBALI where SYSTEM_ID = " + codiceCorrispondente + ")))";
                                    }
                                }
                                else
                                {
                                    //

                                    queryStr += operatoreRicerca + " A.system_id in  (SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() +
                                        " AND upper(dpa0.valore_oggetto_db) in( select SYSTEM_ID from DPA_CORR_GLOBALI where UPPER(VAR_DESC_CORR) " + this.getOperatoreQueryOggettoCustomStringa(oggetto.TIPO_RICERCA_STRINGA) + " UPPER('" + this.getValoreQueryOggettoCustomStringa(oggetto.TIPO_RICERCA_STRINGA, oggetto.VALORE_DATABASE) + "') )))";

                                    //queryStr += operatoreRicerca + " A.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND upper(dpa0.valore_oggetto_db) in( select SYSTEM_ID from DPA_CORR_GLOBALI where UPPER(VAR_DESC_CORR) like UPPER('%" + oggetto.VALORE_DATABASE.Replace("'", "''") + "%') )))";
                                }
                            }
                            break;
                        case "OggettoEsterno":
                            if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE) || !string.IsNullOrEmpty(oggetto.CODICE_DB))
                            {
                                string assQuery = "SELECT /*+ index(dpa0)*/ id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    assQuery = assQuery + " AND UPPER(dpa0.valore_oggetto_db) LIKE UPPER('%" + oggetto.VALORE_DATABASE.Replace("'", "''") + "%')";
                                }
                                if (!string.IsNullOrEmpty(oggetto.CODICE_DB))
                                {
                                    assQuery = assQuery + " AND UPPER(dpa0.codice_db) = UPPER('" + oggetto.CODICE_DB.Replace("'", "''") + "')";
                                }
                                assQuery = assQuery + ")";
                                queryStr += operatoreRicerca + " A.system_id in  (" + assQuery + ")";
                            }
                            break;
                    }

                    if (firstFilter)
                        firstFilter = false;
                }

                if (!string.IsNullOrEmpty(queryStr))
                {
                    queryStr = string.Format(" {0} ({1})",
                        DocsPaVO.ProfilazioneDinamica.OperatoriRicercaOggettiCustomEnum.And.ToString().ToLowerInvariant(), queryStr);
                }
            }

            return queryStr;
        }

        public ArrayList getIdModelliTrasmAssociatiFasc(string idTipoFasc, string idDiagramma, string idStato)
        {
            ArrayList idModelliTrasmSelezionati = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            
            try
            {
                if (idStato != "")
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_ID_MODELLO_TIPOFASC_DS");
                    queryMng.setParam("idTipoFasc", idTipoFasc);
                    queryMng.setParam("idDiagramma", idDiagramma);
                    queryMng.setParam("idStato", idStato);
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getIdModelliTrasmAssociatiFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getIdModelliTrasmAssociatiFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm obj = new DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm();
                        obj.SYTSEM_ID = ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString();
                        //obj.ID_TIPO_DOC = ds.Tables[0].Rows[i]["Id_tipo_doc"].ToString();
                        obj.ID_TIPO_FASC = ds.Tables[0].Rows[i]["Id_tipo_fasc"].ToString();
                        obj.ID_DIAGRAMMA = ds.Tables[0].Rows[i]["Id_diagramma"].ToString();
                        obj.ID_TEMPLATE = ds.Tables[0].Rows[i]["Id_Mod_Trasm"].ToString();
                        obj.ID_STATO = ds.Tables[0].Rows[i]["Id_stato"].ToString();
                        obj.TRASM_AUT = ds.Tables[0].Rows[i]["Trasm_aut"].ToString();
                        idModelliTrasmSelezionati.Add(obj);
                    }
                }
                else
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_GET_ID_MODELLO_TIPOFASC_DS_1");
                    queryMng.setParam("idTipoFasc", idTipoFasc);
                    queryMng.setParam("idDiagramma", idDiagramma);
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getIdModelliTrasmAssociatiFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getIdModelliTrasmAssociatiFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm obj = new DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm();
                        obj.SYTSEM_ID = ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString();
                        //obj.ID_TIPO_DOC = ds.Tables[0].Rows[i]["Id_tipo_doc"].ToString();
                        obj.ID_TIPO_FASC = ds.Tables[0].Rows[i]["Id_tipo_fasc"].ToString();
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

        public void salvaAssociazioneModelliFasc(string idTipoFasc, string idDiagramma, ArrayList modelliSelezionati, string idStato)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                
                if (idStato == "")
                {
                    if (idDiagramma == "0")
                    {
                        DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_DISASSOCIA_TIPO_FASC_DIAGRAMMA_1");
                        queryMng.setParam("idTipoFasc", idTipoFasc);
                        string commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneModelliFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaAssociazioneModelliFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);                        
                    }
                    else
                    {
                        DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_DISASSOCIA_TIPO_FASC_DIAGRAMMA_2");
                        queryMng.setParam("idDiagramma", idDiagramma);
                        queryMng.setParam("idTipoFasc", idTipoFasc);
                        string commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneModelliFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaAssociazioneModelliFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);

                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_DISASSOCIA_TIPO_FASC_DIAGRAMMA_1");
                        queryMng.setParam("idTipoFasc", idTipoFasc);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneModelliFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaAssociazioneModelliFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        dbProvider.ExecuteNonQuery(commandText);                                                
                    }

                    for (int i = 0; i < modelliSelezionati.Count; i++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm obj = (DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm)modelliSelezionati[i];
                        if (idDiagramma == "0")
                        {
                            bool retValue = false;
                            int rowsAffected;
                            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_ASSOCIA_TIPO_FASC_DIAGRAMMA_1");
                            queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                            queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_DIAGRAMMI"));
                            queryMng.setParam("idTipoFasc", obj.ID_TIPO_FASC);
                            queryMng.setParam("idTemplate", obj.ID_TEMPLATE);

                            string commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneModelliFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            logger.Debug("SQL - salvaAssociazioneModelliFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);                            
                        }
                        else
                        {
                            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_ASSOCIA_TIPO_FASC_DIAGRAMMA_2");
                            queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                            queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_DIAGRAMMI"));
                            queryMng.setParam("idTipoFasc", obj.ID_TIPO_FASC);
                            queryMng.setParam("idDiagramma", obj.ID_DIAGRAMMA);
                            queryMng.setParam("idTemplate", obj.ID_TEMPLATE);

                            string commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneModelliFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            logger.Debug("SQL - salvaAssociazioneModelliFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                            dbProvider.ExecuteNonQuery(commandText);
                        }                       
                    }
                }
                else
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_DISASSOCIA_TIPO_FASC_DIAGRAMMA_3");
                    queryMng.setParam("idDiagramma", idDiagramma);
                    queryMng.setParam("idTipoFasc", idTipoFasc);
                    queryMng.setParam("idStato", idStato);
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneModelliFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaAssociazioneModelliFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                    
                    for (int i = 0; i < modelliSelezionati.Count; i++)
                    {
                        DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm obj = (DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm)modelliSelezionati[i];

                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DS_INS_ASSOCIA_TIPO_FASC_DIAGRAMMA_3");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_ASS_DIAGRAMMI"));
                        queryMng.setParam("idTipoFasc", obj.ID_TIPO_FASC);
                        queryMng.setParam("idDiagramma", obj.ID_DIAGRAMMA);
                        queryMng.setParam("idTemplate", obj.ID_TEMPLATE);
                        queryMng.setParam("idStato", obj.ID_STATO);
                        queryMng.setParam("trasmAut", obj.TRASM_AUT);

                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaAssociazioneModelliFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaAssociazioneModelliFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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

        public void updateScadenzeTipoFasc(int systemId_template, string scadenza, string preScadenza)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_SCADENZE_FASC");
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
                System.Diagnostics.Debug.WriteLine("SQL - updateScadenzeTipoFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - updateScadenzeTipoFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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

        public string getIdTemplateFasc(string idProject)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            string idTemplate = "";

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_ID_TEMPLATE_FASC");
                queryMng.setParam("idProject", idProject);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getIdTemplateFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getIdTemplateFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
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

        public DocsPaVO.ProfilazioneDinamica.Templates getTemplateFascCampiComuniById(DocsPaVO.utente.InfoUtente infoUtente, string idTemplate)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();

            try
            {
                /*
                //Selezione template
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TEMPLATE_FASC");
                queryMng.setParam("idTemplate", idTemplate);
                //queryMng.setParam("docNumber", "''");
                queryMng.setParam("idProject", " AND (DPA_ASS_TEMPLATES_FASC.ID_PROJECT  = '' OR DPA_ASS_TEMPLATES_FASC.ID_PROJECT IS NULL) ");
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascCampiComuniById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplateFascCampiComuniById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                DataSet ds_template = new DataSet();
                dbProvider.ExecuteQuery(ds_template, commandText);

                setTemplate(ref template, ds_template, 0);
                */

                //Selezione template
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE_2_FASC");
                queryMng.setParam("idTemplate", idTemplate);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascCampiComuniById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplateFascCampiComuniById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                DataSet ds_template = new DataSet();
                dbProvider.ExecuteQuery(ds_template, commandText);

                setTemplate(ref template, ds_template, 0);
                
                //Seleziono i SYSTEM_ID per gli oggettiCustom associati al template dalla DPA_ASSOCIAZIONE_TEMPLATES
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_SYSTEM_ID_OGGETTI_CUSTOM_FASC_CAMPI_COMUNI");
                queryMng.setParam("idTemplate", idTemplate);
                queryMng.setParam("idAmm",infoUtente.idAmministrazione);
                queryMng.setParam("idRuolo",infoUtente.idGruppo);
                commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascCampiComuniById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplateFascCampiComuniById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                DataSet ds_systemId_oggettiCustom = new DataSet();
                dbProvider.ExecuteQuery(ds_systemId_oggettiCustom, commandText);
                
                //Cerco gli oggetti custom associati al template
                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;
                for (int j = 0; j < ds_systemId_oggettiCustom.Tables[0].Rows.Count; j++)
                //for (int j = 0; j < ds_template.Tables[0].Rows.Count; j++)
                {
                    /*
                    //Imposto i valori degli oggetti custome dei tipi oggetto
                    oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                    setOggettoCustom(ref oggettoCustom, ds_template, j);

                    DocsPaVO.ProfilazioneDinamica.TipoOggetto tipoOggetto = new DocsPaVO.ProfilazioneDinamica.TipoOggetto();
                    setTipoOggetto(ref tipoOggetto, ds_template, j);

                    //Aggiungo il tipo oggetto all'oggettoCustom
                    oggettoCustom.TIPO = tipoOggetto;
                    */

                    oggettoCustom = new DocsPaVO.ProfilazioneDinamica.OggettoCustom();
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_OGGETTI_CUSTOM_FASC");
                    queryMng.setParam("idOggettoCustom", ds_systemId_oggettiCustom.Tables[0].Rows[j]["ID_OGGETTO"].ToString());
                    queryMng.setParam("idTemplate", idTemplate);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascCampiComuniById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateFascCampiComuniById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

                    DataSet ds_oggettoCustom = new DataSet();
                    dbProvider.ExecuteQuery(ds_oggettoCustom, commandText);
                    setOggettoCustom(ref oggettoCustom, ds_oggettoCustom, 0);

                    //Seleziono il tipo di oggetto
                    DocsPaVO.ProfilazioneDinamica.TipoOggetto tipoOggetto = new DocsPaVO.ProfilazioneDinamica.TipoOggetto();
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TIPI_OGGETTO_1_FASC");
                    queryMng.setParam("idOggettoCustom", ds_oggettoCustom.Tables[0].Rows[0]["ID_TIPO_OGGETTO"].ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascCampiComuniById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateFascCampiComuniById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

                    DataSet ds_tipoOggetto = new DataSet();
                    dbProvider.ExecuteQuery(ds_tipoOggetto, commandText);
                    setTipoOggetto(ref tipoOggetto, ds_tipoOggetto, 0);
                    
                    //Aggiungo il tipo oggetto all'oggettoCustom
                    oggettoCustom.TIPO = tipoOggetto;

                    //campo CLOB di configurazione
                    if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("OGGETTOESTERNO"))
                    {
                        string config = dbProvider.GetLargeText("DPA_OGGETTI_CUSTOM_FASC", oggettoCustom.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST");
                        oggettoCustom.CONFIG_OBJ_EST = config;
                    }

                    //Selezioni i valori per l'oggettoCustom
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI_FASC");
                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascCampiComuniById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateFascCampiComuniById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

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

        public ArrayList getDirittiCampiTipologiaFasc(string idRuolo, string idTemplate)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            ArrayList listaDirittiCampi = new ArrayList();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_A_R_CAMPI_FASC");
                queryMng.setParam("idTemplate", idTemplate);
                queryMng.setParam("idRuolo", idRuolo);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getDirittiCampiTipologiaFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getDirittiCampiTipologiaFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
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
                    DocsPaVO.ProfilazioneDinamica.Templates template = this.getTemplateFascById(idTemplate);
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

        public void salvaDirittiCampiTipologiaFasc(ArrayList listaDirittiCampiSelezionati)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                foreach (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli in listaDirittiCampiSelezionati)
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_A_R_CAMPI_FASC");
                    queryMng.setParam("idTemplate", assDocFascRuoli.ID_TIPO_DOC_FASC);
                    queryMng.setParam("idOggettoCustom", assDocFascRuoli.ID_OGGETTO_CUSTOM);
                    queryMng.setParam("idRuolo", assDocFascRuoli.ID_GRUPPO);
                    queryMng.setParam("insMod", assDocFascRuoli.INS_MOD_OGG_CUSTOM);
                    queryMng.setParam("vis", assDocFascRuoli.VIS_OGG_CUSTOM);
                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - salvaDirittiCampiTipologiaFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - salvaDirittiCampiTipologiaFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    int rowsAffected;
                    dbProvider.ExecuteNonQuery(commandText,out rowsAffected);

                    if (rowsAffected == 0)
                    {
                        queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INS_A_R_CAMPI_FASC");
                        queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                        queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_A_R_OGG_CUSTOM_FASC"));
                        queryMng.setParam("idTemplate", assDocFascRuoli.ID_TIPO_DOC_FASC);
                        queryMng.setParam("idOggettoCustom", assDocFascRuoli.ID_OGGETTO_CUSTOM);
                        queryMng.setParam("idRuolo", assDocFascRuoli.ID_GRUPPO);
                        queryMng.setParam("inserimento", assDocFascRuoli.INS_MOD_OGG_CUSTOM);
                        queryMng.setParam("visibilita", assDocFascRuoli.VIS_OGG_CUSTOM);
                        commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - salvaDirittiCampiTipologiaFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                        logger.Debug("SQL - salvaDirittiCampiTipologiaFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
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

        public void estendiDirittiCampiARuoliFasc(ArrayList listaDirittiCampiSelezionati, ArrayList listaRuoli)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                foreach (DocsPaVO.utente.Ruolo ruolo in listaRuoli)
                {
                    foreach (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli in listaDirittiCampiSelezionati)
                    {
                        DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_UPDATE_A_R_CAMPI_FASC");
                        queryMng.setParam("idTemplate", assDocFascRuoli.ID_TIPO_DOC_FASC);
                        queryMng.setParam("idOggettoCustom", assDocFascRuoli.ID_OGGETTO_CUSTOM);
                        queryMng.setParam("idRuolo", ruolo.idGruppo);
                        queryMng.setParam("insMod", assDocFascRuoli.INS_MOD_OGG_CUSTOM);
                        queryMng.setParam("vis", assDocFascRuoli.VIS_OGG_CUSTOM);
                        string commandText = queryMng.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - estendiDirittiCampiARuoliFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        logger.Debug("SQL - estendiDirittiCampiARuoliFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                        int rowsAffected;
                        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                        if (rowsAffected == 0)
                        {
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_INS_A_R_CAMPI_FASC");
                            queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                            queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_A_R_OGG_CUSTOM_FASC"));
                            queryMng.setParam("idTemplate", assDocFascRuoli.ID_TIPO_DOC_FASC);
                            queryMng.setParam("idOggettoCustom", assDocFascRuoli.ID_OGGETTO_CUSTOM);
                            queryMng.setParam("idRuolo", ruolo.idGruppo);
                            queryMng.setParam("inserimento", assDocFascRuoli.INS_MOD_OGG_CUSTOM);
                            queryMng.setParam("visibilita", assDocFascRuoli.VIS_OGG_CUSTOM);
                            commandText = queryMng.getSQL();
                            System.Diagnostics.Debug.WriteLine("SQL - estendiDirittiCampiARuoliFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                            logger.Debug("SQL - estendiDirittiCampiARuoliFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
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

        public DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli getDirittiCampoTipologiaFasc(string idRuolo, string idTemplate, string idOggettoCustom)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoliResult = null;
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_A_R_CAMPO_FASC");
                queryMng.setParam("idTemplate", idTemplate);
                queryMng.setParam("idOggettoCustom", idOggettoCustom);
                queryMng.setParam("idRuolo", idRuolo);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getDirittiCampoTipologiaFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getDirittiCampoTipologiaFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
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

        public void estendiDirittiRuoloACampiFasc(ArrayList listaDirittiRuoli, ArrayList listaCampi)
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
                            ArrayList listaDirittiCampiSelezionati = new ArrayList();
                            listaDirittiCampiSelezionati.Add(assDocsFascRuoliNew);
                            this.salvaDirittiCampiTipologiaFasc(listaDirittiCampiSelezionati);
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
                            ArrayList listaDirittiCampiSelezionati = new ArrayList();
                            listaDirittiCampiSelezionati.Add(assDocsFascRuoliNew);
                            this.salvaDirittiCampiTipologiaFasc(listaDirittiCampiSelezionati);
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
                            ArrayList listaDirittiCampiSelezionati = new ArrayList();
                            listaDirittiCampiSelezionati.Add(assDocsFascRuoliNew);
                            this.salvaDirittiCampiTipologiaFasc(listaDirittiCampiSelezionati);
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

        public ArrayList getRuoliFromOggettoCustomFasc(string idTemplate, string idOggettoCustom)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            ArrayList ruoliFromOggettoCustom = new ArrayList();
            Utenti ut = new Utenti();

            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_A_R_RUOLI_FROM_OGG_CUSTOM_FASC");
                queryMng.setParam("idTemplate", idTemplate);
                queryMng.setParam("idOggettoCustom", idOggettoCustom);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getRuoliFromOggettoCustomFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
                logger.Debug("SQL - getRuoliFromOggettoCustomFasc - ProfilazioneDinamica/Database/model.cs - QUERY : " + commandText);
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
                logger.Debug("SQL - setTipoOggetto - ProfilazioneDinamica/Database/modelFasc.cs - Exception : " + ex.Message);
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
                if (dataSet.Tables[0].Columns.Contains("ABILITATO"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["ABILITATO"].ToString()))
                        valoreOggetto.ABILITATO = Convert.ToInt32(dataSet.Tables[0].Rows[rowNumber]["ABILITATO"]);
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - setValoreOggetto - ProfilazioneDinamica/Database/modelFasc.cs - Exception : " + ex.Message);
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
                if (dataSet.Tables[0].Columns.Contains("ID_TIPO_FASC"))
                    assDocFascRuoli.ID_TIPO_DOC_FASC = dataSet.Tables[0].Rows[rowNumber]["ID_TIPO_FASC"].ToString();
                if (dataSet.Tables[0].Columns.Contains("ID_OGGETTO_CUSTOM"))
                    assDocFascRuoli.ID_OGGETTO_CUSTOM = dataSet.Tables[0].Rows[rowNumber]["ID_OGGETTO_CUSTOM"].ToString();
                if (dataSet.Tables[0].Columns.Contains("INS_MOD"))
                    assDocFascRuoli.INS_MOD_OGG_CUSTOM = dataSet.Tables[0].Rows[rowNumber]["INS_MOD"].ToString();
                if (dataSet.Tables[0].Columns.Contains("VIS"))
                    assDocFascRuoli.VIS_OGG_CUSTOM = dataSet.Tables[0].Rows[rowNumber]["VIS"].ToString();
                if (dataSet.Tables[0].Columns.Contains("DIRITTI"))
                    assDocFascRuoli.DIRITTI_TIPOLOGIA = dataSet.Tables[0].Rows[rowNumber]["DIRITTI"].ToString();
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - setAssDocFascRuoli - ProfilazioneDinamica/Database/modelFasc.cs - Exception : " + ex.Message);
            }
        }

        private void setTemplate(ref DocsPaVO.ProfilazioneDinamica.Templates template, DataSet dataSet, int rowNumber)
        {
            // Se la riga richiesta non appartiene al dataSet, si esce subito
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count < rowNumber)
                return;

            try
            {
                if (dataSet.Tables[0].Columns.Contains("SYSTEM_ID"))
                {
                    template.SYSTEM_ID = Convert.ToInt32(dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID"].ToString());
                    template.ID_TIPO_FASC = dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID"].ToString();
                }
                if (dataSet.Tables[0].Columns.Contains("SYSTEM_ID_TEMPLATE"))
                {
                    template.SYSTEM_ID = Convert.ToInt32(dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID_TEMPLATE"].ToString());
                    template.ID_TIPO_FASC = dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID_TEMPLATE"].ToString();
                }
                if (dataSet.Tables[0].Columns.Contains("VAR_DESC_FASC"))
                    template.DESCRIZIONE = dataSet.Tables[0].Rows[rowNumber]["VAR_DESC_FASC"].ToString();
                if (dataSet.Tables[0].Columns.Contains("ID_PROJECT"))
                    template.ID_PROJECT = dataSet.Tables[0].Rows[rowNumber]["ID_PROJECT"].ToString();
                if (dataSet.Tables[0].Columns.Contains("ABILITATO_SI_NO"))
                    template.ABILITATO_SI_NO = dataSet.Tables[0].Rows[rowNumber]["ABILITATO_SI_NO"].ToString();
                if (dataSet.Tables[0].Columns.Contains("IN_ESERCIZIO"))
                    template.IN_ESERCIZIO = dataSet.Tables[0].Rows[rowNumber]["IN_ESERCIZIO"].ToString();
                if (dataSet.Tables[0].Columns.Contains("PATH_MOD_1"))
                    template.PATH_MODELLO_1 = dataSet.Tables[0].Rows[rowNumber]["PATH_MOD_1"].ToString();
                if (dataSet.Tables[0].Columns.Contains("PATH_MOD_2"))
                    template.PATH_MODELLO_2 = dataSet.Tables[0].Rows[rowNumber]["PATH_MOD_2"].ToString();
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
                if (dataSet.Tables[0].Columns.Contains("IPERFASCICOLO"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["IPERFASCICOLO"].ToString()) && dataSet.Tables[0].Rows[rowNumber]["IPERFASCICOLO"].ToString() == "1")
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

                if (dataSet.Tables[0].Columns.Contains("ID_TEMPLATE_STRUTTURA"))
                {
                    template.ID_TEMPLATE_STRUTTURA = dataSet.Tables[0].Rows[rowNumber]["ID_TEMPLATE_STRUTTURA"].ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - setTemplate - ProfilazioneDinamica/Database/modelFasc.cs - Exception : " + ex.Message);
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
                if(dataSet.Tables[0].Columns.Contains("FORMATO_ORA"))
                    oggettoCustom.FORMATO_ORA = dataSet.Tables[0].Rows[rowNumber]["FORMATO_ORA"].ToString();
                if (dataSet.Tables[0].Columns.Contains("TIPO_LINK"))
                    oggettoCustom.TIPO_LINK = dataSet.Tables[0].Rows[rowNumber]["TIPO_LINK"].ToString();
                if (dataSet.Tables[0].Columns.Contains("TIPO_OBJ_LINK"))
                    oggettoCustom.TIPO_OBJ_LINK = dataSet.Tables[0].Rows[rowNumber]["TIPO_OBJ_LINK"].ToString();
                if(dataSet.Tables[0].Columns.Contains("CODICE_DB"))
                    oggettoCustom.CODICE_DB = dataSet.Tables[0].Rows[rowNumber]["CODICE_DB"].ToString();
                if (dataSet.Tables[0].Columns.Contains("MANUAL_INSERT"))
                {
                    string value = dataSet.Tables[0].Rows[rowNumber]["MANUAL_INSERT"].ToString();
                    if (!string.IsNullOrEmpty(value) && "1".Equals(value))
                    {
                        oggettoCustom.MANUAL_INSERT = true;
                    }
                }
                if (dataSet.Tables[0].Columns.Contains("DTA_INS"))
                    oggettoCustom.DATA_INSERIMENTO = dataSet.Tables[0].Rows[rowNumber]["DTA_INS"].ToString();
                this.ControllaCustom(ref oggettoCustom);
                if (dataSet.Tables[0].Columns.Contains("ANNO_ACC"))
                {
                    if (!string.IsNullOrEmpty(dataSet.Tables[0].Rows[rowNumber]["ANNO_ACC"].ToString()))
                        oggettoCustom.ANNO_ACC = dataSet.Tables[0].Rows[rowNumber]["ANNO_ACC"].ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - setOggettoCustom - ProfilazioneDinamica/Database/model.cs - Exception : " + ex.Message);
            }
        }

        public ArrayList getAttributiTipoFasc(string idTipoFasc)
        {
            ArrayList listaAttributi = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            try
            {
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_ATTRIBUTI_TIPOFASC");
                queryMng.setParam("id_template", idTipoFasc);
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getAttributiTipoFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - getAttributiTipoFasc - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                DataSet ds_attributi = new DataSet();
                dbProvider.ExecuteQuery(ds_attributi, commandText);
                if (ds_attributi.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds_attributi.Tables[0].Rows.Count; i++)
                    {
                        //listaAttributi.Add(ds_attributi.Tables[0].Rows[i]["descrizione"].ToString());
                        DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();

                        template.SYSTEM_ID = Convert.ToInt32(ds_attributi.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                        template.DESCRIZIONE = ds_attributi.Tables[0].Rows[i]["DESCRIZIONE"].ToString();
                        listaAttributi.Add(template);
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

            return listaAttributi;



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
                Query query = DocsPaUtils.InitQuery.getInstance().getQuery("GET_PRJ_MODEL_NAME");

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

        #region STORICO
        /// <summary>
        /// Funzione per il recupero dello storico di un modello inserito in un fascicolo
        /// </summary>
        /// <param name="modelId">Id del modello di cui caricare il nome</param>
        /// <returns>Lista degli oggetti StoricoProfilati</returns>
        public ArrayList getListaStoricoFascicolo(string id_tipo_fasc, string idProject)
        {
            ArrayList storico = new ArrayList();
            //sto inserendo i campi profilati ma non ho ancora salvato, quindi non ho dati sui template
            if (id_tipo_fasc == "")
            {
                return storico;
            }
            using (DBProvider dbProvider = new DBProvider())
            {
                // Caricamento della query da eseguire
                Query query = DocsPaUtils.InitQuery.getInstance().getQuery("GET_STO_FASCICOLO");

                // Impostazione dell'id del modello
                query.setParam("id_template", id_tipo_fasc);
                //impostazione dell'id del fascicolo al quale è associato il modello
                query.setParam("id_project", idProject);
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
                            string commandText = string.Empty;
                            DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue ogg = (DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue)listOldObj[indexListOldObj];
                            //DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_PROFIL_FASC_STO");
                            //if (dbProvider.DBType.ToUpper().Equals("ORACLE"))
                            //    queryMng.setParam("param0", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_PROFIL_FASC_STO"));
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

                            ArrayList parameters = new ArrayList();
                            parameters.Add(new ParameterSP("objType", "F", DirectionParameter.ParamInput));
                            parameters.Add(new ParameterSP("Idtemplate", ogg.IDTemplate, DirectionParameter.ParamInput));
                            parameters.Add(new ParameterSP("idDocOrFasc", ogg.ID_Doc_Fasc, DirectionParameter.ParamInput));
                            parameters.Add(new ParameterSP("Idoggcustom", ogg.ID_Oggetto, DirectionParameter.ParamInput));
                            parameters.Add(new ParameterSP("Idpeople", ogg.ID_People, DirectionParameter.ParamInput));
                            parameters.Add(new ParameterSP("Idruoloinuo", ogg.ID_Ruolo_In_UO, DirectionParameter.ParamInput));
                            parameters.Add(new ParameterSP("Descmodifica", ogg.Valore, DirectionParameter.ParamInput));

                            dbProvider.ExecuteStoreProcedure("InsertDataInHistoryProf", parameters);
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
        #endregion


        public String GetTipologyDescriptionByIdProject(string idPrj)
        {
            Query query = InitQuery.getInstance().getQuery("S_DOC_TIPOLOGY_DESC_BY_ID_PRJ");
            query.setParam("idPrj", idPrj);

            String retVal = String.Empty;
            using (DBProvider dbProvider = new DBProvider())
            {
                IDataReader reader = dbProvider.ExecuteReader(query.getSQL());
                while (reader.Read())
                    retVal = reader[0].ToString();
            }

            return retVal;
        }

        public string getSeriePerRicercaProfilazionePolicy(DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            string queryStr = string.Empty;

            if (template != null && template.ELENCO_OGGETTI != null)
            {
                //Imposto il filtro per la ricercare solo sui campi profilati che sono campi di ricerca
                //queryStr += "A.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0, dpa_oggetti_custom_fasc dpa1 where dpa0.id_oggetto =  dpa1.system_id and dpa1.campo_di_ricerca = 'SI') ";

                bool firstFilter = true;

                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto in template.ELENCO_OGGETTI)
                {
                    string operatoreRicerca = string.Empty;

                    if (!firstFilter && !string.IsNullOrEmpty(queryStr))
                        operatoreRicerca = template.OPERATORE_RICERCA_OGGETTI.ToString();

                    switch (oggetto.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CampoDiTesto":
                            if (oggetto.VALORE_DATABASE != null && oggetto.VALORE_DATABASE != "")
                            {
                                queryStr += operatoreRicerca + " pr.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND UPPER(dpa0.valore_oggetto_db) " + this.getOperatoreQueryOggettoCustomStringa(oggetto.TIPO_RICERCA_STRINGA) + " UPPER('" + this.getValoreQueryOggettoCustomStringa(oggetto.TIPO_RICERCA_STRINGA, oggetto.VALORE_DATABASE) + "') ) ) ";
                            }
                            break;

                        case "CasellaDiSelezione":
                            for (int j = 0; j < oggetto.VALORI_SELEZIONATI.Count; j++)
                            {
                                if (oggetto.VALORI_SELEZIONATI[j] != null && (string)oggetto.VALORI_SELEZIONATI[j] != "")
                                {
                                    queryStr += operatoreRicerca + " pr.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND UPPER(dpa0.valore_oggetto_db) = UPPER('" + oggetto.VALORI_SELEZIONATI[j].ToString().Replace("'", "''") + "')) ) ";
                                    operatoreRicerca = template.OPERATORE_RICERCA_OGGETTI.ToString().ToLowerInvariant();
                                    firstFilter = false;
                                }
                                //else
                                //{
                                //    queryStr += operatoreRicerca + " A.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND ( dpa0.valore_oggetto_db = '' OR dpa0.valore_oggetto_db is null )) ) ";
                                //}
                            }
                            break;

                        case "MenuATendina":
                            if (oggetto.VALORE_DATABASE != null && oggetto.VALORE_DATABASE != "")
                            {
                                queryStr += operatoreRicerca + " pr.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND UPPER(dpa0.valore_oggetto_db) = UPPER('" + oggetto.VALORE_DATABASE.Replace("'", "''") + "')) ) ";
                            }
                            break;

                        case "SelezioneEsclusiva":
                            if (oggetto.VALORE_DATABASE != null && oggetto.VALORE_DATABASE != "")
                            {
                                queryStr += operatoreRicerca + " pr.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND UPPER(dpa0.valore_oggetto_db) = UPPER('" + oggetto.VALORE_DATABASE.Replace("'", "''") + "')) ) ";
                            }
                            break;

                        case "Contatore":
                            switch (oggetto.TIPO_CONTATORE)
                            {
                                case "T":
                                    if (!oggetto.VALORE_DATABASE.Equals(""))
                                    {
                                        if (oggetto.VALORE_DATABASE.IndexOf('@') != -1)
                                        {
                                            string[] contatore = oggetto.VALORE_DATABASE.Split('@');
                                            queryStr += operatoreRicerca + " pr.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[1], false) + " ) )";
                                        }
                                        else
                                        {
                                            queryStr += operatoreRicerca + " pr.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_DATABASE, false) + " ) )";
                                        }
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
                                                queryStr += operatoreRicerca + " pr.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[1], false) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ) )";
                                            }
                                            else
                                            {
                                                queryStr += operatoreRicerca + " pr.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_DATABASE, false) + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ) )";
                                            }
                                        }
                                        else
                                        {
                                            //Nr.Contatore NO - Aoo/Rf SI
                                            queryStr += operatoreRicerca + " pr.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND dpa0.ID_AOO_RF = " + oggetto.ID_AOO_RF + " ) )";
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
                                                queryStr += operatoreRicerca + " pr.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[0], false) + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " <= " + DocsPaDbManagement.Functions.Functions.ToInt(contatore[1], false) + " ) )";
                                            }
                                            else
                                            {
                                                queryStr += operatoreRicerca + " pr.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToInt("dpa0.valore_oggetto_db", true) + " >= " + DocsPaDbManagement.Functions.Functions.ToInt(oggetto.VALORE_DATABASE, false) + " ) )";
                                            }
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
                                    queryStr += operatoreRicerca + " pr.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToDateColumn("dpa0.valore_oggetto_db") + " >= " + DocsPaDbManagement.Functions.Functions.ToDate(date[0]) + " AND " + DocsPaDbManagement.Functions.Functions.ToDateColumn("dpa0.valore_oggetto_db") + " <= " + DocsPaDbManagement.Functions.Functions.ToDate(date[1]) + " ) ) ";
                                }
                                else
                                {
                                    queryStr += operatoreRicerca + " pr.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND " + DocsPaDbManagement.Functions.Functions.ToDateColumn("dpa0.valore_oggetto_db") + " >= " + DocsPaDbManagement.Functions.Functions.ToDate(oggetto.VALORE_DATABASE) + " ) ) ";
                                }
                            }
                            break;

                        case "Corrispondente":
                            if (oggetto.VALORE_DATABASE != null && oggetto.VALORE_DATABASE != "")
                            {
                                int codiceCorrispondente;
                                if (Int32.TryParse(oggetto.VALORE_DATABASE, out codiceCorrispondente))
                                {
                                    queryStr += operatoreRicerca + " pr.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND upper(dpa0.valore_oggetto_db) in( select upper(SYSTEM_ID) from DPA_CORR_GLOBALI where SYSTEM_ID = " + codiceCorrispondente + ")))";
                                }
                                else
                                {
                                    //

                                    queryStr += operatoreRicerca + " pr.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() +
                                        " AND upper(dpa0.valore_oggetto_db) in( select SYSTEM_ID from DPA_CORR_GLOBALI where UPPER(VAR_DESC_CORR) " + this.getOperatoreQueryOggettoCustomStringa(oggetto.TIPO_RICERCA_STRINGA) + " UPPER('" + this.getValoreQueryOggettoCustomStringa(oggetto.TIPO_RICERCA_STRINGA, oggetto.VALORE_DATABASE) + "') )))";

                                    //queryStr += operatoreRicerca + " A.system_id in  (SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString() + " AND upper(dpa0.valore_oggetto_db) in( select SYSTEM_ID from DPA_CORR_GLOBALI where UPPER(VAR_DESC_CORR) like UPPER('%" + oggetto.VALORE_DATABASE.Replace("'", "''") + "%') )))";
                                }
                            }
                            break;
                        case "OggettoEsterno":
                            if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE) || !string.IsNullOrEmpty(oggetto.CODICE_DB))
                            {
                                string assQuery = "SELECT id_project FROM dpa_ass_templates_fasc dpa0 where (dpa0.id_oggetto = " + oggetto.SYSTEM_ID.ToString();
                                if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                                {
                                    assQuery = assQuery + " AND UPPER(dpa0.valore_oggetto_db) LIKE UPPER('%" + oggetto.VALORE_DATABASE.Replace("'", "''") + "%')";
                                }
                                if (!string.IsNullOrEmpty(oggetto.CODICE_DB))
                                {
                                    assQuery = assQuery + " AND UPPER(dpa0.codice_db) = UPPER('" + oggetto.CODICE_DB.Replace("'", "''") + "')";
                                }
                                assQuery = assQuery + ")";
                                queryStr += operatoreRicerca + " pr.system_id in  (" + assQuery + ")";
                            }
                            break;
                    }

                    if (firstFilter)
                        firstFilter = false;
                }

                if (!string.IsNullOrEmpty(queryStr))
                {
                    queryStr = string.Format(" {0} ({1})",
                        DocsPaVO.ProfilazioneDinamica.OperatoriRicercaOggettiCustomEnum.And.ToString().ToLowerInvariant(), queryStr);
                }
            }

            return queryStr;
        }

        public DocsPaVO.ProfilazioneDinamica.Templates getTemplateFascByDescrizione(string descrizione)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            DocsPaVO.ProfilazioneDinamica.Templates template = new DocsPaVO.ProfilazioneDinamica.Templates();

            try
            {
                //Selezione template
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_TEMPLATE_FASC_BY_DESC");
                queryMng.setParam("descrizione", descrizione.Replace("'", "''"));
                //queryMng.setParam("docNumber", "''");
                queryMng.setParam("idProject", " AND (DPA_ASS_TEMPLATES_FASC.ID_PROJECT  = '' OR DPA_ASS_TEMPLATES_FASC.ID_PROJECT IS NULL) ");
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                logger.Debug("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                DataSet ds_template = new DataSet();
                dbProvider.ExecuteQuery(ds_template, commandText);

                //Se il template non ha oggetti custom vengono restituite solo le proprietà del template
                if (ds_template.Tables[0].Rows.Count == 0)
                {
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_TEMPLATE_2_FASC_BY_DESC");
                    queryMng.setParam("descrizione", descrizione);
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    ds_template = new DataSet();
                    dbProvider.ExecuteQuery(ds_template, commandText);

                    setTemplate(ref template, ds_template, 0);
                    return template;
                }

                setTemplate(ref template, ds_template, 0);

                //Cerco gli oggetti custom associati al template
                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;
                //for (int j = 0; j < ds_systemId_oggettiCustom.Tables[0].Rows.Count; j++)
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
                        string config = dbProvider.GetLargeText("DPA_OGGETTI_CUSTOM_FASC", oggettoCustom.SYSTEM_ID.ToString(), "CONFIG_OBJ_EST");
                        oggettoCustom.CONFIG_OBJ_EST = config;
                    }

                    //Selezioni i valori per l'oggettoCustom
                    queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PD_GET_DPA_ASSOCIAZIONE_VALORI_FASC");
                    queryMng.setParam("idOggettoCustom", oggettoCustom.SYSTEM_ID.ToString());
                    commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);
                    logger.Debug("SQL - getTemplateFascById - ProfilazioneDinamica/Database/modelFasc.cs - QUERY : " + commandText);

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

        public ArrayList getIdPrjByAssTemplates(string idOggFasc, string valoreDB, string ordine)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            ArrayList retVal = new ArrayList();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_IDPRJ_BY_ASS_TEMPLATES");
                q.setParam("id_ogg_fasc", idOggFasc);
                q.setParam("valore_oggetto_db", valoreDB);
                if (string.IsNullOrEmpty(ordine) || ordine.ToUpper() != "DESC")
                    q.setParam("ordine", "ASC");
                else q.setParam("ordine", "DESC");

                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataset = new DataSet();
                dbProvider.ExecuteQuery(out dataset, "IDPROJECTS", queryString);
                if (dataset.Tables["IDPROJECTS"] != null && dataset.Tables["IDPROJECTS"].Rows.Count > 0)
                {
                    foreach (DataRow r in dataset.Tables["IDPROJECTS"].Rows)
                    {
                        retVal.Add(r["ID_PROJECT"].ToString());
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
    }
}
