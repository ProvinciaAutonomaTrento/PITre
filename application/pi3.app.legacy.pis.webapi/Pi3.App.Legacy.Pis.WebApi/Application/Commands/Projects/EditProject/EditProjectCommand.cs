// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.EditProject
{
    public class EditProjectCommand: IRequest<EditProjectCommandResponse>
    {        
        public Project Project { get; set; }
    }

    public class EditProjectCommandResponse: GetProjectResponse
    {
	}
}