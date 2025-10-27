// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Events
{
    public class ContatoreRepertorioAnnullatoEvent : Event
    {
        public ContatoreRepertorioAnnullatoEvent()
        { }

        public string IdProfile { get; init; }

        public string IdField { get; init; }

        public DateTime? Data { get; init; } = null;
    }
}
