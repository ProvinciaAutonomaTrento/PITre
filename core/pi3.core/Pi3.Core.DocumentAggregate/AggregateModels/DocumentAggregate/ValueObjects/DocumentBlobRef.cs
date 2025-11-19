// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.SeedWork;

namespace Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects
{
    public class DocumentBlobRef : ValueObject
    {
        public DocumentBlobRef() : base()
        {
        }

        [Required]
        public string IdBlob { get; init; } = null;
        [Required]
        public DateTime? CreationDate { get; init; } = default;
        [Required]
        public string? FileName { get; init; } = null;
        [Required]
        public long? FileSize { get; init; } = default;
        public string? ContentType { get; init; } = null;
        [Required]
        public byte[]? Hash { get; init; } = null;
        [Required]
        public HashNamesEnum? HashName { get; init; } = null;
        public bool? Cartaceo { get; init; } = false;
        public bool? SegnaturaPermanente { get; init; } = false;
        public TipoFirmaEnum? TipoFirma { get; init; } = null;
    }
}
