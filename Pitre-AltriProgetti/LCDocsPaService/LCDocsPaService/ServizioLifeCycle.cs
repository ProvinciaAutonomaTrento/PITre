using System;
using System.ServiceProcess;
using System.Timers;

namespace LCDocsPaService
{
    partial class ServizioLifeCycle : ServiceBase
    {
        public System.Timers.Timer myTimer = null;

        static Logica logica = null;

        public ServizioLifeCycle()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            logica = new Logica();
            fileLog(Logica.info, "inizio del processo");
            myTimer = new System.Timers.Timer();
            myTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            // myTimer.Interval = 2000;
            myTimer.Interval = @Configurazione.pollingDelay;
            myTimer.Enabled = true;

            GC.KeepAlive(myTimer);
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            bool result = false;
            myTimer.Enabled = false;
            try
            {
                result = logica.StartLifeCycleCheck;
            }
            catch (Exception exce)
            {
                fileLog(Logica.errore, exce.Message);
            }

            myTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            fileLog(Logica.info, "fine del processo");
        }

        private void fileLog(string tipoLog, string messaggio)
        {
            if (!string.IsNullOrEmpty(Configurazione.pathLog))
                System.IO.File.AppendAllText(@Configurazione.pathLog, "[" + tipoLog + "]:" + "[" + System.DateTime.Now.ToString() + "]" + messaggio + System.Environment.NewLine);
        }

        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

      
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.ServiceName = "ServizioLifecycleDocsapa";
        }
    }
}
