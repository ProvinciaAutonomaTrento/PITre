using System;
using System.Collections;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Xml;
using System.Configuration;
using System.IO;
using System.Data;
using System.Threading;
using DocsPaUtils.Data;
using DocsPaDB;
using log4net;

namespace ProspettiRiepilogativi
{

	public class Model
	{
        private ILog logger = LogManager.GetLogger(typeof(Model));
		public Model()
		{
		}


		#region Metodi Pubblici
		/*
		 * Questa Region conterrà i metodi per il recupero dei 
		 * dati necessari alla produzione dei reports
		 */

		#region DO_GetAmministrazioni
		/// <summary>
		/// DO_GetAmministrazioni
		/// </summary>
		/// <returns>arrayList di amministrazioni</returns>
		public ArrayList DO_GetAmministrazioni()
		{
			ArrayList result = new ArrayList();

			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_AMMINISTRAZIONI");		
				string commandText=queryMng.getSQL();
				logger.Debug("Prospetti Riepilogativi - PR_DO_GET_AMMINISTRAZIONI: "+commandText);
				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				if (ds.Tables[0].Rows.Count != 0)
				{
					for (int i=0; i<ds.Tables[0].Rows.Count;i++)
					{
						PR_Amministrazione prAmm = new PR_Amministrazione
							(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString(),
							ds.Tables[0].Rows[i]["VAR_CODICE_AMM"].ToString(),
							ds.Tables[0].Rows[i]["VAR_DESC_AMM"].ToString(),
							ds.Tables[0].Rows[i]["VAR_LIBRERIA"].ToString());

						result.Add(prAmm);
					}
				}
				result.TrimToSize();
				

			}
			catch (Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_AMMINISTRAZIONI: ",ex);
			}
			return result;
		}
		#endregion

		#region DO_GetRegistri
		/// <summary>
		/// DO_GetRegistri
		/// </summary>
		/// <param name="id_amm">System_id amministrazione</param>
		/// <returns>arrayList di registri</returns>
		public ArrayList DO_GetRegistri(int id_amm)
		{
			ArrayList result = new ArrayList();

			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_REGISTRI_BY_AMM");
                queryMng.setParam("IDAMM", id_amm.ToString());
                string commandText=queryMng.getSQL();
				logger.Debug("Prospetti Riepilogativi - PR_DO_GET_REGISTRI_BY_AMM: "+commandText);
				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				if (ds.Tables[0].Rows.Count != 0)
				{
					for (int i=0; i<ds.Tables[0].Rows.Count;i++)
					{
						PR_Registro prReg = new PR_Registro
							(ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString(),
							ds.Tables[0].Rows[i]["VAR_CODICE"].ToString(),
							ds.Tables[0].Rows[i]["VAR_DESC_REGISTRO"].ToString());
						result.Add(prReg);
					}
				}
				result.TrimToSize();
			}
			catch (Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_REGISTRI_BY_AMM: ",ex);
			}
			return result;
		}
		#endregion
		
		#region DO_GetAmmById
		public PR_Amministrazione DO_GetAmmById(int id_amm)
		{
			PR_Amministrazione prAmm = new PR_Amministrazione();
			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_DATI_AMMINSTRAZIONE_BY_COD");		
				queryMng.setParam("IDAMM",id_amm.ToString());
				string commandText=queryMng.getSQL();
				logger.Debug("Prospetti Riepilogativi - PR_DO_GET_DATI_AMMINSTRAZIONE_BY_COD: "+commandText);
				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				if (ds.Tables[0].Rows.Count != 0)
				{
					prAmm.System_id = id_amm.ToString();
					prAmm.Descrizione = ds.Tables[0].Rows[0]["VAR_DESC_AMM"].ToString();
					prAmm.Codice = ds.Tables[0].Rows[0]["VAR_CODICE_AMM"].ToString();
				}
			}
			catch (Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_DATI_AMMINSTRAZIONE_BY_COD: ",ex);
			}
			return prAmm;
		}
		#endregion

		#region DO_GetVarDescAmmById
		public string DO_GetVarDescAmmById(int id_amm)
		{
			string result = String.Empty;
			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_DATI_AMMINSTRAZIONE_BY_COD");		
				queryMng.setParam("IDAMM",id_amm.ToString());
				string commandText=queryMng.getSQL();
				logger.Debug("Prospetti Riepilogativi - PR_DO_GET_DATI_AMMINSTRAZIONE_BY_COD: "+commandText);
				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				if (ds.Tables[0].Rows.Count != 0)
				{
					result = ds.Tables[0].Rows[0]["VAR_DESC_AMM"].ToString();
				}
			}
			catch (Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_DATI_AMMINSTRAZIONE_BY_COD: ",ex);
			}
			return result;
		}
		#endregion

		#region DO_GetIdAmmByCodice
		/// <summary>
		/// DO_GetIdAmmByCodice
		/// </summary>
		/// <param name="codAmm">codice amministrazione</param>
		/// <returns>system_id amministrazione come intero</returns>
		public int DO_GetIdAmmByCodice(string codAmm)
		{
			int result = 0;
			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_COD_AMM_BY_DESC_AMM");		
				queryMng.setParam("CODAMM",codAmm);
				string commandText=queryMng.getSQL();
				logger.Debug("Prospetti Riepilogativi - PR_DO_GET_COD_AMM_BY_DESC_AMM: "+commandText);
				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				if (ds.Tables[0].Rows.Count != 0)
				{
					result = Convert.ToInt32(ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString());
				}
			}
			catch (Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_COD_AMM_BY_DESC_AMM: ",ex);
			}
			return result;
		}
		#endregion

		#region DO_GetAnniProfilazione
		/// <summary>
		/// DO_GetAnniProfilazione
		/// </summary>
		/// <param name="idReg">id del registro di interesse</param>
		/// <returns>arrayList contenente tutti gli anni di profilazione</returns>
		public ArrayList DO_GetAnniProfilazione(int idReg)
		{
			ArrayList result = new ArrayList();
			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_ANNI_PROFILAZIONE_BY_REG");		
				queryMng.setParam("IDREG",idReg.ToString());
				string commandText=queryMng.getSQL();
				logger.Debug("Prospetti Riepilogativi - PR_DO_GET_ANNI_PROFILAZIONE_BY_REG: "+commandText);
				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				if (ds.Tables[0].Rows.Count != 0)
				{
					for (int i=0; i<ds.Tables[0].Rows.Count;i++)
					{
						result.Add(ds.Tables[0].Rows[i]["ANNO"].ToString());
					}
				}
				result.TrimToSize();
			}
			catch (Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_ANNI_PROFILAZIONE_BY_REG: ",ex);
			}

			return result;
		}
		#endregion

		#region DO_GetSedi (DA RIVEDERE!!)
		/// <summary>
		/// DO_GetSedi(): recupera se disponibili, le sedi 
		/// </summary>
		/// <returns>
		/// lista di sedi
		/// </returns>
		public ArrayList DO_GetSedi() //TODO: DA RIVEDERE GESTIRE ANCHE L'AMMINISTRAZIONE!!!!!
		{
			ArrayList result = new ArrayList();

			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_SEDI");		
				string commandText=queryMng.getSQL();
				logger.Debug("Prospetti Riepilogativi - PR_DO_GET_SEDI: "+commandText);
				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				if (ds.Tables[0].Rows.Count != 0)
				{
					for (int i=0; i<ds.Tables[0].Rows.Count;i++)
					{
						result.Add(ds.Tables[0].Rows[i]["SEDE"].ToString());
					}
				}
				result.TrimToSize();
			}
			catch (Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_SEDI: ",ex);
			}

			return result;
		}
		#endregion

		#region DO_GetDescReg
		/// <summary>
		/// DO_GetDescReg
		/// </summary>
		/// <param name="idReg">system_id del registro</param>
		/// <returns>descrizione del registro</returns>
		public string DO_GetDescReg(string idReg)
		{
			string result = String.Empty;
			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_DATI_REGISTRO_BY_ID_REG");		
				queryMng.setParam("IDREG",idReg.ToString());
				string commandText=queryMng.getSQL();
				logger.Debug("Prospetti Riepilogativi - PR_DO_GET_DATI_REGISTRO_BY_ID_REG: "+commandText);
				DataSet ds = new DataSet();
				dbProvider.ExecuteQuery(ds,commandText);
				if (ds.Tables[0].Rows.Count != 0)
				{
					result = ds.Tables[0].Rows[0]["VAR_DESC_REGISTRO"].ToString();
				}
			}
			catch (Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_DATI_REGISTRO_BY_ID_REG: ",ex);
			}
			return result;
		}

		#endregion

        #region DO_GetTitolari
        public ArrayList DO_GetTitolari(int idAmm)
        {
            ArrayList result = new ArrayList();

            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_TITOLARI");
                queryMng.setParam("idAmm", idAmm.ToString());
                string commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - PR_DO_GET_TITOLARI: " + commandText);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, commandText);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        PR_Titolario prAmm = new PR_Titolario
                            (ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString(),
                            ds.Tables[0].Rows[i]["DESCRIPTION"].ToString(),
                            ds.Tables[0].Rows[i]["ET_TITOLARIO"].ToString());

                        result.Add(prAmm);
                    }
                }
                result.TrimToSize();


            }
            catch (Exception ex)
            {
                logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_TITOLARI: ", ex);
            }
            return result;
        }

        #endregion

        #region DO_GetNumDocSpediti
        public string DO_GetNumDocSpediti(string dataSpedDa, string dataSpedA, string idReg, string confermaProt, int idAmm)
        {
            string result = string.Empty;

            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
              
                string _objDBType = dbProvider.DBType.ToUpper().ToString();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_DOC_SPEDITI");
               
                string filters = string.Empty;
                queryMng.setParam("idAmm", idAmm.ToString());
                if (!string.IsNullOrEmpty(idReg))
                {
                    filters += " and profile.ID_REGISTRO=" + idReg;
                }
                else
                {
                    filters += " and profile.ID_REGISTRO is null";
                }
                switch (_objDBType)
                {
                    case "ORACLE":
                        if (dataSpedDa != string.Empty)
                        {
                            filters += " and (dpa_stato_invio.DTA_SPEDIZIONE >= to_date('" + dataSpedDa + " 00.00.00','DD/MM/YYYY HH24:mi:ss' ) and dpa_stato_invio.DTA_SPEDIZIONE <= to_date('" + dataSpedA + " 23:59:59','DD/MM/YYYY HH24:mi:ss' ))";
                        }
                        else
                        {
                            filters += " and dpa_stato_invio.DTA_SPEDIZIONE <= to_date('" + dataSpedA + " 23:59:59','DD/MM/YYYY HH24:mi:ss')";
                        }

                        if (confermaProt != "T")
                        {
                            if (confermaProt == "1")
                                filters += " and dpa_stato_invio.VAR_PROTO_DEST is not null";
                            else
                                filters += " and dpa_stato_invio.VAR_PROTO_DEST is null";
                        }
                        
                        break;
                    case "SQL":
                        if (dataSpedDa != string.Empty)
                        {
                            filters += " and (dpa_stato_invio.DTA_SPEDIZIONE >= convert(datetime, '" + dataSpedDa + " 00.00.00',103) and dpa_stato_invio.DTA_SPEDIZIONE <= convert(datetime, '" + dataSpedA + " 23:59:59',103))";
                        }
                        else
                        {
                            filters += " and dpa_stato_invio.DTA_SPEDIZIONE <= convert(datetime, '" + dataSpedA + " 23:59:59',103)";
                        }
                        if (confermaProt != "T")
                        {
                            if (confermaProt == "1")
                                filters += " and dpa_stato_invio.VAR_PROTO_DEST is not null";
                            else
                                filters += " and dpa_stato_invio.VAR_PROTO_DEST is null";
                        }
                        break;
                }
                queryMng.setParam("filters", filters);
                string commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - PR_DO_GET_DOC_SPEDITI: " + commandText);
                DataSet ds = new DataSet();
               // string field=string.Empty;
                //dbProvider.ExecuteScalar(out field, commandText);
                //result = field;
                dbProvider.ExecuteQuery(ds, commandText);
                //DataRow row = ds.Tables[0].Rows[0];
                result = ds.Tables[0].Rows[0][0].ToString();
                
                //string field;
                //System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText);

                //result = reader.GetValue(reader.GetOrdinal("tot_documenti")).ToString();
                
            }
            catch (Exception ex)
            {
                logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_DOC_SPEDITI: ", ex);
            }
           
            return result;
        }

        #endregion

        #region Metodi di recupero dataset dei prospetti

        #region DO_GetDSReportAnnualeByReg
        /// <summary>
		/// DO_GetDSReportAnnualeByReg: recupera il DS che dovrà essere 
		/// stampato nel report annuale per registro
		/// </summary>
		/// <param name="idReg">system_id del registro di interesse</param>
		/// <param name="anno">anno di interesse</param>
		/// <param name="mese">mese di interesse</param>
		/// <param name="simpleSP">se true viene chiamata quella sul singolo mese</param>
		/// <returns></returns>
        public DataSet DO_GetDSReportAnnualeByReg(int id_amm, int idReg, int anno, int mese, string sede, bool simpleSP, int titolario)
		{
			DataSet ds = new DataSet();
			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				string _objDBType = dbProvider.DBType.ToUpper().ToString();
				switch(_objDBType)
				{
					case "SQL":
						 
						ArrayList _parametri = new ArrayList();
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("id_amm",id_amm.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("id_registro",idReg.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("anno",anno.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("mese",mese.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("var_sede",sede));
                        _parametri.Add(new DocsPaUtils.Data.ParameterSP("id_titolario", titolario.ToString()));
						
						if(!simpleSP)
						{
							logger.Debug("Prospetti Riepilogativi - SP: REPORT_ANNUALE_REG ");
							dbProvider.ExecuteStoredProcedure("REPORT_ANNUALE_REG",_parametri,ds);
						}
						else
						{
							logger.Debug("Prospetti Riepilogativi - SP: REPORT_MENSILE_REG ");
							dbProvider.ExecuteStoredProcedure("REPORT_MENSILE_REG",_parametri,ds);				
						}
						break;
					case "ORACLE":
						if((!simpleSP) || (mese == 0))
						{
							if(mese==0) mese=12;
							DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_REPORT_ANNUALE_BY_REG");
							queryMng.setParam("IDREG",idReg.ToString());
							queryMng.setParam("IDAMM",id_amm.ToString());
							queryMng.setParam("ANNO",anno.ToString());
							queryMng.setParam("MESE",mese.ToString());
							queryMng.setParam("SEDE",sede.ToString());
                            queryMng.setParam("TIT", titolario.ToString());
							string commandText=queryMng.getSQL();
							logger.Debug("Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_BY_REG: "+commandText);
							dbProvider.ExecuteQuery(ds,commandText);
						}
						else
						{
							DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_REPORT_MENSILE_BY_REG");
							queryMng.setParam("IDREG",idReg.ToString());
							queryMng.setParam("IDAMM",id_amm.ToString());
							queryMng.setParam("ANNO",anno.ToString());
							queryMng.setParam("MESE",mese.ToString());
							queryMng.setParam("SEDE",sede.ToString());
                            queryMng.setParam("TIT", titolario.ToString());
							string commandText=queryMng.getSQL();
							logger.Debug("Prospetti Riepilogativi - PR_DO_GET_REPORT_MENSILE_BY_REG: "+commandText);
							dbProvider.ExecuteQuery(ds,commandText);
						}
						break;
				}
			}
			catch(Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_REPORT_BY_REG: ",ex);
			}
			return ds;
		}
		#endregion

		#region DO_GetDSReportDocClass
		/// <summary>
		/// DO_GetDSReportDocClass
		/// </summary>
		/// <param name="idReg">id del registro di interesse</param>
		/// <param name="anno">anno id interesse</param>
		/// <param name="idamm">id dell'amministrazione di interesse</param>
		/// <returns>DataSet con i dati</returns>
        public DataSet DO_GetDSReportDocClass(int idReg, int anno, int idamm, string sede, int titolario)
		{
			DataSet ds = new DataSet();

			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				string _objDBType = dbProvider.DBType.ToUpper().ToString();
				switch(_objDBType)
				{
					case "SQL":
						ArrayList _parametri = new ArrayList();
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("id_registro",idReg.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("id_anno",anno.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("id_amm",idamm.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("var_sede",sede));
                        _parametri.Add(new DocsPaUtils.Data.ParameterSP("id_titolario", titolario.ToString()));
						
						logger.Debug("Prospetti Riepilogativi - SP: REPORT_ANNUALE_DOC_CLASS");
						dbProvider.ExecuteStoredProcedure("REPORT_ANNUALE_DOC_CLASS",_parametri,ds);
						break;
					case "ORACLE":
                        //DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_REPORT_ANNUALE_DOC_CLASS");
                        DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_REPORT_ANNUALE_DOC_CLASS_NEW");
                        queryMng.setParam("IDREG", idReg.ToString());
                        queryMng.setParam("IDAMM", idamm.ToString());
                        queryMng.setParam("ANNO", anno.ToString());
                        if (sede == "") // caso tutte le sedi
                            queryMng.setParam("SEDE", "");
                        else
                            queryMng.setParam("SEDE", " and trim(mv.SEDE) ='" + sede.ToString() + "'");
                        if (titolario == 0) // caso tutti i titolari
                            queryMng.setParam("TIT", null);
                        else
                            queryMng.setParam("TIT", " and mv.TITOLARIO='" + titolario.ToString() + "'");
                        string commandText = queryMng.getSQL();
                        //logger.Debug("Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_DOC_CLASS: "+commandText);
                        logger.Debug("Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_DOC_CLASS_NEW: " + commandText);
                        dbProvider.ExecuteQuery(ds, commandText);
                        break;
				}
			}
			catch(Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_DOC_CLASS: ",ex);
			}
			return ds;
		} 
		#endregion

		#region DO_GetDSReportDocClassCompact
		/// <summary>
		/// DO_GetDSReportDocClassCompact:
		/// </summary>
		/// <param name="idReg">id del registro di interesse</param>
		/// <param name="anno">anno id interesse</param>
		/// <param name="idamm">id dell'amministrazione di interesse</param>
		/// <returns>DataSet con i dati</returns>
		/// <returns></returns>
        public DataSet DO_GetDSReportDocClassCompact(int idReg, int anno, int idamm, string sede, int titolario)
		{
			DataSet ds = new DataSet();
			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				string _objDBType = dbProvider.DBType.ToUpper().ToString();
				switch(_objDBType)
				{
					case "SQL":
						ArrayList _parametri = new ArrayList();
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("id_registro",idReg.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("id_anno",anno.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("id_amm",idamm.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("var_sede",sede));
                        _parametri.Add(new DocsPaUtils.Data.ParameterSP("id_titolario", titolario.ToString()));
						
						logger.Debug("Prospetti Riepilogativi - SP: REPORT_ANNUALE_DOC_CLASS_COMPATTA");
						dbProvider.ExecuteStoredProcedure("REPORT_ANNUALE_DOC_CLASS_COMPATTA",_parametri,ds);
						break;
					case "ORACLE":
                        //DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_REPORT_ANNUALE_DOC_CLASS_COMPATTA");
                        DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_REPORT_ANNUALE_DOC_CLASS_COMPATTA_NEW");
                        queryMng.setParam("IDAMM", idamm.ToString());
                        queryMng.setParam("IDREG", idReg.ToString());
                        queryMng.setParam("ANNO", anno.ToString());
                        if (sede == "") // caso tutte le sedi
                            queryMng.setParam("SEDE", "");
                        else
                            queryMng.setParam("SEDE", " and trim(mv.SEDE) ='" + sede.ToString() + "'");
                        if (titolario == 0) // caso tutti i titolari
                            queryMng.setParam("TIT", null);
                        else
                            queryMng.setParam("TIT", " and mv.TITOLARIO='" + titolario.ToString() + "'");
                        string commandText = queryMng.getSQL();
                        //logger.Debug("Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_DOC_CLASS_COMPATTA: "+commandText);
                        logger.Debug("Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_DOC_CLASS_COMPATTA_NEW: " + commandText);
                        dbProvider.ExecuteQuery(ds, commandText);
                        break;
				}
			}
			catch(Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_DOC_CLASS_COMPATTA: ",ex);
			}
			return ds;
		}

        public string DO_GetCountReportDocClassCompact(int idReg, int anno, int idamm, string sede, int titolario)
        {
            string result = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                string _objDBType = dbProvider.DBType.ToUpper().ToString();
                switch (_objDBType)
                {
                    // case SQL da implementare lato DB
                    case "SQL":
                        ArrayList _parametri = new ArrayList();
                        _parametri.Add(new DocsPaUtils.Data.ParameterSP("id_registro", idReg.ToString()));
                        _parametri.Add(new DocsPaUtils.Data.ParameterSP("id_anno", anno.ToString()));
                        _parametri.Add(new DocsPaUtils.Data.ParameterSP("id_amm", idamm.ToString()));
                        if (sede == "") // caso tutte le sedi
                            _parametri.Add(new DocsPaUtils.Data.ParameterSP("var_sede", ""));
                        else
                            _parametri.Add(new DocsPaUtils.Data.ParameterSP("var_sede", " and trim(mv.SEDE) ='" + sede.ToString() + "'"));

                        if (titolario == 0) // caso tutti i titolari
                            _parametri.Add(new DocsPaUtils.Data.ParameterSP("id_titolario", null));
                        else
                            _parametri.Add(new DocsPaUtils.Data.ParameterSP("id_titolario", " and mv.TITOLARIO='" + titolario.ToString() + "'"));

                        logger.Debug("Prospetti Riepilogativi - SP: REPORT_ANNUALE_DOC_CLASS_COMPATTA");
                        dbProvider.ExecuteStoredProcedure("REPORT_GET_COUNT_ANNUALE_DOC_CLASS_COMPATTA", _parametri, ds);

                        break;
                    case "ORACLE":
                        DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PR_GET_COUNT_REPORT_DOC_CLASS_COMPATTA_NEW");
                        queryMng.setParam("IDAMM", idamm.ToString());
                        queryMng.setParam("IDREG", idReg.ToString());
                        queryMng.setParam("ANNO", anno.ToString());
                        if (sede == "") // caso tutte le sedi
                            queryMng.setParam("SEDE", "");
                        else
                            queryMng.setParam("SEDE", " and trim(mv.SEDE) ='" + sede.ToString() + "'");
                        if (titolario == 0) // caso tutti i titolari
                            queryMng.setParam("TIT", null);
                        else
                            queryMng.setParam("TIT", " and mv.TITOLARIO='" + titolario.ToString() + "'");
                        string commandText = queryMng.getSQL();
                        logger.Debug("Prospetti Riepilogativi - PR_GET_COUNT_REPORT_DOC_CLASS_COMPATTA_NEW: " + commandText);
                        dbProvider.ExecuteQuery(ds, commandText);
                        if (ds.Tables[0].Rows.Count != 0)
                        {
                            result = ds.Tables[0].Rows[0][0].ToString();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore Prospetti Riepilogativi - PR_GET_COUNT_REPORT_DOC_CLASS_COMPATTA_NEW: ", ex);
            }
            return result;
        }

        public string DO_GetCountDistinctReportDocClassCompact(int idReg, int anno, int idamm, string sede, int titolario)
        {
            string result = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                string _objDBType = dbProvider.DBType.ToUpper().ToString();
                switch (_objDBType)
                {
                    // case SQL da implementare lato DB
                    case "SQL":
                        ArrayList _parametri = new ArrayList();
                        _parametri.Add(new DocsPaUtils.Data.ParameterSP("id_registro", idReg.ToString()));
                        _parametri.Add(new DocsPaUtils.Data.ParameterSP("id_anno", anno.ToString()));
                        _parametri.Add(new DocsPaUtils.Data.ParameterSP("id_amm", idamm.ToString()));
                        if (sede == "") // caso tutte le sedi
                            _parametri.Add(new DocsPaUtils.Data.ParameterSP("var_sede", ""));
                        else
                            _parametri.Add(new DocsPaUtils.Data.ParameterSP("var_sede", " and trim(mv.VAR_SEDE) ='" + sede.ToString() + "'"));

                        if (titolario == 0) // caso tutti i titolari
                            _parametri.Add(new DocsPaUtils.Data.ParameterSP("id_titolario", null));
                        else
                            _parametri.Add(new DocsPaUtils.Data.ParameterSP("id_titolario", " and mv.ID_TITOLARIO='" + titolario.ToString() + "'"));

                        logger.Debug("Prospetti Riepilogativi - SP: REPORT_ANNUALE_DOC_CLASS_COMPATTA");
                        dbProvider.ExecuteStoredProcedure("REPORT_GET_COUNT_DIST_ANNUALE_DOC_CLASS_COMPATTA", _parametri, ds);

                        break;
                    case "ORACLE":
                        DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PR_GET_COUNT_DIST_REPORT_DOC_CLASS_COMPATTA_NEW");
                        queryMng.setParam("IDAMM", idamm.ToString());
                        queryMng.setParam("IDREG", idReg.ToString());
                        string parAnno = DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("01/01/{0}", anno), true) + " AND " + DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("31/12/{0}", anno), false);
                        queryMng.setParam("DATA_STR", parAnno.ToString());
                        if (sede == "") // caso tutte le sedi
                            queryMng.setParam("SEDE", "");
                        else
                            queryMng.setParam("SEDE", " and trim(mv.VAR_SEDE) ='" + sede.ToString() + "'");
                        if (titolario == 0) // caso tutti i titolari
                            queryMng.setParam("TIT", null);
                        else
                            queryMng.setParam("TIT", " and mv.ID_TITOLARIO='" + titolario.ToString() + "'");
                        string commandText = queryMng.getSQL();
                        logger.Debug("Prospetti Riepilogativi - PR_GET_COUNT_DIST_REPORT_DOC_CLASS_COMPATTA_NEW: " + commandText);
                        dbProvider.ExecuteQuery(ds, commandText);
                        if (ds.Tables[0].Rows.Count != 0)
                        {
                            result = ds.Tables[0].Rows[0][0].ToString();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore Prospetti Riepilogativi - PR_GET_COUNT_REPORT_DOC_CLASS_COMPATTA_NEW: ", ex);
            }
            return result;
        }

		#endregion

		#region DO_GetDSReportDocTrasmToAOO
		/// <summary>
		/// DO_GetDSReportDocTrasmToAOO
		/// </summary>
		/// <param name="idReg">system_id del registro di interesse</param>
		/// <param name="anno">anno di interesse</param>
		/// <returns>DataSet con i dati</returns>
		public DataSet DO_GetDSReportDocTrasmToAOO(int idReg,int anno, int idAmm)
		{
			DataSet ds = new DataSet();
			string mese = "";
			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				string _objDBType = dbProvider.DBType.ToUpper().ToString();
                if (anno < DateTime.Now.Year)
                {
                    mese = "12";
                }
                else
                {
                    mese = DateTime.Now.Month.ToString();
                }
				ArrayList _parametri = new ArrayList();
				switch(_objDBType)
				{
					case "SQL":

						_parametri.Add (new DocsPaUtils.Data.ParameterSP("id_registro",idReg.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("anno",anno.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("mese",mese));

						logger.Debug("Prospetti Riepilogativi - SP: REPORT_ANNUALE_DOC_TRASM_INTEROP ");
						dbProvider.ExecuteStoredProcedure("REPORT_ANNUALE_DOC_TRASM_INTEROP",_parametri,ds);
						break;
					case "ORACLE":
						DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_REPORT_ANNUALE_DOC_TRASM_INTEROP");
						queryMng.setParam("ANNO",anno.ToString());
						queryMng.setParam("IDREG",idReg.ToString());
						queryMng.setParam("IDAMM",idAmm.ToString());
						string commandText=queryMng.getSQL();
						logger.Debug("Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_DOC_TRASM_INTEROP: "+commandText);
						dbProvider.ExecuteQuery(ds,commandText);
						break;
				}
			}
			catch(Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_DOC_TRASM_INTEROP: ",ex);
			}
			return ds;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idReg"></param>
        /// <param name="anno"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public string DO_GetCountReportDocTrasmToAOO(int idReg, int anno, int idAmm)
        {
            string result = string.Empty;
            DataSet dataSet = new DataSet();

            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                string _objDBType = dbProvider.DBType.ToUpper().ToString();
                switch (_objDBType)
                {
                    case "SQL":
                        // da implementare
                        break;
                    case "ORACLE":
                        DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_COUNT_REPORT_ANNUALE_DOC_TRASM_INTEROP");
                        queryMng.setParam("ANNO",anno.ToString());
						queryMng.setParam("REG",idReg.ToString());
						queryMng.setParam("IDAMM",idAmm.ToString());
						string commandText = queryMng.getSQL();
                        logger.Debug("Prospetti Riepilogativi - PR_DO_GET_COUNT_REPORT_ANNUALE_DOC_TRASM_INTEROP: " + commandText);
                        dbProvider.ExecuteQuery(dataSet, commandText);
                        result = dataSet.Tables[0].Rows[0][0].ToString() + "§" + dataSet.Tables[0].Rows[1][0].ToString() + "§" + dataSet.Tables[0].Rows[2][0].ToString();
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_COUNT_REPORT_ANNUALE_DOC_TRASM_INTEROP: ", ex);
            }

            return result;
        }

		#endregion

		#region DO_GetDSReportFascicoliPerVT
		/// <summary>
		/// DO_GetDSReportFascicoliPerVT
		/// </summary>
		/// <param name="idAmm">system_id dell'amministrazione di interesse</param>
		/// <param name="idReg">system_id del registro di interesse</param>
		/// <param name="anno">anno di interesse</param>
		/// <param name="mese">mese di interesse</param>
		/// <returns>DataSet con i dati</returns>
        public DataSet DO_GetDSReportFascicoliPerVT(int idAmm, int idReg, int anno, int mese, int titolario)
		{
			DataSet ds = new DataSet();
			
			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				string _objDBType = dbProvider.DBType.ToUpper().ToString();
				switch(_objDBType)
				{
					case "SQL":
						ArrayList _parametri = new ArrayList();
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ID_AMM",idAmm.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ID_REGISTRO",idReg.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ANNO",anno.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("MESE",mese.ToString()));
                        _parametri.Add(new DocsPaUtils.Data.ParameterSP("TIT", titolario.ToString()));

						logger.Debug("Prospetti Riepilogativi - SP: PR_DO_GET_REPORT_ANNUALE_FASC_VT ");
						dbProvider.ExecuteStoredProcedure("REPORT_ANNUALE_FASC_VT",_parametri,ds);
						break;
					case "ORACLE":
						DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_REPORT_ANNUALE_FASC_VT");
						queryMng.setParam("IDAMM",idAmm.ToString());
						queryMng.setParam("IDREG",idReg.ToString());
						queryMng.setParam("ANNO",anno.ToString());
						queryMng.setParam("MESE",mese.ToString());
                        queryMng.setParam("TIT", titolario.ToString());
						string commandText=queryMng.getSQL();
						logger.Debug("Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_FASC_VT: "+commandText);
						dbProvider.ExecuteQuery(ds,commandText);
						break;
				}
			}
			catch(Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_FASC_VT: ",ex);
			}
			return ds;
		}
		#endregion

		#region DO_GetDSReportFascicoliPerVTCompact
		/// <summary>
		/// DO_GetDSReportFascicoliPerVTCompact
		/// </summary>
		/// <param name="idAmm">system_id dell'amministrazione di interesse</param>
		/// <param name="idReg">system_id del registro di interesse</param>
		/// <param name="anno">anno di interesse</param>
		/// <param name="mese">mese di interesse</param>
		/// <returns>DataSet con i dati</returns>
        public DataSet DO_GetDSReportFascicoliPerVTCompact(int idAmm, int idReg, int anno, int mese, int titolario)
		{
			DataSet ds = new DataSet();

			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				string _objDBType = dbProvider.DBType.ToUpper().ToString();
				switch(_objDBType)
				{
					case "SQL":
						ArrayList _parametri = new ArrayList();
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ID_AMM",idAmm.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ID_REGISTRO",idReg.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ANNO",anno.ToString()));
                        _parametri.Add (new DocsPaUtils.Data.ParameterSP("MESE",mese.ToString()));
                        _parametri.Add (new DocsPaUtils.Data.ParameterSP("TIT",titolario.ToString()));

						logger.Debug("Prospetti Riepilogativi - SP: REPORT_ANNUALE_FASC_VT_COMPATTA");
						dbProvider.ExecuteStoredProcedure("REPORT_ANNUALE_FASC_VT_COMPATTA",_parametri,ds);
						break;
					case "ORACLE":
						DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_REPORT_ANNUALE_FASC_VT_COMPATTA");
						queryMng.setParam("IDAMM",idAmm.ToString());
						queryMng.setParam("IDREG",idReg.ToString());
						queryMng.setParam("ANNO",anno.ToString());
						queryMng.setParam("MESE",mese.ToString());
                        queryMng.setParam("TIT", titolario.ToString());
						string commandText=queryMng.getSQL();
						logger.Debug("Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_FASC_VT_COMPATTA: "+commandText);
						dbProvider.ExecuteQuery(ds,commandText);
						break;
				}
			}
			catch(Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_FASC_VT_COMPATTA: ",ex);
			}
			return ds;
		}
		#endregion
		
		#region DO_GetDSReportAnnualeByFasc
		/// <summary>
		/// DO_GetDSReportAnnualeByFasc
		/// </summary>
		/// <param name="idAmm">system_id dell'amministrazione di interesse</param>
		/// <param name="idReg">system_id del registro di interesse</param>
		/// <param name="anno">anno di interesse</param>
		/// <param name="mese">mese di interesse</param>
		/// <returns>DataSet con i dati</returns>
		public DataSet DO_GetDSReportAnnualeByFasc(int idAmm,int idReg,int anno, int mese, int titolario, bool simpleSP)
		{
			DataSet ds = new DataSet();

			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				string _objDBType = dbProvider.DBType.ToUpper().ToString();
				switch(_objDBType)
				{
					case "SQL":
						ArrayList _parametri = new ArrayList();
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ID_AMM",idAmm.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ID_REGISTRO",idReg.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ANNO",anno.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("MESE",mese.ToString()));
                        _parametri.Add (new DocsPaUtils.Data.ParameterSP("TITOLARIO", titolario.ToString()));

						if(!simpleSP)
						{
							logger.Debug("Prospetti Riepilogativi - SP: REPORT_ANNUALE_FASCICOLI");
							dbProvider.ExecuteStoredProcedure("REPORT_ANNUALE_FASCICOLI",_parametri,ds);
						}
						else
						{
							logger.Debug("Prospetti Riepilogativi - SP: REPORT_MENSILE_FASCICOLI");
							dbProvider.ExecuteStoredProcedure("REPORT_MENSILE_FASCICOLI",_parametri,ds);
						}
						break;
					case "ORACLE":
						if(!simpleSP || (mese == 0))
						{
							if(mese==0) mese=12;
							DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_REPORT_ANNUALE_FASCICOLI");
							queryMng.setParam("IDAMM",idAmm.ToString());
							queryMng.setParam("IDREG",idReg.ToString());
							queryMng.setParam("ANNO",anno.ToString());
							queryMng.setParam("MESE",mese.ToString());
                            queryMng.setParam("TITOLARIO", titolario.ToString());
                            string commandText = queryMng.getSQL();
							logger.Debug("Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_FASCICOLI: "+commandText);
							dbProvider.ExecuteQuery(ds,commandText);
						}
						else
						{
							DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_REPORT_MENSILE_FASCICOLI");
							queryMng.setParam("IDAMM",idAmm.ToString());
							queryMng.setParam("IDREG",idReg.ToString());
							queryMng.setParam("ANNO",anno.ToString());
							queryMng.setParam("MESE",mese.ToString());
                            queryMng.setParam("TITOLARIO", titolario.ToString());
                            string commandText = queryMng.getSQL();
							logger.Debug("Prospetti Riepilogativi - PR_DO_GET_REPORT_MENSILE_FASCICOLI: "+commandText);
							dbProvider.ExecuteQuery(ds,commandText);
						}
						break;
				}
			}
			catch(Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_REPORT_FASCICOLI: ",ex);
			}
			return ds;
		}
		#endregion

		#region DO_GetDSTempiMediLavorazione
		/// <summary>
		/// DO_GetDSTempiMediLavorazione
		/// </summary>
		/// <param name="idAmm">system_id dell'amministrazione di interesse</param>
		/// <param name="idReg">system_id del registro di interesse</param>
		/// <param name="anno">anno di interesse</param>
		/// <param name="mese">mese di interesse</param>
		/// <returns>DataSet con i dati</returns>
		public DataSet DO_GetDSTempiMediLavorazione(int idAmm,int idReg,int anno, int mese, int titolario)
		{
			DataSet ds = new DataSet();
			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				string _objDBType = dbProvider.DBType.ToUpper().ToString();
				switch(_objDBType)
				{
					case "SQL":
						ArrayList _parametri = new ArrayList();
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ID_AMM",idAmm.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ID_REGISTRO",idReg.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ANNO",anno.ToString()));
                        _parametri.Add (new DocsPaUtils.Data.ParameterSP("TITOLARIO", titolario.ToString()));

						logger.Debug("Prospetti Riepilogativi - SP: REPORT_TEMPO_MEDIO_LAV");
						dbProvider.ExecuteStoredProcedure("REPORT_TEMPO_MEDIO_LAV",_parametri,ds);
						break;
					case "ORACLE":
						DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_REPORT_TEMPO_MEDIO_LAV");
						queryMng.setParam("IDAMM",idAmm.ToString());
						queryMng.setParam("IDREG",idReg.ToString());
						queryMng.setParam("ANNO",anno.ToString());
						queryMng.setParam("TITOLARIO",titolario.ToString());
						string commandText=queryMng.getSQL();
						logger.Debug("Prospetti Riepilogativi - PR_DO_GET_REPORT_TEMPO_MEDIO_LAV: "+commandText);
						dbProvider.ExecuteQuery(ds,commandText);
						break;
				}
			}
			catch(Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_REPORT_TEMPO_MEDIO_LAV: ",ex);
			}
			return ds;
		}
		#endregion 

		#region DO_GetDSTempiMediLavorazioneCompact
		/// <summary>
		/// DO_GetDSTempiMediLavorazioneCompact
		/// </summary>
		/// <param name="idAmm">system_id dell'amministrazione di interesse</param>
		/// <param name="idReg">system_id del registro di interesse</param>
		/// <param name="anno">anno di interesse</param>
		/// <param name="mese">mese di interesse</param>
		/// <returns>DataSet con i dati</returns>
		public DataSet DO_GetDSTempiMediLavorazioneCompact(int idAmm,int idReg,int anno, int mese, int titolario)
		{
			DataSet ds = new DataSet();
			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				string _objDBType = dbProvider.DBType.ToUpper().ToString();
				switch(_objDBType)
				{
					case "SQL":
						ArrayList _parametri = new ArrayList();
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ID_AMM",idAmm.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ID_REGISTRO",idReg.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ANNO",anno.ToString()));
                        _parametri.Add (new DocsPaUtils.Data.ParameterSP("TITOLARIO", titolario.ToString()));

						logger.Debug("Prospetti Riepilogativi - SP: REPORT_TEMPO_MEDIO_LAV_COMPACT");
						dbProvider.ExecuteStoredProcedure("REPORT_TEMPO_MEDIO_LAV_COMPACT",_parametri,ds);
						break;
					case "ORACLE":
                        DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_REPORT_TEMPO_MEDIO_LAV_COMPACT");
                        queryMng.setParam("IDAMM", idAmm.ToString());
                        queryMng.setParam("IDREG", idReg.ToString());
                        queryMng.setParam("ANNO", anno.ToString());
                        queryMng.setParam("TITOLARIO", titolario.ToString());
                        string commandText = queryMng.getSQL();
                        logger.Debug("Prospetti Riepilogativi - PR_DO_GET_REPORT_TEMPO_MEDIO_LAV_COMPACT: " + commandText);
                        dbProvider.ExecuteQuery(ds, commandText);

						break;
				}
			}
			catch(Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_REPORT_TEMPO_MEDIO_LAV_COMPACT: ",ex);
			}
			return ds;
		}

		#endregion

		#region DO_GetDSReportDocXSede
		///<summary>
		///DO_GetDSReportDocXSede
		///</summary>
		/// <param name="idAmm">system_id dell'amministrazione di interesse</param>
		/// <param name="idReg">system_id del registro di interesse</param>
		/// <param name="anno">anno di interesse</param>
		/// <returns>DataSet con i dati</returns>
		
		public DataSet DO_GetDSReportDocXSede(int idAmm, int idReg, int anno, int idPeople, string timeStamp)
		{
			DataSet ds = new DataSet();
			try
			{
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				string _objDBType = dbProvider.DBType.ToUpper().ToString();
				
				switch(_objDBType)
				{
					case "SQL":
						ArrayList _parametri = new ArrayList();
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ID_AMM",idAmm));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ID_REGISTRO",idReg));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ANNO",anno));

						logger.Debug("Prospetti Riepilogativi - SP: REPORT_ANNUALE_DOC_X_SEDE");
						dbProvider.ExecuteStoredProcedure("REPORT_ANNUALE_DOC_X_SEDE",_parametri,ds);
						break;
					case "ORACLE":
						string commandText = String.Empty;
						//DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PR_EXEC_REPORT_ANNUALE_DOC_X_SEDE");
						//queryMng.setParam("IDAMM",idAmm.ToString());
						//queryMng.setParam("IDREG",idReg.ToString());
						//queryMng.setParam("ANNO",anno.ToString());
						//queryMng.setParam("IDPEOPLE",idPeople.ToString());
						//queryMng.setParam("TIMESTAMP",timeStamp);

						//commandText=queryMng.getSQL();
						//logger.Debug("Prospetti Riepilogativi - PR_EXEC_REPORT_ANNUALE_DOC_X_SEDE: "+commandText);
						//dbProvider.ExecuteNonQuery(commandText);
				
						//prelevo i dati appena inseriti
                        DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_GET_REPORT_ANNUALE_DOC_X_SEDE");
						queryMng.setParam("IDPEOPLE",idPeople.ToString());
						queryMng.setParam("TIMESTAMP",timeStamp);
                        queryMng.setParam("ANNO", anno.ToString());
						commandText=queryMng.getSQL();
						logger.Debug("Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_DOC_X_SEDE: "+commandText);
						dbProvider.ExecuteQuery(ds,commandText);
						break;
				}
			}
			catch(Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_DOC_X_SEDE: ",ex);
			}
			return ds;
		}
		#endregion

		#region DO_GetDSReportDocXUo
		///<summary>
		///DO_GetDSReportDocXUo
		///</summary>
		/// <param name="idAmm">system_id dell'amministrazione di interesse</param>
		/// <param name="idReg">system_id del registro di interesse</param>
		/// <param name="anno">anno di interesse</param>
		/// <returns>DataSet con i dati</returns>
		
		public DataSet DO_GetDSReportDocXUo(int idAmm, int idReg, int anno)
		{
			DataSet ds = new DataSet();

			try
			{ 
				DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
				string _objDBType = dbProvider.DBType.ToUpper().ToString();
				switch(_objDBType)
				{
					case "SQL":
						ArrayList _parametri = new ArrayList();
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ID_AMM",idAmm.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ID_REGISTRO",idReg.ToString()));
						_parametri.Add (new DocsPaUtils.Data.ParameterSP("ANNO",anno.ToString()));
				
						logger.Debug("Prospetti Riepilogativi - SP: REPORT_ANNUALE_DOC_X_UO ");
						dbProvider.ExecuteStoredProcedure("REPORT_ANNUALE_DOC_X_UO",_parametri,ds);
						break;
					case "ORACLE":
						DocsPaUtils.Query queryMng=DocsPaUtils.InitQuery.getInstance().getQuery("PR_DO_REPORT_ANNUALE_DOC_X_UO");
						queryMng.setParam("ANNO",anno.ToString());
						queryMng.setParam("IDREG",idReg.ToString());
						queryMng.setParam("IDAMM",idAmm.ToString());
						string commandText=queryMng.getSQL();
						logger.Debug("Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_DOC_X_UO: "+commandText);
						dbProvider.ExecuteQuery(ds,commandText);
						break;
				}
			}
			catch(Exception ex)
			{
				logger.Debug("Errore Prospetti Riepilogativi - PR_DO_GET_REPORT_ANNUALE_DOC_X_UO: ",ex);
			}
			return ds;
		}
		#endregion

        //VERONICA per piani di rientro
        #region DO_GetDSReportPianiRientro
        /// <summary>
        /// DO_GetDSReportPianiRientro
        /// </summary>
        /// <param name="idamm">id dell'amministrazione di interesse</param>
        /// <param name="idReg">id del registro di interesse</param>
        /// <param name="dataInizio">data inizio periodo</param>
        /// <param name="dataFine">data fine periodo</param>
        /// <returns>DataSet con i dati</returns>
        public DataSet DO_GetDSReportPianiRientroConScadenza(int idReg, string dataInizio, string dataFine, out int res)
        {
            DataSet ds = new DataSet();
            res = 0;
            try
            {
                string commandText = "";
                //  ATTENZIONE RIEMPO IL DATASET QUI
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                // query per provvedimenti in istruttoria 
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_PIANI_RIENTRO_ISTRUTTORIA");
                queryMng.setParam("param1", idReg.ToString());
                queryMng.setParam("param2", dataInizio);
                queryMng.setParam("param3", dataFine);
                //commandText = queryMng.getSQL() + "union";
                commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_PIANI_RIENTRO_ISTRUTTORIA: " + commandText);
                dbProvider.ExecuteQuery(ds, "ISTRUTTORIA", commandText);

                //query per provvedimenti trasmessi al MEF
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_PIANI_RIENTRO_TRASMESSI_MEF");
                queryMng.setParam("param1", idReg.ToString());
                queryMng.setParam("param2", dataInizio);
                queryMng.setParam("param3", dataFine);
                queryMng.setParam("param4", "<");
                //commandText += queryMng.getSQL() + "union";
                commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_PIANI_RIENTRO_TRASMESSI_MEF: " + commandText);
                dbProvider.ExecuteQuery(ds, "TRASMESSI_MEF", commandText);

                //query per provvedimenti ricevuti dal MEF
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_PIANI_RIENTRO_RICEVUTI_MEF");
                queryMng.setParam("param1", idReg.ToString());
                queryMng.setParam("param2", dataInizio);
                queryMng.setParam("param3", dataFine);
                queryMng.setParam("param4", "<");
                //commandText += queryMng.getSQL() + "union";
                commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_PIANI_RIENTRO_RICEVUTI_MEF: " + commandText);
                dbProvider.ExecuteQuery(ds, "RICEVUTI_MEF", commandText);

                //query per provvedimenti trasmessi alla regione
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_PIANI_RIENTRO_TRASMESSI_REGIONE");
                queryMng.setParam("param1", idReg.ToString());
                queryMng.setParam("param2", dataInizio);
                queryMng.setParam("param3", dataFine);
                commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_PIANI_RIENTRO_TRASMESSI_REGIONE: " + commandText);
                dbProvider.ExecuteQuery(ds, "TRASMESSI_REGIONE", commandText);
                dbProvider.ExecuteQuery(ds, commandText);
            }
            catch (Exception ex)
            {
                res = -1;
                logger.Debug("Errore Prospetti Riepilogativi - S_PIANI_RIENTRO: ", ex);
            }
            return ds;
        }

        public DataSet DO_GetDSReportPianiRientroSenzaScadenza(int idReg, string dataInizio, string dataFine, out int res)
        {
            DataSet ds = new DataSet();
            res = 0;
            try
            {
                string commandText = "";
                //  ATTENZIONE RIEMPO IL DATASET QUI
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                // query per provvedimenti in istruttoria 
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_PIANI_RIENTRO_ISTRUTTORIA_SC");
                queryMng.setParam("param1", idReg.ToString());
                queryMng.setParam("param2", dataInizio);
                queryMng.setParam("param3", dataFine);
                commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_PIANI_RIENTRO_ISTRUTTORIA_SC: " + commandText);
                dbProvider.ExecuteQuery(ds, "ISTRUTTORIA", commandText);

                //query per provvedimenti trasmessi al MEF
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_PIANI_RIENTRO_TRASMESSI_MEF_SC");
                queryMng.setParam("param1", idReg.ToString());
                queryMng.setParam("param2", dataInizio);
                queryMng.setParam("param3", dataFine);
                commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_PIANI_RIENTRO_TRASMESSI_MEF_SC: " + commandText);
                dbProvider.ExecuteQuery(ds, "TRASMESSI_MEF", commandText);

                //query per provvedimenti ricevuti dal MEF
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_PIANI_RIENTRO_RICEVUTI_MEF_SC");
                queryMng.setParam("param1", idReg.ToString());
                queryMng.setParam("param2", dataInizio);
                queryMng.setParam("param3", dataFine);
                queryMng.setParam("param4", "<");
                commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_PIANI_RIENTRO_RICEVUTI_MEF_SC: " + commandText);
                dbProvider.ExecuteQuery(ds, "RICEVUTI_MEF", commandText);

                //query per provvedimenti trasmessi alla regione
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_PIANI_RIENTRO_TRASMESSI_REGIONE_SC");
                queryMng.setParam("param1", idReg.ToString());
                queryMng.setParam("param2", dataInizio);
                queryMng.setParam("param3", dataFine);
                commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_PIANI_RIENTRO_TRASMESSI_REGIONE_SC: " + commandText);
                dbProvider.ExecuteQuery(ds, "TRASMESSI_REGIONE", commandText);
            }
            catch (Exception ex)
            {
                res = -1;
                logger.Debug("Errore Prospetti Riepilogativi - S_PIANI_RIENTRO: ", ex);
            }
            return ds;
        }

        public DataSet DO_GetDSReportPianiRientroFuoriPiano(int idReg, string dataInizio, string dataFine, out int res)
        {
            DataSet ds = new DataSet();
            res = 0;
            try
            {
                string commandText = "";
                //  ATTENZIONE RIEMPO IL DATASET QUI
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                // query per provvedimenti in istruttoria 
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_PIANI_RIENTRO_ISTRUTTORIA_FP");
                queryMng.setParam("param1", idReg.ToString());
                queryMng.setParam("param2", dataInizio);
                queryMng.setParam("param3", dataFine);
                commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_PIANI_RIENTRO_ISTRUTTORIA_FP: " + commandText);
                dbProvider.ExecuteQuery(ds, "ISTRUTTORIA", commandText);

                //query per provvedimenti trasmessi al MEF
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_PIANI_RIENTRO_TRASMESSI_MEF_FP");
                queryMng.setParam("param1", idReg.ToString());
                queryMng.setParam("param2", dataInizio);
                queryMng.setParam("param3", dataFine);
                commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_PIANI_RIENTRO_TRASMESSI_MEF_FP: " + commandText);
                dbProvider.ExecuteQuery(ds, "TRASMESSI_MEF", commandText);

                //query per provvedimenti ricevuti dal MEF
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_PIANI_RIENTRO_RICEVUTI_MEF_FP");
                queryMng.setParam("param1", idReg.ToString());
                queryMng.setParam("param2", dataInizio);
                queryMng.setParam("param3", dataFine);
                queryMng.setParam("param4", "<");
                commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_PIANI_RIENTRO_RICEVUTI_MEF_FP: " + commandText);
                dbProvider.ExecuteQuery(ds, "RICEVUTI_MEF", commandText);

                //query per provvedimenti trasmessi alla regione
                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_PIANI_RIENTRO_TRASMESSI_REGIONE_FP");
                queryMng.setParam("param1", idReg.ToString());
                queryMng.setParam("param2", dataInizio);
                queryMng.setParam("param3", dataFine);
                commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_PIANI_RIENTRO_TRASMESSI_REGIONE_FP: " + commandText);
                dbProvider.ExecuteQuery(ds, "TRASMESSI_REGIONE", commandText);
            }
            catch (Exception ex)
            {
                res = -1;
                logger.Debug("Errore Prospetti Riepilogativi - S_PIANI_RIENTRO: ", ex);
            }
            return ds;
        }

        #endregion

        #region DO_GetDSReportContatoriDocumento
        public DataSet DO_GetDSReportContatoriDocumento(int idAmm, int anno)
        {
            DataSet ds = new DataSet();
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_CONTATORI_DOCUMENTO");
                queryMng.setParam("idAmm", idAmm.ToString());
                queryMng.setParam("anno", anno.ToString());
                string commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_REPORT_CONTATORI_DOCUMENTO: " + commandText);
                dbProvider.ExecuteQuery(ds, commandText);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore Prospetti Riepilogativi - S_REPORT_CONTATORI_DOCUMENTO: ", ex);
            }
            return ds;
        }
        #endregion

        #region DO_GetDSReportContatoriFascicolo
        public DataSet DO_GetDSReportContatoriFascicolo(int idAmm, int anno)
        {
            DataSet ds = new DataSet();
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_CONTATORI_FASCICOLO");
                queryMng.setParam("idAmm", idAmm.ToString());
                queryMng.setParam("anno", anno.ToString());
                string commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_REPORT_CONTATORI_FASCICOLO: " + commandText);
                dbProvider.ExecuteQuery(ds, commandText);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore Prospetti Riepilogativi - S_REPORT_CONTATORI_FASCICOLO: ", ex);
            }
            return ds;
        }
        #endregion

        #region DO_GetDSReportProtocolloArma
        public DataSet DO_GetDSReportProtocolloArma(string idReg, int idTitolario, int idAmm)
        {
            DataSet ds = new DataSet();
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_PROTOCOLLO_ARMA");
                if (!string.IsNullOrEmpty(idReg))
                {
                    queryMng.setParam("idReg", " ID_REGISTRO=" + idReg);
                }
                else
                {
                    queryMng.setParam("idReg", " ID_REGISTRO is null");
                }
                queryMng.setParam("paramDate", DocsPaDbManagement.Functions.Functions.ToChar("project.dta_apertura", false));
                queryMng.setParam("idTitolario", idTitolario.ToString());
                queryMng.setParam("idAmm", idAmm.ToString());
                string commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_REPORT_PROTOCOLLO_ARMA: " + commandText);
                dbProvider.ExecuteQuery(ds, commandText);

            }
            catch (Exception ex)
            {
                logger.Debug("Errore Prospetti Riepilogativi - S_REPORT_PROTOCOLLO_ARMA: ", ex);
            }
            return ds;
        }
        #endregion

        #region DO_GetDSReportDettaglioPratica
        public DataSet DO_GetDSReportDettaglioPratica(string idReg, int idTitolario, int numPratica, int idAmm)
        {
            DataSet ds = new DataSet();
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_DETTAGLIO_PRATICA");
                if (!string.IsNullOrEmpty(idReg))
                {
                    queryMng.setParam("idReg", " ID_REGISTRO=" + idReg);
                }
                else
                {
                    queryMng.setParam("idReg", " ID_REGISTRO is null");
                }
                queryMng.setParam("paramDate", DocsPaDbManagement.Functions.Functions.ToChar("p.DTA_CREAZIONE", false));
                queryMng.setParam("idTitolario", idTitolario.ToString());
                queryMng.setParam("numPratica", numPratica.ToString());
                queryMng.setParam("idAmm", idAmm.ToString());
                string commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_REPORT_DETTAGLIO_PRATICA: " + commandText);
                dbProvider.ExecuteQuery(ds, commandText);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore Prospetti Riepilogativi - S_REPORT_DETTAGLIO_PRATICA: ", ex);
            }
            return ds;
        }
        #endregion

        #region DO_GetDSReportGiornaleRegistri
        public DataSet DO_GetDSReportGiornaleRegistri(string idReg, int idAmm)
        {
            DataSet ds = new DataSet();
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_GIORNALE_RISCONTRI");
                queryMng.setParam("idAmm", idAmm.ToString());
                if (!string.IsNullOrEmpty(idReg))
                {
                    queryMng.setParam("idReg", " dpa_riscontri_classifica.ID_REG_DEST=" + idReg);
                }
                else
                {
                    queryMng.setParam("idReg", " dpa_riscontri_classifica.ID_REG_DEST is null");
                }
                string commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_REPORT_GIORNALE_RISCONTRI: " + commandText);
                dbProvider.ExecuteQuery(ds, commandText);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string classificaMitt = ds.Tables[0].Rows[i]["CLASSIFICA_MITTENTE"].ToString();
                        if (classificaMitt.Contains("$"))
                        {
                            classificaMitt = classificaMitt.Substring(classificaMitt.IndexOf("$") + 1, classificaMitt.Length - (classificaMitt.IndexOf("$") + 1));
                            ds.Tables[0].Rows[i]["CLASSIFICA_MITTENTE"] = classificaMitt;
                        }                       
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore Prospetti Riepilogativi - S_REPORT_GIORNALE_RISCONTRI: ", ex);
            }
            return ds;
        }
        #endregion

        #region DO_GetDSReportDocSpeditiInterop
        public DataSet DO_GetDSReportDocSpeditiInterop(int idAmm, string idRegistro, string dataSpedDa, string dataSpedA, string confermaProt)
        {
            DataSet ds = new DataSet();
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                string _objDBType = dbProvider.DBType.ToUpper().ToString();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_REPORT_DOC_SPEDITI_INTEROP");
                string filter = string.Empty;
                queryMng.setParam("idAmm", idAmm.ToString());
                if (!string.IsNullOrEmpty(idRegistro))
                {
                    filter += " and profile.ID_REGISTRO=" + idRegistro;                    
                }
                else
                {
                    filter += " and profile.ID_REGISTRO is null";                   
                }
                switch (_objDBType)
                {
                    case "ORACLE":
                        if (dataSpedDa != string.Empty)
                        {
                            filter += " and (dpa_stato_invio.DTA_SPEDIZIONE >= to_date('" + dataSpedDa + " 00.00.00','DD/MM/YYYY HH24:mi:ss' ) and dpa_stato_invio.DTA_SPEDIZIONE<=to_date('" + dataSpedA + " 23:59:59','DD/MM/YYYY HH24:mi:ss' ))";
                        }
                        else
                        {
                            filter += " and dpa_stato_invio.DTA_SPEDIZIONE<=to_date('" + dataSpedA + " 23:59:59','DD/MM/YYYY HH24:mi:ss')";
                        }

                        if (confermaProt != "T")
                        {
                            if (confermaProt == "1")
                                filter += " and dpa_stato_invio.VAR_PROTO_DEST is not null";
                            else
                                filter += " and dpa_stato_invio.VAR_PROTO_DEST is null";
                        }

                        break;
                    case "SQL":
                        if (dataSpedDa != string.Empty)
                        {
                            filter += " and (dpa_stato_invio.DTA_SPEDIZIONE >= convert(datetime, '" + dataSpedDa + " 00.00.00',103) and dpa_stato_invio.DTA_SPEDIZIONE<=convert(datetime, '" + dataSpedA + " 23:59:59',103))";
                        }
                        else
                        {
                            filter += " and dpa_stato_invio.DTA_SPEDIZIONE<=convert(datetime, '" + dataSpedA + " 23:59:59',103)";
                        }
                        if (confermaProt != "T")
                        {
                            if (confermaProt == "1")
                                filter += " and dpa_stato_invio.VAR_PROTO_DEST is not null";
                            else
                                filter += " and dpa_stato_invio.VAR_PROTO_DEST is null";
                        }
                        break;
                }
                queryMng.setParam("filters", filter);
                string commandText = queryMng.getSQL();
                logger.Debug("Prospetti Riepilogativi - S_REPORT_DOC_SPEDITI_INTEROP: " + commandText);
                dbProvider.ExecuteQuery(ds, commandText);                
            }
            catch (Exception ex)
            {
                logger.Debug("Errore Prospetti Riepilogativi - S_REPORT_DOC_SPEDITI_INTEROP: ", ex);
            }
            return ds;
        }
        #endregion

        #region DO_StampaReportFascExcel
        /// <summary>
        /// DO_StampaReportFascExcel:
        /// </summary>
        public DataSet DO_StampaReportFascExcel(string timeStamp, string searchType, string idAmm, string tipo_ricerca, string tipo_scelta, string valore_scelta, bool sottoposti, string dataC, string dataCdal, string dataCal, string dataCh, string dataChdal, string dataChal, string idTitolario, string[] ruoliUoSottoposti)
        {
            DataSet ds = new DataSet();
            string queryExec = "";
            DocsPaUtils.Query q;
            using (DBProvider dbProvider = new DBProvider())
            {
                string _objDBType = dbProvider.DBType.ToUpper().ToString();

                if (tipo_ricerca.Equals("reportNumFasc"))
                {
                        //ESPORTA I NUMERI DI FASCIOLI APERTI E CHIUSI
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_EXP_COUNT_FASC");
                    if (tipo_scelta.Equals("uo"))
                    {
                        queryExec += " project p, dpa_corr_globali d where p.ID_AMM = " + idAmm + " AND p.CHA_TIPO_FASCICOLO ='P' AND p.ID_TITOLARIO in (" + idTitolario + ")";
                        if (sottoposti == true && ruoliUoSottoposti.Length>0)
                        {
                            //CERCA IN UO E NELLE SOTTOPOSTE
                            string ruoliSott=" IN ( ";
                            for(int i=0; i<ruoliUoSottoposti.Length; i++)
                            {
                                ruoliSott += " "+ruoliUoSottoposti[i]+" ";
                                if (i < ruoliUoSottoposti.Length - 1)
                                {
                                    ruoliSott += ", ";
                                }
                            }
                            ruoliSott += " )";
                            queryExec += " AND ((d.ID_UO = p.ID_UO_CREATORE AND p.ID_RUOLO_CREATORE = d.SYSTEM_ID  AND p.ID_UO_CREATORE = " + valore_scelta + " ) OR (d.ID_UO = p.ID_RUOLO_CHIUSURA AND p.ID_RUOLO_CHIUSURA = d.SYSTEM_ID AND p.ID_UO_CHIUSURA = " + valore_scelta + ") OR (p.ID_RUOLO_CREATORE " + ruoliSott + " AND p.id_ruolo_creatore = d.system_id) OR (p.ID_RUOLO_CHIUSURA " + ruoliSott + " AND p.id_ruolo_chiusura = d.system_id)) "; 
                            
                        }
                        else
                        {
                            //CERCA SOLO NELLA UO
                            queryExec += " AND ((d.ID_UO = p.ID_UO_CREATORE AND p.ID_RUOLO_CREATORE = d.SYSTEM_ID  AND p.ID_UO_CREATORE = '" + valore_scelta + "' ) OR (d.ID_UO = p.ID_RUOLO_CHIUSURA AND p.ID_RUOLO_CHIUSURA = d.SYSTEM_ID AND p.ID_UO_CHIUSURA = '" + valore_scelta + "')) "; 
                        }
                    }
                    else
                    {
                        queryExec += " project p, dpa_l_ruolo_reg g, dpa_corr_globali d where p.ID_AMM = " + idAmm + " AND p.CHA_TIPO_FASCICOLO ='P' AND p.ID_TITOLARIO IN (" + idTitolario + ") AND ((p.ID_RUOLO_CREATORE = g.ID_RUOLO_IN_UO) OR (p.ID_RUOLO_CHIUSURA = g.ID_RUOLO_IN_UO)) AND g.ID_REGISTRO = " + valore_scelta + " AND d.SYSTEM_ID = p.ID_RUOLO_CREATORE"; 
                    }

                    bool dataval = true;

                    if(dataC.Equals("APERTURA_SC"))
                    {
                        // data apertura nella settimana corrente
                        if (!_objDBType.ToUpper().Equals("SQL"))
                           queryExec += " AND DTA_APERTURA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND DTA_APERTURA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                        else
                           queryExec += " AND DTA_APERTURA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND DTA_APERTURA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";

                        dataval = false;
                    }

                    if(dataC.Equals("APERTURA_MC"))
                    {
                        // data apertura nel mese corrente
                        if (!_objDBType.ToUpper().Equals("SQL"))
                            queryExec += " AND DTA_APERTURA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND DTA_APERTURA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                        else
                            queryExec += " AND DTA_APERTURA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND DTA_APERTURA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";

                        dataval = false;
                    }

                    if (dataC.Equals("APERTURA_TODAY"))
                    {
                        if (!_objDBType.ToUpper().Equals("SQL"))
                            queryExec += " AND to_char(DTA_APERTURA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                        else
                            queryExec += " AND DTA_APERTURA>=(SELECT getdate()) AND DTA_APERTURA<=(SELECT getdate()) ";

                        dataval = false;
                    }


                    if (!dataC.Equals("") && dataval)
                    {
                        queryExec += " AND DTA_APERTURA >= " + DocsPaDbManagement.Functions.Functions.ToDate(dataC, false) + "";
                        if (dataCdal.Equals("") && dataCal.Equals(""))
                        {
                            queryExec += " AND DTA_APERTURA < " + DocsPaDbManagement.Functions.Functions.ToDate(dataC + " 23:59:59");
                        }
                    }

                    if (!dataCdal.Equals(""))
                    {
                        queryExec += " AND DTA_APERTURA >= " + DocsPaDbManagement.Functions.Functions.ToDate(dataCdal, false) + "";
                        if (dataCal.Equals(""))
                        {
                            queryExec += " AND DTA_APERTURA < " + DocsPaDbManagement.Functions.Functions.ToDate(dataCdal + " 23:59:59");
                        }
                    }

                    if (!dataCal.Equals(""))
                    {
                        queryExec += " AND DTA_APERTURA <= " + DocsPaDbManagement.Functions.Functions.ToDate(dataCal + " 23:59:59");
                    }


                    dataval = true;

                    if(dataCh.Equals("CHIUSURA_SC"))
                    {
                        // data chiusura nella settimana corrente
                        if (!_objDBType.ToUpper().Equals("SQL"))
                            queryExec += " AND DTA_CHIUSURA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND DTA_CHIUSURA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                        else
                            queryExec += " AND DTA_CHIUSURA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND DTA_CHIUSURA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";

                        dataval = false;
                    }

                    if(dataCh.Equals("CHIUSURA_MC")){
                        // data chiusura nel mese corrente
                        if (!_objDBType.ToUpper().Equals("SQL"))
                                queryExec += " AND DTA_CHIUSURA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND DTA_CHIUSURA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryExec += " AND DTA_CHIUSURA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND DTA_CHIUSURA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";

                        dataval = false;
                    }

                    if (dataCh.Equals("CHIUSURA_TODAY"))
                    {
                        if (!_objDBType.ToUpper().Equals("SQL"))
                            queryExec += " AND to_char(DTA_CHIUSURA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                        else
                            queryExec += " AND DTA_CHIUSURA>=(SELECT getdate()) AND DTA_CHIUSURA<=(SELECT getdate()) ";

                        dataval = false;
                    }

                    if (!dataCh.Equals("") && dataval)
                    {
                        queryExec += " AND DTA_CHIUSURA >= " + DocsPaDbManagement.Functions.Functions.ToDate(dataCh, false) + "";
                        if (dataChdal.Equals("") && dataChal.Equals(""))
                        {
                            queryExec += " AND DTA_CHIUSURA < " + DocsPaDbManagement.Functions.Functions.ToDate(dataCh + " 23:59:59");
                        }
                       
                    }

                    if (!dataChdal.Equals(""))
                    {
                        queryExec +=  " AND DTA_CHIUSURA >= " + DocsPaDbManagement.Functions.Functions.ToDate(dataChdal, false) + "";
                        if (dataChal.Equals(""))
                        {
                            queryExec += " AND DTA_CHIUSURA < " + DocsPaDbManagement.Functions.Functions.ToDate(dataChdal + " 23:59:59");
                        }
                    }

                    if (!dataChal.Equals(""))
                    {
                        queryExec += " AND DTA_CHIUSURA <= " + DocsPaDbManagement.Functions.Functions.ToDate(dataChal + " 23:59:59");
                    }

                    queryExec += " group by p.CHA_STATO,d.VAR_DESC_CORR,d.SYSTEM_ID order by d.VAR_DESC_CORR, p.cha_stato asc";
                  }
                 else
                 {
                     //ESPORTA DESCRIZIONE DEI FASCICOLI E DEI DOCUMENTI CONTENUTI
                     q = DocsPaUtils.InitQuery.getInstance().getQuery("S_EXP_COUNT_FASC_DESCR");
                     if (tipo_scelta.Equals("uo"))
                     {
                         if (sottoposti == true && ruoliUoSottoposti.Length > 0)
                        {
                            //CERCA IN UO E NELLE SOTTOPOSTE
                            string ruoliSott = " IN ( ";
                            for (int i = 0; i < ruoliUoSottoposti.Length; i++)
                            {
                                ruoliSott += " " + ruoliUoSottoposti[i] + " ";
                                if (i < ruoliUoSottoposti.Length - 1)
                                {
                                    ruoliSott += ", ";
                                }
                            }
                            ruoliSott += " )";
                            queryExec += " project p, project p1, project_components ps, dpa_corr_globali d WHERE p1.system_id = p.id_fascicolo AND p.id_fascicolo IN (SELECT f.system_id FROM project f WHERE f.cha_tipo_proj = 'F' AND f.cha_tipo_fascicolo = 'P' AND ((d.id_uo = f.id_uo_creatore AND f.id_ruolo_creatore = d.system_id AND f.id_uo_creatore = " + valore_scelta + ") OR (d.id_uo = f.id_ruolo_chiusura AND f.id_ruolo_chiusura = d.system_id AND f.id_uo_chiusura = " + valore_scelta + ") OR (f.ID_RUOLO_CREATORE " + ruoliSott + " AND f.id_ruolo_creatore = d.system_id) OR (f.ID_RUOLO_CHIUSURA " + ruoliSott + " AND f.id_ruolo_chiusura = d.system_id))) AND p.system_id = ps.project_id AND p1.cha_tipo_proj = 'F' AND p1.cha_tipo_fascicolo = 'P' AND p.cha_tipo_proj = 'C' AND p.id_amm = " + idAmm + " AND p.id_titolario IN (" + idTitolario + ")";
                        }
                        else
                        {
                            //CERCA SOLO NELLA UO
                            queryExec += " project p, project p1, project_components ps, dpa_corr_globali d WHERE p1.system_id = p.id_fascicolo AND p.id_fascicolo IN (SELECT f.system_id FROM project f WHERE f.cha_tipo_proj = 'F' AND f.cha_tipo_fascicolo = 'P' AND ((d.id_uo = f.id_uo_creatore AND f.id_ruolo_creatore = d.system_id AND f.id_uo_creatore = "+ valore_scelta + ") OR (d.id_uo = f.id_ruolo_chiusura AND f.id_ruolo_chiusura = d.system_id AND f.id_uo_chiusura = "+ valore_scelta +"))) AND p.system_id = ps.project_id AND p1.cha_tipo_proj = 'F' AND p1.cha_tipo_fascicolo = 'P' AND p.cha_tipo_proj = 'C' AND p.id_amm = " + idAmm + " AND p.id_titolario IN ("+ idTitolario +")";
                        }
                     }
                     else
                     {
                        //CERCA IN RF 
                         queryExec += " project p, project p1, project_components ps, dpa_corr_globali d, dpa_l_ruolo_reg g WHERE p1.system_id = p.id_fascicolo AND p.id_fascicolo IN ( SELECT f.system_id FROM project f WHERE f.cha_tipo_proj = 'F' AND f.cha_tipo_fascicolo = 'P' AND  ( (f.ID_RUOLO_CREATORE = d.SYSTEM_ID) OR (f.ID_RUOLO_CHIUSURA = d.SYSTEM_ID)) AND f.id_ruolo_creatore = g.id_ruolo_in_uo AND g.id_registro = " + valore_scelta + ") AND p.system_id = ps.project_id AND p1.cha_tipo_proj = 'F' AND p1.cha_tipo_fascicolo = 'P' AND p.cha_tipo_proj = 'C' AND p.id_amm = " + idAmm + " AND p.id_titolario IN (" + idTitolario + ")"; 
                     }


                    bool dataval = true;

                    if(dataC.Equals("APERTURA_SC"))
                    {
                        // data apertura nella settimana corrente
                        if (!_objDBType.ToUpper().Equals("SQL"))
                            queryExec += " AND p.DTA_APERTURA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND p.DTA_APERTURA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                        else
                            queryExec += " AND p.DTA_APERTURA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND p.DTA_APERTURA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";

                        dataval = false;
                    }

                    if(dataC.Equals("APERTURA_MC"))
                    {
                        // data apertura nel mese corrente
                        if (!_objDBType.ToUpper().Equals("SQL"))
                            queryExec += " AND p.DTA_APERTURA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND p.DTA_APERTURA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                        else
                            queryExec += " AND p.DTA_APERTURA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND p.DTA_APERTURA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";

                        dataval = false;
                    }

                    if (dataC.Equals("APERTURA_TODAY"))
                    {
                        if (!_objDBType.ToUpper().Equals("SQL"))
                            queryExec += " AND to_char(p.DTA_APERTURA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                        else
                            queryExec += " AND p.DTA_APERTURA>=(SELECT getdate()) AND p.DTA_APERTURA<=(SELECT getdate()) ";

                        dataval = false;
                    }


                    if (!dataC.Equals("") && dataval)
                     {
                         queryExec += " AND p.DTA_APERTURA >= " + DocsPaDbManagement.Functions.Functions.ToDate(dataC, false) + "";
                         if (dataCdal.Equals("") && dataCal.Equals(""))
                         {
                             queryExec += " AND p.DTA_APERTURA < " + DocsPaDbManagement.Functions.Functions.ToDate(dataC + " 23:59:59");
                         }
                     }

                     if (!dataCdal.Equals(""))
                     {
                         queryExec += " AND p.DTA_APERTURA >= " + DocsPaDbManagement.Functions.Functions.ToDate(dataCdal, false) + "";
                         if (dataCal.Equals(""))
                         {
                             queryExec += " AND p.DTA_APERTURA < " + DocsPaDbManagement.Functions.Functions.ToDate(dataC + " 23:59:59");
                         }
                     }

                     if (!dataCal.Equals(""))
                     {
                         queryExec += " AND p.DTA_APERTURA <= " + DocsPaDbManagement.Functions.Functions.ToDate(dataCal + " 23:59:59");
                     }

                     dataval = true;

                     if (dataCh.Equals("CHIUSURA_SC"))
                     {
                         // data chiusura nella settimana corrente
                         if (!_objDBType.ToUpper().Equals("SQL"))
                             queryExec += " AND p.DTA_CHIUSURA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND p.DTA_CHIUSURA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                         else
                             queryExec += " AND p.DTA_CHIUSURA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND p.DTA_CHIUSURA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";

                         dataval = false;
                     }

                     if (dataCh.Equals("CHIUSURA_MC"))
                     {
                         // data chiusura nel mese corrente
                         if (!_objDBType.ToUpper().Equals("SQL"))
                             queryExec += " AND p.DTA_CHIUSURA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND p.DTA_CHIUSURA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                         else
                             queryExec += " AND p.DTA_CHIUSURA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND p.DTA_CHIUSURA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";

                         dataval = false;
                     }

                     if (dataCh.Equals("CHIUSURA_TODAY"))
                     {
                         if (!_objDBType.ToUpper().Equals("SQL"))
                             queryExec += " AND to_char(p.DTA_CHIUSURA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                         else
                             queryExec += " AND p.DTA_CHIUSURA>=(SELECT getdate()) AND p.DTA_CHIUSURA<=(SELECT getdate()) ";

                         dataval = false;
                     }

                     if (!dataCh.Equals("") && dataval)
                     {
                         queryExec += " AND p.DTA_CHIUSURA >= " + DocsPaDbManagement.Functions.Functions.ToDate(dataCh, false) + "";
                         if (dataChdal.Equals("") && dataChal.Equals(""))
                         {
                             queryExec += " AND p.DTA_CHIUSURA < " + DocsPaDbManagement.Functions.Functions.ToDate(dataCh + " 23:59:59");
                         }
                     }

                     if (!dataChdal.Equals(""))
                     {
                         queryExec += " AND p.DTA_CHIUSURA >= " + DocsPaDbManagement.Functions.Functions.ToDate(dataChdal, false) + "";
                         if (dataChal.Equals(""))
                         {
                             queryExec += " AND p.DTA_CHIUSURA < " + DocsPaDbManagement.Functions.Functions.ToDate(dataChdal + " 23:59:59");
                         }
                     }

                     if (!dataChal.Equals(""))
                     {
                         queryExec += " AND p.DTA_CHIUSURA <= " + DocsPaDbManagement.Functions.Functions.ToDate(dataChal + " 23:59:59");
                     }

                     queryExec += " GROUP BY p1.var_codice, p1.description, d.var_desc_corr, d.system_id ORDER BY d.system_id";
              }
              q.setParam("param1", queryExec);
              string commandText = q.getSQL();
              logger.Debug("Prospetti Riepilogativi - DO_StampaReportFascExcel: " + commandText);
              dbProvider.ExecuteQuery(ds, commandText);
           }
           return ds;
        }
        #endregion

        #region DO_CDCGetDSReportControlloPreventivo
        public DataSet DO_CDCGetDSReportControlloPreventivo(string idAmm, string CDCDataDa, string CDCDataA, string CDCCodiceUffici, string CDCCodiceMagistrato, string CDCCodiceRevisore)
        {
            DataSet ds = new DataSet();
            try
            {
                using (TransactionContext transaction = new TransactionContext(IsolationLevel.Serializable))
                {
                    DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                    ArrayList _parametri = new ArrayList();
                    _parametri.Add(new DocsPaUtils.Data.ParameterSP("my_int_elabora_h", 0.01));
                    _parametri.Add(new DocsPaUtils.Data.ParameterSP("inizio", CDCDataDa));
                    _parametri.Add(new DocsPaUtils.Data.ParameterSP("fine", CDCDataA));
                    _parametri.Add(new DocsPaUtils.Data.ParameterSP("my_id_amm", idAmm));
                    _parametri.Add(new DocsPaUtils.Data.ParameterSP("magistrato", CDCCodiceMagistrato));
                    _parametri.Add(new DocsPaUtils.Data.ParameterSP("revisore", CDCCodiceRevisore));
                    _parametri.Add(new DocsPaUtils.Data.ParameterSP("ufficio", CDCCodiceUffici));

                    logger.Debug("Prospetti Riepilogativi - DO_CDCGetDSReportControlloPreventivo - SP: CDC_SP_CONTROLLO_PREV");
                    dbProvider.ExecuteStoredProcedure("CDC_SP_CONTROLLO_PREV", _parametri, null);

                    string commandText = "SELECT * FROM CDC_CONTROLLO_PREV";
                    logger.Debug("Prospetti Riepilogativi - DO_CDCGetDSReportControlloPreventivo: " + commandText);
                    dbProvider.ExecuteQuery(ds, commandText);

                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1][0] = "Totali";
                    }
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore Prospetti Riepilogativi - DO_CDCGetDSReportControlloPreventivo: ", ex);
            }
            return ds;
        }
        #endregion DO_CDCGetDSReportControlloPreventivo

        #region DO_CDCGetDSReportControlloSuccessivoSCCLA
        /// <summary>
        ///Giordano Iacozzilli: 15/06/2012
        ///Modifica Per aggiungere i nuovi report 
        ///discriminati dal tipo di controllo(Suuc/Prev).
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="CDCDataDa"></param>
        /// <param name="CDCDataA"></param>
        /// <param name="CDCCodiceUffici"></param>
        /// <param name="CDCCodiceMagistrato"></param>
        /// <param name="CDCCodiceRevisore"></param>
        /// <returns></returns>
        public DataSet DO_CDCGetDSReportControlloSuccessivoSCCLA(string idAmm, string CDCDataDa, string CDCDataA, string CDCCodiceUffici, string CDCCodiceMagistrato, string CDCCodiceRevisore)
        {
            DataSet ds = new DataSet();
            try
            {
                using (TransactionContext transaction = new TransactionContext(IsolationLevel.Serializable))
                {
                    DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                    ArrayList _parametri = new ArrayList();
                    _parametri.Add(new DocsPaUtils.Data.ParameterSP("my_int_elabora_h", 0.01));
                    _parametri.Add(new DocsPaUtils.Data.ParameterSP("inizio", CDCDataDa));
                    _parametri.Add(new DocsPaUtils.Data.ParameterSP("fine", CDCDataA));
                    _parametri.Add(new DocsPaUtils.Data.ParameterSP("my_id_amm", idAmm));
                    _parametri.Add(new DocsPaUtils.Data.ParameterSP("magistrato", CDCCodiceMagistrato));
                    _parametri.Add(new DocsPaUtils.Data.ParameterSP("revisore", CDCCodiceRevisore));
                    _parametri.Add(new DocsPaUtils.Data.ParameterSP("ufficio", CDCCodiceUffici));

                    logger.Debug("Prospetti Riepilogativi - DO_CDCGetDSReportControlloSuccessivoSCCLA - SP: CDC_SP_CONTROLLO_SUCC");
                    dbProvider.ExecuteStoredProcedure("CDC_SP_CONTROLLO_SUCC", _parametri, null);

                    string commandText = "SELECT * FROM CDC_CONTROLLO_SUCC";
                    logger.Debug("Prospetti Riepilogativi - DO_CDCGetDSReportControlloSuccessivoSCCLA: " + commandText);
                    dbProvider.ExecuteQuery(ds, commandText);

                    if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1][0] = "Totali";
                    }
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore Prospetti Riepilogativi - DO_CDCGetDSReportControlloSuccessivoSCCLA: ", ex);
            }
            return ds;
        }
        #endregion DO_CDCGetDSReportControlloSuccessivoSCCLA

        #region DO_CDCGetDSReportControlloPreventivoSRC
        public DataSet DO_CDCGetDSReportControlloPreventivoSRC(string idAmm, string CDCDataDa, string CDCDataA, string CDCCodiceUffici, string CDCCodiceMagistrato, string CDCCodiceRevisore)
        {
            DataSet ds = new DataSet();
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                ArrayList _parametri = new ArrayList();
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("my_int_elabora_h", 0.01));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("inizio", CDCDataDa));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("fine", CDCDataA));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("my_id_amm", idAmm));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("magistrato", CDCCodiceMagistrato));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("revisore", CDCCodiceRevisore));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("ufficio", CDCCodiceUffici));

                logger.Debug("Prospetti Riepilogativi - DO_CDCGetDSReportControlloPreventivoSRC - SP: CDC_SP_CONTROLLO_PREV_SRC");
                dbProvider.ExecuteStoredProcedure("CDC_SP_CONTROLLO_PREV_SRC", _parametri, null);

                string commandText = "SELECT * FROM CDC_CONTROLLO_PREV_SRC";
                logger.Debug("Prospetti Riepilogativi - DO_CDCGetDSReportControlloPreventivoSRC: " + commandText);
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1][0] = "Totali";
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore Prospetti Riepilogativi - DO_CDCGetDSReportControlloPreventivoSRC: ", ex);
            }
            return ds;
        }
        #endregion DO_CDCGetDSReportControlloPreventivoSRC

        #region DO_CDCGetDSReportPensioniCivili
        public DataSet DO_CDCGetDSReportPensioniCivili(string idAmm, string CDCDataDa, string CDCDataA, string CDCCodiceUffici, string CDCCodiceMagistrato, string CDCCodiceRevisore)
        {
            DataSet ds = new DataSet();
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                ArrayList _parametri = new ArrayList();
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("my_int_elabora_h", 0.01));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("inizio", CDCDataDa));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("fine", CDCDataA));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("my_id_amm", idAmm));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("magistrato", CDCCodiceMagistrato));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("revisore", CDCCodiceRevisore));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("ufficio", CDCCodiceUffici));

                logger.Debug("Prospetti Riepilogativi - DO_CDCGetDSReportPensioniCivili - SP: CDC_SP_CONTROLLO_PENS_CIV");
                dbProvider.ExecuteStoredProcedure("CDC_SP_CONTROLLO_PENS_CIV", _parametri, null);

                string commandText = "SELECT * FROM CDC_CONTROLLO_PENS_CIV";
                logger.Debug("Prospetti Riepilogativi - DO_CDCGetDSReportPensioniCivili: " + commandText);
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1][0] = "Totali";
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore Prospetti Riepilogativi - DO_CDCGetDSReportPensioniCivili: ", ex);
            }
            return ds;
        }
        #endregion DO_CDCGetDSReportPensioniCivili

        #region DO_CDCGetDSReportPensioniMilitari
        public DataSet DO_CDCGetDSReportPensioniMilitari(string idAmm, string CDCDataDa, string CDCDataA, string CDCCodiceUffici, string CDCCodiceMagistrato, string CDCCodiceRevisore)
        {
            DataSet ds = new DataSet();
            try
            {
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                ArrayList _parametri = new ArrayList();
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("my_int_elabora_h", 0.01));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("inizio", CDCDataDa));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("fine", CDCDataA));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("my_id_amm", idAmm));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("magistrato", CDCCodiceMagistrato));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("revisore", CDCCodiceRevisore));
                _parametri.Add(new DocsPaUtils.Data.ParameterSP("ufficio", CDCCodiceUffici));

                logger.Debug("Prospetti Riepilogativi - DO_CDCGetDSReportPensioniMilitari - SP: CDC_SP_CONTROLLO_PENS_MIL");
                dbProvider.ExecuteStoredProcedure("CDC_SP_CONTROLLO_PENS_MIL", _parametri, null);

                string commandText = "SELECT * FROM CDC_CONTROLLO_PENS_MIL";
                logger.Debug("Prospetti Riepilogativi - DO_CDCGetDSReportPensioniMilitari: " + commandText);
                dbProvider.ExecuteQuery(ds, commandText);

                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1][0] = "Totali";
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore Prospetti Riepilogativi - DO_CDCGetDSReportPensioniMilitari: ", ex);
            }
            return ds;
        }
        #endregion DO_CDCGetDSReportPensioniMilitari

        #region DO_StampaExportLogExcel
        /// <summary>
        /// DO_StampaExportLogExcel: (ATTENZIONE!! SOLO PER ORACLE)
        /// </summary>
        public DataSet DO_StampaExportLogExcel(string data_inizio_iop, string data_fine_iop, string periodo_ipa, bool sottoposti, string ampiezza_ipa, string numero_ampiezza_ipa, string iagg, string ruolo, string uo, string rf, string aoo, string evento, string evento_secondario, string idAmministrazione, string descrizione_evento_secondario, string tipo_ruolo)
        {
            DataSet ds = new DataSet();
            bool numero = false;
            string queryExec = "";
            string query_periodo_iap = "";
            string annoInizioIOP;
            string annoFineIOP;
            string query_iagg = "";
            string query_ampiezza_ipa = "";
            
            //MEV CONS 1.3
            //se evento_secondario contiene la chiave PGU_FE_DISABLE_AMM_GEST_CONS
            //significa che la gestione dei log conservazione dal tool di amministrazione
            //è disabilitata
            bool isDisabledConsAmm = false;
            if (evento_secondario == "PGU_FE_DISABLE_AMM_GEST_CONS")
            {
                isDisabledConsAmm = true;
                evento_secondario = string.Empty;
                descrizione_evento_secondario = string.Empty;
            }

            DocsPaUtils.Query q;
            using (DBProvider dbProvider = new DBProvider())
            {
                if (ampiezza_ipa.Equals(periodo_ipa))
                {
                    numero = true;
                }

                //ANNO DI INIZIO IOP
                int inizio = data_inizio_iop.IndexOf('/');
                int fine = data_inizio_iop.LastIndexOf('/');

                string giorno = data_inizio_iop.Substring(0, inizio);
                annoInizioIOP = data_inizio_iop.Substring(fine + 1);
                //

                //ANNO DI FINE IOP
                inizio = data_fine_iop.IndexOf('/');
                fine = data_fine_iop.LastIndexOf('/');

                giorno = data_fine_iop.Substring(0, inizio);
                annoFineIOP = data_fine_iop.Substring(fine + 1);
                //

                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ESTRAZIONE_LOG");
                q.setParam("idAmm", idAmministrazione);

                string iop = " a.DTA_AZIONE < TO_DATE ('" + data_fine_iop + "', 'dd/mm/yyyy') AND a.DTA_AZIONE > TO_DATE ('"+ data_inizio_iop + "', 'dd/mm/yyyy')";
                q.setParam("iop", iop);

                //PERIODO IAP
                if (periodo_ipa.Equals("6"))
                {
                    query_periodo_iap = " and to_char(a.dta_azione,'YYYY') >= " + annoInizioIOP + " and to_char(a.dta_azione,'YYYY') <= " + annoFineIOP + "";
                }

                if (periodo_ipa.Equals("5"))
                {
                   query_periodo_iap = " and to_char(a.dta_azione,'Q') >= 1 and to_char(a.dta_azione,'Q') <= 4";
                }

                if (periodo_ipa.Equals("4"))
                {
                    query_periodo_iap = " and to_char(a.dta_azione,'MM') >= 1 and to_char(a.dta_azione,'MM') <= 12";
                }

                if (periodo_ipa.Equals("3"))
                {
                    query_periodo_iap = " and to_char(a.dta_azione,'W') >= 1 and to_char(a.dta_azione,'W') <= 4";
                }

                if (periodo_ipa.Equals("2"))
                {
                    query_periodo_iap = " and to_char(a.dta_azione,'D') >= 1 and to_char(a.dta_azione,'D') <= 31";
                }

                if (periodo_ipa.Equals("1"))
                {
                   query_periodo_iap = " and to_char(a.dta_azione,'HH') >= 1 and to_char(a.dta_azione,'HH') <= 24";
                }

                q.setParam("periodo_iap", query_periodo_iap);
                //


                //AMPIEZZA IAP
                if (ampiezza_ipa.Equals("6"))
                {
                    if (!numero)
                    {
                        query_ampiezza_ipa = " and to_char(a.dta_azione,'YYYY') >= " + annoInizioIOP + " and to_char(a.dta_azione,'YYYY') <= " + annoFineIOP + "";
                    }
                }

                if (ampiezza_ipa.Equals("5"))
                {
                    if (!numero && !string.IsNullOrEmpty(numero_ampiezza_ipa))
                    {
                        query_ampiezza_ipa = " and a.dta_azione IN (select b.dta_azione from dpa_log b where a.dta_azione=b.dta_azione and to_char(b.dta_azione,'Q') >= 1 and to_char(b.dta_azione,'Q') <= " + Convert.ToInt32(numero_ampiezza_ipa) + ")";
                    }
                }

                if (ampiezza_ipa.Equals("4"))
                {
                    if (!numero && !string.IsNullOrEmpty(numero_ampiezza_ipa))
                    {
                        if (periodo_ipa.Equals("5"))
                        {
                            int numeroMesiTrimestre = Convert.ToInt32(numero_ampiezza_ipa);
                            if (numeroMesiTrimestre == 1)
                            {
                                query_ampiezza_ipa = " and a.dta_azione IN (select b.dta_azione from dpa_log b where a.dta_azione=b.dta_azione and ( (to_char(b.dta_azione,'MM') = 1) or (to_char(b.dta_azione,'MM') = 4) or (to_char(b.dta_azione,'MM') = 7) or (to_char(b.dta_azione,'MM') = 10)) )";
                            }
                            if (numeroMesiTrimestre == 2)
                            {
                                query_ampiezza_ipa = " and a.dta_azione IN (select b.dta_azione from dpa_log b where a.dta_azione=b.dta_azione and ( (to_char(b.dta_azione,'MM') = 2) or (to_char(b.dta_azione,'MM') = 5) or (to_char(b.dta_azione,'MM') = 8) or (to_char(b.dta_azione,'MM') = 11)) )";
                            }
                            if (numeroMesiTrimestre == 3)
                            {
                                query_ampiezza_ipa = " and a.dta_azione IN (select b.dta_azione from dpa_log b where a.dta_azione=b.dta_azione and ( (to_char(b.dta_azione,'MM') = 3) or (to_char(b.dta_azione,'MM') = 6) or (to_char(b.dta_azione,'MM') = 9) or (to_char(b.dta_azione,'MM') = 12)) )";
                            }
                        }
                        else
                        {
                            query_ampiezza_ipa = " and a.dta_azione IN (select b.dta_azione from dpa_log b where a.dta_azione=b.dta_azione and to_char(b.dta_azione,'MM') >= 1 and to_char(b.dta_azione,'MM') <= " + Convert.ToInt32(numero_ampiezza_ipa) + ")";
                        }
                    }
                }

                if (ampiezza_ipa.Equals("3"))
                {
                    if (!numero && !string.IsNullOrEmpty(numero_ampiezza_ipa))
                    {
                        query_ampiezza_ipa = " and a.dta_azione IN (select b.dta_azione from dpa_log b where a.dta_azione=b.dta_azione and to_char(b.dta_azione,'W') >= 1 and to_char(b.dta_azione,'W') <= " + Convert.ToInt32(numero_ampiezza_ipa) + ")";
                    }
                }

                if (ampiezza_ipa.Equals("2"))
                {
                    if (!numero && !string.IsNullOrEmpty(numero_ampiezza_ipa))
                    {
                        query_ampiezza_ipa = " and a.dta_azione IN (select b.dta_azione from dpa_log b where a.dta_azione=b.dta_azione and to_char(b.dta_azione,'DD') >= 1 and to_char(b.dta_azione,'DD') <= " + Convert.ToInt32(numero_ampiezza_ipa) + ")";
                    }
                    
                }

                if (ampiezza_ipa.Equals("1"))
                {
                    if (!numero && !string.IsNullOrEmpty(numero_ampiezza_ipa))
                    {
                        query_ampiezza_ipa = " and a.dta_azione IN (select b.dta_azione from dpa_log b where a.dta_azione=b.dta_azione and to_char(b.dta_azione,'HH') >= 1 and to_char(b.dta_azione,'HH') <= " + Convert.ToInt32(numero_ampiezza_ipa) + ")";
                    }
                }

                q.setParam("ampiezza_iap", query_ampiezza_ipa);
                //FINE IAP

                //IAGG
                if (iagg.Equals("5"))
                {
                    query_iagg = "Q";
                }

                if (iagg.Equals("4"))
                {
                    query_iagg = "MM";
                }

                if (iagg.Equals("3"))
                {
                    query_iagg = "WW";
                }

                if (iagg.Equals("2"))
                {
                    query_iagg = "DDD";
                }

                if (iagg.Equals("1"))
                {
                    query_iagg = "HH";
                }

                if (iagg.Equals("0"))
                {
                    query_iagg = "MI";
                }

                q.setParam("iagg", query_iagg);
                //FINE IAGG

                //ALTRI FILTRI
                if (!string.IsNullOrEmpty(evento))
                {
                    queryExec += " and VAR_OGGETTO = '" + evento + "'";

                    if (!string.IsNullOrEmpty(evento_secondario))
                    {
                        queryExec += " and VAR_COD_AZIONE = '" + evento_secondario + "'";
                    }

                }

                //MEV CONS 1.3
                //per rimuovere log conservazione se è abilitata la chiave corrispondente
                    if (isDisabledConsAmm)
                    {
                        queryExec += " and VAR_OGGETTO NOT IN ('CONSERVAZIONE') ";
                    }

                if (!string.IsNullOrEmpty(ruolo))
                {
                    queryExec += " AND a.ID_GRUPPO_OPERATORE IN (select b.id_GRUPPO from dpa_corr_globali b where b.ID_GRUPPO = a.ID_GRUPPO_OPERATORE and b.system_id="+ ruolo + ")";
                }

                if(!string.IsNullOrEmpty(aoo))
                {
                    queryExec += " AND a.ID_GRUPPO_OPERATORE IN (select b.id_GRUPPO from dpa_corr_globali b, dpa_l_ruolo_reg c, dpa_el_registri g where g.SYSTEM_ID="+ aoo + " and c.ID_REGISTRO=g.system_id and c.id_ruolo_in_uo = b.system_id and b.ID_GRUPPO = a.ID_GRUPPO_OPERATORE)";
                }

                if (!string.IsNullOrEmpty(rf))
                {
                    queryExec += " AND a.ID_GRUPPO_OPERATORE IN (select b.id_GRUPPO from dpa_corr_globali b, dpa_l_ruolo_reg c, dpa_el_registri g where g.SYSTEM_ID=" + rf + " and c.ID_REGISTRO=g.system_id and c.id_ruolo_in_uo = b.system_id and b.ID_GRUPPO = a.ID_GRUPPO_OPERATORE)";
                }

                if (!string.IsNullOrEmpty(uo))
                {
                    if (sottoposti == true)
                    {
                        queryExec += " AND a.id_gruppo_operatore IN (SELECT g.id_gruppo FROM dpa_corr_globali g WHERE g.id_uo IN (SELECT p.system_id FROM dpa_corr_globali p START WITH p.system_id = "+ uo +" CONNECT BY PRIOR p.system_id = p.id_parent))";

                    }
                    else
                    {
                        queryExec += " AND a.ID_GRUPPO_OPERATORE IN (select b.id_GRUPPO from dpa_corr_globali b where b.ID_UO=" + uo + " and a.ID_GRUPPO_OPERATORE = b.id_gruppo)";
                    }
                    
                }

                if (!string.IsNullOrEmpty(tipo_ruolo))
                {
                    queryExec += " AND a.ID_GRUPPO_OPERATORE IN (select b.id_GRUPPO from dpa_corr_globali b where b.id_gruppo= a.id_gruppo_operatore and b.id_tipo_ruolo ="+ tipo_ruolo +")";
                }

                q.setParam("altro", queryExec);

                //FINE ALTRI FILTRI

                string commandText = q.getSQL();
                logger.Debug("Prospetti Riepilogativi - DO_StampaExportLogExcel: " + commandText);
                dbProvider.ExecuteQuery(ds, commandText);
            }
            return ds;
        }
        #endregion

        #endregion

        #endregion

        #region utils
        public DocsPaUtils.Data.ParameterSP CreateParameter(string name, object value)
		{	
			return new DocsPaUtils.Data.ParameterSP(name,value);
		}
		#endregion
	}

	#region Parametro
	/// <summary>
	/// Parametro, oggetto da passare alle 
	/// StoredProcedure.
	/// </summary>
	public class Parametro
	{
		private string _descrizione;
		private string _valore;
		private string _nome;

		#region Proprietà
		public string Nome
		{
			get
			{
				return _nome;
			}
			set
			{
				_nome = value;
			}
		}

		public string Descrizione
		{
			get
			{
				return _descrizione;
			}
			set
			{
				_descrizione = value;
			}
		}

		public string Valore
		{
			get
			{
				return _valore;
			}
			set
			{
				_valore = value;
			}
		}
		#endregion

		/// <summary>
		/// Parametro, overload del parametro.
		/// </summary>
		public Parametro()
		{
		}

		/// <summary>
		/// Parametro.
		/// </summary>
		/// <param name="descrizione">descrizione: nome del parametro</param>
		/// <param name="valore">valore: valore del parametro</param>
		public Parametro(string descrizione, string valore)
		{
			_descrizione = descrizione;
			_valore = valore;
		}

		/// <summary>
		/// Parametro.
		/// </summary>
		/// <param name="descrizione">descrizione: nome del parametro</param>
		/// <param name="valore">valore: valore del parametro</param>
		/// <param name="nome">nome: nome del parametro, serve quando
		/// usato come filtro di ricerca</param>
		public Parametro(string nome,string descrizione, string valore)
		{
			_nome = nome;
			_descrizione = descrizione;
			_valore = valore;
		}
	}
	#endregion


}
