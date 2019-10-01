using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaUtils.LogsManagement
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
