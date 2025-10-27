// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.MarcaTemporale
{
    public class MarcaTemporaleServiceOptions 
    {
        [Required(AllowEmptyStrings = false)]
        public string ServiceUrl { get; set; } = null!;

        [Required]
        public X509Certificate2 Certificate { get; set; } = null!;
    }
}
