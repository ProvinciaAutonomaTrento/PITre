// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles
{
    public class Role
    {
        /// <summary>
        /// System id del ruolo (id gruppo no id corr globali)
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del ruolo
        /// </summary>
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del ruolo
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Registri associati al ruolo
        /// </summary>
        public Register[] Registers
        {
            get;
            set;
        }
    }
}
