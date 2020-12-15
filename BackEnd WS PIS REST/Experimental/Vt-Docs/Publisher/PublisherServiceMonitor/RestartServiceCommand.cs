using log4net;
using Publisher.Proxy;
using System;

namespace PublisherServiceMonitor
{
    public class RestartServiceCommand : ICommand
    {
        protected static readonly ILog _logger = LogManager.GetLogger(typeof(RestartServiceCommand));

        static RestartServiceCommand()
        {
        }

        public void Execute(CommandArgs args)
        {
            RestartServiceCommand._logger.Info((object)"BEGIN - RestartServiceCommand.Execute");
            try
            {
                using (PublisherWebService publisherWebService = new PublisherWebService())
                {
                    publisherWebService.Url = args.PublisherServiceUrl;
                    publisherWebService.RestartUnexpectedStoppedChannels();
                    RestartServiceCommand._logger.InfoFormat("RestartServiceCommand.Execute - Url: {0}", (object)args.PublisherServiceUrl);
                }
            }
            catch (Exception ex)
            {
                RestartServiceCommand._logger.Error((object)"ERROR - RestartServiceCommand.Execute", ex);
                throw ex;
            }
            finally
            {
                RestartServiceCommand._logger.Info((object)"END - RestartServiceCommand.Execute");
            }
        }
    }
}
