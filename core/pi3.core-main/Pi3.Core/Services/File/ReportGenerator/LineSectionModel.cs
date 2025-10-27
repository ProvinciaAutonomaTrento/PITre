// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.ReportGenerator
{
    public class LineSectionModel : ISectionModel
    {
        public LineStyles Style { get; set; } = LineStyles.Solid;

        public System.Drawing.Color? Color { get; set; } = null;

        public int? Size { get; set; } = null;
    }
}
