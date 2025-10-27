// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Email.BoxScanner;
using System.ComponentModel.DataAnnotations;

namespace Pi3.Infrastructure.Chilkat.Services.Email.BoxScanner
{
    public class ChilkatEmailBoxConfigurations : EmailBoxConfigurations
    {
        [Required(AllowEmptyStrings = false)]
        public string Host { get; set; } = null!;

        [Required]
        public int Port { get; set; }

        [Required]
        public bool RequireSsl { get; set; }

        public bool StartTls { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public EmailBoxTypeEnum EmailBoxTypeEnum { get; set; }

        public EmailBoxIMAPBehavior? IMAPBehavior { get; set; } = null;

        public EmailBoxPOPBehavior? POPBehavior { get; set; } = null;

        public Func<string, bool> IsEmailProcessedByMessageId { get; set; } = null!;
    }

    public class EmailBoxIMAPBehavior : ValueObject
    {
        [Required]
        public EmailBoxFolders? Folders { get; init; } = null!;
    }

    public class EmailBoxPOPBehavior : ValueObject
    {
        public bool? DeleteEmailIfProcessed { get; init; } = false;
    }
}