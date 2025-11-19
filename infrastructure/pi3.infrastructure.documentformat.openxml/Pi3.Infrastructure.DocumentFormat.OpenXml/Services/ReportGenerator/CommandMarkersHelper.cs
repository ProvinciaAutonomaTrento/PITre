// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.DocumentFormat.OpenXml.Services.ReportGenerator
{
    public static class CommandMarkersHelper
    {
        public const string CurrentPageNumberMarker = "PAGE";
        public const string NumPagesMarker = "NUMPAGES";
        public const string DateMarker = "DATE \\@ \"MMMM d, yyyy\"";
    }
}
