// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes
{
    public class ClassificationScheme
    {
        /// <summary>
        /// System id del titolario
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del titolario
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il titolario è attivo
        /// </summary>
        public bool Active
        {
            get;
            set;
        }
    }
}
