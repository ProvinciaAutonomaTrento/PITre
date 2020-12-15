using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class DelegheRequest
    {
        public UserInfo UserInfo
        {
            get;
            set;
        }

        public TipoDelega TipoDelega
        {
            get;
            set;
        }

        public StatoDelega StatoDelega
        {
            get;
            set;
        }

    }
}
