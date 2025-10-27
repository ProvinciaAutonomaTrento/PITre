// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.CreateProject
{
    public class CreateProjectCommand: IRequest<CreateProjectCommandResponse>
    {
        public Project Project { get; set; }
    }

    public class CreateProjectCommandResponse:GetProjectResponse
    {
	}
}