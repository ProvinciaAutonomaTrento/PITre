// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.ComponentModel.DataAnnotations;

namespace Pi3.Infrastructure.Chilkat.Services.Email.BoxScanner
{
    public class EmailBoxFolders
    {
        [Required]
        public string CurrentFolder { get; set; } = null!;

        public string? ProcessedFolder { get; set; }

        public string? FailedFolder { get; set; }
    }
}