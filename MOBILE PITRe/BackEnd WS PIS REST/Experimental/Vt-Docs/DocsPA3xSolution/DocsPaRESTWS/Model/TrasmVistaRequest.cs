using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class TrasmVistaRequest
    {
        public string IdTrasmissione
        {
            get;
            set;
        }

        public string IdTrasmissioneUtente
        {
            get;
            set;
        }

        public string Note
        {
            get;
            set;
        }

        public string Action
        {
            get;
            set;
        }

        public string IdEvento { get; set; }
    }
}