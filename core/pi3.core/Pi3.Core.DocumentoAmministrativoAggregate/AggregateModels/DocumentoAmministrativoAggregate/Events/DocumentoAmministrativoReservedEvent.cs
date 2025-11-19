// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ContentElementAggregate.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Events
{
    public class DocumentoAmministrativoReservedEvent : ContentElementReservedEvent
    {
        public DocumentoAmministrativoReservedEvent() : base()
        { }

        public string? ReserveIdGroup { get; init; }

        public string? DocumentLocation { get; init; }

        public string? MachineName { get; init; }
    }
}
