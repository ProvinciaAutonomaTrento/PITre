using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class ListaModelliTrasmRequest
    {
        public UserInfo UserInfo
        {
            get;
            set;
        }

        public string IdCorrGlobali
        {
            get;
            set;
        }

        public bool TrasmFasc
        {
            get;
            set;
        }
    }
}
