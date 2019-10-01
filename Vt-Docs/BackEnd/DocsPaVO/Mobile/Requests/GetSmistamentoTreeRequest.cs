using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class GetSmistamentoTreeRequest
    {
        public UserInfo UserInfo
        {
            get;
            set;
        }

        public RuoloInfo Ruolo
        {
            get;
            set;
        }

        // MEV MOBILE - smistamento
        // per navigazione UO inf/sup
        public string IdUO
        {
            get;
            set;
        }
    }
}
