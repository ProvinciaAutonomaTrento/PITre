using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class AccettaRifiutaTrasmRequest
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


        public AccettaRifiutaAction Action
        {
            get;
            set;
        }
        
    }

    public enum AccettaRifiutaAction
    {
        ACCETTA,RIFIUTA
    }
}
