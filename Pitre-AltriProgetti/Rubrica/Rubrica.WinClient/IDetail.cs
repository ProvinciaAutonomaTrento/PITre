using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rubrica.WinClient
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDetail : IDisposable
    {
        void SetCredentials(ClientProxy.SecurityCredentials credentials);
 
        object Current { get; }

        void RequestInsert();

        void LoadData();

        void SaveCurrent();

        void DeleteCurrent();
    }
}
