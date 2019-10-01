using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MText.DomainObjects
{
    /// <summary>
    /// Questo oggetto contiene informazioni su di un modello M/Text
    /// </summary>
    public class ModelInfo
    {
        /// <summary>
        /// Nome del databinding
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Path del databinding
        /// </summary>
        public String Path { get; set; }
    }
}
