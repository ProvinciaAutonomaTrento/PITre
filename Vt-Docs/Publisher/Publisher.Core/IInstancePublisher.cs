using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Publisher
{
    /// <summary>
    /// Interfaccia per la pubblicazione di oggetti
    /// </summary>
    public interface IInstancePublisher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        void Publish(ChannelRefInfo instance);
    }
}
