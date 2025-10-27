// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.YearFilter
{
    [ResourceSwaggerSchema(nameof(Documentation.PaginazioneRicerca))]
    public class Pagination : ValueObject
    {
        [ResourceSwaggerSchema(nameof(Documentation.Pagination_Salta))]
        public int Salta { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.Pagination_Prendi))]
        public int Prendi { get; set; }
    }


    [ResourceSwaggerSchema(nameof(Documentation.YearFilterQueryResponse))]
    public class YearFilterQueryResponse : ValueObject
    {
        //public int NumeroRecord { get; set; }
        //public int RecordTotali { get; set; }
        [ResourceSwaggerSchema(nameof(Documentation.PiuDati))]
        public bool PiuDati { get; set; }
        
        [ResourceSwaggerSchema(nameof(Documentation.Aggregati))]
        public IList<AggregateResult> Aggregati { get; set; }

        [ResourceSwaggerSchema(nameof(Documentation.listaServiziApi))]
        public IEnumerable<Link> Links { get; set; }
    }

    public class YearFilterQuery: IRequest<YearFilterQueryResponse>
    {
        public string CodiceRegistro { get; set; }
        public int? Anno { get; set; }
        public Pagination? Paginazione { get; set; }  
        public int? IdTitolario { get; set; }
    }
}
