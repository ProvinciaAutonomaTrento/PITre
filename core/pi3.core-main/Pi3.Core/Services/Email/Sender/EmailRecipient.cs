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
    public class EmailRecipient : ValueObject
    {
        [Required(AllowEmptyStrings = false)]
        public string Address { get; init; } = null!;

        public string DisplayName { get; init; } = null!;
    }
}
