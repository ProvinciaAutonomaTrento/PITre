// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.CreateProjectWithArchivePlan
{
    public class CreateProjectWithArchivePlanCommand: IRequest<CreateProjectWithArchivePlanCommandResponse>
    {
        public ProjectWithArchivePlan Project { get; set; }
    }

    public class CreateProjectWithArchivePlanCommandResponse
    {
        public ProjectWithArchivePlan Project { get; set; }
        public string ErrorMessage { get; set; }
        public GetProjectWithArchivePlanResponseCode Code { get; set; }
    }
    public enum GetProjectWithArchivePlanResponseCode { OK, SYSTEM_ERROR }
}