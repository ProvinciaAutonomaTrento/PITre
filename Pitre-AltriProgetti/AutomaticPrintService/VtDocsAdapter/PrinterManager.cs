using System;
using System.Threading;
using System.IO;

namespace VtDocsAdapter
{
    /// <summary>
    /// Questa classe rappresenta un 
    /// </summary>
    public abstract class PrinterManager
    {
        // Istanza del WebService
        private DocsPaWebService _docsPaWS;

        // Modifica di Volani - 15/07/13
        // Logger su file 
        private StreamWriter log;

        public StreamWriter Log
        {
            get { return log; }
            set { log = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webServiceUrl"></param>
        /// <returns></returns>
        protected DocsPaWebService GetWebServiceInstance(String webServiceUrl)
        {
            if (_docsPaWS == null)
            {
                this._docsPaWS = new DocsPaWebService(webServiceUrl);
                this._docsPaWS.Timeout = Timeout.Infinite;
            }

            return this._docsPaWS;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webServiceUrl"></param>
        public abstract void PrintReports(String webServiceUrl);

    }
}
