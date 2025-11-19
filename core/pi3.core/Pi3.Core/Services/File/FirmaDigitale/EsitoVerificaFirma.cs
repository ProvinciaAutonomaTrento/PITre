// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.FirmaDigitale
{
    public class EsitoVerificaFirma : ValueObject
    {
        [Required]
        public DateTime DataVerificaFirma { get; init; }

        [Required]
        public string DatiGeneraliVerifica { get; set; } = null!;

        [Required]
        public IReadOnlyCollection<Firmatario> Firmatari { get; set; } = null!;

        public bool FileMarcato { get; set; }

        public Marca? MarcaDetached { get; set; } = null;

        public ParteFirmata? ParteFirmata { get; set; } = null;
    }
}
