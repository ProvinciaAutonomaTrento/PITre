// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.FirmaRemota2
{
    public class FirmaPAdESRequest : ValueObject
    {
        [Required]
        [MinLength(1)]
        public List<FileDaFirmare> FilesDaFirmare { get; init; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string AliasCertificato { get; init; } = null!;

        public string DominioCertificato { get; init; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string PinCertificato { get; init; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string OtpFirma { get; init; } = null!;

        public bool? MarcaTemporale { get; init; } = default!;

        public bool? unzipOutput { get; init; } = default!;
    }
}
