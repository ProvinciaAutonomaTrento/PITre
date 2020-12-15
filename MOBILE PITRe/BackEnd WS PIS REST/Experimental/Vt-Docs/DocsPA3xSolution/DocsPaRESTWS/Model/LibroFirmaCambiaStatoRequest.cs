using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class LibroFirmaCambiaStatoRequest
    {
        public DocsPaVO.Mobile.LibroFirmaElement elemento
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