// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.RabbitMQ;
using System.Security.Claims;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.ConvertVersionToPdf
{
    public class ConvertToPdfCommand : MessageQueueBaseCommand
    {
        public ConvertToPdfCommand()
            : base()
        { }

        public ConvertToPdfCommand(ClaimsPrincipal claimsPrincipal)
            : base(claimsPrincipal)
        { }

        public string Id { get; init; } = null!;

        public string? IdVersion { get; init; } = null;
    }
}
