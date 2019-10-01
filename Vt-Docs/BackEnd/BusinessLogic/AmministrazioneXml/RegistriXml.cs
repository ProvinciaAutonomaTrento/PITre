using System;
using System.Xml;
using log4net;

namespace BusinessLogic.AmministrazioneXml
{
	/// <summary>
	/// Classe per la gestione dei registri di DocsPA tramite XML
	/// </summary>
	public class RegistriXml
	{
        private ILog logger = LogManager.GetLogger(typeof(RegistriXml));
		private ErrorCode errorCode;
		private XmlDocument parser;

		private DocsPaVO.utente.InfoUtente infoUtente;

		/// <summary>
		/// Acquisisce uno stream XML
		/// </summary>
		/// <param name="xmlSource"></param>
		public RegistriXml(string xmlSource)
		{
			errorCode = ErrorCode.NoError;

			try
			{
				// Validazione file XML
				parser = new XmlDocument();
				parser.LoadXml(xmlSource);
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la validazione dell'XML", exception);
				errorCode = ErrorCode.BadXmlFile;
			}
		}

		/// <summary>
		/// </summary>
		public RegistriXml()
		{
		}

		/// <summary>Ritorna l'ultimo codice di errore</summary>
		/// <returns></returns>
		public ErrorCode GetErrorCode()
		{
			return errorCode;
		}

		/// <summary>
		/// Cancella la struttura dei documenti esistenti
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
					
					if(!amministrazioneXml.ClearRegistri())
					{
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
		private string GetXmlField(string fieldName, XmlNode node, bool nulled)
		{
			string result = null;

			if(!nulled)
			{
				result = "";
			}

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
				//				userData.utente		 = GetXmlField("UTENTE",	  node);
				//				userData.password	 = GetXmlField("PASSWORD",	  node);
				//				userData.ruolo		 = GetXmlField("RUOLO",		  node);
				//				userData.idAmm		 = GetXmlField("IDAMM",		  node);
				//				userData.registro	 = GetXmlField("REGISTRO",	  node);
				//				userData.descrizione = GetXmlField("DESCRIZIONE", node);
				//				userData.codice		 = GetXmlField("CODICE",	  node);
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
		public bool CreateStructure(XmlNode node, string idAmm, bool dropAll)
		{
			bool result = true;
			errorCode = ErrorCode.NoError;
			
			try
			{
				//				if(!Login())
				//				{
				//					throw new Exception();
				//				}

				if(errorCode == ErrorCode.NoError)
				{
					XmlAttribute attribute = node.Attributes["dropAll"];

					if(attribute != null)
					{
						dropAll |= Boolean.Parse(attribute.InnerText);
					}

					if(dropAll)
					{
						this.DropAll();
					}
						
					UserData userData = new UserData(false);
					NavigateXml(node, idAmm);
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la creazione dell'XML", exception);
				errorCode = ErrorCode.GenericError;
			}

			if(errorCode != ErrorCode.NoError)
			{
				result = false;	
			}

			return result;
		}

		public bool UpdateStructure(XmlNode node, string idAmm)
		{
			bool result = true;
			errorCode = ErrorCode.NoError;
			
			try
			{
				if(errorCode == ErrorCode.NoError)
				{
					NavigateXmlForUpdate(node, idAmm);
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la lettura dell'XML", exception);
				errorCode = ErrorCode.GenericError;
			}

			if(errorCode != ErrorCode.NoError)
			{
				result = false;	
			}

			return result;
		}

		/// <summary>
		/// Procedura per la lettura dell'XML
		/// </summary>
		/// <param name="rootNode"></param>
		private void NavigateXml(XmlNode rootNode, string idAmm)
		{
			XmlNodeList nodiRegistro = rootNode.SelectNodes("REGISTRO");

			// Estrazione dati e nodi sottostanti
			foreach(XmlNode node in nodiRegistro)
			{
				string codice	   = this.GetXmlField("CODICE", node, false);
				codice=codice.ToUpper();
				string descrizione = this.GetXmlField("DESCRIZIONE", node, false);
				//Celeste
				string automatico = this.GetXmlField("AUTOMATICO", node, false);
				//fine

				// Leggi email
				XmlNode email = node.SelectSingleNode("EMAIL");
				
				if(email == null)
				{
					throw new Exception();
				}

				string indirizzo = this.GetXmlField("INDIRIZZO", email, false);
				string utente	 = this.GetXmlField("UTENTE", email, false);
				string password  = this.GetXmlField("PASSWORD", email, false);
				string smtp	     = this.GetXmlField("SMTP", email, false);
				string pop		 = this.GetXmlField("POP", email, false);
				string portaSmtp = this.GetXmlField("PORTASMTP", email, false);
				string portaPop  = this.GetXmlField("PORTAPOP", email, false);
				string usersmtp  = this.GetXmlField("USERSMTP", email, false);
				string pwdsmtp  = this.GetXmlField("PASSWORDSMTP", email, false);
				string ruoloRif  = this.GetXmlField("RUO_RIF", email, false);
				string utenteRif = this.GetXmlField("UT_RIF", email, false);

				// Inserisci il registro
				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();							
				string idRegistro = amministrazioneXml.NewRegistro(codice, 
					descrizione,
					automatico, //Celeste
					indirizzo,					
					utente,
					password,
					smtp,
					pop,
					portaSmtp,
					portaPop,
					usersmtp,
					pwdsmtp,
					idAmm);
				
				if(idRegistro == null)
				{
					throw new Exception();
				}

				/*
				if(ruoloRif!=null)
				{
					string idRuolo=amministrazioneXml.GetRuoloUOByName(ruoloRif,idAmm);
					if(idRuolo!=null)
					{
						if(amministrazioneXml.UpdateRuoloRif(idRuolo,idRegistro)==false)
						{
							logger.Debug("Errore nella impostazione ruolo di riferimento registro");
							throw new Exception();
						}
					}
					string idGruppo=amministrazioneXml.GetGroupByName(ruoloRif);
					string idUtente=amministrazioneXml.GetUserByName(utenteRif,idAmm);
					if(idUtente!=null && idGruppo!=null)
					{
						if(amministrazioneXml.UpdateUtenteRif(idUtente,idGruppo,idAmm)==false)
						{
							logger.Debug("Errore nella impostazione utente di riferimento registro");
							throw new Exception();
						}
					}
				}*/
			}
		}

		private void NavigateXmlForUpdate(XmlNode rootNode, string idAmm)
		{
			XmlNodeList nodiRegistro = rootNode.SelectNodes("REGISTRO");

			// Estrazione dati e nodi sottostanti
			foreach(XmlNode node in nodiRegistro)
			{
				XmlAttribute attribute=node.Attributes["MODE"];
				string mode="";
				if(attribute!=null)
				{
					mode=attribute.InnerText.ToUpper();
				}

				string codice	   = this.GetXmlField("CODICE", node, false);
				string descrizione = this.GetXmlField("DESCRIZIONE", node, false);
				string automatico = this.GetXmlField("AUTOMATICO", node, false); //Celeste
				descrizione=DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione);
				XmlNode email = node.SelectSingleNode("EMAIL");
				codice=codice.ToUpper();
				if(email == null)
				{
					//logger.DebugAdm (true,"Indirizzo email non valido",null);
					throw new Exception("Indirizzo email non valido");
				}
				string indirizzo = this.GetXmlField("INDIRIZZO", email, false);
				string utente	 = this.GetXmlField("UTENTE", email, false);
				string password  = this.GetXmlField("PASSWORD", email, false);
				string smtp	     = this.GetXmlField("SMTP", email, false);
				string pop		 = this.GetXmlField("POP", email, false);
				string portaSmtp = this.GetXmlField("PORTASMTP", email, false);
				string portaPop  = this.GetXmlField("PORTAPOP", email, false);
				string usersmtp  = this.GetXmlField("USERSMTP", email, false);
				string pwdsmtp   = this.GetXmlField("PASSWORDSMTP", email, false);
				string ruoloRif  = this.GetXmlField("RUO_RIF", node, false);
				string utenteRif = this.GetXmlField("UT_RIF", node, false);

				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();							

				string idRegistro=null;

				if(mode=="CREATED")
				{
					// Inserisci il registro
					/* Celeste idRegistro=amministrazioneXml.NewRegistro(codice,descrizione indirizzo,utente, */
						idRegistro=amministrazioneXml.NewRegistro(codice,descrizione,automatico,indirizzo,utente,
						password,
						smtp,
						pop,
						portaSmtp,
						portaPop,
						usersmtp,
						pwdsmtp,
						idAmm);
					if(idRegistro==null)
					{
						logger.Debug("Errore nella creazione registro");
						throw new Exception();
					}
				}

				if(mode=="MODIFIED")
				{
					// modifica il registro
					/* Celeste if(!amministrazioneXml.UpdateRegistro(codice,descrizione,indirizzo,utente, */
						if(!amministrazioneXml.UpdateRegistro(codice,descrizione,automatico,indirizzo,utente,
						password,
						smtp,
						pop,
						portaSmtp,
						portaPop,
						usersmtp,
						pwdsmtp,
						idAmm))
					{
						throw new Exception();
					}
					idRegistro=amministrazioneXml.GetRegByName(codice);
				}

				/*
				if(mode=="MODIFIED" || mode=="CREATED")
				{
					if(idRegistro!=null && ruoloRif!=null)
					{
						string idRuolo=amministrazioneXml.GetRuoloUOByName(ruoloRif,idAmm);
						if(idRuolo!=null)
						{
							if(amministrazioneXml.UpdateRuoloRif(idRuolo,idRegistro)==false)
							{
								logger.Debug("Errore nella impostazione ruolo di riferimento registro");
								throw new Exception();
							}
						}
						string idGruppo=amministrazioneXml.GetGroupByName(ruoloRif);
						string idUtente=amministrazioneXml.GetUserByName(utenteRif,idAmm);
						if(idUtente!=null && idGruppo!=null)
						{
							if(amministrazioneXml.UpdateUtenteRif(idUtente,idGruppo,idAmm)==false)
							{
								logger.Debug("Errore nella impostazione utente di riferimento registro");
								throw new Exception();
							}
						}
					}
				}*/

				if(mode=="DELETED")
				{
					//TO DO
					//cancellazione registro, da valutare
				}

			}
		}

		public bool ExportStructure(XmlDocument doc,XmlNode amministrazione,string idAmm)
		{
			bool result=true; //presume successo
			try
			{
				System.Data.DataSet dataSetRegistri;
				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();							
				result=amministrazioneXml.Exp_GetRegistri(out dataSetRegistri,idAmm);
				if(!result)
				{
					throw new Exception();
				}

				if(dataSetRegistri!= null)
				{
					if(dataSetRegistri.Tables["REGISTRI"].Rows.Count>0)
					{
						XmlNode registri=amministrazione.AppendChild (doc.CreateElement("REGISTRI"));

						foreach( System.Data.DataRow rowRegistro in dataSetRegistri.Tables["REGISTRI"].Rows)
						{
							XmlNode registro=registri.AppendChild (doc.CreateElement("REGISTRO"));
							registro.AppendChild(doc.CreateElement("DESCRIZIONE")).InnerText	= rowRegistro["VAR_DESC_REGISTRO"].ToString ();
							registro.AppendChild(doc.CreateElement("CODICE")).InnerText			= rowRegistro["VAR_CODICE"].ToString ().ToUpper();
							registro.AppendChild(doc.CreateElement("AUTOMATICO")).InnerText		= rowRegistro["CHA_AUTOMATICO"].ToString ().ToUpper();

							XmlNode registroMail=registro.AppendChild (doc.CreateElement("EMAIL"));
							registroMail.AppendChild(doc.CreateElement("INDIRIZZO")).InnerText	= rowRegistro["VAR_EMAIL_REGISTRO"].ToString ();
							registroMail.AppendChild(doc.CreateElement("UTENTE")).InnerText		= rowRegistro["VAR_USER_MAIL"].ToString();
							registroMail.AppendChild(doc.CreateElement("PASSWORD")).InnerText	= rowRegistro["VAR_PWD_MAIL"].ToString();
							registroMail.AppendChild(doc.CreateElement("SMTP")).InnerText		= rowRegistro["VAR_SERVER_SMTP"].ToString();
							registroMail.AppendChild(doc.CreateElement("POP")).InnerText		= rowRegistro["VAR_SERVER_POP"].ToString();
							registroMail.AppendChild(doc.CreateElement("PORTASMTP")).InnerText	= rowRegistro["NUM_PORTA_SMTP"].ToString();
							registroMail.AppendChild(doc.CreateElement("PORTAPOP")).InnerText	= rowRegistro["NUM_PORTA_POP"].ToString();
							registroMail.AppendChild(doc.CreateElement("USERSMTP")).InnerText	= rowRegistro["VAR_USER_SMTP"].ToString();
							registroMail.AppendChild(doc.CreateElement("PASSWORDSMTP")).InnerText	= rowRegistro["VAR_PWD_SMTP"].ToString();
							//string ruoloRif=amministrazioneXml.GetRuoloRif(rowRegistro["SYSTEM_ID"].ToString (),idAmm);
							string ruoloRif="";
							registro.AppendChild(doc.CreateElement("RUO_RIF")).InnerText		= ruoloRif;
							/*string utenteRif=amministrazioneXml.GetUtenteRif(ruoloRif,idAmm);
							if(utenteRif==null) 
							{
								utenteRif="";
							}
							else
							{
								utenteRif=utenteRif.ToUpper();
							}*/
							string utenteRif="";
							registro.AppendChild(doc.CreateElement("UT_RIF")).InnerText =utenteRif;
						}
					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'esportazione delle ragioni di trasmissione", exception);
				result=false;
			}
			return result;
		}
	}
}
