// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents
{
    public class StateDiagram
    {
        /// <summary>
        /// System id del diagramma
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del diagramma
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Stati del diagramma
        /// </summary>
        public StateOfDiagram[] StateOfDiagram
        {
            get;
            set;
        }
    }
}
