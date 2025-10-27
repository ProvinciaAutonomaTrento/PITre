// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetUsersInRole
{
    public class GetUsersInRoleCommand:IRequest<GetUsersInRoleCommandResponse>
    {
        [Required]
        public string codeRole { get; set; }
    }

    public class GetUsersInRoleCommandResponse
    {
        public User[] Users { get; set; }
        public string ErrorMessage { get; set; }
        public GetUsersResponseCode Code { get; set; }
    }
    public enum GetUsersResponseCode { OK, SYSTEM_ERROR }
}
