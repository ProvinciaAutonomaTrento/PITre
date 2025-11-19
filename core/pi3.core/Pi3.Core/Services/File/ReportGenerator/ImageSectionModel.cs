// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.ReportGenerator
{
    public class ImageSectionModel : ISectionModel
    {
        [Required]
        public byte[] ImageContent { get; set; } = null!;


        [Required]
        public string ImageName { get; set; } = null!;

        public string? ImageContentType { get; set; } = null;
    }
}
