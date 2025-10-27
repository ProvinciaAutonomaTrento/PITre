// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.NonProtocollato
{
    [ResourceSwaggerSchema("CreaDocumentoResponse")]
    public class CreaNonProtocollatoCommandResponse : ValueObject
    {
        [ResourceSwaggerSchema("IdDocumentoGenerato")]
        [Required(AllowEmptyStrings = false)]
        public string Id { get; init; }
        [ResourceSwaggerSchema("listaServiziApi")]
        public IEnumerable<Link> Links { get; set; }
    }

    [ResourceSwaggerSchema("CreaDocumentoNonProtocollato")]
    public class CreaNonProtocollatoCommand : IRequest<CreaNonProtocollatoCommandResponse>
    {
        [Required]
        [ResourceSwaggerSchema("oggettoDocumento")]
        public OggettoDelDocumento OggettoDelDocumento { get; init; } = null!;

        [ResourceSwaggerSchema("codiceRegistro")]
        [Required(AllowEmptyStrings = false)]
        public string CodiceRegistro { get; set; } = null!;

        [ResourceSwaggerSchema("Documento_idDoc")]
        public IdDoc? IdDoc { get; init; }

        [ResourceSwaggerSchema("Riservato")]
        public bool? Riservato { get; init; }

        [ResourceSwaggerSchema("Classificazioni")]
        public IReadOnlyList<string>? Classificazioni { get; init; }

        [ResourceSwaggerSchema("Aggregazioni")]
        public IReadOnlyList<Aggregazione>? Aggregazioni { get; init; }

        [ResourceSwaggerSchema("Command_Profilo")]
        public Profilo? Profilo { get; init; }

        [ResourceSwaggerSchema("Command_Keywords")]
        public IReadOnlyList<string>? Keywords { get; init; }
    }
}
