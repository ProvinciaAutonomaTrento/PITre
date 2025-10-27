// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Pi3.App.Indexer.Application.Queries.Request;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Indexer.Application.Queries.Request
{
    public class RequestsEFQueryHandler : IRequestHandler<RequestQuery, RequestsQueryResults>
    {
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IPi3DbContext _dbContext;

        public RequestsEFQueryHandler(IClaimsPrincipalService claimsPrincipalService, IPi3DbContext dbContext)
        {
            this._claimsPrincipalService = claimsPrincipalService;
            this._dbContext = dbContext;
        }


        public async Task<RequestsQueryResults> Handle(RequestQuery request, CancellationToken cancellationToken)
        {
            var fileContent = File.ReadAllText("test.csv");
            var righe = fileContent.Split("\n");

            var ids = new List<string>();
            foreach (var riga in righe)
            {
                var campi = riga.Replace("\"", string.Empty).Split(new char[] { ',' });

                if (Int32.TryParse(campi[0], out int n))
                    ids.Add(campi[0]);
                else
                    Console.WriteLine("id: " + campi[0]);
            }

            return new RequestsQueryResults()
            {
                RequestIds = ids.AsReadOnly()
            };

            //var idAmm = Convert.ToInt64(request.IdTenant);

            //IQueryable<IndexerRequestEntity> query = this._dbContext.IndexerRequests
            //    .Where(r => r.ID_AMM == idAmm && r.REMAINING_ATTEMPTS > 0
            //            && r.ID_ELEMENT_TYPE == (int)request.ElementType);

            //if (request.ElementCreationDateFrom.HasValue && request.ElementCreationDateTo.HasValue)
            //{
            //    query = query.Where(r => r.ELEMENT_CREATION_DATE >= request.ElementCreationDateFrom
            //                    && r.ELEMENT_CREATION_DATE <= request.ElementCreationDateTo);
            //}

            //return new RequestsQueryResults()
            //{
            //    RequestIds = (await query
            //            .OrderBy(d => d.REQUEST_DATE)
            //            .Take(request.NTop)
            //            .Select(d => d.ID.ToString())
            //            .ToListAsync())
            //            .AsReadOnly()
            //};
        }
    }
}