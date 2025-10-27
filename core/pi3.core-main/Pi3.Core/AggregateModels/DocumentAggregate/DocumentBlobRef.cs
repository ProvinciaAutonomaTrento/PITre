// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.SeedWork;

namespace Pi3.Core.AggregateModels.DocumentAggregate
{
    public class DocumentBlobRef : ValueObject
    {
        public DocumentBlobRef() : base()
        {
        }

        [Required]
        public string IdBlob { get; init; } = null;
        [Required]
        public DateTime? CreationDate { get; init; } = default(DateTime?);
        [Required]
        public string? FileName { get; init; } = null;
        [Required]
        public long? FileSize { get; init; } = default(long?);
        public string? ContentType { get; init; } = null;
        [Required]
        public string Hash { get; init; } = null;
        [Required]
        public string HashAlghoritm { get; init; } = null;
    }

    public class TargetVersionBehavior : ValueObject
    {
        public TargetVersionBehavior() : base()
        {
        }


        [Required]
        public bool CreateNewVersion { get; init; } = true;

        public string? IdVersion { get; init; }

        public TextValue? Name { get; init; } = null;
    }
}
