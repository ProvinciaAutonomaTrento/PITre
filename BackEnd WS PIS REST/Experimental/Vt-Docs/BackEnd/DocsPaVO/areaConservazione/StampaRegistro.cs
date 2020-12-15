using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.areaConservazione
{
    [Serializable()]
    public class StampaRegistro
    {
        public string docNumber;
        public string anno;
        public string idRegistro;
        public string idRepertorio;
        public string numProtoStart;
        public string numProtoEnd;
        public string dtaStampaTruncString;
    }
}
