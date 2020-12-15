using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using Publisher.Schedule;

namespace PublisherServiceMonitor
{
    public partial class PublisherServiceMonitor : ServiceBase
    {

        protected static readonly ILog _logger = LogManager.GetLogger(typeof(PublisherServiceMonitor));
       

        public PublisherServiceMonitor()
        {
            this.InitializeComponent();
            this.ServiceName = "PublisherServiceMonitor";
            this.EventLog.Log = "Application";
            this.CanHandlePowerEvent = true;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = false;
            this.CanShutdown = true;
            this.CanStop = true;
        }

        protected virtual string[] GetUrls()
        {
            string path = ConfigurationManager.AppSettings["urlFile"];
            PublisherServiceMonitor._logger.InfoFormat("UrlFilePath: {0}", (object)path);
            return File.ReadAllLines(path);
        }


        protected override void OnStart(string[] args)
        {
            try
            {
                PublisherServiceMonitor._logger.Info((object)"BEGIN - OnStart");
                this._timer = new ScheduleTimer();
                this._timer.Error += new ExceptionEventHandler(this.OnScheduleError);
                ScheduledTime scheduledTime = new ScheduledTime(EventTimeBase.ByMinute, TimeSpan.FromMinutes(10.0));
                List<CommandArgs> list = new List<CommandArgs>();
                foreach (string str in this.GetUrls())
                {
                    list.Add(new CommandArgs()
                    {
                        PublisherServiceUrl = str
                    });
                    PublisherServiceMonitor._logger.InfoFormat("Url '{0}' initialized.", (object)str);
                }
                this._timer.AddJob((IScheduledItem)scheduledTime, (Delegate)new ScheduledEventHandler(this.ExecuteCommand), new object[1]
        {
          (object) list
        });
                this._timer.Start();
                PublisherServiceMonitor._logger.Info((object)"END - OnStart");
            }
            catch (Exception ex)
            {
                PublisherServiceMonitor._logger.Error((object)ex.Message, ex);
            }
        }

        protected override void OnStop()
        {
            PublisherServiceMonitor._logger.Info((object)"BEGIN - OnStop");
            if (this._timer != null)
            {
                this._timer.ClearJobs();
                this._timer.Stop();
                this._timer.Dispose();
            }
            PublisherServiceMonitor._logger.Info((object)"END - OnStop");
        }

        protected virtual void ExecuteCommand(object sender, ScheduledEventArgs e)
        {
            try
            {
                PublisherServiceMonitor._logger.Info((object)"BEGIN - ExecuteCommand");
                foreach (CommandArgs args in (List<CommandArgs>)sender)
                    new RestartServiceCommand().Execute(args);
            }
            catch (Exception ex)
            {
                PublisherServiceMonitor._logger.Error((object)"ERROR - ExecuteCommand", ex);
            }
            finally
            {
                PublisherServiceMonitor._logger.Info((object)"END - ExecuteCommand");
            }
        }

        protected void OnScheduleError(object sender, ExceptionEventArgs e)
        {
        }

    }
}
