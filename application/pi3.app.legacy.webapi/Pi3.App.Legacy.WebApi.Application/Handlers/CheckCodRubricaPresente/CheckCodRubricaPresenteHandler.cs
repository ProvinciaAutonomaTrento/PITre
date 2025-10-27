// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.IsCodRubricaPresente;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CheckCodRubricaPresenteRequest = Pi3.App.Legacy.WebApi.Application.Requests.CheckCodRubricaPresente;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckCodRubricaPresente
{
    public class CheckCodRubricaPresenteHandler : IRequestHandler<CheckCodRubricaPresenteRequest, CheckCodRubricaPresenteResult>
    {
        #region Public Members

        public CheckCodRubricaPresenteHandler(ILogger<CheckCodRubricaPresenteHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._dbContext = dbContext;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._logger = logger;
        }

        public async Task<CheckCodRubricaPresenteResult> Handle(CheckCodRubricaPresenteRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            var conditions = new List<Expression<Func<CorrGlobaliEntity, bool>>>
            {
                (corr) => corr.VAR_COD_RUBRICA != null ? corr.VAR_COD_RUBRICA.ToUpper().Equals(request.codRubrica.ToUpper()) : false
            };


            if (!string.IsNullOrEmpty(request.idAmm))
                conditions.Add((corr) => (corr.ID_AMM == null || corr.ID_AMM == request.idAmm.AsLong()));

            if (!string.IsNullOrEmpty(request.idReg))
                conditions.Add((corr) =>
                (corr.ID_REGISTRO != null ? corr.ID_REGISTRO == request.idReg.AsLong() : false) ||
                (corr.ID_REGISTRO == null && (corr.CHA_TIPO_IE != null ? corr.CHA_TIPO_IE.Equals("I") : false)));
            else
                conditions.Add((corr) => corr.ID_REGISTRO == null);

            conditions.Add(corr => (!corr.DTA_FINE.HasValue));

            Expression<Func<CorrGlobaliEntity, bool>> predicate = BuildAndPredicate(conditions);

            var corrRub = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(predicate).Select(
                corr => new
                {
                    corr.SYSTEM_ID,
                    corr.CHA_TIPO_IE,
                    corr.CHA_TIPO_CORR
                }).ToListAsync();

            if (corrRub != null && corrRub.Count > 0)
            {
                if (request.inRubricaComune)
                {
                    foreach (var corr in corrRub)
                    {
                        if ((corr.CHA_TIPO_IE is string ? corr.CHA_TIPO_IE.Equals("E") : false) || (corr.CHA_TIPO_CORR is string ? corr.CHA_TIPO_CORR.Equals("C") : false))
                        {
                            throw new CodiceRubricaEsistenteException();
                        }
                        else
                        {

                            string newCod = request.codRubrica + "_" + corr.SYSTEM_ID.ToString();
                            InvalidaCorr(corr.SYSTEM_ID.ToString(), newCod);

                        }
                    }
                }
                else
                {
                    foreach (var corr in corrRub)
                    {
                        if (corr.CHA_TIPO_CORR is string ? corr.CHA_TIPO_CORR.ToString().Equals("C") : false)
                        {
                            throw new CodiceRubricaComuneEsistenteException();
                        }
                        else
                        {
                            throw new CodiceRubricaEsistenteException();
                        }
                    }
                }
            }

            if (!request.inRubricaComune)
            {
                var res = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(
                    corr => (corr.CHA_TIPO_URP != null ? corr.CHA_TIPO_URP.Equals("L") : false) && (corr.ID_AMM != null ? corr.ID_AMM == request.idAmm.AsLong() : false) &&
                            (corr.VAR_COD_RUBRICA != null ? corr.VAR_COD_RUBRICA.ToUpper().Equals(request.codRubrica.ToUpper()) : false)
                    ).Select(corr => new
                    {
                        corr.SYSTEM_ID
                    }).ToListAsync();
                if (res != null && res.Count > 0)
                {
                    throw new CodiceListaEsistenteException();
                }
            }

            return new CheckCodRubricaPresenteResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CheckCodRubricaPresenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        private static Expression<Func<T, bool>> BuildAndPredicate<T>(IEnumerable<Expression<Func<T, bool>>> conditions)
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
                    predicate = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(predicate.Body, invokedExpr), predicate.Parameters);
                }
            }

            return predicate ?? (t => false);
        }

        private async Task InvalidaCorr(string corrSystemId, string cod_rubrica)
        {
            CorrGlobaliEntity? corrEntity = await this._dbContext.CorrGlobaliEntities.FirstAsync(corr => corr.SYSTEM_ID == corrSystemId.AsLong());

            if (corrEntity != null)
            {
                corrEntity.DTA_FINE = (await this._dbContext.GetSystemDateTime());
                corrEntity.VAR_COD_RUBRICA = cod_rubrica;
                corrEntity.VAR_CODICE = cod_rubrica;
                corrEntity.ID_PARENT = null;
                var rowsAffected = await ((DbContext)this._dbContext).SaveChangesAsync();

                if (rowsAffected == 0)
                {
                    throw new Exception();
                }
            }
        }

        #endregion
    }
}
