using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.areaConservazione
{
    [Serializable()]
    public class ChiaveVersamento
    {
        public string numero;
        public string anno;
        public string tipoRegistro;
    }
}
