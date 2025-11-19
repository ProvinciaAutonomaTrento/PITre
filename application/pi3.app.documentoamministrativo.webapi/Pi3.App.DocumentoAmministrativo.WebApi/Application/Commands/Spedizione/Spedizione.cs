// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Spedizione
{
    [ResourceSwaggerSchema("Spedizione_Head")]
    public class Spedizione
    {
        [ResourceSwaggerSchema("Spedizione_Mezzo")]
        public string MezzoSpedizione { get; init; }
    }
}
