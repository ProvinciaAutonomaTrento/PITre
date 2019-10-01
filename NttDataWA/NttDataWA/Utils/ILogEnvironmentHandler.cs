using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NttDataWA.Utils
{
    public interface ILogEnvironmentHandler
    {
        string FileName
        {
            get;
        }

        string FilePath
        {
            get;
        }

        bool Enabled
        {
            get;
        }
    }
}