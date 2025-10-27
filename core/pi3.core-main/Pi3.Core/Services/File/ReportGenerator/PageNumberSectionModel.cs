// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.ReportGenerator
{
    public class PageNumberSectionModel : ISectionModel
    {
        public string? Format { get; set; } = null;

        public TextStyleModel? TextStyle { get; set; } = null;

        public TextSectionStyleModel? Style { get; set; } = null;
    }
}