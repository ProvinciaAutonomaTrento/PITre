// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.Spreadsheet
{
    public class CellStyleModel : ValueObject
    {
        public Color? ForegroundColor { get; set; } = null;

        public string? FontName { get; set; } = null;

        public Color? FontColor { get; set; } = null;

        public bool? FontIsBold { get; set; } = null;

        public bool? FontIsItalic { get; set; } = null;

        public bool? FontIsStrikeout { get; set; } = null;

        public int? FontSize { get; set; } = null;

        public CellTextAlignments? VerticalAlignment { get; set; } = null;

        public CellTextAlignments? HorizontalAlignment { get; set; } = null;
        public bool? HasBorder { get; set; }
        public int? Width { get; set; }
        public Color? BorderColor { get; set; }
        public bool WrapText{ get; set; }

    }
}
