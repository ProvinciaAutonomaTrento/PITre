using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VTDocsMobile.VTDocsWSMobile;

namespace VTDocs.mobile.fe.commands
{
    public abstract class Command<X>
    {
        private NavigationHandler _navigationHandler = new NavigationHandler();

        public NavigationHandler NavigationHandler
        {
            get { return _navigationHandler; }
        }

        private ConfigurationHandler _configurationHandler = new ConfigurationHandler();

        public ConfigurationHandler ConfigurationHandler
        {
            get { return _configurationHandler; }
        }

        public VTDocsWSMobileClient WSStub
        {
            get
            {
                return new VTDocsWSMobileClient();
            }
        }

        public abstract X Execute();
    }
}
