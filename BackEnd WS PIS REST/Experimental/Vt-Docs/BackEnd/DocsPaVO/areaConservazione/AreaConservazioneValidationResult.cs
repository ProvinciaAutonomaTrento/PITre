using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.areaConservazione
{
    /// <summary>
    /// Riporta l'esito della validazione di un'istanza di conservazione
    /// </summary>
    [Serializable()]
    public class AreaConservazioneValidationResult
    {
        /// <summary>
        /// 
        /// </summary>
        public AreaConservazioneValidationResult()
        {
            Items = new InvalidItemConservazione[0];
        }

        /// <summary>
        /// Se true, l'istanza di conservazione è valida
        /// </summary>
        public bool IsValid
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Elementi di conservazione non validi
        /// </summary>
        public InvalidItemConservazione[] Items { get; set; }
    }
}
