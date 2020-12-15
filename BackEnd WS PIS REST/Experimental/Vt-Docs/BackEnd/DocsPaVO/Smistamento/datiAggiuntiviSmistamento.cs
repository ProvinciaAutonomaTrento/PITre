using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.Smistamento
{
    public class datiAggiuntiviSmistamento
    {
        //a questo oggetto si fa riferimento nell'oggetto RuoloSmistamento e UtenteSmistamento
        //per tenere traccia delle note individuali e dalla data di scadenza della trasmissione singola
        public string NoteIndividuali = string.Empty;
        public string dtaScadenza = string.Empty;
        public string tipoTrasm = string.Empty;
    }
}

 