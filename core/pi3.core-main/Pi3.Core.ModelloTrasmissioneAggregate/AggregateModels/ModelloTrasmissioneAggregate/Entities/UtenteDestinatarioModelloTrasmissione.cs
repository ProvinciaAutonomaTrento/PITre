// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Entities
{
    public class UtenteDestinatarioModelloTrasmissione : DestinatarioModelloTrasmissione
    {
        public UtenteDestinatarioModelloTrasmissione(string id,
            string? userId,
            string? cognome,
            string? nome,
            RagioneTrasmissione ragioneTrasmissione,
            TextValue? noteTrasmissioneSingola,
            int? giorniScadenza,
            bool? nascondiVersioniPrecedenti)
            : base(id, ragioneTrasmissione, noteTrasmissioneSingola, giorniScadenza, nascondiVersioniPrecedenti)
        {
            UserId = userId;
            Cognome = cognome;
            Nome = nome;
        }

        public string? UserId { get; protected set; }

        public string? Cognome { get; protected set; }

        public string? Nome { get; protected set; }
    }
}
