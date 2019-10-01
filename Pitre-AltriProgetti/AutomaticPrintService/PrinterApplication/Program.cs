using System;
using VtDocsAdapter;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace PrinterApplication
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
          
            StreamWriter log = new StreamWriter("StampaRepertori.log", true);
            log.AutoFlush = true;
            
            try
            {
                // Se c'è almeno un argomento di cui il primo è un url valido, viene avviata la stampa
                PrinterManager manager = new RepertoriManager();
                manager.Log = log;

                if (args.Length > 0 && Uri.IsWellFormedUriString(args[0], UriKind.Absolute))
                    manager.PrintReports(args[0]);
                else
                    log.WriteLine(DateTime.Now + " [WARN] Attenzione! L'argomento passato al servizio di stampa non è un URL valido");
                    //EventLog.WriteEntry("AutomaticPrintService", "Attenzione! L'argomento passato al servizio di stampa non è un URL valido", EventLogEntryType.Warning, 1);
            }
            catch (Exception e)
            {
                //EventLog.WriteEntry("AutomaticPrintService", String.Format("Eccezione durante la stampa dei repertori. Segue dettaglio.\n\n{0}", e.ToString()), EventLogEntryType.Error, 2);
                log.WriteLine(DateTime.Now + String.Format("[ERROR] Eccezione durante la stampa dei repertori. Segue dettaglio.\n\n{0}", e.ToString()) );                    
            }

            log.Close();
        }
    }
}
