using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class LibroFirmaCambiaStatoRequest
    {
        public LibroFirmaElement elemento
        {
            get;
            set;
        }

        public UserInfo UserInfo
        {
            get;
            set;
        }

        public string IdorrGlobaliRuolo
        {
            get;
            set;
        }

        public string NuovoStato
        {
            get;
            set;
        }
    }
}
