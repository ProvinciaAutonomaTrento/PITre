using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServizioVersamentoPARER
{
    public class ServizioVersamentoPARER
    {

        static Logica logica = null;

        public void Start()
        {
            logica = new Logica();

            bool result = false;
            try 
            {
                result = logica.StartConservazione;
                logica.fileLog(Logica.info, result.ToString(), 6);
            }
            catch (Exception ex)
            {
                fileLog(Logica.errore, ex.Message);
            }
            
        }

        public void fileLog(string tipoLog, string messaggio)
        {
            if (!string.IsNullOrEmpty(Configurazione.pathLog))
                System.IO.File.AppendAllText(@Configurazione.pathLog, "[" + tipoLog + "]:" + "[" + System.DateTime.Now.ToString() + "]" + messaggio + System.Environment.NewLine);
        }
    }
}
