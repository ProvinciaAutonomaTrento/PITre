// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Adobe.Services.ConverterService
{
    public class AdobePdfFileConverterServiceOptions
    {
        public string? ServiceUrl { get; set; } = null;

        public string? ServiceCredentialsUserName { get; set; } = null;

        public string? ServiceCredentialsPassword { get; set; } = null;

        public string? PdfSettings { get; set; } = "PDFA1b 2005 CMYK";

        public string? FileTypeSettings { get; set; } = "Standard PITRE";

        public string? SecuritySettings { get; set; } = "No Security";

        public int? RequestTimeoutInSeconds { get; set; } = 300;
    }
}
