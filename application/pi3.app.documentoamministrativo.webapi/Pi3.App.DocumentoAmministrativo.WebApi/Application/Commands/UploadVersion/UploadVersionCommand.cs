// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.UploadVersion
{
    [ResourceSwaggerSchema("UploadVersionRequestResponse")]
    public class UploadVersionRequestResponse : ValueObject {
        [ResourceSwaggerSchema("listaServiziApi")]
        public IEnumerable<Link>? Links { get; set; } = null;
    }


    [ResourceSwaggerSchema("UploadVersion_Object")]
    public class UploadVersion : ValueObject
    {
        [ResourceSwaggerSchema("UploadVersion_UploadId")]
        [Required]
        public Guid UploadId { get; init; }

        [ResourceSwaggerSchema("UploadVersion_Descrizione")]
        public string? Descrizione { get; init; }
    }

    public class UploadVersionRequest : ValueObject, IRequest<UploadVersionRequestResponse>
    {
        [Required]
        public string Id { get; init; } = null!;

        public string? IdVersion { get; init; }

        [Required]
        public UploadVersion UploadVersion { get; set; } = null!;
    }
}
