using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.areaConservazione
{
    [Serializable()]
    public class TipoSupporto
    {
        public string SystemId;
        public string TipoSupp;
        public string Capacità;
        public string Periodo_ver;
        public string Descrizione;
    }
}
