// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.InteropPitre.WebApi.Models
{
    public class SuperadminUserOptions
    {
        [Required(AllowEmptyStrings = false)]
        public string UserId { get; set; } = null!;
    }
}
