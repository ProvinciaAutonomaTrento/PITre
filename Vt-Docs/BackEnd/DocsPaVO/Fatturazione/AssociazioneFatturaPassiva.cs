using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Fatturazione
{
    [Serializable]
    public class AssociazioneFatturaPassiva
    {
        public String IdFattura { get; set; }

        public String IdSdi { get; set; }

        public String Docnumber { get; set; }
    }
}
