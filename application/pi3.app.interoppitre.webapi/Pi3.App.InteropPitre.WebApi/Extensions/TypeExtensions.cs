// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.InteropPitre.WebApi.Infrastructure.Services.CommonAddressBook;
using Pi3.Core.AggregateModels.CorrispondenteAggregate.ValueObjects;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Runtime.CompilerServices;

namespace Pi3.App.InteropPitre.WebApi.Extensions
{
    public static class TypeExtensions
    {
        public static IndirizzoCorrispondente ToIndirizzoCorrispondente(this Corrispondente c)
        {
            return new IndirizzoCorrispondente
            {
                Indirizzo = c.Indirizzo,
                Cap = c.CAP,
                Citta = c.Citta,
                Provincia = c.Provincia,
                Nazione = c.Nazione,
                TelefonoPrincipale = c.Telefono,
                Fax = c.Fax
            };
        }

    }
}
