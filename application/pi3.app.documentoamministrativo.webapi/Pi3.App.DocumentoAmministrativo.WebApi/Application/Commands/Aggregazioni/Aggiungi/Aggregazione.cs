// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.SeedWork;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Aggregazioni.Aggiungi;

[ResourceSwaggerSchema("Aggregazione_Head")]
public class Aggregazione: ValueObject
{
    [ResourceSwaggerSchema("TipoAggregazione")]
    public TipiAggregazioneEnum Tipo { get; init; }
    [ResourceSwaggerSchema("IdAggrazione")]
    public string Id { get; init; }
}
