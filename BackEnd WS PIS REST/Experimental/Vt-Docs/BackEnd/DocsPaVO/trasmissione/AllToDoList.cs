using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.trasmissione
{
    [Serializable()]
    public class AllToDoList
    {
        public string ruoloDesc = string.Empty;
        public string ruoloId = string.Empty;
        public int trasmDocTot = 0;
        public int trasmDocNonLetti = 0;
        public int trasmDocNonAccettati = 0;
        public int trasmFascTot = 0;
        public int trasmFascNonLetti = 0;
        public int trasmFascNonAccettati = 0;
        public int docPredisposti = 0;
    }
}