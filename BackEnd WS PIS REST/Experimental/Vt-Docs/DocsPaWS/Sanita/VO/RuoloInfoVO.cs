using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.utente;
using DocsPaVO.amministrazione;

namespace DocsPaWS.Sanita.VO
{
    public class RuoloInfoVO
    {

        public string Descrizione
        {
            get; 
            set; 
        }

        public string IdGruppo
        {
            get; 
            set;
        }

        public string Codice
        {
            get; 
            set;
        }

        public string Id
        {
            get; 
            set;
        }
    }
}