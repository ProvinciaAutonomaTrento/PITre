// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test.OrderAggregate.Entities
{

    public class Product : Entity<int>
    {
        #region Public Members

        internal Product(int id, TextValue productName, double unitPrice)
        {
            productName = productName ?? throw new ArgumentNullException(nameof(productName));
            
            Validator.ValidateObject(productName, new ValidationContext(productName), true);

            this.Id = id;
            this.ProductName = productName;
            this.UnitPrice = unitPrice;
        }

        public TextValue ProductName { get; protected set; }

        public double UnitPrice { get; protected set; }

        #endregion

        #region Private Members

        #endregion
    }

}
