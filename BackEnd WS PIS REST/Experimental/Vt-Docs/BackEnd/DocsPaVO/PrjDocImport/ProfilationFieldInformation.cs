using System;

namespace DocsPaVO.PrjDocImport
{
    /// <summary>
    /// Classe per la rappresentazione delle informazioni di base su di un campo
    /// profilato.
    /// Le informazioni di base sono costituite da una etichetta e da una serie di
    /// valoroi assegnati alla proprietà
    /// </summary>
    [Serializable()]
    public class ProfilationFieldInformation
    {
        /// <summary>
        /// L'etichetta associata al campo
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// L'insieme dei valori associati al campo
        /// </summary>
        public string[] Values { get; set; }

    }
}
