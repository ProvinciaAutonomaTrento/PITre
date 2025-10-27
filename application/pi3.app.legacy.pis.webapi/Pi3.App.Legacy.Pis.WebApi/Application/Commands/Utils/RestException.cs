// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.Reflection;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils
{
    public class RestException : ApplicationException
    {
        private static Dictionary<string, string> _pisError = null;
        private static readonly object _pisErrorLock = new object();

        public RestException(string errorCode)
            : base(GetDescriptionFromFile(errorCode))
        {
            this.ErrorCode = errorCode;

            this.Description = GetDescriptionFromFile(errorCode);
        }

        public RestException(string errorCode, string errorDescription)
        {
            this.ErrorCode = errorCode;
            this.Description = errorDescription;
        }

        /// <summary>
        /// Codice dell'errore riscontrato
        /// </summary>
        public string ErrorCode
        {
            get;
            protected set;
        }

        /// <summary>
        /// Descrizione dell'errore letta dal file
        /// </summary>
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
                lock (_pisErrorLock)
                {
                    // Creazione oggetto dictionary contenente i dati delle etichette per tutte le amministrazioni
                    _pisError = new Dictionary<string, string>();
                    ReadFileErrors();
                }
            }

            /*Il lock su _pisError è segnalato come errore su Sonarqube, sostituito con il codice di sopra
            if (_pisError == null)
            {
                // Creazione oggetto dictionary contenente i dati delle etichette per tutte le amministrazioni
                _pisError = new Dictionary<string, string>();
                lock (_pisError)
                {
                    ReadFileErrors();
                }
            }
            */

            if (_pisError != null && _pisError.ContainsKey(errorCode))
            {
                retValue = _pisError[errorCode];
            }

            return retValue;
        }

        private static void ReadFileErrors()
        {
            //string basePathFiles = System.Configuration.ConfigurationManager.AppSettings["REPORTS_PATH"];
            //basePathFiles = basePathFiles.Replace("%DATA", "Errors.txt");

            string basePathFiles = "datafiles/Errors.txt";
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? "", basePathFiles);
            using (StreamReader reader = new StreamReader(path))
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
