using System;

namespace MText.DomainObjects
{
    /// <summary>
    /// Oggetto utilizzato per rappresentare coppie Label-Value in cui Label è l'etichetta che identifica il nome
    /// di un campo profilato M/Text e Value è il valore associato al campo
    /// </summary>
    public class LabelValuePair
    {
        /// <summary>
        /// Nome del campo profilato M/Text
        /// </summary>
        public String Label { get; set; }

        /// <summary>
        /// Valore assunto dal campo profilato M/Text
        /// </summary>
        public String Value { get; set; }
        
    }
}
