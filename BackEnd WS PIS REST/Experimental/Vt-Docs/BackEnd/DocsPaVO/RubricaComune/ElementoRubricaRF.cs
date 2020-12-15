using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.utente;

namespace DocsPaVO.RubricaComune
{
    /// <summary>
    /// Informazioni sull'RF da inviare a Rubrica Comune
    /// </summary>
    [Serializable]
    public class ElementoRubricaRF : ElementoRC
    {
        /// <summary>
        /// Raggruppamento Funzionale da inviare a Rubrica Comune
        /// </summary>
        public RaggruppamentoFunzionale RF { get; set; }
    }
}
