// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate
{
    public abstract class Aggregazione : Entity<string>
    {
        public Aggregazione(string id, TextValue? denominazione = null)
        {
            this.Id = id;
            this.Denominazione = denominazione;
        }

        public virtual string Tipo 
        { 
            get
            {
                return this.GetType().Name;
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
