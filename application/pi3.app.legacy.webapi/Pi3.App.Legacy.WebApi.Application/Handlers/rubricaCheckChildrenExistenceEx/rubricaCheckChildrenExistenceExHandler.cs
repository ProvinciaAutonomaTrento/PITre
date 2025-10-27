// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rubricaCheckChildrenExistenceExRequest = Pi3.App.Legacy.WebApi.Application.Requests.rubricaCheckChildrenExistenceEx;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.rubricaCheckChildrenExistenceEx
{
    public class rubricaCheckChildrenExistenceExHandler : IRequestHandler<rubricaCheckChildrenExistenceExRequest, rubricaCheckChildrenExistenceExResult>
    {

        protected readonly ILogger<rubricaCheckChildrenExistenceExHandler> _logger;
        protected readonly IPi3DbContext _dbContext;

        public rubricaCheckChildrenExistenceExHandler(
            ILogger<rubricaCheckChildrenExistenceExHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }


        public async Task<rubricaCheckChildrenExistenceExResult> Handle(rubricaCheckChildrenExistenceExRequest request, CancellationToken cancellationToken)
        {

            try
            {
                if (request.ers == null || request.ers.Length == 0)
                    return new(request.ers);
                else
                {
                    string codeStr = string.Empty;
                    List<string> codes = new();
                    List<MetaEl> elementi = new();
                    long idAmm = !string.IsNullOrEmpty(request.u.idAmministrazione) ? request.u.idAmministrazione.AsLong() : 0;

                    foreach (var el in request.ers)
                    {
                        codeStr = el.codice.ToUpper();
                        codeStr = codeStr.Substring(0, codeStr.Length - 1);
                        codes = codeStr.IndexOf(',') != -1 ? codeStr.Split(',').ToList() : new () { codeStr };

                        if (!string.IsNullOrEmpty(codeStr))
                        {
                            var queryable = this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                .Where(c => c.VAR_COD_RUBRICA != null && codes.Contains(c.VAR_COD_RUBRICA.ToUpper()));

                            if (idAmm != 0)
                            {
                                queryable = queryable.Where(c => c.ID_AMM == idAmm);
                            }

                            var corrGlobaliEntities = await queryable
                                .Select(c => new
                                {
                                    c.ID_AMM,
                                    c.VAR_COD_RUBRICA,
                                    has_children = IPi3DbContextMappedFunctions.HasChildren(c.SYSTEM_ID, c.CHA_TIPO_URP)
                                })
                                .ToListAsync();


                            corrGlobaliEntities.ForEach(e =>
                            {
                                elementi.Add(new()
                                {
                                    IdAmm = e.ID_AMM,
                                    CodRub = e.VAR_COD_RUBRICA,
                                    hasChildren = e.has_children
                                });
                            });

                        }
                    };

                    elementi.ForEach((e =>
                    {
                        for(int i =0; i< request.ers.Length; i++)
                        {
                            if (request.ers[i].codice.Equals(e.CodRub))
                                request.ers[i].has_children = e.hasChildren != null ? e.hasChildren.AsLong() == 1 : false;
                        }
                    }));
                }
                
            }
            catch ( Exception ex )
            {
                this._logger.LogError(exception: ex , message: ex.ToString());
            }

            return new(request.ers);
        }

        private class MetaEl
        {
            public long? IdAmm { get; set; }
            public string? CodRub { get; set; }
            public string? hasChildren { get; set; }
        }
    }
}
