// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.Common
{
    public class CreaCommonCommandResponse : ValueObject
    {
        [Required(AllowEmptyStrings = false)]
        public string Id { get; init; }

        // inizio entrata, interno, uscita
        [Required(AllowEmptyStrings = false)]
        public string CodiceRegistro { get; set; }

        public DateTime? Data { get; init; }

        public long? Numero { get; init; }

        public string? Segnatura { get; init; }
        // fine entrata, interno
    }

    // entrata
    public class CreaCommonCommand: IRequest<CreaCommonCommandResponse>
    {
        public TipologiaFlussoEnum?  TipoDocumento { get; set; }
        // inizio no non protocollato
        //[Required]
        //public Registro? Registro { get; init; }
        public string CodiceRegistro { get; init; }
        // fine no non protocollato

        [Required]
        public OggettoDelDocumento OggettoDelDocumento { get; init; }

        public IdDoc? IdDoc { get; init; }

        public bool? Riservato { get; init; }

        public IReadOnlyList<string>? Classificazioni { get; init; }

        public IReadOnlyList<Aggregazione>? Aggregazioni { get; init; }

        // inizio no non protocollato
        [Required]
        public string Mittente { get; init; }

        // inizio entrata
        public IReadOnlyList<string>? MittentiMultipli { get; init; }

        public string? MezzoSpedizione { get; init; }

        public ProtocolloMittente? ProtocolloMittente { get; init; }

        public ProtocolloEmergenza? ProtocolloEmergenza { get; init; }
        // fine entrata entrata

        // inizio interno, uscita
        [Required]
        [MinLength(1)]
        public IReadOnlyList<string> Destinatari { get; init; }

        public IReadOnlyList<string>? DestinatariCc { get; init; }
        // fine interno, uscita
        // fine no non protocollato

        //public IReadOnlyList<Profile>? Profiles { get; init; }
        public Profilo? Profilo { get; init; }

        public IReadOnlyList<string>? Keywords { get; init; }

        public bool? Predisponi { get; init; }

    }
}
