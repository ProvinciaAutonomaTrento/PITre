using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.documento
{
    [Serializable()]
    public class FascicolazioneTipiDocumento
    {
        /// <summary>
        /// 
        /// </summary>
        public string Codice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool FascicolazioneObbligatoria { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IdAmministrazione { get; set; }
    }
}
