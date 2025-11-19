// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.IText.ReportGenerator.Services;

public static class CommandMarkersHelper
{
    public static string GetCurrentPageNumberMarker() => "PAGE";
    public static string GetNumPagesMarker() => "NUMPAGES";
    public static string GetDateMarker() => "DATE \\@ \"MMMM d, yyyy\"";
}
