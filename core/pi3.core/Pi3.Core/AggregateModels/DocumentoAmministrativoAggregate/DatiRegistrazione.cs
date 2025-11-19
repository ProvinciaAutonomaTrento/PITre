// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate
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
                return this.NumeroProtocollo.HasValue && this.DataProtocollazione.HasValue;
            }
        }

        public string? NumeroProtocolloAsString
        {
            get
            {
                return this.NumeroProtocollo?.ToString().PadLeft(7, '0');
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
                return this.NumeroRegistrazione.HasValue && this.DataRegistrazione.HasValue;
            }
        }
    }
}
