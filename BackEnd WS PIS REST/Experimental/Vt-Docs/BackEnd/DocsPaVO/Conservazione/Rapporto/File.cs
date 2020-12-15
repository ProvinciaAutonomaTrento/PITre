using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.Rapporto
{
    [Serializable]
    public class File
    {
        public string URN { get; set; }

        public string Hash { get; set; }

        public string AlgoritmoHash { get; set; }

        public string Encoding { get; set; }
    }
}
