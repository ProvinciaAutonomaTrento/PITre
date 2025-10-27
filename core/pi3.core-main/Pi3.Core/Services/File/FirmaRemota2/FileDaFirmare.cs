// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.FirmaRemota2
{
    public class FileDaFirmare
    {
        [Required]
        public byte[] FileBase64 { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string FileName { get; set; } = null!;
    }
}
