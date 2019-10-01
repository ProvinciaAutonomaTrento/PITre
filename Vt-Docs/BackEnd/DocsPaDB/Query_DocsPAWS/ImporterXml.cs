using System;
using DocsPaDbManagement.Functions;
using log4net;
using System.Data;

namespace DocsPaDB.Query_DocsPAWS
{
	/// <summary>
	/// Classe per l'amministrazione via XML
	/// </summary>
	public class ImporterXml : DBProvider
	{
        private ILog logger = LogManager.GetLogger(typeof(ImporterXml));
		//
		//da cancellare
		static public int cont;
		
		/// <summary>
		/// 
		/// </summary>
		public ImporterXml(string connectionString)
		{
			this.InstantiateConnection(connectionString);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="corrispondente"></param>
		/// <returns></returns>
		public bool Imp_NewCorrispondente(DocsPaDB.Query_DocsPAWS.Strutture.Corrispondente corrispondente)
		{
			bool result = true; // Presume successo
			
			try
			{
				string smtpPort = "NULL";
				if(corrispondente.portaSmtp != null && corrispondente.portaSmtp != "") smtpPort = corrispondente.portaSmtp;
				if(corrispondente.idRegistro == null) corrispondente.idRegistro = "NULL";
				if(corrispondente.idAmm == null)	  corrispondente.idAmm = "NULL";
				if(corrispondente.numLivello == null || corrispondente.numLivello == "") corrispondente.numLivello = "NULL";
				if(corrispondente.idParent == null || corrispondente.idParent == "")	  corrispondente.idParent = "NULL";


				//inserisce record nella tabella DPA_CORR_GLOBALI
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_CORRISPONDENTE");
				q.setParam("param1",  DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
				q.setParam("param2",  DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
				q.setParam("param3",  corrispondente.idRegistro);
				q.setParam("param4",  corrispondente.codRubrica);
				q.setParam("param5",  DocsPaUtils.Functions.Functions.ReplaceApexes(corrispondente.descrizione));
				q.setParam("param6",  Functions.ToDate(corrispondente.dataInizio));
				q.setParam("param7",  Functions.ToDate(corrispondente.dataFine));
				q.setParam("param8",  corrispondente.codice);
				q.setParam("param9",  DocsPaUtils.Functions.Functions.ReplaceApexes(corrispondente.cognome));
				q.setParam("param10", DocsPaUtils.Functions.Functions.ReplaceApexes(corrispondente.nome));
				q.setParam("param11", corrispondente.tipoCorr);
				q.setParam("param12", corrispondente.tipoIE);
				q.setParam("param13", corrispondente.codiceAOO);
				q.setParam("param14", corrispondente.pa);
				q.setParam("param15", corrispondente.email);
				q.setParam("param16", corrispondente.smtp);
				q.setParam("param17", smtpPort);
				q.setParam("param18", corrispondente.idAmm);
				q.setParam("param19", corrispondente.tipoURP);
				q.setParam("param20", corrispondente.numLivello);
				q.setParam("param21", corrispondente.idParent);
				string command = q.getSQL();
				logger.Debug(command);
				
				if (!this.ExecuteNonQuery(command)) throw new Exception();



				// Legge l'ID del record appena creato ricercandolo per codice o codice rubrica
				string codiceUtilizzato = null;
				string campoUtilizzato  = null;

				if(corrispondente.codice != null && corrispondente.codice != "")
				{
					campoUtilizzato = "VAR_CODICE";
					codiceUtilizzato = corrispondente.codice;
				}
				else
				{
					campoUtilizzato = "VAR_COD_RUBRICA";
					codiceUtilizzato = corrispondente.codRubrica;
				}

				if(codiceUtilizzato != null && codiceUtilizzato != "")
				{
                    q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_CORR_BY_CODE");
                    q.setParam("param1", campoUtilizzato);
                    q.setParam("param2", "'" + codiceUtilizzato + "'");
                    command = q.getSQL();
                    string systemId = String.Empty;
                    logger.Debug(command);

                    using (DBProvider dbProvider = new DBProvider())
                    {
                        using (IDataReader reader = dbProvider.ExecuteReader(command))
                        {
                            while (reader.Read())
                            {
                                // System_ID  DPA_EL_REGISTRI
                                systemId = reader[0].ToString();
                            }
                        }
                    }

                    //if (!this.ExecuteScalar(out systemId,command)) throw new Exception();

                    //inserisce il record nella tabella DPA_DETT_GLOBALI
                    string test = corrispondente.cap + 
						corrispondente.citta +
						corrispondente.codFiscale + 
						corrispondente.fax +
						corrispondente.indirizzo + 
						corrispondente.nazione + 
						corrispondente.note + 
						corrispondente.telefono + 
						corrispondente.telefono2 + 
						corrispondente.provincia; 

					if(test != "")
					{
						q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_CORR_DETTAGLI");
						q.setParam("param1",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
						q.setParam("param2",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
						q.setParam("param3",systemId);
						q.setParam("param4",DocsPaUtils.Functions.Functions.ReplaceApexes(corrispondente.indirizzo));
						q.setParam("param5",DocsPaUtils.Functions.Functions.ReplaceApexes(corrispondente.citta));
						q.setParam("param6",corrispondente.cap );
						q.setParam("param7",DocsPaUtils.Functions.Functions.ReplaceApexes(corrispondente.provincia));
						q.setParam("param8",DocsPaUtils.Functions.Functions.ReplaceApexes(corrispondente.nazione));
						q.setParam("param9",corrispondente.codFiscale );
						q.setParam("param10",corrispondente.telefono );
						q.setParam("param11",corrispondente.telefono2 );
						q.setParam("param12",corrispondente.fax );
						q.setParam("param13",DocsPaUtils.Functions.Functions.ReplaceApexes(corrispondente.note));
						command = q.getSQL();
						logger.Debug(command);
					
						if(!this.ExecuteNonQuery(command)) throw new Exception();					

						// Update corrispondenti
						q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("UPDATE_DPA_CORR_GLOBALI");
						q.setParam("param1", "1");
						q.setParam("param2", systemId);
						command = q.getSQL();
						logger.Debug(command);

						if(!this.ExecuteNonQuery(command)) throw new Exception();					
					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione nuovo corrispondente", exception);

				result = false;
			}
		

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="codice"></param>
		/// <param name="descrizione"></param>
		/// <param name="tipo"></param>
		/// <returns></returns>
		public bool ImportOggettario(string descrizione, string amministrazione, string registro)
		{
			bool result = true; // Presume successo
			
			try
			{
				if(descrizione == null)		descrizione = "";
				if(amministrazione == null) amministrazione = "NULL";
				if(registro == null)		registro = "NULL";

				if(descrizione.Length > 250) descrizione.Substring(0, 250);

				//inserisce record nella tabella DPA_OGGETTARIO
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_OGGETTO");
				q.setParam("param1",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
				q.setParam("param2",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));					
				q.setParam("param3", registro);
				q.setParam("param4", amministrazione);
				q.setParam("param5", DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione));
				q.setParam("param6", "0");
				string command = q.getSQL();
				logger.Debug(command);
				
				if(!this.ExecuteNonQuery(command)) throw new Exception();
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione nuovo tipo documento", exception);

				result = false;
			}
		
			return result;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="data_modifica"></param>
		/// <param name="documento"></param>
		/// <param name="oggetto"></param>
		/// <param name="utente"></param>
		/// <param name="rubrica"></param>
		/// <returns></returns>
		public bool ImportStoricoOggettario(string data_modifica, string documento, 
														string oggetto, string utente, string rubrica )
		{
			
			try
			{
				if(data_modifica == null)	data_modifica ="NULL";
				if(documento == null)		documento = "NULL";
				if(oggetto == null)			oggetto = "NULL";
				if(utente == null)			utente = "NULL";
				if(rubrica == null)			rubrica = "NULL";


				//	  **** Inserimento nello storico dell'oggettario
				//----------------------------------------------------------------------------------------------------------
				string res1="";
				string res2="";
				string res3="";
				string res4="";
				
				if ( !ExistsLinkWith("GET_ID_DOCUMENTO", documento, out res1) ||
						!ExistsLinkWith("GET_ID_UTENTE", utente.ToUpper(), out res2) ||
							!ExistsLinkWith("GET_ID_RUOLO_IN_UO", rubrica.ToUpper(), out res3) ||
								!ExistsLinkWith("GET_ID_OGGETTO", 
									DocsPaUtils.Functions.Functions.ReplaceApexes(oggetto).ToUpper(), out res4) )
					
					return false;
					
				else
				{
					//inserisce nello storico dell'oggettario
					DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("SET_OGGETTO_STORICO");
					q.setParam("param1",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
					q.setParam("param2",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
						
					q.setParam("param3",  DocsPaDbManagement.Functions.Functions.ToDate(data_modifica));
					q.setParam("param4",  res1);
					q.setParam("param5",	 DocsPaUtils.Functions.Functions.ReplaceApexes(res4));
					q.setParam("param6",  res2);
					q.setParam("param7",  res3);
					
					string sCommand = q.getSQL();
					
					if(!this.ExecuteNonQuery(sCommand)) throw new Exception();
					logger.Debug(sCommand);
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione nuovo tipo documento", exception);
				
				return false;
			}
		
			return true;
		}
		
		
		public bool Imp_NewTipoDocumento(string codice,string descrizione,string tipo)
		{
			bool result = true; // Presume successo
			
			try
			{
				//inserisce record nella tabella DPA_CORR_GLOBALI
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_TIPODOCUMENTO");
				q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
				q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
				q.setParam("param3",codice);
				q.setParam("param4", DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione));
				q.setParam("param5",tipo);
				string command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteNonQuery(command))
				{
					throw new Exception();
				}
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione nuovo tipo documento", exception);

				result = false;
			}
		
			return result;
		}

		public bool Imp_NewTipoAtto(string codice)
		{
			bool result = true; // Presume successo
			
			try
			{
				//inserisce record nella tabella DPA_CORR_GLOBALI
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_TIPOATTO");
				q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
				q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
				q.setParam("param3", DocsPaUtils.Functions.Functions.ReplaceApexes(codice));
				string command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteNonQuery(command))
				{
					throw new Exception();
				}				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione nuovo tipo atto", exception);

				result = false;
			}
		
			return result;
		}

		public string Imp_GetUserSystemId(string userId)
		{
			string systemId = null; // Presume successo
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_ID_PEOPLE");
				q.setParam("param1",userId);
				string command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteScalar(out systemId, command))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura della tabella people", exception);
			}
			return systemId;
		}

		public string Imp_GetCorrispondenteByCodRubrica(string codRubrica)
		{
			string systemId = null; // Presume successo
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_ID_CORR_BY_COD_RUBRICA");
				q.setParam("param1",codRubrica);
				string command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteScalar(out systemId, command))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura della tabella corrispondenti", exception);
			}
			return systemId;
		}

		public string Imp_GetIdFascicolo(string codice,string idAmministrazione)
		{
			string systemId = null; // Presume successo
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_PROJECT_BY_CODE");
				q.setParam("param1",codice);
				q.setParam("param2",idAmministrazione);
				q.setParam("param3"," AND CHA_TIPO_PROJ='F'");
				string command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteScalar(out systemId, command))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura della tabella project", exception);
			}
			return systemId;
		}

		public string Imp_GetIdRootFolder(string idFascicolo)
		{
			string systemId = null; // Presume successo
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_ROOT_FOLDER");
				q.setParam("param1",idFascicolo);
				string command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteScalar(out systemId, command))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura della tabella project", exception);
			}
			return systemId;
		}

		public string Imp_GetIdPeopleByCodRubrica(string codRubrica)
		{
			string peopleId = null; // Presume successo
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_ID_PEOPLE_BY_COD_RUBRICA");
				q.setParam("param1",codRubrica);
				string command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteScalar(out peopleId, command))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura della tabella corrispondenti", exception);
			}
			return peopleId;
		}

		public string Imp_GetIdGroupByCodRubrica(string codRubrica)
		{
			string groupId = null; // Presume successo
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_ID_GROUP_BY_COD_RUBRICA");
				q.setParam("param1",codRubrica);
				string command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteScalar(out groupId, command))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura della tabella corrispondenti", exception);
			}
			return groupId;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="peoples"></param>
		/// <param name="idGruppo"></param>
		/// <returns></returns>
		public bool Imp_GetPeopleInGroup(out System.Data.DataSet peoples, string idGruppo)
		{
			bool result=true;
			peoples=null;
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_PEOPLE_IN_GROUP");
				q.setParam("param1",idGruppo);
				string command = q.getSQL();
				logger.Debug(command);
				
				if(!this.ExecuteQuery(out peoples,"PEOPLES", command))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura degli utenti appartenenti al gruppo", exception);
				result=false;
			}
			return result;
		}

		public string Imp_NewDocumento(DocsPaDB.Query_DocsPAWS.Strutture.Documento documento)
		{
			string result = null;
			
			try
			{
				//inserisce l'oggetto
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_OGGETTO");
				q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
				q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
				if(documento.oggetto.registro==null)
				{
					q.setParam("param3","NULL");
				}
				else
				{
					q.setParam("param3",documento.oggetto.registro);
				}
				if(documento.oggetto.amministrazione ==null)
				{
					q.setParam("param4","NULL");
				}
				else
				{
					q.setParam("param4",documento.oggetto.amministrazione);
				}
				q.setParam("param5",DocsPaUtils.Functions.Functions.ReplaceApexes(documento.oggetto.descrizione));
				q.setParam("param6","1");
				string command = q.getSQL();
				logger.Debug(command);
				
				if(!this.ExecuteNonQuery(command))
				{
					throw new Exception();
				}

				//legge l'id dell'oggetto creato
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_OGGETTO");
				q.setParam("param1",DocsPaUtils.Functions.Functions.ReplaceApexes(documento.oggetto.descrizione));
				command = q.getSQL();
				logger.Debug(command);
				string idOggetto;
				
				if(!this.ExecuteScalar(out idOggetto,command))
				{
					throw new Exception();
				}

				//inserisce record nella tabella PROFILE
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_DOCUMENTO");
				q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
				q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
				q.setParam("param3",documento.numero );
				q.setParam("param4",documento.tipoProto );
				q.setParam("param5",documento.img );
				q.setParam("param6",documento.invioConferma );
				q.setParam("param7",documento.congelato );
				q.setParam("param8",documento.consolidato );
				q.setParam("param9",documento.privato );
				q.setParam("param10",documento.assegnato );
				q.setParam("param11",documento.fascicolato );
				q.setParam("param12",documento.tipo );
				q.setParam("param19",documento.predispostoProto);
				
				if(documento.tipoAtto != null)
				{
					q.setParam("param13",documento.tipoAtto);
				}
				else
				{
					q.setParam("param13", "NULL");
				}

				q.setParam("param14",Functions.ToDate(documento.dataCreazione));
				q.setParam("param15",idOggetto);
				q.setParam("param16",documento.note );

				string additionalFields = "";
				string additionalValues = "";

				if(documento.protocollo.numero != null && documento.protocollo.numero != "")
				{
					// Acquisizione ID registro
					DocsPaUtils.Query regQuery = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_REG_BY_NAME");
					regQuery.setParam("param1",documento.protocollo.registro);
					command = regQuery.getSQL();
					logger.Debug(command);
					string idRegistro;
					if(!this.ExecuteScalar(out idRegistro, command)) throw new Exception("Errore nella lettura dell'ID del registro.");

					//specifica i dati della protocollazione
					additionalFields += ",NUM_PROTO,NUM_ANNO_PROTO,DTA_PROTO,VAR_SEGNATURA,ID_REGISTRO,VAR_CHIAVE_PROTO";
					additionalValues += "," + documento.protocollo.numero + "," + documento.protocollo.anno + "," + Functions.ToDate(documento.protocollo.data);
					additionalValues += ",'" + documento.protocollo.segnatura + "'," + idRegistro + ",'" + documento.protocollo.chiave + "'"  ; 
				}

				if(documento.protocollo.protoEme != null && documento.protocollo.protoEme !="")
				{
					//specifica i dati della protocollazione
					additionalFields+=",VAR_PROTO_EME,DTA_PROTO_EME,VAR_COGNOME_EME,VAR_NOME_EME";
					additionalValues+=",'" + documento.protocollo.protoEme  + "'," + Functions.ToDate(documento.protocollo.dataProtoEme);
					additionalValues+=",'" + documento.protocollo.cognomeProtoEme  +"','" + documento.protocollo.nomeProtoEme  + "'" ; 
				}

				q.setParam("param17",additionalFields);
				q.setParam("param18",additionalValues);
			
				command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteNonQuery(command)) throw new Exception();

				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_ID_DOCUMENTO");
				q.setParam("param1",documento.numero);
				command = q.getSQL();
				logger.Debug(command);
				string idDocumento;
				if(!this.ExecuteScalar(out idDocumento,command)) throw new Exception();
			
				result=idDocumento;
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione nuovo documento", exception);
			}
		
			return result;
		}

		public bool Imp_NewCorrispondenteDocumento(string idCorrispondente,string idDocumento,string tipo)
		{
			bool result = true; // Presume successo
			
			try
			{
				//inserisce record nella tabella DPA_DOC_ARRIVO_PAR
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_CORRISPONDENTE_DOC");
				q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
				q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
				q.setParam("param3",idCorrispondente);
				q.setParam("param4",idDocumento);
				q.setParam("param5",tipo);
				string command = q.getSQL();
				logger.Debug(command);
				
				if(!this.ExecuteNonQuery(command))
				{
					throw new Exception();
				}
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione nuovo corrispondente per documento", exception);

				result = false;
			}
		
			return result;
		}

		public bool Imp_NewFascicoloDoc(string idDocumento,string id,string tipo)
		{
			bool result = true; // Presume successo
			
			try
			{
				//inserisce record nella tabella DPA_DOC_ARRIVO_PAR
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_FASCICOLO_DOC");
				q.setParam("param1",tipo);
				q.setParam("param2",id);
				q.setParam("param3",idDocumento);
				string command = q.getSQL();
				logger.Debug(command);
				
				if(!this.ExecuteNonQuery(command))
				{
					throw new Exception();
				}
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione fascicolatura per documento", exception);

				result = false;
			}
		
			return result;
		}

		public bool Imp_NewVersione(DocsPaDB.Query_DocsPAWS.Strutture.Versione versione)
		{
			bool result = true; // Presume successo
			
			try
			{
				//inserisce record nella tabella VERSIONS
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_VERSIONE");
				q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetVersionIdColName());
				q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
				q.setParam("param3",versione.docNumber);
				q.setParam("param4",versione.ID);
				q.setParam("param5",versione.subVersion);
				if(versione.label==null || versione.label=="")
				{
					q.setParam("param6","no label");
				}
				else
				{
					q.setParam("param6",versione.label);
				}
				q.setParam("param7",Imp_GetUserSystemId(versione.author));
				q.setParam("param8",Imp_GetUserSystemId(versione.typist));
				q.setParam("param9",Functions.ToDate(versione.dataCreazione));
				q.setParam("param10",Functions.ToDate(versione.dataArrivo));
				q.setParam("param11",versione.daInviare);
				q.setParam("param12",versione.commento);
				string command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteNonQuery(command))
				{
					throw new Exception();
				}
				//legge l'ID della versione appena creata
				string versionId;				
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_VERSIONE");
				q.setParam("param1",versione.docNumber);
				q.setParam("param2",versione.ID);
				if(versione.subVersion==null || versione.subVersion =="")
				{
					q.setParam("param3"," IS NULL");
				}
				else
				{
					q.setParam("param3"," = '" + versione.subVersion+"'");
				}
				//q.setParam("param3",versione.label);
				command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteScalar(out versionId,command))
				{
					throw new Exception();
				}

				//inserisce il record nella tabella COMPONENTS
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_COMPONENT");
				q.setParam("param1",versione.fileName);
				q.setParam("param2",versionId);
				q.setParam("param3",versione.docNumber);
				q.setParam("param4",versione.fileSize);
				q.setParam("param5",versione.impronta);
				command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteNonQuery(command))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione nuova versione", exception);

				result = false;
			}
		
			return result;
		}

		public bool Imp_NewAllegato(DocsPaDB.Query_DocsPAWS.Strutture.Allegato allegato)
		{
			bool result = true; // Presume successo
			
			try
			{
				//inserisce record nella tabella VERSIONS
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_VERSIONE");
				q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetVersionIdColName());
				q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
				q.setParam("param3",allegato.docNumber);
				q.setParam("param4",allegato.id);
				q.setParam("param5","");
				if(allegato.label==null || allegato.label=="")
				{
					q.setParam("param6","no label");
				}
				else
				{
					q.setParam("param6",allegato.label );
				}
				
				q.setParam("param7",Imp_GetUserSystemId(allegato.author));
				q.setParam("param8",Imp_GetUserSystemId(allegato.typist));
				q.setParam("param9",Functions.ToDate(allegato.dataCreazione));
				q.setParam("param10",Functions.ToDate(allegato.dataArrivo));
				q.setParam("param11",allegato.daInviare);
				q.setParam("param12",allegato.commento);
				string command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteNonQuery(command))
				{
					throw new Exception();
				}
				//legge l'ID dell'allegato appena creato
				string allegatoId;				
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_VERSIONE");
				q.setParam("param1",allegato.docNumber);
				q.setParam("param2",allegato.id);
				q.setParam("param3"," IS NULL");
				//q.setParam("param3",allegato.label);
				command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteScalar(out allegatoId,command))
				{
					throw new Exception();
				}

				//inserisce il record nella tabella COMPONENTS
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_COMPONENT");
				q.setParam("param1",allegato.fileName);
				q.setParam("param2",allegatoId);
				q.setParam("param3",allegato.docNumber);
				q.setParam("param4",allegato.fileSize);
				q.setParam("param5",allegato.impronta);
				command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteNonQuery(command))
				{
					throw new Exception();
				}

				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione nuovo allegato", exception);

				result = false;
			}
		
			return result;
		}

		public bool Imp_NewSicurezza(string idDocumento,string idUtenteRuolo,string tipo)
		{
			bool result = true; // Presume successo
			
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_SICUREZZA");
				q.setParam("param1",idDocumento);
				q.setParam("param2",idUtenteRuolo);
				q.setParam("param3",tipo);
				string command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteNonQuery(command))
				{
					throw new Exception();
				}
				
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione nuovo record sicurezza", exception);

				result = false;
			}
		
			return result;
		}

		public string Imp_NewTrasmissione(DocsPaDB.Query_DocsPAWS.Strutture.Trasmissione trasmissione,string idDocumento)
		{
			string result = null; // Presume successo
			
			try
			{
				//inserisce record nella tabella DPA_CORR_GLOBALI
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_TRASMISSIONE");
				q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
				q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
				q.setParam("param3",trasmissione.ruolo);
				q.setParam("param4",trasmissione.utente);
				q.setParam("param5",trasmissione.tipoOggetto);
				q.setParam("param6",trasmissione.idProfile);
				q.setParam("param7",Functions.ToDate(trasmissione.dataInvio));
				q.setParam("param8",trasmissione.noteGenerali);
				string command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteNonQuery(command))
				{
					throw new Exception();
				}
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_LAST_TRASMISSIONE");
				command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteScalar(out result,command))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione nuovo record trasmissione", exception);

				result = null;
			}
		
			return result;
		}

		public string Imp_NewTrasmissioneSingola(DocsPaDB.Query_DocsPAWS.Strutture.TrasmissioneSingola trasmissione,string idTrasmissione)
		{
			string result = null; // Presume successo
			
			try
			{
				//inserisce record nella tabella DPA_CORR_GLOBALI
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_TRASMISSIONESINGOLA");
				q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
				q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
				q.setParam("param3",trasmissione.ragione);
				q.setParam("param4",trasmissione.tipoDest);
				q.setParam("param5",trasmissione.corrispondente);
				q.setParam("param6",trasmissione.noteSing);
				q.setParam("param7",Functions.ToDate(trasmissione.dataScadenza));
				q.setParam("param8",idTrasmissione);
				string command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteNonQuery(command))
				{
					throw new Exception();
				}
				
				q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_GET_LAST_TRASMISSIONESINGOLA");
				command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteScalar(out result,command))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione nuovo record trasmissione singola", exception);

				result = null;
			}
		
			return result;
		}
		
		
		public bool Imp_NewTrasmissioneUtente(DocsPaDB.Query_DocsPAWS.Strutture.TrasmissioneUtente trasmissione,string idTrasmissioneSingola)
		{
			bool result = true; // Presume successo
			
			try
			{
				//inserisce record nella tabella DPA_CORR_GLOBALI
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("IMP_NEW_TRASMISSIONEUTENTE");
				q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
				q.setParam("param2", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
				q.setParam("param3",idTrasmissioneSingola);
				q.setParam("param4",trasmissione.corrispondente);
				q.setParam("param5",Functions.ToDate(trasmissione.dataVista));
				q.setParam("param6",Functions.ToDate(trasmissione.dataAccettata));
				q.setParam("param7",Functions.ToDate(trasmissione.dataRifiutata));
				q.setParam("param8",Functions.ToDate(trasmissione.dataRisposta));
				q.setParam("param9",trasmissione.vista);
				q.setParam("param10",trasmissione.accettata);
				q.setParam("param11",trasmissione.rifiutata);
				q.setParam("param12",trasmissione.noteAcc);
				q.setParam("param13",trasmissione.noteRif );
				q.setParam("param14",trasmissione.valida);
				string command = q.getSQL();
				logger.Debug(command);
				if(!this.ExecuteNonQuery(command))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione nuovo record trasmissione utente", exception);

				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="listaRuoli"></param>
		/// <param name="idRuolo"></param>
		/// <returns></returns>
		public bool GetRuoliSuperiori(ref System.Collections.ArrayList listaRuoli, string idRuolo)
		{
			bool result = true;

            try
            {
				string idParent;
				DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery("GET_RUOLI_LIVELLO_SUPERIORE");
				q.setParam("param1", idRuolo);
				string command = q.getSQL();
				logger.Debug(command);

				if(!this.ExecuteScalar(out idParent, command)) throw new Exception();

				if(idParent != null && idParent != string.Empty)
				{
					if(listaRuoli.IndexOf(idParent) < 0) listaRuoli.Add(idParent);				
				}

				// Accedi ricorsivamente ai ruoli superiori di ogni ruolo
				if(idParent != null) this.GetRuoliSuperiori(ref listaRuoli, idParent);
            }
            catch(Exception exception)
            {
				logger.Debug("Errore durante la lettura dei ruoli superiori", exception);
				result = false;
            }

			return result;
		}
		
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sNomeQuery"></param>
		/// <returns></returns>
		private bool ExistsLinkWith(string sNomeQuery, string sVal, out string res)
		{
			System.Data.DataSet ds;
			bool result=false;
			
			DocsPaUtils.Query q = DocsPaUtils.InitImportExportQuery.getInstance().getQuery(sNomeQuery);
			
			q.setParam("param1",sVal);
			string command = q.getSQL();
			
			if(!this.ExecuteQuery(out ds,command)) throw new Exception();
			if((ds != null) && (ds.Tables[0].Rows.Count > 0)) //successo
			{
				//da cancellare (scopi diagnostici)
				if (sNomeQuery=="GET_ID_DOCUMENTO")
					cont ++;
				//---------------------------------
					
				res=ds.Tables[0].Rows[0][0].ToString();
				result=true;
			}
			else
			{	
				res=null;
				result=false;
			}
			logger.Debug(command);
			return result;
		}
		
		
		
		
	}
}

