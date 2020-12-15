using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ServizioVersamentoPARER
{
    class Program
    {
        static void Main(string[] args)
        {

            ServizioVersamentoPARER s = new ServizioVersamentoPARER();

            try
            {
                s.fileLog(Logica.info, "inizio del processo");
                s.Start();
                s.fileLog(Logica.info, "fine del processo");
            }
            catch (Exception ex)
            {
                s.fileLog(Logica.errore, "Eccezione durante il versamento: " + ex.ToString());
            }
        }
    }
}
