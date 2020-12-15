using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Interoperabilità
{
    /// <summary>
    /// Questa classe fornisce tutti i metodi utilizzati per la gestione della funzionalità che consenta di scegliere il tipo
    /// di trasmissione uno/tutti per l'interoperabilità interna.
    /// </summary>
    public partial class InteroperabilitaSegnatura
    {
        /// <summary>
        /// Nome della chiave che abilita la possibilà di scegliere se la trasmimssione per interoperabilità deve essere di tipo
        /// uno o tutti.
        /// </summary>
        public const String TRASM_UNO_TUTTI_INTEROP = "TRASM_UNO_TUTTI_INTEROP";
        
        /// <summary>
        /// Metodo utilizzato per determinare il tipo di trasmissione per interoperabilità (S -> singola, T -> Tutti)
        /// </summary>
        /// <param name="adminId">Id dell'amministrazione</param>
        /// <returns>Tipologia della trasmissione per interoperabilità</returns>
        public static String GetInteropTrasmType(String adminId)
        {
            string value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(adminId, TRASM_UNO_TUTTI_INTEROP);
            if (String.IsNullOrEmpty(value))
                value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", TRASM_UNO_TUTTI_INTEROP);

            // Per evitare errori di configurazione, solo se la chiave è valorizzata con T viene cambiato il valore altrimenti
            // per default la trasmissione è S.
            if (String.IsNullOrEmpty(value) || (value.Trim() != "S" && value.Trim() != "T"))
                value = "S";
            else
                value = value.Trim();

            return value;

        }
    }
}
