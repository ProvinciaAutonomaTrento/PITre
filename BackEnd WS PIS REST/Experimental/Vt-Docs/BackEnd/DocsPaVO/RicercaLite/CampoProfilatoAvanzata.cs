using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.RicercaLite
{
    public class CampoProfilatoAvanzata : DocsPaVO.ProfilazioneDinamicaLite.CampoProfilato
    {
        /// <summary>
        /// 
        /// </summary>
        public int IsIntervalloDa { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int IsIntervalloA { get; set; }
    }
}
