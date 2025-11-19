// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.Uploader
{
    public class UploadMetadata : ValueObject
    {
        public Guid Id { get; init; }
        public DateTime InitializationDate { get; init; }
        public DateTime FinalizationDate { get; init; }
        public string FileName { get; init; } = null!;
        public int PartsNumber { get; init; }
        public string Checksum { get; init; } = null!;
    }
}
