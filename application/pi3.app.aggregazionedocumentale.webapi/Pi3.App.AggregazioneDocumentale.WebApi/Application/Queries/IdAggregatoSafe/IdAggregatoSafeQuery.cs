// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.IdAggregatoSafe
{
    public class IdAggregatoSafeQueryResponse: ValueObject
    {
        public string Id { get; set; }
    }

    public class IdAggregatoSafeQuery : IRequest<IdAggregatoSafeQueryResponse>
    {
        public string Id { get; set; }
    }
}
