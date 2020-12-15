using System;
using System.ServiceProcess;
using System.Text;
using System.Configuration;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using ServiceNotifications.CoreNotification;
using log4net;

namespace ServiceNotifications
{
    class ServiceNotify : ServiceBase
    {
        // This is a flag to indicate the service status
        private bool serviceStarted = false;
        private static ILog logger = LogManager.GetLogger(typeof(ServiceNotify));
        // the thread that will do the work
        Thread engineThread;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // The service name is linked to the instance of vt-docs
            this.ServiceName = @Configurazione.getServiceName();
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
            logger.Info("Starting");
            ThreadStart threadEngine = new ThreadStart(StartupNotificationEngine);
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
            logger.Info("Stopping");
            // flag to tell the worker process to stop  
            serviceStarted = false;
            // give it a little time to finish any pending work  
            engineThread.Join(new TimeSpan(0, 5, 0));
        }

        /// <summary>
        ///
        /// </summary>
        private void StartupNotificationEngine()
        {
            logger.Debug("Start Mainthread.. waiting 5 seconds to allow debug to attach");
            System.Threading.Thread.Sleep(5000);
            logger.Info("Started");
            int sleep = @Configurazione.getSleepServiceValue();
            // start an endless loop; loop will abort only when serviceStarted == false
            while (serviceStarted)
            {
                EventProcessingEngine engineEvent = new EventProcessingEngine();
                try
                {
                    Thread.Sleep(sleep);
                    logger.Debug("Start Cycle");
                    engineEvent.ListToBeProcessed();
                    engineEvent.BuildRecipientsOfEvents();
                    engineEvent.FilterByAssertions();
                    engineEvent.FilterByFollowDomainObject();
                    engineEvent.CreatesEventNotifications();
                    engineEvent.WriteEventsNotifications();
                    engineEvent.UnlockEventQueue();
                    logger.Debug("End Cycle");
                }
                catch (Exception exc)
                {
                    engineEvent.UnlockEventQueue();
                    logger.Error(exc);
                    EventLog.WriteEntry(exc.Message + exc.Data + exc.InnerException + exc.StackTrace + "\n");
                }
            }
            // time to end the thread
            Thread.CurrentThread.Abort();
        }
    }
}
