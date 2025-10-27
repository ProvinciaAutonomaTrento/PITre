// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Documenti.AddCollegato;

[ResourceSwaggerSchema("AddDocumentoCollegato_Head")]
public class AddDocumentoCollegato
{
    [ResourceSwaggerSchema("AddDocumentoCollegato_IdDocumento")]
    public string IdDocumento { get; init; }

    [ResourceSwaggerSchema("AddDocumentoCollegato_DocumentoPadre")]
    public bool DocumentoPadre { get; init; }
}
