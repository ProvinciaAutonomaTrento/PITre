using System;
using System.Data;
using System.Globalization;
using System.IO;
using log4net;

namespace DocsPaUtils.Functions
{
	/// <summary>
	/// </summary>
	public class Functions
	{
        private static ILog logger = LogManager.GetLogger(typeof(Functions));
		/// <summary>
		/// Sostituisce un apice con un doppio apice per evitare errori nell'interpretazione
		/// delle stringhe dai motori DB.
		/// </summary>
		/// <param name="sourceString"></param>
		/// <returns></returns>
		public static string ReplaceApexes(string sourceString)
		{
			string result = null;

			if(sourceString != null)
			{
				result = sourceString.Replace("'", "''");
			}

			return result;
		}

		/// <summary>
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public static string GetDate(bool time) 
		{
			if(time)
			{
				return DateTime.Now.ToString("dd/MM/yyyy h:mm:ss").Replace('.',':');
			}
			else
			{
				return DateTime.Now.ToString("dd/MM/yyyy");
			}
		}

       
		/// <summary>
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime ToDate(string date) 
		{
            string[] formats = {"dd/MM/yyyy",
								"dd/MM/yyyy h:mm:ss",
								"dd/MM/yyyy h.mm.ss",
								"dd/MM/yyyy HH.mm.ss",
                                "dd/MM/yyyy HH:mm:ss"
                               };

			return DateTime.ParseExact(date, formats, new CultureInfo("it-IT"), DateTimeStyles.AllowWhiteSpaces);
		}

		/// <summary>Crea una directory al path specificato</summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool CreateDirectory(string path)
		{
			bool result = true; // Presume successo

			try
			{
				if(!System.IO.Directory.Exists(path))
				{
					System.IO.Directory.CreateDirectory(path);
				}
			}
			catch(Exception exception)
			{
				logger.Debug("Errore nella creazione della directory: " + path , exception);
				result = false;
			}

			return result;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public static string GetReportsPath() 
		{
			string basePathFiles = System.Configuration.ConfigurationManager.AppSettings["REPORTS_PATH"];
			basePathFiles = basePathFiles.Replace("%DATA", "StampaRegistro_" + DateTime.Now.ToString("yyyyMMdd"));

			return basePathFiles;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public static string GetArchivioLogPath() 
		{
			string basePathFiles = System.Configuration.ConfigurationManager.AppSettings["ARCHIVIO_LOG_PATH"];			

			return basePathFiles;
		}

		/// <summary>
		/// </summary>
		/// <param name="nomeDirectory"></param>
		public static void CancellaDirectory(string nomeDirectory)
		{
			if(System.IO.Directory.Exists(nomeDirectory))
			{				
				System.IO.Directory.Delete(nomeDirectory,true);
			}
		
		}

		/// <summary>
		/// </summary>
		/// <param name="nomeDirectory"></param>
		public static void CheckEsistenzaDirectory(string nomeDirectory)
		{
			if(System.IO.Directory.Exists(nomeDirectory))
			{
				string[] filenames=System.IO.Directory.GetFiles(nomeDirectory);
				for(int i=0;i<filenames.Length;i++)
				{
					if(System.IO.File.Exists(filenames[i]))
					{
						System.IO.File.Delete(filenames[i]);
					}
				}
			}
			else
			{
				System.IO.Directory.CreateDirectory(nomeDirectory);
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string CheckData(string data)
		{
			try 
			{
				CultureInfo ci = new CultureInfo("it-IT");
				string[] formati={"yyyy-MM-dd"};
				DateTime d_ap =	DateTime.ParseExact(data,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);
				string res=d_ap.ToString("dd/MM/yyyy");
				return res;
			}
			catch(Exception) 
			{
				return null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string CheckData_Invio(string data)
		{
			try 
			{
				CultureInfo ci = new CultureInfo("it-IT");
				string[] formati={"dd/MM/yyyy","dd/MM/yyyy h:mm:ss","dd/MM/yyyy h.mm.ss"};
				DateTime d_ap =	DateTime.ParseExact(data,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);
				string res=d_ap.ToString("yyyy-MM-dd");
				return res;
			}
			catch(Exception) 
			{
				return null;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static string GetAlternativeId(System.DateTime date)
		{
			string res=null;
			CultureInfo ci = new CultureInfo("it-IT");
			res=date.ToString("dd/mm/yyyy hh:mm:ss");
			return res;
		}


		//Celeste
		public static bool isDocumentoFirmato(string filename)
		{
			bool result = false;

			if (filename.IndexOf(".p7m") > 0)
				result = true;

			return result;
		}
		//Fine Celeste

		#region Metodi utilizzati dal documentale Filenet

		public static string getExt(string fileName) 
		{
			if (fileName == null || fileName.Length==0)
				return "";

			// se il nome del file ha estensione multipla, la classe Path restituisce solo l'ultima
			string ext = Path.GetExtension(fileName);
			if(ext.StartsWith("."))
				ext = ext.Substring(1);
			if(ext.ToUpper().Equals("P7M")) 
				ext = fileName.Substring(fileName.IndexOf("."));
			
			return ext;
		}
		
		public static int toInt(object obj) 
		{
			try 
			{
				if (obj != null)
					return Int32.Parse(obj.ToString());
			} 
			catch (Exception) {}
			return -1;			
		}

		#endregion


	}
}
