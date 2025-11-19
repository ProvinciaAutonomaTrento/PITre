// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.ReportGenerator
{
    public class ReportGeneratorCapabilities
    {
        public ReportOutputTypes[] SupportedOutputTypes { get; set; } = null!;

        public PageSizes[] SupportedPageTypes { get; set; } = null!;

        public PageOrientations[] SupportedOrientations { get; set; } = null!;

        public Type[] SupportedSections { get; set; } = null!;

        public bool HeaderSupported { get; set; }

        public bool FooterSupported { get; set; }
    }
}
