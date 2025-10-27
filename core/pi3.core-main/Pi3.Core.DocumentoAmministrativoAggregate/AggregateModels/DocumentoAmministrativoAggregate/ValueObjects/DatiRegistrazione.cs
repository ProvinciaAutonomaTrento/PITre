// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects
{
    public enum TipologiaFlussoEnum
    {
        E = 0,
        U = 1,
        I = 2
    }

    public abstract class DatiRegistrazione : ValueObject
    {
        [Required]
        public TipologiaFlussoEnum TipologiaFlusso { get; init; }

        public abstract bool IsRegistrato { get; }

        [Required]
        public string IdRegistro { get; init; }

        [Required]
        [RegularExpression("[A-Za-z0-9_\\.\\-]{1,16}")]
        public string CodiceRegistro { get; init; }
    }

    public class DatiRegistrazioneProtocollo : DatiRegistrazione
    {
        public DateTime? DataProtocollazione { get; init; }

        public long? NumeroProtocollo { get; init; }

        public override bool IsRegistrato
        {
            get
            {
                return NumeroProtocollo.HasValue && DataProtocollazione.HasValue;
            }
        }

        public string? NumeroProtocolloAsString
        {
            get
            {
                return NumeroProtocollo?.ToString().PadLeft(7, '0');
            }
        }
    }

    public class DatiRegistrazioneRepertorio : DatiRegistrazione
    {
        public DateTime? DataRegistrazione { get; init; }

        public long? NumeroRegistrazione { get; init; }

        public override bool IsRegistrato
        {
            get
            {
                return NumeroRegistrazione.HasValue && DataRegistrazione.HasValue;
            }
        }
    }
}
