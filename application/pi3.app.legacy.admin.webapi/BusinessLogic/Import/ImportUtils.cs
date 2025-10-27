// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Import;
using Serilog;
using System.Data.OleDb;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace BusinessLogic.Import
{
    public class ImportUtils
    {
        private static ILogger logger = Log.ForContext(typeof(ImportUtils));

        public static bool AcceptAllCertificatePolicy(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }

        public static List<string> getTitolarioByCodeFascicolo(string ProjectCodes, string ProjectDescription, string SysIDAmministrazione, string IDRegistro)
        {
            List<string> listaIDTitolari = new List<string>();

            //TO DO:
            //Query che a partire dal projectCodes || ProjectDescription restituisce l'ID del Titolario in cui è presente il fascicolo
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            listaIDTitolari = amm.getTitolarioByCodeFascicolo(ProjectCodes, ProjectDescription, SysIDAmministrazione, IDRegistro);

            return listaIDTitolari;
        }

        /// <summary>
        /// Funzione per la connessione ad una sorgente dati
        /// </summary>
        /// <param name="provider">Classpath del provider</param>
        /// <param name="extendedProperty">Proprietà avanzate di connessione</param>
        /// <param name="completePath">Path del file cui connettersi</param>
        /// <returns>L'oggetto per la gestione della connessione</returns>
        public static OleDbConnection ConnectToFile(string provider, string extendedProperty, string completePath)
        {
            // Stringa di connessione da utilizzare per connettersi al foglio excel
            string connectionString;

            // L'oggetto da restituire
            OleDbConnection oleConnection;

            // Creazione della stringa di connessione
            connectionString = String.Format("Provider={0}Data Source={1};Extended Properties=\"{2}\"",
                provider,
                completePath,
                extendedProperty);
            logger.Debug(connectionString);
            try
            {
                // Creazione dell'oggetto per la connessione al foglio excel
                oleConnection = new OleDbConnection(connectionString);

                // Apertura connessione
                oleConnection.Open();


            }
            catch (Exception e)
            {
                // Viene rilanciata un'eccezione al livello superiore
                logger.Error("Errore durante la connessione al file excel.");
                logger.Error(e.Message);
                logger.Error(e.StackTrace);
                throw new ImportException("Errore durante la connessione al file excel. Dettagli: " + e.Message);

            }

            // Restituzione della connessione aperta
            return oleConnection;

        }

        /// <summary>
        /// Questa funzione restituisce il contenuto di un file salvato in una cartella FTP
        /// </summary>
        /// <param name="FTPAddress">Indirizzo del server FTP</param>
        /// <param name="filePath">Path del file da aprire</param>
        /// <param name="username">Username per accedere al server FTP</param>
        /// <param name="password">Password per accedere al server FTP</param>
        /// <returns>Contenuto del file</returns>
        public static byte[] DownloadFileFromFTP(string FTPAddress, string filePath, string username, string password)
        {
            // Richiesta FTP
            FtpWebRequest request;

            // File stream
            Stream stream;

            // Array da restituire
            byte[] toReturn;

            logger.Debug(String.Format("Creazione request FTP per lettura file {0}", filePath));

            // Creazione della request FTP
            request = (FtpWebRequest)FtpWebRequest.Create(FTPAddress + "/" + filePath);

            //
            //SSL Region
            bool ftpSSL = false;
            //Chiave per abilitare l'utilizzo del ftp in modalità SSL
            ftpSSL = ((!string.IsNullOrEmpty(DocsPaVO.Settings.AppSettings.Instance.FTPSSL) && DocsPaVO.Settings.AppSettings.Instance.FTPSSL.Equals("0")) || string.IsNullOrEmpty(DocsPaVO.Settings.AppSettings.Instance.FTPSSL) ? false : true);

            if (ftpSSL)
            {
                //ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertificatePolicy;
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                {
                    return sslPolicyErrors == SslPolicyErrors.None;
                };
                //ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertificatePolicy);

                request.EnableSsl = true;
                //request.AuthenticationLevel = AuthenticationLevel.MutualAuthRequested;
            }
            //End SSL Region
            //

            // Download
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            // Impostazione delle credenziali
            request.Credentials = new NetworkCredential(username, password);

            // Impostazione parametri di connessione
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            logger.Debug("Request FTP creata per {0}", request.RequestUri.ToString());

            try
            {
                //Load the file
                logger.Debug("Lettura del file");

                // Apertura stream
                using (stream = request.GetResponse().GetResponseStream())
                {
                    logger.Debug("collegato");
                    List<byte> list = new List<byte>();

                    while (true)
                    {
                        int b = stream.ReadByte();

                        if (b == -1)
                            break;

                        list.Add((byte)b);
                    }

                    // Inizializzazione array
                    toReturn = list.ToArray();
                }

                //// Lettura dei dati
                //stream.Write(toReturn, 0, toReturn.Length);

                //// Chiusura stream
                //stream.Close();

                //Lettura conmpletata con successo
                logger.Debug("Lettura del file completata con successo");
            }
            catch (EntryPointNotFoundException e)
            {
                logger.Error(e, "Errore DownloadFileFromFTP");

                throw new Exception("Impossibile leggere le informazioni sul file.");
            }
            catch (Exception ex)
            {

                logger.Error("Errore {0} {1}", ex.Message, ex.StackTrace);
                throw new Exception("Impossibile leggere le informazioni sul file.");
            }

            return toReturn;

        }
    }
}
