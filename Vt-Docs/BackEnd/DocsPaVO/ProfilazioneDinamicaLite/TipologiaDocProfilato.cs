using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.ProfilazioneDinamicaLite
{
    public class TipologiaDocProfilato
    {
        public string nome {get; set;}
        public CampoProfilato[] listaCampiProfilati {get; set;}
    }
}
