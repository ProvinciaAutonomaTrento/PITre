using System;
using System.Xml;
using log4net;

namespace BusinessLogic.AmministrazioneXml
{
	/// <summary>
	/// Classe per la gestione dei registri di DocsPA tramite XML
	/// </summary>
	public class ServerXml
	{
        private ILog logger = LogManager.GetLogger(typeof(ServerXml));
		private ErrorCode errorCode;

		/// <summary>
		/// Acquisisce uno stream XML
		/// </summary>
		/// <param name="xmlSource"></param>
		
		/// <summary>
		/// </summary>
		public ServerXml()
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
					
					if(!amministrazioneXml.ClearServer())
					{
						throw new Exception();
					}
					
					amministrazioneXml.Dispose();
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante la cancellazione dei server", exception);
				errorCode = ErrorCode.GenericError;
			}

			if(errorCode != ErrorCode.NoError)
			{
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
		/// caricamento dei server
		/// </summary>
		/// <returns></returns>
		public bool CreateStructure(XmlNode node)
		{
			bool result = true;
			errorCode = ErrorCode.NoError;
			
			try
			{
				if(errorCode == ErrorCode.NoError)
				{
					bool dropAll =false;
					XmlAttribute attribute = node.Attributes["dropAll"];
					if(attribute!=null)
					{
						dropAll = Boolean.Parse(attribute.InnerText);
					}
					if(dropAll)
					{
						this.DropAll();
					}
						
					NavigateXml(node);
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

		public bool UpdateStructure(XmlNode node)
		{
			bool result = true;
			errorCode = ErrorCode.NoError;
			
			try
			{
				if(errorCode == ErrorCode.NoError)
				{
					NavigateXmlForUpdate(node);
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
		private void NavigateXml(XmlNode rootNode)
		{

			//lettura dei server di posta
			XmlNodeList nodiServers = rootNode.SelectNodes ("SERVERPOSTA");
			
			// Estrazione dati e nodi sottostanti
			foreach(XmlNode node in nodiServers)
			{
				string codice      = this.GetXmlField("CODICE", node, false);
				string serverPop   = this.GetXmlField("SERVERPOP", node, false);
				string portaPop	   = this.GetXmlField("PORTAPOP", node, false);
				string serverSmtp  = this.GetXmlField("SERVERSMTP", node, false);
				string portaSmtp   = this.GetXmlField("PORTASMTP", node, false);
				string dominio	   = this.GetXmlField("DOMINIO", node, false);
				string descrizione = this.GetXmlField("DESCRIZIONE", node, false);

				codice=codice.ToUpper();
				// Inserisci il server di posta
				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml(System.Configuration.ConfigurationManager.AppSettings["importConnectionString"]);							
				if(!amministrazioneXml.NewServerPosta(codice,serverPop,portaPop,serverSmtp,portaSmtp,dominio,descrizione))
				{
					throw new Exception();
				}
			}
		}

		private void NavigateXmlForUpdate(XmlNode rootNode)
		{
			//lettura dei server di posta
			XmlNodeList nodiServers = rootNode.SelectNodes ("SERVERPOSTA");
			
			// Estrazione dati e nodi sottostanti
			foreach(XmlNode node in nodiServers)
			{
				XmlAttribute attribute=node.Attributes["MODE"];
				string mode="";
				if(attribute!=null)
				{
					mode=attribute.InnerText.ToUpper();
				}

				string codice	   = this.GetXmlField("CODICE", node, false);
				string serverPop   = this.GetXmlField("SERVERPOP", node, false);
				string portaPop	   = this.GetXmlField("PORTAPOP", node, false);
				string serverSmtp  = this.GetXmlField("SERVERSMTP", node, false);
				string portaSmtp   = this.GetXmlField("PORTASMTP", node, false);
				string dominio	   = this.GetXmlField("DOMINIO", node, false);
				string descrizione = this.GetXmlField("DESCRIZIONE", node, false);
				descrizione=DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione);

				codice=codice.ToUpper();

				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();							

				if(mode=="CREATED")
				{
					// Inserisci il server di posta
					if(!amministrazioneXml.NewServerPosta(codice,serverPop,portaPop,serverSmtp,portaSmtp,dominio,descrizione))
					{
						throw new Exception();
					}
				}
				if(mode=="MODIFIED")
				{
					// Inserisci il server di posta
					if(!amministrazioneXml.UpdateServerPosta(codice,serverPop,portaPop,serverSmtp,portaSmtp,dominio,descrizione))
					{
						throw new Exception();
					}
				}
				if(mode=="DELETED")
				{
					//cancella server di posta
					if(!amministrazioneXml.DeleteServerPosta(codice))
					{
						throw new Exception();
					}
				}
			}
		}

		public bool ExportStructure(XmlDocument doc,XmlNode node)
		{
			bool result=true; //presume successo
			try
			{
				//legge i server
				System.Data.DataSet dataSet;
				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml=new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
				result=amministrazioneXml.Exp_GetServerPosta(out dataSet);
				if(!result)
				{
					throw new Exception();
				}
				if(dataSet != null)
				{
					XmlNode servers=node.AppendChild (doc.CreateElement("SERVERS"));
					foreach( System.Data.DataRow row in dataSet.Tables["SERVERPOSTA"].Rows)
					{
						//esportazione dati della amministrazione
						XmlNode serverPosta=servers.AppendChild (doc.CreateElement("SERVERPOSTA"));
						serverPosta.AppendChild(doc.CreateElement("CODICE")).InnerText		= row["VAR_CODICE"].ToString ().ToUpper();
						serverPosta.AppendChild(doc.CreateElement("SERVERPOP")).InnerText	= row["VAR_SERVER_POP"].ToString ();
						serverPosta.AppendChild(doc.CreateElement("PORTAPOP")).InnerText	= row["NUM_PORTA_POP"].ToString ();
						serverPosta.AppendChild(doc.CreateElement("SERVERSMTP")).InnerText	= row["VAR_SERVER_SMTP"].ToString ();
						serverPosta.AppendChild(doc.CreateElement("PORTASMTP")).InnerText	= row["NUM_PORTA_SMTP"].ToString();
						serverPosta.AppendChild(doc.CreateElement("DOMINIO")).InnerText		= row["VAR_DOMINIO"].ToString();
						serverPosta.AppendChild(doc.CreateElement("DESCRIZIONE")).InnerText = row["VAR_DESCRIZIONE"].ToString();
					}
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore durante l'esportazione dei server", exception);
				result=false;
			}
			return result;
		}
	}
}
