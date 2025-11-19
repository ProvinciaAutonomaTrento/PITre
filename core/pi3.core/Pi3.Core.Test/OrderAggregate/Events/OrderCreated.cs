// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Core.Test.OrderAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test.OrderAggregate.Events
{
    public class OrderCreated : Event
    {
    }

    public class CustomerAdded : Event
    {
        public string IdCustomer { get; init; } = null!;
        public string CustomerName { get; init; } = null!;
        public ShipAddress ShipAddress { get; init; } = null!;
    }

    public class CustomerShipAddressChanged : Event
    {
        public ShipAddress NewShipAddress { get; init; } = null!;
    }

    public class OrderDetailAdded : Event
    {
        public Guid IdOrderDetail { get; init; }

        public int IdProduct { get; init; }

        public TextValue ProductName { get; init; } = null!;

        public double UnitPrice { get; init; }

        public int Quantity { get; init; }

        public int? Discount { get; init; } = null;
    }

    public class OrderDetailQuantityChanged : Event
    {
        public Guid IdOrderDetail { get; init; }

        public int NewQuantity { get; init; }
    }

    public class OrderDetailDiscountChanged : Event
    {
        public Guid IdOrderDetail { get; init; }

        public int NewDiscount { get; init; }
    }
}
