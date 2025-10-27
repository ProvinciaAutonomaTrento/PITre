// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProject
{
    public class GetProjectCommand: IRequest<GetProjectCommandResponse>
    {        
        public string idProject {  get; set; }
        public string codeProject { get; set; }

        public string classificationSchemeId { get; set; }
    }

    public class GetProjectCommandResponse: GetProjectResponse
    {
	}
}