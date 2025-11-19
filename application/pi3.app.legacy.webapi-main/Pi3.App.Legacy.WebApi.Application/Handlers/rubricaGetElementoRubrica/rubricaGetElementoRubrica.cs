// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.rubrica;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.rubricaGetElementoRubrica
{
    // Richiede libreria MediatR
    public class rubricaGetElementoRubricaHandler : IRequestHandler<Application.Requests.rubricaGetElementoRubrica, rubricaGetElementoRubricaResult>
    {
        #region Public Members

        public rubricaGetElementoRubricaHandler(ILogger<rubricaGetElementoRubricaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<rubricaGetElementoRubricaResult> Handle(Application.Requests.rubricaGetElementoRubrica request, CancellationToken cancellationToken)
        {
            DocsPaVO.rubrica.ElementoRubrica er = new ElementoRubrica();
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            string cod = request.cod;
            DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica = request.smistamentoRubrica;
            string condRegistri = request.condRegistri;
            var predicate = PredicateBuilder.New<CorrGlobaliEntity>();

            try
            {
                if (cod.IndexOf(@"\") < 0)
                    predicate.And(x => !string.IsNullOrEmpty(x.VAR_COD_RUBRICA) && 
                        x.VAR_COD_RUBRICA.ToUpper().Equals(cod.Replace("'", "''").ToUpper()) && 
                        (x.CHA_TIPO_CORR != "C" || string.IsNullOrEmpty(x.CHA_TIPO_CORR)));
                else
                {
                    string[] flds = System.Text.RegularExpressions.Regex.Split(cod, @"\\", RegexOptions.None, TimeSpan.FromSeconds(5));
                    predicate.And(x => !string.IsNullOrEmpty(x.VAR_COD_RUBRICA) && 
                        x.VAR_COD_RUBRICA.ToUpper().Equals(flds[1].ToUpper()) && 
                        (x.CHA_TIPO_CORR != "C" || string.IsNullOrEmpty(x.CHA_TIPO_CORR)) && 
                        x.CHA_TIPO_IE.Equals(flds[0]));
                }

                predicate = predicate.And(x => x.ID_AMM == idTenant);

                var erEntity = await this._dbContext.CorrGlobaliEntities
                    .Where(predicate)
                    .Select(x => new
                    {
                        SYSTEM_ID = x.SYSTEM_ID,
                        VAR_COD_RUBRICA = x.VAR_COD_RUBRICA,
                        VAR_DESC_CORR = x.VAR_DESC_CORR,
                        INTERNO = x.CHA_TIPO_IE != null && x.CHA_TIPO_IE.Equals("I") ? 1 : 0,
                        CHA_TIPO_URP = x.CHA_TIPO_URP,
                        DTA_FINE = x.DTA_FINE,
                        CHA_DISABLED_TRASM = x.CHA_DISABLED_TRASM
                    })
                    .FirstOrDefaultAsync();

                if (erEntity != null)
                {
                    er.systemId = erEntity.SYSTEM_ID.ToString();
                    er.codice = erEntity.VAR_COD_RUBRICA ?? string.Empty;
                    er.descrizione = erEntity.VAR_DESC_CORR ?? string.Empty;
                    er.interno = erEntity.INTERNO == 1;
                    er.tipo = erEntity.CHA_TIPO_URP ?? string.Empty;
                    er.disabledTrasm = erEntity.CHA_DISABLED_TRASM != null && erEntity.CHA_DISABLED_TRASM.Equals("1");
                    er.disabled = erEntity.DTA_FINE != null;

                    er.has_children = await this.HasChildren(er.systemId, er.tipo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new rubricaGetElementoRubricaResult(er);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<rubricaGetElementoRubricaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        private async Task<bool> HasChildren(string corrId, string tipoUrp)
        {
            bool rtnUO1;
            bool rtnUO2;
            bool rtnUO3;

            long corrIdAsLong = corrId.AsLong();
            var groupsSystemIdList = await this._dbContext.CorrGlobaliEntities.Where(x => x.SYSTEM_ID == corrIdAsLong).Select(x => x.ID_GRUPPO).ToListAsync();

            rtnUO1 = await this._dbContext.CorrGlobaliEntities.Where(x => tipoUrp.Equals("U") && x.CHA_TIPO_IE.Equals("I") && x.ID_PARENT == corrIdAsLong).AnyAsync();
            rtnUO2 = await this._dbContext.CorrGlobaliEntities.Where(x => tipoUrp.Equals("U") && x.CHA_TIPO_IE.Equals("I") && x.ID_UO == corrIdAsLong).AnyAsync();
            rtnUO3 = await this._dbContext.CorrGlobaliEntities.Where(x => tipoUrp.Equals("R") &&
                x.CHA_TIPO_IE.Equals("I") &&
                this._dbContext.PeopleGroupEntities.Where(p => groupsSystemIdList.Contains(p.GROUPS_SYSTEM_ID)).Any() &&
                x.DTA_FINE == null).AnyAsync();

            return rtnUO1 || rtnUO2 || rtnUO3;
        }


        #endregion
    }

}
