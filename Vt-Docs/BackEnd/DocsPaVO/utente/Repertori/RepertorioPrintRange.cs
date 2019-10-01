using System;

namespace DocsPaVO.utente.Repertori
{
    /// <summary>
    /// Intervallo di stampa relativo ad un repertorio.
    /// </summary>
    [Serializable()]
    public class RepertorioPrintRange
    {
        /// <summary>
        /// Anno cui si riferisce l'intervallo di stampa
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Estremo inferiore del range
        /// </summary>
        public int FirstNumber { get; set; }

        /// <summary>
        /// Estremo inferiore del range
        /// </summary>
        public int LastNumber { get; set; }

    }
}
