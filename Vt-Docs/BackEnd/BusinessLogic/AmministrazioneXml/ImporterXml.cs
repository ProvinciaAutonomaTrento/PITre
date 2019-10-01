using System;
using System.Xml;
using log4net;

namespace BusinessLogic.AmministrazioneXml
{
	/// <summary>
	/// Classe per la gestione delle amministrazioni di DocsPA tramite XML
	/// </summary>
	public class ImporterXml
	{
        private ILog logger = LogManager.GetLogger(typeof(ImporterXml));
		private ErrorCode errorCode;
		
		public ImporterXml()
		{
			errorCode = ErrorCode.NoError;
		}

		/// <summary>Ritorna l'ultimo codice di errore</summary>
		/// <returns></returns>
		public ErrorCode GetErrorCode()
		{
			return errorCode;
		}

//		/// <summary>
//		/// Ritorna i path dei file relativi all'import dei documenti										
//		/// </summary>
//		/// <returns></returns>
//		public bool GetFilePaths(out string sourcePath, out string destinationPath)
//		{
//			bool result = true;
//			sourcePath = null;
//			destinationPath = null;
//
//			try
//			{					
//				sourcePath      = System.Configuration.ConfigurationManager.AppSettings["sourceDocsRepositoryPath"];
//				destinationPath = System.Configuration.ConfigurationManager.AppSettings["destinationDocsRepositoryPath"];
//			}
//			catch(Exception exception)
//			{
//				logger.Debug("Errore durante la lettura dei path", exception);
//				result=false;
//			}
//
//			return result;
//		}

		/// <summary>
		/// importazione dei corrispondenti
		/// </summary>
		/// <returns></returns>
		public bool ImportCorrispondenti(string filePath)
		{
			bool result = true; //presume successo

			try
			{
				DocsPaUtils.LogsManagement.ImportExportLogger.ResetLog("import", 0);

				//apre il file XML
				XmlDocument doc=new XmlDocument();
				doc.Load(filePath);

				XmlNode root= doc.SelectSingleNode("DOCSPA");
				
				//importazione dei corrispondenti esterni					
				if(!ImportCorrispondenti(doc,root))
				{
					logger.Debug("Errore importazione corrispondenti");
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'importazione dei corrispondenti", exception);
				result=false;
			}

			DocsPaUtils.LogsManagement.ImportExportLogger.ResetLog("import", 0);

			return result;

		}

		/// <summary>
		/// importazione dei tipi atto
		/// </summary>
		/// <returns></returns>
		public bool ImportTipiAtto(string filePath)
		{
			bool result = true; //presume successo
			try
			{
				//apre il file XML
				XmlDocument doc=new XmlDocument();
				doc.Load(filePath);

				XmlNode root= doc.SelectSingleNode("DOCSPA");
				
				//importazione dei tipi atto
				if(!ImportTipiAtto(doc,root))
				{
					logger.Debug("Errore importazione tipi atto");
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'importazione dei tipi atto", exception);
				result=false;
			}

			return result;

		}

		/// <summary>
		/// importazione dei tipi documento.
		/// </summary>
		/// <returns></returns>
		public bool ImportTipiDocumento(string filePath)
		{
			bool result = true; //presume successo
			try
			{
				//apre il file XML
				XmlDocument doc=new XmlDocument();
				doc.Load(filePath);

				XmlNode root= doc.SelectSingleNode("DOCSPA");

				//importazione dei tipi documento
				if(!ImportTipiDocumento(doc,root))
				{
					logger.Debug("Errore importazione tipi documento");
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'importazione dei tipi documento", exception);
				result=false;
			}

			return result;

		}

		/// <summary>
		/// importazione dei documenti.
		/// </summary>
		/// <returns></returns>
		public bool ImportDocumenti(string filePath)
		{
			bool result = true; //presume successo

			try
			{
				DocsPaUtils.LogsManagement.ImportExportLogger.ResetLog("import", 0);

				//apre il file XML
				XmlDocument doc=new XmlDocument();
				doc.Load(filePath);

				XmlNode root= doc.SelectSingleNode("DOCSPA");

				//importazione dei documenti
				if(!ImportDocumenti(doc,root))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'importazione dei documenti", exception);
				result=false;
			}

			DocsPaUtils.LogsManagement.ImportExportLogger.ResetLog("import", 0);

			return result;
		}

		/// <summary>
		/// importazione dell'oggettario.
		/// </summary>
		/// <returns></returns>
		public bool ImportOggettario(string filePath)
		{
			bool result = true; // Presume successo

			try
			{
				DocsPaUtils.LogsManagement.ImportExportLogger.ResetLog("import", 0);

				//apre il file XML
				XmlDocument doc=new XmlDocument();
				doc.Load(filePath);

				XmlNode root = doc.SelectSingleNode("DOCSPA");

				//importazione dei documenti
				if(!ImportOggettario(doc, root))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'importazione dell'oggettario", exception);
				result=false;
			}

			DocsPaUtils.LogsManagement.ImportExportLogger.ResetLog("import", 0);

			return result;
		}

		/// <summary>
		/// Import dell'oggettario
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="node"></param>
		/// <param name="idAmm"></param>
		/// <returns></returns>
		public bool ImportOggettario(XmlDocument doc,XmlNode node)
		{
			bool result = true;

			try
			{
				XmlNodeList oggetti = node.SelectSingleNode("OGGETTARIO").SelectNodes("OGGETTO");
				
				if(oggetti != null)
				{
					DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);

					foreach(XmlNode oggetto in oggetti)
					{						
						if(oggetto != null)
						{
							string descrizione     = oggetto.SelectSingleNode("DESCRIZIONE").InnerText;
							string amministrazione = amministrazioneXml.GetAdminByName(oggetto.SelectSingleNode("AMMINISTRAZIONE").InnerText);
							string registro		   = null;
							if(oggetto.SelectSingleNode("REGISTRO")!=null)
							{
								registro=amministrazioneXml.GetRegByName(oggetto.SelectSingleNode("REGISTRO").InnerText);
							}
						
							DocsPaDB.Query_DocsPAWS.ImporterXml importer = new DocsPaDB.Query_DocsPAWS.ImporterXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);
							
							if(!importer.ImportOggettario(descrizione, amministrazione, registro))
							{
								logger.Debug("Errore durante l'import dell'oggetto.");
							}
						}					
					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'importazione dei documenti", exception);
				result=false;
			}

			return result;

		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public bool ImportStoricoOggettario(string filePath)
		{
			bool result = true; // Presume successo
			try
			{
				DocsPaUtils.LogsManagement.ImportExportLogger.ResetLog("import", 0);
				//apre il file XML
				XmlDocument doc=new XmlDocument();
				doc.Load(filePath);

				XmlNode root = doc.SelectSingleNode("DOCSPA");

				//importazione dello storico
				if(!ImportStoricoOggettario(doc, root))
				{
					throw new Exception();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'importazione dell0 storico oggettario", exception);
				result=false;
			}

			DocsPaUtils.LogsManagement.ImportExportLogger.ResetLog("import", 0);

			return result;
		}
		
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		public bool ImportStoricoOggettario(XmlDocument doc, XmlNode node)
		{
			bool result = true;

			try
			{
				XmlNodeList oggetti_storici = node.SelectSingleNode("STORICO_OGGETTARIO").SelectNodes("OGGETTO_STORICO");
				
				if(oggetti_storici != null)
				{
					DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = 
						new DocsPaDB.Query_DocsPAWS.AmministrazioneXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);

					foreach(XmlNode obj in oggetti_storici)
					{						
						if(obj != null)
						{
							string data_modifica    = obj.SelectSingleNode("data_modifica").InnerText;
							string documento			= obj.SelectSingleNode("documento").InnerText;
							string oggetto				= obj.SelectSingleNode("oggetto").InnerText;
							string utente				= obj.SelectSingleNode("utente").InnerText;
							string rubrica				= obj.SelectSingleNode("rubrica").InnerText;
							
							DocsPaDB.Query_DocsPAWS.ImporterXml importer = 
								new DocsPaDB.Query_DocsPAWS.ImporterXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);
							
							//if(!importer.ImportOggettario(descrizione, amministrazione, registro))
							if(!importer.ImportStoricoOggettario(data_modifica, documento, oggetto, utente,rubrica))
							{
								logger.Debug("Errore durante l'import dello storico oggettario.");
							}
						}					
					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'importazione dei documenti", exception);
				result=false;
			}

			return result;

		}
		
		
		
		/// <summary>
		/// Esportazione dei corrispondenti. Se il corrispondente non ha definito il codice rubrica,
		/// viene assegnato automaticamente l'ID del record. Questo per identificare univocamente i
		/// corrispondenti
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="node"></param>
		/// <param name="idAmm"></param>
		/// <returns></returns>
		public bool ImportCorrispondenti(XmlDocument doc, XmlNode node)
		{
			bool result = true;
			try
			{
				XmlNode corrispondenti=node.SelectSingleNode("CORRISPONDENTI");

				if(corrispondenti!=null)
				{
					DocsPaDB.Query_DocsPAWS.Strutture.Corrispondente datiCorrispondente=new DocsPaDB.Query_DocsPAWS.Strutture.Corrispondente();
					DocsPaDB.Query_DocsPAWS.ImporterXml importer = new DocsPaDB.Query_DocsPAWS.ImporterXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);
					DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazione=new DocsPaDB.Query_DocsPAWS.AmministrazioneXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);
					XmlNodeList nodiCorr = corrispondenti.SelectNodes("CORRISPONDENTE");
					int counter = 0;

					foreach(XmlNode corrispondente in nodiCorr)
					{
						counter++;
						DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage(counter, nodiCorr.Count, "Import del corrispondente " + counter + " di " + nodiCorr.Count);

						//ciclo sui corrispondenti: legge i dati del corrispondente
						
						datiCorrispondente.cap		   = corrispondente.SelectSingleNode("CAP").InnerText;
						datiCorrispondente.citta	   = corrispondente.SelectSingleNode("CITTA").InnerText;
						datiCorrispondente.codFiscale  = corrispondente.SelectSingleNode("COD_FISCALE").InnerText;
						datiCorrispondente.codice	   = corrispondente.SelectSingleNode("CODICE").InnerText;
						datiCorrispondente.codiceAOO   = corrispondente.SelectSingleNode("CODICE_AOO").InnerText;
						datiCorrispondente.codRubrica  = corrispondente.SelectSingleNode("COD_RUBRICA").InnerText;
						datiCorrispondente.cognome     = corrispondente.SelectSingleNode("COGNOME").InnerText;
						datiCorrispondente.dataFine    = corrispondente.SelectSingleNode("DATA_FINE").InnerText;
						datiCorrispondente.dataInizio  = corrispondente.SelectSingleNode("DATA_INIZIO").InnerText;
						datiCorrispondente.descrizione = corrispondente.SelectSingleNode("DESC_CORR").InnerText;
						datiCorrispondente.fax		   = corrispondente.SelectSingleNode("FAX").InnerText;
						datiCorrispondente.email	   = corrispondente.SelectSingleNode("EMAIL").InnerText;
						datiCorrispondente.smtp		   = corrispondente.SelectSingleNode("SMTP").InnerText;
						datiCorrispondente.portaSmtp   = corrispondente.SelectSingleNode("PORTA_SMTP").InnerText;
						datiCorrispondente.indirizzo   = corrispondente.SelectSingleNode("INDIRIZZO").InnerText;
						datiCorrispondente.nazione     = corrispondente.SelectSingleNode("NAZIONE").InnerText;
						datiCorrispondente.nome		   = corrispondente.SelectSingleNode("NOME").InnerText;
						datiCorrispondente.note		   = corrispondente.SelectSingleNode("NOTE").InnerText;
						datiCorrispondente.provincia   = corrispondente.SelectSingleNode("PROVINCIA").InnerText;
						datiCorrispondente.systemId    = corrispondente.SelectSingleNode("SYSTEM_ID").InnerText;
						datiCorrispondente.telefono    = corrispondente.SelectSingleNode("TELEFONO").InnerText;
						datiCorrispondente.telefono2   = corrispondente.SelectSingleNode("TELEFONO2").InnerText;
						datiCorrispondente.tipoCorr    = corrispondente.SelectSingleNode("TIPO_CORR").InnerText;
						datiCorrispondente.tipoIE	   = corrispondente.SelectSingleNode("TIPO_IE").InnerText;
						datiCorrispondente.tipoURP	   = corrispondente.SelectSingleNode("TIPO_URP").InnerText;
						datiCorrispondente.numLivello  = corrispondente.SelectSingleNode("NUM_LIVELLO").InnerText;
						datiCorrispondente.idParent	   = corrispondente.SelectSingleNode("ID_PARENT").InnerText;
						datiCorrispondente.pa		   = corrispondente.SelectSingleNode("PA").InnerText;
						datiCorrispondente.idAmm	   = amministrazione.GetAdminByName(corrispondente.SelectSingleNode("AMMINISTRAZIONE").InnerText);
						datiCorrispondente.idRegistro  = amministrazione.GetRegByName(corrispondente.SelectSingleNode("REGISTRO").InnerText);

						if(datiCorrispondente.idAmm!=null && datiCorrispondente.idAmm!="")
						{
							if(!importer.Imp_NewCorrispondente(datiCorrispondente))
							{
								logger.Debug("Impossibile importare corrispondente: " + datiCorrispondente.codice);
							}
						}
						else
						{
							logger.Debug("Impossibile importare corrispondente: " + datiCorrispondente.codice + " -> amministrazione sconosciuta");
						}
					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'importazione dei corrispondenti", exception);
				result=false;
			}

			return result;
		}
		
		/// <summary>
		/// import dei documenti
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="node"></param>
		/// <param name="idAmm"></param>
		/// <returns></returns>
		public bool ImportDocumenti(XmlDocument doc,XmlNode node)
		{
			bool result = true;
			
			try
			{
				XmlNode documenti=node.SelectSingleNode("DOCUMENTI");

				if(documenti!=null)
				{
					DocsPaDB.Query_DocsPAWS.Strutture.Documento datiDocumento=new DocsPaDB.Query_DocsPAWS.Strutture.Documento();
					DocsPaDB.Query_DocsPAWS.ImporterXml importer=new DocsPaDB.Query_DocsPAWS.ImporterXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);
					DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazione = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);
					XmlNodeList nodiDoc = documenti.SelectNodes("DOCUMENTO");
					int counter = 0;

					foreach(XmlNode documento in nodiDoc)
					{
						counter++;
						DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage(counter, nodiDoc.Count, "Import del documento " + counter + " di " + nodiDoc.Count);

						//ciclo sui documenti
						XmlNode dati = documento.SelectSingleNode("DATI");
						datiDocumento.assegnato			= dati.SelectSingleNode("ASSEGNATO").InnerText;
						datiDocumento.congelato			= dati.SelectSingleNode("CONGELATO").InnerText;
						datiDocumento.consolidato		= dati.SelectSingleNode("CONSOLIDATO").InnerText;
						datiDocumento.dataCreazione		= dati.SelectSingleNode("DATA_CREAZIONE").InnerText;
						datiDocumento.fascicolato		= dati.SelectSingleNode("FASCICOLATO").InnerText;
						datiDocumento.img				= dati.SelectSingleNode("IMG").InnerText;
						datiDocumento.invioConferma		= dati.SelectSingleNode("INVIO_CONFERMA").InnerText;
						datiDocumento.note				= dati.SelectSingleNode("NOTE").InnerText;
						datiDocumento.numero			= dati.SelectSingleNode("NUMERO").InnerText;
						datiDocumento.privato			= dati.SelectSingleNode("PRIVATO").InnerText;
						datiDocumento.tipo				= amministrazione.GetTipoDocumento(dati.SelectSingleNode("TIPO").InnerText);
						datiDocumento.tipoAtto			= amministrazione.GetTipoAtto(dati.SelectSingleNode("TIPO_ATTO").InnerText);
						datiDocumento.tipoProto			= dati.SelectSingleNode("TIPO_PROTO").InnerText;
						datiDocumento.predispostoProto	= dati.SelectSingleNode("PREDISPOSTO_PROTO").InnerText;
						datiDocumento.author			= importer.Imp_GetUserSystemId(dati.SelectSingleNode("AUTORE").InnerText);
						datiDocumento.typist			= importer.Imp_GetUserSystemId(dati.SelectSingleNode("TYPIST").InnerText);
						

						XmlNode oggetto=dati.SelectSingleNode("OGGETTO");

						if(oggetto!=null)
						{
							datiDocumento.oggetto.descrizione	  = oggetto.SelectSingleNode("DESCRIZIONE").InnerText;
							datiDocumento.oggetto.amministrazione = amministrazione.GetAdminByName(oggetto.SelectSingleNode("AMMINISTRAZIONE").InnerText);
							datiDocumento.oggetto.registro		  = amministrazione.GetRegByName(oggetto.SelectSingleNode("REGISTRO").InnerText);
							logger.Debug("ID AMM= " + datiDocumento.oggetto.amministrazione );
							logger.Debug("ID REGISTRO= " + datiDocumento.oggetto.registro);
						}
					
						XmlNode protocollo=dati.SelectSingleNode("PROTOCOLLO");

						if(protocollo!=null)
						{
							datiDocumento.protocollo.anno		= protocollo.SelectSingleNode("ANNO").InnerText;
							datiDocumento.protocollo.chiave		= protocollo.SelectSingleNode("TIPO").InnerText;
							datiDocumento.protocollo.data		= protocollo.SelectSingleNode("DATA").InnerText;
							datiDocumento.protocollo.numero		= protocollo.SelectSingleNode("NUMERO").InnerText;
							datiDocumento.protocollo.registro	= protocollo.SelectSingleNode("REGISTRO").InnerText;
							datiDocumento.protocollo.segnatura	= protocollo.SelectSingleNode("SEGNATURA").InnerText;
							XmlNode emergenza=protocollo.SelectSingleNode("EMERGENZA");
							if(emergenza!=null)
							{
								datiDocumento.protocollo.cognomeProtoEme = emergenza.SelectSingleNode("COGNOME").InnerText;
								datiDocumento.protocollo.dataProtoEme    = emergenza.SelectSingleNode("DATA").InnerText;
								datiDocumento.protocollo.nomeProtoEme    = emergenza.SelectSingleNode("NOME").InnerText;
								datiDocumento.protocollo.protoEme        = emergenza.SelectSingleNode("NUMERO").InnerText;
							}
						}

						//creazione del documento e lettura del docnumber e systemid del documento creato
						datiDocumento.systemId=importer.Imp_NewDocumento(datiDocumento);			
						if(datiDocumento.systemId==null || datiDocumento.systemId=="") throw new Exception();						

						//lettura dei corrispondenti asociati al documento
						if(!ImportCorrispondentiDocumenti(doc,documento,datiDocumento.systemId)) throw new Exception();	

						//lettura delle versioni del documento
						if(!ImportVersioni(doc,documento,datiDocumento.numero, dati.SelectSingleNode("AUTORE").InnerText, dati.SelectSingleNode("TYPIST").InnerText)) throw new Exception();	

						//lettura degli allegati del documento
						if(!ImportAllegati(doc,documento,datiDocumento.numero, dati.SelectSingleNode("AUTORE").InnerText, dati.SelectSingleNode("TYPIST").InnerText)) throw new Exception();	

						//lettura delle trasmissioni del documento
						if(!ImportTrasmissioni(doc,documento,datiDocumento.systemId )) throw new Exception();	

						//lettura dei fascicoli
						if(!ImportFascicoli(doc,documento,datiDocumento.systemId)) throw new Exception();	

						//lettura delle regole di sicurezza
						if(!ImportSicurezza(doc,documento,datiDocumento.systemId)) throw new Exception();	

					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'importazione dei documenti", exception);
				result=false;
			}

			return result;

		}

		/// <summary>
		/// inserisce le relazioni tra corrispondeti e documenti
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="node"></param>
		/// <param name="idDocumento"></param>
		/// <returns></returns>
		public bool ImportCorrispondentiDocumenti(XmlDocument doc,XmlNode node,string idDocumento)
		{
			bool result = true;
			try
			{
				XmlNode corrispondenti=node.SelectSingleNode("CORRISPONDENTI");
				if(corrispondenti!=null)
				{

					DocsPaDB.Query_DocsPAWS.ImporterXml importer = new DocsPaDB.Query_DocsPAWS.ImporterXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);

					foreach(XmlNode corrispondente in corrispondenti.SelectNodes("CORRISPONDENTE"))
					{
						string idCorrispondente;
						string tipo;
						idCorrispondente = importer.Imp_GetCorrispondenteByCodRubrica(corrispondente.SelectSingleNode("ID").InnerText);
						tipo=corrispondente.SelectSingleNode("TIPO_MITT_DEST").InnerText;
						//crea il corrispondente associato al documento
						if(!importer.Imp_NewCorrispondenteDocumento(idCorrispondente,idDocumento,tipo))
						{
							throw new Exception();
						}
					}
				}
			}
			catch(Exception e)
			{
				logger.Debug("Errore durante l'import dei corrispondenti del documento",e);
				result=false;
			}
			return result;
		}

		/// <summary>
		/// inserisce la parole associate al documento. TO DO -> manca l'assegnazione della parola alla amministrazione
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="node"></param>
		/// <param name="idDocumento"></param>
		/// <returns></returns>
		public bool ImportParole(XmlDocument doc,XmlNode node,string idDocumento)
		{
			bool result = true;
			try
			{
				XmlNode parole=node.SelectSingleNode("PAROLE");
				if(parole!=null)
				{
					foreach(XmlNode parola in parole.SelectNodes("PAROLA"))
					{
						string testoParola;
						testoParola=parola.InnerText;
						//crea la parola
					}
				}
			}
			catch(Exception e)
			{
				logger.Debug("Errore durante l'import delle parole",e);
				result=false;
			}
			return result;
		}

		/// <summary>
		/// inserisce le versioni per il documento
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="node"></param>
		/// <param name="docNumber"></param>
		/// <returns></returns>
		public bool ImportVersioni(XmlDocument doc, XmlNode node, string docNumber, string defaultAuthor, string defaultTypist)
		{
			bool result = true;
			try
			{
				XmlNode versioni=node.SelectSingleNode("VERSIONI");
				if(versioni!=null)
				{
					DocsPaDB.Query_DocsPAWS.ImporterXml importer=new DocsPaDB.Query_DocsPAWS.ImporterXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);
					
					foreach(XmlNode nodoVersione in versioni.SelectNodes("VERSIONE"))
					{
						XmlNode datiVersione=nodoVersione.SelectSingleNode("DATI");
						DocsPaDB.Query_DocsPAWS.Strutture.Versione versione=new DocsPaDB.Query_DocsPAWS.Strutture.Versione();
						versione.author = datiVersione.SelectSingleNode("AUTORE").InnerText;
						versione.typist  = datiVersione.SelectSingleNode("TYPIST").InnerText;
						
						if(versione.author == null || versione.author == "") versione.author = defaultAuthor;
						if(versione.typist == null || versione.typist == "") versione.typist = defaultTypist;

						versione.commento = datiVersione.SelectSingleNode("COMMENTO").InnerText;
						versione.daInviare  = datiVersione.SelectSingleNode("DA_INVIARE").InnerText;
						versione.dataArrivo  = datiVersione.SelectSingleNode("DATA_ARRIVO").InnerText;
						versione.dataCreazione  = datiVersione.SelectSingleNode("DATA_CREAZIONE").InnerText;
						versione.ID  = datiVersione.SelectSingleNode("ID").InnerText;
						versione.label  = datiVersione.SelectSingleNode("LABEL").InnerText;
						XmlNode fileVersione=nodoVersione.SelectSingleNode("FILE");
						versione.fileName  = fileVersione.SelectSingleNode("NOME").InnerText;
						versione.fileSize  = fileVersione.SelectSingleNode("SIZE").InnerText;
						versione.impronta  = fileVersione.SelectSingleNode("IMPRONTA").InnerText;
						versione.docNumber=docNumber;
						
						//crea la versione
						if(!importer.Imp_NewVersione(versione))
						{
							throw new Exception();
						}
					}
				}
			}
			catch(Exception e)
			{
				logger.Debug("Errore durante l'import delle versioni",e);
				result=false;
			}
			return result;
		}

		/// <summary>
		/// inserisce gli allegati del documento
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="node"></param>
		/// <param name="docNumber"></param>
		/// <returns></returns>
		public bool ImportAllegati(XmlDocument doc, XmlNode node, string docNumber, string defaultAuthor, string defaultTypist)
		{
			bool result = true;
			try
			{
				XmlNode allegati=node.SelectSingleNode("ALLEGATI");
				if(allegati!=null)
				{

					DocsPaDB.Query_DocsPAWS.ImporterXml importer=new DocsPaDB.Query_DocsPAWS.ImporterXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);

					foreach(XmlNode nodoAllegato in allegati.SelectNodes("ALLEGATO"))
					{
						XmlNode datiAllegato=nodoAllegato.SelectSingleNode("DATI");
						DocsPaDB.Query_DocsPAWS.Strutture.Allegato allegato=new DocsPaDB.Query_DocsPAWS.Strutture.Allegato();
						allegato.id = datiAllegato.SelectSingleNode("ID").InnerText;
						allegato.label  = datiAllegato.SelectSingleNode("LABEL").InnerText;
						//if(allegato.label ==null || allegato.label =="") allegato.label ="Allegato";
						allegato.commento = datiAllegato.SelectSingleNode("COMMENTO").InnerText;
						allegato.dataCreazione  = datiAllegato.SelectSingleNode("DATA_CREAZIONE").InnerText;
						allegato.dataArrivo  = datiAllegato.SelectSingleNode("DATA_ARRIVO").InnerText;
						allegato.typist  = datiAllegato.SelectSingleNode("TYPIST").InnerText;
						allegato.author = datiAllegato.SelectSingleNode("AUTORE").InnerText;
						
						if(allegato.author == null || allegato.author == "") allegato.author = defaultAuthor;
						if(allegato.typist == null || allegato.typist == "") allegato.typist = defaultTypist;
						
						allegato.daInviare  = datiAllegato.SelectSingleNode("DA_INVIARE").InnerText;
						allegato.pagine  = datiAllegato.SelectSingleNode("PAGINE").InnerText;
						XmlNode fileAllegato=nodoAllegato.SelectSingleNode("FILE");
						allegato.fileName  = fileAllegato.SelectSingleNode("NOME").InnerText;
						allegato.fileSize  = fileAllegato.SelectSingleNode("SIZE").InnerText;
						allegato.impronta  = fileAllegato.SelectSingleNode("IMPRONTA").InnerText;
						allegato.docNumber=docNumber;
						//crea l'allegato
						if(!importer.Imp_NewAllegato(allegato))
						{
							throw new Exception();
						}
					}
				}
			}
			catch(Exception e)
			{
				logger.Debug("Errore durante l'import degli allegati",e);
				result=false;
			}
			return result;
		}

		/// <summary>
		/// inserisce le trasmissioni del documento
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="node"></param>
		/// <param name="idDocumento"></param>
		/// <returns></returns>
		public bool ImportTrasmissioni(XmlDocument doc,XmlNode node,string idDocumento)
		{
			bool result = true;
			try
			{
				XmlNode trasmissioni=node.SelectSingleNode("TRASMISSIONI");
				if(trasmissioni!=null)
				{

					DocsPaDB.Query_DocsPAWS.ImporterXml importer = new DocsPaDB.Query_DocsPAWS.ImporterXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);

					foreach(XmlNode nodoTrasmissione in trasmissioni.SelectNodes("TRASMISSIONE"))
					{
						try
						{									
							XmlNode datiTrasmissione=nodoTrasmissione.SelectSingleNode("DATI");
							DocsPaDB.Query_DocsPAWS.Strutture.Trasmissione trasmissione=new DocsPaDB.Query_DocsPAWS.Strutture.Trasmissione();
							trasmissione.dataInvio =datiTrasmissione.SelectSingleNode("DATA_INVIO").InnerText;
							trasmissione.noteGenerali  =datiTrasmissione.SelectSingleNode("NOTE_GENERALI").InnerText;
							trasmissione.ruolo = importer.Imp_GetCorrispondenteByCodRubrica( datiTrasmissione.SelectSingleNode("RUOLO").InnerText);
							trasmissione.tipoOggetto =datiTrasmissione.SelectSingleNode("TIPO_OGGETTO").InnerText;
							trasmissione.utente = importer.Imp_GetIdPeopleByCodRubrica(datiTrasmissione.SelectSingleNode("UTENTE").InnerText);
							trasmissione.idProfile=idDocumento;
							//crea la trasmissione
							string idTrasmissione=importer.Imp_NewTrasmissione(trasmissione,idDocumento);
							if(idTrasmissione==null || idTrasmissione=="")
							{
								throw new Exception();
							}
							//ciclo sulle trasmissioni singole
							XmlNode trasmissioniSingole=nodoTrasmissione.SelectSingleNode("TRASMISSIONI_SINGOLE");
							if(trasmissioniSingole!=null)
							{
								DocsPaDB.Query_DocsPAWS.Strutture.TrasmissioneSingola trasmissioneSingola=new DocsPaDB.Query_DocsPAWS.Strutture.TrasmissioneSingola();
								foreach(XmlNode nodoTrasmissioneSingola in trasmissioniSingole.SelectNodes("TRASMISSIONE_SINGOLA"))
								{
									XmlNode datiTrasmissioneSingola=nodoTrasmissioneSingola.SelectSingleNode("DATI");
									trasmissioneSingola.corrispondente = importer.Imp_GetCorrispondenteByCodRubrica(datiTrasmissioneSingola.SelectSingleNode("CORRISPONDENTE").InnerText);
									trasmissioneSingola.dataScadenza  =datiTrasmissioneSingola.SelectSingleNode("DATA_SCADENZA").InnerText;
									trasmissioneSingola.noteSing  =datiTrasmissioneSingola.SelectSingleNode("NOTE_SING").InnerText;
									trasmissioneSingola.ragione  =datiTrasmissioneSingola.SelectSingleNode("RAGIONE").InnerText;
									trasmissioneSingola.tipo  =datiTrasmissioneSingola.SelectSingleNode("TIPO").InnerText;
									trasmissioneSingola.tipoDest  =datiTrasmissioneSingola.SelectSingleNode("TIPO_DEST").InnerText;
									trasmissioneSingola.idTrasmissione=idTrasmissione;
									//crea la trasmissione singola
									string idTrasmissioneSingola=importer.Imp_NewTrasmissioneSingola(trasmissioneSingola,idTrasmissione);
									if(idTrasmissioneSingola==null || idTrasmissioneSingola=="")
									{
										throw new Exception();
									}
									//ciclo sulle trasmissioni utente
									XmlNode trasmissioniUtente=nodoTrasmissioneSingola.SelectSingleNode("TRASMISSIONI_UTENTE");
									if(trasmissioniUtente!=null)
									{
										DocsPaDB.Query_DocsPAWS.Strutture.TrasmissioneUtente trasmissioneUtente=new DocsPaDB.Query_DocsPAWS.Strutture.TrasmissioneUtente();
										foreach(XmlNode nodoTrasmissioneUtente in trasmissioniUtente.SelectNodes("TRASMISSIONE_UTENTE"))
										{
											trasmissioneUtente.accettata=nodoTrasmissioneUtente.SelectSingleNode("ACCETTATA").InnerText;
											trasmissioneUtente.corrispondente=importer.Imp_GetCorrispondenteByCodRubrica( nodoTrasmissioneUtente.SelectSingleNode("CORRISPONDENTE").InnerText);
											trasmissioneUtente.dataAccettata=nodoTrasmissioneUtente.SelectSingleNode("DATA_ACCETTATA").InnerText;
											trasmissioneUtente.dataRifiutata=nodoTrasmissioneUtente.SelectSingleNode("DATA_RIFIUTATA").InnerText;
											trasmissioneUtente.dataRisposta=nodoTrasmissioneUtente.SelectSingleNode("DATA_RISPOSTA").InnerText;
											trasmissioneUtente.dataVista=nodoTrasmissioneUtente.SelectSingleNode("DATA_VISTA").InnerText;
											trasmissioneUtente.noteAcc=nodoTrasmissioneUtente.SelectSingleNode("NOTE_ACC").InnerText;
											trasmissioneUtente.noteRif=nodoTrasmissioneUtente.SelectSingleNode("NOTE_RIF").InnerText;
											trasmissioneUtente.rifiutata=nodoTrasmissioneUtente.SelectSingleNode("RIFIUTATA").InnerText;
											trasmissioneUtente.valida=nodoTrasmissioneUtente.SelectSingleNode("VALIDA").InnerText;
											trasmissioneUtente.vista=nodoTrasmissioneUtente.SelectSingleNode("VISTA").InnerText;
											trasmissioneUtente.idTrasmissioneSingola=idTrasmissioneSingola ;

											//crea la trasmissione utente
											if(!importer.Imp_NewTrasmissioneUtente(trasmissioneUtente,idTrasmissioneSingola))
											{
												throw new Exception();
											}
										}								
									}														
								}
							}
						}
						catch(Exception exception)
						{
							logger.Debug("Errore durante l'import di una trasmissione", exception);
						}
					}
				}
			}
			catch(Exception e)
			{
				logger.Debug("Errore durante l'import delle trasmissioni",e);
				result=false;
			}
			return result;
		}

		/// <summary>
		/// inserisce le regole di visibilità per i documenti. Nella attuale versione 3.0
		/// le regole di visibilità per ruolo vengono scomposte nelle regole di visibilità 
		/// per utente.
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="node"></param>
		/// <param name="idDocumento"></param>
		/// <returns></returns>
		public bool ImportSicurezza(XmlDocument doc, XmlNode node, string idDocumento)
		{
			bool result = true;

			try
			{
				XmlNode nodoSicurezza=node.SelectSingleNode("SICUREZZA");
			
				if(nodoSicurezza != null)
				{
					string tipo;
					string idRuolo;
					string idUtente;
					DocsPaDB.Query_DocsPAWS.ImporterXml importer=new DocsPaDB.Query_DocsPAWS.ImporterXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);
					XmlNode utenti = nodoSicurezza.SelectSingleNode("ACCESSO_UTENTE");

					// Regole di visibilità per utente
					foreach(XmlNode accesso in utenti.SelectNodes("UTENTE"))
					{
						idUtente = importer.Imp_GetIdPeopleByCodRubrica(accesso.SelectSingleNode("ID").InnerText);						
						tipo = accesso.SelectSingleNode("TIPO").InnerText;						
						
						//crea il record di sicurezza per utente
						if(!importer.Imp_NewSicurezza(idDocumento,idUtente,tipo))
						{
							logger.Debug("Errore durante l'import delle regole di sicurezza per utente.");
							result = false;
						}
					}
					
					// Regole di visibilità per ruolo
					XmlNode ruoli=nodoSicurezza.SelectSingleNode("ACCESSO_RUOLO");
					
					foreach(XmlNode accesso in ruoli.SelectNodes("RUOLO"))
					{
						idRuolo = importer.Imp_GetIdGroupByCodRubrica(accesso.SelectSingleNode("ID").InnerText);
						tipo = accesso.SelectSingleNode("DIRITTI").InnerText;						
						
						// Aggiungi ruolo
						if(!importer.Imp_NewSicurezza(idDocumento,idRuolo,tipo))
						{
							logger.Debug("Errore durante l'import delle regole di sicurezza per ruolo.");
							result = false;
						}
						
						// Legge il ruolo ed i suoi ruoli superiori e crea i relativi record di sicurezza
						if(accesso.SelectSingleNode("PRIVATO").InnerText == "0")
						{
							System.Collections.ArrayList listaRuoli = new System.Collections.ArrayList();
							
							if(idRuolo != null && !importer.GetRuoliSuperiori(ref listaRuoli, idRuolo))
							{
								logger.Debug("Errore durante l'import delle regole di sicurezza per ruolo ed i suoi ruoli superiori.");
								result = false;
							}

							foreach(string ruolo in listaRuoli)
							{
								//crea il record di sicurezza per utente
								if(!importer.Imp_NewSicurezza(idDocumento, ruolo, "A"))
								{
									logger.Debug("Errore durante l'import delle regole di sicurezza per utente.");
									result = false;
								}
							}
						}	

						// legge gli utenti del ruolo						
//						System.Data.DataSet dataSet;
//						if(!importer.Imp_GetPeopleInGroup(out dataSet,id))
//						{
//							throw new Exception();
//						}
//						
//						//crea il record di sicurezza per gli utenti del ruolo
//						foreach(System.Data.DataRow row in dataSet.Tables["PEOPLES"].Rows)
//						{
//							idUtente=row["PEOPLE_SYSTEM_ID"].ToString();
//							if(!importer.Imp_NewSicurezza(idDocumento,idUtente,tipo))
//							{
//								throw new Exception();
//							}
//						}										
					}
				}
			}
			catch(Exception e)
			{
				logger.Debug("Errore durante l'import delle regole di sicurezza",e);
				result=false;
			}

			return result;
		}


		public bool ImportFascicoli(XmlDocument doc,XmlNode node,string idDocumento)
		{
			bool result = true;
			try
			{
				XmlNode fascicoli=node.SelectSingleNode("FASCICOLATURA");
				if(fascicoli!=null)
				{
					string tipo;
					string idFascicolo;
					string idRootFolder;
					string idAmministrazione;
					DocsPaDB.Query_DocsPAWS.ImporterXml importer=new DocsPaDB.Query_DocsPAWS.ImporterXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);
					DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);
					//XmlNode utenti = nodoSicurezza.SelectSingleNode("ACCESSO_UTENTE");

					//ciclo sui fascicoli 
					foreach(XmlNode fascicolo in fascicoli.SelectNodes("FASCICOLO"))
					{
						idAmministrazione = amministrazioneXml.GetAdminByName(fascicolo.SelectSingleNode("AMMINISTRAZIONE").InnerText);
						idFascicolo= importer.Imp_GetIdFascicolo(fascicolo.SelectSingleNode("CODICE").InnerText,idAmministrazione);						
						//cerca il folder associato al fascicolo
						idRootFolder=importer.Imp_GetIdRootFolder(idFascicolo);	
						if(idRootFolder!=null)
						{
							tipo=fascicolo.SelectSingleNode("TIPO").InnerText;						
							//crea il record di associazione al fascicolo
							if(!importer.Imp_NewFascicoloDoc(idDocumento,idRootFolder,tipo))
							{
								throw new Exception();
							}
						}
					}
					
				}
			}
			catch(Exception e)
			{
				logger.Debug("Errore durante l'import della fascicolatura documenti",e);
				result=false;
			}
			return result;
		}

		/// <summary>
		/// import dei tipi atto
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		public bool ImportTipiAtto(XmlDocument doc,XmlNode node)
		{
			bool result = true;
			try
			{
				XmlNode tipiAtto=node.SelectSingleNode("TIPI_ATTO");
				if(tipiAtto!=null)
				{
					DocsPaDB.Query_DocsPAWS.ImporterXml importer=new DocsPaDB.Query_DocsPAWS.ImporterXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);

					foreach(XmlNode tipoAtto in tipiAtto.SelectNodes("TIPO"))
					{
						//ciclo sui tipi atto
						string codice = tipoAtto.InnerText;
						if(!importer.Imp_NewTipoAtto(codice))
						{
							throw new Exception();
						}
					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'importazione dei tipi atto", exception);
				result=false;
			}

			return result;

		}

		/// <summary>
		/// import dei tipi documento
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		public bool ImportTipiDocumento(XmlDocument doc,XmlNode node)
		{
			bool result = true;
			try
			{
				XmlNode tipiDocumento=node.SelectSingleNode("TIPI_DOCUMENTO");
				if(tipiDocumento!=null)
				{
					DocsPaDB.Query_DocsPAWS.ImporterXml importer=new DocsPaDB.Query_DocsPAWS.ImporterXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);

					foreach(XmlNode tipoDocumento in tipiDocumento.SelectNodes("TIPO"))
					{
						//ciclo sui tipi documento
						string codice=tipoDocumento.SelectSingleNode("ID").InnerText;
						string descrizione=tipoDocumento.SelectSingleNode("DESCRIZIONE").InnerText;
						string tipo=tipoDocumento.SelectSingleNode("TIPO_CANALE").InnerText;

						if(!importer.Imp_NewTipoDocumento(codice,descrizione,tipo))
						{
							throw new Exception();
						}

					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'importazione dei tipi documento", exception);
				result=false;
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="counter"></param>
		/// <param name="total"></param>
		/// <returns></returns>
		public bool ReadImportState(out int counter, out int total, out string message)
		{
			bool result = true; // Presume successo
			counter = 0;
			total = 0;
			message = null;

			try
			{
				string logFile = System.Configuration.ConfigurationManager.AppSettings["importExportLogPath"];
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(logFile);

				counter = Int32.Parse(xmlDocument.DocumentElement.Attributes["counter"].InnerText);
				total   = Int32.Parse(xmlDocument.DocumentElement.Attributes["totalItems"].InnerText);
				message = xmlDocument.DocumentElement.Attributes["message"].InnerText;
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura del log del caricamento dei corrispondenti.", exception);
				result = false;
			}

			return result;
		}
	}
}


