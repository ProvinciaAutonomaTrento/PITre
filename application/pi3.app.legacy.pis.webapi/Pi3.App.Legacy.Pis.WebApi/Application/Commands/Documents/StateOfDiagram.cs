// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents
{
    public class StateOfDiagram
    {
        /// <summary>
        /// Id dello stato del diagramma
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Id del diagramma dello stato
        /// </summary>
        public string DiagramId
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione dello stato del diagramma
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Con valore true se è uno stato iniziale
        /// </summary>
        public bool InitialState
        {
            get;
            set;
        }

        /// <summary>
        /// Con valore true se è uno stato finale
        /// </summary>
        public bool FinaleState
        {
            get;
            set;
        }
    }
}
