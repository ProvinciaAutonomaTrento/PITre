// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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