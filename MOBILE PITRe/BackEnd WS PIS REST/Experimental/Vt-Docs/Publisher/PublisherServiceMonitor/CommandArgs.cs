using System;

namespace PublisherServiceMonitor
{
    [Serializable]
    public class CommandArgs
    {
        public string PublisherServiceUrl { get; set; }
    }
}
