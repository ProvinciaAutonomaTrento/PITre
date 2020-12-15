using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Runtime.Serialization;

namespace VtDocsWS
{
    [DataContract]
    public class PisException : ApplicationException
    {
        [DataMember]
        private static Dictionary<string, string> _pisError = null;

        public PisException(string errorCode)
            : base(GetDescriptionFromFile(errorCode))
        {
            this.ErrorCode = errorCode;

            this.Description = GetDescriptionFromFile(errorCode);
        }

        /// <summary>
        /// Codice dell'errore riscontrato
        /// </summary>
        [DataMember]
        public string ErrorCode
        {
            get;
            protected set;
        }

        /// <summary>
        /// Descrizione dell'errore letta dal file
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            protected set;
        }

        //Inizializza il singleton e prendi la descrizione dell'errore dal codice se presente
        private static string GetDescriptionFromFile(string errorCode)
        {
            string retValue = "Application error";

            if (_pisError == null)
            {
                // Creazione oggetto dictionary contenente i dati delle etichette per tutte le amministrazioni
                _pisError = new Dictionary<string, string>();
                lock (_pisError)
                {
                    ReadFileErrors();
                }
            }

            if (_pisError != null && _pisError.ContainsKey(errorCode))
            {
                retValue = _pisError[errorCode];
            }

            return retValue;
        }

        private static void ReadFileErrors()
        {
            string basePathFiles = System.Configuration.ConfigurationManager.AppSettings["REPORTS_PATH"];
            basePathFiles = basePathFiles.Replace("%DATA", "Errors.txt");

            using (StreamReader reader = new StreamReader(basePathFiles))
            {
                string line = string.Empty;
                string descrizione = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    char[] delimiterChars = { '=' };
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] words = line.Split(delimiterChars);
                        if (words != null && words.Length == 2)
                        {
                            _pisError.Add(words[0], words[1]);
                        }
                    }
                }
            }
        }
    }
}