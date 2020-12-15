using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;
using DocsPaVO.utente;
using DocsPaVO.Interoperabilita;

namespace BusinessLogic.Interoperabilità.Log
{
	/// <summary>
	/// Classe per la gestione dei log su file xml relativi al controllo della 
	/// casella di posta elettronica istituzionale.
	/// Struttura delle cartelle su filesystem:
	/// DOCSPA_LOG_PATH
	///		|
	///		|__ CODICE_REGISTRO
	///			|
	///			|__ ANNO
	///				|
	///				|__ MESE
	///					|
	///					|__ GIORNO
	/// </summary>
	public class LogCheckMail
	{
		/// <summary>
		/// Chiave del file web.config contenente il percorso radice 
		/// di docspa backend per la memorizzazione dei file di log
		/// </summary>
		private const string APP_CONFIG_LOG_KEY="LOG_PATH";

		/// <summary>
		/// Cartella di base in cui vengono memorizzati i file di log
		/// </summary>
		private const string BASE_LOG_FOLDER="Interop.CheckMail";

		public LogCheckMail()
		{
		}

		/// <summary>
		/// Scrittura log
		/// </summary>
		/// <param name="registro"></param>
		/// <param name="checkResponse"></param>
		public void LogElement(Registro registro,MailAccountCheckResponse checkResponse)
		{
			this.LogElement(new LogElementCheckMail(registro,checkResponse));
		}

		/// <summary>
		/// Scrittura log
		/// </summary>
		/// <param name="logElement"></param>
		public void LogElement(LogElementCheckMail logElement)
		{	
			string logPath=this.GetLogFolder(logElement.CodiceRegistro,logElement.MailRegistro);

			if (!Directory.Exists(logPath))
				Directory.CreateDirectory(logPath);

			this.SerializeContent(logPath,logElement.CheckResponse);
		}

		/// <summary>
		/// Restituzione di tutti i log del controllo casella istituzionale
		/// effettuati in un registro e ordinati per data e ora decrescente
		/// </summary>
		/// <param name="registro"></param>
		/// <param name="logDateTime"></param>
		/// <returns></returns>
		public LogElementCheckMail[] GetLogElements(Registro registro,DateTime logDateTime)
		{
			ArrayList elements=new ArrayList();

			string logFolder=this.GetLogFolder(registro.codRegistro,registro.email,logDateTime);

			DirectoryInfo dirInfo=new DirectoryInfo(logFolder);
			FileInfo[] logFiles=dirInfo.GetFiles("*.xml");

			foreach (FileInfo logFile in logFiles)
			{
				MailAccountCheckResponse checkResponse=this.DeserializeContent(logFile.FullName);
				elements.Add(new LogElementCheckMail(registro,checkResponse,logFile.LastWriteTime));
			}

			elements.Sort(new LogElementCheckMailComparer());
			
			return (LogElementCheckMail[]) elements.ToArray(typeof(LogElementCheckMail));
		}

		/// <summary>
		/// Reperimento di tutte le date in cui, per un registro,
		/// sono stati effettuati i controlli della casella istituzionale
		/// </summary>
		/// <param name="registro"></param>
		/// <returns></returns>
		public DateTime[] GetLogDateList(Registro registro)
		{
			ArrayList retValue=new ArrayList();

			string rootLogFolder=Path.Combine(this.GetRootLogFolder(),registro.codRegistro);

			if (Directory.Exists(rootLogFolder))
			{
				foreach (string directory in Directory.GetDirectories(rootLogFolder))
				{
					DirectoryInfo dirInfo=new DirectoryInfo(directory);
				
					int year=Convert.ToInt32(dirInfo.Name);

					foreach (DirectoryInfo monthDir in dirInfo.GetDirectories())
					{
						int month=Convert.ToInt32(monthDir.Name);
					
						foreach (DirectoryInfo dayDir in monthDir.GetDirectories())
						{
							int day=Convert.ToInt32(dayDir.Name);

							retValue.Add(new DateTime(year,month,day));
						}
					}
				}
			}
			
			return (DateTime[]) retValue.ToArray(typeof(DateTime));
		}

		/// <summary>
		/// Reperimento cartella radice in cui vengono memorizzati i file di log
		/// </summary>
		/// <returns></returns>
		protected virtual string GetRootLogFolder()
		{
			string rootPath=ConfigurationManager.AppSettings[APP_CONFIG_LOG_KEY];
			rootPath=rootPath.Replace("%DATA",BASE_LOG_FOLDER);
			return rootPath;
		}

		/// <summary>
		/// Creazione percorso completo in cui vengono memorizzati i file di log
		/// </summary>
		/// <param name="codiceRegistro"></param>
		/// <param name="mailRegistro"></param>
		/// <param name="logDateTime"></param>
		/// <returns></returns>
		protected virtual string GetLogFolder(string codiceRegistro,string mailRegistro,DateTime logDateTime)
		{
			string rootLogFolder=this.GetRootLogFolder();

			if (mailRegistro==null || mailRegistro==string.Empty)
				mailRegistro="MAIL_NON_DEFINITA";

			// Creazione cartella interna per gestire i log in forma giornaliera
			string innerFolder=	codiceRegistro + @"\" +
								logDateTime.Year.ToString() + @"\" +
								logDateTime.Month.ToString() + @"\" +
								logDateTime.Day.ToString() + @"\";

			return Path.Combine(rootLogFolder,innerFolder);
		}

		/// <summary>
		/// Creazione percorso completo in cui vengono memorizzati i file di log
		/// </summary>
		/// <param name="codiceRegistro"></param>
		/// <param name="mailRegistro"></param>
		/// <returns></returns>
		protected virtual string GetLogFolder(string codiceRegistro,string mailRegistro)
		{
			return this.GetLogFolder(codiceRegistro,mailRegistro,DateTime.Now);
		}

		/// <summary>
		/// Serializzazione del contenuto su file di log
		/// </summary>
		/// <param name="logPath"></param>
		/// <param name="checkResponse"></param>
		protected virtual void SerializeContent(string logPath,MailAccountCheckResponse checkResponse)
		{	
			string fullName=Path.Combine(logPath,this.GetLogFileName());

			TextWriter textWriter=new StreamWriter(fullName,false);
			XmlTextWriter writer=new XmlTextWriter(textWriter);

			XmlSerializer serializer=new XmlSerializer(typeof(MailAccountCheckResponse));
			serializer.Serialize(writer,checkResponse);
			
			writer.Flush();
			textWriter.Flush();

			writer.Close();
			textWriter.Close();
		}

		/// <summary>
		/// Deserializzazione del contenuto del file di log
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		protected virtual MailAccountCheckResponse DeserializeContent(string filePath)
		{
			MailAccountCheckResponse retValue=null;

			XmlSerializer serializer=new XmlSerializer(typeof(MailAccountCheckResponse));

			XmlTextReader reader=new XmlTextReader(filePath);
			
			if (serializer.CanDeserialize(reader))
				retValue=(MailAccountCheckResponse) serializer.Deserialize(reader);

			reader.Close();

			return retValue;
		}

		/// <summary>
		/// Reperimento del nome del file di log
		/// </summary>
		/// <returns></returns>
		private string GetLogFileName()
		{
			DateTime dateTime=DateTime.Now;

			return	dateTime.Hour.ToString() + 
					dateTime.Minute.ToString() +
					dateTime.Second.ToString() +
					dateTime.Millisecond.ToString() + ".xml";
		}

		/// <summary>
		/// Classe per il confronto delle date delle mail
		/// </summary>
		private class LogElementCheckMailComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				LogElementCheckMail mailX=x as LogElementCheckMail;
				LogElementCheckMail mailY=y as LogElementCheckMail;

				return mailY.LogDateTime.CompareTo(mailX.LogDateTime);
			}
		}
	}
}