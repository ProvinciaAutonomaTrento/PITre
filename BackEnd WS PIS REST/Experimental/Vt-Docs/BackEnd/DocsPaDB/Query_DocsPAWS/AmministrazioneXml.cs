using System;
using System.Data;
using DocsPaDbManagement.Functions;
using System.Xml;
using System.Collections;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
	/// <summary>
	/// Classe per l'amministrazione via XML
	/// </summary>
	public class AmministrazioneXml : DBProvider
	{
        private ILog logger = LogManager.GetLogger(typeof(AmministrazioneXml));
		/// <summary>
		/// 
		/// </summary>
		/// <param name="connectionString"></param>
		public AmministrazioneXml()
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="connectionString"></param>
		public AmministrazioneXml(string connectionString)
		{
			this.InstantiateConnection(connectionString);
		}





		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public string ContaArchivioLog(string idAmm,string type)
		{
			System.Data.DataSet rec;
			string result = ""; 
			
			try
			{
                if (type.Equals("Amministrazione"))
                    idAmm += " AND VAR_COD_AZIONE LIKE 'AMM_%'";
                else
                    idAmm += " AND VAR_COD_AZIONE NOT LIKE 'AMM_%'";

				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_CONTA_LOG");				
				q.setParam("param1",idAmm);
				string command = q.getSQL();

				logger.Debug(command);
				if(!this.ExecuteQuery(out rec, command))
				{
					throw new Exception();
				}
				else
				{
					if(rec!=null)
					{
						if(rec.Tables[0].Rows.Count>0)
						{
							int valore=Int32.Parse(rec.Tables[0].Rows[0]["TOT"].ToString());
							if(valore>0)
							{
								logger.Debug("Trovati nr. " + valore + " record sulla tabella dei LOG.");
								result=Convert.ToString(valore);
							}
						}
					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la conta dei record di log sul db.", exception);
			}
			return result;
		}

		/// <summary>
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="nomeUtente"></param>
		/// <param name="nomeRuolo"></param>
		/// <returns></returns>
		public bool GetUserByName(out DocsPaVO.utente.InfoUtente infoUtente, string nomeUtente, string nomeRuolo)
		{
			bool result = true; // Presume successo
			infoUtente = new DocsPaVO.utente.InfoUtente();

			try
			{
				// Acquisizione Utente
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_USER_BY_NAME");
				q.setParam("param1",nomeUtente);
				string command = q.getSQL();
				//string command = "SELECT system_id FROM PEOPLE WHERE user_id='" + nomeUtente + "'";
				
				if(!this.ExecuteScalar(out infoUtente.idPeople, command))
				{
					throw new Exception();
				}

				// Acquisizione Ruolo
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_ROLE_BY_NAME");
				q.setParam("param1",nomeRuolo);
				//command = "SELECT system_id FROM Groups WHERE Group_Id='" + nomeRuolo + "'";
				command = q.getSQL();
				this.ExecuteScalar(out infoUtente.idGruppo, command);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'acquisizione del ruolo.", exception);

				result = false;
			}
		
			return result;
		}
		
		public string GetIDTipoRuoloByIDCorr(string idCorr)
		{
			string result = null; 

			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_IDTIPORUOLO_BY_IDCORR");
				q.setParam("param1",idCorr);
				string command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteScalar(out result, command))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la GetIDTipoRuoloByIDCorr: ", exception);

				result = null;
			}
	
			return result;
		}

		public string GetIDUtenteByName(string nomeUtente, string idAmm)
		{
			string result = null; 

			try
			{
				// Acquisizione Utente
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_IDUTENTEUO_BY_USERID");
				q.setParam("param1",nomeUtente);
				q.setParam("param2",idAmm);
				string command = q.getSQL();
			
				if(!this.ExecuteScalar(out result, command))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dell'utente", exception);

				result = null;
			}
	
			return result;
		}

		public string GetUserByName(string nomeUtente, string idAmm)
		{
			string result = null; 

			try
			{
				// Acquisizione Utente
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_USER_BY_NAME");
				q.setParam("param1",nomeUtente);
				string command = q.getSQL();
				
				if(!this.ExecuteScalar(out result, command))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dell'utente", exception);

				result = null;
			}
		
			return result;
		}

		public string GetUserByNameAndIdAmm(string nomeUtente, string idAmm)
		{
			string result = null; 

			try
			{
				// Acquisizione Utente
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_USER_BY_NAME_AND_ID_AMM");
				q.setParam("param1",nomeUtente);
				q.setParam("param2",idAmm);
				string command = q.getSQL();
				
				if(!this.ExecuteScalar(out result, command))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dell'utente", exception);

				result = null;
			}
		
			return result;
		}

		public string GetUserNetworkAliases(string idPeople)
		{
			string result = null; 

			try
			{
				// lettura record NETWORK_ALIASES
				// SELECT NETWORK_ID FROM NETWORK_ALIASES WHERE PERSONORGROUP=@param1@
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_NETWORK_ALIASES");
				q.setParam("param1",idPeople);
				string command = q.getSQL();

				logger.Debug(command);

				if(!this.ExecuteScalar(out result,command))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dei network aliases", exception);
				result = null;
			}
		
			return result;
		}

		public bool SetUserNetworkAliases(string idPeople, string networkAliases)
		{
			bool result = true; 

			try
			{
				// cancellazione record NETWORK_ALIASES
				// DELETE * FROM NETWORK_ALIASES WHERE PERSONORGROUP=@param1@
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_NETWORK_ALIASES");
				q.setParam("param1",idPeople);
				string command = q.getSQL();
				
				if(!this.ExecuteNonQuery(command))
				{
					throw new Exception();
				}

				// creazione record NETWORK_ALIASES
				if(networkAliases!="")
				{
					// INSERT INTO NETWORK_ALIASES (@param1@ PERSONORGROUP,NETWORK_TYPE,NETWORK_ID) VALUES (@param2@ @param3@,8,'@param4@')
					q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("INSERT_NETWORK_ALIASES");
					q.setParam("param1",Functions.GetSystemIdColName());
					q.setParam("param2",Functions.GetSystemIdNextVal(null));
					q.setParam("param3",idPeople);
					q.setParam("param4",networkAliases);
					command = q.getSQL();
				
					if(!this.ExecuteNonQuery(command))
					{
						throw new Exception();
					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la gestione dei network aliases: " + networkAliases, exception);

				result = false;
			}
		
			return result;
		}

		public string GetUserRole(string nomeUtente)
		{
			string result = null;
			
			try
			{
				// Acquisizione Ruolo
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_USER_ROLE");
				q.setParam("param1",nomeUtente);
				//command = "SELECT GROUP_ID FROM GROUPS,PEOPLEGROUPS,PEOPLE WHERE GROUPS.SYSTEM_ID=PEOPLEGROUPS.GROUPS_SYSTEM_ID AND PEOPLE.SYSTEM_ID=PEOPLEGROUPS.PEOPLE_SYSTEM_ID AND USER_ID='@param1@' 
				string command = q.getSQL();
				this.ExecuteScalar(out result, command);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'acquisizione del ruolo utente.", exception);

				result = null;
			}
		
			return result;
		}

		public string GetCodTipoRuoloByID(string system_id)
		{
			string result =null;

			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_COD_TIPO_RUOLO_BY_ID");
				q.setParam("param1",system_id);				
				string command = q.getSQL();
				logger.Debug(command);
				this.ExecuteScalar(out result, command);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la GetCodTipoRuoloByID: ", exception);

				result = null;
			}
		
			return result;
		}

		public string GetDescTipoRuoloByID(string system_id)
		{
			string result =null;

			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_DESC_TIPO_RUOLO_BY_ID");
				q.setParam("param1",system_id);				
				string command = q.getSQL();
				logger.Debug(command);
				this.ExecuteScalar(out result, command);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la GetDescTipoRuoloByID: ", exception);

				result = null;
			}
		
			return result;
		}

		public string GetGroupByName(string nomeGruppo)
		{
			string result =null;

			try
			{
				// Acquisizione Ruolo
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_ROLE_BY_NAME");
				q.setParam("param1",nomeGruppo);
				//command = "SELECT system_id FROM Groups WHERE Group_Id='" + nomeRuolo + "'";
				string command = q.getSQL();
				logger.Debug(command);
				this.ExecuteScalar(out result, command);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura del ruolo.", exception);

				result = null;
			}
		
			return result;
		}

		public string GetTipoFunzione(string codice)
		{
			string result = null;
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_TIPO_FUNZIONE_BY_NAME");
				q.setParam("param1",codice);
				string queryString =q.getSQL();
				logger.Debug(queryString);
				this.ExecuteScalar(out result, queryString);

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura del tipo funzione.", exception);

				result = null;
			}
		
			return result;
		}

		/// <summary>
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="nomeUtente"></param>
		/// <param name="nomeRuolo"></param>
		/// <returns></returns>
		public string GetRegByName(string nomeRegistro)
		{
			string result = null;

			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_REG_BY_NAME");
				q.setParam("param1",nomeRegistro);
				string command=q.getSQL();
				//string command = "SELECT system_id FROM DPA_EL_REGISTRI WHERE var_codice='" + nomeRegistro + "'";
				this.ExecuteScalar(out result, command);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'acquisizione del registro.", exception);

				result = null;
			}
		
			return result;
		}



		public bool GetFolderFascicolo(string idFascicolo,out System.Data.DataSet folder)
		{
			bool result = true;
			folder=null;

			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_FASC_FOLDER");
				q.setParam("param1",idFascicolo);
				string command=q.getSQL();
				logger.Debug(command);
				this.ExecuteQuery(out folder, command);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dei folder del fascicolo.", exception);

				result = false;
			}
			
			return result;
		}

		public string GetIdFascicoloGenerale(string idNodo)
		{
			string result = null;

			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_ID_FASC_GEN");
				q.setParam("param1",idNodo);
				string command=q.getSQL();
				logger.Debug(command);
				this.ExecuteScalar(out result, command);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'acquisizione del registro.", exception);

				result = null;
			}
		
			return result;
		}
		

		/// <summary>
		/// </summary>
		/// <param name="nomeAmm"></param>
		/// <returns></returns>
		public string GetAdminByName(string nomeAmm)
		{
			string result = null;

			try
			{
				//string command = "SELECT system_id FROM DPA_AMMINISTRA WHERE var_codice_amm='" + nomeAmm + "'";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_ADMIN_BY_NAME");
				q.setParam("param1",nomeAmm);
				string command=q.getSQL();
				logger.Debug(command);
				this.ExecuteScalar(out result, command);

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'acquisizione dell'amministrazione.", exception);

				result = null;
			}
		
			return result;
		}

		/// <summary>
		/// Imposta la visibilità di un dato nodo titolario/fascicolo/folder
		/// </summary>
		/// <param name="idPeople"></param>
		/// <param name="nodeId"></param>
		/// <returns></returns>
		public bool UpdateSecurity(string idPeople, string nodeId)
		{
			bool result = true; // Presume successo
			
			try
			{
				DocsPaUtils.Query q;
				q = DocsPaUtils.InitQuery.getInstance().getQuery("U_SecurityXml");
				q.setParam("param1", nodeId);
				q.setParam("param2", idPeople);
				
				string queryString = q.getSQL();
				logger.Debug(queryString);

				this.ExecuteNonQuery(queryString);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'aggiornamento della visibilità del nodo Titolario/Fascicolo/Folder.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Inserisce un record di visibilità per ogni utente di un dato ruolo. Se idPeople è NULL 
		/// allora vengono inseriti tutti gli utenti.
		/// </summary>
		/// <param name="idGruppo"></param>
		/// <param name="nodeId"></param>
		/// <returns></returns>
		public bool InsertSecurity(string idPeople, string idGruppo, string nodeId)
		{
			bool result = true; // Presume successo
			
			try
			{

				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("SELECT_PEOPLEGROUPS");
				string param=idGruppo ;

				System.Data.DataSet dataSet;
				//string queryString = "SELECT People_System_Id FROM PEOPLEGROUPS WHERE Groups_System_Id=" + idGruppo; 
				
				if(idPeople != null)
				{
					param += " AND People_System_Id<>" + idPeople;
				}

				q.setParam("param1",param);
				string queryString=q.getSQL();

				logger.Debug(queryString);

				if(this.ExecuteQuery(out dataSet, queryString))
				{
					foreach(System.Data.DataRow dataRow in dataSet.Tables[0].Rows)
					{
						string count;
						q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("CHECK_SECURITY");
						q.setParam("param1",nodeId);
						q.setParam("param2",dataRow[0].ToString());
						queryString=q.getSQL();
						logger.Debug(queryString);

						if(ExecuteScalar(out count,queryString))
						{
							if(count!="0")
							{
								q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("INSERT_SECURITY");
								q.setParam("param1",nodeId);
								q.setParam("param2",dataRow[0].ToString());
								//queryString = "INSERT INTO Security (THING, PERSONORGROUP, ACCESSRIGHTS, CHA_TIPO_DIRITTO) VALUES (" + nodeId + ", " + dataRow[0] + ", 255, 'P')";
								queryString=q.getSQL();
								logger.Debug(queryString);

								this.ExecuteNonQuery(queryString);	
							}
						}
					}
				}
				else
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento della visibilità.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Inserisce una amministrazione.
		/// </summary>
		/// <param name="idGruppo"></param>
		/// <param name="nodeId"></param>
		/// <returns></returns>
		public string NewAmministrazione(string codice, string descrizione, string libreria, string segnatura, string fascicolatura,string dominio,string serversmtp,string portasmtp,string protoInt,string ragioneTO,string ragioneCC,string usersmtp,string pwdsmtp)
		{
			string result = null;
			
			try
			{
				//controllo univocità del codice
				if(CheckUniqueCode("DPA_AMMINISTRA","VAR_CODICE_AMM",codice,"")==false)
				{
					logger.Debug("Duplicazione codice amministrazione");
					//logger.DebugAdm (true,"Duplicazione codice amministrazione",null);
					throw new Exception();
				}
				//string queryString = "INSERT INTO DPA_Amministra (" + Functions.GetSystemIdColName() + " VAR_CODICE_AMM, VAR_DESC_AMM, VAR_LIBRERIA, VAR_FORMATO_SEGNATURA, VAR_FORMATO_FASCICOLATURA) VALUES (" + Functions.GetSystemIdNextVal(null) + "'" + codice + "', '" + descrizione + "', '" + libreria + "', '" + segnatura + "', '" + fascicolatura + "')";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_AMMINISTRAZIONE");
				q.setParam("param1",Functions.GetSystemIdColName());
				q.setParam("param2",Functions.GetSystemIdNextVal(null));
				q.setParam("param3",codice.ToUpper());
				q.setParam("param4",DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione));
				q.setParam("param5",libreria);
				q.setParam("param6",segnatura);
				q.setParam("param7",fascicolatura);
				q.setParam("param8",dominio);
				q.setParam("param9",serversmtp);
				q.setParam("param10",portasmtp);
				//q.setParam("param11",protoInt);
				q.setParam("param12",ragioneTO);
				q.setParam("param13",ragioneCC);
				q.setParam("param14",usersmtp);
				q.setParam("param15",pwdsmtp);
				string queryString =q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//queryString = "SELECT system_id FROM DPA_Amministra WHERE VAR_CODICE_AMM='" + codice + "'";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_ADMIN_BY_NAME");
				q.setParam("param1",codice);
				queryString=q.getSQL();
				
				logger.Debug(queryString);

				this.ExecuteScalar(out result, queryString);	
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento dell'amministrazione.", exception);
				//logger.DebugAdm(true,"Errore durante l'inserimento dell'amministrazione.", exception);

				result = null;
			}
		
			return result;
		}

		public bool UpdateAmministrazione(string codice, string descrizione, string libreria, string segnatura, string fascicolatura,string dominio,string serversmtp,string portasmtp,string protoInt,string ragioneTO,string ragioneCC,string usersmtp,string pwdsmtp)
		{
			bool result = true;
			
			try
			{
				//string queryString = "INSERT INTO DPA_Amministra (" + Functions.GetSystemIdColName() + " VAR_CODICE_AMM, VAR_DESC_AMM, VAR_LIBRERIA, VAR_FORMATO_SEGNATURA, VAR_FORMATO_FASCICOLATURA) VALUES (" + Functions.GetSystemIdNextVal(null) + "'" + codice + "', '" + descrizione + "', '" + libreria + "', '" + segnatura + "', '" + fascicolatura + "')";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_AMMINISTRAZIONE");
				q.setParam("param1",descrizione);
				q.setParam("param2",libreria);
				q.setParam("param3",segnatura);
				q.setParam("param4",fascicolatura);
				q.setParam("param5",dominio);
				q.setParam("param6",codice.ToUpper());
				q.setParam("param7",serversmtp);
				q.setParam("param8",portasmtp);
				//q.setParam("param9",protoInt);
				q.setParam("param10",ragioneTO);
				q.setParam("param11",ragioneCC);
				q.setParam("param12",usersmtp);
				q.setParam("param13",pwdsmtp);

				string queryString =q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'aggiornamento dell'amministrazione.", exception);
				//logger.DebugAdm(true,"Errore durante l'aggiornamento dell'amministrazione.", exception);

				result = true;
			}
		
			return result;
		}

		/// <summary>
		/// Inserisce un registro.
		/// </summary>
		/// <param name="codice"></param>
		/// <param name="descrizione"></param>
		/// <param name="indirizzo"></param>
		/// <param name="utente"></param>
		/// <param name="password"></param>
		/// <param name="smtp"></param>
		/// <param name="pop"></param>
		/// <param name="porta"></param>
		/// <returns></returns>
		public string NewRegistro(string codice, string descrizione, string automatico, string indirizzo, string utente, string password, string smtp,	string pop,	string portaSmtp, string portaPop, string usersmtp, string pwdsmtp, string idAmm)
		{
			string result = null;
			
			try
			{
				if(CheckUniqueCode("DPA_EL_REGISTRI","VAR_CODICE",codice,"")==false)
				{
					logger.Debug("Duplicazione codice registro");
					//logger.DebugAdm(true,"Duplicazione codice registro",null);
					throw new Exception();
				}

				string currentDateTime = Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy HH.mm.ss"));
				string currentDate = Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy"));
				
				//				string queryString = "INSERT INTO DPA_EL_Registri (" + Functions.GetSystemIdColName() + " ID_AMM, VAR_CODICE, VAR_DESC_REGISTRO, VAR_EMAIL_REGISTRO, VAR_USER_MAIL, VAR_PWD_MAIL, NUM_RIF, CHA_STATO, VAR_SERVER_SMTP, NUM_PORTA_SMTP, VAR_SERVER_POP, NUM_PORTA_POP, DTA_OPEN, DTA_CLOSE, DTA_ULTIMO_PROTO) VALUES (" + 
				//					Functions.GetSystemIdNextVal(null) + 
				//					idAmm			+ ", '" +
				//					codice			+ "', '" + 
				//					descrizione		+ "', '" + 
				//					indirizzo		+ "', '" + 
				//					utente			+ "', '" +	
				//					password		+ "', 1, 'A', '" + 
				//					smtp			+ "', " + 
				//					portaSmtp		+ ", '" + 
				//					pop				+ "', " + 
				//					portaPop		+ ", " + 
				//					currentDateTime + ", " + 
				//					currentDateTime + ", " + 
				//					currentDate		+  ")";

				if(portaPop  == null || portaPop  == "") portaPop  = "NULL";
				if(portaSmtp == null || portaSmtp == "") portaSmtp = "NULL";

				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_REGISTRO");
				q.setParam("param1",Functions.GetSystemIdColName());
				q.setParam("param2",Functions.GetSystemIdNextVal(null));
				q.setParam("param3",idAmm);
				q.setParam("param4",codice);
				q.setParam("param5",descrizione);
				q.setParam("param6",indirizzo); 
				q.setParam("param7",utente);
				q.setParam("param8",password);
				q.setParam("param9",smtp);
				q.setParam("param10",portaSmtp);
				q.setParam("param11",pop);
				q.setParam("param12",portaPop);
				q.setParam("param13",currentDateTime);
				q.setParam("param14",currentDateTime);
				q.setParam("param15",currentDate);
				q.setParam("param16",automatico);
				q.setParam("param17",usersmtp);
				q.setParam("param18",pwdsmtp);
				string queryString =q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//queryString = "SELECT system_id FROM DPA_EL_Registri WHERE VAR_CODICE='" + codice + "'";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_REG_BY_NAME");
				q.setParam("param1",codice);
				queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteScalar(out result, queryString))
				{
					throw new Exception();
				}

				//queryString = "INSERT INTO DPA_Reg_Proto (" + Functions.GetSystemIdColName() + " num_rif, id_registro) VALUES (" + Functions.GetSystemIdNextVal(null) + "1, " + result + ")";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_REGISTRO_PROTO");
				q.setParam("param1",Functions.GetSystemIdColName());
				q.setParam("param2",Functions.GetSystemIdNextVal(null));
				q.setParam("param3","1");
				q.setParam("param4",result);
				queryString=q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento del registro.", exception);
				//logger.DebugAdm(true,"Errore durante l'inserimento del registro.", exception);
				result = null;
			}
		
			return result;
		}

		public bool UpdateRegistro(string codice, string descrizione, string automatico, string indirizzo, string utente, string password, string smtp,	string pop,	string portaSmtp, string portaPop, string usersmtp, string pwdsmtp, string idAmm)
		{
			bool result = true; //presume successo
			
			try
			{				
				if(portaSmtp == "" || portaSmtp == null)
					portaSmtp = "NULL";				
				if(portaPop == "" || portaPop == null)
					portaPop = "NULL";				

				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_REGISTRO");
				//				UPDATE DPA_EL_Registri SET VAR_DESC_REGISTRO='@param1@', 
				//				VAR_EMAIL_REGISTRO='@param2@', VAR_USER_MAIL='@param3@', VAR_PWD_MAIL='@param4@', 
				//				VAR_SERVER_SMTP='@param5@', NUM_PORTA_SMTP=@param6@, VAR_SERVER_POP='@param7@',
				//				NUM_PORTA_POP=@param8@ WHERE VAR_CODICE='@param9@' AND ID_AMM=@param10@ 
				q.setParam("param1",descrizione);
				q.setParam("param2",indirizzo);
				q.setParam("param3",utente);
				q.setParam("param4",password);
				q.setParam("param5",smtp);
				q.setParam("param6",portaSmtp);
				q.setParam("param7",pop);
				q.setParam("param8",portaPop);
				q.setParam("param9",codice);
				q.setParam("param10",idAmm);
				q.setParam("param11",automatico);	
				q.setParam("param12",usersmtp);
				q.setParam("param13",pwdsmtp);

				string queryString =q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento del registro.", exception);
				//logger.DebugAdm(true,"Errore durante l'inserimento del registro.", exception);
				result = false;
			}
		
			return result;
		}

		public bool NewServerPosta(string codice,string serverPop, string portaPop, string serverSmtp, string portaSmtp, string dominio, string descrizione)
		{
			bool result = true; //presume successo
			
			try
			{
				if(CheckUniqueCode("DPA_SERVER_POSTA","VAR_CODICE",codice,"")==false)
				{
					logger.Debug("Duplicazione codice server di posta");
					throw new Exception();
				}

				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_SERVER_POSTA");
				q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
				q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
				q.setParam("param3",codice);
				q.setParam("param4",serverPop);
				q.setParam("param5",portaPop);
				q.setParam("param6",serverSmtp);
				q.setParam("param7",portaSmtp);
				q.setParam("param8",dominio);
				q.setParam("param9",DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione));
				
				
				string queryString =q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento del server di posta.", exception);

				result = false;
			}
		
			return result;
		}

		public bool UpdateServerPosta(string codice,string serverPop, string portaPop, string serverSmtp, string portaSmtp, string dominio, string descrizione)
		{
			bool result = true; //presume successo
			
			try
			{
		
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_SERVER_POSTA");
				q.setParam("param1",serverPop);
				q.setParam("param2",portaPop);
				q.setParam("param3",serverSmtp);
				q.setParam("param4",portaSmtp);
				q.setParam("param5",dominio);
				q.setParam("param6",descrizione);
				q.setParam("param7",codice);
				
				string queryString =q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la modifica del server di posta.", exception);

				result = false;
			}
		
			return result;
		}

		public bool DeleteServerPosta(string codice)
		{
			bool result = true; //presume successo
			
			try
			{
		
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_SERVER_POSTA");
				q.setParam("param1",codice);
				
				string queryString =q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione del server di posta.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Inserisce una relazione fra utente e ruolo.
		/// </summary>
		/// <param name="idGruppo"></param>
		/// <param name="utente"></param>
		/// <returns></returns>
		public string NewUtenteRuolo(string idGruppo, string utente,bool ruoloPreferito)
		{
			string result = null;
			
			try
			{
				//string queryString = "SELECT system_id FROM People WHERE user_id='" + utente + "'";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_USER_BY_NAME");
				q.setParam("param1",utente);
				string queryString = q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteScalar(out result, queryString))
				{
					throw new Exception();
				}

				DeleteRuoloPreferitoUtente(result);

				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_UTENTE_RUOLO");
				q.setParam ("param1",idGruppo);
				q.setParam ("param2",result);
				if(ruoloPreferito)
				{
					q.setParam ("param3",", CHA_PREFERITO");
					q.setParam ("param4",", '1'");
				}
				else
				{
					q.setParam ("param3","");
					q.setParam ("param4","");
				}
				//queryString = "INSERT INTO PeopleGroups (groups_system_id, people_system_id) VALUES (" + idGruppo + ", " + result + ")";
				queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento della relazione utente/ruolo.", exception);

				result = null;
			}
		
			return result;
		}

		public string DeleteRuoloPreferitoUtente(string idUtente)
		{
			string result = null;
			
			try
			{
				//string queryString = "SELECT system_id FROM People WHERE user_id='" + utente + "'";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("REMOVE_CHA_PREFERITO_UTENTE");
				q.setParam("param1",idUtente);
				string queryString = q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteScalar(out result, queryString))
				{
					throw new Exception();
				}
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione del ruolo preferito.", exception);

				result = null;
			}
		
			return result;
		}

		/// <summary>
		/// Inserisce una relazione fra registro e ruolo.
		/// </summary>
		/// <param name="idGruppo"></param>
		/// <param name="utente"></param>
		/// <returns></returns>
		public bool NewRegistroRuolo(string idRuolo, string registro)
		{
			bool result = true; // Presume successo
			
			try
			{
				string idRegistro;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_REG_BY_NAME");
				q.setParam("param1",registro);
				string queryString=q.getSQL();
				//string queryString = "SELECT system_id FROM DPA_EL_REGISTRI WHERE VAR_CODICE='" + registro + "'";
				logger.Debug(queryString);

				if(!this.ExecuteScalar(out idRegistro, queryString))
				{
					throw new Exception();
				}

				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_RUOLO_REGISTRO");
				q.setParam("param1",Functions.GetSystemIdColName());
				q.setParam("param2",Functions.GetSystemIdNextVal(null));
				q.setParam("param3",idRegistro);
				q.setParam("param4",idRuolo);
				queryString=q.getSQL();
				//queryString = "INSERT INTO DPA_L_RUOLO_REG (" + Functions.GetSystemIdColName() + "id_registro, id_ruolo_in_uo) VALUES (" + Functions.GetSystemIdNextVal(null) + idRegistro + ", " + idRuolo + ")";
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento della relazione utente/ruolo.", exception);
				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Inserisce una relazione fra utente e ruolo.
		/// </summary>
		/// <param name="idGruppo"></param>
		/// <param name="utente"></param>
		/// <returns></returns>
		public bool NewFunzioneRuolo(string idRuolo, string tipoFunzione)
		{
			bool result = true; // Presume successo
			
			try
			{
				string idTipo;
				//string queryString = "SELECT system_id FROM DPA_Tipo_Funzione WHERE var_cod_tipo='" + tipoFunzione + "'";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_TIPO_FUNZIONE_BY_NAME");
				q.setParam("param1",tipoFunzione);
				string queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteScalar(out idTipo, queryString))
				{
					throw new Exception();
				}

				//queryString = "INSERT INTO DPA_Tipo_F_Ruolo (" + Functions.GetSystemIdColName() + " id_tipo_funz, id_ruolo_in_uo) VALUES (" + Functions.GetSystemIdNextVal(null) + idTipo + ", " + idRuolo + ")";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_TIPO_FUNZIONE_RUOLO");
				q.setParam("param1",Functions.GetSystemIdColName());
				q.setParam("param2",Functions.GetSystemIdNextVal(null));
				q.setParam("param3",idTipo);
				q.setParam("param4",idRuolo);
				queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento della relazione utente/ruolo.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Inserisce un ruolo.
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="codice"></param>
		/// <param name="descrizione"></param>
		/// <param name="livello"></param>
		/// <returns></returns>
		public string NewRuolo(string idAmm, string codice, string descrizione, int livello)
		{
			string result = null;
			
			try
			{
				//string queryString = "INSERT INTO DPA_Tipo_Ruolo (" + Functions.GetSystemIdColName() + " ID_AMM, ID_PARENT, VAR_CODICE, NUM_LIVELLO, VAR_DESC_RUOLO) VALUES (" + 
				//					Functions.GetSystemIdNextVal(null) + 
				//					idAmm		+ ", 0, '" +
				//					codice		+ "', " + 
				//					livello		+ ", '" + 
				//					descrizione + "')";
				if(CheckUniqueCode("DPA_TIPO_RUOLO","VAR_CODICE",codice,"AND ID_AMM = " + idAmm)==false)
				{
					logger.Debug("Duplicazione codice ruolo");
					//logger.DebugAdm(true,"Duplicazione codice ruolo",null);
					throw new Exception();
				}
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_TIPO_RUOLO");
				q.setParam("param1",Functions.GetSystemIdColName());
				q.setParam("param2",Functions.GetSystemIdNextVal(null));
				q.setParam("param3",idAmm);
				q.setParam("param4",codice);
				q.setParam("param5",livello.ToString ());
				q.setParam("param6",descrizione);
				string queryString=q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//queryString = "SELECT system_id FROM DPA_Tipo_Ruolo WHERE VAR_CODICE='" + codice + "'";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_TIPO_RUOLO");
				q.setParam("param1",codice);
				q.setParam("param2",idAmm);
				queryString=q.getSQL();
				logger.Debug(queryString);

				this.ExecuteScalar(out result, queryString);	
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento del ruolo.", exception);
				//logger.DebugAdm(true,"Errore durante l'inserimento del ruolo.", exception);
				result = null;
			}
		
			return result;
		}

		public string NewVisibilita(string thing,string personOrGroup)
		{
			string result = null;
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("INSERT_SECURITY");
				q.setParam("param1",thing);
				q.setParam("param2",personOrGroup);
				string queryString=q.getSQL();

				logger.Debug(queryString);

				if(thing == null || personOrGroup == null) throw new Exception();
				if(!this.ExecuteNonQuery(queryString))     throw new Exception();
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento della regola di visibilità.", exception);

				result = null;
			}
		
			return result;
		}

		/// <summary>
		/// Aggiorna la GROUPS per lo spostamento di un ruolo
		/// </summary>
		/// <param name="codTipoRuolo"></param>
		/// <param name="descTipoRuolo"></param>
		/// <param name="idGroups"></param>
		/// <returns>bool</returns>
		public bool UpdateSpostaRuoloGroups(string codTipoRuolo, string descTipoRuolo, string idGroups)
		{
			bool result = true;
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_GROUPS_SPOSTA_RUOLO");
				q.setParam("param1",codTipoRuolo);
				q.setParam("param2",descTipoRuolo);
				q.setParam("param3",idGroups);				
				string queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore nella UpdateSpostaRuoloGroups: ", exception);				
				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Aggiorna la DPA_CORR_GLOBALI per lo spostamento di un ruolo da una UO ad un altra
		/// </summary>
		/// <param name="codTipoRuolo"></param>
		/// <param name="descTipoRuolo"></param>
		/// <param name="codNewUO"></param>
		/// <param name="descNewUO"></param>
		/// <param name="idNewUO"></param>
		/// <param name="idGroups"></param>
		/// <returns>bool</returns>
		public bool UpdateSpostaRuoloCorrGlob(string codTipoRuolo, string descTipoRuolo, string idNewUO, string idGroups)
		{
			bool result = true;
		
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_CORR_GLOB_SPOSTA_RUOLO");
				q.setParam("param1",codTipoRuolo);
				q.setParam("param2",descTipoRuolo);
				q.setParam("param3",idNewUO);
				q.setParam("param4",idGroups);				
				string queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore nella UpdateSpostaRuoloCorrGlob: ", exception);				
				result = false;
			}
		
			return result;
		}

		public bool UpdateRuolo(string idAmm, string codice, string descrizione, int livello)
		{
			bool result = true;
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_TIPO_RUOLO");
				q.setParam("param1",livello.ToString ());
				q.setParam("param2",descrizione);
				q.setParam("param3",idAmm);
				q.setParam("param4",codice);
				string queryString=q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la modifica del ruolo.", exception);
				//logger.DebugAdm(true,"Errore durante la modifica del ruolo.", exception);
				result = false;
			}
		
			return result;
		}

		public bool UpdateRuoloUO(string idAmm, string codice, string descrizione, string rubrica, string riferimento,out string idRuolo)
		{
			bool result = true;
			idRuolo=null;	
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_DPA_CORR_RUOLO");
				q.setParam("param1",descrizione);
				q.setParam("param2",rubrica);
				q.setParam("param3",riferimento);
				q.setParam("param4",idAmm);
				q.setParam("param5",codice);
				string queryString=q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//recupera l'ID del ruolo in UO
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_CORR_BY_NAME");
				q.setParam("param1",codice);
				q.setParam("param2",idAmm);
				q.setParam("param3"," AND CHA_TIPO_URP='R'");
				queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteScalar(out idRuolo, queryString))
				{
					throw new Exception();
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la modifica del ruolo.", exception);
				result = false;
			}
		
			return result;
		}

		public bool DeleteRuolo(string idAmm, string codice)
		{
			bool result = true;
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_TIPO_RUOLO");
				q.setParam("param1",idAmm);
				q.setParam("param2",codice);
				string queryString=q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione del ruolo.", exception);
				//logger.DebugAdm(true,"Errore durante la cancellazione del ruolo.", exception);
				result = false;
			}
		
			return result;
		}

		public bool DeleteGroup(string codice)
		{
			bool result = true;
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_GROUPS_BY_NAME");
				q.setParam("param1",codice);
				string queryString=q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione del ruolo.", exception);
				result = false;
			}
		
			return result;
		}

		public bool DeleteCorrispondente(string idAmm, string codice)
		{
			bool result = true;
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_DPA_CORR_SINGOLO");
				q.setParam("param1",codice);
				q.setParam("param2",idAmm);
				string queryString=q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione del corrispondente.", exception);
				result = false;
			}
		
			return result;
		}

		public bool DeleteDatiRuoliUO(string idAmm, string codice)
		{
			bool result = true;
			
			try
			{
				//legge l'id del gruppo
				string idGruppo;
				DocsPaUtils.Query  q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_ROLE_BY_NAME");
				q.setParam("param1",codice);
				string queryString=q.getSQL();
				logger.Debug(queryString);
				this.ExecuteScalar(out idGruppo, queryString);	

				//legge l'ID della DPA_CORR_GLOBALI relativa al gruppo
				string idGruppoInUO;
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_CORR_BY_NAME");
				q.setParam("param1",codice);
				q.setParam("param2",idAmm);
				q.setParam("param3"," AND CHA_TIPO_URP='R'");
				queryString=q.getSQL();
				logger.Debug(queryString);
				this.ExecuteScalar(out idGruppoInUO, queryString);	
				
				//cancella il nodo dalla DPA_CORR_GLOBALI
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_DPA_CORR_SINGOLO");
				q.setParam("param1",codice);
				q.setParam("param2",idAmm);
				queryString=q.getSQL();
				logger.Debug(queryString);
				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//cancella la PEOPLEGROUPS relativa al gruppo
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_PEOPLEGROUPS");
				q.setParam("param1",idGruppo);
				queryString=q.getSQL();
				logger.Debug(queryString);
				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//cancella i record dalla DPA_TIPO_F_RUOLO
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_TIPO_FUNZIONE_RUOLO");
				q.setParam("param1",idGruppoInUO);
				queryString=q.getSQL();
				logger.Debug(queryString);
				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//cancella i record dalla DPA_L_RUOLO_REG
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_RUOLO_REGISTRO");
				q.setParam("param1",idGruppoInUO);
				queryString=q.getSQL();
				logger.Debug(queryString);
				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione del ruolo.", exception);
				result = false;
			}
		
			return result;
		}

		public bool DeleteUtentiFunzioniRegistriRuoliUO(string idAmm, string codice)
		{
			bool result = true;
			
			try
			{
				//legge l'id del gruppo nella tabella GROUPS
				string idGruppo;
				DocsPaUtils.Query  q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_ROLE_BY_NAME");
				q.setParam("param1",codice);
				string queryString=q.getSQL();
				logger.Debug(queryString);
				this.ExecuteScalar(out idGruppo, queryString);	

				//legge l'ID della DPA_CORR_GLOBALI relativa al gruppo
				string idGruppoInUO;
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_CORR_BY_NAME");
				q.setParam("param1",codice);
				q.setParam("param2",idAmm);
				q.setParam("param3"," AND CHA_TIPO_URP='R'");
				queryString=q.getSQL();
				logger.Debug(queryString);
				this.ExecuteScalar(out idGruppoInUO, queryString);	
				
				//cancella i record dalla DPA_TIPO_F_RUOLO
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_TIPO_FUNZIONE_RUOLO");
				q.setParam("param1",idGruppoInUO);
				queryString=q.getSQL();
				logger.Debug(queryString);
				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//cancella i record dalla DPA_L_RUOLO_REG
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_RUOLO_REGISTRO");
				q.setParam("param1",idGruppoInUO);
				queryString=q.getSQL();
				logger.Debug(queryString);
				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//cancella i record dalla PeopleGroups per disassociare gli utenti dal gruppo
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_PEOPLEGROUPS");
				q.setParam("param1",idGruppo);
				queryString=q.getSQL();
				logger.Debug(queryString);
				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione del ruolo.", exception);
				//logger.DebugAdm(true,"Errore durante la cancellazione del ruolo.", exception);
				result = false;
			}
		
			return result;
		}

		public bool DeleteDatiUO(string idAmm, string codice)
		{
			bool result = true;
			
			try
			{
				string idUO=GetUOByName(codice,idAmm);
				//lettura dei ruoli della UO
				System.Data.DataSet dataSetRuoli;
				if(!Exp_GetRuoliUO(out dataSetRuoli,idAmm,idUO))
				{
					throw new Exception();
				}

				if(dataSetRuoli!= null)
				{
					foreach( System.Data.DataRow rowRuoliUO in dataSetRuoli.Tables["RuoliUO"].Rows)
					{
						string codiceRuolo=rowRuoliUO["CODICE"].ToString ();
						//cancella tutti i dati relativi al nodo UO
						DeleteDatiRuoliUO(idAmm,codiceRuolo);
						//cancella il record da GROUP
						if(!DeleteGroup(codiceRuolo))
						{
							throw new Exception();
						}
					}
				}
				//cancella i dati del nodo UO
				if(!DeleteCorrispondente(idAmm,codice))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione del ruolo.", exception);
				//logger.DebugAdm(true,"Errore durante la cancellazione del ruolo.", exception);
				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Inserisce una UO.
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="codice"></param>
		/// <param name="descrizione"></param>
		/// <param name="rubrica"></param>
		/// <param name="livello"></param>
		/// <param name="idParent"></param>
		/// <returns></returns>
		public string NewUO(string idAmm, string codice, string descrizione, string rubrica, int livello, string idParent, string AOO)
		{
			string result = null;
			
			try
			{
				if(CheckUniqueCode("DPA_CORR_GLOBALI","VAR_CODICE",codice," AND ID_AMM=" + idAmm)==false)
				{
					logger.Debug("Duplicazione codice UO");
					//logger.DebugAdm(true,"Duplicazione codice UO",null);
					throw new Exception();
				}
				//				string queryString = "INSERT INTO DPA_Corr_Globali (" + Functions.GetSystemIdColName() + " ID_AMM, VAR_COD_RUBRICA, VAR_DESC_CORR, ID_PARENT, NUM_LIVELLO, VAR_CODICE, CHA_TIPO_IE, CHA_TIPO_URP) VALUES (" + 
				//					Functions.GetSystemIdNextVal(null) + 
				//					idAmm		+ ", '" +
				//					rubrica		+ "', '" +
				//					descrizione + "', " + 
				//					idParent	+ ", " + 
				//					livello		+ ", '" +
				//					codice		+ "', 'I', 'U')";

				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_DPA_CORR_UO");
				q.setParam("param1",Functions.GetSystemIdColName());
				q.setParam("param2",Functions.GetSystemIdNextVal(null));
				q.setParam("param3",idAmm);
				q.setParam("param4",rubrica);
				q.setParam("param5",descrizione);
				q.setParam("param6",idParent);
				q.setParam("param7",livello.ToString());
				q.setParam("param8",codice);
				if(AOO==null || AOO=="")
				{
					q.setParam("param9","");
					q.setParam("param10","");
				}
				else
				{
					q.setParam("param9","1");
					q.setParam("param10",AOO);
				}
				q.setParam("param11",Functions.GetDate());

				string queryString=q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//queryString = "SELECT system_id FROM DPA_Corr_Globali WHERE VAR_CODICE='" + codice + "'";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_CORR_BY_NAME");
				q.setParam("param1",codice);
				q.setParam("param2",idAmm);
				q.setParam("param3"," AND CHA_TIPO_URP='U'");
				queryString=q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteScalar(out result, queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento della UO.", exception);
				//logger.DebugAdm(true,"Errore durante l'inserimento della UO.", exception);
				result = null;
			}
		
			return result;
		}

		public string UpdateUO(string idAmm, string codice, string descrizione, string rubrica, string AOO)
		{
			string result =null;
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_DPA_CORR_UO");
				q.setParam("param1",descrizione);
				q.setParam("param2",rubrica);
				q.setParam("param3",idAmm);
				q.setParam("param4",codice);
				if(AOO==null || AOO=="")
				{
					q.setParam("param5","");
					q.setParam("param6","");
				}
				else
				{
					q.setParam("param5","1");
					q.setParam("param6",AOO);
				}
				string queryString=q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//queryString = "SELECT system_id FROM DPA_Corr_Globali WHERE VAR_CODICE='" + codice + "'";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_CORR_BY_NAME");
				q.setParam("param1",codice);
				q.setParam("param2",idAmm);
				q.setParam("param3"," AND CHA_TIPO_URP='U'");
				queryString=q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteScalar(out result, queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'aggiornamento della UO.", exception);
				//logger.DebugAdm(true,"Errore durante l'aggiornamento della UO.", exception);
				result = null;
			}
		
			return result;
		}

		public string UpdateOrdinamento(string id, string ordinamento)
		{
			string result =null;
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_ORDERING");
				q.setParam("param1",id);
				q.setParam("param2",ordinamento);
				string queryString=q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'aggiornamento dell'ordinamento", exception);

				result = null;
			}
		
			return result;
		}

		/// <summary>
		/// Creo un nuovo gruppo
		/// </summary>
		/// <param name="codice"></param>
		/// <param name="descrizione"></param>
		/// <returns></returns>
		public string NewGroup(string codice, string descrizione, string systemId)
		{
			string result = null;
			string colName = Functions.GetSystemIdColName();
			string colValue = Functions.GetSystemIdNextVal(null);

			if(systemId != null)
			{
				colName  = "SYSTEM_ID, ";
				colValue = systemId + ", ";
			}
			
			try
			{
				//controllo univocità del codice
				if(CheckUniqueCode("GROUPS","GROUP_ID",codice,"")==false)
				{
					logger.Debug("Duplicazione codice gruppo: " + codice);
					throw new Exception();
				}
				//string queryString = "INSERT INTO Groups (" + colName + " GROUP_ID, GROUP_NAME, DISABLED, ALLOW_LOGIN) VALUES (" + colValue + "'" + codice + "', '" + descrizione + "', 'N', 'Y')";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_GROUP");
				q.setParam("param1",colName);
				q.setParam("param2",colValue);
				q.setParam("param3",codice);
				q.setParam("param4",descrizione);
				string queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//queryString = "SELECT system_id FROM Groups WHERE Group_Id='" + codice + "'";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_ROLE_BY_NAME");
				q.setParam("param1",codice);
				queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteScalar(out result, queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento del gruppo.", exception);

				result = null;
			}
		
			return result;
		}

		/// <summary>
		/// Inserisce una relazione fra Ruolo e UO.
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="codice"></param>
		/// <param name="descrizione"></param>
		/// <param name="rubrica"></param>
		/// <param name="livello"></param>
		/// <param name="idParent"></param>
		/// <returns></returns>
		public bool NewRuoloUO(string idAmm, string codice, string descrizione, string codiceTipoRuolo, string rubrica, string idUO, string idGruppo, string riferimento, out string idRuolo)
		{
			bool result = true; // Presume successo
			idRuolo  = null;
			
			try
			{
				string idTipoRuolo;
				//string queryString = "SELECT system_id FROM DPA_Tipo_Ruolo WHERE VAR_CODICE='" + codiceTipoRuolo + "'";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_TIPO_RUOLO");
				q.setParam("param1",codiceTipoRuolo);
				q.setParam("param2",idAmm);
				string queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteScalar(out idTipoRuolo, queryString))
				{
					throw new Exception();
				}

				//				queryString = "INSERT INTO DPA_Corr_Globali (" + Functions.GetSystemIdColName() + " ID_AMM, VAR_COD_RUBRICA, VAR_DESC_CORR, VAR_CODICE, ID_TIPO_RUOLO, ID_UO, CHA_TIPO_IE, CHA_TIPO_URP, ID_GRUPPO) VALUES (" + 
				//					Functions.GetSystemIdNextVal(null) + 
				//					idAmm		+ ", '" +
				//					rubrica		+ "', '" +
				//					descrizione + "', '" + 
				//					codice		+ "', " + 
				//					idTipoRuolo + ", " +
				//					idUO		+ ", 'I', 'R', " + 
				//				    idGruppo	+ ")";

				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_DPA_CORR_RUOLO");
				q.setParam("param1",Functions.GetSystemIdColName());
				q.setParam("param2",Functions.GetSystemIdNextVal(null));
				q.setParam("param3",idAmm);
				q.setParam("param4",rubrica);
				q.setParam("param5",descrizione);
				q.setParam("param6",codice);
				q.setParam("param7",idTipoRuolo);
				q.setParam("param8",idUO);
				q.setParam("param9",idGruppo);
				q.setParam("param10",riferimento);
				queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//queryString = "SELECT system_id FROM DPA_Corr_Globali WHERE VAR_CODICE='" + codice + "'";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_CORR_BY_NAME");
				q.setParam("param1",codice);
				q.setParam("param2",idAmm);
				q.setParam("param3"," AND CHA_TIPO_URP='R'");
				queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteScalar(out idRuolo, queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento della UO.", exception);
				//logger.DebugAdm(true,"Errore durante l'inserimento della UO.", exception);
				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Inserisce un record nella tabella people.
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="codice"></param>
		/// <param name="descrizione"></param>
		/// <param name="livello"></param>
		/// <returns></returns>
		public string NewPeople(string idAmm, string userId, string password, string nome, string cognome, string systemId, string amministratore, string email,string abilitato,string notifica,string sede,string email_allegata)
		{
			string result = null;
			string colName = Functions.GetSystemIdColName();
			string colValue = Functions.GetSystemIdNextVal(null);

			if(systemId != null)
			{
				colName  = "SYSTEM_ID, ";
				colValue = systemId + ", ";
			}

			try
			{				
				//controllo univocità del codice
//				if(CheckUniqueCode("PEOPLE","USER_ID",userId,"AND ID_AMM = " + idAmm)==false)
//				{
//					logger.Debug("Duplicazione codice utente");
//					throw new Exception();
//				}

				nome	= DocsPaUtils.Functions.Functions.ReplaceApexes(nome);
				cognome = DocsPaUtils.Functions.Functions.ReplaceApexes(cognome);

				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_PEOPLE");
				q.setParam("param1",colName);
				q.setParam("param2",colValue);
				q.setParam("param3",userId);
				q.setParam("param4",nome + " " + cognome);
				q.setParam("param5",password);
				q.setParam("param6",idAmm);
				q.setParam("param7",cognome);
				q.setParam("param8",nome);
				q.setParam("param9",amministratore);
				q.setParam("param10",email);
				q.setParam("param11",abilitato);
				q.setParam("param12",notifica);
				q.setParam("param13",sede);
				q.setParam("param14",email_allegata);
				string queryString=q.getSQL();
				logger.Debug(queryString);


				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//queryString = "SELECT system_id FROM People WHERE User_Id='" + userId + "'";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_USER_BY_NAME");
				q.setParam("param1",userId);
				queryString=q.getSQL();
				logger.Debug(queryString);

				this.ExecuteScalar(out result, queryString);
	
				if(result == null)
				{
					throw new Exception();
				}
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento nella tabella people.", exception);

				result = null;
			}
		
			return result;
		}

		public bool UpdatePeople(string idAmm, string userId, string password, string nome, string cognome, string amministrazione, string email,string notifica,string sede,string email_allegata)
		{
			bool result = true; //presume successo
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_PEOPLE");
				q.setParam("param1",nome + " " + cognome.Replace("'","''"));
				q.setParam("param2",password);
				q.setParam("param3",cognome.Replace("'","''"));
				q.setParam("param4",nome);
				q.setParam("param5",userId);
				q.setParam("param6",idAmm);
				q.setParam("param7",amministrazione);
				q.setParam("param8",email);
				q.setParam("param9",notifica);
				q.setParam("param10",sede);
				q.setParam("param11",email_allegata);
				string queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'aggiornamento nella tabella people.", exception);

				result = false;
			}
		
			return result;
		}

		public bool UpdateRuoloRif(string idRuolo,string idRegistro)
		{
			bool result = true; //presume successo
			try
			{
				//cancella l'eventuale ruolo di riferimento precedente
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_RUOLO_RIF");
				q.setParam("param1","");
				q.setParam("param2",idRegistro);
				q.setParam("param3"," AND CHA_RIFERIMENTO='1'");
				string queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//imposta il ruolo di riferimento
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_RUOLO_RIF");
				q.setParam("param1","1");
				q.setParam("param2",idRegistro);
				q.setParam("param3"," AND ID_RUOLO_IN_UO=" + idRuolo);
				queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'aggiornamento nella tabella people.", exception);

				result = false;
			}
		
			return result;
		}

		public bool UpdateUtenteRif(string idUtente,string idGruppo,string idAmm)
		{
			bool result = true; //presume successo
			try
			{
				//cancella l'eventuale utente di riferimento precedente
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("REMOVE_UTENTE_RIF");
				q.setParam("param1",idAmm);
				string queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//imposta l'utente di riferimento
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_UTENTE_RIF");
				q.setParam("param1","1");
				q.setParam("param2","GROUPS_SYSTEM_ID="+idGruppo);
				q.setParam("param3"," AND PEOPLE_SYSTEM_ID=" + idUtente);
				queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'aggiornamento nella tabella peoplegroups.", exception);

				result = false;
			}
		
			return result;
		}

		//la cancellazione di un utente viene effettuata modificando l'ID aggiungendo '_DEL'
		public bool DisablePeople(string idAmm, string userId)
		{
			bool result = true; //presume successo
			try
			{
				//disabilita l'utente sulla people
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DISABLE_PEOPLE");
				q.setParam("param1",userId);
				q.setParam("param2",idAmm);
				string queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				string idPeople;
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_USER_BY_NAME");
				q.setParam("param1",userId);
				queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteScalar(out idPeople,queryString))
				{
					throw new Exception();
				}

				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_UTENTE_DTA_FINE");
				q.setParam("param1",Functions.GetDate());
				q.setParam("param2",idPeople);
				queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la disabilitazione di un utente.", exception);

				result = false;
			}
		
			return result;
		}

		//la cancellazione di un utente viene effettuata modificando l'ID aggiungendo '_DEL'
		public bool EnablePeople(string idAmm, string userId)
		{
			bool result = true; //presume successo
			try
			{
				//abilita l'utente sulla people
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("ENABLE_PEOPLE");
				q.setParam("param1",userId);
				q.setParam("param2",idAmm);
				string queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				string idPeople;
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_USER_BY_NAME");
				q.setParam("param1",userId);
				queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteScalar(out idPeople,queryString))
				{
					throw new Exception();
				}

				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_UTENTE_DTA_FINE");
				q.setParam("param1","NULL");
				q.setParam("param2",idPeople);
				queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la disabilitazione di un utente.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Inserisce un utente.
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="codice"></param>
		/// <param name="descrizione"></param>
		/// <param name="livello"></param>
		/// <returns></returns>
		public bool NewUtente(string peopleId, string idAmm, string userId, string password, string nome, string cognome, string codiceRubrica, string registro, string email, string smtp, string portaSmtp)
		{
			bool result = true; // Presume Successo
			if(portaSmtp == null || portaSmtp == "") portaSmtp = "NULL";
			
			try
			{
				string codiceRegistro = null;

				if(registro != null)
				{
					codiceRegistro = GetRegByName(registro);
				}

				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_DPA_CORR_UTENTE");
				q.setParam("param1",Functions.GetSystemIdColName());
				string regField="";
				string regValue="";
				
				nome    = DocsPaUtils.Functions.Functions.ReplaceApexes(nome);
				cognome = DocsPaUtils.Functions.Functions.ReplaceApexes(cognome);

				if(codiceRegistro != null)
				{
					regField= ", ID_REGISTRO";
					regValue=", "+ codiceRegistro +")";
				}
				q.setParam("param2",regField);
				q.setParam("param12",regValue);
				q.setParam("param3",Functions.GetSystemIdNextVal(null));
				q.setParam("param4",idAmm);
				q.setParam("param5",codiceRubrica);
				q.setParam("param6",DocsPaUtils.Functions.Functions.ReplaceApexes(nome) + " "+ DocsPaUtils.Functions.Functions.ReplaceApexes(cognome));
				//q.setParam("param61",DocsPaUtils.Functions.Functions.GetDate(false)); cinquepalmi - adamo
				q.setParam("param61",Functions.GetDate());
				q.setParam("param7",userId);
				q.setParam("param8",DocsPaUtils.Functions.Functions.ReplaceApexes(cognome));
				q.setParam("param9",DocsPaUtils.Functions.Functions.ReplaceApexes(nome));
				q.setParam("param10",peopleId);
				q.setParam("param11",email);
				q.setParam("param13",smtp);
				q.setParam("param14",portaSmtp);
				string queryString=q.getSQL();

				//string queryString = "INSERT INTO DPA_Corr_Globali (" + Functions.GetSystemIdColName() + " ID_AMM, VAR_COD_RUBRICA, VAR_DESC_CORR, VAR_CODICE, VAR_COGNOME, VAR_NOME, ID_PEOPLE, CHA_TIPO_CORR, CHA_TIPO_IE, CHA_TIPO_URP";
				
				//				if(codiceRegistro != null)
				//				{
				//					queryString += ", ID_REGISTRO";
				//				}

				//				queryString += ") VALUES (" + 
				//					Functions.GetSystemIdNextVal(null) + 
				//					idAmm		  + ", '" +
				//					codiceRubrica + "', '" +
				//					nome + " "	  + cognome + "', '" + 
				//					userId		  + "', '" + 
				//					cognome		  + "', '" +
				//					nome		  + "', " +
				//					peopleId	  + ", 'S', 'I', 'P'";

				//				if(codiceRegistro != null)
				//				{
				//					queryString += ", " + codiceRegistro;
				//				}

				//queryString += ")";

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento dell'utente.", exception);
				//logger.DebugAdm(true,"Errore durante l'inserimento dell'utente.", exception);
				result = false;
			}
		
			return result;
		}

		public bool UpdateUtente(string idAmm, string userId, string password, string nome, string cognome, string codiceRubrica, string registro,string email)
		{
			bool result = true; // Presume Successo
			
			try
			{
				string codiceRegistro = null;

				if(registro != null)
				{
					codiceRegistro = GetRegByName(registro);
				}

				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_DPA_CORR_UTENTE");
				string regString="";
				if(codiceRegistro != null)
				{
					regString= ", ID_REGISTRO=" + codiceRegistro;
				}
				q.setParam("param1",codiceRubrica);
				q.setParam("param2",nome + " "+ cognome.Replace("'","''"));
				q.setParam("param3",cognome.Replace("'","''"));
				q.setParam("param4",nome);
				q.setParam("param5",regString);
				q.setParam("param6",userId);
				q.setParam("param7",idAmm);
				q.setParam("param8",email);
				string queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'aggiornamento dell'utente.", exception);
				//logger.DebugAdm(true,"Errore durante l'aggiornamento dell'utente.", exception);
				result = false;
			}
		
			return result;
		}
		

		/// <summary>
		/// Inserisce un tipo funzione.
		/// </summary>
		/// <param name="codice"></param>
		/// <param name="descrizione"></param>
		/// <returns></returns>
		public string NewTipoFunzione(string codice, string descrizione)
		{
			string result = null;
			
			try
			{
				//verifica l'esistenza del tipo funzione passato (per codice)
				//string queryString = "SELECT system_id FROM DPA_Tipo_Funzione WHERE VAR_COD_TIPO='" + codice + "'";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_TIPO_FUNZIONE_BY_NAME");
				q.setParam("param1",codice);
				string queryString =q.getSQL();
				logger.Debug(queryString);

				this.ExecuteScalar(out result, queryString);

				if(result==null || result=="")
				{
					//se il tipo funzione non esiste, lo crea e torna il system_id
					//queryString = "INSERT INTO DPA_Tipo_Funzione (" + Functions.GetSystemIdColName() + " VAR_COD_TIPO, VAR_DESC_TIPO_FUN, CHA_VIS) VALUES (" + Functions.GetSystemIdNextVal(null) + "'" + codice + "', '" + descrizione + "', 1)";
					q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_TIPO_FUNZIONE");
					q.setParam("param1",Functions.GetSystemIdColName() );
					q.setParam("param2",Functions.GetSystemIdNextVal(null));
					q.setParam("param3",codice);
					q.setParam("param4",descrizione);
					queryString =q.getSQL();
					logger.Debug(queryString);

					if(!this.ExecuteNonQuery(queryString))
					{
						throw new Exception();
					}

					//queryString = "SELECT system_id FROM DPA_Tipo_Funzione WHERE VAR_COD_TIPO='" + codice + "'";
					q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_TIPO_FUNZIONE_BY_NAME");
					q.setParam("param1",codice);
					queryString =q.getSQL();
					logger.Debug(queryString);

					this.ExecuteScalar(out result, queryString);	
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento del tipo funzione.", exception);
				//logger.DebugAdm(true,"Errore durante l'inserimento del tipo funzione.", exception);

				result = null;
			}
		
			return result;
		}

		public bool UpdateTodoListUtente(string idOldUser,string idNewUser,string idRuolo)
		{
			bool result = true;

			System.Data.DataSet preTrasm;

			System.Data.DataSet trasmissioni;

			try
			{
				//prende la system_id della corr_globali del vecchio utente
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_CORR_BY_CODE");
				q.setParam("param1", "ID_PEOPLE");
				q.setParam("param2", idOldUser);
				string command = q.getSQL();
				string idCorrOld;
				logger.Debug(command);

				if(!this.ExecuteScalar(out idCorrOld,command)) throw new Exception();

				//legge le trasmissioni 
				q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_TRASMISSIONI_COMPLETE");
				q.setParam("param1",idRuolo);
				q.setParam("param2",idOldUser);	
				q.setParam("param3",idCorrOld);
				string queryString =q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteQuery(out trasmissioni,queryString))
				{
					throw new Exception();					
				}

				if(trasmissioni!=null)
				{
					foreach(System.Data.DataRow trasmissione in trasmissioni.Tables[0].Rows)
					{
						// verifica il tipo di trasmissione (a utente o a ruolo)
						switch (trasmissione[1].ToString())
						{
							case "R": // ruolo
								
								//verifica se il nuovo utente ha già questa trasmissione
								q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_TRASM_UTENTE_BY_IDTRASM_IDPEOPLE");
								q.setParam("param1",trasmissione[2].ToString());
								q.setParam("param2",idNewUser);
								command = q.getSQL();
								logger.Debug(command);
								
								if(this.ExecuteQuery(out preTrasm,command))
								{
									if(preTrasm!=null)
									{
										if(preTrasm.Tables[0].Rows.Count>0)
										{
											int valore=Int32.Parse(preTrasm.Tables[0].Rows[0]["TOT"].ToString());
											if(valore>0)
											{
												logger.Debug("Trovate già trasmissioni per il nuovo utente: " + idNewUser + ". Esegue cancellazione delle trasmissioni del vecchio utente: " + idOldUser);
												//elimina le trasmissioni del vecchio utente
												q = DocsPaUtils.InitQuery.getInstance().getQuery("DELETE_TRASM_UTENTE");
												q.setParam("param1",trasmissione[0].ToString());
												q.setParam("param2",idOldUser);
												queryString = q.getSQL();
												logger.Debug(queryString);

												if(!this.ExecuteNonQuery(queryString))
												{
													throw new Exception();							
												}
											}
											else
											{
												//esegue l'update del record di trasm_utente
												q = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_TRASM_UTENTE");
												q.setParam("param1",idNewUser);
												q.setParam("param2",trasmissione[0].ToString());
												queryString = q.getSQL();
												logger.Debug(queryString);

												if(!this.ExecuteNonQuery(queryString))
												{
													throw new Exception();							
												}
											}
										}										
									}
								}
								
								break;

							case "U": // utente

								//esegue l'update del record di trasm_utente
								q = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_TRASM_UTENTE");
								q.setParam("param1",idNewUser);
								q.setParam("param2",trasmissione[0].ToString());
								queryString = q.getSQL();
								logger.Debug(queryString);

								if(!this.ExecuteNonQuery(queryString))
								{
									throw new Exception();							
								}
								
								//prende prima la system_id della corr_globali del nuovo utente per eseguire successivamente l'update del record di trasm_singola
								q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_CORR_BY_CODE");
								q.setParam("param1", "ID_PEOPLE");
								q.setParam("param2", idNewUser);
								command = q.getSQL();
								string idCorrNew;
								logger.Debug(command);

								if(!this.ExecuteScalar(out idCorrNew,command)) throw new Exception();
								
								//esegue l'update sulla trasm_singola
								q = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_TRASM_SINGOLA");
								q.setParam("param1",idCorrNew);
								q.setParam("param2",trasmissione[2].ToString());
								queryString = q.getSQL();
								logger.Debug(queryString);

								if(!this.ExecuteNonQuery(queryString))
								{
									throw new Exception();							
								}

								//quindi esegue l'update della security
								q = DocsPaUtils.InitQuery.getInstance().getQuery("UPDATE_TRASM_SECURITY");
								q.setParam("param1",idNewUser);
								q.setParam("param2",idOldUser);
								queryString = q.getSQL();
								logger.Debug(queryString);

								if(!this.ExecuteNonQuery(queryString))
								{
									throw new Exception();							
								}

								break;
						}
					}

				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'aggiornamento della todolist utente", exception);
				result = false;
			}
		
			return result;
		}


		public string UpdateTipoFunzione(string codice, string descrizione)
		{
			string result = null;
			
			try
			{
				
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_TIPO_FUNZIONE");
				q.setParam("param1",descrizione);
				q.setParam("param2",codice);
				string queryString =q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_TIPO_FUNZIONE_BY_NAME");
				q.setParam("param1",codice);
				queryString =q.getSQL();
				logger.Debug(queryString);

				this.ExecuteScalar(out result, queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la modifica del tipo funzione.", exception);
				//logger.DebugAdm(true,"Errore durante la modifica del tipo funzione.", exception);

				result = null;
			}
		
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="codice"></param>
		/// <param name="tipo"></param>
		/// <param name="visibilita"></param>
		/// <param name="diritti"></param>
		/// <param name="destinatario"></param>
		/// <param name="risposta"></param>
		/// <param name="tipoRisposta"></param>
		/// <param name="eredita"></param>
		/// <param name="note"></param>
		/// <param name="idAmm"></param>
		/// <returns></returns>
		public bool NewRagioneTrasmissione(string codice, string tipo, string visibilita, string diritti, string destinatario, string risposta, string tipoRisposta, string eredita, string note, string notifica, string idAmm)
		{
			bool result = true; // Presume successo
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_DPA_RAGIONE_TRASM");
				q.setParam("param1",Functions.GetSystemIdColName());
				q.setParam("param2",Functions.GetSystemIdNextVal(null));
				q.setParam("param3",codice.Replace("'","''"));
				q.setParam("param4",tipo);
				q.setParam("param5",visibilita);
				q.setParam("param6",diritti);
				q.setParam("param7",destinatario);
				q.setParam("param8",risposta);
				q.setParam("param9",note.Replace("'","''"));
				q.setParam("param10",eredita);
				q.setParam("param11",tipoRisposta);
				q.setParam("param12",idAmm);

				if(notifica==null || notifica==string.Empty || notifica.ToUpper().Equals("NULL"))
					q.setParam("param13","NULL");
				else
					q.setParam("param13","'" + notifica + "'");

				string queryString =q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento della ragione trasmissione.", exception);
				//logger.DebugAdm(true,"Errore durante l'inserimento della ragione trasmissione.", exception);
				result = false;
			}
		
			return result;
		}

		public bool UpdateRagioneTrasmissione(string codice, string tipo, string visibilita, string diritti, string destinatario, string risposta, string tipoRisposta, string eredita, string note, string notifica, string idAmm)
		{
			bool result = true; // Presume successo
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_DPA_RAGIONE_TRASM");
				q.setParam("param1",tipo);
				q.setParam("param2",visibilita);
				q.setParam("param3",diritti);
				q.setParam("param4",destinatario);
				q.setParam("param5",risposta);
				q.setParam("param6",note.Replace("'","''"));
				q.setParam("param7",eredita);
				q.setParam("param8",tipoRisposta);
				q.setParam("param9",codice.Replace("'","''"));
				q.setParam("param10",idAmm);
				if(notifica==null || notifica==string.Empty || notifica.ToUpper().Equals("NULL"))
					q.setParam("param11","NULL");
				else
					q.setParam("param11","'" + notifica + "'");

				string queryString =q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la modifica della ragione trasmissione.", exception);
				//logger.DebugAdm(true,"Errore durante la modifica della ragione trasmissione.", exception);
				result = false;
			}
		
			return result;
		}

		public bool DeleteRagioneTrasmissione(string codice, string idAmm)
		{
			bool result = true; // Presume successo
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_DPA_RAGIONE_TRASM");
				q.setParam("param1",codice);
				q.setParam("param2",idAmm);
				string queryString =q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione della ragione trasmissione.", exception);
				//logger.DebugAdm(true,"Errore durante la cancellazione della ragione trasmissione.", exception);
				result = false;
			}
		
			return result;
		}

		public bool DeleteTipoFunzione(string codice)
		{
			bool result = true; // Presume successo
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_TIPO_FUNZIONE");
				q.setParam("param1",codice);
				string queryString =q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione del tipo funzione.", exception);
				//logger.DebugAdm(true,"Errore durante la cancellazione del tipo funzione.", exception);

				result = false;
			}
		
			return result;
		}

		public bool DeleteVisibilita(string thing)
		{
			bool result = true; // Presume successo
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_SECURITY");
				q.setParam("param1",thing);
				string queryString =q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione delle regole di visibilità.", exception);

				result = false;
			}
		
			return result;
		}

		public bool CopyVisibilita(string source,string destination)
		{
			bool result = true; // Presume successo
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_SECURITY");
				q.setParam("param1",source);
				string queryString =q.getSQL();

				logger.Debug(queryString);
				System.Data.DataSet security;
				if(!this.ExecuteQuery(out security,queryString))
				{
					throw new Exception();
				}
				//copia i record di security
				if(security!=null)
				{
					foreach(System.Data.DataRow row in security.Tables[0].Rows )
					{
						NewVisibilita(destination,row["PERSONORGROUP"].ToString());
					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la copia delle regole di visibilità.", exception);

				result = false;
			}
		
			return result;
		}

		public bool CopyVisibilita2(string source,string destination)
		{
			bool result = true; // Presume successo
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("COPY_VISIBILITA");
				q.setParam("param2",source);
				q.setParam("param1",destination);

				string queryString =q.getSQL();

				logger.Debug(queryString);
				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la copia delle regole di visibilità.", exception);

				result = false;
			}
		
			return result;
		}

		public bool DeleteFunzioni(string idTipo)
		{
			bool result = true; // Presume successo
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_DPA_FUNZIONI");
				q.setParam("param1",idTipo);
				string queryString =q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione della funzioni.", exception);
				//logger.DebugAdm(true,"Errore durante la cancellazione della funzioni.", exception);
				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Inserisce una funzione.
		/// </summary>
		/// <param name="codice"></param>
		/// <param name="descrizione"></param>
		/// <returns></returns>
		public string NewFunzione(string codice, string descrizione, string accesso, string idTipoFunzione)
		{
			string result = null;
			
			switch(accesso)
			{
				case "Read":  accesso = "'R'";  break;
				case "Write": accesso = "'W'";  break;
				default:	  accesso = "NULL"; break;
			}

			try
			{
				//				string queryString = "INSERT INTO DPA_Funzioni (" + Functions.GetSystemIdColName() + " ID_AMM, COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, ID_TIPO_FUNZIONE) VALUES (" +
				//					Functions.GetSystemIdNextVal(null) +
				//					idAmm		   + ", '" +
				//					codice		   + "', '" + 
				//					descrizione	   + "', " + 
				//					accesso		   + ", " + 
				//					idTipoFunzione + ")";

				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_DPA_FUNZIONI");
				q.setParam("param1",Functions.GetSystemIdColName());
				q.setParam("param2",Functions.GetSystemIdNextVal(null));
				//				q.setParam("param3",idAmm);
				q.setParam("param4",codice);
				q.setParam("param5",descrizione);
				q.setParam("param6",accesso);
				q.setParam("param7",idTipoFunzione);
				string queryString =q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//queryString = "SELECT system_id FROM DPA_Funzioni WHERE COD_FUNZIONE='" + codice + "'";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_DPA_FUNZIONI_BY_NAME");
				q.setParam("param1",codice);
				queryString =q.getSQL();
				logger.Debug(queryString);

				this.ExecuteScalar(out result, queryString);	
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'inserimento della funzione.", exception);
				//logger.DebugAdm(true,"Errore durante l'inserimento della funzione.", exception);

				result = null;
			}
		
			return result;
		}

		/// <summary>
		/// Restituisce la system ID gestita dal documentale.
		/// </summary>
		/// <returns></returns>
		public string GetSystemId()
		{
			string result = null;

			try
			{
				//string queryString = "SELECT LASTKEY FROM DOCS_UNIQUE_KEYS WHERE TBNAME='SYSTEMKEY'";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_LASTKEY");
				string queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteScalar(out result, queryString))
				{
					throw new Exception();
				}

				//queryString = "UPDATE DOCS_UNIQUE_KEYS SET LASTKEY=(LASTKEY+1) WHERE TBNAME='SYSTEMKEY'";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_LASTKEY");
				queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'estrazione della system ID.", exception);

				result = null;
			}
		
			return result;
		}

		/// <summary>
		/// Cancella l'intero titolario
		/// </summary>
		/// <returns></returns>
		public bool ClearTitolario()
		{
			bool result = true; // Presume successo
			
			try
			{			
				//string queryString = "SELECT system_id FROM Project WHERE cha_tipo_proj IN ('T', 'F', 'C')";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_NODO_TITOLARIO");
				string queryString=q.getSQL();
				logger.Debug(queryString);

				System.Data.DataSet dataSet;

				if(!this.ExecuteQuery(out dataSet, queryString))
				{
					throw new Exception();
				}
				
				foreach(System.Data.DataRow dataRow in dataSet.Tables[0].Rows)
				{
					//queryString = "DELETE FROM Security WHERE thing=" + dataRow[0];
					q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_SECURITY");
					q.setParam("param1",dataRow[0].ToString());
					queryString=q.getSQL();

					if(!this.ExecuteNonQuery(queryString))
					{
						throw new Exception();
					}
				}

				//queryString = "DELETE FROM Project WHERE cha_tipo_proj IN ('T', 'F', 'C')";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_NODI_TITOLARIO");
				queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione dell'intero titolario.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Cancella gli utenti
		/// </summary>
		/// <returns></returns>
		public bool ClearUtenti()
		{
			bool result = true; // Presume successo
			
			try
			{			
				//string queryString = "DELETE FROM People WHERE NOT (var_nome IS NULL) OR NOT (var_cognome IS NULL)";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_PEOPLE");
				string queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//queryString = "DELETE FROM dpa_corr_globali WHERE cha_tipo_urp='P'";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_DPA_CORR");
				q.setParam("param1","P");
				queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione degli utenti.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Cancella le UO
		/// </summary>
		/// <returns></returns>
		public bool ClearUO()
		{
			bool result = true; // Presume successo
			
			try
			{			
				//string queryString = "DELETE FROM dpa_corr_globali WHERE cha_tipo_urp='U'";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_DPA_CORR");
				q.setParam("param1","U");
				string queryString=q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione delle UO.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Cancella i dati relativi ai ruoli
		/// </summary>
		/// <returns></returns>
		public bool ClearRuoli()
		{
			bool result = true; // Presume successo
			
			try
			{			
				System.Data.DataSet groupList;
				//string queryString = "SELECT DISTINCT id_gruppo FROM DPA_Corr_Globali WHERE cha_tipo_urp='R'";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_DPA_CORR_GRUPPO");
				string queryString =q.getSQL();
				if(!this.ExecuteQuery(out groupList, queryString))
				{
					throw new Exception();
				}

				//queryString = "DELETE FROM DPA_Corr_Globali WHERE cha_tipo_urp='R'";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_DPA_CORR");
				q.setParam("param1","R");
				queryString=q.getSQL();

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//queryString = "DELETE FROM DPA_Tipo_F_Ruolo";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_TABLE");
				q.setParam("param1","DPA_TIPO_F_RUOLO");
				queryString =q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				if(groupList.Tables[0].Rows.Count > 0)
				{
					foreach(System.Data.DataRow row in groupList.Tables[0].Rows)
					{
						//queryString = "DELETE FROM PeopleGroups WHERE groups_system_id=" + row[0];
						q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_PEOPLEGROUPS");
						q.setParam("param1",row[0].ToString());
						queryString=q.getSQL();

						if(!this.ExecuteNonQuery(queryString))
						{
							throw new Exception();
						}

						//queryString = "DELETE FROM Groups WHERE system_id=" + row[0];
						q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_GROUPS");
						q.setParam("param1",row[0].ToString());
						queryString=q.getSQL();

						if(!this.ExecuteNonQuery(queryString))
						{
							throw new Exception();
						}
					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione dei dati relativi ai ruoli.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Cancella i dati relativi alle ragioni trasmissione
		/// </summary>
		/// <returns></returns>
		public bool ClearRagioniTrasmissione()
		{
			bool result = true; // Presume successo
			
			try
			{	
				//string queryString = "DELETE FROM DPA_ragione_trasm";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_TABLE");
				q.setParam("param1","DPA_RAGIONE_TRASM");
				string queryString =q.getSQL();

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione dei dati relativi alle ragioni trasmissione.", exception);

				result = false;
			}
		
			return result;
		}

		public bool ClearServer()
		{
			bool result = true; // Presume successo
			
			try
			{	
				//string queryString = "DELETE FROM DPA_ragione_trasm";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_TABLE");
				q.setParam("param1","DPA_SERVER_POSTA");
				string queryString =q.getSQL();

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione dei dati relativi ai server.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Cancella i ruoli
		/// </summary>
		/// <returns></returns>
		public bool ClearTipiRuolo()
		{
			bool result = true; // Presume successo
			
			try
			{		
				//string queryString = "DELETE FROM DPA_Tipo_Ruolo";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_TABLE");
				q.setParam("param1","DPA_TIPO_RUOLO");
				string queryString =q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione dei ruoli.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Cancella un organigramma
		/// </summary>
		/// <returns></returns>
		public bool ClearOrganigramma()
		{
			bool result = true; // Presume successo
			
			try
			{			
				this.ClearRuoli();
				this.ClearUO();
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione dell'organigramma.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Cancella tutte le amministrazioni
		/// </summary>
		/// <returns></returns>
		public bool ClearAmministrazioni()
		{
			bool result = true; // Presume successo
			
			try
			{			
				//string queryString = "DELETE FROM DPA_Amministra";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_TABLE");
				q.setParam("param1","DPA_AMMINISTRA");
				string queryString =q.getSQL();

				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione delle amministrazioni.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Cancella tutti i registri
		/// </summary>
		/// <returns></returns>
		public bool ClearRegistri()
		{
			bool result = true; // Presume successo
			
			try
			{	
				//string queryString = "DELETE FROM DPA_EL_Registri";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_TABLE");
				q.setParam("param1","DPA_EL_REGISTRI");
				string queryString =q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//queryString = "DELETE FROM DPA_L_RUOLO_REG";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_TABLE");
				q.setParam("param1","DPA_L_RUOLO_REG");
				queryString =q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//queryString = "DELETE FROM DPA_REG_PROTO";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_TABLE");
				q.setParam("param1","DPA_REG_PROTO");
				queryString =q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione dei registri.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Cancella tutti i tipi funzione e le relative funzioni
		/// </summary>
		/// <returns></returns>
		public bool ClearFunzioni()
		{
			bool result = true; // Presume successo
			
			try
			{			
				//string queryString = "DELETE FROM DPA_Funzioni";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_TABLE");
				q.setParam("param1","DPA_FUNZIONI");
				string queryString =q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}

				//queryString = "DELETE FROM DPA_Tipo_Funzione";
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_TABLE");
				q.setParam("param1","DPA_TIPO_FUNZIONE");
				queryString =q.getSQL();
				logger.Debug(queryString);

				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione dei tipi funzione e le relative funzioni.", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Restituisce l'ID del parent della UO passata a parametro.
		/// </summary>
		/// <param name="systemId"></param>
		public string GetUOParent(string systemId)
		{
			string result = null;
		
			try
			{
				//string queryString = "SELECT Id_Parent FROM DPA_Corr_Globali WHERE System_Id=" + systemId;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_UO_PARENT");
				q.setParam("param1",systemId );
				string queryString =q.getSQL();
				logger.Debug(queryString);

				this.ExecuteScalar(out result, queryString);

				if(result.Equals("0") || result.Equals(""))
				{
					result = null;
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'estrazione del parent della UO.", exception);

				result = null;
			}
	
			return result;
		}

		/// <summary>
		///  Restituisce l'ID del parent della UO passata a parametro.
		/// </summary>
		/// <param name="systemId"></param>
		/// <param name="dbProvider"></param>
		/// <returns></returns>
		public string GetUOParent(string systemId, DocsPaDB.DBProvider dbProvider)
		{
			string result = null;
		
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_UO_PARENT");
				q.setParam("param1",systemId );
				string queryString =q.getSQL();
				logger.Debug(queryString);

				dbProvider.ExecuteScalar(out result, queryString);

				if(result.Equals("0") || result.Equals(""))
				{
					result = null;
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'estrazione del parent della UO.", exception);

				result = null;
			}
	
			return result;
		}

		/// <summary>
		/// Restituisce l'ID_UO di un ruolo
		/// </summary>
		/// <param name="systemId"></param>
		public string GetIDUOParent(string systemId)
		{
			string result = null;
		
			try
			{		
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_ID_UO_PARENT");
				q.setParam("param1",systemId );
				string queryString =q.getSQL();
				logger.Debug(queryString);

				this.ExecuteScalar(out result, queryString);

				if(result.Equals("0") || result.Equals(""))
				{
					result = null;
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la GetIDUOParent: ", exception);

				result = null;
			}
	
			return result;
		}

		public string GetRuoloRif(string idRegistro, string idAmm)
		{
			string result = "";
		
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_RUOLO_RIF");
				q.setParam("param1",idRegistro);
				q.setParam("param2",idAmm);
				string queryString =q.getSQL();
				logger.Debug(queryString);

				this.ExecuteScalar(out result, queryString);

				if(result!=null)
				{
					if(result.Equals("0"))
					{
						result = "";
					}
				}
				else
				{
					result="";
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'estrazione del ruolo riferimento.", exception);

				result = "";
			}
	
			return result;
		}

		public string GetUtenteRif(string ruoloRif, string idAmm)
		{
			string result = "";
		
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_UTENTE_RIF");
				q.setParam("param1",ruoloRif);
				q.setParam("param2",idAmm);
				string queryString =q.getSQL();
				logger.Debug(queryString);

				this.ExecuteScalar(out result, queryString);
				if(result!=null)
				{
					if(result.Equals("0") || result.Equals(""))
					{
						result = "";
					}
				}
				else
				{
					result="";
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'estrazione dell'utente di riferimento.", exception);

				result = "";
			}
	
			return result;
		}

		public string GetUOByName(string codice,string idAmm)
		{
			string result = null;
		
			try
			{
				//string queryString = "SELECT Id_Parent FROM DPA_Corr_Globali WHERE System_Id=" + systemId;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_CORR_BY_NAME");
				q.setParam("param1",codice);
				q.setParam("param2",idAmm);
				q.setParam("param3"," AND CHA_TIPO_URP='U'");
				string queryString =q.getSQL();
				logger.Debug(queryString);

				this.ExecuteScalar(out result, queryString);

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dell'id della UO.", exception);

				result = null;
			}
	
			return result;
		}

		public string GetDescUOByName(string codice,string idAmm)
		{
			string result = null;
		
			try
			{				
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_DESC_CORR_BY_NAME");
				q.setParam("param1",codice);
				q.setParam("param2",idAmm);
				q.setParam("param3"," AND CHA_TIPO_URP='U'");
				string queryString =q.getSQL();
				logger.Debug(queryString);

				this.ExecuteScalar(out result, queryString);

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la GetDescUOByName: ", exception);

				result = null;
			}
	
			return result;
		}

		public string GetDescUOByID(string system_id)
		{
			string result = null;
		
			try
			{				
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_DESC_CORR_BY_ID");
				q.setParam("param1",system_id);			
				string queryString =q.getSQL();
				logger.Debug(queryString);

				this.ExecuteScalar(out result, queryString);

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la GetDescUOByID: ", exception);

				result = null;
			}
	
			return result;
		}

		public string GetRuoloUOByName(string codice,string idAmm)
		{
			string result = null;
		
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_CORR_BY_NAME");
				q.setParam("param1",codice);
				q.setParam("param2",idAmm);
				q.setParam("param3"," AND CHA_TIPO_URP='R'");
				string queryString =q.getSQL();
				logger.Debug(queryString);

				this.ExecuteScalar(out result, queryString);

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dell'id del ruolo in UO.", exception);

				result = null;
			}
	
			return result;
		}

		public string GetGroupByName(string codice,string idAmm)
		{
			string result = null;
		
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_GROUP_BY_NAME");
				q.setParam("param1",codice);
				q.setParam("param2",idAmm);
				string queryString =q.getSQL();
				logger.Debug(queryString);

				this.ExecuteScalar(out result, queryString);

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la GetGroupByName: ", exception);

				result = null;
			}
	
			return result;
		}

		/// <summary>
		/// </summary>
		/// <param name="idRuolo"></param>
		/// <returns></returns>
		public DocsPaVO.utente.Ruolo GetRuolo(string idRuolo) 
		{
			DocsPaVO.utente.Ruolo objRuolo = null;
			System.Data.DataSet dataSet = new System.Data.DataSet();
			/*string queryString = "";
			queryString = 
				"SELECT A.PEOPLE_SYSTEM_ID, B.SYSTEM_ID, B.ID_GRUPPO, C.NUM_LIVELLO, B.ID_REGISTRO, " +
				"C.VAR_CODICE, C.VAR_DESC_RUOLO, D.NUM_LIVELLO AS NUM_LIVELLO_UO, B.ID_UO, B.VAR_COD_RUBRICA " +
				"FROM PEOPLEGROUPS A, DPA_CORR_GLOBALI B, DPA_TIPO_RUOLO C, DPA_CORR_GLOBALI D " +
				"WHERE B.ID_GRUPPO=A.GROUPS_SYSTEM_ID AND B.ID_TIPO_RUOLO=C.SYSTEM_ID AND D.SYSTEM_ID=B.ID_UO "+
				"AND B.SYSTEM_ID = "+idRuolo;*/
			string sql = "";
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLEGROUPS__TIPO_RUOLO__CORR_GLOBALI");
			q.setParam("param1", "B.ID_GRUPPO = "+idRuolo);
			sql = q.getSQL();
			logger.Debug(sql);
			this.ExecuteQuery(dataSet,"RUOLI",sql);
			if(dataSet.Tables["RUOLI"].Rows.Count>0) 
			{				
				// cerco il valore di livello più alto per le UO associate all'utente in modo
				// da limitare il dimensione della tabella delle UO.				
				//string maxLivello = dataSet.Tables["RUOLI"].Select("","NUM_LIVELLO_UO desc")[0]["NUM_LIVELLO_UO"].ToString();
				string maxLivello = dataSet.Tables["RUOLI"].Select("","NUM_LIVELLO desc")[0]["NUM_LIVELLO"].ToString();
				logger.Debug("GetRuolo: maxlivello="+maxLivello);
				
				/*queryString =
					"SELECT * FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='U' " +
					"AND CHA_TIPO_IE='I' AND DTA_FINE IS NULL AND NUM_LIVELLO <=" + 
					maxLivello;
				logger.Debug(queryString);
				db.fillTable(queryString,dataSet,"UO");*/
				DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
				utenti.GetRuoloMaxLivello(dataSet,maxLivello,false);
				objRuolo = utenti.GetRuoloData(dataSet,dataSet.Tables["RUOLI"].Rows[0]);
			}

			dataSet.Dispose();
			
			return objRuolo;
		}

		/// <summary>
		/// Leggi la stringa di formattazione della segnatura e della fascicolatura
		/// </summary>
		/// <param name="idAmm"></param>
		/// <returns></returns>
		public bool GetSegnaturaFascicolatura(string idAmm, out string segnatura, out string fascicolatura)
		{
			bool result	  = true; // Presume successo
			segnatura	  = null;
			fascicolatura = null;
		
			try
			{
				System.Data.DataSet dataSet;
				//string queryString = "SELECT VAR_FORMATO_SEGNATURA, VAR_FORMATO_FASCICOLATURA FROM DPA_AMMINISTRA WHERE SYSTEM_ID = " + idAmm;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_SEGNATURA_FASCICOLATURA");
                q.setParam("param1", idAmm);
				string queryString =q.getSQL();
                logger.Debug("param idamm"+idAmm);
				logger.Debug(queryString);

				this.ExecuteQuery(out dataSet, queryString);

                if(dataSet==null)
                    logger.Debug("dataSet GetSegnaturaFascicolatura is NULL");

				if( dataSet.Tables[0].Rows.Count > 0)
				{
					segnatura     = dataSet.Tables[0].Rows[0][0].ToString();	
					fascicolatura = dataSet.Tables[0].Rows[0][1].ToString();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'estrazione delle stringhe segnatura e fascicolatura.", exception);

				result = false;
			}
	
			return result;
		}

		public bool Exp_GetAmministrazioni(out System.Data.DataSet dataSet) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				//string queryString ="SELECT * FROM DPA_AMMINISTRA";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_AMMINISTRAZIONI");
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"AMMINISTRAZIONI",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura delle amministrazioni", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetAmministrazione(out System.Data.DataSet dataSet,string idAmm) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				//string queryString ="SELECT * FROM DPA_AMMINISTRA WHERE SYSTEM_ID=" + idAmm;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_ADMIN_BY_ID");
				q.setParam("param1",idAmm);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"AMMINISTRAZIONE",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura della amministrazione", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetRagioniTrasmissione(out System.Data.DataSet dataSet,string idAmm) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				//string queryString ="SELECT * FROM DPA_RAGIONE_TRASM WHERE ID_AMM=" + idAmm;
				DocsPaUtils.Query q;
				if(idAmm!=null)
				{
					q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_RAGIONE_TRASM");
					q.setParam("param1",idAmm);
				}
				else
				{
					q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_RAGIONE_TRASM_NULL");
				}
				string queryString =q.getSQL();
				
				this.ExecuteQuery(out dataSet,"RAGIONI",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura delle amministrazioni", exception);
				result=false;
			}
			return result;
		}
		
		public bool Exp_GetTipiFunzione(out System.Data.DataSet dataSet) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				//string queryString ="SELECT * FROM DPA_TIPO_FUNZIONE";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_TIPO_FUNZIONE");
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"TIPIFUNZIONE",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dei tipi funzione", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetFunzioniElementari(out System.Data.DataSet dataSet) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				//string queryString ="SELECT * FROM DPA_TIPO_FUNZIONE";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_FUNZIONI_ELEMENTARI");
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"FUNZIONIELEMENTARI",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dei tipi funzione", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetFunzioni(out System.Data.DataSet dataSet,string idTipoFunzione) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				//string queryString ="SELECT * FROM DPA_FUNZIONI WHERE ID_TIPO_FUNZIONE=" + idTipoFunzione;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_FUNZIONI_BY_TIPO");
				q.setParam("param1",idTipoFunzione);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"FUNZIONI",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura delle funzioni", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetRegistri(out System.Data.DataSet dataSet,string idAmm) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				//string queryString ="SELECT * FROM DPA_EL_REGISTRI WHERE ID_AMM=" + idAmm;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_REGISTRI_BY_AMM");
				q.setParam("param1",idAmm);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"REGISTRI",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dei registri", exception);
				result=false;
			}
			return result;
		}

		

		public bool Exp_GetRuoli(out System.Data.DataSet dataSet,string idAmm) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				//string queryString ="SELECT * FROM DPA_TIPO_RUOLO WHERE ID_AMM=" + idAmm + " ORDER BY NUM_LIVELLO ASC";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_RUOLI_BY_AMM");
				q.setParam("param1",idAmm);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"RUOLI",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dei ruoli", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetUtenti(out System.Data.DataSet dataSet,string idAmm) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				//string queryString ="SELECT * FROM PEOPLE,DPA_CORR_GLOBALI WHERE DPA_CORR_GLOBALI.ID_PEOPLE=PEOPLE.SYSTEM_ID AND PEOPLE.ID_AMM=" + idAmm;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_UTENTI_BY_AMM");
				q.setParam("param1",idAmm);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"UTENTI",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura degli utenti", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetUO(out System.Data.DataSet dataSet,string idAmm,string parent) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				//string queryString ="SELECT * FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='U' AND ID_AMM=" + idAmm + " AND ID_PARENT=" + parent;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_UO_BY_AMM");
				q.setParam("param1",idAmm);
				q.setParam("param2",parent);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"UO",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura delle UO", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetRuoliUO(out System.Data.DataSet dataSet,string idAmm,string idUO) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				//string queryString ="SELECT A.SYSTEM_ID AS ID,A.VAR_CODICE AS CODICE,B.VAR_CODICE AS CODICESTRUTTURA, A.VAR_DESC_CORR AS DESCRIZIONE FROM DPA_CORR_GLOBALI A,DPA_TIPO_RUOLO B WHERE B.SYSTEM_ID=A.ID_TIPO_RUOLO AND CHA_TIPO_URP='R' AND A.ID_AMM=" + idAmm + " AND ID_UO=" + idUO;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_RUOLI_UO_BY_AMM");
				q.setParam("param1",idAmm);
				q.setParam("param2",idUO);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"RUOLIUO",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dei ruoli della UO", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetUtentiRuoloUO(out System.Data.DataSet dataSet,string idAmm,string ruolo) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				//string queryString ="SELECT USER_ID FROM GROUPS G, PEOPLEGROUPS PG, PEOPLE P WHERE PG.GROUPS_SYSTEM_ID=G.SYSTEM_ID AND PG.PEOPLE_SYSTEM_ID=P.SYSTEM_ID AND G.GROUP_ID='" + ruolo +"'";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_UTENTI_RUOLO_UO");
				q.setParam("param1",ruolo);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"UTENTIRUOLO",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura degli utenti del ruolo della UO", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetFunzioniRuoloUO(out System.Data.DataSet dataSet,string idAmm,string ruolo) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				//string queryString ="SELECT TR.VAR_COD_TIPO AS CODICE FROM DPA_TIPO_F_RUOLO TF,DPA_TIPO_FUNZIONE TR WHERE TR.SYSTEM_ID=TF.ID_TIPO_FUNZ AND TF.ID_RUOLO_IN_UO=" + ruolo;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_FUNZIONI_RUOLO_UO");
				q.setParam("param1",ruolo);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"FUNZIONIRUOLO",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura delle funzioni del ruolo della UO", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetRegistriRuoloUO(out System.Data.DataSet dataSet,string idAmm,string ruolo) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				//string queryString ="SELECT R.VAR_CODICE AS CODICE FROM DPA_L_RUOLO_REG RR,DPA_EL_REGISTRI R WHERE R.SYSTEM_ID=RR.ID_REGISTRO AND RR.ID_RUOLO_IN_UO=" + ruolo;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_REGISTRI_RUOLO_UO");
				q.setParam("param1",ruolo);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"REGISTRIRUOLO",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura deli registri del ruolo della UO", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetNodiTitolario(out System.Data.DataSet dataSet,string idAmm,string parent,bool soloNodiT) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				//string queryString ="SELECT * FROM PROJECT WHERE ID_AMM=" + idAmm + " AND ID_PARENT=" + parent;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_NODI_TITOLARIO_AMM");
				q.setParam("param1",idAmm);
				q.setParam("param2",parent);
				string tipo="";
				if(soloNodiT)
				{
					tipo=" AND CHA_TIPO_PROJ='T'";
				}
				q.setParam("param3",tipo);
				string queryString =q.getSQL();
				logger.Debug ("Query #1: " + queryString);
				this.ExecuteQuery(out dataSet,"NODITITOLARIO",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dei nodi totalario", exception);
				result=false;
			}
			return result;
		}

		#region GADAMO: ipotesi di gestione diversa del file XML del titolario
		//		/// <summary>
		//		/// TEST GADAMO
		//		/// </summary>
		//		/// <param name="dataSet"></param>
		//		/// <param name="idAmm"></param>
		//		/// <param name="parent"></param>
		//		/// <param name="soloNodiT"></param>
		//		/// <returns></returns>
		//		public bool Exp_GetNodiTitolario_2(out System.Data.DataSet dataSet,string idAmm) 
		//		{
		//			bool result = true; 
		//			dataSet		= null;
		//			try
		//			{
		//				/*string queryString = "
		//				SELECT system_id, description, id_parent 
		//					FROM PROJECT
		//				WHERE cha_tipo_proj = 'T' AND id_amm = $idAmm
		//					START WITH id_parent = 0
		//					CONNECT BY PRIOR system_id = id_parent;
		//				*/
		//				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_NODI_TITOLARIO");
		//				q.setParam("param1",idAmm);
		//				string queryString = q.getSQL();
		//				logger.Debug(queryString);
		//
		//				this.ExecuteQuery(out dataSet,"NODITITOLARIO",queryString);				
		//			}
		//			catch(Exception exception)
		//			{
		//				logger.Debug("Errore durante la lettura dei nodi titalario", exception);
		//				result = false;
		//			}
		//			return result;
		//		}
		#endregion

		public bool Exp_GetIdCorrispondenti(int startPage, int rows, out int totalPages, out System.Data.DataSet dataSet) 
		{
			bool result = true; //presume successo
			dataSet = null;
			totalPages = 0;

			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_ID_CORRISPONDENTI");
				string queryString =q.getSQL();
				int totalRecords = 0;
				this.ExecutePaging(out dataSet, out totalPages, out totalRecords, startPage, rows, queryString, "Corrispondenti");
				//this.ExecuteQuery(out dataSet,"CORRISPONDENTI",queryString);	
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura delle id dei corrispondenti", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetDocumenti(out System.Data.DataSet dataSet, int startPage, int rows, out int totalPages) 
		{
			bool result=true; //presume successo
			dataSet=null;
			totalPages = 0;

			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_DOCUMENTI");
				string queryString =q.getSQL();
				int totalRecords;
				this.ExecutePaging(out dataSet, out totalPages, out totalRecords, startPage, rows, queryString, "DOCUMENTI");
				//				this.ExecuteQuery(out dataSet,"DOCUMENTI",queryString);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dei documenti", exception);
				result=false;
			}

			return result;
		}

		public bool Exp_GetDocumento(out System.Data.DataSet dataSet, string systemId) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_DOCUMENTO");
				q.setParam ("param1", systemId);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"DOCUMENTO",queryString);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura del documento", exception);
				result=false;
			}
			return result;
		}

		/// <summary>
		/// Carica un oggetto 
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="idOggetto"></param>
		/// <returns></returns>
		public bool Exp_GetOggetto(out System.Data.DataSet dataSet, string idOggetto) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_OGGETTO");
				q.setParam("param1",idOggetto);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"OGGETTI",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dei documenti", exception);
				result=false;
			}
			return result;
		}

		/// <summary>
		/// Carica un tipo documento 
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="idOggetto"></param>
		/// <returns></returns>
		public bool Exp_GetTipoDocumento(out System.Data.DataSet dataSet, string idTipoDocumento) 
		{
			bool result=true; //presume successo
			dataSet=null;

			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_TIPO_DOCUMENTO");
				q.setParam("param1", idTipoDocumento);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet, queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura del tipo documento", exception);
				result=false;
			}
			return result;
		}

		/// <summary>
		/// Carica un tipo atto 
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="idOggetto"></param>
		/// <returns></returns>
		public bool Exp_GetTipoAtto(out System.Data.DataSet dataSet, string idTipoAtto) 
		{
			bool result=true; //presume successo
			dataSet=null;

			try
			{
				//string queryString = "SELECT SYSTEM_ID FROM DPA_TIPO_ATTO WHERE SYSTEM_ID=" + idTipoAtto;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_TIPO_ATTO");
				q.setParam("param1", idTipoAtto);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet, queryString);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura del tipo atto", exception);
				result=false;
			}
			return result;
		}

		/// <summary>
		/// Acquisisce i tipi atto
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="idOggetto"></param>
		/// <returns></returns>
		public bool Exp_GetTipiAtto(out System.Data.DataSet dataSet) 
		{
			bool result=true; //presume successo
			dataSet=null;

			try
			{
				//string queryString ="SELECT var_desc_atto FROM DPA_TIPO_ATTO";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_TIPI_ATTO");
				string queryString = q.getSQL();
				this.ExecuteQuery(out dataSet, queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dei tipi atto", exception);
				result=false;
			}
			return result;
		}

		/// <summary>
		/// Acquisisce i tipi documento
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="idOggetto"></param>
		/// <returns></returns>
		public bool Exp_GetTipiDocumento(out System.Data.DataSet dataSet) 
		{
			bool result=true; // Presume successo
			dataSet=null;

			try
			{
				//string queryString = "SELECT VAR_CODICE, VAR_DESCRIZIONE, CHA_TIPO_CANALE FROM DPA_TIPIDOCUMENTO";
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_TIPI_DOCUMENTO");
				string queryString = q.getSQL();
				this.ExecuteQuery(out dataSet, queryString);				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dei tipi documento", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetCorrispondentiDocumento(out System.Data.DataSet dataSet, string idProfile) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_CORRISPONDENTI_DOCUMENTO");
				q.setParam("param1",idProfile);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"CORRISPONDENTI",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dei corrispondenti", exception);
				result=false;
			}
			return result;
		}

		public int Exp_CountCorrispondenti() 
		{
			int result = 0;

			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_COUNT_CORRISPONDENTI");
				string queryString =q.getSQL();
				string countResult;
				this.ExecuteScalar(out countResult, queryString);
				result = Int32.Parse(countResult);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la conta dei corrispondenti", exception);
				result = -1;
			}

			return result;
		}

		public int Exp_CountDocumenti() 
		{
			int result = 0;

			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_COUNT_DOCUMENTI");
				string queryString = q.getSQL();
				string countResult;
				this.ExecuteScalar(out countResult, queryString);
				result = Int32.Parse(countResult);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la conta dei documenti", exception);
				result = -1;
			}

			return result;
		}

		/// <summary>
		/// Legge i dati relativi ad un corrispondente
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="idCorr"></param>
		/// <returns></returns>
		public bool Exp_GetCorrispondente(out System.Data.DataSet dataSet, string idCorr) 
		{
			bool result=true; //presume successo
			dataSet=null;

			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_CORRISPONDENTE");
				q.setParam("param1", idCorr);
				string queryString = q.getSQL();
				this.ExecuteQuery(out dataSet, queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura del corrispondente", exception);
				result = false;
			}

			return result;
		}

		public string Exp_GetCodiceCorrispondente(string idCorrispondente) 
		{
			string result="";
			try
			{
				//string queryString ="SELECT * FROM PROJECT WHERE ID_AMM=" + idAmm + " AND ID_PARENT=" + parent;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_CODICE_CORRISPONDENTE");
				q.setParam("param1",idCorrispondente);
				string queryString =q.getSQL();
				this.ExecuteScalar(out result,queryString);
				if (result==null || result=="")
				{
					result=idCorrispondente;
				}
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura del codice corrispondente", exception);
				result="";
			}
			return result;
		}

		public string Exp_GetCodicePeople(string idPeople) 
		{
			string result="";
			try
			{
				//string queryString ="SELECT * FROM PROJECT WHERE ID_AMM=" + idAmm + " AND ID_PARENT=" + parent;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_CODICE_PEOPLE");
				q.setParam("param1",idPeople);
				string queryString =q.getSQL();
				this.ExecuteScalar(out result,queryString);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura del codice utente (people)", exception);
				result="";
			}
			return result;
		}

		public string Exp_GetCodiceRuoloUO(string idRuolo) 
		{
			string result="";
			try
			{
				//string queryString ="SELECT * FROM PROJECT WHERE ID_AMM=" + idAmm + " AND ID_PARENT=" + parent;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_CODICE_RUOLO_UO");
				q.setParam("param1",idRuolo);
				string queryString =q.getSQL();
				this.ExecuteScalar(out result,queryString);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura del codice ruolo (DPA_CORR_GLOBALI)", exception);
				result="";
			}
			return result;
		}

		public bool Exp_GetParoleDocumento(out System.Data.DataSet dataSet,string idProfile) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_PAROLE_DOCUMENTO");
				q.setParam("param1",idProfile);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"PAROLE",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura delle parole", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetVersioniDocumento(out System.Data.DataSet dataSet,string docNumber) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_VERSIONI_DOCUMENTO");
				q.setParam("param1",docNumber);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"VERSIONI", queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura delle versioni", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetTrasmissioniDocumento(out System.Data.DataSet dataSet,string idProfile) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_TRASMISSIONI_DOCUMENTO");
				q.setParam("param1",idProfile);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"TRASMISSIONI",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura delle trasmissioni", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetTrasmissioniSingole(out System.Data.DataSet dataSet,string idTrasmissione) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_TRASMISSIONI_SINGOLE");
				q.setParam("param1",idTrasmissione);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"TRASMISSIONISINGOLE",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura delle trasmissioni singole", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetTrasmissioniUtente(out System.Data.DataSet dataSet,string idTrasmissioneSingola) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_TRASMISSIONI_UTENTE");
				q.setParam("param1",idTrasmissioneSingola);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"TRASMISSIONIUTENTE",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura delle trasmissioni singole", exception);
				result=false;
			}
			return result;
		}

		public string Exp_GetRagioneTrasm(string idRagione) 
		{
			string result="";
			try
			{
				//string queryString ="SELECT * FROM PROJECT WHERE ID_AMM=" + idAmm + " AND ID_PARENT=" + parent;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_DESC_RAGIONE_TRASM");
				q.setParam("param1",idRagione);
				string queryString =q.getSQL();
				this.ExecuteScalar(out result,queryString);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura della ragione trasmissione", exception);
				result="";
			}
			return result;
		}

		public bool Exp_GetVisibilitaUtente(out System.Data.DataSet dataSet,string idDocumento) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_VISIBILITA_DOC_UTENTE");
				q.setParam("param1",idDocumento);
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"VISIBILITAUTENTE",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura delle visibilita utente", exception);
				result=false;
			}
			return result;
		}

		public bool Exp_GetVisibilitaRuolo(out System.Data.DataSet dataSet,string idDocumento) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_VISIBILITA_DOC_RUOLO");
				q.setParam("param1",idDocumento);
				string queryString =q.getSQL();
				logger.Debug ("Query #3: " + queryString);
				this.ExecuteQuery(out dataSet,"VISIBILITARUOLO",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura delle visibilita ruolo", exception);
				result=false;
			}
			return result;
		}

		#region GADAMO: ipotesi di gestione diversa del file XML del titolario
		//		public bool Exp_GetVisibilitaRuoloReg(out System.Data.DataSet dataSet) 
		//		{
		//			bool result = true; 
		//			dataSet = null;
		//			try
		//			{
		//				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_VISIBILITA_DOC_RUOLO_REG");
		//				string queryString = q.getSQL();
		//				this.ExecuteQuery(out dataSet,"VISIBILITARUOLO",queryString);				
		//			}
		//			catch(Exception exception)
		//			{
		//				logger.Debug("Errore durante la lettura delle visibilita ruolo reg", exception);
		//				result=false;
		//			}
		//			return result;
		//		}
		#endregion

		public string Exp_GetRegistroByID(string idRegistro) 
		{
			string result="";
			try
			{
				if(idRegistro!=null && idRegistro!="")
				{
					DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_REGISTRO_BY_ID");
					q.setParam("param1",idRegistro);
					string queryString =q.getSQL();
					logger.Debug("Query #2: " + queryString);
					this.ExecuteScalar(out result,queryString);
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura del registro", exception);
				result="";
			}
			return result;
		}

		public bool Exp_GetServerPosta(out System.Data.DataSet dataSet) 
		{
			bool result=true; //presume successo
			dataSet=null;
			try
			{
				//string queryString ="SELECT * FROM PROJECT WHERE ID_AMM=" + idAmm + " AND ID_PARENT=" + parent;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("EXP_GET_SERVER_POSTA");
				string queryString =q.getSQL();
				this.ExecuteQuery(out dataSet,"SERVERPOSTA",queryString);
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dei server di posta", exception);
				result=false;
			}
			return result;
		}

		/// <summary>
		/// funzione che torna true se il codice 'code' è univoco nel campo 'column' della
		/// tabella 'table'
		/// </summary>
		/// <param name="table"></param>
		/// <param name="column"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		public bool CheckUniqueCode(string table,string column,string code,string condition) 
		{
			/*
			 SELECT COUNT(*) AS TOT FROM @param1@ WHERE @param2@='@param3@' @param4@ 
			 */
		
			bool result=true; //presume successo
			try
			{				
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("CHECK_UNIQUE_CODE");
				q.setParam("param1",table);
				q.setParam("param2",column);
				q.setParam("param3",code);
				q.setParam("param4",condition);
				string queryString =q.getSQL();
				logger.Debug (queryString);
				string valore;
				this.ExecuteScalar(out valore,queryString);
				if(valore!="0")
				{
					result=false;
				}
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la verifica del codice univoco", exception);
				result=false;
			}
			return result;
		}

		/// <summary>
		/// funzione che torna true se il codice 'code' è univoco nel campo 'column' della
		/// tabella 'table'
		/// </summary>
		/// <param name="table"></param>
		/// <param name="column"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		public bool CheckUniqueCode(string table,string column,string code,string condition, DocsPaDB.DBProvider dbProvider) 
		{
			/*
			 SELECT COUNT(*) AS TOT FROM @param1@ WHERE @param2@='@param3@' @param4@ 
			 */
		
			bool result=true; //presume successo
			try
			{				
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("CHECK_UNIQUE_CODE");
				q.setParam("param1",table);
				q.setParam("param2",column);
				q.setParam("param3",code);
				q.setParam("param4",condition);
				string queryString =q.getSQL();
				logger.Debug (queryString);
				string valore;
				dbProvider.ExecuteScalar(out valore,queryString);
				if(valore!="0")
				{
					result=false;
				}
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la verifica del codice univoco", exception);
				result=false;
			}
			return result;
		}

		/// <summary>
		/// funzione che torna true se il codice 'code' è univoco nel campo 'column' della
		/// tabella 'table'
		/// </summary>
		/// <param name="table"></param>
		/// <param name="column"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		public bool CheckCountCondition(string table,string condition) 
		{
			/*
			 SELECT COUNT(*) AS TOT FROM @param1@ WHERE @param2@
			 */
		
			bool result=true; //presume successo
			try
			{				
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("CHECK_COUNT_CONDITION");
				q.setParam("param1",table);
				q.setParam("param2",condition);
				string queryString =q.getSQL();
				logger.Debug (queryString);
				string valore;
				this.ExecuteScalar(out valore,queryString);
				if(valore!="0")
				{
					result=false;
				}
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore nel metodo: CheckCountCondition - ", exception);
				result=false;
			}
			return result;
		}

		public bool ChangeAdminPwd(string user,string password)
		{
			bool result=true;
			try
			{
				//string queryString ="SELECT * FROM PROJECT WHERE ID_AMM=" + idAmm + " AND ID_PARENT=" + parent;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_ADMIN_PWD");
				q.setParam("param1",password);
				q.setParam("param2",user);
				string queryString =q.getSQL();
				logger.Debug(queryString);
				result=this.ExecuteNonQuery(queryString);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante il cambio password dell'amministratore", exception);
				result=false;
			}
			return result;
		}

		public bool CheckAdminLogin(string user,string password)
		{
			bool result=false;
			IDataReader dr = null;

			try
			{				
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("CHECK_ADMIN_LOGIN");
				q.setParam("param1",user);
				q.setParam("param2",password);

				dr = (IDataReader) this.ExecuteReader(q.getSQL());
				if(dr == null) 
					throw new Exception();
				
				if (dr.Read()) 
				{
					object o = dr[0];
					result = (Convert.ToDecimal(o) != 0);
				}
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la verifica dell'utente amministratore", exception);
				result=false;
			}
			finally
			{
				if (dr != null && (!dr.IsClosed))
					dr.Close();
			}
			return result;
		}

		/// <summary>
		/// Verifica dell'amministratore corrente. Se l'amministratore non è già collegato
		/// oppure è lo stesso utente che si vuole collegare, restituisce il nome dell'utente
		/// corrente. In caso contrario restituisce il nome dell'utente collegato come
		/// amministratore
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public string CheckUniqueAdmin(string userId)
		{
			string result=null;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_ADMIN_LOGIN");
				string queryString =q.getSQL();
				string valore;
				this.ExecuteScalar(out valore,queryString);
				if(valore!=null)
				{
					valore=valore.TrimEnd().ToUpper();
					//controlla se amministratore = utente corrente
					string current="AMMINISTRAZIONE/" + userId.ToUpper();
					result=userId;
					if(current!=valore)
					{
						//altro amministratore, torna il nome dell'utente
						int indice=valore.IndexOf("/");
						if(indice>0 && (indice+1)<valore.Length)
						{
							result=valore.Substring(indice+1);
						}
					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la verifica dell'utente amministratore", exception);
			}
			return result;
		}

		
		/*public bool CheckUniqueAdmin(string userId)
		{
			bool result=false;
			try
			{
				//string queryString ="SELECT * FROM PROJECT WHERE ID_AMM=" + idAmm + " AND ID_PARENT=" + parent;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("CHECK_UNIQUE_ADMIN");
				q.setParam("param1","= 'AMMINISTRAZIONE/" + userId + "'");
				string queryString =q.getSQL();
				string valore;
				this.ExecuteScalar(out valore,queryString);
				if(valore!="0")
				{
					result=false;
				}
				else
				{
					q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("CHECK_UNIQUE_ADMIN");
					q.setParam("param1"," LIKE 'AMMINISTRAZIONE/%'" );
					queryString =q.getSQL();
					this.ExecuteScalar(out valore,queryString);
					if(valore!="0")
					{
						result=true;
					}
				}
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la verifica dell'utente amministratore", exception);
				result=false;
			}
			return result;
		}*/

		public bool DeleteUniqueAdmin()
		{
			bool result=true;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_UNIQUE_ADMIN");
				string queryString =q.getSQL();
				logger.Debug(queryString);
				this.ExecuteNonQuery(queryString);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante lo sblocco dell'amministratore", exception);
				result=false;
			}
			return result;
		}

		public bool SetUniqueAdmin(string userId)
		{
			bool result=true;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("INSERT_UNIQUE_ADMIN");
				q.setParam("param1","AMMINISTRAZIONE/" + userId);
				q.setParam("param2","1");
				string queryString =q.getSQL();
				this.ExecuteNonQuery(queryString);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante il blocco dell'amministratore", exception);
				result=false;
			}
			return result;
		}

		public string GetTipoAtto(string codice)
		{
			string result = null;
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_TIPO_ATTO");
				q.setParam("param1",codice);
				string queryString =q.getSQL();
				logger.Debug(queryString);
				this.ExecuteScalar(out result, queryString);

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura del tipo atto.", exception);

				result = null;
			}
		
			return result;
		}

		public string GetTipoDocumento(string codice)
		{
			string result = null;
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_TIPO_DOCUMENTO");
				q.setParam("param1",codice);
				string queryString =q.getSQL();
				logger.Debug(queryString);
				this.ExecuteScalar(out result, queryString);

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura del tipo docuemnto.", exception);

				result = null;
			}
		
			return result;
		}
		
		/// <summary>
		/// Ritorna i dati necessari alla connessione in scrittura al documentale
		/// </summary>
		/// <param name="codice"></param>
		/// <returns></returns>
		public bool GetTitolarioAccessData(string amm, out string utente, out string password, out string ruolo)
		{
			bool result = true;
			System.Data.DataSet dataSet = null;
			string queryString = null;
			utente	 = null;
			password = null;
			ruolo	 = null;
			
			try
			{
				string adminId = this.GetAdminByName(amm);
				string userId = null;
				string roleId = null;

				// Ritorna la lista di utenti/ruoli								
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_UTENTI_RUOLI");
				q.setParam("param1",adminId);
				queryString = q.getSQL();
				if(!this.ExecuteQuery(out dataSet, queryString)) throw new Exception();

				if(dataSet.Tables[0].Rows.Count > 0)
				{
					roleId = dataSet.Tables[0].Rows[0]["GROUP_ID"].ToString();
					userId = dataSet.Tables[0].Rows[0]["USER_ID"].ToString();
				}

				// Ritorna l'utente e la sua password
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_UTENTE");
				q.setParam("param1", userId);
				queryString =q.getSQL();
				//				logger.Debug(queryString);
				if(!this.ExecuteQuery(out dataSet, queryString)) throw new Exception();

				if(dataSet.Tables[0].Rows.Count > 0)
				{
					utente   = dataSet.Tables[0].Rows[0]["NAME"].ToString();
					password = dataSet.Tables[0].Rows[0]["PASSWORD"].ToString();
				}

				// Ritorna il ruolo
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_RUOLO");
				q.setParam("param1", roleId);
				queryString =q.getSQL();
				//				logger.Debug(queryString);
				if(!this.ExecuteScalar(out ruolo, queryString)) throw new Exception();
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura del tipo documento:\n" + queryString, exception);

				result = false;
			}
		
			return result;
		}

		public bool CheckVisibilitaUtente(string peopleId)
		{
			bool result = false; 
			System.Data.DataSet documenti;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("CHECK_VISIBILITA_UTENTE");
				q.setParam("param1",peopleId);
				string queryString=q.getSQL();
				logger.Debug(queryString);
				//ottiene la lista dei documenti visibili all'utente
				if(this.ExecuteQuery(out documenti,queryString))
				{
					//letti i documenti
					//verifica che i documenti abbiano visibilità anche da altri utenti
					//conteggia per ogni documento i record di visibila con accessrigths>0
					//se il primo record contiene un valore 1 o minore, esiste almeno un documento
					//per il quale c'è la visibilità del solo utente passato
					if(documenti!=null)
					{
						if(documenti.Tables[0].Rows.Count>0)
						{
							int valore=Int32.Parse(documenti.Tables[0].Rows[0]["TOT"].ToString());
							if(valore<=1)
							{
								result=true;
							}
						}
					}
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura delle visibilità private dell'utente.", exception);

				result = false;
			}
		
			return result;
		}

		public bool CheckADLUtente(string peopleId)
		{
			bool result = false; 
			System.Data.DataSet adl;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("CHECK_ADL_UTENTE");
				q.setParam("param1",peopleId);
				string queryString=q.getSQL();
				logger.Debug(queryString);
				//ottiene la lista dei documenti visibili all'utente
				if(this.ExecuteQuery(out adl,queryString))
				{
					if(adl!=null)
					{
						if(adl.Tables[0].Rows.Count>0)
						{
							int valore=Int32.Parse(adl.Tables[0].Rows[0]["TOT"].ToString());
							if(valore>0)
							{
								result=true;
							}
						}
					}
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dell'area di lavoro dell'utente.", exception);

				result = false;
			}
		
			return result;
		}

		public bool CheckTODOLISTUtente(string peopleId)
		{
			bool result = false; 
			System.Data.DataSet tdl;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("CHECK_TODOLIST_UTENTE2");
				q.setParam("param1",peopleId);
				string queryString=q.getSQL();
				logger.Debug(queryString);
				
				if(this.ExecuteQuery(out tdl,queryString))
				{
					if(tdl!=null)
					{
						if(tdl.Tables[0].Rows.Count>0)
						{
							int valore=Int32.Parse(tdl.Tables[0].Rows[0]["TOT"].ToString());
							if(valore>0)
							{
								logger.Debug("Trovate " + valore + " trasmissioni per PEOPLE.SYSTEM_ID = " + peopleId);
								result=true;
							}
						}
					}
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura della todo list dell'utente.", exception);

				result = false;
			}
		
			return result;
		}

		public bool CheckTODOLISTUtente(string peopleId, string ruoloId)
		{
			bool result = false; 
			System.Data.DataSet tdl;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("CHECK_TODOLIST_UTENTE");
				q.setParam("param1",peopleId);
				q.setParam("param2",ruoloId);
				string queryString=q.getSQL();
				logger.Debug(queryString);
			
				if(this.ExecuteQuery(out tdl,queryString))
				{
					if(tdl!=null)
					{
						if(tdl.Tables[0].Rows.Count>0)
						{
							int valore=Int32.Parse(tdl.Tables[0].Rows[0]["TOT"].ToString());
							if(valore>0)
							{
								result=true;
							}
						}
					}
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura della todo list dell'utente.", exception);

				result = false;
			}
		
			return result;
		}

		public bool CheckTrasmissioniUtente(string peopleId)
		{
			bool result = false; 
			System.Data.DataSet trasmissioni;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("CHECK_TRASMISSIONI_UTENTE");
				q.setParam("param1",peopleId);
				string queryString=q.getSQL();
				logger.Debug(queryString);
				//ottiene la lista dei documenti visibili all'utente
				if(this.ExecuteQuery(out trasmissioni,queryString))
				{
					if(trasmissioni!=null)
					{
						if(trasmissioni.Tables[0].Rows.Count>0)
						{
							int valore=Int32.Parse(trasmissioni.Tables[0].Rows[0]["TOT"].ToString());
							if(valore>0)
							{
								result=true;
							}
						}
					}
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura della todo list dell'utente.", exception);

				result = false;
			}
		
			return result;
		}

		public bool DeleteUtenteInRuoloUO(string codUtente, string codAmm, string codNewUtente, string codRuolo)
		{
			bool result = true;
			
			try
			{
				string idAmm = GetAdminByName(codAmm);

				string idUtente = GetUserByNameAndIdAmm(codUtente,idAmm);

				string idNewUtente = GetUserByNameAndIdAmm(codNewUtente,idAmm);

				string idRuolo = GetGroupByName(codRuolo,idAmm);

				// sposta la ToDoList ad un altro utente
				if(!UpdateTodoListUtente(idUtente,idNewUtente,idRuolo))
				{
					throw new Exception();					
				}

				// elimina il record sulla peoplegroups
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_PEOPLEGROUPS_FROM_IDPEOPLE_IDRUOLO");
				q.setParam("param1",idUtente);
				q.setParam("param2",idRuolo);
				string queryString = q.getSQL();
				logger.Debug(queryString);
				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();					
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore nella DeleteUtenteInRuoloUO: ", exception);
				result = false;
			}
			return result;
		}

		public bool DeleteOrDisablePeople(string codUtente, string codAmm, bool delete)
		{
			bool result = true;
			
			try
			{
				string idAmm = GetAdminByName(codAmm);

				string idUtente = GetUserByNameAndIdAmm(codUtente,idAmm);				

				if(delete)
				{
					// elimina fisicamente l'utente
					DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_SINGLE_PEOPLE");
					q.setParam("param1","SYSTEM_ID = " + idUtente);					
					string queryString = q.getSQL();
					logger.Debug(queryString);
					if(!this.ExecuteNonQuery(queryString))
					{
						throw new Exception();					
					}
				}
				else
				{	
					// disabilita l'utente
					DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DISABLE_PEOPLE_BY_ID");
					q.setParam("param1",idUtente);					
					string queryString = q.getSQL();
					logger.Debug(queryString);
					if(!this.ExecuteNonQuery(queryString))
					{
						throw new Exception();					
					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore nella DeleteOrDisablePeople: ", exception);
				result = false;
			}
			return result;
		}

		public bool MoveUserFromRole(string codUtente, string codAmm, string codOldRole, string codNewRole, string codUtenteErede)
		{
			bool result = true;
			try
			{
				string idAmm = GetAdminByName(codAmm);

				string idUtente = GetUserByNameAndIdAmm(codUtente,idAmm);

				string idOldRuolo = GetGroupByName(codOldRole,idAmm);

				string idNewRuolo = GetGroupByName(codNewRole,idAmm);

				if(codUtenteErede != "" || codUtenteErede != null)
				{
					string idUtenteErede = GetUserByNameAndIdAmm(codUtenteErede,idAmm);

					// sposta la ToDoList ad un altro utente
					if(!UpdateTodoListUtente(idUtente,idUtenteErede,idOldRuolo))
					{
						throw new Exception();					
					}
				}

				// elimina il record sulla peoplegroups
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("DELETE_PEOPLEGROUPS_FROM_IDPEOPLE_IDRUOLO");
				q.setParam("param1",idUtente);
				q.setParam("param2",idOldRuolo);
				string queryString = q.getSQL();
				logger.Debug(queryString);
				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();					
				}

				// lo lega al nuovo ruolo
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("NEW_UTENTE_RUOLO");
				q.setParam("param1",idNewRuolo);
				q.setParam("param2",idUtente);
				queryString = q.getSQL();
				logger.Debug(queryString);
				if(!this.ExecuteNonQuery(queryString))
				{
					throw new Exception();					
				}

			}
			catch(Exception exception)
			{
				logger.Debug("Errore nella DeleteUtenteInRuoloUO: ", exception);
				result = false;
			}
			return result;
		}

		/// <summary>
		/// sposta un ruolo da una uo ad un'altra
		/// </summary>
		/// <param name="codRuolo">codice del ruolo da spostare</param>
		/// <param name="codAmm">codice Amm.ne</param>
		/// <param name="codNewUO">codice della nuova UO nella quale deve essere spostato il ruolo</param>
		/// <returns>bool</returns>
		public bool MoveRoleToNewUO(string codRuolo, string codAmm, string codNewUO, string descNewUO, string codTipoRuolo, string descTipoRuolo, string codNewRuolo, string descNewRuolo)
		{
			bool result = true;

			try
			{
				//prende la system_id dell'amm.ne
				//SELECT system_id FROM DPA_AMMINISTRA WHERE UPPER(var_codice_amm)=UPPER('$codAmm')
				string idAmm = GetAdminByName(codAmm);
				logger.Debug("idAmm="+idAmm);
				if(idAmm == null)
				{
					throw new Exception();					
				}

				//prende la system_id del ruolo nella DPA_CORR_GLOBALI
				//SELECT system_id FROM DPA_Corr_Globali WHERE UPPER(VAR_CODICE)=UPPER('$codRuolo') AND ID_AMM=$idAmm AND CHA_TIPO_URP='R'
				string idRuolo = GetRuoloUOByName(codRuolo,idAmm);
				logger.Debug("idRuolo="+idRuolo);
				if(idRuolo == null)
				{
					throw new Exception();					
				}

				//prende la system_id del tipo ruolo
				//select id_tipo_ruolo from DPA_CORR_GLOBALI where system_id = $idRuolo
//				string idTipoRuolo = GetIDTipoRuoloByIDCorr(idRuolo);
//				logger.Debug("idTipoRuolo="+idTipoRuolo);
//				if(idTipoRuolo == null)
//				{
//					throw new Exception();					
//				}


				//				//prende var_codice da DPA_TIPO_RUOLO
				//				//select var_codice from DPA_TIPO_RUOLO where system_id = $idTipoRuolo
				//				string codTipoRuolo = GetCodTipoRuoloByID(idTipoRuolo);
				//				logger.Debug("codTipoRuolo="+codTipoRuolo);
				//				if(codTipoRuolo == null)
				//				{
				//					throw new Exception();					
				//				}

				//				//prende var_desc_ruolo da DPA_TIPO_RUOLO
				//				//select var_codice from DPA_TIPO_RUOLO where system_id = $idTipoRuolo
				//				string descTipoRuolo = GetDescTipoRuoloByID(idTipoRuolo);
				//				logger.Debug("descTipoRuolo="+descTipoRuolo);
				//				if(descTipoRuolo == null)
				//				{
				//					throw new Exception();					
				//				}
				
				//prende la system_id dalla GROUPS 
				//select system_id from Groups where UPPER(Group_Id)=UPPER('$codRuolo')
				string idGruppo = GetGroupByName(codRuolo);
				logger.Debug("idGruppo="+idGruppo);
				if(idGruppo == null)
				{
					throw new Exception();					
				}				

				//prende la system_id della della DPA_CORR_GLOBALI della nuova UO
				//select system_id FROM DPA_Corr_Globali where UPPER(VAR_CODICE) = UPPER('$codNewUO') AND ID_AMM = $idAmm AND CHA_TIPO_URP='U'
				string idNewUO = GetUOByName(codNewUO,idAmm);
				logger.Debug("idNewUO="+idNewUO);
				if(idNewUO == null)
				{
					throw new Exception();					
				}

				//				//prende la var_desc_corr della DPA_CORR_GLOBALI
				//				//SELECT var_desc_corr FROM DPA_Corr_Globali WHERE system_id = $idNewUO
				//				string descNewUO = GetDescUOByID(idNewUO);
				//				logger.Debug("descNewUO="+descNewUO);
				//				if(descNewUO == null)
				//				{
				//					throw new Exception();					
				//				}				
				
				//modifica la GROUPS		
				if(!UpdateSpostaRuoloGroups(codNewRuolo,descNewRuolo,idGruppo))
				{
					throw new Exception();					
				}

				//modifica la DPA_CORR_GLOBALI			
				if(!UpdateSpostaRuoloCorrGlob(codNewRuolo,descNewRuolo,idNewUO,idRuolo))
				{
					throw new Exception();					
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore nella MoveRoleToNewUO: ", exception);
				result = false;
			}

			return result;
		}

		#region gestione TITOLARIO diretto al db (gadamo)

		/// <summary>
		/// 
		/// </summary>
		/// <param name="codAmm"></param>
		/// <param name="idParent"></param>
		/// <returns></returns>
		public System.Data.DataSet Nodo_Titolario(string codAmm, string idParent)
		{
			System.Data.DataSet ds;
			DocsPaUtils.Query q;
			string Sql ="";			
			string idAmm;

			try
			{			
				AmministrazioneXml amm = new AmministrazioneXml();
				idAmm = amm.GetAdminByName(codAmm);
			
				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_NODO_TITOLARIO");		
				q.setParam("param1",idAmm);
				q.setParam("param2",idParent);

				Sql = q.getSQL();	
				logger.Debug("Nodo_Titolario - sql: " + Sql);

				if (!this.ExecuteQuery(out ds, "QUERY", Sql))
				{
					throw new Exception();
				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message.ToString(),ex);
				ds = null;
			}

			return ds;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="idParent"></param>
		/// <param name="idGruppo"></param>
		/// <returns></returns>
		public System.Data.DataSet Nodo_Titolario_Security(string idAmm, string idParent, string idGruppo, string idRegistro, string idTitolario)
		{
			System.Data.DataSet ds;
			DocsPaUtils.Query q;
			string Sql ="";		

			try
			{
                if (idParent == null || idParent == "0")
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_NODO_TITOLARIO_SECURITY_2");
                    q.setParam("idAmm", idAmm);
                    q.setParam("systemIdTit", idTitolario);
                }
                else
                {
                    //modifica SABRINA x gestione DEPOSITO
                    DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
                    if (ut.isUtArchivistaDeposito(null, idGruppo))
                    {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_NODO_TITOLARIO_DEPOSITO");
                    }
                    else
                    {
                         q = DocsPaUtils.InitQuery.getInstance().getQuery("S_NODO_TITOLARIO_SECURITY");
                    }
                    q.setParam("param1", idAmm);
                    q.setParam("param2", idParent);
                    q.setParam("param3", idGruppo);
                    q.setParam("param4", idRegistro);
                }

				Sql = q.getSQL();	
				logger.Debug("Nodo_Titolario_Security - sql: " + Sql);

				if (!this.ExecuteQuery(out ds, "QUERY", Sql))
				{
					throw new Exception();
				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message.ToString(),ex);
				ds = null;
			}

			return ds;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="codAmm"></param>
		/// <returns></returns>
		public System.Data.DataSet RegistriInAmministrazione(string codAmm)
		{
			System.Data.DataSet ds;
			DocsPaUtils.Query q;
			string Sql ="";			
			string idAmm;

			try
			{			
				AmministrazioneXml amm = new AmministrazioneXml();
				idAmm = amm.GetAdminByName(codAmm);
			
				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REGISTRI_IN_AMMINISTRAZIONE");		
				q.setParam("param1",idAmm);
				
				Sql = q.getSQL();	
				logger.Debug("RegistriInAmministrazione - sql: " + Sql);

				if (!this.ExecuteQuery(out ds, "QUERY", Sql))
				{
					throw new Exception();
				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message.ToString(),ex);
				ds = null;
			}

			return ds;
		}

		public System.Data.DataSet RegistriInAmministrazioneRestricted(string sessionID)
		{
			System.Data.DataSet ds;
			DocsPaUtils.Query q;
			string Sql ="";			

			try
			{							
				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_REGISTRI_IN_AMM_RESTRICTED");		
				q.setParam("param1",sessionID);
				
				Sql = q.getSQL();	
				logger.Debug("RegistriInAmministrazione [RESTRICTED AREA] - sql: " + Sql);

				if (!this.ExecuteQuery(out ds, "QUERY", Sql))
				{
					throw new Exception();
				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message.ToString(),ex);
				ds = null;
			}

			return ds;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="idNodo"></param>
		/// <param name="codAmm"></param>
		/// <returns></returns>
		public System.Data.DataSet securityNodo_Ruoli(string idNodo, string codAmm)
		{
			System.Data.DataSet ds;
			DocsPaUtils.Query q;
			string Sql ="";			
			string idAmm;

			try
			{			
				AmministrazioneXml amm = new AmministrazioneXml();
				idAmm = amm.GetAdminByName(codAmm);
			
				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_VISIBILITA_NODO_RUOLI");
				if(idNodo == null || idNodo == "")
					idNodo = "NULL";
				q.setParam("param1",idNodo);
				q.setParam("param2",idAmm);
				
				Sql = q.getSQL();	
				logger.Debug("securityNodo_Ruoli - sql: " + Sql);

				if (!this.ExecuteQuery(out ds, "QUERY", Sql))
				{
					throw new Exception();
				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message.ToString(),ex);
				ds = null;
			}

			return ds;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cosaModificare"></param>
		/// <param name="num_blocco"></param>
		/// <returns></returns>
		private bool ToBeUpd(string cosaModificare, string num_blocco)
		{
			bool result = false;
			char[] underscore={'_'};
			string[] cosa = cosaModificare.Split(underscore);
			for(int i=0;i<cosa.Length;i++)
			{
				if(cosa[i].Equals(num_blocco))
					return true;
			}
			return result;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="codice"></param>
		/// <param name="descrizione"></param>
		/// <param name="idAmm"></param>
		/// <returns></returns>
		public System.Data.DataSet filtroRicTitolario(string codice, string descrizione, string idAmm)
		{
			System.Data.DataSet ds;
			DocsPaUtils.Query q;
			string Sql ="";			

			try
			{			
				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FILTRO_RIC_TIT");	

				if(codice != null && codice != "")
					q.setParam("param1","AND UPPER(var_codice) = UPPER('"+codice+"')");
				if(descrizione != null && descrizione != "")
					q.setParam("param2","AND UPPER(description) LIKE UPPER('%"+descrizione+"%')");
				q.setParam("param3",idAmm);
				
				Sql = q.getSQL();	
				logger.Debug("filtroRicTitolario - sql: " + Sql);

				if (!this.ExecuteQuery(out ds, "QUERY", Sql))
				{
					throw new Exception();
				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message.ToString(),ex);
				ds = null;
			}

			return ds;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="codice"></param>
		/// <param name="descrizione"></param>
		/// <param name="codAmm"></param>
		/// <returns></returns>
		public System.Data.DataSet filtroRicTitolarioAmm(string codice, string descrizione, string codAmm, string idRegistro)
		{
			System.Data.DataSet ds;
			DocsPaUtils.Query q;
			string Sql ="";		
			string idAmm;

			try
			{			
				AmministrazioneXml amm = new AmministrazioneXml();
				idAmm = amm.GetAdminByName(codAmm);

				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FILTRO_RIC_TIT");	

				if(codice != null && codice != "")
					q.setParam("param1","AND UPPER(var_codice) = UPPER('"+codice+"')");
				if(descrizione != null && descrizione != "")
					q.setParam("param2","AND UPPER(description) LIKE UPPER('%"+descrizione+"%')");
				q.setParam("param3",idAmm);

				if(idRegistro==null)
					idRegistro=string.Empty;

				if(idRegistro!=string.Empty)
				{
					q.setParam("param4","AND id_registro = " + idRegistro);					
				}
				else
				{
					q.setParam("param4","AND id_registro IS NULL");
				}

				Sql = q.getSQL();	
				logger.Debug("filtroRicTitolarioAmm - sql: " + Sql);

				if (!this.ExecuteQuery(out ds, "QUERY", Sql))
				{
					throw new Exception();
				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message.ToString(),ex);
				ds = null;
			}

			return ds;
		}

		public System.Data.DataSet cercaNodoCod(string codClass, string idAmm, string idReg, string idTitolario)
		{
			System.Data.DataSet ds;
			DocsPaUtils.Query q;
			string Sql ="";			

			try
			{							
				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_NODO_BY_COD");	

				q.setParam("param1",codClass);
				q.setParam("param2",idAmm);
				q.setParam("param3",idReg);
                q.setParam("param4", idTitolario);
				Sql = q.getSQL();	
				logger.Debug("cercaNodoCod - sql: " + Sql);

				if (!this.ExecuteQuery(out ds, "QUERY", Sql))
				{
					throw new Exception();
				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message.ToString(),ex);
				ds = null;
			}

			return ds;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="idrecord"></param>
		/// <returns></returns>
		public System.Data.DataSet ricercaRoot(string idrecord)
		{
			System.Data.DataSet ds;
			DocsPaUtils.Query q;
			string Sql ="";				

			try
			{			
				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_NODO_PADRE");	

				q.setParam("param1",idrecord);
				
				Sql = q.getSQL();	
				logger.Debug("ricercaRoot - sql: " + Sql);

				if (!this.ExecuteQuery(out ds, "QUERY", Sql))
				{
					throw new Exception();
				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message.ToString(),ex);
				ds = null;
			}

			return ds;
		}

		/// <summary>
		/// Inserisce un nuovo nodo di titolario
		/// </summary>
		/// <param name="padre"></param>
		/// <param name="codice"></param>
		/// <param name="descrizione"></param>
		/// <param name="idregistro"></param>
		/// <param name="livello"></param>
		/// <param name="codAmm"></param>
		/// <param name="codliv"></param>
		/// <param name="R_W"></param>
		/// <param name="ruoliTitolario"></param>
		/// <returns></returns>
		public string insertNodoTit(
									string padre, 
									string codice, 
									string descrizione, 
									string idregistro, 
									string livello, 
									string codAmm,
									string codliv,
									string R_W)
		{
			string ID_T; // system_id Titolario
			string ID_F; // system_id Fascicolo Generale
			string ID_C; // system_id Root Folder

			string valore;
			string result = "0";
			DocsPaUtils.Query q;
			string Sql ="";	
			
			if(idregistro == null || idregistro == "") idregistro = "NULL"; // tutti i registri
			if(padre == null || padre == "") padre = "0";					// nodo sotto la Root: id_parent=0

			try
			{			
				AmministrazioneXml amm = new AmministrazioneXml();
				string idAmm = amm.GetAdminByName(codAmm);

				// verifica che il codice sia univoco
				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_TOT_CODICE_TIT");		
				q.setParam("param1",codice);
				q.setParam("param2",idAmm);
			
				Sql = q.getSQL();	
				logger.Debug("AddNewNodo - sql 1: " + Sql);
				valore = null;
				if(!this.ExecuteScalar(out valore, Sql))
				{
					throw new Exception();					
				}
				else
				{
					if(valore != "0") return "1";
				}

				//INSERT su Project
				/* Utilizzo la generazione di un numero RANDOM per l'identificazione 
				 * univoca del record all'interno della tabella project.
				 */
				System.Random random = new System.Random();
				long randomized = 1000000000 + random.Next(10000000);

				// insert titolario -------------------------------------------------------------------------
				q = DocsPaUtils.InitQuery.getInstance().getQuery("I_NODO_TITOLARIO");					
				q.setParam("param1", Functions.GetSystemIdColName());				
				q.setParam("param2", Functions.GetSystemIdNextVal(null));
				q.setParam("param3", "'"+DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione)+"'");
				q.setParam("param4", "'Y'");
				q.setParam("param5", "'T'");
				q.setParam("param6", "'"+codice+"'");
				q.setParam("param7", idAmm);
				q.setParam("param8", idregistro);
				q.setParam("param9", livello);
				q.setParam("param10", "NULL");
				q.setParam("param11", padre);
				q.setParam("param12", "'"+codliv+"'");
				q.setParam("param13", Convert.ToString(randomized));
				q.setParam("param14", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")));
				q.setParam("param15", "NULL");
				q.setParam("param16", "NULL");
				q.setParam("param17", "'"+R_W+"'");
				
				Sql = q.getSQL();
				logger.Debug("AddNewNodo - sql 2: " + Sql);
				if(!this.ExecuteNonQuery(Sql)) throw new Exception();
				
				// query per prendere la system_id titolario --------------------------------------------------
				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project0");	
				q.setParam("param1", "SYSTEM_ID");				
				q.setParam("param2", "WHERE ETDOC_RANDOM_ID = " + randomized);
				
				Sql = q.getSQL();
				logger.Debug("AddNewNodo - sql 3: " + Sql);
				if(!this.ExecuteScalar(out ID_T, Sql)) throw new Exception();

				// insert fascicolo generale ------------------------------------------------------------------
				q = DocsPaUtils.InitQuery.getInstance().getQuery("I_NODO_TITOLARIO");					
				q.setParam("param1", Functions.GetSystemIdColName());				
				q.setParam("param2", Functions.GetSystemIdNextVal(null));
				q.setParam("param3", "'"+DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione)+"'");
				q.setParam("param4", "'Y'");
				q.setParam("param5", "'F'");
				q.setParam("param6", "'"+codice+"'");
				q.setParam("param7", idAmm);
				q.setParam("param8", idregistro);
				q.setParam("param9", "NULL");
				q.setParam("param10", "'G'");
				q.setParam("param11", ID_T);
				q.setParam("param12", "NULL");
				q.setParam("param13", Convert.ToString(randomized));
				q.setParam("param14", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")));
				q.setParam("param15", "'A'");
				q.setParam("param16", "NULL");
				q.setParam("param17", "'"+R_W+"'"); //vedere se il concetto di r/w va essteso anche  a  F

				Sql = q.getSQL();
				logger.Debug("AddNewNodo - sql 4: " + Sql);
				if(!this.ExecuteNonQuery(Sql)) throw new Exception();

				// query per prendere la system_id fascicolo generale -----------------------------------------
				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project0");	
				q.setParam("param1", "SYSTEM_ID");				
				q.setParam("param2", "WHERE ETDOC_RANDOM_ID = " + randomized + " and cha_tipo_proj = 'F'");
				
				Sql = q.getSQL();
				logger.Debug("AddNewNodo - sql 5: " + Sql);
				if(!this.ExecuteScalar(out ID_F, Sql)) throw new Exception();

				// insert Root Folder -------------------------------------------------------------------------
				q = DocsPaUtils.InitQuery.getInstance().getQuery("I_NODO_TITOLARIO");					
				q.setParam("param1", Functions.GetSystemIdColName());				
				q.setParam("param2", Functions.GetSystemIdNextVal(null));
				q.setParam("param3", "'Root Folder'");
				q.setParam("param4", "'Y'");
				q.setParam("param5", "'C'");
				q.setParam("param6", "NULL");
				q.setParam("param7", idAmm);
				q.setParam("param8", "NULL");
				q.setParam("param9", "NULL");
				q.setParam("param10", "NULL");
				q.setParam("param11", ID_F);
				q.setParam("param12", "NULL");
				q.setParam("param13", Convert.ToString(randomized));
				q.setParam("param14", DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")));
				q.setParam("param15", "NULL");
				q.setParam("param16", ID_F);
				q.setParam("param17", "'"+R_W+"'"); //vedere se il concetto di r/w va essteso anche  a  C

				Sql = q.getSQL();
				logger.Debug("AddNewNodo - sql 6: " + Sql);
				if(!this.ExecuteNonQuery(Sql)) throw new Exception();

				// query per prendere la system_id Root Folder ------------------------------------------------
				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project0");	
				q.setParam("param1", "SYSTEM_ID");				
				q.setParam("param2", "WHERE ETDOC_RANDOM_ID = " + randomized + " and cha_tipo_proj = 'C'");
				
				Sql = q.getSQL();
				logger.Debug("AddNewNodo - sql 7: " + Sql);
				if(!this.ExecuteScalar(out ID_C, Sql)) throw new Exception();

				// pulisce la ETDOC_RANDOM_ID -----------------------------------------------------------------
				q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECTRANDOM");					
				q.setParam("param1", randomized.ToString());

				Sql = q.getSQL();
				logger.Debug("AddNewNodo - sql 8: " + Sql);
				if(!this.ExecuteNonQuery(Sql)) throw new Exception();

				// inserisce la visibilità dei ruoli in UO ----------------------------------------------------

//				ArrayList listaNodi = new ArrayList();
//				listaNodi.Add(ID_T);
//				listaNodi.Add(ID_F);
//				listaNodi.Add(ID_C);
//				IEnumerator myEnumerator = listaNodi.GetEnumerator();
//
//				XmlDocument doc = new XmlDocument();
//				doc.LoadXml(streamRuoli);
//			
//				XmlNode lista = doc.SelectSingleNode("NewDataSet");
//				if(lista.ChildNodes.Count > 0)
//				{										
//					while (myEnumerator.MoveNext())
//					{					
//						foreach (XmlNode nodo in lista.ChildNodes)
//						{					
//							q = DocsPaUtils.InitQuery.getInstance().getQuery("I_VISIBILITA_NODO_RUOLI");
//							q.setParam("param1", myEnumerator.Current.ToString());
//							q.setParam("param2", nodo.SelectSingleNode("idRuolo").InnerText);
//
//							Sql = q.getSQL();
//							logger.Debug("AddNewNodo - sql: " + Sql);
//							if(!this.ExecuteNonQuery(Sql)) throw new Exception();					
//						}
//					}
//				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message.ToString(),ex);
				result = "9";
			}

			return result;
		}




		/// <summary>
		/// Elimina nodo titolario
		/// </summary>
		/// <param name="idrecord"></param>
		/// <returns></returns>
		public string delNodo(string idrecord)
		{
			string result = "0";
			DocsPaUtils.Query q;
			string Sql = "";	
			string valore = null;
			
			try
			{
				// cerca fascicoli procedimentali
				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONTA_FASC_PROC_TIT");		
				q.setParam("param1",idrecord);				
			
				Sql = q.getSQL();	
				logger.Debug("delNodo - sql 1: " + Sql);
				valore = null;
				if(!this.ExecuteScalar(out valore, Sql))
				{
					throw new Exception();					
				}
				else
				{
					if(valore != "0") return "1";
				}

				// cerca documenti in root folder
				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONTA_DOC_IN_ROOT");		
				q.setParam("param1",idrecord);				
			
				Sql = q.getSQL();	
				logger.Debug("delNodo - sql 2: " + Sql);
				valore = null;
				if(!this.ExecuteScalar(out valore, Sql))
				{
					throw new Exception();					
				}
				else
				{
					if(valore != "0") return "2";
				}

				// elimina i record su security
				q = DocsPaUtils.InitQuery.getInstance().getQuery("D_SECURITY_NODO");
				q.setParam("param1",idrecord);
			
				Sql = q.getSQL();	
				logger.Debug("delNodo - sql 3: " + Sql);

				if (!this.ExecuteNonQuery(Sql))
				{
					throw new Exception();
				}

				// elimina i record su project
				q = DocsPaUtils.InitQuery.getInstance().getQuery("D_NODO_TITOLARIO");
				q.setParam("param1",idrecord);
			
				Sql = q.getSQL();	
				logger.Debug("delNodo - sql 4: " + Sql);

				if (!this.ExecuteNonQuery(Sql))
				{
					throw new Exception();
				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message.ToString(),ex);
				result = "9";
			}

			return result;
		}

		public string getCodiceLivello(string codliv, string livello, string codAmm, string idTitolario, string idRegistro)
		{
			string result;			

			try
			{			
				AmministrazioneXml amm = new AmministrazioneXml();
				string idAmm = amm.GetAdminByName(codAmm);

				DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COD_LIV");	

				q.setParam("param1",codliv);
				q.setParam("param2",idAmm);
				q.setParam("param3",livello);
                if (idTitolario != null && idTitolario != "")
                    q.setParam("param4", " AND ID_TITOLARIO = " + idTitolario);
                else
                    q.setParam("param4", "");

               /* if(idRegistro != null && idRegistro != "")
                    q.setParam("param5", " AND ID_REGISTRO = " + idRegistro); 
                else
                    q.setParam("param5", "");
                */

				string Sql = q.getSQL();	
				logger.Debug("getCodiceLivello - sql: " + Sql);

				if(!this.ExecuteScalar(out result, Sql))
				{
					throw new Exception();
				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message.ToString(),ex);
				result = "0";
			}

			return result;
		}

		public string moveNodoTitolario(string currentCodLiv, string newCodLiv, string codAmm, string idRegistro)
		{
			string result = "0";	
			string ID_current;
			string ID_new;
			int lenght_currentCodLiv = currentCodLiv.Length + 1;
			int lenght_newCodLiv = newCodLiv.Length + 1;

			try
			{			
				AmministrazioneXml amm = new AmministrazioneXml();
				string idAmm = amm.GetAdminByName(codAmm);

				string gestRegistro = string.Empty;
				if(idRegistro==null || idRegistro.Equals(string.Empty) || idRegistro.Equals(""))
				{
					gestRegistro = "IS NULL";
				}
				else
				{
					gestRegistro = "= " + idRegistro;
				}

				// prende la system_id del currentCodLiv
				DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project0");	
				q.setParam("param1", "SYSTEM_ID");				
				q.setParam("param2", "WHERE VAR_COD_LIV1 = '" + currentCodLiv + "' AND ID_REGISTRO " + gestRegistro + " AND CHA_TIPO_PROJ = 'T' AND ID_AMM = " + idAmm);
				
				string Sql = q.getSQL();
				logger.Debug("moveNodoTitolario - sql 1: " + Sql);
				if(!this.ExecuteScalar(out ID_current, Sql)) throw new Exception();

				// prende la system_id del newCodLiv
				q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project0");	
				q.setParam("param1", "SYSTEM_ID");				
				q.setParam("param2", "WHERE VAR_COD_LIV1 = '" + newCodLiv + "' AND ID_REGISTRO " + gestRegistro + " AND CHA_TIPO_PROJ = 'T' AND ID_AMM = " + idAmm);
				
				Sql = q.getSQL();
				logger.Debug("moveNodoTitolario - sql 2: " + Sql);
				if(!this.ExecuteScalar(out ID_new, Sql)) throw new Exception();

				if(ID_new != null)
				{
					// update del currentCodLiv ---------------------------------------------------------------
					q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT_CODLIV1");
					q.setParam("param1",newCodLiv);
					q.setParam("param2",ID_current);
			
					Sql = q.getSQL();	
					logger.Debug("moveNodoTitolario - sql 3: " + Sql);

					if (!this.ExecuteNonQuery(Sql))
					{
						throw new Exception();
					}

					// update dei figli
					q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT_CODLIV2");
					q.setParam("param1","'" + newCodLiv + "'" + DocsPaDbManagement.Functions.Functions.ConcatStr() + DocsPaDbManagement.Functions.Functions.SubStr("VAR_COD_LIV1",lenght_newCodLiv.ToString(),"32") + ", ETDOC_RANDOM_ID = 9999");
					q.setParam("param2",currentCodLiv);
					q.setParam("param3",idAmm);
					q.setParam("param4","AND ID_REGISTRO " + gestRegistro);
			
					Sql = q.getSQL();	
					logger.Debug("moveNodoTitolario - sql 4: " + Sql);

					if (!this.ExecuteNonQuery(Sql))
					{
						throw new Exception();
					}


					// update del newCodLiv -------------------------------------------------------------------
					q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT_CODLIV1");
					q.setParam("param1",currentCodLiv);
					q.setParam("param2",ID_new);
			
					Sql = q.getSQL();	
					logger.Debug("moveNodoTitolario - sql 5: " + Sql);

					if (!this.ExecuteNonQuery(Sql))
					{
						throw new Exception();
					}

					// update dei figli
					q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT_CODLIV2");
					q.setParam("param1","'" + currentCodLiv + "'" + DocsPaDbManagement.Functions.Functions.ConcatStr() + DocsPaDbManagement.Functions.Functions.SubStr("VAR_COD_LIV1",lenght_currentCodLiv.ToString(),"32"));
					q.setParam("param2",newCodLiv);
					q.setParam("param3",idAmm);
					q.setParam("param4","AND ID_REGISTRO " + gestRegistro + " AND ETDOC_RANDOM_ID NOT IN (9999) AND SYSTEM_ID NOT IN ("+ID_current+")");
			
					Sql = q.getSQL();	
					logger.Debug("moveNodoTitolario - sql 6: " + Sql);

					if (!this.ExecuteNonQuery(Sql))
					{
						throw new Exception();
					}

					// update della etdoc_random_id
					q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT_CODLIV3");
					q.setParam("param1","etdoc_random_id = 0");
					q.setParam("param2","etdoc_random_id = 9999");
					
					Sql = q.getSQL();	
					logger.Debug("moveNodoTitolario - sql 7: " + Sql);

					if (!this.ExecuteNonQuery(Sql))
					{
						throw new Exception();
					}
				}
			}
			catch (Exception ex)
			{
				logger.Debug(ex.Message.ToString(),ex);
				result = "9";
			}

			return result;
		}


		



		#endregion
	}
}

