// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities
{
    public abstract class Aggregazione : Entity<string>
    {
        public Aggregazione(string id, TextValue? denominazione = null)
        {
            Id = id;
            Denominazione = denominazione;
        }

        public virtual string Tipo
        {
            get
            {
                return GetType().Name;
            }
        }

        public TextValue? Denominazione { get; protected set; } = null;
    }

    public class Fascicolo : Aggregazione
    {
        [JsonConstructor]
        public Fascicolo(string id, TextValue? denominazione = null) : base(id, denominazione)
        {
        }
    }

    public class SerieDocumentale : Aggregazione
    {
        [JsonConstructor]
        public SerieDocumentale(string id, TextValue? denominazione = null) : base(id, denominazione)
        {
        }
    }

    public class SerieDiFascicoli : Aggregazione
    {
        [JsonConstructor]
        public SerieDiFascicoli(string id, TextValue? denominazione = null) : base(id, denominazione)
        {
        }
    }
}
