// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pi3.Core.Services.Email.Sender
{
    public class SendEmailInstructions : ValueObject
    {
        [Required]
        public EmailSender Sender { get; init; } = null!;

        public List<EmailRecipient> To { get; init; } = null!;

        public List<EmailRecipient> Cc { get; init; } = null!;

        public List<EmailRecipient> Bcc { get; init; } = null!;

        public TextValue Subject { get; init; } = null!;

        public TextValue Body { get; init; } = null!;

        public bool BodyIsHtml { get; init; } 

        public bool? HighPriority { get; init; } = null!;

        public List<EmailContentAttachment> Attachments { get; init; } = null!;
        public List<EmailHeader> Headers { get; init; } = null!;
    }
}
