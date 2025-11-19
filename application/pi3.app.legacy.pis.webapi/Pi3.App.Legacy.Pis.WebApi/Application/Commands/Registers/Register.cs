// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers
{
    public class Register
    {
        /// <summary>
        /// Id del registro
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del registro
        /// </summary>
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Indica del il registro è un rf
        /// </summary>
        public bool IsRF
        {
            get;
            set;
        }

        /// <summary>
        /// Indica lo stato del registro chiuso/aperto/giallo
        /// </summary>
        public string State
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la descrizione del registro/rf
        /// </summary>
        public string Description
        {
            get;
            set;
        }
    }
}
