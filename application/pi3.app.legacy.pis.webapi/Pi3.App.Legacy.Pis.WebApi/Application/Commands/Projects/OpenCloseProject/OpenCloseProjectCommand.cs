// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.OpenCloseProject
{
    public class OpenCloseProjectCommand: IRequest<OpenCloseProjectCommandResponse>
    {
        /// <summary>
        /// Id del fascicolo di provenienza
        /// </summary>
        public string IdProject
        {
            get;
            set;
        }

        /// <summary>
        /// Id del titolario
        /// </summary>
        public string ClassificationSchemeId
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del fascicolo
        /// </summary>
        public string CodeProject
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del fascicolo
        /// </summary>
        public string Action
        {
            get;
            set;
        }
    }

    public class OpenCloseProjectCommandResponse: GetProjectResponse
    {
	}
}