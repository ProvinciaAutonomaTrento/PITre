// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.ComponentModel.DataAnnotations;


namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentMetadata.DocumentMetadataManagement
{
    public class DocumentMetadataManagementCommand : Infrastructure.Services.RabbitMQ.MessageQueueBaseCommand
    {
        public DocumentMetadataManagementCommand() : base() { }
        public DocumentMetadataManagementCommand(System.Security.Claims.ClaimsPrincipal claimsPrincipal) : base(claimsPrincipal) { }

        [Required]
        public string Method { get; init; }

        [Required]
        public long IdOggetto { get; init; }

        [Required]
        public string DescrizioneOggetto { get; init; }

        [Required]
        public DateTime DataAzione { get; init; }

        public long? IdTrasmissione { get; init; } = null!;
    }


}
