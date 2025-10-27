// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ListaDistribuzioneAggregate
{
    public class Destinatario : Entity<string>
    {
      
        internal Destinatario(string id, TipiDestinatariEnum tipoDestinatario, TextValue? descrizione, bool? esterno)
        {
            Id = id;
            TipoDestinataro = tipoDestinatario;
            Descrizione = descrizione;
            Esterno = esterno;
        }

        public string Id { get; init; }
        public TipiDestinatariEnum TipoDestinataro { get; init;}
        public TextValue? Descrizione { get; init;}
        public bool? Esterno { get; init;}


    }
}
