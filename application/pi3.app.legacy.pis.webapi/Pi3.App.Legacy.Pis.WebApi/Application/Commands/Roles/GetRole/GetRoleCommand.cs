// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetRole
{
    public class GetRoleCommand: IRequest<GetRoleCommandResponse>
    {
        public string codeRole {  get; set; }

        public string idRole { get; set; }
    }

    public class GetRoleCommandResponse 
    {
        public Role Role { get; set; }
        public string ErrorMessage { get; set; }
        public GetRoleResponseCode Code { get; set; }
    }
    public enum GetRoleResponseCode { OK, SYSTEM_ERROR }
}
