using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    public class RicProfCampiComuni : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(RicProfCampiComuni));
        public RicProfCampiComuni() { }

        public ArrayList eseguiRicercaCampiComuni(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] listaFiltri, int  numPage, int pageSize, out int nRec)
        {
            ArrayList listaDocFasc = new ArrayList();
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            nRec = getCountDocFasc(infoUtente, listaFiltri);

            if (nRec > 0)
            {
                try
                {
                    //Condizioni security e amministrazione
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_CAMPI_COMUNI_DOC_FASC");
                    queryMng.setParam("idPeople", infoUtente.idPeople);
                    queryMng.setParam("idGruppo", infoUtente.idGruppo);
                    queryMng.setParam("idAmm", infoUtente.idAmministrazione);

                    string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                    if (string.IsNullOrEmpty(idRuoloPubblico))
                        idRuoloPubblico = "0";
                    queryMng.setParam("idRuoloPubblico", idRuoloPubblico);

                    //SQL Server
                    int pageSizeSqlServer = pageSize;
                    int totalRowsSqlServer = (numPage * pageSize);
                    if ((nRec - totalRowsSqlServer) <= 0)
                    {
                        pageSizeSqlServer -= System.Math.Abs(nRec - totalRowsSqlServer);
                        totalRowsSqlServer = nRec;
                    }
                    queryMng.setParam("pageSize", pageSizeSqlServer.ToString());
                    queryMng.setParam("totalRows", totalRowsSqlServer.ToString());

                    //ORACLE
                    int numTotPage = (nRec / pageSize);
                    int startRow = ((numPage * pageSize) - pageSize) + 1;
                    int endRow = (startRow - 1) + pageSize;
                    queryMng.setParam("startRow", startRow.ToString());
                    queryMng.setParam("endRow", endRow.ToString());

                    //Condizioni registro
                    Utenti utenti = new Utenti();
                    ArrayList registri = utenti.GetRuoloByIdGruppo(infoUtente.idGruppo).registri;
                    string paramRegistri = " AND (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO IN (";
                    foreach (DocsPaVO.utente.Registro registro in registri)
                    {
                        paramRegistri += registro.systemId + ",";
                    }
                    paramRegistri = paramRegistri.Substring(0, paramRegistri.Length - 1) + ")) ";
                    queryMng.setParam("condIdRegistri", paramRegistri);

                    //Condizioni titolari
                    Amministrazione amministrazione = new Amministrazione();
                    ArrayList titolari = amministrazione.getTitolariUtilizzabili(infoUtente.idAmministrazione);
                    string paramTitolari = " AND A.ID_TITOLARIO IN (";
                    foreach (DocsPaVO.amministrazione.OrgTitolario titolario in titolari)
                    {
                        paramTitolari += titolario.ID + ",";
                    }
                    paramTitolari = paramTitolari.Substring(0, paramTitolari.Length - 1) + ") ";
                    queryMng.setParam("condIdTitolari", paramTitolari);

                    //Condizioni profilazione campi comuni 
                    for (int i = 0; i < listaFiltri.Length; i++)
                    {
                        for (int j = 0; j < listaFiltri[i].Length; j++)
                        {
                            DocsPaVO.filtri.FiltroRicerca f = listaFiltri[i][j];
                            if (f.valore != null && !f.valore.Equals(""))
                            {
                                switch (f.argomento)
                                {
                                    //Condizioni profilazione campi comuni fascicolo
                                    case "TEMPLATE_CAMPI_COMUNI_FASC":
                                        DocsPaDB.Query_DocsPAWS.ModelFasc modelFasc = new ModelFasc();
                                        string paramCondProfilazioneFascicoli = string.Empty;
                                        paramCondProfilazioneFascicoli = modelFasc.getSeriePerRicercaProfilazione(f.template, "");
                                        if (!string.IsNullOrEmpty(paramCondProfilazioneFascicoli))
                                            queryMng.setParam("condProfilazioneFascicoli", paramCondProfilazioneFascicoli);
                                        else
                                            queryMng.setParam("condProfilazioneFascicoli", " ");
                                        break;
                                    //Condizioni profilazione campi comuni documento
                                    case "TEMPLATE_CAMPI_COMUNI_DOC":
                                        DocsPaDB.Query_DocsPAWS.Model model = new Model();
                                        string paramCondProfilazioneDocumenti = string.Empty;
                                        paramCondProfilazioneDocumenti = model.getSeriePerRicercaProfilazione(f.template, "");
                                        if (!string.IsNullOrEmpty(paramCondProfilazioneDocumenti))
                                            queryMng.setParam("condProfilazioneDocumenti", paramCondProfilazioneDocumenti);
                                        else
                                            queryMng.setParam("condProfilazioneDocumenti", " ");
                                        break;
                                }
                            }
                        }
                    }

                    //Condizioni inizio e fine anno (Per il momento viene considerato di default l'anno corrente)
                    string anno = Convert.ToString(System.DateTime.Now.Year);
                    queryMng.setParam("startDateDoc", "01/01/" + anno);
                    queryMng.setParam("endDateDoc", "31/12/" + anno);

                    //Solo per il database SQL Server va effettuata la sostituzione del parametro @dbUser@
                    string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
                    if (dbType.ToUpper() == "SQL")
                        queryMng.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - eseguiRicercaCampiComuni - RicProfCampiComuni.cs - QUERY : " + commandText);
                    logger.Debug("SQL - eseguiRicercaCampiComuni - RicProfCampiComuni.cs - QUERY : " + commandText);
                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);

                    DocsPaVO.ProfilazioneDinamica.ItemRicCampiComuni itemRicCampiComuni = null;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        itemRicCampiComuni = new DocsPaVO.ProfilazioneDinamica.ItemRicCampiComuni();
                        setItemRicCampiComuni(ref itemRicCampiComuni, ds, i);
                        listaDocFasc.Add(itemRicCampiComuni);
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("SQL - eseguiRicercaCampiComuni - RicProfCampiComuni.cs - Eccezione : " + ex.Message);
                    logger.Debug("SQL - eseguiRicercaCampiComuni - RicProfCampiComuni.cs - Eccezione : " + ex.Message);
                    return null;
                }
                finally
                {
                    dbProvider.Dispose();
                }
            }            
            return listaDocFasc;
        }

        private int getCountDocFasc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] listaFiltri)
        {
            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
            int retValue = 0;

            try
            {
                //Condizioni security e amministrazione
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_CAMPI_COMUNI_DOC_FASC");
                queryMng.setParam("idPeople", infoUtente.idPeople);
                queryMng.setParam("idGruppo", infoUtente.idGruppo);
                queryMng.setParam("idAmm", infoUtente.idAmministrazione);

                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                if (string.IsNullOrEmpty(idRuoloPubblico))
                    idRuoloPubblico = "0";
                queryMng.setParam("idRuoloPubblico", idRuoloPubblico);

                //Condizioni registro
                Utenti utenti = new Utenti();
                ArrayList registri = utenti.GetRuoloByIdGruppo(infoUtente.idGruppo).registri;
                string paramRegistri = " AND (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO IN (";
                foreach (DocsPaVO.utente.Registro registro in registri)
                {
                    paramRegistri += registro.systemId + ",";
                }
                paramRegistri = paramRegistri.Substring(0, paramRegistri.Length - 1) + ")) ";
                queryMng.setParam("condIdRegistri", paramRegistri);

                //Condizioni titolari
                Amministrazione amministrazione = new Amministrazione();
                ArrayList titolari = amministrazione.getTitolariUtilizzabili(infoUtente.idAmministrazione);
                string paramTitolari = " AND A.ID_TITOLARIO IN (";
                foreach (DocsPaVO.amministrazione.OrgTitolario titolario in titolari)
                {
                    paramTitolari += titolario.ID + ",";
                }
                paramTitolari = paramTitolari.Substring(0, paramTitolari.Length - 1) + ") ";
                queryMng.setParam("condIdTitolari", paramTitolari);

                //Condizioni profilazione campi comuni 
                for (int i = 0; i < listaFiltri.Length; i++)
                {
                    for (int j = 0; j < listaFiltri[i].Length; j++)
                    {
                        DocsPaVO.filtri.FiltroRicerca f = listaFiltri[i][j];
                        if (f.valore != null && !f.valore.Equals(""))
                        {
                            switch (f.argomento)
                            {
                                //Condizioni profilazione campi comuni fascicolo
                                case "TEMPLATE_CAMPI_COMUNI_FASC":
                                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFasc = new ModelFasc();
                                    string paramCondProfilazioneFascicoli = string.Empty;
                                    paramCondProfilazioneFascicoli = modelFasc.getSeriePerRicercaProfilazione(f.template, "");
                                    if (!string.IsNullOrEmpty(paramCondProfilazioneFascicoli))
                                        queryMng.setParam("condProfilazioneFascicoli", paramCondProfilazioneFascicoli);
                                    else
                                        queryMng.setParam("condProfilazioneFascicoli", " ");
                                    break;
                                //Condizioni profilazione campi comuni documento
                                case "TEMPLATE_CAMPI_COMUNI_DOC":
                                    DocsPaDB.Query_DocsPAWS.Model model = new Model();
                                    string paramCondProfilazioneDocumenti = string.Empty;
                                    paramCondProfilazioneDocumenti = model.getSeriePerRicercaProfilazione(f.template, "");
                                    if (!string.IsNullOrEmpty(paramCondProfilazioneDocumenti))
                                        queryMng.setParam("condProfilazioneDocumenti", paramCondProfilazioneDocumenti);
                                    else
                                        queryMng.setParam("condProfilazioneDocumenti", " ");
                                    break;
                            }
                        }
                    }
                }

                //Condizioni inizio e fine anno (Per il momento viene considerato di default l'anno corrente)
                string anno = Convert.ToString(System.DateTime.Now.Year);
                queryMng.setParam("startDateDoc", "01/01/" + anno);
                queryMng.setParam("endDateDoc", "31/12/" + anno);

                //Solo per il database SQL Server va effettuata la sostituzione del parametro @dbUser@
                string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
                if (dbType.ToUpper() == "SQL")
                    queryMng.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - getCountDocFasc - RicProfCampiComuni.cs - QUERY : " + commandText);
                logger.Debug("SQL - getCountDocFasc - RicProfCampiComuni.cs - QUERY : " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);

                string field;
                if (this.ExecuteScalar(out field, commandText))
                    Int32.TryParse(field, out retValue);

                return retValue;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SQL - getCountDocFasc - RicProfCampiComuni.cs - Eccezione : " + ex.Message);
                logger.Debug("SQL - getCountDocFasc - RicProfCampiComuni.cs - Eccezione : " + ex.Message);
                return retValue;
            }
            finally
            {
                dbProvider.Dispose();
            }
        }

        private void setItemRicCampiComuni(ref DocsPaVO.ProfilazioneDinamica.ItemRicCampiComuni itemRicCampiComuni, DataSet dataSet, int rowNumber)
        {
            try
            {
                if (dataSet.Tables[0].Columns.Contains("SYSTEM_ID"))
                    itemRicCampiComuni.SYSTEM_ID = dataSet.Tables[0].Rows[rowNumber]["SYSTEM_ID"].ToString();                
                if (dataSet.Tables[0].Columns.Contains("DESCRIPTION"))
                    itemRicCampiComuni.DESCRIPTION = dataSet.Tables[0].Rows[rowNumber]["DESCRIPTION"].ToString();        
                if (dataSet.Tables[0].Columns.Contains("TIPO"))
                    itemRicCampiComuni.TIPO = dataSet.Tables[0].Rows[rowNumber]["TIPO"].ToString();     
                if (dataSet.Tables[0].Columns.Contains("CODICE_SEGNATURA"))
                    itemRicCampiComuni.CODICE_SEGNATURA = dataSet.Tables[0].Rows[rowNumber]["CODICE_SEGNATURA"].ToString();
                if (dataSet.Tables[0].Columns.Contains("DATA_CREAZIONE"))
                    itemRicCampiComuni.DATA_CREAZIONE = dataSet.Tables[0].Rows[rowNumber]["DATA_CREAZIONE"].ToString();     
            }
            catch (Exception ex)
            {
                logger.Debug("SQL - setItemRicCampiComuni - RicProfCampiComuni.cs - Eccezione : " + ex.Message);
            }
        }
    }
}
