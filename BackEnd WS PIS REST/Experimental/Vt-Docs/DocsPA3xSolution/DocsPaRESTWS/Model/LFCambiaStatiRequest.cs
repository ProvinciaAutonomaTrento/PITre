using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class LFCambiaStatiRequest
    {
        public ElementToChange[] Elementi { get; set; }
    }

    public class ElementToChange
    {
        public DocsPaVO.Mobile.LibroFirmaElement Elemento
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