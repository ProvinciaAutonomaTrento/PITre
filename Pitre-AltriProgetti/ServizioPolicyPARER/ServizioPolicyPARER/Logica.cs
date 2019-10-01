using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ServizioPolicyPARER
{
    class Logica
    {

        public const string errore = "ERROR";
        public const string info = "INFO";
        public const string debug = "DEBUG";

        private DocsPaConservazione.PolicyExecutionServices wsConservazione = null;

        public Logica()
        {
            wsConservazione = new DocsPaConservazione.PolicyExecutionServices();
            wsConservazione.Timeout = System.Threading.Timeout.Infinite;

            fileLog(debug, string.Format("Indirizzo wsConservazione {0}", wsConservazione.Url), 4);
        }

        public void fileLog(string tipoLog, string messaggio, int loglevel)
        {
            if (Configurazione.logLevel > loglevel)
            {
                if (!string.IsNullOrEmpty(Configurazione.pathLog))
                    File.AppendAllText(@Configurazione.pathLog, "[" + tipoLog + "]:" + "[" + System.DateTime.Now.ToString() + "]" + messaggio + System.Environment.NewLine);
            }
        }

        public bool StartConservazione
        {
            get
            {
                fileLog(info, "chiamaWebService:Start ", 6);
                bool result = false;
                try
                {
                    DocsPaConservazione.PolicyExecutionServices ws = new DocsPaConservazione.PolicyExecutionServices();
                    ws.Timeout = System.Threading.Timeout.Infinite;

                    // ESECUZIONE POLICY
                    fileLog(info, "Esecuzione policy", 5);
                    ws.ExecutePolicyPARER();
                    fileLog(info, "Web service chiamato con successo!", 5);

                    result = true;

                }
                catch (Exception e)
                {
                    fileLog(info, String.Format("Errore nella chiamata al web services! {0} : {1} ", e.Message, e.StackTrace), 0);
                }
                return result;
            }
        }

    }
}
