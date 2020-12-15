using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.trasmissione;

namespace DocsPaWS.Sanita.VO
{
    public class TrasmissioneVO
    {
        public string CodiceCorrispondente
        {
            get;
            set;
        }

        public string Ragione
        {
            get;
            set;
        }

        public int GiorniScadenza
        {
            get;
            set;
        }

        public string Note
        {
            get;
            set;
        }

        public string NoteSingola
        {
            get;
            set;
        }
    }
}