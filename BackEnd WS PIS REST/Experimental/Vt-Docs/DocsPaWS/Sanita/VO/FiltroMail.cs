using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Sanita.VO
{
    public class FiltroMail
    {
        public TipoFiltroMail Tipo
        {
            get;
            set;
        }

        public string Valore
        {
            get;
            set;
        }
    }


    public enum TipoFiltroMail
    {
        MITTENTE,OGGETTO
    }
}