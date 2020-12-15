using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class RicercaUtentiRequest
    {
        public string Descrizione
        {
            get;
            set;
        }

        public UserInfo UserInfo
        {
            get;
            set;
        }

        public int NumMaxResults
        {
            get;
            set;
        }

        public RuoloInfo Ruolo
        {
            get;
            set;
        }
    }
}
