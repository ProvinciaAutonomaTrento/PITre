// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetArchivePlan
{
    public class GetArchivePlanCommand: IRequest<GetArchivePlanCommandResponse>
    {        
        public string idArchivePlan {  get; set; }
    }

    public class GetArchivePlanCommandResponse
    {
        public ArchivePlan ArchivePlan { get; set; }
        public string ErrorMessage { get; set; }
        public GetArchivePlanResponseCode Code { get; set; }
    }
    public enum GetArchivePlanResponseCode { OK, SYSTEM_ERROR }
}