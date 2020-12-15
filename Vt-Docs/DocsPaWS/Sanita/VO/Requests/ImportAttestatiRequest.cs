using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.utente;

namespace DocsPaWS.Sanita.VO.Requests
{
    public class ImportAttestatiRequest
    {
        public AttestatiListVO Attestati
        {
            get;
            set;
        }

        public string IdRegistro
        {
            get;
            set;
        }

        public UserInfoVO InfoUtente
        {
            get;
            set;
        }

        public RuoloInfoVO Ruolo
        {
            get;
            set;
        }

        public TrasmissioneVO Trasmissione
        {
            get;
            set;
        }
    }
}
