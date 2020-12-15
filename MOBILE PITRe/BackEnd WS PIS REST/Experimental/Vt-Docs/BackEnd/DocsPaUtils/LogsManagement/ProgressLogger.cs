using System;
using System.Collections;
using System.Xml;
using System.Configuration;
using log4net;

namespace DocsPaUtils.LogsManagement 
{
	/// <summary>
	/// </summary>
	public class ProgressLogger
	{
        private static ILog logger = LogManager.GetLogger(typeof(ProgressLogger));
		private static ProgressLogger itsInstance = null;
		private static XmlDocument xmlDocument = null;
		private static string fileName = null;
		private static int idMessage = 1;

		/// <summary>
		/// Costruttore privato poichè la classe implementa il design pattern singleton
		/// </summary>
		private ProgressLogger()
		{}

		/// <summary>
		/// Crea, se non esiste, l’istanza della classe e inizializza gli attributi 
		/// utilizzando il metodo initializeQuery
		/// </summary>
		/// <returns></returns>
		public static ProgressLogger getInstance(bool NewDoc) 
		{
			if (itsInstance == null) 
			{
				itsInstance = new ProgressLogger();
				//Initialize(System.Configuration.ConfigurationManager.AppSettings["STATUS_FILE"]);
				
			}

			if(xmlDocument==null || NewDoc==true) Initialize(null);

			return itsInstance;
		}

		/// <summary>
		/// Crea dal file Web.Config le informazioni relative alla posizione del file XML 
		/// delle query. 
		/// </summary>
		/// <param name="xmlPathName"></param>
		public static void Initialize(string xmlPathName)
		{
			fileName = xmlPathName;
			idMessage = 1;

			try
			{				
				//lettura file xml
				xmlDocument = new XmlDocument();
			
				System.Diagnostics.Debug.WriteLine("Creating XML file: " + fileName);
				xmlDocument.LoadXml("<STATUS COMPLETO='False' />");
				XmlNode root=xmlDocument.SelectSingleNode("STATUS");
				if(root!=null)
				{
					XmlNode messaggi=root.AppendChild (xmlDocument.CreateElement("MESSAGGI"));
					itsInstance.SaveXmlFile();
				}
			}
			catch(Exception e)
			{
				logger.Debug("Errore durante la creazione del file di log.", e);
			}
		}

		public void Terminate()
		{
			try
			{
				if(xmlDocument!=null)
				{
					XmlAttribute attribute=xmlDocument.SelectSingleNode("STATUS").Attributes["COMPLETO"];
					if(attribute!=null)
					{
						attribute.InnerText = "True";
						SaveXmlFile();
					}
				}
			}
			catch(Exception e)
			{
				logger.Debug("Errore durante l'aggiornamento del file di log.", e);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public int NewMessage(string message)
		{
			int result=0;
			if(xmlDocument!=null)
			{
				XmlNode messaggi=xmlDocument.SelectSingleNode("STATUS").SelectSingleNode("MESSAGGI");
				if(messaggi!=null)
				{
					XmlNode messaggio=messaggi.AppendChild (xmlDocument.CreateElement("MESSAGGIO"));
					
					messaggio.InnerText =message;

					XmlAttribute attributo3=xmlDocument.CreateAttribute("ID");
					attributo3.InnerText = idMessage.ToString();
					messaggio.Attributes.Append(attributo3);
					result=idMessage;
					
					idMessage++;

					XmlAttribute attributo2=xmlDocument.CreateAttribute("COMPLETO");
					attributo2.InnerText = "False";
					messaggio.Attributes.Append(attributo2);
					
					XmlAttribute attributo=xmlDocument.CreateAttribute("ERRORE");
					attributo.InnerText = "False";
					messaggio.Attributes.Append(attributo);
					
					SaveXmlFile();
				}
			}
			return result;
		}

		public void UpdateMessage(int id, string message,bool completo,bool errore)
		{
			if(xmlDocument!=null)
			{
				XmlNode messaggi=xmlDocument.SelectSingleNode("STATUS").SelectSingleNode("MESSAGGI");
				if(messaggi!=null)
				{
					XmlNode messaggio=messaggi.SelectSingleNode("MESSAGGIO [@ID='" + id + "']");
					if (messaggio!=null)
					{
						if(message!=null)
						{
							messaggio.InnerText = message;
						}
						XmlAttribute attributo=messaggio.Attributes["COMPLETO"];
						if(attributo!=null)
						{
							if(completo)
							{
								attributo.InnerText="True";
							}
							else
							{
								attributo.InnerText="False";
							}
						}
						attributo=messaggio.Attributes["ERRORE"];
						if(attributo!=null)
						{
							if(errore)
							{
								attributo.InnerText="True";
							}
							else
							{
								attributo.InnerText="False";
							}
						}
						SaveXmlFile();
					}
				}
			}
		}


		public string GetXml()
		{
			if(xmlDocument!=null)
			{
				return xmlDocument.InnerXml;
			}
			else
			{
				return null;
			}
		}
		
		/// <summary>
		/// Salva il file XML su disco
		/// </summary>
		private void SaveXmlFile()
		{
			try
			{
				if(fileName!=null)
				{
					xmlDocument.Save(fileName);
					System.Diagnostics.Debug.WriteLine("Aggiornato il file di log per import/export.");
				}
			}
			catch(Exception)
			{
				System.Diagnostics.Debug.WriteLine("Il file di log per import/export e' in uso da un altro thread.\nIl salvataggio non e' stato effettuato.");
			}
		}

	}
}
