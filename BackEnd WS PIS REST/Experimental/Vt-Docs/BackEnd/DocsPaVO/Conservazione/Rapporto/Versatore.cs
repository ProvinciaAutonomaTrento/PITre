using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.Rapporto
{
    [Serializable]
    public class Versatore
    {
        public string Amministrazione { get; set; }

        public string Struttura { get; set; }

        public string UserID { get; set; }
    }
}
