using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaVO.filtri
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