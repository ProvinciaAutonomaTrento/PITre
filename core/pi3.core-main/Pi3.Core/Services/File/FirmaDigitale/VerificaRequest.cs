// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.FirmaDigitale
{
    public class VerificaRequest : ValueObject
    {
        [Required]
        public byte[] FileFirmato { get; init; } = null!;

        public bool? FirmaSha1WithRSA { get; init; } = null;

        public bool? MarcaSha1WithRSA { get; init; } = null;

        public DateTime? DataVerifica { get; init; } = null;

        public bool? ControlloFirmeAnnidate { get; init; } = null;
    }
}
