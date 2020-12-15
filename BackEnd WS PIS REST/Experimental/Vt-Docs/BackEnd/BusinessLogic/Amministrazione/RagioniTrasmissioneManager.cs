using System;
using System.Collections;
using System.Data;
using DocsPaDB;
using DocsPaVO.amministrazione;
using DocsPaVO.Validations;
using log4net;

namespace BusinessLogic.Amministrazione
{
	/// <summary>
	/// Classe per la gestione delle ragioni trasmissione
	/// </summary>
	public class RagioniTrasmissioneManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(RagioniTrasmissioneManager));
		public RagioniTrasmissioneManager()
		{
		}

		#region Public methods

		/// <summary>
		/// Reperimento ragione trasmissione
		/// </summary>
		/// <param name="idTrasmissione"></param>
		/// <returns></returns>
		public static OrgRagioneTrasmissione GetRagioneTrasmissione(string idRagione)
		{
			OrgRagioneTrasmissione retValue=null;

			DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_RAGIONE_TRASMISSIONE");
			queryDef.setParam("idRagione",idRagione);

			string commandText=queryDef.getSQL();
			logger.Debug(commandText);

			using (DBProvider dbProvider=new DBProvider())
				using (IDataReader reader=dbProvider.ExecuteReader(commandText))
					if (reader.Read())
						retValue=CreateRagioneTrasmissione(reader);

			return retValue;
		}

		/// <summary>
		/// Reperimento di tutte le ragioni trasmissione in anagrafica
		/// </summary>
		/// <returns></returns>
		public static OrgRagioneTrasmissione[] GetRagioniTrasmissione()
		{
			return FetchRagioniTrasmissione(DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_RAGIONI_TRASMISSIONE_ANAGRAFICA").getSQL());
		}

		/// <summary>
		/// Reperimento delle ragioni trasmissione nell'ambito di un'amministrazione
		/// </summary>
		/// <param name="idAmministrazione"></param>
		/// <returns></returns>
		public static OrgRagioneTrasmissione[] GetRagioniTrasmissione(string idAmministrazione)
		{
			DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_RAGIONI_TRASMISSIONE_AMMINISTRAZIONE");
			queryDef.setParam("idAmministrazione",idAmministrazione);

			return FetchRagioniTrasmissione(queryDef.getSQL());
		}

		/// <summary>
		/// Reperimento info delle ragioni trasmissione
		/// </summary>
		/// <param name="idAmministrazione"></param>
		/// <returns></returns>
		public static OrgRagioneTrasmissione[] GetInfoRagioniTrasmissione(string idAmministrazione)
		{
			ArrayList retValue=new ArrayList();	
			OrgRagioneTrasmissione ragione = null;

			DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("S_DPARagTrasm_ALL");
			queryDef.setParam("param1","system_id as ID, var_desc_ragione as DESCRIZIONE, cha_tipo_dest as TIPO_DESTINATARIO");
			if(idAmministrazione==null || idAmministrazione.ToUpper().Equals("NULL"))
				queryDef.setParam("param2","WHERE id_amm IS NULL");
			else
				queryDef.setParam("param2","WHERE id_amm = " + idAmministrazione);

			string commandText = queryDef.getSQL();
			logger.Debug(commandText);

			using (DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider())
			{
				using (System.Data.IDataReader reader=dbProvider.ExecuteReader(commandText))
				{
					while (reader.Read())
					{
						ragione = new OrgRagioneTrasmissione();						

						ragione.ID=reader.GetValue(reader.GetOrdinal("ID")).ToString();						
						ragione.Codice=reader.GetString(reader.GetOrdinal("DESCRIZIONE"));
						ragione.TipoDestinatario=OrgRagioneTrasmissione.ParseTipoDestinatario(reader.GetString(reader.GetOrdinal("TIPO_DESTINATARIO")));

						retValue.Add(ragione);
					}
				}	
			}

			return (OrgRagioneTrasmissione[]) retValue.ToArray(typeof(OrgRagioneTrasmissione));			
		}

		/// <summary>
		/// Inserimento nuova ragione trasmissione
		/// </summary>
		/// <param name="ragione"></param>
		/// <returns></returns>
		public static ValidationResultInfo InsertRagioneTrasmissione(OrgRagioneTrasmissione ragione)
		{
			ValidationResultInfo retValue=CanInsertRagioneTrasmissione(ragione);

			if (retValue.Value)
			{
				DBProvider dbProvider=new DBProvider();

				try
				{
					dbProvider.BeginTransaction();

                    // Reperimento systemID appena inserita
                    string commandText = "SELECT " + DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null).Replace(",","") + " FROM DUAL";
                    logger.Debug(commandText);

                    string systemID;
                    retValue.Value = dbProvider.ExecuteScalar(out systemID, commandText);

                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_INSERT_RAGIONE_TRASMISSIONE");
                    ragione.ID = systemID;
                    FillParameters(true, queryDef, ragione);

                    commandText = queryDef.getSQL();
                    logger.Debug(commandText);


					int rowsAffected;
					retValue.Value=(dbProvider.ExecuteNonQuery(commandText,out rowsAffected));
				
					if (retValue.Value)
					{
                        //INSERISCO NELLA DPA_TIPO_RAGIONE
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_INSERT_DPA_TIPO_RAGIONE");
                        queryDef.setParam("idRagione", ragione.ID);
                        queryDef.setParam("codice", ragione.Codice);
                        queryDef.setParam("idAmm", ragione.IDAmministrazione);
                        queryDef.setParam("chaTipoTask", GetStringParameterValue(ragione.TipoTask ? "1" : "0"));
                        queryDef.setParam("chaContributoObbligatorio", GetStringParameterValue(ragione.ContributoTaskObbligatorio ? "1" : "0"));
                        queryDef.setParam("idTipoAtto", !string.IsNullOrEmpty(ragione.IdTipoAtto) ? ragione.IdTipoAtto : "null");

                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);
                        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    }

					if (retValue.Value)
						dbProvider.CommitTransaction();
					else
						dbProvider.RollbackTransaction();
				}
				catch
				{
					dbProvider.RollbackTransaction();
					retValue.Value=false;
				}
				finally
				{
					dbProvider.Dispose();
				}

				if (!retValue.Value)
					retValue.BrokenRules.Add(new BrokenRule("DB_ERROR","Errore nell'inserimento della ragione trasmissione"));
			}

			return retValue;
		}

		/// <summary>
		/// Aggiornamento ragione trasmissione
		/// </summary>
		/// <param name="ragione"></param>
		/// <returns></returns>
		public static ValidationResultInfo UpdateRagioneTrasmissione(OrgRagioneTrasmissione ragione)
		{
			ValidationResultInfo retValue=CanUpdateRagioneTrasmissione(ragione);

			if (retValue.Value)
			{
				DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_U_UPDATE_RAGIONE_TRASMISSIONE");
			
				FillParameters(false,queryDef,ragione);
			
				string commandText=queryDef.getSQL();
				logger.Debug(commandText);

				DBProvider dbProvider=new DBProvider();

				try
				{
					dbProvider.BeginTransaction();

					int rowsAffected;
					retValue.Value=(dbProvider.ExecuteNonQuery(commandText,out rowsAffected));
				
					if (!retValue.Value || rowsAffected==0)
					{
						retValue.Value=false;
						retValue.BrokenRules.Add(new BrokenRule("DB_ERROR","Errore nell'aggiornamento della ragione trasmissione"));
					}

                    if (retValue.Value)
                    {
                        //Aggiorno la DPA_TIPO_RAGIONE
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_U_DPA_TIPO_RAGIONE");
                        queryDef.setParam("idRagione", ragione.ID);
                        queryDef.setParam("chaTipoTask", GetStringParameterValue(ragione.TipoTask ? "1" : "0"));
                        queryDef.setParam("chaContributoObbligatorio", GetStringParameterValue(ragione.ContributoTaskObbligatorio ? "1" : "0"));
                        queryDef.setParam("idTipoAtto", !string.IsNullOrEmpty(ragione.IdTipoAtto) ? ragione.IdTipoAtto : "null");
                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);
                        dbProvider.ExecuteNonQuery(commandText, out rowsAffected);

                        if (rowsAffected == 0)
                        {
                            queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_INSERT_DPA_TIPO_RAGIONE");
                            queryDef.setParam("idRagione", ragione.ID);
                            queryDef.setParam("codice", ragione.Codice);
                            queryDef.setParam("idAmm", ragione.IDAmministrazione);
                            queryDef.setParam("chaTipoTask", GetStringParameterValue(ragione.TipoTask ? "1" : "0"));
                            queryDef.setParam("chaContributoObbligatorio", GetStringParameterValue(ragione.ContributoTaskObbligatorio ? "1" : "0"));
                            queryDef.setParam("idTipoAtto", !string.IsNullOrEmpty(ragione.IdTipoAtto) ? ragione.IdTipoAtto : "null");

                            commandText = queryDef.getSQL();
                            logger.Debug(commandText);
                            dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                        }

                        dbProvider.CommitTransaction();
                        
                        //
                        // MEV cessione Diritti - Mantieni lettura/scrittura
                        // Vengono aggiornati i modelli di trasmissione la cui ragione è stata modificata
                        #region Aggiornamento modelli di trasmissione
                        try
                        {
                            if (ragione != null)
                            {
                                int rowsAff = 0;
                                logger.Debug("Aggiornamento modelli trasmissione all'atto dell'aggiornamento della ragione di trasmissione");
                                logger.Debug("Begin Transaction");
                                dbProvider.BeginTransaction();

                                DocsPaUtils.Query queryUpd = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_MODELLI_TRASMISSIONE_BY_RAGIONE");
                                //Set
                                queryUpd.setParam("paramMantieniLettura", GetStringParameterValue(ragione.MantieniLettura ? "1" : "0"));
                                queryUpd.setParam("paramMantieniScrittura", GetStringParameterValue(ragione.MantieniScrittura ? "1" : "0"));
                                //Where
                                queryUpd.setParam("paramIdRagione", ragione.ID);
                                queryUpd.setParam("paramIdAmm", ragione.IDAmministrazione);

                                string commandTextQuery = queryUpd.getSQL();
                                logger.Debug(commandTextQuery);

                                dbProvider.ExecuteNonQuery(commandTextQuery, out rowsAff);

                                dbProvider.CommitTransaction();
                                logger.Debug("Commit Transaction");
                                logger.Debug("End Aggiornamento modelli trasmissione all'atto dell'aggiornamento della ragione di trasmissione");
                            }
                        }
                        catch
                        {
                            dbProvider.RollbackTransaction();
                            logger.Debug("Rollback Transaction");
                            logger.Debug("End Aggiornamento modelli trasmissione all'atto dell'aggiornamento della ragione di trasmissione");
                        }
                        #endregion
                        // End MEV
                        //
                    }
                    else
                        dbProvider.RollbackTransaction();
				}
				catch
				{
					dbProvider.RollbackTransaction();
					retValue.Value=false;
				}
				finally
				{
				}
			}
            
            if (ragione.PrevedeCessione == OrgRagioneTrasmissione.CedeDiritiEnum.No)
            {
                if (!GetModelliConRagioneConCessione(ragione.ID))
                {
                    retValue.Value = false;
                    retValue.BrokenRules.Add(new BrokenRule("AVVISO", "Esistono modelli di trasmissione con questa ragione che hanno la cessione dei diritti impostata"));
                }
            }

			return retValue;
		}

		/// <summary>
		/// Caricamento parametri
		/// </summary>
		/// <param name="insertMode"></param>
		/// <param name="queryDef"></param>
		/// <param name="ragione"></param>
		private static void FillParameters(bool insertMode,DocsPaUtils.Query queryDef,OrgRagioneTrasmissione ragione)
		{
			if (insertMode)
			{
				queryDef.setParam("colSystemID",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
				queryDef.setParam("systemID",string.IsNullOrEmpty(ragione.ID) ? null : ragione.ID + ",");
			}

			queryDef.setParam("tipoRagione",GetStringParameterValue(OrgRagioneTrasmissione.ParseTipoTrasmissione(ragione.Tipo)));
			queryDef.setParam("visibile",GetStringParameterValue(ragione.Visibilita?"1":"0"));
			queryDef.setParam("tipoDiritto",GetStringParameterValue(OrgRagioneTrasmissione.ParseTipoDiritto(ragione.TipoDiritto)));
			queryDef.setParam("tipoDestinatario",GetStringParameterValue(OrgRagioneTrasmissione.ParseTipoDestinatario(ragione.TipoDestinatario)));
			queryDef.setParam("risposta",GetStringParameterValue(ragione.Risposta?"1":"0"));
			queryDef.setParam("descrizione",GetStringParameterValue(ragione.Descrizione));
			queryDef.setParam("eredita",GetStringParameterValue(ragione.Eredita?"1":"0"));
			queryDef.setParam("tipoRisposta",GetStringParameterValue(ragione.PrevedeRisposta?"R":"C"));
			queryDef.setParam("idAmministrazione",ragione.IDAmministrazione);
            queryDef.setParam("mantieniLettura", GetStringParameterValue(ragione.MantieniLettura ? "1" : "0"));
            queryDef.setParam("fascicolazioneObbligatoria", GetStringParameterValue(ragione.ClassificazioneObbligatoria ? "1" : "0"));
            //
            // Aggiunto campo e parametro alle query AMM_I_INSERT_RAGIONE_TRASMISSIONE e AMM_U_UPDATE_RAGIONE_TRASMISSIONE
            // campo CHA_MANTIENI_SCRITT = @mantieniScrittura@
            queryDef.setParam("mantieniScrittura", GetStringParameterValue(ragione.MantieniScrittura ? "1" : "0"));
            // End
            //
			string notifica=OrgRagioneTrasmissione.ParseTipoNotifica(ragione.TipoNotifica);
			if (notifica!="Null")
				notifica="'" + GetStringParameterValue(notifica) +"'";

			queryDef.setParam("notifica",notifica);
			queryDef.setParam("codiceRagione",GetStringParameterValue(ragione.Codice));
            //queryDef.setParam("idAmministrazione",ragione.IDAmministrazione);
            queryDef.setParam("cedeDiritti", OrgRagioneTrasmissione.ParseCedeDiritti(ragione.PrevedeCessione));
		}

		/// <summary>
		/// Cancellazione ragione trasmissione
		/// </summary>
		/// <param name="ragione"></param>
		/// <returns></returns>
		public static ValidationResultInfo DeleteRagioneTrasmissione(OrgRagioneTrasmissione ragione)
		{
			ValidationResultInfo retValue=CanDeleteRagioneTrasmissione(ragione);

			if (retValue.Value)
			{
				DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_RAGIONE_TRASMISSIONE");
				queryDef.setParam("idRagione",ragione.ID);
			
				string commandText=queryDef.getSQL();
				logger.Debug(commandText);

				DBProvider dbProvider=null;

				try
				{
					dbProvider=new DBProvider();

					dbProvider.BeginTransaction();

					int rowsAffected;
					retValue.Value=dbProvider.ExecuteNonQuery(commandText,out rowsAffected);

					if (!retValue.Value || rowsAffected==0)
					{
						retValue.Value=false;
						retValue.BrokenRules.Add(new BrokenRule("DB_ERROR","Errore nella cancellazione della ragione trasmissione"));
					}

					if (retValue.Value)
						dbProvider.CommitTransaction();
					else
						dbProvider.RollbackTransaction();
				}
				catch
				{
					dbProvider.RollbackTransaction();
					retValue.Value=false;
				}
				finally
				{
					dbProvider.Dispose();
				}
			}
			
			return retValue;
		}

		/// <summary>
		/// Verifica vincoli in inserimento ragione trasmissione
		/// </summary>
		/// <param name="ragione"></param>
		/// <returns></returns>
		public static ValidationResultInfo CanInsertRagioneTrasmissione(OrgRagioneTrasmissione ragione)
		{	
			ValidationResultInfo retValue=IsValidRequiredFieldsRagione(DBActionTypeRagioneEnum.InsertMode,ragione);

			if (retValue.Value)
			{
				// Verifica presenza codice ragione trasmissione
				if (ContainsCodiceRagione(ragione))
				{
					retValue.Value=false;
					retValue.BrokenRules.Add(new BrokenRule("CODICE_RAGIONE","Codice ragione trasmissione già presente"));
				}
			}

			return retValue;
		}

		/// <summary>
		/// Verifica vincoli in aggiornamento ragione trasmissione
		/// </summary>
		/// <param name="ragione"></param>
		/// <returns></returns>
		public static ValidationResultInfo CanUpdateRagioneTrasmissione(OrgRagioneTrasmissione ragione)
		{
			return IsValidRequiredFieldsRagione(DBActionTypeRagioneEnum.UpdateMode,ragione);
		}

		/// <summary>
		/// Verifica vincoli in cancellazione ragione trasmissione
		/// </summary>
		/// <param name="ragione"></param>
		/// <returns></returns>
		public static ValidationResultInfo CanDeleteRagioneTrasmissione(OrgRagioneTrasmissione ragione)
		{
			ValidationResultInfo retValue=IsValidRequiredFieldsRagione(DBActionTypeRagioneEnum.DeleteMode,ragione);

			if (retValue.Value)
			{
				// Verifica presenza di almeno una trasmissione
				if (ContainsTrasmissioni(ragione.ID))
				{
					retValue.Value=false;
					retValue.BrokenRules.Add(new BrokenRule("CONTAINS_TRASMISSIONI","La ragione trasmissione è stata utilizzata in almeno una trasmissione"));
				}

				// Verifica che la ragione sia tra quella di default scelte nell'amministrazione
				if (IsRagioneDefaultAmministrazione(ragione))
				{
					retValue.Value=false;
					retValue.BrokenRules.Add(new BrokenRule("RAGIONE_DEFAULT_AMMINISTRAZIONE","La ragione trasmissione è tra quelle scelte come predefinite in amministrazione"));
				}
			}

			return retValue;
		}

        public static bool GetModelliConRagioneConCessione(string idRagione)
        {
            bool retValue = true;
            string tot = string.Empty;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONTA_MOD_RAG_CESSIONE");
                queryDef.setParam("idRagione", idRagione);
                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                DBProvider dbProvider = new DBProvider();

                dbProvider.ExecuteScalar(out tot, commandText);

                retValue = (tot.Equals("0"));
            }
            catch
            {
                retValue = true;
            }

            return retValue;
        }

		#endregion

		#region Private methods

		/// <summary>
		/// Creazione oggetto ragione trasmissione
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		private static OrgRagioneTrasmissione CreateRagioneTrasmissione(IDataReader reader)
		{
			OrgRagioneTrasmissione ragione=new OrgRagioneTrasmissione();

			ragione.ID=reader.GetValue(reader.GetOrdinal("ID")).ToString();
			ragione.Codice=reader.GetString(reader.GetOrdinal("CODICE"));
			ragione.Descrizione=reader.GetString(reader.GetOrdinal("DESCRIZIONE"));
			ragione.Tipo=OrgRagioneTrasmissione.ParseTipoTrasmissione(reader.GetString(reader.GetOrdinal("TIPO")));
			ragione.Visibilita=reader.GetString(reader.GetOrdinal("VISIBILITA"))!="0";
			ragione.TipoDiritto=OrgRagioneTrasmissione.ParseTipoDiritto(reader.GetString(reader.GetOrdinal("TIPO_DIRITTO")));
			ragione.TipoDestinatario=OrgRagioneTrasmissione.ParseTipoDestinatario(reader.GetString(reader.GetOrdinal("TIPO_DESTINATARIO")));
			ragione.Risposta=reader.GetString(reader.GetOrdinal("RISPOSTA"))!="0"; 
			ragione.Eredita=reader.GetString(reader.GetOrdinal("EREDITA"))!="0";
            if (reader.GetSchemaTable().Select("ColumnName='CHA_TIPO_TASK'").Length > 0)
            {
                if (!reader.IsDBNull(reader.GetOrdinal("CHA_TIPO_TASK")))
                    ragione.TipoTask = reader.GetString(reader.GetOrdinal("CHA_TIPO_TASK")) != "0";
                if (!reader.IsDBNull(reader.GetOrdinal("ID_TIPO_ATTO")))
                    ragione.IdTipoAtto = reader.GetValue(reader.GetOrdinal("ID_TIPO_ATTO")).ToString();
                if (!reader.IsDBNull(reader.GetOrdinal("CHA_CONTRIBUTO_OBBLIGATORIO")))
                    ragione.ContributoTaskObbligatorio = reader.GetString(reader.GetOrdinal("CHA_CONTRIBUTO_OBBLIGATORIO")) != "0";
            }
            if (!reader.IsDBNull(reader.GetOrdinal("CHA_FASC_OBBLIGATORIA")))
                ragione.ClassificazioneObbligatoria = reader.GetString(reader.GetOrdinal("CHA_FASC_OBBLIGATORIA")) != "0";
            else
                ragione.ClassificazioneObbligatoria = false;

			ragione.IDAmministrazione=reader.GetValue(reader.GetOrdinal("ID_AMMINISTRAZIONE")).ToString();
            ragione.testoMsgNotificaDoc = reader.GetValue(reader.GetOrdinal("VAR_TESTO_MSG_NOTIFICA_DOC")).ToString();
            ragione.testoMsgNotificaFasc = reader.GetValue(reader.GetOrdinal("VAR_TESTO_MSG_NOTIFICA_FASC")).ToString();
            if (!reader.IsDBNull(reader.GetOrdinal("CHA_MANTIENI_LETT")))
                ragione.MantieniLettura = reader.GetString(reader.GetOrdinal("CHA_MANTIENI_LETT")) != "0";
            //
            // Gestione Cessione Diritti: MantieniScrittura
            if (!reader.IsDBNull(reader.GetOrdinal("CHA_MANTIENI_SCRITT")))
                ragione.MantieniScrittura = reader.GetString(reader.GetOrdinal("CHA_MANTIENI_SCRITT")) != "0";
            // End
            //

			string tipoNotifica=string.Empty;
			if (!reader.IsDBNull(reader.GetOrdinal("NOTIFICA")))
				tipoNotifica=reader.GetString(reader.GetOrdinal("NOTIFICA"));
			ragione.TipoNotifica=OrgRagioneTrasmissione.ParseTipoNotifica(tipoNotifica);

			if (!reader.IsDBNull(reader.GetOrdinal("TIPO_RISPOSTA")))
				ragione.PrevedeRisposta=reader.GetString(reader.GetOrdinal("TIPO_RISPOSTA"))=="R"; // R=true, C=false
			
			// Ragioni di default in amministrazione
			if (!reader.IsDBNull(reader.GetOrdinal("ID_AMMINISTRAZIONE_TO")))
			{
				string idRagioneTo=reader.GetValue(reader.GetOrdinal("ID_AMMINISTRAZIONE_TO")).ToString();
				ragione.RagionePredefinitaDestinatari=(ragione.IDAmministrazione.Equals(idRagioneTo));
			}

			if (!reader.IsDBNull(reader.GetOrdinal("ID_AMMINISTRAZIONE_CC")))
			{
				string idRagioneCC=reader.GetValue(reader.GetOrdinal("ID_AMMINISTRAZIONE_CC")).ToString();
				ragione.RagionePredefinitaDestinatariCC=(ragione.IDAmministrazione.Equals(idRagioneCC));
			}

            if (!reader.IsDBNull(reader.GetOrdinal("CHA_CEDE_DIRITTI")))
                ragione.PrevedeCessione = OrgRagioneTrasmissione.ParseCedeDiritti(reader.GetString(reader.GetOrdinal("CHA_CEDE_DIRITTI")));
            else 
                ragione.PrevedeCessione = OrgRagioneTrasmissione.CedeDiritiEnum.No;

            if (!reader.IsDBNull(reader.GetOrdinal("CHA_RAG_SISTEMA")))
                ragione.DiSistema = OrgRagioneTrasmissione.ParseRagioneDiSistema(reader.GetString(reader.GetOrdinal("CHA_RAG_SISTEMA")));
            else
                ragione.DiSistema = OrgRagioneTrasmissione.RagioneDiSistemaEnum.No;

            return ragione;
		}

		/// <summary>
		/// Accesso ai dati e caricamento ragioni trasmissione
		/// </summary>
		/// <param name="commandText"></param>
		/// <returns></returns>
		private static OrgRagioneTrasmissione[] FetchRagioniTrasmissione(string commandText)
		{
			ArrayList retValue=new ArrayList();

			logger.Debug(commandText);

			using (DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider())
			{
				using (System.Data.IDataReader reader=dbProvider.ExecuteReader(commandText))
				{
					while (reader.Read())
						retValue.Add(CreateRagioneTrasmissione(reader));
				}	
			}

			return (OrgRagioneTrasmissione[]) retValue.ToArray(typeof(OrgRagioneTrasmissione));
		}

		/// <summary>
		/// Verifica se la ragione è stata utilizzata da almeno una trasmissione
		/// </summary>
		/// <returns></returns>
		private static bool ContainsTrasmissioni(string idRagione)
		{
			bool retValue=false;

			using (DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider())
			{
				DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_COUNT_RAGIONI_TRASMISSE");
				queryDef.setParam("idRagione",idRagione);
				
				string commandText=queryDef.getSQL();
				logger.Debug(commandText);

				string outParam;
				if (dbProvider.ExecuteScalar(out outParam,commandText))
				{
					try
					{
						retValue=(Convert.ToInt32(outParam)>0);
					}
					catch
					{
					}
				}
			}

			return retValue;
		}

		/// <summary>
		/// Verifica se la ragione trasmissione è tra quelle predefinite scelte in amministrazione
		/// </summary>
		/// <param name="ragione"></param>
		/// <returns></returns>
		private static bool IsRagioneDefaultAmministrazione(OrgRagioneTrasmissione ragione)
		{
			bool retValue=false;

			using (DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider())
			{
				DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_IS_RAGIONE_TRASMISSIONE_DEFAULT");
				queryDef.setParam("idRagione",ragione.ID);
				
				string commandText=queryDef.getSQL();
				logger.Debug(commandText);

				string outParam;
				if (dbProvider.ExecuteScalar(out outParam,commandText))
				{
					try
					{
						retValue=(Convert.ToInt32(outParam)>0);
					}
					catch
					{
					}
				}
			}

			return retValue;
		}

		/// <summary>
		/// Verifica univocità codice ragione trasmissione
		/// </summary>
		/// <returns></returns>
		private static bool ContainsCodiceRagione(DocsPaVO.amministrazione.OrgRagioneTrasmissione ragione)
		{
			bool retValue=false;

			DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_CONTAINS_CODICE_RAGIONE_TRASMISSIONE");
			queryDef.setParam("codiceRagione",ragione.Codice);
			queryDef.setParam("idAmministrazione",ragione.IDAmministrazione);

			string commandText=queryDef.getSQL();
			logger.Debug(commandText);

			using (DBProvider dbProvider=new DBProvider())
			{
				string outParam;
				if (dbProvider.ExecuteScalar(out outParam,commandText))
					retValue=(Convert.ToInt32(outParam)>0);
			}

			return retValue;
		}

		private static string GetStringParameterValue(string paramValue)
		{
			return DocsPaUtils.Functions.Functions.ReplaceApexes(paramValue);
		}

		private enum DBActionTypeRagioneEnum
		{
			InsertMode,
			UpdateMode,
			DeleteMode
		}

		private static ValidationResultInfo IsValidRequiredFieldsRagione(
			DBActionTypeRagioneEnum actionType,
			DocsPaVO.amministrazione.OrgRagioneTrasmissione ragione)
		{
			ValidationResultInfo retValue=new ValidationResultInfo();

			if (actionType!=DBActionTypeRagioneEnum.InsertMode &&
				(ragione.ID==null || 
				ragione.ID==string.Empty || 
				ragione.ID=="0"))
			{
				retValue.Value=false;
				retValue.BrokenRules.Add(new BrokenRule("ID_RAGIONE","ID ragione trasmissione mancante"));
			}

			if (actionType==DBActionTypeRagioneEnum.InsertMode ||
				actionType==DBActionTypeRagioneEnum.UpdateMode)
			{
				if (ragione.Codice==null || ragione.Codice==string.Empty)
				{
					retValue.Value=false;
					retValue.BrokenRules.Add(new BrokenRule("CODICE_RAGIONE","Codice ragione trasmissione mancante"));
				}

				if (ragione.Descrizione==null || ragione.Descrizione==string.Empty)
				{
					retValue.Value=false;
					retValue.BrokenRules.Add(new BrokenRule("DESCRIZIONE_RAGIONE","Descrizione ragione trasmissione mancante"));
				}

				if ((ragione.Descrizione!=null || ragione.Descrizione!=string.Empty) && ragione.Descrizione.Length>250)
				{
					retValue.Value=false;
					retValue.BrokenRules.Add(new BrokenRule("DESCRIZIONE_RAGIONE","Campo descrizione maggiore di 250 caratteri"));
				}

			}

			if (ragione.IDAmministrazione==null || 
				ragione.IDAmministrazione==string.Empty || 
				ragione.IDAmministrazione=="0")
			{
				retValue.Value=false;
				retValue.BrokenRules.Add(new BrokenRule("ID_AMMINISTRAZIONE","ID amministrazione mancante"));
			}

			return retValue;
		}

        public static ValidationResultInfo UpdateMessageNotificaRagioneTrasmissione(string codiceRagione, string idAmministrazione, string msgNotificaDocumenti, string msgNotificaFascioli, bool allRagioniDoc, bool allRagioniFasc)
        {
            ValidationResultInfo retValue = new ValidationResultInfo();


            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_U_UPDATE_MSG_NOTIFICA_RAGIONE_TRASMISSIONE");
            
            ArrayList executeQueries = new ArrayList();

            
            queryDef.setParam("idAmministrazione", idAmministrazione);

            //aggiorno entrambe le descrizioni per tutte le ragioni di trasmissioni
            if (allRagioniDoc && allRagioniFasc)
            {
                queryDef.setParam("testoMsgNotificaDoc", " VAR_TESTO_MSG_NOTIFICA_DOC ='" + msgNotificaDocumenti.Replace("'","''") + "' , ");
                queryDef.setParam("testoMsgNotificaFasc", " VAR_TESTO_MSG_NOTIFICA_FASC ='" + msgNotificaFascioli.Replace("'", "''") + "'");
                queryDef.setParam("paramAllRag", "");
                executeQueries.Add(queryDef.getSQL());
            }
            else
            {

                // il salvataggio riguarda solamente la ragione corrente
                if (!allRagioniDoc && !allRagioniFasc)
                {
                    queryDef.setParam("testoMsgNotificaDoc", " VAR_TESTO_MSG_NOTIFICA_DOC ='" + msgNotificaDocumenti.Replace("'","''") + "' , ");
                    queryDef.setParam("testoMsgNotificaFasc", " VAR_TESTO_MSG_NOTIFICA_FASC ='" + msgNotificaFascioli.Replace("'","''") + "'");

                    queryDef.setParam("paramAllRag", "AND UPPER(VAR_DESC_RAGIONE)='" + codiceRagione.ToUpper() + "'");

                    executeQueries.Add(queryDef.getSQL());
                }
                else
                {
                    
                    if (allRagioniDoc && !allRagioniFasc) //|| (!allRagioniDoc && allRagioniFasc))
                    {
                        //caso:    per i DOCUMENTI devo settare il testo nuovo a tutte le ragioni
                        //         per i FASCICOLI, lo setto solo per la ragione corrente

                        queryDef.setParam("testoMsgNotificaDoc", " VAR_TESTO_MSG_NOTIFICA_DOC ='" + msgNotificaDocumenti.Replace("'","''") + "'");
                        queryDef.setParam("testoMsgNotificaFasc", "");
                        queryDef.setParam("paramAllRag", "");

                        executeQueries.Add(queryDef.getSQL());

                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_U_UPDATE_MSG_NOTIFICA_RAGIONE_TRASMISSIONE");

                        queryDef.setParam("idAmministrazione", idAmministrazione);
                   
                      
                        queryDef.setParam("testoMsgNotificaDoc", "");
                        queryDef.setParam("testoMsgNotificaFasc", " VAR_TESTO_MSG_NOTIFICA_FASC ='" + msgNotificaFascioli.Replace("'","''") + "'");
                        queryDef.setParam("paramAllRag", "AND UPPER(VAR_DESC_RAGIONE)='" + codiceRagione.ToUpper() + "'");

                        executeQueries.Add(queryDef.getSQL());
                    }


                    if (!allRagioniDoc && allRagioniFasc) //|| (!allRagioniDoc && allRagioniFasc))
                    {
                        //caso:    per i FASCICOLI devo settare il testo nuovo a tutte le ragioni
                        //         per i DOCUMENTI, lo setto solo per la ragione corrente

                        queryDef.setParam("testoMsgNotificaFasc", " VAR_TESTO_MSG_NOTIFICA_FASC ='" + msgNotificaFascioli.Replace("'","''") + "'");
                        queryDef.setParam("testoMsgNotificaDoc", "");
                        queryDef.setParam("paramAllRag", "");

                        executeQueries.Add(queryDef.getSQL());

                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_U_UPDATE_MSG_NOTIFICA_RAGIONE_TRASMISSIONE");

                        queryDef.setParam("idAmministrazione", idAmministrazione);

                        queryDef.setParam("testoMsgNotificaDoc", " VAR_TESTO_MSG_NOTIFICA_DOC ='" + msgNotificaDocumenti.Replace("'","''") + "'");
                        queryDef.setParam("testoMsgNotificaFasc", "");
                        queryDef.setParam("paramAllRag", "AND UPPER(VAR_DESC_RAGIONE)='" + codiceRagione.ToUpper() + "'");

                        executeQueries.Add(queryDef.getSQL());
                    }
                }
            }
            DBProvider dbProvider = new DBProvider();

            try
            {
                dbProvider.BeginTransaction();

                int rowsAffected;

                for (int k = 0; k < executeQueries.Count; k++)
                {
                   retValue.Value = dbProvider.ExecuteNonQuery(executeQueries[k].ToString(), out rowsAffected);
                   logger.Debug("Eseguita query " + executeQueries[k]);
                   if (!retValue.Value || rowsAffected == 0)
                   {
                       retValue.Value = false;
                       retValue.BrokenRules.Add(new BrokenRule("DB_ERROR", "Errore nell'aggiornamento della ragione trasmissione"));
                       break;
                   }
                }
              

                if (retValue.Value)
                    dbProvider.CommitTransaction();
                else
                    dbProvider.RollbackTransaction();
            }
            catch
            {
                dbProvider.RollbackTransaction();
                retValue.Value = false;
            }
            finally
            {
            }


            return retValue;
        }

		#endregion
	}
}
