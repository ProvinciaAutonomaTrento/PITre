// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.ReportGenerator
{
    public class TextStyleModel 
    {
        public System.Drawing.Color? HighlightColor { get; set; } = null;

        public string? FontName { get; set; } = null;

        public System.Drawing.Color? FontColor { get; set; } = null;

        public bool? FontIsBold { get; set; } = null;

        public bool? FontIsItalic { get; set; } = null;

        public bool? FontIsStrikeout { get; set; } = null;

        public int? FontSize { get; set; } = null;
    }
}
