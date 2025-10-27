// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.Email.BoxScanner;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Graph.Services.Email.BoxScanner
{
    public class GraphEmailBoxConfiguration : EmailBoxConfigurations
    {
        [Required(AllowEmptyStrings = false)]
        public string ClientId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string TenantId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ClientSecret { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string MailBox { get; set; }

        public string? FolderToRead { get; set; }

    }
}
