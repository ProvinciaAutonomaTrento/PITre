using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Publisher
{
    /// <summary>
    /// Stati del canale di pubblicazione
    /// </summary>
    [XmlType(Namespace = "http://www.valueteam.com/VTDocs/Publisher")]
    public enum ChannelStateEnum
    {
        /// <summary>
        /// Canale avviato
        /// </summary>
        Started,

        /// <summary>
        /// Canale fermato
        /// </summary>
        Stopped,

        /// <summary>
        /// Canale fermo in maniera inaspettata
        /// </summary>
        UnexpectedStopped,
    }
}
