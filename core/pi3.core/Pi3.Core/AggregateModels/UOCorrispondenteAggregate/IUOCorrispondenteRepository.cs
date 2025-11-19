// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.CorrispondenteAggregate;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.PersonaCorrispondenteAggregate;
using Pi3.Core.AggregateModels.RuoloCorrispondenteAggregate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.UOCorrispondenteAggregate
{
    public interface IUOCorrispondenteRepository : IElementRepository<UOCorrispondente>
    {
        Task<(UOCorrispondente, bool)> Storicizza(UOCorrispondente aggregate);
    }
}
