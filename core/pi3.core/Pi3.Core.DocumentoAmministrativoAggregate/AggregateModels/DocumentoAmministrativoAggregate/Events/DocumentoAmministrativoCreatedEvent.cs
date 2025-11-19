// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Events
{
    public class DocumentoAmministrativoCreatedEvent : ElementCreatedEvent
    {
        public DocumentoAmministrativoCreatedEvent()
        { }

        public OggettoDelDocumento OggettoDelDocumento { get; init; }

        public DatiRegistro? DatiRegistro { get; init; }

        public TipologiaFlussoEnum? TipologiaFlusso { get; init; }

        public TipologieVisibilitaEnum? TipologiaVisibilita { get; init; }

        public IdDoc? IdDocPrimario { get; init; }
    }
}
