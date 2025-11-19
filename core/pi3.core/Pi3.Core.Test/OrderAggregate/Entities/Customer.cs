// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Core.Test.OrderAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test.OrderAggregate.Entities
{
    public class Customer : Entity<string>
    {
        #region Public Members

        internal Customer(string id, string name, ShipAddress shipAddress)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));
            name = name ?? throw new ArgumentNullException(nameof(name));
            shipAddress = shipAddress ?? throw new ArgumentNullException(nameof(shipAddress));

            Validator.ValidateObject(shipAddress, new ValidationContext(shipAddress), true);

            this.Id = id;
            this.Name = name;
            this.ShipAddress = shipAddress;
        }

        public string Name { get; protected set; }

        internal void ChangeShipAddress(ShipAddress newShipAddress)
        {
            newShipAddress = newShipAddress ?? throw new ArgumentNullException(nameof(newShipAddress));
            Validator.ValidateObject(newShipAddress, new ValidationContext(newShipAddress), true);

            this.ShipAddress = newShipAddress;
        }

        public ShipAddress ShipAddress { get; protected set; }

        #endregion

        #region Private Members

        #endregion
    }
}
