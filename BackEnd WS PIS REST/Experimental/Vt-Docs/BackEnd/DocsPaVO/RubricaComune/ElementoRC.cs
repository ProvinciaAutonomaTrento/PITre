using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.RubricaComune
{
    /// <summary>
    /// Attributi comuni a tutti gli elementi che vengono pubblicati in Rubrica Comune
    /// </summary>
    [Serializable()]
    public abstract class ElementoRC
    {
        /// <summary>
        /// Rappresenta il codice univoco del corrispondente docspa in rubrica comune
        /// </summary>
        public string CodiceRubrica;

        /// <summary>
        /// Rappresenta la descrizione del corrispondente docspa in rubrica comune
        /// </summary>
        public string DescrizioneRubrica;

        /// <summary>
        /// EMail del registro della UO scelto per l'interoperabilità
        /// </summary>
        public string EMailRegistro;

        /// <summary>
        /// Codice dell'amministrazione scelta per interoperabilità
        /// </summary>
        public string Amministrazione;

        /// <summary>
        /// Codice dell'AOO scelta per interopabilità
        /// </summary>
        public string AOO;
    }
}
