// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.amministrazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmmGetListUtentiRuoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmGetListUtentiRuolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetListUtentiRuolo
{
    public class AmmGetListUtentiRuoloHandler : IRequestHandler<AmmGetListUtentiRuoloRequest, AmmGetListUtentiRuoloResult>
    {
        #region Public Members

        public AmmGetListUtentiRuoloHandler(ILogger<AmmGetListUtentiRuoloHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<AmmGetListUtentiRuoloResult> Handle(AmmGetListUtentiRuoloRequest request, CancellationToken cancellationToken)
        {
            ArrayList output = new ArrayList();

            var idGruppo = request.idRuolo.AsLong();

            var utentiEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Join(this._dbContext.PeopleGroupEntities, corr => corr.ID_PEOPLE, groups => groups.PEOPLE_SYSTEM_ID, (corr, groups) => new
                {
                    CG_SYSTEM_ID = corr.SYSTEM_ID,
                    CG_ID_PEOPLE = corr.ID_PEOPLE,
                    CG_CODICE = corr.VAR_CODICE,
                    CG_CODICE_RUBRICA = corr.VAR_COD_RUBRICA,
                    CG_NOME = corr.VAR_NOME,
                    CG_COGNOME = corr.VAR_COGNOME,
                    CG_ID_AMM = corr.ID_AMM,
                    CG_DTA_FINE = corr.DTA_FINE,
                    CG_CHA_TIPO_URP = corr.CHA_TIPO_URP,
                    CG_CHA_TIPO_IE = corr.CHA_TIPO_IE,
                    PG_ID_GRUPPO = groups.GROUPS_SYSTEM_ID,
                    PG_DTA_FINE = groups.DTA_FINE,
                })
                .Join(this._dbContext.PeopleEntities, corr => corr.CG_ID_PEOPLE, people => people.SYSTEM_ID, (corr, people) => new
                {
                    corr,
                    people
                })
                .Where(c => c.corr.PG_ID_GRUPPO == idGruppo && c.corr.PG_DTA_FINE == null && c.corr.CG_DTA_FINE == null && c.corr.CG_CHA_TIPO_URP == "P" && c.corr.CG_CHA_TIPO_IE == "I" && c.people.DISABLED == "N")
                .Select(c => new
                {
                    c.corr.CG_SYSTEM_ID,
                    c.corr.CG_ID_PEOPLE,
                    c.corr.CG_CODICE,
                    c.corr.CG_CODICE_RUBRICA,
                    c.corr.CG_NOME,
                    c.corr.CG_COGNOME,
                    c.corr.CG_ID_AMM,
                    c.people.MATRICOLA,
                    c.people.EMAIL_ADDRESS
                })
                .ToListAsync();

            utentiEntity.ForEach(u =>
                output.Add(new OrgUtente
                {
                    IDCorrGlobale = u.CG_SYSTEM_ID.ToString(),
                    IDPeople = u.CG_ID_PEOPLE.ToString(),
                    Codice = u.CG_CODICE,
                    CodiceRubrica = u.CG_CODICE_RUBRICA,
                    Nome = u.CG_NOME,
                    Cognome = u.CG_COGNOME,
                    IDAmministrazione = u.CG_ID_AMM.ToString(),
                    Matricola = u.MATRICOLA,
                    Email = u.EMAIL_ADDRESS
                })
            );

            return new AmmGetListUtentiRuoloResult(output.Cast<OrgUtente>().ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmGetListUtentiRuoloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
