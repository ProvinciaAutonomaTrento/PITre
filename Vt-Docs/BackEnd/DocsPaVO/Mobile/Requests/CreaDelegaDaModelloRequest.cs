using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class CreaDelegaDaModelloRequest
    {
        public UserInfo UserInfo
        {
            get;
            set;
        }

        public string IdModelloDelega
        {
            get;
            set;
        }

        public DateTime DataInizio
        {
            get;
            set;
        }

        public DateTime DataFine
        {
            get;
            set;
        }
    }
}
