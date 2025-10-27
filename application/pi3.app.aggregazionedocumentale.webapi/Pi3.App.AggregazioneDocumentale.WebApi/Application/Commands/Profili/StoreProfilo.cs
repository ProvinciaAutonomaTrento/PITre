// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Profili;

[ResourceSwaggerSchema(nameof(Documentation.StoreProfilo))]
public class StoreProfilo
{
    [ResourceSwaggerSchema(nameof(Documentation.NomeProfilo))]
    public string Nome { get; set; }
    [ResourceSwaggerSchema(nameof(Documentation.CampiProfilo))]
    public IReadOnlyList<Campo> Campi { get; set; }
}