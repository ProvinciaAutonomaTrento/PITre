using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile
{
    public class SmistamentoTree
    {
        public SmistamentoUONode UOAppartenenza
        {
            get;
            set;
        }

        public List<SmistamentoUONode> AltreUO
        {
            get;
            set;
        }

        // MEV MOBILE - smistamento
        // navigazione UO inferiori/superiori
        public string idParent
        {
            get;
            set;
        }
    }
}
