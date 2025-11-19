// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test.OrderAggregate.ValueObjects
{
    public class ShipAddress : ValueObject
    {
        [Required(AllowEmptyStrings = false)]
        public string Address { get; init; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string City { get; init; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string Country { get; init; } = null!;
    }
}
