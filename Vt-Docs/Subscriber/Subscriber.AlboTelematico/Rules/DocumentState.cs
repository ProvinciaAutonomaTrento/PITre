using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.AlboTelematico.Rules
{
    /// <summary>
    /// Questa classe contiene gli stati di interesse nella comunicazione Subscriber --> ALT
    /// </summary>
    public sealed class DocumentState
    {
        public const string PUBBLICATO = "PUBBLICATO";
        public const string PUBBLICARE = "DA PUBBLICARE";
        public const string REVOCARE = "DA REVOCARE";
        public const string ANNULLARE = "DA ANNULLARE";
        public const string PUBBLICARE_ERRORE = "DA PUBBLICARE ERRORE";
        public const string REVOCARE_ERRORE = "DA REVOCARE ERRORE";
        public const string ANNULLARE_ERRORE = "DA ANNULLARE ERRORE";
        public const string IMPOSSIBILE_ANNULLARE = "PUBBLICATO - IMPOSSIBILE ANNULLARE - SUPERATA LA DATA CONSENTITA";
        public const string IMPOSSIBILE_REVOCARE = "PUBBLICATO - IMPOSSIBILE REVOCARE DOC SENZA REVOCA";
        public const string PUBBLICATO_ERRORE = "DA PUBBLICARE ERRORE";
    }
}
