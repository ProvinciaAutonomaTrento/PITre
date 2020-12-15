using System;
using System.Xml;
using log4net;

namespace BusinessLogic.AmministrazioneXml
{
	/// <summary>
	/// Classe per la gestione dei registri di DocsPA tramite XML
	/// </summary>
	public class RuoliXml
	{
        private ILog logger = LogManager.GetLogger(typeof(RuoliXml));
		private ErrorCode errorCode;
		private XmlDocument parser;

		private DocsPaVO.utente.InfoUtente infoUtente;

		/// <summary>
		/// Acquisisce uno stream XML
		/// </summary>
		/// <param name="xmlSource"></param>
		public RuoliXml(string xmlSource)
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
		public RuoliXml()
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
					
					if(!amministrazioneXml.ClearTipiRuolo())
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
						
					//					UserData userData = new UserData(false);
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
				logger.Debug("Errore durante la creazione dell'XML", exception);
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
			try
			{
				XmlNodeList nodiRuolo = rootNode.SelectNodes("RUOLO");

				// Estrazione dati e nodi sottostanti
				foreach(XmlNode node in nodiRuolo)
				{
					// Leggi dati
					string codice	   = this.GetXmlField("CODICE", node, false);
					string descrizione = this.GetXmlField("DESCRIZIONE", node, false);
					int livello = Int32.Parse(this.GetXmlField("LIVELLO", node, false));
					codice=codice.ToUpper();

					DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage(7, 10, "Import tipo ruolo: " + codice);

					DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
					string idRuolo = amministrazioneXml.NewRuolo(idAmm, codice, descrizione, livello);

					if(idRuolo == null)
					{
						throw new Exception();
					}

				}
			}
			catch(Exception e)
			{
				logger.Debug("Errore durante l'importazione dei ruoli", e);
			}

		}

		private void NavigateXmlForUpdate(XmlNode rootNode, string idAmm)
		{
			try
			{
				XmlNodeList nodiRuolo = rootNode.SelectNodes("RUOLO");

				// Estrazione dati e nodi sottostanti
				foreach(XmlNode node in nodiRuolo)
				{
					XmlAttribute attribute=node.Attributes["MODE"];
					string mode="";
					if(attribute!=null)
					{
						mode=attribute.InnerText.ToUpper(); 
					}
				
					string codice = this.GetXmlField("CODICE", node, false);
					string descrizione = this.GetXmlField("DESCRIZIONE", node, false);
					descrizione=DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione);
					int livello = Int32.Parse(this.GetXmlField("LIVELLO", node, false));
					codice=codice.ToUpper();
					DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();

					if(mode=="CREATED")
					{
						string idRuolo = amministrazioneXml.NewRuolo(idAmm, codice, descrizione, livello);
						if(idRuolo==null)
						{
							throw new Exception();
						}
					}

					if(mode=="MODIFIED")
					{
						if(!amministrazioneXml.UpdateRuolo(idAmm, codice, descrizione, livello))
						{
							throw new Exception();
						}
					}

					if(mode=="DELETED")
					{
						if(!amministrazioneXml.DeleteRuolo(idAmm, codice))
						{
							throw new Exception();
						}
					}
				}
			}
			catch(Exception e)
			{
				logger.Debug("Errore durante l'update dei ruoli", e);
			}
		}

		public bool ExportStructure(XmlDocument doc,XmlNode amministrazione,string idAmm)
		{
			bool result=true; //presume successo
			try
			{
				System.Data.DataSet dataSetRuoli;
				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
				result=amministrazioneXml.Exp_GetRuoli(out dataSetRuoli,idAmm);
				if(!result)
				{
					throw new Exception();
				}

				if(dataSetRuoli!= null)
				{
					XmlNode ruolo;
					XmlNode ruoli=amministrazione.AppendChild (doc.CreateElement("RUOLI"));
					foreach( System.Data.DataRow rowRuolo in dataSetRuoli.Tables["RUOLI"].Rows)
					{
						ruolo=ruoli.AppendChild (doc.CreateElement("RUOLO"));
						ruolo.AppendChild(doc.CreateElement("DESCRIZIONE")).InnerText = rowRuolo["VAR_DESC_RUOLO"].ToString ();
						ruolo.AppendChild(doc.CreateElement("CODICE")).InnerText	  = rowRuolo["VAR_CODICE"].ToString ().ToUpper();
						ruolo.AppendChild(doc.CreateElement("LIVELLO")).InnerText	  = rowRuolo["NUM_LIVELLO"].ToString ();
					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'esportazione dei ruoli", exception);
				result=false;
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
			bool result = false;

			DocsPaDB.Query_DocsPAWS.AmministrazioneXml amm = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
			result = amm.MoveRoleToNewUO(codRuolo, codAmm, codNewUO, descNewUO, codTipoRuolo, descTipoRuolo, codNewRuolo, descNewRuolo);

			return result;
		}
	}
}
