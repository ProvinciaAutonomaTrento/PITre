// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Authenticate.Authenticate
{

    public class AuthenticateCommand : IRequest<AuthenticateCommandResponse>
    {
       
        [Required]
        public string Username { get; set; }

        public string? CodeRole { get; set; }

        [Required]
        public string CodeApplication { get; set; }

        [Required]
        public string CodeAdm { get; set; }
    }

    public class AuthenticateCommandResponse
    {
        public AuthenticateResponseCode Code { get; set; }
        public string Token { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum AuthenticateResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
