using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.Rapporto
{
    [Serializable]
    public class Tipologia
    {
        public string Nome { get; set; }
        public Campo[] CampiProfilati { get; set; }
    }

    [Serializable]
    public class Campo
    {
        public string Nome { get; set; }

        public string Valore { get; set; }
    }
}
