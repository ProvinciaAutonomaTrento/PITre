using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ServizioPolicyPARER
{
    class Program
    {
        static void Main(string[] args)
        {

            ServizioPolicyPARER s = new ServizioPolicyPARER();

            try
            {
                s.fileLog(Logica.info, "inizio del processo");
                s.Start();
                s.fileLog(Logica.info, "fine del processo");
            }
            catch (Exception ex)
            {
                s.fileLog(Logica.errore, "Eccezione durante l'esecuzione delle policy: " + ex.ToString());
            }
        }
    }
}
