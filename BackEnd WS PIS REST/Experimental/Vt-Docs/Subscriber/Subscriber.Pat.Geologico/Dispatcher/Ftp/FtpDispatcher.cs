using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Subscriber.Pat.Geologico.Dispatcher.Ftp
{
    /// <summary>
    /// Dispatcher per la pubblicazione dei contenuti tramite Ftp
    /// </summary>
    public class FtpDispatcher : Subscriber.Dispatcher.IDispatcher
    {
        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(FtpDispatcher));

        /// <summary>
        /// 
        /// </summary>
        private string _fileName = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private BaseRuleInfo _rule = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="rule"></param>
        public FtpDispatcher(string fileName, BaseRuleInfo rule)
        {
            this._fileName = fileName;
            this._rule = rule;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Dispatch(Stream data)
        {
            string address = this._rule.GetOptionByName("address", true);
            string userName = this._rule.GetOptionByName("userName", true);
            string password = this._rule.GetOptionByName("password", true);
            string directory = this._rule.GetOptionByName("directory", true);

            string path = string.Format("ftp://{0}/{1}/{2}", address, directory, this._fileName);

            _logger.DebugFormat("Pubblicazione su path FTP '{0}'", path);

            // Creazione della request FTP
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(path);

            // Download
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // Impostazione delle credenziali
            request.Credentials = new NetworkCredential(userName, password);

            // Impostazione parametri di connessione
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            data.Position = 0;
            byte[] buffer = new byte[data.Length];
            data.Read(buffer, 0, buffer.Length);

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Dispatch(object data)
        {
            this.Dispatch(data as Stream);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }
    }
}
