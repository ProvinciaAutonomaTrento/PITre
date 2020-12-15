using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.utente;

namespace DocsPaVO.Sanita.Requests
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

        public InfoUtente InfoUtente
        {
            get;
            set;
        }

        public Ruolo Ruolo
        {
            get;
            set;
        }
    }
}
