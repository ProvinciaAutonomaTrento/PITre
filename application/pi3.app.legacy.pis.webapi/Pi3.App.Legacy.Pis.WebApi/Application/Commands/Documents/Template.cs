// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents
{
    public class Template
    {
        /// <summary>
        /// System id del template
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del template
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Campi del template
        /// </summary>
        public Field[] Fields
        {
            get;
            set;
        }

        /// <summary>
        /// Diagramma di stato associato
        /// </summary>
        public StateDiagram StateDiagram
        {
            get;
            set;
        }

        /// <summary>
        /// D per documenti F per fascicoli
        /// </summary>
        public string Type
        {
            get;
            set;
        }
    }
}
