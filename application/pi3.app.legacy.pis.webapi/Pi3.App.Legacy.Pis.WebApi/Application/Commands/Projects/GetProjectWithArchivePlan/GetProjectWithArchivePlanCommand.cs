// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectWithArchivePlan
{
    public class GetProjectWithArchivePlanCommand: IRequest<GetProjectWithArchivePlanCommandResponse>
    {
        public string idProject { get; set; }
        public string codeProject { get; set; }

        public string classificationSchemeId { get; set; }
    }

    public class GetProjectWithArchivePlanCommandResponse
    {
        public ProjectWithArchivePlan Project { get; set; }
        public string ErrorMessage { get; set; }
        public GetProjectWithArchivePlanResponseCode Code { get; set; }
    }
    public enum GetProjectWithArchivePlanResponseCode { OK, SYSTEM_ERROR }
}