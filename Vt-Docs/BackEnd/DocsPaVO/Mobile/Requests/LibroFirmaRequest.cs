using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class LibroFirmaRequest
    {
        public UserInfo UserInfo
        {
            get;
            set;
        }

        public string IdGruppo
        {
            get;
            set;
        }

        public int RequestedPage
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public string Testo
        {
            get;
            set;
        }

        public RicercaType TipoRicerca
        {
            get;
            set;
        }
    }
}
