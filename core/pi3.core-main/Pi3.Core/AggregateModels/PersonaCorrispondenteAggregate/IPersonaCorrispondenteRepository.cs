// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.RuoloCorrispondenteAggregate;
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.PersonaCorrispondenteAggregate
{
    public interface IPersonaCorrispondenteRepository : IElementRepository<PersonaCorrispondente>
    {
        Task<(PersonaCorrispondente, bool)> Storicizza(PersonaCorrispondente aggregate);

    }
}
