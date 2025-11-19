// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
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
using AmmGetListRuoliUORequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmGetListRuoliUO;
using AmmGetListUtentiRuoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmGetListUtentiRuolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetListRuoliUO
{
    public class AmmGetListRuoliUOHandler : IRequestHandler<AmmGetListRuoliUORequest, AmmGetListRuoliUOResult>
    {
        #region Public Members

        public AmmGetListRuoliUOHandler(ILogger<AmmGetListRuoliUOHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<AmmGetListRuoliUOResult> Handle(AmmGetListRuoliUORequest request, CancellationToken cancellationToken)
        {
            List<OrgRuolo> output = new List<OrgRuolo>();

            var idUOAsLong = request.idUO.AsLong();

            var ruoliEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Join(this._dbContext.TipoRuoloEntities, ruolo => ruolo.ID_TIPO_RUOLO, tipo => tipo.SYSTEM_ID, (ruolo, tipo) => new { ruolo, tipo })
                .Where(c => c.ruolo.CHA_TIPO_URP == "R" && c.ruolo.CHA_TIPO_IE == "I" && idUOAsLong == c.ruolo.ID_UO && c.ruolo.DTA_FINE == null)
                .OrderBy(c => c.tipo.NUM_LIVELLO)
                .ThenBy(c => c.ruolo.VAR_DESC_CORR)
                .Select(c => new
                {
                    c.ruolo.SYSTEM_ID,
                    c.ruolo.ID_GRUPPO,
                    c.ruolo.ID_TIPO_RUOLO,
                    CODICE_RUOLO = c.ruolo.VAR_CODICE,
                    CODICE_TIPO_RUOLO = c.tipo.VAR_CODICE,
                    CODICE_RUBRICA = c.ruolo.VAR_COD_RUBRICA,
                    c.ruolo.VAR_DESC_CORR,
                    c.ruolo.CHA_RIFERIMENTO,
                    c.ruolo.ID_AMM,
                    c.ruolo.CHA_RESPONSABILE,
                    c.ruolo.ID_PESO,
                    c.ruolo.CHA_SEGRETARIO,
                    c.tipo.NUM_LIVELLO,
                    c.ruolo.CHA_DISABLED_TRASM
                })
                .ToListAsync();

            foreach (var r in ruoliEntity)
            {
                output.Add(new OrgRuolo
                {
                    IDCorrGlobale = r.SYSTEM_ID.ToString(),
                    IDGruppo = r.ID_GRUPPO.ToString(),
                    IDTipoRuolo = r.ID_TIPO_RUOLO.ToString(),
                    CodiceTipoRuolo = r.CODICE_TIPO_RUOLO,
                    Codice = r.CODICE_RUOLO,
                    CodiceRubrica = r.CODICE_RUBRICA,
                    Descrizione = r.VAR_DESC_CORR,
                    DiRiferimento = r.CHA_RIFERIMENTO,
                    IDAmministrazione = r.ID_AMM.ToString(),
                    Responsabile = r.CHA_RESPONSABILE,
                    IDPeso = r.ID_PESO.ToString(),
                    Segretario = r.CHA_SEGRETARIO,
                    DisabledTrasm = r.CHA_DISABLED_TRASM,
                    Utenti = await GetUtentiRuolo((long)r.ID_GRUPPO)
                });
            }

            return new AmmGetListRuoliUOResult(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmGetListRuoliUOHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected virtual async Task<List<long>> GetLowerUO(long idUO)
        {
            var listLowerUO = new List<long>();

            var lowerUO = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(c => c.ID_PARENT == idUO)
                .Select(c => new { c.SYSTEM_ID })
                .ToListAsync();

            if (lowerUO != null && lowerUO.Count > 0)
            {
                listLowerUO.AddRange(lowerUO.Select(c => c.SYSTEM_ID));
                foreach (var uo in lowerUO)
                {
                    var sottoposte = await this.GetLowerUO(uo.SYSTEM_ID);
                    listLowerUO.AddRange(sottoposte);
                }
            }

            return listLowerUO;
        }
        protected virtual async Task<OrgUtente[]> GetUtentiRuolo(long idGruppo)
        {
            ArrayList output = new ArrayList();

            var utentiEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(c => c.DTA_FINE == null && c.CHA_TIPO_URP == "P" && c.CHA_TIPO_IE == "I")
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
                .Where(c => c.PG_ID_GRUPPO == idGruppo && c.PG_DTA_FINE == null )
                .Join(this._dbContext.PeopleEntities, corr => corr.CG_ID_PEOPLE, people => people.SYSTEM_ID, (corr, people) => new 
                {
                    corr, 
                    people 
                })
                .Where(c => c.people.DISABLED == "N")
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

            return (OrgUtente[])output.ToArray(typeof(OrgUtente));
        }
        #endregion
    }
}
