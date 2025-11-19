// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.Entrata
{
    [ResourceSwaggerSchema("CreaDocumentoResponse")]
    public class CreaInternoCommandResponse : ValueObject
    {
        [ResourceSwaggerSchema("IdDocumentoGenerato")]
        [Required(AllowEmptyStrings = false)]
        public string Id { get; init; }

        [ResourceSwaggerSchema("codiceRegistro")]
        [Required(AllowEmptyStrings = false)]
        public string CodiceRegistro { get; set; }

        [ResourceSwaggerSchema("DataProtocollazioneRegistrazione")]
        public DateTime? Data { get; init; }

        [ResourceSwaggerSchema("NumeroProtocolloInserimento")]
        public long? Numero { get; init; }

        [ResourceSwaggerSchema("Segnatura")]
        public string? Segnatura { get; init; }

        [ResourceSwaggerSchema("listaServiziApi")]
        public IEnumerable<Link> Links { get; set; }
    }

    [ResourceSwaggerSchema("CreaDocumentoInterno")]
    public class CreaInternoCommand : IRequest<CreaInternoCommandResponse>
    {
        [Required]
        //public Registro? Registro { get; init; }
        [ResourceSwaggerSchema("codiceRegistro")]
        public string CodiceRegistro { get; init; }

        [Required]
        [ResourceSwaggerSchema("oggettoDocumento")]
        public OggettoDelDocumento OggettoDelDocumento { get; init; }

        [ResourceSwaggerSchema("Documento_idDoc")]
        public IdDoc? IdDoc { get; init; }

        [ResourceSwaggerSchema("Riservato")]
        public bool? Riservato { get; init; }

        [ResourceSwaggerSchema("Classificazioni")]
        public IReadOnlyList<string>? Classificazioni { get; init; }

        [ResourceSwaggerSchema("Aggregazioni")]
        public IReadOnlyList<Aggregazione>? Aggregazioni { get; init; }

        [Required]
        [ResourceSwaggerSchema("CodiceMittente")]
        public string? Mittente { get; init; }

        [Required]
        [MinLength(1)]
        [ResourceSwaggerSchema("Command_Destinatari")]
        public IReadOnlyList<string> Destinatari { get; init; }

        [ResourceSwaggerSchema("Command_DestinatariCC")]
        public IReadOnlyList<string>? DestinatariCc { get; init; }

        [ResourceSwaggerSchema("Command_Profilo")]
        public Profilo? Profilo { get; init; }

        [ResourceSwaggerSchema("Command_Keywords")]
        public IReadOnlyList<string>? Keywords { get; init; }

        [ResourceSwaggerSchema("Command_Predisponi")]
        public bool? Predisponi { get; init; }
    }
}
