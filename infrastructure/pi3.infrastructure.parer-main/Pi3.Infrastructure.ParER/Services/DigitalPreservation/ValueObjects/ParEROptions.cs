// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.ParER.Services.DigitalPreservation.ValueObjects
{
    public class ParEROptions
    {
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Version { get; set; }

        public string VersionGet { get; set; }

        public long MaxRetries { get; set; }

        public bool MultiAOO { get; set; } = false;

        [Required(AllowEmptyStrings = false)]
        public string Ambiente { get; set; }

    }
}
