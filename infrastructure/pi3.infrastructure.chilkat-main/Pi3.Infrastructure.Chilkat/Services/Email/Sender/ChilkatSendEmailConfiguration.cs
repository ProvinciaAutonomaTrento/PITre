// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.Email.Sender;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Chilkat.Services.Email.Sender
{
    public class ChilkatSendEmailConfiguration : SendEmailConfigurations
    {
        [Required(AllowEmptyStrings = false)]
        public string Host { get; set; } = null!;

        [Required]
        public int Port { get; set; }

        [Required]
        public bool RequireSsl { get; set; }

        public string? UserName { get; set; } = null;
        public string? Password { get; set; } = null;
        public bool StartTLS { get; set; }
    }
}
