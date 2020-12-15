using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Publisher
{
    /// <summary>
    /// Classe astratta per il mapping di oggetti da pubblicare
    /// </summary>
    public interface IDataMapper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logInfo"></param>
        /// <param name="ev"></param>
        /// <returns></returns>
        Subscriber.Proxy.PublishedObject Map(VtDocs.LogInfo logInfo, EventInfo ev);
    }
}
