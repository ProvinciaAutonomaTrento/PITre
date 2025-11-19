// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Core.Test.OrderAggregate.Entities;
using Pi3.Core.Test.OrderAggregate.Events;
using Pi3.Core.Test.OrderAggregate.Exceptions;
using Pi3.Core.Test.OrderAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RangeAttribute = System.ComponentModel.DataAnnotations.RangeAttribute;

namespace Pi3.Core.Test.ExampleAggregate
{
    public class Order : AggregateRoot<string>
    {
        #region Public Members

        public Order(string id)
        {
            this.ApplyChange(new OrderCreated() { Id = id });
        }

        public override IEnumerable<Pi3Exception> GetErrors()
        {
            var errors = new List<Pi3Exception>();

            if (this.Customer == null)
                errors.Add(new MissingCustomerPi3Exception());

            if (!this._orderDetails.Any())
                errors.Add(new MissingOrderDetailsPi3Exception());

            return errors;
        }

        public void AddCustomer(string idCustomer, string customerName, ShipAddress shipAddress)
        {
            idCustomer = idCustomer ?? throw new ArgumentNullException(nameof(idCustomer));
            customerName = customerName ?? throw new ArgumentNullException(nameof(customerName));
            shipAddress = shipAddress ?? throw new ArgumentNullException(nameof(shipAddress));

            Validator.ValidateObject(shipAddress, new ValidationContext(shipAddress), true);

            this.ApplyChange(new CustomerAdded()
            {
                IdCustomer = idCustomer,
                CustomerName = customerName,
                ShipAddress = shipAddress
            });
        }

        public void ChangeCustomerShipAddress(ShipAddress newShipAddress)
        {
            newShipAddress = newShipAddress ?? throw new ArgumentNullException(nameof(newShipAddress));

            Validator.ValidateObject(newShipAddress, new ValidationContext(newShipAddress), true);

            this.ApplyChange(new CustomerShipAddressChanged()
            {
                NewShipAddress = newShipAddress
            });
        }

        public Customer Customer { get; protected set; } = null!;

        public void AddOrderDetail(int idProduct, TextValue productName, double unitPrice, int quantity = 1, int? discount = null)
        {
            productName = productName ?? throw new ArgumentNullException(nameof(productName));

            this.ApplyChange(new OrderDetailAdded()
            {
                IdOrderDetail = Guid.NewGuid(),
                IdProduct = idProduct,
                ProductName = productName,
                UnitPrice = unitPrice,
                Quantity = quantity,
                Discount = discount
            });
        }

        public void ChangeOrderDetailQuantity(Guid idOrderDetail, int newQuantity)
        {
            this.ApplyChange(new OrderDetailQuantityChanged()
            {
                IdOrderDetail = Guid.NewGuid(),
                NewQuantity = newQuantity
            });
        }

        public void ChangeOrderDetailDiscount(Guid idOrderDetail, int newDiscount)
        {
            this.ApplyChange(new OrderDetailDiscountChanged()
            {
                IdOrderDetail = Guid.NewGuid(),
                NewDiscount = newDiscount
            });
        }

        public IReadOnlyCollection<OrderDetail>? OrderDetails
        {
            get
            {
                return this._orderDetails?.AsReadOnly();
            }
        }

        public double? OrderAmount
        {
            get
            {
                return this._orderDetails?.Sum(od => od.Amount);
            }
        }

        #endregion

        #region Private Members

        protected List<OrderDetail> _orderDetails = null!;

        protected void Handle(OrderCreated @event)
        {
            this.Id = @event.Id;
            this._orderDetails = new List<OrderDetail>();
        }

        protected void Handle(CustomerAdded @event)
        {
            this.Customer = new Customer(@event.IdCustomer, @event.CustomerName, @event.ShipAddress);
        }

        protected void Handle(CustomerShipAddressChanged @event)
        {
            this.Customer?.ChangeShipAddress(@event.NewShipAddress);
        }

        protected void Handle(OrderDetailAdded @event)
        {
            this._orderDetails.Add(new OrderDetail(@event.IdOrderDetail, @event.IdProduct, @event.ProductName, @event.UnitPrice, @event.Quantity, @event.Discount));
        }

        protected void Handle(OrderDetailQuantityChanged @event)
        {
            var entity = this._orderDetails?.Find(od => od.Id == @event.IdOrderDetail);

            entity?.ChangeQuantity(@event.NewQuantity);
        }

        protected void Handle(OrderDetailDiscountChanged @event)
        {
            var entity = this._orderDetails?.Find(od => od.Id == @event.IdOrderDetail);

            entity?.ChangeDiscount(@event.NewDiscount);
        }

        #endregion
    }
}
