using System;
using System.Xml;
using log4net;

namespace BusinessLogic.AmministrazioneXml
{
	/// <summary>
	/// Classe per la gestione delle amministrazioni di DocsPA tramite XML
	/// </summary>
	public class AmministrazioniXml
	{
        private ILog logger = LogManager.GetLogger(typeof(AmministrazioniXml));
		private ErrorCode errorCode;
		private XmlDocument parser;

		private DocsPaVO.utente.InfoUtente infoUtente;

		private bool dropChildren = false;

		/// <summary>
		/// 
		/// </summary>
		public AmministrazioniXml()
		{
			errorCode = ErrorCode.NoError;
		}

		/// <summary>
		/// Acquisisce uno stream XML
		/// </summary>
		public AmministrazioniXml(string xmlDoc)
		{
			errorCode = ErrorCode.NoError;

			try
			{
				// Validazione file XML
				parser = new XmlDocument();
				parser.LoadXml(xmlDoc);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la validazione dell'XML", exception);
				errorCode = ErrorCode.BadXmlFile;
			}
		}

		/// <summary>
		/// Carica un file XML
		/// </summary>
		public AmministrazioniXml(string xmlPath, out bool xmlLoadResult)
		{
			errorCode = ErrorCode.NoError;
			xmlLoadResult = true;

			try
			{
				// Validazione file XML
				parser = new XmlDocument();
				parser.Load(xmlPath);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la validazione dell'XML", exception);
				errorCode = ErrorCode.BadXmlFile;
				xmlLoadResult = false;
			}
		}

		/// <summary>Ritorna l'ultimo codice di errore</summary>
		/// <returns></returns>
		public ErrorCode GetErrorCode()
		{
			return errorCode;
		}

		/// <summary>
		/// Cancella tutte le amministrazioni
		/// </summary>
		/// <returns></returns>
		public bool DropAll()
		{
			bool result = true;
			
			try
			{
				if(errorCode == ErrorCode.NoError)
				{
					DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
					if(!amministrazioneXml.ClearAmministrazioni())
					{
						logger.Debug("Errore cancellazione amministrazioni");
						throw new Exception();
					}
					amministrazioneXml.Dispose();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione del titolario", exception);
				errorCode = ErrorCode.GenericError;
			}

			if(errorCode != ErrorCode.NoError)
			{
				result = false;	
			}

			return result;
		}

		/// <summary>
		/// Caricamento dati per login
		/// </summary>
		/// <returns></returns>
		private bool Login()
		{
			bool result = true; // Presume successo

			try
			{
				UserData userData = new UserData(true);
				GetUserData(ref userData, parser.DocumentElement.SelectSingleNode("DATI"));

				DocsPaVO.utente.UserLogin userLogin = new DocsPaVO.utente.UserLogin(userData.utente, userData.password, userData.idAmm, "");

				DocsPaVO.utente.Utente utente;
				DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();
				DocsPaVO.utente.UserLogin.LoginResult loginResult;
				userManager.LoginUser(userLogin, out utente, out loginResult);
				
				DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

				this.infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la login.", exception);
				result = false;
			}

			return result;
		}

		/// <summary>
		/// Ritorna il testo contenuto in un dato tag XML
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		private string GetXmlField(string fieldName, XmlNode node)
		{
			string result = null;

			XmlNode child = node.SelectSingleNode(fieldName);

			if(child != null)
			{
				if(child.InnerText != "")
				{
					result = child.InnerText;
				}
			}

			return result;
		}

		/// <summary>
		/// </summary>
		/// <param name="userData"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		private bool GetUserData(ref UserData userData, XmlNode node)
		{
			bool result = true; // Presume successo
			
			try
			{
				userData.utente		 = GetXmlField("UTENTE",	  node);
				userData.password	 = GetXmlField("PASSWORD",	  node);
				userData.ruolo		 = GetXmlField("RUOLO",		  node);
				userData.idAmm		 = GetXmlField("IDAMM",		  node);
				userData.registro	 = GetXmlField("REGISTRO",	  node);
				userData.descrizione = GetXmlField("DESCRIZIONE", node);
				userData.codice		 = GetXmlField("CODICE",	  node);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dei dati utente.", exception);
				result = false;
			}

			return result;
		}

		/// <summary>
		/// Crea la struttura documentale a partire dall'XML passato al costruttore
		/// </summary>
		/// <returns></returns>
		public bool CreateStructure()
		{
			bool result = true;
			errorCode = ErrorCode.NoError;
			
			try
			{
				if(errorCode == ErrorCode.NoError)
				{
					DocsPaUtils.LogsManagement.ImportExportLogger.ResetLog("import", 0);
					UserData userData = new UserData(false);
					NavigateXml(parser.DocumentElement);
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante il caricamento dell'XML", exception);
				errorCode = ErrorCode.GenericError;
			}

			if(errorCode != ErrorCode.NoError)
			{
				result = false;	
			}

			DocsPaUtils.LogsManagement.ImportExportLogger.ResetLog("import", 0);

			return result;
		}

		/// <summary>
		/// aggiorna la struttura documentale a partire dall'XML passato al costruttore
		/// </summary>
		/// <returns></returns>
		public bool UpdateStructure()
		{
			bool result = true;
			errorCode = ErrorCode.NoError;
			
			try
			{
				if(errorCode == ErrorCode.NoError)
				{
					XmlAttribute attribute = parser.DocumentElement.SelectSingleNode("AMMINISTRAZIONE").Attributes["MODE"];

					UserData userData = new UserData(false);
					NavigateXmlForUpdate(parser.DocumentElement);
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante il caricamento dell'XML", exception);
				errorCode = ErrorCode.GenericError;
			}

			if(errorCode != ErrorCode.NoError)
			{
				result = false;	
			}

			return result;
		}


		public XmlDocument ExportStructureXmlDocument()
		{
			return ExportStructureCommon();
		}

		public bool ExportStructure()
		{
			bool result=true;
			XmlDocument doc=ExportStructureCommon();
			if(doc!=null)
			{
				result=true;
				doc.Save("C:\\titolario.xml");
			}
			return result;
		}

		public XmlDocument ExportStructureCommon()
		{
			bool result = false;
			XmlDocument doc =null;
			try
			{
				doc = new XmlDocument();

				//Create the root element and add it to the document.
				XmlNode root=doc.AppendChild(doc.CreateElement("DOCSPA"));

				//legge i server
				/*
				BusinessLogic.AmministrazioneXml.ServerXml serverXml=new BusinessLogic.AmministrazioneXml.ServerXml();
				result=serverXml.ExportStructure(doc,root);
				if(!result)
				{
					logger.Debug("Errore esportazione server");
					throw new Exception();
				}*/

				//esporta l'anagrafica delle funzioni elementari
				BusinessLogic.AmministrazioneXml.FunzioniXml funzioniXml=new BusinessLogic.AmministrazioneXml.FunzioniXml();
				result=funzioniXml.ExportAnagraficaFunzioni(doc,root);
				if(!result)
				{
					logger.Debug("Errore esportazione funzioni elementari");
					throw new Exception();
				}

				//esportazione tipi funzione
				result=funzioniXml.ExportStructure(doc,root);
				if(!result)
				{
					logger.Debug("Errore esportazione funzioni");
					throw new Exception();
				}

				//esportazione ragioni trasmissioni generali
				BusinessLogic.AmministrazioneXml.RagioniTrasmissioneXml ragioniXml=new BusinessLogic.AmministrazioneXml.RagioniTrasmissioneXml();
				result=ragioniXml.ExportStructure(doc,root,null);
				if(!result)
				{
					logger.Debug("Errore esportazione ragioni trasmissioni generali");
					throw new Exception();
				}

				//legge le amministrazioni
				System.Data.DataSet dataSet;
				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml=new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
				result=amministrazioneXml.Exp_GetAmministrazioni(out dataSet);
				if(!result)
				{
					logger.Debug("Errore esportazione amministrazioni");
					throw new Exception();
				}
				if(dataSet != null)
				{
					foreach( System.Data.DataRow row in dataSet.Tables["AMMINISTRAZIONI"].Rows)
					{
						string idAmm=row["SYSTEM_ID"].ToString ();

						//esportazione dati della amministrazione
						XmlNode amministrazione=root.AppendChild (doc.CreateElement("AMMINISTRAZIONE"));
						XmlNode dati=amministrazione.AppendChild (doc.CreateElement("DATI"));

						dati.AppendChild(doc.CreateElement("CODICE")).InnerText = row["VAR_CODICE_AMM"].ToString ().ToUpper();
						dati.AppendChild(doc.CreateElement("DESCRIZIONE")).InnerText = row["VAR_DESC_AMM"].ToString ();
						dati.AppendChild(doc.CreateElement("LIBRERIA")).InnerText = row["VAR_LIBRERIA"].ToString ();
						dati.AppendChild(doc.CreateElement("SEGNATURA")).InnerText = row["VAR_FORMATO_SEGNATURA"].ToString();
						dati.AppendChild(doc.CreateElement("FASCICOLATURA")).InnerText = row["VAR_FORMATO_FASCICOLATURA"].ToString();
						//new gadamo
						dati.AppendChild(doc.CreateElement("SERVERSMTP")).InnerText = row["VAR_SMTP"].ToString();
						dati.AppendChild(doc.CreateElement("PORTASMTP")).InnerText = row["NUM_PORTA_SMTP"].ToString();
						dati.AppendChild(doc.CreateElement("USERSMTP")).InnerText = row["VAR_USER_SMTP"].ToString();
						dati.AppendChild(doc.CreateElement("PASSWORDSMTP")).InnerText = row["VAR_PWD_SMTP"].ToString();
						dati.AppendChild(doc.CreateElement("PROTOCOLLO_INTERNO")).InnerText = row["CHA_PROTOINT"].ToString();
						dati.AppendChild(doc.CreateElement("RAGIONE_TO")).InnerText = row["VAR_RAGIONE_TO"].ToString();
						dati.AppendChild(doc.CreateElement("RAGIONE_CC")).InnerText = row["VAR_RAGIONE_CC"].ToString();
						dati.AppendChild(doc.CreateElement("DOMINIO")).InnerText = row["VAR_DOMINIO"].ToString();
						// old
						#region codice commentato
//						if(row["VAR_FORMATO_FASCICOLATURA"]!=null)
//						{
//							dati.AppendChild(doc.CreateElement("DOMINIO")).InnerText = row["VAR_DOMINIO"].ToString();
//						}
//						if(row["VAR_SMTP"]!=null)
//						{
//							dati.AppendChild(doc.CreateElement("SERVERSMTP")).InnerText = row["VAR_SMTP"].ToString();
//						}
//						if(row["NUM_PORTA_SMTP"]!=null)
//						{
//							dati.AppendChild(doc.CreateElement("PORTASMTP")).InnerText = row["NUM_PORTA_SMTP"].ToString();
//						}
//						string temp="";
//						if(row["CHA_PROTOINT"]!=null)
//						{
//							if(row["CHA_PROTOINT"].ToString()!="") temp="1";
//						}
//						dati.AppendChild(doc.CreateElement("PROTOCOLLO_INTERNO")).InnerText = temp;
//
//						temp="";
//						if(row["VAR_RAGIONE_TO"]!=null)
//						{
//							temp=row["VAR_RAGIONE_TO"].ToString();
//						}
//						dati.AppendChild(doc.CreateElement("RAGIONE_TO")).InnerText = temp;
//
//						temp="";
//						if(row["VAR_RAGIONE_CC"]!=null)
//						{
//							temp=row["VAR_RAGIONE_CC"].ToString();
//						}
//						dati.AppendChild(doc.CreateElement("RAGIONE_CC")).InnerText = temp;
						#endregion

						//esportazione ragioni trasmissione
						BusinessLogic.AmministrazioneXml.RagioniTrasmissioneXml ragioniGenXml=new BusinessLogic.AmministrazioneXml.RagioniTrasmissioneXml ();
						result=ragioniGenXml.ExportStructure(doc,amministrazione,idAmm);
						if(!result)
						{
							logger.Debug("Errore esportazione ragioni trasmissione");
							throw new Exception();
						}

						//esportazione registri
						BusinessLogic.AmministrazioneXml.RegistriXml registriXml=new BusinessLogic.AmministrazioneXml.RegistriXml();
						result=registriXml.ExportStructure(doc,amministrazione,idAmm);
						if(!result)
						{
							logger.Debug("Errore esportazione registri");
							throw new Exception();
						}

						//esportazione ruoli
						BusinessLogic.AmministrazioneXml.RuoliXml ruoliXml=new BusinessLogic.AmministrazioneXml.RuoliXml();
						result=ruoliXml.ExportStructure(doc,amministrazione,idAmm);
						if(!result)
						{
							logger.Debug("Errore esportazione ruoli");
							throw new Exception();
						}

						//esportazione utenti
//						BusinessLogic.AmministrazioneXml.UtentiXml utentiXml=new BusinessLogic.AmministrazioneXml.UtentiXml();
//						result=utentiXml.ExportStructure(doc,amministrazione,idAmm);
//						if(!result)
//						{
//							logger.Debug("Errore esportazione utenti");
//							throw new Exception();
//						}

						//esportazione organigramma
//						BusinessLogic.AmministrazioneXml.OrganigrammaXml organigramma=new BusinessLogic.AmministrazioneXml.OrganigrammaXml();
//						result=organigramma.ExportStructure(doc,amministrazione,idAmm,"0");
//						if(!result)
//						{
//							logger.Debug("Errore esportazione organigramma");
//							throw new Exception();
//						}

					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione dell'XML", exception);
				doc=null;
			}

			logger.Debug("Amministrazione: Export XML amministrazioni: OK!");
			return doc;
		}

		/// <summary>
		/// Procedura per la lettura dell'XML
		/// </summary>
		/// <param name="rootNode"></param>
		private void NavigateXml(XmlNode rootNode)
		{
			// Servers
			/*
			XmlNode servers= rootNode.SelectSingleNode("SERVERS");

			DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage(1, 10, "Import dei server.");

			if(servers!=null)
			{
				ServerXml serverXml = new ServerXml();
					
				if(!serverXml.CreateStructure(servers))
				{
					logger.Debug("Errore creazione servers");
					throw new Exception();
				}
			}*/

				XmlNodeList nodiAmministrazione = rootNode.SelectNodes("AMMINISTRAZIONE");

			// Estrazione dati e nodi sottostanti
			foreach(XmlNode node in nodiAmministrazione)
			{
				DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage(2, 10, "Creazione amministrazioni.");

				//legge l'attributo dropAll ed esegue il drop dell'amministrazione se richiesto
				dropChildren = false;
				XmlAttribute attribute = node.Attributes["dropAll"];
				if(attribute != null)
				{
					bool dropAll = Boolean.Parse(attribute.InnerText);
					if(dropAll)
					{
						this.DropAll();
						dropChildren = true;
					}
				}

				XmlNode dati = node.SelectSingleNode("DATI");
				if(dati == null)
				{
					logger.Debug("Errore mancanza corpo DATI amministrazione");
					throw new Exception();
				}

				string codice	     = this.GetXmlField("CODICE", dati);
				string descrizione   = this.GetXmlField("DESCRIZIONE", dati);				
				string libreria      = this.GetXmlField("LIBRERIA", dati);
				string segnatura     = this.GetXmlField("SEGNATURA", dati);
				string fascicolatura = this.GetXmlField("FASCICOLATURA", dati);
				string dominio		 = this.GetXmlField("DOMINIO", dati);
				string serversmtp    = this.GetXmlField("SERVERSMTP", dati);
				string portasmtp     = this.GetXmlField("PORTASMTP", dati);
				string usersmtp     = this.GetXmlField("USERSMTP", dati);
				string pwdsmtp     = this.GetXmlField("PASSWORDSMTP", dati);
				if(portasmtp==null || portasmtp=="") portasmtp="NULL";
				string protocolloInt = this.GetXmlField("PROTOCOLLO_INTERNO", dati);
				if(protocolloInt!=null && protocolloInt!="") protocolloInt="1";
				string ragioneTO     = this.GetXmlField("RAGIONE_TO", dati);
				string ragioneCC     = this.GetXmlField("RAGIONE_CC", dati);

				codice=codice.ToUpper();

				DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage(3, 10, "Import amministrazione: " + codice);

				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();							
				string idAmm = amministrazioneXml.NewAmministrazione(codice, descrizione, libreria, segnatura, fascicolatura,dominio,serversmtp,portasmtp,protocolloInt,ragioneTO,ragioneCC,usersmtp,pwdsmtp);
				
				if(idAmm == null)
				{
					logger.Debug("Errore nella creazione nuova amministrazione");
					throw new Exception();
				}
				
				// Ragioni Trasmissione
				DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage(4, 10, "Import ragioni trasmissione dell'amministrazione: " + codice);

				XmlNode ragioni = node.SelectSingleNode("RAGIONITRASMISSIONE");
				RagioniTrasmissioneXml ragioniTrasmissioneXml = new RagioniTrasmissioneXml();
				
				if(!ragioniTrasmissioneXml.CreateStructure(ragioni, idAmm, dropChildren))
				{
					logger.Debug("Errore nella creazione delle ragioni trasmissione");
					throw new Exception();
				}

				// Registri
				DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage(5, 10, "Import registri dell'amministrazione: " + codice);
				XmlNode registri = node.SelectSingleNode("REGISTRI");
				RegistriXml registriXml = new RegistriXml();
				
				if(!registriXml.CreateStructure(registri, idAmm, dropChildren))
				{
					logger.Debug("Errore nella creazione dei registri");
					throw new Exception();
				}

				// Funzioni
				DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage(6, 10, "Import funzioni dell'amministrazione: " + codice);
				XmlNode funzioni = node.SelectSingleNode("TIPIFUNZIONE");
				FunzioniXml funzioniXml = new FunzioniXml();
				
				if(!funzioniXml.CreateStructure(funzioni, idAmm, dropChildren))
				{
					logger.Debug("Errore nella creazione dei tipi funzione");
					throw new Exception();
				}

				// Tipi Ruolo
				DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage(7, 10, "Import tipi ruolo dell'amministrazione: " + codice);
				XmlNode ruoli = node.SelectSingleNode("RUOLI");
				RuoliXml ruoliXml = new RuoliXml();
				
				if(!ruoliXml.CreateStructure(ruoli, idAmm, dropChildren))
				{
					logger.Debug("Errore nella creazione dei ruoli");
					throw new Exception();
				}

//				// Utenti
//				DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage(8, 10, "Import utenti dell'amministrazione: " + codice);
//				XmlNode utenti = node.SelectSingleNode("UTENTI");
//				UtentiXml utentiXml = new UtentiXml();
//				
//				if(!utentiXml.CreateStructure(utenti, idAmm, dropChildren))
//				{
//					logger.Debug("Errore nella creazione degli utenti");
//					throw new Exception();
//				}
//
//				// Organigramma
//				DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage(9, 10, "Import organigramma dell'amministrazione: " + codice);
//				XmlNode organigramma = node.SelectSingleNode("ORGANIGRAMMA");
//				OrganigrammaXml organigrammaXml = new OrganigrammaXml();
//				
//				if(!organigrammaXml.CreateStructure(organigramma, idAmm, dropChildren))
//				{
//					logger.Debug("Errore nella creazione dell'organigramma");
//					throw new Exception();
//				}

				DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage(10, 10, "Import dell'amministrazione completo");
			}
		}

		private void NavigateXmlForUpdate(XmlNode rootNode)
		{
			DocsPaUtils.LogsManagement.ProgressLogger pl=DocsPaUtils.LogsManagement.ProgressLogger.getInstance(false);
			
			try
			{
				/*XmlNode servers= rootNode.SelectSingleNode("SERVERS");
				if(servers!=null)
				{
					int idMessage=pl.NewMessage("Caricamento Servers di Posta");
					ServerXml serverXml = new ServerXml();
					
					if(!serverXml.UpdateStructure(servers))
					{
						pl.UpdateMessage(idMessage,null,true,true);
						logger.Debug("Errore aggiornamento servers");
						throw new Exception();
					}
					pl.UpdateMessage(idMessage,null,true,false);
				}*/

				// Funzioni
				XmlNode funzioni = rootNode.SelectSingleNode("TIPIFUNZIONE");
				if(funzioni!=null)
				{
					int idMessage=pl.NewMessage("Caricamento Tipi Funzione");
					logger.Debug("Caricamento Tipi Funzione...");
					
					FunzioniXml funzioniXml = new FunzioniXml();
				
					if(!funzioniXml.UpdateStructure(funzioni))
					{
						pl.UpdateMessage(idMessage,null,true,true);
						logger.Debug("Errore aggiornamento tipi funzione");
						throw new Exception();
					}
					pl.UpdateMessage(idMessage,null,true,false);
				}

				
				XmlNodeList nodiAmministrazione = rootNode.SelectNodes("AMMINISTRAZIONE");

				// Estrazione dati e nodi sottostanti
				foreach(XmlNode node in nodiAmministrazione)
				{
					
					XmlNode dati = node.SelectSingleNode("DATI");
					if(dati == null)
					{
						logger.Debug("Errore mancanza corpo DATI amministrazione");
						//logger.DebugAdm(true,"Errore mancanza corpo DATI tag Amministrazione",null);
						throw new Exception();
					}

					DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
					string idAmm="";
					string mode="";
					XmlAttribute attribute=node.Attributes["MODE"];
					if(attribute!=null)
					{
						mode=attribute.InnerText.ToUpper();
					}
				
					string codice	     = this.GetXmlField("CODICE", dati);
					string descrizione   = this.GetXmlField("DESCRIZIONE", dati);
					string libreria      = this.GetXmlField("LIBRERIA", dati);
					string segnatura     = this.GetXmlField("SEGNATURA", dati);
					string fascicolatura = this.GetXmlField("FASCICOLATURA", dati);
					string dominio	     = this.GetXmlField("DOMINIO", dati);
					string serversmtp    = this.GetXmlField("SERVERSMTP", dati);
					string portasmtp     = this.GetXmlField("PORTASMTP", dati);
					if(portasmtp==null || portasmtp=="") portasmtp="NULL";
					string usersmtp		 = this.GetXmlField("USERSMTP", dati);
					string pwdsmtp		 = this.GetXmlField("PASSWORDSMTP", dati);
					string protocolloInt = this.GetXmlField("PROTOCOLLO_INTERNO", dati);
					if(protocolloInt!=null && protocolloInt!="") protocolloInt="1";
					string ragioneTO     = this.GetXmlField("RAGIONE_TO", dati);
					string ragioneCC     = this.GetXmlField("RAGIONE_CC", dati);

					codice=codice.ToUpper();
					descrizione=DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione);

					int idMessage=pl.NewMessage("Caricamento Amministrazione: " + codice);
					logger.Debug("Caricamento Amministrazione: " + codice);

					if(mode=="CREATED")
					{
						//aggiunge la nuova amministrazione
						idAmm = amministrazioneXml.NewAmministrazione(codice, descrizione, libreria, segnatura, fascicolatura,dominio,serversmtp,portasmtp,protocolloInt,ragioneTO,ragioneCC,usersmtp,pwdsmtp);
						if(idAmm == null)
						{
							pl.UpdateMessage(idMessage,null,true,true);
							logger.Debug("Errore nella creazione nuova amministrazione");
							throw new Exception();
						}
						DocsPaDB.Utils.Personalization.Reset();
					}
					else if(mode=="MODIFIED")
					{
						//modifica i dati della amministrazione
						idAmm = amministrazioneXml.GetAdminByName(codice);
						if(!amministrazioneXml.UpdateAmministrazione(codice, descrizione, libreria, segnatura, fascicolatura,dominio,serversmtp,portasmtp,protocolloInt,ragioneTO,ragioneCC,usersmtp,pwdsmtp))
						{
							pl.UpdateMessage(idMessage,null,true,true);
							logger.Debug("Errore aggiornamento nuova amministrazione");
							throw new Exception();
						}
						DocsPaDB.Utils.Personalization.Reset();
					}
					else if(mode=="DELETED")
					{
						//TO DO - > cancellazione della amministrazione
					}
					else
					{
						//in a4ltri casi (mode="") seleziona l'id
						idAmm = amministrazioneXml.GetAdminByName(codice);
						if(idAmm == null)
						{
							pl.UpdateMessage(idMessage,null,true,true);
							logger.Debug("Amministrazione: " + codice + " sconosciuta");
							//logger.DebugAdm(true,"Amministrazione: " + codice + " sconosciuta",null);
							throw new Exception();
						}
					}
				
					//si analizza il contenuto della amministrazione solo se non è stata cancellata
					if(mode!="DELETED")
					{
						// Ragioni Trasmissione
						
						XmlNode ragioni = node.SelectSingleNode("RAGIONITRASMISSIONE");
						if(ragioni!=null)
						{
							int idMessageAmm=pl.NewMessage("Caricamento Ragioni Trasmissione");
							logger.Debug("Caricamento Ragioni Trasmissione...");

							RagioniTrasmissioneXml ragioniTrasmissioneXml = new RagioniTrasmissioneXml();
				
							if(!ragioniTrasmissioneXml.UpdateStructure(ragioni, idAmm))
							{
								pl.UpdateMessage(idMessageAmm,null,true,true);
								logger.Debug("Errore aggiornamento ragioni trasmissione");
								throw new Exception();
							}
							pl.UpdateMessage(idMessageAmm,null,true,false);
						}

						// Registri
						XmlNode registri = node.SelectSingleNode("REGISTRI");
						if(registri!=null)
						{
							int idMessageAmm=pl.NewMessage("Caricamento Registri");
							logger.Debug("Caricamento Registri...");

							RegistriXml registriXml = new RegistriXml();
				
							if(!registriXml.UpdateStructure(registri, idAmm))
							{
								pl.UpdateMessage(idMessageAmm,null,true,true);
								logger.Debug("Errore aggiornamento registri");
								throw new Exception();
							}
							pl.UpdateMessage(idMessageAmm,null,true,false);
						}

//						// Funzioni
//						XmlNode funzioni = node.SelectSingleNode("TIPIFUNZIONE");
//						if(funzioni!=null)
//						{
//							FunzioniXml funzioniXml = new FunzioniXml();
//				
//							if(!funzioniXml.UpdateStructure(funzioni, idAmm))
//							{
//								logger.Debug("Errore aggiornamento tipi funzione");
//								throw new Exception();
//							}
//						}

						// Tipi Ruolo
						XmlNode ruoli = node.SelectSingleNode("RUOLI");
						if(ruoli!=null)
						{
							int idMessageAmm=pl.NewMessage("Caricamento Ruoli");
							logger.Debug("Caricamento Tipo Ruoli...");

							RuoliXml ruoliXml = new RuoliXml();
				
							if(!ruoliXml.UpdateStructure(ruoli, idAmm))
							{
								pl.UpdateMessage(idMessageAmm,null,true,true);
								logger.Debug("Errore aggiornamento ruoli");
								throw new Exception();
							}
							pl.UpdateMessage(idMessageAmm,null,true,false);
						}

						// Utenti
						
//						XmlNode utenti = node.SelectSingleNode("UTENTI");
//						if(utenti!=null)
//						{
//							int idMessageAmm=pl.NewMessage("Caricamento Utenti");
//							UtentiXml utentiXml = new UtentiXml();
//				
//							if(!utentiXml.UpdateStructure(utenti, idAmm))
//							{
//								pl.UpdateMessage(idMessageAmm,null,true,true);
//								logger.Debug("Errore aggiornamento utenti");
//								throw new Exception();
//							}
//							pl.UpdateMessage(idMessageAmm,null,true,false);
//						}

						// Organigramma
//						XmlNode organigramma = node.SelectSingleNode("ORGANIGRAMMA");
//						if(organigramma!=null)
//						{
//							int idMessageAmm=pl.NewMessage("Caricamento Organigramma");
//							OrganigrammaXml organigrammaXml = new OrganigrammaXml();
//				
//							if(!organigrammaXml.UpdateStructure(organigramma, idAmm))
//							{
//								pl.UpdateMessage(idMessageAmm,null,true,true);
//								logger.Debug("Errore aggiornamento organigramma");
//								throw new Exception();
//							}
//							pl.UpdateMessage(idMessageAmm,null,true,false);
//						}
					}
					pl.UpdateMessage(idMessage,null,true,false);
				}
				
			}
			catch(Exception e)
			{
				logger.Debug("Errore durante la navigazione ricorsiva per aggiornamento",e);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="password"></param>
		/// <returns>
		/// 1-> errore utente sconosciuto
		/// 2-> errore amministratore già loggato
		/// 0-> ok
		/// </returns>
		public string CheckAdminLogin(string user,string password)
		{
			string result=null;
			DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazione=new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();			
			if(amministrazione.CheckAdminLogin(user,password)==true)
			{
				//amministratore valido, verifica DPA_LOCK
				if(user.ToUpper()=="ADMINISTRATOR")
				{
					amministrazione.DeleteUniqueAdmin();
				}
				result=amministrazione.CheckUniqueAdmin(user);
				if(result==null)
				{
					amministrazione.SetUniqueAdmin(user);
					result=user;
				}
			}
			logger.Debug("Login amministratore: utente = " + user + ": esito = " + result);
			return result;
		}

		public bool CheckAdminLogged(string user)
		{
			bool result=false;
			DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazione=new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();			
			if(amministrazione.CheckUniqueAdmin(user)==user)
			{
				result=true;
			}
			return result;
		}

		public bool ChangeAdminPwd(string user,string password)
		{
			bool result;
			DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazione=new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
			result=amministrazione.ChangeAdminPwd(user,password);
			return result;
		}

		public bool LogOutAdmin()
		{
			DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazione=new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
			return amministrazione.DeleteUniqueAdmin();
		}
		
	}
}
