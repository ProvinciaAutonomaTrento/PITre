// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.Entrata
{
    [ResourceSwaggerSchema("CreaDocumentoResponse")]
    public class CreaEntrataCommandResponse : ValueObject
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

    [ResourceSwaggerSchema("CreaDocumentoEntrata")]
    public class CreaEntrataCommand : IRequest<CreaEntrataCommandResponse>
    {
        [Required]
        [ResourceSwaggerSchema("codiceRegistro")]
        public string CodiceRegistro { get; init; } = null!;

        [Required]
        [ResourceSwaggerSchema("oggettoDocumento")]
        public OggettoDelDocumento OggettoDelDocumento { get; init; } = null!;

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

        [ResourceSwaggerSchema("MittentiMultipli")]
        public IReadOnlyList<string>? MittentiMultipli { get; init; }

        [ResourceSwaggerSchema("MezzoSpedizione")]
        public string? MezzoSpedizione { get; init; }

        [ResourceSwaggerSchema("Command_ProtocolloMittente")]
        public ProtocolloMittente? ProtocolloMittente { get; init; }

        [ResourceSwaggerSchema("Command_ProtocolloEmergenza")]
        public ProtocolloEmergenza? ProtocolloEmergenza { get; init; }

        [ResourceSwaggerSchema("Command_Profilo")]
        public Profilo? Profilo { get; init; }

        [ResourceSwaggerSchema("Command_Keywords")]
        public IReadOnlyList<string>? Keywords { get; init; }

        [ResourceSwaggerSchema("Command_Predisponi")]
        public bool? Predisponi { get; init; }
    }
}
