using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adlib.Director.DirectorWSAWrapper.Jobs
{
    public class JobHandlerBase
    {
        protected bool _StopFlag = false;
        protected object _syncObject = new object();
        protected Logging.LogMngr _LogManager = new Logging.LogMngr();
        protected string _ThreadTag = "BaseThread";

        public string ThreadTag
        {
            get { return _ThreadTag; }
            set { _ThreadTag = value; }
        }


        public virtual void Execute() { }
        public virtual void Execute(Object state) { Execute(); }

        public void Initialize(Logging.LogMngr logMngr)
        {
            if (logMngr != null)
            {
                _LogManager = logMngr;
            }
        }
        public bool NeedStop()
        {
            bool stopFlag = false;
            lock (_syncObject)
            {
                stopFlag = _StopFlag;
            }
            return stopFlag;
        }
    }
}
