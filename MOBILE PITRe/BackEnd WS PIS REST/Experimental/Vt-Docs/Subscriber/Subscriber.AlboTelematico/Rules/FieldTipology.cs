using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.AlboTelematico.Rules
{
    /// <summary>
    /// Campi comuni ai tipi documento
    /// </summary>
    public sealed class FieldTipology
    {        
        /// <summary>
        /// Nome dell'ente per il quale si sta pubblicando
        /// </summary>
        public const string NOME_ENTE_PER_CUI_PUBBLICA = "Nome dell'Ente per cui si pubblica";

        /// <summary>
        /// Data di inizio validità dell'atto
        /// </summary>
        public const string DATA_ATTO = "Data dell'atto";
        /// <summary>
        /// Numerazione dell'atto che lo identifica in maniera univoca
        /// </summary>
        public const string NUMERO_ATTO = "Numero dell'atto";
        
        /// <summary>
        /// Organo dell'ente che ha emanato l'atto
        /// </summary>
        public const string ORGANO_EMANANTE = "Organo emanante";
        
        /// <summary>
        /// Oggetto dell'atto
        /// </summary>
        public const string OGGETTO = "Oggetto";
        
        /// <summary>
        /// Durata della pubblicazione. Nel caso di pubblicazione con revoca si assume che il valore sia 0
        /// </summary>
        public const string DURATA = "Durata";
        
        /// <summary>
        /// E' il documento che deve essere pubblicato su albo telematico
        /// </summary>
        public const string DATA_PUBBLICAZIONE = "Data di pubblicazione";
        
        /// <summary>
        /// Indica se al termine della pubblicazione deve essere archiviato da albo
        /// </summary>
        public const string ARCHIVIA = "Archivia";
        
        /// <summary>
        /// Indica se l'atto è immediatamente eseguibile
        /// </summary>
        public const string IMMEDIATA_ESEGUIBILITA = "Immediata eseguibilità";
    }
}
