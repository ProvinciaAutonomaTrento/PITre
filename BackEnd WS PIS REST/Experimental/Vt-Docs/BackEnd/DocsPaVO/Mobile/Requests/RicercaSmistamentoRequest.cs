using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class RicercaSmistamentoRequest
    {
        public UserInfo UserInfo
        {
            get;
            set;
        }

        public string Descrizione
        {
            get;
            set;
        }

        public int NumMaxResults
        {
            get;
            set;
        }


        public int numMaxResultsForCategory
        {
            get;
            set;
        }

        public RuoloInfo Ruolo
        {
            get;
            set;
        }

        // MEV MOBILE
        public string Ragione
        {
            get;
            set;
        }
    }
}
