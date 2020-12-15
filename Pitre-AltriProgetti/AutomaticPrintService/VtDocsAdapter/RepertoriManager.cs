using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace VtDocsAdapter
{
    /// <summary>
    /// Questa classe implementa il gestore per le stampe di repertorio
    /// </summary>
    public class RepertoriManager : PrinterManager
    {
        // Informazioni sul tempo di esecuzione del task
        private TimeInfo timeInfo;


        public override void PrintReports(String webServiceUrl)
        {

            // Inizializzo il log
            if (Log == null)
            {
                Log = new StreamWriter("StampaRepertori.log", true);
                Log.AutoFlush = true;
            }
            Log.WriteLine(DateTime.Now + " [INFO] Inizio stampa repertori");
            Log.WriteLine(DateTime.Now + String.Format(" [INFO] Paremetro url passato {0}", webServiceUrl));

            // Stringa da scrivere nel registro di sistema
            // StringBuilder message = new StringBuilder("Stampa repertori.\n");

            // Recupero dei repertori da stampare
            GetRegistersToPrintAutomaticServiceResponse response = base.GetWebServiceInstance(webServiceUrl).GetRegistersToPrint();

            //message.AppendFormat("Sono stati individuati {0} repertori da stampare.\n\n", response.RegistersToPrint.Length);
            Log.WriteLine(DateTime.Now + String.Format(" [INFO] Sono stati individuati {0} repertori da stampare.", response.RegistersToPrint.Length));

            using (Tracer tracer = new Tracer(this.AnalyzeTracerResult))
            {
                // Stampa dei report
                foreach (var print in response.RegistersToPrint)
                {
                    Log.WriteLine(DateTime.Now + String.Format(" [INFO] Generazione stampe per il repertorio {0} ID:{1}. Documenti che verranno prodotti: ", print.CounterDescription, print.CounterId));

                    GetRepertoriPrintRangesResponse ranges = base.GetWebServiceInstance(webServiceUrl).GetRepertoriPrintRanges(
                        new GetRepertoriPrintRangesRequest()
                        {
                            CounterId = print.CounterId,
                            RegistryId = print.RegistryId,
                            RfId = print.RFId
                        });

                    //message.AppendLine(String.Format("Generazione stampe per il repertorio {0}. Documenti che verranno prodotti: ", print.CounterDescription));
                    if (ranges.Ranges.Length > 0)
                        foreach (var r in ranges.Ranges)
                            Log.WriteLine(DateTime.Now + String.Format(" [INFO] Documento con i repertori dal numero {0} al numero {1} dell'anno {2}", r.FirstNumber, r.LastNumber, r.Year));
                    else
                        Log.WriteLine(DateTime.Now + " [INFO] Nessuna stampa da generare.");
                    // Se c'è almeno un range di repertori da stampare, si procede, altrimenti è inutile invocare la stampa
                    // e far scatenare l'eccezione che avverte dell'inesistenza di repertori da stampare
                    if(ranges.Ranges.Length > 0)
                        this.GeneratePrint(print, webServiceUrl);

                    //message.AppendLine(String.Format("Fine stampa repertorio {0}", print.CounterDescription));
                    //message.AppendLine();
                    Log.WriteLine(DateTime.Now + String.Format(" [INFO] Fine stampa repertorio {0}", print.CounterDescription));

                }
            }

            //message.AppendLine(String.Format("Tempo di inizio: {0} - Tempo di fine: {1} - Tempo impiegato per la stampa: {2}", this.timeInfo.StartTime, this.timeInfo.EndTime, this.timeInfo.ElapsedTime));
            //EventLog.WriteEntry("AutomaticPrintService", message.ToString(), EventLogEntryType.Information, 3);
            Log.WriteLine(DateTime.Now + String.Format(" [INFO] Stampa repertori completata. Tempo di inizio: {0} - Tempo di fine: {1} - Tempo impiegato per la stampa: {2}", this.timeInfo.StartTime, this.timeInfo.EndTime, this.timeInfo.ElapsedTime));

        }

        private void GeneratePrint(RegistroRepertorioPrint print, String webServiceUrl)
        {
            // Volani - aggiunto try/catch, in alcuni casi l'utente e/o ruolo di gestione stampe possono
            // essere stati cambiati o disabilitati (valori a null), in amm non c'è nessun controllo di consistenza o alert a riguardo.
            // In questo modo viene saltata solo la stampa che da problemi e le altre continuano invece di bloccare tutto.
            try
            {
                // Costruzione dell'info utente a partire dalla stampa
                InfoUtente userInfo = new InfoUtente()
                {
                    idAmministrazione = print.PrinterUser.idAmministrazione,
                    idCorrGlobali = print.PrinterUser.systemId,
                    idGruppo = print.PrinterRole.idGruppo,
                    idPeople = print.PrinterUser.idPeople,
                    userId = print.PrinterUser.userId
                };

                bool changeStateResult = true;

                // Se il repertorio è aperto, bisogna chiuderlo
                if (print.CounterState == RepertorioState.O)
                    changeStateResult = this.ChangeRepertorioState(print.CounterId, userInfo.idAmministrazione, print.RegistryId, print.RFId, print.CounterDescription, webServiceUrl);

                // Se il contatore è chiuso, si procede con la generazione del report
                if (changeStateResult)
                {
                    this.GenerateDocument(print.CounterId, print.RegistryId, print.RFId, print.PrinterRole, userInfo, print.CounterDescription, webServiceUrl);
                    this.ChangeRepertorioState(print.CounterId, userInfo.idAmministrazione, print.RegistryId, print.RFId, print.CounterDescription, webServiceUrl);

                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(DateTime.Now + String.Format(" [ERROR] - Errore nella stampa del repertorio {0}: {1} ID:{2}", print.CounterDescription, ex.Message, print.CounterId));
                Log.WriteLine(DateTime.Now + " [ERROR] - Verificare che ruoli e utenti responsabile e gestione stampe siano esistenti ed attivi lato amministrazione.");
                // in caso di errore riapro comunque il repertorio
                InfoUtente userInfo = new InfoUtente()
                {
                    idAmministrazione = print.PrinterUser.idAmministrazione,
                    idCorrGlobali = print.PrinterUser.systemId,
                    idGruppo = print.PrinterRole.idGruppo,
                    idPeople = print.PrinterUser.idPeople,
                    userId = print.PrinterUser.userId
                };
                this.ChangeRepertorioState(print.CounterId, userInfo.idAmministrazione, print.RegistryId, print.RFId, print.CounterDescription, webServiceUrl);
                // ----------------------------------------------
            }

        }

        private bool ChangeRepertorioState(String counterId, String adminId, String registryId, String rfId, String counterDescription, String webServiceUrl)
        {
            bool changeStateResult = false;

            try
            {
                changeStateResult = base.GetWebServiceInstance(webServiceUrl).ChangeRepertorioState(
                        new ChangeRepertorioStateRequest()
                        {
                            CounterId = counterId,
                            IdAmm = adminId,
                            RegistryId = registryId,
                            RfId = rfId
                        }).ChangeStateResult;
            }
            catch (Exception e)
            {
                // Recupero eccezione interna
                Exception ex = SoapExceptionParser.GetOriginalException(e);

                //EventLog.WriteEntry("AutomaticPrintService", String.Format("Cambio stato repertorio {0}: {1}", counterDescription, ex.Message), EventLogEntryType.Error);
                Log.WriteLine(DateTime.Now + String.Format(" [ERROR] Errore nel cambio stato repertorio {0}: {1}", counterDescription, ex.Message));
            }

            return changeStateResult;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="registryId"></param>
        /// <param name="rfId"></param>
        /// <param name="printerRole"></param>
        /// <param name="userInfo"></param>
        /// <param name="counterDescription"></param>
        /// <param name="webSericeUrl"></param>
        private void GenerateDocument(String counterId, String registryId, String rfId, Ruolo printerRole, InfoUtente userInfo, String counterDescription, String webSericeUrl)
        {
            try
            {
                GeneratePrintRepertorioResponse response = base.GetWebServiceInstance(webSericeUrl).GeneratePrintRepertorio(new GeneratePrintRepertorioRequest()
                {
                    CounterId = counterId,
                    RegistryId = registryId,
                    RfId = rfId,
                    Role = printerRole,
                    UserInfo = userInfo
                });
            }
            catch (Exception e)
            {
                // Recupero eccezione interna
                Exception ex = SoapExceptionParser.GetOriginalException(e);

                // Logging dell'eccezione originale
                //EventLog.WriteEntry("AutomaticPrintService", String.Format("Errore creazione documento per repertorio {0}: {1}", counterDescription, ex.Message), EventLogEntryType.Error);

                Log.WriteLine(DateTime.Now + String.Format(" [ERROR] Errore creazione documento per repertorio {0}: {1}", counterDescription, ex.Message));

            }
        }

        /// <summary>
        /// Evento registrato nel Tracer per la gestione del tempo di esecuzione
        /// </summary>
        private void AnalyzeTracerResult(object sender, TimeInfo args)
        {
            this.timeInfo = args;
        }

    }
}
