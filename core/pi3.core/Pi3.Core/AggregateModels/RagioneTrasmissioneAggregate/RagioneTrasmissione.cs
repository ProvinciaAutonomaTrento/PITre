// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore.Metadata;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.RagioneTrasmissioneAggregate
{

    public class OpzioniChanged : Event
    {
        public OpzioniChanged()
        { }

        public string IdRagioneTrasmissione { get; init; }

        public OpzioniRagioneTrasmissione NewOpzioni { get; init; }
    }

    public class RagioneTrasmissione : Element
    {
        #region Public Members

        protected RagioneTrasmissione() : base()
        { }

        public RagioneTrasmissione(string idTenant, DateTime creationDate, TextValue name, TextValue? description = null)
            : base(idTenant, "RagioneTrasmissione", creationDate, name, description)
        {
        }

        public RagioneTrasmissione(
            string id,
            string idTenant,
            DateTime creationDate,
            TextValue name,
            TextValue? description = null)
            : base(id, idTenant, "RagioneTrasmissione", creationDate, name, description)
        {
        }

        public OpzioniRagioneTrasmissione Opzioni { get; protected set; }


        public void ChangeOpzioni(OpzioniRagioneTrasmissione opzioniRagioneTramissione)
        {
            opzioniRagioneTramissione = opzioniRagioneTramissione ?? throw new ArgumentNullException(nameof(opzioniRagioneTramissione));

            Validator.ValidateObject(opzioniRagioneTramissione, new ValidationContext(opzioniRagioneTramissione), true);

            this.ApplyChange(new OpzioniChanged()
            {
                IdRagioneTrasmissione = this.Id,
                NewOpzioni = opzioniRagioneTramissione
            });
        }

        #endregion

        #region Private Members

        protected void Handle(OpzioniChanged @event)
        {
            this.Opzioni = @event.NewOpzioni;
        }

        protected override void Handle(ElementCreatedEvent @event)
        {
            base.Handle(@event);
            this.Opzioni = new OpzioniRagioneTrasmissione();
        }

        #endregion
    }

    public interface IRagioneTrasmissioneRepository: IElementRepository<RagioneTrasmissione>
    {
    }

}