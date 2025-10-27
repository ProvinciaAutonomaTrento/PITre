// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application;

[ResourceSwaggerSchema("Link")]
public class Link
{
    [ResourceSwaggerSchema("Link_Url")]
    public string url { get; set; }
    [ResourceSwaggerSchema("Link_Rel")]
    public string rel { get; set; }
    [ResourceSwaggerSchema("Link_Type")]
    public string type { get; set; }
    [ResourceSwaggerSchema("Link_OperationId")]
    public string? operationId { get; set; }
}
