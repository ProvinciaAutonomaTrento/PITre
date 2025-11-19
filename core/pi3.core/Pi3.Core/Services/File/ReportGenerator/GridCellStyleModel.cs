// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.ReportGenerator
{
    public class GridCellStyleModel : ValueObject
    {
        public System.Drawing.Color? ForegroundColor { get; set; } = null;

        public Justifications? Justification { get; set; } = null;

        public VerticalAlignments? VerticalAlignment { get; set; } = null;

        [Range(0, 100)]
        public int? WithPercentage { get; set; } = null;
    }
}
