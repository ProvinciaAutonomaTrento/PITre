// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents
{
    public class FieldLite
    {
        /// <summary>
        /// Descrizione del campo
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Valore del campo
        /// </summary>
        public string Value
        {
            get;
            set;
        }
    }
}
