// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static DocsPaVO.Logger.CodAzione;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.DocumentoGetListaOggetti
{
    public class DocumentoGetListaOggettiHandler : IRequestHandler<Application.Requests.DocumentoGetListaOggetti, DocumentoGetListaOggettiResult>
    {
        private readonly ILogger _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IMediator _mediator;
        private readonly IPi3DbContext _dbContext;

        public DocumentoGetListaOggettiHandler(
            ILogger<DocumentoGetListaOggettiHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        private class ResultEntity
        {
            public long SYSTEM_ID { get; set; }
            public string? VAR_DESC_OGGETTO { get; set; }
            public long? ID_REGISTRO { get; set; }
            public string? VAR_COD_OGGETTO { get; set; }
            public long? ID_AMM { get; set; }
            public string? VAR_CODICE { get; set; }
        }

        private static Expression<Func<T, bool>> BuildOrPredicate<T>(IEnumerable<Expression<Func<T, bool>>> conditions)
        {
            Expression<Func<T, bool>> predicate = null;

            foreach (var condition in conditions)
            {
                if (predicate == null)
                {
                    predicate = condition;
                }
                else
                {
                    var invokedExpr = Expression.Invoke(condition, predicate.Parameters.Cast<Expression>());
                    predicate = Expression.Lambda<Func<T, bool>>(Expression.OrElse(predicate.Body, invokedExpr), predicate.Parameters);
                }
            }

            return predicate ?? (t => false);
        }


        public async Task<DocumentoGetListaOggettiResult> Handle(Application.Requests.DocumentoGetListaOggetti request, CancellationToken cancellationToken)
        {
            Oggetto[]? output = null!;
            ArrayList resultBody = new ();

            await Task.Run(() =>
            {
                IQueryable<String> g;

            var grouping = this._dbContext.OggettarioEntities
                            .Where(o => o.CHA_OCCASIONALE == "0")
                            .GroupJoin(
                                this._dbContext.RegistroEntities,
                                oggettario => oggettario.ID_REGISTRO,
                                registri => registri.SYSTEM_ID,
                                (oggettario, registri) => new { oggettario, registri }
                                )
                            .SelectMany(
                                x => x.registri.DefaultIfEmpty(),
                                (x, registro) => new ResultEntity
                                {
                                    SYSTEM_ID = x.oggettario.SYSTEM_ID,
                                    VAR_DESC_OGGETTO = x.oggettario.VAR_DESC_OGGETTO,
                                    ID_REGISTRO = x.oggettario.ID_REGISTRO,
                                    VAR_COD_OGGETTO = x.oggettario.VAR_COD_OGGETTO,
                                    ID_AMM = x.oggettario.ID_AMM,

                                    VAR_CODICE = registro != null ? registro.VAR_CODICE : null 
                                });

                int indexRegistri = 0;
                var conditions = new List<Expression<Func<ResultEntity, bool>>>();
                foreach (var idRegistro in request.queryOggetto.idRegistri) 
                {
                    DocsPaVO.utente.Registro? registro = null;
                    long localId = String.IsNullOrEmpty(idRegistro.ToString()) ? 0 : Convert.ToInt64(idRegistro);
                    if(localId > 0)
                    {
                        conditions.Add(x => x.ID_REGISTRO == localId);

                        var queryRegistro = this._dbContext.RegistroEntities
                                            .Where(w => w.SYSTEM_ID == localId)
                                            .Select(s => new
                                            {
                                                s.SYSTEM_ID,
                                                s.VAR_CODICE,
                                                s.CHA_STATO,
                                                s.ID_AMM,
                                                s.VAR_DESC_REGISTRO,
                                                s.CHA_RF
                                            });
                        
                        var tempRegistro = queryRegistro.FirstOrDefault();
                        if(tempRegistro != null)
                        {
                            registro = new DocsPaVO.utente.Registro()
                            {
                                systemId = tempRegistro.SYSTEM_ID.ToString(),
                                codRegistro = tempRegistro.VAR_CODICE ?? String.Empty,
                                stato = tempRegistro.CHA_STATO ?? String.Empty,
                                idAmministrazione = tempRegistro.ID_AMM?.ToString() ?? String.Empty,
                                chaRF = tempRegistro.CHA_RF ?? String.Empty
                            };
                        }
                    }
                    else
                    {
                        conditions.Add(x => x.ID_REGISTRO == null);
                    }

                    if(indexRegistri == (request.queryOggetto.idRegistri.Length - 1))
                    {
                        if( registro != null && !"1".Equals(registro?.chaRF))
                        {
                            conditions.Add(x => x.ID_REGISTRO == null);
                        }
                    }

                    indexRegistri++;
                }

                var orPredicate = BuildOrPredicate(conditions);

                var subGrouping = grouping.Where(orPredicate);

                if ( !String.IsNullOrWhiteSpace(request.queryOggetto.idAmministrazione) )
                {
                    subGrouping = subGrouping.Where(w => w.ID_AMM == int.Parse(request.queryOggetto.idAmministrazione));
                }

                if (!String.IsNullOrEmpty(request.queryOggetto.queryDescrizione))
                {
                    string descrizione = request.queryOggetto.queryDescrizione.ToUpper();
                    subGrouping = subGrouping.Where(w => w.VAR_DESC_OGGETTO != null && Microsoft.EntityFrameworkCore.EF.Functions.Like(w.VAR_DESC_OGGETTO.ToUpper(), $"%{descrizione}%"));
                }

                if (!String.IsNullOrWhiteSpace(request.queryOggetto.queryCodice))
                {
                    if (request.queryOggetto.queryCodice.StartsWith("$@"))
                    {
                        var queryCodice = request.queryOggetto.queryCodice.Remove(0, 2).ToUpper();
                        subGrouping = subGrouping.Where( w => w.VAR_COD_OGGETTO != null ? Microsoft.EntityFrameworkCore.EF.Functions.Like(w.VAR_COD_OGGETTO.ToUpper(), $"%{queryCodice}%") : false);
                    }
                    else
                    {
                        var queryCodice = request.queryOggetto.queryCodice.ToUpper();
                        subGrouping = subGrouping.Where(w => w.VAR_COD_OGGETTO != null ? w.VAR_COD_OGGETTO.ToUpper().Equals(queryCodice) : false);
                    }
                }

                subGrouping = subGrouping.OrderBy(o => o.ID_REGISTRO);
                subGrouping = subGrouping.OrderBy(o => o.VAR_DESC_OGGETTO);
                              
                var result = subGrouping.ToList();
                foreach( var ogg in result )
                {
                    resultBody.Add(new DocsPaVO.documento.Oggetto()
                    {
                        systemId = ogg.SYSTEM_ID.ToString(),
                        descrizione = ogg.VAR_DESC_OGGETTO ?? String.Empty,
                        codRegistro = ogg.VAR_CODICE ?? String.Empty,
                        idRegistro = ogg.ID_REGISTRO?.ToString() ?? String.Empty,
                        codOggetto = ogg.VAR_COD_OGGETTO ?? String.Empty
                    });
                }
                
                
            }, cancellationToken);

            if (resultBody != null && resultBody.Count > 0)
                output = resultBody.Cast<Oggetto>().ToArray();

            return new DocumentoGetListaOggettiResult(output ?? Array.Empty<Oggetto>());
        }
    }
}
