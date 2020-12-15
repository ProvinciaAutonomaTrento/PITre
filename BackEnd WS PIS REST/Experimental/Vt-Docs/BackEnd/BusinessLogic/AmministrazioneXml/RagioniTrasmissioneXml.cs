using System;
using System.Xml;
using log4net;

namespace BusinessLogic.AmministrazioneXml
{
	/// <summary>
	/// Classe per la gestione delle ragioni trasmissione di DocsPA tramite XML
	/// </summary>
	public class RagioniTrasmissioneXml
	{
        private ILog logger = LogManager.GetLogger(typeof(RagioniTrasmissioneXml));
		private ErrorCode errorCode;
		private XmlDocument parser;

		private DocsPaVO.utente.InfoUtente infoUtente;

		/// <summary>
		/// Acquisisce uno stream XML
		/// </summary>
		/// <param name="xmlSource"></param>
		public RagioniTrasmissioneXml(string xmlSource)
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

		public RagioniTrasmissioneXml()
		{
			errorCode = ErrorCode.NoError;
		}


		/// <summary>
		/// </summary>
		
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
					
					if(!amministrazioneXml.ClearRagioniTrasmissione())
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
			XmlNodeList nodiRagioni = rootNode.SelectNodes("RAGIONE");

			// Estrazione dati e nodi sottostanti
			foreach(XmlNode node in nodiRagioni)
			{
				// Leggi dati
				string codice		= this.GetXmlField("CODICE", node, false);
				string tipo			= this.GetXmlField("TIPO", node, false);
				string visibilita	= this.GetXmlField("VISIBILITA", node, false);
				string diritti		= this.GetXmlField("DIRITTI", node, false);
				string destinatario = this.GetXmlField("DESTINATARIO", node, false);
				string risposta		= this.GetXmlField("RISPOSTA", node, false);
				string tipoRisposta	= this.GetXmlField("TIPORISPOSTA", node, false);
				string eredita		= this.GetXmlField("EREDITA", node, false);
				string note			= this.GetXmlField("NOTE", node, false);
				string notifica		= this.GetXmlField("NOTIFICA", node, false);

				codice=codice.ToUpper();

				// Inserisci i dati
				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();							
				bool result = amministrazioneXml.NewRagioneTrasmissione(codice, tipo, visibilita, diritti, destinatario, risposta, tipoRisposta, eredita, note, notifica, idAmm);
				
				if(!result)
				{
					throw new Exception();
				}
			}
		}

		private void NavigateXmlForUpdate(XmlNode rootNode, string idAmm)
		{
			XmlNodeList nodiRagioni = rootNode.SelectNodes("RAGIONE");

			// Estrazione dati e nodi sottostanti
			foreach(XmlNode node in nodiRagioni)
			{
				string mode="";
				XmlAttribute attribute = node.Attributes["MODE"];
				if(attribute != null)
				{
					mode=attribute.InnerText.ToUpper() ;
				}

				// Leggi dati
				string codice		= this.GetXmlField("CODICE", node, false).ToUpper();
				string tipo			= this.GetXmlField("TIPO", node, false);
				string visibilita	= this.GetXmlField("VISIBILITA", node, false);
				string diritti		= this.GetXmlField("DIRITTI", node, false);
				string destinatario = this.GetXmlField("DESTINATARIO", node, false);
				string risposta		= this.GetXmlField("RISPOSTA", node, false);
				string tipoRisposta	= this.GetXmlField("TIPORISPOSTA", node, false);
				string eredita		= this.GetXmlField("EREDITA", node, false);
				string note			= this.GetXmlField("NOTE", node, false);
				string notifica		= this.GetXmlField("NOTIFICA",node, false);
				
				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();							

				if(mode=="CREATED")
				{
					if(!amministrazioneXml.NewRagioneTrasmissione(codice, tipo, visibilita, diritti, destinatario, risposta, tipoRisposta, eredita, note, notifica, idAmm))
					{
						throw new Exception();
					}
				}

				if(mode=="MODIFIED")
				{
					// modifica i dati
					if(!amministrazioneXml.UpdateRagioneTrasmissione(codice, tipo, visibilita, diritti, destinatario, risposta, tipoRisposta, eredita, note, notifica, idAmm))
					{
						throw new Exception();
					}
				}

				if(mode=="DELETED")
				{
					if(!amministrazioneXml.DeleteRagioneTrasmissione(codice, idAmm))
					{
						throw new Exception();
					}
				}
			}
		}

		public bool ExportStructure(XmlDocument doc,XmlNode amministrazione,string idAmm)
		{
			bool result=true; //presume successo
			try
			{
				System.Data.DataSet dataSetRagioni;
				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();							
				result=amministrazioneXml.Exp_GetRagioniTrasmissione(out dataSetRagioni,idAmm);
				if(!result)
				{
					throw new Exception();
				}

				if(dataSetRagioni != null)
				{
					XmlNode ragioni=amministrazione.AppendChild (doc.CreateElement("RAGIONITRASMISSIONE"));

					foreach( System.Data.DataRow rowRagione in dataSetRagioni.Tables["RAGIONI"].Rows)
					{
						XmlNode ragione=ragioni.AppendChild (doc.CreateElement("RAGIONE"));
						ragione.AppendChild(doc.CreateElement("CODICE")).InnerText			= rowRagione["VAR_DESC_RAGIONE"].ToString ().ToUpper();
						ragione.AppendChild(doc.CreateElement("TIPO")).InnerText			= rowRagione["CHA_TIPO_RAGIONE"].ToString ();
						ragione.AppendChild(doc.CreateElement("VISIBILITA")).InnerText		= rowRagione["CHA_VIS"].ToString ();
						ragione.AppendChild(doc.CreateElement("DIRITTI")).InnerText			= rowRagione["CHA_TIPO_DIRITTI"].ToString();
						ragione.AppendChild(doc.CreateElement("DESTINATARIO")).InnerText	= rowRagione["CHA_TIPO_DEST"].ToString();
						ragione.AppendChild(doc.CreateElement("RISPOSTA")).InnerText		= rowRagione["CHA_RISPOSTA"].ToString();
						ragione.AppendChild(doc.CreateElement("TIPORISPOSTA")).InnerText	= rowRagione["CHA_TIPO_RISPOSTA"].ToString();
						ragione.AppendChild(doc.CreateElement("EREDITA")).InnerText			= rowRagione["CHA_EREDITA"].ToString();
						ragione.AppendChild(doc.CreateElement("NOTE")).InnerText			= rowRagione["VAR_NOTE"].ToString();
						ragione.AppendChild(doc.CreateElement("NOTIFICA")).InnerText		= rowRagione["VAR_NOTIFICA_TRASM"].ToString();
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
