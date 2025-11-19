// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.Email.Sender;
using System.ComponentModel.DataAnnotations;

namespace Pi3.Infrastructure.Graph.Services.Email.Sender
{
    public class GraphSendEmailConfiguration : SendEmailConfigurations
    {
        [Required(AllowEmptyStrings = false)]
        public string ClientId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string TenantId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ClientSecret { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string MailBox { get; set; }
    }
}