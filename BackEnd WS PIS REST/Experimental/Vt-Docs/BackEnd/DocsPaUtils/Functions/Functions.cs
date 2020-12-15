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

		public static void write(string fullName, DocsPaVO.documento.FileDocumento fileDocumento) 
		{
			string dir = Path.GetDirectoryName(fullName);
			if(!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			FileStream file = System.IO.File.Create(fullName);
			file.Write(fileDocumento.content, 0, fileDocumento.length);
			file.Flush();
			file.Close();
		}

		public static DocsPaVO.documento.FileDocumento read(DocsPaVO.documento.FileRequest fileRequest) 
		{
			return read(Path.Combine(fileRequest.docServerLoc, fileRequest.path), fileRequest.fileName);
		}


		public static DocsPaVO.documento.FileDocumento read(string path, string fileName) 
		{
			return read(Path.Combine(path, fileName));
		}

		public static DocsPaVO.documento.FileDocumento read(string fullName) 
		{
			DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();

			fileDoc.name = Path.GetFileName(fullName);
			fileDoc.path = Path.GetDirectoryName(fullName);
			fileDoc.fullName = fullName;
			
			if (!System.IO.File.Exists(fileDoc.fullName))
			{
				logger.Error("File non trovato: "+ fileDoc.fullName);
				return null;
			}
			FileStream file =  System.IO.File.Open(fileDoc.fullName, FileMode.Open);
			fileDoc.length = (int) file.Length;
			fileDoc.content = new Byte[fileDoc.length];
			file.Read(fileDoc.content, 0, fileDoc.length);
			file.Close();
			return fileDoc;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objToSerialize">Oggetto da serializzare-deserializzare</param>
        /// <param name="t1">Type più esterno</param>
        /// <param name="t2">Extratype</param>
        /// <param name="encode">codifica</param>
        /// <returns></returns>
        public static Object XML_Serialization_Deserialization_By_Encode(Object objToSerialize, Type t1, Type[] t2, System.Text.Encoding encode)
        {
            #region serializzazione

            System.Xml.Serialization.XmlSerializer serializer = null;
            if (t2 != null)
            {
                serializer = new System.Xml.Serialization.XmlSerializer(
                    t1,
                    t2
                    );
            }
            else
                serializer = new System.Xml.Serialization.XmlSerializer(
                    t1
                    );

            MemoryStream memoryStream = new MemoryStream();
            //Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            //StreamWriter streamWriter = new StreamWriter(memoryStream, iso);
            StreamWriter streamWriter = new StreamWriter(memoryStream, encode);
            serializer.Serialize(streamWriter, objToSerialize);

            streamWriter.Flush();

            memoryStream.Position = 0;
            StreamReader sr = new StreamReader(memoryStream);

            string resultSer = sr.ReadToEnd();

            if (!string.IsNullOrEmpty(resultSer))
            {
                //
                // Invalid Character for XML 1.0
                // &#xhhhhhhh..hh;, essentially is random binary data that can not be printed
                while (resultSer.Contains("&#x"))
                {
                    int startIndx = -1;
                    int endIndx = -1;
                    startIndx = resultSer.IndexOf("&#x");
                    if (startIndx != -1)
                    {
                        endIndx = resultSer.IndexOf(";", startIndx);
                        int numCharToDelete = (endIndx + 1) - startIndx;

                        resultSer = resultSer.Remove(startIndx, numCharToDelete);
                    }
                }
            }
            #endregion

            #region deserializzazione

            //// Test 1
            TextReader reader = new StringReader(resultSer);
            Object obj = serializer.Deserialize(reader);
            reader.Close();

            #endregion
            return obj;
        }

        //#region Metodi per configurazione stampa Registro
        
        ///// <summary>
        ///// aggiunge l'intrevallo di giorni alla data passata in input
        ///// </summary>
        ///// <param name="date"></param>
        ///// <param name="days"></param>
        //public static void addDays(DateTime date, double days) 
        //{
        //    date.AddDays(days);
        //}

        //#endregion
    }
}
