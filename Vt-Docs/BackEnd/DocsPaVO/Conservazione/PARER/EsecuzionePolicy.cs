using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.PARER
{
    [Serializable()]
    public class EsecuzionePolicy
    {
        public string idPolicy;
        public string dataUltimaEsecuzione;
        public string dataProssimaEsecuzione;
        public string numeroEsecuzioni;
    }
}
