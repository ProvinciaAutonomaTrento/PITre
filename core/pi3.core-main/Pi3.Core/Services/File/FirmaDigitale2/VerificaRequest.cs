// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.FirmaDigitale2
{
    public class VerificaRequest
    {
        public bool? VerificaCompleta { get; init; } = null;

        [Required]
        public byte[] FileFirmato { get; init; } = null!;

        public byte[] FileOriginale { get; init; } = null!;

        public DateTime? DataVerifica { get; init; } = null;

        public TipiVerifica? TipoVerifica { get; init; } = null;

        public bool? ReturnFileOriginale { get; set; } = null;

        public bool? ReturnXmlCompleto { get; set; } = null;
    }

    public enum TipiVerifica
    {
        Esterna = 0,
        Appiattita = 1,
        Incapsulata = 2
    }
}
