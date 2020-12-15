using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class CreaDelegaDaModelloRequest
    {
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