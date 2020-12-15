using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Grid
{
    public enum SpecialFieldsEnum
    {
        Icons,      // Icone di ricerca
        CheckBox    // Checkbox per la selezione dell'item
    }

    /// <summary>
    /// Questa classe, che estende la classe Field, rappresenta un campo speciale
    /// Un esempio di campo speciale è rappresentato dalle icone della pulsantiera
    /// </summary>
    [Serializable()]
    public class SpecialField : Field
    {
        /// <summary>
        /// Tipo di campo
        /// </summary>
        public SpecialFieldsEnum FieldType { get; set; }

    }
}
