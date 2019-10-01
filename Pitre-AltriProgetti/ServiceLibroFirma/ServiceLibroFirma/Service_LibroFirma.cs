using System;
using System.Collections;
using System.ServiceProcess;
using System.Text;
using System.Configuration;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using DocsPaUtils;
//using ServiceNotifications.CoreNotification;
using ServiceLibroFirma.CoreService;

namespace ServiceLibroFirma
{
    class Service_LibroFirma : ServiceBase
    {
        private bool serviceStarted = false;
        
        Thread engineThread;

        /// <summary>
        /// Entry point
        /// </summary>
        static void Main()
        {
            //Creation of windows service
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new Service_LibroFirma() };
            //execution of the service windows
            System.ServiceProcess.ServiceBase.Run(ServicesToRun);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // The service name is linked to the instance of vt-docs
            this.ServiceName = System.Configuration.ConfigurationManager.AppSettings["name_service"];
        }

        /// <summary>
        /// Start service
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            // Create worker thread; this will invoke the WorkerFunction  
            // when we start it.  
            // Since we use a separate worker thread, the main service  
            // thread will return quickly, telling Windows that service has started  
            ThreadStart threadEngine = new ThreadStart(StartupLibroFirmaEngine);
            engineThread = new Thread(threadEngine);

            // set flag to indicate worker thread is active  
            serviceStarted = true;

            // start the engine thread  
            engineThread.Start();
        }

        /// <summary>
        /// stop service
        /// </summary>
        protected override void OnStop()
        {
            // flag to tell the worker process to stop  
            serviceStarted = false;
            // give it a little time to finish any pending work  
            engineThread.Join(new TimeSpan(0, 5, 0));
        }

        
        /// <summary>
        ///
        /// </summary>
        private void StartupLibroFirmaEngine()
        {
            int sleep = 4000;
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings["sleep_service"] != null)
                {
                    sleep = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["sleep_service"]);
                }

                if (System.Configuration.ConfigurationManager.AppSettings["logFilePath"] != null)
                {
                    logWriter.logSourcePath = System.Configuration.ConfigurationManager.AppSettings["logFilePath"].ToString();

                    if (!System.IO.File.Exists(logWriter.logSourcePath))
                        System.IO.File.CreateText(logWriter.logSourcePath);
                }

                if (System.Configuration.ConfigurationManager.AppSettings["logAttivo"] != null)
                {
                    string strlogAttivo = System.Configuration.ConfigurationManager.AppSettings["logAttivo"];
                    string strloglevel = System.Configuration.ConfigurationManager.AppSettings["logLevel"];

                    if (strlogAttivo.ToUpper().Trim() == "NO" || strlogAttivo.ToUpper().Trim() == "FALSE" || strlogAttivo.Trim() == "0")
                    {
                        logWriter.logAttive = false;
                    }
                    else
                    {
                        logWriter.logAttive = true;
                    }

                    if (!string.IsNullOrEmpty(strloglevel))
                    {
                        int logLevel;
                        try
                        {
                            int.TryParse(strloglevel, out logLevel);
                            if (logLevel < 1)
                                logLevel = 1;
                            else if (logLevel > 3)
                                logLevel = 3;
                        }
                        catch
                        {
                            logLevel = 1;
                        }

                        logWriter.loglevel = logLevel;
                    }

                    logWriter.addLog(logWriter.INIT, "ATTESA: " + (sleep/1000).ToString() + " SECONDI");
                    logWriter.addLog(logWriter.INIT, "LOG ATTIVO, LIVELLO " + logWriter.loglevel + (logWriter.loglevel == 1 ? " (ERRORI)" : (logWriter.loglevel == 2 ? " (ERRORI e INFO)" : " (ERRORI, INFO e DEBUG)")));
                }
                else 
                {
                    logWriter.logAttive = false;
                }
            }
            catch (Exception exc)
            {
                EventLog.WriteEntry(exc.Message + exc.InnerException + exc.Source +exc.StackTrace + exc.Data);
            }

            string[] listOfEventType = null;
            if (System.Configuration.ConfigurationManager.AppSettings["SignificativeEvent"] != null)
            {
                string tempList = System.Configuration.ConfigurationManager.AppSettings["SignificativeEvent"];
                listOfEventType = tempList.Split('|');
            }

            string eventWithoutActor = null;
            if (System.Configuration.ConfigurationManager.AppSettings["EventWithoutActor"] != null)
            {
                eventWithoutActor = System.Configuration.ConfigurationManager.AppSettings["EventWithoutActor"];
            }
            
            // start an endless loop; loop will abort only when serviceStarted == false
            while (serviceStarted)
            {
                    EventProcessingEngine engineEvent = new EventProcessingEngine();

                try
                {
                    Thread.Sleep(sleep);
                    engineEvent.ListToBeProcessed(listOfEventType);
                    engineEvent.ElaborateListOfEvents(eventWithoutActor);
                    engineEvent.CloseManualProcess();
                    //engineEvent.ClearDatabase();
                }
                catch (Exception exc)
                {
                    logWriter.addLog(logWriter.ERRORE, exc.Message);
                    //if (exc.InnerException!=null)
                    //    EventLog.WriteEntry(exc.Message + exc.Data.ToString() + exc.InnerException.Message  + exc.StackTrace + "\n");
                    //else
                    //    EventLog.WriteEntry(exc.Message + exc.Data.ToString() + exc.StackTrace + "\n");
                }
            }
                    
            // time to end the thread
            Thread.CurrentThread.Abort();
        }
    }
}
