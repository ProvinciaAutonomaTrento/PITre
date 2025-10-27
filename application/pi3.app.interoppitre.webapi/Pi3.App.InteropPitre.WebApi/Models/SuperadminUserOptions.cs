// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.InteropPitre.WebApi.Models
{
    public class SuperadminUserOptions
    {
        [Required(AllowEmptyStrings = false)]
        public string UserId { get; set; } = null!;
    }
}
