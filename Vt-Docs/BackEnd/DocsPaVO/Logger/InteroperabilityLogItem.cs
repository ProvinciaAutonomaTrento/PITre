using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Interoperabilita.Semplificata;

namespace DocsPaVO.Logger
{
    /// <summary>
    /// Questa classe rappresenta una voce del log relativo all'interoperabilità semplificata.
    /// </summary>
    [Serializable]
    public class InteroperabilityLogItem
    {
        public int ProfileId { get; set; }

        public String MessageIdentifier { get; set; }

        public bool ReceivedPrivate { get; set; }

        public String Subject { get; set; }

        public String SenderDescription { get; set; }

        public String SenderUrl { get; set; }

        public bool IsErrorMessage { get; set; }

        public String LogMessage { get; set; }

        public RecordInfo SenderRecordInfo { get; set; }

        public String ReceiverCode { get; set; }

    }

}
