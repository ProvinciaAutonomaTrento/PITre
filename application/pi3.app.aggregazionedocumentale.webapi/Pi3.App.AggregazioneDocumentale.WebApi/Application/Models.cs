// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application
{
    [ResourceSwaggerSchema(nameof(Documentation.Link))]
    public class Link
    {
        [ResourceSwaggerSchema(nameof(Documentation.Link_Url))]
        public string url { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Link_Rel))]
        public string rel { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Link_Type))]
        public string type { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Link_OperationId))]
        public string? operationId { get; set; }
    }
}
