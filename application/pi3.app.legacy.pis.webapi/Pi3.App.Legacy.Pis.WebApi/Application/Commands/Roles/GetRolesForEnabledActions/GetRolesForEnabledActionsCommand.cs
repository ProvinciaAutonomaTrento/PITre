// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetRolesForEnabledActions
{
    public class GetRolesForEnabledActionsCommand : IRequest<GetRolesForEnabledActionsCommandResponse>
    {
        [Required]
        public string userId { get; set; }

        [Required]
        public string codeFunction { get; set; }
    }

    public class GetRolesForEnabledActionsCommandResponse
    {
        public Role[] Roles { get; set; }
        public string ErrorMessage { get; set; }
        public GetRolesResponseCode Code { get; set; }
    }
    public enum GetRolesResponseCode { OK, SYSTEM_ERROR }
}
