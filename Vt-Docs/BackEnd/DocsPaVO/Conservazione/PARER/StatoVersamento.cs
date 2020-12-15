using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Conservazione.PARER
{
    public struct StatoVersamento
    {
        public const string IN_ATTESA = "V";
        public const string VERSAMENTO_IN_CORSO = "W";
        public const string PRESO_IN_CARICO = "C";
        public const string RIFIUTATO = "R";
        public const string ERRORE_INVIO = "E";
        public const string TIMEOUT_INVIO = "T";
        public const string FALLITO = "F";

        public const string IN_ATTESA_BIG_FILE = "B";
    }
}
