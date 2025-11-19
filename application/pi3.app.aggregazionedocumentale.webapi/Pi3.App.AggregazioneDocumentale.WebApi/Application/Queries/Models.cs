// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries
{
    [ResourceSwaggerSchema(nameof(Documentation.AggregateResult_Head))]
    public class AggregateResult
    {
        [ResourceSwaggerSchema(nameof(Documentation.idAggregazioneLeggere))]
        public long Id { get; set; }

        public string? Codice { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.DescrizioneAggregazione))]
        public string? Descrizione { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.idTenant))]
        public long? IdTenant { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.DataCreazione))]
        public DateTime ?DataCreazione { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.DataApertura))]
        public DateTime? DataApertura { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.DataChiusura))]
        public DateTime? DataChiusura { get; set; }
    }
}
