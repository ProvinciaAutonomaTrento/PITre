// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.RequestConvertToPdf
{
    [ResourceSwaggerSchema("RequestConvertToPdfCommandResponse")]
    public class RequestConvertToPdfCommandResponse : ValueObject {
        [ResourceSwaggerSchema("listaServiziApi")]
        public IEnumerable<Link> Links { get; set; }
    }

    public class RequestConvertToPdfCommand : IRequest<RequestConvertToPdfCommandResponse>
    {
        /// <summary>
        /// id del docuento da elaborare
        /// </summary>
        [Required]
        public string Id { get; init; } = null!;
        /// <summary>
        /// id della versione documento da convertire in pdf
        /// </summary>
        public string? IdVersion { get; init; }
    }
}
