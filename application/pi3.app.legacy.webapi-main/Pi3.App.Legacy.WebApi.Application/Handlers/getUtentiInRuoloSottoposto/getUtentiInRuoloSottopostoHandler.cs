// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Claims;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getUtentiInRuoloSottopostoRequest = Pi3.App.Legacy.WebApi.Application.Requests.getUtentiInRuoloSottoposto;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getUtentiInRuoloSottoposto
{
    public class getUtentiInRuoloSottopostoHandler : IRequestHandler<getUtentiInRuoloSottopostoRequest, getUtentiInRuoloSottopostoResult>
    {
        #region Public Members

        public getUtentiInRuoloSottopostoHandler(ILogger<getUtentiInRuoloSottopostoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getUtentiInRuoloSottopostoResult> Handle(getUtentiInRuoloSottopostoRequest request, CancellationToken cancellationToken)
        {
            DataSet output = new DataSet();

            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var corrGlobali = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Join(this._dbContext.TipoRuoloEntities.AsNoTracking(), corr => corr.ID_TIPO_RUOLO, tipo => tipo.SYSTEM_ID, (corr, tipo) => new 
                {
                    tipo.NUM_LIVELLO, 
                    corr.ID_GRUPPO,
                    corr.SYSTEM_ID
                })
                .Where(c => c.ID_GRUPPO == idGruppo)
                .Select(c => new { c.NUM_LIVELLO, ID_CORR_GLOBALI = c.SYSTEM_ID })
                .FirstOrDefaultAsync();

            var idUOGruppo = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.ID_UO).FirstOrDefaultAsync();
            var livelloUO = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idUOGruppo).Select(c => c.NUM_LIVELLO).FirstOrDefaultAsync();

            var lowerUO = await this.GetLowerUO((long)idUOGruppo, (long)livelloUO);
            lowerUO.Add((long)idUOGruppo);

            var lowerRole = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Join(this._dbContext.TipoRuoloEntities.AsNoTracking(), corr => corr.ID_TIPO_RUOLO, tipo => tipo.SYSTEM_ID, (corr, tipo) => new
                {
                    corr.SYSTEM_ID,
                    corr.ID_GRUPPO,
                    tipo.NUM_LIVELLO,
                    corr.VAR_DESC_CORR,
                    corr.VAR_CODICE,
                    corr.VAR_COD_RUBRICA,
                    corr.ID_PARENT,
                    corr.ID_UO,
                    corr.ID_AMM
                })
                .Where(c => c.ID_AMM == idTenant && c.NUM_LIVELLO > corrGlobali.NUM_LIVELLO && lowerUO.Contains((long)c.ID_UO))
                .ToListAsync();

            var idCorrispondenteAsLong = request.corrispondente.AsLong();
            var idGruppoCorrispondente = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idCorrispondenteAsLong).Select(c => c.ID_GRUPPO).FirstOrDefaultAsync();

            var ruolo = lowerRole.Where(r => r.ID_GRUPPO == idGruppoCorrispondente).FirstOrDefault();

            if(ruolo != null || corrGlobali.ID_CORR_GLOBALI.ToString().Equals(request.corrispondente))
            {
                var peopleRole = await this._dbContext.PeopleEntities.AsNoTracking()
                    .Join(this._dbContext.PeopleGroupEntities, people => people.SYSTEM_ID, groups => groups.PEOPLE_SYSTEM_ID, (people, groups) => new
                    {
                        people.SYSTEM_ID,
                        people.FULL_NAME,
                        groups.DTA_FINE,
                        groups.GROUPS_SYSTEM_ID
                    })
                    .Where(p => p.GROUPS_SYSTEM_ID == idGruppoCorrispondente)
                    .ToListAsync();

                output = peopleRole.AsDataSet();
            }
            else
            {
                output = null;
            }

            return new getUtentiInRuoloSottopostoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getUtentiInRuoloSottopostoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected virtual async Task<List<long>> GetLowerUO(long idUO, long livelloUO)
        {
            var listLowerUO = new List<long>();

            var lowerUO = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(c => c.CHA_TIPO_URP == "U" && c.NUM_LIVELLO >= livelloUO && c.ID_PARENT == idUO)
                .Select(c => new { c.SYSTEM_ID, c.ID_PARENT, c.NUM_LIVELLO })
                .ToListAsync();

            if (lowerUO != null && lowerUO.Count > 0)
            {
                listLowerUO.AddRange(lowerUO.Select(c => c.SYSTEM_ID));
                foreach (var uo in lowerUO)
                {
                    var sottoposte = await this.GetLowerUO(uo.SYSTEM_ID, (long)uo.NUM_LIVELLO);
                    listLowerUO.AddRange(sottoposte);
                }
            }

            return listLowerUO;
        }

        #endregion
    }
}
