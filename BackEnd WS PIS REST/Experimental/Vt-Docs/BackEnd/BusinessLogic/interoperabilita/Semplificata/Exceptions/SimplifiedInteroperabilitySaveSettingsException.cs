using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.interoperabilita.Semplificata.Exceptions
{
    /// <summary>
    /// Eccezione scatenata quando si verifica un problema durante il salvataggio delle impostazioni di interoperabilità
    /// relative ad un registro
    /// </summary>
    [Serializable()]
    public class SimplifiedInteroperabilitySaveSettingsException : Exception
    {
        private List<String> _invalidFields = new List<String>();

        public void AddInvalidField(String message)
        {
            _invalidFields.Add(message);

        }

        public List<String> GetInvalidFields()
        {
            return this._invalidFields;
        }

        public override string Message
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                this._invalidFields.ForEach(m => builder.AppendFormat(", {0}", m));

                return builder.ToString().Substring(2);
            }
        }
    }
}
