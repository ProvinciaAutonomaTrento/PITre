// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Core.Test.OrderAggregate.Exceptions;
using Pi3.Core.Test.OrderAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test.OrderAggregate.Entities
{
    public class OrderDetail : Entity<Guid>
    {
        #region Public Members

        internal OrderDetail(Guid id, int idProduct, TextValue productName, double unitPrice, int quantity = 1, int? discount = null)
        {
            if (discount < 0 || discount > 30)
                throw new InvalidDiscountPi3Exception();

            this.Id = id;
            this.Product = new Product(idProduct, productName, unitPrice);
            this.Quantity = quantity;
            this.Discount = discount;
        }

        public Product Product { get; protected set; } = null!;

        internal void ChangeQuantity(int quantity)
        {
            this.Quantity = quantity;
        }

        public int Quantity { get; protected set; }

        internal void ChangeDiscount(int? discount = null)
        {
            if (discount < 0 || discount > 30)
                throw new InvalidDiscountPi3Exception();

            this.Discount = discount;
        }

        public int? Discount { get; protected set; } = null;

        public double Amount
        {
            get
            {
                var amount = (this.Product.UnitPrice * this.Quantity);
                if (this.Discount > 0)
                    amount -= (amount * this.Discount.Value) / 100;
                return amount;
            }
        }

        #endregion

        #region Private Members

        #endregion
    }

}
